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
            comboBoxPosition = new System.Windows.Forms.ComboBox();
            label1 = new System.Windows.Forms.Label();
            buttonAnnuler = new System.Windows.Forms.Button();
            buttonValider = new System.Windows.Forms.Button();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            textBoxNom = new System.Windows.Forms.TextBox();
            comboBoxPolice = new System.Windows.Forms.ComboBox();
            buttonSupprimer = new System.Windows.Forms.Button();
            label4 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            label6 = new System.Windows.Forms.Label();
            labelX = new System.Windows.Forms.Label();
            labelY = new System.Windows.Forms.Label();
            label7 = new System.Windows.Forms.Label();
            textBoxIDCASE = new System.Windows.Forms.TextBox();
            textBoxIDNOM = new System.Windows.Forms.TextBox();
            label8 = new System.Windows.Forms.Label();
            textBoxVictoire = new System.Windows.Forms.TextBox();
            label9 = new System.Windows.Forms.Label();
            label10 = new System.Windows.Forms.Label();
            textBoxPourcentArriveeRenfort = new System.Windows.Forms.TextBox();
            label11 = new System.Windows.Forms.Label();
            label12 = new System.Windows.Forms.Label();
            label13 = new System.Windows.Forms.Label();
            label14 = new System.Windows.Forms.Label();
            label15 = new System.Windows.Forms.Label();
            comboBoxModeleRenfort = new System.Windows.Forms.ComboBox();
            groupBoxNom = new System.Windows.Forms.GroupBox();
            groupBoxRenfort = new System.Windows.Forms.GroupBox();
            textBoxRenfortRavitaillement = new System.Windows.Forms.TextBox();
            textBoxRenfortMateriel = new System.Windows.Forms.TextBox();
            textBoxRenfortCavaliers = new System.Windows.Forms.TextBox();
            textBoxRenfortMoral = new System.Windows.Forms.TextBox();
            textBoxRenfortCanons = new System.Windows.Forms.TextBox();
            textBoxRenfortFantassins = new System.Windows.Forms.TextBox();
            comboBoxPionPropriétaire = new System.Windows.Forms.ComboBox();
            label19 = new System.Windows.Forms.Label();
            label18 = new System.Windows.Forms.Label();
            label17 = new System.Windows.Forms.Label();
            label16 = new System.Windows.Forms.Label();
            checkBoxPrison = new System.Windows.Forms.CheckBox();
            checkBoxHopital = new System.Windows.Forms.CheckBox();
            buttonSupprimerTout = new System.Windows.Forms.Button();
            comboBoxProprietaire = new System.Windows.Forms.ComboBox();
            label20 = new System.Windows.Forms.Label();
            listBoxBlesses = new System.Windows.Forms.ListBox();
            listBoxPrisonniers = new System.Windows.Forms.ListBox();
            checkBoxCreationDepot = new System.Windows.Forms.CheckBox();
            button_controlePCC = new System.Windows.Forms.Button();
            buttonReconstructionPCC = new System.Windows.Forms.Button();
            groupBoxNom.SuspendLayout();
            groupBoxRenfort.SuspendLayout();
            SuspendLayout();
            // 
            // comboBoxPosition
            // 
            comboBoxPosition.FormattingEnabled = true;
            comboBoxPosition.Items.AddRange(new object[] { "Central", "En Haut", "En Haut à droite", "A droite", "En bas à droite", "En bas", "En bas à gauche", "A gauche", "En Haut à gauche" });
            comboBoxPosition.Location = new System.Drawing.Point(624, 72);
            comboBoxPosition.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            comboBoxPosition.Name = "comboBoxPosition";
            comboBoxPosition.Size = new System.Drawing.Size(247, 23);
            comboBoxPosition.TabIndex = 2;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(500, 72);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(50, 15);
            label1.TabIndex = 5;
            label1.Text = "Position";
            // 
            // buttonAnnuler
            // 
            buttonAnnuler.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            buttonAnnuler.Location = new System.Drawing.Point(206, 644);
            buttonAnnuler.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            buttonAnnuler.Name = "buttonAnnuler";
            buttonAnnuler.Size = new System.Drawing.Size(88, 27);
            buttonAnnuler.TabIndex = 7;
            buttonAnnuler.Text = "Annuler";
            buttonAnnuler.UseVisualStyleBackColor = true;
            // 
            // buttonValider
            // 
            buttonValider.DialogResult = System.Windows.Forms.DialogResult.OK;
            buttonValider.Location = new System.Drawing.Point(18, 644);
            buttonValider.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            buttonValider.Name = "buttonValider";
            buttonValider.Size = new System.Drawing.Size(88, 27);
            buttonValider.TabIndex = 5;
            buttonValider.Text = "Valider";
            buttonValider.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(500, 30);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(34, 15);
            label2.TabIndex = 7;
            label2.Text = "Nom";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(20, 72);
            label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(39, 15);
            label3.TabIndex = 6;
            label3.Text = "Police";
            // 
            // textBoxNom
            // 
            textBoxNom.Location = new System.Drawing.Point(624, 33);
            textBoxNom.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            textBoxNom.MaxLength = 255;
            textBoxNom.Name = "textBoxNom";
            textBoxNom.Size = new System.Drawing.Size(247, 23);
            textBoxNom.TabIndex = 0;
            // 
            // comboBoxPolice
            // 
            comboBoxPolice.FormattingEnabled = true;
            comboBoxPolice.Location = new System.Drawing.Point(140, 72);
            comboBoxPolice.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            comboBoxPolice.Name = "comboBoxPolice";
            comboBoxPolice.Size = new System.Drawing.Size(333, 23);
            comboBoxPolice.TabIndex = 1;
            // 
            // buttonSupprimer
            // 
            buttonSupprimer.DialogResult = System.Windows.Forms.DialogResult.OK;
            buttonSupprimer.Location = new System.Drawing.Point(112, 644);
            buttonSupprimer.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            buttonSupprimer.Name = "buttonSupprimer";
            buttonSupprimer.Size = new System.Drawing.Size(88, 27);
            buttonSupprimer.TabIndex = 6;
            buttonSupprimer.Text = "Supprimer";
            buttonSupprimer.UseVisualStyleBackColor = true;
            buttonSupprimer.Click += buttonSupprimer_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(20, 10);
            label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(78, 15);
            label4.TabIndex = 9;
            label4.Text = "Coordonnées";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(236, 10);
            label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(22, 15);
            label5.TabIndex = 10;
            label5.Text = "Y=";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(149, 10);
            label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(22, 15);
            label6.TabIndex = 11;
            label6.Text = "X=";
            // 
            // labelX
            // 
            labelX.AutoSize = true;
            labelX.Location = new System.Drawing.Point(180, 10);
            labelX.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            labelX.Name = "labelX";
            labelX.Size = new System.Drawing.Size(37, 15);
            labelX.TabIndex = 12;
            labelX.Text = "00000";
            // 
            // labelY
            // 
            labelY.AutoSize = true;
            labelY.Location = new System.Drawing.Point(266, 10);
            labelY.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            labelY.Name = "labelY";
            labelY.Size = new System.Drawing.Size(37, 15);
            labelY.TabIndex = 13;
            labelY.Text = "00000";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(22, 40);
            label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(57, 15);
            label7.TabIndex = 14;
            label7.Text = "ID_CASE :";
            // 
            // textBoxIDCASE
            // 
            textBoxIDCASE.Location = new System.Drawing.Point(153, 40);
            textBoxIDCASE.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            textBoxIDCASE.MaxLength = 255;
            textBoxIDCASE.Name = "textBoxIDCASE";
            textBoxIDCASE.ReadOnly = true;
            textBoxIDCASE.Size = new System.Drawing.Size(140, 23);
            textBoxIDCASE.TabIndex = 15;
            // 
            // textBoxIDNOM
            // 
            textBoxIDNOM.Location = new System.Drawing.Point(140, 30);
            textBoxIDNOM.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            textBoxIDNOM.MaxLength = 255;
            textBoxIDNOM.Name = "textBoxIDNOM";
            textBoxIDNOM.ReadOnly = true;
            textBoxIDNOM.Size = new System.Drawing.Size(140, 23);
            textBoxIDNOM.TabIndex = 17;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new System.Drawing.Point(18, 33);
            label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(58, 15);
            label8.TabIndex = 16;
            label8.Text = "ID_NOM :";
            // 
            // textBoxVictoire
            // 
            textBoxVictoire.Location = new System.Drawing.Point(527, 45);
            textBoxVictoire.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            textBoxVictoire.MaxLength = 5;
            textBoxVictoire.Name = "textBoxVictoire";
            textBoxVictoire.Size = new System.Drawing.Size(69, 23);
            textBoxVictoire.TabIndex = 5;
            textBoxVictoire.Text = "0";
            textBoxVictoire.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new System.Drawing.Point(400, 45);
            label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(81, 15);
            label9.TabIndex = 19;
            label9.Text = "Pts de victoire";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new System.Drawing.Point(15, 30);
            label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label10.Name = "label10";
            label10.Size = new System.Drawing.Size(106, 15);
            label10.TabIndex = 20;
            label10.Text = "% d'arrivée / heure";
            // 
            // textBoxPourcentArriveeRenfort
            // 
            textBoxPourcentArriveeRenfort.Location = new System.Drawing.Point(138, 22);
            textBoxPourcentArriveeRenfort.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            textBoxPourcentArriveeRenfort.MaxLength = 5;
            textBoxPourcentArriveeRenfort.Name = "textBoxPourcentArriveeRenfort";
            textBoxPourcentArriveeRenfort.Size = new System.Drawing.Size(69, 23);
            textBoxPourcentArriveeRenfort.TabIndex = 0;
            textBoxPourcentArriveeRenfort.Text = "0";
            textBoxPourcentArriveeRenfort.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new System.Drawing.Point(491, 31);
            label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label11.Name = "label11";
            label11.Size = new System.Drawing.Size(74, 15);
            label11.TabIndex = 22;
            label11.Text = "Modèle pion";
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new System.Drawing.Point(15, 63);
            label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label12.Name = "label12";
            label12.Size = new System.Drawing.Size(78, 15);
            label12.TabIndex = 23;
            label12.Text = "Nb fantassins";
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new System.Drawing.Point(15, 97);
            label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label13.Name = "label13";
            label13.Size = new System.Drawing.Size(64, 15);
            label13.TabIndex = 24;
            label13.Text = "Nb canons";
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Location = new System.Drawing.Point(490, 65);
            label14.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label14.Name = "label14";
            label14.Size = new System.Drawing.Size(71, 15);
            label14.TabIndex = 25;
            label14.Text = "Nb cavaliers";
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Location = new System.Drawing.Point(491, 96);
            label15.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label15.Name = "label15";
            label15.Size = new System.Drawing.Size(74, 15);
            label15.TabIndex = 26;
            label15.Text = "Variation (%)";
            // 
            // comboBoxModeleRenfort
            // 
            comboBoxModeleRenfort.DisplayMember = "nomListe";
            comboBoxModeleRenfort.FormattingEnabled = true;
            comboBoxModeleRenfort.Items.AddRange(new object[] { "Central", "En Haut", "En Haut à droite", "A droite", "En bas à droite", "En bas", "En bas à gauche", "A gauche", "En Haut à gauche" });
            comboBoxModeleRenfort.Location = new System.Drawing.Point(612, 22);
            comboBoxModeleRenfort.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            comboBoxModeleRenfort.Name = "comboBoxModeleRenfort";
            comboBoxModeleRenfort.Size = new System.Drawing.Size(249, 23);
            comboBoxModeleRenfort.TabIndex = 1;
            // 
            // groupBoxNom
            // 
            groupBoxNom.Controls.Add(textBoxIDNOM);
            groupBoxNom.Controls.Add(label8);
            groupBoxNom.Controls.Add(comboBoxPolice);
            groupBoxNom.Controls.Add(textBoxNom);
            groupBoxNom.Controls.Add(label3);
            groupBoxNom.Controls.Add(label2);
            groupBoxNom.Controls.Add(comboBoxPosition);
            groupBoxNom.Controls.Add(label1);
            groupBoxNom.Location = new System.Drawing.Point(13, 123);
            groupBoxNom.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBoxNom.Name = "groupBoxNom";
            groupBoxNom.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBoxNom.Size = new System.Drawing.Size(878, 115);
            groupBoxNom.TabIndex = 1;
            groupBoxNom.TabStop = false;
            groupBoxNom.Text = "Nom";
            // 
            // groupBoxRenfort
            // 
            groupBoxRenfort.Controls.Add(textBoxRenfortRavitaillement);
            groupBoxRenfort.Controls.Add(textBoxRenfortMateriel);
            groupBoxRenfort.Controls.Add(textBoxRenfortCavaliers);
            groupBoxRenfort.Controls.Add(textBoxRenfortMoral);
            groupBoxRenfort.Controls.Add(textBoxRenfortCanons);
            groupBoxRenfort.Controls.Add(textBoxRenfortFantassins);
            groupBoxRenfort.Controls.Add(comboBoxPionPropriétaire);
            groupBoxRenfort.Controls.Add(label19);
            groupBoxRenfort.Controls.Add(label18);
            groupBoxRenfort.Controls.Add(label17);
            groupBoxRenfort.Controls.Add(label16);
            groupBoxRenfort.Controls.Add(label10);
            groupBoxRenfort.Controls.Add(label15);
            groupBoxRenfort.Controls.Add(textBoxPourcentArriveeRenfort);
            groupBoxRenfort.Controls.Add(comboBoxModeleRenfort);
            groupBoxRenfort.Controls.Add(label13);
            groupBoxRenfort.Controls.Add(label14);
            groupBoxRenfort.Controls.Add(label11);
            groupBoxRenfort.Controls.Add(label12);
            groupBoxRenfort.Location = new System.Drawing.Point(18, 412);
            groupBoxRenfort.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBoxRenfort.Name = "groupBoxRenfort";
            groupBoxRenfort.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBoxRenfort.Size = new System.Drawing.Size(874, 205);
            groupBoxRenfort.TabIndex = 4;
            groupBoxRenfort.TabStop = false;
            groupBoxRenfort.Text = "Renfort";
            // 
            // textBoxRenfortRavitaillement
            // 
            textBoxRenfortRavitaillement.Location = new System.Drawing.Point(614, 132);
            textBoxRenfortRavitaillement.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            textBoxRenfortRavitaillement.MaxLength = 5;
            textBoxRenfortRavitaillement.Name = "textBoxRenfortRavitaillement";
            textBoxRenfortRavitaillement.Size = new System.Drawing.Size(69, 23);
            textBoxRenfortRavitaillement.TabIndex = 7;
            textBoxRenfortRavitaillement.Text = "0";
            textBoxRenfortRavitaillement.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBoxRenfortMateriel
            // 
            textBoxRenfortMateriel.Location = new System.Drawing.Point(612, 98);
            textBoxRenfortMateriel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            textBoxRenfortMateriel.MaxLength = 5;
            textBoxRenfortMateriel.Name = "textBoxRenfortMateriel";
            textBoxRenfortMateriel.Size = new System.Drawing.Size(69, 23);
            textBoxRenfortMateriel.TabIndex = 5;
            textBoxRenfortMateriel.Text = "0";
            textBoxRenfortMateriel.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBoxRenfortCavaliers
            // 
            textBoxRenfortCavaliers.Location = new System.Drawing.Point(612, 65);
            textBoxRenfortCavaliers.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            textBoxRenfortCavaliers.MaxLength = 5;
            textBoxRenfortCavaliers.Name = "textBoxRenfortCavaliers";
            textBoxRenfortCavaliers.Size = new System.Drawing.Size(69, 23);
            textBoxRenfortCavaliers.TabIndex = 3;
            textBoxRenfortCavaliers.Text = "0";
            textBoxRenfortCavaliers.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBoxRenfortMoral
            // 
            textBoxRenfortMoral.Location = new System.Drawing.Point(138, 127);
            textBoxRenfortMoral.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            textBoxRenfortMoral.MaxLength = 5;
            textBoxRenfortMoral.Name = "textBoxRenfortMoral";
            textBoxRenfortMoral.Size = new System.Drawing.Size(69, 23);
            textBoxRenfortMoral.TabIndex = 6;
            textBoxRenfortMoral.Text = "0";
            textBoxRenfortMoral.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBoxRenfortCanons
            // 
            textBoxRenfortCanons.Location = new System.Drawing.Point(138, 93);
            textBoxRenfortCanons.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            textBoxRenfortCanons.MaxLength = 5;
            textBoxRenfortCanons.Name = "textBoxRenfortCanons";
            textBoxRenfortCanons.Size = new System.Drawing.Size(69, 23);
            textBoxRenfortCanons.TabIndex = 4;
            textBoxRenfortCanons.Text = "0";
            textBoxRenfortCanons.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBoxRenfortFantassins
            // 
            textBoxRenfortFantassins.Location = new System.Drawing.Point(138, 60);
            textBoxRenfortFantassins.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            textBoxRenfortFantassins.MaxLength = 5;
            textBoxRenfortFantassins.Name = "textBoxRenfortFantassins";
            textBoxRenfortFantassins.Size = new System.Drawing.Size(69, 23);
            textBoxRenfortFantassins.TabIndex = 2;
            textBoxRenfortFantassins.Text = "0";
            textBoxRenfortFantassins.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // comboBoxPionPropriétaire
            // 
            comboBoxPionPropriétaire.DisplayMember = "nomListe";
            comboBoxPionPropriétaire.FormattingEnabled = true;
            comboBoxPionPropriétaire.Items.AddRange(new object[] { "Central", "En Haut", "En Haut à droite", "A droite", "En bas à droite", "En bas", "En bas à gauche", "A gauche", "En Haut à gauche" });
            comboBoxPionPropriétaire.Location = new System.Drawing.Point(135, 157);
            comboBoxPionPropriétaire.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            comboBoxPionPropriétaire.Name = "comboBoxPionPropriétaire";
            comboBoxPionPropriétaire.Size = new System.Drawing.Size(333, 23);
            comboBoxPionPropriétaire.TabIndex = 8;
            // 
            // label19
            // 
            label19.AutoSize = true;
            label19.Location = new System.Drawing.Point(491, 96);
            label19.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label19.Name = "label19";
            label19.Size = new System.Drawing.Size(71, 15);
            label19.TabIndex = 31;
            label19.Text = "Matériel (%)";
            // 
            // label18
            // 
            label18.AutoSize = true;
            label18.Location = new System.Drawing.Point(491, 132);
            label18.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label18.Name = "label18";
            label18.Size = new System.Drawing.Size(106, 15);
            label18.TabIndex = 30;
            label18.Text = "Ravitaillement  (%)";
            // 
            // label17
            // 
            label17.AutoSize = true;
            label17.Location = new System.Drawing.Point(15, 164);
            label17.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label17.Name = "label17";
            label17.Size = new System.Drawing.Size(95, 15);
            label17.TabIndex = 29;
            label17.Text = "Pion propriétaire";
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.Location = new System.Drawing.Point(15, 130);
            label16.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label16.Name = "label16";
            label16.Size = new System.Drawing.Size(38, 15);
            label16.TabIndex = 28;
            label16.Text = "Moral";
            // 
            // checkBoxPrison
            // 
            checkBoxPrison.AutoSize = true;
            checkBoxPrison.Location = new System.Drawing.Point(35, 292);
            checkBoxPrison.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            checkBoxPrison.Name = "checkBoxPrison";
            checkBoxPrison.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            checkBoxPrison.Size = new System.Drawing.Size(59, 19);
            checkBoxPrison.TabIndex = 2;
            checkBoxPrison.Text = "Prison";
            checkBoxPrison.UseVisualStyleBackColor = true;
            // 
            // checkBoxHopital
            // 
            checkBoxHopital.AutoSize = true;
            checkBoxHopital.Location = new System.Drawing.Point(511, 295);
            checkBoxHopital.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            checkBoxHopital.Name = "checkBoxHopital";
            checkBoxHopital.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            checkBoxHopital.Size = new System.Drawing.Size(65, 19);
            checkBoxHopital.TabIndex = 3;
            checkBoxHopital.Text = "Hopital";
            checkBoxHopital.UseVisualStyleBackColor = true;
            // 
            // buttonSupprimerTout
            // 
            buttonSupprimerTout.DialogResult = System.Windows.Forms.DialogResult.OK;
            buttonSupprimerTout.Location = new System.Drawing.Point(630, 644);
            buttonSupprimerTout.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            buttonSupprimerTout.Name = "buttonSupprimerTout";
            buttonSupprimerTout.Size = new System.Drawing.Size(261, 27);
            buttonSupprimerTout.TabIndex = 20;
            buttonSupprimerTout.Text = "Supprimer tous les noms de la carte";
            buttonSupprimerTout.UseVisualStyleBackColor = true;
            buttonSupprimerTout.Click += buttonSupprimerTout_Click;
            // 
            // comboBoxProprietaire
            // 
            comboBoxProprietaire.DisplayMember = "nomListe";
            comboBoxProprietaire.FormattingEnabled = true;
            comboBoxProprietaire.Location = new System.Drawing.Point(525, 81);
            comboBoxProprietaire.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            comboBoxProprietaire.Name = "comboBoxProprietaire";
            comboBoxProprietaire.Size = new System.Drawing.Size(247, 23);
            comboBoxProprietaire.TabIndex = 32;
            // 
            // label20
            // 
            label20.AutoSize = true;
            label20.Location = new System.Drawing.Point(400, 90);
            label20.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label20.Name = "label20";
            label20.Size = new System.Drawing.Size(68, 15);
            label20.TabIndex = 33;
            label20.Text = "Propriétaire";
            // 
            // listBoxBlesses
            // 
            listBoxBlesses.DisplayMember = "nomListeEffectifs";
            listBoxBlesses.FormattingEnabled = true;
            listBoxBlesses.HorizontalScrollbar = true;
            listBoxBlesses.ItemHeight = 15;
            listBoxBlesses.Location = new System.Drawing.Point(588, 292);
            listBoxBlesses.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            listBoxBlesses.Name = "listBoxBlesses";
            listBoxBlesses.Size = new System.Drawing.Size(303, 109);
            listBoxBlesses.TabIndex = 34;
            // 
            // listBoxPrisonniers
            // 
            listBoxPrisonniers.DisplayMember = "nomListeEffectifs";
            listBoxPrisonniers.FormattingEnabled = true;
            listBoxPrisonniers.HorizontalScrollbar = true;
            listBoxPrisonniers.ItemHeight = 15;
            listBoxPrisonniers.Location = new System.Drawing.Point(153, 295);
            listBoxPrisonniers.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            listBoxPrisonniers.Name = "listBoxPrisonniers";
            listBoxPrisonniers.Size = new System.Drawing.Size(333, 109);
            listBoxPrisonniers.TabIndex = 35;
            // 
            // checkBoxCreationDepot
            // 
            checkBoxCreationDepot.AutoSize = true;
            checkBoxCreationDepot.Location = new System.Drawing.Point(34, 265);
            checkBoxCreationDepot.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            checkBoxCreationDepot.Name = "checkBoxCreationDepot";
            checkBoxCreationDepot.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            checkBoxCreationDepot.Size = new System.Drawing.Size(177, 19);
            checkBoxCreationDepot.TabIndex = 36;
            checkBoxCreationDepot.Text = "Création de dépôts autorisée";
            checkBoxCreationDepot.UseVisualStyleBackColor = true;
            // 
            // button_controlePCC
            // 
            button_controlePCC.Location = new System.Drawing.Point(513, 642);
            button_controlePCC.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            button_controlePCC.Name = "button_controlePCC";
            button_controlePCC.Size = new System.Drawing.Size(88, 27);
            button_controlePCC.TabIndex = 37;
            button_controlePCC.Text = "Contrôle PCC";
            button_controlePCC.UseVisualStyleBackColor = true;
            button_controlePCC.Click += button_controlePCC_Click;
            // 
            // buttonReconstructionPCC
            // 
            buttonReconstructionPCC.Location = new System.Drawing.Point(382, 644);
            buttonReconstructionPCC.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            buttonReconstructionPCC.Name = "buttonReconstructionPCC";
            buttonReconstructionPCC.Size = new System.Drawing.Size(123, 27);
            buttonReconstructionPCC.TabIndex = 38;
            buttonReconstructionPCC.Text = "Reconstruction PCC";
            buttonReconstructionPCC.UseVisualStyleBackColor = true;
            buttonReconstructionPCC.Click += buttonReconstructionPCC_Click;
            // 
            // FormNomCarte
            // 
            AcceptButton = buttonValider;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = buttonAnnuler;
            ClientSize = new System.Drawing.Size(905, 681);
            Controls.Add(buttonReconstructionPCC);
            Controls.Add(button_controlePCC);
            Controls.Add(checkBoxCreationDepot);
            Controls.Add(listBoxPrisonniers);
            Controls.Add(listBoxBlesses);
            Controls.Add(label20);
            Controls.Add(comboBoxProprietaire);
            Controls.Add(buttonSupprimerTout);
            Controls.Add(checkBoxHopital);
            Controls.Add(checkBoxPrison);
            Controls.Add(groupBoxRenfort);
            Controls.Add(groupBoxNom);
            Controls.Add(textBoxVictoire);
            Controls.Add(label9);
            Controls.Add(textBoxIDCASE);
            Controls.Add(label7);
            Controls.Add(labelY);
            Controls.Add(labelX);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(buttonSupprimer);
            Controls.Add(buttonAnnuler);
            Controls.Add(buttonValider);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormNomCarte";
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "FormNomCarte";
            Load += FormNomCarte_Load;
            groupBoxNom.ResumeLayout(false);
            groupBoxNom.PerformLayout();
            groupBoxRenfort.ResumeLayout(false);
            groupBoxRenfort.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
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
        private System.Windows.Forms.ListBox listBoxBlesses;
        private System.Windows.Forms.ListBox listBoxPrisonniers;
        private System.Windows.Forms.CheckBox checkBoxCreationDepot;
        private System.Windows.Forms.Button button_controlePCC;
        private System.Windows.Forms.Button buttonReconstructionPCC;
    }
}