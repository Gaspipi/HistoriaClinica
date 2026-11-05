using System.Globalization;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

namespace HistoriaClinica.Core.Data;

public sealed class DatabaseSchemaVersionException : InvalidOperationException
{
    public DatabaseSchemaVersionException(int databaseVersion, int supportedVersion)
        : base($"La base de datos usa el esquema {databaseVersion}, pero esta versión de Historia Clínica admite hasta la versión {supportedVersion}. Actualizá la aplicación.")
    {
        DatabaseVersion = databaseVersion;
        SupportedVersion = supportedVersion;
    }

    public int DatabaseVersion { get; }

    public int SupportedVersion { get; }
}

public sealed class DatabaseMigrationException : InvalidOperationException
{
    public DatabaseMigrationException(string message, string? backupPath, Exception innerException)
        : base(message, innerException)
    {
        BackupPath = backupPath;
    }

    public string? BackupPath { get; }
}

public sealed record SchemaMigrationResult(int FromVersion, int ToVersion, string? BackupPath);

// Migrador propio de esquema (legacy): ya no recibe cambios. Solo se usa para
// llevar bases anteriores al esquema 8 (el punto donde toma el relevo EF Core
// Migrations). Para evolucionar el modelo a partir del esquema 8, agregá una
// migración con "dotnet dotnet-ef migrations add" (manifiesto en
// .config/dotnet-tools.json).
public static class DatabaseSchemaMigrator
{
    public const int CurrentVersion = 8;

    private static readonly SemaphoreSlim _migrationLock = new(1, 1);

    private const string CreateUsuariosSql = """
        CREATE TABLE IF NOT EXISTS "Usuarios" (
          "Id" INTEGER NOT NULL CONSTRAINT "PK_Usuarios" PRIMARY KEY AUTOINCREMENT,
          "Username" TEXT NOT NULL,
          "NormalizedUsername" TEXT NOT NULL,
          "PasswordHash" TEXT NOT NULL,
          "PasswordSalt" TEXT NOT NULL,
          "PasswordIterations" INTEGER NOT NULL,
          "CreatedAt" TEXT NOT NULL
        );
        """;

    private const string CreateUsuariosIndexSql = """
        CREATE UNIQUE INDEX IF NOT EXISTS "IX_Usuarios_NormalizedUsername"
        ON "Usuarios" ("NormalizedUsername");
        """;

    private const string MigrationToVersion1Sql = $"{CreateUsuariosSql}\n{CreateUsuariosIndexSql}";

    private const string MigrationToVersion2Sql = """
        PRAGMA foreign_keys = OFF;
        CREATE TABLE "Pacientes_Nuevo" (
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
        INSERT INTO "Pacientes_Nuevo" ("Dni","FirstName","LastName","DOB","Phone","ObraSocial","NroSocio","AntecFam","AntecPers","Medicacion")
        SELECT "Dni",
               COALESCE("FirstName", ''),
               COALESCE("LastName", ''),
               CASE
                   WHEN "DOB" IS NULL OR "DOB" = '' OR "DOB" = '0001-01-01 00:00:00' OR "DOB" = '0001-01-01' THEN NULL
                   ELSE substr("DOB", 1, 10)
               END,
               COALESCE("Phone", ''),
               COALESCE("ObraSocial", ''),
               COALESCE("NroSocio", ''),
               COALESCE("AntecFam", ''),
               COALESCE("AntecPers", ''),
               COALESCE("Medicacion", '')
        FROM "Pacientes";
        DROP TABLE "Pacientes";
        ALTER TABLE "Pacientes_Nuevo" RENAME TO "Pacientes";
        PRAGMA foreign_keys = ON;
        """;

    private const string MigrationToVersion3Sql = """
        ALTER TABLE "FichasDiarias" ADD COLUMN "Cerrada" INTEGER NOT NULL DEFAULT 0;
        """;

