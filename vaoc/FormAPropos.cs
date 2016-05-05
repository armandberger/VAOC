using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace vaoc
{
    public partial class FormAPropos : Form
    {
        public FormAPropos()
        {
            InitializeComponent();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            Close();
        }

        public string test
        {
            get { return this.labelVersion.Text; }
            set { labelVersion.Text = value; }
        }
	
    }
}