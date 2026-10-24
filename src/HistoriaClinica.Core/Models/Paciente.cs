using System.ComponentModel.DataAnnotations;

namespace HistoriaClinica.Core.Models;

public class Paciente
{
    [Key, Required, StringLength(8)]
    public string Dni { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public DateTime DOB { get; set; }

    public string Phone { get; set; } = string.Empty;

    public string ObraSocial { get; set; } = string.Empty;

    public string NroSocio { get; set; } = string.Empty;

    public string AntecFam { get; set; } = string.Empty;

    public string AntecPers { get; set; } = string.Empty;

    public string Medicacion { get; set; } = string.Empty;

    public List<FichaDiaria> FichasDiarias { get; set; } = new();
}
