namespace vaoc
{
    partial class FormBroderie
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
            this.textBoxFichierImage = new System.Windows.Forms.TextBox();
            this.buttonImage = new System.Windows.Forms.Button();
            this.textBoxVertical = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxHorizontal = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.label3 = new System.Windows.Forms.Label();
            this.colorDialog = new System.Windows.Forms.ColorDialog();
            this.textBoxCouleur = new System.Windows.Forms.TextBox();
            this.buttonTraitement = new System.Windows.Forms.Button();
            this.checkBoxMajorite = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // textBoxFichierImage
            // 
            this.textBoxFichierImage.Location = new System.Drawing.Point(167, 15);
            this.textBoxFichierImage.Name = "textBoxFichierImage";
            this.textBoxFichierImage.Size = new System.Drawing.Size(449, 20);
            this.textBoxFichierImage.TabIndex = 7;
            // 
            // buttonImage
            // 
            this.buttonImage.Location = new System.Drawing.Point(12, 12);
            this.buttonImage.Name = "buttonImage";
            this.buttonImage.Size = new System.Drawing.Size(138, 23);
            this.buttonImage.TabIndex = 6;
            this.buttonImage.Text = "Ouvrir Image";
            this.buttonImage.UseVisualStyleBackColor = true;
            this.buttonImage.Click += new System.EventHandler(this.buttonImage_Click);
            // 
            // textBoxVertical
            // 
            this.textBoxVertical.Location = new System.Drawing.Point(407, 54);
            this.textBoxVertical.Name = "textBoxVertical";
            this.textBoxVertical.Size = new System.Drawing.Size(100, 20);
            this.textBoxVertical.TabIndex = 18;
            this.textBoxVertical.Text = "50";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(303, 57);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 13);
            this.label2.TabIndex = 17;
            this.label2.Text = "nb cases verticales";
            // 
            // textBoxHorizontal
            // 
            this.textBoxHorizontal.Location = new System.Drawing.Point(167, 54);
            this.textBoxHorizontal.Name = "textBoxHorizontal";
            this.textBoxHorizontal.Size = new System.Drawing.Size(100, 20);
            this.textBoxHorizontal.TabIndex = 16;
            this.textBoxHorizontal.Text = "50";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 57);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(109, 13);
            this.label1.TabIndex = 15;
            this.label1.Text = "nb cases horizontales";
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(21, 97);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(88, 13);
            this.label3.TabIndex = 19;
            this.label3.Text = "Couleur des traits";
            // 
            // textBoxCouleur
            // 
            this.textBoxCouleur.BackColor = System.Drawing.Color.Black;
            this.textBoxCouleur.Location = new System.Drawing.Point(167, 97);
            this.textBoxCouleur.Name = "textBoxCouleur";
            this.textBoxCouleur.ReadOnly = true;
            this.textBoxCouleur.Size = new System.Drawing.Size(29, 20);
            this.textBoxCouleur.TabIndex = 20;
            this.textBoxCouleur.MouseClick += new System.Windows.Forms.MouseEventHandler(this.textBox1_MouseClick);
            // 
            // buttonTraitement
            // 
            this.buttonTraitement.Location = new System.Drawing.Point(24, 151);
            this.buttonTraitement.Name = "buttonTraitement";
            this.buttonTraitement.Size = new System.Drawing.Size(138, 23);
            this.buttonTraitement.TabIndex = 21;
            this.buttonTraitement.Text = "Traitement";
            this.buttonTraitement.UseVisualStyleBackColor = true;
            this.buttonTraitement.Click += new System.EventHandler(this.buttonTraitement_Click);
            // 
            // checkBoxMajorite
            // 
            this.checkBoxMajorite.AutoSize = true;
            this.checkBoxMajorite.Checked = true;
            this.checkBoxMajorite.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxMajorite.Location = new System.Drawing.Point(306, 99);
            this.checkBoxMajorite.Name = "checkBoxMajorite";
            this.checkBoxMajorite.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.checkBoxMajorite.Size = new System.Drawing.Size(121, 17);
            this.checkBoxMajorite.TabIndex = 22;
            this.checkBoxMajorite.Text = "Majorité de couleurs";
            this.checkBoxMajorite.UseVisualStyleBackColor = true;
            // 
            // FormBroderie
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(640, 189);
            this.Controls.Add(this.checkBoxMajorite);
            this.Controls.Add(this.buttonTraitement);
            this.Controls.Add(this.textBoxCouleur);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxVertical);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxHorizontal);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxFichierImage);
            this.Controls.Add(this.buttonImage);
            this.Name = "FormBroderie";
            this.Text = "FormBroderie";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxFichierImage;
        private System.Windows.Forms.Button buttonImage;
        private System.Windows.Forms.TextBox textBoxVertical;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxHorizontal;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ColorDialog colorDialog;
        private System.Windows.Forms.TextBox textBoxCouleur;
        private System.Windows.Forms.Button buttonTraitement;
        private System.Windows.Forms.CheckBox checkBoxMajorite;
    }
}