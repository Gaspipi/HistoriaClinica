using HistoriaClinica.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace HistoriaClinica
{
    public class Paciente
    {
        [Key, Required]
        public string Dni { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime DOB { get; set; }
        public string Phone { get; set; } = string.Empty;
        public string ObraSocial { get; set; } = string.Empty;
        public string NroSocio { get; set; } = string.Empty;
        public string AntecFam { get; set; } = string.Empty;
        public string AntecPers { get; set; } = string.Empty;
        public string Medicacion { get; set; } = string.Empty;
        public List<FichaDiaria> FichasDiarias { get; set; }

        private Paciente(string dni)
        {
            Dni = dni;
        }

        // Constructor de Paciente con parámetros completos
        public Paciente(string doc, string fn, string ln, string os, string nro, DateTime nac, string ph, string antFm, string antPer, string med)
        {
            Dni = doc;
            FirstName = fn;
            LastName = ln;
            Phone = ph;
            ObraSocial = os;
            NroSocio = nro;
            DOB = nac;
            AntecFam = antFm;
            AntecPers = antPer;
            Medicacion = med;
        }

        // Constructor de Paciente con parámetros de cadena de texto
        public Paciente(string doc, string fn, string ln, string os, string nro, string nac, string ph, string antFm, string antPer, string med)
        {
            CultureInfo myCIintl = new("es-ES", false);
            Dni = doc;
            FirstName = fn;
            LastName = ln;
            Phone = ph;
            ObraSocial = os;
            NroSocio = nro;

            // Intenta convertir la cadena de texto a DateTime usando el formato esperado
            if (DateTime.TryParse(nac, out DateTime dt))
            {
                DOB = dt;
            }
            else
            {
                DOB = DateTime.MinValue;
            }

            AntecFam = antFm;
            AntecPers = antPer;
            Medicacion = med;
        }

        // Método para obtener la fecha de nacimiento formateada
        public string DevDOB()
        {
            string Date = DOB.Date.ToString("dd/MM/yyyy");
            return Date;
        }
    }
}
