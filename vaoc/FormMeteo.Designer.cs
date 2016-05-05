namespace vaoc
{
    partial class FormMeteo
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
            this.dataGridViewMeteo = new System.Windows.Forms.DataGridView();
            this.tABMETEOBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.dataSetCoutDonnees = new vaoc.Donnees();
            this.iDMETEODataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sNOMDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iCHANCEDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iPOURCENTRAVITAILLEMENTDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iPOURCENTFATIGUEDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iPOURCENTRALLIEMENTDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewMeteo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tABMETEOBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSetCoutDonnees)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonAnnuler
            // 
            this.buttonAnnuler.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonAnnuler.Location = new System.Drawing.Point(201, 166);
            this.buttonAnnuler.Name = "buttonAnnuler";
            this.buttonAnnuler.Size = new System.Drawing.Size(75, 23);
            this.buttonAnnuler.TabIndex = 2;
            this.buttonAnnuler.Text = "Annuler";
            this.buttonAnnuler.UseVisualStyleBackColor = true;
            // 
            // buttonValider
            // 
            this.buttonValider.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonValider.Location = new System.Drawing.Point(67, 166);
            this.buttonValider.Name = "buttonValider";
            this.buttonValider.Size = new System.Drawing.Size(75, 23);
            this.buttonValider.TabIndex = 1;
            this.buttonValider.Text = "Valider";
            this.buttonValider.UseVisualStyleBackColor = true;
            // 
            // dataGridViewMeteo
            // 
            this.dataGridViewMeteo.AllowUserToOrderColumns = true;
            this.dataGridViewMeteo.AutoGenerateColumns = false;
            this.dataGridViewMeteo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewMeteo.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.iDMETEODataGridViewTextBoxColumn,
            this.sNOMDataGridViewTextBoxColumn,
            this.iCHANCEDataGridViewTextBoxColumn,
            this.iPOURCENTRAVITAILLEMENTDataGridViewTextBoxColumn,
            this.iPOURCENTFATIGUEDataGridViewTextBoxColumn,
            this.iPOURCENTRALLIEMENTDataGridViewTextBoxColumn});
            this.dataGridViewMeteo.DataSource = this.tABMETEOBindingSource;
            this.dataGridViewMeteo.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewMeteo.Name = "dataGridViewMeteo";
            this.dataGridViewMeteo.Size = new System.Drawing.Size(814, 150);
            this.dataGridViewMeteo.TabIndex = 0;
            // 
            // tABMETEOBindingSource
            // 
            this.tABMETEOBindingSource.DataMember = "TAB_METEO";
            this.tABMETEOBindingSource.DataSource = this.dataSetCoutDonnees;
            // 
            // dataSetCoutDonnees
            // 
            this.dataSetCoutDonnees.DataSetName = "DataSetCoutDonnees";
            this.dataSetCoutDonnees.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // iDMETEODataGridViewTextBoxColumn
            // 
            this.iDMETEODataGridViewTextBoxColumn.DataPropertyName = "ID_METEO";
            this.iDMETEODataGridViewTextBoxColumn.HeaderText = "ID_METEO";
            this.iDMETEODataGridViewTextBoxColumn.Name = "iDMETEODataGridViewTextBoxColumn";
            // 
            // sNOMDataGridViewTextBoxColumn
            // 
            this.sNOMDataGridViewTextBoxColumn.DataPropertyName = "S_NOM";
            this.sNOMDataGridViewTextBoxColumn.HeaderText = "S_NOM";
            this.sNOMDataGridViewTextBoxColumn.Name = "sNOMDataGridViewTextBoxColumn";
            // 
            // iCHANCEDataGridViewTextBoxColumn
            // 
            this.iCHANCEDataGridViewTextBoxColumn.DataPropertyName = "I_CHANCE";
            this.iCHANCEDataGridViewTextBoxColumn.HeaderText = "I_CHANCE";
            this.iCHANCEDataGridViewTextBoxColumn.Name = "iCHANCEDataGridViewTextBoxColumn";
            // 
            // iPOURCENTRAVITAILLEMENTDataGridViewTextBoxColumn
            // 
            this.iPOURCENTRAVITAILLEMENTDataGridViewTextBoxColumn.DataPropertyName = "I_POURCENT_RAVITAILLEMENT";
            this.iPOURCENTRAVITAILLEMENTDataGridViewTextBoxColumn.HeaderText = "I_POURCENT_RAVITAILLEMENT";
            this.iPOURCENTRAVITAILLEMENTDataGridViewTextBoxColumn.Name = "iPOURCENTRAVITAILLEMENTDataGridViewTextBoxColumn";
            // 
            // iPOURCENTFATIGUEDataGridViewTextBoxColumn
            // 
            this.iPOURCENTFATIGUEDataGridViewTextBoxColumn.DataPropertyName = "I_POURCENT_FATIGUE";
            this.iPOURCENTFATIGUEDataGridViewTextBoxColumn.HeaderText = "I_POURCENT_FATIGUE";
            this.iPOURCENTFATIGUEDataGridViewTextBoxColumn.Name = "iPOURCENTFATIGUEDataGridViewTextBoxColumn";
            // 
            // iPOURCENTRALLIEMENTDataGridViewTextBoxColumn
            // 
            this.iPOURCENTRALLIEMENTDataGridViewTextBoxColumn.DataPropertyName = "I_POURCENT_RALLIEMENT";
            this.iPOURCENTRALLIEMENTDataGridViewTextBoxColumn.HeaderText = "I_POURCENT_RALLIEMENT";
            this.iPOURCENTRALLIEMENTDataGridViewTextBoxColumn.Name = "iPOURCENTRALLIEMENTDataGridViewTextBoxColumn";
            // 
            // FormMeteo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(812, 211);
            this.Controls.Add(this.dataGridViewMeteo);
            this.Controls.Add(this.buttonAnnuler);
            this.Controls.Add(this.buttonValider);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormMeteo";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Météo";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewMeteo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tABMETEOBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSetCoutDonnees)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonAnnuler;
        private System.Windows.Forms.Button buttonValider;
        private System.Windows.Forms.DataGridView dataGridViewMeteo;
        private System.Windows.Forms.BindingSource tABMETEOBindingSource;
        private Donnees dataSetCoutDonnees;
        private System.Windows.Forms.DataGridViewTextBoxColumn iDMETEODataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn sNOMDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iCHANCEDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iPOURCENTRAVITAILLEMENTDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iPOURCENTFATIGUEDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iPOURCENTRALLIEMENTDataGridViewTextBoxColumn;
    }
}