namespace vaoc
{
    partial class FormRenfort
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
            this.buttonAnnuler = new System.Windows.Forms.Button();
            this.buttonValider = new System.Windows.Forms.Button();
            this.dataGridViewRenfort = new System.Windows.Forms.DataGridView();
            this.labelCommentaire = new System.Windows.Forms.Label();
            this.ID_PION = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MODELE_PION = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.ID_PION_PROPRIETAIRE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.S_NOM = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ID_CASE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.I_TOUR_ARRIVEE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.I_INFANTERIE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.I_CAVALERIE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.I_ARTILLERIE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.I_FATIGUE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.I_MORAL = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.I_MORAL_MAX = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.B_CAVALERIE_DE_LIGNE = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.B_CAVALERIE_LOURDE = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.B_GARDE = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.B_VIEILLE_GARDE = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.C_NIVEAU_DEPOT = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.I_EXPERIENCE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.I_TACTIQUE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.I_STRATEGIQUE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.C_NIVEAU_HIERARCHIQUE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.I_MATERIEL = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.I_RAVITAILLEMENT = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.B_RENFORT = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.S_MESSAGE_ARRIVEE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRenfort)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonAnnuler
            // 
            this.buttonAnnuler.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonAnnuler.Location = new System.Drawing.Point(380, 413);
            this.buttonAnnuler.Name = "buttonAnnuler";
            this.buttonAnnuler.Size = new System.Drawing.Size(75, 23);
            this.buttonAnnuler.TabIndex = 8;
            this.buttonAnnuler.Text = "Annuler";
            this.buttonAnnuler.UseVisualStyleBackColor = true;
            // 
            // buttonValider
            // 
            this.buttonValider.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonValider.Location = new System.Drawing.Point(246, 413);
            this.buttonValider.Name = "buttonValider";
            this.buttonValider.Size = new System.Drawing.Size(75, 23);
            this.buttonValider.TabIndex = 7;
            this.buttonValider.Text = "Valider";
            this.buttonValider.UseVisualStyleBackColor = true;
            // 
            // dataGridViewRenfort
            // 
            this.dataGridViewRenfort.AllowUserToOrderColumns = true;
            this.dataGridViewRenfort.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewRenfort.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ID_PION,
            this.MODELE_PION,
            this.ID_PION_PROPRIETAIRE,
            this.S_NOM,
            this.ID_CASE,
            this.I_TOUR_ARRIVEE,
            this.I_INFANTERIE,
            this.I_CAVALERIE,
            this.I_ARTILLERIE,
            this.I_FATIGUE,
            this.I_MORAL,
            this.I_MORAL_MAX,
            this.B_CAVALERIE_DE_LIGNE,
            this.B_CAVALERIE_LOURDE,
            this.B_GARDE,
            this.B_VIEILLE_GARDE,
            this.C_NIVEAU_DEPOT,
            this.I_EXPERIENCE,
            this.I_TACTIQUE,
            this.I_STRATEGIQUE,
            this.C_NIVEAU_HIERARCHIQUE,
            this.I_MATERIEL,
            this.I_RAVITAILLEMENT,
            this.B_RENFORT,
            this.S_MESSAGE_ARRIVEE});
            this.dataGridViewRenfort.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewRenfort.Name = "dataGridViewRenfort";
            this.dataGridViewRenfort.Size = new System.Drawing.Size(965, 228);
            this.dataGridViewRenfort.TabIndex = 6;
            // 
            // labelCommentaire
            // 
            this.labelCommentaire.AutoSize = true;
            this.labelCommentaire.Location = new System.Drawing.Point(12, 423);
            this.labelCommentaire.Name = "labelCommentaire";
            this.labelCommentaire.Size = new System.Drawing.Size(240, 13);
            this.labelCommentaire.TabIndex = 9;
            this.labelCommentaire.Text = "Artillerie : effectifs = nb canons, bonus = Tactique";
            // 
            // ID_PION
            // 
            this.ID_PION.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.ID_PION.HeaderText = "ID_PION";
            this.ID_PION.Name = "ID_PION";
            this.ID_PION.Width = 75;
            // 
            // MODELE_PION
            // 
            this.MODELE_PION.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.MODELE_PION.HeaderText = "MODELE_PION";
            this.MODELE_PION.Name = "MODELE_PION";
            this.MODELE_PION.Width = 90;
            // 
            // ID_PION_PROPRIETAIRE
            // 
            this.ID_PION_PROPRIETAIRE.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.ID_PION_PROPRIETAIRE.HeaderText = "ID_PION_PROPRIETAIRE";
            this.ID_PION_PROPRIETAIRE.Name = "ID_PION_PROPRIETAIRE";
            this.ID_PION_PROPRIETAIRE.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ID_PION_PROPRIETAIRE.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ID_PION_PROPRIETAIRE.Width = 142;
            // 
            // S_NOM
            // 
            this.S_NOM.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.S_NOM.HeaderText = "S_NOM";
            this.S_NOM.Name = "S_NOM";
            this.S_NOM.Width = 70;
            // 
            // ID_CASE
            // 
            this.ID_CASE.HeaderText = "ID_CASE";
            this.ID_CASE.Name = "ID_CASE";
            // 
            // I_TOUR_ARRIVEE
            // 
            this.I_TOUR_ARRIVEE.HeaderText = "I_TOUR_ARRIVEE";
            this.I_TOUR_ARRIVEE.Name = "I_TOUR_ARRIVEE";
            // 
            // I_INFANTERIE
            // 
            this.I_INFANTERIE.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.I_INFANTERIE.HeaderText = "I_INFANTERIE";
            this.I_INFANTERIE.Name = "I_INFANTERIE";
            this.I_INFANTERIE.Width = 105;
            // 
            // I_CAVALERIE
            // 
            this.I_CAVALERIE.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.I_CAVALERIE.HeaderText = "I_CAVALERIE";
            this.I_CAVALERIE.Name = "I_CAVALERIE";
            // 
            // I_ARTILLERIE
            // 
            this.I_ARTILLERIE.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.I_ARTILLERIE.HeaderText = "I_ARTILLERIE";
            this.I_ARTILLERIE.Name = "I_ARTILLERIE";
            this.I_ARTILLERIE.Width = 103;
            // 
            // I_FATIGUE
            // 
            this.I_FATIGUE.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.I_FATIGUE.HeaderText = "I_FATIGUE";
            this.I_FATIGUE.Name = "I_FATIGUE";
            this.I_FATIGUE.Width = 87;
            // 
            // I_MORAL
            // 
            this.I_MORAL.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.I_MORAL.HeaderText = "I_MORAL";
            this.I_MORAL.Name = "I_MORAL";
            this.I_MORAL.Width = 79;
            // 
            // I_MORAL_MAX
            // 
            this.I_MORAL_MAX.HeaderText = "I_MORAL_MAX";
            this.I_MORAL_MAX.Name = "I_MORAL_MAX";
            // 
            // B_CAVALERIE_DE_LIGNE
            // 
            this.B_CAVALERIE_DE_LIGNE.HeaderText = "B_CAVALERIE_DE_LIGNE";
            this.B_CAVALERIE_DE_LIGNE.Name = "B_CAVALERIE_DE_LIGNE";
            // 
            // B_CAVALERIE_LOURDE
            // 
            this.B_CAVALERIE_LOURDE.HeaderText = "B_CAVALERIE_LOURDE";
            this.B_CAVALERIE_LOURDE.Name = "B_CAVALERIE_LOURDE";
            // 
            // B_GARDE
            // 
            this.B_GARDE.HeaderText = "B_GARDE";
            this.B_GARDE.Name = "B_GARDE";
            // 
            // B_VIEILLE_GARDE
            // 
            this.B_VIEILLE_GARDE.HeaderText = "B_VIEILLE_GARDE";
            this.B_VIEILLE_GARDE.Name = "B_VIEILLE_GARDE";
            // 
            // C_NIVEAU_DEPOT
            // 
            this.C_NIVEAU_DEPOT.HeaderText = "C_NIVEAU_DEPOT";
            this.C_NIVEAU_DEPOT.Name = "C_NIVEAU_DEPOT";
            // 
            // I_EXPERIENCE
            // 
            this.I_EXPERIENCE.HeaderText = "I_EXPERIENCE";
            this.I_EXPERIENCE.Name = "I_EXPERIENCE";
            // 
            // I_TACTIQUE
            // 
            this.I_TACTIQUE.HeaderText = "I_TACTIQUE";
            this.I_TACTIQUE.Name = "I_TACTIQUE";
            // 
            // I_STRATEGIQUE
            // 
            this.I_STRATEGIQUE.HeaderText = "I_STRATEGIQUE";
            this.I_STRATEGIQUE.Name = "I_STRATEGIQUE";
            // 
            // C_NIVEAU_HIERARCHIQUE
            // 
            this.C_NIVEAU_HIERARCHIQUE.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.C_NIVEAU_HIERARCHIQUE.HeaderText = "C_NIVEAU_HIERARCHIQUE";
            this.C_NIVEAU_HIERARCHIQUE.Name = "C_NIVEAU_HIERARCHIQUE";
            this.C_NIVEAU_HIERARCHIQUE.Width = 173;
            // 
            // I_MATERIEL
            // 
            this.I_MATERIEL.HeaderText = "I_MATERIEL";
            this.I_MATERIEL.Name = "I_MATERIEL";
            // 
            // I_RAVITAILLEMENT
            // 
            this.I_RAVITAILLEMENT.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.I_RAVITAILLEMENT.HeaderText = "I_RAVITAILLEMENT";
            this.I_RAVITAILLEMENT.Name = "I_RAVITAILLEMENT";
            this.I_RAVITAILLEMENT.Width = 133;
            // 
            // B_RENFORT
            // 
            this.B_RENFORT.HeaderText = "B_RENFORT";
            this.B_RENFORT.Name = "B_RENFORT";
            // 
            // S_MESSAGE_ARRIVEE
            // 
            this.S_MESSAGE_ARRIVEE.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.S_MESSAGE_ARRIVEE.HeaderText = "S_MESSAGE_ARRIVEE";
            this.S_MESSAGE_ARRIVEE.Name = "S_MESSAGE_ARRIVEE";
            this.S_MESSAGE_ARRIVEE.Width = 150;
            // 
            // FormRenfort
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(977, 448);
            this.Controls.Add(this.labelCommentaire);
            this.Controls.Add(this.buttonAnnuler);
            this.Controls.Add(this.buttonValider);
            this.Controls.Add(this.dataGridViewRenfort);
            this.Name = "FormRenfort";
            this.Text = "FormRenfort";
            this.Load += new System.EventHandler(this.FormPion_Load);
            this.Resize += new System.EventHandler(this.FormPion_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRenfort)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonAnnuler;
        private System.Windows.Forms.Button buttonValider;
        private System.Windows.Forms.DataGridView dataGridViewRenfort;
        private System.Windows.Forms.Label labelCommentaire;
        private System.Windows.Forms.DataGridViewTextBoxColumn ID_PION;
        private System.Windows.Forms.DataGridViewComboBoxColumn MODELE_PION;
        private System.Windows.Forms.DataGridViewTextBoxColumn ID_PION_PROPRIETAIRE;
        private System.Windows.Forms.DataGridViewTextBoxColumn S_NOM;
        private System.Windows.Forms.DataGridViewTextBoxColumn ID_CASE;
        private System.Windows.Forms.DataGridViewTextBoxColumn I_TOUR_ARRIVEE;
        private System.Windows.Forms.DataGridViewTextBoxColumn I_INFANTERIE;
        private System.Windows.Forms.DataGridViewTextBoxColumn I_CAVALERIE;
        private System.Windows.Forms.DataGridViewTextBoxColumn I_ARTILLERIE;
        private System.Windows.Forms.DataGridViewTextBoxColumn I_FATIGUE;
        private System.Windows.Forms.DataGridViewTextBoxColumn I_MORAL;
        private System.Windows.Forms.DataGridViewTextBoxColumn I_MORAL_MAX;
        private System.Windows.Forms.DataGridViewCheckBoxColumn B_CAVALERIE_DE_LIGNE;
        private System.Windows.Forms.DataGridViewCheckBoxColumn B_CAVALERIE_LOURDE;
        private System.Windows.Forms.DataGridViewCheckBoxColumn B_GARDE;
        private System.Windows.Forms.DataGridViewCheckBoxColumn B_VIEILLE_GARDE;
        private System.Windows.Forms.DataGridViewTextBoxColumn C_NIVEAU_DEPOT;
        private System.Windows.Forms.DataGridViewTextBoxColumn I_EXPERIENCE;
        private System.Windows.Forms.DataGridViewTextBoxColumn I_TACTIQUE;
        private System.Windows.Forms.DataGridViewTextBoxColumn I_STRATEGIQUE;
        private System.Windows.Forms.DataGridViewTextBoxColumn C_NIVEAU_HIERARCHIQUE;
        private System.Windows.Forms.DataGridViewTextBoxColumn I_MATERIEL;
        private System.Windows.Forms.DataGridViewTextBoxColumn I_RAVITAILLEMENT;
        private System.Windows.Forms.DataGridViewCheckBoxColumn B_RENFORT;
        private System.Windows.Forms.DataGridViewTextBoxColumn S_MESSAGE_ARRIVEE;
    }
}