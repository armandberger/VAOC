using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace vaoc
{
    public partial class FormPCCNomsChoix : Form
    {
        public enum PCCNomsChoix { INITIAL, REPRISE, VERIFICATION };

        public PCCNomsChoix choix
        {
            get
            {
                if (this.radioButtonInitialiser.Checked) { return PCCNomsChoix.INITIAL; }
                if (this.radioButtonReprise.Checked) { return PCCNomsChoix.REPRISE; }
                return PCCNomsChoix.VERIFICATION;
            }
        }
        public FormPCCNomsChoix()
        {
            InitializeComponent();
        }
    }
}
