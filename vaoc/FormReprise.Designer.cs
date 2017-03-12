namespace vaoc
{
    partial class FormReprise
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
            this.buttonMajDonnees = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.checkBoxSauvegarde = new System.Windows.Forms.CheckBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.backgroundTraitement = new System.ComponentModel.BackgroundWorker();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxTour = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // buttonMajDonnees
            // 
            this.buttonMajDonnees.Location = new System.Drawing.Point(141, 43);
            this.buttonMajDonnees.Name = "buttonMajDonnees";
            this.buttonMajDonnees.Size = new System.Drawing.Size(151, 23);
            this.buttonMajDonnees.TabIndex = 27;
            this.buttonMajDonnees.Tag = "Uniquement si TAB_VIDEO n\'a pas été mis à jour durant la partie";
            this.buttonMajDonnees.Text = "Mise à jour des données";
            this.buttonMajDonnees.UseVisualStyleBackColor = true;
            this.buttonMajDonnees.Click += new System.EventHandler(this.buttonMajDonnees_Click);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(12, 12);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(732, 23);
            this.progressBar.Step = 1;
            this.progressBar.TabIndex = 28;
            // 
            // checkBoxSauvegarde
            // 
            this.checkBoxSauvegarde.AutoSize = true;
            this.checkBoxSauvegarde.Checked = true;
            this.checkBoxSauvegarde.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxSauvegarde.Location = new System.Drawing.Point(12, 47);
            this.checkBoxSauvegarde.Name = "checkBoxSauvegarde";
            this.checkBoxSauvegarde.Size = new System.Drawing.Size(110, 17);
            this.checkBoxSauvegarde.TabIndex = 29;
            this.checkBoxSauvegarde.Text = "Avec sauvegarde";
            this.checkBoxSauvegarde.UseVisualStyleBackColor = true;
            // 
            // buttonOK
            // 
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(591, 47);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(151, 23);
            this.buttonOK.TabIndex = 30;
            this.buttonOK.Tag = "";
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            // 
            // backgroundTraitement
            // 
            this.backgroundTraitement.WorkerReportsProgress = true;
            this.backgroundTraitement.WorkerSupportsCancellation = true;
            this.backgroundTraitement.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundTraitement_DoWork);
            this.backgroundTraitement.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundTraitement_ProgressChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(298, 48);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 13);
            this.label1.TabIndex = 31;
            this.label1.Text = "A partir du tour :";
            // 
            // textBoxTour
            // 
            this.textBoxTour.Location = new System.Drawing.Point(386, 45);
            this.textBoxTour.MaxLength = 3;
            this.textBoxTour.Name = "textBoxTour";
            this.textBoxTour.Size = new System.Drawing.Size(34, 20);
            this.textBoxTour.TabIndex = 32;
            this.textBoxTour.Text = "0";
            // 
            // FormReprise
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(754, 85);
            this.Controls.Add(this.textBoxTour);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.checkBoxSauvegarde);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.buttonMajDonnees);
            this.Name = "FormReprise";
            this.Text = "FormReprise";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonMajDonnees;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.CheckBox checkBoxSauvegarde;
        private System.Windows.Forms.Button buttonOK;
        private System.ComponentModel.BackgroundWorker backgroundTraitement;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxTour;
    }
}