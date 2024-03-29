﻿using HistoriaClinica.Models;
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
            // Configuración de la conexión a la base de datos SQLite
            optionsBuilder.UseSqlite(connectionString: "Filename=" + dbase,
                sqliteOptionsAction: op =>
                {
                    op.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
                });
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuración de las tablas y propiedades de las entidades

            // Tabla Pacientes
            modelBuilder.Entity<Paciente>().ToTable("Pacientes");
            modelBuilder.Entity<Paciente>(entity =>
            {
                entity.HasKey(p => p.Dni); // Clave primaria de la entidad Paciente
                entity.Property(p => p.DOB).IsRequired(); // Propiedad DOB requerida
                entity.HasMany(p => p.FichasDiarias).WithOne(f => f.Paciente).HasForeignKey(f => f.Dni);
            });

            // Tabla FichasDiarias
            modelBuilder.Entity<FichaDiaria>().ToTable("FichasDiarias");
            modelBuilder.Entity<FichaDiaria>(entity =>
            {
                entity.Property(e => e.Motivo).IsRequired(); // Propiedad Motivo requerida
                entity.Property(e => e.Enfermedad).IsRequired(); // Propiedad Enfermedad requerida
                entity.Property(e => e.Indicaciones).IsRequired(); // Propiedad Indicaciones requerida
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
