using System.Globalization;
using Avalonia.Data.Converters;
using HistoriaClinica.Core.Validation;

namespace HistoriaClinica.App.Converters;

public sealed class FechaConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not DateTime fecha || !FechaClinica.HasValue(fecha))
        {
            return string.Empty;
        }

        return parameter switch
        {
            "date" => FechaClinica.FormatDate(fecha),
            "time" => fecha.ToString("HH:mm", FechaClinica.Culture),
            _ => FechaClinica.FormatDateTime(fecha),
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
