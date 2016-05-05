namespace vaoc
{
    partial class FormNomsPromus
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
            this.dataGridViewNomsPromus = new System.Windows.Forms.DataGridView();
            this.iDNOMPROMUDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iDNATIONDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sNOMDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bNOMPROMUDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.NomsPromusBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.dataSetCoutDonnees = new vaoc.Donnees();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewNomsPromus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NomsPromusBindingSource)).BeginInit();
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
            // dataGridViewNomsPromus
            // 
            this.dataGridViewNomsPromus.AllowUserToOrderColumns = true;
            this.dataGridViewNomsPromus.AutoGenerateColumns = false;
            this.dataGridViewNomsPromus.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewNomsPromus.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.iDNOMPROMUDataGridViewTextBoxColumn,
            this.iDNATIONDataGridViewTextBoxColumn,
            this.sNOMDataGridViewTextBoxColumn,
            this.bNOMPROMUDataGridViewCheckBoxColumn});
            this.dataGridViewNomsPromus.DataSource = this.NomsPromusBindingSource;
            this.dataGridViewNomsPromus.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewNomsPromus.Name = "dataGridViewNomsPromus";
            this.dataGridViewNomsPromus.Size = new System.Drawing.Size(899, 143);
            this.dataGridViewNomsPromus.TabIndex = 8;
            // 
            // iDNOMPROMUDataGridViewTextBoxColumn
            // 
            this.iDNOMPROMUDataGridViewTextBoxColumn.DataPropertyName = "ID_NOM_PROMU";
            this.iDNOMPROMUDataGridViewTextBoxColumn.HeaderText = "ID_NOM_PROMU";
            this.iDNOMPROMUDataGridViewTextBoxColumn.Name = "iDNOMPROMUDataGridViewTextBoxColumn";
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
            // bNOMPROMUDataGridViewCheckBoxColumn
            // 
            this.bNOMPROMUDataGridViewCheckBoxColumn.DataPropertyName = "B_NOM_PROMU";
            this.bNOMPROMUDataGridViewCheckBoxColumn.HeaderText = "B_NOM_PROMU";
            this.bNOMPROMUDataGridViewCheckBoxColumn.Name = "bNOMPROMUDataGridViewCheckBoxColumn";
            // 
            // NomsPromusBindingSource
            // 
            this.NomsPromusBindingSource.DataMember = "TAB_NOMS_PROMUS";
            this.NomsPromusBindingSource.DataSource = this.dataSetCoutDonnees;
            // 
            // dataSetCoutDonnees
            // 
            this.dataSetCoutDonnees.DataSetName = "DataSetCoutDonnees";
            this.dataSetCoutDonnees.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // FormNomsPromus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(911, 334);
            this.Controls.Add(this.dataGridViewNomsPromus);
            this.Controls.Add(this.buttonAnnuler);
            this.Controls.Add(this.buttonValider);
            this.Name = "FormNomsPromus";
            this.Text = "FormNomsPromus";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.FormNomsPromus_Load);
            this.Resize += new System.EventHandler(this.FormNomsPromus_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewNomsPromus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NomsPromusBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSetCoutDonnees)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonAnnuler;
        private System.Windows.Forms.Button buttonValider;
        private System.Windows.Forms.DataGridView dataGridViewNomsPromus;
        private Donnees dataSetCoutDonnees;
        private System.Windows.Forms.BindingSource NomsPromusBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn iDNOMPROMUDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iDNATIONDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn sNOMDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn bNOMPROMUDataGridViewCheckBoxColumn;
        //private System.Windows.Forms.DataGridViewTextBoxColumn iHEUREFINDataGridViewTextBoxColumn;
    }
}