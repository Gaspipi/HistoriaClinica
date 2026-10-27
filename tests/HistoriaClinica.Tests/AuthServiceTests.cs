using System.Security.Cryptography;
using HistoriaClinica.Core.Models;
using HistoriaClinica.Core.Repositories;
using HistoriaClinica.Core.Security;
using HistoriaClinica.Core.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace HistoriaClinica.Tests;

public sealed class AuthServiceTests : IDisposable
{
    private readonly TestDatabase _database;
    private readonly AuthService _auth;

    public AuthServiceTests()
    {
        _database = new TestDatabase();
        _auth = new AuthService(new UsuarioRepository(_database.Factory));
    }

    [Fact]
    public async Task HasUsersAsync_EsFalseEnUnaBaseNueva()
    {
        Assert.False(await _auth.HasUsersAsync());
    }

    [Fact]
    public async Task RegistrarPrimerUsuario_GuardaCredencialesYNormalizaElUsuario()
    {
        var usuario = await _auth.RegisterFirstUserAsync("  Ana.01  ", "clave-segura-123");

        Assert.True(usuario.Id > 0);
        Assert.Equal("Ana.01", usuario.Username);
        Assert.Equal("ANA.01", usuario.NormalizedUsername);
        Assert.Equal(PasswordHasher.Iterations, usuario.PasswordIterations);
        Assert.True(PasswordHasher.VerifyPassword(
            "clave-segura-123",
            usuario.PasswordHash,
            usuario.PasswordSalt,
            usuario.PasswordIterations));
        Assert.True(await _auth.HasUsersAsync());
    }

    [Fact]
    public async Task RegistrarSegundoUsuario_EsRechazado()
    {
        await _auth.RegisterFirstUserAsync("ana", "clave-segura-123");

        var error = await Assert.ThrowsAsync<ValidationException>(
            () => _auth.RegisterFirstUserAsync("otro", "otra-clave-123"));

        Assert.Contains("Ya existe un usuario", error.Message);
    }

    [Fact]
    public async Task Login_EsCaseInsensitiveYRechazaCredencialesInvalidas()
    {
        await _auth.RegisterFirstUserAsync("Ana.01", "clave-segura-123");

        var usuario = await _auth.LoginAsync("ana.01", "clave-segura-123");

        Assert.NotNull(usuario);
        Assert.Equal("Ana.01", usuario.Username);
        Assert.Null(await _auth.LoginAsync("ana.01", "incorrecta"));
        Assert.Null(await _auth.LoginAsync("inexistente", "clave-segura-123"));
    }

    [Fact]
    public async Task RegistrarElMismoUsuarioDosVeces_EsRechazadoComoDuplicado()
    {
        await _auth.RegisterFirstUserAsync("ana", "clave-segura-123");

        var error = await Assert.ThrowsAsync<ValidationException>(
            () => _auth.RegisterFirstUserAsync("ana", "otra-clave-123"));

        Assert.Contains("Ya existe un usuario registrado", error.Message);
    }

    [Fact]
    public async Task Login_UsuarioInexistente_RetornaNullSinLanzar()
    {
        var usuario = await _auth.LoginAsync("inexistente", "clave-segura-123");

        Assert.Null(usuario);
    }

    [Fact]
    public async Task Login_ConHashDeIteracionesAnteriores_LoRegeneraYLaClaveSigueVerificando()
    {
        const string clave = "clave-segura-123";
        await _auth.RegisterFirstUserAsync("ana", clave);
        DegradarHashAIteraciones(100_000, clave);

        var usuario = await _auth.LoginAsync("ana", clave);

        Assert.NotNull(usuario);
        Assert.Equal(PasswordHasher.Iterations, usuario.PasswordIterations);
        Assert.True(PasswordHasher.VerifyPassword(
            clave,
            usuario.PasswordHash,
            usuario.PasswordSalt,
            usuario.PasswordIterations));

        using (var context = _database.Factory.CreateDbContext())
        {
            var persistido = await context.Usuarios.SingleAsync();
            Assert.Equal(PasswordHasher.Iterations, persistido.PasswordIterations);
            Assert.True(PasswordHasher.VerifyPassword(
                clave,
                persistido.PasswordHash,
                persistido.PasswordSalt,
                persistido.PasswordIterations));
        }
    }

    [Fact]
    public async Task Registro_ConViolacionDeIndiceUnico_TraduceADuplicado()
    {
        var auth = CrearAuthConRepositorioQueFalla(new DbUpdateException(
            "No se pudo guardar el usuario.",
            new SqliteException(
                "SQLite Error 19: 'UNIQUE constraint failed: Usuarios.NormalizedUsername'.",
                19,
                2067)));

        var error = await Assert.ThrowsAsync<ValidationException>(
            () => auth.RegisterFirstUserAsync("ana", "clave-segura-123"));

        Assert.Contains("Ya existe un usuario registrado", error.Message);
    }

    [Fact]
    public async Task Registro_ConOtroErrorDeBase_NoSeInformaComoDuplicado()
    {
        var auth = CrearAuthConRepositorioQueFalla(new DbUpdateException(
            "No se pudo guardar el usuario.",
            new SqliteException("SQLite Error 10: 'disk I/O error'.", 10, 10)));

        await Assert.ThrowsAsync<DbUpdateException>(
            () => auth.RegisterFirstUserAsync("ana", "clave-segura-123"));
    }

    [Theory]
    [InlineData("ab", "clave-segura-123")]
    [InlineData("usuario inválido", "clave-segura-123")]
    [InlineData("usuario", "corta")]
    public async Task Registro_Invalido_EsRechazado(string username, string password)
    {
        await Assert.ThrowsAsync<ValidationException>(
            () => _auth.RegisterFirstUserAsync(username, password));
    }

    private AuthService CrearAuthConRepositorioQueFalla(DbUpdateException error) =>
        new(new RepositorioQueFalla(error));

    // Reemplaza el hash guardado por uno con menos iteraciones, simulando
    // credenciales creadas con una versión anterior del hasher.
    private void DegradarHashAIteraciones(int iteraciones, string clave)
    {
        using var context = _database.Factory.CreateDbContext();
        var usuario = context.Usuarios.Single();
        var salt = RandomNumberGenerator.GetBytes(PasswordHasher.SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            clave,
            salt,
            iteraciones,
            HashAlgorithmName.SHA256,
            PasswordHasher.HashSize);
        usuario.PasswordHash = Convert.ToBase64String(hash);
        usuario.PasswordSalt = Convert.ToBase64String(salt);
        usuario.PasswordIterations = iteraciones;
        context.SaveChanges();
    }

    private sealed class RepositorioQueFalla : IUsuarioRepository
    {
        private readonly DbUpdateException _error;

        public RepositorioQueFalla(DbUpdateException error) => _error = error;

        public Task<int> CountAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(0);

        public Task<Usuario?> GetByNormalizedUsernameAsync(
            string normalizedUsername,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<Usuario?>(null);

        public Task AddAsync(Usuario usuario, CancellationToken cancellationToken = default) =>
            Task.FromException(_error);

        public Task ActualizarPasswordAsync(Usuario usuario, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;
    }

    public void Dispose() => _database.Dispose();
}
