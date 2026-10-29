using HistoriaClinica.Core.Models;

namespace HistoriaClinica.Core.Repositories;

public interface IPlantillaRepository
{
    Task<IReadOnlyList<Plantilla>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Plantilla?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task SaveAsync(Plantilla plantilla, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Plantilla plantilla, CancellationToken cancellationToken = default);
}
