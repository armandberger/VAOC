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
    public partial class FormMajCases : Form
    {
        public FormMajCases()
        {
            InitializeComponent();
        }
        private void Redimensionner()
        {
            #region positionnement des résultats
            textBoxResultat.Top = buttonValider.Bottom + buttonValider.Height;
            textBoxResultat.Width = Width - 3 * textBoxResultat.Left;
            textBoxResultat.Height = Height - 6 * buttonValider.Height;
            #endregion
        }

        private void FormMajCases_Load(object sender, EventArgs e)
        {
            Redimensionner();
        }

        private void FormMajCases_Resize(object sender, EventArgs e)
        {
            Redimensionner();
        }
        private void buttonGeneration_Click(object sender, EventArgs e)
        {
            StringBuilder texte = new StringBuilder();
            int nbcases=0;
            textBoxResultat.Clear();
            // on charge toutes les cases qui ne seraient pas encore chargées
            Donnees.m_donnees.ChargerToutesLesCases();
            //retrait de tous les nouveaux proprietaires qui auraient du être à null de toute manière
            foreach(Donnees.TAB_CASERow ligneCase in Donnees.m_donnees.TAB_CASE)
            {
                if (!ligneCase.IsID_NOUVEAU_PROPRIETAIRENull())
                {
                    texte.AppendLine(string.Format("{0}:{1},{2} occupée par {3}", ligneCase.ID_CASE, ligneCase.I_X, ligneCase.I_Y, ligneCase.ID_NOUVEAU_PROPRIETAIRE));
                    ligneCase.SetID_NOUVEAU_PROPRIETAIRENull();
                    nbcases++;
                }
            }
            textBoxResultat.Text = texte.ToString();
            textBoxResultat.Text += nbcases + " modifiées.";
        }

    }
}
