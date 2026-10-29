namespace HistoriaClinica.Core.Validation;

public static class DniValidator
{
    public const int Length = 8;
    public const string ErrorMessage = "El campo DNI debe tener 8 dígitos numéricos.";

    public static bool IsValid(string? dni) =>
        !string.IsNullOrWhiteSpace(dni)
        && dni.Length == Length
        && dni.All(char.IsAsciiDigit);

    // Conserva solo los dígitos ASCII y corta en la longitud máxima del DNI;
    // la validez completa se valida aparte al guardar.
    public static string Normalizar(string? dni) =>
        new string((dni ?? string.Empty).Where(char.IsAsciiDigit).Take(Length).ToArray());

    public static string? GetError(string? dni) => IsValid(dni) ? null : ErrorMessage;
}
