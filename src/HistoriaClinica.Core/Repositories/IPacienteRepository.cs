using HistoriaClinica.Core.Models;

namespace HistoriaClinica.Core.Repositories;

public interface IPacienteRepository
{
    Task<IReadOnlyList<string>> GetDnisAsync(CancellationToken cancellationToken = default);

    Task<Paciente?> GetByDniAsync(string dni, CancellationToken cancellationToken = default);

    Task SaveAsync(Paciente paciente, CancellationToken cancellationToken = default);
}
