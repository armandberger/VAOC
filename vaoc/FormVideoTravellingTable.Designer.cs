namespace vaoc
{
    partial class FormVideoTravellingTable
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
            this.dataGridViewTravelling = new System.Windows.Forms.DataGridView();
            this.donnees = new vaoc.Donnees();
            this.tABTRAVELLINGBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.iTOURDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iXDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iYDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewTravelling)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.donnees)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tABTRAVELLINGBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonAnnuler
            // 
            this.buttonAnnuler.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonAnnuler.Location = new System.Drawing.Point(88, 305);
            this.buttonAnnuler.Name = "buttonAnnuler";
            this.buttonAnnuler.Size = new System.Drawing.Size(75, 23);
            this.buttonAnnuler.TabIndex = 14;
            this.buttonAnnuler.Text = "Annuler";
            this.buttonAnnuler.UseVisualStyleBackColor = true;
            // 
            // buttonValider
            // 
            this.buttonValider.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonValider.Location = new System.Drawing.Point(7, 305);
            this.buttonValider.Name = "buttonValider";
            this.buttonValider.Size = new System.Drawing.Size(75, 23);
            this.buttonValider.TabIndex = 13;
            this.buttonValider.Text = "Valider";
            this.buttonValider.UseVisualStyleBackColor = true;
            // 
            // dataGridViewTravelling
            // 
            this.dataGridViewTravelling.AutoGenerateColumns = false;
            this.dataGridViewTravelling.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewTravelling.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.iTOURDataGridViewTextBoxColumn,
            this.iXDataGridViewTextBoxColumn,
            this.iYDataGridViewTextBoxColumn});
            this.dataGridViewTravelling.DataSource = this.tABTRAVELLINGBindingSource;
            this.dataGridViewTravelling.Dock = System.Windows.Forms.DockStyle.Top;
            this.dataGridViewTravelling.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewTravelling.Name = "dataGridViewTravelling";
            this.dataGridViewTravelling.Size = new System.Drawing.Size(581, 150);
            this.dataGridViewTravelling.TabIndex = 15;
            // 
            // donnees
            // 
            this.donnees.DataSetName = "Donnees";
            this.donnees.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // tABTRAVELLINGBindingSource
            // 
            this.tABTRAVELLINGBindingSource.DataMember = "TAB_TRAVELLING";
            this.tABTRAVELLINGBindingSource.DataSource = this.donnees;
            // 
            // iTOURDataGridViewTextBoxColumn
            // 
            this.iTOURDataGridViewTextBoxColumn.DataPropertyName = "I_TOUR";
            this.iTOURDataGridViewTextBoxColumn.HeaderText = "I_TOUR";
            this.iTOURDataGridViewTextBoxColumn.Name = "iTOURDataGridViewTextBoxColumn";
            // 
            // iXDataGridViewTextBoxColumn
            // 
            this.iXDataGridViewTextBoxColumn.DataPropertyName = "I_X";
            this.iXDataGridViewTextBoxColumn.HeaderText = "I_X";
            this.iXDataGridViewTextBoxColumn.Name = "iXDataGridViewTextBoxColumn";
            // 
            // iYDataGridViewTextBoxColumn
            // 
            this.iYDataGridViewTextBoxColumn.DataPropertyName = "I_Y";
            this.iYDataGridViewTextBoxColumn.HeaderText = "I_Y";
            this.iYDataGridViewTextBoxColumn.Name = "iYDataGridViewTextBoxColumn";
            // 
            // FormVideoTravellingTable
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(581, 393);
            this.Controls.Add(this.dataGridViewTravelling);
            this.Controls.Add(this.buttonAnnuler);
            this.Controls.Add(this.buttonValider);
            this.Name = "FormVideoTravellingTable";
            this.Text = "FormVideoTravellingTable";
            this.Load += new System.EventHandler(this.FormVideoTravellingTable_Load);
            this.Resize += new System.EventHandler(this.FormVideoTravellingTable_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewTravelling)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.donnees)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tABTRAVELLINGBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonAnnuler;
        private System.Windows.Forms.Button buttonValider;
        private System.Windows.Forms.DataGridView dataGridViewTravelling;
        private System.Windows.Forms.DataGridViewTextBoxColumn iTOURDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iXDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iYDataGridViewTextBoxColumn;
        private System.Windows.Forms.BindingSource tABTRAVELLINGBindingSource;
        private Donnees donnees;
    }
}