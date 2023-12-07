namespace vaoc
{
    partial class FormControlePCC
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
            BoiteInformation = new System.Windows.Forms.GroupBox();
            SuspendLayout();
            // 
            // BoiteInformation
            // 
            BoiteInformation.Location = new System.Drawing.Point(470, 233);
            BoiteInformation.Name = "BoiteInformation";
            BoiteInformation.Size = new System.Drawing.Size(200, 100);
            BoiteInformation.TabIndex = 0;
            BoiteInformation.TabStop = false;
            BoiteInformation.Text = "Information";
            // 
            // FormControlePCC
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(800, 450);
            Controls.Add(BoiteInformation);
            Name = "FormControlePCC";
            Text = "Controle PCC";
            WindowState = System.Windows.Forms.FormWindowState.Maximized;
            Load += FormControlePCC_Load;
            SizeChanged += FormControlePCC_SizeChanged;
            Paint += FormControlePCC_Paint;
            MouseClick += FormControlePCC_MouseClick;
            MouseMove += FormControlePCC_MouseMove;
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.GroupBox BoiteInformation;
    }
}