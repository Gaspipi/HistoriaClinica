using HistoriaClinica.Core.Repositories;
using HistoriaClinica.Core.Security;
using HistoriaClinica.Core.Services;

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

    [Theory]
    [InlineData("ab", "clave-segura-123")]
    [InlineData("usuario inválido", "clave-segura-123")]
    [InlineData("usuario", "corta")]
    public async Task Registro_Invalido_EsRechazado(string username, string password)
    {
        await Assert.ThrowsAsync<ValidationException>(
            () => _auth.RegisterFirstUserAsync(username, password));
    }

    public void Dispose() => _database.Dispose();
}
