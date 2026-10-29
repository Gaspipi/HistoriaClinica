using HistoriaClinica.Core.Models;
using HistoriaClinica.Core.Repositories;
using HistoriaClinica.Core.Services;

namespace HistoriaClinica.Tests;

public sealed class FichaServiceTests
{
    [Fact]
    public async Task SaveAsync_NuevaRegistraCreacion()
    {
        var repositorio = new FichaServiceFakeRepository();
        var auditoria = new FichaServiceAuditSpy();
        var service = new FichaService(repositorio, auditoria);
        var ficha = CrearFichaValida();

        await service.SaveAsync(ficha, "ana");

        Assert.Equal(1, ficha.Id);
        var registro = Assert.Single(auditoria.Registros);
        Assert.Equal("Create", registro.Accion);
        Assert.Equal("FichasDiarias", registro.Entity);
        Assert.Equal("ana", registro.Username);
    }

    [Fact]
    public async Task SaveAsync_ExistenteRegistraActualizacion()
    {
        var repositorio = new FichaServiceFakeRepository();
        var auditoria = new FichaServiceAuditSpy();
        var service = new FichaService(repositorio, auditoria);
        var ficha = CrearFichaValida();
        ficha.Id = 7;

        await service.SaveAsync(ficha, "ana");

        var registro = Assert.Single(auditoria.Registros);
        Assert.Equal("Update", registro.Accion);
        Assert.Equal("ana", registro.Username);
    }

    [Theory]
    [InlineData("   ", null)]
    [InlineData(" 120/80 ", "120/80")]
    [InlineData(null, null)]
    public async Task SaveAsync_NormalizaPresionArterial(string? presion, string? esperado)
    {
        var repositorio = new FichaServiceFakeRepository();
        var service = new FichaService(repositorio);
        var ficha = CrearFichaValida();
        ficha.PresionArterial = presion;

        await service.SaveAsync(ficha);

        Assert.Equal(esperado, ficha.PresionArterial);
        Assert.Equal(1, repositorio.Guardados);
    }

    [Fact]
    public async Task SaveAsync_NormalizaCamposDeTexto()
    {
        var repositorio = new FichaServiceFakeRepository();
        var service = new FichaService(repositorio);
        var ficha = new FichaDiaria
        {
            Dni = " 12345678 ",
            Fecha = new DateTime(2026, 10, 28, 10, 0, 0),
            Motivo = "  Control  ",
            Enfermedad = "  Sin particularidades  ",
            Indicaciones = "  Reposo  ",
            PresionArterial = " 120/80 ",
        };

        await service.SaveAsync(ficha);

        Assert.Equal("12345678", ficha.Dni);
        Assert.Equal("Control", ficha.Motivo);
        Assert.Equal("Sin particularidades", ficha.Enfermedad);
        Assert.Equal("Reposo", ficha.Indicaciones);
        Assert.Equal("120/80", ficha.PresionArterial);
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("120")]
    [InlineData("120/80/90")]
    public async Task SaveAsync_PresionInvalida_LanzaYNoPersisteNiAudita(string presion)
    {
        var repositorio = new FichaServiceFakeRepository();
        var auditoria = new FichaServiceAuditSpy();
        var service = new FichaService(repositorio, auditoria);
        var ficha = CrearFichaValida();
        ficha.PresionArterial = presion;

        await Assert.ThrowsAsync<ValidationException>(() => service.SaveAsync(ficha, "ana"));

        Assert.Equal(0, repositorio.Guardados);
        Assert.Empty(auditoria.Registros);
    }

