using System.ComponentModel.DataAnnotations;

namespace HistoriaClinica.Core.Models;

public class Usuario
{
    [Key]
    public int Id { get; set; }

    [Required, StringLength(32)]
    public string Username { get; set; } = string.Empty;

    [Required, StringLength(32)]
    public string NormalizedUsername { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    public string PasswordSalt { get; set; } = string.Empty;

    public int PasswordIterations { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
