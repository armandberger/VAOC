﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WaocLib;
using static vaoc.FormControlePCC;

namespace vaoc
{
    public partial class FormControlePCC : Form
    {
        public class CASETrajet
        {
            public int cout { get; set; }
            public int id { get; set; }
            public Point debut { get; set; }
            public Point fin { get; set; }

            public CASETrajet(int idTrajet, int xd, int yd, int xf, int yf, int i_COUT)
            {
                this.id = idTrajet;
                this.cout = i_COUT;
                this.debut = new Point(xd, yd);
                this.fin = new Point(xf, yf);
            }
        }

        public Bloc m_bloc;
        private readonly List<CASETrajet> m_listeCases = new();
        Donnees.TAB_PCC_COUTSRow[] m_listeCouts;
        private int m_tailleBloc;
        private Point m_ptBlocBase;
        //private Point m_ptSouris = new();
        private SizeF m_taillePoint = new();
        private int idTrajetSelection = -1;
        private Bitmap m_imageFond;
        
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
                m_listeCases.Add(new CASETrajet(ligneCout.ID_TRAJET, ligneCaseDebut.I_X, ligneCaseDebut.I_Y, ligneCaseFin.I_X, ligneCaseFin.I_Y, ligneCout.I_COUT));
            }

            //Image de fond
            if (Donnees.m_donnees.TAB_JEU.Count > 0 && !Donnees.m_donnees.TAB_JEU[0].IsS_NOM_CARTE_TOPOGRAPHIQUENull()
                && Donnees.m_donnees.TAB_JEU[0].S_NOM_CARTE_TOPOGRAPHIQUE.Length > 0)
            {
                if (File.Exists(Constantes.repertoireDonnees + Donnees.m_donnees.TAB_JEU[0].S_NOM_CARTE_TOPOGRAPHIQUE))
                {
                    Image imageCarte = Image.FromFile(Constantes.repertoireDonnees + Donnees.m_donnees.TAB_JEU[0].S_NOM_CARTE_TOPOGRAPHIQUE);
                    BitmapData imageCible = new();
                    Rectangle rect = new Rectangle(m_ptBlocBase.X, m_ptBlocBase.Y, m_tailleBloc, m_tailleBloc);
                    ((Bitmap)imageCarte).LockBits(rect, ImageLockMode.ReadOnly, imageCarte.PixelFormat, imageCible);
                    ((Bitmap)imageCarte).UnlockBits(imageCible);
                    m_imageFond = new Bitmap(imageCible.Width, imageCible.Height, imageCible.Stride, imageCible.PixelFormat, imageCible.Scan0);
                }
                else
                {
                    m_imageFond = null;
                }
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
            return new Point((int)(x / m_taillePoint.Width) + m_ptBlocBase.X, (int)(y / m_taillePoint.Height) + m_ptBlocBase.Y);
        }
        private Point EcranToCase(Point pt)
        {
            return EcranToCase(pt.X, pt.Y);
        }

        private void FormControlePCC_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            //g.DrawLine(System.Drawing.Pens.Red, this.PointToClient(new Point(this.Left, this.Top)), this.PointToClient(new Point(this.Right, this.Bottom)));
            //g.DrawLine(System.Drawing.Pens.Blue, new Point(0,0), new Point(this.ClientSize.Width, this.ClientSize.Height));
            g.DrawImage(m_imageFond, 0, 0,this.ClientSize.Width, this.ClientSize.Height); 

            Pen styloSelection = new(Color.Red, 3);
            foreach (CASETrajet trajet in m_listeCases)
            {
                Point ptDebut = CaseToEcran(trajet.debut);
                Point ptFin = CaseToEcran(trajet.fin);
                if (idTrajetSelection == trajet.id)
                {
                    g.DrawLine(styloSelection, ptDebut, ptFin);
                }
                else
                {
                    g.DrawLine(System.Drawing.Pens.Black, ptDebut, ptFin);
                }
                //g.FillRectangle(Brushes.Black, new Rectangle(CaseToEcran(Case.X, Case.Y), m_taillePoint));
                Debug.WriteLine(string.Format("FormControlePCC_Paint ({0},{1}) -> ({2},{3})", ptDebut.X, ptDebut.Y, ptFin.X, ptFin.Y));
            }

