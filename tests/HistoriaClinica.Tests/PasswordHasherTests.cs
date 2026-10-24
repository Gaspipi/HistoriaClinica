using HistoriaClinica.Core.Security;

namespace HistoriaClinica.Tests;

public sealed class PasswordHasherTests
{
    [Fact]
    public void HashPassword_VerificaLaContrasenaCorrecta()
    {
        var resultado = PasswordHasher.HashPassword("clave-segura-123");

        Assert.True(PasswordHasher.VerifyPassword(
            "clave-segura-123",
            resultado.Hash,
            resultado.Salt,
            resultado.Iterations));
    }

    [Fact]
    public void HashPassword_RechazaUnaContrasenaIncorrecta()
    {
        var resultado = PasswordHasher.HashPassword("clave-segura-123");

        Assert.False(PasswordHasher.VerifyPassword(
            "otra-clave",
            resultado.Hash,
            resultado.Salt,
            resultado.Iterations));
    }

    [Fact]
    public void HashPassword_UsaSalDistintaEnCadaHash()
    {
        var primero = PasswordHasher.HashPassword("clave-segura-123");
        var segundo = PasswordHasher.HashPassword("clave-segura-123");

        Assert.NotEqual(primero.Salt, segundo.Salt);
        Assert.NotEqual(primero.Hash, segundo.Hash);
    }

    [Fact]
    public void VerifyPassword_DevuelveFalseConDatosInvalidos()
    {
        Assert.False(PasswordHasher.VerifyPassword("clave", "no-es-base64", "tampoco", 1));
        Assert.False(PasswordHasher.VerifyPassword("clave", string.Empty, string.Empty, 0));
    }
}
