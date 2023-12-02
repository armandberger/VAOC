using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace vaoc
{
    public partial class FormControlePCC : Form
    {
        public class CASETrajet
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int idTrajet { get; set; }
            public int idCase { get; set; }
            public CASETrajet(int idcase, int x, int y, int idTrajet)
            {
                this.idCase = idcase;
                this.X = x;
                this.Y = y;
                this.idTrajet = idTrajet;
            }
        }

        public Bloc m_bloc;
        private List<CASETrajet> listeCases = new();
        private int m_tailleBloc;

        public FormControlePCC()
        {
            InitializeComponent();
        }

        private void FormControlePCC_Load(object sender, EventArgs e)
        {
            m_tailleBloc = Donnees.m_donnees.TAB_JEU[0].I_TAILLEBLOC_PCC;
            string requete = string.Format("I_BLOCX={0} AND I_BLOCY={1}", m_bloc.xBloc, m_bloc.yBloc);
            Donnees.TAB_PCC_COUTSRow[] listeTrajets = (Donnees.TAB_PCC_COUTSRow[])Donnees.m_donnees.TAB_PCC_COUTS.Select(requete);
            foreach (Donnees.TAB_PCC_COUTSRow trajet in listeTrajets)
            {
                Donnees.TAB_PCC_TRAJETRow[] tableTrajet = (Donnees.TAB_PCC_TRAJETRow[])Donnees.m_donnees.TAB_PCC_TRAJET.Select("ID_TRAJET=" + trajet.ID_TRAJET.ToString(), "I_ORDRE");
                foreach (Donnees.TAB_PCC_TRAJETRow ligneTrajet in tableTrajet)
                {
                    Donnees.TAB_CASERow ligneCase = Donnees.m_donnees.TAB_CASE.FindParID_CASE(ligneTrajet.ID_CASE);
                    listeCases.Add(new CASETrajet(ligneTrajet.ID_CASE, ligneCase.I_X, ligneCase.I_Y, ligneTrajet.ID_TRAJET));
                }
            }
        }
        private void FormControlePCC_Paint(object sender, PaintEventArgs e)
        {

        }

    }
}
