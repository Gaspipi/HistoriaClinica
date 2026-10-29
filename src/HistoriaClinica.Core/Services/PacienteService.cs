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
                paciente.FirstName,
                paciente.LastName,
                paciente.DOB,
                paciente.Phone,
                paciente.ObraSocial,
                paciente.NroSocio
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
        paciente.FirstName = paciente.FirstName?.Trim() ?? string.Empty;
        paciente.LastName = paciente.LastName?.Trim() ?? string.Empty;
        paciente.Phone = paciente.Phone?.Trim() ?? string.Empty;
        paciente.ObraSocial = paciente.ObraSocial?.Trim() ?? string.Empty;
        paciente.NroSocio = paciente.NroSocio?.Trim() ?? string.Empty;
        paciente.AntecFam = paciente.AntecFam?.Trim() ?? string.Empty;
        paciente.AntecPers = paciente.AntecPers?.Trim() ?? string.Empty;
        paciente.Medicacion = paciente.Medicacion?.Trim() ?? string.Empty;
        paciente.Alergias = paciente.Alergias?.Trim() ?? string.Empty;
        paciente.ProblemasActivos = paciente.ProblemasActivos?.Trim() ?? string.Empty;
    }
}
