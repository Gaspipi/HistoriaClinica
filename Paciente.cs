using System.Globalization;

namespace AppParaMama
{
    public class Paciente
    {

        string Dni = "";
        string FirstName = "";
        string LastName = "";
        DateTime DOB = DateTime.MinValue;
        string Phone = "";
        string ObraSocial = "";
        string NroSocio = "";
        string AntecFam = "";
        string AntecPers = "";
        string Medicacion = "";

        public void CreaPaciente(string doc, string Fn, string Ln, string Os, string Nro, DateTime Nac, string Ph, string AntFm, string AntPer, string Med)
        {
            Dni = doc;
            FirstName = Fn;
            LastName = Ln;
            Phone = Ph;
            ObraSocial = Os;
            NroSocio = Nro;
            DOB = Nac;
            AntecFam = AntFm;
            AntecPers = AntPer;
            Medicacion = Med;
        }
        public void CreaPaciente(string doc, string Fn, string Ln, string Os, string Nro, string Nac, string Ph, string AntFm, string AntPer, string Med)
        {
            CultureInfo myCIintl = new("es-ES", false);
            Dni = doc;
            FirstName = Fn;
            LastName = Ln;
            Phone = Ph;
            ObraSocial = Os;
            NroSocio = Nro;
            if (Nac == "00/00/0000" || Nac == "")
            {
                DOB = DateTime.MinValue;
            }
            else
            {
                DOB = DateTime.Parse(Nac, myCIintl);
            }
            AntecFam = AntFm;
            AntecPers = AntPer;
            Medicacion = Med;
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