    private const string MigrationToVersion4Sql = """
        ALTER TABLE "Pacientes" ADD COLUMN "Alergias" TEXT NOT NULL DEFAULT '';
        ALTER TABLE "Pacientes" ADD COLUMN "ProblemasActivos" TEXT NOT NULL DEFAULT '';
        """;

    private const string MigrationToVersion5Sql = """
        ALTER TABLE "FichasDiarias" ADD COLUMN "PresionArterial" TEXT NULL;
        ALTER TABLE "FichasDiarias" ADD COLUMN "FrecuenciaCardiaca" INTEGER NULL;
        ALTER TABLE "FichasDiarias" ADD COLUMN "Temperatura" REAL NULL;
        ALTER TABLE "FichasDiarias" ADD COLUMN "Peso" REAL NULL;
        """;

    private const string MigrationToVersion6Sql = """
        CREATE TABLE IF NOT EXISTS "Plantillas" (
          "Id" INTEGER NOT NULL CONSTRAINT "PK_Plantillas" PRIMARY KEY AUTOINCREMENT,
          "Nombre" TEXT NOT NULL,
          "Motivo" TEXT NOT NULL,
          "Enfermedad" TEXT NOT NULL DEFAULT '',
          "Indicaciones" TEXT NOT NULL DEFAULT '',
          "CreadorDni" TEXT NULL,
          CONSTRAINT "FK_Plantillas_Pacientes_CreadorDni"
            FOREIGN KEY ("CreadorDni") REFERENCES "Pacientes" ("Dni") ON DELETE SET NULL
        );
        CREATE UNIQUE INDEX IF NOT EXISTS "IX_Plantillas_Nombre"
        ON "Plantillas" ("Nombre");
        """;

    private const string MigrationToVersion7Sql = """
        CREATE TABLE IF NOT EXISTS "AuditLogs" (
          "Id" INTEGER NOT NULL CONSTRAINT "PK_AuditLogs" PRIMARY KEY AUTOINCREMENT,
          "Username" TEXT NOT NULL,
          "Timestamp" TEXT NOT NULL,
          "Entity" TEXT NOT NULL,
          "Action" TEXT NOT NULL,
          "Details" TEXT NULL
        );
        """;

    // Reconstruye "Pacientes" con columnas en español y sin las columnas
    // "Medicacion", "Alergias" y "ProblemasActivos". Conserva todos los datos,
    // normalizando "DOB" para no heredar valores '0001-01-01' de bases viejas.
    private const string MigrationToVersion8Sql = """
        CREATE TABLE "Pacientes_Nuevo" (
          "Dni" TEXT NOT NULL CONSTRAINT "PK_Pacientes" PRIMARY KEY,
          "Nombre" TEXT NOT NULL,
          "Apellido" TEXT NOT NULL,
          "FechaNacimiento" TEXT NULL,
          "Telefono" TEXT NOT NULL,
          "ObraSocial" TEXT NOT NULL,
          "NumeroSocio" TEXT NOT NULL,
          "AntecedentesFamiliares" TEXT NOT NULL,
          "AntecedentesPersonales" TEXT NOT NULL
        );
        INSERT INTO "Pacientes_Nuevo" ("Dni","Nombre","Apellido","FechaNacimiento","Telefono","ObraSocial","NumeroSocio","AntecedentesFamiliares","AntecedentesPersonales")
        SELECT "Dni",
               COALESCE("FirstName", ''),
               COALESCE("LastName", ''),
               CASE
                   WHEN "DOB" IS NULL OR "DOB" = '' OR "DOB" = '0001-01-01 00:00:00' OR "DOB" = '0001-01-01' THEN NULL
                   ELSE substr("DOB", 1, 10)
               END,
               COALESCE("Phone", ''),
               COALESCE("ObraSocial", ''),
               COALESCE("NroSocio", ''),
               COALESCE("AntecFam", ''),
               COALESCE("AntecPers", '')
        FROM "Pacientes";
        DROP TABLE "Pacientes";
        ALTER TABLE "Pacientes_Nuevo" RENAME TO "Pacientes";
        """;

