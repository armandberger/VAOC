namespace vaoc
{
    partial class FormForcesInitiales
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
            this.buttonFermer = new System.Windows.Forms.Button();
            this.textBoxResultat = new System.Windows.Forms.TextBox();
            this.checkBoxCampagne = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // buttonFermer
            // 
            this.buttonFermer.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonFermer.Location = new System.Drawing.Point(713, 415);
            this.buttonFermer.Name = "buttonFermer";
            this.buttonFermer.Size = new System.Drawing.Size(75, 23);
            this.buttonFermer.TabIndex = 12;
            this.buttonFermer.Text = "Fermer";
            this.buttonFermer.UseVisualStyleBackColor = true;
            // 
            // textBoxResultat
            // 
            this.textBoxResultat.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxResultat.Location = new System.Drawing.Point(12, 12);
            this.textBoxResultat.Multiline = true;
            this.textBoxResultat.Name = "textBoxResultat";
            this.textBoxResultat.ReadOnly = true;
            this.textBoxResultat.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxResultat.Size = new System.Drawing.Size(776, 397);
            this.textBoxResultat.TabIndex = 13;
            // 
            // checkBoxCampagne
            // 
            this.checkBoxCampagne.AutoSize = true;
            this.checkBoxCampagne.Location = new System.Drawing.Point(13, 421);
            this.checkBoxCampagne.Name = "checkBoxCampagne";
            this.checkBoxCampagne.Size = new System.Drawing.Size(124, 17);
            this.checkBoxCampagne.TabIndex = 14;
            this.checkBoxCampagne.Text = "affichage Campagne";
            this.checkBoxCampagne.UseVisualStyleBackColor = true;
            this.checkBoxCampagne.CheckedChanged += new System.EventHandler(this.checkBoxCampagne_CheckedChanged);
            // 
            // FormForcesInitiales
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.checkBoxCampagne);
            this.Controls.Add(this.textBoxResultat);
            this.Controls.Add(this.buttonFermer);
            this.Name = "FormForcesInitiales";
            this.Text = "FormForcesInitiales";
            this.Load += new System.EventHandler(this.FormForcesInitiales_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonFermer;
        private System.Windows.Forms.TextBox textBoxResultat;
        private System.Windows.Forms.CheckBox checkBoxCampagne;
    }
}