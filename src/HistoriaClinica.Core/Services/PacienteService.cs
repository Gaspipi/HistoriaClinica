using HistoriaClinica.Core.Models;
using HistoriaClinica.Core.Repositories;
using HistoriaClinica.Core.Validation;
using System.Text.Json;

namespace HistoriaClinica.Core.Services;

public sealed class PacienteService
{
    private readonly IPacienteRepository _repository;
    private readonly IAuditService? _auditService;

    public PacienteService(IPacienteRepository repository, IAuditService? auditService = null)
    {
        _repository = repository;
        _auditService = auditService;
    }

    public Task<IReadOnlyList<string>> GetDnisAsync(CancellationToken cancellationToken = default) =>
        _repository.GetDnisAsync(cancellationToken);

    public Task<Paciente?> GetByDniAsync(string dni, CancellationToken cancellationToken = default) =>
        _repository.GetByDniAsync(dni, cancellationToken);

    public Task<IReadOnlyList<Paciente>> SearchAsync(string query, CancellationToken cancellationToken = default) =>
        _repository.SearchAsync((query ?? string.Empty).Trim(), cancellationToken);

    public Task SaveAsync(Paciente paciente, CancellationToken cancellationToken = default) =>
        SaveAsync(paciente, null, cancellationToken);

    public async Task SaveAsync(Paciente paciente, string? username, CancellationToken cancellationToken = default)
    {
        Normalizar(paciente);

        var validation = HistoriaValidators.ValidatePaciente(paciente);
        if (!validation.IsValid)
        {
            throw new ValidationException(validation.Errors);
        }

        var existente = await _repository.GetByDniAsync(paciente.Dni, cancellationToken);

        await _repository.SaveAsync(paciente, cancellationToken);

        if (_auditService is not null)
        {
            var details = JsonSerializer.Serialize(new
            {
                paciente.Dni,
                paciente.Nombre,
                paciente.Apellido,
                paciente.FechaNacimiento,
                paciente.Telefono,
                paciente.ObraSocial,
                paciente.NumeroSocio
            });
            if (existente is null)
            {
                await _auditService.RegistrarCreacion(
                    username ?? string.Empty,
                    "Pacientes",
                    details,
                    cancellationToken);
            }
            else
            {
                await _auditService.RegistrarActualizacion(
                    username ?? string.Empty,
                    "Pacientes",
                    details,
                    cancellationToken);
            }
        }
    }

    private static void Normalizar(Paciente paciente)
    {
        paciente.Dni = paciente.Dni?.Trim() ?? string.Empty;
        paciente.Nombre = paciente.Nombre?.Trim() ?? string.Empty;
        paciente.Apellido = paciente.Apellido?.Trim() ?? string.Empty;
        paciente.Telefono = paciente.Telefono?.Trim() ?? string.Empty;
        paciente.ObraSocial = paciente.ObraSocial?.Trim() ?? string.Empty;
        paciente.NumeroSocio = paciente.NumeroSocio?.Trim() ?? string.Empty;
        paciente.AntecedentesFamiliares = paciente.AntecedentesFamiliares?.Trim() ?? string.Empty;
        paciente.AntecedentesPersonales = paciente.AntecedentesPersonales?.Trim() ?? string.Empty;
    }
}
