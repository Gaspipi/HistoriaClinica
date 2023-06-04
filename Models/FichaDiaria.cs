using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HistoriaClinica.Models
{
    public class FichaDiaria
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("Dni"), StringLength(8), Required]
        public string Dni { get; set; }
        public string Enfermedad { get; set; }
        public string Motivo { get; set; }
        public string Indicaciones { get; set; }
        public DateTime Fecha { get; set; }


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
