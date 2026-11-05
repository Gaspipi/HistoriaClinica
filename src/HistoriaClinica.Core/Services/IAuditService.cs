using HistoriaClinica.Core.Models;
using HistoriaClinica.Core.Repositories;

namespace HistoriaClinica.Core.Services;

public interface IAuditService
{
    Task<AuditLog> RegistrarCreacion(
        string username,
        string entity,
        string? details = null,
        CancellationToken cancellationToken = default);

    Task<AuditLog> RegistrarActualizacion(
        string username,
        string entity,
        string? details = null,
        CancellationToken cancellationToken = default);

    Task<AuditLog> RegistrarEliminacion(
        string username,
        string entity,
        string? details = null,
        CancellationToken cancellationToken = default);
}
