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
            buttonOK = new System.Windows.Forms.Button();
            buttonImage = new System.Windows.Forms.Button();
            textBoxFichierImage = new System.Windows.Forms.TextBox();
            buttonRetraitCouleur = new System.Windows.Forms.Button();
            openFileDialog = new System.Windows.Forms.OpenFileDialog();
            buttonDecoupe = new System.Windows.Forms.Button();
            buttonZoom = new System.Windows.Forms.Button();
            textBoxFichierImageFond = new System.Windows.Forms.TextBox();
            buttonImageFond = new System.Windows.Forms.Button();
            label1 = new System.Windows.Forms.Label();
            textBoxLargeur = new System.Windows.Forms.TextBox();
            label2 = new System.Windows.Forms.Label();
            textBoxHauteur = new System.Windows.Forms.TextBox();
            groupBoxImages = new System.Windows.Forms.GroupBox();
            buttonMosaique = new System.Windows.Forms.Button();
            textBoxNbCoteCarre = new System.Windows.Forms.TextBox();
            label3 = new System.Windows.Forms.Label();
            buttonFusion = new System.Windows.Forms.Button();
            buttonCoupe = new System.Windows.Forms.Button();
            groupBoxSQL = new System.Windows.Forms.GroupBox();
            textBoxRepertoireSources = new System.Windows.Forms.TextBox();
            buttonRepertoireSource = new System.Windows.Forms.Button();
            buttonMinMaj = new System.Windows.Forms.Button();
            folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            groupBoxImport = new System.Windows.Forms.GroupBox();
            listBoxImport = new System.Windows.Forms.ListBox();
            textBoxChoixFichierImport = new System.Windows.Forms.TextBox();
            buttonChoixFichierImport = new System.Windows.Forms.Button();
            buttonImport = new System.Windows.Forms.Button();
            groupBoxImages.SuspendLayout();
            groupBoxSQL.SuspendLayout();
            groupBoxImport.SuspendLayout();
            SuspendLayout();
            // 
            // buttonOK
            // 
            buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            buttonOK.Location = new System.Drawing.Point(344, 654);
            buttonOK.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            buttonOK.Name = "buttonOK";
            buttonOK.Size = new System.Drawing.Size(70, 31);
            buttonOK.TabIndex = 3;
            buttonOK.Text = "Fermer";
            buttonOK.UseVisualStyleBackColor = true;
            // 
            // buttonImage
            // 
            buttonImage.Location = new System.Drawing.Point(20, 33);
            buttonImage.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            buttonImage.Name = "buttonImage";
            buttonImage.Size = new System.Drawing.Size(161, 27);
            buttonImage.TabIndex = 4;
            buttonImage.Text = "Ouvrir Image";
            buttonImage.UseVisualStyleBackColor = true;
            buttonImage.Click += buttonImage_Click;
            // 
            // textBoxFichierImage
            // 
            textBoxFichierImage.Location = new System.Drawing.Point(212, 36);
            textBoxFichierImage.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            textBoxFichierImage.Name = "textBoxFichierImage";
            textBoxFichierImage.Size = new System.Drawing.Size(523, 23);
            textBoxFichierImage.TabIndex = 5;
            // 
            // buttonRetraitCouleur
            // 
            buttonRetraitCouleur.Location = new System.Drawing.Point(20, 125);
            buttonRetraitCouleur.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            buttonRetraitCouleur.Name = "buttonRetraitCouleur";
            buttonRetraitCouleur.Size = new System.Drawing.Size(161, 27);
            buttonRetraitCouleur.TabIndex = 6;
            buttonRetraitCouleur.Text = "Retrait des couleurs";
            buttonRetraitCouleur.UseVisualStyleBackColor = true;
            buttonRetraitCouleur.Click += buttonRetraitCouleur_Click;
            // 
            // openFileDialog
            // 
            openFileDialog.Filter = "JPEG|*.jpg|BMP|*.bmp|PNG|*.png|tous|*.*";
            // 
            // buttonDecoupe
            // 
            buttonDecoupe.Location = new System.Drawing.Point(20, 158);
            buttonDecoupe.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            buttonDecoupe.Name = "buttonDecoupe";
            buttonDecoupe.Size = new System.Drawing.Size(88, 27);
            buttonDecoupe.TabIndex = 7;
            buttonDecoupe.Text = "Decoupe";
            buttonDecoupe.UseVisualStyleBackColor = true;
            buttonDecoupe.Click += buttonDecoupe_Click;
            // 
            // buttonZoom
            // 
            buttonZoom.Location = new System.Drawing.Point(20, 192);
            buttonZoom.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            buttonZoom.Name = "buttonZoom";
            buttonZoom.Size = new System.Drawing.Size(88, 27);
            buttonZoom.TabIndex = 8;
            buttonZoom.Text = "ZoomGris";
            buttonZoom.UseVisualStyleBackColor = true;
            buttonZoom.Click += buttonZoom_Click;
            // 
            // textBoxFichierImageFond
            // 
            textBoxFichierImageFond.Location = new System.Drawing.Point(212, 81);
            textBoxFichierImageFond.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            textBoxFichierImageFond.Name = "textBoxFichierImageFond";
            textBoxFichierImageFond.Size = new System.Drawing.Size(523, 23);
            textBoxFichierImageFond.TabIndex = 10;
            // 
            // buttonImageFond
            // 
            buttonImageFond.Location = new System.Drawing.Point(20, 78);
            buttonImageFond.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            buttonImageFond.Name = "buttonImageFond";
            buttonImageFond.Size = new System.Drawing.Size(161, 27);
            buttonImageFond.TabIndex = 9;
            buttonImageFond.Text = "Ouvrir Image Fond";
            buttonImageFond.UseVisualStyleBackColor = true;
            buttonImageFond.Click += buttonImageFond_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(173, 168);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(47, 15);
            label1.TabIndex = 11;
            label1.Text = "Largeur";
            // 
            // textBoxLargeur
            // 
            textBoxLargeur.Location = new System.Drawing.Point(231, 168);
            textBoxLargeur.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            textBoxLargeur.Name = "textBoxLargeur";
            textBoxLargeur.Size = new System.Drawing.Size(116, 23);
            textBoxLargeur.TabIndex = 12;
            textBoxLargeur.Text = "760";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(368, 175);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(50, 15);
            label2.TabIndex = 13;
            label2.Text = "Hauteur";
            // 
            // textBoxHauteur
            // 
            textBoxHauteur.Location = new System.Drawing.Point(440, 168);
            textBoxHauteur.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            textBoxHauteur.Name = "textBoxHauteur";
            textBoxHauteur.Size = new System.Drawing.Size(116, 23);
            textBoxHauteur.TabIndex = 14;
            textBoxHauteur.Text = "1344";
            // 
            // groupBoxImages
            // 
            groupBoxImages.Controls.Add(buttonMosaique);
            groupBoxImages.Controls.Add(textBoxNbCoteCarre);
            groupBoxImages.Controls.Add(label3);
            groupBoxImages.Controls.Add(buttonFusion);
            groupBoxImages.Controls.Add(buttonCoupe);
            groupBoxImages.Controls.Add(textBoxHauteur);
            groupBoxImages.Controls.Add(label2);
            groupBoxImages.Controls.Add(textBoxLargeur);
            groupBoxImages.Controls.Add(label1);
            groupBoxImages.Controls.Add(textBoxFichierImageFond);
            groupBoxImages.Controls.Add(buttonImageFond);
            groupBoxImages.Controls.Add(buttonZoom);
            groupBoxImages.Controls.Add(buttonDecoupe);
            groupBoxImages.Controls.Add(buttonRetraitCouleur);
            groupBoxImages.Controls.Add(textBoxFichierImage);
            groupBoxImages.Controls.Add(buttonImage);
            groupBoxImages.Location = new System.Drawing.Point(8, 14);
            groupBoxImages.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBoxImages.Name = "groupBoxImages";
            groupBoxImages.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBoxImages.Size = new System.Drawing.Size(746, 300);
            groupBoxImages.TabIndex = 15;
            groupBoxImages.TabStop = false;
            groupBoxImages.Text = "Traitment d'images";
            // 
            // buttonMosaique
            // 
            buttonMosaique.Location = new System.Drawing.Point(21, 225);
            buttonMosaique.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            buttonMosaique.Name = "buttonMosaique";
            buttonMosaique.Size = new System.Drawing.Size(88, 27);
            buttonMosaique.TabIndex = 19;
            buttonMosaique.Text = "Mosaïque";
            buttonMosaique.UseVisualStyleBackColor = true;
            buttonMosaique.Click += buttonMosaique_Click;
            // 
            // textBoxNbCoteCarre
            // 
            textBoxNbCoteCarre.Location = new System.Drawing.Point(499, 258);
            textBoxNbCoteCarre.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            textBoxNbCoteCarre.Name = "textBoxNbCoteCarre";
            textBoxNbCoteCarre.Size = new System.Drawing.Size(74, 23);
            textBoxNbCoteCarre.TabIndex = 18;
            textBoxNbCoteCarre.Text = "3";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(245, 256);
            label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(234, 15);
            label3.TabIndex = 17;
            label3.Text = "Nombre de cotés du carré (ex: 3 pour 3x3) :";
            // 
            // buttonFusion
            // 
            buttonFusion.Location = new System.Drawing.Point(135, 256);
            buttonFusion.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            buttonFusion.Name = "buttonFusion";
            buttonFusion.Size = new System.Drawing.Size(88, 27);
            buttonFusion.TabIndex = 16;
            buttonFusion.Text = "Fusion";
            buttonFusion.UseVisualStyleBackColor = true;
            buttonFusion.Click += buttonFusion_Click;
            // 
            // buttonCoupe
            // 
            buttonCoupe.Location = new System.Drawing.Point(21, 256);
            buttonCoupe.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            buttonCoupe.Name = "buttonCoupe";
            buttonCoupe.Size = new System.Drawing.Size(102, 27);
            buttonCoupe.TabIndex = 15;
            buttonCoupe.Text = "Coupe carree";
            buttonCoupe.UseVisualStyleBackColor = true;
            buttonCoupe.Click += buttonCoupe_Click;
            // 
            // groupBoxSQL
            // 
            groupBoxSQL.Controls.Add(textBoxRepertoireSources);
            groupBoxSQL.Controls.Add(buttonRepertoireSource);
            groupBoxSQL.Controls.Add(buttonMinMaj);
            groupBoxSQL.Location = new System.Drawing.Point(8, 321);
            groupBoxSQL.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBoxSQL.Name = "groupBoxSQL";
            groupBoxSQL.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBoxSQL.Size = new System.Drawing.Size(744, 111);
            groupBoxSQL.TabIndex = 16;
            groupBoxSQL.TabStop = false;
            groupBoxSQL.Text = "Traitements bases";
            // 
            // textBoxRepertoireSources
            // 
            textBoxRepertoireSources.Location = new System.Drawing.Point(212, 25);
            textBoxRepertoireSources.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            textBoxRepertoireSources.Name = "textBoxRepertoireSources";
            textBoxRepertoireSources.Size = new System.Drawing.Size(523, 23);
            textBoxRepertoireSources.TabIndex = 15;
            textBoxRepertoireSources.Text = "C:\\Projects\\PHP\\vaoc";
            // 
            // buttonRepertoireSource
            // 
            buttonRepertoireSource.Location = new System.Drawing.Point(20, 22);
            buttonRepertoireSource.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            buttonRepertoireSource.Name = "buttonRepertoireSource";
            buttonRepertoireSource.Size = new System.Drawing.Size(161, 27);
            buttonRepertoireSource.TabIndex = 16;
            buttonRepertoireSource.Text = "Repertoire des sources";
            buttonRepertoireSource.UseVisualStyleBackColor = true;
            buttonRepertoireSource.Click += buttonRepertoireSource_Click;
            // 
            // buttonMinMaj
            // 
            buttonMinMaj.Location = new System.Drawing.Point(20, 72);
            buttonMinMaj.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            buttonMinMaj.Name = "buttonMinMaj";
            buttonMinMaj.Size = new System.Drawing.Size(161, 27);
            buttonMinMaj.TabIndex = 15;
            buttonMinMaj.Text = "Tables en minuscules";
            buttonMinMaj.UseVisualStyleBackColor = true;
            buttonMinMaj.Click += buttonMinMaj_Click;
            // 
            // groupBoxImport
            // 
            groupBoxImport.Controls.Add(listBoxImport);
            groupBoxImport.Controls.Add(textBoxChoixFichierImport);
            groupBoxImport.Controls.Add(buttonChoixFichierImport);
            groupBoxImport.Controls.Add(buttonImport);
            groupBoxImport.Location = new System.Drawing.Point(9, 438);
            groupBoxImport.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBoxImport.Name = "groupBoxImport";
            groupBoxImport.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBoxImport.Size = new System.Drawing.Size(744, 209);
            groupBoxImport.TabIndex = 17;
            groupBoxImport.TabStop = false;
            groupBoxImport.Text = "Import de tables d'un autre fichier vaoc";
            // 
            // listBoxImport
            // 
            listBoxImport.FormattingEnabled = true;
            listBoxImport.ItemHeight = 15;
            listBoxImport.Location = new System.Drawing.Point(212, 72);
            listBoxImport.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            listBoxImport.Name = "listBoxImport";
            listBoxImport.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            listBoxImport.Size = new System.Drawing.Size(522, 124);
            listBoxImport.Sorted = true;
            listBoxImport.TabIndex = 17;
            // 
            // textBoxChoixFichierImport
            // 
            textBoxChoixFichierImport.Location = new System.Drawing.Point(212, 25);
            textBoxChoixFichierImport.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            textBoxChoixFichierImport.Name = "textBoxChoixFichierImport";
            textBoxChoixFichierImport.ReadOnly = true;
            textBoxChoixFichierImport.Size = new System.Drawing.Size(523, 23);
            textBoxChoixFichierImport.TabIndex = 15;
            // 
            // buttonChoixFichierImport
            // 
            buttonChoixFichierImport.Location = new System.Drawing.Point(20, 22);
            buttonChoixFichierImport.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            buttonChoixFichierImport.Name = "buttonChoixFichierImport";
            buttonChoixFichierImport.Size = new System.Drawing.Size(161, 27);
            buttonChoixFichierImport.TabIndex = 16;
            buttonChoixFichierImport.Text = "Fichier source";
            buttonChoixFichierImport.UseVisualStyleBackColor = true;
            buttonChoixFichierImport.Click += buttonChoixFichierImport_Click;
            // 
            // buttonImport
            // 
            buttonImport.Location = new System.Drawing.Point(20, 72);
            buttonImport.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            buttonImport.Name = "buttonImport";
            buttonImport.Size = new System.Drawing.Size(161, 27);
            buttonImport.TabIndex = 15;
            buttonImport.Text = "Importer";
            buttonImport.UseVisualStyleBackColor = true;
            buttonImport.Click += buttonImport_Click;
            // 
            // FormOutils
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(760, 690);
            Controls.Add(groupBoxImport);
            Controls.Add(groupBoxSQL);
            Controls.Add(groupBoxImages);
            Controls.Add(buttonOK);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "FormOutils";
            Text = "FormOutils";
            groupBoxImages.ResumeLayout(false);
            groupBoxImages.PerformLayout();
            groupBoxSQL.ResumeLayout(false);
            groupBoxSQL.PerformLayout();
            groupBoxImport.ResumeLayout(false);
            groupBoxImport.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonImage;
        private System.Windows.Forms.TextBox textBoxFichierImage;
        private System.Windows.Forms.Button buttonRetraitCouleur;
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
        private System.Windows.Forms.Button buttonMosaique;
    }
}