using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data;

namespace HistoriaClinica.Core.Data;

public sealed class DatabaseInitializer
{
    // Migración EF que refleja el esquema 8 (el último generado por el
    // migrador propio). Las bases heredadas que llegan a ese esquema marcan
    // la migración como ya aplicada en el historial de EF para que no
    // intente recrear las tablas.
    private const string MigracionInicialId = "20261105045533_InitialCreate";

    private readonly IDbContextFactory<HistoriaContext> _contextFactory;
    private readonly ILogger<DatabaseInitializer> _logger;

    public DatabaseInitializer(
        IDbContextFactory<HistoriaContext> contextFactory,
        ILogger<DatabaseInitializer> logger)
    {
        _contextFactory = contextFactory;
        _logger = logger;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        var databasePath = ResolveDatabasePath(context);
        var existedBefore = File.Exists(databasePath)
            && new FileInfo(databasePath).Length > 0;

        if (existedBefore)
        {
            var existingVersion = await DatabaseSchemaMigrator.ReadSchemaVersionAsync(
                databasePath,
                cancellationToken);

            if (existingVersion > DatabaseSchemaMigrator.CurrentVersion)
            {
                throw new DatabaseSchemaVersionException(
                    existingVersion,
                    DatabaseSchemaMigrator.CurrentVersion);
            }

            // Bases anteriores al esquema 8: el migrador propio las lleva al
            // esquema 8 una última vez (con backup). De ahí en adelante el
            // esquema evoluciona con las migraciones de EF Core.
            if (existingVersion < DatabaseSchemaMigrator.CurrentVersion)
            {
                await DatabaseSchemaMigrator.MigrateAsync(
                    databasePath,
                    createBackup: true,
                    _logger,
                    cancellationToken);
            }
        }

        // Si es una base nueva (sin tablas), aplicar migraciones directamente.
        // Si es una base existente con tablas pero sin historial EF, insertar baseline.
        var debeBasar = await DebeInsertarBaselineAsync(context, cancellationToken);
        if (debeBasar)
        {
            await InsertarBaselineAsync(context, cancellationToken);
        }

        await context.Database.MigrateAsync(cancellationToken);

        // EF Migrations gestiona su propio historial (__EFMigrationsHistory);
        // user_version se mantiene igualado al esquema 8 porque es el marcador
        // que usa el migrador heredado para detectar bases más nuevas.
        await IgualarUserVersionSiMenorAsync(context, cancellationToken);

        _logger.LogInformation(
            "Base de datos lista en {DatabasePath} (migraciones EF: {MigracionesAplicadas})",
            databasePath,
            (await context.Database.GetAppliedMigrationsAsync(cancellationToken)).ToList().Count);
    }

    private static async Task<bool> DebeInsertarBaselineAsync(
        HistoriaContext context,
        CancellationToken cancellationToken)
    {
        var connection = (SqliteConnection)context.Database.GetDbConnection();
        var habiaQueAbrir = connection.State != ConnectionState.Open;
        if (habiaQueAbrir)
        {
            await connection.OpenAsync(cancellationToken);
        }

        try
        {
            if (await ExisteHistorialEFAsync(connection, cancellationToken))
            {
                return false;
            }

            if (!await TieneTablasExistentesAsync(connection, cancellationToken))
            {
                return false;
            }

            var version = await LeerUserVersionAsync(connection, cancellationToken);
            if (version < DatabaseSchemaMigrator.CurrentVersion)
            {
                return false;
            }

            return true;
        }
        finally
        {
            if (habiaQueAbrir)
            {
                await connection.CloseAsync();
            }
        }
    }

    private static async Task InsertarBaselineAsync(
        HistoriaContext context,
        CancellationToken cancellationToken)
    {
        var connection = (SqliteConnection)context.Database.GetDbConnection();
        var habiaQueAbrir = connection.State != ConnectionState.Open;
        if (habiaQueAbrir)
        {
            await connection.OpenAsync(cancellationToken);
        }

        try
        {
            await using var command = connection.CreateCommand();
            command.CommandText = $"""
                CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
                    "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
                    "ProductVersion" TEXT NOT NULL
                );
                INSERT OR IGNORE INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
                VALUES ('{MigracionInicialId}', '10.0.13');
                """;
            await command.ExecuteNonQueryAsync(cancellationToken);
        }
        finally
        {
            if (habiaQueAbrir)
            {
                await connection.CloseAsync();
            }
        }
    }

    private static async Task<bool> TieneTablasExistentesAsync(
        SqliteConnection connection,
        CancellationToken cancellationToken)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT 1
            FROM sqlite_master
            WHERE type = 'table' AND (name = 'Pacientes' OR name = 'Usuarios')
            LIMIT 1;
            """;
        var result = await command.ExecuteScalarAsync(cancellationToken);
        return result is not null;
    }

    private static async Task<bool> ExisteHistorialEFAsync(
        SqliteConnection connection,
        CancellationToken cancellationToken)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT 1
            FROM sqlite_master
            WHERE type = 'table' AND name = '__EFMigrationsHistory'
            LIMIT 1;
            """;
        var result = await command.ExecuteScalarAsync(cancellationToken);
        return result is not null;
    }

    private static async Task<int> LeerUserVersionAsync(
        SqliteConnection connection,
        CancellationToken cancellationToken)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = "PRAGMA user_version;";
        var value = await command.ExecuteScalarAsync(cancellationToken);
        return Convert.ToInt32(value);
    }

    private static async Task IgualarUserVersionSiMenorAsync(
        HistoriaContext context,
        CancellationToken cancellationToken)
    {
        var connection = (SqliteConnection)context.Database.GetDbConnection();
        var habiaQueAbrir = connection.State != ConnectionState.Open;
        if (habiaQueAbrir)
        {
            await connection.OpenAsync(cancellationToken);
        }

        try
        {
            var version = await LeerUserVersionAsync(connection, cancellationToken);
            if (version >= DatabaseSchemaMigrator.CurrentVersion)
            {
                return;
            }

            await using var command = connection.CreateCommand();
            command.CommandText = $"PRAGMA user_version = {DatabaseSchemaMigrator.CurrentVersion};";
            await command.ExecuteNonQueryAsync(cancellationToken);
        }
        finally
        {
            if (habiaQueAbrir)
            {
                await connection.CloseAsync();
            }
        }
    }

    private static string ResolveDatabasePath(HistoriaContext context)
    {
        var dataSource = context.Database.GetDbConnection().DataSource;
        if (string.IsNullOrWhiteSpace(dataSource)
            || dataSource.Equals(":memory:", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "La base de datos de Historia Clínica debe usar un archivo SQLite.");
        }

        return Path.GetFullPath(dataSource);
    }
}