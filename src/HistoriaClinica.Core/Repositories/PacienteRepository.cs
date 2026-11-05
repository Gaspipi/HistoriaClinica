using System.Globalization;
using System.Text;
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

        var normalizedTerm = RemoveDiacritics(term);

        var pacientes = await context.Pacientes
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return pacientes
            .Where(p => RemoveDiacritics(p.Nombre).Contains(normalizedTerm, StringComparison.OrdinalIgnoreCase)
                     || RemoveDiacritics(p.Apellido).Contains(normalizedTerm, StringComparison.OrdinalIgnoreCase)
                     || p.Dni.StartsWith(normalizedTerm, StringComparison.Ordinal))
            .OrderBy(p => p.Apellido)
            .ThenBy(p => p.Nombre)
            .Select(p => new Paciente
            {
                Dni = p.Dni,
                Nombre = p.Nombre,
                Apellido = p.Apellido,
                FechaNacimiento = p.FechaNacimiento,
                Telefono = p.Telefono,
                ObraSocial = p.ObraSocial,
                NumeroSocio = p.NumeroSocio,
                AntecedentesFamiliares = p.AntecedentesFamiliares,
                AntecedentesPersonales = p.AntecedentesPersonales,
            })
            .ToList();
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

    private static string RemoveDiacritics(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        var normalized = value.Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(normalized.Length);
        foreach (var character in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark)
            {
                builder.Append(character);
            }
        }

        return builder.ToString().Normalize(NormalizationForm.FormC);
    }
}
