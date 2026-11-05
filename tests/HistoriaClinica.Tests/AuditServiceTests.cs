using HistoriaClinica.Core.Models;
using HistoriaClinica.Core.Repositories;
using HistoriaClinica.Core.Services;

namespace HistoriaClinica.Tests;

public sealed class AuditServiceTests
{
    [Fact]
    public async Task RegistrarCreacion_AsignaAccionEntidadYTimestamp()
    {
        var repositorio = new AuditServiceCapturingRepository();
        var service = new AuditService(repositorio);
        var antes = DateTime.Now;

        var log = await service.RegistrarCreacion("ana", "Pacientes", "{\"Dni\":\"12345678\"}");

        var despues = DateTime.Now;
        Assert.Equal("Create", log.Action);
        Assert.Equal("Pacientes", log.Entity);
        Assert.Equal("ana", log.Username);
        Assert.Equal("{\"Dni\":\"12345678\"}", log.Details);
        Assert.InRange(log.Timestamp, antes, despues);
        Assert.Same(log, Assert.Single(repositorio.Guardados));
    }

    [Fact]
    public async Task RegistrarActualizacion_AsignaAccionUpdate()
    {
        var repositorio = new AuditServiceCapturingRepository();
        var service = new AuditService(repositorio);

        var log = await service.RegistrarActualizacion("ana", "Pacientes", "detalle");

        Assert.Equal("Update", log.Action);
        Assert.Equal("detalle", log.Details);
        Assert.Same(log, Assert.Single(repositorio.Guardados));
    }

    [Fact]
    public async Task RegistrarEliminacion_AsignaAccionDelete()
    {
        var repositorio = new AuditServiceCapturingRepository();
        var service = new AuditService(repositorio);

        var log = await service.RegistrarEliminacion("ana", "FichasDiarias", "{\"Id\":5}");

        Assert.Equal("Delete", log.Action);
        Assert.Equal("{\"Id\":5}", log.Details);
        Assert.Same(log, Assert.Single(repositorio.Guardados));
    }

    [Fact]
    public async Task Registrar_UsernameNulo_GuardaVacio()
    {
        var repositorio = new AuditServiceCapturingRepository();
        var service = new AuditService(repositorio);

        // Se fuerza null para verificar que el servicio lo traduce a cadena vacía.
        var log = await service.RegistrarCreacion(null!, "Pacientes");
        Assert.Equal(string.Empty, log.Username);
        Assert.Equal(string.Empty, Assert.Single(repositorio.Guardados).Username);
    }

    [Fact]
    public async Task Registrar_RepositorioFalla_Propaga()
    {
        var service = new AuditService(new AuditServiceFailingRepository());

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.RegistrarCreacion("ana", "Pacientes", "detalle"));
    }
}

internal sealed class AuditServiceCapturingRepository : IAuditLogRepository
{
    public List<AuditLog> Guardados { get; } = new();

    public Task<IReadOnlyList<AuditLog>> GetAllAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<AuditLog>>(Guardados.ToList());

    public Task<IReadOnlyList<AuditLog>> GetByEntityAsync(
        string entity,
        CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<AuditLog>>(Guardados
            .Where(log => log.Entity == entity)
            .ToList());

    public Task SaveAsync(AuditLog auditLog, CancellationToken cancellationToken = default)
    {
        Guardados.Add(auditLog);
        return Task.CompletedTask;
    }
}

internal sealed class AuditServiceFailingRepository : IAuditLogRepository
{
    public Task<IReadOnlyList<AuditLog>> GetAllAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<AuditLog>>(Array.Empty<AuditLog>());

    public Task<IReadOnlyList<AuditLog>> GetByEntityAsync(
        string entity,
        CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<AuditLog>>(Array.Empty<AuditLog>());

    public Task SaveAsync(AuditLog auditLog, CancellationToken cancellationToken = default) =>
        throw new InvalidOperationException("Falla simulada del repositorio de auditoría.");
}
