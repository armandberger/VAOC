
namespace vaoc
{
    partial class FormMajCases
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
            this.buttonValider = new System.Windows.Forms.Button();
            this.buttonGeneration = new System.Windows.Forms.Button();
            this.textBoxResultat = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // buttonValider
            // 
            this.buttonValider.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonValider.Location = new System.Drawing.Point(703, 12);
            this.buttonValider.Name = "buttonValider";
            this.buttonValider.Size = new System.Drawing.Size(85, 23);
            this.buttonValider.TabIndex = 12;
            this.buttonValider.Text = "Valider";
            this.buttonValider.UseVisualStyleBackColor = true;
            // 
            // buttonGeneration
            // 
            this.buttonGeneration.Location = new System.Drawing.Point(12, 12);
            this.buttonGeneration.Name = "buttonGeneration";
            this.buttonGeneration.Size = new System.Drawing.Size(85, 23);
            this.buttonGeneration.TabIndex = 11;
            this.buttonGeneration.Text = "Generation";
            this.buttonGeneration.UseVisualStyleBackColor = true;
            this.buttonGeneration.Click += new System.EventHandler(this.buttonGeneration_Click);
            // 
            // textBoxResultat
            // 
            this.textBoxResultat.Location = new System.Drawing.Point(12, 41);
            this.textBoxResultat.Multiline = true;
            this.textBoxResultat.Name = "textBoxResultat";
            this.textBoxResultat.ReadOnly = true;
            this.textBoxResultat.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxResultat.Size = new System.Drawing.Size(776, 388);
            this.textBoxResultat.TabIndex = 10;
            // 
            // FormMajCases
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.buttonValider);
            this.Controls.Add(this.buttonGeneration);
            this.Controls.Add(this.textBoxResultat);
            this.Name = "FormMajCases";
            this.Text = "FormMajCases";
            this.Load += new System.EventHandler(this.FormMajCases_Load);
            this.Resize += new System.EventHandler(this.FormMajCases_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonValider;
        private System.Windows.Forms.Button buttonGeneration;
        private System.Windows.Forms.TextBox textBoxResultat;
    }
}