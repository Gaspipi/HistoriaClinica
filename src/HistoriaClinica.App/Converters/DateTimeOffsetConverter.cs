using System.Globalization;
using Avalonia.Data.Converters;

namespace HistoriaClinica.App.Converters;

public sealed class DateTimeOffsetConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value switch
        {
            DateTime fecha => new DateTimeOffset(fecha.Date),
            DateTimeOffset fecha => fecha,
            _ => null,
        };

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value switch
        {
            DateTimeOffset fecha => fecha.Date,
            DateTime fecha => fecha.Date,
            _ => null,
        };
}
