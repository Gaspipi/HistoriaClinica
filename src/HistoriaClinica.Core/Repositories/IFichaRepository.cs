using HistoriaClinica.Core.Models;

namespace HistoriaClinica.Core.Repositories;

public interface IFichaRepository
{
    Task<IReadOnlyList<FichaDiaria>> GetByDniAsync(string dni, CancellationToken cancellationToken = default);

    Task<FichaDiaria?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task SaveAsync(FichaDiaria ficha, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
