namespace vaoc
{
    partial class FormMessageArbitre
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
            this.buttonOK = new System.Windows.Forms.Button();
            this.labelMessageArbitre = new System.Windows.Forms.Label();
            this.textBoxMessage = new System.Windows.Forms.TextBox();
            this.dataGridViewUtilisateur = new System.Windows.Forms.DataGridView();
            this.buttonAnnuler = new System.Windows.Forms.Button();
            this.webBrowserNotification = new System.Windows.Forms.WebBrowser();
            this.listBoxRoles = new System.Windows.Forms.ListBox();
            this.labelDernierNotification = new System.Windows.Forms.Label();
            this.textBoxDernierNotification = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewUtilisateur)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(112, 175);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(129, 27);
            this.buttonOK.TabIndex = 1;
            this.buttonOK.Text = "Envoyer les notifications";
            this.buttonOK.UseVisualStyleBackColor = true;
            // 
            // labelMessageArbitre
            // 
            this.labelMessageArbitre.AutoSize = true;
            this.labelMessageArbitre.Location = new System.Drawing.Point(12, 9);
            this.labelMessageArbitre.Name = "labelMessageArbitre";
            this.labelMessageArbitre.Size = new System.Drawing.Size(107, 13);
            this.labelMessageArbitre.TabIndex = 1;
            this.labelMessageArbitre.Text = "Message de l\'arbitre :";
            // 
            // textBoxMessage
            // 
            this.textBoxMessage.Location = new System.Drawing.Point(12, 33);
            this.textBoxMessage.Multiline = true;
            this.textBoxMessage.Name = "textBoxMessage";
            this.textBoxMessage.Size = new System.Drawing.Size(315, 81);
            this.textBoxMessage.TabIndex = 0;
            // 
            // dataGridViewUtilisateur
            // 
            this.dataGridViewUtilisateur.AllowUserToAddRows = false;
            this.dataGridViewUtilisateur.AllowUserToDeleteRows = false;
            this.dataGridViewUtilisateur.AllowUserToOrderColumns = true;
            this.dataGridViewUtilisateur.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.ColumnHeader;
            this.dataGridViewUtilisateur.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewUtilisateur.Location = new System.Drawing.Point(456, 33);
            this.dataGridViewUtilisateur.Name = "dataGridViewUtilisateur";
            this.dataGridViewUtilisateur.Size = new System.Drawing.Size(272, 160);
            this.dataGridViewUtilisateur.TabIndex = 8;
            // 
            // buttonAnnuler
            // 
            this.buttonAnnuler.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonAnnuler.Location = new System.Drawing.Point(309, 198);
            this.buttonAnnuler.Name = "buttonAnnuler";
            this.buttonAnnuler.Size = new System.Drawing.Size(129, 27);
            this.buttonAnnuler.TabIndex = 10;
            this.buttonAnnuler.Text = "Annuler";
            this.buttonAnnuler.UseVisualStyleBackColor = true;
            // 
            // webBrowserNotification
            // 
            this.webBrowserNotification.Location = new System.Drawing.Point(15, 250);
            this.webBrowserNotification.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowserNotification.Name = "webBrowserNotification";
            this.webBrowserNotification.Size = new System.Drawing.Size(386, 149);
            this.webBrowserNotification.TabIndex = 11;
            // 
            // listBoxRoles
            // 
            this.listBoxRoles.FormattingEnabled = true;
            this.listBoxRoles.Location = new System.Drawing.Point(553, 326);
            this.listBoxRoles.Name = "listBoxRoles";
            this.listBoxRoles.Size = new System.Drawing.Size(120, 95);
            this.listBoxRoles.TabIndex = 12;
            this.listBoxRoles.SelectedIndexChanged += new System.EventHandler(this.listBoxRoles_SelectedIndexChanged);
            // 
            // labelDernierNotification
            // 
            this.labelDernierNotification.AutoSize = true;
            this.labelDernierNotification.Location = new System.Drawing.Point(39, 471);
            this.labelDernierNotification.Name = "labelDernierNotification";
            this.labelDernierNotification.Size = new System.Drawing.Size(383, 13);
            this.labelDernierNotification.TabIndex = 13;
            this.labelDernierNotification.Text = "Tour de dernière notification (mise à jour automatique, puis sauvegare du fichier" +
    ")";
            // 
            // textBoxDernierNotification
            // 
            this.textBoxDernierNotification.Location = new System.Drawing.Point(512, 471);
            this.textBoxDernierNotification.Name = "textBoxDernierNotification";
            this.textBoxDernierNotification.Size = new System.Drawing.Size(100, 20);
            this.textBoxDernierNotification.TabIndex = 14;
            // 
            // FormMessageArbitre
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.CancelButton = this.buttonAnnuler;
            this.ClientSize = new System.Drawing.Size(1182, 640);
            this.Controls.Add(this.textBoxDernierNotification);
            this.Controls.Add(this.labelDernierNotification);
            this.Controls.Add(this.listBoxRoles);
            this.Controls.Add(this.webBrowserNotification);
            this.Controls.Add(this.buttonAnnuler);
            this.Controls.Add(this.dataGridViewUtilisateur);
            this.Controls.Add(this.textBoxMessage);
            this.Controls.Add(this.labelMessageArbitre);
            this.Controls.Add(this.buttonOK);
            this.Name = "FormMessageArbitre";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Message de l\'arbitre";
            this.Load += new System.EventHandler(this.FormMessageArbitre_Load);
            this.Resize += new System.EventHandler(this.FormMessageArbitre_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewUtilisateur)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Label labelMessageArbitre;
        public System.Windows.Forms.TextBox textBoxMessage;
        private System.Windows.Forms.DataGridView dataGridViewUtilisateur;
        private System.Windows.Forms.Button buttonAnnuler;
        private System.Windows.Forms.WebBrowser webBrowserNotification;
        private System.Windows.Forms.ListBox listBoxRoles;
        private System.Windows.Forms.Label labelDernierNotification;
        private System.Windows.Forms.TextBox textBoxDernierNotification;
    }
}