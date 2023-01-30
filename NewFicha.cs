namespace WinFormsApp1
{
    public partial class NewFicha : Form
    {
        DateTime date;

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
            date = Data.DevFecha();
        }

        private void DniTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void EnfermedadTextBox_TextChanged(object sender, EventArgs e)
        {

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
                fd.CreaFichadiaria(DniTextBox.Text, EnfermedadTextBox.Text, MotivoTextBox.Text, date, IndicacionesTextBox.Text);
                Datos.SetFichaDiaria(fd);
                this.Close();
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
