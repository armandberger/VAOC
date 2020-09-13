namespace vaoc
{
    partial class FormBackup
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
            this.components = new System.ComponentModel.Container();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.textBoxRepertoireCible = new System.Windows.Forms.TextBox();
            this.buttonCible = new System.Windows.Forms.Button();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.buttonSynchroniser = new System.Windows.Forms.Button();
            this.buttonValider = new System.Windows.Forms.Button();
            this.timerTraitement = new System.Windows.Forms.Timer(this.components);
            this.textBoxRepertoireSource = new System.Windows.Forms.TextBox();
            this.labelRepertoireSource = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(28, 113);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(760, 23);
            this.progressBar.Step = 1;
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar.TabIndex = 8;
            // 
            // textBoxRepertoireCible
            // 
            this.textBoxRepertoireCible.Location = new System.Drawing.Point(193, 12);
            this.textBoxRepertoireCible.Name = "textBoxRepertoireCible";
            this.textBoxRepertoireCible.Size = new System.Drawing.Size(595, 20);
            this.textBoxRepertoireCible.TabIndex = 10;
            // 
            // buttonCible
            // 
            this.buttonCible.Location = new System.Drawing.Point(28, 10);
            this.buttonCible.Name = "buttonCible";
            this.buttonCible.Size = new System.Drawing.Size(138, 23);
            this.buttonCible.TabIndex = 9;
            this.buttonCible.Text = "Répertoire Cible";
            this.buttonCible.UseVisualStyleBackColor = true;
            this.buttonCible.Click += new System.EventHandler(this.buttonCible_Click);
            // 
            // buttonSynchroniser
            // 
            this.buttonSynchroniser.Location = new System.Drawing.Point(28, 153);
            this.buttonSynchroniser.Name = "buttonSynchroniser";
            this.buttonSynchroniser.Size = new System.Drawing.Size(628, 23);
            this.buttonSynchroniser.TabIndex = 11;
            this.buttonSynchroniser.Text = "Synchroniser les sauvegardes";
            this.buttonSynchroniser.UseVisualStyleBackColor = true;
            this.buttonSynchroniser.Click += new System.EventHandler(this.buttonSynchroniser_Click);
            // 
            // buttonValider
            // 
            this.buttonValider.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonValider.Location = new System.Drawing.Point(713, 153);
            this.buttonValider.Name = "buttonValider";
            this.buttonValider.Size = new System.Drawing.Size(75, 23);
            this.buttonValider.TabIndex = 12;
            this.buttonValider.Text = "Fermer";
            this.buttonValider.UseVisualStyleBackColor = true;
            this.buttonValider.Click += new System.EventHandler(this.buttonValider_Click);
            // 
            // timerTraitement
            // 
            this.timerTraitement.Interval = 50;
            this.timerTraitement.Tick += new System.EventHandler(this.timerTraitement_Tick);
            // 
            // textBoxRepertoireSource
            // 
            this.textBoxRepertoireSource.Enabled = false;
            this.textBoxRepertoireSource.Location = new System.Drawing.Point(193, 53);
            this.textBoxRepertoireSource.Name = "textBoxRepertoireSource";
            this.textBoxRepertoireSource.Size = new System.Drawing.Size(595, 20);
            this.textBoxRepertoireSource.TabIndex = 13;
            // 
            // labelRepertoireSource
            // 
            this.labelRepertoireSource.AutoSize = true;
            this.labelRepertoireSource.Location = new System.Drawing.Point(28, 59);
            this.labelRepertoireSource.Name = "labelRepertoireSource";
            this.labelRepertoireSource.Size = new System.Drawing.Size(91, 13);
            this.labelRepertoireSource.TabIndex = 14;
            this.labelRepertoireSource.Text = "Répertoire source";
            // 
            // FormBackup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 189);
            this.Controls.Add(this.labelRepertoireSource);
            this.Controls.Add(this.textBoxRepertoireSource);
            this.Controls.Add(this.buttonValider);
            this.Controls.Add(this.buttonSynchroniser);
            this.Controls.Add(this.textBoxRepertoireCible);
            this.Controls.Add(this.buttonCible);
            this.Controls.Add(this.progressBar);
            this.Name = "FormBackup";
            this.Text = "FormBackup";
            this.Load += new System.EventHandler(this.FormBackup_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.TextBox textBoxRepertoireCible;
        private System.Windows.Forms.Button buttonCible;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.Button buttonSynchroniser;
        private System.Windows.Forms.Button buttonValider;
        private System.Windows.Forms.Timer timerTraitement;
        private System.Windows.Forms.TextBox textBoxRepertoireSource;
        private System.Windows.Forms.Label labelRepertoireSource;
    }
}