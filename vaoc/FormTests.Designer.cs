namespace vaoc
{
    partial class FormTests
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTests));
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonMessages = new System.Windows.Forms.Button();
            this.textBoxResultat = new System.Windows.Forms.TextBox();
            this.textBoxFormats = new System.Windows.Forms.TextBox();
            this.buttonCasesNoms = new System.Windows.Forms.Button();
            this.buttonMessageTexteFictif = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(350, 573);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(60, 27);
            this.buttonOK.TabIndex = 2;
            this.buttonOK.Text = "Fermer";
            this.buttonOK.UseVisualStyleBackColor = true;
            // 
            // buttonMessages
            // 
            this.buttonMessages.Location = new System.Drawing.Point(12, 12);
            this.buttonMessages.Name = "buttonMessages";
            this.buttonMessages.Size = new System.Drawing.Size(75, 23);
            this.buttonMessages.TabIndex = 3;
            this.buttonMessages.Text = "Messages";
            this.buttonMessages.UseVisualStyleBackColor = true;
            this.buttonMessages.Click += new System.EventHandler(this.buttonMessages_Click);
            // 
            // textBoxResultat
            // 
            this.textBoxResultat.Location = new System.Drawing.Point(12, 48);
            this.textBoxResultat.Multiline = true;
            this.textBoxResultat.Name = "textBoxResultat";
            this.textBoxResultat.ReadOnly = true;
            this.textBoxResultat.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxResultat.Size = new System.Drawing.Size(520, 519);
            this.textBoxResultat.TabIndex = 4;
            // 
            // textBoxFormats
            // 
            this.textBoxFormats.Location = new System.Drawing.Point(538, 48);
            this.textBoxFormats.Multiline = true;
            this.textBoxFormats.Name = "textBoxFormats";
            this.textBoxFormats.ReadOnly = true;
            this.textBoxFormats.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxFormats.Size = new System.Drawing.Size(211, 519);
            this.textBoxFormats.TabIndex = 5;
            this.textBoxFormats.Tag = "";
            this.textBoxFormats.Text = resources.GetString("textBoxFormats.Text");
            // 
            // buttonCasesNoms
            // 
            this.buttonCasesNoms.Location = new System.Drawing.Point(392, 12);
            this.buttonCasesNoms.Name = "buttonCasesNoms";
            this.buttonCasesNoms.Size = new System.Drawing.Size(140, 23);
            this.buttonCasesNoms.TabIndex = 6;
            this.buttonCasesNoms.Text = "Cases des lieux nommés";
            this.buttonCasesNoms.UseVisualStyleBackColor = true;
            this.buttonCasesNoms.Click += new System.EventHandler(this.buttonCasesNoms_Click);
            // 
            // buttonMessageTexteFictif
            // 
            this.buttonMessageTexteFictif.Location = new System.Drawing.Point(93, 12);
            this.buttonMessageTexteFictif.Name = "buttonMessageTexteFictif";
            this.buttonMessageTexteFictif.Size = new System.Drawing.Size(135, 23);
            this.buttonMessageTexteFictif.TabIndex = 7;
            this.buttonMessageTexteFictif.Text = "Messages texte fictif";
            this.buttonMessageTexteFictif.UseVisualStyleBackColor = true;
            this.buttonMessageTexteFictif.Click += new System.EventHandler(this.buttonMessageTexteFictif_Click);
            // 
            // FormTests
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(761, 612);
            this.Controls.Add(this.buttonMessageTexteFictif);
            this.Controls.Add(this.buttonCasesNoms);
            this.Controls.Add(this.textBoxFormats);
            this.Controls.Add(this.textBoxResultat);
            this.Controls.Add(this.buttonMessages);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "FormTests";
            this.Text = "FormTests";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonMessages;
        private System.Windows.Forms.TextBox textBoxResultat;
        private System.Windows.Forms.TextBox textBoxFormats;
        private System.Windows.Forms.Button buttonCasesNoms;
        private System.Windows.Forms.Button buttonMessageTexteFictif;
    }
}