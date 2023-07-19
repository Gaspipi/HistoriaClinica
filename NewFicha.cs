using HistoriaClinica.Models;

namespace HistoriaClinica
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
        public NewFicha(AppClinica AppCli, string Date)
        {
            InitializeComponent();
            _appCli = AppCli;
            _date = Date;
        }

        public void ShowData(FichaDiaria Data)
        {
            DniTextBox.Text = Data.Dni;
            MotivoTextBox.Text = Data.Motivo;
            IndicacionesTextBox.Text = Data.Indicaciones;
            EnfermedadTextBox.Text = Data.Enfermedad;
            Date = Data.GetFecha();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {

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
                FichaDiaria fd = new(DniTextBox.Text, EnfermedadTextBox.Text, MotivoTextBox.Text, Date, IndicacionesTextBox.Text);
                CRUD_Historias.InsertFichaDiaria(fd);
                Close();
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

        private void NewFicha_FormClosed(object sender, FormClosedEventArgs e)
        {
            AppCli.ListadoFichas();
        }
    }
}
