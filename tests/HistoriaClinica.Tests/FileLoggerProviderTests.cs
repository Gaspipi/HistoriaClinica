using HistoriaClinica.Core.Data;
using Microsoft.Extensions.Logging;
using FileLoggerProviderCore = HistoriaClinica.Core.Logging.FileLoggerProvider;

namespace HistoriaClinica.Tests;

public sealed class FileLoggerProviderTests : IDisposable
{
    private readonly string _tempDirectory;

    public FileLoggerProviderTests()
    {
        _tempDirectory = Path.Combine(Path.GetTempPath(), $"historia-log-{Guid.NewGuid():N}");
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
    public async Task Log_EscribeArchivoEnSubcarpetaLogs()
    {
        using (var provider = new FileLoggerProviderCore(_tempDirectory))
        {
            var logger = provider.CreateLogger("Pruebas");
            logger.LogError(new InvalidOperationException("falla simulada"), "mensaje de prueba");
        }

        var archivo = Path.Combine(
            _tempDirectory,
            "Logs",
            $"historia-{DateTime.Now:yyyy-MM-dd}.log");

        Assert.True(File.Exists(archivo));
        var contenido = await File.ReadAllTextAsync(archivo);
        Assert.Contains("[Error]", contenido);
        Assert.Contains("mensaje de prueba", contenido);
        Assert.Contains(nameof(InvalidOperationException), contenido);
    }

    [Fact]
    public async Task Log_SegundaInstancia_AppendeaSinDuplicarCarpeta()
    {
        var carpetaLogs = Path.Combine(_tempDirectory, "Logs");

        for (var i = 0; i < 2; i++)
        {
            using var provider = new FileLoggerProviderCore(carpetaLogs);
            provider.CreateLogger("Pruebas").LogInformation("linea {Numero}", i);
        }

        Assert.False(Directory.Exists(Path.Combine(carpetaLogs, "Logs")));

        var archivo = Path.Combine(carpetaLogs, $"historia-{DateTime.Now:yyyy-MM-dd}.log");
        Assert.True(File.Exists(archivo));
        var contenido = await File.ReadAllTextAsync(archivo);
        Assert.Contains("linea 0", contenido);
        Assert.Contains("linea 1", contenido);
        Assert.Equal(
            2,
            contenido.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).Length);
    }

    [Fact]
    public void Dispose_EsIdempotenteYCrearLoggerFalla()
    {
        var provider = new FileLoggerProviderCore(_tempDirectory);

        provider.Dispose();
        provider.Dispose();

        Assert.Throws<ObjectDisposedException>(() => provider.CreateLogger("Pruebas"));
    }

    [Fact]
    public void IsEnabled_SoloNoneEsFalse()
    {
        using var provider = new FileLoggerProviderCore(_tempDirectory);
        var logger = provider.CreateLogger("Pruebas");

        Assert.False(logger.IsEnabled(LogLevel.None));
        Assert.True(logger.IsEnabled(LogLevel.Trace));
        Assert.True(logger.IsEnabled(LogLevel.Debug));
        Assert.True(logger.IsEnabled(LogLevel.Information));
        Assert.True(logger.IsEnabled(LogLevel.Warning));
        Assert.True(logger.IsEnabled(LogLevel.Error));
        Assert.True(logger.IsEnabled(LogLevel.Critical));
    }

    [Fact]
    public void BeginScope_DevuelveDisposableNoNulo()
    {
        using var provider = new FileLoggerProviderCore(_tempDirectory);
        var logger = provider.CreateLogger("Pruebas");

        using var scope = logger.BeginScope("contexto");

        Assert.NotNull(scope);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_RutaVacia_LanzaArgumentException(string ruta)
    {
        Assert.Throws<ArgumentException>(() => new FileLoggerProviderCore(ruta));
    }

    [Fact]
    public async Task Constructor_ConPathProvider_UsaLaCarpetaDeLaBase()
    {
        var carpetaDatos = Path.Combine(_tempDirectory, "datos");
        var pathProvider = new FileLoggerFakeDatabasePathProvider(
            Path.Combine(carpetaDatos, "Historia.db"));

        using (var provider = new FileLoggerProviderCore(pathProvider))
        {
            provider.CreateLogger("Pruebas").LogInformation("con ruta de base");
        }

        var archivo = Path.Combine(
            carpetaDatos,
            "Logs",
            $"historia-{DateTime.Now:yyyy-MM-dd}.log");

        Assert.True(File.Exists(archivo));
        Assert.Contains("con ruta de base", await File.ReadAllTextAsync(archivo));
    }
}

internal sealed class FileLoggerFakeDatabasePathProvider : IDatabasePathProvider
{
    public FileLoggerFakeDatabasePathProvider(string databasePath)
    {
        DatabasePath = databasePath;
    }

    public string DatabasePath { get; }
}
