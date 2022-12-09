using System.Text;
using WinFormsApp1;

namespace AppParaMama
{
    internal class TxtManager
    {
        string Dir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Historias Clinicas";

        public void CreateFile(Paciente Pac)
        {
            var path = Path.Combine(Dir, Pac.DevDni() + ".pac");

            using (FileStream fs = File.Create(path))
            {
                AddText(fs, Pac.DevDni());
                AddText(fs, Pac.DevFirstName());
                AddText(fs, Pac.DevLastName());
                AddText(fs, Pac.DevDOB());
                AddText(fs, Pac.DevPhone());
                AddText(fs, Pac.DevObraSocial());
                AddText(fs, Pac.DevNroSocio());
            }

            AddPac(Pac);
        }
        public void AddPac(Paciente Pac)
        {
            var path = Path.Combine(Dir, "Pacientes.dat");
            string DniDir = Pac.DevDni() + "|" + Path.Combine(Dir, Pac.DevDni());
            AppendText(path, DniDir);

        }
        public void CreateFile(Paciente Pac, Historia Hist)
        {
            string Path = System.IO.Path.Combine(Dir, Pac.DevDni());
            Directory.CreateDirectory(Path);
            using (FileStream fs = File.Create(Path))
            {

            }
        }
        public void CreateFile(Paciente Pac, FichaDiaria Fic)
        {
            using (FileStream fs = File.Create(Dir + "\\" + Pac.DevDni()))
            {

            }
        }
        public string[] ReadFile(Paciente Pac)
        {
            string[] lines = File.ReadAllLines(Dir + "\\" + Pac.DevDni());
            return lines;
        }
        public string[] ReadFile(Paciente Pac, Historia Hist)
        {
            string Path = System.IO.Path.Combine(Dir, Pac.DevDni());
            string[] lines = File.ReadAllLines(Path);
            return lines;
        }
        public string[] ReadFile(Paciente Pac, FichaDiaria Fic)
        {
            string[] lines = File.ReadAllLines(Dir + "\\" + Pac.DevDni());
            return lines;
        }

        private static void AppendText(string Dir, string str)
        {
            using (StreamWriter sw = File.AppendText(Dir))
            {
                sw.WriteLine(str);
            }
        }
        private static void AddText(FileStream fs, string text)
        {
            fs.Seek(fs.Position,
           SeekOrigin.Begin);
            byte[] info = new UTF8Encoding().GetBytes(text + Environment.NewLine);
            fs.Write(info, 0, info.Length);
        }
        public Paciente Search(string a)
        {
            Paciente Pac = new();
            a = a.ToLower();
            return Pac;
        }
    }
}
