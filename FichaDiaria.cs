using System.Globalization;

namespace WinFormsApp1
{
    public class FichaDiaria
    {

        private string _dni = "";
        private string _enfermedad = "";
        private string _motivo = "";
        private string _indicaciones = "";
        private DateTime _fecha;
        
        public string Dni
        {
            get { return _dni; }
            set { _dni = value; }
        }

        public string Enfermedad
        {
            get { return _enfermedad; }
            set { _enfermedad = value; }
        }

        public string Motivo
        {
            get { return _motivo; }
            set { _motivo = value; }
        }

        public string Indicaciones
        {
            get { return _indicaciones; }
            set { _indicaciones = value; }
        }

        public DateTime Fecha
        {
            get { return _fecha; }
            set { _fecha = value; }
        }

        public void CreaFichadiaria(string doc, string enf, string mot, string fec, string indic)
        {
            Dni = doc;
            Enfermedad = $@"{enf}";
            Motivo = $@"{mot}";
            Fecha = DateTime.Parse(fec);
            Indicaciones = $@"{indic}";
        }
        public void CreaFichadiaria(string doc, string enf, string mot, DateTime fec, string indic)
        {
            Dni = doc;
            Enfermedad = $@"{enf}";
            Motivo = $@"{mot}";
            Fecha = fec;
            Indicaciones = $@"{indic}";
        }
        public string DevDni()
        {
            return Dni;
        }
        public string DevMotivo()
        {
            return Motivo;
        }
        public string DevEnfermedad()
        {
            return Enfermedad;
        }
        public string DevFecha()
        {
            string date = Fecha.ToString("dd/MM/yyyy HH:mm:ss");
            return date;
        }

        public string DevIndicaciones()
        {
            return Indicaciones;
        }
    }
}
