using HistoriaClinica.Core.Models;
using HistoriaClinica.Core.Repositories;
using HistoriaClinica.Core.Validation;

namespace HistoriaClinica.Core.Services;

public sealed class FichaService
{
    private readonly IFichaRepository _repository;

    public FichaService(IFichaRepository repository)
    {
        _repository = repository;
    }

    public Task<IReadOnlyList<FichaDiaria>> GetByDniAsync(string dni, CancellationToken cancellationToken = default) =>
        _repository.GetByDniAsync(dni, cancellationToken);

    public Task<FichaDiaria?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        _repository.GetByIdAsync(id, cancellationToken);

    public async Task SaveAsync(FichaDiaria ficha, CancellationToken cancellationToken = default)
    {
        Normalizar(ficha);

        var validation = HistoriaValidators.ValidateFicha(ficha);
        if (!validation.IsValid)
        {
            throw new ValidationException(validation.Errors);
        }

        await _repository.SaveAsync(ficha, cancellationToken);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default) =>
        _repository.DeleteAsync(id, cancellationToken);

    private static void Normalizar(FichaDiaria ficha)
    {
        ficha.Dni = ficha.Dni?.Trim() ?? string.Empty;
        ficha.Motivo = ficha.Motivo?.Trim() ?? string.Empty;
        ficha.Enfermedad = ficha.Enfermedad?.Trim() ?? string.Empty;
        ficha.Indicaciones = ficha.Indicaciones?.Trim() ?? string.Empty;
    }
}
