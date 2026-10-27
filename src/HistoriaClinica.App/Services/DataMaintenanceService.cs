using System.Diagnostics;
using HistoriaClinica.Core.Data;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

namespace HistoriaClinica.App.Services;

public interface IDataMaintenanceService
{
    string DatabasePath { get; }

    void AbrirCarpetaDatos();

    Task<string> CrearCopiaSeguridadAsync(CancellationToken cancellationToken = default);
}

public sealed class DataMaintenanceService : IDataMaintenanceService
{
    private readonly IDatabasePathProvider _pathProvider;
    private readonly ILogger<DataMaintenanceService> _logger;

    public DataMaintenanceService(
        IDatabasePathProvider pathProvider,
        ILogger<DataMaintenanceService> logger)
    {
        _pathProvider = pathProvider;
        _logger = logger;
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

    public async Task<string> CrearCopiaSeguridadAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(DatabasePath))
        {
            throw new FileNotFoundException("No se encontró la base de datos.", DatabasePath);
        }

        var carpeta = Path.Combine(
            Path.GetDirectoryName(DatabasePath) ?? AppContext.BaseDirectory,
            "Backups");
        Directory.CreateDirectory(carpeta);

        var destino = ObtenerRutaCopia(carpeta);

        try
        {
            await Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                using var origen = new SqliteConnection(new SqliteConnectionStringBuilder
                {
                    DataSource = DatabasePath,
                    Mode = SqliteOpenMode.ReadOnly,
                }.ToString());
                using var copia = new SqliteConnection(new SqliteConnectionStringBuilder
                {
                    DataSource = destino,
                    Mode = SqliteOpenMode.ReadWriteCreate,
                }.ToString());
                origen.Open();
                copia.Open();
                origen.BackupDatabase(copia);
            }, cancellationToken);

            _logger.LogInformation("Copia de seguridad creada en {BackupPath}", destino);
            return destino;
        }
        catch
        {
            // El borrado de la copia incompleta no debe enmascarar la excepción original.
            try
            {
                if (File.Exists(destino))
                {
                    File.Delete(destino);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "No se pudo borrar la copia de seguridad incompleta en {BackupPath}",
                    destino);
            }

            throw;
        }
    }

    private static string ObtenerRutaCopia(string carpeta)
    {
        var baseNombre = $"Historia-{DateTime.Now:yyyyMMdd-HHmmss}";
        var destino = Path.Combine(carpeta, $"{baseNombre}.db");

        var indice = 1;
        while (File.Exists(destino))
        {
            destino = Path.Combine(carpeta, $"{baseNombre}-{indice}.db");
            indice++;
        }

        return destino;
    }
}
