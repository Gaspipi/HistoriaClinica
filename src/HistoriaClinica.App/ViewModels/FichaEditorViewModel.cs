using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HistoriaClinica.App.Services;
using HistoriaClinica.Core.Models;
using HistoriaClinica.Core.Services;
using HistoriaClinica.Core.Validation;
using Microsoft.Extensions.Logging;

namespace HistoriaClinica.App.ViewModels;

public partial class FichaEditorViewModel : EditorViewModelBase
{
    private readonly FichaService _fichaService;
    private readonly PlantillaService? _plantillaService;
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
        string nombrePaciente = "",
        PlantillaService? plantillaService = null)
        : base(dialogs, logger)
    {
        _fichaService = fichaService;
        _plantillaService = plantillaService;
        _esNuevo = esNuevo;
        _id = ficha.Id;
        _fecha = ficha.Fecha;
        _nombrePaciente = nombrePaciente;

        Dni = ficha.Dni;
        Motivo = ficha.Motivo;
        Enfermedad = ficha.Enfermedad;
        Indicaciones = ficha.Indicaciones;
        FechaCreacion = esNuevo ? DateTime.Now : ficha.Fecha;

        Cerrada = ficha.Cerrada;

        PresionArterial = ficha.PresionArterial;
        FrecuenciaCardiaca = ficha.FrecuenciaCardiaca;
        Temperatura = ficha.Temperatura;
        Peso = ficha.Peso;

        PlantillasDisponibles = new List<Plantilla>();

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

    public bool EsNuevo => _esNuevo;

    public bool Cerrada { get; }

    protected override bool PuedeGuardarCore() => !Cerrada;

    public string FechaTexto => FechaClinica.FormatDateTime(Resultado?.Fecha ?? _fecha);

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HoraCreacionTime))]
    private DateTime? _fechaCreacion;

    public TimeSpan? HoraCreacionTime
    {
        get => FechaCreacion?.TimeOfDay;
        set
        {
            if (value is { } t)
            {
                var baseFecha = FechaCreacion?.Date ?? DateTime.Today;
                FechaCreacion = baseFecha + t;
            }
            OnPropertyChanged(nameof(HoraCreacionTime));
        }
    }

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

    [ObservableProperty]
    private List<Plantilla> _plantillasDisponibles = new();

    [ObservableProperty]
    private Plantilla? _plantillaSeleccionada;

    [ObservableProperty]
    private string? _presionArterial;

    [ObservableProperty]
    private int? _frecuenciaCardiaca;

    [ObservableProperty]
    private decimal? _temperatura;

    [ObservableProperty]
    private decimal? _peso;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TieneErrorPresionArterial))]
    private string? _errorPresionArterial;

    public bool TieneErrorMotivo => !string.IsNullOrWhiteSpace(ErrorMotivo);

    public bool TieneErrorEnfermedad => !string.IsNullOrWhiteSpace(ErrorEnfermedad);

    public bool TieneErrorIndicaciones => !string.IsNullOrWhiteSpace(ErrorIndicaciones);

    public bool TieneErrorPresionArterial => !string.IsNullOrWhiteSpace(ErrorPresionArterial);

    partial void OnMotivoChanged(string value) => ErrorMotivo = null;

    partial void OnEnfermedadChanged(string value) => ErrorEnfermedad = null;

    partial void OnIndicacionesChanged(string value) => ErrorIndicaciones = null;

    partial void OnPresionArterialChanged(string? value) => ErrorPresionArterial = null;

    public Plantilla? SelectedPlantilla
    {
        get => PlantillaSeleccionada;
        set => PlantillaSeleccionada = value;
    }

    public Plantilla? PlantillaActual
    {
        get => PlantillaSeleccionada;
        set => PlantillaSeleccionada = value;
    }

    public async Task InitializeAsync()
    {
        if (_plantillaService is null)
        {
            PlantillasDisponibles = new List<Plantilla>();
            return;
        }

        PlantillasDisponibles = (await _plantillaService.GetAllAsync()).ToList();
    }

    [RelayCommand]
    public void AplicarPlantilla(Plantilla? plantilla = null)
    {
        var seleccionada = plantilla ?? PlantillaSeleccionada;
        if (seleccionada is null)
        {
            return;
        }

        Motivo = seleccionada.Motivo ?? string.Empty;
        Enfermedad = seleccionada.Enfermedad ?? string.Empty;
        Indicaciones = seleccionada.Indicaciones ?? string.Empty;
    }

    protected override bool Validar()
    {
        if (Cerrada)
        {
            MensajeError = "La ficha está cerrada y no puede modificarse.";
            return false;
        }

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
        ErrorPresionArterial = ValidarPresionArterial(ficha.PresionArterial);

        if (validation.IsValid && ErrorPresionArterial is null)
        {
            return true;
        }

        if (!TieneErrorMotivo && !TieneErrorEnfermedad && !TieneErrorIndicaciones && !TieneErrorPresionArterial)
        {
            MensajeError = validation.ErrorMessage;
        }

        return false;
    }

    private static string? ValidarPresionArterial(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            return null;
        }

        var partes = valor.Trim().Split('/');
        if (partes.Length != 2
            || !int.TryParse(partes[0], out var sistolica)
            || !int.TryParse(partes[1], out var diastolica)
            || sistolica < 40 || sistolica > 300
            || diastolica < 20 || diastolica > 200)
        {
            return "Formato de presión arterial inválido (ej: 120/80).";
        }

        return null;
    }

    protected override async Task<bool> GuardarCoreAsync()
    {
        if (Cerrada)
        {
            MensajeError = "La ficha está cerrada y no puede modificarse.";
            return false;
        }

        var ficha = CrearFicha();

        // En una ficha nueva la fecha se toma del selector (FechaCreacion).
        if (_esNuevo)
        {
            if (FechaCreacion is null)
            {
                MensajeError = "La fecha de creación es obligatoria.";
                return false;
            }

            ficha.Fecha = FechaCreacion.Value;
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
        Cerrada = Cerrada,
        PresionArterial = string.IsNullOrWhiteSpace(PresionArterial) ? null : PresionArterial.Trim(),
        FrecuenciaCardiaca = FrecuenciaCardiaca,
        Temperatura = Temperatura,
        Peso = Peso,
    };

    private Snapshot Capturar() => new(Dni, Motivo, Enfermedad, Indicaciones, FechaCreacion, PresionArterial, FrecuenciaCardiaca, Temperatura, Peso);

    private sealed record Snapshot(
        string Dni,
        string Motivo,
        string Enfermedad,
        string Indicaciones,
        DateTime? FechaCreacion,
        string? PresionArterial,
        int? FrecuenciaCardiaca,
        decimal? Temperatura,
        decimal? Peso);
}
