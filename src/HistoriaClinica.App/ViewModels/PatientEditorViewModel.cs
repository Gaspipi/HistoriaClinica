using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HistoriaClinica.App.Services;
using HistoriaClinica.Core.Models;
using HistoriaClinica.Core.Services;
using HistoriaClinica.Core.Validation;
using Microsoft.Extensions.Logging;

namespace HistoriaClinica.App.ViewModels;

public partial class PatientEditorViewModel : EditorViewModelBase
{
    private readonly PacienteService _pacienteService;
    private readonly bool _esNuevo;
    private readonly Snapshot _original;

    public PatientEditorViewModel(
        PacienteService pacienteService,
        IDialogService dialogs,
        ILogger<PatientEditorViewModel> logger,
        Paciente paciente,
        bool esNuevo)
        : base(dialogs, logger)
    {
        _pacienteService = pacienteService;
        _esNuevo = esNuevo;

        Dni = paciente.Dni;
        FirstName = paciente.FirstName;
        LastName = paciente.LastName;
        FechaNacimiento = FechaClinica.ToNullable(paciente.DOB);
        Phone = paciente.Phone;
        ObraSocial = paciente.ObraSocial;
        NroSocio = paciente.NroSocio;
        AntecFam = paciente.AntecFam;
        AntecPers = paciente.AntecPers;
        Medicacion = paciente.Medicacion;

        _original = Capturar();
    }

    public override string Titulo
    {
        get
        {
            if (_esNuevo)
            {
                return "Nuevo paciente";
            }

            var nombre = string.Join(
                " ",
                new[] { FirstName, LastName }.Where(parte => !string.IsNullOrWhiteSpace(parte)));

            return string.IsNullOrWhiteSpace(nombre)
                ? "Editar paciente"
                : $"Editar paciente — {nombre}";
        }
    }

    public override bool HayCambios => Capturar() != _original;

    public Paciente? Resultado { get; private set; }

    public bool DniEditable => _esNuevo;

    [ObservableProperty]
    private string _dni = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TieneErrorDni))]
    private string? _errorDni;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Titulo))]
    private string _firstName = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Titulo))]
    private string _lastName = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TieneErrorFechaNacimiento))]
    [NotifyPropertyChangedFor(nameof(TieneFechaNacimiento))]
    private DateTime? _fechaNacimiento;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TieneErrorFechaNacimiento))]
    private string? _errorFechaNacimiento;

    [ObservableProperty]
    private string _phone = string.Empty;

    [ObservableProperty]
    private string _obraSocial = string.Empty;

    [ObservableProperty]
    private string _nroSocio = string.Empty;

    [ObservableProperty]
    private string _antecFam = string.Empty;

    [ObservableProperty]
    private string _antecPers = string.Empty;

    [ObservableProperty]
    private string _medicacion = string.Empty;

    public bool TieneErrorDni => !string.IsNullOrWhiteSpace(ErrorDni);

    public bool TieneErrorFechaNacimiento => !string.IsNullOrWhiteSpace(ErrorFechaNacimiento);

    public bool TieneFechaNacimiento => FechaNacimiento.HasValue;

    [RelayCommand]
    private void LimpiarFechaNacimiento()
    {
        FechaNacimiento = null;
        ErrorFechaNacimiento = null;
    }

    partial void OnDniChanged(string value)
    {
        var limpio = DniValidator.Normalizar(value);
        if (!string.Equals(value, limpio, StringComparison.Ordinal))
        {
            Dni = limpio;
            return;
        }

        ErrorDni = null;
    }

    partial void OnFechaNacimientoChanged(DateTime? value) => ErrorFechaNacimiento = null;

    protected override bool Validar()
    {
        var paciente = CrearPaciente();
        var validation = HistoriaValidators.ValidatePaciente(paciente);

        ErrorDni = validation.IsValid ? null : DniValidator.GetError(paciente.Dni);
        ErrorFechaNacimiento =
            FechaClinica.HasValue(paciente.DOB) && paciente.DOB.Date > DateTime.Today
                ? "La fecha de nacimiento no puede ser posterior a hoy."
                : null;

        if (validation.IsValid)
        {
            return true;
        }

        if (!TieneErrorDni && !TieneErrorFechaNacimiento)
        {
            MensajeError = validation.ErrorMessage;
        }

        return false;
    }

    protected override async Task<bool> GuardarCoreAsync()
    {
        var paciente = CrearPaciente();

        if (_esNuevo)
        {
            var existente = await _pacienteService.GetByDniAsync(paciente.Dni);
            if (existente is not null)
            {
                ErrorDni = $"Ya existe un paciente con el DNI {paciente.Dni}.";
                return false;
            }
        }

        await _pacienteService.SaveAsync(paciente);
        Resultado = paciente;
        return true;
    }

    private Paciente CrearPaciente() => new()
    {
        Dni = Dni.Trim(),
        FirstName = FirstName,
        LastName = LastName,
        DOB = FechaClinica.FromNullable(FechaNacimiento),
        Phone = Phone,
        ObraSocial = ObraSocial,
        NroSocio = NroSocio,
        AntecFam = AntecFam,
        AntecPers = AntecPers,
        Medicacion = Medicacion,
    };

    private Snapshot Capturar() => new(
        Dni,
        FirstName,
        LastName,
        FechaNacimiento,
        Phone,
        ObraSocial,
        NroSocio,
        AntecFam,
        AntecPers,
        Medicacion);

    private sealed record Snapshot(
        string Dni,
        string FirstName,
        string LastName,
        DateTime? FechaNacimiento,
        string Phone,
        string ObraSocial,
        string NroSocio,
        string AntecFam,
        string AntecPers,
        string Medicacion);
}
