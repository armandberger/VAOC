namespace vaoc
{
    partial class FormPion
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
            this.buttonAnnuler = new System.Windows.Forms.Button();
            this.buttonValider = new System.Windows.Forms.Button();
            this.dataGridViewPions = new System.Windows.Forms.DataGridView();
            this.labelCommentaire = new System.Windows.Forms.Label();
            this.buttonRenfort = new System.Windows.Forms.Button();
            this.donnees = new vaoc.Donnees();
            this.tABMODELEPIONBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.ID_PION = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.B_DETRUIT = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.MODELE_PION = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.ID_PION_PROPRIETAIRE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ID_NOUVEAU_PION_PROPRIETAIRE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ID_ANCIEN_PION_PROPRIETAIRE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.S_NOM = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.I_INFANTERIE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.I_INFANTERIE_INITIALE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.I_CAVALERIE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.I_CAVALERIE_INITIALE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.I_ARTILLERIE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.I_ARTILLERIE_INITIALE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.I_FATIGUE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.I_MORAL = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.I_MORAL_MAX = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.I_EXPERIENCE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.I_TACTIQUE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.I_STRATEGIQUE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.C_NIVEAU_HIERARCHIQUE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.I_DISTANCE_A_PARCOURIR = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.I_NB_PHASES_MARCHE_JOUR = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.I_NB_PHASES_MARCHE_NUIT = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.I_NB_HEURES_COMBAT = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ID_CASE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.I_TOUR_SANS_RAVITAILLEMENT = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ID_BATAILLE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.I_ZONE_BATAILLE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.I_TOUR_RETRAITE_RESTANT = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.I_TOUR_FUITE_RESTANT = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.B_INTERCEPTION = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.B_FUITE_AU_COMBAT = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.B_REDITION_RAVITAILLEMENT = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.B_TELEPORTATION = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.B_ENNEMI_OBSERVABLE = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.I_MATERIEL = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.I_RAVITAILLEMENT = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.B_CAVALERIE_DE_LIGNE = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.B_CAVALERIE_LOURDE = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.B_GARDE = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.B_VIEILLE_GARDE = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.I_TOUR_CONVOI_CREE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ID_DEPOT_SOURCE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.I_SOLDATS_RAVITAILLES = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.I_NB_HEURES_FORTIFICATION = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.I_NIVEAU_FORTIFICATION = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ID_PION_REMPLACE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.I_DUREE_HORS_COMBAT = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.I_TOUR_BLESSURE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.B_BLESSES = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.B_PRISONNIERS = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.B_RENFORT = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ID_LIEU_RATTACHEMENT = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.C_NIVEAU_DEPOT = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ID_PION_ESCORTE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.I_INFANTERIE_ESCORTE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.I_CAVALERIE_ESCORTE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.I_MATERIEL_ESCORTE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPions)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.donnees)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tABMODELEPIONBindingSource)).BeginInit();
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
            // dataGridViewPions
            // 
            this.dataGridViewPions.AllowUserToOrderColumns = true;
            this.dataGridViewPions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewPions.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ID_PION,
            this.B_DETRUIT,
            this.MODELE_PION,
            this.ID_PION_PROPRIETAIRE,
            this.ID_NOUVEAU_PION_PROPRIETAIRE,
            this.ID_ANCIEN_PION_PROPRIETAIRE,
            this.S_NOM,
            this.I_INFANTERIE,
            this.I_INFANTERIE_INITIALE,
            this.I_CAVALERIE,
            this.I_CAVALERIE_INITIALE,
            this.I_ARTILLERIE,
            this.I_ARTILLERIE_INITIALE,
            this.I_FATIGUE,
            this.I_MORAL,
            this.I_MORAL_MAX,
            this.I_EXPERIENCE,
            this.I_TACTIQUE,
            this.I_STRATEGIQUE,
            this.C_NIVEAU_HIERARCHIQUE,
            this.I_DISTANCE_A_PARCOURIR,
            this.I_NB_PHASES_MARCHE_JOUR,
            this.I_NB_PHASES_MARCHE_NUIT,
            this.I_NB_HEURES_COMBAT,
            this.ID_CASE,
            this.I_TOUR_SANS_RAVITAILLEMENT,
            this.ID_BATAILLE,
            this.I_ZONE_BATAILLE,
            this.I_TOUR_RETRAITE_RESTANT,
            this.I_TOUR_FUITE_RESTANT,
            this.B_INTERCEPTION,
            this.B_FUITE_AU_COMBAT,
            this.B_REDITION_RAVITAILLEMENT,
            this.B_TELEPORTATION,
            this.B_ENNEMI_OBSERVABLE,
            this.I_MATERIEL,
            this.I_RAVITAILLEMENT,
            this.B_CAVALERIE_DE_LIGNE,
            this.B_CAVALERIE_LOURDE,
            this.B_GARDE,
            this.B_VIEILLE_GARDE,
            this.I_TOUR_CONVOI_CREE,
            this.ID_DEPOT_SOURCE,
            this.I_SOLDATS_RAVITAILLES,
            this.I_NB_HEURES_FORTIFICATION,
            this.I_NIVEAU_FORTIFICATION,
            this.ID_PION_REMPLACE,
            this.I_DUREE_HORS_COMBAT,
            this.I_TOUR_BLESSURE,
            this.B_BLESSES,
            this.B_PRISONNIERS,
            this.B_RENFORT,
            this.ID_LIEU_RATTACHEMENT,
            this.C_NIVEAU_DEPOT,
            this.ID_PION_ESCORTE,
            this.I_INFANTERIE_ESCORTE,
            this.I_CAVALERIE_ESCORTE,
            this.I_MATERIEL_ESCORTE});
            this.dataGridViewPions.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewPions.Name = "dataGridViewPions";
            this.dataGridViewPions.Size = new System.Drawing.Size(1001, 228);
            this.dataGridViewPions.TabIndex = 6;
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
            // buttonRenfort
            // 
            this.buttonRenfort.Location = new System.Drawing.Point(524, 413);
            this.buttonRenfort.Name = "buttonRenfort";
            this.buttonRenfort.Size = new System.Drawing.Size(114, 23);
            this.buttonRenfort.TabIndex = 10;
            this.buttonRenfort.Text = "Mettre en renfort";
            this.buttonRenfort.UseVisualStyleBackColor = true;
            this.buttonRenfort.Click += new System.EventHandler(this.buttonRenfort_Click);
            // 
            // donnees
            // 
            this.donnees.DataSetName = "Donnees";
            this.donnees.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // tABMODELEPIONBindingSource
            // 
            this.tABMODELEPIONBindingSource.DataMember = "TAB_MODELE_PION";
            this.tABMODELEPIONBindingSource.DataSource = this.donnees;
            // 
            // ID_PION
            // 
            this.ID_PION.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.ID_PION.Frozen = true;
            this.ID_PION.HeaderText = "ID_PION";
            this.ID_PION.Name = "ID_PION";
            this.ID_PION.Width = 75;
            // 
            // B_DETRUIT
            // 
            this.B_DETRUIT.HeaderText = "B_DETRUIT";
            this.B_DETRUIT.Name = "B_DETRUIT";
            this.B_DETRUIT.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
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
            this.ID_PION_PROPRIETAIRE.Width = 161;
            // 
            // ID_NOUVEAU_PION_PROPRIETAIRE
            // 
            this.ID_NOUVEAU_PION_PROPRIETAIRE.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.ID_NOUVEAU_PION_PROPRIETAIRE.HeaderText = "ID_NOUVEAU_PION_PROPRIETAIRE";
            this.ID_NOUVEAU_PION_PROPRIETAIRE.Name = "ID_NOUVEAU_PION_PROPRIETAIRE";
            this.ID_NOUVEAU_PION_PROPRIETAIRE.Width = 220;
            // 
            // ID_ANCIEN_PION_PROPRIETAIRE
            // 
            this.ID_ANCIEN_PION_PROPRIETAIRE.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.ID_ANCIEN_PION_PROPRIETAIRE.HeaderText = "ID_ANCIEN_PION_PROPRIETAIRE";
            this.ID_ANCIEN_PION_PROPRIETAIRE.Name = "ID_ANCIEN_PION_PROPRIETAIRE";
            this.ID_ANCIEN_PION_PROPRIETAIRE.Width = 207;
            // 
            // S_NOM
            // 
            this.S_NOM.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.S_NOM.HeaderText = "S_NOM";
            this.S_NOM.Name = "S_NOM";
            this.S_NOM.Width = 70;
            // 
            // I_INFANTERIE
            // 
            this.I_INFANTERIE.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.I_INFANTERIE.HeaderText = "I_INFANTERIE";
            this.I_INFANTERIE.Name = "I_INFANTERIE";
            this.I_INFANTERIE.Width = 105;
            // 
            // I_INFANTERIE_INITIALE
            // 
            this.I_INFANTERIE_INITIALE.HeaderText = "I_INFANTERIE_INITIALE";
            this.I_INFANTERIE_INITIALE.Name = "I_INFANTERIE_INITIALE";
            // 
            // I_CAVALERIE
            // 
            this.I_CAVALERIE.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.I_CAVALERIE.HeaderText = "I_CAVALERIE";
            this.I_CAVALERIE.Name = "I_CAVALERIE";
            // 
            // I_CAVALERIE_INITIALE
            // 
            this.I_CAVALERIE_INITIALE.HeaderText = "I_CAVALERIE_INITIALE";
            this.I_CAVALERIE_INITIALE.Name = "I_CAVALERIE_INITIALE";
            // 
            // I_ARTILLERIE
            // 
            this.I_ARTILLERIE.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.I_ARTILLERIE.HeaderText = "I_ARTILLERIE";
            this.I_ARTILLERIE.Name = "I_ARTILLERIE";
            this.I_ARTILLERIE.Width = 103;
            // 
            // I_ARTILLERIE_INITIALE
            // 
            this.I_ARTILLERIE_INITIALE.HeaderText = "I_ARTILLERIE_INITIALE";
            this.I_ARTILLERIE_INITIALE.Name = "I_ARTILLERIE_INITIALE";
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
            this.C_NIVEAU_HIERARCHIQUE.HeaderText = "C_NIVEAU_HIERARCHIQUE";
            this.C_NIVEAU_HIERARCHIQUE.Name = "C_NIVEAU_HIERARCHIQUE";
            // 
            // I_DISTANCE_A_PARCOURIR
            // 
            this.I_DISTANCE_A_PARCOURIR.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.I_DISTANCE_A_PARCOURIR.HeaderText = "I_DISTANCE_A_PARCOURIR";
            this.I_DISTANCE_A_PARCOURIR.Name = "I_DISTANCE_A_PARCOURIR";
            this.I_DISTANCE_A_PARCOURIR.Width = 178;
            // 
            // I_NB_PHASES_MARCHE_JOUR
            // 
            this.I_NB_PHASES_MARCHE_JOUR.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.I_NB_PHASES_MARCHE_JOUR.HeaderText = "I_NB_PHASES_MARCHE_JOUR";
            this.I_NB_PHASES_MARCHE_JOUR.Name = "I_NB_PHASES_MARCHE_JOUR";
            this.I_NB_PHASES_MARCHE_JOUR.Width = 192;
            // 
            // I_NB_PHASES_MARCHE_NUIT
            // 
            this.I_NB_PHASES_MARCHE_NUIT.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.I_NB_PHASES_MARCHE_NUIT.HeaderText = "I_NB_PHASES_MARCHE_NUIT";
            this.I_NB_PHASES_MARCHE_NUIT.Name = "I_NB_PHASES_MARCHE_NUIT";
            this.I_NB_PHASES_MARCHE_NUIT.Width = 189;
            // 
            // I_NB_HEURES_COMBAT
            // 
            this.I_NB_HEURES_COMBAT.HeaderText = "I_NB_HEURES_COMBAT";
            this.I_NB_HEURES_COMBAT.Name = "I_NB_HEURES_COMBAT";
            // 
            // ID_CASE
            // 
            this.ID_CASE.HeaderText = "ID_CASE";
            this.ID_CASE.Name = "ID_CASE";
            // 
            // I_TOUR_SANS_RAVITAILLEMENT
            // 
            this.I_TOUR_SANS_RAVITAILLEMENT.HeaderText = "I_TOUR_SANS_RAVITAILLEMENT";
            this.I_TOUR_SANS_RAVITAILLEMENT.Name = "I_TOUR_SANS_RAVITAILLEMENT";
            // 
            // ID_BATAILLE
            // 
            this.ID_BATAILLE.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.ID_BATAILLE.HeaderText = "ID_BATAILLE";
            this.ID_BATAILLE.Name = "ID_BATAILLE";
            this.ID_BATAILLE.Width = 99;
            // 
            // I_ZONE_BATAILLE
            // 
            this.I_ZONE_BATAILLE.HeaderText = "I_ZONE_BATAILLE";
            this.I_ZONE_BATAILLE.Name = "I_ZONE_BATAILLE";
            // 
            // I_TOUR_RETRAITE_RESTANT
            // 
            this.I_TOUR_RETRAITE_RESTANT.HeaderText = "I_TOUR_RETRAITE_RESTANT";
            this.I_TOUR_RETRAITE_RESTANT.Name = "I_TOUR_RETRAITE_RESTANT";
            // 
            // I_TOUR_FUITE_RESTANT
            // 
            this.I_TOUR_FUITE_RESTANT.HeaderText = "I_TOUR_FUITE_RESTANT";
            this.I_TOUR_FUITE_RESTANT.Name = "I_TOUR_FUITE_RESTANT";
            // 
            // B_INTERCEPTION
            // 
            this.B_INTERCEPTION.HeaderText = "B_INTERCEPTION";
            this.B_INTERCEPTION.Name = "B_INTERCEPTION";
            // 
            // B_FUITE_AU_COMBAT
            // 
            this.B_FUITE_AU_COMBAT.HeaderText = "B_FUITE_AU_COMBAT";
            this.B_FUITE_AU_COMBAT.Name = "B_FUITE_AU_COMBAT";
            // 
            // B_REDITION_RAVITAILLEMENT
            // 
            this.B_REDITION_RAVITAILLEMENT.HeaderText = "B_REDITION_RAVITAILLEMENT";
            this.B_REDITION_RAVITAILLEMENT.Name = "B_REDITION_RAVITAILLEMENT";
            // 
            // B_TELEPORTATION
            // 
            this.B_TELEPORTATION.HeaderText = "B_TELEPORTATION";
            this.B_TELEPORTATION.Name = "B_TELEPORTATION";
            this.B_TELEPORTATION.ReadOnly = true;
            // 
            // B_ENNEMI_OBSERVABLE
            // 
            this.B_ENNEMI_OBSERVABLE.HeaderText = "B_ENNEMI_OBSERVABLE";
            this.B_ENNEMI_OBSERVABLE.Name = "B_ENNEMI_OBSERVABLE";
            // 
            // I_MATERIEL
            // 
            this.I_MATERIEL.HeaderText = "I_MATERIEL";
            this.I_MATERIEL.Name = "I_MATERIEL";
            // 
            // I_RAVITAILLEMENT
            // 
            this.I_RAVITAILLEMENT.HeaderText = "I_RAVITAILLEMENT";
            this.I_RAVITAILLEMENT.Name = "I_RAVITAILLEMENT";
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
            // I_TOUR_CONVOI_CREE
            // 
            this.I_TOUR_CONVOI_CREE.HeaderText = "I_TOUR_CONVOI_CREE";
            this.I_TOUR_CONVOI_CREE.Name = "I_TOUR_CONVOI_CREE";
            // 
            // ID_DEPOT_SOURCE
            // 
            this.ID_DEPOT_SOURCE.HeaderText = "ID_DEPOT_SOURCE";
            this.ID_DEPOT_SOURCE.Name = "ID_DEPOT_SOURCE";
            // 
            // I_SOLDATS_RAVITAILLES
            // 
            this.I_SOLDATS_RAVITAILLES.HeaderText = "I_SOLDATS_RAVITAILLES";
            this.I_SOLDATS_RAVITAILLES.Name = "I_SOLDATS_RAVITAILLES";
            // 
            // I_NB_HEURES_FORTIFICATION
            // 
            this.I_NB_HEURES_FORTIFICATION.HeaderText = "I_NB_HEURES_FORTIFICATION";
            this.I_NB_HEURES_FORTIFICATION.Name = "I_NB_HEURES_FORTIFICATION";
            // 
            // I_NIVEAU_FORTIFICATION
            // 
            this.I_NIVEAU_FORTIFICATION.HeaderText = "I_NIVEAU_FORTIFICATION";
            this.I_NIVEAU_FORTIFICATION.Name = "I_NIVEAU_FORTIFICATION";
            // 
            // ID_PION_REMPLACE
            // 
            this.ID_PION_REMPLACE.HeaderText = "ID_PION_REMPLACE";
            this.ID_PION_REMPLACE.Name = "ID_PION_REMPLACE";
            // 
            // I_DUREE_HORS_COMBAT
            // 
            this.I_DUREE_HORS_COMBAT.HeaderText = "I_DUREE_HORS_COMBAT";
            this.I_DUREE_HORS_COMBAT.Name = "I_DUREE_HORS_COMBAT";
            // 
            // I_TOUR_BLESSURE
            // 
            this.I_TOUR_BLESSURE.HeaderText = "I_TOUR_BLESSURE";
            this.I_TOUR_BLESSURE.Name = "I_TOUR_BLESSURE";
            // 
            // B_BLESSES
            // 
            this.B_BLESSES.HeaderText = "B_BLESSES";
            this.B_BLESSES.Name = "B_BLESSES";
            // 
            // B_PRISONNIERS
            // 
            this.B_PRISONNIERS.HeaderText = "B_PRISONNIERS";
            this.B_PRISONNIERS.Name = "B_PRISONNIERS";
            // 
            // B_RENFORT
            // 
            this.B_RENFORT.HeaderText = "B_RENFORT";
            this.B_RENFORT.Name = "B_RENFORT";
            // 
            // ID_LIEU_RATTACHEMENT
            // 
            this.ID_LIEU_RATTACHEMENT.HeaderText = "ID_LIEU_RATTACHEMENT";
            this.ID_LIEU_RATTACHEMENT.Name = "ID_LIEU_RATTACHEMENT";
            // 
            // C_NIVEAU_DEPOT
            // 
            this.C_NIVEAU_DEPOT.HeaderText = "C_NIVEAU_DEPOT";
            this.C_NIVEAU_DEPOT.Name = "C_NIVEAU_DEPOT";
            // 
            // ID_PION_ESCORTE
            // 
            this.ID_PION_ESCORTE.HeaderText = "ID_PION_ESCORTE";
            this.ID_PION_ESCORTE.Name = "ID_PION_ESCORTE";
            // 
            // I_INFANTERIE_ESCORTE
            // 
            this.I_INFANTERIE_ESCORTE.HeaderText = "I_INFANTERIE_ESCORTE";
            this.I_INFANTERIE_ESCORTE.Name = "I_INFANTERIE_ESCORTE";
            // 
            // I_CAVALERIE_ESCORTE
            // 
            this.I_CAVALERIE_ESCORTE.HeaderText = "I_CAVALERIE_ESCORTE";
            this.I_CAVALERIE_ESCORTE.Name = "I_CAVALERIE_ESCORTE";
            // 
            // I_MATERIEL_ESCORTE
            // 
            this.I_MATERIEL_ESCORTE.HeaderText = "I_MATERIEL_ESCORTE";
            this.I_MATERIEL_ESCORTE.Name = "I_MATERIEL_ESCORTE";
            // 
            // FormPion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1007, 448);
            this.Controls.Add(this.buttonRenfort);
            this.Controls.Add(this.labelCommentaire);
            this.Controls.Add(this.buttonAnnuler);
            this.Controls.Add(this.buttonValider);
            this.Controls.Add(this.dataGridViewPions);
            this.Name = "FormPion";
            this.Text = "FormPion";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.FormPion_Load);
            this.Resize += new System.EventHandler(this.FormPion_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPions)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.donnees)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tABMODELEPIONBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonAnnuler;
        private System.Windows.Forms.Button buttonValider;
        private System.Windows.Forms.DataGridView dataGridViewPions;
        private System.Windows.Forms.Label labelCommentaire;
        private System.Windows.Forms.Button buttonRenfort;
        private System.Windows.Forms.BindingSource tABMODELEPIONBindingSource;
        private Donnees donnees;
        private System.Windows.Forms.DataGridViewTextBoxColumn ID_PION;
        private System.Windows.Forms.DataGridViewCheckBoxColumn B_DETRUIT;
        private System.Windows.Forms.DataGridViewComboBoxColumn MODELE_PION;
        private System.Windows.Forms.DataGridViewTextBoxColumn ID_PION_PROPRIETAIRE;
        private System.Windows.Forms.DataGridViewTextBoxColumn ID_NOUVEAU_PION_PROPRIETAIRE;
        private System.Windows.Forms.DataGridViewTextBoxColumn ID_ANCIEN_PION_PROPRIETAIRE;
        private System.Windows.Forms.DataGridViewTextBoxColumn S_NOM;
        private System.Windows.Forms.DataGridViewTextBoxColumn I_INFANTERIE;
        private System.Windows.Forms.DataGridViewTextBoxColumn I_INFANTERIE_INITIALE;
        private System.Windows.Forms.DataGridViewTextBoxColumn I_CAVALERIE;
        private System.Windows.Forms.DataGridViewTextBoxColumn I_CAVALERIE_INITIALE;
        private System.Windows.Forms.DataGridViewTextBoxColumn I_ARTILLERIE;
        private System.Windows.Forms.DataGridViewTextBoxColumn I_ARTILLERIE_INITIALE;
        private System.Windows.Forms.DataGridViewTextBoxColumn I_FATIGUE;
        private System.Windows.Forms.DataGridViewTextBoxColumn I_MORAL;
        private System.Windows.Forms.DataGridViewTextBoxColumn I_MORAL_MAX;
        private System.Windows.Forms.DataGridViewTextBoxColumn I_EXPERIENCE;
        private System.Windows.Forms.DataGridViewTextBoxColumn I_TACTIQUE;
        private System.Windows.Forms.DataGridViewTextBoxColumn I_STRATEGIQUE;
        private System.Windows.Forms.DataGridViewTextBoxColumn C_NIVEAU_HIERARCHIQUE;
        private System.Windows.Forms.DataGridViewTextBoxColumn I_DISTANCE_A_PARCOURIR;
        private System.Windows.Forms.DataGridViewTextBoxColumn I_NB_PHASES_MARCHE_JOUR;
        private System.Windows.Forms.DataGridViewTextBoxColumn I_NB_PHASES_MARCHE_NUIT;
        private System.Windows.Forms.DataGridViewTextBoxColumn I_NB_HEURES_COMBAT;
        private System.Windows.Forms.DataGridViewTextBoxColumn ID_CASE;
        private System.Windows.Forms.DataGridViewTextBoxColumn I_TOUR_SANS_RAVITAILLEMENT;
        private System.Windows.Forms.DataGridViewTextBoxColumn ID_BATAILLE;
        private System.Windows.Forms.DataGridViewTextBoxColumn I_ZONE_BATAILLE;
        private System.Windows.Forms.DataGridViewTextBoxColumn I_TOUR_RETRAITE_RESTANT;
        private System.Windows.Forms.DataGridViewTextBoxColumn I_TOUR_FUITE_RESTANT;
        private System.Windows.Forms.DataGridViewCheckBoxColumn B_INTERCEPTION;
        private System.Windows.Forms.DataGridViewCheckBoxColumn B_FUITE_AU_COMBAT;
        private System.Windows.Forms.DataGridViewCheckBoxColumn B_REDITION_RAVITAILLEMENT;
        private System.Windows.Forms.DataGridViewCheckBoxColumn B_TELEPORTATION;
        private System.Windows.Forms.DataGridViewCheckBoxColumn B_ENNEMI_OBSERVABLE;
        private System.Windows.Forms.DataGridViewTextBoxColumn I_MATERIEL;
        private System.Windows.Forms.DataGridViewTextBoxColumn I_RAVITAILLEMENT;
        private System.Windows.Forms.DataGridViewCheckBoxColumn B_CAVALERIE_DE_LIGNE;
        private System.Windows.Forms.DataGridViewCheckBoxColumn B_CAVALERIE_LOURDE;
        private System.Windows.Forms.DataGridViewCheckBoxColumn B_GARDE;
        private System.Windows.Forms.DataGridViewCheckBoxColumn B_VIEILLE_GARDE;
        private System.Windows.Forms.DataGridViewTextBoxColumn I_TOUR_CONVOI_CREE;
        private System.Windows.Forms.DataGridViewTextBoxColumn ID_DEPOT_SOURCE;
        private System.Windows.Forms.DataGridViewTextBoxColumn I_SOLDATS_RAVITAILLES;
        private System.Windows.Forms.DataGridViewTextBoxColumn I_NB_HEURES_FORTIFICATION;
        private System.Windows.Forms.DataGridViewTextBoxColumn I_NIVEAU_FORTIFICATION;
        private System.Windows.Forms.DataGridViewTextBoxColumn ID_PION_REMPLACE;
        private System.Windows.Forms.DataGridViewTextBoxColumn I_DUREE_HORS_COMBAT;
        private System.Windows.Forms.DataGridViewTextBoxColumn I_TOUR_BLESSURE;
        private System.Windows.Forms.DataGridViewCheckBoxColumn B_BLESSES;
        private System.Windows.Forms.DataGridViewCheckBoxColumn B_PRISONNIERS;
        private System.Windows.Forms.DataGridViewCheckBoxColumn B_RENFORT;
        private System.Windows.Forms.DataGridViewTextBoxColumn ID_LIEU_RATTACHEMENT;
        private System.Windows.Forms.DataGridViewTextBoxColumn C_NIVEAU_DEPOT;
        private System.Windows.Forms.DataGridViewTextBoxColumn ID_PION_ESCORTE;
        private System.Windows.Forms.DataGridViewTextBoxColumn I_INFANTERIE_ESCORTE;
        private System.Windows.Forms.DataGridViewTextBoxColumn I_CAVALERIE_ESCORTE;
        private System.Windows.Forms.DataGridViewTextBoxColumn I_MATERIEL_ESCORTE;
    }
}