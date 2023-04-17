using AppParaMama;

namespace WinFormsApp1
{
    public partial class NewFicha : Form
    {
        private string _date;
        private AppClinica _appCli;

        public string Date
        {
            get { return _date; }
            set { _date = value; }
        }

        public AppClinica AppCli
        {
            get { return _appCli; }
            set { _appCli = value; }
        }
        public NewFicha()
        {
            InitializeComponent();
        }
        #region

        public L_Historias Datos = null;



        #endregion

        public void ShowData(FichaDiaria Data)
        {
            DniTextBox.Text = Data.DevDni();
            MotivoTextBox.Text = Data.DevMotivo();
            IndicacionesTextBox.Text = Data.DevIndicaciones();
            EnfermedadTextBox.Text = Data.DevEnfermedad();
            Date = Data.DevFecha();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            FichaDiaria fd = new();
            if (MotivoTextBox.Text.Length < 1)
            {
                MessageBox.Show("El campo Motivo debe ser llenado", "Error - Motivo invalido", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (EnfermedadTextBox.Text.Length < 1)
            {
                MessageBox.Show("El campo Enfermedad debe ser llenado", "Error - Enfermedad invalida", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (IndicacionesTextBox.Text.Length < 1)
            {
                MessageBox.Show("El campo Indicaciones debe ser llenado", "Error - Indicaciones invalidas", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (IndicacionesTextBox.Text.Length > 0 && MotivoTextBox.Text.Length > 0 && EnfermedadTextBox.Text.Length > 0)
            {
                fd.CreaFichadiaria(DniTextBox.Text, EnfermedadTextBox.Text, MotivoTextBox.Text, Date, IndicacionesTextBox.Text);
                Datos.SetFichaDiaria(fd);
                Close();
            }
        }
        public void Setapp(AppClinica app)
        {
            AppCli = app;
        }
        public AppClinica Devapp()
        {
            return AppCli;
        }
        private void CancelarButton_Click(object sender, EventArgs e)
        {
            DialogResult resp = MessageBox.Show($"Esta seguro de que desea Cancelar?", "Cancelar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (resp == DialogResult.Yes)
            {
                Close();
            }
        }

        private void NewFicha_FormClosed(object sender, FormClosedEventArgs e)
        {
            AppClinica app = Devapp();
            app.ListadoFichas();
        }
    }
}
