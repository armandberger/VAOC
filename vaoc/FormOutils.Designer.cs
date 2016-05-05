namespace vaoc
{
    partial class FormOutils
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
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonImage = new System.Windows.Forms.Button();
            this.textBoxFichierImage = new System.Windows.Forms.TextBox();
            this.buttonTraitement = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.buttonDecoupe = new System.Windows.Forms.Button();
            this.buttonZoom = new System.Windows.Forms.Button();
            this.textBoxFichierImageFond = new System.Windows.Forms.TextBox();
            this.buttonImageFond = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxLargeur = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxHauteur = new System.Windows.Forms.TextBox();
            this.groupBoxImages = new System.Windows.Forms.GroupBox();
            this.groupBoxSQL = new System.Windows.Forms.GroupBox();
            this.textBoxRepertoireSources = new System.Windows.Forms.TextBox();
            this.buttonRepertoireSource = new System.Windows.Forms.Button();
            this.buttonMinMaj = new System.Windows.Forms.Button();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.groupBoxImport = new System.Windows.Forms.GroupBox();
            this.listBoxImport = new System.Windows.Forms.ListBox();
            this.textBoxChoixFichierImport = new System.Windows.Forms.TextBox();
            this.buttonChoixFichierImport = new System.Windows.Forms.Button();
            this.buttonImport = new System.Windows.Forms.Button();
            this.buttonCoupe = new System.Windows.Forms.Button();
            this.buttonFusion = new System.Windows.Forms.Button();
            this.textBoxNbCoteCarre = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBoxImages.SuspendLayout();
            this.groupBoxSQL.SuspendLayout();
            this.groupBoxImport.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(295, 567);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(60, 27);
            this.buttonOK.TabIndex = 3;
            this.buttonOK.Text = "Fermer";
            this.buttonOK.UseVisualStyleBackColor = true;
            // 
            // buttonImage
            // 
            this.buttonImage.Location = new System.Drawing.Point(17, 29);
            this.buttonImage.Name = "buttonImage";
            this.buttonImage.Size = new System.Drawing.Size(138, 23);
            this.buttonImage.TabIndex = 4;
            this.buttonImage.Text = "Ouvrir Image";
            this.buttonImage.UseVisualStyleBackColor = true;
            this.buttonImage.Click += new System.EventHandler(this.buttonImage_Click);
            // 
            // textBoxFichierImage
            // 
            this.textBoxFichierImage.Location = new System.Drawing.Point(182, 31);
            this.textBoxFichierImage.Name = "textBoxFichierImage";
            this.textBoxFichierImage.Size = new System.Drawing.Size(449, 20);
            this.textBoxFichierImage.TabIndex = 5;
            // 
            // buttonTraitement
            // 
            this.buttonTraitement.Location = new System.Drawing.Point(17, 108);
            this.buttonTraitement.Name = "buttonTraitement";
            this.buttonTraitement.Size = new System.Drawing.Size(75, 23);
            this.buttonTraitement.TabIndex = 6;
            this.buttonTraitement.Text = "Traitement";
            this.buttonTraitement.UseVisualStyleBackColor = true;
            this.buttonTraitement.Click += new System.EventHandler(this.buttonTraitement_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "JPEG|*.jpg|BMP|*.bmp|PNG|*.png|tous|*.*";
            // 
            // buttonDecoupe
            // 
            this.buttonDecoupe.Location = new System.Drawing.Point(17, 137);
            this.buttonDecoupe.Name = "buttonDecoupe";
            this.buttonDecoupe.Size = new System.Drawing.Size(75, 23);
            this.buttonDecoupe.TabIndex = 7;
            this.buttonDecoupe.Text = "Decoupe";
            this.buttonDecoupe.UseVisualStyleBackColor = true;
            this.buttonDecoupe.Click += new System.EventHandler(this.buttonDecoupe_Click);
            // 
            // buttonZoom
            // 
            this.buttonZoom.Location = new System.Drawing.Point(17, 166);
            this.buttonZoom.Name = "buttonZoom";
            this.buttonZoom.Size = new System.Drawing.Size(75, 23);
            this.buttonZoom.TabIndex = 8;
            this.buttonZoom.Text = "ZoomGris";
            this.buttonZoom.UseVisualStyleBackColor = true;
            this.buttonZoom.Click += new System.EventHandler(this.buttonZoom_Click);
            // 
            // textBoxFichierImageFond
            // 
            this.textBoxFichierImageFond.Location = new System.Drawing.Point(182, 70);
            this.textBoxFichierImageFond.Name = "textBoxFichierImageFond";
            this.textBoxFichierImageFond.Size = new System.Drawing.Size(449, 20);
            this.textBoxFichierImageFond.TabIndex = 10;
            // 
            // buttonImageFond
            // 
            this.buttonImageFond.Location = new System.Drawing.Point(17, 68);
            this.buttonImageFond.Name = "buttonImageFond";
            this.buttonImageFond.Size = new System.Drawing.Size(138, 23);
            this.buttonImageFond.TabIndex = 9;
            this.buttonImageFond.Text = "Ouvrir Image Fond";
            this.buttonImageFond.UseVisualStyleBackColor = true;
            this.buttonImageFond.Click += new System.EventHandler(this.buttonImageFond_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(148, 146);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Largeur";
            // 
            // textBoxLargeur
            // 
            this.textBoxLargeur.Location = new System.Drawing.Point(198, 146);
            this.textBoxLargeur.Name = "textBoxLargeur";
            this.textBoxLargeur.Size = new System.Drawing.Size(100, 20);
            this.textBoxLargeur.TabIndex = 12;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(315, 152);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "Hauteur";
            // 
            // textBoxHauteur
            // 
            this.textBoxHauteur.Location = new System.Drawing.Point(377, 146);
            this.textBoxHauteur.Name = "textBoxHauteur";
            this.textBoxHauteur.Size = new System.Drawing.Size(100, 20);
            this.textBoxHauteur.TabIndex = 14;
            // 
            // groupBoxImages
            // 
            this.groupBoxImages.Controls.Add(this.textBoxNbCoteCarre);
            this.groupBoxImages.Controls.Add(this.label3);
            this.groupBoxImages.Controls.Add(this.buttonFusion);
            this.groupBoxImages.Controls.Add(this.buttonCoupe);
            this.groupBoxImages.Controls.Add(this.textBoxHauteur);
            this.groupBoxImages.Controls.Add(this.label2);
            this.groupBoxImages.Controls.Add(this.textBoxLargeur);
            this.groupBoxImages.Controls.Add(this.label1);
            this.groupBoxImages.Controls.Add(this.textBoxFichierImageFond);
            this.groupBoxImages.Controls.Add(this.buttonImageFond);
            this.groupBoxImages.Controls.Add(this.buttonZoom);
            this.groupBoxImages.Controls.Add(this.buttonDecoupe);
            this.groupBoxImages.Controls.Add(this.buttonTraitement);
            this.groupBoxImages.Controls.Add(this.textBoxFichierImage);
            this.groupBoxImages.Controls.Add(this.buttonImage);
            this.groupBoxImages.Location = new System.Drawing.Point(7, 12);
            this.groupBoxImages.Name = "groupBoxImages";
            this.groupBoxImages.Size = new System.Drawing.Size(639, 260);
            this.groupBoxImages.TabIndex = 15;
            this.groupBoxImages.TabStop = false;
            this.groupBoxImages.Text = "Traitment d\'images";
            // 
            // groupBoxSQL
            // 
            this.groupBoxSQL.Controls.Add(this.textBoxRepertoireSources);
            this.groupBoxSQL.Controls.Add(this.buttonRepertoireSource);
            this.groupBoxSQL.Controls.Add(this.buttonMinMaj);
            this.groupBoxSQL.Location = new System.Drawing.Point(7, 278);
            this.groupBoxSQL.Name = "groupBoxSQL";
            this.groupBoxSQL.Size = new System.Drawing.Size(638, 96);
            this.groupBoxSQL.TabIndex = 16;
            this.groupBoxSQL.TabStop = false;
            this.groupBoxSQL.Text = "Traitements bases";
            // 
            // textBoxRepertoireSources
            // 
            this.textBoxRepertoireSources.Location = new System.Drawing.Point(182, 22);
            this.textBoxRepertoireSources.Name = "textBoxRepertoireSources";
            this.textBoxRepertoireSources.Size = new System.Drawing.Size(449, 20);
            this.textBoxRepertoireSources.TabIndex = 15;
            this.textBoxRepertoireSources.Text = "C:\\Projects\\PHP\\vaoc";
            // 
            // buttonRepertoireSource
            // 
            this.buttonRepertoireSource.Location = new System.Drawing.Point(17, 19);
            this.buttonRepertoireSource.Name = "buttonRepertoireSource";
            this.buttonRepertoireSource.Size = new System.Drawing.Size(138, 23);
            this.buttonRepertoireSource.TabIndex = 16;
            this.buttonRepertoireSource.Text = "Repertoire des sources";
            this.buttonRepertoireSource.UseVisualStyleBackColor = true;
            this.buttonRepertoireSource.Click += new System.EventHandler(this.buttonRepertoireSource_Click);
            // 
            // buttonMinMaj
            // 
            this.buttonMinMaj.Location = new System.Drawing.Point(17, 62);
            this.buttonMinMaj.Name = "buttonMinMaj";
            this.buttonMinMaj.Size = new System.Drawing.Size(138, 23);
            this.buttonMinMaj.TabIndex = 15;
            this.buttonMinMaj.Text = "Tables en minuscules";
            this.buttonMinMaj.UseVisualStyleBackColor = true;
            this.buttonMinMaj.Click += new System.EventHandler(this.buttonMinMaj_Click);
            // 
            // groupBoxImport
            // 
            this.groupBoxImport.Controls.Add(this.listBoxImport);
            this.groupBoxImport.Controls.Add(this.textBoxChoixFichierImport);
            this.groupBoxImport.Controls.Add(this.buttonChoixFichierImport);
            this.groupBoxImport.Controls.Add(this.buttonImport);
            this.groupBoxImport.Location = new System.Drawing.Point(8, 380);
            this.groupBoxImport.Name = "groupBoxImport";
            this.groupBoxImport.Size = new System.Drawing.Size(638, 181);
            this.groupBoxImport.TabIndex = 17;
            this.groupBoxImport.TabStop = false;
            this.groupBoxImport.Text = "Import de tables d\'un autre fichier vaoc";
            // 
            // listBoxImport
            // 
            this.listBoxImport.FormattingEnabled = true;
            this.listBoxImport.Location = new System.Drawing.Point(182, 62);
            this.listBoxImport.Name = "listBoxImport";
            this.listBoxImport.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.listBoxImport.Size = new System.Drawing.Size(448, 108);
            this.listBoxImport.Sorted = true;
            this.listBoxImport.TabIndex = 17;
            // 
            // textBoxChoixFichierImport
            // 
            this.textBoxChoixFichierImport.Location = new System.Drawing.Point(182, 22);
            this.textBoxChoixFichierImport.Name = "textBoxChoixFichierImport";
            this.textBoxChoixFichierImport.ReadOnly = true;
            this.textBoxChoixFichierImport.Size = new System.Drawing.Size(449, 20);
            this.textBoxChoixFichierImport.TabIndex = 15;
            // 
            // buttonChoixFichierImport
            // 
            this.buttonChoixFichierImport.Location = new System.Drawing.Point(17, 19);
            this.buttonChoixFichierImport.Name = "buttonChoixFichierImport";
            this.buttonChoixFichierImport.Size = new System.Drawing.Size(138, 23);
            this.buttonChoixFichierImport.TabIndex = 16;
            this.buttonChoixFichierImport.Text = "Fichier source";
            this.buttonChoixFichierImport.UseVisualStyleBackColor = true;
            this.buttonChoixFichierImport.Click += new System.EventHandler(this.buttonChoixFichierImport_Click);
            // 
            // buttonImport
            // 
            this.buttonImport.Location = new System.Drawing.Point(17, 62);
            this.buttonImport.Name = "buttonImport";
            this.buttonImport.Size = new System.Drawing.Size(138, 23);
            this.buttonImport.TabIndex = 15;
            this.buttonImport.Text = "Importer";
            this.buttonImport.UseVisualStyleBackColor = true;
            this.buttonImport.Click += new System.EventHandler(this.buttonImport_Click);
            // 
            // buttonCoupe
            // 
            this.buttonCoupe.Location = new System.Drawing.Point(18, 222);
            this.buttonCoupe.Name = "buttonCoupe";
            this.buttonCoupe.Size = new System.Drawing.Size(87, 23);
            this.buttonCoupe.TabIndex = 15;
            this.buttonCoupe.Text = "Coupe carree";
            this.buttonCoupe.UseVisualStyleBackColor = true;
            this.buttonCoupe.Click += new System.EventHandler(this.buttonCoupe_Click);
            // 
            // buttonFusion
            // 
            this.buttonFusion.Location = new System.Drawing.Point(116, 222);
            this.buttonFusion.Name = "buttonFusion";
            this.buttonFusion.Size = new System.Drawing.Size(75, 23);
            this.buttonFusion.TabIndex = 16;
            this.buttonFusion.Text = "Fusion";
            this.buttonFusion.UseVisualStyleBackColor = true;
            this.buttonFusion.Click += new System.EventHandler(this.buttonFusion_Click);
            // 
            // textBoxNbCoteCarre
            // 
            this.textBoxNbCoteCarre.Location = new System.Drawing.Point(428, 224);
            this.textBoxNbCoteCarre.Name = "textBoxNbCoteCarre";
            this.textBoxNbCoteCarre.Size = new System.Drawing.Size(64, 20);
            this.textBoxNbCoteCarre.TabIndex = 18;
            this.textBoxNbCoteCarre.Text = "3";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(210, 222);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(212, 13);
            this.label3.TabIndex = 17;
            this.label3.Text = "Nombre de cotés du carré (ex: 3 pour 3x3) :";
            // 
            // FormOutils
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(651, 598);
            this.Controls.Add(this.groupBoxImport);
            this.Controls.Add(this.groupBoxSQL);
            this.Controls.Add(this.groupBoxImages);
            this.Controls.Add(this.buttonOK);
            this.Name = "FormOutils";
            this.Text = "FormOutils";
            this.groupBoxImages.ResumeLayout(false);
            this.groupBoxImages.PerformLayout();
            this.groupBoxSQL.ResumeLayout(false);
            this.groupBoxSQL.PerformLayout();
            this.groupBoxImport.ResumeLayout(false);
            this.groupBoxImport.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonImage;
        private System.Windows.Forms.TextBox textBoxFichierImage;
        private System.Windows.Forms.Button buttonTraitement;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Button buttonDecoupe;
        private System.Windows.Forms.Button buttonZoom;
        private System.Windows.Forms.TextBox textBoxFichierImageFond;
        private System.Windows.Forms.Button buttonImageFond;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxLargeur;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxHauteur;
        private System.Windows.Forms.GroupBox groupBoxImages;
        private System.Windows.Forms.GroupBox groupBoxSQL;
        private System.Windows.Forms.TextBox textBoxRepertoireSources;
        private System.Windows.Forms.Button buttonRepertoireSource;
        private System.Windows.Forms.Button buttonMinMaj;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.GroupBox groupBoxImport;
        private System.Windows.Forms.TextBox textBoxChoixFichierImport;
        private System.Windows.Forms.Button buttonChoixFichierImport;
        private System.Windows.Forms.Button buttonImport;
        private System.Windows.Forms.ListBox listBoxImport;
        private System.Windows.Forms.TextBox textBoxNbCoteCarre;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonFusion;
        private System.Windows.Forms.Button buttonCoupe;
    }
}