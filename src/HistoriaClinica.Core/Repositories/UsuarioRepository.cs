using HistoriaClinica.Core.Data;
using HistoriaClinica.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace HistoriaClinica.Core.Repositories;

public sealed class UsuarioRepository : IUsuarioRepository
{
    private readonly IDbContextFactory<HistoriaContext> _contextFactory;

    public UsuarioRepository(IDbContextFactory<HistoriaContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        return await context.Usuarios.CountAsync(cancellationToken);
    }

    public async Task<Usuario?> GetByNormalizedUsernameAsync(
        string normalizedUsername,
        CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        return await context.Usuarios
            .AsNoTracking()
            .FirstOrDefaultAsync(
                usuario => usuario.NormalizedUsername == normalizedUsername,
                cancellationToken);
    }

    public async Task AddAsync(Usuario usuario, CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        context.Usuarios.Add(usuario);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task ActualizarPasswordAsync(Usuario usuario, CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        var entry = context.Usuarios.Attach(usuario);
        entry.Property(u => u.PasswordHash).IsModified = true;
        entry.Property(u => u.PasswordSalt).IsModified = true;
        entry.Property(u => u.PasswordIterations).IsModified = true;
        await context.SaveChangesAsync(cancellationToken);
    }
}
