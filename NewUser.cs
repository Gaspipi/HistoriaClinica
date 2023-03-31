using AppParaMama;

namespace WinFormsApp1
{
    public partial class NewUser : Form
    {
        AppClinica AppCli;
        public NewUser()
        {
            InitializeComponent();
        }

        public void Edit()
        {
            DniTextBox.Enabled = false;
        }
        private void AntecFamiTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void PhoneTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void PhoneLabel_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void AntecPersTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void FirstNameTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void NroAsociadoLabel_Click(object sender, EventArgs e)
        {

        }

        private void BirthDateTextBox_TextChanged(object sender, EventArgs e)
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

        private void ObraSocialLabel_Click(object sender, EventArgs e)
        {

        }

        private void LastNameLabel_Click(object sender, EventArgs e)
        {

        }

        private void FirstNameLabel_Click(object sender, EventArgs e)
        {

        }

        private void BirthLabel_Click(object sender, EventArgs e)
        {

        }

        private void NroAsociadoTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void ObraSocialTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void LastNameTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            Paciente Pac = new();
            if (DniTextBox.Text.Length < 8)
            {
                MessageBox.Show("El campo DNI debe tener 8 digitos", "Error - Dni invalido", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (BirthDateTextBox.Text.Length < 10 && BirthDateTextBox.Text.Length > 0)
                {
                    MessageBox.Show("El campo Fecha debe tener 10 digitos o quedar vacio", "Error - Fecha invalido", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    SaveButton.Enabled = false;
                    FirstNameTextBox.ReadOnly = true;
                    LastNameTextBox.ReadOnly = true;
                    ObraSocialTextBox.ReadOnly = true;
                    NroAsociadoTextBox.ReadOnly = true;
                    BirthDateTextBox.ReadOnly = true;
                    PhoneTextBox.ReadOnly = true;
                    AntecFamiTextBox.ReadOnly = true;
                    AntecPersTextBox.ReadOnly = true;
                    Medicacion_TextBox.ReadOnly = true;
                    Pac.CreaPaciente(DniTextBox.Text, FirstNameTextBox.Text, LastNameTextBox.Text, ObraSocialTextBox.Text, NroAsociadoTextBox.Text, BirthDateTextBox.Text, PhoneTextBox.Text, AntecFamiTextBox.Text, AntecPersTextBox.Text, Medicacion_TextBox.Text);
                    Datos.SetPaciente(Pac);
                    Close();
                }
            }
        }

        #region
        public L_Historias Datos = null;
        public void Setapp(AppClinica app)
        {
            AppCli = app;
        }
        public AppClinica Devapp()
        {
            return AppCli;
        }

        public void CargarData(Paciente pac)
        {
            FirstNameTextBox.Text = pac.DevFirstName();
            LastNameTextBox.Text = pac.DevLastName();
            BirthDateTextBox.Text = pac.DevDOB();
            DniTextBox.Text = pac.DevDni();
            PhoneTextBox.Text = pac.DevPhone();
            ObraSocialTextBox.Text = pac.DevObraSocial();
            NroAsociadoTextBox.Text = pac.DevNroSocio();
            AntecPersTextBox.Text = pac.DevAntecPers();
            AntecFamiTextBox.Text = pac.DevAntecFam();
            Medicacion_TextBox.Text = pac.DevMed();

        }
        #endregion
        public void ShowDni(string Dni)
        {
            DniTextBox.Text = Dni;
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

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void NewUser_FormClosed(object sender, FormClosedEventArgs e)
        {
            AppClinica app = Devapp();
            app.ListadoFichas();
        }

        private void NewUser_Load(object sender, EventArgs e)
        {

        }

        private void NewUser_FormClosing(object sender, FormClosingEventArgs e)
        {

        }
    }
}
