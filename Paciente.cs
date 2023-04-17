using System.Globalization;

namespace AppParaMama
{
    public class Paciente
    {

        private string _dni = "";
        private string _firstName = "";
        private string _lastName = "";
        private DateTime _dob = DateTime.MinValue;
        private string _phone = "";
        private string _obraSocial = "";
        private string _nroSocio = "";
        private string _antecFam = "";
        private string _antecPers = "";
        private string _medicacion = "";

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

        public void CreaPaciente(string doc, string fn, string ln, string os, string nro, DateTime nac, string ph, string antFm, string antPer, string med)
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
        public void CreaPaciente(string doc, string fn, string ln, string os, string nro, string nac, string ph, string antFm, string antPer, string med)
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
        public string DevDni()
        {
            return Dni;
        }
        public string DevMed()
        {
            return Medicacion;
        }
        public string DevFirstName()
        {
            return FirstName;
        }
        public string DevLastName()
        {
            return LastName;
        }
        public string DevPhone()
        {
            return Phone;
        }
        public string DevObraSocial()
        {
            return ObraSocial;
        }
        public string DevNroSocio()
        {
            return NroSocio;
        }
        public string DevDOB()
        {
            string Date = DOB.Date.ToString("dd/MM/yyyy");
            return Date;
        }
        public DateTime DevDateTime()
        {
            return DOB;
        }
        public string DevAntecFam()
        {
            return AntecFam;
        }
        public string DevAntecPers()
        {
            return AntecPers;
        }
    }

}
