namespace vaoc
{
    partial class FormFondDeCarte
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
            this.dataGridViewCouleurs = new System.Windows.Forms.DataGridView();
            this.ID_METEO = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ID_MODELE_MOUVEMENT = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ID_MODELE_DU_TERRAIN = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Meteo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Modele = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NOM_DU_TERRAIN = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Couleur = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Vision = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Cout = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.buttonImageCarte = new System.Windows.Forms.Button();
            this.buttonAnnuler = new System.Windows.Forms.Button();
            this.buttonValider = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.labelNom = new System.Windows.Forms.Label();
            this.labelLargeur = new System.Windows.Forms.Label();
            this.labelHauteur = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.labelTraitement = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.timerTraitement = new System.Windows.Forms.Timer(this.components);
            this.dataGridViewModelesTerrain = new System.Windows.Forms.DataGridView();
            this.labelValiditéBase = new System.Windows.Forms.Label();
            this.textBoxCoutDeBase = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.labelValiditeMax = new System.Windows.Forms.Label();
            this.buttonEffacer = new System.Windows.Forms.Button();
            this.buttonRechercherCouleur = new System.Windows.Forms.Button();
            this.textBoxR = new System.Windows.Forms.TextBox();
            this.textBoxB = new System.Windows.Forms.TextBox();
            this.textBoxG = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBoxPositionCouleur = new System.Windows.Forms.GroupBox();
            this.buttonTraitement = new System.Windows.Forms.Button();
            this.buttonAjouterModele = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.buttonModifierModele = new System.Windows.Forms.Button();
            this.buttonSupprimerModele = new System.Windows.Forms.Button();
            this.ID_MODELE_TERRAIN = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NOM_TERRAIN = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.B_ANNULEE_SI_OCCUPEE = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.I_MODIFICATEUR_DEFENSE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TERRAIN_COULEUR = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RGB = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NB_CASES = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ID_GRAPHIQUE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Pont = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Ponton = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Detruit = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Routier = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ID_NOUVEAU_TERRAIN = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.B_OBSTACLE_DEFENSIF = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.B_ANNULEE_EN_COMBAT = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCouleurs)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewModelesTerrain)).BeginInit();
            this.groupBoxPositionCouleur.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridViewCouleurs
            // 
            this.dataGridViewCouleurs.AllowUserToAddRows = false;
            this.dataGridViewCouleurs.AllowUserToDeleteRows = false;
            this.dataGridViewCouleurs.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewCouleurs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewCouleurs.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ID_METEO,
            this.ID_MODELE_MOUVEMENT,
            this.ID_MODELE_DU_TERRAIN,
            this.Meteo,
            this.Modele,
            this.NOM_DU_TERRAIN,
            this.Couleur,
            this.Vision,
            this.Cout});
            this.dataGridViewCouleurs.Location = new System.Drawing.Point(0, 409);
            this.dataGridViewCouleurs.Name = "dataGridViewCouleurs";
            this.dataGridViewCouleurs.Size = new System.Drawing.Size(828, 225);
            this.dataGridViewCouleurs.TabIndex = 4;
            this.dataGridViewCouleurs.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewCouleurs_CellValueChanged);
            // 
            // ID_METEO
            // 
            this.ID_METEO.HeaderText = "ID_METEO";
            this.ID_METEO.Name = "ID_METEO";
            this.ID_METEO.Visible = false;
            // 
            // ID_MODELE_MOUVEMENT
            // 
            this.ID_MODELE_MOUVEMENT.HeaderText = "ID_MODELE_MOUVEMENT";
            this.ID_MODELE_MOUVEMENT.Name = "ID_MODELE_MOUVEMENT";
            this.ID_MODELE_MOUVEMENT.Visible = false;
            // 
            // ID_MODELE_DU_TERRAIN
            // 
            this.ID_MODELE_DU_TERRAIN.HeaderText = "ID_MODELE_DU_TERRAIN";
            this.ID_MODELE_DU_TERRAIN.Name = "ID_MODELE_DU_TERRAIN";
            this.ID_MODELE_DU_TERRAIN.ReadOnly = true;
            this.ID_MODELE_DU_TERRAIN.Visible = false;
            // 
            // Meteo
            // 
            this.Meteo.HeaderText = "Météo";
            this.Meteo.Name = "Meteo";
            this.Meteo.ReadOnly = true;
            // 
            // Modele
            // 
            this.Modele.HeaderText = "Modèle";
            this.Modele.Name = "Modele";
            this.Modele.ReadOnly = true;
            // 
            // NOM_DU_TERRAIN
            // 
            this.NOM_DU_TERRAIN.HeaderText = "Terrain";
            this.NOM_DU_TERRAIN.Name = "NOM_DU_TERRAIN";
            this.NOM_DU_TERRAIN.ReadOnly = true;
            // 
            // Couleur
            // 
            this.Couleur.HeaderText = "Couleur";
            this.Couleur.Name = "Couleur";
            this.Couleur.ReadOnly = true;
            // 
            // Vision
            // 
            this.Vision.HeaderText = "Vision";
            this.Vision.Name = "Vision";
            this.Vision.ReadOnly = true;
            // 
            // Cout
            // 
            this.Cout.HeaderText = "Cout";
            this.Cout.MaxInputLength = 5;
            this.Cout.Name = "Cout";
            // 
            // buttonImageCarte
            // 
            this.buttonImageCarte.Location = new System.Drawing.Point(15, 104);
            this.buttonImageCarte.Name = "buttonImageCarte";
            this.buttonImageCarte.Size = new System.Drawing.Size(138, 23);
            this.buttonImageCarte.TabIndex = 0;
            this.buttonImageCarte.Text = "Ouvrir Image Carte";
            this.buttonImageCarte.UseVisualStyleBackColor = true;
            this.buttonImageCarte.Click += new System.EventHandler(this.buttonImageCarte_Click);
            // 
            // buttonAnnuler
            // 
            this.buttonAnnuler.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonAnnuler.Location = new System.Drawing.Point(721, 68);
            this.buttonAnnuler.Name = "buttonAnnuler";
            this.buttonAnnuler.Size = new System.Drawing.Size(75, 23);
            this.buttonAnnuler.TabIndex = 4;
            this.buttonAnnuler.Text = "Annuler";
            this.buttonAnnuler.UseVisualStyleBackColor = true;
            // 
            // buttonValider
            // 
            this.buttonValider.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonValider.Location = new System.Drawing.Point(721, 40);
            this.buttonValider.Name = "buttonValider";
            this.buttonValider.Size = new System.Drawing.Size(75, 23);
            this.buttonValider.TabIndex = 3;
            this.buttonValider.Text = "Valider";
            this.buttonValider.UseVisualStyleBackColor = true;
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(12, 7);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(784, 23);
            this.progressBar.Step = 1;
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar.TabIndex = 7;
            // 
            // labelNom
            // 
            this.labelNom.AutoSize = true;
            this.labelNom.Location = new System.Drawing.Point(68, 78);
            this.labelNom.Name = "labelNom";
            this.labelNom.Size = new System.Drawing.Size(82, 13);
            this.labelNom.TabIndex = 8;
            this.labelNom.Text = "Nom de la carte";
            // 
            // labelLargeur
            // 
            this.labelLargeur.AutoSize = true;
            this.labelLargeur.Location = new System.Drawing.Point(567, 78);
            this.labelLargeur.Name = "labelLargeur";
            this.labelLargeur.Size = new System.Drawing.Size(37, 13);
            this.labelLargeur.TabIndex = 9;
            this.labelLargeur.Text = "0 pixel";
            // 
            // labelHauteur
            // 
            this.labelHauteur.AutoSize = true;
            this.labelHauteur.Location = new System.Drawing.Point(445, 78);
            this.labelHauteur.Name = "labelHauteur";
            this.labelHauteur.Size = new System.Drawing.Size(37, 13);
            this.labelHauteur.TabIndex = 10;
            this.labelHauteur.Text = "0 pixel";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Traitement :";
            // 
            // labelTraitement
            // 
            this.labelTraitement.AutoSize = true;
            this.labelTraitement.Location = new System.Drawing.Point(80, 50);
            this.labelTraitement.Name = "labelTraitement";
            this.labelTraitement.Size = new System.Drawing.Size(38, 13);
            this.labelTraitement.TabIndex = 12;
            this.labelTraitement.Text = "Aucun";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 78);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "Nom :";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(388, 78);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "Hauteur :";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(509, 78);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(52, 13);
            this.label4.TabIndex = 15;
            this.label4.Text = "Largeur : ";
            // 
            // openFileDialog
            // 
            this.openFileDialog.Title = "Ouvrir un fichier Carte";
            // 
            // timerTraitement
            // 
            this.timerTraitement.Interval = 10;
            this.timerTraitement.Tick += new System.EventHandler(this.timerTraitement_Tick);
            // 
            // dataGridViewModelesTerrain
            // 
            this.dataGridViewModelesTerrain.AllowUserToAddRows = false;
            this.dataGridViewModelesTerrain.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewModelesTerrain.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewModelesTerrain.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ID_MODELE_TERRAIN,
            this.NOM_TERRAIN,
            this.B_ANNULEE_SI_OCCUPEE,
            this.I_MODIFICATEUR_DEFENSE,
            this.TERRAIN_COULEUR,
            this.RGB,
            this.NB_CASES,
            this.ID_GRAPHIQUE,
            this.Pont,
            this.Ponton,
            this.Detruit,
            this.Routier,
            this.ID_NOUVEAU_TERRAIN,
            this.B_OBSTACLE_DEFENSIF,
            this.B_ANNULEE_EN_COMBAT});
            this.dataGridViewModelesTerrain.Location = new System.Drawing.Point(0, 259);
            this.dataGridViewModelesTerrain.Name = "dataGridViewModelesTerrain";
            this.dataGridViewModelesTerrain.Size = new System.Drawing.Size(828, 150);
            this.dataGridViewModelesTerrain.TabIndex = 17;
            this.dataGridViewModelesTerrain.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewModelesTerrain_CellValueChanged);
            this.dataGridViewModelesTerrain.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.dataGridViewModelesTerrain_UserDeletingRow);
            // 
            // labelValiditéBase
            // 
            this.labelValiditéBase.Location = new System.Drawing.Point(12, 171);
            this.labelValiditéBase.Name = "labelValiditéBase";
            this.labelValiditéBase.Size = new System.Drawing.Size(355, 32);
            this.labelValiditéBase.TabIndex = 20;
            this.labelValiditéBase.Text = "Test de cohérence base";
            // 
            // textBoxCoutDeBase
            // 
            this.textBoxCoutDeBase.Location = new System.Drawing.Point(111, 140);
            this.textBoxCoutDeBase.Name = "textBoxCoutDeBase";
            this.textBoxCoutDeBase.Size = new System.Drawing.Size(42, 20);
            this.textBoxCoutDeBase.TabIndex = 2;
            this.textBoxCoutDeBase.Tag = "la valeur pour laquelle une case équivaut à la valeur \'1\'";
            this.textBoxCoutDeBase.TextChanged += new System.EventHandler(this.textBoxCoutDeBase_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 140);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(70, 13);
            this.label5.TabIndex = 22;
            this.label5.Text = "Cout de base";
            // 
            // labelValiditeMax
            // 
            this.labelValiditeMax.Location = new System.Drawing.Point(9, 207);
            this.labelValiditeMax.Name = "labelValiditeMax";
            this.labelValiditeMax.Size = new System.Drawing.Size(337, 32);
            this.labelValiditeMax.TabIndex = 23;
            this.labelValiditeMax.Text = "Test de cohérence max";
            // 
            // buttonEffacer
            // 
            this.buttonEffacer.Location = new System.Drawing.Point(303, 104);
            this.buttonEffacer.Name = "buttonEffacer";
            this.buttonEffacer.Size = new System.Drawing.Size(138, 23);
            this.buttonEffacer.TabIndex = 1;
            this.buttonEffacer.Text = "Effacer les modèles";
            this.buttonEffacer.UseVisualStyleBackColor = true;
            this.buttonEffacer.Click += new System.EventHandler(this.buttonEffacer_Click);
            // 
            // buttonRechercherCouleur
            // 
            this.buttonRechercherCouleur.Location = new System.Drawing.Point(64, 56);
            this.buttonRechercherCouleur.Name = "buttonRechercherCouleur";
            this.buttonRechercherCouleur.Size = new System.Drawing.Size(138, 23);
            this.buttonRechercherCouleur.TabIndex = 3;
            this.buttonRechercherCouleur.Text = "Rechercher";
            this.buttonRechercherCouleur.UseVisualStyleBackColor = true;
            this.buttonRechercherCouleur.Click += new System.EventHandler(this.buttonRechercherCouleur_Click);
            // 
            // textBoxR
            // 
            this.textBoxR.Location = new System.Drawing.Point(52, 25);
            this.textBoxR.Name = "textBoxR";
            this.textBoxR.Size = new System.Drawing.Size(42, 20);
            this.textBoxR.TabIndex = 0;
            this.textBoxR.Tag = "la valeur pour laquelle une case équivaut à la valeur \'1\'";
            // 
            // textBoxB
            // 
            this.textBoxB.Location = new System.Drawing.Point(189, 25);
            this.textBoxB.Name = "textBoxB";
            this.textBoxB.Size = new System.Drawing.Size(42, 20);
            this.textBoxB.TabIndex = 2;
            this.textBoxB.Tag = "la valeur pour laquelle une case équivaut à la valeur \'1\'";
            // 
            // textBoxG
            // 
            this.textBoxG.Location = new System.Drawing.Point(121, 25);
            this.textBoxG.Name = "textBoxG";
            this.textBoxG.Size = new System.Drawing.Size(42, 20);
            this.textBoxG.TabIndex = 1;
            this.textBoxG.Tag = "la valeur pour laquelle une case équivaut à la valeur \'1\'";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(28, 32);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(21, 13);
            this.label6.TabIndex = 29;
            this.label6.Text = "R :";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(97, 32);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(21, 13);
            this.label7.TabIndex = 30;
            this.label7.Text = "G :";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(166, 32);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(20, 13);
            this.label8.TabIndex = 31;
            this.label8.Text = "B :";
            // 
            // groupBoxPositionCouleur
            // 
            this.groupBoxPositionCouleur.Controls.Add(this.label8);
            this.groupBoxPositionCouleur.Controls.Add(this.label7);
            this.groupBoxPositionCouleur.Controls.Add(this.label6);
            this.groupBoxPositionCouleur.Controls.Add(this.textBoxG);
            this.groupBoxPositionCouleur.Controls.Add(this.textBoxB);
            this.groupBoxPositionCouleur.Controls.Add(this.textBoxR);
            this.groupBoxPositionCouleur.Controls.Add(this.buttonRechercherCouleur);
            this.groupBoxPositionCouleur.Location = new System.Drawing.Point(448, 104);
            this.groupBoxPositionCouleur.Name = "groupBoxPositionCouleur";
            this.groupBoxPositionCouleur.Size = new System.Drawing.Size(267, 88);
            this.groupBoxPositionCouleur.TabIndex = 32;
            this.groupBoxPositionCouleur.TabStop = false;
            this.groupBoxPositionCouleur.Text = "Rechercher la position d\'une couleur";
            // 
            // buttonTraitement
            // 
            this.buttonTraitement.Location = new System.Drawing.Point(159, 104);
            this.buttonTraitement.Name = "buttonTraitement";
            this.buttonTraitement.Size = new System.Drawing.Size(138, 23);
            this.buttonTraitement.TabIndex = 33;
            this.buttonTraitement.Text = "Traitement";
            this.buttonTraitement.UseVisualStyleBackColor = true;
            this.buttonTraitement.Click += new System.EventHandler(this.buttonTraitement_Click);
            // 
            // buttonAjouterModele
            // 
            this.buttonAjouterModele.Location = new System.Drawing.Point(448, 198);
            this.buttonAjouterModele.Name = "buttonAjouterModele";
            this.buttonAjouterModele.Size = new System.Drawing.Size(75, 55);
            this.buttonAjouterModele.TabIndex = 34;
            this.buttonAjouterModele.Text = "Ajouter un modèle de terrain";
            this.buttonAjouterModele.UseVisualStyleBackColor = true;
            this.buttonAjouterModele.Click += new System.EventHandler(this.buttonAjouterModele_Click);
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(162, 138);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(259, 32);
            this.label9.TabIndex = 35;
            this.label9.Text = "Inutilisable si occupée = indique une case qui, si elle est occupée, bloque le mo" +
                "uvement";
            // 
            // buttonModifierModele
            // 
            this.buttonModifierModele.Location = new System.Drawing.Point(529, 198);
            this.buttonModifierModele.Name = "buttonModifierModele";
            this.buttonModifierModele.Size = new System.Drawing.Size(75, 55);
            this.buttonModifierModele.TabIndex = 36;
            this.buttonModifierModele.Text = "Modifier un modèle de terrain";
            this.buttonModifierModele.UseVisualStyleBackColor = true;
            this.buttonModifierModele.Click += new System.EventHandler(this.buttonModifierModele_Click);
            // 
            // buttonSupprimerModele
            // 
            this.buttonSupprimerModele.Location = new System.Drawing.Point(610, 198);
            this.buttonSupprimerModele.Name = "buttonSupprimerModele";
            this.buttonSupprimerModele.Size = new System.Drawing.Size(75, 55);
            this.buttonSupprimerModele.TabIndex = 37;
            this.buttonSupprimerModele.Text = "Supprimer un modèle de terrain";
            this.buttonSupprimerModele.UseVisualStyleBackColor = true;
            this.buttonSupprimerModele.Click += new System.EventHandler(this.buttonSupprimerModele_Click);
            // 
            // ID_MODELE_TERRAIN
            // 
            this.ID_MODELE_TERRAIN.FillWeight = 103.7132F;
            this.ID_MODELE_TERRAIN.HeaderText = "ID_MODELE_TERRAIN";
            this.ID_MODELE_TERRAIN.Name = "ID_MODELE_TERRAIN";
            this.ID_MODELE_TERRAIN.ReadOnly = true;
            // 
            // NOM_TERRAIN
            // 
            this.NOM_TERRAIN.FillWeight = 103.7132F;
            this.NOM_TERRAIN.HeaderText = "Terrain";
            this.NOM_TERRAIN.Name = "NOM_TERRAIN";
            // 
            // B_ANNULEE_SI_OCCUPEE
            // 
            this.B_ANNULEE_SI_OCCUPEE.FillWeight = 103.7132F;
            this.B_ANNULEE_SI_OCCUPEE.HeaderText = "Inutilisable si occupée";
            this.B_ANNULEE_SI_OCCUPEE.Name = "B_ANNULEE_SI_OCCUPEE";
            // 
            // I_MODIFICATEUR_DEFENSE
            // 
            this.I_MODIFICATEUR_DEFENSE.FillWeight = 103.7132F;
            this.I_MODIFICATEUR_DEFENSE.HeaderText = "Modificateur de défense";
            this.I_MODIFICATEUR_DEFENSE.Name = "I_MODIFICATEUR_DEFENSE";
            // 
            // TERRAIN_COULEUR
            // 
            this.TERRAIN_COULEUR.FillWeight = 103.7132F;
            this.TERRAIN_COULEUR.HeaderText = "Couleur";
            this.TERRAIN_COULEUR.Name = "TERRAIN_COULEUR";
            this.TERRAIN_COULEUR.ReadOnly = true;
            // 
            // RGB
            // 
            this.RGB.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.RGB.HeaderText = "RGB";
            this.RGB.Name = "RGB";
            this.RGB.ReadOnly = true;
            this.RGB.Width = 55;
            // 
            // NB_CASES
            // 
            this.NB_CASES.FillWeight = 103.7132F;
            this.NB_CASES.HeaderText = "Nb Cases";
            this.NB_CASES.Name = "NB_CASES";
            this.NB_CASES.ReadOnly = true;
            // 
            // ID_GRAPHIQUE
            // 
            this.ID_GRAPHIQUE.HeaderText = "ID_GRAPHIQUE";
            this.ID_GRAPHIQUE.Name = "ID_GRAPHIQUE";
            this.ID_GRAPHIQUE.Visible = false;
            // 
            // Pont
            // 
            this.Pont.FillWeight = 103.7132F;
            this.Pont.HeaderText = "Pont";
            this.Pont.Name = "Pont";
            // 
            // Ponton
            // 
            this.Ponton.FillWeight = 59.15494F;
            this.Ponton.HeaderText = "Ponton";
            this.Ponton.Name = "Ponton";
            // 
            // Detruit
            // 
            this.Detruit.HeaderText = "Detruit";
            this.Detruit.Name = "Detruit";
            this.Detruit.ToolTipText = "indique un élément résultant de la destruction d\'un autre";
            // 
            // Routier
            // 
            this.Routier.FillWeight = 103.7132F;
            this.Routier.HeaderText = "Routier";
            this.Routier.Name = "Routier";
            // 
            // ID_NOUVEAU_TERRAIN
            // 
            this.ID_NOUVEAU_TERRAIN.FillWeight = 103.7132F;
            this.ID_NOUVEAU_TERRAIN.HeaderText = "ID_NOUVEAU_TERRAIN";
            this.ID_NOUVEAU_TERRAIN.Name = "ID_NOUVEAU_TERRAIN";
            this.ID_NOUVEAU_TERRAIN.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ID_NOUVEAU_TERRAIN.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // B_OBSTACLE_DEFENSIF
            // 
            this.B_OBSTACLE_DEFENSIF.FillWeight = 103.7132F;
            this.B_OBSTACLE_DEFENSIF.HeaderText = "Obstacle défensif";
            this.B_OBSTACLE_DEFENSIF.Name = "B_OBSTACLE_DEFENSIF";
            this.B_OBSTACLE_DEFENSIF.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.B_OBSTACLE_DEFENSIF.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // B_ANNULEE_EN_COMBAT
            // 
            this.B_ANNULEE_EN_COMBAT.FillWeight = 103.7132F;
            this.B_ANNULEE_EN_COMBAT.HeaderText = "Sans effet au combat";
            this.B_ANNULEE_EN_COMBAT.Name = "B_ANNULEE_EN_COMBAT";
            this.B_ANNULEE_EN_COMBAT.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.B_ANNULEE_EN_COMBAT.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // FormFondDeCarte
            // 
            this.AcceptButton = this.buttonValider;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonAnnuler;
            this.ClientSize = new System.Drawing.Size(826, 634);
            this.Controls.Add(this.buttonSupprimerModele);
            this.Controls.Add(this.buttonModifierModele);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.buttonAjouterModele);
            this.Controls.Add(this.buttonTraitement);
            this.Controls.Add(this.groupBoxPositionCouleur);
            this.Controls.Add(this.buttonEffacer);
            this.Controls.Add(this.labelValiditeMax);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBoxCoutDeBase);
            this.Controls.Add(this.labelValiditéBase);
            this.Controls.Add(this.dataGridViewModelesTerrain);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.labelTraitement);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.labelHauteur);
            this.Controls.Add(this.labelLargeur);
            this.Controls.Add(this.labelNom);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.buttonAnnuler);
            this.Controls.Add(this.buttonValider);
            this.Controls.Add(this.buttonImageCarte);
            this.Controls.Add(this.dataGridViewCouleurs);
            this.Name = "FormFondDeCarte";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Fond de Carte";
            this.Load += new System.EventHandler(this.FormFondDeCarte_Load);
            this.Resize += new System.EventHandler(this.FormFondDeCarte_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCouleurs)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewModelesTerrain)).EndInit();
            this.groupBoxPositionCouleur.ResumeLayout(false);
            this.groupBoxPositionCouleur.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewCouleurs;
        private System.Windows.Forms.Button buttonImageCarte;
        private System.Windows.Forms.Button buttonAnnuler;
        private System.Windows.Forms.Button buttonValider;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label labelNom;
        private System.Windows.Forms.Label labelLargeur;
        private System.Windows.Forms.Label labelHauteur;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelTraitement;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Timer timerTraitement;
        private System.Windows.Forms.DataGridView dataGridViewModelesTerrain;
        private System.Windows.Forms.DataGridViewTextBoxColumn ID_METEO;
        private System.Windows.Forms.DataGridViewTextBoxColumn ID_MODELE_MOUVEMENT;
        private System.Windows.Forms.DataGridViewTextBoxColumn ID_MODELE_DU_TERRAIN;
        private System.Windows.Forms.DataGridViewTextBoxColumn Meteo;
        private System.Windows.Forms.DataGridViewTextBoxColumn Modele;
        private System.Windows.Forms.DataGridViewTextBoxColumn NOM_DU_TERRAIN;
        private System.Windows.Forms.DataGridViewTextBoxColumn Couleur;
        private System.Windows.Forms.DataGridViewTextBoxColumn Vision;
        private System.Windows.Forms.DataGridViewTextBoxColumn Cout;
        private System.Windows.Forms.Label labelValiditéBase;
        private System.Windows.Forms.TextBox textBoxCoutDeBase;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label labelValiditeMax;
        private System.Windows.Forms.Button buttonEffacer;
        private System.Windows.Forms.Button buttonRechercherCouleur;
        private System.Windows.Forms.TextBox textBoxR;
        private System.Windows.Forms.TextBox textBoxB;
        private System.Windows.Forms.TextBox textBoxG;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.GroupBox groupBoxPositionCouleur;
        private System.Windows.Forms.Button buttonTraitement;
        private System.Windows.Forms.Button buttonAjouterModele;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button buttonModifierModele;
        private System.Windows.Forms.Button buttonSupprimerModele;
        private System.Windows.Forms.DataGridViewTextBoxColumn ID_MODELE_TERRAIN;
        private System.Windows.Forms.DataGridViewTextBoxColumn NOM_TERRAIN;
        private System.Windows.Forms.DataGridViewCheckBoxColumn B_ANNULEE_SI_OCCUPEE;
        private System.Windows.Forms.DataGridViewTextBoxColumn I_MODIFICATEUR_DEFENSE;
        private System.Windows.Forms.DataGridViewTextBoxColumn TERRAIN_COULEUR;
        private System.Windows.Forms.DataGridViewTextBoxColumn RGB;
        private System.Windows.Forms.DataGridViewTextBoxColumn NB_CASES;
        private System.Windows.Forms.DataGridViewTextBoxColumn ID_GRAPHIQUE;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Pont;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Ponton;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Detruit;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Routier;
        private System.Windows.Forms.DataGridViewTextBoxColumn ID_NOUVEAU_TERRAIN;
        private System.Windows.Forms.DataGridViewCheckBoxColumn B_OBSTACLE_DEFENSIF;
        private System.Windows.Forms.DataGridViewCheckBoxColumn B_ANNULEE_EN_COMBAT;
    }
}