    private static readonly (int Version, string Sql)[] Migrations =
    [
        (1, MigrationToVersion1Sql),
        (2, MigrationToVersion2Sql),
        (3, MigrationToVersion3Sql),
        (4, MigrationToVersion4Sql),
        (5, MigrationToVersion5Sql),
        (6, MigrationToVersion6Sql),
        (7, MigrationToVersion7Sql),
        (8, MigrationToVersion8Sql),
    ];

    public static async Task<int> ReadSchemaVersionAsync(
        string databasePath,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(databasePath);

        await _migrationLock.WaitAsync(cancellationToken);
        try
        {
            var fullPath = Path.GetFullPath(databasePath);
            if (!File.Exists(fullPath))
            {
                return 0;
            }

            await using var connection = new SqliteConnection(new SqliteConnectionStringBuilder
            {
                DataSource = fullPath,
                Mode = SqliteOpenMode.ReadWrite,
                Pooling = false,
            }.ToString());
            await connection.OpenAsync(cancellationToken);
            await ConfigureConnectionAsync(connection, cancellationToken);
            return await GetUserVersionAsync(connection, cancellationToken);
        }
        finally
        {
            _migrationLock.Release();
        }
    }

    public static async Task<SchemaMigrationResult> MigrateAsync(
        string databasePath,
        bool createBackup,
        ILogger logger,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(databasePath);
        ArgumentNullException.ThrowIfNull(logger);

        await _migrationLock.WaitAsync(cancellationToken);
        try
        {
        ValidateMigrations();

        var fullPath = Path.GetFullPath(databasePath);
        var connectionString = new SqliteConnectionStringBuilder
        {
            DataSource = fullPath,

            // Con el archivo presente abrimos en ReadWrite para no crear una base
            // vacía por error; ReadWriteCreate queda solo para el caso inexistente.
            Mode = File.Exists(fullPath) ? SqliteOpenMode.ReadWrite : SqliteOpenMode.ReadWriteCreate,
            Pooling = false,
        }.ToString();

        await using var connection = new SqliteConnection(connectionString);
        await connection.OpenAsync(cancellationToken);
        await ConfigureConnectionAsync(connection, cancellationToken);

        var fromVersion = await GetUserVersionAsync(connection, cancellationToken);
        if (fromVersion > CurrentVersion)
        {
            throw new DatabaseSchemaVersionException(fromVersion, CurrentVersion);
        }

        if (fromVersion == CurrentVersion)
        {
            return new SchemaMigrationResult(fromVersion, fromVersion, null);
        }

        if (fromVersion == 0)
        {
            await EnsureLegacyTablesAsync(connection, fullPath, cancellationToken);
        }

        string? backupPath = null;
        if (createBackup && File.Exists(fullPath))
        {
            backupPath = await CreateBackupAsync(fullPath, fromVersion, logger, cancellationToken);
        }

        // Desactivar claves foráneas durante la migración para poder recrear
        // tablas referenciadas (ej. Pacientes) sin cascada destructiva.
        await using (var fkCmd = connection.CreateCommand())
        {
            fkCmd.CommandText = "PRAGMA foreign_keys = OFF;";
            await fkCmd.ExecuteNonQueryAsync(cancellationToken);
        }

        var targetVersion = fromVersion;
        using var transaction = connection.BeginTransaction();
        try
        {
            foreach (var migration in Migrations)
            {
                if (migration.Version <= targetVersion)
                {
                    continue;
                }

                // Bases creadas con EnsureCreated sobre el modelo nuevo ya tienen
                // las columnas en español ("Nombre"), por lo que las migraciones
                // que modifican la estructura de "Pacientes" (2, 4 y 8) solo deben
                // actualizar la versión del esquema.
                if (migration.Version is 2 or 4 or 8
                    && await EsquemaPacientesNuevoAsync(connection, cancellationToken))
                {
                    await SetUserVersionAsync(connection, transaction, migration.Version, cancellationToken);
                    targetVersion = migration.Version;
                    continue;
                }

                // La migración 3 añade la columna "Cerrada" a "FichasDiarias".
                // En bases recién creadas con EnsureCreated la columna ya existe,
                // por lo que se omite la ejecución para evitar un error de columna duplicada.
                if (migration.Version == 3
                    && await ColumnExistsAsync(connection, "FichasDiarias", "Cerrada", cancellationToken))
                {
                    await SetUserVersionAsync(connection, transaction, migration.Version, cancellationToken);
                    targetVersion = migration.Version;
                    continue;
                }

                // La migración 4 añade las columnas "Alergias" y "ProblemasActivos"
                // a "Pacientes". En bases recién creadas con EnsureCreated ya existen,
                // por lo que se omite la ejecución para evitar columnas duplicadas.
                if (migration.Version == 4
                    && await ColumnExistsAsync(connection, "Pacientes", "Alergias", cancellationToken)
                    && await ColumnExistsAsync(connection, "Pacientes", "ProblemasActivos", cancellationToken))
                {
                    await SetUserVersionAsync(connection, transaction, migration.Version, cancellationToken);
                    targetVersion = migration.Version;
                    continue;
                }

                // La migración 5 añade los signos vitales a "FichasDiarias".
                // En bases recién creadas con EnsureCreated ya existen,
                // por lo que se omite la ejecución para evitar columnas duplicadas.
                if (migration.Version == 5
                    && await ColumnExistsAsync(connection, "FichasDiarias", "PresionArterial", cancellationToken)
                    && await ColumnExistsAsync(connection, "FichasDiarias", "FrecuenciaCardiaca", cancellationToken)
                    && await ColumnExistsAsync(connection, "FichasDiarias", "Temperatura", cancellationToken)
                    && await ColumnExistsAsync(connection, "FichasDiarias", "Peso", cancellationToken))
                {
                    await SetUserVersionAsync(connection, transaction, migration.Version, cancellationToken);
                    targetVersion = migration.Version;
                    continue;
                }
                await ExecuteAsync(connection, transaction, migration.Sql, cancellationToken);
                await SetUserVersionAsync(connection, transaction, migration.Version, cancellationToken);
                targetVersion = migration.Version;
            }

            if (targetVersion != CurrentVersion)
            {
                throw new InvalidOperationException(
                    $"No hay una migración disponible para llevar el esquema de la versión {fromVersion} a la versión {CurrentVersion}.");
            }

            transaction.Commit();

            await using (var fkCmd = connection.CreateCommand())
            {
                fkCmd.CommandText = "PRAGMA foreign_keys = ON;";
                await fkCmd.ExecuteNonQueryAsync(cancellationToken);
            }
        }
        catch (Exception exception)
        {
            try
            {
                transaction.Rollback();
            }
            catch (Exception rollbackException)
            {
                logger.LogError(
                    rollbackException,
                    "También falló el rollback de la migración. Backup: {BackupPath}",
                    backupPath ?? "no requerido");
            }

            logger.LogError(
                exception,
                "Falló la migración del esquema de la versión {FromVersion}. Backup: {BackupPath}",
                fromVersion,
                backupPath ?? "no requerido");

            throw new DatabaseMigrationException(
                backupPath is null
                    ? "No se pudo migrar la base de datos. Revisá el log para más detalles."
                    : $"No se pudo migrar la base de datos. Se conservó un backup en {backupPath}.",
                backupPath,
                exception);
        }

        logger.LogInformation(
            "Esquema de base de datos migrado de {FromVersion} a {ToVersion}. Backup: {BackupPath}",
            fromVersion,
            targetVersion,
            backupPath ?? "no requerido");

        return new SchemaMigrationResult(fromVersion, targetVersion, backupPath);
        }
        finally
        {
            _migrationLock.Release();
        }
    }

