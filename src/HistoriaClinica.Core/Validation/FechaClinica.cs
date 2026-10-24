using System.Globalization;

namespace HistoriaClinica.Core.Validation;

public static class FechaClinica
{
    public static readonly CultureInfo Culture = CultureInfo.GetCultureInfo("es-ES");

    public const string DateFormat = "dd/MM/yyyy";
    public const string DateTimeFormat = "dd/MM/yyyy HH:mm:ss";

    private static readonly string[] DateFormats = [DateFormat];
    private static readonly string[] DateTimeFormats = [DateTimeFormat, DateFormat];

    public static bool HasValue(DateTime value) => value != default && value.Year > 1;

    public static string FormatDate(DateTime value) =>
        HasValue(value) ? value.ToString(DateFormat, Culture) : string.Empty;

    public static string FormatDateTime(DateTime value) =>
        HasValue(value) ? value.ToString(DateTimeFormat, Culture) : string.Empty;

    public static bool TryParseDate(string? value, out DateTime result)
    {
        result = default;
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        return DateTime.TryParseExact(
            value.Trim(),
            DateFormats,
            Culture,
            DateTimeStyles.None,
            out result);
    }

    public static bool TryParseDateTime(string? value, out DateTime result)
    {
        result = default;
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        return DateTime.TryParseExact(
            value.Trim(),
            DateTimeFormats,
            Culture,
            DateTimeStyles.None,
            out result);
    }

    public static DateTime? ToNullable(DateTime value) => HasValue(value) ? value : null;

    public static DateTime FromNullable(DateTime? value) => value ?? default;
}
