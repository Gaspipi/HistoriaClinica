using System.Globalization;

namespace WinFormsApp1
{
    public class FichaDiaria
    {
        string Dni = "";
        string Enfermedad = "";
        string Motivo = "";
        string Indicaciones = "";
        DateTime Fecha;
        public void CreaFichadiaria(string doc, string Enf, string Mot, string Fec, string Indic)
        {
            Dni = doc;
            Enfermedad = Enf;
            Motivo = Mot;
            Fecha = DateTime.Parse(Fec);
            Indicaciones = Indic;
        }
        public void CreaFichadiaria(string doc, string Enf, string Mot, DateTime Fec, string Indic)
        {
            Dni = doc;
            Enfermedad = Enf;
            Motivo = Mot;
            Fecha = Fec;
            Indicaciones = Indic;
        }
        public string DevDni()
        {
            return Dni;
        }
        public string DevMotivo() { 
            return Motivo; 
        }
        public string DevEnfermedad() { 
            return Enfermedad; 
        }
        public string DevFecha()
        {
            string date = Fecha.ToString();
            return date;
        }
        public string DevFecha2()
        {
            string date = Fecha.ToString();
            return date;
        }

        public string DevIndicaciones() { 
            return Indicaciones; 
        }

    }
}
