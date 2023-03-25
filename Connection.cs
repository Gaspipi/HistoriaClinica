using System.Data.Odbc;

namespace WinFormsApp1
{
    public class Connection
    {
        private static Connection Con = null;
        private readonly string ConnectionString = "Driver={Microsoft Access Driver (*.mdb, *.accdb)}; Dbq=" + Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Historias.accdb; Uid = Admin; Pwd =; ";
        
        public OdbcConnection CreateConnection()
        {
            OdbcConnection Cadena = new();
            try
            {
                Cadena.ConnectionString = ConnectionString;
            }
            catch (Exception e)
            {
                throw e;
            }
            return Cadena;
        }
        public static Connection GetInstancia()
        {
            if (Con == null)
            {
                Con = new();
            }
            return Con;
        }


    }
}
