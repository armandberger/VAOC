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
    public partial class FormBataillesVideo : Form
    {
        public FormBataillesVideo()
        {
            InitializeComponent();
        }

        private void FormBataillesVideo_Load(object sender, EventArgs e)
        {
            Redimensionner();
        }

        private void FormBataillesVideo_Resize(object sender, EventArgs e)
        {
            Redimensionner();
        }

        private void Redimensionner()
        {
            #region positionnement des boutons annuler et valider
            buttonValider.Left = (Width - buttonValider.Width - buttonAnnuler.Width) / 3;
            buttonAnnuler.Left = buttonValider.Width + 2 * (Width - buttonValider.Width - buttonAnnuler.Width) / 3;
            buttonValider.Top = buttonAnnuler.Top = Height - 3 * buttonValider.Height;
            #endregion

            #region positionnement des grilles
            dataGridViewBatailles.Width = Width - dataGridViewBatailles.Left * 2;
            dataGridViewBatailles.Height = Height / 3;

            dataGridViewBataillesPions.Top = dataGridViewBatailles.Height + 10 + dataGridViewBatailles.Top;
            dataGridViewBataillesPions.Width = Width - dataGridViewBataillesPions.Left * 2;
            dataGridViewBataillesPions.Height = Height - 4 * buttonValider.Height - dataGridViewBatailles.Height - 10 - dataGridViewBatailles.Top;
            #endregion
        }

        #region propriétés
        public Donnees.TAB_BATAILLE_VIDEODataTable tableBatailleVideo
        {
            get
            {
                return (Donnees.TAB_BATAILLE_VIDEODataTable)this.dataGridViewBatailles.DataSource;
            }
            set
            {
                dataGridViewBatailles.DataSource = value.Copy();
            }
        }

        public Donnees.TAB_BATAILLE_PIONS_VIDEODataTable tableBataillePionsVideo
        {
            get
            {
                return (Donnees.TAB_BATAILLE_PIONS_VIDEODataTable)this.dataGridViewBataillesPions.DataSource;
            }
            set
            {
                dataGridViewBataillesPions.DataSource = value.Copy();
            }
        }
        #endregion

    }
}
