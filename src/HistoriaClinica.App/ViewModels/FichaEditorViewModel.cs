using CommunityToolkit.Mvvm.ComponentModel;
using HistoriaClinica.App.Services;
using HistoriaClinica.Core.Models;
using HistoriaClinica.Core.Services;
using HistoriaClinica.Core.Validation;
using Microsoft.Extensions.Logging;

namespace HistoriaClinica.App.ViewModels;

public partial class FichaEditorViewModel : EditorViewModelBase
{
    private readonly FichaService _fichaService;
    private readonly bool _esNuevo;
    private readonly int _id;
    private readonly DateTime _fecha;
    private readonly string _nombrePaciente;
    private readonly Snapshot _original;

    public FichaEditorViewModel(
        FichaService fichaService,
        IDialogService dialogs,
        ILogger<FichaEditorViewModel> logger,
        FichaDiaria ficha,
        bool esNuevo,
        string nombrePaciente = "")
        : base(dialogs, logger)
    {
        _fichaService = fichaService;
        _esNuevo = esNuevo;
        _id = ficha.Id;
        _fecha = ficha.Fecha;
        _nombrePaciente = nombrePaciente;

        Dni = ficha.Dni;
        Motivo = ficha.Motivo;
        Enfermedad = ficha.Enfermedad;
        Indicaciones = ficha.Indicaciones;

        _original = Capturar();
    }

    public override string Titulo
    {
        get
        {
            var baseTitulo = _esNuevo ? "Nueva ficha" : "Editar ficha";
            return string.IsNullOrWhiteSpace(_nombrePaciente)
                ? baseTitulo
                : $"{baseTitulo} — {_nombrePaciente}";
        }
    }

    public override bool HayCambios => Capturar() != _original;

    public FichaDiaria? Resultado { get; private set; }

    public string FechaTexto => FechaClinica.FormatDateTime(Resultado?.Fecha ?? _fecha);

    [ObservableProperty]
    private string _dni = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TieneErrorMotivo))]
    private string? _errorMotivo;

    [ObservableProperty]
    private string _motivo = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TieneErrorEnfermedad))]
    private string? _errorEnfermedad;

    [ObservableProperty]
    private string _enfermedad = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TieneErrorIndicaciones))]
    private string? _errorIndicaciones;

    [ObservableProperty]
    private string _indicaciones = string.Empty;

    public bool TieneErrorMotivo => !string.IsNullOrWhiteSpace(ErrorMotivo);

    public bool TieneErrorEnfermedad => !string.IsNullOrWhiteSpace(ErrorEnfermedad);

    public bool TieneErrorIndicaciones => !string.IsNullOrWhiteSpace(ErrorIndicaciones);

    partial void OnMotivoChanged(string value) => ErrorMotivo = null;

    partial void OnEnfermedadChanged(string value) => ErrorEnfermedad = null;

    partial void OnIndicacionesChanged(string value) => ErrorIndicaciones = null;

    protected override bool Validar()
    {
        var ficha = CrearFicha();
        var validation = HistoriaValidators.ValidateFicha(ficha);

        ErrorMotivo = string.IsNullOrWhiteSpace(ficha.Motivo)
            ? "El campo Motivo es obligatorio."
            : null;
        ErrorEnfermedad = string.IsNullOrWhiteSpace(ficha.Enfermedad)
            ? "El campo Enfermedad es obligatorio."
            : null;
        ErrorIndicaciones = string.IsNullOrWhiteSpace(ficha.Indicaciones)
            ? "El campo Indicaciones es obligatorio."
            : null;

        if (validation.IsValid)
        {
            return true;
        }

        if (!TieneErrorMotivo && !TieneErrorEnfermedad && !TieneErrorIndicaciones)
        {
            MensajeError = validation.ErrorMessage;
        }

        return false;
    }

    protected override async Task<bool> GuardarCoreAsync()
    {
        var ficha = CrearFicha();

        // En una ficha nueva la fecha se captura al guardar, no al abrir el diálogo.
        if (_esNuevo)
        {
            ficha.Fecha = DateTime.UtcNow;
        }

        await _fichaService.SaveAsync(ficha);
        Resultado = ficha;
        return true;
    }

    private FichaDiaria CrearFicha() => new()
    {
        Id = _id,
        Dni = Dni.Trim(),
        Fecha = _fecha,
        Motivo = Motivo,
        Enfermedad = Enfermedad,
        Indicaciones = Indicaciones,
    };

    private Snapshot Capturar() => new(Dni, Motivo, Enfermedad, Indicaciones);

    private sealed record Snapshot(
        string Dni,
        string Motivo,
        string Enfermedad,
        string Indicaciones);
}
