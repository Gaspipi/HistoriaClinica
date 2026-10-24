using System.Text.Json;
using Avalonia;
using Avalonia.Styling;
using HistoriaClinica.Core.Data;
using Microsoft.Extensions.Logging;

namespace HistoriaClinica.App.Services;

public enum AppTheme
{
    System,
    Light,
    Dark,
}

public enum AppTextSize
{
    Normal = 100,
    Grande = 115,
    MuyGrande = 130,
    ExtraGrande = 150,
}

public interface IUserPreferencesService
{
    AppTheme Tema { get; }

    AppTextSize TamanoTexto { get; }

    void Inicializar();

    bool Guardar(AppTheme tema, AppTextSize tamanoTexto);
}

public sealed class UserPreferencesService : IUserPreferencesService
{
    private const string FileName = "preferences.json";
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    private readonly string _path;
    private readonly ILogger<UserPreferencesService> _logger;

    public UserPreferencesService(
        IDatabasePathProvider pathProvider,
        ILogger<UserPreferencesService> logger)
    {
        var carpeta = Path.GetDirectoryName(pathProvider.DatabasePath);
        _path = Path.Combine(
            string.IsNullOrWhiteSpace(carpeta) ? AppContext.BaseDirectory : carpeta,
            FileName);
        _logger = logger;
    }

    public AppTheme Tema { get; private set; } = AppTheme.System;

    public AppTextSize TamanoTexto { get; private set; } = AppTextSize.Normal;

    public void Inicializar()
    {
        var preferencias = Leer();
        Tema = preferencias.Tema;
        TamanoTexto = preferencias.TamanoTexto;
        Aplicar(Tema, TamanoTexto);
    }

    public bool Guardar(AppTheme tema, AppTextSize tamanoTexto)
    {
        if (!Enum.IsDefined(tema))
        {
            tema = AppTheme.System;
        }

        if (!Enum.IsDefined(tamanoTexto))
        {
            tamanoTexto = AppTextSize.Normal;
        }

        try
        {
            var carpeta = Path.GetDirectoryName(_path);
            if (!string.IsNullOrWhiteSpace(carpeta))
            {
                Directory.CreateDirectory(carpeta);
            }

            var contenido = JsonSerializer.Serialize(
                new PreferencesFile(tema.ToString(), tamanoTexto.ToString()),
                JsonOptions);
            File.WriteAllText(_path, contenido);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "No se pudieron guardar las preferencias en {Ruta}", _path);
            return false;
        }

        Tema = tema;
        TamanoTexto = tamanoTexto;
        Aplicar(tema, tamanoTexto);
        return true;
    }

    private Preferencias Leer()
    {
        try
        {
            if (!File.Exists(_path))
            {
                return new Preferencias(AppTheme.System, AppTextSize.Normal);
            }

            var contenido = File.ReadAllText(_path);
            var preferencias = JsonSerializer.Deserialize<PreferencesFile>(contenido, JsonOptions);

            var tema = Enum.TryParse<AppTheme>(preferencias?.Tema, ignoreCase: true, out var temaLeido)
                && Enum.IsDefined(temaLeido)
                ? temaLeido
                : AppTheme.System;

            var tamano = Enum.TryParse<AppTextSize>(
                    preferencias?.TamanoTexto,
                    ignoreCase: true,
                    out var tamanoLeido)
                && Enum.IsDefined(tamanoLeido)
                ? tamanoLeido
                : AppTextSize.Normal;

            return new Preferencias(tema, tamano);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "No se pudieron leer las preferencias en {Ruta}", _path);
            return new Preferencias(AppTheme.System, AppTextSize.Normal);
        }
    }

    private static void Aplicar(AppTheme tema, AppTextSize tamanoTexto)
    {
        if (Application.Current is { } app)
        {
            app.RequestedThemeVariant = tema switch
            {
                AppTheme.Light => ThemeVariant.Light,
                AppTheme.Dark => ThemeVariant.Dark,
                _ => ThemeVariant.Default,
            };
        }

        UiMetrics.Aplicar(tamanoTexto);
    }

    private sealed record PreferencesFile(string? Tema, string? TamanoTexto);

    private sealed record Preferencias(AppTheme Tema, AppTextSize TamanoTexto);
}
