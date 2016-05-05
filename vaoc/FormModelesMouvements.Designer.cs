namespace vaoc
{
    partial class FormModelesMouvements
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
            this.dataGridViewModelesMouvements = new System.Windows.Forms.DataGridView();
            this.iDMODELEMOUVEMENTDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sNOMDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iVITESSEINFANTERIEDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iVITESSECAVALERIEDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iVITESSEARTILLERIEDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tABMODELEMOUVEMENTBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.dataSetCoutDonnees = new vaoc.Donnees();
            this.buttonAnnuler = new System.Windows.Forms.Button();
            this.buttonValider = new System.Windows.Forms.Button();
            this.labelVitesse = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewModelesMouvements)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tABMODELEMOUVEMENTBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSetCoutDonnees)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewModelesMouvements
            // 
            this.dataGridViewModelesMouvements.AllowUserToOrderColumns = true;
            this.dataGridViewModelesMouvements.AutoGenerateColumns = false;
            this.dataGridViewModelesMouvements.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewModelesMouvements.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.iDMODELEMOUVEMENTDataGridViewTextBoxColumn,
            this.sNOMDataGridViewTextBoxColumn,
            this.iVITESSEINFANTERIEDataGridViewTextBoxColumn,
            this.iVITESSECAVALERIEDataGridViewTextBoxColumn,
            this.iVITESSEARTILLERIEDataGridViewTextBoxColumn});
            this.dataGridViewModelesMouvements.DataSource = this.tABMODELEMOUVEMENTBindingSource;
            this.dataGridViewModelesMouvements.Location = new System.Drawing.Point(12, 0);
            this.dataGridViewModelesMouvements.Name = "dataGridViewModelesMouvements";
            this.dataGridViewModelesMouvements.Size = new System.Drawing.Size(691, 188);
            this.dataGridViewModelesMouvements.TabIndex = 3;
            // 
            // iDMODELEMOUVEMENTDataGridViewTextBoxColumn
            // 
            this.iDMODELEMOUVEMENTDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.iDMODELEMOUVEMENTDataGridViewTextBoxColumn.DataPropertyName = "ID_MODELE_MOUVEMENT";
            this.iDMODELEMOUVEMENTDataGridViewTextBoxColumn.HeaderText = "ID_MODELE_MOUVEMENT";
            this.iDMODELEMOUVEMENTDataGridViewTextBoxColumn.Name = "iDMODELEMOUVEMENTDataGridViewTextBoxColumn";
            // 
            // sNOMDataGridViewTextBoxColumn
            // 
            this.sNOMDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.sNOMDataGridViewTextBoxColumn.DataPropertyName = "S_NOM";
            this.sNOMDataGridViewTextBoxColumn.HeaderText = "S_NOM";
            this.sNOMDataGridViewTextBoxColumn.Name = "sNOMDataGridViewTextBoxColumn";
            this.sNOMDataGridViewTextBoxColumn.Width = 70;
            // 
            // iVITESSEINFANTERIEDataGridViewTextBoxColumn
            // 
            this.iVITESSEINFANTERIEDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.iVITESSEINFANTERIEDataGridViewTextBoxColumn.DataPropertyName = "I_VITESSE_INFANTERIE";
            this.iVITESSEINFANTERIEDataGridViewTextBoxColumn.HeaderText = "I_VITESSE_INFANTERIE";
            this.iVITESSEINFANTERIEDataGridViewTextBoxColumn.Name = "iVITESSEINFANTERIEDataGridViewTextBoxColumn";
            // 
            // iVITESSECAVALERIEDataGridViewTextBoxColumn
            // 
            this.iVITESSECAVALERIEDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.iVITESSECAVALERIEDataGridViewTextBoxColumn.DataPropertyName = "I_VITESSE_CAVALERIE";
            this.iVITESSECAVALERIEDataGridViewTextBoxColumn.HeaderText = "I_VITESSE_CAVALERIE";
            this.iVITESSECAVALERIEDataGridViewTextBoxColumn.Name = "iVITESSECAVALERIEDataGridViewTextBoxColumn";
            // 
            // iVITESSEARTILLERIEDataGridViewTextBoxColumn
            // 
            this.iVITESSEARTILLERIEDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.iVITESSEARTILLERIEDataGridViewTextBoxColumn.DataPropertyName = "I_VITESSE_ARTILLERIE";
            this.iVITESSEARTILLERIEDataGridViewTextBoxColumn.HeaderText = "I_VITESSE_ARTILLERIE";
            this.iVITESSEARTILLERIEDataGridViewTextBoxColumn.Name = "iVITESSEARTILLERIEDataGridViewTextBoxColumn";
            // 
            // tABMODELEMOUVEMENTBindingSource
            // 
            this.tABMODELEMOUVEMENTBindingSource.DataMember = "TAB_MODELE_MOUVEMENT";
            this.tABMODELEMOUVEMENTBindingSource.DataSource = this.dataSetCoutDonnees;
            // 
            // dataSetCoutDonnees
            // 
            this.dataSetCoutDonnees.DataSetName = "DataSetCoutDonnees";
            this.dataSetCoutDonnees.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // buttonAnnuler
            // 
            this.buttonAnnuler.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonAnnuler.Location = new System.Drawing.Point(387, 205);
            this.buttonAnnuler.Name = "buttonAnnuler";
            this.buttonAnnuler.Size = new System.Drawing.Size(75, 23);
            this.buttonAnnuler.TabIndex = 5;
            this.buttonAnnuler.Text = "Annuler";
            this.buttonAnnuler.UseVisualStyleBackColor = true;
            // 
            // buttonValider
            // 
            this.buttonValider.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonValider.Location = new System.Drawing.Point(253, 205);
            this.buttonValider.Name = "buttonValider";
            this.buttonValider.Size = new System.Drawing.Size(75, 23);
            this.buttonValider.TabIndex = 4;
            this.buttonValider.Text = "Valider";
            this.buttonValider.UseVisualStyleBackColor = true;
            // 
            // labelVitesse
            // 
            this.labelVitesse.AutoSize = true;
            this.labelVitesse.Location = new System.Drawing.Point(12, 205);
            this.labelVitesse.Name = "labelVitesse";
            this.labelVitesse.Size = new System.Drawing.Size(154, 13);
            this.labelVitesse.TabIndex = 6;
            this.labelVitesse.Text = "La vitesse est donnée en km/h";
            // 
            // FormModelesMouvements
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(715, 240);
            this.Controls.Add(this.labelVitesse);
            this.Controls.Add(this.dataGridViewModelesMouvements);
            this.Controls.Add(this.buttonAnnuler);
            this.Controls.Add(this.buttonValider);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormModelesMouvements";
            this.Text = "FormModelesMouvements";
            this.Load += new System.EventHandler(this.FormModelesMouvements_Load);
            this.Resize += new System.EventHandler(this.FormModelesMouvements_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewModelesMouvements)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tABMODELEMOUVEMENTBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSetCoutDonnees)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewModelesMouvements;
        private System.Windows.Forms.BindingSource tABMODELEMOUVEMENTBindingSource;
        private Donnees dataSetCoutDonnees;
        private System.Windows.Forms.Button buttonAnnuler;
        private System.Windows.Forms.Button buttonValider;
        private System.Windows.Forms.DataGridViewTextBoxColumn iDMODELEMOUVEMENTDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn sNOMDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iVITESSEINFANTERIEDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iVITESSECAVALERIEDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iVITESSEARTILLERIEDataGridViewTextBoxColumn;
        private System.Windows.Forms.Label labelVitesse;
    }
}