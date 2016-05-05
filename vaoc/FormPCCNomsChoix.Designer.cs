namespace vaoc
{
    partial class FormPCCNomsChoix
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
            this.radioButtonInitialiser = new System.Windows.Forms.RadioButton();
            this.radioButtonReprise = new System.Windows.Forms.RadioButton();
            this.radioButtonCorrection = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // buttonValider
            // 
            this.buttonValider.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonValider.Location = new System.Drawing.Point(74, 121);
            this.buttonValider.Name = "buttonValider";
            this.buttonValider.Size = new System.Drawing.Size(75, 23);
            this.buttonValider.TabIndex = 0;
            this.buttonValider.Text = "Valider";
            this.buttonValider.UseVisualStyleBackColor = true;
            // 
            // radioButtonInitialiser
            // 
            this.radioButtonInitialiser.AutoSize = true;
            this.radioButtonInitialiser.Location = new System.Drawing.Point(22, 12);
            this.radioButtonInitialiser.Name = "radioButtonInitialiser";
            this.radioButtonInitialiser.Size = new System.Drawing.Size(170, 17);
            this.radioButtonInitialiser.TabIndex = 1;
            this.radioButtonInitialiser.TabStop = true;
            this.radioButtonInitialiser.Text = "Tout reprendre depuis le début";
            this.radioButtonInitialiser.UseVisualStyleBackColor = true;
            // 
            // radioButtonReprise
            // 
            this.radioButtonReprise.AutoSize = true;
            this.radioButtonReprise.Location = new System.Drawing.Point(22, 47);
            this.radioButtonReprise.Name = "radioButtonReprise";
            this.radioButtonReprise.Size = new System.Drawing.Size(179, 17);
            this.radioButtonReprise.TabIndex = 2;
            this.radioButtonReprise.TabStop = true;
            this.radioButtonReprise.Text = "Reprendre depuis le dernier arrêt";
            this.radioButtonReprise.UseVisualStyleBackColor = true;
            // 
            // radioButtonCorrection
            // 
            this.radioButtonCorrection.AutoSize = true;
            this.radioButtonCorrection.Location = new System.Drawing.Point(22, 82);
            this.radioButtonCorrection.Name = "radioButtonCorrection";
            this.radioButtonCorrection.Size = new System.Drawing.Size(183, 17);
            this.radioButtonCorrection.TabIndex = 3;
            this.radioButtonCorrection.TabStop = true;
            this.radioButtonCorrection.Text = "Rechercher les trajets manquants";
            this.radioButtonCorrection.UseVisualStyleBackColor = true;
            // 
            // FormPCCNomsChoix
            // 
            this.AcceptButton = this.buttonValider;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonValider;
            this.ClientSize = new System.Drawing.Size(219, 161);
            this.Controls.Add(this.radioButtonCorrection);
            this.Controls.Add(this.radioButtonReprise);
            this.Controls.Add(this.radioButtonInitialiser);
            this.Controls.Add(this.buttonValider);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormPCCNomsChoix";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Choix de génération des PCC noms";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonValider;
        private System.Windows.Forms.RadioButton radioButtonInitialiser;
        private System.Windows.Forms.RadioButton radioButtonReprise;
        private System.Windows.Forms.RadioButton radioButtonCorrection;
    }
}