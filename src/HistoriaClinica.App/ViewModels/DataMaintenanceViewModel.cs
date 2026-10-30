using System.Collections.ObjectModel;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HistoriaClinica.App.Services;
using HistoriaClinica.Core.Models;
using HistoriaClinica.Core.Repositories;
using HistoriaClinica.Core.Services;
using Microsoft.Extensions.Logging;

namespace HistoriaClinica.App.ViewModels;

public partial class DataMaintenanceViewModel : ObservableObject
{
    private readonly IDataMaintenanceService _maintenance;
    private readonly IDialogService _dialogs;
    private readonly ILogger<DataMaintenanceViewModel> _logger;
    private readonly IAuditLogRepository? _auditRepository;

    public DataMaintenanceViewModel(
        IDataMaintenanceService maintenance,
        IDialogService dialogs,
        ILogger<DataMaintenanceViewModel> logger,
        IAuditLogRepository? auditRepository = null)
    {
        _maintenance = maintenance;
        _dialogs = dialogs;
        _logger = logger;
        _auditRepository = auditRepository;
        EntidadFiltro = "Todas";
    }

    public ObservableCollection<BackupInfo> Backups { get; } = new();

    public ObservableCollection<AuditLog> AuditLogs { get; } = new();

    public IReadOnlyList<string> EntidadesAudit { get; } = new[] { "Todas", "Pacientes", "FichasDiarias" };

    [ObservableProperty]
    private DateTime? _fechaDesde;

    [ObservableProperty]
    private DateTime? _fechaHasta;

    [ObservableProperty]
    private string? _entidadFiltro;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RestaurarBackupCommand))]
    private BackupInfo? _backupSeleccionado;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HayError))]
    private string? _mensajeError;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HayMensajeEstado))]
    private string? _mensajeEstado;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CargarBackupsCommand))]
    [NotifyCanExecuteChangedFor(nameof(RestaurarBackupCommand))]
    private bool _isBusy;

    public bool HayError => !string.IsNullOrWhiteSpace(MensajeError);

    public bool HayMensajeEstado => !string.IsNullOrWhiteSpace(MensajeEstado);

    public event EventHandler? CloseRequested;

    public Task InicializarAsync() => CargarBackupsCommand.ExecuteAsync(null);

    public Task CargarAsync() => CargarBackupsCommand.ExecuteAsync(null);

    [RelayCommand]
    private void Cerrar() => CloseRequested?.Invoke(this, EventArgs.Empty);

    private bool PuedeCargar() => !IsBusy;

    [RelayCommand(CanExecute = nameof(PuedeCargar))]
    private Task CargarBackupsAsync() => EjecutarAsync(async () =>
    {
        var lista = await _maintenance.ListarBackupsAsync();
        Backups.Clear();
        foreach (var b in lista)
        {
            Backups.Add(b);
        }
    }, "Error al cargar los respaldos");

    private bool PuedeCargarAudit() => !IsBusy && _auditRepository is not null;

    [RelayCommand(CanExecute = nameof(PuedeCargarAudit))]
    private Task CargarAuditLogsAsync() => EjecutarAsync(async () =>
    {
        if (_auditRepository is null)
        {
            return;
        }

        var lista = string.IsNullOrWhiteSpace(EntidadFiltro) || EntidadFiltro == "Todas"
            ? await _auditRepository.GetAllAsync()
            : await _auditRepository.GetByEntityAsync(EntidadFiltro!);

        AuditLogs.Clear();
        foreach (var log in lista.Where(log =>
            (!FechaDesde.HasValue || log.Timestamp.Date >= FechaDesde.Value.Date) &&
            (!FechaHasta.HasValue || log.Timestamp.Date <= FechaHasta.Value.Date)))
        {
            AuditLogs.Add(log);
        }
    }, "Error al cargar el historial de auditoría");

    private bool PuedeRestaurar() => !IsBusy && BackupSeleccionado is not null;

    [RelayCommand(CanExecute = nameof(PuedeRestaurar))]
    private Task RestaurarBackupAsync(BackupInfo? backup) => EjecutarAsync(async () =>
    {
        var seleccionado = backup ?? BackupSeleccionado;
        if (seleccionado is null)
        {
            return;
        }

        var confirmado = await _dialogs.ConfirmAsync(
            "Restaurar respaldo",
            $"¿Restaurar el respaldo \"{seleccionado.FileName}\"?\n\nEsta acción reemplazará la base de datos actual. Se creará una copia de seguridad previa. Esta operación no se puede deshacer.",
            esDestructiva: true);

        if (!confirmado)
        {
            return;
        }

        await _maintenance.RestaurarBackupAsync(seleccionado.FullPath);
        MensajeEstado = "Respaldo restaurado correctamente.";
        await CargarBackupsAsync();
    }, "Error al restaurar el respaldo");

    private bool PuedeRestaurarDesdeArchivo() => !IsBusy;

    [RelayCommand(CanExecute = nameof(PuedeRestaurarDesdeArchivo))]
    private Task RestaurarBackupDesdeArchivoAsync() => EjecutarAsync(async () =>
    {
        var fileTypes = new[]
        {
            new FilePickerFileType("Archivos de backup") { Patterns = new[] { "*.db", "*.db.bak", "*.bak" } },
            new FilePickerFileType("Base de datos SQLite") { Patterns = new[] { "*.db" } },
        };

        var ruta = await _dialogs.ShowOpenFileDialogAsync("Seleccionar respaldo", fileTypes);
        if (string.IsNullOrWhiteSpace(ruta))
        {
            return;
        }

        var fileName = Path.GetFileName(ruta);
        var confirmado = await _dialogs.ConfirmAsync(
            "Restaurar respaldo",
            $"¿Restaurar el respaldo \"{fileName}\"?\n\nEsta acción reemplazará la base de datos actual. Se creará una copia de seguridad previa. Esta operación no se puede deshacer.",
            esDestructiva: true);

        if (!confirmado)
        {
            return;
        }

        await _maintenance.RestaurarBackupAsync(ruta);
        MensajeEstado = $"Respaldo restaurado correctamente desde {fileName}.";
        await CargarBackupsAsync();
    }, "Error al restaurar el respaldo");

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
            _logger.LogError(ex, "{Contexto}", contexto);
            MensajeError = ex.Message;
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "{Contexto}", contexto);
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
