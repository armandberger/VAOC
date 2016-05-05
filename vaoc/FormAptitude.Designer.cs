namespace vaoc
{
    partial class FormAptitude
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
            this.dataGridViewAptitude = new System.Windows.Forms.DataGridView();
            this.iDAPTITUDEDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sNOMDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iDUREEDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tABAPTITUDESBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.dataSetCoutDonnees = new vaoc.Donnees();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAptitude)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tABAPTITUDESBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSetCoutDonnees)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonAnnuler
            // 
            this.buttonAnnuler.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonAnnuler.Location = new System.Drawing.Point(346, 296);
            this.buttonAnnuler.Name = "buttonAnnuler";
            this.buttonAnnuler.Size = new System.Drawing.Size(75, 23);
            this.buttonAnnuler.TabIndex = 2;
            this.buttonAnnuler.Text = "Annuler";
            this.buttonAnnuler.UseVisualStyleBackColor = true;
            // 
            // buttonValider
            // 
            this.buttonValider.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonValider.Location = new System.Drawing.Point(52, 298);
            this.buttonValider.Name = "buttonValider";
            this.buttonValider.Size = new System.Drawing.Size(75, 23);
            this.buttonValider.TabIndex = 3;
            this.buttonValider.Text = "Valider";
            this.buttonValider.UseVisualStyleBackColor = true;
            // 
            // dataGridViewAptitude
            // 
            this.dataGridViewAptitude.AllowUserToOrderColumns = true;
            this.dataGridViewAptitude.AutoGenerateColumns = false;
            this.dataGridViewAptitude.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewAptitude.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.iDAPTITUDEDataGridViewTextBoxColumn,
            this.sNOMDataGridViewTextBoxColumn,
            this.iDUREEDataGridViewTextBoxColumn});
            this.dataGridViewAptitude.DataSource = this.tABAPTITUDESBindingSource;
            this.dataGridViewAptitude.Location = new System.Drawing.Point(12, 12);
            this.dataGridViewAptitude.Name = "dataGridViewAptitude";
            this.dataGridViewAptitude.Size = new System.Drawing.Size(465, 278);
            this.dataGridViewAptitude.TabIndex = 4;
            // 
            // iDAPTITUDEDataGridViewTextBoxColumn
            // 
            this.iDAPTITUDEDataGridViewTextBoxColumn.DataPropertyName = "ID_APTITUDE";
            this.iDAPTITUDEDataGridViewTextBoxColumn.HeaderText = "ID_APTITUDE";
            this.iDAPTITUDEDataGridViewTextBoxColumn.Name = "iDAPTITUDEDataGridViewTextBoxColumn";
            // 
            // sNOMDataGridViewTextBoxColumn
            // 
            this.sNOMDataGridViewTextBoxColumn.DataPropertyName = "S_NOM";
            this.sNOMDataGridViewTextBoxColumn.HeaderText = "S_NOM";
            this.sNOMDataGridViewTextBoxColumn.Name = "sNOMDataGridViewTextBoxColumn";
            // 
            // iDUREEDataGridViewTextBoxColumn
            // 
            this.iDUREEDataGridViewTextBoxColumn.DataPropertyName = "I_DUREE";
            this.iDUREEDataGridViewTextBoxColumn.HeaderText = "I_DUREE";
            this.iDUREEDataGridViewTextBoxColumn.Name = "iDUREEDataGridViewTextBoxColumn";
            // 
            // tABAPTITUDESBindingSource
            // 
            this.tABAPTITUDESBindingSource.DataMember = "TAB_APTITUDES";
            this.tABAPTITUDESBindingSource.DataSource = this.dataSetCoutDonnees;
            // 
            // dataSetCoutDonnees
            // 
            this.dataSetCoutDonnees.DataSetName = "DataSetCoutDonnees";
            this.dataSetCoutDonnees.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // FormAptitude
            // 
            this.AcceptButton = this.buttonValider;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(489, 333);
            this.Controls.Add(this.dataGridViewAptitude);
            this.Controls.Add(this.buttonAnnuler);
            this.Controls.Add(this.buttonValider);
            this.Name = "FormAptitude";
            this.Text = "Aptitudes";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAptitude)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tABAPTITUDESBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSetCoutDonnees)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonAnnuler;
        private System.Windows.Forms.Button buttonValider;
        private System.Windows.Forms.DataGridView dataGridViewAptitude;
        private System.Windows.Forms.DataGridViewTextBoxColumn iDAPTITUDEDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn sNOMDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iDUREEDataGridViewTextBoxColumn;
        private System.Windows.Forms.BindingSource tABAPTITUDESBindingSource;
        private Donnees dataSetCoutDonnees;

    }
}