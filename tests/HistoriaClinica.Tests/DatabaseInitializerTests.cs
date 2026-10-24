using HistoriaClinica.Core.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace HistoriaClinica.Tests;

public sealed class DatabaseInitializerTests : IDisposable
{
    private readonly string _tempDirectory;
    private readonly string _databasePath;

    public DatabaseInitializerTests()
    {
        _tempDirectory = Path.Combine(Path.GetTempPath(), $"historia-init-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDirectory);
        _databasePath = Path.Combine(_tempDirectory, "Historia.db");
    }

    [Fact]
    public async Task InicializaUnaBaseNuevaConEsquemaVersionadoYSinBackup()
    {
        var factory = new TestDbContextFactory(_databasePath);
        var initializer = CrearInitializer(factory);

        await initializer.InitializeAsync();

        await using var context = factory.CreateDbContext();
        Assert.Equal(0, await context.Usuarios.CountAsync());
        Assert.Equal(DatabaseSchemaMigrator.CurrentVersion, await ObtenerVersionEsquemaAsync());
        Assert.False(Directory.Exists(Path.Combine(_tempDirectory, "Backups")));
    }

    [Fact]
    public async Task MigraUnaBaseHeredadaConservandoPacientesYFichasYCreandoBackup()
    {
        await CrearBaseHeredadaAsync();
        var factory = new TestDbContextFactory(_databasePath);
        var initializer = CrearInitializer(factory);

        await initializer.InitializeAsync();

        await using var context = factory.CreateDbContext();
        Assert.Equal(1, await context.Pacientes.CountAsync());
        Assert.Equal(1, await context.FichasDiarias.CountAsync());
        Assert.Equal(0, await context.Usuarios.CountAsync());
        Assert.Equal(DatabaseSchemaMigrator.CurrentVersion, await ObtenerVersionEsquemaAsync());

        var backupPath = Assert.Single(Directory.GetFiles(
            Path.Combine(_tempDirectory, "Backups"),
            "*.db"));
        Assert.Contains("pre-migracion-v0", Path.GetFileName(backupPath));

        await using var backup = new SqliteConnection(
            new SqliteConnectionStringBuilder
            {
                DataSource = backupPath,
                Mode = SqliteOpenMode.ReadOnly,
            }.ToString());
        await backup.OpenAsync();
        Assert.Equal(1L, await EjecutarScalarAsync(backup, "SELECT COUNT(*) FROM Pacientes;"));
        Assert.Equal(0L, await EjecutarScalarAsync(backup, "SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name = 'Usuarios';"));
        Assert.Equal(0L, await EjecutarScalarAsync(backup, "PRAGMA user_version;"));
    }

    [Fact]
    public async Task MigraUnaBaseDeLaVersion20QueNoTieneVersionDeEsquema()
    {
        var factory = new TestDbContextFactory(_databasePath);
        await using (var context = factory.CreateDbContext())
        {
            await context.Database.EnsureCreatedAsync();
            context.Pacientes.Add(new()
            {
                Dni = "12345678",
                FirstName = "Ana",
                LastName = "Pérez",
                DOB = new DateTime(1990, 5, 4),
            });
            await context.SaveChangesAsync();
        }

        await EjecutarSqlAsync("PRAGMA user_version = 0;");
        var initializer = CrearInitializer(factory);

        await initializer.InitializeAsync();

        await using var contextFinal = factory.CreateDbContext();
        Assert.Equal(1, await contextFinal.Pacientes.CountAsync());
        Assert.Equal(0, await contextFinal.Usuarios.CountAsync());
        Assert.Equal(DatabaseSchemaMigrator.CurrentVersion, await ObtenerVersionEsquemaAsync());
        Assert.Single(Directory.GetFiles(Path.Combine(_tempDirectory, "Backups"), "*.db"));
    }

    [Fact]
    public async Task NoVuelveAMigrarUnaBaseActualNiCreaOtroBackup()
    {
        var factory = new TestDbContextFactory(_databasePath);
        var initializer = CrearInitializer(factory);

        await initializer.InitializeAsync();

        await using (var context = factory.CreateDbContext())
        {
            context.Pacientes.Add(new()
            {
                Dni = "12345678",
                FirstName = "Ana",
                LastName = "Pérez",
                DOB = new DateTime(1990, 5, 4),
            });
            await context.SaveChangesAsync();
        }

        await initializer.InitializeAsync();

        await using var contextFinal = factory.CreateDbContext();
        Assert.Equal(1, await contextFinal.Pacientes.CountAsync());
        Assert.Equal(DatabaseSchemaMigrator.CurrentVersion, await ObtenerVersionEsquemaAsync());
        Assert.False(Directory.Exists(Path.Combine(_tempDirectory, "Backups")));
    }

    [Fact]
    public async Task RechazaUnaBaseCreadaPorUnaVersionMasNuevaSinModificarla()
    {
        var factory = new TestDbContextFactory(_databasePath);
        await using (var context = factory.CreateDbContext())
        {
            await context.Database.EnsureCreatedAsync();
        }

        await EjecutarSqlAsync("PRAGMA user_version = 99;");
        var initializer = CrearInitializer(factory);

        var exception = await Assert.ThrowsAsync<DatabaseSchemaVersionException>(
            () => initializer.InitializeAsync());

        Assert.Equal(99, exception.DatabaseVersion);
        Assert.Equal(DatabaseSchemaMigrator.CurrentVersion, exception.SupportedVersion);
        Assert.Equal(99, await ObtenerVersionEsquemaAsync());
        Assert.False(Directory.Exists(Path.Combine(_tempDirectory, "Backups")));
    }

    [Fact]
    public async Task RechazaUnArchivoSqliteDesconocidoSinCrearElEsquema()
    {
        await EjecutarSqlAsync("CREATE TABLE OtraAplicacion (Id INTEGER PRIMARY KEY);");

        var factory = new TestDbContextFactory(_databasePath);
        var initializer = CrearInitializer(factory);

        await Assert.ThrowsAsync<InvalidOperationException>(() => initializer.InitializeAsync());

        await using var connection = new SqliteConnection($"Data Source={_databasePath}");
        await connection.OpenAsync();
        Assert.Equal(1L, await EjecutarScalarAsync(connection, "SELECT COUNT(*) FROM sqlite_master WHERE type = 'table';"));
        Assert.Equal(0L, await EjecutarScalarAsync(connection, "SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name = 'Pacientes';"));
    }

    [Fact]
    public async Task SiFallaLaMigracionConservaElBackupYNoAvanzaLaVersion()
    {
        await CrearBaseHeredadaAsync();
        await EjecutarSqlAsync("""
            CREATE TABLE "Usuarios" (
              "Id" INTEGER NOT NULL CONSTRAINT "PK_Usuarios" PRIMARY KEY
            );
            """);

        var factory = new TestDbContextFactory(_databasePath);
        var initializer = CrearInitializer(factory);

        var exception = await Assert.ThrowsAsync<DatabaseMigrationException>(
            () => initializer.InitializeAsync());

        Assert.NotNull(exception.BackupPath);
        Assert.True(File.Exists(exception.BackupPath));
        Assert.Equal(0, await ObtenerVersionEsquemaAsync());

        await using var connection = new SqliteConnection($"Data Source={_databasePath}");
        await connection.OpenAsync();
        Assert.Equal(0L, await EjecutarScalarAsync(
            connection,
            "SELECT COUNT(*) FROM sqlite_master WHERE type = 'index' AND name = 'IX_Usuarios_NormalizedUsername';"));
    }

    [Fact]
    public async Task RechazaUnaVersionFuturaAntesDeCrearElEsquema()
    {
        await EjecutarSqlAsync("PRAGMA user_version = 99;");
        var factory = new TestDbContextFactory(_databasePath);
        var initializer = CrearInitializer(factory);

        await Assert.ThrowsAsync<DatabaseSchemaVersionException>(() => initializer.InitializeAsync());

        await using var connection = new SqliteConnection($"Data Source={_databasePath}");
        await connection.OpenAsync();
        Assert.Equal(99L, await EjecutarScalarAsync(connection, "PRAGMA user_version;"));
        Assert.Equal(0L, await EjecutarScalarAsync(connection, "SELECT COUNT(*) FROM sqlite_master WHERE type = 'table';"));
    }

    [Fact]
    public async Task RechazaUnaBaseExistenteSinElEsquemaRequeridoSinModificarla()
    {
        await EjecutarSqlAsync("""
            CREATE TABLE "Pacientes" (
              "Dni" TEXT NOT NULL CONSTRAINT "PK_Pacientes" PRIMARY KEY
            );
            """);

        var factory = new TestDbContextFactory(_databasePath);
        var initializer = CrearInitializer(factory);

        await Assert.ThrowsAsync<InvalidOperationException>(() => initializer.InitializeAsync());

        Assert.Equal(0, await ObtenerVersionEsquemaAsync());
        Assert.False(Directory.Exists(Path.Combine(_tempDirectory, "Backups")));
    }

    public void Dispose()
    {
        SqliteConnection.ClearAllPools();
        if (Directory.Exists(_tempDirectory))
        {
            Directory.Delete(_tempDirectory, recursive: true);
        }
    }

    private static DatabaseInitializer CrearInitializer(TestDbContextFactory factory) =>
        new(factory, NullLogger<DatabaseInitializer>.Instance);

    private async Task CrearBaseHeredadaAsync()
    {
        await EjecutarSqlAsync("""
            CREATE TABLE "Pacientes" (
              "Dni" TEXT NOT NULL CONSTRAINT "PK_Pacientes" PRIMARY KEY,
              "FirstName" TEXT NOT NULL,
              "LastName" TEXT NOT NULL,
              "DOB" TEXT NOT NULL,
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

    private async Task<int> ObtenerVersionEsquemaAsync()
    {
        await using var connection = new SqliteConnection($"Data Source={_databasePath}");
        await connection.OpenAsync();
        return Convert.ToInt32(await EjecutarScalarAsync(connection, "PRAGMA user_version;"));
    }

    private static async Task<object?> EjecutarScalarAsync(SqliteConnection connection, string sql)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = sql;
        return await command.ExecuteScalarAsync();
    }
}
