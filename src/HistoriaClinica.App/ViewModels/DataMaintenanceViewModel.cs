using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HistoriaClinica.App.Services;
using HistoriaClinica.Core.Services;
using Microsoft.Extensions.Logging;

namespace HistoriaClinica.App.ViewModels;

public partial class DataMaintenanceViewModel : ObservableObject
{
    private readonly IDataMaintenanceService _maintenance;
    private readonly IDialogService _dialogs;
    private readonly ILogger<DataMaintenanceViewModel> _logger;

    public DataMaintenanceViewModel(
        IDataMaintenanceService maintenance,
        IDialogService dialogs,
        ILogger<DataMaintenanceViewModel> logger)
    {
        _maintenance = maintenance;
        _dialogs = dialogs;
        _logger = logger;
    }

    public ObservableCollection<BackupInfo> Backups { get; } = new();

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
