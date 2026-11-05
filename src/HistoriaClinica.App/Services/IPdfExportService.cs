using HistoriaClinica.Core.Models;

namespace HistoriaClinica.App.Services;

public interface IPdfExportService
{
    byte[] GenerarPdf(Paciente paciente, FichaDiaria? ficha);

    Task<byte[]> GenerarPdfAsync(
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

    Task ExportarPdfAsync(
        Paciente paciente,
        FichaDiaria? ficha,
        string rutaArchivo,
        CancellationToken cancellationToken = default);

    Task<string?> ExportarPdfAsync(
        Paciente paciente,
        FichaDiaria? ficha,
        CancellationToken cancellationToken = default);
}
