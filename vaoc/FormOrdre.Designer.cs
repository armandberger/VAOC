namespace vaoc
{
    partial class FormOrdre
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
            this.dataGridViewOrdres = new System.Windows.Forms.DataGridView();
            this.OrdresBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.dataSetCoutDonnees = new vaoc.Donnees();
            this.iDORDREDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iDORDRETRANSMISDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iDORDRESUIVANTDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iDORDREWEBDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iORDRETYPEDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iDPIONDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iDCASEDEPARTDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iEFFECTIFDEPARTDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iDCASEDESTINATIONDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iDNOMDESTINATIONDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iEFFECTIFDESTINATIONDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iTOURDEBUTDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iPHASEDEBUTDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iTOURFINDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iPHASEFINDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iDMESSAGEDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iDDESTINATAIREDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iDCIBLEDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iDDESTINATAIRECIBLEDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iDBATAILLEDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iZONEBATAILLEDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iHEUREDEBUTDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iDUREEDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iENGAGEMENTDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewOrdres)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.OrdresBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSetCoutDonnees)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonAnnuler
            // 
            this.buttonAnnuler.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonAnnuler.Location = new System.Drawing.Point(175, 238);
            this.buttonAnnuler.Name = "buttonAnnuler";
            this.buttonAnnuler.Size = new System.Drawing.Size(75, 23);
            this.buttonAnnuler.TabIndex = 7;
            this.buttonAnnuler.Text = "Annuler";
            this.buttonAnnuler.UseVisualStyleBackColor = true;
            // 
            // buttonValider
            // 
            this.buttonValider.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonValider.Location = new System.Drawing.Point(41, 238);
            this.buttonValider.Name = "buttonValider";
            this.buttonValider.Size = new System.Drawing.Size(75, 23);
            this.buttonValider.TabIndex = 6;
            this.buttonValider.Text = "Valider";
            this.buttonValider.UseVisualStyleBackColor = true;
            // 
            // dataGridViewOrdres
            // 
            this.dataGridViewOrdres.AllowUserToOrderColumns = true;
            this.dataGridViewOrdres.AutoGenerateColumns = false;
            this.dataGridViewOrdres.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewOrdres.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.iDORDREDataGridViewTextBoxColumn,
            this.iDORDRETRANSMISDataGridViewTextBoxColumn,
            this.iDORDRESUIVANTDataGridViewTextBoxColumn,
            this.iDORDREWEBDataGridViewTextBoxColumn,
            this.iORDRETYPEDataGridViewTextBoxColumn,
            this.iDPIONDataGridViewTextBoxColumn,
            this.iDCASEDEPARTDataGridViewTextBoxColumn,
            this.iEFFECTIFDEPARTDataGridViewTextBoxColumn,
            this.iDCASEDESTINATIONDataGridViewTextBoxColumn,
            this.iDNOMDESTINATIONDataGridViewTextBoxColumn,
            this.iEFFECTIFDESTINATIONDataGridViewTextBoxColumn,
            this.iTOURDEBUTDataGridViewTextBoxColumn,
            this.iPHASEDEBUTDataGridViewTextBoxColumn,
            this.iTOURFINDataGridViewTextBoxColumn,
            this.iPHASEFINDataGridViewTextBoxColumn,
            this.iDMESSAGEDataGridViewTextBoxColumn,
            this.iDDESTINATAIREDataGridViewTextBoxColumn,
            this.iDCIBLEDataGridViewTextBoxColumn,
            this.iDDESTINATAIRECIBLEDataGridViewTextBoxColumn,
            this.iDBATAILLEDataGridViewTextBoxColumn,
            this.iZONEBATAILLEDataGridViewTextBoxColumn,
            this.iHEUREDEBUTDataGridViewTextBoxColumn,
            this.iDUREEDataGridViewTextBoxColumn,
            this.iENGAGEMENTDataGridViewTextBoxColumn});
            this.dataGridViewOrdres.DataSource = this.OrdresBindingSource;
            this.dataGridViewOrdres.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewOrdres.Name = "dataGridViewOrdres";
            this.dataGridViewOrdres.Size = new System.Drawing.Size(899, 143);
            this.dataGridViewOrdres.TabIndex = 8;
            // 
            // OrdresBindingSource
            // 
            this.OrdresBindingSource.DataMember = "TAB_ORDRE";
            this.OrdresBindingSource.DataSource = this.dataSetCoutDonnees;
            // 
            // dataSetCoutDonnees
            // 
            this.dataSetCoutDonnees.DataSetName = "DataSetCoutDonnees";
            this.dataSetCoutDonnees.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // iDORDREDataGridViewTextBoxColumn
            // 
            this.iDORDREDataGridViewTextBoxColumn.DataPropertyName = "ID_ORDRE";
            this.iDORDREDataGridViewTextBoxColumn.HeaderText = "ID_ORDRE";
            this.iDORDREDataGridViewTextBoxColumn.Name = "iDORDREDataGridViewTextBoxColumn";
            // 
            // iDORDRETRANSMISDataGridViewTextBoxColumn
            // 
            this.iDORDRETRANSMISDataGridViewTextBoxColumn.DataPropertyName = "ID_ORDRE_TRANSMIS";
            this.iDORDRETRANSMISDataGridViewTextBoxColumn.HeaderText = "ID_ORDRE_TRANSMIS";
            this.iDORDRETRANSMISDataGridViewTextBoxColumn.Name = "iDORDRETRANSMISDataGridViewTextBoxColumn";
            // 
            // iDORDRESUIVANTDataGridViewTextBoxColumn
            // 
            this.iDORDRESUIVANTDataGridViewTextBoxColumn.DataPropertyName = "ID_ORDRE_SUIVANT";
            this.iDORDRESUIVANTDataGridViewTextBoxColumn.HeaderText = "ID_ORDRE_SUIVANT";
            this.iDORDRESUIVANTDataGridViewTextBoxColumn.Name = "iDORDRESUIVANTDataGridViewTextBoxColumn";
            // 
            // iDORDREWEBDataGridViewTextBoxColumn
            // 
            this.iDORDREWEBDataGridViewTextBoxColumn.DataPropertyName = "ID_ORDRE_WEB";
            this.iDORDREWEBDataGridViewTextBoxColumn.HeaderText = "ID_ORDRE_WEB";
            this.iDORDREWEBDataGridViewTextBoxColumn.Name = "iDORDREWEBDataGridViewTextBoxColumn";
            // 
            // iORDRETYPEDataGridViewTextBoxColumn
            // 
            this.iORDRETYPEDataGridViewTextBoxColumn.DataPropertyName = "I_ORDRE_TYPE";
            this.iORDRETYPEDataGridViewTextBoxColumn.HeaderText = "I_ORDRE_TYPE";
            this.iORDRETYPEDataGridViewTextBoxColumn.Name = "iORDRETYPEDataGridViewTextBoxColumn";
            // 
            // iDPIONDataGridViewTextBoxColumn
            // 
            this.iDPIONDataGridViewTextBoxColumn.DataPropertyName = "ID_PION";
            this.iDPIONDataGridViewTextBoxColumn.HeaderText = "ID_PION";
            this.iDPIONDataGridViewTextBoxColumn.Name = "iDPIONDataGridViewTextBoxColumn";
            // 
            // iDCASEDEPARTDataGridViewTextBoxColumn
            // 
            this.iDCASEDEPARTDataGridViewTextBoxColumn.DataPropertyName = "ID_CASE_DEPART";
            this.iDCASEDEPARTDataGridViewTextBoxColumn.HeaderText = "ID_CASE_DEPART";
            this.iDCASEDEPARTDataGridViewTextBoxColumn.Name = "iDCASEDEPARTDataGridViewTextBoxColumn";
            // 
            // iEFFECTIFDEPARTDataGridViewTextBoxColumn
            // 
            this.iEFFECTIFDEPARTDataGridViewTextBoxColumn.DataPropertyName = "I_EFFECTIF_DEPART";
            this.iEFFECTIFDEPARTDataGridViewTextBoxColumn.HeaderText = "I_EFFECTIF_DEPART";
            this.iEFFECTIFDEPARTDataGridViewTextBoxColumn.Name = "iEFFECTIFDEPARTDataGridViewTextBoxColumn";
            // 
            // iDCASEDESTINATIONDataGridViewTextBoxColumn
            // 
            this.iDCASEDESTINATIONDataGridViewTextBoxColumn.DataPropertyName = "ID_CASE_DESTINATION";
            this.iDCASEDESTINATIONDataGridViewTextBoxColumn.HeaderText = "ID_CASE_DESTINATION";
            this.iDCASEDESTINATIONDataGridViewTextBoxColumn.Name = "iDCASEDESTINATIONDataGridViewTextBoxColumn";
            // 
            // iDNOMDESTINATIONDataGridViewTextBoxColumn
            // 
            this.iDNOMDESTINATIONDataGridViewTextBoxColumn.DataPropertyName = "ID_NOM_DESTINATION";
            this.iDNOMDESTINATIONDataGridViewTextBoxColumn.HeaderText = "ID_NOM_DESTINATION";
            this.iDNOMDESTINATIONDataGridViewTextBoxColumn.Name = "iDNOMDESTINATIONDataGridViewTextBoxColumn";
            // 
            // iEFFECTIFDESTINATIONDataGridViewTextBoxColumn
            // 
            this.iEFFECTIFDESTINATIONDataGridViewTextBoxColumn.DataPropertyName = "I_EFFECTIF_DESTINATION";
            this.iEFFECTIFDESTINATIONDataGridViewTextBoxColumn.HeaderText = "I_EFFECTIF_DESTINATION";
            this.iEFFECTIFDESTINATIONDataGridViewTextBoxColumn.Name = "iEFFECTIFDESTINATIONDataGridViewTextBoxColumn";
            // 
            // iTOURDEBUTDataGridViewTextBoxColumn
            // 
            this.iTOURDEBUTDataGridViewTextBoxColumn.DataPropertyName = "I_TOUR_DEBUT";
            this.iTOURDEBUTDataGridViewTextBoxColumn.HeaderText = "I_TOUR_DEBUT";
            this.iTOURDEBUTDataGridViewTextBoxColumn.Name = "iTOURDEBUTDataGridViewTextBoxColumn";
            // 
            // iPHASEDEBUTDataGridViewTextBoxColumn
            // 
            this.iPHASEDEBUTDataGridViewTextBoxColumn.DataPropertyName = "I_PHASE_DEBUT";
            this.iPHASEDEBUTDataGridViewTextBoxColumn.HeaderText = "I_PHASE_DEBUT";
            this.iPHASEDEBUTDataGridViewTextBoxColumn.Name = "iPHASEDEBUTDataGridViewTextBoxColumn";
            // 
            // iTOURFINDataGridViewTextBoxColumn
            // 
            this.iTOURFINDataGridViewTextBoxColumn.DataPropertyName = "I_TOUR_FIN";
            this.iTOURFINDataGridViewTextBoxColumn.HeaderText = "I_TOUR_FIN";
            this.iTOURFINDataGridViewTextBoxColumn.Name = "iTOURFINDataGridViewTextBoxColumn";
            // 
            // iPHASEFINDataGridViewTextBoxColumn
            // 
            this.iPHASEFINDataGridViewTextBoxColumn.DataPropertyName = "I_PHASE_FIN";
            this.iPHASEFINDataGridViewTextBoxColumn.HeaderText = "I_PHASE_FIN";
            this.iPHASEFINDataGridViewTextBoxColumn.Name = "iPHASEFINDataGridViewTextBoxColumn";
            // 
            // iDMESSAGEDataGridViewTextBoxColumn
            // 
            this.iDMESSAGEDataGridViewTextBoxColumn.DataPropertyName = "ID_MESSAGE";
            this.iDMESSAGEDataGridViewTextBoxColumn.HeaderText = "ID_MESSAGE";
            this.iDMESSAGEDataGridViewTextBoxColumn.Name = "iDMESSAGEDataGridViewTextBoxColumn";
            // 
            // iDDESTINATAIREDataGridViewTextBoxColumn
            // 
            this.iDDESTINATAIREDataGridViewTextBoxColumn.DataPropertyName = "ID_DESTINATAIRE";
            this.iDDESTINATAIREDataGridViewTextBoxColumn.HeaderText = "ID_DESTINATAIRE";
            this.iDDESTINATAIREDataGridViewTextBoxColumn.Name = "iDDESTINATAIREDataGridViewTextBoxColumn";
            // 
            // iDCIBLEDataGridViewTextBoxColumn
            // 
            this.iDCIBLEDataGridViewTextBoxColumn.DataPropertyName = "ID_CIBLE";
            this.iDCIBLEDataGridViewTextBoxColumn.HeaderText = "ID_CIBLE";
            this.iDCIBLEDataGridViewTextBoxColumn.Name = "iDCIBLEDataGridViewTextBoxColumn";
            // 
            // iDDESTINATAIRECIBLEDataGridViewTextBoxColumn
            // 
            this.iDDESTINATAIRECIBLEDataGridViewTextBoxColumn.DataPropertyName = "ID_DESTINATAIRE_CIBLE";
            this.iDDESTINATAIRECIBLEDataGridViewTextBoxColumn.HeaderText = "ID_DESTINATAIRE_CIBLE";
            this.iDDESTINATAIRECIBLEDataGridViewTextBoxColumn.Name = "iDDESTINATAIRECIBLEDataGridViewTextBoxColumn";
            // 
            // iDBATAILLEDataGridViewTextBoxColumn
            // 
            this.iDBATAILLEDataGridViewTextBoxColumn.DataPropertyName = "ID_BATAILLE";
            this.iDBATAILLEDataGridViewTextBoxColumn.HeaderText = "ID_BATAILLE";
            this.iDBATAILLEDataGridViewTextBoxColumn.Name = "iDBATAILLEDataGridViewTextBoxColumn";
            // 
            // iZONEBATAILLEDataGridViewTextBoxColumn
            // 
            this.iZONEBATAILLEDataGridViewTextBoxColumn.DataPropertyName = "I_ZONE_BATAILLE";
            this.iZONEBATAILLEDataGridViewTextBoxColumn.HeaderText = "I_ZONE_BATAILLE";
            this.iZONEBATAILLEDataGridViewTextBoxColumn.Name = "iZONEBATAILLEDataGridViewTextBoxColumn";
            // 
            // iHEUREDEBUTDataGridViewTextBoxColumn
            // 
            this.iHEUREDEBUTDataGridViewTextBoxColumn.DataPropertyName = "I_HEURE_DEBUT";
            this.iHEUREDEBUTDataGridViewTextBoxColumn.HeaderText = "I_HEURE_DEBUT";
            this.iHEUREDEBUTDataGridViewTextBoxColumn.Name = "iHEUREDEBUTDataGridViewTextBoxColumn";
            // 
            // iDUREEDataGridViewTextBoxColumn
            // 
            this.iDUREEDataGridViewTextBoxColumn.DataPropertyName = "I_DUREE";
            this.iDUREEDataGridViewTextBoxColumn.HeaderText = "I_DUREE";
            this.iDUREEDataGridViewTextBoxColumn.Name = "iDUREEDataGridViewTextBoxColumn";
            // 
            // iENGAGEMENTDataGridViewTextBoxColumn
            // 
            this.iENGAGEMENTDataGridViewTextBoxColumn.DataPropertyName = "I_ENGAGEMENT";
            this.iENGAGEMENTDataGridViewTextBoxColumn.HeaderText = "I_ENGAGEMENT";
            this.iENGAGEMENTDataGridViewTextBoxColumn.Name = "iENGAGEMENTDataGridViewTextBoxColumn";
            // 
            // FormOrdre
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(911, 334);
            this.Controls.Add(this.dataGridViewOrdres);
            this.Controls.Add(this.buttonAnnuler);
            this.Controls.Add(this.buttonValider);
            this.Name = "FormOrdre";
            this.Text = "FormOrdre";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.FormOrdre_Load);
            this.Resize += new System.EventHandler(this.FormOrdre_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewOrdres)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.OrdresBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSetCoutDonnees)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonAnnuler;
        private System.Windows.Forms.Button buttonValider;
        private System.Windows.Forms.DataGridView dataGridViewOrdres;
        private Donnees dataSetCoutDonnees;
        private System.Windows.Forms.BindingSource OrdresBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn iDORDREDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iDORDRETRANSMISDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iDORDRESUIVANTDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iDORDREWEBDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iORDRETYPEDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iDPIONDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iDCASEDEPARTDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iEFFECTIFDEPARTDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iDCASEDESTINATIONDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iDNOMDESTINATIONDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iEFFECTIFDESTINATIONDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iTOURDEBUTDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iPHASEDEBUTDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iTOURFINDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iPHASEFINDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iDMESSAGEDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iDDESTINATAIREDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iDCIBLEDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iDDESTINATAIRECIBLEDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iDBATAILLEDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iZONEBATAILLEDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iHEUREDEBUTDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iDUREEDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iENGAGEMENTDataGridViewTextBoxColumn;
        //private System.Windows.Forms.DataGridViewTextBoxColumn iHEUREFINDataGridViewTextBoxColumn;
    }
}