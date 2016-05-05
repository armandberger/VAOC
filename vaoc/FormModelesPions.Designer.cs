namespace vaoc
{
    partial class FormModelesPions
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
            this.dataGridViewModelesPions = new System.Windows.Forms.DataGridView();
            this.ID_MODELE_PION = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.S_NOM = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MODELE_DE_MOUVEMENT = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.ID_NATION = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.CHOIX = new System.Windows.Forms.DataGridViewButtonColumn();
            this.COULEUR = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.I_VISION_JOUR = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.I_VISION_NUIT = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.buttonAnnuler = new System.Windows.Forms.Button();
            this.buttonValider = new System.Windows.Forms.Button();
            this.couleurDialog = new System.Windows.Forms.ColorDialog();
            this.labelModele = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewModelesPions)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewModelesPions
            // 
            this.dataGridViewModelesPions.AllowUserToOrderColumns = true;
            this.dataGridViewModelesPions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewModelesPions.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ID_MODELE_PION,
            this.S_NOM,
            this.MODELE_DE_MOUVEMENT,
            this.ID_NATION,
            this.CHOIX,
            this.COULEUR,
            this.I_VISION_JOUR,
            this.I_VISION_NUIT});
            this.dataGridViewModelesPions.Location = new System.Drawing.Point(12, 0);
            this.dataGridViewModelesPions.Name = "dataGridViewModelesPions";
            this.dataGridViewModelesPions.Size = new System.Drawing.Size(820, 278);
            this.dataGridViewModelesPions.TabIndex = 7;
            this.dataGridViewModelesPions.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewModelesPions_CellContentClick);
            // 
            // ID_MODELE_PION
            // 
            this.ID_MODELE_PION.HeaderText = "ID_MODELE_PION";
            this.ID_MODELE_PION.Name = "ID_MODELE_PION";
            // 
            // S_NOM
            // 
            this.S_NOM.HeaderText = "Nom";
            this.S_NOM.Name = "S_NOM";
            this.S_NOM.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.S_NOM.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // MODELE_DE_MOUVEMENT
            // 
            this.MODELE_DE_MOUVEMENT.HeaderText = "MODELE DE MOUVEMENT";
            this.MODELE_DE_MOUVEMENT.Name = "MODELE_DE_MOUVEMENT";
            // 
            // ID_NATION
            // 
            this.ID_NATION.HeaderText = "Nation";
            this.ID_NATION.Name = "ID_NATION";
            // 
            // CHOIX
            // 
            this.CHOIX.HeaderText = "Choisir";
            this.CHOIX.Name = "CHOIX";
            this.CHOIX.Text = "Couleur";
            // 
            // COULEUR
            // 
            this.COULEUR.HeaderText = "Couleur";
            this.COULEUR.Name = "COULEUR";
            this.COULEUR.ReadOnly = true;
            this.COULEUR.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.COULEUR.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // I_VISION_JOUR
            // 
            this.I_VISION_JOUR.HeaderText = "Portée de la vision de jour";
            this.I_VISION_JOUR.Name = "I_VISION_JOUR";
            // 
            // I_VISION_NUIT
            // 
            this.I_VISION_NUIT.HeaderText = "Portée de la vision de nuit";
            this.I_VISION_NUIT.Name = "I_VISION_NUIT";
            // 
            // buttonAnnuler
            // 
            this.buttonAnnuler.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonAnnuler.Location = new System.Drawing.Point(653, 295);
            this.buttonAnnuler.Name = "buttonAnnuler";
            this.buttonAnnuler.Size = new System.Drawing.Size(75, 23);
            this.buttonAnnuler.TabIndex = 5;
            this.buttonAnnuler.Text = "Annuler";
            this.buttonAnnuler.UseVisualStyleBackColor = true;
            // 
            // buttonValider
            // 
            this.buttonValider.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonValider.Location = new System.Drawing.Point(118, 295);
            this.buttonValider.Name = "buttonValider";
            this.buttonValider.Size = new System.Drawing.Size(75, 23);
            this.buttonValider.TabIndex = 6;
            this.buttonValider.Text = "Valider";
            this.buttonValider.UseVisualStyleBackColor = true;
            // 
            // labelModele
            // 
            this.labelModele.AutoSize = true;
            this.labelModele.Location = new System.Drawing.Point(274, 295);
            this.labelModele.MaximumSize = new System.Drawing.Size(350, 0);
            this.labelModele.Name = "labelModele";
            this.labelModele.Size = new System.Drawing.Size(317, 26);
            this.labelModele.TabIndex = 8;
            this.labelModele.Text = "Chaque nation doit posséder des modèles de pion nommés \"MESSAGER\", \"PATROUILLE\" e" +
                "t \"\"PATROUILLEMESSAGER\"";
            // 
            // FormModelesPions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(844, 326);
            this.Controls.Add(this.labelModele);
            this.Controls.Add(this.dataGridViewModelesPions);
            this.Controls.Add(this.buttonAnnuler);
            this.Controls.Add(this.buttonValider);
            this.Name = "FormModelesPions";
            this.Text = "ModelesPions";
            this.Load += new System.EventHandler(this.FormModelesPions_Load);
            this.Resize += new System.EventHandler(this.FormModelesPions_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewModelesPions)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewModelesPions;
        private System.Windows.Forms.Button buttonAnnuler;
        private System.Windows.Forms.Button buttonValider;
        private System.Windows.Forms.ColorDialog couleurDialog;
        private System.Windows.Forms.DataGridViewTextBoxColumn ID_MODELE_PION;
        private System.Windows.Forms.DataGridViewTextBoxColumn S_NOM;
        private System.Windows.Forms.DataGridViewComboBoxColumn MODELE_DE_MOUVEMENT;
        private System.Windows.Forms.DataGridViewComboBoxColumn ID_NATION;
        private System.Windows.Forms.DataGridViewButtonColumn CHOIX;
        private System.Windows.Forms.DataGridViewTextBoxColumn COULEUR;
        private System.Windows.Forms.DataGridViewTextBoxColumn I_VISION_JOUR;
        private System.Windows.Forms.DataGridViewTextBoxColumn I_VISION_NUIT;
        private System.Windows.Forms.Label labelModele;
    }
}