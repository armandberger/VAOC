namespace vaoc
{
    partial class FormNation
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
            this.dataGridViewModelesCombats = new System.Windows.Forms.DataGridView();
            this.tABNATIONBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.dataSetCoutDonnees = new vaoc.Donnees();
            this.buttonAnnuler = new System.Windows.Forms.Button();
            this.buttonValider = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.iDNATIONDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sNOMDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iLIMITEDEPOTADataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewModelesCombats)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tABNATIONBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSetCoutDonnees)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewModelesCombats
            // 
            this.dataGridViewModelesCombats.AllowUserToOrderColumns = true;
            this.dataGridViewModelesCombats.AutoGenerateColumns = false;
            this.dataGridViewModelesCombats.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewModelesCombats.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.iDNATIONDataGridViewTextBoxColumn,
            this.sNOMDataGridViewTextBoxColumn,
            this.iLIMITEDEPOTADataGridViewTextBoxColumn});
            this.dataGridViewModelesCombats.DataSource = this.tABNATIONBindingSource;
            this.dataGridViewModelesCombats.Dock = System.Windows.Forms.DockStyle.Top;
            this.dataGridViewModelesCombats.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewModelesCombats.Name = "dataGridViewModelesCombats";
            this.dataGridViewModelesCombats.Size = new System.Drawing.Size(854, 188);
            this.dataGridViewModelesCombats.TabIndex = 3;
            // 
            // tABNATIONBindingSource
            // 
            this.tABNATIONBindingSource.DataMember = "TAB_NATION";
            this.tABNATIONBindingSource.DataSource = this.dataSetCoutDonnees;
            // 
            // dataSetCoutDonnees
            // 
            this.dataSetCoutDonnees.DataSetName = "DataSetCoutDonnees";
            this.dataSetCoutDonnees.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // buttonAnnuler
            // 
            this.buttonAnnuler.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonAnnuler.Location = new System.Drawing.Point(140, 202);
            this.buttonAnnuler.Name = "buttonAnnuler";
            this.buttonAnnuler.Size = new System.Drawing.Size(75, 23);
            this.buttonAnnuler.TabIndex = 5;
            this.buttonAnnuler.Text = "Annuler";
            this.buttonAnnuler.UseVisualStyleBackColor = true;
            // 
            // buttonValider
            // 
            this.buttonValider.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonValider.Location = new System.Drawing.Point(6, 202);
            this.buttonValider.Name = "buttonValider";
            this.buttonValider.Size = new System.Drawing.Size(75, 23);
            this.buttonValider.TabIndex = 4;
            this.buttonValider.Text = "Valider";
            this.buttonValider.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(247, 191);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(559, 23);
            this.label1.TabIndex = 6;
            this.label1.Text = "Encombrement : nombre d\'hommes necessaires pour occuper 1 km sur route (1 km2 � l" +
    "\'arr�t)";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(247, 214);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(559, 23);
            this.label2.TabIndex = 7;
            this.label2.Text = "Fourgon : pourcentage d\'encombrement li� aux fourgons";
            // 
            // iDNATIONDataGridViewTextBoxColumn
            // 
            this.iDNATIONDataGridViewTextBoxColumn.DataPropertyName = "ID_NATION";
            this.iDNATIONDataGridViewTextBoxColumn.HeaderText = "ID_NATION";
            this.iDNATIONDataGridViewTextBoxColumn.Name = "iDNATIONDataGridViewTextBoxColumn";
            // 
            // sNOMDataGridViewTextBoxColumn
            // 
            this.sNOMDataGridViewTextBoxColumn.DataPropertyName = "S_NOM";
            this.sNOMDataGridViewTextBoxColumn.HeaderText = "S_NOM";
            this.sNOMDataGridViewTextBoxColumn.Name = "sNOMDataGridViewTextBoxColumn";
            // 
            // iLIMITEDEPOTADataGridViewTextBoxColumn
            // 
            this.iLIMITEDEPOTADataGridViewTextBoxColumn.DataPropertyName = "I_LIMITE_DEPOT_A";
            this.iLIMITEDEPOTADataGridViewTextBoxColumn.HeaderText = "I_LIMITE_DEPOT_A";
            this.iLIMITEDEPOTADataGridViewTextBoxColumn.Name = "iLIMITEDEPOTADataGridViewTextBoxColumn";
            // 
            // FormNation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(854, 237);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dataGridViewModelesCombats);
            this.Controls.Add(this.buttonAnnuler);
            this.Controls.Add(this.buttonValider);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormNation";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Nations";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewModelesCombats)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tABNATIONBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSetCoutDonnees)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewModelesCombats;
        private System.Windows.Forms.Button buttonAnnuler;
        private System.Windows.Forms.Button buttonValider;
        //private System.Windows.Forms.DataGridViewTextBoxColumn iDMODELECOMBATDataGridViewTextBoxColumn;
        private Donnees dataSetCoutDonnees;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.BindingSource tABNATIONBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn iDNATIONDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn sNOMDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iLIMITEDEPOTADataGridViewTextBoxColumn;
    }
}