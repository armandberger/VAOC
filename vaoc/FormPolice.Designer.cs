namespace vaoc
{
    partial class FormPolice
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
            this.fontDialog = new System.Windows.Forms.FontDialog();
            this.buttonAnnuler = new System.Windows.Forms.Button();
            this.buttonValider = new System.Windows.Forms.Button();
            this.dataGridViewPolice = new System.Windows.Forms.DataGridView();
            this.Nom = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.choisir = new System.Windows.Forms.DataGridViewButtonColumn();
            this.Couleur = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Police = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Taille = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Italique = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Barre = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Souligne = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Gras = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPolice)).BeginInit();
            this.SuspendLayout();
            // 
            // fontDialog
            // 
            this.fontDialog.AllowVerticalFonts = false;
            this.fontDialog.Font = new System.Drawing.Font("Arial Narrow", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fontDialog.ShowColor = true;
            // 
            // buttonAnnuler
            // 
            this.buttonAnnuler.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonAnnuler.Location = new System.Drawing.Point(364, 323);
            this.buttonAnnuler.Name = "buttonAnnuler";
            this.buttonAnnuler.Size = new System.Drawing.Size(75, 23);
            this.buttonAnnuler.TabIndex = 5;
            this.buttonAnnuler.Text = "Annuler";
            this.buttonAnnuler.UseVisualStyleBackColor = true;
            // 
            // buttonValider
            // 
            this.buttonValider.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonValider.Location = new System.Drawing.Point(266, 323);
            this.buttonValider.Name = "buttonValider";
            this.buttonValider.Size = new System.Drawing.Size(75, 23);
            this.buttonValider.TabIndex = 4;
            this.buttonValider.Text = "Valider";
            this.buttonValider.UseVisualStyleBackColor = true;
            // 
            // dataGridViewPolice
            // 
            this.dataGridViewPolice.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewPolice.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Nom,
            this.choisir,
            this.Couleur,
            this.Police,
            this.Taille,
            this.Italique,
            this.Barre,
            this.Souligne,
            this.Gras});
            this.dataGridViewPolice.Location = new System.Drawing.Point(3, 1);
            this.dataGridViewPolice.Name = "dataGridViewPolice";
            this.dataGridViewPolice.Size = new System.Drawing.Size(709, 306);
            this.dataGridViewPolice.TabIndex = 7;
            this.dataGridViewPolice.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // Nom
            // 
            this.Nom.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Nom.HeaderText = "Nom";
            this.Nom.Name = "Nom";
            // 
            // choisir
            // 
            this.choisir.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.choisir.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.choisir.HeaderText = "Choisir";
            this.choisir.Name = "choisir";
            this.choisir.Text = "Police";
            this.choisir.UseColumnTextForButtonValue = true;
            this.choisir.Width = 44;
            // 
            // Couleur
            // 
            this.Couleur.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.Couleur.HeaderText = "Couleur";
            this.Couleur.Name = "Couleur";
            this.Couleur.ReadOnly = true;
            this.Couleur.Width = 68;
            // 
            // Police
            // 
            this.Police.HeaderText = "Police";
            this.Police.Name = "Police";
            this.Police.ReadOnly = true;
            // 
            // Taille
            // 
            this.Taille.HeaderText = "Taille";
            this.Taille.Name = "Taille";
            this.Taille.ReadOnly = true;
            // 
            // Italique
            // 
            this.Italique.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.Italique.HeaderText = "Italique";
            this.Italique.Name = "Italique";
            this.Italique.ReadOnly = true;
            this.Italique.Width = 66;
            // 
            // Barre
            // 
            this.Barre.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.Barre.HeaderText = "Barre";
            this.Barre.Name = "Barre";
            this.Barre.ReadOnly = true;
            this.Barre.Width = 57;
            // 
            // Souligne
            // 
            this.Souligne.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.Souligne.HeaderText = "Souligne";
            this.Souligne.Name = "Souligne";
            this.Souligne.ReadOnly = true;
            this.Souligne.Width = 73;
            // 
            // Gras
            // 
            this.Gras.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.Gras.HeaderText = "Gras";
            this.Gras.Name = "Gras";
            this.Gras.ReadOnly = true;
            this.Gras.Width = 54;
            // 
            // FormPolice
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(714, 371);
            this.Controls.Add(this.dataGridViewPolice);
            this.Controls.Add(this.buttonAnnuler);
            this.Controls.Add(this.buttonValider);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormPolice";
            this.ShowInTaskbar = false;
            this.Text = "FormPolice";
            this.Load += new System.EventHandler(this.FormPolice_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPolice)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FontDialog fontDialog;
        private System.Windows.Forms.Button buttonAnnuler;
        private System.Windows.Forms.Button buttonValider;
        private System.Windows.Forms.DataGridView dataGridViewPolice;
        private System.Windows.Forms.DataGridViewTextBoxColumn Nom;
        private System.Windows.Forms.DataGridViewButtonColumn choisir;
        private System.Windows.Forms.DataGridViewTextBoxColumn Couleur;
        private System.Windows.Forms.DataGridViewTextBoxColumn Police;
        private System.Windows.Forms.DataGridViewTextBoxColumn Taille;
        private System.Windows.Forms.DataGridViewTextBoxColumn Italique;
        private System.Windows.Forms.DataGridViewTextBoxColumn Barre;
        private System.Windows.Forms.DataGridViewTextBoxColumn Souligne;
        private System.Windows.Forms.DataGridViewTextBoxColumn Gras;
    }
}