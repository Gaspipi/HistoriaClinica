using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HistoriaClinica.Core.Data;

public sealed class DatabaseInitializer
{
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
        }
        else
        {
            await context.Database.EnsureCreatedAsync(cancellationToken);
        }

        var migration = await DatabaseSchemaMigrator.MigrateAsync(
            databasePath,
            existedBefore,
            _logger,
            cancellationToken);

        _logger.LogInformation(
            "Base de datos lista en {DatabasePath} (esquema {SchemaVersion})",
            databasePath,
            migration.ToVersion);
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
