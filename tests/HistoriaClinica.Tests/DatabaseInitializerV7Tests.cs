using HistoriaClinica.Core.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace HistoriaClinica.Tests;

public sealed class DatabaseInitializerV7Tests : IDisposable
{
    private readonly string _tempDirectory;
    private readonly string _databasePath;

    public DatabaseInitializerV7Tests()
    {
        _tempDirectory = Path.Combine(Path.GetTempPath(), $"historia-init-v7-{Guid.NewGuid():N}");
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
    public async Task InicializaBaseNueva_CreaTablasDePlantillasYAuditoriaVacias()
    {
        var factory = new TestDbContextFactory(_databasePath);
        var initializer = new DatabaseInitializer(factory, NullLogger<DatabaseInitializer>.Instance);

        await initializer.InitializeAsync();

        await using var context = factory.CreateDbContext();
        Assert.Equal(DatabaseSchemaMigrator.CurrentVersion, await ObtenerVersionAsync());
        Assert.Equal(0, await context.Plantillas.CountAsync());
        Assert.Equal(0, await context.AuditLogs.CountAsync());

        Assert.Equal(1L, await ContarEsquemaAsync("table", "Plantillas"));
        Assert.Equal(1L, await ContarEsquemaAsync("table", "AuditLogs"));
        Assert.Equal(1L, await ContarEsquemaAsync("index", "IX_Plantillas_Nombre"));
    }

    [Fact]
    public async Task InicializarDosVeces_NoCreaBackupAdicional()
    {
        var factory = new TestDbContextFactory(_databasePath);
        var initializer = new DatabaseInitializer(factory, NullLogger<DatabaseInitializer>.Instance);

        await initializer.InitializeAsync();
        await initializer.InitializeAsync();

        Assert.Equal(DatabaseSchemaMigrator.CurrentVersion, await ObtenerVersionAsync());
        Assert.False(Directory.Exists(Path.Combine(_tempDirectory, "Backups")));
    }

    private async Task<int> ObtenerVersionAsync()
    {
        await using var connection = new SqliteConnection($"Data Source={_databasePath}");
        await connection.OpenAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = "PRAGMA user_version;";
        return Convert.ToInt32(await command.ExecuteScalarAsync());
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
}
