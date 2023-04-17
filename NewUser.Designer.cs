namespace WinFormsApp1
{
    partial class NewUser
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewUser));
            PhoneTextBox = new TextBox();
            PhoneLabel = new Label();
            label8 = new Label();
            label7 = new Label();
            AntecFamiTextBox = new TextBox();
            AntecPersTextBox = new TextBox();
            FirstNameTextBox = new TextBox();
            NroAsociadoLabel = new Label();
            BirthDateTextBox = new TextBox();
            ObraSocialLabel = new Label();
            LastNameLabel = new Label();
            FirstNameLabel = new Label();
            BirthLabel = new Label();
            NroAsociadoTextBox = new TextBox();
            ObraSocialTextBox = new TextBox();
            LastNameTextBox = new TextBox();
            SaveButton = new Button();
            CancelarButton = new Button();
            DniLabel = new Label();
            DniTextBox = new TextBox();
            Medicacion_Label = new Label();
            Medicacion_TextBox = new TextBox();
            SuspendLayout();
            // 
            // PhoneTextBox
            // 
            PhoneTextBox.BackColor = SystemColors.Window;
            PhoneTextBox.Location = new Point(244, 125);
            PhoneTextBox.Margin = new Padding(4, 3, 4, 3);
            PhoneTextBox.MaxLength = 50;
            PhoneTextBox.Name = "PhoneTextBox";
            PhoneTextBox.Size = new Size(187, 23);
            PhoneTextBox.TabIndex = 43;
            // 
            // PhoneLabel
            // 
            PhoneLabel.AutoSize = true;
            PhoneLabel.Location = new Point(241, 107);
            PhoneLabel.Margin = new Padding(4, 0, 4, 0);
            PhoneLabel.Name = "PhoneLabel";
            PhoneLabel.Size = new Size(21, 15);
            PhoneLabel.TabIndex = 55;
            PhoneLabel.Text = "Tel";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(491, 220);
            label8.Margin = new Padding(4, 0, 4, 0);
            label8.Name = "label8";
            label8.Size = new Size(135, 15);
            label8.TabIndex = 54;
            label8.Text = "Antecedentes Familiares";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(491, 10);
            label7.Margin = new Padding(4, 0, 4, 0);
            label7.Name = "label7";
            label7.Size = new Size(138, 15);
            label7.TabIndex = 53;
            label7.Text = "Antecedentes Personales";
            // 
            // AntecFamiTextBox
            // 
            AntecFamiTextBox.Location = new Point(495, 238);
            AntecFamiTextBox.Margin = new Padding(4, 3, 4, 3);
            AntecFamiTextBox.Multiline = true;
            AntecFamiTextBox.Name = "AntecFamiTextBox";
            AntecFamiTextBox.ScrollBars = ScrollBars.Vertical;
            AntecFamiTextBox.Size = new Size(292, 143);
            AntecFamiTextBox.TabIndex = 47;
            // 
            // AntecPersTextBox
            // 
            AntecPersTextBox.Location = new Point(495, 30);
            AntecPersTextBox.Margin = new Padding(4, 3, 4, 3);
            AntecPersTextBox.Multiline = true;
            AntecPersTextBox.Name = "AntecPersTextBox";
            AntecPersTextBox.ScrollBars = ScrollBars.Vertical;
            AntecPersTextBox.Size = new Size(292, 143);
            AntecPersTextBox.TabIndex = 46;
            // 
            // FirstNameTextBox
            // 
            FirstNameTextBox.BackColor = SystemColors.Window;
            FirstNameTextBox.Location = new Point(21, 74);
            FirstNameTextBox.Margin = new Padding(4, 3, 4, 3);
            FirstNameTextBox.MaxLength = 50;
            FirstNameTextBox.Name = "FirstNameTextBox";
            FirstNameTextBox.Size = new Size(187, 23);
            FirstNameTextBox.TabIndex = 40;
            // 
            // NroAsociadoLabel
            // 
            NroAsociadoLabel.AutoSize = true;
            NroAsociadoLabel.Location = new Point(241, 166);
            NroAsociadoLabel.Margin = new Padding(4, 0, 4, 0);
            NroAsociadoLabel.Name = "NroAsociadoLabel";
            NroAsociadoLabel.Size = new Size(79, 15);
            NroAsociadoLabel.TabIndex = 52;
            NroAsociadoLabel.Text = "Nro Asociado";
            // 
            // BirthDateTextBox
            // 
            BirthDateTextBox.BackColor = SystemColors.Window;
            BirthDateTextBox.Location = new Point(21, 125);
            BirthDateTextBox.Margin = new Padding(4, 3, 4, 3);
            BirthDateTextBox.MaxLength = 10;
            BirthDateTextBox.Name = "BirthDateTextBox";
            BirthDateTextBox.PlaceholderText = "dd/mm/aaaa";
            BirthDateTextBox.Size = new Size(116, 23);
            BirthDateTextBox.TabIndex = 42;
            BirthDateTextBox.TextChanged += BirthDateTextBox_TextChanged;
            // 
            // ObraSocialLabel
            // 
            ObraSocialLabel.AutoSize = true;
            ObraSocialLabel.Location = new Point(18, 166);
            ObraSocialLabel.Margin = new Padding(4, 0, 4, 0);
            ObraSocialLabel.Name = "ObraSocialLabel";
            ObraSocialLabel.Size = new Size(67, 15);
            ObraSocialLabel.TabIndex = 51;
            ObraSocialLabel.Text = "Obra Social";
            // 
            // LastNameLabel
            // 
            LastNameLabel.AutoSize = true;
            LastNameLabel.Location = new Point(241, 55);
            LastNameLabel.Margin = new Padding(4, 0, 4, 0);
            LastNameLabel.Name = "LastNameLabel";
            LastNameLabel.Size = new Size(51, 15);
            LastNameLabel.TabIndex = 50;
            LastNameLabel.Text = "Apellido";
            // 
            // FirstNameLabel
            // 
            FirstNameLabel.AutoSize = true;
            FirstNameLabel.Location = new Point(18, 55);
            FirstNameLabel.Margin = new Padding(4, 0, 4, 0);
            FirstNameLabel.Name = "FirstNameLabel";
            FirstNameLabel.Size = new Size(51, 15);
            FirstNameLabel.TabIndex = 49;
            FirstNameLabel.Text = "Nombre";
            // 
            // BirthLabel
            // 
            BirthLabel.AutoSize = true;
            BirthLabel.Location = new Point(18, 107);
            BirthLabel.Margin = new Padding(4, 0, 4, 0);
            BirthLabel.Name = "BirthLabel";
            BirthLabel.Size = new Size(69, 15);
            BirthLabel.TabIndex = 48;
            BirthLabel.Text = "Nacimiento";
            // 
            // NroAsociadoTextBox
            // 
            NroAsociadoTextBox.BackColor = SystemColors.Window;
            NroAsociadoTextBox.Location = new Point(244, 184);
            NroAsociadoTextBox.Margin = new Padding(4, 3, 4, 3);
            NroAsociadoTextBox.MaxLength = 50;
            NroAsociadoTextBox.Name = "NroAsociadoTextBox";
            NroAsociadoTextBox.Size = new Size(187, 23);
            NroAsociadoTextBox.TabIndex = 45;
            // 
            // ObraSocialTextBox
            // 
            ObraSocialTextBox.BackColor = SystemColors.Window;
            ObraSocialTextBox.Location = new Point(21, 184);
            ObraSocialTextBox.Margin = new Padding(4, 3, 4, 3);
            ObraSocialTextBox.MaxLength = 50;
            ObraSocialTextBox.Name = "ObraSocialTextBox";
            ObraSocialTextBox.Size = new Size(187, 23);
            ObraSocialTextBox.TabIndex = 44;
            // 
            // LastNameTextBox
            // 
            LastNameTextBox.BackColor = SystemColors.Window;
            LastNameTextBox.Location = new Point(244, 74);
            LastNameTextBox.Margin = new Padding(4, 3, 4, 3);
            LastNameTextBox.MaxLength = 50;
            LastNameTextBox.Name = "LastNameTextBox";
            LastNameTextBox.Size = new Size(187, 23);
            LastNameTextBox.TabIndex = 41;
            // 
            // SaveButton
            // 
            SaveButton.Location = new Point(358, 415);
            SaveButton.Name = "SaveButton";
            SaveButton.Size = new Size(75, 23);
            SaveButton.TabIndex = 56;
            SaveButton.Text = "Guardar";
            SaveButton.UseVisualStyleBackColor = true;
            SaveButton.Click += SaveButton_Click;
            // 
            // CancelarButton
            // 
            CancelarButton.Location = new Point(23, 415);
            CancelarButton.Name = "CancelarButton";
            CancelarButton.Size = new Size(75, 23);
            CancelarButton.TabIndex = 57;
            CancelarButton.Text = "Cancelar";
            CancelarButton.UseVisualStyleBackColor = true;
            CancelarButton.Click += CancelarButton_Click;
            // 
            // DniLabel
            // 
            DniLabel.AutoSize = true;
            DniLabel.Location = new Point(18, 10);
            DniLabel.Margin = new Padding(4, 0, 4, 0);
            DniLabel.Name = "DniLabel";
            DniLabel.Size = new Size(27, 15);
            DniLabel.TabIndex = 59;
            DniLabel.Text = "DNI";
            // 
            // DniTextBox
            // 
            DniTextBox.BackColor = SystemColors.Window;
            DniTextBox.Location = new Point(21, 29);
            DniTextBox.Margin = new Padding(4, 3, 4, 3);
            DniTextBox.MaxLength = 8;
            DniTextBox.Name = "DniTextBox";
            DniTextBox.Size = new Size(187, 23);
            DniTextBox.TabIndex = 58;
            DniTextBox.TextChanged += DniTextBox_TextChanged;
            // 
            // Medicacion_Label
            // 
            Medicacion_Label.AutoSize = true;
            Medicacion_Label.Location = new Point(21, 220);
            Medicacion_Label.Margin = new Padding(4, 0, 4, 0);
            Medicacion_Label.Name = "Medicacion_Label";
            Medicacion_Label.Size = new Size(69, 15);
            Medicacion_Label.TabIndex = 61;
            Medicacion_Label.Text = "Medicacion";
            // 
            // Medicacion_TextBox
            // 
            Medicacion_TextBox.Location = new Point(18, 238);
            Medicacion_TextBox.Margin = new Padding(4, 3, 4, 3);
            Medicacion_TextBox.Multiline = true;
            Medicacion_TextBox.Name = "Medicacion_TextBox";
            Medicacion_TextBox.ScrollBars = ScrollBars.Vertical;
            Medicacion_TextBox.Size = new Size(292, 143);
            Medicacion_TextBox.TabIndex = 62;
            // 
            // NewUser
            // 
            AcceptButton = SaveButton;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(Medicacion_TextBox);
            Controls.Add(Medicacion_Label);
            Controls.Add(DniLabel);
            Controls.Add(DniTextBox);
            Controls.Add(CancelarButton);
            Controls.Add(SaveButton);
            Controls.Add(PhoneTextBox);
            Controls.Add(PhoneLabel);
            Controls.Add(label8);
            Controls.Add(label7);
            Controls.Add(AntecFamiTextBox);
            Controls.Add(AntecPersTextBox);
            Controls.Add(FirstNameTextBox);
            Controls.Add(NroAsociadoLabel);
            Controls.Add(BirthDateTextBox);
            Controls.Add(ObraSocialLabel);
            Controls.Add(LastNameLabel);
            Controls.Add(FirstNameLabel);
            Controls.Add(BirthLabel);
            Controls.Add(NroAsociadoTextBox);
            Controls.Add(ObraSocialTextBox);
            Controls.Add(LastNameTextBox);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "NewUser";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Paciente";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private TextBox PhoneTextBox;
        private Label PhoneLabel;
        private Label label8;
        private Label label7;
        private TextBox AntecFamiTextBox;
        private TextBox AntecPersTextBox;
        private TextBox FirstNameTextBox;
        private Label NroAsociadoLabel;
        private TextBox BirthDateTextBox;
        private Label ObraSocialLabel;
        private Label LastNameLabel;
        private Label FirstNameLabel;
        private Label BirthLabel;
        private TextBox NroAsociadoTextBox;
        private TextBox ObraSocialTextBox;
        private TextBox LastNameTextBox;
        private Button SaveButton;
        private Button CancelarButton;
        private Label DniLabel;
        private TextBox DniTextBox;
        private Label Medicacion_Label;
        private TextBox Medicacion_TextBox;
    }
}