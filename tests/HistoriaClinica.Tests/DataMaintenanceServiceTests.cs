using HistoriaClinica.App.Services;
using HistoriaClinica.Core.Data;
using HistoriaClinica.Core.Models;
using HistoriaClinica.Core.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging.Abstractions;

namespace HistoriaClinica.Tests;

public sealed class DataMaintenanceServiceTests : IDisposable
{
    private readonly string _tempDirectory;
    private readonly string _databasePath;

    public DataMaintenanceServiceTests()
    {
        _tempDirectory = Path.Combine(Path.GetTempPath(), $"historia-backup-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDirectory);
        _databasePath = Path.Combine(_tempDirectory, "Historia.db");
    }

    [Fact]
    public async Task CrearCopiaSeguridad_CopiaLosDatos()
    {
        var factory = new TestDbContextFactory(_databasePath);
        using (var context = factory.CreateDbContext())
        {
            context.Database.EnsureCreated();
        }

        var pacientes = new PacienteRepository(factory);
        await pacientes.SaveAsync(new Paciente
        {
            Dni = "12345678",
            FirstName = "Ana",
            LastName = "Pérez",
            DOB = new DateTime(1990, 5, 4),
        });

        var service = new DataMaintenanceService(
            new DatabasePathProvider(_databasePath),
            NullLogger<DataMaintenanceService>.Instance);

        var copia = await service.CrearCopiaSeguridadAsync();

        Assert.True(File.Exists(copia));
        Assert.Equal(1, await ContarPacientesAsync(copia));
        Assert.Equal(
            Path.Combine(Path.GetDirectoryName(_databasePath)!, "Backups"),
            Path.GetDirectoryName(copia));
    }

    [Fact]
    public async Task CrearCopiaSeguridad_RutaConEspacios_CopiaLosDatos()
    {
        var carpeta = Path.Combine(_tempDirectory, "Documentos Clinica");
        Directory.CreateDirectory(carpeta);
        var rutaBase = Path.Combine(carpeta, "Historia con espacios.db");

        var factory = new TestDbContextFactory(rutaBase);
        using (var context = factory.CreateDbContext())
        {
            context.Database.EnsureCreated();
        }

        var pacientes = new PacienteRepository(factory);
        await pacientes.SaveAsync(new Paciente
        {
            Dni = "12345678",
            FirstName = "Ana",
            LastName = "Pérez",
            DOB = new DateTime(1990, 5, 4),
        });

        var service = new DataMaintenanceService(
            new DatabasePathProvider(rutaBase),
            NullLogger<DataMaintenanceService>.Instance);

        var copia = await service.CrearCopiaSeguridadAsync();

        Assert.True(File.Exists(copia));
        Assert.Equal(1, await ContarPacientesAsync(copia));
    }

    public void Dispose()
    {
        SqliteConnection.ClearAllPools();

        if (Directory.Exists(_tempDirectory))
        {
            Directory.Delete(_tempDirectory, recursive: true);
        }
    }

    private static async Task<long> ContarPacientesAsync(string databasePath)
    {
        await using var connection = new SqliteConnection($"Data Source={databasePath};Mode=ReadOnly");
        await connection.OpenAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM Pacientes;";
        return (long)(await command.ExecuteScalarAsync() ?? 0L);
    }
}
