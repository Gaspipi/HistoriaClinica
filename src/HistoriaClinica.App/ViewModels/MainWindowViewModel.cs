using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HistoriaClinica.App.Collections;
using HistoriaClinica.App.Services;
using HistoriaClinica.Core.Models;
using HistoriaClinica.Core.Services;
using HistoriaClinica.Core.Validation;
using Microsoft.Extensions.Logging;

namespace HistoriaClinica.App.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly PacienteService _pacienteService;
    private readonly FichaService _fichaService;
    private readonly IDialogService _dialogs;
    private readonly UserSession _session;
    private readonly IAppNavigator _navigator;
    private readonly IClipboardService _clipboard;
    private readonly IDataMaintenanceService _maintenance;
    private readonly ILogger<MainWindowViewModel> _logger;

    public MainWindowViewModel(
        PacienteService pacienteService,
        FichaService fichaService,
        IDialogService dialogs,
        UserSession session,
        IAppNavigator navigator,
        IClipboardService clipboard,
        IDataMaintenanceService maintenance,
        ILogger<MainWindowViewModel> logger)
    {
        _pacienteService = pacienteService;
        _fichaService = fichaService;
        _dialogs = dialogs;
        _session = session;
        _navigator = navigator;
        _clipboard = clipboard;
        _maintenance = maintenance;
        _logger = logger;

        Fichas.CollectionChanged += (_, _) =>
        {
            OnPropertyChanged(nameof(HayFichas));
            OnPropertyChanged(nameof(CantidadFichasTexto));
        };
    }

    public RangeObservableCollection<string> DnisSugeridos { get; } = new();

    public RangeObservableCollection<FichaDiaria> Fichas { get; } = new();

    [ObservableProperty]
    private string _dni = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HayError))]
    private string? _mensajeError;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HayMensajeEstado))]
    private string? _mensajeEstado;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FechaNacimientoTexto))]
    [NotifyPropertyChangedFor(nameof(HayPaciente))]
    [NotifyPropertyChangedFor(nameof(NombreCompleto))]
    [NotifyPropertyChangedFor(nameof(InicialesPaciente))]
    [NotifyPropertyChangedFor(nameof(DniPacienteTexto))]
    [NotifyPropertyChangedFor(nameof(EdadTexto))]
    [NotifyPropertyChangedFor(nameof(ObraSocialTexto))]
    [NotifyPropertyChangedFor(nameof(PacienteResumen))]
    [NotifyPropertyChangedFor(nameof(ResumenCopiable))]
    [NotifyCanExecuteChangedFor(nameof(EditarPacienteCommand))]
    [NotifyCanExecuteChangedFor(nameof(NuevaFichaCommand))]
    [NotifyCanExecuteChangedFor(nameof(CopiarResumenCommand))]
    [NotifyCanExecuteChangedFor(nameof(CopiarDniCommand))]
    [NotifyCanExecuteChangedFor(nameof(CopiarTelefonoCommand))]
    private Paciente? _pacienteActual;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HayFichaSeleccionada))]
    [NotifyCanExecuteChangedFor(nameof(EditarFichaCommand))]
    [NotifyCanExecuteChangedFor(nameof(BorrarFichaCommand))]
    private FichaDiaria? _fichaSeleccionada;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(BuscarCommand))]
    [NotifyCanExecuteChangedFor(nameof(NuevoPacienteCommand))]
    [NotifyCanExecuteChangedFor(nameof(EditarPacienteCommand))]
    [NotifyCanExecuteChangedFor(nameof(NuevaFichaCommand))]
    [NotifyCanExecuteChangedFor(nameof(EditarFichaCommand))]
    [NotifyCanExecuteChangedFor(nameof(BorrarFichaCommand))]
    [NotifyCanExecuteChangedFor(nameof(CerrarSesionCommand))]
    private bool _isBusy;

    public bool HayError => !string.IsNullOrWhiteSpace(MensajeError);

    public bool HayMensajeEstado => !string.IsNullOrWhiteSpace(MensajeEstado);

    public bool HayPaciente => PacienteActual is not null;

    public bool HayFichas => Fichas.Count > 0;

    public bool HayFichaSeleccionada => FichaSeleccionada is not null;

    public string NombreUsuario => _session.NombreUsuario;

    public string InicialesUsuario => _session.InicialesUsuario;

    public string ResumenCopiable => HayPaciente ? $"{NombreCompleto} · {PacienteResumen}" : string.Empty;

    public string FechaNacimientoTexto =>
        PacienteActual is null || !PacienteActual.DOB.HasValue ? string.Empty : PacienteActual.DOB.Value.ToString("dd/MM/yyyy", FechaClinica.Culture);

    public string NombreCompleto
    {
        get
        {
            if (PacienteActual is null)
            {
                return "Paciente sin nombre";
            }

            var partes = new[] { PacienteActual.FirstName, PacienteActual.LastName }
                .Where(parte => !string.IsNullOrWhiteSpace(parte));

            return partes.Any() ? string.Join(" ", partes) : "Paciente sin nombre";
        }
    }

    public string InicialesPaciente
    {
        get
        {
            if (PacienteActual is null)
            {
                return "?";
            }

            var iniciales = new[] { PacienteActual.FirstName, PacienteActual.LastName }
                .Where(parte => !string.IsNullOrWhiteSpace(parte))
                .Select(parte => char.ToUpperInvariant(parte.Trim()[0]))
                .ToArray();

            return iniciales.Length == 0 ? "?" : new string(iniciales);
        }
    }

    public string DniPacienteTexto =>
        PacienteActual is null || string.IsNullOrWhiteSpace(PacienteActual.Dni)
            ? "Sin DNI"
            : $"DNI {PacienteActual.Dni}";

    public string EdadTexto
    {
        get
        {
            if (PacienteActual is null || !PacienteActual.DOB.HasValue)
            {
                return "Sin fecha de nacimiento";
            }

            var hoy = DateOnly.FromDateTime(DateTime.Today);
            var nacimiento = PacienteActual.DOB.Value;
            if (nacimiento > hoy)
            {
                return "Fecha inválida";
            }

            var edad = hoy.Year - nacimiento.Year;
            if (nacimiento > hoy.AddYears(-edad))
            {
                edad--;
            }

            return edad switch
            {
                0 => "Menos de 1 año",
                1 => "1 año",
                _ => $"{edad} años",
            };
        }
    }

    public string ObraSocialTexto =>
        PacienteActual is null || string.IsNullOrWhiteSpace(PacienteActual.ObraSocial)
            ? "Sin obra social"
            : PacienteActual.ObraSocial;

    public string PacienteResumen => $"{DniPacienteTexto} · {EdadTexto} · {ObraSocialTexto}";

    public string CantidadFichasTexto => Fichas.Count switch
    {
        0 => "Sin fichas",
        1 => "1 ficha",
        _ => $"{Fichas.Count} fichas",
    };

    public Task InitializeAsync() => EjecutarAsync(
        () => CargarDnisAsync(),
        "Error al inicializar la aplicación");

    partial void OnDniChanged(string value)
    {
        var limpio = DniValidator.Normalizar(value);
        if (!string.Equals(value, limpio, StringComparison.Ordinal))
        {
            Dni = limpio;
            return;
        }

        MensajeError = null;
    }

    [RelayCommand]
    private void LimpiarError() => MensajeError = null;

    [RelayCommand]
    private void LimpiarMensaje() => MensajeEstado = null;

    [RelayCommand]
    private void LimpiarBusqueda()
    {
        Dni = string.Empty;
        MensajeError = null;
        MensajeEstado = null;
    }

    private bool PuedeCopiarResumen() => HayPaciente && !IsBusy;

    [RelayCommand(CanExecute = nameof(PuedeCopiarResumen))]
    private Task CopiarResumenAsync() => EjecutarAsync(async () =>
    {
        await _clipboard.CopiarAsync(ResumenCopiable);
        MensajeEstado = "Resumen del paciente copiado.";
    }, "Error al copiar el resumen del paciente");

    private bool PuedeCopiarDni() =>
        !IsBusy && !string.IsNullOrWhiteSpace(PacienteActual?.Dni);

    [RelayCommand(CanExecute = nameof(PuedeCopiarDni))]
    private Task CopiarDniAsync() => EjecutarAsync(async () =>
    {
        await _clipboard.CopiarAsync(PacienteActual!.Dni);
        MensajeEstado = "DNI copiado.";
    }, "Error al copiar el DNI");

    private bool PuedeCopiarTelefono() =>
        !IsBusy && !string.IsNullOrWhiteSpace(PacienteActual?.Phone);

    [RelayCommand(CanExecute = nameof(PuedeCopiarTelefono))]
    private Task CopiarTelefonoAsync() => EjecutarAsync(async () =>
    {
        await _clipboard.CopiarAsync(PacienteActual!.Phone);
        MensajeEstado = "Teléfono copiado.";
    }, "Error al copiar el teléfono");

    [RelayCommand]
    private Task AbrirCarpetaDatosAsync() => EjecutarAsync(() =>
    {
        _maintenance.AbrirCarpetaDatos();
        MensajeEstado = "Carpeta de datos abierta.";
        return Task.CompletedTask;
    }, "Error al abrir la carpeta de datos");

    [RelayCommand]
    private Task CrearCopiaSeguridadAsync() => EjecutarAsync(async () =>
    {
        var ruta = await _maintenance.CrearCopiaSeguridadAsync();
        MensajeEstado = $"Copia de seguridad creada: {Path.GetFileName(ruta)}.";
    }, "Error al crear la copia de seguridad");

    [RelayCommand]
    private async Task MostrarPreferenciasAsync()
    {
        MensajeError = null;
        MensajeEstado = null;

        try
        {
            await _dialogs.ShowPreferencesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al mostrar las preferencias");
            MensajeError = "No se pudieron abrir las preferencias. Intente nuevamente.";
        }
    }

    [RelayCommand]
    private Task MostrarAcercaDeAsync() => EjecutarAsync(async () =>
    {
        var version = typeof(MainWindowViewModel).Assembly.GetName().Version?.ToString() ?? "desconocida";
        var mensaje = $"""
            Historia Clínica {version}
            Usuario: {_session.NombreUsuario}
            Base de datos: {_maintenance.DatabasePath}

            Atajos:
            Enter: buscar paciente
            Ctrl+F: enfocar el DNI
            Ctrl+N: nuevo paciente
            Ctrl+E: editar paciente
            Ctrl+Shift+N: nueva ficha
            Supr: borrar la ficha seleccionada
            Esc: limpiar búsqueda y mensajes
            """;

        await _dialogs.ShowInfoAsync("Acerca de", mensaje);
    }, "Error al mostrar la información de la aplicación");

    private bool PuedeCerrarSesion() => !IsBusy;

    [RelayCommand(CanExecute = nameof(PuedeCerrarSesion))]
    private void CerrarSesion()
    {
        _session.Cerrar();
        Dni = string.Empty;
        PacienteActual = null;
        Fichas.Clear();
        FichaSeleccionada = null;
        MensajeError = null;
        MensajeEstado = null;
        _navigator.ShowAuth();
    }

    private bool PuedeBuscar() => !IsBusy;

    [RelayCommand(CanExecute = nameof(PuedeBuscar))]
    private Task BuscarAsync() => EjecutarAsync(async () =>
    {
        var dni = DniValidator.Normalizar(Dni);
        if (!DniValidator.IsValid(dni))
        {
            MensajeError = DniValidator.ErrorMessage;
            return;
        }

        await CargarPacienteAsync(dni, preguntarCreacion: true);
    }, "Error al buscar el paciente");

    private bool PuedeCrearPaciente() => !IsBusy;

    [RelayCommand(CanExecute = nameof(PuedeCrearPaciente))]
    private Task NuevoPacienteAsync() => EjecutarAsync(
        () => AbrirNuevoPacienteAsync(DniValidator.Normalizar(Dni)),
        "Error al crear el paciente");

    private bool PuedeEditarPaciente() => PacienteActual is not null && !IsBusy;

    [RelayCommand(CanExecute = nameof(PuedeEditarPaciente))]
    private Task EditarPacienteAsync() => EjecutarAsync(async () =>
    {
        if (PacienteActual is null)
        {
            return;
        }

        var guardado = await _dialogs.ShowPatientEditorAsync(PacienteActual, esNuevo: false);
        if (guardado is null)
        {
            return;
        }

        Dni = guardado.Dni;
        await CargarPacienteAsync(guardado.Dni, preguntarCreacion: false);
        await CargarDnisAsync();
        MensajeEstado = "Paciente actualizado.";
    }, "Error al editar el paciente");

    private bool PuedeCrearFicha() => PacienteActual is not null && !IsBusy;

    [RelayCommand(CanExecute = nameof(PuedeCrearFicha))]
    private Task NuevaFichaAsync() => EjecutarAsync(async () =>
    {
        if (PacienteActual is null)
        {
            return;
        }

        // La fecha de la ficha nueva la captura el editor al guardar.
        var nueva = new FichaDiaria
        {
            Dni = PacienteActual.Dni,
        };

        var guardada = await _dialogs.ShowFichaEditorAsync(
            nueva,
            esNuevo: true,
            NombreCompleto);
        if (guardada is null)
        {
            return;
        }

        await RecargarFichasAsync(PacienteActual.Dni, guardada.Id);
        MensajeEstado = "Ficha creada.";
    }, "Error al crear la ficha");

    private bool PuedeEditarFicha() => FichaSeleccionada is not null && !IsBusy;

    [RelayCommand(CanExecute = nameof(PuedeEditarFicha))]
    private Task EditarFichaAsync() => EjecutarAsync(async () =>
    {
        if (FichaSeleccionada is null)
        {
            return;
        }

        var guardada = await _dialogs.ShowFichaEditorAsync(
            FichaSeleccionada,
            esNuevo: false,
            NombreCompleto);
        if (guardada is null)
        {
            return;
        }

        await RecargarFichasAsync(guardada.Dni, guardada.Id);
        MensajeEstado = "Ficha actualizada.";
    }, "Error al editar la ficha");

    private bool PuedeBorrarFicha() => FichaSeleccionada is not null && !IsBusy;

    [RelayCommand(CanExecute = nameof(PuedeBorrarFicha))]
    private Task BorrarFichaAsync() => EjecutarAsync(async () =>
    {
        if (FichaSeleccionada is null)
        {
            return;
        }

        var fecha = FechaClinica.FormatDateTime(FichaSeleccionada.Fecha);
        var motivo = string.IsNullOrWhiteSpace(FichaSeleccionada.Motivo)
            ? "Sin motivo"
            : string.Join(" ", FichaSeleccionada.Motivo.Split(
                ['\r', '\n'],
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));

        // Se trunca por elementos de texto para no partir emojis ni surrogates.
        var info = new StringInfo(motivo);
        if (info.LengthInTextElements > 90)
        {
            motivo = $"{info.SubstringByTextElements(0, 87)}...";
        }

        var confirmado = await _dialogs.ConfirmAsync(
            "Borrar ficha",
            $"¿Borrar la ficha del {fecha}?\nMotivo: {motivo}\n\nEsta acción no se puede deshacer.",
            esDestructiva: true);

        if (!confirmado)
        {
            return;
        }

        var borrado = await _fichaService.DeleteAsync(FichaSeleccionada.Id);
        if (!borrado)
        {
            MensajeError = "No se pudo borrar la ficha.";
            return;
        }
        await RecargarFichasAsync(FichaSeleccionada.Dni);
        MensajeEstado = "Ficha borrada.";
    }, "Error al borrar la ficha");

    private async Task AbrirNuevoPacienteAsync(string dniInicial)
    {
        var nuevo = new Paciente { Dni = dniInicial };
        var guardado = await _dialogs.ShowPatientEditorAsync(nuevo, esNuevo: true);
        if (guardado is null)
        {
            return;
        }

        Dni = guardado.Dni;
        await CargarPacienteAsync(guardado.Dni, preguntarCreacion: false);
        await CargarDnisAsync();
        MensajeEstado = "Paciente creado.";
    }

    private async Task CargarPacienteAsync(string dni, bool preguntarCreacion)
    {
        var paciente = await _pacienteService.GetByDniAsync(dni);
        if (paciente is null)
        {
            PacienteActual = null;
            Fichas.Clear();
            FichaSeleccionada = null;

            if (preguntarCreacion)
            {
                var crear = await _dialogs.ConfirmAsync(
                    "Paciente inexistente",
                    $"No existe el paciente con DNI {dni}. ¿Desea crear uno nuevo?");

                if (crear)
                {
                    await AbrirNuevoPacienteAsync(dni);
                }
            }

            return;
        }

        PacienteActual = paciente;
        await RecargarFichasAsync(paciente.Dni);
    }

    private async Task RecargarFichasAsync(string dni, int? seleccionarId = null)
    {
        var fichas = await _fichaService.GetByDniAsync(dni);
        Fichas.ReplaceAll(fichas);

        if (Fichas.Count == 0)
        {
            FichaSeleccionada = null;
            return;
        }

        FichaSeleccionada = seleccionarId is int id
            ? Fichas.FirstOrDefault(f => f.Id == id)
            : Fichas.FirstOrDefault();
    }

    private async Task CargarDnisAsync()
    {
        var dnis = await _pacienteService.GetDnisAsync();
        DnisSugeridos.ReplaceAll(dnis);
    }

    private async Task EjecutarAsync(Func<Task> accion, string contexto)
    {
        if (IsBusy)
        {
            return;
        }

        MensajeError = null;
        MensajeEstado = null;

        try
        {
            IsBusy = true;
            await accion();
        }
        catch (ValidationException ex)
        {
            MensajeError = ex.Message;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Contexto}", contexto);
            MensajeError = "Ocurrió un error inesperado. Intente nuevamente.";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
