using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HistoriaClinica.Core.Models;

public class Plantilla
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    public string Motivo { get; set; } = string.Empty;

    public string Enfermedad { get; set; } = string.Empty;

    public string Indicaciones { get; set; } = string.Empty;

    public string? CreadorDni { get; set; }

    public Paciente? Creador { get; set; }
}
