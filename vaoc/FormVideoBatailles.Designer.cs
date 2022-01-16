
namespace vaoc
{
    partial class FormVideoBatailles
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
            this.buttonGenerer = new System.Windows.Forms.Button();
            this.comboBoxBatailles = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // buttonValider
            // 
            this.buttonValider.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonValider.Location = new System.Drawing.Point(283, 50);
            this.buttonValider.Name = "buttonValider";
            this.buttonValider.Size = new System.Drawing.Size(75, 23);
            this.buttonValider.TabIndex = 12;
            this.buttonValider.Text = "Fermer";
            this.buttonValider.UseVisualStyleBackColor = true;
            // 
            // buttonGenerer
            // 
            this.buttonGenerer.Location = new System.Drawing.Point(26, 50);
            this.buttonGenerer.Name = "buttonGenerer";
            this.buttonGenerer.Size = new System.Drawing.Size(188, 23);
            this.buttonGenerer.TabIndex = 13;
            this.buttonGenerer.Text = "Gener le(s) Film(s)";
            this.buttonGenerer.UseVisualStyleBackColor = true;
            this.buttonGenerer.Click += new System.EventHandler(this.buttonGenerer_Click);
            // 
            // comboBoxBatailles
            // 
            this.comboBoxBatailles.FormattingEnabled = true;
            this.comboBoxBatailles.Location = new System.Drawing.Point(26, 12);
            this.comboBoxBatailles.Name = "comboBoxBatailles";
            this.comboBoxBatailles.Size = new System.Drawing.Size(332, 21);
            this.comboBoxBatailles.TabIndex = 14;
            // 
            // FormVideoBatailles
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(406, 119);
            this.Controls.Add(this.comboBoxBatailles);
            this.Controls.Add(this.buttonGenerer);
            this.Controls.Add(this.buttonValider);
            this.Name = "FormVideoBatailles";
            this.Text = "FormVideoBatailles";
            this.Load += new System.EventHandler(this.FormVideoBatailles_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonValider;
        private System.Windows.Forms.Button buttonGenerer;
        private System.Windows.Forms.ComboBox comboBoxBatailles;
    }
}