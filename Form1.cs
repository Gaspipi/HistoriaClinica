using System.Globalization;
using WinFormsApp1;

namespace AppParaMama
{

    public partial class AppClinica : Form
    {
        public AppClinica()
        {
            InitializeComponent();
        }
        private void dni_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void AppClinica_Load(object sender, EventArgs e)
        {

        }

        private void BirthDate_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void Search_Click(object sender, EventArgs e)
        {
            
            if (DniTextBox.Text.Length < 8)
            {
                MessageBox.Show("El campo DNI debe tener 8 digitos", "Error - Dni invalido", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                ListadoFichas();
            }
            int cant = FichasDiariasListBox.Items.Count;
            if (cant < 1)
            {
                EditButton.Enabled = false;
            }
            else
            {
                EditButton.Enabled = true;
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            CultureInfo myCIintl = new("es-ES", false);
            string date = FichasDiariasListBox.Items[FichasDiariasListBox.SelectedIndex].ToString();
            if (FichasDiariasListBox.Items.Count > 0 && date != null)
            {
                DateTime RealDate = DateTime.Parse(date, myCIintl);
                BuscarFicha(RealDate);
            }

        }

        private void DniLabel_Click(object sender, EventArgs e)
        {

        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            if (TabControl.SelectedIndex == 0)
            {
             
            }
            if (TabControl.SelectedIndex == 1)
            {
                CultureInfo myCIintl = new("es-ES", false);
                string fecha = FichasDiariasListBox.Items[FichasDiariasListBox.SelectedIndex].ToString();

                FichaDiaria fd = new();
                DateTime date = DateTime.Parse(fecha, myCIintl);
                fd.CreaFichadiaria(DniTextBox.Text, EnfermedadTextBox.Text, MotivoTextBox.Text, date, IndicacionesTextBox.Text);//ARREGLAR
                NewFicha fic = new();
                fic.ShowData(fd);
                fic.Datos = Datos;
                fic.Show();

            }
        }

        private void SaveButton2_Click(object sender, EventArgs e)
        {
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            
        }

        private void DniTextBox_TextChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < DniTextBox.Text.Length; i++)
            {
                if (!char.IsDigit(DniTextBox.Text[i]))
                {
                    MessageBox.Show("El campo DNI debe tener 8 digitos numericos", "Error - Dni invalido", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    DniTextBox.Text = DniTextBox.Text.Remove(DniTextBox.Text.Length - 1);
                    DniTextBox.Select(DniTextBox.Text.Length, 0);

                    break;
                }
            }
        }

        private void label7_Click_1(object sender, EventArgs e)
        {

        }
        private void NewPacienteButton_Click(object sender, EventArgs e)
        {
            var nuevo = new NewUser();
            nuevo.Show();
            nuevo.Datos = Datos;
        }
        #region
        public L_Historias Datos = new();
        private void BuscarFicha(DateTime date)
        {
            FichaDiaria fd = new();
            fd.CreaFichadiaria(DniTextBox.Text, EnfermedadTextBox.Text, MotivoTextBox.Text, date, IndicacionesTextBox.Text);
            FichaDiaria newfd = Datos.DevFichaDiaria(fd);
            MotivoTextBox.Text = newfd.DevMotivo();
            EnfermedadTextBox.Text = newfd.DevEnfermedad();
            IndicacionesTextBox.Text = newfd.DevIndicaciones();
        }
        private void ListadoFichas()
        {
            dataGridView1.DataSource = Datos.DevTabla(DniTextBox.Text);
            FichasDiariasListBox.DataSource = Datos.DevListadoFichas(DniTextBox.Text);
            Paciente Pac = new();
            Pac = Datos.DevDatosPaciente(DniTextBox.Text);
            if (Pac == null)
            {
                DialogResult dialogResult = MessageBox.Show("No existe el paciente especificado, desea crear uno nuevo?", "No existe el paciente", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    var nuevo = new NewUser();
                    nuevo.Show();
                    nuevo.ShowDni(DniTextBox.Text);
                    nuevo.Datos = Datos;
                }
            }
            else
            {
                VolcarDatos(Pac);
            }

        }
        private void VolcarDatos(Paciente Pac)
        {
            FirstNameTextBox.Text = Pac.DevFirstName();
            LastNameTextBox.Text = Pac.DevLastName();
            BirthDateTextBox.Text = Pac.DevDOB();
            PhoneTextBox.Text = Pac.DevPhone();
            ObraSocialTextBox.Text = Pac.DevObraSocial();
            NroAsociadoTextBox.Text = Pac.DevNroSocio();
            AntecFamiTextBox.Text = Pac.DevAntecFam();
            AntecPersTextBox.Text = Pac.DevAntecPers();
        }
        private void CrearPaciente(Paciente Pac)
        {
            Datos.SetPaciente(Pac);
        }
        #endregion

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void NroAsociadoTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void BirthDateTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void BirthDateTextBox_TextChanged_1(object sender, EventArgs e)
        {
            for (int i = 0; i < BirthDateTextBox.Text.Length; i++)
            {
                if (!char.IsDigit(BirthDateTextBox.Text[i]))
                {
                    if (i == 2 || i == 5) { }
                    else
                    {
                        MessageBox.Show("El campo Fecha debe tener  un formato valido del tipo dd/mm/yyyy", "Error - Fecha invalida", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        BirthDateTextBox.Text = BirthDateTextBox.Text.Remove(BirthDateTextBox.Text.Length - 1);
                        BirthDateTextBox.Select(BirthDateTextBox.Text.Length, 0);
                        break;
                    }
                }
            }
            if (BirthDateTextBox.Text.Length > 2 && BirthDateTextBox.Text[2] != '/')
            {
                BirthDateTextBox.Text = BirthDateTextBox.Text.Remove(BirthDateTextBox.Text.Length - 1);
                BirthDateTextBox.Text += '/';
                BirthDateTextBox.Select(BirthDateTextBox.Text.Length, 0);

            }
            if (BirthDateTextBox.Text.Length > 5 && BirthDateTextBox.Text[5] != '/')
            {
                BirthDateTextBox.Text = BirthDateTextBox.Text.Remove(BirthDateTextBox.Text.Length - 1);
                BirthDateTextBox.Text += '/';
                BirthDateTextBox.Select(BirthDateTextBox.Text.Length, 0);

            }
        }

        private void PhoneTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void NewButton_Click(object sender, EventArgs e)
        {
            var nuevo = new NewFicha();
            nuevo.Show();
            FichaDiaria fichaDiaria = new FichaDiaria();
            fichaDiaria.CreaFichadiaria(DniTextBox.Text, "", "", DateTime.Now, "");
            nuevo.ShowData(fichaDiaria);
            nuevo.Datos = Datos;
        }

        private void HistoriaClinica_Click(object sender, EventArgs e)
        {

        }
    }
}
