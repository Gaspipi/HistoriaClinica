using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsApp1.Models
{
    public class Pacientes
    { 
        public string Dni { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string ObraSocial { get; set; }
        public string NroSocio { get; set; }
        public DateTime FechaNac { get; set; }
        public string Telefono { get; set; }
        public string AntecFam { get; set; }
        public string AntecPers { get; set; }
    }
}
