namespace vaoc
{
    partial class FormNomCarte
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
            this.comboBoxPosition = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonAnnuler = new System.Windows.Forms.Button();
            this.buttonValider = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxNom = new System.Windows.Forms.TextBox();
            this.comboBoxPolice = new System.Windows.Forms.ComboBox();
            this.buttonSupprimer = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.labelX = new System.Windows.Forms.Label();
            this.labelY = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxIDCASE = new System.Windows.Forms.TextBox();
            this.textBoxIDNOM = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.textBoxVictoire = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.textBoxPourcentArriveeRenfort = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.comboBoxModeleRenfort = new System.Windows.Forms.ComboBox();
            this.groupBoxNom = new System.Windows.Forms.GroupBox();
            this.groupBoxRenfort = new System.Windows.Forms.GroupBox();
            this.textBoxRenfortRavitaillement = new System.Windows.Forms.TextBox();
            this.textBoxRenfortMateriel = new System.Windows.Forms.TextBox();
            this.textBoxRenfortCavaliers = new System.Windows.Forms.TextBox();
            this.textBoxRenfortMoral = new System.Windows.Forms.TextBox();
            this.textBoxRenfortCanons = new System.Windows.Forms.TextBox();
            this.textBoxRenfortFantassins = new System.Windows.Forms.TextBox();
            this.comboBoxPionPropriétaire = new System.Windows.Forms.ComboBox();
            this.label19 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.checkBoxPrison = new System.Windows.Forms.CheckBox();
            this.checkBoxHopital = new System.Windows.Forms.CheckBox();
            this.buttonSupprimerTout = new System.Windows.Forms.Button();
            this.comboBoxProprietaire = new System.Windows.Forms.ComboBox();
            this.label20 = new System.Windows.Forms.Label();
            this.groupBoxNom.SuspendLayout();
            this.groupBoxRenfort.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboBoxPosition
            // 
            this.comboBoxPosition.FormattingEnabled = true;
            this.comboBoxPosition.Items.AddRange(new object[] {
            "Central",
            "En Haut",
            "En Haut à droite",
            "A droite",
            "En bas à droite",
            "En bas",
            "En bas à gauche",
            "A gauche",
            "En Haut à gauche"});
            this.comboBoxPosition.Location = new System.Drawing.Point(378, 62);
            this.comboBoxPosition.Name = "comboBoxPosition";
            this.comboBoxPosition.Size = new System.Drawing.Size(123, 21);
            this.comboBoxPosition.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(272, 62);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Position";
            // 
            // buttonAnnuler
            // 
            this.buttonAnnuler.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonAnnuler.Location = new System.Drawing.Point(175, 453);
            this.buttonAnnuler.Name = "buttonAnnuler";
            this.buttonAnnuler.Size = new System.Drawing.Size(75, 23);
            this.buttonAnnuler.TabIndex = 7;
            this.buttonAnnuler.Text = "Annuler";
            this.buttonAnnuler.UseVisualStyleBackColor = true;
            // 
            // buttonValider
            // 
            this.buttonValider.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonValider.Location = new System.Drawing.Point(13, 453);
            this.buttonValider.Name = "buttonValider";
            this.buttonValider.Size = new System.Drawing.Size(75, 23);
            this.buttonValider.TabIndex = 5;
            this.buttonValider.Text = "Valider";
            this.buttonValider.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(272, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Nom";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 62);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(36, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Police";
            // 
            // textBoxNom
            // 
            this.textBoxNom.Location = new System.Drawing.Point(378, 29);
            this.textBoxNom.MaxLength = 255;
            this.textBoxNom.Name = "textBoxNom";
            this.textBoxNom.Size = new System.Drawing.Size(121, 20);
            this.textBoxNom.TabIndex = 0;
            // 
            // comboBoxPolice
            // 
            this.comboBoxPolice.FormattingEnabled = true;
            this.comboBoxPolice.Location = new System.Drawing.Point(120, 62);
            this.comboBoxPolice.Name = "comboBoxPolice";
            this.comboBoxPolice.Size = new System.Drawing.Size(121, 21);
            this.comboBoxPolice.TabIndex = 1;
            // 
            // buttonSupprimer
            // 
            this.buttonSupprimer.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonSupprimer.Location = new System.Drawing.Point(94, 453);
            this.buttonSupprimer.Name = "buttonSupprimer";
            this.buttonSupprimer.Size = new System.Drawing.Size(75, 23);
            this.buttonSupprimer.TabIndex = 6;
            this.buttonSupprimer.Text = "Supprimer";
            this.buttonSupprimer.UseVisualStyleBackColor = true;
            this.buttonSupprimer.Click += new System.EventHandler(this.buttonSupprimer_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(70, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Coordonnées";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(202, 9);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(20, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Y=";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(128, 9);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(20, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "X=";
            // 
            // labelX
            // 
            this.labelX.AutoSize = true;
            this.labelX.Location = new System.Drawing.Point(154, 9);
            this.labelX.Name = "labelX";
            this.labelX.Size = new System.Drawing.Size(37, 13);
            this.labelX.TabIndex = 12;
            this.labelX.Text = "00000";
            // 
            // labelY
            // 
            this.labelY.AutoSize = true;
            this.labelY.Location = new System.Drawing.Point(228, 9);
            this.labelY.Name = "labelY";
            this.labelY.Size = new System.Drawing.Size(37, 13);
            this.labelY.TabIndex = 13;
            this.labelY.Text = "00000";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(19, 35);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(58, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "ID_CASE :";
            // 
            // textBoxIDCASE
            // 
            this.textBoxIDCASE.Location = new System.Drawing.Point(131, 35);
            this.textBoxIDCASE.MaxLength = 255;
            this.textBoxIDCASE.Name = "textBoxIDCASE";
            this.textBoxIDCASE.ReadOnly = true;
            this.textBoxIDCASE.Size = new System.Drawing.Size(121, 20);
            this.textBoxIDCASE.TabIndex = 15;
            // 
            // textBoxIDNOM
            // 
            this.textBoxIDNOM.Location = new System.Drawing.Point(120, 26);
            this.textBoxIDNOM.MaxLength = 255;
            this.textBoxIDNOM.Name = "textBoxIDNOM";
            this.textBoxIDNOM.ReadOnly = true;
            this.textBoxIDNOM.Size = new System.Drawing.Size(121, 20);
            this.textBoxIDNOM.TabIndex = 17;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(15, 29);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(55, 13);
            this.label8.TabIndex = 16;
            this.label8.Text = "ID_NOM :";
            // 
            // textBoxVictoire
            // 
            this.textBoxVictoire.Location = new System.Drawing.Point(389, 35);
            this.textBoxVictoire.MaxLength = 5;
            this.textBoxVictoire.Name = "textBoxVictoire";
            this.textBoxVictoire.Size = new System.Drawing.Size(60, 20);
            this.textBoxVictoire.TabIndex = 0;
            this.textBoxVictoire.Text = "0";
            this.textBoxVictoire.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(280, 35);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(74, 13);
            this.label9.TabIndex = 19;
            this.label9.Text = "Pts de victoire";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(13, 26);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(96, 13);
            this.label10.TabIndex = 20;
            this.label10.Text = "% d\'arrivée / heure";
            // 
            // textBoxPourcentArriveeRenfort
            // 
            this.textBoxPourcentArriveeRenfort.Location = new System.Drawing.Point(118, 19);
            this.textBoxPourcentArriveeRenfort.MaxLength = 5;
            this.textBoxPourcentArriveeRenfort.Name = "textBoxPourcentArriveeRenfort";
            this.textBoxPourcentArriveeRenfort.Size = new System.Drawing.Size(60, 20);
            this.textBoxPourcentArriveeRenfort.TabIndex = 0;
            this.textBoxPourcentArriveeRenfort.Text = "0";
            this.textBoxPourcentArriveeRenfort.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(271, 26);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(65, 13);
            this.label11.TabIndex = 22;
            this.label11.Text = "Modèle pion";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(13, 55);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(71, 13);
            this.label12.TabIndex = 23;
            this.label12.Text = "Nb fantassins";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(13, 84);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(59, 13);
            this.label13.TabIndex = 24;
            this.label13.Text = "Nb canons";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(270, 55);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(66, 13);
            this.label14.TabIndex = 25;
            this.label14.Text = "Nb cavaliers";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(271, 82);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(65, 13);
            this.label15.TabIndex = 26;
            this.label15.Text = "Variation (%)";
            // 
            // comboBoxModeleRenfort
            // 
            this.comboBoxModeleRenfort.DisplayMember = "nomListe";
            this.comboBoxModeleRenfort.FormattingEnabled = true;
            this.comboBoxModeleRenfort.Items.AddRange(new object[] {
            "Central",
            "En Haut",
            "En Haut à droite",
            "A droite",
            "En bas à droite",
            "En bas",
            "En bas à gauche",
            "A gauche",
            "En Haut à gauche"});
            this.comboBoxModeleRenfort.Location = new System.Drawing.Point(375, 18);
            this.comboBoxModeleRenfort.Name = "comboBoxModeleRenfort";
            this.comboBoxModeleRenfort.Size = new System.Drawing.Size(123, 21);
            this.comboBoxModeleRenfort.TabIndex = 1;
            // 
            // groupBoxNom
            // 
            this.groupBoxNom.Controls.Add(this.textBoxIDNOM);
            this.groupBoxNom.Controls.Add(this.label8);
            this.groupBoxNom.Controls.Add(this.comboBoxPolice);
            this.groupBoxNom.Controls.Add(this.textBoxNom);
            this.groupBoxNom.Controls.Add(this.label3);
            this.groupBoxNom.Controls.Add(this.label2);
            this.groupBoxNom.Controls.Add(this.comboBoxPosition);
            this.groupBoxNom.Controls.Add(this.label1);
            this.groupBoxNom.Location = new System.Drawing.Point(11, 107);
            this.groupBoxNom.Name = "groupBoxNom";
            this.groupBoxNom.Size = new System.Drawing.Size(527, 100);
            this.groupBoxNom.TabIndex = 1;
            this.groupBoxNom.TabStop = false;
            this.groupBoxNom.Text = "Nom";
            // 
            // groupBoxRenfort
            // 
            this.groupBoxRenfort.Controls.Add(this.textBoxRenfortRavitaillement);
            this.groupBoxRenfort.Controls.Add(this.textBoxRenfortMateriel);
            this.groupBoxRenfort.Controls.Add(this.textBoxRenfortCavaliers);
            this.groupBoxRenfort.Controls.Add(this.textBoxRenfortMoral);
            this.groupBoxRenfort.Controls.Add(this.textBoxRenfortCanons);
            this.groupBoxRenfort.Controls.Add(this.textBoxRenfortFantassins);
            this.groupBoxRenfort.Controls.Add(this.comboBoxPionPropriétaire);
            this.groupBoxRenfort.Controls.Add(this.label19);
            this.groupBoxRenfort.Controls.Add(this.label18);
            this.groupBoxRenfort.Controls.Add(this.label17);
            this.groupBoxRenfort.Controls.Add(this.label16);
            this.groupBoxRenfort.Controls.Add(this.label10);
            this.groupBoxRenfort.Controls.Add(this.label15);
            this.groupBoxRenfort.Controls.Add(this.textBoxPourcentArriveeRenfort);
            this.groupBoxRenfort.Controls.Add(this.comboBoxModeleRenfort);
            this.groupBoxRenfort.Controls.Add(this.label13);
            this.groupBoxRenfort.Controls.Add(this.label14);
            this.groupBoxRenfort.Controls.Add(this.label11);
            this.groupBoxRenfort.Controls.Add(this.label12);
            this.groupBoxRenfort.Location = new System.Drawing.Point(13, 252);
            this.groupBoxRenfort.Name = "groupBoxRenfort";
            this.groupBoxRenfort.Size = new System.Drawing.Size(524, 178);
            this.groupBoxRenfort.TabIndex = 4;
            this.groupBoxRenfort.TabStop = false;
            this.groupBoxRenfort.Text = "Renfort";
            // 
            // textBoxRenfortRavitaillement
            // 
            this.textBoxRenfortRavitaillement.Location = new System.Drawing.Point(376, 113);
            this.textBoxRenfortRavitaillement.MaxLength = 5;
            this.textBoxRenfortRavitaillement.Name = "textBoxRenfortRavitaillement";
            this.textBoxRenfortRavitaillement.Size = new System.Drawing.Size(60, 20);
            this.textBoxRenfortRavitaillement.TabIndex = 7;
            this.textBoxRenfortRavitaillement.Text = "0";
            this.textBoxRenfortRavitaillement.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBoxRenfortMateriel
            // 
            this.textBoxRenfortMateriel.Location = new System.Drawing.Point(375, 84);
            this.textBoxRenfortMateriel.MaxLength = 5;
            this.textBoxRenfortMateriel.Name = "textBoxRenfortMateriel";
            this.textBoxRenfortMateriel.Size = new System.Drawing.Size(60, 20);
            this.textBoxRenfortMateriel.TabIndex = 5;
            this.textBoxRenfortMateriel.Text = "0";
            this.textBoxRenfortMateriel.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBoxRenfortCavaliers
            // 
            this.textBoxRenfortCavaliers.Location = new System.Drawing.Point(375, 55);
            this.textBoxRenfortCavaliers.MaxLength = 5;
            this.textBoxRenfortCavaliers.Name = "textBoxRenfortCavaliers";
            this.textBoxRenfortCavaliers.Size = new System.Drawing.Size(60, 20);
            this.textBoxRenfortCavaliers.TabIndex = 3;
            this.textBoxRenfortCavaliers.Text = "0";
            this.textBoxRenfortCavaliers.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBoxRenfortMoral
            // 
            this.textBoxRenfortMoral.Location = new System.Drawing.Point(118, 110);
            this.textBoxRenfortMoral.MaxLength = 5;
            this.textBoxRenfortMoral.Name = "textBoxRenfortMoral";
            this.textBoxRenfortMoral.Size = new System.Drawing.Size(60, 20);
            this.textBoxRenfortMoral.TabIndex = 6;
            this.textBoxRenfortMoral.Text = "0";
            this.textBoxRenfortMoral.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBoxRenfortCanons
            // 
            this.textBoxRenfortCanons.Location = new System.Drawing.Point(118, 81);
            this.textBoxRenfortCanons.MaxLength = 5;
            this.textBoxRenfortCanons.Name = "textBoxRenfortCanons";
            this.textBoxRenfortCanons.Size = new System.Drawing.Size(60, 20);
            this.textBoxRenfortCanons.TabIndex = 4;
            this.textBoxRenfortCanons.Text = "0";
            this.textBoxRenfortCanons.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBoxRenfortFantassins
            // 
            this.textBoxRenfortFantassins.Location = new System.Drawing.Point(118, 52);
            this.textBoxRenfortFantassins.MaxLength = 5;
            this.textBoxRenfortFantassins.Name = "textBoxRenfortFantassins";
            this.textBoxRenfortFantassins.Size = new System.Drawing.Size(60, 20);
            this.textBoxRenfortFantassins.TabIndex = 2;
            this.textBoxRenfortFantassins.Text = "0";
            this.textBoxRenfortFantassins.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // comboBoxPionPropriétaire
            // 
            this.comboBoxPionPropriétaire.DisplayMember = "nomListe";
            this.comboBoxPionPropriétaire.FormattingEnabled = true;
            this.comboBoxPionPropriétaire.Items.AddRange(new object[] {
            "Central",
            "En Haut",
            "En Haut à droite",
            "A droite",
            "En bas à droite",
            "En bas",
            "En bas à gauche",
            "A gauche",
            "En Haut à gauche"});
            this.comboBoxPionPropriétaire.Location = new System.Drawing.Point(116, 136);
            this.comboBoxPionPropriétaire.Name = "comboBoxPionPropriétaire";
            this.comboBoxPionPropriétaire.Size = new System.Drawing.Size(123, 21);
            this.comboBoxPionPropriétaire.TabIndex = 8;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(271, 82);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(61, 13);
            this.label19.TabIndex = 31;
            this.label19.Text = "Matériel (%)";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(271, 113);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(93, 13);
            this.label18.TabIndex = 30;
            this.label18.Text = "Ravitaillement  (%)";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(13, 142);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(83, 13);
            this.label17.TabIndex = 29;
            this.label17.Text = "Pion propriétaire";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(13, 113);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(33, 13);
            this.label16.TabIndex = 28;
            this.label16.Text = "Moral";
            // 
            // checkBoxPrison
            // 
            this.checkBoxPrison.AutoSize = true;
            this.checkBoxPrison.Location = new System.Drawing.Point(30, 213);
            this.checkBoxPrison.Name = "checkBoxPrison";
            this.checkBoxPrison.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.checkBoxPrison.Size = new System.Drawing.Size(55, 17);
            this.checkBoxPrison.TabIndex = 2;
            this.checkBoxPrison.Text = "Prison";
            this.checkBoxPrison.UseVisualStyleBackColor = true;
            // 
            // checkBoxHopital
            // 
            this.checkBoxHopital.AutoSize = true;
            this.checkBoxHopital.Location = new System.Drawing.Point(283, 213);
            this.checkBoxHopital.Name = "checkBoxHopital";
            this.checkBoxHopital.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.checkBoxHopital.Size = new System.Drawing.Size(59, 17);
            this.checkBoxHopital.TabIndex = 3;
            this.checkBoxHopital.Text = "Hopital";
            this.checkBoxHopital.UseVisualStyleBackColor = true;
            // 
            // buttonSupprimerTout
            // 
            this.buttonSupprimerTout.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonSupprimerTout.Location = new System.Drawing.Point(311, 453);
            this.buttonSupprimerTout.Name = "buttonSupprimerTout";
            this.buttonSupprimerTout.Size = new System.Drawing.Size(224, 23);
            this.buttonSupprimerTout.TabIndex = 20;
            this.buttonSupprimerTout.Text = "Supprimer tous les noms de la carte";
            this.buttonSupprimerTout.UseVisualStyleBackColor = true;
            this.buttonSupprimerTout.Click += new System.EventHandler(this.buttonSupprimerTout_Click);
            // 
            // comboBoxProprietaire
            // 
            this.comboBoxProprietaire.DisplayMember = "nomListe";
            this.comboBoxProprietaire.FormattingEnabled = true;
            this.comboBoxProprietaire.Location = new System.Drawing.Point(387, 66);
            this.comboBoxProprietaire.Name = "comboBoxProprietaire";
            this.comboBoxProprietaire.Size = new System.Drawing.Size(123, 21);
            this.comboBoxProprietaire.TabIndex = 32;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(280, 74);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(60, 13);
            this.label20.TabIndex = 33;
            this.label20.Text = "Propriétaire";
            // 
            // FormNomCarte
            // 
            this.AcceptButton = this.buttonValider;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonAnnuler;
            this.ClientSize = new System.Drawing.Size(547, 486);
            this.Controls.Add(this.label20);
            this.Controls.Add(this.comboBoxProprietaire);
            this.Controls.Add(this.buttonSupprimerTout);
            this.Controls.Add(this.checkBoxHopital);
            this.Controls.Add(this.checkBoxPrison);
            this.Controls.Add(this.groupBoxRenfort);
            this.Controls.Add(this.groupBoxNom);
            this.Controls.Add(this.textBoxVictoire);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.textBoxIDCASE);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.labelY);
            this.Controls.Add(this.labelX);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.buttonSupprimer);
            this.Controls.Add(this.buttonAnnuler);
            this.Controls.Add(this.buttonValider);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormNomCarte";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FormNomCarte";
            this.Load += new System.EventHandler(this.FormNomCarte_Load);
            this.groupBoxNom.ResumeLayout(false);
            this.groupBoxNom.PerformLayout();
            this.groupBoxRenfort.ResumeLayout(false);
            this.groupBoxRenfort.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxPosition;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonAnnuler;
        private System.Windows.Forms.Button buttonValider;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxNom;
        private System.Windows.Forms.ComboBox comboBoxPolice;
        private System.Windows.Forms.Button buttonSupprimer;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label labelX;
        private System.Windows.Forms.Label labelY;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBoxIDCASE;
        private System.Windows.Forms.TextBox textBoxIDNOM;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBoxVictoire;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox textBoxPourcentArriveeRenfort;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.ComboBox comboBoxModeleRenfort;
        private System.Windows.Forms.GroupBox groupBoxNom;
        private System.Windows.Forms.GroupBox groupBoxRenfort;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.ComboBox comboBoxPionPropriétaire;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.CheckBox checkBoxPrison;
        private System.Windows.Forms.CheckBox checkBoxHopital;
        private System.Windows.Forms.TextBox textBoxRenfortRavitaillement;
        private System.Windows.Forms.TextBox textBoxRenfortMateriel;
        private System.Windows.Forms.TextBox textBoxRenfortCavaliers;
        private System.Windows.Forms.TextBox textBoxRenfortMoral;
        private System.Windows.Forms.TextBox textBoxRenfortCanons;
        private System.Windows.Forms.TextBox textBoxRenfortFantassins;
        private System.Windows.Forms.Button buttonSupprimerTout;
        private System.Windows.Forms.ComboBox comboBoxProprietaire;
        private System.Windows.Forms.Label label20;
    }
}