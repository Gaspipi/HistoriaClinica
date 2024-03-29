﻿namespace HistoriaClinica
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AppClinica));
            TabControl = new TabControl();
            Paciente = new TabPage();
            MedicacionLabel = new Label();
            MedicacionTextBox = new TextBox();
            FirstNameTextBox = new TextBox();
            LastNameTextBox = new TextBox();
            PhoneTextBox = new TextBox();
            BirthDateTextBox = new TextBox();
            ObraSocialTextBox = new TextBox();
            NroAsociadoTextBox = new TextBox();
            NewPacienteButton = new Button();
            PhoneLabel = new Label();
            AntecFamiLabel = new Label();
            AntecPersLabel = new Label();
            AntecFamiTextBox = new TextBox();
            AntecPersTextBox = new TextBox();
            NroAsociadoLabel = new Label();
            ObraSocialLabel = new Label();
            LastNameLabel = new Label();
            FirstNameLabel = new Label();
            BirthLabel = new Label();
            HistoriaClinica = new TabPage();
            DeleteButton = new Button();
            IndicacionesLabel = new Label();
            IndicacionesTextBox = new TextBox();
            NewButton = new Button();
            FichasDiariasLabel = new Label();
            MotivoLabel = new Label();
            EnfermedadLabel = new Label();
            EnfermedadTextBox = new TextBox();
            MotivoTextBox = new TextBox();
            FichasDiariasListBox = new ListBox();
            SearchButton = new Button();
            DniLabel = new Label();
            DniTextBox = new TextBox();
            EditButton = new Button();
            TabControl.SuspendLayout();
            Paciente.SuspendLayout();
            HistoriaClinica.SuspendLayout();
            SuspendLayout();
            // 
            // TabControl
            // 
            TabControl.Controls.Add(Paciente);
            TabControl.Controls.Add(HistoriaClinica);
            TabControl.Location = new Point(14, 55);
            TabControl.Margin = new Padding(4, 3, 4, 3);
            TabControl.Name = "TabControl";
            TabControl.SelectedIndex = 0;
            TabControl.Size = new Size(887, 562);
            TabControl.TabIndex = 4;
            TabControl.SelectedIndexChanged += TabControl_SelectedIndexChanged;
            // 
            // Paciente
            // 
            Paciente.BackColor = Color.White;
            Paciente.Controls.Add(MedicacionLabel);
            Paciente.Controls.Add(MedicacionTextBox);
            Paciente.Controls.Add(FirstNameTextBox);
            Paciente.Controls.Add(LastNameTextBox);
            Paciente.Controls.Add(PhoneTextBox);
            Paciente.Controls.Add(BirthDateTextBox);
            Paciente.Controls.Add(ObraSocialTextBox);
            Paciente.Controls.Add(NroAsociadoTextBox);
            Paciente.Controls.Add(NewPacienteButton);
            Paciente.Controls.Add(PhoneLabel);
            Paciente.Controls.Add(AntecFamiLabel);
            Paciente.Controls.Add(AntecPersLabel);
            Paciente.Controls.Add(AntecFamiTextBox);
            Paciente.Controls.Add(AntecPersTextBox);
            Paciente.Controls.Add(NroAsociadoLabel);
            Paciente.Controls.Add(ObraSocialLabel);
            Paciente.Controls.Add(LastNameLabel);
            Paciente.Controls.Add(FirstNameLabel);
            Paciente.Controls.Add(BirthLabel);
            Paciente.Location = new Point(4, 24);
            Paciente.Margin = new Padding(4, 3, 4, 3);
            Paciente.Name = "Paciente";
            Paciente.Padding = new Padding(4, 3, 4, 3);
            Paciente.Size = new Size(879, 534);
            Paciente.TabIndex = 0;
            Paciente.Text = "Paciente";
            // 
            // MedicacionLabel
            // 
            MedicacionLabel.AutoSize = true;
            MedicacionLabel.Location = new Point(40, 253);
            MedicacionLabel.Margin = new Padding(4, 0, 4, 0);
            MedicacionLabel.Name = "MedicacionLabel";
            MedicacionLabel.Size = new Size(69, 15);
            MedicacionLabel.TabIndex = 40;
            MedicacionLabel.Text = "Medicacion";
            // 
            // MedicacionTextBox
            // 
            MedicacionTextBox.Location = new Point(44, 271);
            MedicacionTextBox.Margin = new Padding(4, 3, 4, 3);
            MedicacionTextBox.Multiline = true;
            MedicacionTextBox.Name = "MedicacionTextBox";
            MedicacionTextBox.ReadOnly = true;
            MedicacionTextBox.ScrollBars = ScrollBars.Vertical;
            MedicacionTextBox.Size = new Size(382, 184);
            MedicacionTextBox.TabIndex = 14;
            // 
            // FirstNameTextBox
            // 
            FirstNameTextBox.Location = new Point(16, 26);
            FirstNameTextBox.Margin = new Padding(4, 3, 4, 3);
            FirstNameTextBox.MaxLength = 50;
            FirstNameTextBox.Name = "FirstNameTextBox";
            FirstNameTextBox.ReadOnly = true;
            FirstNameTextBox.ScrollBars = ScrollBars.Vertical;
            FirstNameTextBox.Size = new Size(187, 23);
            FirstNameTextBox.TabIndex = 7;
            // 
            // LastNameTextBox
            // 
            LastNameTextBox.Location = new Point(239, 27);
            LastNameTextBox.Margin = new Padding(4, 3, 4, 3);
            LastNameTextBox.MaxLength = 50;
            LastNameTextBox.Name = "LastNameTextBox";
            LastNameTextBox.ReadOnly = true;
            LastNameTextBox.ScrollBars = ScrollBars.Vertical;
            LastNameTextBox.Size = new Size(187, 23);
            LastNameTextBox.TabIndex = 8;
            // 
            // PhoneTextBox
            // 
            PhoneTextBox.Location = new Point(239, 78);
            PhoneTextBox.Margin = new Padding(4, 3, 4, 3);
            PhoneTextBox.MaxLength = 50;
            PhoneTextBox.Name = "PhoneTextBox";
            PhoneTextBox.ReadOnly = true;
            PhoneTextBox.ScrollBars = ScrollBars.Vertical;
            PhoneTextBox.Size = new Size(187, 23);
            PhoneTextBox.TabIndex = 10;
            // 
            // BirthDateTextBox
            // 
            BirthDateTextBox.Location = new Point(16, 78);
            BirthDateTextBox.Margin = new Padding(4, 3, 4, 3);
            BirthDateTextBox.MaxLength = 10;
            BirthDateTextBox.Name = "BirthDateTextBox";
            BirthDateTextBox.ReadOnly = true;
            BirthDateTextBox.ScrollBars = ScrollBars.Vertical;
            BirthDateTextBox.Size = new Size(119, 23);
            BirthDateTextBox.TabIndex = 9;
            BirthDateTextBox.TextChanged += BirthDateTextBox_TextChanged_1;
            // 
            // ObraSocialTextBox
            // 
            ObraSocialTextBox.Location = new Point(16, 137);
            ObraSocialTextBox.Margin = new Padding(4, 3, 4, 3);
            ObraSocialTextBox.MaxLength = 50;
            ObraSocialTextBox.Name = "ObraSocialTextBox";
            ObraSocialTextBox.ReadOnly = true;
            ObraSocialTextBox.ScrollBars = ScrollBars.Vertical;
            ObraSocialTextBox.Size = new Size(187, 23);
            ObraSocialTextBox.TabIndex = 11;
            // 
            // NroAsociadoTextBox
            // 
            NroAsociadoTextBox.Location = new Point(239, 137);
            NroAsociadoTextBox.Margin = new Padding(4, 3, 4, 3);
            NroAsociadoTextBox.MaxLength = 50;
            NroAsociadoTextBox.Name = "NroAsociadoTextBox";
            NroAsociadoTextBox.ReadOnly = true;
            NroAsociadoTextBox.ScrollBars = ScrollBars.Vertical;
            NroAsociadoTextBox.Size = new Size(187, 23);
            NroAsociadoTextBox.TabIndex = 12;
            // 
            // NewPacienteButton
            // 
            NewPacienteButton.Location = new Point(16, 498);
            NewPacienteButton.Margin = new Padding(4, 3, 4, 3);
            NewPacienteButton.Name = "NewPacienteButton";
            NewPacienteButton.Size = new Size(89, 27);
            NewPacienteButton.TabIndex = 5;
            NewPacienteButton.Text = "Nuevo";
            NewPacienteButton.TextImageRelation = TextImageRelation.ImageAboveText;
            NewPacienteButton.UseVisualStyleBackColor = true;
            NewPacienteButton.Click += NewPacienteButton_Click;
            // 
            // PhoneLabel
            // 
            PhoneLabel.AutoSize = true;
            PhoneLabel.Location = new Point(236, 60);
            PhoneLabel.Margin = new Padding(4, 0, 4, 0);
            PhoneLabel.Name = "PhoneLabel";
            PhoneLabel.Size = new Size(21, 15);
            PhoneLabel.TabIndex = 38;
            PhoneLabel.Text = "Tel";
            // 
            // AntecFamiLabel
            // 
            AntecFamiLabel.AutoSize = true;
            AntecFamiLabel.Location = new Point(484, 253);
            AntecFamiLabel.Margin = new Padding(4, 0, 4, 0);
            AntecFamiLabel.Name = "AntecFamiLabel";
            AntecFamiLabel.Size = new Size(135, 15);
            AntecFamiLabel.TabIndex = 36;
            AntecFamiLabel.Text = "Antecedentes Familiares";
            // 
            // AntecPersLabel
            // 
            AntecPersLabel.AutoSize = true;
            AntecPersLabel.Location = new Point(484, 8);
            AntecPersLabel.Margin = new Padding(4, 0, 4, 0);
            AntecPersLabel.Name = "AntecPersLabel";
            AntecPersLabel.Size = new Size(138, 15);
            AntecPersLabel.TabIndex = 35;
            AntecPersLabel.Text = "Antecedentes Personales";
            // 
            // AntecFamiTextBox
            // 
            AntecFamiTextBox.Location = new Point(488, 271);
            AntecFamiTextBox.Margin = new Padding(4, 3, 4, 3);
            AntecFamiTextBox.Multiline = true;
            AntecFamiTextBox.Name = "AntecFamiTextBox";
            AntecFamiTextBox.ReadOnly = true;
            AntecFamiTextBox.ScrollBars = ScrollBars.Vertical;
            AntecFamiTextBox.Size = new Size(382, 184);
            AntecFamiTextBox.TabIndex = 15;
            // 
            // AntecPersTextBox
            // 
            AntecPersTextBox.Location = new Point(488, 28);
            AntecPersTextBox.Margin = new Padding(4, 3, 4, 3);
            AntecPersTextBox.Multiline = true;
            AntecPersTextBox.Name = "AntecPersTextBox";
            AntecPersTextBox.ReadOnly = true;
            AntecPersTextBox.ScrollBars = ScrollBars.Vertical;
            AntecPersTextBox.Size = new Size(382, 184);
            AntecPersTextBox.TabIndex = 13;
            // 
            // NroAsociadoLabel
            // 
            NroAsociadoLabel.AutoSize = true;
            NroAsociadoLabel.Location = new Point(236, 119);
            NroAsociadoLabel.Margin = new Padding(4, 0, 4, 0);
            NroAsociadoLabel.Name = "NroAsociadoLabel";
            NroAsociadoLabel.Size = new Size(79, 15);
            NroAsociadoLabel.TabIndex = 30;
            NroAsociadoLabel.Text = "Nro Asociado";
            // 
            // ObraSocialLabel
            // 
            ObraSocialLabel.AutoSize = true;
            ObraSocialLabel.Location = new Point(13, 119);
            ObraSocialLabel.Margin = new Padding(4, 0, 4, 0);
            ObraSocialLabel.Name = "ObraSocialLabel";
            ObraSocialLabel.Size = new Size(67, 15);
            ObraSocialLabel.TabIndex = 28;
            ObraSocialLabel.Text = "Obra Social";
            // 
            // LastNameLabel
            // 
            LastNameLabel.AutoSize = true;
            LastNameLabel.Location = new Point(236, 8);
            LastNameLabel.Margin = new Padding(4, 0, 4, 0);
            LastNameLabel.Name = "LastNameLabel";
            LastNameLabel.Size = new Size(51, 15);
            LastNameLabel.TabIndex = 26;
            LastNameLabel.Text = "Apellido";
            // 
            // FirstNameLabel
            // 
            FirstNameLabel.AutoSize = true;
            FirstNameLabel.Location = new Point(13, 8);
            FirstNameLabel.Margin = new Padding(4, 0, 4, 0);
            FirstNameLabel.Name = "FirstNameLabel";
            FirstNameLabel.Size = new Size(51, 15);
            FirstNameLabel.TabIndex = 25;
            FirstNameLabel.Text = "Nombre";
            // 
            // BirthLabel
            // 
            BirthLabel.AutoSize = true;
            BirthLabel.Location = new Point(13, 60);
            BirthLabel.Margin = new Padding(4, 0, 4, 0);
            BirthLabel.Name = "BirthLabel";
            BirthLabel.Size = new Size(69, 15);
            BirthLabel.TabIndex = 24;
            BirthLabel.Text = "Nacimiento";
            // 
            // HistoriaClinica
            // 
            HistoriaClinica.BackColor = Color.White;
            HistoriaClinica.Controls.Add(DeleteButton);
            HistoriaClinica.Controls.Add(IndicacionesLabel);
            HistoriaClinica.Controls.Add(IndicacionesTextBox);
            HistoriaClinica.Controls.Add(NewButton);
            HistoriaClinica.Controls.Add(FichasDiariasLabel);
            HistoriaClinica.Controls.Add(MotivoLabel);
            HistoriaClinica.Controls.Add(EnfermedadLabel);
            HistoriaClinica.Controls.Add(EnfermedadTextBox);
            HistoriaClinica.Controls.Add(MotivoTextBox);
            HistoriaClinica.Controls.Add(FichasDiariasListBox);
            HistoriaClinica.Location = new Point(4, 24);
            HistoriaClinica.Margin = new Padding(4, 3, 4, 3);
            HistoriaClinica.Name = "HistoriaClinica";
            HistoriaClinica.Padding = new Padding(4, 3, 4, 3);
            HistoriaClinica.Size = new Size(879, 534);
            HistoriaClinica.TabIndex = 1;
            HistoriaClinica.Text = "Historia Clinica";
            // 
            // DeleteButton
            // 
            DeleteButton.Location = new Point(28, 467);
            DeleteButton.Margin = new Padding(4, 3, 4, 3);
            DeleteButton.Name = "DeleteButton";
            DeleteButton.Size = new Size(89, 27);
            DeleteButton.TabIndex = 42;
            DeleteButton.Text = "Borrar";
            DeleteButton.UseVisualStyleBackColor = true;
            DeleteButton.Click += DeleteButton_Click;
            // 
            // IndicacionesLabel
            // 
            IndicacionesLabel.AutoSize = true;
            IndicacionesLabel.Location = new Point(524, 3);
            IndicacionesLabel.Margin = new Padding(4, 0, 4, 0);
            IndicacionesLabel.Name = "IndicacionesLabel";
            IndicacionesLabel.Size = new Size(73, 15);
            IndicacionesLabel.TabIndex = 40;
            IndicacionesLabel.Text = "Indicaciones";
            // 
            // IndicacionesTextBox
            // 
            IndicacionesTextBox.Location = new Point(524, 22);
            IndicacionesTextBox.Margin = new Padding(4, 3, 4, 3);
            IndicacionesTextBox.Multiline = true;
            IndicacionesTextBox.Name = "IndicacionesTextBox";
            IndicacionesTextBox.ReadOnly = true;
            IndicacionesTextBox.Size = new Size(320, 304);
            IndicacionesTextBox.TabIndex = 41;
            // 
            // NewButton
            // 
            NewButton.Location = new Point(28, 498);
            NewButton.Margin = new Padding(4, 3, 4, 3);
            NewButton.Name = "NewButton";
            NewButton.Size = new Size(89, 27);
            NewButton.TabIndex = 39;
            NewButton.Text = "Nuevo";
            NewButton.UseVisualStyleBackColor = true;
            NewButton.Click += NewButton_Click;
            // 
            // FichasDiariasLabel
            // 
            FichasDiariasLabel.AutoSize = true;
            FichasDiariasLabel.Location = new Point(10, 3);
            FichasDiariasLabel.Margin = new Padding(4, 0, 4, 0);
            FichasDiariasLabel.Name = "FichasDiariasLabel";
            FichasDiariasLabel.Size = new Size(120, 15);
            FichasDiariasLabel.TabIndex = 5;
            FichasDiariasLabel.Text = "Fichas Diarias (Fecha)";
            // 
            // MotivoLabel
            // 
            MotivoLabel.AutoSize = true;
            MotivoLabel.Location = new Point(172, 3);
            MotivoLabel.Margin = new Padding(4, 0, 4, 0);
            MotivoLabel.Name = "MotivoLabel";
            MotivoLabel.Size = new Size(45, 15);
            MotivoLabel.TabIndex = 4;
            MotivoLabel.Text = "Motivo";
            // 
            // EnfermedadLabel
            // 
            EnfermedadLabel.AutoSize = true;
            EnfermedadLabel.Location = new Point(172, 171);
            EnfermedadLabel.Margin = new Padding(4, 0, 4, 0);
            EnfermedadLabel.Name = "EnfermedadLabel";
            EnfermedadLabel.Size = new Size(106, 15);
            EnfermedadLabel.TabIndex = 3;
            EnfermedadLabel.Text = "Enfermedad actual";
            // 
            // EnfermedadTextBox
            // 
            EnfermedadTextBox.Location = new Point(172, 189);
            EnfermedadTextBox.Margin = new Padding(4, 3, 4, 3);
            EnfermedadTextBox.Multiline = true;
            EnfermedadTextBox.Name = "EnfermedadTextBox";
            EnfermedadTextBox.ReadOnly = true;
            EnfermedadTextBox.Size = new Size(320, 137);
            EnfermedadTextBox.TabIndex = 7;
            // 
            // MotivoTextBox
            // 
            MotivoTextBox.Location = new Point(172, 22);
            MotivoTextBox.Margin = new Padding(4, 3, 4, 3);
            MotivoTextBox.Multiline = true;
            MotivoTextBox.Name = "MotivoTextBox";
            MotivoTextBox.ReadOnly = true;
            MotivoTextBox.Size = new Size(320, 134);
            MotivoTextBox.TabIndex = 6;
            // 
            // FichasDiariasListBox
            // 
            FichasDiariasListBox.FormattingEnabled = true;
            FichasDiariasListBox.ItemHeight = 15;
            FichasDiariasListBox.Location = new Point(14, 22);
            FichasDiariasListBox.Margin = new Padding(4, 3, 4, 3);
            FichasDiariasListBox.Name = "FichasDiariasListBox";
            FichasDiariasListBox.Size = new Size(116, 439);
            FichasDiariasListBox.TabIndex = 5;
            FichasDiariasListBox.SelectedIndexChanged += listBox1_SelectedIndexChanged;
            // 
            // SearchButton
            // 
            SearchButton.Location = new Point(422, 23);
            SearchButton.Margin = new Padding(4, 3, 4, 3);
            SearchButton.Name = "SearchButton";
            SearchButton.Size = new Size(89, 27);
            SearchButton.TabIndex = 2;
            SearchButton.Text = "Buscar";
            SearchButton.TextImageRelation = TextImageRelation.ImageAboveText;
            SearchButton.UseVisualStyleBackColor = true;
            SearchButton.Click += Search_Click;
            // 
            // DniLabel
            // 
            DniLabel.AutoSize = true;
            DniLabel.Location = new Point(29, 6);
            DniLabel.Margin = new Padding(4, 0, 4, 0);
            DniLabel.Name = "DniLabel";
            DniLabel.Size = new Size(27, 15);
            DniLabel.TabIndex = 34;
            DniLabel.Text = "DNI";
            // 
            // DniTextBox
            // 
            DniTextBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            DniTextBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
            DniTextBox.Location = new Point(33, 25);
            DniTextBox.Margin = new Padding(4, 3, 4, 3);
            DniTextBox.MaxLength = 8;
            DniTextBox.Name = "DniTextBox";
            DniTextBox.PlaceholderText = "12345678";
            DniTextBox.Size = new Size(116, 23);
            DniTextBox.TabIndex = 1;
            DniTextBox.TextChanged += DniTextBox_TextChanged;
            // 
            // EditButton
            // 
            EditButton.Enabled = false;
            EditButton.Location = new Point(802, 23);
            EditButton.Margin = new Padding(4, 3, 4, 3);
            EditButton.Name = "EditButton";
            EditButton.Size = new Size(88, 27);
            EditButton.TabIndex = 3;
            EditButton.Text = "Editar";
            EditButton.TextImageRelation = TextImageRelation.ImageAboveText;
            EditButton.UseVisualStyleBackColor = true;
            EditButton.Click += EditButton_Click;
            // 
            // AppClinica
            // 
            AcceptButton = SearchButton;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            BackColor = SystemColors.Window;
            ClientSize = new Size(915, 621);
            Controls.Add(EditButton);
            Controls.Add(SearchButton);
            Controls.Add(DniLabel);
            Controls.Add(DniTextBox);
            Controls.Add(TabControl);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4, 3, 4, 3);
            MinimumSize = new Size(931, 660);
            Name = "AppClinica";
            RightToLeft = RightToLeft.No;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Historia Clinica";
            TabControl.ResumeLayout(false);
            Paciente.ResumeLayout(false);
            Paciente.PerformLayout();
            HistoriaClinica.ResumeLayout(false);
            HistoriaClinica.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TabControl TabControl;
        private TabPage Paciente;
        private Label AntecFamiLabel;
        private Label AntecPersLabel;
        private TextBox AntecFamiTextBox;
        private TextBox AntecPersTextBox;
        private Label NroAsociadoLabel;
        private Label ObraSocialLabel;
        private Label LastNameLabel;
        private Label FirstNameLabel;
        private Label BirthLabel;
        private TabPage HistoriaClinica;
        private Button SearchButton;
        private Label DniLabel;
        private TextBox DniTextBox;
        private ListBox FichasDiariasListBox;
        private Button EditButton;
        private Label FichasDiariasLabel;
        private Label MotivoLabel;
        private Label EnfermedadLabel;
        private TextBox EnfermedadTextBox;
        private TextBox MotivoTextBox;
        private Button NewButton;
        private Button NewPacienteButton;
        private Label PhoneLabel;
        private TextBox BirthDateTextBox;
        private TextBox ObraSocialTextBox;
        private TextBox NroAsociadoTextBox;
        private TextBox PhoneTextBox;
        private TextBox FirstNameTextBox;
        private TextBox LastNameTextBox;
        private Label IndicacionesLabel;
        private TextBox IndicacionesTextBox;
        private Button DeleteButton;
        private Label MedicacionLabel;
        private TextBox MedicacionTextBox;
    }
}

