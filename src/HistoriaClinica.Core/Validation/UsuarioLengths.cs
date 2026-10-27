namespace HistoriaClinica.Core.Validation;

// Longitudes válidas del nombre de usuario, compartidas por la validación,
// el servicio de autenticación y el mapeo de entidades de EF.
public static class UsuarioLengths
{
    public const int Min = 3;

    public const int Max = 32;
}
