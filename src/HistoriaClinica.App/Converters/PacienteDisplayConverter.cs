using System.Globalization;
using Avalonia.Data.Converters;
using HistoriaClinica.Core.Models;

namespace HistoriaClinica.App.Converters;

public sealed class PacienteDisplayConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            null => "—",
            Paciente p => string.Join(" ",
                new[] { p.Nombre, p.Apellido }
                    .Where(s => !string.IsNullOrWhiteSpace(s))),
            _ => value.ToString() ?? "—",
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}