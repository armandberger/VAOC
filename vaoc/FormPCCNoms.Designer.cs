namespace vaoc
{
    partial class FormPCCNoms
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
            this.buttonValider = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.labelTraitement = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.labelTempsPasse = new System.Windows.Forms.Label();
            this.labelTempsRestant = new System.Windows.Forms.Label();
            this.textBoxTailleVille = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.buttonPCC = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.labelNbVillesSource = new System.Windows.Forms.Label();
            this.backgroundTraitement = new System.ComponentModel.BackgroundWorker();
            this.labelNbVillesDestination = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxDistanceMax = new System.Windows.Forms.TextBox();
            this.buttonDistanceMax = new System.Windows.Forms.Button();
            this.buttonReparerTablePCC = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonValider
            // 
            this.buttonValider.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonValider.Location = new System.Drawing.Point(717, 122);
            this.buttonValider.Name = "buttonValider";
            this.buttonValider.Size = new System.Drawing.Size(75, 23);
            this.buttonValider.TabIndex = 2;
            this.buttonValider.Text = "Fermer";
            this.buttonValider.UseVisualStyleBackColor = true;
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(12, 12);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(780, 23);
            this.progressBar.Step = 1;
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar.TabIndex = 7;
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
            // textBoxTailleVille
            // 
            this.textBoxTailleVille.Location = new System.Drawing.Point(79, 127);
            this.textBoxTailleVille.Name = "textBoxTailleVille";
            this.textBoxTailleVille.Size = new System.Drawing.Size(42, 20);
            this.textBoxTailleVille.TabIndex = 38;
            this.textBoxTailleVille.Tag = "la zone autour du nom considérée comme valide";
            this.textBoxTailleVille.Text = "3";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(15, 130);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(64, 13);
            this.label10.TabIndex = 37;
            this.label10.Text = "Taille villes :";
            // 
            // buttonPCC
            // 
            this.buttonPCC.Location = new System.Drawing.Point(542, 122);
            this.buttonPCC.Name = "buttonPCC";
            this.buttonPCC.Size = new System.Drawing.Size(157, 23);
            this.buttonPCC.TabIndex = 39;
            this.buttonPCC.Text = "Generer le PCC";
            this.buttonPCC.UseVisualStyleBackColor = true;
            this.buttonPCC.Click += new System.EventHandler(this.buttonPCC_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(248, 87);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 13);
            this.label2.TabIndex = 40;
            this.label2.Text = "# ville source :";
            // 
            // labelNbVillesSource
            // 
            this.labelNbVillesSource.AutoSize = true;
            this.labelNbVillesSource.Location = new System.Drawing.Point(326, 87);
            this.labelNbVillesSource.Name = "labelNbVillesSource";
            this.labelNbVillesSource.Size = new System.Drawing.Size(24, 13);
            this.labelNbVillesSource.TabIndex = 41;
            this.labelNbVillesSource.Text = "0/0";
            // 
            // backgroundTraitement
            // 
            this.backgroundTraitement.WorkerReportsProgress = true;
            this.backgroundTraitement.WorkerSupportsCancellation = true;
            this.backgroundTraitement.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundTraitement_DoWork);
            this.backgroundTraitement.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundTraitement_ProgressChanged);
            this.backgroundTraitement.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundTraitement_RunWorkerCompleted);
            // 
            // labelNbVillesDestination
            // 
            this.labelNbVillesDestination.AutoSize = true;
            this.labelNbVillesDestination.Location = new System.Drawing.Point(537, 87);
            this.labelNbVillesDestination.Name = "labelNbVillesDestination";
            this.labelNbVillesDestination.Size = new System.Drawing.Size(24, 13);
            this.labelNbVillesDestination.TabIndex = 43;
            this.labelNbVillesDestination.Text = "0/0";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(445, 87);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(95, 13);
            this.label4.TabIndex = 42;
            this.label4.Text = "# ville destination :";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(142, 130);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(166, 13);
            this.label3.TabIndex = 44;
            this.label3.Text = "distance max entre villes (en km) :";
            // 
            // textBoxDistanceMax
            // 
            this.textBoxDistanceMax.Location = new System.Drawing.Point(314, 127);
            this.textBoxDistanceMax.Name = "textBoxDistanceMax";
            this.textBoxDistanceMax.Size = new System.Drawing.Size(42, 20);
            this.textBoxDistanceMax.TabIndex = 45;
            this.textBoxDistanceMax.Tag = "la zone autour du nom considérée comme valide";
            this.textBoxDistanceMax.Text = "50";
            // 
            // buttonDistanceMax
            // 
            this.buttonDistanceMax.Location = new System.Drawing.Point(655, 45);
            this.buttonDistanceMax.Name = "buttonDistanceMax";
            this.buttonDistanceMax.Size = new System.Drawing.Size(137, 23);
            this.buttonDistanceMax.TabIndex = 46;
            this.buttonDistanceMax.Text = "Stats distances calculées";
            this.buttonDistanceMax.UseVisualStyleBackColor = true;
            this.buttonDistanceMax.Click += new System.EventHandler(this.buttonDistanceMax_Click);
            // 
            // buttonReparerTablePCC
            // 
            this.buttonReparerTablePCC.Location = new System.Drawing.Point(655, 77);
            this.buttonReparerTablePCC.Name = "buttonReparerTablePCC";
            this.buttonReparerTablePCC.Size = new System.Drawing.Size(137, 23);
            this.buttonReparerTablePCC.TabIndex = 47;
            this.buttonReparerTablePCC.Text = "Reparer la table PCC";
            this.buttonReparerTablePCC.UseVisualStyleBackColor = true;
            this.buttonReparerTablePCC.Click += new System.EventHandler(this.buttonReparerTablePCC_Click);
            // 
            // FormPCCNoms
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(801, 157);
            this.Controls.Add(this.buttonReparerTablePCC);
            this.Controls.Add(this.buttonDistanceMax);
            this.Controls.Add(this.textBoxDistanceMax);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.labelNbVillesDestination);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.labelNbVillesSource);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxTailleVille);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.buttonPCC);
            this.Controls.Add(this.labelTempsRestant);
            this.Controls.Add(this.labelTempsPasse);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.labelTraitement);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.buttonValider);
            this.Name = "FormPCCNoms";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Generation PCC Noms";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonValider;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelTraitement;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label labelTempsPasse;
        private System.Windows.Forms.Label labelTempsRestant;
        private System.Windows.Forms.TextBox textBoxTailleVille;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button buttonPCC;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelNbVillesSource;
        private System.ComponentModel.BackgroundWorker backgroundTraitement;
        private System.Windows.Forms.Label labelNbVillesDestination;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxDistanceMax;
        private System.Windows.Forms.Button buttonDistanceMax;
        private System.Windows.Forms.Button buttonReparerTablePCC;
    }
}