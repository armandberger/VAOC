namespace vaoc
{
    partial class FormControlePCC
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
            BoiteInformation = new System.Windows.Forms.GroupBox();
            label4 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            textBoxIDTrajet = new System.Windows.Forms.TextBox();
            textBoxCout = new System.Windows.Forms.TextBox();
            textBoxDebut = new System.Windows.Forms.TextBox();
            textBoxFin = new System.Windows.Forms.TextBox();
            BoiteInformation.SuspendLayout();
            SuspendLayout();
            // 
            // BoiteInformation
            // 
            BoiteInformation.Controls.Add(textBoxFin);
            BoiteInformation.Controls.Add(textBoxDebut);
            BoiteInformation.Controls.Add(textBoxCout);
            BoiteInformation.Controls.Add(textBoxIDTrajet);
            BoiteInformation.Controls.Add(label4);
            BoiteInformation.Controls.Add(label3);
            BoiteInformation.Controls.Add(label2);
            BoiteInformation.Controls.Add(label1);
            BoiteInformation.Location = new System.Drawing.Point(470, 233);
            BoiteInformation.Name = "BoiteInformation";
            BoiteInformation.Size = new System.Drawing.Size(200, 142);
            BoiteInformation.TabIndex = 0;
            BoiteInformation.TabStop = false;
            BoiteInformation.Text = "Information";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(6, 103);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(23, 15);
            label4.TabIndex = 3;
            label4.Text = "Fin";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(6, 74);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(39, 15);
            label3.TabIndex = 2;
            label3.Text = "Début";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(6, 45);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(33, 15);
            label2.TabIndex = 1;
            label2.Text = "Coût";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(6, 19);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(48, 15);
            label1.TabIndex = 0;
            label1.Text = "ID trajet";
            // 
            // textBoxIDTrajet
            // 
            textBoxIDTrajet.Location = new System.Drawing.Point(65, 16);
            textBoxIDTrajet.Name = "textBoxIDTrajet";
            textBoxIDTrajet.Size = new System.Drawing.Size(100, 23);
            textBoxIDTrajet.TabIndex = 4;
            // 
            // textBoxCout
            // 
            textBoxCout.Location = new System.Drawing.Point(65, 45);
            textBoxCout.Name = "textBoxCout";
            textBoxCout.Size = new System.Drawing.Size(100, 23);
            textBoxCout.TabIndex = 5;
            // 
            // textBoxDebut
            // 
            textBoxDebut.Location = new System.Drawing.Point(65, 74);
            textBoxDebut.Name = "textBoxDebut";
            textBoxDebut.Size = new System.Drawing.Size(100, 23);
            textBoxDebut.TabIndex = 6;
            // 
            // textBoxFin
            // 
            textBoxFin.Location = new System.Drawing.Point(65, 103);
            textBoxFin.Name = "textBoxFin";
            textBoxFin.Size = new System.Drawing.Size(100, 23);
            textBoxFin.TabIndex = 7;
            // 
            // FormControlePCC
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(800, 450);
            Controls.Add(BoiteInformation);
            Name = "FormControlePCC";
            Text = "Controle PCC";
            WindowState = System.Windows.Forms.FormWindowState.Maximized;
            Load += FormControlePCC_Load;
            SizeChanged += FormControlePCC_SizeChanged;
            Paint += FormControlePCC_Paint;
            MouseClick += FormControlePCC_MouseClick;
            MouseMove += FormControlePCC_MouseMove;
            BoiteInformation.ResumeLayout(false);
            BoiteInformation.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.GroupBox BoiteInformation;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxFin;
        private System.Windows.Forms.TextBox textBoxDebut;
        private System.Windows.Forms.TextBox textBoxCout;
        private System.Windows.Forms.TextBox textBoxIDTrajet;
    }
}