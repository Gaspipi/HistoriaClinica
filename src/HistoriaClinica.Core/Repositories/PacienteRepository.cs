using HistoriaClinica.Core.Data;
using HistoriaClinica.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace HistoriaClinica.Core.Repositories;

public sealed class PacienteRepository : IPacienteRepository
{
    private readonly IDbContextFactory<HistoriaContext> _contextFactory;

    public PacienteRepository(IDbContextFactory<HistoriaContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<IReadOnlyList<string>> GetDnisAsync(CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        return await context.Pacientes
            .AsNoTracking()
            .OrderBy(p => p.Dni)
            .Select(p => p.Dni)
            .ToListAsync(cancellationToken);
    }

    public async Task<Paciente?> GetByDniAsync(string dni, CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        return await context.Pacientes
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Dni == dni, cancellationToken);
    }

    public async Task SaveAsync(Paciente paciente, CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        var existing = await context.Pacientes
            .FirstOrDefaultAsync(p => p.Dni == paciente.Dni, cancellationToken);

        if (existing is null)
        {
            context.Pacientes.Add(paciente);
        }
        else
        {
            context.Entry(existing).CurrentValues.SetValues(paciente);
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}
