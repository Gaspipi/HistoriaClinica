using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HistoriaClinica.Core.Models;

public class FichaDiaria
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [StringLength(8), Required]
    public string Dni { get; set; } = string.Empty;

    public Paciente? Paciente { get; set; }

    public string Enfermedad { get; set; } = string.Empty;

    public string Motivo { get; set; } = string.Empty;

    public string Indicaciones { get; set; } = string.Empty;

    public DateTime Fecha { get; set; } = DateTime.Now;

    public bool Cerrada { get; set; } = false;

    public string? PresionArterial { get; set; }

    public int? FrecuenciaCardiaca { get; set; }

    public decimal? Temperatura { get; set; }

    public decimal? Peso { get; set; }
}
