using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace vaoc
{
    public partial class FormVideoTable : Form
    {
        public FormVideoTable()
        {
            InitializeComponent();
        }

        #region propriétés
        public Donnees.TAB_VIDEODataTable tableVideo
        {
            get
            {
                return (Donnees.TAB_VIDEODataTable)dataGridViewVideo.DataSource;
            }
            set
            {
                dataGridViewVideo.DataSource = value.Copy();
            }
        }
        #endregion
    }
}