    private static void ValidateMigrations()
    {
        var expectedVersion = 1;
        foreach (var migration in Migrations)
        {
            if (migration.Version != expectedVersion)
            {
                throw new InvalidOperationException(
                    $"La migración {migration.Version} no es contigua; se esperaba la versión {expectedVersion}.");
            }

            expectedVersion++;
        }

        if (expectedVersion - 1 != CurrentVersion)
        {
            throw new InvalidOperationException(
                $"Las migraciones llegan hasta la versión {expectedVersion - 1}, pero CurrentVersion es {CurrentVersion}.");
        }
    }

    private static async Task ConfigureConnectionAsync(
        SqliteConnection connection,
        CancellationToken cancellationToken)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = "PRAGMA foreign_keys = ON; PRAGMA busy_timeout = 5000;";
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private static async Task EnsureLegacyTablesAsync(
        SqliteConnection connection,
        string databasePath,
        CancellationToken cancellationToken)
    {
        if (!await TableExistsAsync(connection, "Pacientes", cancellationToken)
            || !await TableExistsAsync(connection, "FichasDiarias", cancellationToken))
        {
            throw new InvalidOperationException(
                $"La base de datos {databasePath} no tiene el esquema esperado de Historia Clínica. No se modificó el archivo.");
        }
    }

