using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HistoriaClinica.Core.Models;

public class AuditLog
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string Username { get; set; } = string.Empty;

    public DateTime Timestamp { get; set; } = DateTime.Now;

    public string Entity { get; set; } = string.Empty;

    public string Action { get; set; } = string.Empty;

    public string? Details { get; set; }
}
