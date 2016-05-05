namespace vaoc
{
    partial class FormStatistiques
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
            this.dataGridOrdres = new System.Windows.Forms.DataGridView();
            this.Nation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.JOUEUR = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ORDRES = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.buttonGenerer = new System.Windows.Forms.Button();
            this.buttonFermer = new System.Windows.Forms.Button();
            this.dataGridMessages = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Emis = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Recus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Correspondant = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.textBoxRayon = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBoxCouleurs = new System.Windows.Forms.GroupBox();
            this.buttonCouleurOrdre = new System.Windows.Forms.Button();
            this.buttonCouleurRecu = new System.Windows.Forms.Button();
            this.buttonCouleurEmis = new System.Windows.Forms.Button();
            this.couleurDialog = new System.Windows.Forms.ColorDialog();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridOrdres)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridMessages)).BeginInit();
            this.groupBoxCouleurs.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridOrdres
            // 
            this.dataGridOrdres.AllowUserToAddRows = false;
            this.dataGridOrdres.AllowUserToDeleteRows = false;
            this.dataGridOrdres.AllowUserToOrderColumns = true;
            this.dataGridOrdres.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridOrdres.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Nation,
            this.JOUEUR,
            this.ORDRES});
            this.dataGridOrdres.Location = new System.Drawing.Point(12, 94);
            this.dataGridOrdres.Name = "dataGridOrdres";
            this.dataGridOrdres.ReadOnly = true;
            this.dataGridOrdres.Size = new System.Drawing.Size(319, 151);
            this.dataGridOrdres.TabIndex = 0;
            // 
            // Nation
            // 
            this.Nation.HeaderText = "Nation";
            this.Nation.Name = "Nation";
            this.Nation.ReadOnly = true;
            // 
            // JOUEUR
            // 
            this.JOUEUR.HeaderText = "Joueur";
            this.JOUEUR.Name = "JOUEUR";
            this.JOUEUR.ReadOnly = true;
            // 
            // ORDRES
            // 
            this.ORDRES.HeaderText = "Nb ordres";
            this.ORDRES.Name = "ORDRES";
            this.ORDRES.ReadOnly = true;
            // 
            // buttonGenerer
            // 
            this.buttonGenerer.Location = new System.Drawing.Point(479, 12);
            this.buttonGenerer.Name = "buttonGenerer";
            this.buttonGenerer.Size = new System.Drawing.Size(170, 23);
            this.buttonGenerer.TabIndex = 12;
            this.buttonGenerer.Text = "Generer les images statistiques";
            this.buttonGenerer.UseVisualStyleBackColor = true;
            this.buttonGenerer.Click += new System.EventHandler(this.buttonGenerer_Click);
            // 
            // buttonFermer
            // 
            this.buttonFermer.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonFermer.Location = new System.Drawing.Point(757, 12);
            this.buttonFermer.Name = "buttonFermer";
            this.buttonFermer.Size = new System.Drawing.Size(75, 23);
            this.buttonFermer.TabIndex = 11;
            this.buttonFermer.Text = "Fermer";
            this.buttonFermer.UseVisualStyleBackColor = true;
            // 
            // dataGridMessages
            // 
            this.dataGridMessages.AllowUserToAddRows = false;
            this.dataGridMessages.AllowUserToDeleteRows = false;
            this.dataGridMessages.AllowUserToOrderColumns = true;
            this.dataGridMessages.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridMessages.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2,
            this.Emis,
            this.Recus,
            this.Correspondant});
            this.dataGridMessages.Location = new System.Drawing.Point(69, 175);
            this.dataGridMessages.Name = "dataGridMessages";
            this.dataGridMessages.ReadOnly = true;
            this.dataGridMessages.Size = new System.Drawing.Size(541, 199);
            this.dataGridMessages.TabIndex = 13;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "Nation";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "Joueur";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            // 
            // Emis
            // 
            this.Emis.HeaderText = "Emis vers";
            this.Emis.Name = "Emis";
            this.Emis.ReadOnly = true;
            // 
            // Recus
            // 
            this.Recus.HeaderText = "Recus de";
            this.Recus.Name = "Recus";
            this.Recus.ReadOnly = true;
            // 
            // Correspondant
            // 
            this.Correspondant.HeaderText = "Correspondant";
            this.Correspondant.Name = "Correspondant";
            this.Correspondant.ReadOnly = true;
            // 
            // textBoxRayon
            // 
            this.textBoxRayon.Location = new System.Drawing.Point(114, 5);
            this.textBoxRayon.Name = "textBoxRayon";
            this.textBoxRayon.Size = new System.Drawing.Size(65, 20);
            this.textBoxRayon.TabIndex = 14;
            this.textBoxRayon.Text = "2";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(102, 13);
            this.label1.TabIndex = 15;
            this.label1.Text = "rayon d\'un élément :";
            // 
            // groupBoxCouleurs
            // 
            this.groupBoxCouleurs.Controls.Add(this.buttonCouleurEmis);
            this.groupBoxCouleurs.Controls.Add(this.buttonCouleurRecu);
            this.groupBoxCouleurs.Controls.Add(this.buttonCouleurOrdre);
            this.groupBoxCouleurs.ForeColor = System.Drawing.SystemColors.ControlText;
            this.groupBoxCouleurs.Location = new System.Drawing.Point(213, 3);
            this.groupBoxCouleurs.Name = "groupBoxCouleurs";
            this.groupBoxCouleurs.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.groupBoxCouleurs.Size = new System.Drawing.Size(252, 57);
            this.groupBoxCouleurs.TabIndex = 16;
            this.groupBoxCouleurs.TabStop = false;
            this.groupBoxCouleurs.Text = "Couleurs";
            // 
            // buttonCouleurOrdre
            // 
            this.buttonCouleurOrdre.BackColor = System.Drawing.SystemColors.HighlightText;
            this.buttonCouleurOrdre.Location = new System.Drawing.Point(6, 22);
            this.buttonCouleurOrdre.Name = "buttonCouleurOrdre";
            this.buttonCouleurOrdre.Size = new System.Drawing.Size(75, 23);
            this.buttonCouleurOrdre.TabIndex = 0;
            this.buttonCouleurOrdre.Text = "Ordres";
            this.buttonCouleurOrdre.UseVisualStyleBackColor = false;
            this.buttonCouleurOrdre.Click += new System.EventHandler(this.buttonCouleurOrdre_Click);
            // 
            // buttonCouleurRecu
            // 
            this.buttonCouleurRecu.BackColor = System.Drawing.SystemColors.HighlightText;
            this.buttonCouleurRecu.Location = new System.Drawing.Point(87, 22);
            this.buttonCouleurRecu.Name = "buttonCouleurRecu";
            this.buttonCouleurRecu.Size = new System.Drawing.Size(75, 23);
            this.buttonCouleurRecu.TabIndex = 1;
            this.buttonCouleurRecu.Text = "Recus";
            this.buttonCouleurRecu.UseVisualStyleBackColor = false;
            this.buttonCouleurRecu.Click += new System.EventHandler(this.buttonCouleurRecu_Click);
            // 
            // buttonCouleurEmis
            // 
            this.buttonCouleurEmis.BackColor = System.Drawing.SystemColors.HighlightText;
            this.buttonCouleurEmis.Location = new System.Drawing.Point(168, 22);
            this.buttonCouleurEmis.Name = "buttonCouleurEmis";
            this.buttonCouleurEmis.Size = new System.Drawing.Size(75, 23);
            this.buttonCouleurEmis.TabIndex = 2;
            this.buttonCouleurEmis.Text = "Emis";
            this.buttonCouleurEmis.UseVisualStyleBackColor = false;
            this.buttonCouleurEmis.Click += new System.EventHandler(this.buttonCouleurEmis_Click);
            // 
            // FormStatistiques
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1234, 425);
            this.Controls.Add(this.groupBoxCouleurs);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxRayon);
            this.Controls.Add(this.dataGridMessages);
            this.Controls.Add(this.buttonGenerer);
            this.Controls.Add(this.buttonFermer);
            this.Controls.Add(this.dataGridOrdres);
            this.Name = "FormStatistiques";
            this.Text = "Statistiques";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.FormStatistiques_Load);
            this.Resize += new System.EventHandler(this.FormStatistiques_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridOrdres)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridMessages)).EndInit();
            this.groupBoxCouleurs.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridOrdres;
        private System.Windows.Forms.Button buttonGenerer;
        private System.Windows.Forms.Button buttonFermer;
        private System.Windows.Forms.DataGridViewTextBoxColumn Nation;
        private System.Windows.Forms.DataGridViewTextBoxColumn JOUEUR;
        private System.Windows.Forms.DataGridViewTextBoxColumn ORDRES;
        private System.Windows.Forms.DataGridView dataGridMessages;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Emis;
        private System.Windows.Forms.DataGridViewTextBoxColumn Recus;
        private System.Windows.Forms.DataGridViewTextBoxColumn Correspondant;
        private System.Windows.Forms.TextBox textBoxRayon;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBoxCouleurs;
        private System.Windows.Forms.Button buttonCouleurOrdre;
        private System.Windows.Forms.Button buttonCouleurEmis;
        private System.Windows.Forms.Button buttonCouleurRecu;
        private System.Windows.Forms.ColorDialog couleurDialog;
    }
}