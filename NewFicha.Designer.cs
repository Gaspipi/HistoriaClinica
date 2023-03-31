namespace WinFormsApp1
{
    partial class NewFicha
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewFicha));
            SaveButton = new Button();
            CancelButton = new Button();
            DniTextBox = new TextBox();
            EnfermedadTextBox = new TextBox();
            MotivoTextBox = new TextBox();
            DniLabel = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            IndicacionesTextBox = new TextBox();
            SuspendLayout();
            // 
            // SaveButton
            // 
            SaveButton.Location = new Point(295, 292);
            SaveButton.Name = "SaveButton";
            SaveButton.Size = new Size(75, 23);
            SaveButton.TabIndex = 11;
            SaveButton.Text = "Guardar";
            SaveButton.UseVisualStyleBackColor = true;
            SaveButton.Click += SaveButton_Click;
            // 
            // CancelButton
            // 
            CancelButton.Location = new Point(35, 292);
            CancelButton.Name = "CancelButton";
            CancelButton.Size = new Size(75, 23);
            CancelButton.TabIndex = 12;
            CancelButton.Text = "Cancelar";
            CancelButton.UseVisualStyleBackColor = true;
            CancelButton.Click += CancelButton_Click;
            // 
            // DniTextBox
            // 
            DniTextBox.Location = new Point(35, 26);
            DniTextBox.MaxLength = 8;
            DniTextBox.Name = "DniTextBox";
            DniTextBox.ReadOnly = true;
            DniTextBox.Size = new Size(100, 23);
            DniTextBox.TabIndex = 2;
            DniTextBox.TextChanged += DniTextBox_TextChanged;
            // 
            // EnfermedadTextBox
            // 
            EnfermedadTextBox.Location = new Point(243, 94);
            EnfermedadTextBox.Multiline = true;
            EnfermedadTextBox.Name = "EnfermedadTextBox";
            EnfermedadTextBox.Size = new Size(180, 182);
            EnfermedadTextBox.TabIndex = 5;
            EnfermedadTextBox.TextChanged += EnfermedadTextBox_TextChanged;
            // 
            // MotivoTextBox
            // 
            MotivoTextBox.Location = new Point(35, 94);
            MotivoTextBox.Multiline = true;
            MotivoTextBox.Name = "MotivoTextBox";
            MotivoTextBox.Size = new Size(180, 182);
            MotivoTextBox.TabIndex = 4;
            // 
            // DniLabel
            // 
            DniLabel.AutoSize = true;
            DniLabel.Location = new Point(35, 8);
            DniLabel.Name = "DniLabel";
            DniLabel.Size = new Size(27, 15);
            DniLabel.TabIndex = 5;
            DniLabel.Text = "DNI";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(243, 76);
            label2.Name = "label2";
            label2.Size = new Size(106, 15);
            label2.TabIndex = 6;
            label2.Text = "Enfermedad actual";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(35, 76);
            label3.Name = "label3";
            label3.Size = new Size(45, 15);
            label3.TabIndex = 7;
            label3.Text = "Motivo";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(451, 76);
            label4.Name = "label4";
            label4.Size = new Size(73, 15);
            label4.TabIndex = 8;
            label4.Text = "Indicaciones";
            // 
            // IndicacionesTextBox
            // 
            IndicacionesTextBox.Location = new Point(451, 94);
            IndicacionesTextBox.Multiline = true;
            IndicacionesTextBox.Name = "IndicacionesTextBox";
            IndicacionesTextBox.Size = new Size(180, 182);
            IndicacionesTextBox.TabIndex = 6;
            // 
            // NewFicha
            // 
            AcceptButton = SaveButton;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            CancelButton = CancelButton;
            ClientSize = new Size(661, 333);
            Controls.Add(IndicacionesTextBox);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(DniLabel);
            Controls.Add(MotivoTextBox);
            Controls.Add(EnfermedadTextBox);
            Controls.Add(DniTextBox);
            Controls.Add(CancelButton);
            Controls.Add(SaveButton);
            HelpButton = true;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "NewFicha";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Ficha diaria";
            FormClosed += NewFicha_FormClosed;
            Load += NewFicha_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button SaveButton;
        private Button CancelButton;
        private TextBox DniTextBox;
        private TextBox EnfermedadTextBox;
        private TextBox MotivoTextBox;
        private Label DniLabel;
        private Label label2;
        private Label label3;
        private Label label4;
        private TextBox IndicacionesTextBox;
    }
}