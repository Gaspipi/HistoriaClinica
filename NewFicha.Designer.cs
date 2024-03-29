﻿namespace HistoriaClinica
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
            CancelarButton = new Button();
            DniTextBox = new TextBox();
            EnfermedadTextBox = new TextBox();
            MotivoTextBox = new TextBox();
            DniLabel = new Label();
            EnfermedadLabel = new Label();
            MotivoLabel = new Label();
            IndicacionesLabel = new Label();
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
            // CancelarButton
            // 
            CancelarButton.Location = new Point(35, 292);
            CancelarButton.Name = "CancelarButton";
            CancelarButton.Size = new Size(75, 23);
            CancelarButton.TabIndex = 12;
            CancelarButton.Text = "Cancelar";
            CancelarButton.UseVisualStyleBackColor = true;
            CancelarButton.Click += CancelarButton_Click;
            // 
            // DniTextBox
            // 
            DniTextBox.Location = new Point(35, 26);
            DniTextBox.MaxLength = 8;
            DniTextBox.Name = "DniTextBox";
            DniTextBox.ReadOnly = true;
            DniTextBox.Size = new Size(100, 23);
            DniTextBox.TabIndex = 2;
            // 
            // EnfermedadTextBox
            // 
            EnfermedadTextBox.Location = new Point(243, 94);
            EnfermedadTextBox.Multiline = true;
            EnfermedadTextBox.Name = "EnfermedadTextBox";
            EnfermedadTextBox.Size = new Size(180, 182);
            EnfermedadTextBox.TabIndex = 5;
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
            // EnfermedadLabel
            // 
            EnfermedadLabel.AutoSize = true;
            EnfermedadLabel.Location = new Point(243, 76);
            EnfermedadLabel.Name = "EnfermedadLabel";
            EnfermedadLabel.Size = new Size(106, 15);
            EnfermedadLabel.TabIndex = 6;
            EnfermedadLabel.Text = "Enfermedad actual";
            // 
            // MotivoLabel
            // 
            MotivoLabel.AutoSize = true;
            MotivoLabel.Location = new Point(35, 76);
            MotivoLabel.Name = "MotivoLabel";
            MotivoLabel.Size = new Size(45, 15);
            MotivoLabel.TabIndex = 7;
            MotivoLabel.Text = "Motivo";
            // 
            // IndicacionesLabel
            // 
            IndicacionesLabel.AutoSize = true;
            IndicacionesLabel.Location = new Point(451, 76);
            IndicacionesLabel.Name = "IndicacionesLabel";
            IndicacionesLabel.Size = new Size(73, 15);
            IndicacionesLabel.TabIndex = 8;
            IndicacionesLabel.Text = "Indicaciones";
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
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            CancelButton = CancelarButton;
            ClientSize = new Size(661, 333);
            Controls.Add(IndicacionesTextBox);
            Controls.Add(IndicacionesLabel);
            Controls.Add(MotivoLabel);
            Controls.Add(EnfermedadLabel);
            Controls.Add(DniLabel);
            Controls.Add(MotivoTextBox);
            Controls.Add(EnfermedadTextBox);
            Controls.Add(DniTextBox);
            Controls.Add(CancelarButton);
            Controls.Add(SaveButton);
            HelpButton = true;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "NewFicha";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Ficha diaria";
            FormClosed += NewFicha_FormClosed;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button SaveButton;
        private Button CancelarButton;
        private TextBox DniTextBox;
        private TextBox EnfermedadTextBox;
        private TextBox MotivoTextBox;
        private Label DniLabel;
        private Label EnfermedadLabel;
        private Label MotivoLabel;
        private Label IndicacionesLabel;
        private TextBox IndicacionesTextBox;
    }
}