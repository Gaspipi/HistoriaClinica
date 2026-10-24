using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HistoriaClinica.App.Services;
using HistoriaClinica.Core.Services;
using Microsoft.Extensions.Logging;

namespace HistoriaClinica.App.ViewModels;

public abstract partial class EditorViewModelBase : ObservableObject
{
    private readonly IDialogService _dialogs;
    private readonly ILogger _logger;

    protected EditorViewModelBase(IDialogService dialogs, ILogger logger)
    {
        _dialogs = dialogs;
        _logger = logger;
    }

    public event EventHandler<bool>? CloseRequested;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HayError))]
    private string? _mensajeError;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(GuardarCommand))]
    private bool _isBusy;

    public abstract string Titulo { get; }

    public abstract bool HayCambios { get; }

    public bool HayError => !string.IsNullOrWhiteSpace(MensajeError);

    public bool CierreConfirmado { get; private set; }

    protected abstract Task<bool> GuardarCoreAsync();

    protected virtual bool Validar() => true;

    protected void Cerrar(bool guardado)
    {
        CierreConfirmado = true;
        CloseRequested?.Invoke(this, guardado);
    }

    public Task SolicitarCancelacionAsync() => CancelarAsync();

    private bool PuedeGuardar() => !IsBusy;

    [RelayCommand(CanExecute = nameof(PuedeGuardar))]
    private async Task GuardarAsync()
    {
        if (IsBusy)
        {
            return;
        }

        MensajeError = null;

        if (!Validar())
        {
            return;
        }

        try
        {
            IsBusy = true;
            if (await GuardarCoreAsync())
            {
                Cerrar(true);
            }
        }
        catch (ValidationException ex)
        {
            MensajeError = ex.Message;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al guardar");
            MensajeError = "No se pudo guardar. Intente nuevamente.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task CancelarAsync()
    {
        if (!HayCambios
            || await _dialogs.ConfirmAsync("Cancelar", "¿Está seguro de que desea cancelar? Se perderán los cambios."))
        {
            Cerrar(false);
        }
    }
}