    private static async Task<bool> TableExistsAsync(
        SqliteConnection connection,
        string tableName,
        CancellationToken cancellationToken)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT 1
            FROM sqlite_master
            WHERE type = 'table' AND name = $name
            LIMIT 1;
            """;
        command.Parameters.AddWithValue("$name", tableName);
        var result = await command.ExecuteScalarAsync(cancellationToken);
        return result is not null;
    }

    private static async Task<bool> ColumnExistsAsync(
        SqliteConnection connection,
        string tableName,
        string columnName,
        CancellationToken cancellationToken)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = $"PRAGMA table_info(\"{tableName}\");";
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            if (string.Equals(reader.GetString(1), columnName, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    // Detecta bases creadas con EnsureCreated sobre el modelo actual: si ya
    // tienen columnas en español y no queda ninguna con los nombres viejos.
    private static async Task<bool> EsquemaPacientesNuevoAsync(
        SqliteConnection connection,
        CancellationToken cancellationToken)
    {
        return !await ColumnExistsAsync(connection, "Pacientes", "FirstName", cancellationToken)
            && await ColumnExistsAsync(connection, "Pacientes", "Nombre", cancellationToken);
    }

    private static async Task<int> GetUserVersionAsync(
        SqliteConnection connection,
        CancellationToken cancellationToken)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = "PRAGMA user_version;";
        var value = await command.ExecuteScalarAsync(cancellationToken);
        return Convert.ToInt32(value, CultureInfo.InvariantCulture);
    }

    private static async Task SetUserVersionAsync(
        SqliteConnection connection,
        SqliteTransaction transaction,
        int version,
        CancellationToken cancellationToken)
    {
        await using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = $"PRAGMA user_version = {version};";
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private static async Task ExecuteAsync(
        SqliteConnection connection,
        SqliteTransaction transaction,
        string sql,
        CancellationToken cancellationToken)
    {
        await using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = sql;
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private static async Task<string> CreateBackupAsync(
        string databasePath,
        int fromVersion,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        var directory = Path.Combine(
            Path.GetDirectoryName(databasePath) ?? AppContext.BaseDirectory,
            "Backups");
        Directory.CreateDirectory(directory);

        var backupPath = GetBackupPath(directory, fromVersion);

        try
        {
            await Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                using var source = new SqliteConnection(new SqliteConnectionStringBuilder
                {
                    DataSource = databasePath,
                    Mode = SqliteOpenMode.ReadOnly,
                    Pooling = false,
                }.ToString());
                using var destination = new SqliteConnection(new SqliteConnectionStringBuilder
                {
                    DataSource = backupPath,
                    Mode = SqliteOpenMode.ReadWriteCreate,
                    Pooling = false,
                }.ToString());

                source.Open();
                destination.Open();
                source.BackupDatabase(destination);
            }, cancellationToken);

            await VerifyBackupAsync(databasePath, backupPath, cancellationToken);

            logger.LogInformation(
                "Copia de seguridad previa a la migración creada en {BackupPath}",
                backupPath);

            return backupPath;
        }
        catch
        {
            // El borrado del backup no debe enmascarar la excepción original.
            try
            {
                if (File.Exists(backupPath))
                {
                    File.Delete(backupPath);
                }
            }
            catch (Exception deleteException)
            {
                logger.LogWarning(
                    deleteException,
                    "No se pudo borrar el backup incompleto en {BackupPath}",
                    backupPath);
            }

            throw;
        }
    }

    // Verifica que el backup sea un archivo SQLite íntegro y con la misma
    // estructura que la base original; si algo no coincide, se descarta.
    private static async Task VerifyBackupAsync(
        string databasePath,
        string backupPath,
        CancellationToken cancellationToken)
    {
        int versionOrigen;
        int tablasOrigen;

        await using (var origen = new SqliteConnection(new SqliteConnectionStringBuilder
        {
            DataSource = databasePath,
            Mode = SqliteOpenMode.ReadOnly,
            Pooling = false,
        }.ToString()))
        {
            await origen.OpenAsync(cancellationToken);
            versionOrigen = await GetUserVersionAsync(origen, cancellationToken);
            tablasOrigen = await CountTablesAsync(origen, cancellationToken);
        }

        await using var copia = new SqliteConnection(new SqliteConnectionStringBuilder
        {
            DataSource = backupPath,
            Mode = SqliteOpenMode.ReadOnly,
            Pooling = false,
        }.ToString());
        await copia.OpenAsync(cancellationToken);

        var quickCheck = await ExecuteQuickCheckAsync(copia, cancellationToken);
        if (!string.Equals(quickCheck, "ok", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"El backup {backupPath} no pasó la verificación quick_check: {quickCheck}.");
        }

        var versionCopia = await GetUserVersionAsync(copia, cancellationToken);
        if (versionCopia != versionOrigen)
        {
            throw new InvalidOperationException(
                $"El backup {backupPath} tiene user_version {versionCopia}, pero la base original tiene {versionOrigen}.");
        }

        var tablasCopia = await CountTablesAsync(copia, cancellationToken);
        if (tablasCopia != tablasOrigen)
        {
            throw new InvalidOperationException(
                $"El backup {backupPath} tiene {tablasCopia} tablas, pero la base original tiene {tablasOrigen}.");
        }
    }

    private static async Task<string> ExecuteQuickCheckAsync(
        SqliteConnection connection,
        CancellationToken cancellationToken)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = "PRAGMA quick_check;";
        var result = await command.ExecuteScalarAsync(cancellationToken);
        return Convert.ToString(result, CultureInfo.InvariantCulture) ?? string.Empty;
    }

    private static async Task<int> CountTablesAsync(
        SqliteConnection connection,
        CancellationToken cancellationToken)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM sqlite_master WHERE type = 'table';";
        var result = await command.ExecuteScalarAsync(cancellationToken);
        return Convert.ToInt32(result, CultureInfo.InvariantCulture);
    }

    private static string GetBackupPath(string directory, int fromVersion)
    {
        var timestamp = DateTime.Now.ToString("yyyyMMdd-HHmmss", CultureInfo.InvariantCulture);
        var baseName = $"Historia-pre-migracion-v{fromVersion}-{timestamp}";
        var path = Path.Combine(directory, $"{baseName}.db");

        var index = 1;
        while (File.Exists(path))
        {
            path = Path.Combine(directory, $"{baseName}-{index}.db");
            index++;
        }

        return path;
    }
}
