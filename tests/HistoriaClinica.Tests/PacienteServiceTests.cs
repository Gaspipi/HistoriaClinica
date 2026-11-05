using HistoriaClinica.Core.Models;
using HistoriaClinica.Core.Repositories;
using HistoriaClinica.Core.Services;
using HistoriaClinica.Core.Validation;

namespace HistoriaClinica.Tests;

public sealed class PacienteServiceTests
{
    [Fact]
    public async Task SaveAsync_NormalizaNombreYAntecedentes()
    {
        var repositorio = new PacienteServiceFakeRepository();
        var service = new PacienteService(repositorio);
        var paciente = new Paciente
        {
            Dni = " 12345678 ",
            Nombre = " Ana ",
            Apellido = " Pérez ",
            AntecedentesFamiliares = "  Diabetes  ",
            AntecedentesPersonales = "  Hipertensión  ",
        };

        await service.SaveAsync(paciente);

        Assert.Equal("Diabetes", paciente.AntecedentesFamiliares);
        Assert.Equal("Hipertensión", paciente.AntecedentesPersonales);

        var guardado = await repositorio.GetByDniAsync("12345678");
        Assert.NotNull(guardado);
        Assert.Equal("Ana", guardado.Nombre);
        Assert.Equal("Diabetes", guardado.AntecedentesFamiliares);
        Assert.Equal("Hipertensión", guardado.AntecedentesPersonales);
    }

    [Fact]
    public async Task SaveAsync_AltaRegistraCreacion_EdicionRegistraActualizacion()
    {
        var repositorio = new PacienteServiceFakeRepository();
        var auditoria = new PacienteServiceAuditSpy();
        var service = new PacienteService(repositorio, auditoria);
        var paciente = new Paciente
        {
            Dni = "12345678",
            Nombre = "Ana",
            Apellido = "Pérez",
        };

        await service.SaveAsync(paciente, "ana");
        paciente.Nombre = "Ana María";
        await service.SaveAsync(paciente, "ana");

        Assert.Equal(2, auditoria.Registros.Count);
        Assert.Equal("Create", auditoria.Registros[0].Accion);
        Assert.Equal("Update", auditoria.Registros[1].Accion);
        Assert.All(auditoria.Registros, registro => Assert.Equal("ana", registro.Username));
        Assert.All(auditoria.Registros, registro => Assert.Equal("Pacientes", registro.Entity));

        var details = auditoria.Registros[0].Details;
        Assert.NotNull(details);
        Assert.Contains("12345678", details);
    }

    [Fact]
    public async Task SaveAsync_SinUsername_RegistraUsernameVacio()
    {
        var repositorio = new PacienteServiceFakeRepository();
        var auditoria = new PacienteServiceAuditSpy();
        var service = new PacienteService(repositorio, auditoria);

        await service.SaveAsync(new Paciente
        {
            Dni = "12345678",
            Nombre = "Ana",
            Apellido = "Pérez",
        });

        var registro = Assert.Single(auditoria.Registros);
        Assert.Equal(string.Empty, registro.Username);
        Assert.Equal("Create", registro.Accion);
    }

    [Fact]
    public async Task SaveAsync_ConDniInvalido_LanzaYNoPersisteNiAudita()
    {
        var repositorio = new PacienteServiceFakeRepository();
        var auditoria = new PacienteServiceAuditSpy();
        var service = new PacienteService(repositorio, auditoria);

        var exception = await Assert.ThrowsAsync<ValidationException>(() => service.SaveAsync(
            new Paciente { Dni = "123", Nombre = "Ana", Apellido = "Pérez" },
            "ana"));

        Assert.Contains(DniValidator.ErrorMessage, exception.Errors);
        Assert.Equal(0, repositorio.Guardados);
        Assert.Empty(auditoria.Registros);
    }

    [Fact]
    public async Task SearchAsync_RecortaEspaciosYDelega()
    {
        var repositorio = new PacienteServiceFakeRepository();
        var service = new PacienteService(repositorio);

        await service.SearchAsync("  ana  ");
        await service.SearchAsync(null!);

        Assert.Equal(new[] { "ana", string.Empty }, repositorio.Consultas);
    }

    [Fact]
    public async Task GetDnisYGetByDni_DeleganEnElRepositorio()
    {
        var repositorio = new PacienteServiceFakeRepository();
        repositorio.Agregar(new Paciente
        {
            Dni = "12345678",
            Nombre = "Ana",
            Apellido = "Pérez",
        });
        var service = new PacienteService(repositorio);

        var dnis = await service.GetDnisAsync();
        var paciente = await service.GetByDniAsync("12345678");
        var inexistente = await service.GetByDniAsync("99999999");

        Assert.Equal(new[] { "12345678" }, dnis);
        Assert.NotNull(paciente);
        Assert.Equal("Ana", paciente.Nombre);
        Assert.Null(inexistente);
    }
}

internal sealed class PacienteServiceFakeRepository : IPacienteRepository
{
    private readonly Dictionary<string, Paciente> _pacientes = new(StringComparer.Ordinal);
    private readonly List<string> _consultas = new();

    public int Guardados { get; private set; }

    public IReadOnlyList<string> Consultas => _consultas;

    public void Agregar(Paciente paciente) => _pacientes[paciente.Dni] = paciente;

    public Task<IReadOnlyList<string>> GetDnisAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<string>>(_pacientes.Keys
            .OrderBy(dni => dni, StringComparer.Ordinal)
            .ToList());

    public Task<Paciente?> GetByDniAsync(string dni, CancellationToken cancellationToken = default) =>
        Task.FromResult(_pacientes.TryGetValue(dni, out var paciente) ? paciente : null);

    public Task<IReadOnlyList<Paciente>> SearchAsync(string query, CancellationToken cancellationToken = default)
    {
        _consultas.Add(query);
        return Task.FromResult<IReadOnlyList<Paciente>>(_pacientes.Values
            .Where(paciente =>
                paciente.Nombre.Contains(query, StringComparison.OrdinalIgnoreCase)
                || paciente.Apellido.Contains(query, StringComparison.OrdinalIgnoreCase)
                || paciente.Dni.StartsWith(query, StringComparison.Ordinal))
            .OrderBy(paciente => paciente.Apellido, StringComparer.Ordinal)
            .ThenBy(paciente => paciente.Nombre, StringComparer.Ordinal)
            .ToList());
    }

    public Task SaveAsync(Paciente paciente, CancellationToken cancellationToken = default)
    {
        Guardados++;
        _pacientes[paciente.Dni] = paciente;
        return Task.CompletedTask;
    }
}

internal sealed class PacienteServiceAuditSpy : IAuditService
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
