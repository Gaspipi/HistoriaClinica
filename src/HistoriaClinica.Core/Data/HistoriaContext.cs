using HistoriaClinica.Core.Models;
using HistoriaClinica.Core.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace HistoriaClinica.Core.Data;

public class HistoriaContext : DbContext
{
    public HistoriaContext(DbContextOptions<HistoriaContext> options) : base(options)
    {
    }

    public DbSet<Paciente> Pacientes => Set<Paciente>();

    public DbSet<FichaDiaria> FichasDiarias => Set<FichaDiaria>();

    public DbSet<Usuario> Usuarios => Set<Usuario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Paciente>().ToTable("Pacientes");
        modelBuilder.Entity<Paciente>(entity =>
        {
            entity.HasKey(p => p.Dni);
            var dateOnlyConverter = new ValueConverter<DateOnly?, string?>(
                v => v.HasValue ? v.Value.ToString("yyyy-MM-dd") : null,
                v => string.IsNullOrWhiteSpace(v) ? null : DateOnly.Parse(v));
            var dateOnlyComparer = new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<DateOnly?>(
                (d1, d2) => Nullable.Equals(d1, d2),
                d => d.HasValue ? d.Value.GetHashCode() : 0,
                d => d.HasValue ? d.Value : null);
            entity.Property(p => p.DOB)
                .HasConversion(dateOnlyConverter)
                .HasColumnType("TEXT")
                .Metadata.SetValueComparer(dateOnlyComparer);
            entity.HasMany(p => p.FichasDiarias)
                .WithOne(f => f.Paciente)
                .HasForeignKey(f => f.Dni);
        });

        modelBuilder.Entity<FichaDiaria>().ToTable("FichasDiarias");
        modelBuilder.Entity<FichaDiaria>(entity =>
        {
            entity.Property(e => e.Motivo).IsRequired();
            entity.Property(e => e.Enfermedad).IsRequired();
            entity.Property(e => e.Indicaciones).IsRequired();
        });

        modelBuilder.Entity<Usuario>().ToTable("Usuarios");
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Username).HasMaxLength(UsuarioLengths.Max).IsRequired();
            entity.Property(u => u.NormalizedUsername)
                .HasMaxLength(UsuarioLengths.Max)
                .IsRequired();
            entity.Property(u => u.PasswordHash).IsRequired();
            entity.Property(u => u.PasswordSalt).IsRequired();
            entity.HasIndex(u => u.NormalizedUsername).IsUnique();
        });

        base.OnModelCreating(modelBuilder);
    }
}
