using HistoriaClinica.Core.Models;

namespace HistoriaClinica.Core.Repositories;

public interface IUsuarioRepository
{
    Task<int> CountAsync(CancellationToken cancellationToken = default);

    Task<Usuario?> GetByNormalizedUsernameAsync(
        string normalizedUsername,
        CancellationToken cancellationToken = default);

    Task AddAsync(Usuario usuario, CancellationToken cancellationToken = default);
}
