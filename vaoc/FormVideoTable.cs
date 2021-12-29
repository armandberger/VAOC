using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WaocLib;

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
            int intervalleBoutons = Width - (buttonValider.Width + buttonAnnuler.Width + buttonExportCSV.Width);
            buttonValider.Left = intervalleBoutons / 4;
            buttonAnnuler.Left = buttonValider.Width + 2 * intervalleBoutons / 4;
            buttonExportCSV.Left = buttonExportCSV.Width + 3 * intervalleBoutons / 4;
            buttonValider.Top = buttonAnnuler.Top = buttonExportCSV.Top = Height - 3 * buttonValider.Height;
            #endregion

            #region positionnement de la grille
            this.dataGridViewVideo.Width = Width;
            this.dataGridViewVideo.Height = Height - 4 * buttonValider.Height;
            #endregion
        }

        private void buttonExportCSV_Click(object sender, EventArgs e)
        {
            string nomfichier;
            //on ajoute des colonnes pour les traitements statistiques sous tableur ensuite
            Donnees.TAB_VIDEODataTable table = (Donnees.TAB_VIDEODataTable)((Donnees.TAB_VIDEODataTable)dataGridViewVideo.DataSource).Copy();
            table.Columns.Add("UniteDeCombat", typeof(bool));
            foreach (Donnees.TAB_VIDEORow ligneVideo in table)
            {
                if (ligneVideo.ID_PION > 0)
                { 
                    Donnees.TAB_PIONRow lignePion = Donnees.m_donnees.TAB_PION.FindByID_PION(ligneVideo.ID_PION);
                    ligneVideo["UniteDeCombat"] = lignePion.estCombattifQG(false, true);
                }
                else
                {
                    //possible sur les prisons/hopitaux, etc.
                    ligneVideo["UniteDeCombat"] = false;
                }
            }

            string messageErreur = Dal.exportCSV(table, out nomfichier);
            //string messageErreur = Dal.exportCSV(dataGridViewVideo, "DonneesVideo", out nomfichier);

            if (string.Empty == messageErreur)
            {
                MessageBox.Show("Fichier CSV exporté : " + nomfichier, "FormPion", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Erreur sur l'export du fichier CSV : " + nomfichier + " : " + messageErreur,
                    "FormPion", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridViewVideo_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
