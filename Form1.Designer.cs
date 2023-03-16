namespace AppParaMama
{
    partial class AppClinica
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.TabControl = new System.Windows.Forms.TabControl();
            this.Paciente = new System.Windows.Forms.TabPage();
            this.FirstNameTextBox = new System.Windows.Forms.TextBox();
            this.LastNameTextBox = new System.Windows.Forms.TextBox();
            this.PhoneTextBox = new System.Windows.Forms.TextBox();
            this.BirthDateTextBox = new System.Windows.Forms.TextBox();
            this.ObraSocialTextBox = new System.Windows.Forms.TextBox();
            this.NroAsociadoTextBox = new System.Windows.Forms.TextBox();
            this.NewPacienteButton = new System.Windows.Forms.Button();
            this.PhoneLabel = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.AntecFamiTextBox = new System.Windows.Forms.TextBox();
            this.AntecPersTextBox = new System.Windows.Forms.TextBox();
            this.NroAsociadoLabel = new System.Windows.Forms.Label();
            this.ObraSocialLabel = new System.Windows.Forms.Label();
            this.LastNameLabel = new System.Windows.Forms.Label();
            this.FirstNameLabel = new System.Windows.Forms.Label();
            this.BirthLabel = new System.Windows.Forms.Label();
            this.HistoriaClinica = new System.Windows.Forms.TabPage();
            this.DeleteButton = new System.Windows.Forms.Button();
            this.IndicacionesLabel = new System.Windows.Forms.Label();
            this.IndicacionesTextBox = new System.Windows.Forms.TextBox();
            this.NewButton = new System.Windows.Forms.Button();
            this.FichasDiariasLabel = new System.Windows.Forms.Label();
            this.MotivoLabel = new System.Windows.Forms.Label();
            this.EnfermedadLabel = new System.Windows.Forms.Label();
            this.EnfermedadTextBox = new System.Windows.Forms.TextBox();
            this.MotivoTextBox = new System.Windows.Forms.TextBox();
            this.FichasDiariasListBox = new System.Windows.Forms.ListBox();
            this.SQLTab = new System.Windows.Forms.TabPage();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Dni = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SearchButton = new System.Windows.Forms.Button();
            this.DniLabel = new System.Windows.Forms.Label();
            this.DniTextBox = new System.Windows.Forms.TextBox();
            this.EditButton = new System.Windows.Forms.Button();
            this.TabControl.SuspendLayout();
            this.Paciente.SuspendLayout();
            this.HistoriaClinica.SuspendLayout();
            this.SQLTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // TabControl
            // 
            this.TabControl.Controls.Add(this.Paciente);
            this.TabControl.Controls.Add(this.HistoriaClinica);
            this.TabControl.Controls.Add(this.SQLTab);
            this.TabControl.Location = new System.Drawing.Point(14, 55);
            this.TabControl.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.TabControl.Name = "TabControl";
            this.TabControl.SelectedIndex = 0;
            this.TabControl.Size = new System.Drawing.Size(887, 562);
            this.TabControl.TabIndex = 4;
            // 
            // Paciente
            // 
            this.Paciente.BackColor = System.Drawing.SystemColors.Menu;
            this.Paciente.Controls.Add(this.FirstNameTextBox);
            this.Paciente.Controls.Add(this.LastNameTextBox);
            this.Paciente.Controls.Add(this.PhoneTextBox);
            this.Paciente.Controls.Add(this.BirthDateTextBox);
            this.Paciente.Controls.Add(this.ObraSocialTextBox);
            this.Paciente.Controls.Add(this.NroAsociadoTextBox);
            this.Paciente.Controls.Add(this.NewPacienteButton);
            this.Paciente.Controls.Add(this.PhoneLabel);
            this.Paciente.Controls.Add(this.label8);
            this.Paciente.Controls.Add(this.label7);
            this.Paciente.Controls.Add(this.AntecFamiTextBox);
            this.Paciente.Controls.Add(this.AntecPersTextBox);
            this.Paciente.Controls.Add(this.NroAsociadoLabel);
            this.Paciente.Controls.Add(this.ObraSocialLabel);
            this.Paciente.Controls.Add(this.LastNameLabel);
            this.Paciente.Controls.Add(this.FirstNameLabel);
            this.Paciente.Controls.Add(this.BirthLabel);
            this.Paciente.Location = new System.Drawing.Point(4, 24);
            this.Paciente.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Paciente.Name = "Paciente";
            this.Paciente.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Paciente.Size = new System.Drawing.Size(879, 534);
            this.Paciente.TabIndex = 0;
            this.Paciente.Text = "Paciente";
            this.Paciente.Click += new System.EventHandler(this.tabPage1_Click);
            // 
            // FirstNameTextBox
            // 
            this.FirstNameTextBox.Location = new System.Drawing.Point(16, 26);
            this.FirstNameTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.FirstNameTextBox.MaxLength = 50;
            this.FirstNameTextBox.Name = "FirstNameTextBox";
            this.FirstNameTextBox.ReadOnly = true;
            this.FirstNameTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.FirstNameTextBox.Size = new System.Drawing.Size(187, 23);
            this.FirstNameTextBox.TabIndex = 7;
            // 
            // LastNameTextBox
            // 
            this.LastNameTextBox.Location = new System.Drawing.Point(239, 27);
            this.LastNameTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.LastNameTextBox.MaxLength = 50;
            this.LastNameTextBox.Name = "LastNameTextBox";
            this.LastNameTextBox.ReadOnly = true;
            this.LastNameTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.LastNameTextBox.Size = new System.Drawing.Size(187, 23);
            this.LastNameTextBox.TabIndex = 8;
            // 
            // PhoneTextBox
            // 
            this.PhoneTextBox.Location = new System.Drawing.Point(239, 78);
            this.PhoneTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.PhoneTextBox.MaxLength = 50;
            this.PhoneTextBox.Name = "PhoneTextBox";
            this.PhoneTextBox.ReadOnly = true;
            this.PhoneTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.PhoneTextBox.Size = new System.Drawing.Size(187, 23);
            this.PhoneTextBox.TabIndex = 10;
            this.PhoneTextBox.TextChanged += new System.EventHandler(this.PhoneTextBox_TextChanged);
            // 
            // BirthDateTextBox
            // 
            this.BirthDateTextBox.Location = new System.Drawing.Point(16, 78);
            this.BirthDateTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.BirthDateTextBox.MaxLength = 10;
            this.BirthDateTextBox.Name = "BirthDateTextBox";
            this.BirthDateTextBox.ReadOnly = true;
            this.BirthDateTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.BirthDateTextBox.Size = new System.Drawing.Size(119, 23);
            this.BirthDateTextBox.TabIndex = 9;
            this.BirthDateTextBox.TextChanged += new System.EventHandler(this.BirthDateTextBox_TextChanged_1);
            // 
            // ObraSocialTextBox
            // 
            this.ObraSocialTextBox.Location = new System.Drawing.Point(16, 137);
            this.ObraSocialTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ObraSocialTextBox.MaxLength = 50;
            this.ObraSocialTextBox.Name = "ObraSocialTextBox";
            this.ObraSocialTextBox.ReadOnly = true;
            this.ObraSocialTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.ObraSocialTextBox.Size = new System.Drawing.Size(187, 23);
            this.ObraSocialTextBox.TabIndex = 11;
            // 
            // NroAsociadoTextBox
            // 
            this.NroAsociadoTextBox.Location = new System.Drawing.Point(239, 137);
            this.NroAsociadoTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.NroAsociadoTextBox.MaxLength = 50;
            this.NroAsociadoTextBox.Name = "NroAsociadoTextBox";
            this.NroAsociadoTextBox.ReadOnly = true;
            this.NroAsociadoTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.NroAsociadoTextBox.Size = new System.Drawing.Size(187, 23);
            this.NroAsociadoTextBox.TabIndex = 12;
            // 
            // NewPacienteButton
            // 
            this.NewPacienteButton.Location = new System.Drawing.Point(16, 498);
            this.NewPacienteButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.NewPacienteButton.Name = "NewPacienteButton";
            this.NewPacienteButton.Size = new System.Drawing.Size(89, 27);
            this.NewPacienteButton.TabIndex = 37;
            this.NewPacienteButton.Text = "Nuevo";
            this.NewPacienteButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.NewPacienteButton.UseVisualStyleBackColor = true;
            this.NewPacienteButton.Click += new System.EventHandler(this.NewPacienteButton_Click);
            // 
            // PhoneLabel
            // 
            this.PhoneLabel.AutoSize = true;
            this.PhoneLabel.Location = new System.Drawing.Point(236, 60);
            this.PhoneLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.PhoneLabel.Name = "PhoneLabel";
            this.PhoneLabel.Size = new System.Drawing.Size(21, 15);
            this.PhoneLabel.TabIndex = 38;
            this.PhoneLabel.Text = "Tel";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(484, 253);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(135, 15);
            this.label8.TabIndex = 36;
            this.label8.Text = "Antecedentes Familiares";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(484, 8);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(138, 15);
            this.label7.TabIndex = 35;
            this.label7.Text = "Antecedentes Personales";
            this.label7.Click += new System.EventHandler(this.label7_Click_1);
            // 
            // AntecFamiTextBox
            // 
            this.AntecFamiTextBox.Location = new System.Drawing.Point(488, 271);
            this.AntecFamiTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.AntecFamiTextBox.Multiline = true;
            this.AntecFamiTextBox.Name = "AntecFamiTextBox";
            this.AntecFamiTextBox.ReadOnly = true;
            this.AntecFamiTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.AntecFamiTextBox.Size = new System.Drawing.Size(382, 184);
            this.AntecFamiTextBox.TabIndex = 14;
            // 
            // AntecPersTextBox
            // 
            this.AntecPersTextBox.Location = new System.Drawing.Point(488, 28);
            this.AntecPersTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.AntecPersTextBox.Multiline = true;
            this.AntecPersTextBox.Name = "AntecPersTextBox";
            this.AntecPersTextBox.ReadOnly = true;
            this.AntecPersTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.AntecPersTextBox.Size = new System.Drawing.Size(382, 184);
            this.AntecPersTextBox.TabIndex = 13;
            // 
            // NroAsociadoLabel
            // 
            this.NroAsociadoLabel.AutoSize = true;
            this.NroAsociadoLabel.Location = new System.Drawing.Point(236, 119);
            this.NroAsociadoLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.NroAsociadoLabel.Name = "NroAsociadoLabel";
            this.NroAsociadoLabel.Size = new System.Drawing.Size(79, 15);
            this.NroAsociadoLabel.TabIndex = 30;
            this.NroAsociadoLabel.Text = "Nro Asociado";
            // 
            // ObraSocialLabel
            // 
            this.ObraSocialLabel.AutoSize = true;
            this.ObraSocialLabel.Location = new System.Drawing.Point(13, 119);
            this.ObraSocialLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.ObraSocialLabel.Name = "ObraSocialLabel";
            this.ObraSocialLabel.Size = new System.Drawing.Size(67, 15);
            this.ObraSocialLabel.TabIndex = 28;
            this.ObraSocialLabel.Text = "Obra Social";
            // 
            // LastNameLabel
            // 
            this.LastNameLabel.AutoSize = true;
            this.LastNameLabel.Location = new System.Drawing.Point(236, 8);
            this.LastNameLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LastNameLabel.Name = "LastNameLabel";
            this.LastNameLabel.Size = new System.Drawing.Size(51, 15);
            this.LastNameLabel.TabIndex = 26;
            this.LastNameLabel.Text = "Apellido";
            // 
            // FirstNameLabel
            // 
            this.FirstNameLabel.AutoSize = true;
            this.FirstNameLabel.Location = new System.Drawing.Point(13, 8);
            this.FirstNameLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.FirstNameLabel.Name = "FirstNameLabel";
            this.FirstNameLabel.Size = new System.Drawing.Size(51, 15);
            this.FirstNameLabel.TabIndex = 25;
            this.FirstNameLabel.Text = "Nombre";
            // 
            // BirthLabel
            // 
            this.BirthLabel.AutoSize = true;
            this.BirthLabel.Location = new System.Drawing.Point(13, 60);
            this.BirthLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.BirthLabel.Name = "BirthLabel";
            this.BirthLabel.Size = new System.Drawing.Size(69, 15);
            this.BirthLabel.TabIndex = 24;
            this.BirthLabel.Text = "Nacimiento";
            // 
            // HistoriaClinica
            // 
            this.HistoriaClinica.Controls.Add(this.DeleteButton);
            this.HistoriaClinica.Controls.Add(this.IndicacionesLabel);
            this.HistoriaClinica.Controls.Add(this.IndicacionesTextBox);
            this.HistoriaClinica.Controls.Add(this.NewButton);
            this.HistoriaClinica.Controls.Add(this.FichasDiariasLabel);
            this.HistoriaClinica.Controls.Add(this.MotivoLabel);
            this.HistoriaClinica.Controls.Add(this.EnfermedadLabel);
            this.HistoriaClinica.Controls.Add(this.EnfermedadTextBox);
            this.HistoriaClinica.Controls.Add(this.MotivoTextBox);
            this.HistoriaClinica.Controls.Add(this.FichasDiariasListBox);
            this.HistoriaClinica.Location = new System.Drawing.Point(4, 24);
            this.HistoriaClinica.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.HistoriaClinica.Name = "HistoriaClinica";
            this.HistoriaClinica.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.HistoriaClinica.Size = new System.Drawing.Size(879, 534);
            this.HistoriaClinica.TabIndex = 1;
            this.HistoriaClinica.Text = "Historia Clinica";
            this.HistoriaClinica.UseVisualStyleBackColor = true;
            this.HistoriaClinica.Click += new System.EventHandler(this.HistoriaClinica_Click);
            // 
            // DeleteButton
            // 
            this.DeleteButton.Location = new System.Drawing.Point(28, 467);
            this.DeleteButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.DeleteButton.Name = "DeleteButton";
            this.DeleteButton.Size = new System.Drawing.Size(89, 27);
            this.DeleteButton.TabIndex = 42;
            this.DeleteButton.Text = "Borrar";
            this.DeleteButton.UseVisualStyleBackColor = true;
            // 
            // IndicacionesLabel
            // 
            this.IndicacionesLabel.AutoSize = true;
            this.IndicacionesLabel.Location = new System.Drawing.Point(524, 3);
            this.IndicacionesLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.IndicacionesLabel.Name = "IndicacionesLabel";
            this.IndicacionesLabel.Size = new System.Drawing.Size(73, 15);
            this.IndicacionesLabel.TabIndex = 40;
            this.IndicacionesLabel.Text = "Indicaciones";
            // 
            // IndicacionesTextBox
            // 
            this.IndicacionesTextBox.Location = new System.Drawing.Point(524, 22);
            this.IndicacionesTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.IndicacionesTextBox.Multiline = true;
            this.IndicacionesTextBox.Name = "IndicacionesTextBox";
            this.IndicacionesTextBox.ReadOnly = true;
            this.IndicacionesTextBox.Size = new System.Drawing.Size(320, 304);
            this.IndicacionesTextBox.TabIndex = 41;
            // 
            // NewButton
            // 
            this.NewButton.Location = new System.Drawing.Point(28, 498);
            this.NewButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.NewButton.Name = "NewButton";
            this.NewButton.Size = new System.Drawing.Size(89, 27);
            this.NewButton.TabIndex = 39;
            this.NewButton.Text = "Nuevo";
            this.NewButton.UseVisualStyleBackColor = true;
            this.NewButton.Click += new System.EventHandler(this.NewButton_Click);
            // 
            // FichasDiariasLabel
            // 
            this.FichasDiariasLabel.AutoSize = true;
            this.FichasDiariasLabel.Location = new System.Drawing.Point(10, 3);
            this.FichasDiariasLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.FichasDiariasLabel.Name = "FichasDiariasLabel";
            this.FichasDiariasLabel.Size = new System.Drawing.Size(120, 15);
            this.FichasDiariasLabel.TabIndex = 5;
            this.FichasDiariasLabel.Text = "Fichas Diarias (Fecha)";
            // 
            // MotivoLabel
            // 
            this.MotivoLabel.AutoSize = true;
            this.MotivoLabel.Location = new System.Drawing.Point(172, 3);
            this.MotivoLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.MotivoLabel.Name = "MotivoLabel";
            this.MotivoLabel.Size = new System.Drawing.Size(45, 15);
            this.MotivoLabel.TabIndex = 4;
            this.MotivoLabel.Text = "Motivo";
            // 
            // EnfermedadLabel
            // 
            this.EnfermedadLabel.AutoSize = true;
            this.EnfermedadLabel.Location = new System.Drawing.Point(172, 171);
            this.EnfermedadLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.EnfermedadLabel.Name = "EnfermedadLabel";
            this.EnfermedadLabel.Size = new System.Drawing.Size(71, 15);
            this.EnfermedadLabel.TabIndex = 3;
            this.EnfermedadLabel.Text = "Enfermedad";
            // 
            // EnfermedadTextBox
            // 
            this.EnfermedadTextBox.Location = new System.Drawing.Point(172, 189);
            this.EnfermedadTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.EnfermedadTextBox.Multiline = true;
            this.EnfermedadTextBox.Name = "EnfermedadTextBox";
            this.EnfermedadTextBox.ReadOnly = true;
            this.EnfermedadTextBox.Size = new System.Drawing.Size(320, 137);
            this.EnfermedadTextBox.TabIndex = 7;
            // 
            // MotivoTextBox
            // 
            this.MotivoTextBox.Location = new System.Drawing.Point(172, 22);
            this.MotivoTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MotivoTextBox.Multiline = true;
            this.MotivoTextBox.Name = "MotivoTextBox";
            this.MotivoTextBox.ReadOnly = true;
            this.MotivoTextBox.Size = new System.Drawing.Size(320, 134);
            this.MotivoTextBox.TabIndex = 6;
            // 
            // FichasDiariasListBox
            // 
            this.FichasDiariasListBox.FormattingEnabled = true;
            this.FichasDiariasListBox.ItemHeight = 15;
            this.FichasDiariasListBox.Location = new System.Drawing.Point(14, 22);
            this.FichasDiariasListBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.FichasDiariasListBox.Name = "FichasDiariasListBox";
            this.FichasDiariasListBox.Size = new System.Drawing.Size(116, 439);
            this.FichasDiariasListBox.TabIndex = 5;
            this.FichasDiariasListBox.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // SQLTab
            // 
            this.SQLTab.Controls.Add(this.dataGridView1);
            this.SQLTab.Location = new System.Drawing.Point(4, 24);
            this.SQLTab.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.SQLTab.Name = "SQLTab";
            this.SQLTab.Size = new System.Drawing.Size(879, 534);
            this.SQLTab.TabIndex = 2;
            this.SQLTab.Text = "SQL";
            this.SQLTab.UseVisualStyleBackColor = true;
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Dni});
            this.dataGridView1.Location = new System.Drawing.Point(3, 3);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 25;
            this.dataGridView1.Size = new System.Drawing.Size(869, 528);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // Dni
            // 
            this.Dni.HeaderText = "Column1";
            this.Dni.Name = "Dni";
            // 
            // SearchButton
            // 
            this.SearchButton.Location = new System.Drawing.Point(422, 23);
            this.SearchButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.SearchButton.Name = "SearchButton";
            this.SearchButton.Size = new System.Drawing.Size(89, 27);
            this.SearchButton.TabIndex = 2;
            this.SearchButton.Text = "Buscar";
            this.SearchButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.SearchButton.UseVisualStyleBackColor = true;
            this.SearchButton.Click += new System.EventHandler(this.Search_Click);
            // 
            // DniLabel
            // 
            this.DniLabel.AutoSize = true;
            this.DniLabel.Location = new System.Drawing.Point(29, 6);
            this.DniLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.DniLabel.Name = "DniLabel";
            this.DniLabel.Size = new System.Drawing.Size(27, 15);
            this.DniLabel.TabIndex = 34;
            this.DniLabel.Text = "DNI";
            this.DniLabel.Click += new System.EventHandler(this.DniLabel_Click);
            // 
            // DniTextBox
            // 
            this.DniTextBox.Location = new System.Drawing.Point(33, 25);
            this.DniTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.DniTextBox.MaxLength = 8;
            this.DniTextBox.Name = "DniTextBox";
            this.DniTextBox.Size = new System.Drawing.Size(116, 23);
            this.DniTextBox.TabIndex = 1;
            this.DniTextBox.Text = "00000000";
            this.DniTextBox.TextChanged += new System.EventHandler(this.DniTextBox_TextChanged);
            // 
            // EditButton
            // 
            this.EditButton.Location = new System.Drawing.Point(802, 23);
            this.EditButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.EditButton.Name = "EditButton";
            this.EditButton.Size = new System.Drawing.Size(88, 27);
            this.EditButton.TabIndex = 3;
            this.EditButton.Text = "Editar";
            this.EditButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.EditButton.UseVisualStyleBackColor = true;
            this.EditButton.Click += new System.EventHandler(this.EditButton_Click);
            // 
            // AppClinica
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(915, 647);
            this.Controls.Add(this.EditButton);
            this.Controls.Add(this.SearchButton);
            this.Controls.Add(this.DniLabel);
            this.Controls.Add(this.DniTextBox);
            this.Controls.Add(this.TabControl);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MinimumSize = new System.Drawing.Size(931, 686);
            this.Name = "AppClinica";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Historia Clinica";
            this.Load += new System.EventHandler(this.AppClinica_Load);
            this.TabControl.ResumeLayout(false);
            this.Paciente.ResumeLayout(false);
            this.Paciente.PerformLayout();
            this.HistoriaClinica.ResumeLayout(false);
            this.HistoriaClinica.PerformLayout();
            this.SQLTab.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl TabControl;
        private System.Windows.Forms.TabPage Paciente;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox AntecFamiTextBox;
        private System.Windows.Forms.TextBox AntecPersTextBox;
        private System.Windows.Forms.Label NroAsociadoLabel;
        private System.Windows.Forms.Label ObraSocialLabel;
        private System.Windows.Forms.Label LastNameLabel;
        private System.Windows.Forms.Label FirstNameLabel;
        private System.Windows.Forms.Label BirthLabel;
        private System.Windows.Forms.TabPage HistoriaClinica;
        private System.Windows.Forms.Button SearchButton;
        private System.Windows.Forms.Label DniLabel;
        private System.Windows.Forms.TextBox DniTextBox;
        private System.Windows.Forms.ListBox FichasDiariasListBox;
        private System.Windows.Forms.Button EditButton;
        private System.Windows.Forms.Label FichasDiariasLabel;
        private System.Windows.Forms.Label MotivoLabel;
        private System.Windows.Forms.Label EnfermedadLabel;
        private System.Windows.Forms.TextBox EnfermedadTextBox;
        private System.Windows.Forms.TextBox MotivoTextBox;
        private System.Windows.Forms.Button NewButton;
        private System.Windows.Forms.TabPage SQLTab;
        private System.Windows.Forms.Button NewPacienteButton;
        private System.Windows.Forms.Label PhoneLabel;
        private DataGridView dataGridView1;
        private DataGridViewTextBoxColumn Dni;
        private TextBox BirthDateTextBox;
        private TextBox ObraSocialTextBox;
        private TextBox NroAsociadoTextBox;
        private TextBox PhoneTextBox;
        private TextBox FirstNameTextBox;
        private TextBox LastNameTextBox;
        private Label IndicacionesLabel;
        private TextBox IndicacionesTextBox;
        private Button DeleteButton;
    }
}

