namespace vaoc
{
    partial class FormUtilisateur
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
            this.dataGridViewUtilisateur = new System.Windows.Forms.DataGridView();
            this.buttonAnnuler = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewUtilisateur)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewUtilisateur
            // 
            this.dataGridViewUtilisateur.AllowUserToAddRows = false;
            this.dataGridViewUtilisateur.AllowUserToDeleteRows = false;
            this.dataGridViewUtilisateur.AllowUserToOrderColumns = true;
            this.dataGridViewUtilisateur.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.ColumnHeader;
            this.dataGridViewUtilisateur.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewUtilisateur.Location = new System.Drawing.Point(12, 12);
            this.dataGridViewUtilisateur.Name = "dataGridViewUtilisateur";
            this.dataGridViewUtilisateur.Size = new System.Drawing.Size(544, 282);
            this.dataGridViewUtilisateur.TabIndex = 7;
            // 
            // buttonAnnuler
            // 
            this.buttonAnnuler.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonAnnuler.Location = new System.Drawing.Point(247, 314);
            this.buttonAnnuler.Name = "buttonAnnuler";
            this.buttonAnnuler.Size = new System.Drawing.Size(75, 23);
            this.buttonAnnuler.TabIndex = 5;
            this.buttonAnnuler.Text = "Fermer";
            this.buttonAnnuler.UseVisualStyleBackColor = true;
            // 
            // FormUtilisateur
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(569, 349);
            this.Controls.Add(this.dataGridViewUtilisateur);
            this.Controls.Add(this.buttonAnnuler);
            this.Name = "FormUtilisateur";
            this.Text = "Utilisateurs";
            this.Load += new System.EventHandler(this.FormUtilisateur_Load);
            this.Resize += new System.EventHandler(this.FormUtilisateur_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewUtilisateur)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewUtilisateur;
        private System.Windows.Forms.Button buttonAnnuler;
    }
}