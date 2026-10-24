using HistoriaClinica.Core.Data;
using HistoriaClinica.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace HistoriaClinica.Core.Repositories;

public sealed class FichaRepository : IFichaRepository
{
    private readonly IDbContextFactory<HistoriaContext> _contextFactory;

    public FichaRepository(IDbContextFactory<HistoriaContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<IReadOnlyList<FichaDiaria>> GetByDniAsync(string dni, CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        return await context.FichasDiarias
            .AsNoTracking()
            .Where(f => f.Dni == dni)
            .OrderByDescending(f => f.Fecha)
            .ThenByDescending(f => f.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<FichaDiaria?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        return await context.FichasDiarias
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.Id == id, cancellationToken);
    }

    public async Task SaveAsync(FichaDiaria ficha, CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        if (ficha.Id == 0)
        {
            context.FichasDiarias.Add(ficha);
        }
        else
        {
            var existing = await context.FichasDiarias
                .FirstOrDefaultAsync(f => f.Id == ficha.Id, cancellationToken);

            if (existing is null)
            {
                throw new InvalidOperationException($"No se encontró la ficha {ficha.Id}.");
            }

            context.Entry(existing).CurrentValues.SetValues(ficha);
        }

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        var existing = await context.FichasDiarias
            .FirstOrDefaultAsync(f => f.Id == id, cancellationToken);

        if (existing is null)
        {
            return false;
        }

        context.FichasDiarias.Remove(existing);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
