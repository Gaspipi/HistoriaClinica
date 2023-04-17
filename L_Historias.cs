using AppParaMama;
using System.Data;
using System.Data.Odbc;
using System.Globalization;

namespace WinFormsApp1
{
    public class L_Historias
    {
        OdbcConnection SqlCon = new();
        public void StartConnection()
        {
            SqlCon = Connection.GetInstancia().CreateConnection();
        }
        public DataTable DevTabla(string Dni)
        {
            OdbcDataReader Reader;
            DataTable Table = new DataTable();
            OdbcConnection SqlCon = new();
            try
            {
                SqlCon = Connection.GetInstancia().CreateConnection();
                string SqlQuery = $"SELECT * FROM FichasDiarias WHERE Dni='{Dni}';";
                OdbcCommand cmd = new(SqlQuery, SqlCon);
                SqlCon.Open();
                Reader = cmd.ExecuteReader();
                Table.Load(Reader);
                return Table;
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
                throw;

            }
            finally
            {
                if (SqlCon.State == ConnectionState.Open) SqlCon.Close();
            }

        }
        public string[] DevDniCollection()
        {
            OdbcDataReader Reader;
            OdbcConnection SqlCon = new();
            List<string> List = new();
            try
            {
                SqlCon = Connection.GetInstancia().CreateConnection();
                string SqlQuery = $"SELECT Dni FROM Pacientes;";
                OdbcCommand cmd = new(SqlQuery, SqlCon);
                SqlCon.Open();
                if (SqlCon.State == ConnectionState.Open)
                {
                    Reader = cmd.ExecuteReader();
                    while (Reader.Read())
                    {
                        string dni = (string)Reader[0];
                        List.Add(dni);
                    }
                    return List.ToArray();
                }
                else return List.ToArray();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw;
            }
            finally
            {
                if (SqlCon.State == ConnectionState.Open) SqlCon.Close();
            }

        }
        public IList<string> DevListadoFichas(string Dni)
        {
            OdbcDataReader Reader;
            DataTable Table = new DataTable();
            OdbcConnection SqlCon = new();
            try
            {
                SqlCon = Connection.GetInstancia().CreateConnection();
                string SqlQuery = $"SELECT * FROM FichasDiarias WHERE Dni='{Dni}';";
                OdbcCommand cmd = new(SqlQuery, SqlCon);
                SqlCon.Open();
                List<string> List = new();
                List<DateTime> DateList = new();
                if (SqlCon.State == ConnectionState.Open)
                {
                    Reader = cmd.ExecuteReader();
                    while (Reader.Read())
                    {
                        DateTime dt = (DateTime)Reader[4];
                        DateList.Add(dt);
                    }
                    DateList = DateList.OrderBy(dt => dt.Date).ToList();
                    foreach (DateTime dt in DateList)
                    {
                        string st = dt.ToString();
                        List.Add(st);
                    }



                    return List;
                }
                else return List;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw;
            }
            finally
            {
                if (SqlCon.State == ConnectionState.Open) SqlCon.Close();
            }
        }
        public Paciente DevDatosPaciente(string Dni)
        {
            Paciente Pac = new Paciente();
            OdbcDataReader Reader;
            try
            {
                SqlCon = Connection.GetInstancia().CreateConnection();
                string SqlQuery = $"SELECT * FROM Pacientes WHERE Dni='{Dni}';";
                OdbcCommand cmd = new(SqlQuery, SqlCon);
                if (SqlCon.State == ConnectionState.Closed)
                {
                    SqlCon.Open();
                }
                if (SqlCon.State == ConnectionState.Open)
                {
                    Reader = cmd.ExecuteReader();
                    if (Reader.HasRows)
                    {
                        Reader.Read();
                        object[] p = new object[] { "", "", "", "", "", "", "", "", "", "", ""};
                        for (int i = 1; i < 11; i++)
                        {
                            if (i != 6)
                            {
                                if (Reader[i] != DBNull.Value)
                                {
                                    p[i] = (string)Reader[i];
                                }
                                else
                                {
                                    p[i] = "N/A";
                                }


                            }
                            if (i == 6)
                            {
                                if (Reader[i] != DBNull.Value)
                                {
                                    p[i] = (DateTime)Reader[i];
                                }
                                else
                                {
                                    p[i] = DateTime.MinValue;
                                }

                            }
                        }
                        if (Reader[0] == DBNull.Value)
                        {
                            Pac = null;
                        }
                        else
                        {
                            string doc = (string)p[1];
                            string Fn = (string)p[2];
                            string Ln = (string)p[3];
                            string Os = (string)p[4];
                            string Nro = (string)p[5];
                            DateTime Date = (DateTime)p[6];
                            string Ph = (string)p[7];
                            string AntFm = (string)p[8];
                            string AntPers = (string)p[9];
                            string Med = (string)p[10];
                            Pac.CreaPaciente(doc, Fn, Ln, Os, Nro, Date, Ph, AntFm, AntPers, Med);
                        }
                    }
                    else
                    {
                        Pac = null;
                    }
                }
                return Pac;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw;
            }
            finally
            {
                if (SqlCon.State == ConnectionState.Open) SqlCon.Close();
            }

        }
        public void SetPaciente(Paciente Pac)
        {
            Paciente paciente = DevDatosPaciente(Pac.DevDni());
            if (paciente != null)
            {
                if (paciente.DevDni() != "" && paciente.DevDni() != "N/A")
                {
                    try
                    {
                        string SqlQuery = $"UPDATE Pacientes SET Dni = '{Pac.DevDni()}',Nombre = '{Pac.DevFirstName()}',Apellido = '{Pac.DevLastName()}',ObraSocial = '{Pac.DevObraSocial()}',NroSocio = '{Pac.DevNroSocio()}',FechaNac = '{Pac.DevDateTime()}',Telefono = '{Pac.DevPhone()}',AntecFam = '{Pac.DevAntecFam()}',AntecPers = '{Pac.DevAntecPers()}', Medicacion = '{Pac.DevMed()}' WHERE Dni = '{Pac.DevDni()}'";
                        OdbcCommand cmd = new(SqlQuery, SqlCon);
                        if (SqlCon.State == ConnectionState.Closed)
                        {
                            SqlCon.Open();
                        }

                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        throw;
                    }
                    finally
                    {
                        if (SqlCon.State == ConnectionState.Open) SqlCon.Close();

                    }
                }
            }
            else
            {

                try
                {
                    string SqlQuery = $"INSERT INTO Pacientes (Dni,Nombre,Apellido,ObraSocial,NroSocio,FechaNac,Telefono,AntecFam,AntecPers,Medicacion) " +
                        $"VALUES ('{Pac.DevDni()}','{Pac.DevFirstName()}','{Pac.DevLastName()}','{Pac.DevObraSocial()}','{Pac.DevNroSocio()}','{Pac.DevDateTime()}','{Pac.DevPhone()}','{Pac.DevAntecFam()}','{Pac.DevAntecPers()}', '{Pac.DevMed()}');";
                    OdbcCommand cmd = new(SqlQuery, SqlCon);
                    if (SqlCon.State == ConnectionState.Closed)
                    {
                        SqlCon.Open();
                    }
                    cmd.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    throw;
                }
                finally
                {
                    if (SqlCon.State == ConnectionState.Open) SqlCon.Close();

                }


            }

        }
        
