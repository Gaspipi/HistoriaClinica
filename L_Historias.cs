using HistoriaClinica.Contexts;
using HistoriaClinica.Models;
using System.Data;
using System.Data.Odbc;
using System.Net;

namespace HistoriaClinica
{
    public class L_Historias
    {
        OdbcConnection SqlCon = new();
        public void StartConnection()
        {
            SqlCon = Connection.GetInstancia().CreateConnection();
        }
        public string[] DevDniCollection()
        {
            string[] pacientes;
            using (var db = new HistoriaContext())
            {
                db.Database.EnsureCreatedAsync();
                pacientes = db.Pacientes.Select(_ => _.Dni).ToArray();

            }
            return pacientes;
        }
        public IList<FichaDiaria> DevListadoFichas(string dni)
        {
            IList<FichaDiaria> listaFichas;
            using (var db = new HistoriaContext())
            {
                db.Database.EnsureCreatedAsync();
                listaFichas = db.FichasDiarias.Where(fd => fd.Dni == dni).ToList();

            }
            return listaFichas;
        }
        public Paciente? DevDatosPaciente(string dni)
        {
            Paciente pac;
            using (var db = new HistoriaContext())
            {
                db.Database.EnsureCreatedAsync();
                pac = db.Pacientes.Where(p => p.Dni == dni).SingleOrDefault();
            }
            return pac;
        }
        public void SetPaciente(Paciente pac)
        {
            Paciente? pacEncontrado = DevDatosPaciente(pac.Dni);
            if (pacEncontrado != null)
            {
                if (pacEncontrado.Dni != "" && pacEncontrado.Dni != "N/A")
                {
                    using (var db = new HistoriaContext())
                    {
                        db.Database.EnsureCreatedAsync();
                        db.Pacientes.Update(pac);
                        db.SaveChanges();
                    }
                }
            }
            else
            {
                using (var db = new HistoriaContext())
                {
                    db.Database.EnsureCreatedAsync();
                    db.Pacientes.Add(pac);
                    db.SaveChanges();
                }
            }
        }

        public void SetFichaDiaria(FichaDiaria fd)
        {
            FichaDiaria? fd_encontrada = DevFichaDiaria(fd);
            if (fd_encontrada != null)
            {
                using (var db = new HistoriaContext())
                {
                    db.Database.EnsureCreatedAsync();
                    db.FichasDiarias.Update(fd);
                    db.SaveChanges();
                }
            }
            else
            {
                using (var db = new HistoriaContext())
                {
                    db.Database.EnsureCreatedAsync();
                    db.FichasDiarias.Add(fd);
                    db.SaveChanges();
                }
            }
        }
        public FichaDiaria? DevFichaDiaria(FichaDiaria fd)
        {
            FichaDiaria? fd_encontrada;
            string dni = fd.Dni;
            string fecha = fd.GetFecha();
            using (var db = new HistoriaContext())
            {
                db.Database.EnsureCreatedAsync();
                fd_encontrada = db.FichasDiarias.Where(f => f.Dni == dni && f.GetFecha() == fecha).SingleOrDefault();
            }
            return fd_encontrada;
        }
        public void DelFicha(FichaDiaria fd)
        {
            string fecha = fd.GetFecha();
            using (var db = new HistoriaContext())
            {
                db.Database.EnsureCreatedAsync();
                db.FichasDiarias.Remove(fd);
                db.SaveChanges();
            }

        }
    }
}
