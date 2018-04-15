using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace vaocVideo
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

        private void FormVideoTable_Load(object sender, EventArgs e)
        {
            Redimensionner();
        }

        private void FormVideoTable_Resize(object sender, EventArgs e)
        {
            Redimensionner();
        }

        private void Redimensionner()
        {
            #region positionnement des boutons annuler et valider
            //buttonValider.Left = (Width - buttonValider.Width - buttonAnnuler.Width) / 3;
            //buttonAnnuler.Left = buttonValider.Width + 2 * (Width - buttonValider.Width - buttonAnnuler.Width) / 3;
            //buttonAnnuler.Top = Height - 3 * buttonValider.Height;
            buttonValider.Top = Height - 3 * buttonValider.Height;
            #endregion

            #region positionnement de la grille
            this.dataGridViewVideo.Width = Width;
            this.dataGridViewVideo.Height = Height - 4 * buttonValider.Height;
            #endregion
        }
    }
}
