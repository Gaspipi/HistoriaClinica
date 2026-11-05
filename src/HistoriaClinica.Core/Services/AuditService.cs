using HistoriaClinica.Core.Models;
using HistoriaClinica.Core.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace HistoriaClinica.Core.Services;

public sealed class AuditService : IAuditService
{
    private readonly IAuditLogRepository _repository;

    public AuditService(IAuditLogRepository repository, ILogger<AuditService>? logger = null)
    {
        _repository = repository;
    }

    public async Task<AuditLog> RegistrarCreacion(
        string username,
        string entity,
        string? details = null,
        CancellationToken cancellationToken = default)
    {
        var log = new AuditLog
        {
            Username = username ?? string.Empty,
            Timestamp = DateTime.Now,
            Entity = entity ?? string.Empty,
            Action = "Create",
            Details = details,
        };

        await _repository.SaveAsync(log, cancellationToken);
        return log;
    }

    public async Task<AuditLog> RegistrarActualizacion(
        string username,
        string entity,
        string? details = null,
        CancellationToken cancellationToken = default)
    {
        var log = new AuditLog
        {
            Username = username ?? string.Empty,
            Timestamp = DateTime.Now,
            Entity = entity ?? string.Empty,
            Action = "Update",
            Details = details,
        };

        await _repository.SaveAsync(log, cancellationToken);
        return log;
    }

    public async Task<AuditLog> RegistrarEliminacion(
        string username,
        string entity,
        string? details = null,
        CancellationToken cancellationToken = default)
    {
        var log = new AuditLog
        {
            Username = username ?? string.Empty,
            Timestamp = DateTime.Now,
            Entity = entity ?? string.Empty,
            Action = "Delete",
            Details = details,
        };

        await _repository.SaveAsync(log, cancellationToken);
        return log;
    }
}
