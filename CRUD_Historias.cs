using HistoriaClinica.Contexts;
using HistoriaClinica.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace HistoriaClinica
{
    public class CRUD_Historias
    {
        // Método para obtener la colección de DNIs de los pacientes
        public static string[] ReadDniCollection()
        {
            try
            {
                string[] pacientes;
                using (var db = new HistoriaContext())
                {
                    db.Database.EnsureCreatedAsync();
                    pacientes = db.Pacientes.Select(p => p.Dni).ToArray();
                }
                return pacientes;
            }
            catch (DbUpdateException ex)
            {
                Console.Error.WriteLine(ex.Message);
                throw ex;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                throw e;
            }
        }

        // Método para obtener el listado de fichas diarias de un paciente
        public static List<FichaDiaria> ReadListadoFichas(string dni)
        {
            try
            {
                List<FichaDiaria> listaFichas;
                using (var db = new HistoriaContext())
                {
                    db.Database.EnsureCreatedAsync();
                    listaFichas = db.FichasDiarias.Where(fd => fd.Dni == dni).ToList();
                }
                return listaFichas;
            }
            catch (DbUpdateException ex)
            {
                Console.Error.WriteLine(ex.Message);
                throw ex;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                throw e;
            }
        }

        // Método para obtener los datos de un paciente
        public static Paciente? SelectDatosPaciente(string dni)
        {
            try
            {
                Paciente? pac;
                using (var db = new HistoriaContext())
                {
                    db.Database.EnsureCreatedAsync();
                    pac = db.Pacientes.Where(p => p.Dni == dni).FirstOrDefault();
                }
                return pac;
            }
            catch (DbUpdateException ex)
            {
                Console.Error.WriteLine(ex.Message);
                throw ex;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                throw e;
            }
        }

        // Método para crear o actualizar un paciente
        public static void InsertPaciente(Paciente pac)
        {
            try
            {
                Paciente? pacEncontrado = SelectDatosPaciente(pac.Dni);
                if (pacEncontrado != null && pacEncontrado.Dni != "")
                {
                    // El paciente existe, asi que actualizamos el record.
                    using var db = new HistoriaContext();
                    db.Database.EnsureCreatedAsync();
                    db.Pacientes.Attach(pac);
                    db.Pacientes.Update(pac);
                    db.SaveChanges();
                }
                else
                {
                    // El paciente no existe, asi que creamos un nuevo record.
                    using var db = new HistoriaContext();
                    db.Database.EnsureCreatedAsync();
                    db.Pacientes.Add(pac);
                    db.SaveChanges();
                }
            }
            catch(DbUpdateConcurrencyException ex)
            {
                Console.Error.WriteLine(ex.Message);
                throw ex;
            }
            catch (DbUpdateException ex)
            {
                Console.Error.WriteLine(ex.Message);
                throw ex;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                throw e;
            }
        }

        // Método para crear o actualizar una ficha diaria
        public static void InsertFichaDiaria(FichaDiaria fd)
        {
            try
            {
                FichaDiaria? fd_encontrada = SelectFichaDiaria(fd);
                if (fd_encontrada != null)
                {
                    // La fichaDiaria existe, asi que actualizamos el record.
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
                    // La fichaDiaria no existe, asi que creamos un nuevo record.
                    using var db = new HistoriaContext();
                    db.Database.EnsureCreatedAsync();
                    db.FichasDiarias.Add(fd);
                    db.SaveChanges();
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Console.Error.WriteLine(ex.Message);
                throw ex;
            }
            catch (DbUpdateException ex)
            {
                Console.Error.WriteLine(ex.Message);
                throw ex;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                throw e;
            }
        }
        // Método para obtener una ficha diaria
        public static FichaDiaria? SelectFichaDiaria(FichaDiaria fd)
        {
            try
            {
                FichaDiaria? fd_encontrada;
                using (var db = new HistoriaContext())
                {
                    db.Database.EnsureCreatedAsync();
                    fd_encontrada = db.FichasDiarias.Where(p => (p.Dni == fd.Dni) && (p.Fecha == fd.Fecha)).FirstOrDefault();
                }
                return fd_encontrada;
            }
            catch (DbUpdateException ex)
            {
                Console.Error.WriteLine(ex.Message);
                throw ex;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                throw e;
            }
        }
        // Método para eliminar una ficha diaria
        public static void DeleteFichaDiaria(FichaDiaria fd)
        {
            try
            {
                using var db = new HistoriaContext();
                db.Database.EnsureCreatedAsync();
                db.FichasDiarias.Attach(fd);
                db.FichasDiarias.Remove(fd);
                db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                Console.Error.WriteLine(ex.Message);
                throw ex;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                throw e;
            }
        }
    }
}