        public void SetFichaDiaria(FichaDiaria Fd)
        {
            FichaDiaria ficha = DevFichaDiaria(Fd);
            if (ficha != null)
            {
                
                    try
                    {   
                        string SqlQuery = $"UPDATE FichasDiarias SET Motivo = '{Fd.DevMotivo()}', Enfermedad = '{Fd.DevEnfermedad()}', Dni = '{Fd.DevDni()}', Indicaciones = '{Fd.DevIndicaciones()}', FechaHora = #{Fd.DevFecha()}# WHERE Dni = '{Fd.DevDni()}' AND FechaHora = #{Fd.DevFecha()}#;";
                        OdbcCommand cmd = new(SqlQuery, SqlCon);
                        if (SqlCon.State == ConnectionState.Closed)
                        {
                            SqlCon.Open();
                        }

                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                    MessageBox.Show(ex.Message);
                    throw;
                }
                    finally
                    {
                        if (SqlCon.State == ConnectionState.Open) SqlCon.Close();

                    }
                
            }
            else
            {

                try
                {
                    string fecha = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    string SqlQuery = $"INSERT INTO FichasDiarias (Motivo, Enfermedad, Dni, Indicaciones, FechaHora) VALUES ('{Fd.DevMotivo()}','{Fd.DevEnfermedad()}','{Fd.DevDni()}','{Fd.DevIndicaciones()}',#{fecha}#);";
                    OdbcCommand cmd = new(SqlQuery, SqlCon);
                    if (SqlCon.State == ConnectionState.Closed)
                    {
                        SqlCon.Open();
                    }
                    cmd.ExecuteNonQuery();
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    throw;
                }
                finally
                {
                    if (SqlCon.State == ConnectionState.Open) SqlCon.Close();

                }


            }

        }
        public FichaDiaria DevFichaDiaria(FichaDiaria FicDia)
        {
            FichaDiaria fd = new();
            string dni = FicDia.DevDni();
            string fecha = FicDia.DevFecha();
            OdbcDataReader Reader;
            try
            {
                SqlCon = Connection.GetInstancia().CreateConnection();
                string SqlQuery = $"SELECT * FROM FichasDiarias WHERE Dni = '{dni}' AND FechaHora = #{fecha}#;";
                OdbcCommand cmd = new(SqlQuery, SqlCon);
                if (SqlCon.State == ConnectionState.Closed)
                {
                    SqlCon.Open();
                }
                if (SqlCon.State == ConnectionState.Open)
                {
                    Reader = cmd.ExecuteReader();
                    if (Reader.HasRows)
                    {
                        Reader.Read();
                        fd.CreaFichadiaria((string)Reader[2], (string)Reader[1], (string)Reader[0], (DateTime)Reader[4], (string)Reader[3]);
                    }
                    else
                    {
                        fd = null;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw;
            }
            finally
            {
                if (SqlCon.State == ConnectionState.Open) SqlCon.Close();
            }
            return fd;
        }
        public void DelFicha(FichaDiaria FicDia)
        {
            FichaDiaria fd = new();
            string fecha = FicDia.DevFecha();
            OdbcDataReader Reader;
            try
            {
                SqlCon = Connection.GetInstancia().CreateConnection();
                string SqlQuery = $"DELETE * FROM FichasDiarias WHERE Dni = '{FicDia.DevDni()}' AND FechaHora = #{fecha}#;";
                OdbcCommand cmd = new(SqlQuery, SqlCon);
                if (SqlCon.State == ConnectionState.Closed)
                {
                    SqlCon.Open();
                }
                if (SqlCon.State == ConnectionState.Open)
                {
                    Reader = cmd.ExecuteReader();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw;
            }
            finally
            {
                if (SqlCon.State == ConnectionState.Open) SqlCon.Close();
            }
        }
    }
}
