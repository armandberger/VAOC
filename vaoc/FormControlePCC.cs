using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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
            Graphics g = e.Graphics;

            Font police = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Bold);
            g.DrawString((m_bloc.xBloc * m_tailleBloc)+ ","+(m_bloc.yBloc * m_tailleBloc), police, System.Drawing.Brushes.Black, new Point(0, 0));
            string texte = (m_bloc.xBloc+1) * m_tailleBloc + "," + (m_bloc.yBloc + 1) * m_tailleBloc;
            SizeF tailleTexte = g.MeasureString(texte, police);
            g.DrawString(texte, police, System.Drawing.Brushes.Black, new Point(this.Right- (int)tailleTexte.Width, this.Bottom- (int)tailleTexte.Height));

            
            g.DrawLine(System.Drawing.Pens.Red, this.PointToClient(new Point(this.Left, this.Top)), this.PointToClient(new Point(this.Right, this.Bottom)));
            g.DrawLine(System.Drawing.Pens.Green, 0, 0,
                this.Right-20, this.Bottom-100);
            Debug.WriteLine("R=" + this.Right + ", B=" + this.Bottom);
        }

        private void FormControlePCC_MouseClick(object sender, MouseEventArgs e)
        {
            groupBoxInfo.Visible = !groupBoxInfo.Visible;
        }
    }
}
