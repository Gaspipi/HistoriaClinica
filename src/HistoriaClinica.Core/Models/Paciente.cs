using System.ComponentModel.DataAnnotations;

namespace HistoriaClinica.Core.Models;

public class Paciente
{
    [Key, Required, StringLength(8)]
    public string Dni { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    public string Apellido { get; set; } = string.Empty;

    public DateOnly? FechaNacimiento { get; set; }

    public string Telefono { get; set; } = string.Empty;

    public string ObraSocial { get; set; } = string.Empty;

    public string NumeroSocio { get; set; } = string.Empty;

    public string AntecedentesFamiliares { get; set; } = string.Empty;

    public string AntecedentesPersonales { get; set; } = string.Empty;

    public List<FichaDiaria> FichasDiarias { get; set; } = new();
}
