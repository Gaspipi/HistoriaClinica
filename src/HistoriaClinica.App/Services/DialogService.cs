using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Avalonia.Controls.ApplicationLifetimes;
using HistoriaClinica.App.ViewModels;
using HistoriaClinica.App.Views;
using HistoriaClinica.Core.Models;
using Microsoft.Extensions.DependencyInjection;

namespace HistoriaClinica.App.Services;

public sealed class DialogService : IDialogService
{
    private readonly IServiceProvider _services;

    // Ventanas hijas abiertas en modo diálogo; sus confirms deben ser modales a ellas.
    private readonly Stack<Window> _owners = new();

    public DialogService(IServiceProvider services)
    {
        _services = services;
    }

    private Window? Owner
    {
        get
        {
            if (_owners.Count > 0)
            {
                return _owners.Peek();
            }

            return (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
        }
    }

    public async Task<Paciente?> ShowPatientEditorAsync(Paciente paciente, bool esNuevo)
    {
        var viewModel = ActivatorUtilities.CreateInstance<PatientEditorViewModel>(_services, paciente, esNuevo);
        var window = new PatientEditorWindow(viewModel);
        var owner = Owner;

        if (owner is null)
        {
            throw new InvalidOperationException("No hay ventana propietaria disponible para mostrar el diálogo.");
        }

        _owners.Push(window);
        try
        {
            var guardado = await window.ShowDialog<bool>(owner);
            return guardado ? viewModel.Resultado : null;
        }
        finally
        {
            _owners.Pop();
        }
    }

    public async Task<FichaDiaria?> ShowFichaEditorAsync(
        FichaDiaria ficha,
        bool esNuevo,
        string nombrePaciente = "")
    {
        var viewModel = ActivatorUtilities.CreateInstance<FichaEditorViewModel>(
            _services,
            ficha,
            esNuevo,
            nombrePaciente);
        var window = new FichaEditorWindow(viewModel);
        var owner = Owner;

        if (owner is null)
        {
            throw new InvalidOperationException("No hay ventana propietaria disponible para mostrar el diálogo.");
        }

        _owners.Push(window);
        try
        {
            var guardado = await window.ShowDialog<bool>(owner);
            return guardado ? viewModel.Resultado : null;
        }
        finally
        {
            _owners.Pop();
        }
    }

    public async Task<bool> ConfirmAsync(
        string titulo,
        string mensaje,
        string aceptar = "Sí",
        string cancelar = "No",
        bool esDestructiva = false)
    {
        var owner = Owner;
        if (owner is null)
        {
            throw new InvalidOperationException("No hay ventana propietaria disponible para mostrar el diálogo.");
        }

        var dialog = new MessageDialog(titulo, mensaje, mostrarCancelar: true, aceptar, cancelar, esDestructiva);
        return await dialog.ShowDialog<bool>(owner);
    }

    public async Task ShowPreferencesAsync()
    {
        var owner = Owner;
        if (owner is null)
        {
            throw new InvalidOperationException("No hay ventana propietaria disponible para mostrar el diálogo.");
        }

        var viewModel = _services.GetRequiredService<PreferencesViewModel>();
        var window = new PreferencesWindow(viewModel);
        await window.ShowDialog(owner);
    }

    public async Task ShowDataMaintenanceAsync()
    {
        var owner = Owner;
        if (owner is null)
        {
            throw new InvalidOperationException("No hay ventana propietaria disponible para mostrar el diálogo.");
        }

        var viewModel = _services.GetRequiredService<DataMaintenanceViewModel>();
        var window = new DataMaintenanceWindow(viewModel);
        _owners.Push(window);
        try
        {
            await window.ShowDialog(owner);
        }
        finally
        {
            _owners.Pop();
        }
    }

    public Task ShowErrorAsync(string titulo, string mensaje) =>
        ShowMessageAsync(titulo, mensaje);

    public Task ShowInfoAsync(string titulo, string mensaje) =>
        ShowMessageAsync(titulo, mensaje);

    private async Task ShowMessageAsync(string titulo, string mensaje)
    {
        var owner = Owner;
        if (owner is null)
        {
            throw new InvalidOperationException("No hay ventana propietaria disponible para mostrar el diálogo.");
        }

        var dialog = new MessageDialog(titulo, mensaje, mostrarCancelar: false);
        await dialog.ShowDialog<bool>(owner);
    }

    public async Task<string?> ShowOpenFileDialogAsync(
        string titulo,
        IReadOnlyList<FilePickerFileType>? fileTypes = null,
        CancellationToken cancellationToken = default)
    {
        var owner = Owner;
        if (owner is null)
        {
            throw new InvalidOperationException("No hay ventana propietaria disponible para mostrar el diálogo.");
        }

        var topLevel = TopLevel.GetTopLevel(owner);
        if (topLevel?.StorageProvider is null)
        {
            return null;
        }

        var result = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = titulo,
            AllowMultiple = false,
            FileTypeFilter = fileTypes,
        });

        return result?.FirstOrDefault()?.Path.LocalPath;
    }

    public async Task<string?> ShowSaveFileDialogAsync(
        string titulo,
        string nombreSugerido,
        string extension = ".html",
        CancellationToken cancellationToken = default)
    {
        var owner = Owner;
        if (owner is null)
        {
            throw new InvalidOperationException("No hay ventana propietaria disponible para mostrar el diálogo.");
        }

        var topLevel = TopLevel.GetTopLevel(owner);
        if (topLevel?.StorageProvider is null)
        {
            return null;
        }

        var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = titulo,
            SuggestedFileName = nombreSugerido,
            DefaultExtension = extension.TrimStart('.'),
            FileTypeChoices = new[]
            {
                new FilePickerFileType("HTML") { Patterns = new[] { "*.html", "*.htm" } },
            },
        });

        return file?.Path.LocalPath;
    }
}
