namespace WinFormsApp1
{
    public class FichaDiaria
    {
        string Dni = "";
        string Enfermedad = "";
        string Motivo = "";
        DateTime Fecha;
        public void CreaPaciente(string doc, string Fn, string Ln, DateTime Fec)
        {
            Dni = doc;
            Enfermedad = Fn;
            Motivo = Ln;
            Fecha = Fec;
        }

    }
}
