namespace vaoc
{
    partial class FormNomsPions
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
            this.labelCommentaire = new System.Windows.Forms.Label();
            this.buttonAnnuler = new System.Windows.Forms.Button();
            this.buttonValider = new System.Windows.Forms.Button();
            this.dataGridViewNoms = new System.Windows.Forms.DataGridView();
            this.sNOMDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tABNOMSPIONSBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.donnees = new vaoc.Donnees();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewNoms)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tABNOMSPIONSBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.donnees)).BeginInit();
            this.SuspendLayout();
            // 
            // labelCommentaire
            // 
            this.labelCommentaire.AutoSize = true;
            this.labelCommentaire.Location = new System.Drawing.Point(17, 312);
            this.labelCommentaire.Name = "labelCommentaire";
            this.labelCommentaire.Size = new System.Drawing.Size(258, 13);
            this.labelCommentaire.TabIndex = 12;
            this.labelCommentaire.Text = "noms uniques pour chaque création de dépôt/convoi";
            // 
            // buttonAnnuler
            // 
            this.buttonAnnuler.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonAnnuler.Location = new System.Drawing.Point(385, 302);
            this.buttonAnnuler.Name = "buttonAnnuler";
            this.buttonAnnuler.Size = new System.Drawing.Size(75, 23);
            this.buttonAnnuler.TabIndex = 11;
            this.buttonAnnuler.Text = "Annuler";
            this.buttonAnnuler.UseVisualStyleBackColor = true;
            // 
            // buttonValider
            // 
            this.buttonValider.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonValider.Location = new System.Drawing.Point(251, 302);
            this.buttonValider.Name = "buttonValider";
            this.buttonValider.Size = new System.Drawing.Size(75, 23);
            this.buttonValider.TabIndex = 10;
            this.buttonValider.Text = "Valider";
            this.buttonValider.UseVisualStyleBackColor = true;
            // 
            // dataGridViewNoms
            // 
            this.dataGridViewNoms.AutoGenerateColumns = false;
            this.dataGridViewNoms.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewNoms.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.sNOMDataGridViewTextBoxColumn});
            this.dataGridViewNoms.DataSource = this.tABNOMSPIONSBindingSource;
            this.dataGridViewNoms.Dock = System.Windows.Forms.DockStyle.Top;
            this.dataGridViewNoms.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewNoms.Name = "dataGridViewNoms";
            this.dataGridViewNoms.Size = new System.Drawing.Size(472, 150);
            this.dataGridViewNoms.TabIndex = 13;
            // 
            // sNOMDataGridViewTextBoxColumn
            // 
            this.sNOMDataGridViewTextBoxColumn.DataPropertyName = "S_NOM";
            this.sNOMDataGridViewTextBoxColumn.HeaderText = "S_NOM";
            this.sNOMDataGridViewTextBoxColumn.Name = "sNOMDataGridViewTextBoxColumn";
            // 
            // tABNOMSPIONSBindingSource
            // 
            this.tABNOMSPIONSBindingSource.DataMember = "TAB_NOMS_PIONS";
            this.tABNOMSPIONSBindingSource.DataSource = this.donnees;
            // 
            // donnees
            // 
            this.donnees.DataSetName = "Donnees";
            this.donnees.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // FormNomsPions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(472, 362);
            this.Controls.Add(this.dataGridViewNoms);
            this.Controls.Add(this.labelCommentaire);
            this.Controls.Add(this.buttonAnnuler);
            this.Controls.Add(this.buttonValider);
            this.Name = "FormNomsPions";
            this.Text = "FormNomsPions";
            this.Load += new System.EventHandler(this.FormNomsPions_Load);
            this.Resize += new System.EventHandler(this.FormNomsPions_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewNoms)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tABNOMSPIONSBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.donnees)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelCommentaire;
        private System.Windows.Forms.Button buttonAnnuler;
        private System.Windows.Forms.Button buttonValider;
        private System.Windows.Forms.DataGridView dataGridViewNoms;
        private System.Windows.Forms.DataGridViewTextBoxColumn sNOMDataGridViewTextBoxColumn;
        private System.Windows.Forms.BindingSource tABNOMSPIONSBindingSource;
        private Donnees donnees;
    }
}