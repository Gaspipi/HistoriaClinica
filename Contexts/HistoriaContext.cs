using HistoriaClinica.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace HistoriaClinica.Contexts
{
    internal class HistoriaContext : DbContext
    {
        readonly string dbase = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Historia.db";
        public DbSet<Paciente> Pacientes { get; set; }
        public DbSet<FichaDiaria> FichasDiarias { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(connectionString: "Filename=" + dbase,
                sqliteOptionsAction: op =>
                {
                    op.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
                });
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Paciente>().ToTable("Pacientes");
            modelBuilder.Entity<Paciente>(entity =>
            {
                entity.HasKey(e => e.Id);
            });
            modelBuilder.Entity<FichaDiaria>().ToTable("FichasDiarias");
            modelBuilder.Entity<FichaDiaria>(entity =>
            {

            });
            
            base.OnModelCreating(modelBuilder);
        }
    }
}
