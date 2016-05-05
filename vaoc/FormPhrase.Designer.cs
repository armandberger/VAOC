namespace vaoc
{
    partial class FormPhrase
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.buttonAnnuler = new System.Windows.Forms.Button();
            this.buttonValider = new System.Windows.Forms.Button();
            this.dataGridViewPhrases = new System.Windows.Forms.DataGridView();
            this.ID_PHRASE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.I_TYPE = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.S_PHRASE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPhrases)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonAnnuler
            // 
            this.buttonAnnuler.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonAnnuler.Location = new System.Drawing.Point(196, 168);
            this.buttonAnnuler.Name = "buttonAnnuler";
            this.buttonAnnuler.Size = new System.Drawing.Size(75, 23);
            this.buttonAnnuler.TabIndex = 9;
            this.buttonAnnuler.Text = "Annuler";
            this.buttonAnnuler.UseVisualStyleBackColor = true;
            // 
            // buttonValider
            // 
            this.buttonValider.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonValider.Location = new System.Drawing.Point(62, 168);
            this.buttonValider.Name = "buttonValider";
            this.buttonValider.Size = new System.Drawing.Size(75, 23);
            this.buttonValider.TabIndex = 8;
            this.buttonValider.Text = "Valider";
            this.buttonValider.UseVisualStyleBackColor = true;
            // 
            // dataGridViewPhrases
            // 
            this.dataGridViewPhrases.AllowUserToOrderColumns = true;
            this.dataGridViewPhrases.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewPhrases.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ID_PHRASE,
            this.I_TYPE,
            this.S_PHRASE});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewPhrases.DefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewPhrases.Location = new System.Drawing.Point(12, 12);
            this.dataGridViewPhrases.Name = "dataGridViewPhrases";
            this.dataGridViewPhrases.Size = new System.Drawing.Size(615, 143);
            this.dataGridViewPhrases.TabIndex = 10;
            // 
            // ID_PHRASE
            // 
            this.ID_PHRASE.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.ID_PHRASE.HeaderText = "ID_PHRASE";
            this.ID_PHRASE.Name = "ID_PHRASE";
            this.ID_PHRASE.Width = 93;
            // 
            // I_TYPE
            // 
            this.I_TYPE.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.I_TYPE.HeaderText = "I_TYPE";
            this.I_TYPE.Name = "I_TYPE";
            this.I_TYPE.Width = 50;
            // 
            // S_PHRASE
            // 
            this.S_PHRASE.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.S_PHRASE.HeaderText = "S_PHRASE";
            this.S_PHRASE.Name = "S_PHRASE";
            // 
            // FormPhrase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(657, 244);
            this.Controls.Add(this.dataGridViewPhrases);
            this.Controls.Add(this.buttonAnnuler);
            this.Controls.Add(this.buttonValider);
            this.Name = "FormPhrase";
            this.Text = "FormPhrase";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.FormPhrase_Load);
            this.Resize += new System.EventHandler(this.FormPhrase_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPhrases)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonAnnuler;
        private System.Windows.Forms.Button buttonValider;
        private System.Windows.Forms.DataGridView dataGridViewPhrases;
        private System.Windows.Forms.DataGridViewTextBoxColumn ID_PHRASE;
        private System.Windows.Forms.DataGridViewComboBoxColumn I_TYPE;
        private System.Windows.Forms.DataGridViewTextBoxColumn S_PHRASE;
    }
}