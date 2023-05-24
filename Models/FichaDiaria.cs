using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HistoriaClinica.Models
{
    public class FichaDiaria
    {
        private int _id { get; set; }
        [ForeignKey("Dni"), StringLength(8)]
        private string _dni { get; set; }
        private string _enfermedad { get; set; }
        private string _motivo { get; set; }
        private string _indicaciones { get; set; }
        private DateTime _fecha { get; set; }

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
        public FichaDiaria() { }

        public FichaDiaria(string doc, string enf, string mot, string fec, string indic)
        {
            Dni = doc;
            Enfermedad = $@"{enf}";
            Motivo = $@"{mot}";
            Fecha = DateTime.Parse(fec);
            Indicaciones = $@"{indic}";
        }

        public FichaDiaria(string doc, string enf, string mot, DateTime fec, string indic)
        {
            Dni = doc;
            Enfermedad = $@"{enf}";
            Motivo = $@"{mot}";
            Fecha = fec;
            Indicaciones = $@"{indic}";
        }

        public string GetFecha()
        {
            string date = Fecha.ToString("dd/MM/yyyy HH:mm:ss");
            return date;
        }
    }
}
