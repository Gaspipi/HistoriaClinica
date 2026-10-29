using HistoriaClinica.Core.Data;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging.Abstractions;

namespace HistoriaClinica.Tests;

public sealed class DatabaseSchemaMigratorTests : IDisposable
{
    private readonly string _tempDirectory;
    private readonly string _databasePath;

    public DatabaseSchemaMigratorTests()
    {
        _tempDirectory = Path.Combine(Path.GetTempPath(), $"historia-mig-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDirectory);
        _databasePath = Path.Combine(_tempDirectory, "Historia.db");
    }

    public void Dispose()
    {
        SqliteConnection.ClearAllPools();
        if (Directory.Exists(_tempDirectory))
        {
            Directory.Delete(_tempDirectory, recursive: true);
        }
    }

    private async Task CrearBaseHeredadaAsync()
    {
        await EjecutarSqlAsync("""
            CREATE TABLE "Pacientes" (
              "Dni" TEXT NOT NULL CONSTRAINT "PK_Pacientes" PRIMARY KEY,
              "FirstName" TEXT NOT NULL,
              "LastName" TEXT NOT NULL,
              "DOB" TEXT NULL,
              "Phone" TEXT NOT NULL,
              "ObraSocial" TEXT NOT NULL,
              "NroSocio" TEXT NOT NULL,
              "AntecFam" TEXT NOT NULL,
              "AntecPers" TEXT NOT NULL,
              "Medicacion" TEXT NOT NULL
            );

            CREATE TABLE "FichasDiarias" (
              "Id" INTEGER NOT NULL CONSTRAINT "PK_FichasDiarias" PRIMARY KEY AUTOINCREMENT,
              "Dni" TEXT NOT NULL,
              "Enfermedad" TEXT NOT NULL,
              "Motivo" TEXT NOT NULL,
              "Indicaciones" TEXT NOT NULL,
              "Fecha" TEXT NOT NULL,
              CONSTRAINT "FK_FichasDiarias_Pacientes_Dni"
                FOREIGN KEY ("Dni") REFERENCES "Pacientes" ("Dni") ON DELETE CASCADE
            );

            INSERT INTO "Pacientes" (
              "Dni", "FirstName", "LastName", "DOB", "Phone",
              "ObraSocial", "NroSocio", "AntecFam", "AntecPers", "Medicacion"
            ) VALUES (
              '12345678', 'Ana', 'Pérez', '1990-05-04 00:00:00',
              '555-0000', 'OSDE', '123', '', '', ''
            );

            INSERT INTO "FichasDiarias" (
              "Dni", "Enfermedad", "Motivo", "Indicaciones", "Fecha"
            ) VALUES (
              '12345678', 'Control', 'Consulta', '', '2026-10-24 10:00:00'
            );
            """);
    }

    private async Task EjecutarSqlAsync(string sql)
    {
        await using var connection = new SqliteConnection($"Data Source={_databasePath}");
        await connection.OpenAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = sql;
        await command.ExecuteNonQueryAsync();
    }

    private async Task<int> ObtenerVersionAsync()
    {
        await using var connection = new SqliteConnection($"Data Source={_databasePath}");
        await connection.OpenAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = "PRAGMA user_version;";
        var value = await command.ExecuteScalarAsync();
        return Convert.ToInt32(value);
    }

    [Fact]
    public async Task MigrateAsync_CuandoDosHilosCorrenSimultaneamente_SeMigraCorrectamente()
    {
        await CrearBaseHeredadaAsync();
        var logger = NullLogger.Instance;
        var semaphore = new SemaphoreSlim(2);

        var tasks = Enumerable.Range(0, 2).Select(async _ =>
        {
            await semaphore.WaitAsync();
            try
            {
                await DatabaseSchemaMigrator.MigrateAsync(_databasePath, false, logger);
            }
            finally
            {
                semaphore.Release();
            }
        }).ToArray();

        await Task.WhenAll(tasks);

        Assert.Equal(DatabaseSchemaMigrator.CurrentVersion, await ObtenerVersionAsync());

        await using var connection = new SqliteConnection($"Data Source={_databasePath}");
        await connection.OpenAsync();
        Assert.Equal(1L, await ScalarAsync(connection, "SELECT COUNT(*) FROM Pacientes;"));
        Assert.Equal(1L, await ScalarAsync(connection, "SELECT COUNT(*) FROM FichasDiarias;"));
    }

    [Fact]
    public async Task MigrateAsync_CuandoUnaBaseYaEstaMigrada_NoTocaDatos()
    {
        await CrearBaseHeredadaAsync();
        var logger = NullLogger.Instance;

        await DatabaseSchemaMigrator.MigrateAsync(_databasePath, false, logger);
        await DatabaseSchemaMigrator.MigrateAsync(_databasePath, false, logger);

        Assert.Equal(DatabaseSchemaMigrator.CurrentVersion, await ObtenerVersionAsync());
    }

    [Fact]
    public async Task ReadSchemaVersionAsync_Concurrencia_leeValorCorrecto()
    {
        await CrearBaseHeredadaAsync();
        var logger = NullLogger.Instance;
        await DatabaseSchemaMigrator.MigrateAsync(_databasePath, false, logger);

        var semaphore = new SemaphoreSlim(5);
        var resultados = new int[10];

        var tasks = Enumerable.Range(0, 10).Select(async i =>
        {
            await semaphore.WaitAsync();
            try
            {
                resultados[i] = await DatabaseSchemaMigrator.ReadSchemaVersionAsync(_databasePath);
            }
            finally
            {
                semaphore.Release();
            }
        }).ToArray();

        await Task.WhenAll(tasks);

        Assert.All(resultados, r => Assert.Equal(DatabaseSchemaMigrator.CurrentVersion, r));
    }

    private static async Task<object?> ScalarAsync(SqliteConnection connection, string sql)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = sql;
        return await command.ExecuteScalarAsync();
    }
}
