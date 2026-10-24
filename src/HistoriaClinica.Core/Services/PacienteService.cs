using HistoriaClinica.Core.Models;
using HistoriaClinica.Core.Repositories;
using HistoriaClinica.Core.Validation;

namespace HistoriaClinica.Core.Services;

public sealed class PacienteService
{
    private readonly IPacienteRepository _repository;

    public PacienteService(IPacienteRepository repository)
    {
        _repository = repository;
    }

    public Task<IReadOnlyList<string>> GetDnisAsync(CancellationToken cancellationToken = default) =>
        _repository.GetDnisAsync(cancellationToken);

    public Task<Paciente?> GetByDniAsync(string dni, CancellationToken cancellationToken = default) =>
        _repository.GetByDniAsync(dni, cancellationToken);

    public async Task SaveAsync(Paciente paciente, CancellationToken cancellationToken = default)
    {
        Normalizar(paciente);

        var validation = HistoriaValidators.ValidatePaciente(paciente);
        if (!validation.IsValid)
        {
            throw new ValidationException(validation.Errors);
        }

        await _repository.SaveAsync(paciente, cancellationToken);
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
    }
}
