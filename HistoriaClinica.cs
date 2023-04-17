using WinFormsApp1;

namespace AppParaMama
{

    public partial class AppClinica : Form
    {
        public AppClinica()
        {
            InitializeComponent();
            if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Historias.accdb"))
            {
                string dir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Historias.accdb";
                MessageBox.Show("No existe el archivo de almacenamiento, procediendo a crearlo", "No existe el fichero", MessageBoxButtons.OK, MessageBoxIcon.Information);
                File.Copy("Historias.accdb", dir);
            }
            AutoFill = Datos.DevDniCollection();
            sugerenciasAutocompletado.AddRange(AutoFill);
            DniTextBox.AutoCompleteCustomSource = sugerenciasAutocompletado;
        }

        private void Search_Click(object sender, EventArgs e)
        {
            if (FichasDiariasListBox.Items.Count == 0)
            {
                MotivoTextBox.Text = string.Empty;
                EnfermedadTextBox.Text = string.Empty;
                IndicacionesTextBox.Text = string.Empty;
            }
            if (DniTextBox.Text.Length < 8)
            {
                MessageBox.Show("El campo DNI debe tener 8 digitos", "Error - Dni invalido", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                ListadoFichas();
            }
            EditEnabled();

        }

        public void EditEnabled()
        {
            if (TabControl.SelectedIndex == 0)
            {
                if (FirstNameTextBox.Text.Length == 0)
                {
                    EditButton.Enabled = false;
                }
                else { EditButton.Enabled = true; }
            }
            if (TabControl.SelectedIndex == 1)
            {
                if (FichasDiariasListBox.Items.Count == 0)
                {
                    MotivoTextBox.Text = string.Empty;
                    EnfermedadTextBox.Text = string.Empty;
                    IndicacionesTextBox.Text = string.Empty;
                    EditButton.Enabled = false;
                }
                else
                {
                    EditButton.Enabled = true;
                }
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

            string date = FichasDiariasListBox.Items[FichasDiariasListBox.SelectedIndex].ToString();
            if (FichasDiariasListBox.Items.Count > 0 && date != null)
            {
                EditButton.Enabled = true;
                BuscarFicha(date);
            }
            else
            {
                EditButton.Enabled = false;
            }

        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            if (TabControl.SelectedIndex == 0)
            {
                Paciente pac = new Paciente();
                pac.CreaPaciente(DniTextBox.Text, FirstNameTextBox.Text, LastNameTextBox.Text, ObraSocialTextBox.Text, NroAsociadoTextBox.Text, BirthDateTextBox.Text, PhoneTextBox.Text, AntecFamiTextBox.Text, AntecPersTextBox.Text, Medicacion_TextBox.Text);
                var nuevo = new NewUser();
                nuevo.Setapp(this);
                nuevo.Show();
                nuevo.Datos = Datos;
                nuevo.Edit();
                nuevo.CargarData(pac);

            }
            if (TabControl.SelectedIndex == 1)
            {
                string fecha = FichasDiariasListBox.Items[FichasDiariasListBox.SelectedIndex].ToString();
                if (!string.IsNullOrEmpty(fecha))
                {
                    FichaDiaria fd = new();
                    fd.CreaFichadiaria(DniTextBox.Text, EnfermedadTextBox.Text, MotivoTextBox.Text, fecha, IndicacionesTextBox.Text);
                    NewFicha fic = new();
                    fic.ShowData(fd);
                    fic.Setapp(this);
                    fic.Datos = Datos;
                    fic.Show();
                }


            }
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

        private void NewPacienteButton_Click(object sender, EventArgs e)
        {
            var nuevo = new NewUser();
            nuevo.Setapp(this);
            nuevo.Show();
            nuevo.Datos = Datos;

        }
        #region
        string[] AutoFill;
        AutoCompleteStringCollection sugerenciasAutocompletado = new AutoCompleteStringCollection();
        private L_Historias _datos = new L_Historias();

        public L_Historias Datos
        {
            get { return _datos; }
            set { _datos = value; }
        }
        private void BuscarFicha(string date)
        {
            FichaDiaria fd = new();
            fd.CreaFichadiaria(DniTextBox.Text, EnfermedadTextBox.Text, MotivoTextBox.Text, date, IndicacionesTextBox.Text);
            FichaDiaria newfd = Datos.DevFichaDiaria(fd);
            MotivoTextBox.Text = newfd.DevMotivo();
            EnfermedadTextBox.Text = newfd.DevEnfermedad();
            IndicacionesTextBox.Text = newfd.DevIndicaciones();
        }
        public void ListadoFichas()
        {
            AutoFill = Datos.DevDniCollection();
            sugerenciasAutocompletado.AddRange(AutoFill);
            DniTextBox.AutoCompleteCustomSource = sugerenciasAutocompletado;
            FichasDiariasListBox.DataSource = Datos.DevListadoFichas(DniTextBox.Text);
            FichasDiariasListBox.SelectedIndex = FichasDiariasListBox.Items.Count - 1;
            Paciente Pac = Datos.DevDatosPaciente(DniTextBox.Text);
            if (Pac == null)
            {
                DialogResult dialogResult = MessageBox.Show("No existe el paciente especificado, desea crear uno nuevo?", "No existe el paciente", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    NewUser nuevo = new();
                    nuevo.Setapp(this);
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
            Medicacion_TextBox.Text = Pac.DevMed();
        }
        #endregion

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
            if (BirthDateTextBox.Text.Length == 2 || BirthDateTextBox.Text.Length == 5)
            {
                BirthDateTextBox.Text.Append('/');
                BirthDateTextBox.Select(BirthDateTextBox.Text.Length, 0);
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

        private void NewButton_Click(object sender, EventArgs e)
        {


            string fechaG = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            var nuevo = new NewFicha();
            nuevo.Setapp(this);
            nuevo.Show();
            FichaDiaria fichaDiaria = new FichaDiaria();
            fichaDiaria.CreaFichadiaria(DniTextBox.Text, "", "", fechaG, "");
            nuevo.ShowData(fichaDiaria);
            nuevo.Datos = Datos;
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            string fecha = FichasDiariasListBox.Items[FichasDiariasListBox.SelectedIndex].ToString();
            DialogResult resp = MessageBox.Show($"Esta seguro de que desea Borrar permanentemente la ficha: {fecha}?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (resp == DialogResult.Yes)
            {
                FichaDiaria fd = new();

                fd.CreaFichadiaria(DniTextBox.Text, EnfermedadTextBox.Text, MotivoTextBox.Text, fecha, IndicacionesTextBox.Text);
                Datos.DelFicha(fd);
                ListadoFichas();
            }

        }

        private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            EditEnabled();
        }
    }
}
