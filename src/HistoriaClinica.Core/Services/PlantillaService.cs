using HistoriaClinica.Core.Models;
using HistoriaClinica.Core.Repositories;
using HistoriaClinica.Core.Validation;

namespace HistoriaClinica.Core.Services;

public sealed class PlantillaService
{
    private readonly IPlantillaRepository _repository;
    private readonly IPacienteRepository _pacienteRepository;

    public PlantillaService(IPlantillaRepository repository, IPacienteRepository pacienteRepository)
    {
        _repository = repository;
        _pacienteRepository = pacienteRepository;
    }

    public Task<IReadOnlyList<Plantilla>> GetAllAsync(CancellationToken cancellationToken = default) =>
        _repository.GetAllAsync(cancellationToken);

    public Task<Plantilla?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        _repository.GetByIdAsync(id, cancellationToken);

    public async Task SaveAsync(Plantilla plantilla, CancellationToken cancellationToken = default)
    {
        Normalizar(plantilla);

        var validation = HistoriaValidators.ValidatePlantilla(plantilla);
        if (!validation.IsValid)
        {
            throw new ValidationException(validation.Errors);
        }

        if (!string.IsNullOrWhiteSpace(plantilla.CreadorDni))
        {
            var creador = await _pacienteRepository.GetByDniAsync(plantilla.CreadorDni, cancellationToken);
            if (creador is null)
            {
                throw new ValidationException(new[] { "El DNI del creador no corresponde a un paciente registrado." });
            }
        }

        await _repository.SaveAsync(plantilla, cancellationToken);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default) =>
        _repository.DeleteAsync(id, cancellationToken);

    public Task<bool> DeleteAsync(Plantilla plantilla, CancellationToken cancellationToken = default) =>
        _repository.DeleteAsync(plantilla, cancellationToken);

    private static void Normalizar(Plantilla plantilla)
    {
        plantilla.Nombre = plantilla.Nombre?.Trim() ?? string.Empty;
        plantilla.Motivo = plantilla.Motivo?.Trim() ?? string.Empty;
        plantilla.Enfermedad = plantilla.Enfermedad?.Trim() ?? string.Empty;
        plantilla.Indicaciones = plantilla.Indicaciones?.Trim() ?? string.Empty;
        plantilla.CreadorDni = string.IsNullOrWhiteSpace(plantilla.CreadorDni)
            ? null
            : plantilla.CreadorDni.Trim();
    }
}
