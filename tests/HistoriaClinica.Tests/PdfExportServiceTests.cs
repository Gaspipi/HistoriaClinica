using HistoriaClinica.App.Services;
using HistoriaClinica.Core.Models;
using HistoriaClinica.Core.Validation;

namespace HistoriaClinica.Tests;

public sealed class PdfExportServiceTests : IDisposable
{
    private readonly string _tempDirectory;

    public PdfExportServiceTests()
    {
        _tempDirectory = Path.Combine(Path.GetTempPath(), $"historia-pdf-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDirectory);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDirectory))
        {
            Directory.Delete(_tempDirectory, recursive: true);
        }
    }

    [Fact]
    public async Task ExportarAsync_GeneraArchivoPdfConDatosDelPacienteYFicha()
    {
        var paciente = new Paciente
        {
            Dni = "12345678",
            Nombre = "Ana",
            Apellido = "Pérez",
            FechaNacimiento = new DateOnly(1990, 5, 4),
            Telefono = "555-0000",
            ObraSocial = "OSDE",
            AntecedentesPersonales = "Hipertensión",
        };

        var ficha = new FichaDiaria
        {
            Id = 7,
            Dni = paciente.Dni,
            Fecha = new DateTime(2026, 10, 20, 10, 30, 0),
            Motivo = "Control",
            Enfermedad = "Sin particularidades",
            Indicaciones = "Reposo",
            PresionArterial = "120/80",
            FrecuenciaCardiaca = 72,
            Temperatura = 36.5m,
            Peso = 60.2m,
            Cerrada = false,
        };

        var ruta = Path.Combine(_tempDirectory, "salida.pdf");
        var service = new PdfExportService();

        await service.ExportarAsync(paciente, ficha, ruta);

        Assert.True(File.Exists(ruta));
        var bytes = await File.ReadAllBytesAsync(ruta);
        Assert.True(bytes.Length > 0);
        var header = System.Text.Encoding.ASCII.GetString(bytes, 0, Math.Min(4, bytes.Length));
        Assert.Equal("%PDF", header);
    }

    [Fact]
    public async Task GenerarPdfAsync_GeneraBytesPdf()
    {
        var paciente = new Paciente
        {
            Dni = "12345678",
            Nombre = "Ana",
            Apellido = "Pérez",
        };

        var service = new PdfExportService();
        var bytes = await service.GenerarPdfAsync(paciente, null);

        Assert.NotNull(bytes);
        Assert.True(bytes.Length > 0);
        var header = System.Text.Encoding.ASCII.GetString(bytes, 0, Math.Min(4, bytes.Length));
        Assert.Equal("%PDF", header);
    }
}
