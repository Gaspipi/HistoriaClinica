using HistoriaClinica.App.Services;
using HistoriaClinica.Core.Data;
using Microsoft.Extensions.Logging.Abstractions;

namespace HistoriaClinica.Tests;

public sealed class UserPreferencesServiceTests : IDisposable
{
    private readonly string _carpeta = Path.Combine(
        Path.GetTempPath(),
        "HistoriaClinica.Tests",
        Guid.NewGuid().ToString("N"));

    public UserPreferencesServiceTests()
    {
        Directory.CreateDirectory(_carpeta);
    }

    [Fact]
    public void GuardarPreferencias_PersisteParaLaProximaApertura()
    {
        var servicio = CrearServicio();
        servicio.Inicializar();

        Assert.Equal(AppTheme.System, servicio.Tema);
        Assert.Equal(AppTextSize.Normal, servicio.TamanoTexto);
        Assert.True(servicio.Guardar(AppTheme.Dark, AppTextSize.Grande));

        var recargado = CrearServicio();
        recargado.Inicializar();

        Assert.Equal(AppTheme.Dark, recargado.Tema);
        Assert.Equal(AppTextSize.Grande, recargado.TamanoTexto);
    }

    [Fact]
    public void ArchivoInvalido_UsaValoresPorDefecto()
    {
        File.WriteAllText(Path.Combine(_carpeta, "preferences.json"), "{invalido");

        var servicio = CrearServicio();
        servicio.Inicializar();

        Assert.Equal(AppTheme.System, servicio.Tema);
        Assert.Equal(AppTextSize.Normal, servicio.TamanoTexto);
    }

    [Fact]
    public void TemaIndefinido_UsaTemaDelSistema()
    {
        File.WriteAllText(Path.Combine(_carpeta, "preferences.json"), """{"Tema":"99"}""");

        var servicio = CrearServicio();
        servicio.Inicializar();

        Assert.Equal(AppTheme.System, servicio.Tema);
    }

    [Fact]
    public void TamanoIndefinido_UsaNormal()
    {
        File.WriteAllText(
            Path.Combine(_carpeta, "preferences.json"),
            """{"Tema":"Light","TamanoTexto":"99"}""");

        var servicio = CrearServicio();
        servicio.Inicializar();

        Assert.Equal(AppTheme.Light, servicio.Tema);
        Assert.Equal(AppTextSize.Normal, servicio.TamanoTexto);
    }

    [Fact]
    public void ArchivoViejoSinTamano_UsaNormal()
    {
        File.WriteAllText(Path.Combine(_carpeta, "preferences.json"), """{"Tema":"Dark"}""");

        var servicio = CrearServicio();
        servicio.Inicializar();

        Assert.Equal(AppTheme.Dark, servicio.Tema);
        Assert.Equal(AppTextSize.Normal, servicio.TamanoTexto);
    }

    [Fact]
    public void GuardarTamanoInvalido_UsaNormal()
    {
        var servicio = CrearServicio();
        servicio.Inicializar();

        Assert.True(servicio.Guardar(AppTheme.System, (AppTextSize)999));

        var recargado = CrearServicio();
        recargado.Inicializar();

        Assert.Equal(AppTextSize.Normal, recargado.TamanoTexto);
    }

    [Fact]
    public void GuardarDosVecesSeguidas_DejaArchivoValidoSinTemporal()
    {
        var servicio = CrearServicio();
        servicio.Inicializar();

        Assert.True(servicio.Guardar(AppTheme.Light, AppTextSize.Grande));
        Assert.True(servicio.Guardar(AppTheme.Dark, AppTextSize.MuyGrande));

        Assert.False(File.Exists(Path.Combine(_carpeta, "preferences.json.tmp")));

        var recargado = CrearServicio();
        recargado.Inicializar();

        Assert.Equal(AppTheme.Dark, recargado.Tema);
        Assert.Equal(AppTextSize.MuyGrande, recargado.TamanoTexto);
    }

    public void Dispose()
    {
        if (Directory.Exists(_carpeta))
        {
            Directory.Delete(_carpeta, recursive: true);
        }
    }

    private UserPreferencesService CrearServicio() => new(
        new DatabasePathProvider(Path.Combine(_carpeta, "Historia.db")),
        NullLogger<UserPreferencesService>.Instance);
}
