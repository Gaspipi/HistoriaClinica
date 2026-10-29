using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HistoriaClinica.App.Services;

namespace HistoriaClinica.App.ViewModels;

public partial class PreferencesViewModel : ObservableObject
{
    private static readonly AppTextSize[] Tamanos =
    [
        AppTextSize.Normal,
        AppTextSize.Grande,
        AppTextSize.MuyGrande,
        AppTextSize.ExtraGrande,
    ];

    private readonly IUserPreferencesService _preferences;

    public PreferencesViewModel(IUserPreferencesService preferences)
    {
        _preferences = preferences;
        TemaSeleccionado = (int)(Enum.IsDefined(preferences.Tema) ? preferences.Tema : AppTheme.System);

        var indiceTamano = Array.IndexOf(Tamanos, preferences.TamanoTexto);
        if (indiceTamano < 0)
        {
            MensajeError = "El tamaño de texto guardado no es válido y se restableció al predeterminado.";
            TamanoTextoSeleccionado = 0;
        }
        else
        {
            TamanoTextoSeleccionado = indiceTamano;
        }
    }

    public IReadOnlyList<string> Temas { get; } = ["Según el sistema", "Claro", "Oscuro"];

    public IReadOnlyList<string> TamanosTexto { get; } =
    [
        "Normal (100 %)",
        "Grande (115 %)",
        "Muy grande (130 %)",
        "Extra grande (150 %)",
    ];

    public event EventHandler? CloseRequested;

    [ObservableProperty]
    private int _temaSeleccionado;

    [ObservableProperty]
    private int _tamanoTextoSeleccionado;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HayError))]
    private string? _mensajeError;

    public bool HayError => !string.IsNullOrWhiteSpace(MensajeError);

    [RelayCommand]
    private void Guardar()
    {
        MensajeError = null;

        var tema = Enum.IsDefined((AppTheme)TemaSeleccionado)
            ? (AppTheme)TemaSeleccionado
            : AppTheme.System;

        var tamano = TamanoTextoSeleccionado >= 0 && TamanoTextoSeleccionado < Tamanos.Length
            ? Tamanos[TamanoTextoSeleccionado]
            : AppTextSize.Normal;

        if (!_preferences.Guardar(tema, tamano))
        {
            MensajeError = "No se pudieron guardar las preferencias. Verificá los permisos de la carpeta de datos.";
            return;
        }

        CloseRequested?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private void Restablecer()
    {
        TemaSeleccionado = (int)AppTheme.System;
        TamanoTextoSeleccionado = 0;
        MensajeError = null;
    }

    [RelayCommand]
    private void Cancelar() => CloseRequested?.Invoke(this, EventArgs.Empty);
}
