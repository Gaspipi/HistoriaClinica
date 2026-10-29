using System.Text;
using HistoriaClinica.Core.Models;
using HistoriaClinica.Core.Validation;
using Microsoft.Extensions.Logging;

namespace HistoriaClinica.App.Services;

public sealed class HtmlExportService : IHtmlExportService
{
    private readonly IDialogService? _dialogs;
    private readonly ILogger<HtmlExportService>? _logger;

    public HtmlExportService(IDialogService? dialogs = null, ILogger<HtmlExportService>? logger = null)
    {
        _dialogs = dialogs;
        _logger = logger;
    }

    public string GenerarHtml(Paciente paciente, FichaDiaria? ficha)
    {
        ArgumentNullException.ThrowIfNull(paciente);

        var sb = new StringBuilder();
        sb.AppendLine("<!DOCTYPE html>");
        sb.AppendLine("<html lang=\"es\">");
        sb.AppendLine("<head>");
        sb.AppendLine("  <meta charset=\"utf-8\" />");
        sb.AppendLine("  <meta name=\"viewport\" content=\"width=device-width, initial-scale=1\" />");
        sb.AppendLine($"  <title>Historia clínica - {Encode(paciente.FirstName)} {Encode(paciente.LastName)}</title>");
        sb.AppendLine("  <style>");
        sb.AppendLine(Css);
        sb.AppendLine("  </style>");
        sb.AppendLine("</head>");
        sb.AppendLine("<body>");
        sb.AppendLine("  <main class=\"pagina\">");
        sb.AppendLine("    <header class=\"encabezado\">");
        sb.AppendLine("      <h1>Historia Clínica</h1>");
        sb.AppendLine($"      <p class=\"subtitulo\">Paciente: {Encode(NombreCompleto(paciente))}</p>");
        sb.AppendLine("    </header>");

        sb.AppendLine("    <section class=\"bloque\">");
        sb.AppendLine("      <h2>Datos del paciente</h2>");
        sb.AppendLine("      <table class=\"datos\">");
        AppendFila(sb, "DNI", paciente.Dni);
        AppendFila(sb, "Nombre", paciente.FirstName);
        AppendFila(sb, "Apellido", paciente.LastName);
        AppendFila(sb, "Fecha de nacimiento", FechaClinica.FormatDate(paciente.DOB));
        AppendFila(sb, "Edad", EdadTexto(paciente));
        AppendFila(sb, "Teléfono", paciente.Phone);
        AppendFila(sb, "Obra social", paciente.ObraSocial);
        AppendFila(sb, "Nº de socio", paciente.NroSocio);
        sb.AppendLine("      </table>");
        sb.AppendLine("    </section>");

        sb.AppendLine("    <section class=\"bloque\">");
        sb.AppendLine("      <h2>Antecedentes</h2>");
        AppendParrafo(sb, "Personales", paciente.AntecPers);
        AppendParrafo(sb, "Familiares", paciente.AntecFam);
        sb.AppendLine("    </section>");

        sb.AppendLine("    <section class=\"bloque\">");
        sb.AppendLine("      <h2>Medicación</h2>");
        AppendParrafo(sb, null, paciente.Medicacion);
        sb.AppendLine("    </section>");

        sb.AppendLine("    <section class=\"bloque\">");
        sb.AppendLine("      <h2>Alergias y problemas activos</h2>");
        AppendParrafo(sb, "Alergias", paciente.Alergias);
        AppendParrafo(sb, "Problemas activos", paciente.ProblemasActivos);
        sb.AppendLine("    </section>");

        if (ficha is not null)
        {
            sb.AppendLine("    <section class=\"bloque\">");
            sb.AppendLine("      <h2>Ficha diaria</h2>");
            sb.AppendLine("      <table class=\"datos\">");
            AppendFila(sb, "Fecha", FechaClinica.FormatDateTime(ficha.Fecha));
            AppendFila(sb, "Motivo", ficha.Motivo);
            AppendFila(sb, "Enfermedad actual", ficha.Enfermedad);
            AppendFila(sb, "Indicaciones", ficha.Indicaciones);
            AppendFila(sb, "Presión arterial", ficha.PresionArterial ?? string.Empty);
            AppendFila(sb, "Frecuencia cardíaca", ficha.FrecuenciaCardiaca?.ToString() ?? string.Empty);
            AppendFila(sb, "Temperatura", ficha.Temperatura?.ToString() ?? string.Empty);
            AppendFila(sb, "Peso", ficha.Peso?.ToString() ?? string.Empty);
            AppendFila(sb, "Cerrada", ficha.Cerrada ? "Sí" : "No");
            sb.AppendLine("      </table>");
            sb.AppendLine("    </section>");
        }

        sb.AppendLine("    <footer class=\"pie\">");
        sb.AppendLine($"      <p>Generado el {DateTime.Now.ToString(FechaClinica.DateTimeFormat, FechaClinica.Culture)}.</p>");
        sb.AppendLine("    </footer>");
        sb.AppendLine("  </main>");
        sb.AppendLine("</body>");
        sb.AppendLine("</html>");

        return sb.ToString();
    }

    public Task<string> GenerarHtmlAsync(Paciente paciente, FichaDiaria? ficha, CancellationToken cancellationToken = default) =>
        Task.FromResult(GenerarHtml(paciente, ficha));

