using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using HistoriaClinica.App.ViewModels;
using HistoriaClinica.App.Views;
using HistoriaClinica.Core.Models;
using Microsoft.Extensions.DependencyInjection;

namespace HistoriaClinica.App.Services;

public sealed class DialogService : IDialogService
{
    private readonly IServiceProvider _services;

    public DialogService(IServiceProvider services)
    {
        _services = services;
    }

    private static Window? Owner =>
        (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;

    public async Task<Paciente?> ShowPatientEditorAsync(Paciente paciente, bool esNuevo)
    {
        var viewModel = ActivatorUtilities.CreateInstance<PatientEditorViewModel>(_services, paciente, esNuevo);
        var window = new PatientEditorWindow(viewModel);
        var owner = Owner;

        if (owner is null)
        {
            window.Show();
            return null;
        }

        var guardado = await window.ShowDialog<bool>(owner);
        return guardado ? viewModel.Resultado : null;
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
            window.Show();
            return null;
        }

        var guardado = await window.ShowDialog<bool>(owner);
        return guardado ? viewModel.Resultado : null;
    }

    public async Task<bool> ConfirmAsync(string titulo, string mensaje, string aceptar = "Sí", string cancelar = "No")
    {
        var owner = Owner;
        if (owner is null)
        {
            return false;
        }

        var dialog = new MessageDialog(titulo, mensaje, mostrarCancelar: true, aceptar, cancelar);
        return await dialog.ShowDialog<bool>(owner);
    }

    public async Task ShowPreferencesAsync()
    {
        var owner = Owner;
        if (owner is null)
        {
            return;
        }

        var viewModel = _services.GetRequiredService<PreferencesViewModel>();
        var window = new PreferencesWindow(viewModel);
        await window.ShowDialog(owner);
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
            return;
        }

        var dialog = new MessageDialog(titulo, mensaje, mostrarCancelar: false);
        await dialog.ShowDialog<bool>(owner);
    }
}