            //affichage des coordonnées des coins
            Font police = new(FontFamily.GenericSansSerif, 15, FontStyle.Bold);
            g.DrawString(m_ptBlocBase.X + "," + m_ptBlocBase.Y, police, System.Drawing.Brushes.DarkBlue, new Point(0, 0));
            string texte = (m_ptBlocBase.X + m_tailleBloc) + "," + (m_ptBlocBase.Y + m_tailleBloc);
            SizeF tailleTexte = g.MeasureString(texte, police);
            g.DrawString(texte, police, System.Drawing.Brushes.DarkBlue, new Point(this.ClientSize.Width - (int)tailleTexte.Width, this.ClientSize.Height - (int)tailleTexte.Height));
        }

        private void FormControlePCC_MouseClick(object sender, MouseEventArgs e)
        {
            BoiteInformation.Visible = !BoiteInformation.Visible;
            if (BoiteInformation.Visible)
            {
                BoiteInformation.Location = e.Location;

                //recherche de la ligne la plus proche du point de clic https://stackoverflow.com/questions/3120357/get-closest-point-to-a-line
                Point clicCase = EcranToCase(e.Location);
                double distanceMin = double.MaxValue;
                CASETrajet trajetMin = new(-1,0,0,0,0,0);

                foreach (CASETrajet trajet in m_listeCases)
                {
                    Point a_to_b = new(trajet.fin.X - trajet.debut.X, trajet.fin.Y - trajet.debut.Y);   // Finding the vector from A to B
                    Point perpendicular = new(-a_to_b.Y, a_to_b.X);  // The vector perpendicular to a_to_b
                    Point Q = new(clicCase.X + perpendicular.X, clicCase.Y + perpendicular.Y); // Finding Q, the point "in the right direction"
                    if (((trajet.debut.X - trajet.fin.X) * (clicCase.Y - Q.Y) - (trajet.debut.X - trajet.fin.Y) * (clicCase.Y - Q.Y)) != 0
                        && ((trajet.debut.X - trajet.fin.X) * (clicCase.Y - Q.Y) - (clicCase.Y - trajet.fin.Y) * (clicCase.Y - Q.Y)) != 0)
                    {
                        Point I = new(((trajet.debut.X * trajet.fin.Y - trajet.debut.Y * trajet.fin.X) * (clicCase.X - Q.X) - (trajet.debut.X - trajet.fin.X) * (clicCase.X * Q.Y - clicCase.Y * Q.X)) /
                                             ((trajet.debut.X - trajet.fin.X) * (clicCase.Y - Q.Y) - (trajet.debut.X - trajet.fin.Y) * (clicCase.Y - Q.Y)),
                            ((trajet.debut.X * trajet.fin.Y - trajet.debut.Y * trajet.fin.X) * (clicCase.Y - Q.Y) - (trajet.debut.Y - trajet.fin.Y) * (clicCase.X * Q.Y - clicCase.Y * Q.X)) /
                            ((trajet.debut.X - trajet.fin.X) * (clicCase.Y - Q.Y) - (clicCase.Y - trajet.fin.Y) * (clicCase.Y - Q.Y)));

                        Point P = clicCase;
                        Point A = trajet.debut;
                        Point B = trajet.fin;
                        Point a_to_p = new(P.X - A.X, P.Y - A.Y);     // Storing vector A->P
                        a_to_b = new(B.X - A.X, B.Y - A.Y);     // Storing vector A->B

                        double atb2 = Math.Pow(a_to_b.X, 2) + Math.Pow(a_to_b.Y, 2);  // **2 means "squared"  BasicallY finding the squared magnitude of a_to_b
                        int atp_dot_atb = a_to_p.X * a_to_b.X + a_to_p.Y * a_to_b.Y;  // The dot product of a_to_p and a_to_b
                        double t = atp_dot_atb / atb2;              // The normalized "distance" from a to Your closest point

                        Point I2 = new((int)(A.X + a_to_b.X * t), (int)(A.Y + a_to_b.Y * t)); // Add the distance to A, moving towards B
                        Debug.WriteLine(string.Format("FormControlePCC I1({0},{1}), I2 ({2},{3})", I.X, I.Y, I2.X, I2.Y));
                        double distance = Constantes.Distance(clicCase.X, clicCase.Y, I2.X, I2.Y);
                        if (distance < distanceMin)
                        {
                            distanceMin = distance;
                            idTrajetSelection = trajet.id;
                            trajetMin = trajet;
                        }
                    }
                }
                this.textBoxIDTrajet.Text = trajetMin.id.ToString();
                this.textBoxCout.Text = trajetMin.cout.ToString();
                this.textBoxDebut.Text = trajetMin.debut.X+","+trajetMin.debut.Y;
                this.textBoxFin.Text= trajetMin.fin.X+","+trajetMin.fin.Y;
                this.Invalidate();
            }
        }

        private void FormControlePCC_MouseMove(object sender, MouseEventArgs e)
        {
            //Debug.WriteLine("FormControlePCC_MouseMove X=" + e.X + ", Y=" + e.Y);
            //m_ptSouris.X = e.X;
            //m_ptSouris.Y = e.Y;
            //if (label2.Visible)
            //{
            //    //Point Case = EcranToCase(e.Location);
            //    //TaskExtensions = string.Format("{0},{1}:{2}", Case.X, Case.Y, Case.c);
            //    label2.Location = new Point(e.X, e.Y);

            //}
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
    }
}
