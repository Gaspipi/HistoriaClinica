using HistoriaClinica.Core.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace HistoriaClinica.Tests;

internal sealed class TestDatabase : IDisposable
{
    public TestDatabase()
    {
        DatabasePath = Path.Combine(Path.GetTempPath(), $"historia-test-{Guid.NewGuid():N}.db");
        Factory = new TestDbContextFactory(DatabasePath);

        using var context = Factory.CreateDbContext();
        context.Database.EnsureCreated();
    }

    public string DatabasePath { get; }

    public TestDbContextFactory Factory { get; }

    public void Dispose()
    {
        SqliteConnection.ClearAllPools();
        if (File.Exists(DatabasePath))
        {
            File.Delete(DatabasePath);
        }
    }
}

internal sealed class TestDbContextFactory : IDbContextFactory<HistoriaContext>
{
    private readonly DbContextOptions<HistoriaContext> _options;

    public TestDbContextFactory(string databasePath)
    {
        _options = new DbContextOptionsBuilder<HistoriaContext>()
            .UseSqlite($"Data Source={databasePath}")
            .Options;
    }

    public HistoriaContext CreateDbContext() => new(_options);

    public Task<HistoriaContext> CreateDbContextAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(CreateDbContext());
}
