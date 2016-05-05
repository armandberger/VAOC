using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace vaoc
{
    public partial class FormNation : Form
    {
        public FormNation()
        {
            InitializeComponent();
        }

        #region propriétés
        public Donnees.TAB_NATIONDataTable tableNation
        {
            get
            {
                return (Donnees.TAB_NATIONDataTable)dataGridViewModelesCombats.DataSource;
            }
            set
            {
                dataGridViewModelesCombats.DataSource = value.Copy();
            }
        }
        #endregion
    }
}