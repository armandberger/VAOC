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
    public partial class FormVideoTravellingTable : Form
    {
        public FormVideoTravellingTable()
        {
            InitializeComponent();
        }

        #region propriétés
        public Donnees.TAB_TRAVELLINGDataTable tableTravelling
        {
            get
            {
                return (Donnees.TAB_TRAVELLINGDataTable)dataGridViewTravelling.DataSource;
            }
            set
            {
                this.dataGridViewTravelling.DataSource = value.Copy();
            }
        }
        #endregion

        private void Redimensionner()
        {
            #region positionnement des boutons annuler et valider
            buttonValider.Left = (Width - buttonValider.Width - buttonAnnuler.Width) / 3;
            buttonAnnuler.Left = buttonValider.Width + 2 * (Width - buttonValider.Width - buttonAnnuler.Width) / 3;
            buttonValider.Top = buttonAnnuler.Top = Height - 3 * buttonValider.Height;
            #endregion

            #region positionnement de la grille
            this.dataGridViewTravelling.Width = Width;
            this.dataGridViewTravelling.Height = Height - 4 * buttonValider.Height;
            #endregion
        }

        private void FormVideoTravellingTable_Load(object sender, EventArgs e)
        {
            Redimensionner();
        }

        private void FormVideoTravellingTable_Resize(object sender, EventArgs e)
        {
            Redimensionner();
        }
    }
}
