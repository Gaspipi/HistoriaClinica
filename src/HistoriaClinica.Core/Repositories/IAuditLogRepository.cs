using HistoriaClinica.Core.Models;

namespace HistoriaClinica.Core.Repositories;

public interface IAuditLogRepository
{
    Task<IReadOnlyList<AuditLog>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AuditLog>> GetByEntityAsync(string entity, CancellationToken cancellationToken = default);

    Task SaveAsync(AuditLog auditLog, CancellationToken cancellationToken = default);
}
