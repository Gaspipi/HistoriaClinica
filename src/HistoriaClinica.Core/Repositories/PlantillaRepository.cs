using HistoriaClinica.Core.Data;
using HistoriaClinica.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace HistoriaClinica.Core.Repositories;

public sealed class PlantillaRepository : IPlantillaRepository
{
    private readonly IDbContextFactory<HistoriaContext> _contextFactory;

    public PlantillaRepository(IDbContextFactory<HistoriaContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<IReadOnlyList<Plantilla>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        return await context.Plantillas
            .AsNoTracking()
            .OrderBy(p => p.Nombre)
            .ToListAsync(cancellationToken);
    }

    public async Task<Plantilla?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        return await context.Plantillas
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task SaveAsync(Plantilla plantilla, CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        if (plantilla.Id == 0)
        {
            context.Plantillas.Add(plantilla);
        }
        else
        {
            var existing = await context.Plantillas
                .FirstOrDefaultAsync(p => p.Id == plantilla.Id, cancellationToken);

            if (existing is null)
            {
                throw new InvalidOperationException($"No se encontró la plantilla {plantilla.Id}.");
            }

            context.Entry(existing).CurrentValues.SetValues(plantilla);
        }

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        var plantilla = await context.Plantillas
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (plantilla is null)
        {
            return false;
        }

        context.Plantillas.Remove(plantilla);
        var borradas = await context.SaveChangesAsync(cancellationToken);
        return borradas > 0;
    }

    public Task<bool> DeleteAsync(Plantilla plantilla, CancellationToken cancellationToken = default) =>
        DeleteAsync(plantilla.Id, cancellationToken);
}
