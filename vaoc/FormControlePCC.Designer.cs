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
            System.Windows.Forms.GroupBox groupBoxInfo;
            textBox1 = new System.Windows.Forms.TextBox();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            groupBoxInfo = new System.Windows.Forms.GroupBox();
            groupBoxInfo.SuspendLayout();
            SuspendLayout();
            // 
            // groupBoxInfo
            // 
            groupBoxInfo.Controls.Add(textBox1);
            groupBoxInfo.Controls.Add(label1);
            groupBoxInfo.Location = new System.Drawing.Point(316, 203);
            groupBoxInfo.Name = "groupBoxInfo";
            groupBoxInfo.Size = new System.Drawing.Size(216, 182);
            groupBoxInfo.TabIndex = 0;
            groupBoxInfo.TabStop = false;
            groupBoxInfo.Text = "Info";
            groupBoxInfo.Visible = false;
            // 
            // textBox1
            // 
            textBox1.Location = new System.Drawing.Point(65, 34);
            textBox1.Name = "textBox1";
            textBox1.Size = new System.Drawing.Size(100, 23);
            textBox1.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(17, 33);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(38, 15);
            label1.TabIndex = 0;
            label1.Text = "label1";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(523, 142);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(38, 15);
            label2.TabIndex = 3;
            label2.Text = "label2";
            label2.Visible = false;
            label2.Click += label2_Click;
            // 
            // FormControlePCC
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(800, 450);
            Controls.Add(label2);
            Controls.Add(groupBoxInfo);
            Name = "FormControlePCC";
            Text = "Controle PCC";
            WindowState = System.Windows.Forms.FormWindowState.Maximized;
            Load += FormControlePCC_Load;
            SizeChanged += FormControlePCC_SizeChanged;
            Paint += FormControlePCC_Paint;
            MouseClick += FormControlePCC_MouseClick;
            MouseMove += FormControlePCC_MouseMove;
            groupBoxInfo.ResumeLayout(false);
            groupBoxInfo.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxInfo;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}