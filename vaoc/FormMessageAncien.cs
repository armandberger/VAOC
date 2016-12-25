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
    public partial class FormMessageAncien : Form
    {
        public FormMessageAncien()
        {
            InitializeComponent();
        }

        #region evenements
        private void FormMessageAncien_Resize(object sender, EventArgs e)
        {
            Redimensionner();
        }

        private void FormMessageAncien_Load(object sender, EventArgs e)
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
            dataGridViewMessages.Width = Width - SystemInformation.VerticalScrollBarWidth;
            dataGridViewMessages.Height = Height - 4 * buttonValider.Height - SystemInformation.HorizontalScrollBarHeight;
            #endregion
        }
         #endregion

        #region propriétés
        public Donnees.TAB_MESSAGE_ANCIENDataTable tableMessage
        {
            get
            {
                return (Donnees.TAB_MESSAGE_ANCIENDataTable)dataGridViewMessages.DataSource;
            }
            set
            {
                dataGridViewMessages.DataSource = value.Copy();
            }
        }
        #endregion

    }
}
