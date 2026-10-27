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

public static class DatabaseSchemaMigrator
{
    public const int CurrentVersion = 1;

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

    private static readonly (int Version, string Sql)[] Migrations =
    [
        (1, MigrationToVersion1Sql),
    ];

    public static async Task<int> ReadSchemaVersionAsync(
        string databasePath,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(databasePath);

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

    public static async Task<SchemaMigrationResult> MigrateAsync(
        string databasePath,
        bool createBackup,
        ILogger logger,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(databasePath);
        ArgumentNullException.ThrowIfNull(logger);

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
