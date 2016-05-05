namespace vaoc
{
    partial class FormRole
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
            this.dataGridViewRole = new System.Windows.Forms.DataGridView();
            this.buttonAnnuler = new System.Windows.Forms.Button();
            this.buttonValider = new System.Windows.Forms.Button();
            this.ID_ROLE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.S_NOM = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.S_UTILISATEUR = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.ID_PION = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRole)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewRole
            // 
            this.dataGridViewRole.AllowUserToOrderColumns = true;
            this.dataGridViewRole.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewRole.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ID_ROLE,
            this.S_NOM,
            this.S_UTILISATEUR,
            this.ID_PION});
            this.dataGridViewRole.Location = new System.Drawing.Point(12, 12);
            this.dataGridViewRole.Name = "dataGridViewRole";
            this.dataGridViewRole.Size = new System.Drawing.Size(607, 192);
            this.dataGridViewRole.TabIndex = 3;
            // 
            // buttonAnnuler
            // 
            this.buttonAnnuler.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonAnnuler.Location = new System.Drawing.Point(327, 222);
            this.buttonAnnuler.Name = "buttonAnnuler";
            this.buttonAnnuler.Size = new System.Drawing.Size(75, 23);
            this.buttonAnnuler.TabIndex = 5;
            this.buttonAnnuler.Text = "Annuler";
            this.buttonAnnuler.UseVisualStyleBackColor = true;
            // 
            // buttonValider
            // 
            this.buttonValider.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonValider.Location = new System.Drawing.Point(193, 222);
            this.buttonValider.Name = "buttonValider";
            this.buttonValider.Size = new System.Drawing.Size(75, 23);
            this.buttonValider.TabIndex = 4;
            this.buttonValider.Text = "Valider";
            this.buttonValider.UseVisualStyleBackColor = true;
            // 
            // ID_ROLE
            // 
            this.ID_ROLE.HeaderText = "ID_ROLE";
            this.ID_ROLE.Name = "ID_ROLE";
            // 
            // S_NOM
            // 
            this.S_NOM.HeaderText = "S_NOM";
            this.S_NOM.Name = "S_NOM";
            // 
            // S_UTILISATEUR
            // 
            this.S_UTILISATEUR.HeaderText = "UTILISATEUR";
            this.S_UTILISATEUR.Name = "S_UTILISATEUR";
            this.S_UTILISATEUR.Width = 200;
            // 
            // ID_PION
            // 
            this.ID_PION.HeaderText = "ID_PION";
            this.ID_PION.Name = "ID_PION";
            // 
            // FormRole
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(631, 259);
            this.Controls.Add(this.dataGridViewRole);
            this.Controls.Add(this.buttonAnnuler);
            this.Controls.Add(this.buttonValider);
            this.Name = "FormRole";
            this.Text = "FormRole";
            this.Load += new System.EventHandler(this.FormRole_Load);
            this.Resize += new System.EventHandler(this.FormRole_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRole)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewRole;
        private System.Windows.Forms.Button buttonAnnuler;
        private System.Windows.Forms.Button buttonValider;
        private System.Windows.Forms.DataGridViewTextBoxColumn ID_ROLE;
        private System.Windows.Forms.DataGridViewTextBoxColumn S_NOM;
        private System.Windows.Forms.DataGridViewComboBoxColumn S_UTILISATEUR;
        private System.Windows.Forms.DataGridViewTextBoxColumn ID_PION;
    }
}