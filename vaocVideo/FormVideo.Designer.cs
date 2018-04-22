﻿namespace vaocVideo
{
    partial class FormVideo
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
            this.buttonCreerFilm = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxRepertoireImages = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxRepertoireVideo = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxLargeurBase = new System.Windows.Forms.TextBox();
            this.textBoxHauteurBase = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.fontDialog = new System.Windows.Forms.FontDialog();
            this.buttonChoixPolice = new System.Windows.Forms.Button();
            this.labelPolice = new System.Windows.Forms.Label();
            this.buttonRepertoireSource = new System.Windows.Forms.Button();
            this.buttonRepertoireSortie = new System.Windows.Forms.Button();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxMasque = new System.Windows.Forms.TextBox();
            this.buttonOuvrirFilm = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.backgroundTraitement = new System.ComponentModel.BackgroundWorker();
            this.checkBoxCarteUnites = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.textBoxTailleUnite = new System.Windows.Forms.TextBox();
            this.textBoxEpaisseurUnite = new System.Windows.Forms.TextBox();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.buttonChargerPartie = new System.Windows.Forms.Button();
            this.textBoxNomPartie = new System.Windows.Forms.TextBox();
            this.buttonDonnees = new System.Windows.Forms.Button();
            this.checkBoxCarteGlobale = new System.Windows.Forms.CheckBox();
            this.checkBoxFilm = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // buttonCreerFilm
            // 
            this.buttonCreerFilm.Location = new System.Drawing.Point(391, 478);
            this.buttonCreerFilm.Name = "buttonCreerFilm";
            this.buttonCreerFilm.Size = new System.Drawing.Size(99, 23);
            this.buttonCreerFilm.TabIndex = 0;
            this.buttonCreerFilm.Tag = "Film simple";
            this.buttonCreerFilm.Text = "Creer Film";
            this.buttonCreerFilm.UseVisualStyleBackColor = true;
            this.buttonCreerFilm.Click += new System.EventHandler(this.buttonCreerFilm_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 54);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(112, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Repertoire des images";
            // 
            // textBoxRepertoireImages
            // 
            this.textBoxRepertoireImages.Location = new System.Drawing.Point(163, 80);
            this.textBoxRepertoireImages.Name = "textBoxRepertoireImages";
            this.textBoxRepertoireImages.Size = new System.Drawing.Size(401, 20);
            this.textBoxRepertoireImages.TabIndex = 2;
            this.textBoxRepertoireImages.Text = "C:\\Users\\Public\\Documents\\vaoc\\1813";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 114);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(99, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Repertoire de sortie";
            // 
            // textBoxRepertoireVideo
            // 
            this.textBoxRepertoireVideo.Location = new System.Drawing.Point(163, 142);
            this.textBoxRepertoireVideo.Name = "textBoxRepertoireVideo";
            this.textBoxRepertoireVideo.Size = new System.Drawing.Size(401, 20);
            this.textBoxRepertoireVideo.TabIndex = 4;
            this.textBoxRepertoireVideo.Text = "C:\\Users\\Public\\Documents\\vaoc\\1813\\video";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(576, 114);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Largeur";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(688, 114);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(45, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Hauteur";
            // 
            // textBoxLargeurBase
            // 
            this.textBoxLargeurBase.Location = new System.Drawing.Point(579, 142);
            this.textBoxLargeurBase.Name = "textBoxLargeurBase";
            this.textBoxLargeurBase.Size = new System.Drawing.Size(67, 20);
            this.textBoxLargeurBase.TabIndex = 7;
            this.textBoxLargeurBase.Text = "800";
            // 
            // textBoxHauteurBase
            // 
            this.textBoxHauteurBase.Location = new System.Drawing.Point(691, 142);
            this.textBoxHauteurBase.Name = "textBoxHauteurBase";
            this.textBoxHauteurBase.Size = new System.Drawing.Size(67, 20);
            this.textBoxHauteurBase.TabIndex = 8;
            this.textBoxHauteurBase.Text = "600";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(212, 227);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(555, 44);
            this.label5.TabIndex = 9;
            this.label5.Text = "note : la taille de la vidéo finale garde la proportion de la source en ayant, po" +
    "ur maximum, soit la hauteur, soit la largeur de base, de façon à ce que aucune d" +
    "es deux valeurs ne soit dépassée";
            // 
            // fontDialog
            // 
            this.fontDialog.AllowVerticalFonts = false;
            this.fontDialog.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fontDialog.ShowColor = true;
            // 
            // buttonChoixPolice
            // 
            this.buttonChoixPolice.Location = new System.Drawing.Point(12, 227);
            this.buttonChoixPolice.Name = "buttonChoixPolice";
            this.buttonChoixPolice.Size = new System.Drawing.Size(178, 20);
            this.buttonChoixPolice.TabIndex = 10;
            this.buttonChoixPolice.Text = "Choix de la Police";
            this.buttonChoixPolice.UseVisualStyleBackColor = true;
            this.buttonChoixPolice.Click += new System.EventHandler(this.buttonChoixPolice_Click);
            // 
            // labelPolice
            // 
            this.labelPolice.Font = new System.Drawing.Font("Gabriola", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPolice.Location = new System.Drawing.Point(15, 271);
            this.labelPolice.Name = "labelPolice";
            this.labelPolice.Size = new System.Drawing.Size(743, 151);
            this.labelPolice.TabIndex = 11;
            this.labelPolice.Text = "Police de caractère";
            // 
            // buttonRepertoireSource
            // 
            this.buttonRepertoireSource.Location = new System.Drawing.Point(15, 77);
            this.buttonRepertoireSource.Name = "buttonRepertoireSource";
            this.buttonRepertoireSource.Size = new System.Drawing.Size(138, 23);
            this.buttonRepertoireSource.TabIndex = 17;
            this.buttonRepertoireSource.Text = "Repertoire des sources";
            this.buttonRepertoireSource.UseVisualStyleBackColor = true;
            this.buttonRepertoireSource.Click += new System.EventHandler(this.buttonRepertoireSource_Click);
            // 
            // buttonRepertoireSortie
            // 
            this.buttonRepertoireSortie.Location = new System.Drawing.Point(15, 139);
            this.buttonRepertoireSortie.Name = "buttonRepertoireSortie";
            this.buttonRepertoireSortie.Size = new System.Drawing.Size(138, 23);
            this.buttonRepertoireSortie.TabIndex = 18;
            this.buttonRepertoireSortie.Text = "Repertoire de sortie";
            this.buttonRepertoireSortie.UseVisualStyleBackColor = true;
            this.buttonRepertoireSortie.Click += new System.EventHandler(this.buttonRepertoireSortie_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(576, 54);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(45, 13);
            this.label6.TabIndex = 20;
            this.label6.Text = "Masque";
            // 
            // textBoxMasque
            // 
            this.textBoxMasque.Location = new System.Drawing.Point(579, 80);
            this.textBoxMasque.Name = "textBoxMasque";
            this.textBoxMasque.Size = new System.Drawing.Size(188, 20);
            this.textBoxMasque.TabIndex = 21;
            this.textBoxMasque.Text = "*.png";
            // 
            // buttonOuvrirFilm
            // 
            this.buttonOuvrirFilm.Enabled = false;
            this.buttonOuvrirFilm.Location = new System.Drawing.Point(602, 478);
            this.buttonOuvrirFilm.Name = "buttonOuvrirFilm";
            this.buttonOuvrirFilm.Size = new System.Drawing.Size(75, 23);
            this.buttonOuvrirFilm.TabIndex = 22;
            this.buttonOuvrirFilm.Tag = "";
            this.buttonOuvrirFilm.Text = "Ouvrir Film";
            this.buttonOuvrirFilm.UseVisualStyleBackColor = true;
            this.buttonOuvrirFilm.Click += new System.EventHandler(this.buttonOuvrirFilm_Click);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(26, 446);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(732, 23);
            this.progressBar.Step = 1;
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar.TabIndex = 25;
            // 
            // backgroundTraitement
            // 
            this.backgroundTraitement.WorkerReportsProgress = true;
            this.backgroundTraitement.WorkerSupportsCancellation = true;
            this.backgroundTraitement.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundTraitement_DoWork);
            this.backgroundTraitement.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundTraitement_ProgressChanged);
            // 
            // checkBoxCarteUnites
            // 
            this.checkBoxCarteUnites.AutoSize = true;
            this.checkBoxCarteUnites.Location = new System.Drawing.Point(31, 482);
            this.checkBoxCarteUnites.Name = "checkBoxCarteUnites";
            this.checkBoxCarteUnites.Size = new System.Drawing.Size(113, 17);
            this.checkBoxCarteUnites.TabIndex = 26;
            this.checkBoxCarteUnites.Text = "cartes avec unités";
            this.checkBoxCarteUnites.UseVisualStyleBackColor = true;
            this.checkBoxCarteUnites.CheckedChanged += new System.EventHandler(this.checkBoxCarteUnites_CheckedChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(9, 189);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(153, 13);
            this.label7.TabIndex = 27;
            this.label7.Text = "Taille des unités (nombre paire)";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(258, 189);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(144, 13);
            this.label8.TabIndex = 28;
            this.label8.Text = "Epaisseur de ligne des unités";
            // 
            // textBoxTailleUnite
            // 
            this.textBoxTailleUnite.Location = new System.Drawing.Point(163, 186);
            this.textBoxTailleUnite.Name = "textBoxTailleUnite";
            this.textBoxTailleUnite.Size = new System.Drawing.Size(67, 20);
            this.textBoxTailleUnite.TabIndex = 29;
            this.textBoxTailleUnite.Text = "18";
            // 
            // textBoxEpaisseurUnite
            // 
            this.textBoxEpaisseurUnite.Location = new System.Drawing.Point(408, 186);
            this.textBoxEpaisseurUnite.Name = "textBoxEpaisseurUnite";
            this.textBoxEpaisseurUnite.Size = new System.Drawing.Size(67, 20);
            this.textBoxEpaisseurUnite.TabIndex = 30;
            this.textBoxEpaisseurUnite.Text = "1";
            // 
            // openFileDialog
            // 
            this.openFileDialog.DefaultExt = "vaoc";
            this.openFileDialog.Filter = "fichiers VAOC |*.vaoc";
            this.openFileDialog.Title = "Ouvrir un fichier VAOC";
            // 
            // buttonChargerPartie
            // 
            this.buttonChargerPartie.Location = new System.Drawing.Point(15, 12);
            this.buttonChargerPartie.Name = "buttonChargerPartie";
            this.buttonChargerPartie.Size = new System.Drawing.Size(138, 23);
            this.buttonChargerPartie.TabIndex = 31;
            this.buttonChargerPartie.Text = "Charger une partie";
            this.buttonChargerPartie.UseVisualStyleBackColor = true;
            this.buttonChargerPartie.Click += new System.EventHandler(this.buttonChargerPartie_Click);
            // 
            // textBoxNomPartie
            // 
            this.textBoxNomPartie.Enabled = false;
            this.textBoxNomPartie.Location = new System.Drawing.Point(163, 15);
            this.textBoxNomPartie.Name = "textBoxNomPartie";
            this.textBoxNomPartie.ReadOnly = true;
            this.textBoxNomPartie.Size = new System.Drawing.Size(604, 20);
            this.textBoxNomPartie.TabIndex = 32;
            this.textBoxNomPartie.Text = "non définie";
            // 
            // buttonDonnees
            // 
            this.buttonDonnees.Enabled = false;
            this.buttonDonnees.Location = new System.Drawing.Point(683, 478);
            this.buttonDonnees.Name = "buttonDonnees";
            this.buttonDonnees.Size = new System.Drawing.Size(75, 23);
            this.buttonDonnees.TabIndex = 33;
            this.buttonDonnees.Tag = "";
            this.buttonDonnees.Text = "Donnees";
            this.buttonDonnees.UseVisualStyleBackColor = true;
            this.buttonDonnees.Click += new System.EventHandler(this.buttonDonnees_Click);
            // 
            // checkBoxCarteGlobale
            // 
            this.checkBoxCarteGlobale.AutoSize = true;
            this.checkBoxCarteGlobale.Location = new System.Drawing.Point(163, 482);
            this.checkBoxCarteGlobale.Name = "checkBoxCarteGlobale";
            this.checkBoxCarteGlobale.Size = new System.Drawing.Size(90, 17);
            this.checkBoxCarteGlobale.TabIndex = 34;
            this.checkBoxCarteGlobale.Text = "Carte Globale";
            this.checkBoxCarteGlobale.UseVisualStyleBackColor = true;
            // 
            // checkBoxFilm
            // 
            this.checkBoxFilm.AutoSize = true;
            this.checkBoxFilm.Location = new System.Drawing.Point(260, 482);
            this.checkBoxFilm.Name = "checkBoxFilm";
            this.checkBoxFilm.Size = new System.Drawing.Size(44, 17);
            this.checkBoxFilm.TabIndex = 35;
            this.checkBoxFilm.Text = "Film";
            this.checkBoxFilm.UseVisualStyleBackColor = true;
            this.checkBoxFilm.CheckedChanged += new System.EventHandler(this.checkBoxFilm_CheckedChanged);
            // 
            // FormVideo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(779, 517);
            this.Controls.Add(this.checkBoxFilm);
            this.Controls.Add(this.checkBoxCarteGlobale);
            this.Controls.Add(this.buttonDonnees);
            this.Controls.Add(this.textBoxNomPartie);
            this.Controls.Add(this.buttonChargerPartie);
            this.Controls.Add(this.textBoxEpaisseurUnite);
            this.Controls.Add(this.textBoxTailleUnite);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.checkBoxCarteUnites);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.buttonOuvrirFilm);
            this.Controls.Add(this.textBoxMasque);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.buttonRepertoireSortie);
            this.Controls.Add(this.buttonRepertoireSource);
            this.Controls.Add(this.labelPolice);
            this.Controls.Add(this.buttonChoixPolice);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBoxHauteurBase);
            this.Controls.Add(this.textBoxLargeurBase);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxRepertoireVideo);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxRepertoireImages);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonCreerFilm);
            this.Name = "FormVideo";
            this.Text = "Création d\'une vidéo à partir d\'images";
            this.Load += new System.EventHandler(this.FormVideo_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonCreerFilm;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxRepertoireImages;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxRepertoireVideo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxLargeurBase;
        private System.Windows.Forms.TextBox textBoxHauteurBase;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.FontDialog fontDialog;
        private System.Windows.Forms.Button buttonChoixPolice;
        private System.Windows.Forms.Label labelPolice;
        private System.Windows.Forms.Button buttonRepertoireSource;
        private System.Windows.Forms.Button buttonRepertoireSortie;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxMasque;
        private System.Windows.Forms.Button buttonOuvrirFilm;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.ComponentModel.BackgroundWorker backgroundTraitement;
        private System.Windows.Forms.CheckBox checkBoxCarteUnites;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBoxTailleUnite;
        private System.Windows.Forms.TextBox textBoxEpaisseurUnite;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Button buttonChargerPartie;
        private System.Windows.Forms.TextBox textBoxNomPartie;
        private System.Windows.Forms.Button buttonDonnees;
        private System.Windows.Forms.CheckBox checkBoxCarteGlobale;
        private System.Windows.Forms.CheckBox checkBoxFilm;
    }
}

