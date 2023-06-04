using HistoriaClinica.Contexts;
using HistoriaClinica.Models;
using System.Data;

namespace HistoriaClinica
{
    public class CRUD_Historias
    {
        public static string[] ReadDniCollection()
        {
            string[] pacientes;
            using (var db = new HistoriaContext())
            {
                db.Database.EnsureCreatedAsync();
                pacientes = db.Pacientes.Select(_ => _.Dni).ToArray();
            }
            return pacientes;
        }

        public static List<FichaDiaria> ReadListadoFichas(string dni)
        {
            List<FichaDiaria> listaFichas;
            using (var db = new HistoriaContext())
            {
                db.Database.EnsureCreatedAsync();
                listaFichas = db.FichasDiarias.Where(fd => fd.Dni == dni).ToList();
            }
            return listaFichas;
        }

        public static Paciente? ReadDatosPaciente(string dni)
        {
            Paciente? pac;
            using (var db = new HistoriaContext())
            {
                db.Database.EnsureCreatedAsync();
                pac = db.Pacientes.Where(p => p.Dni == dni).FirstOrDefault();
            }
            return pac;
        }

        public static void CreatePaciente(Paciente pac)
        {
            Paciente? pacEncontrado = ReadDatosPaciente(pac.Dni);
            if (pacEncontrado != null)
            {
                if (pacEncontrado.Dni != "" && pacEncontrado.Dni != "N/A")
                {
                    using var db = new HistoriaContext();
                    db.Database.EnsureCreatedAsync();
                    db.Pacientes.Attach(pac);
                    db.Pacientes.Update(pac);
                    db.SaveChanges();
                }
            }
            else
            {
                using var db = new HistoriaContext();
                db.Database.EnsureCreatedAsync();
                db.Pacientes.Add(pac);
                db.SaveChanges();
            }
        }

        public static void CreateFichaDiaria(FichaDiaria fd)
        {
            FichaDiaria? fd_encontrada = ReadFichaDiaria(fd);
            if (fd_encontrada != null)
            {
                using var db = new HistoriaContext();
                db.Database.EnsureCreatedAsync();
                fd_encontrada.Indicaciones = fd.Indicaciones;
                fd_encontrada.Enfermedad = fd.Enfermedad;
                fd_encontrada.Motivo = fd.Motivo;
                db.FichasDiarias.Update(fd_encontrada);
                db.SaveChanges();
            }
            else
            {
                using var db = new HistoriaContext();
                db.Database.EnsureCreatedAsync();
                db.FichasDiarias.Add(fd);
                db.SaveChanges();
            }
        }

        public static FichaDiaria? ReadFichaDiaria(FichaDiaria fd)
        {
            FichaDiaria? fd_encontrada;
            using (var db = new HistoriaContext())
            {
                db.Database.EnsureCreatedAsync();
                fd_encontrada = db.FichasDiarias.Where(p => (p.Dni == fd.Dni) && (p.Fecha == fd.Fecha)).FirstOrDefault();
            }
            return fd_encontrada;
        }

        public static void DeleteFicha(FichaDiaria fd)
        {
            using var db = new HistoriaContext();
            db.Database.EnsureCreatedAsync();
            db.FichasDiarias.Attach(fd);
            db.FichasDiarias.Remove(fd);
            db.SaveChanges();
        }
    }
}