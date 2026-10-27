using System.IO;

namespace HistoriaClinica.Core.Services;

/// <summary>
/// Servicio de backups y restauración de la base de datos SQLite.
/// </summary>
public interface ISqliteBackupService
{
    /// <summary>
    /// Crea un backup de la base de datos en la carpeta Backups/.
    /// </summary>
    Task<string> CreateBackupAsync(string databasePath, string? backupPrefix = null, CancellationToken ct = default);

    /// <summary>
    /// Lista los backups disponibles en el directorio dado.
    /// </summary>
    Task<IReadOnlyList<BackupInfo>> ListBackupsAsync(string backupDirectory, CancellationToken ct = default);

    /// <summary>
    /// Elimina backups antiguos hasta dejar un máximo de archivos.
    /// </summary>
    Task RotateBackupsAsync(string backupDirectory, int maxCount = 10, CancellationToken ct = default);

    /// <summary>
    /// Crea un backup con prefijo "pre-restore" antes de restaurar una base.
    /// </summary>
    Task<string> CreatePreRestoreBackupAsync(string databasePath, string backupDirectory, CancellationToken ct = default);

    /// <summary>
    /// Restaura una base de datos desde un archivo de backup.
    /// </summary>
    Task<RestoreResult> RestoreBackupAsync(string backupPath, string databasePath, CancellationToken ct = default);

    /// <summary>
    /// Verifica la integridad de una base de datos SQLite.
    /// </summary>
    Task<bool> VerifyIntegrityAsync(string databasePath, CancellationToken ct = default);
}

/// <summary>
/// Información de un archivo de backup.
/// </summary>
public sealed record BackupInfo(string FullPath, string FileName, DateTime CreatedUtc, long SizeBytes);

/// <summary>
/// Resultado de una operación de restauración.
/// </summary>
public sealed record RestoreResult(bool Success, string Message, string? PreRestoreBackupPath);