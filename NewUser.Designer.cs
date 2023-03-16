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
            this.PhoneTextBox = new System.Windows.Forms.TextBox();
            this.PhoneLabel = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.AntecFamiTextBox = new System.Windows.Forms.TextBox();
            this.AntecPersTextBox = new System.Windows.Forms.TextBox();
            this.FirstNameTextBox = new System.Windows.Forms.TextBox();
            this.NroAsociadoLabel = new System.Windows.Forms.Label();
            this.BirthDateTextBox = new System.Windows.Forms.TextBox();
            this.ObraSocialLabel = new System.Windows.Forms.Label();
            this.LastNameLabel = new System.Windows.Forms.Label();
            this.FirstNameLabel = new System.Windows.Forms.Label();
            this.BirthLabel = new System.Windows.Forms.Label();
            this.NroAsociadoTextBox = new System.Windows.Forms.TextBox();
            this.ObraSocialTextBox = new System.Windows.Forms.TextBox();
            this.LastNameTextBox = new System.Windows.Forms.TextBox();
            this.SaveButton = new System.Windows.Forms.Button();
            this.CancelButton = new System.Windows.Forms.Button();
            this.DniLabel = new System.Windows.Forms.Label();
            this.DniTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // PhoneTextBox
            // 
            this.PhoneTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.PhoneTextBox.Location = new System.Drawing.Point(244, 125);
            this.PhoneTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.PhoneTextBox.MaxLength = 50;
            this.PhoneTextBox.Name = "PhoneTextBox";
            this.PhoneTextBox.Size = new System.Drawing.Size(187, 23);
            this.PhoneTextBox.TabIndex = 43;
            this.PhoneTextBox.TextChanged += new System.EventHandler(this.PhoneTextBox_TextChanged);
            // 
            // PhoneLabel
            // 
            this.PhoneLabel.AutoSize = true;
            this.PhoneLabel.Location = new System.Drawing.Point(241, 107);
            this.PhoneLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.PhoneLabel.Name = "PhoneLabel";
            this.PhoneLabel.Size = new System.Drawing.Size(21, 15);
            this.PhoneLabel.TabIndex = 55;
            this.PhoneLabel.Text = "Tel";
            this.PhoneLabel.Click += new System.EventHandler(this.PhoneLabel_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(491, 255);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(135, 15);
            this.label8.TabIndex = 54;
            this.label8.Text = "Antecedentes Familiares";
            this.label8.Click += new System.EventHandler(this.label8_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(491, 10);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(138, 15);
            this.label7.TabIndex = 53;
            this.label7.Text = "Antecedentes Personales";
            this.label7.Click += new System.EventHandler(this.label7_Click);
            // 
            // AntecFamiTextBox
            // 
            this.AntecFamiTextBox.Location = new System.Drawing.Point(495, 273);
            this.AntecFamiTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.AntecFamiTextBox.Multiline = true;
            this.AntecFamiTextBox.Name = "AntecFamiTextBox";
            this.AntecFamiTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.AntecFamiTextBox.Size = new System.Drawing.Size(292, 143);
            this.AntecFamiTextBox.TabIndex = 47;
            this.AntecFamiTextBox.TextChanged += new System.EventHandler(this.AntecFamiTextBox_TextChanged);
            // 
            // AntecPersTextBox
            // 
            this.AntecPersTextBox.Location = new System.Drawing.Point(495, 30);
            this.AntecPersTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.AntecPersTextBox.Multiline = true;
            this.AntecPersTextBox.Name = "AntecPersTextBox";
            this.AntecPersTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.AntecPersTextBox.Size = new System.Drawing.Size(292, 143);
            this.AntecPersTextBox.TabIndex = 46;
            this.AntecPersTextBox.TextChanged += new System.EventHandler(this.AntecPersTextBox_TextChanged);
            // 
            // FirstNameTextBox
            // 
            this.FirstNameTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.FirstNameTextBox.Location = new System.Drawing.Point(21, 74);
            this.FirstNameTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.FirstNameTextBox.MaxLength = 50;
            this.FirstNameTextBox.Name = "FirstNameTextBox";
            this.FirstNameTextBox.Size = new System.Drawing.Size(187, 23);
            this.FirstNameTextBox.TabIndex = 40;
            this.FirstNameTextBox.TextChanged += new System.EventHandler(this.FirstNameTextBox_TextChanged);
            // 
            // NroAsociadoLabel
            // 
            this.NroAsociadoLabel.AutoSize = true;
            this.NroAsociadoLabel.Location = new System.Drawing.Point(241, 166);
            this.NroAsociadoLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.NroAsociadoLabel.Name = "NroAsociadoLabel";
            this.NroAsociadoLabel.Size = new System.Drawing.Size(79, 15);
            this.NroAsociadoLabel.TabIndex = 52;
            this.NroAsociadoLabel.Text = "Nro Asociado";
            this.NroAsociadoLabel.Click += new System.EventHandler(this.NroAsociadoLabel_Click);
            // 
            // BirthDateTextBox
            // 
            this.BirthDateTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.BirthDateTextBox.Location = new System.Drawing.Point(21, 125);
            this.BirthDateTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.BirthDateTextBox.MaxLength = 10;
            this.BirthDateTextBox.Name = "BirthDateTextBox";
            this.BirthDateTextBox.Size = new System.Drawing.Size(116, 23);
            this.BirthDateTextBox.TabIndex = 42;
            this.BirthDateTextBox.Text = "00/00/0000";
            this.BirthDateTextBox.TextChanged += new System.EventHandler(this.BirthDateTextBox_TextChanged);
            // 
            // ObraSocialLabel
            // 
            this.ObraSocialLabel.AutoSize = true;
            this.ObraSocialLabel.Location = new System.Drawing.Point(18, 166);
            this.ObraSocialLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.ObraSocialLabel.Name = "ObraSocialLabel";
            this.ObraSocialLabel.Size = new System.Drawing.Size(67, 15);
            this.ObraSocialLabel.TabIndex = 51;
            this.ObraSocialLabel.Text = "Obra Social";
            this.ObraSocialLabel.Click += new System.EventHandler(this.ObraSocialLabel_Click);
            // 
            // LastNameLabel
            // 
            this.LastNameLabel.AutoSize = true;
            this.LastNameLabel.Location = new System.Drawing.Point(241, 55);
            this.LastNameLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LastNameLabel.Name = "LastNameLabel";
            this.LastNameLabel.Size = new System.Drawing.Size(51, 15);
            this.LastNameLabel.TabIndex = 50;
            this.LastNameLabel.Text = "Apellido";
            this.LastNameLabel.Click += new System.EventHandler(this.LastNameLabel_Click);
            // 
            // FirstNameLabel
            // 
            this.FirstNameLabel.AutoSize = true;
            this.FirstNameLabel.Location = new System.Drawing.Point(18, 55);
            this.FirstNameLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.FirstNameLabel.Name = "FirstNameLabel";
            this.FirstNameLabel.Size = new System.Drawing.Size(51, 15);
            this.FirstNameLabel.TabIndex = 49;
            this.FirstNameLabel.Text = "Nombre";
            this.FirstNameLabel.Click += new System.EventHandler(this.FirstNameLabel_Click);
            // 
            // BirthLabel
            // 
            this.BirthLabel.AutoSize = true;
            this.BirthLabel.Location = new System.Drawing.Point(18, 107);
            this.BirthLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.BirthLabel.Name = "BirthLabel";
            this.BirthLabel.Size = new System.Drawing.Size(69, 15);
            this.BirthLabel.TabIndex = 48;
            this.BirthLabel.Text = "Nacimiento";
            this.BirthLabel.Click += new System.EventHandler(this.BirthLabel_Click);
            // 
            // NroAsociadoTextBox
            // 
            this.NroAsociadoTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.NroAsociadoTextBox.Location = new System.Drawing.Point(244, 184);
            this.NroAsociadoTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.NroAsociadoTextBox.MaxLength = 50;
            this.NroAsociadoTextBox.Name = "NroAsociadoTextBox";
            this.NroAsociadoTextBox.Size = new System.Drawing.Size(187, 23);
            this.NroAsociadoTextBox.TabIndex = 45;
            this.NroAsociadoTextBox.TextChanged += new System.EventHandler(this.NroAsociadoTextBox_TextChanged);
            // 
            // ObraSocialTextBox
            // 
            this.ObraSocialTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.ObraSocialTextBox.Location = new System.Drawing.Point(21, 184);
            this.ObraSocialTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ObraSocialTextBox.MaxLength = 50;
            this.ObraSocialTextBox.Name = "ObraSocialTextBox";
            this.ObraSocialTextBox.Size = new System.Drawing.Size(187, 23);
            this.ObraSocialTextBox.TabIndex = 44;
            this.ObraSocialTextBox.TextChanged += new System.EventHandler(this.ObraSocialTextBox_TextChanged);
            // 
            // LastNameTextBox
            // 
            this.LastNameTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.LastNameTextBox.Location = new System.Drawing.Point(244, 74);
            this.LastNameTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.LastNameTextBox.MaxLength = 50;
            this.LastNameTextBox.Name = "LastNameTextBox";
            this.LastNameTextBox.Size = new System.Drawing.Size(187, 23);
            this.LastNameTextBox.TabIndex = 41;
            this.LastNameTextBox.TextChanged += new System.EventHandler(this.LastNameTextBox_TextChanged);
            // 
            // SaveButton
            // 
            this.SaveButton.Location = new System.Drawing.Point(358, 415);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(75, 23);
            this.SaveButton.TabIndex = 56;
            this.SaveButton.Text = "Guardar";
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // CancelButton
            // 
            this.CancelButton.Location = new System.Drawing.Point(23, 415);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(75, 23);
            this.CancelButton.TabIndex = 57;
            this.CancelButton.Text = "Cancelar";
            this.CancelButton.UseVisualStyleBackColor = true;
            this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // DniLabel
            // 
            this.DniLabel.AutoSize = true;
            this.DniLabel.Location = new System.Drawing.Point(18, 10);
            this.DniLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.DniLabel.Name = "DniLabel";
            this.DniLabel.Size = new System.Drawing.Size(27, 15);
            this.DniLabel.TabIndex = 59;
            this.DniLabel.Text = "DNI";
            this.DniLabel.Click += new System.EventHandler(this.label1_Click);
            // 
            // DniTextBox
            // 
            this.DniTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.DniTextBox.Location = new System.Drawing.Point(21, 29);
            this.DniTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.DniTextBox.MaxLength = 8;
            this.DniTextBox.Name = "DniTextBox";
            this.DniTextBox.Size = new System.Drawing.Size(187, 23);
            this.DniTextBox.TabIndex = 58;
            this.DniTextBox.TextChanged += new System.EventHandler(this.DniTextBox_TextChanged);
            // 
            // NewUser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.DniLabel);
            this.Controls.Add(this.DniTextBox);
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.SaveButton);
            this.Controls.Add(this.PhoneTextBox);
            this.Controls.Add(this.PhoneLabel);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.AntecFamiTextBox);
            this.Controls.Add(this.AntecPersTextBox);
            this.Controls.Add(this.FirstNameTextBox);
            this.Controls.Add(this.NroAsociadoLabel);
            this.Controls.Add(this.BirthDateTextBox);
            this.Controls.Add(this.ObraSocialLabel);
            this.Controls.Add(this.LastNameLabel);
            this.Controls.Add(this.FirstNameLabel);
            this.Controls.Add(this.BirthLabel);
            this.Controls.Add(this.NroAsociadoTextBox);
            this.Controls.Add(this.ObraSocialTextBox);
            this.Controls.Add(this.LastNameTextBox);
            this.Name = "NewUser";
            this.Text = "NewUser";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.NewUser_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

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
        private Button CancelButton;
        private Label DniLabel;
        private TextBox DniTextBox;
    }
}