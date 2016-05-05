using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace vaoc
{
    public partial class FormNomsPromus : Form
    {
        public FormNomsPromus()
        {
            InitializeComponent();
        }

        #region evenements

        private void FormNomsPromus_Load(object sender, EventArgs e)
        {
            Redimensionner();
        }

        private void FormNomsPromus_Resize(object sender, EventArgs e)
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
            dataGridViewNomsPromus.Width = Width - SystemInformation.VerticalScrollBarWidth;
            dataGridViewNomsPromus.Height = Height - 4 * buttonValider.Height - SystemInformation.HorizontalScrollBarHeight;
            #endregion
        }
        #endregion

        #region propriétés
        public Donnees.TAB_NOMS_PROMUSDataTable tableNomsPromus
        {
            get
            {
                return (Donnees.TAB_NOMS_PROMUSDataTable)dataGridViewNomsPromus.DataSource;
            }
            set
            {
                dataGridViewNomsPromus.DataSource = value.Copy();
            }
        }
        #endregion

    }
}