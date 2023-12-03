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
            public int cout { get; set; }

            public Point debut { get; set; }
            public Point fin { get; set; }

            public CASETrajet(int xd, int yd, int xf, int yf, int i_COUT)
            {
                this.cout = i_COUT;
                this.debut = new Point(xd, yd);
                this.fin = new Point(xf, yf);
            }
        }

        public Bloc m_bloc;
        private List<CASETrajet> m_listeCases = new();
        Donnees.TAB_PCC_COUTSRow[] m_listeCouts;
        private int m_tailleBloc;
        private Point m_ptBlocBase;
        private Point m_ptSouris = new Point();
        private SizeF m_taillePoint = new Size();

        public FormControlePCC()
        {
            InitializeComponent();
        }

        private void FormControlePCC_Load(object sender, EventArgs e)
        {
            m_tailleBloc = Donnees.m_donnees.TAB_JEU[0].I_TAILLEBLOC_PCC;
            TailleChange();
            m_ptBlocBase = new Point((m_bloc.xBloc * m_tailleBloc), (m_bloc.yBloc * m_tailleBloc));
            string requete = string.Format("I_BLOCX={0} AND I_BLOCY={1}", m_bloc.xBloc, m_bloc.yBloc);
            m_listeCouts = (Donnees.TAB_PCC_COUTSRow[])Donnees.m_donnees.TAB_PCC_COUTS.Select(requete);
            foreach (Donnees.TAB_PCC_COUTSRow ligneCout in m_listeCouts)
            {
                Donnees.TAB_CASERow ligneCaseDebut = Donnees.m_donnees.TAB_CASE.FindParID_CASE(ligneCout.ID_CASE_DEBUT); ;
                Donnees.TAB_CASERow ligneCaseFin = Donnees.m_donnees.TAB_CASE.FindParID_CASE(ligneCout.ID_CASE_FIN); ;
                m_listeCases.Add(new CASETrajet(ligneCaseDebut.I_X, ligneCaseDebut.I_Y, ligneCaseFin.I_X, ligneCaseFin.I_Y, ligneCout.I_COUT));
            }
        }
        private Point CaseToEcran(int x, int y)
        {
            //return this.PointToClient(new Point(this.Left + x - m_ptBlocBase.X, this.Top + y - m_ptBlocBase.Y));
            return new Point((int)((x - m_ptBlocBase.X) * m_taillePoint.Width), (int)((y - m_ptBlocBase.Y) * m_taillePoint.Height));
        }
        private Point CaseToEcran(Point pt)
        {
            return CaseToEcran(pt.X, pt.Y);
        }
        private Point EcranToCase(int x, int y)
        {
            return new Point((int)(x / m_taillePoint.Width) + m_ptBlocBase.X, (int)(y / m_taillePoint.Height) + m_ptBlocBase.Y;
        }
        private Point EcranToCase(Point pt)
        {
            return EcranToCase(pt.X, pt.Y);
        }

        private void FormControlePCC_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            Font police = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Bold);
            g.DrawString(m_ptBlocBase.X + "," + m_ptBlocBase.Y, police, System.Drawing.Brushes.Black, new Point(0, 0));
            string texte = (m_ptBlocBase.X + m_tailleBloc) + "," + (m_ptBlocBase.Y + m_tailleBloc);
            SizeF tailleTexte = g.MeasureString(texte, police);
            g.DrawString(texte, police, System.Drawing.Brushes.Black, new Point(this.ClientSize.Width - (int)tailleTexte.Width, this.ClientSize.Height - (int)tailleTexte.Height));

            g.DrawLine(System.Drawing.Pens.Red, this.PointToClient(new Point(this.Left, this.Top)), this.PointToClient(new Point(this.Right, this.Bottom)));
            g.DrawLine(System.Drawing.Pens.Blue, new Point(0,0), new Point(this.ClientSize.Width, this.ClientSize.Height));
            foreach (CASETrajet trajet in m_listeCases)
            {
                Point ptDebut = CaseToEcran(trajet.debut);
                Point ptFin = CaseToEcran(trajet.fin);
                g.DrawLine(System.Drawing.Pens.Black, ptDebut, ptFin);
                //g.FillRectangle(Brushes.Black, new Rectangle(CaseToEcran(Case.X, Case.Y), m_taillePoint));
                Debug.WriteLine(string.Format("FormControlePCC_Paint ({0},{1}) -> ({2},{3})", ptDebut.X, ptDebut.Y, ptFin.X, ptFin.Y));
            }
        }

        private void FormControlePCC_MouseClick(object sender, MouseEventArgs e)
        {
            label2.Visible = !label2.Visible;
        }

        private void FormControlePCC_MouseMove(object sender, MouseEventArgs e)
        {
            Debug.WriteLine("FormControlePCC_MouseMove X=" + e.X + ", Y=" + e.Y);
            m_ptSouris.X = e.X;
            m_ptSouris.Y = e.Y;
            if (label2.Visible)
            {
                //Point Case = EcranToCase(e.Location);
                //TaskExtensions = string.Format("{0},{1}:{2}", Case.X, Case.Y, Case.c);
                label2.Location = new Point(e.X, e.Y);

            }
        }

        private void TailleChange()
        {
            if (m_tailleBloc > 0)
            {
                m_taillePoint.Width = (float)this.ClientSize.Width / m_tailleBloc;
                m_taillePoint.Height = (float)this.ClientSize.Height / m_tailleBloc;
            }
        }
        private void FormControlePCC_SizeChanged(object sender, EventArgs e)
        {
            TailleChange();
        }

        private void label2_Click(object sender, EventArgs e)
        {
            label2.Visible = !label2.Visible;
        }
    }
}
