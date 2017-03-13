namespace vaoc
{
    partial class FormVideoTable
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
            this.buttonValider = new System.Windows.Forms.Button();
            this.dataGridViewVideo = new System.Windows.Forms.DataGridView();
            this.iTOURDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iDNATIONDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iDPIONDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iDMODELEPIONDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iDPIONPROPRIETAIREDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sNOMDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iINFANTERIEDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iINFANTERIEINITIALEDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iCAVALERIEDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iCAVALERIEINITIALEDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iARTILLERIEDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iARTILLERIEINITIALEDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iFATIGUEDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iMORALDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iDCASEDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iDBATAILLEDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bDETRUITDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.bFUITEAUCOMBATDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.iMATERIELDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iRAVITAILLEMENTDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bBLESSESDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.bPRISONNIERSDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.cNIVEAUDEPOTDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iVICTOIREDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tABBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.dataSetCoutDonnees = new vaoc.Donnees();
            this.buttonAnnuler = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewVideo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tABBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSetCoutDonnees)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonValider
            // 
            this.buttonValider.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonValider.Location = new System.Drawing.Point(140, 312);
            this.buttonValider.Name = "buttonValider";
            this.buttonValider.Size = new System.Drawing.Size(75, 23);
            this.buttonValider.TabIndex = 9;
            this.buttonValider.Text = "Valider";
            this.buttonValider.UseVisualStyleBackColor = true;
            // 
            // dataGridViewVideo
            // 
            this.dataGridViewVideo.AllowUserToOrderColumns = true;
            this.dataGridViewVideo.AutoGenerateColumns = false;
            this.dataGridViewVideo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewVideo.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.iTOURDataGridViewTextBoxColumn,
            this.iDNATIONDataGridViewTextBoxColumn,
            this.iDPIONDataGridViewTextBoxColumn,
            this.iDMODELEPIONDataGridViewTextBoxColumn,
            this.iDPIONPROPRIETAIREDataGridViewTextBoxColumn,
            this.sNOMDataGridViewTextBoxColumn,
            this.iINFANTERIEDataGridViewTextBoxColumn,
            this.iINFANTERIEINITIALEDataGridViewTextBoxColumn,
            this.iCAVALERIEDataGridViewTextBoxColumn,
            this.iCAVALERIEINITIALEDataGridViewTextBoxColumn,
            this.iARTILLERIEDataGridViewTextBoxColumn,
            this.iARTILLERIEINITIALEDataGridViewTextBoxColumn,
            this.iFATIGUEDataGridViewTextBoxColumn,
            this.iMORALDataGridViewTextBoxColumn,
            this.iDCASEDataGridViewTextBoxColumn,
            this.iDBATAILLEDataGridViewTextBoxColumn,
            this.bDETRUITDataGridViewCheckBoxColumn,
            this.bFUITEAUCOMBATDataGridViewCheckBoxColumn,
            this.iMATERIELDataGridViewTextBoxColumn,
            this.iRAVITAILLEMENTDataGridViewTextBoxColumn,
            this.bBLESSESDataGridViewCheckBoxColumn,
            this.bPRISONNIERSDataGridViewCheckBoxColumn,
            this.cNIVEAUDEPOTDataGridViewTextBoxColumn,
            this.iVICTOIREDataGridViewTextBoxColumn});
            this.dataGridViewVideo.DataSource = this.tABBindingSource;
            this.dataGridViewVideo.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewVideo.Name = "dataGridViewVideo";
            this.dataGridViewVideo.Size = new System.Drawing.Size(1424, 188);
            this.dataGridViewVideo.TabIndex = 10;
            // 
            // iTOURDataGridViewTextBoxColumn
            // 
            this.iTOURDataGridViewTextBoxColumn.DataPropertyName = "I_TOUR";
            this.iTOURDataGridViewTextBoxColumn.HeaderText = "I_TOUR";
            this.iTOURDataGridViewTextBoxColumn.Name = "iTOURDataGridViewTextBoxColumn";
            // 
            // iDNATIONDataGridViewTextBoxColumn
            // 
            this.iDNATIONDataGridViewTextBoxColumn.DataPropertyName = "ID_NATION";
            this.iDNATIONDataGridViewTextBoxColumn.HeaderText = "ID_NATION";
            this.iDNATIONDataGridViewTextBoxColumn.Name = "iDNATIONDataGridViewTextBoxColumn";
            // 
            // iDPIONDataGridViewTextBoxColumn
            // 
            this.iDPIONDataGridViewTextBoxColumn.DataPropertyName = "ID_PION";
            this.iDPIONDataGridViewTextBoxColumn.HeaderText = "ID_PION";
            this.iDPIONDataGridViewTextBoxColumn.Name = "iDPIONDataGridViewTextBoxColumn";
            // 
            // iDMODELEPIONDataGridViewTextBoxColumn
            // 
            this.iDMODELEPIONDataGridViewTextBoxColumn.DataPropertyName = "ID_MODELE_PION";
            this.iDMODELEPIONDataGridViewTextBoxColumn.HeaderText = "ID_MODELE_PION";
            this.iDMODELEPIONDataGridViewTextBoxColumn.Name = "iDMODELEPIONDataGridViewTextBoxColumn";
            // 
            // iDPIONPROPRIETAIREDataGridViewTextBoxColumn
            // 
            this.iDPIONPROPRIETAIREDataGridViewTextBoxColumn.DataPropertyName = "ID_PION_PROPRIETAIRE";
            this.iDPIONPROPRIETAIREDataGridViewTextBoxColumn.HeaderText = "ID_PION_PROPRIETAIRE";
            this.iDPIONPROPRIETAIREDataGridViewTextBoxColumn.Name = "iDPIONPROPRIETAIREDataGridViewTextBoxColumn";
            // 
            // sNOMDataGridViewTextBoxColumn
            // 
            this.sNOMDataGridViewTextBoxColumn.DataPropertyName = "S_NOM";
            this.sNOMDataGridViewTextBoxColumn.HeaderText = "S_NOM";
            this.sNOMDataGridViewTextBoxColumn.Name = "sNOMDataGridViewTextBoxColumn";
            // 
            // iINFANTERIEDataGridViewTextBoxColumn
            // 
            this.iINFANTERIEDataGridViewTextBoxColumn.DataPropertyName = "I_INFANTERIE";
            this.iINFANTERIEDataGridViewTextBoxColumn.HeaderText = "I_INFANTERIE";
            this.iINFANTERIEDataGridViewTextBoxColumn.Name = "iINFANTERIEDataGridViewTextBoxColumn";
            // 
            // iINFANTERIEINITIALEDataGridViewTextBoxColumn
            // 
            this.iINFANTERIEINITIALEDataGridViewTextBoxColumn.DataPropertyName = "I_INFANTERIE_INITIALE";
            this.iINFANTERIEINITIALEDataGridViewTextBoxColumn.HeaderText = "I_INFANTERIE_INITIALE";
            this.iINFANTERIEINITIALEDataGridViewTextBoxColumn.Name = "iINFANTERIEINITIALEDataGridViewTextBoxColumn";
            // 
            // iCAVALERIEDataGridViewTextBoxColumn
            // 
            this.iCAVALERIEDataGridViewTextBoxColumn.DataPropertyName = "I_CAVALERIE";
            this.iCAVALERIEDataGridViewTextBoxColumn.HeaderText = "I_CAVALERIE";
            this.iCAVALERIEDataGridViewTextBoxColumn.Name = "iCAVALERIEDataGridViewTextBoxColumn";
            // 
            // iCAVALERIEINITIALEDataGridViewTextBoxColumn
            // 
            this.iCAVALERIEINITIALEDataGridViewTextBoxColumn.DataPropertyName = "I_CAVALERIE_INITIALE";
            this.iCAVALERIEINITIALEDataGridViewTextBoxColumn.HeaderText = "I_CAVALERIE_INITIALE";
            this.iCAVALERIEINITIALEDataGridViewTextBoxColumn.Name = "iCAVALERIEINITIALEDataGridViewTextBoxColumn";
            // 
            // iARTILLERIEDataGridViewTextBoxColumn
            // 
            this.iARTILLERIEDataGridViewTextBoxColumn.DataPropertyName = "I_ARTILLERIE";
            this.iARTILLERIEDataGridViewTextBoxColumn.HeaderText = "I_ARTILLERIE";
            this.iARTILLERIEDataGridViewTextBoxColumn.Name = "iARTILLERIEDataGridViewTextBoxColumn";
            // 
            // iARTILLERIEINITIALEDataGridViewTextBoxColumn
            // 
            this.iARTILLERIEINITIALEDataGridViewTextBoxColumn.DataPropertyName = "I_ARTILLERIE_INITIALE";
            this.iARTILLERIEINITIALEDataGridViewTextBoxColumn.HeaderText = "I_ARTILLERIE_INITIALE";
            this.iARTILLERIEINITIALEDataGridViewTextBoxColumn.Name = "iARTILLERIEINITIALEDataGridViewTextBoxColumn";
            // 
            // iFATIGUEDataGridViewTextBoxColumn
            // 
            this.iFATIGUEDataGridViewTextBoxColumn.DataPropertyName = "I_FATIGUE";
            this.iFATIGUEDataGridViewTextBoxColumn.HeaderText = "I_FATIGUE";
            this.iFATIGUEDataGridViewTextBoxColumn.Name = "iFATIGUEDataGridViewTextBoxColumn";
            // 
            // iMORALDataGridViewTextBoxColumn
            // 
            this.iMORALDataGridViewTextBoxColumn.DataPropertyName = "I_MORAL";
            this.iMORALDataGridViewTextBoxColumn.HeaderText = "I_MORAL";
            this.iMORALDataGridViewTextBoxColumn.Name = "iMORALDataGridViewTextBoxColumn";
            // 
            // iDCASEDataGridViewTextBoxColumn
            // 
            this.iDCASEDataGridViewTextBoxColumn.DataPropertyName = "ID_CASE";
            this.iDCASEDataGridViewTextBoxColumn.HeaderText = "ID_CASE";
            this.iDCASEDataGridViewTextBoxColumn.Name = "iDCASEDataGridViewTextBoxColumn";
            // 
            // iDBATAILLEDataGridViewTextBoxColumn
            // 
            this.iDBATAILLEDataGridViewTextBoxColumn.DataPropertyName = "ID_BATAILLE";
            this.iDBATAILLEDataGridViewTextBoxColumn.HeaderText = "ID_BATAILLE";
            this.iDBATAILLEDataGridViewTextBoxColumn.Name = "iDBATAILLEDataGridViewTextBoxColumn";
            // 
            // bDETRUITDataGridViewCheckBoxColumn
            // 
            this.bDETRUITDataGridViewCheckBoxColumn.DataPropertyName = "B_DETRUIT";
            this.bDETRUITDataGridViewCheckBoxColumn.HeaderText = "B_DETRUIT";
            this.bDETRUITDataGridViewCheckBoxColumn.Name = "bDETRUITDataGridViewCheckBoxColumn";
            // 
            // bFUITEAUCOMBATDataGridViewCheckBoxColumn
            // 
            this.bFUITEAUCOMBATDataGridViewCheckBoxColumn.DataPropertyName = "B_FUITE_AU_COMBAT";
            this.bFUITEAUCOMBATDataGridViewCheckBoxColumn.HeaderText = "B_FUITE_AU_COMBAT";
            this.bFUITEAUCOMBATDataGridViewCheckBoxColumn.Name = "bFUITEAUCOMBATDataGridViewCheckBoxColumn";
            // 
            // iMATERIELDataGridViewTextBoxColumn
            // 
            this.iMATERIELDataGridViewTextBoxColumn.DataPropertyName = "I_MATERIEL";
            this.iMATERIELDataGridViewTextBoxColumn.HeaderText = "I_MATERIEL";
            this.iMATERIELDataGridViewTextBoxColumn.Name = "iMATERIELDataGridViewTextBoxColumn";
            // 
            // iRAVITAILLEMENTDataGridViewTextBoxColumn
            // 
            this.iRAVITAILLEMENTDataGridViewTextBoxColumn.DataPropertyName = "I_RAVITAILLEMENT";
            this.iRAVITAILLEMENTDataGridViewTextBoxColumn.HeaderText = "I_RAVITAILLEMENT";
            this.iRAVITAILLEMENTDataGridViewTextBoxColumn.Name = "iRAVITAILLEMENTDataGridViewTextBoxColumn";
            // 
            // bBLESSESDataGridViewCheckBoxColumn
            // 
            this.bBLESSESDataGridViewCheckBoxColumn.DataPropertyName = "B_BLESSES";
            this.bBLESSESDataGridViewCheckBoxColumn.HeaderText = "B_BLESSES";
            this.bBLESSESDataGridViewCheckBoxColumn.Name = "bBLESSESDataGridViewCheckBoxColumn";
            // 
            // bPRISONNIERSDataGridViewCheckBoxColumn
            // 
            this.bPRISONNIERSDataGridViewCheckBoxColumn.DataPropertyName = "B_PRISONNIERS";
            this.bPRISONNIERSDataGridViewCheckBoxColumn.HeaderText = "B_PRISONNIERS";
            this.bPRISONNIERSDataGridViewCheckBoxColumn.Name = "bPRISONNIERSDataGridViewCheckBoxColumn";
            // 
            // cNIVEAUDEPOTDataGridViewTextBoxColumn
            // 
            this.cNIVEAUDEPOTDataGridViewTextBoxColumn.DataPropertyName = "C_NIVEAU_DEPOT";
            this.cNIVEAUDEPOTDataGridViewTextBoxColumn.HeaderText = "C_NIVEAU_DEPOT";
            this.cNIVEAUDEPOTDataGridViewTextBoxColumn.Name = "cNIVEAUDEPOTDataGridViewTextBoxColumn";
            // 
            // iVICTOIREDataGridViewTextBoxColumn
            // 
            this.iVICTOIREDataGridViewTextBoxColumn.DataPropertyName = "I_VICTOIRE";
            this.iVICTOIREDataGridViewTextBoxColumn.HeaderText = "I_VICTOIRE";
            this.iVICTOIREDataGridViewTextBoxColumn.Name = "iVICTOIREDataGridViewTextBoxColumn";
            // 
            // tABBindingSource
            // 
            this.tABBindingSource.DataMember = "TAB_VIDEO";
            this.tABBindingSource.DataSource = this.dataSetCoutDonnees;
            // 
            // dataSetCoutDonnees
            // 
            this.dataSetCoutDonnees.DataSetName = "DataSetCoutDonnees";
            this.dataSetCoutDonnees.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // buttonAnnuler
            // 
            this.buttonAnnuler.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonAnnuler.Location = new System.Drawing.Point(221, 312);
            this.buttonAnnuler.Name = "buttonAnnuler";
            this.buttonAnnuler.Size = new System.Drawing.Size(75, 23);
            this.buttonAnnuler.TabIndex = 12;
            this.buttonAnnuler.Text = "Annuler";
            this.buttonAnnuler.UseVisualStyleBackColor = true;
            // 
            // FormVideoTable
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(1458, 383);
            this.Controls.Add(this.buttonAnnuler);
            this.Controls.Add(this.dataGridViewVideo);
            this.Controls.Add(this.buttonValider);
            this.Name = "FormVideoTable";
            this.Text = "FormVideoTable";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.FormVideoTable_Load);
            this.Resize += new System.EventHandler(this.FormVideoTable_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewVideo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tABBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSetCoutDonnees)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button buttonValider;
        private Donnees dataSetCoutDonnees;
        private System.Windows.Forms.BindingSource tABBindingSource;
        private System.Windows.Forms.DataGridView dataGridViewVideo;
        private System.Windows.Forms.Button buttonAnnuler;
        private System.Windows.Forms.DataGridViewTextBoxColumn iTOURDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iDNATIONDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iDPIONDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iDMODELEPIONDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iDPIONPROPRIETAIREDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn sNOMDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iINFANTERIEDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iINFANTERIEINITIALEDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iCAVALERIEDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iCAVALERIEINITIALEDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iARTILLERIEDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iARTILLERIEINITIALEDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iFATIGUEDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iMORALDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iDCASEDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iDBATAILLEDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn bDETRUITDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn bFUITEAUCOMBATDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iMATERIELDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iRAVITAILLEMENTDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn bBLESSESDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn bPRISONNIERSDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn cNIVEAUDEPOTDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iVICTOIREDataGridViewTextBoxColumn;
    }
}