
namespace vaoc
{
    partial class FormBataillesVideo
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
            this.dataSetCoutDonnees = new vaoc.Donnees();
            this.bindingSourceTAB_BATAILLE_VIDEO = new System.Windows.Forms.BindingSource(this.components);
            this.bindingSourceTAB_BATAILLE_PIONS_VIDEO = new System.Windows.Forms.BindingSource(this.components);
            this.dataGridViewBatailles = new System.Windows.Forms.DataGridView();
            this.dataGridViewBataillesPions = new System.Windows.Forms.DataGridView();
            this.iDBATAILLEDataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iTOURDataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iDPIONDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iDPROPRIETAIREDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sNOMDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iDNATIONDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bENGAGEEDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.bENDEFENSEDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.iINFANTERIEDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iCAVALERIEDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iARTILLERIEDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iMORALDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iFATIGUEDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bRETRAITEDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.bENGAGEMENTDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.iZONEBATAILLEENGAGEMENTDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iDBATAILLEDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iTOURDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iDLEADER012DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iDLEADER345DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iENGAGEMENT0DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iENGAGEMENT1DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iENGAGEMENT2DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iENGAGEMENT3DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iENGAGEMENT4DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iENGAGEMENT5DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sCOMBAT0DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sCOMBAT1DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sCOMBAT2DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sCOMBAT3DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sCOMBAT4DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sCOMBAT5DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iPERTES0DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iPERTES1DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iPERTES2DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iPERTES3DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iPERTES4DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iPERTES5DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.S_FIN = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataSetCoutDonnees)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSourceTAB_BATAILLE_VIDEO)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSourceTAB_BATAILLE_PIONS_VIDEO)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewBatailles)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewBataillesPions)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonAnnuler
            // 
            this.buttonAnnuler.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonAnnuler.Location = new System.Drawing.Point(275, 417);
            this.buttonAnnuler.Name = "buttonAnnuler";
            this.buttonAnnuler.Size = new System.Drawing.Size(75, 23);
            this.buttonAnnuler.TabIndex = 16;
            this.buttonAnnuler.Text = "Annuler";
            this.buttonAnnuler.UseVisualStyleBackColor = true;
            // 
            // buttonValider
            // 
            this.buttonValider.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonValider.Location = new System.Drawing.Point(194, 417);
            this.buttonValider.Name = "buttonValider";
            this.buttonValider.Size = new System.Drawing.Size(75, 23);
            this.buttonValider.TabIndex = 15;
            this.buttonValider.Text = "Valider";
            this.buttonValider.UseVisualStyleBackColor = true;
            // 
            // dataSetCoutDonnees
            // 
            this.dataSetCoutDonnees.DataSetName = "DataSetCoutDonnees";
            this.dataSetCoutDonnees.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // bindingSourceTAB_BATAILLE_VIDEO
            // 
            this.bindingSourceTAB_BATAILLE_VIDEO.DataMember = "TAB_BATAILLE_VIDEO";
            this.bindingSourceTAB_BATAILLE_VIDEO.DataSource = this.dataSetCoutDonnees;
            // 
            // bindingSourceTAB_BATAILLE_PIONS_VIDEO
            // 
            this.bindingSourceTAB_BATAILLE_PIONS_VIDEO.DataMember = "TAB_BATAILLE_PIONS_VIDEO";
            this.bindingSourceTAB_BATAILLE_PIONS_VIDEO.DataSource = this.dataSetCoutDonnees;
            // 
            // dataGridViewBatailles
            // 
            this.dataGridViewBatailles.AutoGenerateColumns = false;
            this.dataGridViewBatailles.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewBatailles.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.iDBATAILLEDataGridViewTextBoxColumn,
            this.iTOURDataGridViewTextBoxColumn,
            this.iDLEADER012DataGridViewTextBoxColumn,
            this.iDLEADER345DataGridViewTextBoxColumn,
            this.iENGAGEMENT0DataGridViewTextBoxColumn,
            this.iENGAGEMENT1DataGridViewTextBoxColumn,
            this.iENGAGEMENT2DataGridViewTextBoxColumn,
            this.iENGAGEMENT3DataGridViewTextBoxColumn,
            this.iENGAGEMENT4DataGridViewTextBoxColumn,
            this.iENGAGEMENT5DataGridViewTextBoxColumn,
            this.sCOMBAT0DataGridViewTextBoxColumn,
            this.sCOMBAT1DataGridViewTextBoxColumn,
            this.sCOMBAT2DataGridViewTextBoxColumn,
            this.sCOMBAT3DataGridViewTextBoxColumn,
            this.sCOMBAT4DataGridViewTextBoxColumn,
            this.sCOMBAT5DataGridViewTextBoxColumn,
            this.iPERTES0DataGridViewTextBoxColumn,
            this.iPERTES1DataGridViewTextBoxColumn,
            this.iPERTES2DataGridViewTextBoxColumn,
            this.iPERTES3DataGridViewTextBoxColumn,
            this.iPERTES4DataGridViewTextBoxColumn,
            this.iPERTES5DataGridViewTextBoxColumn,
            this.S_FIN});
            this.dataGridViewBatailles.DataSource = this.bindingSourceTAB_BATAILLE_VIDEO;
            this.dataGridViewBatailles.Location = new System.Drawing.Point(11, 11);
            this.dataGridViewBatailles.Margin = new System.Windows.Forms.Padding(2);
            this.dataGridViewBatailles.Name = "dataGridViewBatailles";
            this.dataGridViewBatailles.RowHeadersWidth = 62;
            this.dataGridViewBatailles.RowTemplate.Height = 28;
            this.dataGridViewBatailles.Size = new System.Drawing.Size(905, 97);
            this.dataGridViewBatailles.TabIndex = 17;
            // 
            // dataGridViewBataillesPions
            // 
            this.dataGridViewBataillesPions.AutoGenerateColumns = false;
            this.dataGridViewBataillesPions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewBataillesPions.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.iDBATAILLEDataGridViewTextBoxColumn1,
            this.iTOURDataGridViewTextBoxColumn1,
            this.iDPIONDataGridViewTextBoxColumn,
            this.iDPROPRIETAIREDataGridViewTextBoxColumn,
            this.sNOMDataGridViewTextBoxColumn,
            this.iDNATIONDataGridViewTextBoxColumn,
            this.bENGAGEEDataGridViewCheckBoxColumn,
            this.bENDEFENSEDataGridViewCheckBoxColumn,
            this.iINFANTERIEDataGridViewTextBoxColumn,
            this.iCAVALERIEDataGridViewTextBoxColumn,
            this.iARTILLERIEDataGridViewTextBoxColumn,
            this.iMORALDataGridViewTextBoxColumn,
            this.iFATIGUEDataGridViewTextBoxColumn,
            this.bRETRAITEDataGridViewCheckBoxColumn,
            this.bENGAGEMENTDataGridViewCheckBoxColumn,
            this.iZONEBATAILLEENGAGEMENTDataGridViewTextBoxColumn});
            this.dataGridViewBataillesPions.DataSource = this.bindingSourceTAB_BATAILLE_PIONS_VIDEO;
            this.dataGridViewBataillesPions.Location = new System.Drawing.Point(11, 121);
            this.dataGridViewBataillesPions.Margin = new System.Windows.Forms.Padding(2);
            this.dataGridViewBataillesPions.Name = "dataGridViewBataillesPions";
            this.dataGridViewBataillesPions.RowHeadersWidth = 62;
            this.dataGridViewBataillesPions.RowTemplate.Height = 28;
            this.dataGridViewBataillesPions.Size = new System.Drawing.Size(923, 97);
            this.dataGridViewBataillesPions.TabIndex = 18;
            // 
            // iDBATAILLEDataGridViewTextBoxColumn1
            // 
            this.iDBATAILLEDataGridViewTextBoxColumn1.DataPropertyName = "ID_BATAILLE";
            this.iDBATAILLEDataGridViewTextBoxColumn1.HeaderText = "ID_BATAILLE";
            this.iDBATAILLEDataGridViewTextBoxColumn1.MinimumWidth = 8;
            this.iDBATAILLEDataGridViewTextBoxColumn1.Name = "iDBATAILLEDataGridViewTextBoxColumn1";
            this.iDBATAILLEDataGridViewTextBoxColumn1.Width = 150;
            // 
            // iTOURDataGridViewTextBoxColumn1
            // 
            this.iTOURDataGridViewTextBoxColumn1.DataPropertyName = "I_TOUR";
            this.iTOURDataGridViewTextBoxColumn1.HeaderText = "I_TOUR";
            this.iTOURDataGridViewTextBoxColumn1.MinimumWidth = 8;
            this.iTOURDataGridViewTextBoxColumn1.Name = "iTOURDataGridViewTextBoxColumn1";
            this.iTOURDataGridViewTextBoxColumn1.Width = 150;
            // 
            // iDPIONDataGridViewTextBoxColumn
            // 
            this.iDPIONDataGridViewTextBoxColumn.DataPropertyName = "ID_PION";
            this.iDPIONDataGridViewTextBoxColumn.HeaderText = "ID_PION";
            this.iDPIONDataGridViewTextBoxColumn.MinimumWidth = 8;
            this.iDPIONDataGridViewTextBoxColumn.Name = "iDPIONDataGridViewTextBoxColumn";
            this.iDPIONDataGridViewTextBoxColumn.Width = 150;
            // 
            // iDPROPRIETAIREDataGridViewTextBoxColumn
            // 
            this.iDPROPRIETAIREDataGridViewTextBoxColumn.DataPropertyName = "ID_PROPRIETAIRE";
            this.iDPROPRIETAIREDataGridViewTextBoxColumn.HeaderText = "ID_PROPRIETAIRE";
            this.iDPROPRIETAIREDataGridViewTextBoxColumn.MinimumWidth = 8;
            this.iDPROPRIETAIREDataGridViewTextBoxColumn.Name = "iDPROPRIETAIREDataGridViewTextBoxColumn";
            this.iDPROPRIETAIREDataGridViewTextBoxColumn.Width = 150;
            // 
            // sNOMDataGridViewTextBoxColumn
            // 
            this.sNOMDataGridViewTextBoxColumn.DataPropertyName = "S_NOM";
            this.sNOMDataGridViewTextBoxColumn.HeaderText = "S_NOM";
            this.sNOMDataGridViewTextBoxColumn.MinimumWidth = 8;
            this.sNOMDataGridViewTextBoxColumn.Name = "sNOMDataGridViewTextBoxColumn";
            this.sNOMDataGridViewTextBoxColumn.Width = 150;
            // 
            // iDNATIONDataGridViewTextBoxColumn
            // 
            this.iDNATIONDataGridViewTextBoxColumn.DataPropertyName = "ID_NATION";
            this.iDNATIONDataGridViewTextBoxColumn.HeaderText = "ID_NATION";
            this.iDNATIONDataGridViewTextBoxColumn.MinimumWidth = 8;
            this.iDNATIONDataGridViewTextBoxColumn.Name = "iDNATIONDataGridViewTextBoxColumn";
            this.iDNATIONDataGridViewTextBoxColumn.Width = 150;
            // 
            // bENGAGEEDataGridViewCheckBoxColumn
            // 
            this.bENGAGEEDataGridViewCheckBoxColumn.DataPropertyName = "B_ENGAGEE";
            this.bENGAGEEDataGridViewCheckBoxColumn.HeaderText = "B_ENGAGEE";
            this.bENGAGEEDataGridViewCheckBoxColumn.MinimumWidth = 8;
            this.bENGAGEEDataGridViewCheckBoxColumn.Name = "bENGAGEEDataGridViewCheckBoxColumn";
            this.bENGAGEEDataGridViewCheckBoxColumn.Width = 150;
            // 
            // bENDEFENSEDataGridViewCheckBoxColumn
            // 
            this.bENDEFENSEDataGridViewCheckBoxColumn.DataPropertyName = "B_EN_DEFENSE";
            this.bENDEFENSEDataGridViewCheckBoxColumn.HeaderText = "B_EN_DEFENSE";
            this.bENDEFENSEDataGridViewCheckBoxColumn.MinimumWidth = 8;
            this.bENDEFENSEDataGridViewCheckBoxColumn.Name = "bENDEFENSEDataGridViewCheckBoxColumn";
            this.bENDEFENSEDataGridViewCheckBoxColumn.Width = 150;
            // 
            // iINFANTERIEDataGridViewTextBoxColumn
            // 
            this.iINFANTERIEDataGridViewTextBoxColumn.DataPropertyName = "I_INFANTERIE";
            this.iINFANTERIEDataGridViewTextBoxColumn.HeaderText = "I_INFANTERIE";
            this.iINFANTERIEDataGridViewTextBoxColumn.MinimumWidth = 8;
            this.iINFANTERIEDataGridViewTextBoxColumn.Name = "iINFANTERIEDataGridViewTextBoxColumn";
            this.iINFANTERIEDataGridViewTextBoxColumn.Width = 150;
            // 
            // iCAVALERIEDataGridViewTextBoxColumn
            // 
            this.iCAVALERIEDataGridViewTextBoxColumn.DataPropertyName = "I_CAVALERIE";
            this.iCAVALERIEDataGridViewTextBoxColumn.HeaderText = "I_CAVALERIE";
            this.iCAVALERIEDataGridViewTextBoxColumn.MinimumWidth = 8;
            this.iCAVALERIEDataGridViewTextBoxColumn.Name = "iCAVALERIEDataGridViewTextBoxColumn";
            this.iCAVALERIEDataGridViewTextBoxColumn.Width = 150;
            // 
            // iARTILLERIEDataGridViewTextBoxColumn
            // 
            this.iARTILLERIEDataGridViewTextBoxColumn.DataPropertyName = "I_ARTILLERIE";
            this.iARTILLERIEDataGridViewTextBoxColumn.HeaderText = "I_ARTILLERIE";
            this.iARTILLERIEDataGridViewTextBoxColumn.MinimumWidth = 8;
            this.iARTILLERIEDataGridViewTextBoxColumn.Name = "iARTILLERIEDataGridViewTextBoxColumn";
            this.iARTILLERIEDataGridViewTextBoxColumn.Width = 150;
            // 
            // iMORALDataGridViewTextBoxColumn
            // 
            this.iMORALDataGridViewTextBoxColumn.DataPropertyName = "I_MORAL";
            this.iMORALDataGridViewTextBoxColumn.HeaderText = "I_MORAL";
            this.iMORALDataGridViewTextBoxColumn.MinimumWidth = 8;
            this.iMORALDataGridViewTextBoxColumn.Name = "iMORALDataGridViewTextBoxColumn";
            this.iMORALDataGridViewTextBoxColumn.Width = 150;
            // 
            // iFATIGUEDataGridViewTextBoxColumn
            // 
            this.iFATIGUEDataGridViewTextBoxColumn.DataPropertyName = "I_FATIGUE";
            this.iFATIGUEDataGridViewTextBoxColumn.HeaderText = "I_FATIGUE";
            this.iFATIGUEDataGridViewTextBoxColumn.MinimumWidth = 8;
            this.iFATIGUEDataGridViewTextBoxColumn.Name = "iFATIGUEDataGridViewTextBoxColumn";
            this.iFATIGUEDataGridViewTextBoxColumn.Width = 150;
            // 
            // bRETRAITEDataGridViewCheckBoxColumn
            // 
            this.bRETRAITEDataGridViewCheckBoxColumn.DataPropertyName = "B_RETRAITE";
            this.bRETRAITEDataGridViewCheckBoxColumn.HeaderText = "B_RETRAITE";
            this.bRETRAITEDataGridViewCheckBoxColumn.MinimumWidth = 8;
            this.bRETRAITEDataGridViewCheckBoxColumn.Name = "bRETRAITEDataGridViewCheckBoxColumn";
            this.bRETRAITEDataGridViewCheckBoxColumn.Width = 150;
            // 
            // bENGAGEMENTDataGridViewCheckBoxColumn
            // 
            this.bENGAGEMENTDataGridViewCheckBoxColumn.DataPropertyName = "B_ENGAGEMENT";
            this.bENGAGEMENTDataGridViewCheckBoxColumn.HeaderText = "B_ENGAGEMENT";
            this.bENGAGEMENTDataGridViewCheckBoxColumn.MinimumWidth = 8;
            this.bENGAGEMENTDataGridViewCheckBoxColumn.Name = "bENGAGEMENTDataGridViewCheckBoxColumn";
            this.bENGAGEMENTDataGridViewCheckBoxColumn.Width = 150;
            // 
            // iZONEBATAILLEENGAGEMENTDataGridViewTextBoxColumn
            // 
            this.iZONEBATAILLEENGAGEMENTDataGridViewTextBoxColumn.DataPropertyName = "I_ZONE_BATAILLE_ENGAGEMENT";
            this.iZONEBATAILLEENGAGEMENTDataGridViewTextBoxColumn.HeaderText = "I_ZONE_BATAILLE_ENGAGEMENT";
            this.iZONEBATAILLEENGAGEMENTDataGridViewTextBoxColumn.MinimumWidth = 8;
            this.iZONEBATAILLEENGAGEMENTDataGridViewTextBoxColumn.Name = "iZONEBATAILLEENGAGEMENTDataGridViewTextBoxColumn";
            this.iZONEBATAILLEENGAGEMENTDataGridViewTextBoxColumn.Width = 150;
            // 
            // iDBATAILLEDataGridViewTextBoxColumn
            // 
            this.iDBATAILLEDataGridViewTextBoxColumn.DataPropertyName = "ID_BATAILLE";
            this.iDBATAILLEDataGridViewTextBoxColumn.HeaderText = "ID_BATAILLE";
            this.iDBATAILLEDataGridViewTextBoxColumn.MinimumWidth = 8;
            this.iDBATAILLEDataGridViewTextBoxColumn.Name = "iDBATAILLEDataGridViewTextBoxColumn";
            this.iDBATAILLEDataGridViewTextBoxColumn.Width = 150;
            // 
            // iTOURDataGridViewTextBoxColumn
            // 
            this.iTOURDataGridViewTextBoxColumn.DataPropertyName = "I_TOUR";
            this.iTOURDataGridViewTextBoxColumn.HeaderText = "I_TOUR";
            this.iTOURDataGridViewTextBoxColumn.MinimumWidth = 8;
            this.iTOURDataGridViewTextBoxColumn.Name = "iTOURDataGridViewTextBoxColumn";
            this.iTOURDataGridViewTextBoxColumn.Width = 150;
            // 
            // iDLEADER012DataGridViewTextBoxColumn
            // 
            this.iDLEADER012DataGridViewTextBoxColumn.DataPropertyName = "ID_LEADER_012";
            this.iDLEADER012DataGridViewTextBoxColumn.HeaderText = "ID_LEADER_012";
            this.iDLEADER012DataGridViewTextBoxColumn.MinimumWidth = 8;
            this.iDLEADER012DataGridViewTextBoxColumn.Name = "iDLEADER012DataGridViewTextBoxColumn";
            this.iDLEADER012DataGridViewTextBoxColumn.Width = 150;
            // 
            // iDLEADER345DataGridViewTextBoxColumn
            // 
            this.iDLEADER345DataGridViewTextBoxColumn.DataPropertyName = "ID_LEADER_345";
            this.iDLEADER345DataGridViewTextBoxColumn.HeaderText = "ID_LEADER_345";
            this.iDLEADER345DataGridViewTextBoxColumn.MinimumWidth = 8;
            this.iDLEADER345DataGridViewTextBoxColumn.Name = "iDLEADER345DataGridViewTextBoxColumn";
            this.iDLEADER345DataGridViewTextBoxColumn.Width = 150;
            // 
            // iENGAGEMENT0DataGridViewTextBoxColumn
            // 
            this.iENGAGEMENT0DataGridViewTextBoxColumn.DataPropertyName = "I_ENGAGEMENT_0";
            this.iENGAGEMENT0DataGridViewTextBoxColumn.HeaderText = "I_ENGAGEMENT_0";
            this.iENGAGEMENT0DataGridViewTextBoxColumn.MinimumWidth = 8;
            this.iENGAGEMENT0DataGridViewTextBoxColumn.Name = "iENGAGEMENT0DataGridViewTextBoxColumn";
            this.iENGAGEMENT0DataGridViewTextBoxColumn.Width = 150;
            // 
            // iENGAGEMENT1DataGridViewTextBoxColumn
            // 
            this.iENGAGEMENT1DataGridViewTextBoxColumn.DataPropertyName = "I_ENGAGEMENT_1";
            this.iENGAGEMENT1DataGridViewTextBoxColumn.HeaderText = "I_ENGAGEMENT_1";
            this.iENGAGEMENT1DataGridViewTextBoxColumn.MinimumWidth = 8;
            this.iENGAGEMENT1DataGridViewTextBoxColumn.Name = "iENGAGEMENT1DataGridViewTextBoxColumn";
            this.iENGAGEMENT1DataGridViewTextBoxColumn.Width = 150;
            // 
            // iENGAGEMENT2DataGridViewTextBoxColumn
            // 
            this.iENGAGEMENT2DataGridViewTextBoxColumn.DataPropertyName = "I_ENGAGEMENT_2";
            this.iENGAGEMENT2DataGridViewTextBoxColumn.HeaderText = "I_ENGAGEMENT_2";
            this.iENGAGEMENT2DataGridViewTextBoxColumn.MinimumWidth = 8;
            this.iENGAGEMENT2DataGridViewTextBoxColumn.Name = "iENGAGEMENT2DataGridViewTextBoxColumn";
            this.iENGAGEMENT2DataGridViewTextBoxColumn.Width = 150;
            // 
            // iENGAGEMENT3DataGridViewTextBoxColumn
            // 
            this.iENGAGEMENT3DataGridViewTextBoxColumn.DataPropertyName = "I_ENGAGEMENT_3";
            this.iENGAGEMENT3DataGridViewTextBoxColumn.HeaderText = "I_ENGAGEMENT_3";
            this.iENGAGEMENT3DataGridViewTextBoxColumn.MinimumWidth = 8;
            this.iENGAGEMENT3DataGridViewTextBoxColumn.Name = "iENGAGEMENT3DataGridViewTextBoxColumn";
            this.iENGAGEMENT3DataGridViewTextBoxColumn.Width = 150;
            // 
            // iENGAGEMENT4DataGridViewTextBoxColumn
            // 
            this.iENGAGEMENT4DataGridViewTextBoxColumn.DataPropertyName = "I_ENGAGEMENT_4";
            this.iENGAGEMENT4DataGridViewTextBoxColumn.HeaderText = "I_ENGAGEMENT_4";
            this.iENGAGEMENT4DataGridViewTextBoxColumn.MinimumWidth = 8;
            this.iENGAGEMENT4DataGridViewTextBoxColumn.Name = "iENGAGEMENT4DataGridViewTextBoxColumn";
            this.iENGAGEMENT4DataGridViewTextBoxColumn.Width = 150;
            // 
            // iENGAGEMENT5DataGridViewTextBoxColumn
            // 
            this.iENGAGEMENT5DataGridViewTextBoxColumn.DataPropertyName = "I_ENGAGEMENT_5";
            this.iENGAGEMENT5DataGridViewTextBoxColumn.HeaderText = "I_ENGAGEMENT_5";
            this.iENGAGEMENT5DataGridViewTextBoxColumn.MinimumWidth = 8;
            this.iENGAGEMENT5DataGridViewTextBoxColumn.Name = "iENGAGEMENT5DataGridViewTextBoxColumn";
            this.iENGAGEMENT5DataGridViewTextBoxColumn.Width = 150;
            // 
            // sCOMBAT0DataGridViewTextBoxColumn
            // 
            this.sCOMBAT0DataGridViewTextBoxColumn.DataPropertyName = "S_COMBAT_0";
            this.sCOMBAT0DataGridViewTextBoxColumn.HeaderText = "S_COMBAT_0";
            this.sCOMBAT0DataGridViewTextBoxColumn.MinimumWidth = 8;
            this.sCOMBAT0DataGridViewTextBoxColumn.Name = "sCOMBAT0DataGridViewTextBoxColumn";
            this.sCOMBAT0DataGridViewTextBoxColumn.Width = 150;
            // 
            // sCOMBAT1DataGridViewTextBoxColumn
            // 
            this.sCOMBAT1DataGridViewTextBoxColumn.DataPropertyName = "S_COMBAT_1";
            this.sCOMBAT1DataGridViewTextBoxColumn.HeaderText = "S_COMBAT_1";
            this.sCOMBAT1DataGridViewTextBoxColumn.MinimumWidth = 8;
            this.sCOMBAT1DataGridViewTextBoxColumn.Name = "sCOMBAT1DataGridViewTextBoxColumn";
            this.sCOMBAT1DataGridViewTextBoxColumn.Width = 150;
            // 
            // sCOMBAT2DataGridViewTextBoxColumn
            // 
            this.sCOMBAT2DataGridViewTextBoxColumn.DataPropertyName = "S_COMBAT_2";
            this.sCOMBAT2DataGridViewTextBoxColumn.HeaderText = "S_COMBAT_2";
            this.sCOMBAT2DataGridViewTextBoxColumn.MinimumWidth = 8;
            this.sCOMBAT2DataGridViewTextBoxColumn.Name = "sCOMBAT2DataGridViewTextBoxColumn";
            this.sCOMBAT2DataGridViewTextBoxColumn.Width = 150;
            // 
            // sCOMBAT3DataGridViewTextBoxColumn
            // 
            this.sCOMBAT3DataGridViewTextBoxColumn.DataPropertyName = "S_COMBAT_3";
            this.sCOMBAT3DataGridViewTextBoxColumn.HeaderText = "S_COMBAT_3";
            this.sCOMBAT3DataGridViewTextBoxColumn.MinimumWidth = 8;
            this.sCOMBAT3DataGridViewTextBoxColumn.Name = "sCOMBAT3DataGridViewTextBoxColumn";
            this.sCOMBAT3DataGridViewTextBoxColumn.Width = 150;
            // 
            // sCOMBAT4DataGridViewTextBoxColumn
            // 
            this.sCOMBAT4DataGridViewTextBoxColumn.DataPropertyName = "S_COMBAT_4";
            this.sCOMBAT4DataGridViewTextBoxColumn.HeaderText = "S_COMBAT_4";
            this.sCOMBAT4DataGridViewTextBoxColumn.MinimumWidth = 8;
            this.sCOMBAT4DataGridViewTextBoxColumn.Name = "sCOMBAT4DataGridViewTextBoxColumn";
            this.sCOMBAT4DataGridViewTextBoxColumn.Width = 150;
            // 
            // sCOMBAT5DataGridViewTextBoxColumn
            // 
            this.sCOMBAT5DataGridViewTextBoxColumn.DataPropertyName = "S_COMBAT_5";
            this.sCOMBAT5DataGridViewTextBoxColumn.HeaderText = "S_COMBAT_5";
            this.sCOMBAT5DataGridViewTextBoxColumn.MinimumWidth = 8;
            this.sCOMBAT5DataGridViewTextBoxColumn.Name = "sCOMBAT5DataGridViewTextBoxColumn";
            this.sCOMBAT5DataGridViewTextBoxColumn.Width = 150;
            // 
            // iPERTES0DataGridViewTextBoxColumn
            // 
            this.iPERTES0DataGridViewTextBoxColumn.DataPropertyName = "I_PERTES_0";
            this.iPERTES0DataGridViewTextBoxColumn.HeaderText = "I_PERTES_0";
            this.iPERTES0DataGridViewTextBoxColumn.MinimumWidth = 8;
            this.iPERTES0DataGridViewTextBoxColumn.Name = "iPERTES0DataGridViewTextBoxColumn";
            this.iPERTES0DataGridViewTextBoxColumn.Width = 150;
            // 
            // iPERTES1DataGridViewTextBoxColumn
            // 
            this.iPERTES1DataGridViewTextBoxColumn.DataPropertyName = "I_PERTES_1";
            this.iPERTES1DataGridViewTextBoxColumn.HeaderText = "I_PERTES_1";
            this.iPERTES1DataGridViewTextBoxColumn.MinimumWidth = 8;
            this.iPERTES1DataGridViewTextBoxColumn.Name = "iPERTES1DataGridViewTextBoxColumn";
            this.iPERTES1DataGridViewTextBoxColumn.Width = 150;
            // 
            // iPERTES2DataGridViewTextBoxColumn
            // 
            this.iPERTES2DataGridViewTextBoxColumn.DataPropertyName = "I_PERTES_2";
            this.iPERTES2DataGridViewTextBoxColumn.HeaderText = "I_PERTES_2";
            this.iPERTES2DataGridViewTextBoxColumn.MinimumWidth = 8;
            this.iPERTES2DataGridViewTextBoxColumn.Name = "iPERTES2DataGridViewTextBoxColumn";
            this.iPERTES2DataGridViewTextBoxColumn.Width = 150;
            // 
            // iPERTES3DataGridViewTextBoxColumn
            // 
            this.iPERTES3DataGridViewTextBoxColumn.DataPropertyName = "I_PERTES_3";
            this.iPERTES3DataGridViewTextBoxColumn.HeaderText = "I_PERTES_3";
            this.iPERTES3DataGridViewTextBoxColumn.MinimumWidth = 8;
            this.iPERTES3DataGridViewTextBoxColumn.Name = "iPERTES3DataGridViewTextBoxColumn";
            this.iPERTES3DataGridViewTextBoxColumn.Width = 150;
            // 
            // iPERTES4DataGridViewTextBoxColumn
            // 
            this.iPERTES4DataGridViewTextBoxColumn.DataPropertyName = "I_PERTES_4";
            this.iPERTES4DataGridViewTextBoxColumn.HeaderText = "I_PERTES_4";
            this.iPERTES4DataGridViewTextBoxColumn.MinimumWidth = 8;
            this.iPERTES4DataGridViewTextBoxColumn.Name = "iPERTES4DataGridViewTextBoxColumn";
            this.iPERTES4DataGridViewTextBoxColumn.Width = 150;
            // 
            // iPERTES5DataGridViewTextBoxColumn
            // 
            this.iPERTES5DataGridViewTextBoxColumn.DataPropertyName = "I_PERTES_5";
            this.iPERTES5DataGridViewTextBoxColumn.HeaderText = "I_PERTES_5";
            this.iPERTES5DataGridViewTextBoxColumn.MinimumWidth = 8;
            this.iPERTES5DataGridViewTextBoxColumn.Name = "iPERTES5DataGridViewTextBoxColumn";
            this.iPERTES5DataGridViewTextBoxColumn.Width = 150;
            // 
            // S_FIN
            // 
            this.S_FIN.DataPropertyName = "S_FIN";
            this.S_FIN.HeaderText = "S_FIN";
            this.S_FIN.Name = "S_FIN";
            // 
            // FormBataillesVideo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1007, 541);
            this.Controls.Add(this.dataGridViewBataillesPions);
            this.Controls.Add(this.dataGridViewBatailles);
            this.Controls.Add(this.buttonAnnuler);
            this.Controls.Add(this.buttonValider);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "FormBataillesVideo";
            this.Text = "FormBataillesVideo";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.FormBataillesVideo_Load);
            this.Resize += new System.EventHandler(this.FormBataillesVideo_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.dataSetCoutDonnees)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSourceTAB_BATAILLE_VIDEO)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSourceTAB_BATAILLE_PIONS_VIDEO)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewBatailles)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewBataillesPions)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button buttonAnnuler;
        private System.Windows.Forms.Button buttonValider;
        private Donnees dataSetCoutDonnees;
        private System.Windows.Forms.BindingSource bindingSourceTAB_BATAILLE_VIDEO;
        private System.Windows.Forms.BindingSource bindingSourceTAB_BATAILLE_PIONS_VIDEO;
        private System.Windows.Forms.DataGridView dataGridViewBatailles;
        private System.Windows.Forms.DataGridView dataGridViewBataillesPions;
        private System.Windows.Forms.DataGridViewTextBoxColumn iDBATAILLEDataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn iTOURDataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn iDPIONDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iDPROPRIETAIREDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn sNOMDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iDNATIONDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn bENGAGEEDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn bENDEFENSEDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iINFANTERIEDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iCAVALERIEDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iARTILLERIEDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iMORALDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iFATIGUEDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn bRETRAITEDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn bENGAGEMENTDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iZONEBATAILLEENGAGEMENTDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iDBATAILLEDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iTOURDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iDLEADER012DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iDLEADER345DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iENGAGEMENT0DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iENGAGEMENT1DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iENGAGEMENT2DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iENGAGEMENT3DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iENGAGEMENT4DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iENGAGEMENT5DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn sCOMBAT0DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn sCOMBAT1DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn sCOMBAT2DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn sCOMBAT3DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn sCOMBAT4DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn sCOMBAT5DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iPERTES0DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iPERTES1DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iPERTES2DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iPERTES3DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iPERTES4DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iPERTES5DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn S_FIN;
    }
}