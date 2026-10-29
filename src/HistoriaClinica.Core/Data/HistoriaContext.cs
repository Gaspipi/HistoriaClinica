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

    public DbSet<Plantilla> Plantillas => Set<Plantilla>();

    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

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
            entity.Property(p => p.Alergias).HasColumnType("TEXT").IsRequired().HasDefaultValue(string.Empty);
            entity.Property(p => p.ProblemasActivos).HasColumnType("TEXT").IsRequired().HasDefaultValue(string.Empty);
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
            entity.Property(e => e.Cerrada).HasDefaultValue(false);
            entity.Property(e => e.PresionArterial).HasColumnType("TEXT");
            entity.Property(e => e.FrecuenciaCardiaca).HasColumnType("INTEGER");
            entity.Property(e => e.Temperatura).HasColumnType("REAL");
            entity.Property(e => e.Peso).HasColumnType("REAL");
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

        modelBuilder.Entity<Plantilla>().ToTable("Plantillas");
        modelBuilder.Entity<Plantilla>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Nombre).IsRequired();
            entity.Property(p => p.Motivo).IsRequired();
            entity.Property(p => p.Enfermedad).HasColumnType("TEXT").IsRequired().HasDefaultValue(string.Empty);
            entity.Property(p => p.Indicaciones).HasColumnType("TEXT").IsRequired().HasDefaultValue(string.Empty);
            entity.Property(p => p.CreadorDni).HasColumnType("TEXT");
            entity.HasIndex(p => p.Nombre).IsUnique();
            entity.HasOne(p => p.Creador)
                .WithMany()
                .HasForeignKey(p => p.CreadorDni);
        });

        modelBuilder.Entity<AuditLog>().ToTable("AuditLogs");
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Username).IsRequired();
            entity.Property(a => a.Timestamp).IsRequired();
            entity.Property(a => a.Entity).IsRequired();
            entity.Property(a => a.Action).IsRequired();
            entity.Property(a => a.Details).HasColumnType("TEXT");
        });

        base.OnModelCreating(modelBuilder);
    }
}
