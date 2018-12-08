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
    public partial class FormNomsPions : Form
    {
        public FormNomsPions()
        {
            InitializeComponent();
        }

        #region propriétés
        public Donnees.TAB_NOMS_PIONSDataTable tableNomsPions
        {
            get
            {
                return (Donnees.TAB_NOMS_PIONSDataTable)dataGridViewNoms.DataSource;
            }
            set
            {
                dataGridViewNoms.DataSource = value.Copy();
            }
        }
        #endregion

        private void FormNomsPions_Load(object sender, EventArgs e)
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

            #region positionnement du champ commentaire
            labelCommentaire.Left = buttonValider.Left;
            labelCommentaire.Top = buttonAnnuler.Top - 2 * labelCommentaire.Height;
            #endregion

            #region positionnement de la grille
            dataGridViewNoms.Width = Width - SystemInformation.VerticalScrollBarWidth;
            dataGridViewNoms.Height = Height - 4 * buttonValider.Height - 3 * labelCommentaire.Height;
            #endregion
        }

        private void FormNomsPions_Resize(object sender, EventArgs e)
        {
            Redimensionner();
        }
    }
}
