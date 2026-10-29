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

    public async Task<IReadOnlyList<Paciente>> SearchAsync(string query, CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        var term = (query ?? string.Empty).Trim();
        if (term.Length == 0)
        {
            return Array.Empty<Paciente>();
        }

        return await context.Pacientes
            .AsNoTracking()
            .Where(p => p.FirstName.Contains(term) || p.LastName.Contains(term))
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .Select(p => new Paciente
            {
                Dni = p.Dni,
                FirstName = p.FirstName,
                LastName = p.LastName,
                DOB = p.DOB,
                Phone = p.Phone,
                ObraSocial = p.ObraSocial,
                NroSocio = p.NroSocio,
                AntecFam = p.AntecFam,
                AntecPers = p.AntecPers,
                Medicacion = p.Medicacion,
                Alergias = p.Alergias,
                ProblemasActivos = p.ProblemasActivos,
            })
            .ToListAsync(cancellationToken);
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
