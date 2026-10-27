using System.IO;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace HistoriaClinica.Core.Services;

/// <summary>
/// Implementación del servicio de backups y restauración de SQLite.
/// </summary>
public sealed class SqliteBackupService : ISqliteBackupService
{
    private const string BackupFolderName = "Backups";
    private const string BackupFileExtension = ".db.bak";
    private const string PreRestorePrefix = "pre-restore";
    private static readonly string ExpectedBackupSuffix = BackupFileExtension;

    private readonly ILogger<SqliteBackupService> _logger;

    public SqliteBackupService(ILogger<SqliteBackupService>? logger = null)
    {
        _logger = logger ?? NullLogger<SqliteBackupService>.Instance;
    }

    /// <inheritdoc />
    public async Task<string> CreateBackupAsync(
        string databasePath,
        string? backupPrefix = null,
        CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(databasePath);

        var backupDirectory = Path.Combine(Path.GetDirectoryName(databasePath) ?? ".", BackupFolderName);
        Directory.CreateDirectory(backupDirectory);

        var prefix = string.IsNullOrEmpty(backupPrefix) ? "Historia" : backupPrefix;
        var timestamp = DateTime.Now.ToString("yyyyMMdd-HHmmss", System.Globalization.CultureInfo.InvariantCulture);
        var fileName = $"{prefix}_{timestamp}{ExpectedBackupSuffix}";
        var backupPath = Path.Combine(backupDirectory, fileName);

        SqliteConnection.ClearAllPools();

        // Usar API de backup de SQLite para mayor fiabilidad
        await Task.Run(
            () =>
            {
                ct.ThrowIfCancellationRequested();

                using var source = new SqliteConnection(new SqliteConnectionStringBuilder
                {
                    DataSource = databasePath,
                    Mode = SqliteOpenMode.ReadOnly,
                    Pooling = false,
                }.ToString());
                using var destination = new SqliteConnection(new SqliteConnectionStringBuilder
                {
                    DataSource = backupPath,
                    Mode = SqliteOpenMode.ReadWriteCreate,
                    Pooling = false,
                }.ToString());
                source.Open();
                destination.Open();
                source.BackupDatabase(destination);
            },
            ct);

        File.SetCreationTimeUtc(backupPath, DateTime.UtcNow);

        _logger.LogInformation("Backup creado en {BackupPath}", backupPath);
        return backupPath;
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<BackupInfo>> ListBackupsAsync(
        string backupDirectory,
        CancellationToken ct = default)
    {
        if (!Directory.Exists(backupDirectory))
        {
            return Task.FromResult<IReadOnlyList<BackupInfo>>(Array.Empty<BackupInfo>());
        }

        var files = Directory.GetFiles(backupDirectory, "*" + ExpectedBackupSuffix)
            .Select(f => new FileInfo(f))
            .OrderByDescending(f => f.CreationTimeUtc)
            .Select(f => new BackupInfo(
                f.FullName,
                f.Name,
                f.CreationTimeUtc,
                f.Length))
            .ToList();

        return Task.FromResult<IReadOnlyList<BackupInfo>>(files);
    }

    /// <inheritdoc />
    public async Task RotateBackupsAsync(
        string backupDirectory,
        int maxCount = 10,
        CancellationToken ct = default)
    {
        if (!Directory.Exists(backupDirectory) || maxCount <= 0)
        {
            return;
        }

        var files = Directory.GetFiles(backupDirectory, "*" + ExpectedBackupSuffix)
            .Select(f => new FileInfo(f))
            .OrderBy(f => f.CreationTimeUtc)
            .ToList();

        var toDelete = files.Skip(maxCount).ToList();

        foreach (var file in toDelete)
        {
            await Task.Run(() => file.Delete(), ct);
            _logger.LogInformation("Backup eliminado por rotación: {FileName}", file.Name);
        }
    }

    /// <inheritdoc />
    public Task<string> CreatePreRestoreBackupAsync(
        string databasePath,
        string backupDirectory,
        CancellationToken ct = default) =>
        CreateBackupAsync(databasePath, PreRestorePrefix, ct);

    /// <inheritdoc />
    public async Task<RestoreResult> RestoreBackupAsync(
        string backupPath,
        string databasePath,
        CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(backupPath);
        ArgumentException.ThrowIfNullOrEmpty(databasePath);

        if (!File.Exists(backupPath))
        {
            return new RestoreResult(false, "El archivo de backup no existe.", null);
        }

        // Verificar integridad del backup antes de restaurar.
        var backupIntegro = await VerifyIntegrityAsync(backupPath, ct);
        if (!backupIntegro)
        {
            return new RestoreResult(false, "El backup falló la verificación de integridad.", null);
        }

        // Limpiar conexiones abiertas a la base de datos actual.
        SqliteConnection.ClearAllPools();

        // Crear backup previo de la base actual si existe.
        string? preRestoreBackupPath = null;
        if (File.Exists(databasePath))
        {
            preRestoreBackupPath = await CreateBackupAsync(databasePath, PreRestorePrefix, ct);
        }

        // Reemplazar el archivo de base de datos.
        try
        {
            await Task.Run(() =>
            {
                File.Delete(databasePath);
                File.Copy(backupPath, databasePath, overwrite: true);
            }, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al restaurar la base de datos desde {BackupPath}", backupPath);
            return new RestoreResult(false, $"Error al restaurar: {ex.Message}", preRestoreBackupPath);
        }

        // Verificar integridad de la base restaurada.
        var restoredIntegro = await VerifyIntegrityAsync(databasePath, ct);
        if (!restoredIntegro)
        {
            return new RestoreResult(
                false,
                "La base de datos restaurada falló la verificación de integridad.",
                preRestoreBackupPath);
        }

        _logger.LogInformation("Base de datos restaurada desde {BackupPath}", backupPath);
        return new RestoreResult(true, "Base de datos restaurada correctamente.", preRestoreBackupPath);
    }

    /// <inheritdoc />
    public async Task<bool> VerifyIntegrityAsync(string databasePath, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(databasePath);

        if (!File.Exists(databasePath))
        {
            return false;
        }

        try
        {
            return await Task.Run(async () =>
            {
                await using var connection = new SqliteConnection($"Data Source={databasePath}");
                await connection.OpenAsync(ct);

                await using var command = connection.CreateCommand();
                command.CommandText = "PRAGMA quick_check";
                var result = await command.ExecuteScalarAsync(ct) as string;

                return string.Equals(result, "ok", StringComparison.OrdinalIgnoreCase);
            }, ct);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error al verificar integridad de {DatabasePath}", databasePath);
            return false;
        }
    }
}