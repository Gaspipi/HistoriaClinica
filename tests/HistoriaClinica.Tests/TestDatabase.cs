using HistoriaClinica.Core.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace HistoriaClinica.Tests;

internal sealed class TestDatabase : IDisposable
{
    private readonly string _tempDirectory;

    public TestDatabase()
    {
        _tempDirectory = Path.Combine(Path.GetTempPath(), $"historia-test-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDirectory);
        DatabasePath = Path.Combine(_tempDirectory, "Historia.db");
        Factory = new TestDbContextFactory(DatabasePath);

        using var context = Factory.CreateDbContext();
        context.Database.EnsureCreated();
        context.Database.ExecuteSqlRaw(
            $"PRAGMA user_version = {DatabaseSchemaMigrator.CurrentVersion};");
    }

    public string DatabasePath { get; }

    public TestDbContextFactory Factory { get; }

    public void Dispose()
    {
        SqliteConnection.ClearAllPools();
        if (Directory.Exists(_tempDirectory))
        {
            Directory.Delete(_tempDirectory, recursive: true);
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
