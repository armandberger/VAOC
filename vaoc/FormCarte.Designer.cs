namespace vaoc
{
    partial class FormCarte
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
            this.buttonGeneration = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.labelNomCarteTopographique = new System.Windows.Forms.Label();
            this.labelLargeur = new System.Windows.Forms.Label();
            this.labelHauteur = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.labelTraitement = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.timerTraitement = new System.Windows.Forms.Timer(this.components);
            this.tABMODELETERRAINBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.dataSetCoutDonnees = new vaoc.Donnees();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.labelTempsPasse = new System.Windows.Forms.Label();
            this.labelTempsRestant = new System.Windows.Forms.Label();
            this.buttonNettoyage = new System.Windows.Forms.Button();
            this.buttonMiseAJourModelesTerrain = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.tABMODELETERRAINBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSetCoutDonnees)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonAnnuler
            // 
            this.buttonAnnuler.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonAnnuler.Location = new System.Drawing.Point(639, 138);
            this.buttonAnnuler.Name = "buttonAnnuler";
            this.buttonAnnuler.Size = new System.Drawing.Size(75, 23);
            this.buttonAnnuler.TabIndex = 3;
            this.buttonAnnuler.Text = "Annuler";
            this.buttonAnnuler.UseVisualStyleBackColor = true;
            // 
            // buttonValider
            // 
            this.buttonValider.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonValider.Location = new System.Drawing.Point(639, 109);
            this.buttonValider.Name = "buttonValider";
            this.buttonValider.Size = new System.Drawing.Size(75, 23);
            this.buttonValider.TabIndex = 2;
            this.buttonValider.Text = "Valider";
            this.buttonValider.UseVisualStyleBackColor = true;
            // 
            // buttonGeneration
            // 
            this.buttonGeneration.Location = new System.Drawing.Point(15, 128);
            this.buttonGeneration.Name = "buttonGeneration";
            this.buttonGeneration.Size = new System.Drawing.Size(138, 23);
            this.buttonGeneration.TabIndex = 1;
            this.buttonGeneration.Text = "Générer la Carte";
            this.buttonGeneration.UseVisualStyleBackColor = true;
            this.buttonGeneration.Click += new System.EventHandler(this.buttonGeneration_Click);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(12, 12);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(736, 23);
            this.progressBar.Step = 1;
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar.TabIndex = 7;
            // 
            // labelNomCarteTopographique
            // 
            this.labelNomCarteTopographique.AutoSize = true;
            this.labelNomCarteTopographique.Location = new System.Drawing.Point(68, 95);
            this.labelNomCarteTopographique.Name = "labelNomCarteTopographique";
            this.labelNomCarteTopographique.Size = new System.Drawing.Size(82, 13);
            this.labelNomCarteTopographique.TabIndex = 8;
            this.labelNomCarteTopographique.Text = "Nom de la carte";
            // 
            // labelLargeur
            // 
            this.labelLargeur.AutoSize = true;
            this.labelLargeur.Location = new System.Drawing.Point(567, 95);
            this.labelLargeur.Name = "labelLargeur";
            this.labelLargeur.Size = new System.Drawing.Size(37, 13);
            this.labelLargeur.TabIndex = 9;
            this.labelLargeur.Text = "0 pixel";
            // 
            // labelHauteur
            // 
            this.labelHauteur.AutoSize = true;
            this.labelHauteur.Location = new System.Drawing.Point(445, 95);
            this.labelHauteur.Name = "labelHauteur";
            this.labelHauteur.Size = new System.Drawing.Size(37, 13);
            this.labelHauteur.TabIndex = 10;
            this.labelHauteur.Text = "0 pixel";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 55);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Traitement :";
            // 
            // labelTraitement
            // 
            this.labelTraitement.AutoSize = true;
            this.labelTraitement.Location = new System.Drawing.Point(80, 55);
            this.labelTraitement.Name = "labelTraitement";
            this.labelTraitement.Size = new System.Drawing.Size(38, 13);
            this.labelTraitement.TabIndex = 12;
            this.labelTraitement.Text = "Aucun";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 95);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "Nom :";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(388, 95);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "Hauteur :";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(509, 95);
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
            this.timerTraitement.Interval = 5;
            this.timerTraitement.Tick += new System.EventHandler(this.timerTraitement_Tick);
            // 
            // tABMODELETERRAINBindingSource
            // 
            this.tABMODELETERRAINBindingSource.DataMember = "TAB_MODELE_TERRAIN";
            this.tABMODELETERRAINBindingSource.DataSource = this.dataSetCoutDonnees;
            // 
            // dataSetCoutDonnees
            // 
            this.dataSetCoutDonnees.DataSetName = "DataSetCoutDonnees";
            this.dataSetCoutDonnees.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(248, 55);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(76, 13);
            this.label5.TabIndex = 16;
            this.label5.Text = "Temps passé :";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(445, 55);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(80, 13);
            this.label6.TabIndex = 17;
            this.label6.Text = "Temps restant :";
            // 
            // labelTempsPasse
            // 
            this.labelTempsPasse.AutoSize = true;
            this.labelTempsPasse.Location = new System.Drawing.Point(330, 55);
            this.labelTempsPasse.Name = "labelTempsPasse";
            this.labelTempsPasse.Size = new System.Drawing.Size(62, 13);
            this.labelTempsPasse.TabIndex = 18;
            this.labelTempsPasse.Text = "0 secondes";
            // 
            // labelTempsRestant
            // 
            this.labelTempsRestant.AutoSize = true;
            this.labelTempsRestant.Location = new System.Drawing.Point(531, 55);
            this.labelTempsRestant.Name = "labelTempsRestant";
            this.labelTempsRestant.Size = new System.Drawing.Size(62, 13);
            this.labelTempsRestant.TabIndex = 19;
            this.labelTempsRestant.Text = "0 secondes";
            // 
            // buttonNettoyage
            // 
            this.buttonNettoyage.Location = new System.Drawing.Point(173, 128);
            this.buttonNettoyage.Name = "buttonNettoyage";
            this.buttonNettoyage.Size = new System.Drawing.Size(219, 23);
            this.buttonNettoyage.TabIndex = 20;
            this.buttonNettoyage.Text = "Supprimer les cases de remplacement";
            this.buttonNettoyage.UseVisualStyleBackColor = true;
            this.buttonNettoyage.Click += new System.EventHandler(this.buttonNettoyage_Click);
            // 
            // buttonMiseAJourModelesTerrain
            // 
            this.buttonMiseAJourModelesTerrain.Location = new System.Drawing.Point(414, 130);
            this.buttonMiseAJourModelesTerrain.Name = "buttonMiseAJourModelesTerrain";
            this.buttonMiseAJourModelesTerrain.Size = new System.Drawing.Size(182, 23);
            this.buttonMiseAJourModelesTerrain.TabIndex = 21;
            this.buttonMiseAJourModelesTerrain.Tag = "on met seulement à jour les modeles de terrain suite à une modification mineure d" +
    "e l\'image source";
            this.buttonMiseAJourModelesTerrain.Text = "Mise à jour des modeles de terrain";
            this.buttonMiseAJourModelesTerrain.UseVisualStyleBackColor = true;
            this.buttonMiseAJourModelesTerrain.Click += new System.EventHandler(this.buttonMiseAJourModelesTerrain_Click);
            // 
            // FormCarte
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(760, 172);
            this.Controls.Add(this.buttonMiseAJourModelesTerrain);
            this.Controls.Add(this.buttonNettoyage);
            this.Controls.Add(this.labelTempsRestant);
            this.Controls.Add(this.labelTempsPasse);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.labelTraitement);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.labelHauteur);
            this.Controls.Add(this.labelLargeur);
            this.Controls.Add(this.labelNomCarteTopographique);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.buttonGeneration);
            this.Controls.Add(this.buttonAnnuler);
            this.Controls.Add(this.buttonValider);
            this.Name = "FormCarte";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Carte";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormCarte_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.tABMODELETERRAINBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSetCoutDonnees)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonAnnuler;
        private System.Windows.Forms.Button buttonValider;
        private System.Windows.Forms.Button buttonGeneration;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label labelNomCarteTopographique;
        private System.Windows.Forms.Label labelLargeur;
        private System.Windows.Forms.Label labelHauteur;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelTraitement;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Timer timerTraitement;
        private System.Windows.Forms.BindingSource tABMODELETERRAINBindingSource;
        private Donnees dataSetCoutDonnees;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label labelTempsPasse;
        private System.Windows.Forms.Label labelTempsRestant;
        private System.Windows.Forms.Button buttonNettoyage;
        private System.Windows.Forms.Button buttonMiseAJourModelesTerrain;
    }
}