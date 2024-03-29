using WaocLib;
namespace vaoc
{
    partial class FormPrincipale
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPrincipale));
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.buttonData = new System.Windows.Forms.Button();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.buttonVerifierTrajet = new System.Windows.Forms.Button();
            this.buttonRecalculTrajet = new System.Windows.Forms.Button();
            this.comboBoxListeUnites = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxCaseID = new System.Windows.Forms.TextBox();
            this.buttonCaseID = new System.Windows.Forms.Button();
            this.panelInformation = new System.Windows.Forms.Panel();
            this.labelTour = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.labelProprietaire = new System.Windows.Forms.Label();
            this.labelCoutTerrain = new System.Windows.Forms.Label();
            this.labelPhase = new System.Windows.Forms.Label();
            this.labelInformationTempsRestant = new System.Windows.Forms.Label();
            this.labelInformationTempsPasse = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.labelInformationTerrain = new System.Windows.Forms.Label();
            this.labelInformationIDCASE = new System.Windows.Forms.Label();
            this.labelInformationY = new System.Windows.Forms.Label();
            this.labelInformationX = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.panelTestPlusCourtChemin = new System.Windows.Forms.Panel();
            this.labelArriveeTerrain = new System.Windows.Forms.Label();
            this.labelArriveeIDCASE = new System.Windows.Forms.Label();
            this.labelArriveeY = new System.Windows.Forms.Label();
            this.labelArriveeX = new System.Windows.Forms.Label();
            this.labelDepartTerrain = new System.Windows.Forms.Label();
            this.labelDepartIDCASE = new System.Windows.Forms.Label();
            this.labelDepartY = new System.Windows.Forms.Label();
            this.labelDepartX = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panelImage = new System.Windows.Forms.Panel();
            this.ImageCarte = new WaocLib.VaocPictureBox();
            this.menuPrincipal = new System.Windows.Forms.MenuStrip();
            this.fichierToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nouveauToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ouvrirToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.fichiersRecentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.quitterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ParametrageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.meteoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modelesDeMOuvementsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nationsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.policeDeCaract�resToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fondDeCarteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.carteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hPAStarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pontsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.utilisateursToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rolesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nomsDesLeadesPromusToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aptitudesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modelesDePIonsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aptitudesModelesDePIonsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renfortsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ordresToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.phrasesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bataillesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bataillesVideoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mEssagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nomsPionsUniquesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.pionsAnciensToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ordresAnciensToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.messagesAnciensToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.actionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.heureSuivanteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.creationInternetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mise�JourInternetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.notificationAuxJoueursToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copieDeSauvegardeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.initilisationPartieToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.forcesInitialesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.donneesVid�oToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.genererLeFilmToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.genererLesFilmsDeBatailleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.outilsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mise�JourProprietairesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statistiquesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extractionDeLaBaseEnCSVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.repriseDeDonn�esToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sQLDeTousLesMessagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.actuelsAnciensToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.anciensActuelsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.broderiequadrillageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AProposToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripCarte = new System.Windows.Forms.ToolStrip();
            this.toolStripAfficherVilles = new System.Windows.Forms.ToolStripButton();
            this.toolStripAfficherUnites = new System.Windows.Forms.ToolStripButton();
            this.toolStripAfficherBatailles = new System.Windows.Forms.ToolStripButton();
            this.toolStripAffichierTopographie = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripZoomPlus = new System.Windows.Forms.ToolStripButton();
            this.toolStripZoomMoins = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripPlusCourtChemin = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonPontEndommage = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonReparerPont = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonPontDetruit = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonConstruirePonton = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonTrajets = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonTrajetsVilles = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonMemoire = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonDepots = new System.Windows.Forms.ToolStripButton();
            this.backgroundTraitement = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.panelInformation.SuspendLayout();
            this.panelTestPlusCourtChemin.SuspendLayout();
            this.panelImage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ImageCarte)).BeginInit();
            this.menuPrincipal.SuspendLayout();
            this.toolStripCarte.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog
            // 
            this.openFileDialog.DefaultExt = "vaoc";
            this.openFileDialog.Filter = "fichiers VAOC |*.vaoc";
            this.openFileDialog.Title = "Ouvrir un fichier VAOC";
            // 
            // buttonData
            // 
            this.buttonData.AutoSize = true;
            this.buttonData.Location = new System.Drawing.Point(14, 12);
            this.buttonData.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.buttonData.Name = "buttonData";
            this.buttonData.Size = new System.Drawing.Size(97, 29);
            this.buttonData.TabIndex = 3;
            this.buttonData.Text = "Test";
            this.buttonData.UseVisualStyleBackColor = true;
            this.buttonData.Click += new System.EventHandler(this.buttonData_Click);
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "vaoc";
            this.saveFileDialog.Filter = "fichiers VAOC |*.vaoc";
            this.saveFileDialog.Title = "Sauvegarder un fichieer VAOC";
            // 
            // splitContainer
            // 
            this.splitContainer.BackColor = System.Drawing.Color.DeepSkyBlue;
            this.splitContainer.ForeColor = System.Drawing.SystemColors.ControlText;
            this.splitContainer.Location = new System.Drawing.Point(0, 60);
            this.splitContainer.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.AutoScroll = true;
            this.splitContainer.Panel1.BackColor = System.Drawing.SystemColors.Control;
            this.splitContainer.Panel1.Controls.Add(this.buttonVerifierTrajet);
            this.splitContainer.Panel1.Controls.Add(this.buttonRecalculTrajet);
            this.splitContainer.Panel1.Controls.Add(this.comboBoxListeUnites);
            this.splitContainer.Panel1.Controls.Add(this.label7);
            this.splitContainer.Panel1.Controls.Add(this.textBoxCaseID);
            this.splitContainer.Panel1.Controls.Add(this.buttonCaseID);
            this.splitContainer.Panel1.Controls.Add(this.panelInformation);
            this.splitContainer.Panel1.Controls.Add(this.panelTestPlusCourtChemin);
            this.splitContainer.Panel1.Controls.Add(this.buttonData);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.BackColor = System.Drawing.SystemColors.Control;
            this.splitContainer.Panel2.Controls.Add(this.panelImage);
            this.splitContainer.Panel2.Padding = new System.Windows.Forms.Padding(12, 12, 12, 58);
            this.splitContainer.Size = new System.Drawing.Size(958, 670);
            this.splitContainer.SplitterDistance = 556;
            this.splitContainer.SplitterWidth = 6;
            this.splitContainer.TabIndex = 4;
            // 
            // buttonVerifierTrajet
            // 
            this.buttonVerifierTrajet.AutoSize = true;
            this.buttonVerifierTrajet.Location = new System.Drawing.Point(15, 84);
            this.buttonVerifierTrajet.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.buttonVerifierTrajet.Name = "buttonVerifierTrajet";
            this.buttonVerifierTrajet.Size = new System.Drawing.Size(104, 29);
            this.buttonVerifierTrajet.TabIndex = 16;
            this.buttonVerifierTrajet.Text = "V�rifier Trajet";
            this.buttonVerifierTrajet.UseVisualStyleBackColor = true;
            this.buttonVerifierTrajet.Click += new System.EventHandler(this.buttonVerifierTrajet_Click);
            // 
            // buttonRecalculTrajet
            // 
            this.buttonRecalculTrajet.AutoSize = true;
            this.buttonRecalculTrajet.Location = new System.Drawing.Point(126, 84);
            this.buttonRecalculTrajet.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.buttonRecalculTrajet.Name = "buttonRecalculTrajet";
            this.buttonRecalculTrajet.Size = new System.Drawing.Size(107, 29);
            this.buttonRecalculTrajet.TabIndex = 15;
            this.buttonRecalculTrajet.Text = "Recalcul Trajet";
            this.buttonRecalculTrajet.UseVisualStyleBackColor = true;
            this.buttonRecalculTrajet.Click += new System.EventHandler(this.buttonRecalculTrajet_Click);
            // 
            // comboBoxListeUnites
            // 
            this.comboBoxListeUnites.DisplayMember = "nomListe";
            this.comboBoxListeUnites.FormattingEnabled = true;
            this.comboBoxListeUnites.Location = new System.Drawing.Point(15, 44);
            this.comboBoxListeUnites.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.comboBoxListeUnites.MaxDropDownItems = 30;
            this.comboBoxListeUnites.Name = "comboBoxListeUnites";
            this.comboBoxListeUnites.Size = new System.Drawing.Size(312, 23);
            this.comboBoxListeUnites.TabIndex = 14;
            this.comboBoxListeUnites.SelectedIndexChanged += new System.EventHandler(this.comboBoxListeUnites_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(215, 17);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(29, 15);
            this.label7.TabIndex = 13;
            this.label7.Text = "ID =";
            // 
            // textBoxCaseID
            // 
            this.textBoxCaseID.Location = new System.Drawing.Point(253, 14);
            this.textBoxCaseID.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBoxCaseID.Name = "textBoxCaseID";
            this.textBoxCaseID.Size = new System.Drawing.Size(74, 23);
            this.textBoxCaseID.TabIndex = 12;
            // 
            // buttonCaseID
            // 
            this.buttonCaseID.Location = new System.Drawing.Point(122, 12);
            this.buttonCaseID.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.buttonCaseID.Name = "buttonCaseID";
            this.buttonCaseID.Size = new System.Drawing.Size(88, 27);
            this.buttonCaseID.TabIndex = 11;
            this.buttonCaseID.Text = "ID -> X,Y";
            this.buttonCaseID.UseVisualStyleBackColor = true;
            this.buttonCaseID.Click += new System.EventHandler(this.buttonCaseID_Click);
            // 
            // panelInformation
            // 
            this.panelInformation.Controls.Add(this.labelTour);
            this.panelInformation.Controls.Add(this.label9);
            this.panelInformation.Controls.Add(this.labelProprietaire);
            this.panelInformation.Controls.Add(this.labelCoutTerrain);
            this.panelInformation.Controls.Add(this.labelPhase);
            this.panelInformation.Controls.Add(this.labelInformationTempsRestant);
            this.panelInformation.Controls.Add(this.labelInformationTempsPasse);
            this.panelInformation.Controls.Add(this.label6);
            this.panelInformation.Controls.Add(this.label5);
            this.panelInformation.Controls.Add(this.label4);
            this.panelInformation.Controls.Add(this.label3);
            this.panelInformation.Controls.Add(this.labelInformationTerrain);
            this.panelInformation.Controls.Add(this.labelInformationIDCASE);
            this.panelInformation.Controls.Add(this.labelInformationY);
            this.panelInformation.Controls.Add(this.labelInformationX);
            this.panelInformation.Controls.Add(this.label12);
            this.panelInformation.Location = new System.Drawing.Point(14, 118);
            this.panelInformation.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.panelInformation.Name = "panelInformation";
            this.panelInformation.Size = new System.Drawing.Size(526, 321);
            this.panelInformation.TabIndex = 10;
            // 
            // labelTour
            // 
            this.labelTour.AutoSize = true;
            this.labelTour.Location = new System.Drawing.Point(107, 167);
            this.labelTour.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelTour.Name = "labelTour";
            this.labelTour.Size = new System.Drawing.Size(25, 15);
            this.labelTour.TabIndex = 27;
            this.labelTour.Text = "000";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(7, 167);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(36, 15);
            this.label9.TabIndex = 26;
            this.label9.Text = "Tour :";
            // 
            // labelProprietaire
            // 
            this.labelProprietaire.Location = new System.Drawing.Point(97, 123);
            this.labelProprietaire.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelProprietaire.Name = "labelProprietaire";
            this.labelProprietaire.Size = new System.Drawing.Size(426, 23);
            this.labelProprietaire.TabIndex = 25;
            this.labelProprietaire.Text = "Propri�taire : ?";
            // 
            // labelCoutTerrain
            // 
            this.labelCoutTerrain.Location = new System.Drawing.Point(97, 95);
            this.labelCoutTerrain.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelCoutTerrain.Name = "labelCoutTerrain";
            this.labelCoutTerrain.Size = new System.Drawing.Size(338, 29);
            this.labelCoutTerrain.TabIndex = 10;
            this.labelCoutTerrain.Text = "Cout:xxxxxxxxx";
            // 
            // labelPhase
            // 
            this.labelPhase.AutoSize = true;
            this.labelPhase.Location = new System.Drawing.Point(107, 190);
            this.labelPhase.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelPhase.Name = "labelPhase";
            this.labelPhase.Size = new System.Drawing.Size(25, 15);
            this.labelPhase.TabIndex = 24;
            this.labelPhase.Text = "000";
            // 
            // labelInformationTempsRestant
            // 
            this.labelInformationTempsRestant.AutoSize = true;
            this.labelInformationTempsRestant.Location = new System.Drawing.Point(107, 299);
            this.labelInformationTempsRestant.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelInformationTempsRestant.Name = "labelInformationTempsRestant";
            this.labelInformationTempsRestant.Size = new System.Drawing.Size(65, 15);
            this.labelInformationTempsRestant.TabIndex = 23;
            this.labelInformationTempsRestant.Text = "0 secondes";
            // 
            // labelInformationTempsPasse
            // 
            this.labelInformationTempsPasse.AutoSize = true;
            this.labelInformationTempsPasse.Location = new System.Drawing.Point(107, 215);
            this.labelInformationTempsPasse.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelInformationTempsPasse.Name = "labelInformationTempsPasse";
            this.labelInformationTempsPasse.Size = new System.Drawing.Size(65, 15);
            this.labelInformationTempsPasse.TabIndex = 22;
            this.labelInformationTempsPasse.Text = "0 secondes";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 298);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(86, 15);
            this.label6.TabIndex = 21;
            this.label6.Text = "Temps restant :";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 216);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(79, 15);
            this.label5.TabIndex = 20;
            this.label5.Text = "Temps pass� :";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 190);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 15);
            this.label4.TabIndex = 7;
            this.label4.Text = "Phase :";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 145);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 15);
            this.label3.TabIndex = 6;
            this.label3.Text = "Traitement :";
            // 
            // labelInformationTerrain
            // 
            this.labelInformationTerrain.AutoSize = true;
            this.labelInformationTerrain.Location = new System.Drawing.Point(93, 66);
            this.labelInformationTerrain.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelInformationTerrain.Name = "labelInformationTerrain";
            this.labelInformationTerrain.Size = new System.Drawing.Size(99, 15);
            this.labelInformationTerrain.TabIndex = 5;
            this.labelInformationTerrain.Text = "Terrain:xxxxxxxxx";
            // 
            // labelInformationIDCASE
            // 
            this.labelInformationIDCASE.AutoSize = true;
            this.labelInformationIDCASE.Location = new System.Drawing.Point(93, 35);
            this.labelInformationIDCASE.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelInformationIDCASE.Name = "labelInformationIDCASE";
            this.labelInformationIDCASE.Size = new System.Drawing.Size(84, 15);
            this.labelInformationIDCASE.TabIndex = 4;
            this.labelInformationIDCASE.Text = "ID_CASE:00000";
            // 
            // labelInformationY
            // 
            this.labelInformationY.AutoSize = true;
            this.labelInformationY.Location = new System.Drawing.Point(141, 9);
            this.labelInformationY.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelInformationY.Name = "labelInformationY";
            this.labelInformationY.Size = new System.Drawing.Size(35, 15);
            this.labelInformationY.TabIndex = 3;
            this.labelInformationY.Text = "Y:000";
            // 
            // labelInformationX
            // 
            this.labelInformationX.AutoSize = true;
            this.labelInformationX.Location = new System.Drawing.Point(97, 9);
            this.labelInformationX.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelInformationX.Name = "labelInformationX";
            this.labelInformationX.Size = new System.Drawing.Size(35, 15);
            this.labelInformationX.TabIndex = 2;
            this.labelInformationX.Text = "X:000";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(4, 2);
            this.label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(38, 15);
            this.label12.TabIndex = 0;
            this.label12.Text = "Case :";
            // 
            // panelTestPlusCourtChemin
            // 
            this.panelTestPlusCourtChemin.Controls.Add(this.labelArriveeTerrain);
            this.panelTestPlusCourtChemin.Controls.Add(this.labelArriveeIDCASE);
            this.panelTestPlusCourtChemin.Controls.Add(this.labelArriveeY);
            this.panelTestPlusCourtChemin.Controls.Add(this.labelArriveeX);
            this.panelTestPlusCourtChemin.Controls.Add(this.labelDepartTerrain);
            this.panelTestPlusCourtChemin.Controls.Add(this.labelDepartIDCASE);
            this.panelTestPlusCourtChemin.Controls.Add(this.labelDepartY);
            this.panelTestPlusCourtChemin.Controls.Add(this.labelDepartX);
            this.panelTestPlusCourtChemin.Controls.Add(this.label2);
            this.panelTestPlusCourtChemin.Controls.Add(this.label1);
            this.panelTestPlusCourtChemin.Location = new System.Drawing.Point(14, 445);
            this.panelTestPlusCourtChemin.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.panelTestPlusCourtChemin.Name = "panelTestPlusCourtChemin";
            this.panelTestPlusCourtChemin.Size = new System.Drawing.Size(526, 208);
            this.panelTestPlusCourtChemin.TabIndex = 4;
            this.panelTestPlusCourtChemin.Visible = false;
            // 
            // labelArriveeTerrain
            // 
            this.labelArriveeTerrain.AutoSize = true;
            this.labelArriveeTerrain.Location = new System.Drawing.Point(90, 164);
            this.labelArriveeTerrain.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelArriveeTerrain.Name = "labelArriveeTerrain";
            this.labelArriveeTerrain.Size = new System.Drawing.Size(99, 15);
            this.labelArriveeTerrain.TabIndex = 9;
            this.labelArriveeTerrain.Text = "Terrain:xxxxxxxxx";
            // 
            // labelArriveeIDCASE
            // 
            this.labelArriveeIDCASE.AutoSize = true;
            this.labelArriveeIDCASE.Location = new System.Drawing.Point(90, 135);
            this.labelArriveeIDCASE.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelArriveeIDCASE.Name = "labelArriveeIDCASE";
            this.labelArriveeIDCASE.Size = new System.Drawing.Size(90, 15);
            this.labelArriveeIDCASE.TabIndex = 8;
            this.labelArriveeIDCASE.Text = "ID_CASE:000000";
            // 
            // labelArriveeY
            // 
            this.labelArriveeY.AutoSize = true;
            this.labelArriveeY.Location = new System.Drawing.Point(145, 112);
            this.labelArriveeY.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelArriveeY.Name = "labelArriveeY";
            this.labelArriveeY.Size = new System.Drawing.Size(41, 15);
            this.labelArriveeY.TabIndex = 7;
            this.labelArriveeY.Text = "Y:0000";
            // 
            // labelArriveeX
            // 
            this.labelArriveeX.AutoSize = true;
            this.labelArriveeX.Location = new System.Drawing.Point(90, 112);
            this.labelArriveeX.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelArriveeX.Name = "labelArriveeX";
            this.labelArriveeX.Size = new System.Drawing.Size(41, 15);
            this.labelArriveeX.TabIndex = 6;
            this.labelArriveeX.Text = "X:0000";
            // 
            // labelDepartTerrain
            // 
            this.labelDepartTerrain.AutoSize = true;
            this.labelDepartTerrain.Location = new System.Drawing.Point(90, 72);
            this.labelDepartTerrain.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelDepartTerrain.Name = "labelDepartTerrain";
            this.labelDepartTerrain.Size = new System.Drawing.Size(99, 15);
            this.labelDepartTerrain.TabIndex = 5;
            this.labelDepartTerrain.Text = "Terrain:xxxxxxxxx";
            // 
            // labelDepartIDCASE
            // 
            this.labelDepartIDCASE.AutoSize = true;
            this.labelDepartIDCASE.Location = new System.Drawing.Point(90, 43);
            this.labelDepartIDCASE.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelDepartIDCASE.Name = "labelDepartIDCASE";
            this.labelDepartIDCASE.Size = new System.Drawing.Size(90, 15);
            this.labelDepartIDCASE.TabIndex = 4;
            this.labelDepartIDCASE.Text = "ID_CASE:000000";
            // 
            // labelDepartY
            // 
            this.labelDepartY.AutoSize = true;
            this.labelDepartY.Location = new System.Drawing.Point(145, 10);
            this.labelDepartY.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelDepartY.Name = "labelDepartY";
            this.labelDepartY.Size = new System.Drawing.Size(41, 15);
            this.labelDepartY.TabIndex = 3;
            this.labelDepartY.Text = "Y:0000";
            // 
            // labelDepartX
            // 
            this.labelDepartX.AutoSize = true;
            this.labelDepartX.Location = new System.Drawing.Point(90, 10);
            this.labelDepartX.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelDepartX.Name = "labelDepartX";
            this.labelDepartX.Size = new System.Drawing.Size(41, 15);
            this.labelDepartX.TabIndex = 2;
            this.labelDepartX.Text = "X:0000";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 112);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 15);
            this.label2.TabIndex = 1;
            this.label2.Text = "Arriv�e :";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 10);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "D�part :";
            // 
            // panelImage
            // 
            this.panelImage.AutoScroll = true;
            this.panelImage.Controls.Add(this.ImageCarte);
            this.panelImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelImage.Location = new System.Drawing.Point(12, 12);
            this.panelImage.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.panelImage.Name = "panelImage";
            this.panelImage.Size = new System.Drawing.Size(372, 600);
            this.panelImage.TabIndex = 3;
            // 
            // ImageCarte
            // 
            this.ImageCarte.Interpolation = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            this.ImageCarte.Location = new System.Drawing.Point(0, 0);
            this.ImageCarte.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ImageCarte.Name = "ImageCarte";
            this.ImageCarte.QualiteDeComposition = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
            this.ImageCarte.Size = new System.Drawing.Size(117, 58);
            this.ImageCarte.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.ImageCarte.Smoothing = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
            this.ImageCarte.TabIndex = 3;
            this.ImageCarte.TabStop = false;
            this.ImageCarte.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ImageCarte_MouseClick);
            this.ImageCarte.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ImageCarte_MouseMove);
            // 
            // menuPrincipal
            // 
            this.menuPrincipal.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fichierToolStripMenuItem,
            this.ParametrageToolStripMenuItem,
            this.actionsToolStripMenuItem,
            this.AProposToolStripMenuItem});
            this.menuPrincipal.Location = new System.Drawing.Point(0, 0);
            this.menuPrincipal.Name = "menuPrincipal";
            this.menuPrincipal.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
            this.menuPrincipal.Size = new System.Drawing.Size(958, 24);
            this.menuPrincipal.TabIndex = 5;
            this.menuPrincipal.Text = "menuStrip1";
            // 
            // fichierToolStripMenuItem
            // 
            this.fichierToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.nouveauToolStripMenuItem,
            this.ouvrirToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveasToolStripMenuItem,
            this.toolStripSeparator1,
            this.fichiersRecentsToolStripMenuItem,
            this.toolStripSeparator2,
            this.quitterToolStripMenuItem});
            this.fichierToolStripMenuItem.Name = "fichierToolStripMenuItem";
            this.fichierToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
            this.fichierToolStripMenuItem.Text = "&Fichier";
            // 
            // nouveauToolStripMenuItem
            // 
            this.nouveauToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("nouveauToolStripMenuItem.Image")));
            this.nouveauToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.nouveauToolStripMenuItem.Name = "nouveauToolStripMenuItem";
            this.nouveauToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.nouveauToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
            this.nouveauToolStripMenuItem.Text = "&Nouveau";
            this.nouveauToolStripMenuItem.Click += new System.EventHandler(this.nouveauToolStripMenuItem_Click);
            // 
            // ouvrirToolStripMenuItem
            // 
            this.ouvrirToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("ouvrirToolStripMenuItem.Image")));
            this.ouvrirToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ouvrirToolStripMenuItem.Name = "ouvrirToolStripMenuItem";
            this.ouvrirToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.ouvrirToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
            this.ouvrirToolStripMenuItem.Text = "&Ouvrir";
            this.ouvrirToolStripMenuItem.Click += new System.EventHandler(this.ouvrirToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveToolStripMenuItem.Image")));
            this.saveToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
            this.saveToolStripMenuItem.Text = "&Sauvegarder";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveasToolStripMenuItem
            // 
            this.saveasToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveasToolStripMenuItem.Image")));
            this.saveasToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveasToolStripMenuItem.Name = "saveasToolStripMenuItem";
            this.saveasToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.saveasToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
            this.saveasToolStripMenuItem.Text = "s&Auvegarder Sous..";
            this.saveasToolStripMenuItem.Click += new System.EventHandler(this.saveasToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(213, 6);
            // 
            // fichiersRecentsToolStripMenuItem
            // 
            this.fichiersRecentsToolStripMenuItem.Name = "fichiersRecentsToolStripMenuItem";
            this.fichiersRecentsToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
            this.fichiersRecentsToolStripMenuItem.Text = "Fichiers R�cents";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(213, 6);
            // 
            // quitterToolStripMenuItem
            // 
            this.quitterToolStripMenuItem.Name = "quitterToolStripMenuItem";
            this.quitterToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
            this.quitterToolStripMenuItem.Text = "Q&uitter";
            this.quitterToolStripMenuItem.Click += new System.EventHandler(this.quitterToolStripMenuItem_Click);
            // 
            // ParametrageToolStripMenuItem
            // 
            this.ParametrageToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.generalToolStripMenuItem,
            this.meteoToolStripMenuItem,
            this.modelesDeMOuvementsToolStripMenuItem,
            this.nationsToolStripMenuItem,
            this.policeDeCaract�resToolStripMenuItem,
            this.fondDeCarteToolStripMenuItem,
            this.carteToolStripMenuItem,
            this.hPAStarToolStripMenuItem,
            this.pontsToolStripMenuItem,
            this.utilisateursToolStripMenuItem,
            this.rolesToolStripMenuItem,
            this.nomsDesLeadesPromusToolStripMenuItem,
            this.aptitudesToolStripMenuItem,
            this.modelesDePIonsToolStripMenuItem,
            this.aptitudesModelesDePIonsToolStripMenuItem,
            this.pionsToolStripMenuItem,
            this.renfortsToolStripMenuItem,
            this.ordresToolStripMenuItem,
            this.phrasesToolStripMenuItem,
            this.bataillesToolStripMenuItem,
            this.bataillesVideoToolStripMenuItem,
            this.mEssagesToolStripMenuItem,
            this.nomsPionsUniquesToolStripMenuItem,
            this.toolStripSeparator6,
            this.pionsAnciensToolStripMenuItem,
            this.ordresAnciensToolStripMenuItem,
            this.messagesAnciensToolStripMenuItem});
            this.ParametrageToolStripMenuItem.Name = "ParametrageToolStripMenuItem";
            this.ParametrageToolStripMenuItem.Size = new System.Drawing.Size(86, 20);
            this.ParametrageToolStripMenuItem.Text = "&Param�trage";
            // 
            // generalToolStripMenuItem
            // 
            this.generalToolStripMenuItem.Name = "generalToolStripMenuItem";
            this.generalToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
            this.generalToolStripMenuItem.Text = "&G�n�ral";
            this.generalToolStripMenuItem.Click += new System.EventHandler(this.generalToolStripMenuItem_Click);
            // 
            // meteoToolStripMenuItem
            // 
            this.meteoToolStripMenuItem.Name = "meteoToolStripMenuItem";
            this.meteoToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
            this.meteoToolStripMenuItem.Text = "&M�t�o";
            this.meteoToolStripMenuItem.Click += new System.EventHandler(this.meteoToolStripMenuItem_Click);
            // 
            // modelesDeMOuvementsToolStripMenuItem
            // 
            this.modelesDeMOuvementsToolStripMenuItem.Name = "modelesDeMOuvementsToolStripMenuItem";
            this.modelesDeMOuvementsToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
            this.modelesDeMOuvementsToolStripMenuItem.Text = "mod�les de m&Ouvements";
            this.modelesDeMOuvementsToolStripMenuItem.Click += new System.EventHandler(this.modelesDeMOuvementsToolStripMenuItem_Click);
            // 
            // nationsToolStripMenuItem
            // 
            this.nationsToolStripMenuItem.Name = "nationsToolStripMenuItem";
            this.nationsToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
            this.nationsToolStripMenuItem.Text = "&Nations";
            this.nationsToolStripMenuItem.Click += new System.EventHandler(this.modelesDeCombatToolStripMenuItem_Click);
            // 
            // policeDeCaract�resToolStripMenuItem
            // 
            this.policeDeCaract�resToolStripMenuItem.Name = "policeDeCaract�resToolStripMenuItem";
            this.policeDeCaract�resToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
            this.policeDeCaract�resToolStripMenuItem.Text = "&Police de caract�res";
            this.policeDeCaract�resToolStripMenuItem.Click += new System.EventHandler(this.policeDeCaract�resToolStripMenuItem_Click);
            // 
            // fondDeCarteToolStripMenuItem
            // 
            this.fondDeCarteToolStripMenuItem.Name = "fondDeCarteToolStripMenuItem";
            this.fondDeCarteToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
            this.fondDeCarteToolStripMenuItem.Text = "&Fond de carte";
            this.fondDeCarteToolStripMenuItem.Click += new System.EventHandler(this.fondDeCarteToolStripMenuItem_Click);
            // 
            // carteToolStripMenuItem
            // 
            this.carteToolStripMenuItem.Name = "carteToolStripMenuItem";
            this.carteToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
            this.carteToolStripMenuItem.Text = "&Carte";
            this.carteToolStripMenuItem.Click += new System.EventHandler(this.carteToolStripMenuItem_Click);
            // 
            // hPAStarToolStripMenuItem
            // 
            this.hPAStarToolStripMenuItem.Name = "hPAStarToolStripMenuItem";
            this.hPAStarToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
            this.hPAStarToolStripMenuItem.Text = "&HPAStar";
            this.hPAStarToolStripMenuItem.Click += new System.EventHandler(this.hPAStarToolStripMenuItem_Click);
            // 
            // pontsToolStripMenuItem
            // 
            this.pontsToolStripMenuItem.Name = "pontsToolStripMenuItem";
            this.pontsToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
            this.pontsToolStripMenuItem.Text = "Ponts";
            this.pontsToolStripMenuItem.Click += new System.EventHandler(this.pontsToolStripMenuItem_Click);
            // 
            // utilisateursToolStripMenuItem
            // 
            this.utilisateursToolStripMenuItem.Name = "utilisateursToolStripMenuItem";
            this.utilisateursToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
            this.utilisateursToolStripMenuItem.Text = "&Utilisateurs";
            this.utilisateursToolStripMenuItem.Click += new System.EventHandler(this.utilisateursToolStripMenuItem_Click);
            // 
            // rolesToolStripMenuItem
            // 
            this.rolesToolStripMenuItem.Name = "rolesToolStripMenuItem";
            this.rolesToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
            this.rolesToolStripMenuItem.Text = "&R�les";
            this.rolesToolStripMenuItem.Click += new System.EventHandler(this.rolesToolStripMenuItem_Click);
            // 
            // nomsDesLeadesPromusToolStripMenuItem
            // 
            this.nomsDesLeadesPromusToolStripMenuItem.Name = "nomsDesLeadesPromusToolStripMenuItem";
            this.nomsDesLeadesPromusToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
            this.nomsDesLeadesPromusToolStripMenuItem.Text = "Noms des leades promus";
            this.nomsDesLeadesPromusToolStripMenuItem.Click += new System.EventHandler(this.nomsDesLeadesPromusToolStripMenuItem_Click);
            // 
            // aptitudesToolStripMenuItem
            // 
            this.aptitudesToolStripMenuItem.Name = "aptitudesToolStripMenuItem";
            this.aptitudesToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
            this.aptitudesToolStripMenuItem.Text = "&Aptitudes";
            this.aptitudesToolStripMenuItem.Click += new System.EventHandler(this.aptitudesToolStripMenuItem_Click);
            // 
            // modelesDePIonsToolStripMenuItem
            // 
            this.modelesDePIonsToolStripMenuItem.Name = "modelesDePIonsToolStripMenuItem";
            this.modelesDePIonsToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
            this.modelesDePIonsToolStripMenuItem.Text = "mod�les de p&Ions";
            this.modelesDePIonsToolStripMenuItem.Click += new System.EventHandler(this.modelesDePIonsToolStripMenuItem_Click);
            // 
            // aptitudesModelesDePIonsToolStripMenuItem
            // 
            this.aptitudesModelesDePIonsToolStripMenuItem.Name = "aptitudesModelesDePIonsToolStripMenuItem";
            this.aptitudesModelesDePIonsToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
            this.aptitudesModelesDePIonsToolStripMenuItem.Text = "&Aptitudes des mod�les de pions";
            this.aptitudesModelesDePIonsToolStripMenuItem.Click += new System.EventHandler(this.aptitudesModelesDePIonsToolStripMenuItem_Click);
            // 
            // pionsToolStripMenuItem
            // 
            this.pionsToolStripMenuItem.Name = "pionsToolStripMenuItem";
            this.pionsToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
            this.pionsToolStripMenuItem.Text = "&Pions";
            this.pionsToolStripMenuItem.Click += new System.EventHandler(this.pionsToolStripMenuItem_Click);
            // 
            // renfortsToolStripMenuItem
            // 
            this.renfortsToolStripMenuItem.Name = "renfortsToolStripMenuItem";
            this.renfortsToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
            this.renfortsToolStripMenuItem.Text = "&Renforts";
            this.renfortsToolStripMenuItem.Click += new System.EventHandler(this.renfortsToolStripMenuItem_Click);
            // 
            // ordresToolStripMenuItem
            // 
            this.ordresToolStripMenuItem.Name = "ordresToolStripMenuItem";
            this.ordresToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
            this.ordresToolStripMenuItem.Text = "&Ordres";
            this.ordresToolStripMenuItem.Click += new System.EventHandler(this.ordresToolStripMenuItem_Click);
            // 
            // phrasesToolStripMenuItem
            // 
            this.phrasesToolStripMenuItem.Name = "phrasesToolStripMenuItem";
            this.phrasesToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
            this.phrasesToolStripMenuItem.Text = "P&hrases";
            this.phrasesToolStripMenuItem.Click += new System.EventHandler(this.phrasesToolStripMenuItem_Click);
            // 
            // bataillesToolStripMenuItem
            // 
            this.bataillesToolStripMenuItem.Name = "bataillesToolStripMenuItem";
            this.bataillesToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
            this.bataillesToolStripMenuItem.Text = "&Batailles";
            this.bataillesToolStripMenuItem.Click += new System.EventHandler(this.bataillesToolStripMenuItem_Click);
            // 
            // bataillesVideoToolStripMenuItem
            // 
            this.bataillesVideoToolStripMenuItem.Name = "bataillesVideoToolStripMenuItem";
            this.bataillesVideoToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
            this.bataillesVideoToolStripMenuItem.Text = "BataillesVideo";
            this.bataillesVideoToolStripMenuItem.Click += new System.EventHandler(this.bataillesVideoToolStripMenuItem_Click);
            // 
            // mEssagesToolStripMenuItem
            // 
            this.mEssagesToolStripMenuItem.Name = "mEssagesToolStripMenuItem";
            this.mEssagesToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
            this.mEssagesToolStripMenuItem.Text = "M&Essages";
            this.mEssagesToolStripMenuItem.Click += new System.EventHandler(this.mEssagesToolStripMenuItem_Click);
            // 
            // nomsPionsUniquesToolStripMenuItem
            // 
            this.nomsPionsUniquesToolStripMenuItem.Name = "nomsPionsUniquesToolStripMenuItem";
            this.nomsPionsUniquesToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
            this.nomsPionsUniquesToolStripMenuItem.Text = "Noms Pions Uniques";
            this.nomsPionsUniquesToolStripMenuItem.Click += new System.EventHandler(this.nomsPionsUniquesToolStripMenuItem_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(239, 6);
            // 
            // pionsAnciensToolStripMenuItem
            // 
            this.pionsAnciensToolStripMenuItem.Name = "pionsAnciensToolStripMenuItem";
            this.pionsAnciensToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
            this.pionsAnciensToolStripMenuItem.Text = "Pions Anciens";
            this.pionsAnciensToolStripMenuItem.Click += new System.EventHandler(this.pionsAnciensToolStripMenuItem_Click);
            // 
            // ordresAnciensToolStripMenuItem
            // 
            this.ordresAnciensToolStripMenuItem.Name = "ordresAnciensToolStripMenuItem";
            this.ordresAnciensToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
            this.ordresAnciensToolStripMenuItem.Text = "Ordres Anciens";
            this.ordresAnciensToolStripMenuItem.Click += new System.EventHandler(this.ordresAnciensToolStripMenuItem_Click);
            // 
            // messagesAnciensToolStripMenuItem
            // 
            this.messagesAnciensToolStripMenuItem.Name = "messagesAnciensToolStripMenuItem";
            this.messagesAnciensToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
            this.messagesAnciensToolStripMenuItem.Text = "Messages Anciens";
            this.messagesAnciensToolStripMenuItem.Click += new System.EventHandler(this.messagesAnciensToolStripMenuItem_Click);
            // 
            // actionsToolStripMenuItem
            // 
            this.actionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.heureSuivanteToolStripMenuItem,
            this.creationInternetToolStripMenuItem,
            this.mise�JourInternetToolStripMenuItem,
            this.notificationAuxJoueursToolStripMenuItem,
            this.copieDeSauvegardeToolStripMenuItem,
            this.toolStripSeparator5,
            this.initilisationPartieToolStripMenuItem,
            this.forcesInitialesToolStripMenuItem,
            this.donneesVid�oToolStripMenuItem,
            this.genererLeFilmToolStripMenuItem,
            this.genererLesFilmsDeBatailleToolStripMenuItem,
            this.testsToolStripMenuItem,
            this.outilsToolStripMenuItem,
            this.mise�JourProprietairesToolStripMenuItem,
            this.statistiquesToolStripMenuItem,
            this.extractionDeLaBaseEnCSVToolStripMenuItem,
            this.repriseDeDonn�esToolStripMenuItem,
            this.sQLDeTousLesMessagesToolStripMenuItem,
            this.toolStripSeparator7,
            this.actuelsAnciensToolStripMenuItem,
            this.anciensActuelsToolStripMenuItem,
            this.toolStripSeparator8,
            this.broderiequadrillageToolStripMenuItem});
            this.actionsToolStripMenuItem.Name = "actionsToolStripMenuItem";
            this.actionsToolStripMenuItem.Size = new System.Drawing.Size(59, 20);
            this.actionsToolStripMenuItem.Text = "&Actions";
            // 
            // heureSuivanteToolStripMenuItem
            // 
            this.heureSuivanteToolStripMenuItem.Name = "heureSuivanteToolStripMenuItem";
            this.heureSuivanteToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.H)));
            this.heureSuivanteToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.heureSuivanteToolStripMenuItem.Text = "&Heure suivante";
            this.heureSuivanteToolStripMenuItem.Click += new System.EventHandler(this.heureSuivanteToolStripMenuItem_Click);
            // 
            // creationInternetToolStripMenuItem
            // 
            this.creationInternetToolStripMenuItem.Name = "creationInternetToolStripMenuItem";
            this.creationInternetToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.creationInternetToolStripMenuItem.Text = "&Cr�ation Fichiers Internet";
            this.creationInternetToolStripMenuItem.Click += new System.EventHandler(this.creationInternetToolStripMenuItem_Click);
            // 
            // mise�JourInternetToolStripMenuItem
            // 
            this.mise�JourInternetToolStripMenuItem.Name = "mise�JourInternetToolStripMenuItem";
            this.mise�JourInternetToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.mise�JourInternetToolStripMenuItem.Text = "Mise � jour internet";
            this.mise�JourInternetToolStripMenuItem.Click += new System.EventHandler(this.mise�JourInternetToolStripMenuItem_Click);
            // 
            // notificationAuxJoueursToolStripMenuItem
            // 
            this.notificationAuxJoueursToolStripMenuItem.Name = "notificationAuxJoueursToolStripMenuItem";
            this.notificationAuxJoueursToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.notificationAuxJoueursToolStripMenuItem.Text = "&Notification aux Joueurs";
            this.notificationAuxJoueursToolStripMenuItem.Click += new System.EventHandler(this.notificationAuxJoueursToolStripMenuItem_Click);
            // 
            // copieDeSauvegardeToolStripMenuItem
            // 
            this.copieDeSauvegardeToolStripMenuItem.Name = "copieDeSauvegardeToolStripMenuItem";
            this.copieDeSauvegardeToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.copieDeSauvegardeToolStripMenuItem.Text = "Copie de Sauvegarde";
            this.copieDeSauvegardeToolStripMenuItem.Click += new System.EventHandler(this.copieDeSauvegardeToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(219, 6);
            // 
            // initilisationPartieToolStripMenuItem
            // 
            this.initilisationPartieToolStripMenuItem.Name = "initilisationPartieToolStripMenuItem";
            this.initilisationPartieToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.initilisationPartieToolStripMenuItem.Text = "&Initilisation Partie";
            this.initilisationPartieToolStripMenuItem.Click += new System.EventHandler(this.initilisationPartieToolStripMenuItem_Click);
            // 
            // forcesInitialesToolStripMenuItem
            // 
            this.forcesInitialesToolStripMenuItem.Name = "forcesInitialesToolStripMenuItem";
            this.forcesInitialesToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.forcesInitialesToolStripMenuItem.Text = "Forces initiales";
            this.forcesInitialesToolStripMenuItem.Click += new System.EventHandler(this.forcesInitialesToolStripMenuItem_Click);
            // 
            // donneesVid�oToolStripMenuItem
            // 
            this.donneesVid�oToolStripMenuItem.Name = "donneesVid�oToolStripMenuItem";
            this.donneesVid�oToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.donneesVid�oToolStripMenuItem.Text = "Donnees Vid�o";
            this.donneesVid�oToolStripMenuItem.Click += new System.EventHandler(this.donneesVid�oToolStripMenuItem_Click);
            // 
            // genererLeFilmToolStripMenuItem
            // 
            this.genererLeFilmToolStripMenuItem.Name = "genererLeFilmToolStripMenuItem";
            this.genererLeFilmToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.genererLeFilmToolStripMenuItem.Text = "&G�n�rer le film";
            this.genererLeFilmToolStripMenuItem.Click += new System.EventHandler(this.genererLeFilmToolStripMenuItem_Click);
            // 
            // genererLesFilmsDeBatailleToolStripMenuItem
            // 
            this.genererLesFilmsDeBatailleToolStripMenuItem.Name = "genererLesFilmsDeBatailleToolStripMenuItem";
            this.genererLesFilmsDeBatailleToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.genererLesFilmsDeBatailleToolStripMenuItem.Text = "Generer les films de Bataille";
            this.genererLesFilmsDeBatailleToolStripMenuItem.Click += new System.EventHandler(this.genererLesFilmsDeBatailleToolStripMenuItem_Click);
            // 
            // testsToolStripMenuItem
            // 
            this.testsToolStripMenuItem.Name = "testsToolStripMenuItem";
            this.testsToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.testsToolStripMenuItem.Text = "&Tests";
            this.testsToolStripMenuItem.Click += new System.EventHandler(this.testsToolStripMenuItem_Click);
            // 
            // outilsToolStripMenuItem
            // 
            this.outilsToolStripMenuItem.Name = "outilsToolStripMenuItem";
            this.outilsToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.outilsToolStripMenuItem.Text = "&Outils";
            this.outilsToolStripMenuItem.Click += new System.EventHandler(this.outilsToolStripMenuItem_Click);
            // 
            // mise�JourProprietairesToolStripMenuItem
            // 
            this.mise�JourProprietairesToolStripMenuItem.Name = "mise�JourProprietairesToolStripMenuItem";
            this.mise�JourProprietairesToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.mise�JourProprietairesToolStripMenuItem.Text = "Mise � jour Proprietaires";
            this.mise�JourProprietairesToolStripMenuItem.Click += new System.EventHandler(this.mise�JourProprietairesToolStripMenuItem_Click);
            // 
            // statistiquesToolStripMenuItem
            // 
            this.statistiquesToolStripMenuItem.Name = "statistiquesToolStripMenuItem";
            this.statistiquesToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.statistiquesToolStripMenuItem.Text = "&Statistiques";
            this.statistiquesToolStripMenuItem.Click += new System.EventHandler(this.statistiquesToolStripMenuItem_Click);
            // 
            // extractionDeLaBaseEnCSVToolStripMenuItem
            // 
            this.extractionDeLaBaseEnCSVToolStripMenuItem.Name = "extractionDeLaBaseEnCSVToolStripMenuItem";
            this.extractionDeLaBaseEnCSVToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.extractionDeLaBaseEnCSVToolStripMenuItem.Text = "Extraction de la base en CSV";
            this.extractionDeLaBaseEnCSVToolStripMenuItem.Click += new System.EventHandler(this.extractionDeLaBaseEnCSVToolStripMenuItem_Click);
            // 
            // repriseDeDonn�esToolStripMenuItem
            // 
            this.repriseDeDonn�esToolStripMenuItem.Name = "repriseDeDonn�esToolStripMenuItem";
            this.repriseDeDonn�esToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.repriseDeDonn�esToolStripMenuItem.Text = "&Reprise de Donn�es";
            this.repriseDeDonn�esToolStripMenuItem.Click += new System.EventHandler(this.repriseDeDonn�esToolStripMenuItem_Click);
            // 
            // sQLDeTousLesMessagesToolStripMenuItem
            // 
            this.sQLDeTousLesMessagesToolStripMenuItem.Name = "sQLDeTousLesMessagesToolStripMenuItem";
            this.sQLDeTousLesMessagesToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.sQLDeTousLesMessagesToolStripMenuItem.Text = "SQL de tous les messages";
            this.sQLDeTousLesMessagesToolStripMenuItem.Click += new System.EventHandler(this.sQLDeTousLesMessagesToolStripMenuItem_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(219, 6);
            // 
            // actuelsAnciensToolStripMenuItem
            // 
            this.actuelsAnciensToolStripMenuItem.Name = "actuelsAnciensToolStripMenuItem";
            this.actuelsAnciensToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.actuelsAnciensToolStripMenuItem.Text = "Actuels->Anciens";
            this.actuelsAnciensToolStripMenuItem.Click += new System.EventHandler(this.actuelsAnciensToolStripMenuItem_Click);
            // 
            // anciensActuelsToolStripMenuItem
            // 
            this.anciensActuelsToolStripMenuItem.Name = "anciensActuelsToolStripMenuItem";
            this.anciensActuelsToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.anciensActuelsToolStripMenuItem.Text = "Anciens->Actuels";
            this.anciensActuelsToolStripMenuItem.Click += new System.EventHandler(this.anciensActuelsToolStripMenuItem_Click);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(219, 6);
            // 
            // broderiequadrillageToolStripMenuItem
            // 
            this.broderiequadrillageToolStripMenuItem.Name = "broderiequadrillageToolStripMenuItem";
            this.broderiequadrillageToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.broderiequadrillageToolStripMenuItem.Text = "Broderie (quadrillage)";
            this.broderiequadrillageToolStripMenuItem.Click += new System.EventHandler(this.broderiequadrillageToolStripMenuItem_Click);
            // 
            // AProposToolStripMenuItem
            // 
            this.AProposToolStripMenuItem.Name = "AProposToolStripMenuItem";
            this.AProposToolStripMenuItem.Size = new System.Drawing.Size(65, 20);
            this.AProposToolStripMenuItem.Text = "� &Propos";
            this.AProposToolStripMenuItem.Click += new System.EventHandler(this.AProposToolStripMenuItem_Click);
            // 
            // toolStripCarte
            // 
            this.toolStripCarte.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripCarte.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripAfficherVilles,
            this.toolStripAfficherUnites,
            this.toolStripAfficherBatailles,
            this.toolStripAffichierTopographie,
            this.toolStripSeparator3,
            this.toolStripZoomPlus,
            this.toolStripZoomMoins,
            this.toolStripSeparator4,
            this.toolStripPlusCourtChemin,
            this.toolStripButtonPontEndommage,
            this.toolStripButtonReparerPont,
            this.toolStripButtonPontDetruit,
            this.toolStripButtonConstruirePonton,
            this.toolStripButtonTrajets,
            this.toolStripButtonTrajetsVilles,
            this.toolStripButtonMemoire,
            this.toolStripButtonDepots});
            this.toolStripCarte.Location = new System.Drawing.Point(0, 24);
            this.toolStripCarte.Name = "toolStripCarte";
            this.toolStripCarte.Size = new System.Drawing.Size(958, 25);
            this.toolStripCarte.TabIndex = 6;
            this.toolStripCarte.Text = "Barre d\'outils";
            // 
            // toolStripAfficherVilles
            // 
            this.toolStripAfficherVilles.CheckOnClick = true;
            this.toolStripAfficherVilles.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripAfficherVilles.Image = global::vaoc.Properties.Resources.voirVilles;
            this.toolStripAfficherVilles.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripAfficherVilles.Name = "toolStripAfficherVilles";
            this.toolStripAfficherVilles.Size = new System.Drawing.Size(23, 22);
            this.toolStripAfficherVilles.Tag = "Affiche/Efface les villes";
            this.toolStripAfficherVilles.Text = "Afficher/Effacer Villes";
            this.toolStripAfficherVilles.Click += new System.EventHandler(this.toolStripAfficherVilles_Click);
            // 
            // toolStripAfficherUnites
            // 
            this.toolStripAfficherUnites.CheckOnClick = true;
            this.toolStripAfficherUnites.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripAfficherUnites.Image = global::vaoc.Properties.Resources.voirUnites;
            this.toolStripAfficherUnites.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripAfficherUnites.Name = "toolStripAfficherUnites";
            this.toolStripAfficherUnites.Size = new System.Drawing.Size(23, 22);
            this.toolStripAfficherUnites.Text = "Afficher/Effacer Unit�s";
            this.toolStripAfficherUnites.Click += new System.EventHandler(this.toolStripAfficherUnites_Click);
            // 
            // toolStripAfficherBatailles
            // 
            this.toolStripAfficherBatailles.Checked = true;
            this.toolStripAfficherBatailles.CheckOnClick = true;
            this.toolStripAfficherBatailles.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripAfficherBatailles.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripAfficherBatailles.Image = global::vaoc.Properties.Resources.voirBatailles;
            this.toolStripAfficherBatailles.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripAfficherBatailles.Name = "toolStripAfficherBatailles";
            this.toolStripAfficherBatailles.Size = new System.Drawing.Size(23, 22);
            this.toolStripAfficherBatailles.Tag = "Afficher / Effacer Batailles";
            this.toolStripAfficherBatailles.Text = "Afficher / Effacer Batailles";
            this.toolStripAfficherBatailles.Click += new System.EventHandler(this.toolStripButtonAfficherBatailles_Click);
            // 
            // toolStripAffichierTopographie
            // 
            this.toolStripAffichierTopographie.CheckOnClick = true;
            this.toolStripAffichierTopographie.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripAffichierTopographie.Image = global::vaoc.Properties.Resources.voirTopographie;
            this.toolStripAffichierTopographie.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripAffichierTopographie.Name = "toolStripAffichierTopographie";
            this.toolStripAffichierTopographie.Size = new System.Drawing.Size(23, 22);
            this.toolStripAffichierTopographie.Text = "voir carte topographique";
            this.toolStripAffichierTopographie.Click += new System.EventHandler(this.toolStripButtonAffichierTopographie_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripZoomPlus
            // 
            this.toolStripZoomPlus.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripZoomPlus.Image = global::vaoc.Properties.Resources.zoomPlus;
            this.toolStripZoomPlus.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripZoomPlus.Name = "toolStripZoomPlus";
            this.toolStripZoomPlus.Size = new System.Drawing.Size(23, 22);
            this.toolStripZoomPlus.Tag = "Zoom +";
            this.toolStripZoomPlus.Text = "Zoom +";
            this.toolStripZoomPlus.Click += new System.EventHandler(this.toolStripButtonZoomPlus_Click);
            // 
            // toolStripZoomMoins
            // 
            this.toolStripZoomMoins.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripZoomMoins.Image = global::vaoc.Properties.Resources.zoomMoins;
            this.toolStripZoomMoins.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripZoomMoins.Name = "toolStripZoomMoins";
            this.toolStripZoomMoins.Size = new System.Drawing.Size(23, 22);
            this.toolStripZoomMoins.Tag = "Zoom -";
            this.toolStripZoomMoins.Text = "Zoom -";
            this.toolStripZoomMoins.Click += new System.EventHandler(this.toolStripButtonZoomMoins_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripPlusCourtChemin
            // 
            this.toolStripPlusCourtChemin.CheckOnClick = true;
            this.toolStripPlusCourtChemin.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripPlusCourtChemin.Image = global::vaoc.Properties.Resources.testPCC;
            this.toolStripPlusCourtChemin.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripPlusCourtChemin.Name = "toolStripPlusCourtChemin";
            this.toolStripPlusCourtChemin.Size = new System.Drawing.Size(23, 22);
            this.toolStripPlusCourtChemin.Text = "Test sur le plus court chemin";
            this.toolStripPlusCourtChemin.Click += new System.EventHandler(this.toolStripButtonPlusCourtChemin_Click);
            // 
            // toolStripButtonPontEndommage
            // 
            this.toolStripButtonPontEndommage.CheckOnClick = true;
            this.toolStripButtonPontEndommage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonPontEndommage.Image = global::vaoc.Properties.Resources.pontendommage;
            this.toolStripButtonPontEndommage.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonPontEndommage.Name = "toolStripButtonPontEndommage";
            this.toolStripButtonPontEndommage.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonPontEndommage.Text = "Pont endommag�";
            this.toolStripButtonPontEndommage.Click += new System.EventHandler(this.toolStripButtonPontEndommage_Click);
            // 
            // toolStripButtonReparerPont
            // 
            this.toolStripButtonReparerPont.CheckOnClick = true;
            this.toolStripButtonReparerPont.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonReparerPont.Image = global::vaoc.Properties.Resources.pont;
            this.toolStripButtonReparerPont.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonReparerPont.Name = "toolStripButtonReparerPont";
            this.toolStripButtonReparerPont.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonReparerPont.Text = "R�parer un pont";
            this.toolStripButtonReparerPont.Click += new System.EventHandler(this.toolStripButtonPont_Click);
            // 
            // toolStripButtonPontDetruit
            // 
            this.toolStripButtonPontDetruit.CheckOnClick = true;
            this.toolStripButtonPontDetruit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonPontDetruit.Image = global::vaoc.Properties.Resources.pontdetruit;
            this.toolStripButtonPontDetruit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonPontDetruit.Name = "toolStripButtonPontDetruit";
            this.toolStripButtonPontDetruit.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonPontDetruit.Text = "Detruire un pont";
            this.toolStripButtonPontDetruit.Click += new System.EventHandler(this.toolStripButtonPontDetruit_Click);
            // 
            // toolStripButtonConstruirePonton
            // 
            this.toolStripButtonConstruirePonton.CheckOnClick = true;
            this.toolStripButtonConstruirePonton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonConstruirePonton.Image = global::vaoc.Properties.Resources.ponton;
            this.toolStripButtonConstruirePonton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonConstruirePonton.Name = "toolStripButtonConstruirePonton";
            this.toolStripButtonConstruirePonton.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonConstruirePonton.Text = "Construire Ponton";
            this.toolStripButtonConstruirePonton.Click += new System.EventHandler(this.toolStripButtonConstruirePonton_Click);
            // 
            // toolStripButtonTrajets
            // 
            this.toolStripButtonTrajets.CheckOnClick = true;
            this.toolStripButtonTrajets.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonTrajets.Image = global::vaoc.Properties.Resources.voirTrajets;
            this.toolStripButtonTrajets.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonTrajets.Name = "toolStripButtonTrajets";
            this.toolStripButtonTrajets.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonTrajets.Text = "Contr�le des Trajets";
            this.toolStripButtonTrajets.Click += new System.EventHandler(this.toolStripButtonTrajets_Click);
            // 
            // toolStripButtonTrajetsVilles
            // 
            this.toolStripButtonTrajetsVilles.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonTrajetsVilles.Image = global::vaoc.Properties.Resources.voirTrajetsVilles;
            this.toolStripButtonTrajetsVilles.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonTrajetsVilles.Name = "toolStripButtonTrajetsVilles";
            this.toolStripButtonTrajetsVilles.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonTrajetsVilles.Text = "voir les trajets entre villes";
            this.toolStripButtonTrajetsVilles.Click += new System.EventHandler(this.toolStripButtonTrajetsVilles_Click);
            // 
            // toolStripButtonMemoire
            // 
            this.toolStripButtonMemoire.CheckOnClick = true;
            this.toolStripButtonMemoire.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonMemoire.Image = global::vaoc.Properties.Resources.memoire;
            this.toolStripButtonMemoire.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonMemoire.Name = "toolStripButtonMemoire";
            this.toolStripButtonMemoire.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonMemoire.Text = "Afficher les cases charg�es";
            this.toolStripButtonMemoire.Click += new System.EventHandler(this.toolStripButtonMemoire_Click);
            // 
            // toolStripButtonDepots
            // 
            this.toolStripButtonDepots.CheckOnClick = true;
            this.toolStripButtonDepots.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonDepots.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonDepots.Image")));
            this.toolStripButtonDepots.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonDepots.Name = "toolStripButtonDepots";
            this.toolStripButtonDepots.Size = new System.Drawing.Size(48, 22);
            this.toolStripButtonDepots.Text = "D�p�ts";
            this.toolStripButtonDepots.Click += new System.EventHandler(this.toolStripButtonDepots_Click);
            // 
            // backgroundTraitement
            // 
            this.backgroundTraitement.WorkerReportsProgress = true;
            this.backgroundTraitement.WorkerSupportsCancellation = true;
            this.backgroundTraitement.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundTraitement_DoWork);
            this.backgroundTraitement.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundTraitement_ProgressChanged);
            this.backgroundTraitement.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundTraitement_RunWorkerCompleted);
            // 
            // FormPrincipale
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(958, 727);
            this.Controls.Add(this.toolStripCarte);
            this.Controls.Add(this.menuPrincipal);
            this.Controls.Add(this.splitContainer);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "FormPrincipale";
            this.Text = "VAOC";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormPrincipale_FormClosing);
            this.Shown += new System.EventHandler(this.FormPrincipale_Shown);
            this.Resize += new System.EventHandler(this.FormPrincipale_Resize);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel1.PerformLayout();
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.panelInformation.ResumeLayout(false);
            this.panelInformation.PerformLayout();
            this.panelTestPlusCourtChemin.ResumeLayout(false);
            this.panelTestPlusCourtChemin.PerformLayout();
            this.panelImage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ImageCarte)).EndInit();
            this.menuPrincipal.ResumeLayout(false);
            this.menuPrincipal.PerformLayout();
            this.toolStripCarte.ResumeLayout(false);
            this.toolStripCarte.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Button buttonData;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.MenuStrip menuPrincipal;
        private System.Windows.Forms.ToolStripMenuItem fichierToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nouveauToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ouvrirToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveasToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem quitterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ParametrageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem AProposToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem actionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem carteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem utilisateursToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem creationInternetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem heureSuivanteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem genererLeFilmToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem fichiersRecentsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem meteoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem modelesDeMOuvementsToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStripCarte;
        private System.Windows.Forms.ToolStripButton toolStripAfficherVilles;
        private System.Windows.Forms.ToolStripButton toolStripAfficherUnites;
        private System.Windows.Forms.ToolStripButton toolStripZoomPlus;
        private System.Windows.Forms.ToolStripButton toolStripZoomMoins;
        private System.Windows.Forms.ToolStripMenuItem policeDeCaract�resToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fondDeCarteToolStripMenuItem;
        private System.Windows.Forms.Panel panelTestPlusCourtChemin;
        private System.Windows.Forms.Label labelDepartTerrain;
        private System.Windows.Forms.Label labelDepartIDCASE;
        private System.Windows.Forms.Label labelDepartY;
        private System.Windows.Forms.Label labelDepartX;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.Label labelArriveeTerrain;
        private System.Windows.Forms.Label labelArriveeIDCASE;
        private System.Windows.Forms.Label labelArriveeY;
        private System.Windows.Forms.Label labelArriveeX;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton toolStripPlusCourtChemin;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rolesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aptitudesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem modelesDePIonsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nationsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aptitudesModelesDePIonsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ordresToolStripMenuItem;
        private System.Windows.Forms.Panel panelImage;
        public VaocPictureBox ImageCarte;
        private System.Windows.Forms.Panel panelInformation;
        private System.Windows.Forms.Label labelInformationTerrain;
        private System.Windows.Forms.Label labelInformationIDCASE;
        private System.Windows.Forms.Label labelInformationY;
        private System.Windows.Forms.Label labelInformationX;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label labelInformationTempsRestant;
        private System.Windows.Forms.Label labelInformationTempsPasse;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.ComponentModel.BackgroundWorker backgroundTraitement;
        private System.Windows.Forms.Label labelPhase;
        private System.Windows.Forms.Label labelCoutTerrain;
        private System.Windows.Forms.Label labelProprietaire;
        private System.Windows.Forms.ToolStripMenuItem phrasesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bataillesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mEssagesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem testsToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton toolStripAfficherBatailles;
        private System.Windows.Forms.ToolStripMenuItem outilsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem notificationAuxJoueursToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripButton toolStripAffichierTopographie;
        private System.Windows.Forms.ToolStripMenuItem renfortsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mise�JourInternetToolStripMenuItem;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBoxCaseID;
        private System.Windows.Forms.Button buttonCaseID;
        private System.Windows.Forms.ComboBox comboBoxListeUnites;
        private System.Windows.Forms.ToolStripMenuItem hPAStarToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem initilisationPartieToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton toolStripButtonReparerPont;
        private System.Windows.Forms.ToolStripButton toolStripButtonPontDetruit;
        private System.Windows.Forms.Button buttonRecalculTrajet;
        private System.Windows.Forms.Button buttonVerifierTrajet;
        private System.Windows.Forms.ToolStripButton toolStripButtonTrajets;
        private System.Windows.Forms.ToolStripButton toolStripButtonTrajetsVilles;
        private System.Windows.Forms.ToolStripButton toolStripButtonConstruirePonton;
        private System.Windows.Forms.ToolStripButton toolStripButtonPontEndommage;
        private System.Windows.Forms.Label labelTour;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ToolStripMenuItem nomsDesLeadesPromusToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem statistiquesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pontsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem messagesAnciensToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem pionsAnciensToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ordresAnciensToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem repriseDeDonn�esToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem donneesVid�oToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton toolStripButtonMemoire;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripMenuItem actuelsAnciensToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem anciensActuelsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nomsPionsUniquesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem extractionDeLaBaseEnCSVToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sQLDeTousLesMessagesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.ToolStripMenuItem broderiequadrillageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copieDeSauvegardeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem forcesInitialesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mise�JourProprietairesToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton toolStripButtonDepots;
        private System.Windows.Forms.ToolStripMenuItem bataillesVideoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem genererLesFilmsDeBatailleToolStripMenuItem;
        //private System.Windows.Forms.ToolStripButton toolStripButton1;
    }
}

