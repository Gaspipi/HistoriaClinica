using HistoriaClinica.App.Services;
using HistoriaClinica.App.ViewModels;
using HistoriaClinica.Core;
using HistoriaClinica.Core.Data;
using HistoriaClinica.Core.Models;
using HistoriaClinica.Core.Repositories;
using HistoriaClinica.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HistoriaClinica.Tests;

public sealed class DependencyInjectionTests : IDisposable
{
    private readonly string _tempDirectory;
    private readonly string _databasePath;

    public DependencyInjectionTests()
    {
        _tempDirectory = Path.Combine(Path.GetTempPath(), $"historia-di-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDirectory);
        _databasePath = Path.Combine(_tempDirectory, "Historia.db");
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDirectory))
        {
            Directory.Delete(_tempDirectory, recursive: true);
        }
    }

    private ServiceProvider CrearProvider()
    {
        var services = new ServiceCollection();
        services.AddHistoriaClinicaCore(_databasePath);
        services.AddLogging(builder => builder.SetMinimumLevel(LogLevel.Warning));
        services.AddSingleton<IDialogService, FakeDialogService>();
        services.AddSingleton<UserSession>();
        services.AddSingleton<IAppNavigator, FakeAppNavigator>();
        services.AddSingleton<IClipboardService, FakeClipboardService>();
        services.AddSingleton<IDataMaintenanceService, FakeDataMaintenanceService>();
        services.AddSingleton<IHtmlExportService, HtmlExportService>();
        services.AddTransient<MainWindowViewModel>();
        return services.BuildServiceProvider();
    }

    [Fact]
    public void Contenedor_ResuelveMainWindowViewModelSinAmbiguedad()
    {
        using var provider = CrearProvider();

        var viewModel = provider.GetRequiredService<MainWindowViewModel>();

        Assert.NotNull(viewModel);
    }

    [Fact]
    public void Contenedor_ResuelveServiciosDelCore()
    {
        using var provider = CrearProvider();

        Assert.NotNull(provider.GetRequiredService<PacienteService>());
        Assert.NotNull(provider.GetRequiredService<FichaService>());
        Assert.NotNull(provider.GetRequiredService<PlantillaService>());
        Assert.NotNull(provider.GetRequiredService<IAuditService>());
        Assert.NotNull(provider.GetRequiredService<IPacienteRepository>());
        Assert.NotNull(provider.GetRequiredService<IFichaRepository>());
        Assert.NotNull(provider.GetRequiredService<IPlantillaRepository>());
        Assert.NotNull(provider.GetRequiredService<IAuditLogRepository>());
    }

    [Fact]
    public async Task Contenedor_ConectaAuditoriaConPacienteService()
    {
        using var provider = CrearProvider();
        var initializer = provider.GetRequiredService<DatabaseInitializer>();
        await initializer.InitializeAsync();

        var pacientes = provider.GetRequiredService<PacienteService>();
        var auditoria = provider.GetRequiredService<IAuditLogRepository>();

        await pacientes.SaveAsync(
            new Paciente { Dni = "12345678", FirstName = "Ana", LastName = "Pérez" },
            "ana");

        var logs = await auditoria.GetAllAsync();
        var log = Assert.Single(logs);
        Assert.Equal("Pacientes", log.Entity);
        Assert.Equal("Create", log.Action);
        Assert.Equal("ana", log.Username);
    }

    [Fact]
    public void Contenedor_UsaLaRutaDeBaseIndicada()
    {
        using var provider = CrearProvider();

        var pathProvider = provider.GetRequiredService<IDatabasePathProvider>();

        Assert.Equal(_databasePath, pathProvider.DatabasePath);
    }
}
