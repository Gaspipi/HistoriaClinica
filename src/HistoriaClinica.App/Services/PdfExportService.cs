using System.Globalization;
using HistoriaClinica.Core.Models;
using HistoriaClinica.Core.Validation;
using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace HistoriaClinica.App.Services;

public sealed class PdfExportService : IPdfExportService
{
    private readonly IDialogService? _dialogs;
    private readonly ILogger<PdfExportService>? _logger;

    public PdfExportService(IDialogService? dialogs = null, ILogger<PdfExportService>? logger = null)
    {
        _dialogs = dialogs;
        _logger = logger;
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] GenerarPdf(Paciente paciente, FichaDiaria? ficha)
    {
        ArgumentNullException.ThrowIfNull(paciente);

        var documento = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.Arial));

                page.Header().Column(col =>
                {
                    col.Item().Text("Historia Clínica").FontSize(18).SemiBold().FontColor(Colors.Blue.Darken2);
                    col.Item().Text($"Paciente: {NombreCompleto(paciente)}").FontSize(11).FontColor(Colors.Grey.Darken2);
                    col.Item().PaddingTop(4).BorderBottom(1).BorderColor(Colors.Blue.Darken2);
                });

                page.Content().PaddingVertical(10).Column(col =>
                {
                    col.Spacing(12);

                    // Datos del paciente
                    col.Item().Column(bloque =>
                    {
                        bloque.Item().Text("Datos del paciente").FontSize(12).SemiBold().FontColor(Colors.Blue.Darken2);
                    bloque.Item().PaddingTop(4).Table(tabla =>
                    {
                        tabla.ColumnsDefinition(def =>
                        {
                            def.RelativeColumn(1);
                            def.RelativeColumn(2);
                        });

                        tabla.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Background(Colors.Grey.Lighten4).Padding(4).Text("DNI").SemiBold();
                        tabla.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(string.IsNullOrWhiteSpace(paciente.Dni) ? "—" : paciente.Dni);

                        tabla.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Background(Colors.Grey.Lighten4).Padding(4).Text("Nombre").SemiBold();
                        tabla.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(string.IsNullOrWhiteSpace(paciente.Nombre) ? "—" : paciente.Nombre);

                        tabla.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Background(Colors.Grey.Lighten4).Padding(4).Text("Apellido").SemiBold();
                        tabla.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(string.IsNullOrWhiteSpace(paciente.Apellido) ? "—" : paciente.Apellido);

                        tabla.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Background(Colors.Grey.Lighten4).Padding(4).Text("Fecha de nacimiento").SemiBold();
                        tabla.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(paciente.FechaNacimiento.HasValue ? FechaClinica.FormatDate(paciente.FechaNacimiento.Value) : "—");

                        tabla.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Background(Colors.Grey.Lighten4).Padding(4).Text("Edad").SemiBold();
                        tabla.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(EdadTexto(paciente));

                        tabla.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Background(Colors.Grey.Lighten4).Padding(4).Text("Teléfono").SemiBold();
                        tabla.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(string.IsNullOrWhiteSpace(paciente.Telefono) ? "—" : paciente.Telefono);

                        tabla.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Background(Colors.Grey.Lighten4).Padding(4).Text("Obra social").SemiBold();
                        tabla.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(string.IsNullOrWhiteSpace(paciente.ObraSocial) ? "—" : paciente.ObraSocial);

                        tabla.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Background(Colors.Grey.Lighten4).Padding(4).Text("Nº de socio").SemiBold();
                        tabla.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(string.IsNullOrWhiteSpace(paciente.NumeroSocio) ? "—" : paciente.NumeroSocio);
                    });
                    });

                    // Antecedentes
                    col.Item().Column(bloque =>
                    {
                        bloque.Item().Text("Antecedentes").FontSize(12).SemiBold().FontColor(Colors.Blue.Darken2);
                        bloque.Item().PaddingTop(4).Column(c =>
                        {
                            c.Spacing(2);
                            AgregarParrafo(c, "Personales", paciente.AntecedentesPersonales);
                            AgregarParrafo(c, "Familiares", paciente.AntecedentesFamiliares);
                        });
                    });

                    // Ficha diaria
                    if (ficha is not null)
                    {
                        col.Item().Column(bloque =>
                        {
                            bloque.Item().Text("Ficha diaria").FontSize(12).SemiBold().FontColor(Colors.Blue.Darken2);
                            bloque.Item().PaddingTop(4).Table(tabla =>
                            {
                            tabla.ColumnsDefinition(def =>
                            {
                                def.RelativeColumn(1);
                                def.RelativeColumn(2);
                            });
                            tabla.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Background(Colors.Grey.Lighten4).Padding(4).Text("Fecha").SemiBold();
                            tabla.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(FechaClinica.FormatDateTime(ficha.Fecha));
                            tabla.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Background(Colors.Grey.Lighten4).Padding(4).Text("Motivo").SemiBold();
                            tabla.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(string.IsNullOrWhiteSpace(ficha.Motivo) ? "—" : ficha.Motivo);
                            tabla.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Background(Colors.Grey.Lighten4).Padding(4).Text("Enfermedad actual").SemiBold();
                            tabla.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(string.IsNullOrWhiteSpace(ficha.Enfermedad) ? "—" : ficha.Enfermedad);
                            tabla.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Background(Colors.Grey.Lighten4).Padding(4).Text("Indicaciones").SemiBold();
                            tabla.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(string.IsNullOrWhiteSpace(ficha.Indicaciones) ? "—" : ficha.Indicaciones);
                            tabla.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Background(Colors.Grey.Lighten4).Padding(4).Text("Presión arterial").SemiBold();
                            tabla.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(string.IsNullOrWhiteSpace(ficha.PresionArterial) ? "—" : ficha.PresionArterial);
                            tabla.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Background(Colors.Grey.Lighten4).Padding(4).Text("Frecuencia cardíaca").SemiBold();
                            tabla.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(ficha.FrecuenciaCardiaca?.ToString(CultureInfo.InvariantCulture) ?? "—");
                            tabla.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Background(Colors.Grey.Lighten4).Padding(4).Text("Temperatura").SemiBold();
                            tabla.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(ficha.Temperatura?.ToString(CultureInfo.InvariantCulture) ?? "—");
                            tabla.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Background(Colors.Grey.Lighten4).Padding(4).Text("Peso").SemiBold();
                            tabla.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(ficha.Peso?.ToString(CultureInfo.InvariantCulture) ?? "—");
                            tabla.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Background(Colors.Grey.Lighten4).Padding(4).Text("Cerrada").SemiBold();
                            tabla.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(ficha.Cerrada ? "Sí" : "No");
                            });
                        });
                    }
                });

                page.Footer().PaddingTop(10).BorderTop(1).BorderColor(Colors.Grey.Lighten1).AlignRight().Text(text =>
                {
                    text.DefaultTextStyle(x => x.FontSize(8).FontColor(Colors.Grey.Darken1));
                    text.Span("Generado el ");
                    text.Span(DateTime.Now.ToString(FechaClinica.DateTimeFormat, FechaClinica.Culture));
                    text.Span(".");
                });
            });
        });

        return documento.GeneratePdf();
    }

    public Task<byte[]> GenerarPdfAsync(Paciente paciente, FichaDiaria? ficha, CancellationToken cancellationToken = default) =>
        Task.Run(() => GenerarPdf(paciente, ficha), cancellationToken);

    public async Task ExportarAsync(Paciente paciente, FichaDiaria? ficha, string rutaArchivo, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(rutaArchivo))
        {
            throw new ArgumentException("La ruta de destino no puede estar vacía.", nameof(rutaArchivo));
        }

        var pdf = await GenerarPdfAsync(paciente, ficha, cancellationToken);
        var directorio = Path.GetDirectoryName(rutaArchivo);
        if (!string.IsNullOrWhiteSpace(directorio))
        {
            Directory.CreateDirectory(directorio);
        }

        await File.WriteAllBytesAsync(rutaArchivo, pdf, cancellationToken);
        _logger?.LogInformation("Ficha exportada a {Ruta}", rutaArchivo);
    }

    public async Task<string?> ExportarAsync(Paciente paciente, FichaDiaria? ficha, CancellationToken cancellationToken = default)
    {
        if (_dialogs is null)
        {
            throw new InvalidOperationException("No hay servicio de diálogos disponible para solicitar la ruta de guardado.");
        }

        var nombreSugerido = $"Ficha_{paciente.Dni}_{ficha?.Id ?? 0}.pdf";
        var ruta = await _dialogs.ShowSaveFileDialogAsync("Exportar a PDF", nombreSugerido, ".pdf", cancellationToken);
        if (string.IsNullOrWhiteSpace(ruta))
        {
            return null;
        }

        await ExportarAsync(paciente, ficha, ruta, cancellationToken);
        return ruta;
    }

    public Task ExportarPdfAsync(Paciente paciente, FichaDiaria? ficha, string rutaArchivo, CancellationToken cancellationToken = default) =>
        ExportarAsync(paciente, ficha, rutaArchivo, cancellationToken);

    public Task<string?> ExportarPdfAsync(Paciente paciente, FichaDiaria? ficha, CancellationToken cancellationToken = default) =>
        ExportarAsync(paciente, ficha, cancellationToken);

    private static void AgregarParrafo(ColumnDescriptor col, string? etiqueta, string? valor)
    {
        var texto = string.IsNullOrWhiteSpace(valor) ? "Sin datos" : valor;
        if (string.IsNullOrWhiteSpace(etiqueta))
        {
            col.Item().Text(texto);
            return;
        }
        col.Item().Text(t =>
        {
            t.Span($"{etiqueta}: ").SemiBold();
            t.Span(texto);
        });
    }

    private static string NombreCompleto(Paciente paciente)
    {
        var partes = new[] { paciente.Nombre, paciente.Apellido }
            .Where(parte => !string.IsNullOrWhiteSpace(parte));
        return partes.Any() ? string.Join(" ", partes) : "Paciente sin nombre";
    }

    private static string EdadTexto(Paciente paciente)
    {
        if (!paciente.FechaNacimiento.HasValue)
        {
            return "Sin fecha de nacimiento";
        }

        var hoy = DateOnly.FromDateTime(DateTime.Today);
        var nacimiento = paciente.FechaNacimiento.Value;
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
}
