using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace vaoc
{
    public partial class FormOrdre : Form
    {
        public FormOrdre()
        {
            InitializeComponent();
        }

        #region evenements
        private void FormOrdre_Load(object sender, EventArgs e)
        {
            Redimensionner();
        }

        private void FormOrdre_Resize(object sender, EventArgs e)
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

            #region positionnement de la grille
            dataGridViewOrdres.Width = Width - SystemInformation.VerticalScrollBarWidth;
            dataGridViewOrdres.Height = Height - 4 * buttonValider.Height - SystemInformation.HorizontalScrollBarHeight;
            #endregion
        }
        #endregion

        #region propriétés
        public Donnees.TAB_ORDREDataTable tableOrdre
        {
            get
            {
                return (Donnees.TAB_ORDREDataTable)dataGridViewOrdres.DataSource;
            }
            set
            {
                dataGridViewOrdres.DataSource = value.Copy();
            }
        }
        #endregion
    }
}