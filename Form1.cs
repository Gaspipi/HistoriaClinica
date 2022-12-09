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
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void DniLabel_Click(object sender, EventArgs e)
        {

        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            if (TabControl.SelectedIndex == 0)
            {
                FirstNameTextBox.ReadOnly = false;
                LastNameTextBox.ReadOnly = false;
                ObraSocialTextBox.ReadOnly = false;
                NroAsociadoTextBox.ReadOnly = false;
                BirthDateTextBox.ReadOnly = false;
                PhoneTextBox.ReadOnly = false;
                SaveButton.Enabled = true;
            }
            if (TabControl.SelectedIndex == 1)
            {
                SaveButton2.Enabled = true;
                EnfermedadTextBox.ReadOnly = false;
                MotivoTextBox.ReadOnly = false;

            }
        }

        private void SaveButton2_Click(object sender, EventArgs e)
        {
            MotivoTextBox.ReadOnly = true;
            EnfermedadTextBox.ReadOnly = true;
            SaveButton2.Enabled = false;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (DniTextBox.Text.Length < 8)
            {
                MessageBox.Show("El campo DNI debe tener 8 digitos", "Error - Dni invalido", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                CrearPaciente();
                SaveButton.Enabled = false;
                FirstNameTextBox.ReadOnly = true;
                LastNameTextBox.ReadOnly = true;
                ObraSocialTextBox.ReadOnly = true;
                NroAsociadoTextBox.ReadOnly = true;
                BirthDateTextBox.ReadOnly = true;
            }
        }

        private void DniTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void label7_Click_1(object sender, EventArgs e)
        {
        }

        private void NewPacienteButton_Click(object sender, EventArgs e)
        {
            
        }
        #region
        L_Historias Datos = new();
        private void ListadoFichas()
        {
            dataGridView1.DataSource = Datos.DevTabla(DniTextBox.Text);
            FichasDiariasListBox.DataSource = Datos.DevListadoFichas(DniTextBox.Text);
            Paciente Pac = new();
            Pac = Datos.DevDatosPaciente(DniTextBox.Text);

            VolcarDatos(Pac);
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
        private void CrearPaciente()
        {
            Paciente Pac = new();
            Pac.CreaPaciente(DniTextBox.Text, FirstNameTextBox.Text, LastNameTextBox.Text, ObraSocialTextBox.Text, NroAsociadoTextBox.Text, BirthDateTextBox.Text, PhoneTextBox.Text, AntecFamiTextBox.Text, AntecPersTextBox.Text);
            Datos.SetPaciente(Pac);
        }
        #endregion

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void NroAsociadoTextBox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
