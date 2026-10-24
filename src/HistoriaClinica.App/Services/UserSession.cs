using HistoriaClinica.Core.Models;

namespace HistoriaClinica.App.Services;

public sealed record UsuarioSesion(int Id, string Username);

public sealed class UserSession
{
    public UsuarioSesion? UsuarioActual { get; private set; }

    public bool EstaAutenticada => UsuarioActual is not null;

    public string NombreUsuario => UsuarioActual?.Username ?? string.Empty;

    public string InicialesUsuario
    {
        get
        {
            var nombre = NombreUsuario;
            var iniciales = new string(nombre
                .Where(char.IsLetterOrDigit)
                .Take(2)
                .Select(char.ToUpperInvariant)
                .ToArray());

            return string.IsNullOrEmpty(iniciales) ? "?" : iniciales;
        }
    }

    public void Iniciar(Usuario usuario) =>
        UsuarioActual = new UsuarioSesion(usuario.Id, usuario.Username);

    public void Cerrar() => UsuarioActual = null;
}
