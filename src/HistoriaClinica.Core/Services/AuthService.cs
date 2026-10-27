using HistoriaClinica.Core.Models;
using HistoriaClinica.Core.Repositories;
using HistoriaClinica.Core.Security;
using HistoriaClinica.Core.Validation;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Collections.Concurrent;

namespace HistoriaClinica.Core.Services;

public sealed class AuthService
{
    public const int MinPasswordLength = 8;
    public const int MaxPasswordLength = 128;

    private const int MaxFailedAttempts = 5;
    private static readonly TimeSpan LockoutWindow = TimeSpan.FromMinutes(1);
    private static readonly ConcurrentDictionary<string, (int Count, DateTime First)> _failedAttempts = new();

    // Códigos de SQLite para la violación del índice único de usuarios:
    // SQLITE_CONSTRAINT (19) y su código extendido SQLITE_CONSTRAINT_UNIQUE (2067).
    private const int SqliteConstraintErrorCode = 19;
    private const int SqliteConstraintUniqueExtendedCode = 2067;

    // Credenciales ficticias para ejecutar PBKDF2 cuando el usuario no existe,
    // de modo que el tiempo de respuesta no delate su inexistencia.
    private static readonly string HashDummy = Convert.ToBase64String(new byte[PasswordHasher.HashSize]);
    private static readonly string SaltDummy = Convert.ToBase64String(new byte[PasswordHasher.SaltSize]);

    private readonly IUsuarioRepository _repository;
    private readonly ILogger<AuthService> _logger;

    public AuthService(IUsuarioRepository repository, ILogger<AuthService>? logger = null)
    {
        _repository = repository;
        _logger = logger ?? NullLogger<AuthService>.Instance;
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
        catch (DbUpdateException ex) when (EsUsuarioDuplicado(ex))
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

        if (IsRateLimited(username))
        {
            return null;
        }

        var usuario = await _repository.GetByNormalizedUsernameAsync(
            NormalizarUsuario(username),
            cancellationToken);

        if (usuario is null)
        {
            // Verificamos contra credenciales ficticias y descartamos el resultado:
            // así un usuario inexistente tarda lo mismo que uno real.
            await Task.Run(
                () => PasswordHasher.VerifyPassword(
                    password,
                    HashDummy,
                    SaltDummy,
                    PasswordHasher.Iterations),
                cancellationToken);

            return null;
        }

        var valido = await Task.Run(
            () => PasswordHasher.VerifyPassword(
                password,
                usuario.PasswordHash,
                usuario.PasswordSalt,
                usuario.PasswordIterations),
            cancellationToken);

        if (valido && usuario.PasswordIterations < PasswordHasher.Iterations)
        {
            await RegenerarHashAsync(usuario, password, cancellationToken);
        }

        if (valido)
        {
            _failedAttempts.TryRemove(NormalizarUsuario(username), out _);
        }
        else
        {
            RecordFailure(username);
        }

        return valido ? usuario : null;
    }

    // Tras un login exitoso con un hash generado con menos iteraciones que las
    // actuales, lo regeneramos y persistimos. Si la escritura falla, el login
    // igual prospera: solo se registra el problema y se continúa.
    private async Task RegenerarHashAsync(
        Usuario usuario,
        string password,
        CancellationToken cancellationToken)
    {
        try
        {
            var hash = await Task.Run(
                () => PasswordHasher.HashPassword(password),
                cancellationToken);

            usuario.PasswordHash = hash.Hash;
            usuario.PasswordSalt = hash.Salt;
            usuario.PasswordIterations = hash.Iterations;
            await _repository.ActualizarPasswordAsync(usuario, cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogWarning(
                ex,
                "No se pudo regenerar el hash del usuario {Username} con las iteraciones actuales; el login continúa con el hash anterior.",
                usuario.Username);
        }
    }

    // Solo traducimos a "duplicado" cuando SQLite rechazó por el índice único del
    // usuario; cualquier otro error de base se propaga para no confundir al usuario.
    private static bool EsUsuarioDuplicado(DbUpdateException exception) =>
        exception.InnerException is SqliteException sqlite
        && (sqlite.SqliteExtendedErrorCode == SqliteConstraintUniqueExtendedCode
            || (sqlite.SqliteErrorCode == SqliteConstraintErrorCode
                && sqlite.Message.Contains(
                    "Usuarios.NormalizedUsername",
                    StringComparison.OrdinalIgnoreCase)));

    private static bool IsRateLimited(string username)
    {
        var key = NormalizarUsuario(username);
        if (_failedAttempts.TryGetValue(key, out var info))
        {
            if (DateTime.UtcNow - info.First > LockoutWindow)
            {
                _failedAttempts.TryRemove(key, out _);
                return false;
            }
            return info.Count >= MaxFailedAttempts;
        }
        return false;
    }

    private static void RecordFailure(string username)
    {
        var key = NormalizarUsuario(username);
        _failedAttempts.AddOrUpdate(key,
            _ => (1, DateTime.UtcNow),
            (_, info) => (DateTime.UtcNow - info.First > LockoutWindow) ? (1, DateTime.UtcNow) : (info.Count + 1, info.First));
    }

    public static string NormalizarUsuario(string username) =>
        (username ?? string.Empty).Trim().ToUpperInvariant();

    public static string? ObtenerErrorUsuario(string? username)
    {
        var valor = (username ?? string.Empty).Trim();

        if (valor.Length < UsuarioLengths.Min || valor.Length > UsuarioLengths.Max)
        {
            return $"El usuario debe tener entre {UsuarioLengths.Min} y {UsuarioLengths.Max} caracteres.";
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
