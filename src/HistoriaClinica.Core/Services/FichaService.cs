using HistoriaClinica.Core.Models;
using HistoriaClinica.Core.Repositories;
using HistoriaClinica.Core.Validation;
using System.Text.Json;

namespace HistoriaClinica.Core.Services;

public sealed class FichaService
{
    private readonly IFichaRepository _repository;
    private readonly IAuditService? _auditService;

    public FichaService(IFichaRepository repository, IAuditService? auditService = null)
    {
        _repository = repository;
        _auditService = auditService;
    }

    public Task<IReadOnlyList<FichaDiaria>> GetByDniAsync(string dni, CancellationToken cancellationToken = default) =>
        _repository.GetByDniAsync(dni, cancellationToken);

    public Task<FichaDiaria?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        _repository.GetByIdAsync(id, cancellationToken);

    public Task SaveAsync(FichaDiaria ficha, CancellationToken cancellationToken = default) =>
        SaveAsync(ficha, null, cancellationToken);

    public async Task SaveAsync(FichaDiaria ficha, string? username, CancellationToken cancellationToken = default)
    {
        Normalizar(ficha);

        var validation = HistoriaValidators.ValidateFicha(ficha);
        if (!validation.IsValid)
        {
            throw new ValidationException(validation.Errors);
        }

        var esNueva = ficha.Id == 0;

        await _repository.SaveAsync(ficha, cancellationToken);

        if (_auditService is not null)
        {
            var details = JsonSerializer.Serialize(new
            {
                ficha.Id,
                ficha.Dni,
                Fecha = ficha.Fecha.ToString("yyyy-MM-ddTHH:mm:ss"),
                ficha.Cerrada
            });
            if (esNueva)
            {
                await _auditService.RegistrarCreacion(
                    username ?? string.Empty,
                    "FichasDiarias",
                    details,
                    cancellationToken);
            }
            else
            {
                await _auditService.RegistrarActualizacion(
                    username ?? string.Empty,
                    "FichasDiarias",
                    details,
                    cancellationToken);
            }
        }
    }

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default) =>
        DeleteAsync(id, null, cancellationToken);

    public async Task<bool> DeleteAsync(int id, string? username, CancellationToken cancellationToken = default)
    {
        var borrada = await _repository.DeleteAsync(id, cancellationToken);

        if (borrada && _auditService is not null)
        {
            await _auditService.RegistrarEliminacion(
                username ?? string.Empty,
                "FichasDiarias",
                $"{{\"Id\":{id}}}",
                cancellationToken);
        }

        return borrada;
    }

    private static void Normalizar(FichaDiaria ficha)
    {
        ficha.Dni = ficha.Dni?.Trim() ?? string.Empty;
        ficha.Motivo = ficha.Motivo?.Trim() ?? string.Empty;
        ficha.Enfermedad = ficha.Enfermedad?.Trim() ?? string.Empty;
        ficha.Indicaciones = ficha.Indicaciones?.Trim() ?? string.Empty;
        ficha.PresionArterial = string.IsNullOrWhiteSpace(ficha.PresionArterial)
            ? null
            : ficha.PresionArterial.Trim();
    }
}
