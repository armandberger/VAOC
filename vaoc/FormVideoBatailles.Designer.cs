
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
            this.buttonValider.Location = new System.Drawing.Point(330, 435);
            this.buttonValider.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.buttonValider.Name = "buttonValider";
            this.buttonValider.Size = new System.Drawing.Size(88, 27);
            this.buttonValider.TabIndex = 12;
            this.buttonValider.Text = "Fermer";
            this.buttonValider.UseVisualStyleBackColor = true;
            // 
            // buttonGenerer
            // 
            this.buttonGenerer.Location = new System.Drawing.Point(30, 435);
            this.buttonGenerer.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.buttonGenerer.Name = "buttonGenerer";
            this.buttonGenerer.Size = new System.Drawing.Size(219, 27);
            this.buttonGenerer.TabIndex = 13;
            this.buttonGenerer.Text = "Gener le(s) Film(s)";
            this.buttonGenerer.UseVisualStyleBackColor = true;
            this.buttonGenerer.Click += new System.EventHandler(this.buttonGenerer_Click);
            // 
            // comboBoxBatailles
            // 
            this.comboBoxBatailles.FormattingEnabled = true;
            this.comboBoxBatailles.Location = new System.Drawing.Point(30, 14);
            this.comboBoxBatailles.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.comboBoxBatailles.Name = "comboBoxBatailles";
            this.comboBoxBatailles.Size = new System.Drawing.Size(867, 23);
            this.comboBoxBatailles.TabIndex = 14;
            // 
            // textBoxHauteurBase
            // 
            this.textBoxHauteurBase.Location = new System.Drawing.Point(246, 58);
            this.textBoxHauteurBase.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBoxHauteurBase.Name = "textBoxHauteurBase";
            this.textBoxHauteurBase.Size = new System.Drawing.Size(78, 23);
            this.textBoxHauteurBase.TabIndex = 18;
            this.textBoxHauteurBase.Text = "1200";
            // 
            // textBoxLargeurBase
            // 
            this.textBoxLargeurBase.Location = new System.Drawing.Point(85, 58);
            this.textBoxLargeurBase.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBoxLargeurBase.Name = "textBoxLargeurBase";
            this.textBoxLargeurBase.Size = new System.Drawing.Size(78, 23);
            this.textBoxLargeurBase.TabIndex = 17;
            this.textBoxLargeurBase.Text = "1600";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(187, 58);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(50, 15);
            this.label4.TabIndex = 16;
            this.label4.Text = "Hauteur";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(28, 58);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 15);
            this.label3.TabIndex = 15;
            this.label3.Text = "Largeur";
            // 
            // labelPolice
            // 
            this.labelPolice.Font = new System.Drawing.Font("Georgia", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.labelPolice.Location = new System.Drawing.Point(30, 170);
            this.labelPolice.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelPolice.Name = "labelPolice";
            this.labelPolice.Size = new System.Drawing.Size(867, 174);
            this.labelPolice.TabIndex = 21;
            this.labelPolice.Text = "Police de caractère";
            // 
            // buttonChoixPolice
            // 
            this.buttonChoixPolice.Location = new System.Drawing.Point(27, 119);
            this.buttonChoixPolice.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.buttonChoixPolice.Name = "buttonChoixPolice";
            this.buttonChoixPolice.Size = new System.Drawing.Size(208, 23);
            this.buttonChoixPolice.TabIndex = 20;
            this.buttonChoixPolice.Text = "Choix de la Police";
            this.buttonChoixPolice.UseVisualStyleBackColor = true;
            this.buttonChoixPolice.Click += new System.EventHandler(this.buttonChoixPolice_Click);
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(260, 119);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(648, 51);
            this.label5.TabIndex = 19;
            this.label5.Text = "note : la taille de la vidéo finale garde la proportion de la source en ayant, po" +
    "ur maximum, soit la hauteur, soit la largeur de base, de façon à ce que aucune d" +
    "es deux valeurs ne soit dépassée";
            // 
            // FormVideoBatailles
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1091, 750);
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
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
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