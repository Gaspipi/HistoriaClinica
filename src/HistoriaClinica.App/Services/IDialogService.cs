using HistoriaClinica.Core.Models;

namespace HistoriaClinica.App.Services;

public interface IDialogService
{
    Task<Paciente?> ShowPatientEditorAsync(Paciente paciente, bool esNuevo);

    Task<FichaDiaria?> ShowFichaEditorAsync(
        FichaDiaria ficha,
        bool esNuevo,
        string nombrePaciente = "");

    Task<bool> ConfirmAsync(string titulo, string mensaje, string aceptar = "Sí", string cancelar = "No");

    Task ShowPreferencesAsync();

    Task ShowErrorAsync(string titulo, string mensaje);

    Task ShowInfoAsync(string titulo, string mensaje);
}
