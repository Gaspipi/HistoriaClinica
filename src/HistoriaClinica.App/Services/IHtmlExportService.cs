using HistoriaClinica.Core.Models;

namespace HistoriaClinica.App.Services;

public interface IHtmlExportService
{
    string GenerarHtml(Paciente paciente, FichaDiaria? ficha);

    Task<string> GenerarHtmlAsync(
        Paciente paciente,
        FichaDiaria? ficha,
        CancellationToken cancellationToken = default);

    Task ExportarAsync(
        Paciente paciente,
        FichaDiaria? ficha,
        string rutaArchivo,
        CancellationToken cancellationToken = default);

    Task<string?> ExportarAsync(
        Paciente paciente,
        FichaDiaria? ficha,
        CancellationToken cancellationToken = default);

    Task ExportarHtmlAsync(
        Paciente paciente,
        FichaDiaria? ficha,
        string rutaArchivo,
        CancellationToken cancellationToken = default);

    Task<string?> ExportarHtmlAsync(
        Paciente paciente,
        FichaDiaria? ficha,
        CancellationToken cancellationToken = default);
}
