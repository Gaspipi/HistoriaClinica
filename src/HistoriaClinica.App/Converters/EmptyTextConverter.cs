using System.Globalization;
using Avalonia.Data.Converters;
using HistoriaClinica.Core.Validation;

namespace HistoriaClinica.App.Converters;

public sealed class EmptyTextConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var reemplazo = parameter as string ?? "—";

        return value switch
        {
            null => reemplazo,
            string texto when string.IsNullOrWhiteSpace(texto) => reemplazo,
            DateTime fecha when !FechaClinica.HasValue(fecha) => reemplazo,
            _ => value,
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
