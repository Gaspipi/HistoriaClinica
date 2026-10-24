using HistoriaClinica.Core.Models;
using HistoriaClinica.Core.Repositories;
using HistoriaClinica.Core.Security;
using Microsoft.EntityFrameworkCore;

namespace HistoriaClinica.Core.Services;

public sealed class AuthService
{
    public const int MinUsernameLength = 3;
    public const int MaxUsernameLength = 32;
    public const int MinPasswordLength = 8;
    public const int MaxPasswordLength = 128;

    private readonly IUsuarioRepository _repository;

    public AuthService(IUsuarioRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> HasUsersAsync(CancellationToken cancellationToken = default) =>
        await _repository.CountAsync(cancellationToken) > 0;

    public async Task<Usuario> RegisterFirstUserAsync(
        string username,
        string password,
        CancellationToken cancellationToken = default)
    {
        var usernameLimpio = (username ?? string.Empty).Trim();
        ValidarRegistro(usernameLimpio, password);

        if (await _repository.CountAsync(cancellationToken) > 0)
        {
            throw new ValidationException(["Ya existe un usuario registrado."]);
        }

        var passwordLimpia = password ?? string.Empty;
        var hash = await Task.Run(
            () => PasswordHasher.HashPassword(passwordLimpia),
            cancellationToken);

        var usuario = new Usuario
        {
            Username = usernameLimpio,
            NormalizedUsername = NormalizarUsuario(usernameLimpio),
            PasswordHash = hash.Hash,
            PasswordSalt = hash.Salt,
            PasswordIterations = hash.Iterations,
            CreatedAt = DateTime.UtcNow,
        };

        try
        {
            await _repository.AddAsync(usuario, cancellationToken);
        }
        catch (DbUpdateException)
        {
            throw new ValidationException(["Ya existe un usuario registrado."]);
        }

        return usuario;
    }

    public async Task<Usuario?> LoginAsync(
        string username,
        string password,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(password))
        {
            return null;
        }

        var usuario = await _repository.GetByNormalizedUsernameAsync(
            NormalizarUsuario(username),
            cancellationToken);

        if (usuario is null)
        {
            return null;
        }

        var valido = await Task.Run(
            () => PasswordHasher.VerifyPassword(
                password,
                usuario.PasswordHash,
                usuario.PasswordSalt,
                usuario.PasswordIterations),
            cancellationToken);

        return valido ? usuario : null;
    }

    public static string NormalizarUsuario(string username) =>
        (username ?? string.Empty).Trim().ToUpperInvariant();

    public static string? ObtenerErrorUsuario(string? username)
    {
        var valor = (username ?? string.Empty).Trim();

        if (valor.Length < MinUsernameLength || valor.Length > MaxUsernameLength)
        {
            return $"El usuario debe tener entre {MinUsernameLength} y {MaxUsernameLength} caracteres.";
        }

        if (!valor.All(CaracterValido))
        {
            return "El usuario solo puede contener letras, números, punto, guion y guion bajo.";
        }

        return null;
    }

    public static string? ObtenerErrorPassword(string? password)
    {
        if (string.IsNullOrEmpty(password) || password.Length < MinPasswordLength)
        {
            return $"La contraseña debe tener al menos {MinPasswordLength} caracteres.";
        }

        if (password.Length > MaxPasswordLength)
        {
            return $"La contraseña no puede superar los {MaxPasswordLength} caracteres.";
        }

        return null;
    }

    private static void ValidarRegistro(string username, string? password)
    {
        var errores = new List<string>();
        var errorUsuario = ObtenerErrorUsuario(username);
        var errorPassword = ObtenerErrorPassword(password);

        if (errorUsuario is not null)
        {
            errores.Add(errorUsuario);
        }

        if (errorPassword is not null)
        {
            errores.Add(errorPassword);
        }

        if (errores.Count > 0)
        {
            throw new ValidationException(errores);
        }
    }

    private static bool CaracterValido(char caracter) =>
        char.IsAsciiLetterOrDigit(caracter) || caracter is '.' or '_' or '-';
}
