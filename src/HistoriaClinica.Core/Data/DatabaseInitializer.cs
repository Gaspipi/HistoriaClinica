using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HistoriaClinica.Core.Data;

public sealed class DatabaseInitializer
{
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
        await context.Database.EnsureCreatedAsync(cancellationToken);
        await EnsureAuthSchemaAsync(context, cancellationToken);
        _logger.LogInformation("Base de datos lista en {DatabasePath}", context.Database.GetDbConnection().DataSource);
    }

    private static async Task EnsureAuthSchemaAsync(
        HistoriaContext context,
        CancellationToken cancellationToken)
    {
        await context.Database.ExecuteSqlRawAsync(CreateUsuariosSql, cancellationToken);
        await context.Database.ExecuteSqlRawAsync(CreateUsuariosIndexSql, cancellationToken);
    }
}
