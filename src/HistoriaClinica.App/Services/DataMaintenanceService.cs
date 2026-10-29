using System.Diagnostics;
using HistoriaClinica.Core.Data;
using HistoriaClinica.Core.Services;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

namespace HistoriaClinica.App.Services;

public interface IDataMaintenanceService
{
    string DatabasePath { get; }

    void AbrirCarpetaDatos();

    Task<string> CrearCopiaSeguridadAsync(CancellationToken cancellationToken = default);

    Task<string> RestaurarBackupAsync(string backupPath, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<BackupInfo>> ListarBackupsAsync(CancellationToken cancellationToken = default);
}

public sealed class DataMaintenanceService : IDataMaintenanceService
{
    private readonly IDatabasePathProvider _pathProvider;
    private readonly ILogger<DataMaintenanceService> _logger;
    private readonly ISqliteBackupService _backupService;

    public DataMaintenanceService(
        IDatabasePathProvider pathProvider,
        ILogger<DataMaintenanceService> logger,
        ISqliteBackupService backupService)
    {
        _pathProvider = pathProvider;
        _logger = logger;
        _backupService = backupService;
    }

    public string DatabasePath => _pathProvider.DatabasePath;

    public void AbrirCarpetaDatos()
    {
        var carpeta = Path.GetDirectoryName(DatabasePath);
        if (string.IsNullOrWhiteSpace(carpeta))
        {
            throw new InvalidOperationException("No se pudo determinar la carpeta de datos.");
        }

        Directory.CreateDirectory(carpeta);
        Process.Start(new ProcessStartInfo
        {
            FileName = carpeta,
            UseShellExecute = true,
        });
    }

    public Task<string> CrearCopiaSeguridadAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(DatabasePath))
        {
            throw new FileNotFoundException("No se encontró la base de datos.", DatabasePath);
        }

        return _backupService.CreateBackupAsync(DatabasePath, null, cancellationToken);
    }

    public async Task<string> RestaurarBackupAsync(string backupPath, CancellationToken cancellationToken = default)
    {
        var result = await _backupService.RestoreBackupAsync(backupPath, DatabasePath, cancellationToken);
        if (!result.Success)
        {
            throw new InvalidOperationException(result.Message);
        }

        return result.PreRestoreBackupPath ?? string.Empty;
    }

    public Task<IReadOnlyList<BackupInfo>> ListarBackupsAsync(CancellationToken cancellationToken = default)
    {
        var carpeta = Path.Combine(
            Path.GetDirectoryName(DatabasePath) ?? AppContext.BaseDirectory,
            "Backups");
        Directory.CreateDirectory(carpeta);
        return _backupService.ListBackupsAsync(carpeta, cancellationToken);
    }
}
