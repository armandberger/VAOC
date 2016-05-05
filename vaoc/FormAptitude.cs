using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace vaoc
{
    public partial class FormAptitude : Form
    {
        public FormAptitude()
        {
            InitializeComponent();
        }

        #region propriétés
        public Donnees.TAB_APTITUDESDataTable tableAptitudes
        {
            get
            {
                return (Donnees.TAB_APTITUDESDataTable)dataGridViewAptitude.DataSource;
            }
            set
            {
                dataGridViewAptitude.DataSource = value.Copy();
            }
        }
        #endregion
    }
}