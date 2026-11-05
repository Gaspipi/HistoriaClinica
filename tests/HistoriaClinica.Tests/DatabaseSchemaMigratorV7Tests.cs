using HistoriaClinica.Core.Data;
using HistoriaClinica.Core.Models;
using HistoriaClinica.Core.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging.Abstractions;

namespace HistoriaClinica.Tests;

public sealed class DatabaseSchemaMigratorV7Tests : IDisposable
{
    private readonly string _tempDirectory;
    private readonly string _databasePath;

    public DatabaseSchemaMigratorV7Tests()
    {
        _tempDirectory = Path.Combine(Path.GetTempPath(), $"historia-mig-v7-{Guid.NewGuid():N}");
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

    [Fact]
    public async Task MigrateAsync_BaseV2_MigraHastaVersionActualYCreaEstructuras()
    {
        await CrearBaseHeredadaAsync(version: 2);

        var resultado = await DatabaseSchemaMigrator.MigrateAsync(
            _databasePath,
            createBackup: false,
            NullLogger.Instance);

        Assert.Equal(2, resultado.FromVersion);
        Assert.Equal(DatabaseSchemaMigrator.CurrentVersion, resultado.ToVersion);
        Assert.Null(resultado.BackupPath);
        Assert.Equal(DatabaseSchemaMigrator.CurrentVersion, await ObtenerVersionAsync(_databasePath));

        var columnasPacientes = await ObtenerColumnasAsync("Pacientes");
        Assert.Contains("Nombre", columnasPacientes);
        Assert.Contains("Apellido", columnasPacientes);
        Assert.Contains("FechaNacimiento", columnasPacientes);
        Assert.Contains("Telefono", columnasPacientes);
        Assert.Contains("NumeroSocio", columnasPacientes);
        Assert.Contains("AntecedentesFamiliares", columnasPacientes);
        Assert.Contains("AntecedentesPersonales", columnasPacientes);
        Assert.DoesNotContain("FirstName", columnasPacientes);
        Assert.DoesNotContain("DOB", columnasPacientes);
        Assert.DoesNotContain("Medicacion", columnasPacientes);
        Assert.DoesNotContain("Alergias", columnasPacientes);
        Assert.DoesNotContain("ProblemasActivos", columnasPacientes);

        var columnasFichas = await ObtenerColumnasAsync("FichasDiarias");
        Assert.Contains("Cerrada", columnasFichas);
        Assert.Contains("PresionArterial", columnasFichas);
        Assert.Contains("FrecuenciaCardiaca", columnasFichas);
        Assert.Contains("Temperatura", columnasFichas);
        Assert.Contains("Peso", columnasFichas);

        Assert.Equal(1L, await ContarEsquemaAsync("table", "Plantillas"));
        Assert.Equal(1L, await ContarEsquemaAsync("table", "AuditLogs"));
        Assert.Equal(1L, await ContarEsquemaAsync("index", "IX_Plantillas_Nombre"));
    }

    [Fact]
    public async Task MigrateAsync_BaseV2_ConservaFilasYCamposNuevos()
    {
        await CrearBaseHeredadaAsync(version: 2);

        await DatabaseSchemaMigrator.MigrateAsync(_databasePath, createBackup: false, NullLogger.Instance);

        Assert.Equal(2L, await ContarFilasAsync("Pacientes"));
        Assert.Equal(2L, await ContarFilasAsync("FichasDiarias"));
        Assert.Equal(0L, await ScalarAsync(
            "SELECT COUNT(*) FROM Pacientes WHERE AntecedentesFamiliares <> '' OR AntecedentesPersonales <> '';"));
        Assert.Equal(0L, await ScalarAsync("SELECT COUNT(*) FROM FichasDiarias WHERE Cerrada <> 0;"));

        // La migración 8 reconstruye "Pacientes" normalizando la fecha, por lo
        // que el valor heredado '0001-01-01' termina como NULL (sin fecha).
        Assert.Null(await ScalarAsync("SELECT FechaNacimiento FROM Pacientes WHERE Dni = '12345678';"));
    }

    [Fact]
    public async Task MigrateAsync_BaseV1_NormalizaDobYConservaFilas()
    {
        await CrearBaseHeredadaAsync(version: 1);

        await DatabaseSchemaMigrator.MigrateAsync(_databasePath, createBackup: false, NullLogger.Instance);

        Assert.Equal(DatabaseSchemaMigrator.CurrentVersion, await ObtenerVersionAsync(_databasePath));
        Assert.Equal(2L, await ContarFilasAsync("Pacientes"));
        Assert.Equal(2L, await ContarFilasAsync("FichasDiarias"));
        Assert.Null(await ScalarAsync("SELECT FechaNacimiento FROM Pacientes WHERE Dni = '12345678';"));
        Assert.Equal(
            "1990-05-04",
            await ScalarAsync("SELECT FechaNacimiento FROM Pacientes WHERE Dni = '23456789';"));
        Assert.Equal(
            "Consulta",
            await ScalarAsync("SELECT Motivo FROM FichasDiarias WHERE Dni = '12345678';"));
    }

    [Fact]
    public async Task MigrateAsync_BaseEnsureCreatedEnVersionCero_NoFallaPorColumnasDuplicadas()
    {
        var factory = new TestDbContextFactory(_databasePath);
        await using (var context = factory.CreateDbContext())
        {
            await context.Database.EnsureCreatedAsync();
            context.Pacientes.Add(new Paciente
            {
                Dni = "12345678",
                Nombre = "Ana",
                Apellido = "Pérez",
                FechaNacimiento = new DateOnly(1990, 5, 4),
            });
            await context.SaveChangesAsync();
        }

        await EjecutarSqlAsync("PRAGMA user_version = 0;");

        var resultado = await DatabaseSchemaMigrator.MigrateAsync(
            _databasePath,
            createBackup: false,
            NullLogger.Instance);

        Assert.Equal(0, resultado.FromVersion);
        Assert.Equal(DatabaseSchemaMigrator.CurrentVersion, resultado.ToVersion);
        Assert.Equal(DatabaseSchemaMigrator.CurrentVersion, await ObtenerVersionAsync(_databasePath));
        Assert.Equal(1L, await ScalarAsync("SELECT COUNT(*) FROM Pacientes WHERE Dni = '12345678';"));
        Assert.Equal(1L, await ContarEsquemaAsync("table", "AuditLogs"));
    }

    [Fact]
    public async Task MigrateAsync_VersionFutura_LanzaYNoModificaNiCreaBackup()
    {
        await CrearBaseHeredadaAsync(version: 99);
        var contenidoAntes = await File.ReadAllBytesAsync(_databasePath);

        var exception = await Assert.ThrowsAsync<DatabaseSchemaVersionException>(
            () => DatabaseSchemaMigrator.MigrateAsync(_databasePath, createBackup: true, NullLogger.Instance));

        Assert.Equal(99, exception.DatabaseVersion);
        Assert.Equal(DatabaseSchemaMigrator.CurrentVersion, exception.SupportedVersion);
        Assert.Equal(99, await ObtenerVersionAsync(_databasePath));
        Assert.False(Directory.Exists(Path.Combine(_tempDirectory, "Backups")));

        var contenidoDespues = await File.ReadAllBytesAsync(_databasePath);
        Assert.True(contenidoAntes.SequenceEqual(contenidoDespues));
    }

    [Fact]
    public async Task MigrateAsync_ConBackup_CreaCopiaConVersionDeOrigen()
    {
        await CrearBaseHeredadaAsync(version: 2);

        var resultado = await DatabaseSchemaMigrator.MigrateAsync(
            _databasePath,
            createBackup: true,
            NullLogger.Instance);

        Assert.NotNull(resultado.BackupPath);
        Assert.True(File.Exists(resultado.BackupPath));
        Assert.Equal(2, await ObtenerVersionAsync(resultado.BackupPath));
        Assert.Equal("ok", await ScalarAsync("PRAGMA quick_check;", resultado.BackupPath));
        Assert.Equal(DatabaseSchemaMigrator.CurrentVersion, await ObtenerVersionAsync(_databasePath));
    }

    [Fact]
    public async Task MigrateAsync_VersionActual_NoCreaBackup()
    {
        var factory = new TestDbContextFactory(_databasePath);
        await using (var context = factory.CreateDbContext())
        {
            await context.Database.EnsureCreatedAsync();
        }

        await EjecutarSqlAsync($"PRAGMA user_version = {DatabaseSchemaMigrator.CurrentVersion};");

        var resultado = await DatabaseSchemaMigrator.MigrateAsync(
            _databasePath,
            createBackup: true,
            NullLogger.Instance);

        Assert.Equal(
            new SchemaMigrationResult(
                DatabaseSchemaMigrator.CurrentVersion,
                DatabaseSchemaMigrator.CurrentVersion,
                null),
            resultado);
        Assert.False(Directory.Exists(Path.Combine(_tempDirectory, "Backups")));
    }

    [Fact]
    public async Task ReadSchemaVersionAsync_ArchivoInexistente_DevuelveCeroSinCrearArchivo()
    {
        var inexistente = Path.Combine(_tempDirectory, "SinCrear.db");

        var version = await DatabaseSchemaMigrator.ReadSchemaVersionAsync(inexistente);

        Assert.Equal(0, version);
        Assert.False(File.Exists(inexistente));
    }

    [Fact]
    public async Task MigrateAsync_BaseMigrada_EsUsablePorRepositorios()
    {
        await CrearBaseHeredadaAsync(version: 1);
        await DatabaseSchemaMigrator.MigrateAsync(_databasePath, createBackup: false, NullLogger.Instance);

        var factory = new TestDbContextFactory(_databasePath);
        var pacientes = new PacienteRepository(factory);
        var fichas = new FichaRepository(factory);
        var plantillas = new PlantillaRepository(factory);
        var auditoria = new AuditLogRepository(factory);

        var paciente = await pacientes.GetByDniAsync("12345678");
        Assert.NotNull(paciente);
        Assert.Equal(string.Empty, paciente.AntecedentesFamiliares);
        paciente.AntecedentesFamiliares = "Diabetes familiar";
        await pacientes.SaveAsync(paciente);
        var actualizado = await pacientes.GetByDniAsync("12345678");
        Assert.NotNull(actualizado);
        Assert.Equal("Diabetes familiar", actualizado.AntecedentesFamiliares);

        var ficha = Assert.Single(await fichas.GetByDniAsync("12345678"));
        Assert.Equal("Consulta", ficha.Motivo);
        Assert.False(ficha.Cerrada);

        await plantillas.SaveAsync(new Plantilla { Nombre = "Control", Motivo = "Motivo" });
        Assert.Single(await plantillas.GetAllAsync());

        await auditoria.SaveAsync(new AuditLog
        {
            Username = "ana",
            Timestamp = DateTime.Now,
            Entity = "Pacientes",
            Action = "Update",
        });
        Assert.Single(await auditoria.GetAllAsync());
    }

    private async Task CrearBaseHeredadaAsync(int version)
    {
        await EjecutarSqlAsync($"""
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
              '12345678', 'Ana', 'Pérez', '0001-01-01 00:00:00',
              '555-0000', 'OSDE', '123', '', '', ''
            );

            INSERT INTO "Pacientes" (
              "Dni", "FirstName", "LastName", "DOB", "Phone",
              "ObraSocial", "NroSocio", "AntecFam", "AntecPers", "Medicacion"
            ) VALUES (
              '23456789', 'Beto', 'Gómez', '1990-05-04 00:00:00',
              '555-1111', 'IOMA', '456', '', '', ''
            );

            INSERT INTO "FichasDiarias" (
              "Dni", "Enfermedad", "Motivo", "Indicaciones", "Fecha"
            ) VALUES (
              '12345678', 'Control', 'Consulta', '', '2026-10-24 10:00:00'
            );

            INSERT INTO "FichasDiarias" (
              "Dni", "Enfermedad", "Motivo", "Indicaciones", "Fecha"
            ) VALUES (
              '23456789', 'Control', 'Consulta', '', '2026-10-25 11:00:00'
            );

            PRAGMA user_version = {version};
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

    private async Task<int> ObtenerVersionAsync(string databasePath)
    {
        return Convert.ToInt32(await ScalarAsync("PRAGMA user_version;", databasePath));
    }

    private async Task<long> ContarFilasAsync(string tabla) =>
        Convert.ToInt64(await ScalarAsync($"SELECT COUNT(*) FROM \"{tabla}\";"));

    private async Task<List<string>> ObtenerColumnasAsync(string tabla)
    {
        await using var connection = new SqliteConnection($"Data Source={_databasePath}");
        await connection.OpenAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = $"PRAGMA table_info(\"{tabla}\");";
        await using var reader = await command.ExecuteReaderAsync();
        var columnas = new List<string>();
        while (await reader.ReadAsync())
        {
            columnas.Add(reader.GetString(1));
        }

        return columnas;
    }

    private async Task<long> ContarEsquemaAsync(string tipo, string nombre)
    {
        await using var connection = new SqliteConnection($"Data Source={_databasePath}");
        await connection.OpenAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM sqlite_master WHERE type = $tipo AND name = $nombre;";
        command.Parameters.AddWithValue("$tipo", tipo);
        command.Parameters.AddWithValue("$nombre", nombre);
        return Convert.ToInt64(await command.ExecuteScalarAsync());
    }

    private async Task<object?> ScalarAsync(string sql, string? databasePath = null)
    {
        await using var connection = new SqliteConnection(
            new SqliteConnectionStringBuilder
            {
                DataSource = databasePath ?? _databasePath,
                Mode = SqliteOpenMode.ReadWrite,
                Pooling = false,
            }.ToString());
        await connection.OpenAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = sql;
        var resultado = await command.ExecuteScalarAsync();
        return resultado is DBNull ? null : resultado;
    }
}