    [Fact]
    public async Task DeleteAsync_ExistenteRegistraEliminacion()
    {
        var repositorio = new FichaServiceFakeRepository();
        var auditoria = new FichaServiceAuditSpy();
        var service = new FichaService(repositorio, auditoria);
        var ficha = repositorio.Agregar(CrearFichaValida());

        var borrada = await service.DeleteAsync(ficha.Id, "ana");

        Assert.True(borrada);
        var registro = Assert.Single(auditoria.Registros);
        Assert.Equal("Delete", registro.Accion);
        Assert.Equal("ana", registro.Username);
        Assert.NotNull(registro.Details);
        Assert.Contains($"{ficha.Id}", registro.Details);
    }

    [Fact]
    public async Task DeleteAsync_Inexistente_NoAudita()
    {
        var repositorio = new FichaServiceFakeRepository();
        var auditoria = new FichaServiceAuditSpy();
        var service = new FichaService(repositorio, auditoria);

        var borrada = await service.DeleteAsync(99, "ana");

        Assert.False(borrada);
        Assert.Empty(auditoria.Registros);
    }

    [Fact]
    public async Task SaveAsync_SinServicioDeAuditoria_NoLanza()
    {
        var repositorio = new FichaServiceFakeRepository();
        var service = new FichaService(repositorio);

        await service.SaveAsync(CrearFichaValida(), "ana");

        Assert.Equal(1, repositorio.Guardados);
    }

    private static FichaDiaria CrearFichaValida() => new()
    {
        Dni = "12345678",
        Fecha = new DateTime(2026, 10, 28, 10, 0, 0),
        Motivo = "Control",
        Enfermedad = "Sin particularidades",
        Indicaciones = "Reposo",
    };
}

internal sealed class FichaServiceFakeRepository : IFichaRepository
{
    private readonly List<FichaDiaria> _fichas = new();
    private int _siguienteId = 1;

    public int Guardados { get; private set; }

    public FichaDiaria Agregar(FichaDiaria ficha)
    {
        if (ficha.Id == 0)
        {
            ficha.Id = _siguienteId++;
        }

        _fichas.Add(ficha);
        return ficha;
    }

    public Task<IReadOnlyList<FichaDiaria>> GetByDniAsync(string dni, CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<FichaDiaria>>(_fichas
            .Where(ficha => ficha.Dni == dni)
            .OrderByDescending(ficha => ficha.Fecha)
            .ThenByDescending(ficha => ficha.Id)
            .ToList());

    public Task<FichaDiaria?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        Task.FromResult(_fichas.FirstOrDefault(ficha => ficha.Id == id));

    public Task SaveAsync(FichaDiaria ficha, CancellationToken cancellationToken = default)
    {
        Guardados++;

        if (ficha.Id == 0)
        {
            ficha.Id = _siguienteId++;
        }

        var indice = _fichas.FindIndex(actual => actual.Id == ficha.Id);
        if (indice >= 0)
        {
            _fichas[indice] = ficha;
        }
        else
        {
            _fichas.Add(ficha);
        }

        return Task.CompletedTask;
    }

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default) =>
        Task.FromResult(_fichas.RemoveAll(ficha => ficha.Id == id) > 0);
}

internal sealed class FichaServiceAuditSpy : IAuditService
{
    public List<(string Accion, string Username, string Entity, string? Details)> Registros { get; } = new();

    public Task<AuditLog> RegistrarCreacion(
        string username,
        string entity,
        string? details = null,
        CancellationToken cancellationToken = default) =>
        Registrar("Create", username, entity, details);

    public Task<AuditLog> RegistrarActualizacion(
        string username,
        string entity,
        string? details = null,
        CancellationToken cancellationToken = default) =>
        Registrar("Update", username, entity, details);

    public Task<AuditLog> RegistrarEliminacion(
        string username,
        string entity,
        string? details = null,
        CancellationToken cancellationToken = default) =>
        Registrar("Delete", username, entity, details);

    private Task<AuditLog> Registrar(string accion, string username, string entity, string? details)
    {
        Registros.Add((accion, username, entity, details));
        return Task.FromResult(new AuditLog
        {
            Username = username,
            Entity = entity,
            Action = accion,
            Details = details,
        });
    }
}
