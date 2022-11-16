
namespace vaoc
{
    partial class FormVideoBatailles
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
            this.buttonValider = new System.Windows.Forms.Button();
            this.buttonGenerer = new System.Windows.Forms.Button();
            this.comboBoxBatailles = new System.Windows.Forms.ComboBox();
            this.textBoxHauteurBase = new System.Windows.Forms.TextBox();
            this.textBoxLargeurBase = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.labelPolice = new System.Windows.Forms.Label();
            this.buttonChoixPolice = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.fontDialog = new System.Windows.Forms.FontDialog();
            this.SuspendLayout();
            // 
            // buttonValider
            // 
            this.buttonValider.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonValider.Location = new System.Drawing.Point(283, 377);
            this.buttonValider.Name = "buttonValider";
            this.buttonValider.Size = new System.Drawing.Size(75, 23);
            this.buttonValider.TabIndex = 12;
            this.buttonValider.Text = "Fermer";
            this.buttonValider.UseVisualStyleBackColor = true;
            // 
            // buttonGenerer
            // 
            this.buttonGenerer.Location = new System.Drawing.Point(26, 377);
            this.buttonGenerer.Name = "buttonGenerer";
            this.buttonGenerer.Size = new System.Drawing.Size(188, 23);
            this.buttonGenerer.TabIndex = 13;
            this.buttonGenerer.Text = "Gener le(s) Film(s)";
            this.buttonGenerer.UseVisualStyleBackColor = true;
            this.buttonGenerer.Click += new System.EventHandler(this.buttonGenerer_Click);
            // 
            // comboBoxBatailles
            // 
            this.comboBoxBatailles.FormattingEnabled = true;
            this.comboBoxBatailles.Location = new System.Drawing.Point(26, 12);
            this.comboBoxBatailles.Name = "comboBoxBatailles";
            this.comboBoxBatailles.Size = new System.Drawing.Size(332, 21);
            this.comboBoxBatailles.TabIndex = 14;
            // 
            // textBoxHauteurBase
            // 
            this.textBoxHauteurBase.Location = new System.Drawing.Point(211, 50);
            this.textBoxHauteurBase.Name = "textBoxHauteurBase";
            this.textBoxHauteurBase.Size = new System.Drawing.Size(67, 20);
            this.textBoxHauteurBase.TabIndex = 18;
            this.textBoxHauteurBase.Text = "1200";
            // 
            // textBoxLargeurBase
            // 
            this.textBoxLargeurBase.Location = new System.Drawing.Point(73, 50);
            this.textBoxLargeurBase.Name = "textBoxLargeurBase";
            this.textBoxLargeurBase.Size = new System.Drawing.Size(67, 20);
            this.textBoxLargeurBase.TabIndex = 17;
            this.textBoxLargeurBase.Text = "1600";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(160, 50);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(45, 13);
            this.label4.TabIndex = 16;
            this.label4.Text = "Hauteur";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(24, 50);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 13);
            this.label3.TabIndex = 15;
            this.label3.Text = "Largeur";
            // 
            // labelPolice
            // 
            this.labelPolice.Font = new System.Drawing.Font("Georgia", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPolice.Location = new System.Drawing.Point(26, 147);
            this.labelPolice.Name = "labelPolice";
            this.labelPolice.Size = new System.Drawing.Size(743, 151);
            this.labelPolice.TabIndex = 21;
            this.labelPolice.Text = "Police de caractère";
            // 
            // buttonChoixPolice
            // 
            this.buttonChoixPolice.Location = new System.Drawing.Point(23, 103);
            this.buttonChoixPolice.Name = "buttonChoixPolice";
            this.buttonChoixPolice.Size = new System.Drawing.Size(178, 20);
            this.buttonChoixPolice.TabIndex = 20;
            this.buttonChoixPolice.Text = "Choix de la Police";
            this.buttonChoixPolice.UseVisualStyleBackColor = true;
            this.buttonChoixPolice.Click += new System.EventHandler(this.buttonChoixPolice_Click);
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(223, 103);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(555, 44);
            this.label5.TabIndex = 19;
            this.label5.Text = "note : la taille de la vidéo finale garde la proportion de la source en ayant, po" +
    "ur maximum, soit la hauteur, soit la largeur de base, de façon à ce que aucune d" +
    "es deux valeurs ne soit dépassée";
            // 
            // FormVideoBatailles
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(935, 650);
            this.Controls.Add(this.labelPolice);
            this.Controls.Add(this.buttonChoixPolice);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBoxHauteurBase);
            this.Controls.Add(this.textBoxLargeurBase);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.comboBoxBatailles);
            this.Controls.Add(this.buttonGenerer);
            this.Controls.Add(this.buttonValider);
            this.Name = "FormVideoBatailles";
            this.Text = "FormVideoBatailles";
            this.Load += new System.EventHandler(this.FormVideoBatailles_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonValider;
        private System.Windows.Forms.Button buttonGenerer;
        private System.Windows.Forms.ComboBox comboBoxBatailles;
        private System.Windows.Forms.TextBox textBoxHauteurBase;
        private System.Windows.Forms.TextBox textBoxLargeurBase;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label labelPolice;
        private System.Windows.Forms.Button buttonChoixPolice;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.FontDialog fontDialog;
    }
}