    public async Task ExportarAsync(Paciente paciente, FichaDiaria? ficha, string rutaArchivo, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(rutaArchivo))
        {
            throw new ArgumentException("La ruta de destino no puede estar vacía.", nameof(rutaArchivo));
        }

        var html = GenerarHtml(paciente, ficha);
        var directorio = Path.GetDirectoryName(rutaArchivo);
        if (!string.IsNullOrWhiteSpace(directorio))
        {
            Directory.CreateDirectory(directorio);
        }

        await File.WriteAllTextAsync(rutaArchivo, html, Encoding.UTF8, cancellationToken);
        _logger?.LogInformation("Ficha exportada a {Ruta}", rutaArchivo);
    }

    public async Task<string?> ExportarAsync(Paciente paciente, FichaDiaria? ficha, CancellationToken cancellationToken = default)
    {
        if (_dialogs is null)
        {
            throw new InvalidOperationException("No hay servicio de diálogos disponible para solicitar la ruta de guardado.");
        }

        var nombreSugerido = $"Ficha_{paciente.Dni}_{ficha?.Id ?? 0}.html";
        var ruta = await _dialogs.ShowSaveFileDialogAsync("Exportar a HTML", nombreSugerido, ".html", cancellationToken);
        if (string.IsNullOrWhiteSpace(ruta))
        {
            return null;
        }

        await ExportarAsync(paciente, ficha, ruta, cancellationToken);
        return ruta;
    }

    public Task ExportarHtmlAsync(Paciente paciente, FichaDiaria? ficha, string rutaArchivo, CancellationToken cancellationToken = default) =>
        ExportarAsync(paciente, ficha, rutaArchivo, cancellationToken);

    public Task<string?> ExportarHtmlAsync(Paciente paciente, FichaDiaria? ficha, CancellationToken cancellationToken = default) =>
        ExportarAsync(paciente, ficha, cancellationToken);

    private static string NombreCompleto(Paciente paciente)
    {
        var partes = new[] { paciente.FirstName, paciente.LastName }
            .Where(parte => !string.IsNullOrWhiteSpace(parte));
        return partes.Any() ? string.Join(" ", partes) : "Paciente sin nombre";
    }

    private static string EdadTexto(Paciente paciente)
    {
        if (!paciente.DOB.HasValue)
        {
            return "Sin fecha de nacimiento";
        }

        var hoy = DateOnly.FromDateTime(DateTime.Today);
        var nacimiento = paciente.DOB.Value;
        if (nacimiento > hoy)
        {
            return "Fecha inválida";
        }

        var edad = hoy.Year - nacimiento.Year;
        if (nacimiento > hoy.AddYears(-edad))
        {
            edad--;
        }

        return edad switch
        {
            0 => "Menos de 1 año",
            1 => "1 año",
            _ => $"{edad} años",
        };
    }

    private static void AppendFila(StringBuilder sb, string etiqueta, string valor)
    {
        sb.AppendLine("        <tr>");
        sb.AppendLine($"          <th>{Encode(etiqueta)}</th>");
        sb.AppendLine($"          <td>{Encode(string.IsNullOrWhiteSpace(valor) ? "—" : valor)}</td>");
        sb.AppendLine("        </tr>");
    }

    private static void AppendParrafo(StringBuilder sb, string? etiqueta, string? valor)
    {
        var texto = string.IsNullOrWhiteSpace(valor) ? "Sin datos" : valor;
        if (string.IsNullOrWhiteSpace(etiqueta))
        {
            sb.AppendLine($"      <p class=\"parrafo\">{Encode(texto)}</p>");
        }
        else
        {
            sb.AppendLine($"      <p class=\"parrafo\"><strong>{Encode(etiqueta)}:</strong> {Encode(texto)}</p>");
        }
    }

    private static string Encode(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        return value
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("\"", "&quot;");
    }

    private const string Css = """
        body {
            font-family: "Segoe UI", Arial, sans-serif;
            margin: 0;
            padding: 24px;
            color: #1f2937;
            background: #ffffff;
        }
        .pagina {
            max-width: 900px;
            margin: 0 auto;
        }
        .encabezado {
            border-bottom: 2px solid #2563eb;
            padding-bottom: 12px;
            margin-bottom: 20px;
        }
        h1 {
            margin: 0;
            font-size: 24px;
            color: #1d4ed8;
        }
        h2 {
            font-size: 18px;
            color: #1d4ed8;
            margin: 0 0 10px 0;
        }
        .subtitulo {
            margin: 4px 0 0 0;
            color: #4b5563;
        }
        .bloque {
            margin-bottom: 18px;
            padding: 14px;
            border: 1px solid #e5e7eb;
            border-radius: 8px;
            page-break-inside: avoid;
        }
        .datos {
            width: 100%;
            border-collapse: collapse;
        }
        .datos th {
            text-align: left;
            width: 35%;
            padding: 6px 8px;
            background: #f3f4f6;
            border: 1px solid #e5e7eb;
            font-weight: 600;
        }
        .datos td {
            padding: 6px 8px;
            border: 1px solid #e5e7eb;
        }
        .parrafo {
            margin: 6px 0;
            white-space: pre-wrap;
        }
        .pie {
            margin-top: 24px;
            padding-top: 12px;
            border-top: 1px solid #e5e7eb;
            color: #6b7280;
            font-size: 12px;
        }
        @media print {
            body {
                padding: 0;
            }
            .bloque {
                page-break-inside: avoid;
            }
        }
        """;
}
