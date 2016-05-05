namespace vaoc
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
            this.buttonCreerFilmHistorique = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxMasque = new System.Windows.Forms.TextBox();
            this.buttonOuvrirFilm = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonCreerFilm
            // 
            this.buttonCreerFilm.Location = new System.Drawing.Point(195, 343);
            this.buttonCreerFilm.Name = "buttonCreerFilm";
            this.buttonCreerFilm.Size = new System.Drawing.Size(75, 23);
            this.buttonCreerFilm.TabIndex = 0;
            this.buttonCreerFilm.Text = "Creer Film";
            this.buttonCreerFilm.UseVisualStyleBackColor = true;
            this.buttonCreerFilm.Click += new System.EventHandler(this.buttonCreerFilm_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(112, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Repertoire des images";
            // 
            // textBoxRepertoireImages
            // 
            this.textBoxRepertoireImages.Location = new System.Drawing.Point(163, 35);
            this.textBoxRepertoireImages.Name = "textBoxRepertoireImages";
            this.textBoxRepertoireImages.Size = new System.Drawing.Size(401, 20);
            this.textBoxRepertoireImages.TabIndex = 2;
            this.textBoxRepertoireImages.Text = "C:\\personnel\\Test complet\\video";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 69);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(99, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Repertoire de sortie";
            // 
            // textBoxRepertoireVideo
            // 
            this.textBoxRepertoireVideo.Location = new System.Drawing.Point(163, 97);
            this.textBoxRepertoireVideo.Name = "textBoxRepertoireVideo";
            this.textBoxRepertoireVideo.Size = new System.Drawing.Size(401, 20);
            this.textBoxRepertoireVideo.TabIndex = 4;
            this.textBoxRepertoireVideo.Text = "C:\\";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(576, 69);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Largeur";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(688, 69);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(45, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Hauteur";
            // 
            // textBoxLargeurBase
            // 
            this.textBoxLargeurBase.Location = new System.Drawing.Point(579, 97);
            this.textBoxLargeurBase.Name = "textBoxLargeurBase";
            this.textBoxLargeurBase.Size = new System.Drawing.Size(67, 20);
            this.textBoxLargeurBase.TabIndex = 7;
            this.textBoxLargeurBase.Text = "800";
            // 
            // textBoxHauteurBase
            // 
            this.textBoxHauteurBase.Location = new System.Drawing.Point(691, 97);
            this.textBoxHauteurBase.Name = "textBoxHauteurBase";
            this.textBoxHauteurBase.Size = new System.Drawing.Size(67, 20);
            this.textBoxHauteurBase.TabIndex = 8;
            this.textBoxHauteurBase.Text = "600";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(212, 136);
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
            this.buttonChoixPolice.Location = new System.Drawing.Point(12, 136);
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
            this.labelPolice.Location = new System.Drawing.Point(15, 180);
            this.labelPolice.Name = "labelPolice";
            this.labelPolice.Size = new System.Drawing.Size(743, 151);
            this.labelPolice.TabIndex = 11;
            // 
            // buttonRepertoireSource
            // 
            this.buttonRepertoireSource.Location = new System.Drawing.Point(15, 32);
            this.buttonRepertoireSource.Name = "buttonRepertoireSource";
            this.buttonRepertoireSource.Size = new System.Drawing.Size(138, 23);
            this.buttonRepertoireSource.TabIndex = 17;
            this.buttonRepertoireSource.Text = "Repertoire des sources";
            this.buttonRepertoireSource.UseVisualStyleBackColor = true;
            this.buttonRepertoireSource.Click += new System.EventHandler(this.buttonRepertoireSource_Click);
            // 
            // buttonRepertoireSortie
            // 
            this.buttonRepertoireSortie.Location = new System.Drawing.Point(15, 94);
            this.buttonRepertoireSortie.Name = "buttonRepertoireSortie";
            this.buttonRepertoireSortie.Size = new System.Drawing.Size(138, 23);
            this.buttonRepertoireSortie.TabIndex = 18;
            this.buttonRepertoireSortie.Text = "Repertoire de sortie";
            this.buttonRepertoireSortie.UseVisualStyleBackColor = true;
            this.buttonRepertoireSortie.Click += new System.EventHandler(this.buttonRepertoireSortie_Click);
            // 
            // buttonCreerFilmHistorique
            // 
            this.buttonCreerFilmHistorique.Location = new System.Drawing.Point(329, 343);
            this.buttonCreerFilmHistorique.Name = "buttonCreerFilmHistorique";
            this.buttonCreerFilmHistorique.Size = new System.Drawing.Size(120, 23);
            this.buttonCreerFilmHistorique.TabIndex = 19;
            this.buttonCreerFilmHistorique.Text = "Creer Film Historique";
            this.buttonCreerFilmHistorique.UseVisualStyleBackColor = true;
            this.buttonCreerFilmHistorique.Click += new System.EventHandler(this.buttonCreerFilmHistorique_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(576, 9);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(45, 13);
            this.label6.TabIndex = 20;
            this.label6.Text = "Masque";
            // 
            // textBoxMasque
            // 
            this.textBoxMasque.Location = new System.Drawing.Point(579, 35);
            this.textBoxMasque.Name = "textBoxMasque";
            this.textBoxMasque.Size = new System.Drawing.Size(188, 20);
            this.textBoxMasque.TabIndex = 21;
            this.textBoxMasque.Text = "*.png";
            // 
            // buttonOuvrirFilm
            // 
            this.buttonOuvrirFilm.Enabled = false;
            this.buttonOuvrirFilm.Location = new System.Drawing.Point(508, 343);
            this.buttonOuvrirFilm.Name = "buttonOuvrirFilm";
            this.buttonOuvrirFilm.Size = new System.Drawing.Size(75, 23);
            this.buttonOuvrirFilm.TabIndex = 22;
            this.buttonOuvrirFilm.Text = "Ouvrir Film";
            this.buttonOuvrirFilm.UseVisualStyleBackColor = true;
            this.buttonOuvrirFilm.Click += new System.EventHandler(this.buttonOuvrirFilm_Click);
            // 
            // FormVideo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(779, 378);
            this.Controls.Add(this.buttonOuvrirFilm);
            this.Controls.Add(this.textBoxMasque);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.buttonCreerFilmHistorique);
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
        private System.Windows.Forms.Button buttonCreerFilmHistorique;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxMasque;
        private System.Windows.Forms.Button buttonOuvrirFilm;
    }
}

