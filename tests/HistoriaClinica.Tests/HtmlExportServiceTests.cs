using HistoriaClinica.App.Services;
using HistoriaClinica.Core.Models;
using HistoriaClinica.Core.Validation;

namespace HistoriaClinica.Tests;

public sealed class HtmlExportServiceTests : IDisposable
{
    private readonly string _tempDirectory;

    public HtmlExportServiceTests()
    {
        _tempDirectory = Path.Combine(Path.GetTempPath(), $"historia-html-{Guid.NewGuid():N}");
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
    public async Task ExportarAsync_GeneraArchivoConDatosDelPacienteYFicha()
    {
        var paciente = new Paciente
        {
            Dni = "12345678",
            FirstName = "Ana",
            LastName = "Pérez",
            DOB = new DateOnly(1990, 5, 4),
            Phone = "555-0000",
            ObraSocial = "OSDE",
            AntecPers = "Hipertensión",
            Medicacion = "Enalapril",
            Alergias = "Penicilina",
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

        var ruta = Path.Combine(_tempDirectory, "salida.html");
        var service = new HtmlExportService();

        await service.ExportarAsync(paciente, ficha, ruta);

        Assert.True(File.Exists(ruta));
        var contenido = await File.ReadAllTextAsync(ruta);
        Assert.Contains("Ana Pérez", contenido);
        Assert.Contains("12345678", contenido);
        Assert.Contains("Control", contenido);
        Assert.Contains("120/80", contenido);
        Assert.Contains("@media print", contenido);
    }

    [Fact]
    public void GenerarHtml_EscapaContenido()
    {
        var paciente = new Paciente
        {
            Dni = "12345678",
            FirstName = "<script>",
            LastName = "Pérez",
        };

        var service = new HtmlExportService();
        var html = service.GenerarHtml(paciente, null);

        Assert.DoesNotContain("<script>", html);
        Assert.Contains("&lt;script&gt;", html);
    }
}
