namespace HistoriaClinica
{
    public partial class NewUser : Form
    {
        private AppClinica AppCli { get; set; }
        public CRUD_Historias? Datos { get; set; } = null;
        public NewUser(AppClinica AppCli)
        {
            InitializeComponent();
            AppCli = AppCli;
        }
        public void Edit()
        {
            DniTextBox.Enabled = false;
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
        private void SaveButton_Click(object sender, EventArgs e)
        {
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
                    MedicacionTextBox.ReadOnly = true;
                    Paciente Pac = new(DniTextBox.Text, FirstNameTextBox.Text, LastNameTextBox.Text, ObraSocialTextBox.Text, NroAsociadoTextBox.Text, BirthDateTextBox.Text, PhoneTextBox.Text, AntecFamiTextBox.Text, AntecPersTextBox.Text, MedicacionTextBox.Text);
                    CRUD_Historias.InsertPaciente(Pac);
                    AppCli.ListadoFichas();
                    Close();
                }
            }
        }
        public void CargarData(Paciente pac)
        {
            FirstNameTextBox.Text = pac.FirstName;
            LastNameTextBox.Text = pac.LastName;
            BirthDateTextBox.Text = pac.DevDOB();
            DniTextBox.Text = pac.Dni;
            PhoneTextBox.Text = pac.Phone;
            ObraSocialTextBox.Text = pac.ObraSocial;
            NroAsociadoTextBox.Text = pac.NroSocio;
            AntecPersTextBox.Text = pac.AntecPers;
            AntecFamiTextBox.Text = pac.AntecFam;
            MedicacionTextBox.Text = pac.Medicacion;

        }
        public void ShowDni(string Dni)
        {
            DniTextBox.Text = Dni;
            DniTextBox.Enabled = false;
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
        private void CancelarButton_Click(object sender, EventArgs e)
        {
            DialogResult resp = MessageBox.Show($"Esta seguro de que desea Cancelar?", "Cancelar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (resp == DialogResult.Yes)
            {
                Close();
            }
        }
    }
}
