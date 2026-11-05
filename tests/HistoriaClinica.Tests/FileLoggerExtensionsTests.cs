using HistoriaClinica.Core.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using FileLoggerProviderCore = HistoriaClinica.Core.Logging.FileLoggerProvider;

namespace HistoriaClinica.Tests;

public sealed class FileLoggerExtensionsTests : IDisposable
{
    private readonly string _tempDirectory;

    public FileLoggerExtensionsTests()
    {
        _tempDirectory = Path.Combine(Path.GetTempPath(), $"historia-log-ext-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDirectory);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDirectory))
        {
            Directory.Delete(_tempDirectory, recursive: true);
        }
    }

    [Fact]
    public async Task AddFileLogger_ConRuta_RegistraYEscribe()
    {
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddFileLogger(_tempDirectory));

        using (var provider = services.BuildServiceProvider())
        {
            var logger = provider.GetRequiredService<ILoggerFactory>().CreateLogger("Pruebas");
            logger.LogInformation("mensaje desde DI");
        }

        var archivo = Path.Combine(
            _tempDirectory,
            "Logs",
            $"historia-{DateTime.Now:yyyy-MM-dd}.log");

        Assert.True(File.Exists(archivo));
        Assert.Contains("mensaje desde DI", await File.ReadAllTextAsync(archivo));
    }

    [Fact]
    public void AddFileLogger_DosVeces_NoDuplicaElProvider()
    {
        var services = new ServiceCollection();
        services.AddLogging(builder => builder
            .AddFileLogger(_tempDirectory)
            .AddFileLogger(_tempDirectory));

        using var provider = services.BuildServiceProvider();

        var proveedores = provider
            .GetServices<ILoggerProvider>()
            .OfType<FileLoggerProviderCore>()
            .ToList();

        Assert.Single(proveedores);
    }

    [Fact]
    public async Task AddFileLogger_ConPathProvider_UsaLaBase()
    {
        var carpetaDatos = Path.Combine(_tempDirectory, "datos");
        var pathProvider = new FileLoggerExtensionsFakeDatabasePathProvider(
            Path.Combine(carpetaDatos, "Historia.db"));

        var services = new ServiceCollection();
        services.AddSingleton<IDatabasePathProvider>(pathProvider);
        services.AddLogging(builder => builder.AddFileLogger());

        using (var provider = services.BuildServiceProvider())
        {
            var logger = provider.GetRequiredService<ILoggerFactory>().CreateLogger("Pruebas");
            logger.LogInformation("mensaje junto a la base");
        }

        var archivo = Path.Combine(
            carpetaDatos,
            "Logs",
            $"historia-{DateTime.Now:yyyy-MM-dd}.log");

        Assert.True(File.Exists(archivo));
        Assert.Contains("mensaje junto a la base", await File.ReadAllTextAsync(archivo));
    }
}

internal sealed class FileLoggerExtensionsFakeDatabasePathProvider : IDatabasePathProvider
{
    public FileLoggerExtensionsFakeDatabasePathProvider(string databasePath)
    {
        DatabasePath = databasePath;
    }

    public string DatabasePath { get; }
}
