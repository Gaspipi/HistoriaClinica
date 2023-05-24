using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace HistoriaClinica
{
    public class Paciente
    {
        [Required]
        private int _id { get; set; }
        [Key, Required]
        private string _dni { get; set; }
        private string _firstName { get; set; }
        private string _lastName { get; set; }
        private DateTime _dob = DateTime.MinValue;
        private string _phone { get; set; }
        private string _obraSocial { get; set; }
        private string _nroSocio { get; set; }
        private string _antecFam { get; set; }
        private string _antecPers { get; set; }
        private string _medicacion { get; set; }

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Dni
        {
            get { return _dni; }
            set { _dni = value; }
        }

        public string FirstName
        {
            get { return _firstName; }
            set { _firstName = value; }
        }

        public string LastName
        {
            get { return _lastName; }
            set { _lastName = value; }
        }

        public DateTime DOB
        {
            get { return _dob; }
            set { _dob = value; }
        }

        public string Phone
        {
            get { return _phone; }
            set { _phone = value; }
        }

        public string ObraSocial
        {
            get { return _obraSocial; }
            set { _obraSocial = value; }
        }

        public string NroSocio
        {
            get { return _nroSocio; }
            set { _nroSocio = value; }
        }

        public string AntecFam
        {
            get { return _antecFam; }
            set { _antecFam = value; }
        }

        public string AntecPers
        {
            get { return _antecPers; }
            set { _antecPers = value; }
        }

        public string Medicacion
        {
            get { return _medicacion; }
            set { _medicacion = value; }
        }

        public Paciente() { }

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
        public Paciente(string doc, string fn, string ln, string os, string nro, string nac, string ph, string antFm, string antPer, string med)
        {
            CultureInfo myCIintl = new("es-ES", false);
            Dni = doc;
            FirstName = fn;
            LastName = ln;
            Phone = ph;
            ObraSocial = os;
            NroSocio = nro;
            if (nac == "00/00/0000" || nac == "")
            {
                DOB = DateTime.MinValue;
            }
            else
            {
                DOB = DateTime.Parse(nac, myCIintl);
            }
            AntecFam = antFm;
            AntecPers = antPer;
            Medicacion = med;
        }

        public string DevDOB()
        {
            string Date = DOB.Date.ToString("dd/MM/yyyy");
            return Date;
        }
    }
}
