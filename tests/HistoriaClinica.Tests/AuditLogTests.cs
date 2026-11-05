using HistoriaClinica.Core.Data;
using HistoriaClinica.Core.Models;
using HistoriaClinica.Core.Repositories;
using HistoriaClinica.Core.Services;
using Microsoft.Data.Sqlite;

namespace HistoriaClinica.Tests;

public sealed class AuditLogTests : IDisposable
{
    private readonly TestDatabase _database;
    private readonly AuditLogRepository _repository;
    private readonly AuditService _service;

    public AuditLogTests()
    {
        _database = new TestDatabase();
        _repository = new AuditLogRepository(_database.Factory);
        _service = new AuditService(_repository);
    }

    public void Dispose() => _database.Dispose();

    [Fact]
    public async Task SaveAsync_PersisteLog()
    {
        await _repository.SaveAsync(new AuditLog
        {
            Username = "ana",
            Timestamp = DateTime.Now,
            Entity = "Pacientes",
            Action = "Create",
            Details = "{\"Dni\":\"12345678\"}",
        });

        var logs = await _repository.GetAllAsync();
        Assert.Single(logs);
        Assert.Equal("Pacientes", logs[0].Entity);
        Assert.Equal("Create", logs[0].Action);
    }

    [Fact]
    public async Task GetByEntityAsync_FiltraPorEntidad()
    {
        await _repository.SaveAsync(new AuditLog { Username = "ana", Timestamp = DateTime.Now, Entity = "Pacientes", Action = "Create" });
        await _repository.SaveAsync(new AuditLog { Username = "ana", Timestamp = DateTime.Now, Entity = "FichasDiarias", Action = "Create" });

        var logs = await _repository.GetByEntityAsync("FichasDiarias");
        Assert.Single(logs);
        Assert.Equal("FichasDiarias", logs[0].Entity);
    }

    [Fact]
    public async Task AuditService_RegistraCreacionActualizacionEliminacion()
    {
        await _service.RegistrarCreacion("ana", "Pacientes", "detalle");
        await _service.RegistrarActualizacion("ana", "Pacientes", "detalle");
        await _service.RegistrarEliminacion("ana", "FichasDiarias", "detalle");

        var logs = await _repository.GetAllAsync();
        Assert.Equal(3, logs.Count);
    }

    [Fact]
    public async Task PacienteService_RegistraAuditoriaAlGuardar()
    {
        var pacientes = new PacienteRepository(_database.Factory);
        var auditRepo = new AuditLogRepository(_database.Factory);
        var auditService = new AuditService(auditRepo);
        var service = new PacienteService(pacientes, auditService);

        await service.SaveAsync(new Paciente
        {
            Dni = "12345678",
            Nombre = "Ana",
            Apellido = "Pérez",
        });

        var logs = await auditRepo.GetAllAsync();
        Assert.Single(logs);
        Assert.Equal("Pacientes", logs[0].Entity);
        Assert.Equal("Create", logs[0].Action);
    }
}
