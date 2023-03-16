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
            this.SaveButton = new System.Windows.Forms.Button();
            this.CancelButton = new System.Windows.Forms.Button();
            this.DniTextBox = new System.Windows.Forms.TextBox();
            this.EnfermedadTextBox = new System.Windows.Forms.TextBox();
            this.MotivoTextBox = new System.Windows.Forms.TextBox();
            this.DniLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.IndicacionesTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // SaveButton
            // 
            this.SaveButton.Location = new System.Drawing.Point(264, 284);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(75, 23);
            this.SaveButton.TabIndex = 11;
            this.SaveButton.Text = "Guardar";
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // CancelButton
            // 
            this.CancelButton.Location = new System.Drawing.Point(12, 284);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(75, 23);
            this.CancelButton.TabIndex = 12;
            this.CancelButton.Text = "Cancelar";
            this.CancelButton.UseVisualStyleBackColor = true;
            this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // DniTextBox
            // 
            this.DniTextBox.Location = new System.Drawing.Point(12, 27);
            this.DniTextBox.MaxLength = 8;
            this.DniTextBox.Name = "DniTextBox";
            this.DniTextBox.ReadOnly = true;
            this.DniTextBox.Size = new System.Drawing.Size(100, 23);
            this.DniTextBox.TabIndex = 2;
            this.DniTextBox.TextChanged += new System.EventHandler(this.DniTextBox_TextChanged);
            // 
            // EnfermedadTextBox
            // 
            this.EnfermedadTextBox.Location = new System.Drawing.Point(217, 86);
            this.EnfermedadTextBox.Multiline = true;
            this.EnfermedadTextBox.Name = "EnfermedadTextBox";
            this.EnfermedadTextBox.Size = new System.Drawing.Size(180, 125);
            this.EnfermedadTextBox.TabIndex = 5;
            this.EnfermedadTextBox.TextChanged += new System.EventHandler(this.EnfermedadTextBox_TextChanged);
            // 
            // MotivoTextBox
            // 
            this.MotivoTextBox.Location = new System.Drawing.Point(12, 86);
            this.MotivoTextBox.Multiline = true;
            this.MotivoTextBox.Name = "MotivoTextBox";
            this.MotivoTextBox.Size = new System.Drawing.Size(180, 125);
            this.MotivoTextBox.TabIndex = 4;
            // 
            // DniLabel
            // 
            this.DniLabel.AutoSize = true;
            this.DniLabel.Location = new System.Drawing.Point(12, 9);
            this.DniLabel.Name = "DniLabel";
            this.DniLabel.Size = new System.Drawing.Size(27, 15);
            this.DniLabel.TabIndex = 5;
            this.DniLabel.Text = "DNI";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(217, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 15);
            this.label2.TabIndex = 6;
            this.label2.Text = "Enfermedad";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 68);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 15);
            this.label3.TabIndex = 7;
            this.label3.Text = "Motivo";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(421, 68);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(73, 15);
            this.label4.TabIndex = 8;
            this.label4.Text = "Indicaciones";
            // 
            // IndicacionesTextBox
            // 
            this.IndicacionesTextBox.Location = new System.Drawing.Point(421, 86);
            this.IndicacionesTextBox.Multiline = true;
            this.IndicacionesTextBox.Name = "IndicacionesTextBox";
            this.IndicacionesTextBox.Size = new System.Drawing.Size(180, 125);
            this.IndicacionesTextBox.TabIndex = 6;
            // 
            // NewFicha
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(661, 333);
            this.Controls.Add(this.IndicacionesTextBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.DniLabel);
            this.Controls.Add(this.MotivoTextBox);
            this.Controls.Add(this.EnfermedadTextBox);
            this.Controls.Add(this.DniTextBox);
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.SaveButton);
            this.Name = "NewFicha";
            this.Text = "NewFicha";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.NewFicha_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

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