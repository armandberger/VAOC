using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;

using WaocLib;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Threading;

namespace vaoc
{
    class Cartographie
    {
        //static protected AStar m_star = null;
        static protected Bitmap m_imageCarteHistorique;
        static protected Bitmap m_imageCarteGris;
        static protected Bitmap m_imageCarteZoom;
        static protected Bitmap m_imageCarteTopographique;
        static protected float m_rapportZoom;

        //internal static AStar etoile
        //{
        //    get
        //    {
        //        if (null == m_star)
        //        {
        //            m_star = new AStar();
        //        }
        //        return m_star;
        //    }
        //}

        internal static float rapportZoom
        {
            get
            {
                return m_rapportZoom;
            }
        }

        public static bool ChargerLesFichiers()
        {
            return ChargerLesFichiers(true);
        }

        public static bool ChargerLesFichiers(bool bForcage)
        {
            //chargement des fichiers de la carte
            try
            {
                if (null == m_imageCarteHistorique || bForcage)
                {
                    if (null != m_imageCarteHistorique) { m_imageCarteHistorique.Dispose(); }
                    m_imageCarteHistorique = (Bitmap)Image.FromFile(Constantes.repertoireDonnees + Donnees.m_donnees.TAB_JEU[0].S_NOM_CARTE_HISTORIQUE);
                }
            }
            catch (Exception ex)
            {
                LogFile.Notifier(string.Format("ChargerLesFichiers Erreur {1} au chargement de l'image S_NOM_CARTE_HISTORIQUE: {0}", Donnees.m_donnees.TAB_JEU[0].S_NOM_CARTE_HISTORIQUE, ex.Message));
                return false;
            }
            try
            {
                //dorénavant on prend la même carte que la carte historique
                if (null == m_imageCarteGris || bForcage) /*&& !Donnees.m_donnees.TAB_JEU[0].IsS_NOM_CARTE_GRISNull() && Donnees.m_donnees.TAB_JEU[0].S_NOM_CARTE_GRIS.Length > 0)*/
                {
                    if (null != m_imageCarteGris) { m_imageCarteGris.Dispose(); }
                    //m_imageCarteGris = (Bitmap)Image.FromFile(Constantes.repertoireDonnees + Donnees.m_donnees.TAB_JEU[0].S_NOM_CARTE_GRIS);
                    m_imageCarteGris = (Bitmap)Image.FromFile(Constantes.repertoireDonnees + Donnees.m_donnees.TAB_JEU[0].S_NOM_CARTE_HISTORIQUE);
                }
            }
            catch (Exception ex)
            {
                LogFile.Notifier(string.Format("ChargerLesFichiers Erreur {1} au chargement de l'image S_NOM_CARTE_GRIS : {0}", Donnees.m_donnees.TAB_JEU[0].S_NOM_CARTE_GRIS, ex.Message));
                return false;
            }
            try
            {
                if((null == m_imageCarteZoom || bForcage) && !Donnees.m_donnees.TAB_JEU[0].IsS_NOM_CARTE_ZOOMNull() && Donnees.m_donnees.TAB_JEU[0].S_NOM_CARTE_ZOOM.Length > 0)
                {
                    if (null != m_imageCarteZoom) { m_imageCarteZoom.Dispose(); }
                    m_imageCarteZoom = (Bitmap)Image.FromFile(Constantes.repertoireDonnees + Donnees.m_donnees.TAB_JEU[0].S_NOM_CARTE_ZOOM);
                }
            }
            catch (Exception ex)
            {
                LogFile.Notifier(string.Format("ChargerLesFichiers Erreur {1} au chargement de l'image S_NOM_CARTE_ZOOM : {0}", Donnees.m_donnees.TAB_JEU[0].S_NOM_CARTE_ZOOM, ex.Message));
                //return false; c'est possible maintenant, de ne pas avoir de carte zoom
            }
            try
            {
                if (null == m_imageCarteTopographique || bForcage)
                {
                    if (null != m_imageCarteTopographique) { m_imageCarteTopographique.Dispose(); }
                    m_imageCarteTopographique = (Bitmap)Image.FromFile(Constantes.repertoireDonnees + Donnees.m_donnees.TAB_JEU[0].S_NOM_CARTE_TOPOGRAPHIQUE);
                }
            }
            catch (Exception ex)
            {
                LogFile.Notifier(string.Format("ChargerLesFichiers Erreur {1} au chargement de l'image S_NOM_CARTE_TOPOGRAPHIQUE : {0}", Donnees.m_donnees.TAB_JEU[0].S_NOM_CARTE_TOPOGRAPHIQUE, ex.Message));
                return false;
            }
            m_rapportZoom = (null != m_imageCarteZoom) ? (float)m_imageCarteZoom.Width / (float)m_imageCarteHistorique.Width : 1;
            
            return true;
        }

        public static void AfficherBatailles(Bitmap imageSource)
        {
            try
            {
                Pen stylo = new Pen(Color.Red);
                Graphics graph = Graphics.FromImage(imageSource);

                foreach (Donnees.TAB_BATAILLERow ligneBataille in Donnees.m_donnees.TAB_BATAILLE)
                {
                    if (ligneBataille.IsI_TOUR_FINNull())
                    {
                        graph.DrawRectangle(stylo,
                            ligneBataille.I_X_CASE_HAUT_GAUCHE,
                            ligneBataille.I_Y_CASE_HAUT_GAUCHE,
                            ligneBataille.I_X_CASE_BAS_DROITE - ligneBataille.I_X_CASE_HAUT_GAUCHE,
                            ligneBataille.I_Y_CASE_BAS_DROITE - ligneBataille.I_Y_CASE_HAUT_GAUCHE
                            );
                    }
                }
                graph.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("AfficherBatailles exception :" + ex.Message,
                    "Cartographie", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void AfficherArriveeDepart(Bitmap imageSource, Donnees.TAB_CASERow ligneCaseDepart, Donnees.TAB_CASERow ligneCaseArrivee, Color couleur, int taillePinceau)
        {
            Graphics graph = Graphics.FromImage(imageSource);
            Pen stylo = new Pen(couleur, taillePinceau);

            if (null != ligneCaseDepart)
            {
                Point[] triangle = new Point[3];
                triangle[0] = new Point(ligneCaseDepart.I_X, ligneCaseDepart.I_Y - 10);
                triangle[1] = new Point(ligneCaseDepart.I_X + 10, ligneCaseDepart.I_Y + 10);
                triangle[2] = new Point(ligneCaseDepart.I_X - 10, ligneCaseDepart.I_Y + 10);
                graph.DrawPolygon(stylo, triangle);
            }

            if (null != ligneCaseArrivee)
            {
                graph.DrawEllipse(stylo, ligneCaseArrivee.I_X - 10, ligneCaseArrivee.I_Y - 10, 20, 20);
            }
            graph.Dispose();
        }

        public static void AfficherArriveeDepart(Bitmap imageSource, LigneCASE ligneCaseDepart, LigneCASE ligneCaseArrivee, Color couleur, int taillePinceau)
        {
            Graphics graph = Graphics.FromImage(imageSource);
            Pen stylo = new Pen(couleur, taillePinceau);

            if (null != ligneCaseDepart)
            {
                Point[] triangle = new Point[3];
                triangle[0] = new Point(ligneCaseDepart.I_X, ligneCaseDepart.I_Y - 10);
                triangle[1] = new Point(ligneCaseDepart.I_X + 10, ligneCaseDepart.I_Y + 10);
                triangle[2] = new Point(ligneCaseDepart.I_X - 10, ligneCaseDepart.I_Y + 10);
                graph.DrawPolygon(stylo, triangle);
            }

            if (null != ligneCaseArrivee)
            {
                graph.DrawEllipse(stylo, ligneCaseArrivee.I_X - 10, ligneCaseArrivee.I_Y - 10, 20, 20);
            }
            graph.Dispose();
        }

        public static void AfficherChemin(Bitmap imageSource, List<Donnees.TAB_CASERow> chemin, Color couleur, int taillePinceau)
        {
            Graphics graph = Graphics.FromImage(imageSource);
            int x1, y1;
            
            Pen stylo = new Pen(couleur,taillePinceau);
            //stylo.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            //stylo.DashOffset = 5;
            //System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            x1 = y1 = -1;
            //LogFile.CreationLogFile("C:\\Users\\Public\\Documents\\vaoc\\poudre_et_biere\\test.log");
            foreach (Donnees.TAB_CASERow noeud in chemin)
            {
                if (x1 == -1 || y1 == -1)
                {
                    x1 = noeud.I_X;
                    y1 = noeud.I_Y;
                }
                else
                {
                    if (Math.Abs(x1 - noeud.I_X) <= 1 && Math.Abs(y1 - noeud.I_Y) <= 1)
                    {
                        //path.AddLine(x1, y1, noeud.I_X, noeud.I_Y);
                        graph.DrawRectangle(stylo, noeud.I_X, noeud.I_Y, 1, 1);//trace avec des points, permet de voir s'il en manque
                    }
                    x1 = noeud.I_X;
                    y1 = noeud.I_Y;
                    //LogFile.Notifier("AfficherChemin x=" + x1 + " y=" + y1); -> attention provoque des crashs si laissé en cours de traitement
                }
            }
            //graph.DrawPath(stylo, path);
            graph.Dispose();
        }

        public static void AfficherChemin(Bitmap imageSource, List<LigneCASE> chemin, Color couleur, int taillePinceau)
        {
            Graphics graph = Graphics.FromImage(imageSource);
            int x1, y1;

            Pen stylo = new Pen(couleur, taillePinceau);
            x1 = y1 = -1;
            foreach (LigneCASE noeud in chemin)
            {
                if (x1 == -1 || y1 == -1)
                {
                    x1 = noeud.I_X;
                    y1 = noeud.I_Y;
                }
                else
                {
                    if (Math.Abs(x1 - noeud.I_X) <= 1 && Math.Abs(y1 - noeud.I_Y) <= 1)
                    {
                        //path.AddLine(x1, y1, noeud.I_X, noeud.I_Y);
                        graph.DrawRectangle(stylo, noeud.I_X, noeud.I_Y, 1, 1);//trace avec des points, permet de voir s'il en manque
                    }
                    x1 = noeud.I_X;
                    y1 = noeud.I_Y;
                    LogFile.Notifier("AfficherChemin x=" + x1 + " y=" + y1);
                }
            }
            //graph.DrawPath(stylo, path);
            graph.Dispose();
        }

        public static void AfficherUnites(Constantes.MODELESCARTE modele)
        {
            Bitmap imageSource = GetImage(modele);
            if (Constantes.MODELESCARTE.ZOOM == modele)
            {
                AfficherUnites(imageSource, m_rapportZoom, 0, 0, Donnees.m_donnees.TAB_JEU[0].I_LARGEUR_CARTE - 1, Donnees.m_donnees.TAB_JEU[0].I_HAUTEUR_CARTE - 1);
            }
            else
            {
                AfficherUnites(imageSource);
            }
        }

        public static void AfficherUnites(Bitmap imageSource)
        {
            AfficherUnites(imageSource, 1, 0, 0, Donnees.m_donnees.TAB_JEU[0].I_LARGEUR_CARTE - 1, Donnees.m_donnees.TAB_JEU[0].I_HAUTEUR_CARTE - 1);
        }

        public static void AfficherUnites(Bitmap imageSource, float zoom, int xCaseHautGauche, int yCaseHautGauche, int xCaseBasDroite, int yCaseBasDroite)
        {
            Color couleur;
            Graphics graph;
            Rectangle rect;
            SolidBrush brosse;

            if (null == imageSource) return; // possible si fichier zoom non renseigné, car trop lourd pour être chargé
            graph = Graphics.FromImage(imageSource);

            Monitor.Enter(Donnees.m_donnees.TAB_CASE.Rows.SyncRoot);
            foreach (Donnees.TAB_CASERow noeud in Donnees.m_donnees.TAB_CASE)
            {
                if (noeud.I_X < xCaseHautGauche || noeud.I_X > xCaseBasDroite) { continue; }
                if (noeud.I_Y < yCaseHautGauche || noeud.I_Y > yCaseBasDroite) { continue; }
                if (!noeud.IsID_PROPRIETAIRENull())
                {
                    Donnees.TAB_PIONRow lignePion = Donnees.m_donnees.TAB_PION.FindByID_PION(noeud.ID_PROPRIETAIRE);
                    if (null != lignePion)
                    {
                        Donnees.TAB_MODELE_PIONRow ligneModelePion = lignePion.modelePion;
                        couleur = Color.FromArgb(ligneModelePion.I_ROUGE, ligneModelePion.I_VERT, ligneModelePion.I_BLEU);
                        //stylo = new Pen(couleur,zoom);
                        //graph.DrawLine(stylo, (int)(noeud.I_X * zoom), (int)(noeud.I_Y * zoom), (int)(noeud.I_X * zoom), (int)(noeud.I_Y * zoom));
                        //imageSource.SetPixel((int)(noeud.I_X * zoom), (int)(noeud.I_Y * zoom), couleur);
                        brosse = new SolidBrush(couleur);
                        rect = new Rectangle((int)((noeud.I_X - xCaseHautGauche) * zoom - zoom / 2), (int)((noeud.I_Y - yCaseHautGauche) * zoom - zoom / 2), (int)zoom + 1, (int)zoom + 1);
                        graph.FillEllipse(brosse, rect);
                    }
                }
                else
                {
                    if (!noeud.IsID_NOUVEAU_PROPRIETAIRENull())
                    {
                        Donnees.TAB_PIONRow lignePion = Donnees.m_donnees.TAB_PION.FindByID_PION(noeud.ID_NOUVEAU_PROPRIETAIRE);
                        if (null != lignePion)
                        {
                            Donnees.TAB_MODELE_PIONRow ligneModelePion = lignePion.modelePion;
                            couleur = Color.FromArgb(ligneModelePion.I_ROUGE, ligneModelePion.I_VERT, ligneModelePion.I_BLEU);
                            brosse = new SolidBrush(couleur);
                            rect = new Rectangle((int)((noeud.I_X - xCaseHautGauche) * zoom - zoom / 2), (int)((noeud.I_Y - yCaseHautGauche) * zoom - zoom / 2), (int)zoom + 1, (int)zoom + 1);
                            graph.FillEllipse(brosse, rect);
                        }
                    }
                }
            }
            Monitor.Exit(Donnees.m_donnees.TAB_CASE.Rows.SyncRoot);
            graph.Dispose();
        }

        public static void AfficherDepots(Bitmap imageSource)
        {
            Donnees.TAB_CASERow ligneCase;
            Font police;
            SizeF tailleLigne;
            SolidBrush brosse;
            Graphics graph = Graphics.FromImage(imageSource);
            string messageErreur = string.Empty;

            //on redessine la carte avec les dépôts
            brosse = new SolidBrush(Color.Black);
            police = new Font(FontFamily.GenericSansSerif, 20, FontStyle.Regular | FontStyle.Bold);
            tailleLigne = graph.MeasureString("D", police);
            foreach (Donnees.TAB_NOMS_CARTERow ligneNom in Donnees.m_donnees.TAB_NOMS_CARTE)
            {
                if (ligneNom.B_CREATION_DEPOT)
                {
                    ligneCase = Donnees.m_donnees.TAB_CASE.FindParID_CASE(ligneNom.ID_CASE);
                    if (null == ligneCase)
                    {
                        //possible en construction de cartes quand on repart d'un existant
                        //mais  il faut quand meme le signaler car sinon on ne peut pas savoir qu'il reste de vieux noms
                        messageErreur += string.Format("Case {0} de la ville {1} introuvable\r\n", ligneNom.ID_CASE, ligneNom.S_NOM);
                        continue;
                    }
                    int x = (int)Math.Floor(ligneCase.I_X - tailleLigne.Width / 2);
                    int y = (int)Math.Floor(ligneCase.I_Y - tailleLigne.Height / 2);
                    graph.DrawString("D", police, brosse, x, y);
                }
            }

            if (messageErreur != string.Empty)
            {
                MessageBox.Show(messageErreur, "AfficherNoms", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            graph.Dispose();
        }

        public static void AfficherNoms(Bitmap imageSource)
        {
            Donnees.TAB_POLICERow lignePolice;
            Donnees.TAB_CASERow ligneCase;
            Font police;
            SizeF tailleLigne;
            FontStyle stylePolice = FontStyle.Regular;
            SolidBrush brosse;
            Graphics graph = Graphics.FromImage(imageSource);
            string messageErreur = string.Empty;

            //on redessine la carte avec les villes
            foreach (Donnees.TAB_NOMS_CARTERow ligneNom in Donnees.m_donnees.TAB_NOMS_CARTE)
            {
                int x = 0, y = 0;

                lignePolice = Donnees.m_donnees.TAB_POLICE.FindByID_POLICE(ligneNom.ID_POLICE);
                ligneCase = Donnees.m_donnees.TAB_CASE.FindParID_CASE(ligneNom.ID_CASE);
                if (null == ligneCase)
                {
                    //possible en construction de cartes quand on repart d'un existant
                    //mais  il faut quand meme le signaler car sinon on ne peut pas savoir qu'il reste de vieux noms
                    messageErreur += string.Format("Case {0} de la ville {1} introuvable\r\n", ligneNom.ID_CASE, ligneNom.S_NOM);
                    continue;
                }
                brosse = new SolidBrush(Color.FromArgb(lignePolice.I_ROUGE, lignePolice.I_VERT, lignePolice.I_BLEU));
                if (lignePolice.B_BARRE)
                {
                    stylePolice |= FontStyle.Strikeout;
                }
                if (lignePolice.B_GRAS)
                {
                    stylePolice |= FontStyle.Bold;
                }
                if (lignePolice.B_ITALIQUE)
                {
                    stylePolice |= FontStyle.Italic;
                }
                if (lignePolice.B_SOULIGNE)
                {
                    stylePolice |= FontStyle.Underline;
                }
                police = new Font(lignePolice.S_NOM_POLICE, lignePolice.I_TAILLE, stylePolice);
                tailleLigne = graph.MeasureString(ligneNom.S_NOM, police);
                switch (ligneNom.I_POSITION)
                {
                    case 0://Central
                        x = (int)Math.Floor(ligneCase.I_X - tailleLigne.Width / 2);
                        y = (int)Math.Floor(ligneCase.I_Y - tailleLigne.Height / 2);
                        break;
                    case 1://En Haut
                        x = (int)Math.Floor(ligneCase.I_X - tailleLigne.Width / 2);
                        y = ligneCase.I_Y - (int)tailleLigne.Height;
                        break;
                    case 2://En Haut à droite
                        x = ligneCase.I_X;
                        y = ligneCase.I_Y - (int)tailleLigne.Height;
                        break;
                    case 3://A droite
                        x = ligneCase.I_X;
                        y = (int)Math.Floor(ligneCase.I_Y - tailleLigne.Height / 2);
                        break;
                    case 4://En bas à droite
                        x = ligneCase.I_X;
                        y = ligneCase.I_Y;
                        break;
                    case 5://En bas
                        x = (int)Math.Floor(ligneCase.I_X - tailleLigne.Width / 2);
                        y = ligneCase.I_Y;
                        break;
                    case 6://En bas à gauche
                        x = ligneCase.I_X - (int)tailleLigne.Width;
                        y = ligneCase.I_Y;
                        break;
                    case 7://A gauche
                        x = ligneCase.I_X - (int)tailleLigne.Width;
                        y = (int)Math.Floor(ligneCase.I_Y - tailleLigne.Height / 2);
                        break;
                    case 8://En haut à gauche
                        x = ligneCase.I_X - (int)tailleLigne.Width;
                        y = ligneCase.I_Y - (int)tailleLigne.Height;
                        break;
                }
                graph.DrawString(ligneNom.S_NOM, police, brosse, x, y);
            }

            if (messageErreur != string.Empty)
            {
                MessageBox.Show(messageErreur, "AfficherNoms", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            // Color couleur;
            // Random hasard = new Random();

            // if (null == ImageCarte.Image)
            // {
            //     return;//pas d'image chargée
            // }
            // couleur = Color.FromArgb(hasard.Next(256), hasard.Next(256), hasard.Next(256));
            //Pen stylo = new Pen(couleur, hasard.Next(5));

            // graph.DrawLine(stylo, hasard.Next(ImageCarte.Image.Width),
            //     hasard.Next(ImageCarte.Image.Height),
            //     hasard.Next(ImageCarte.Image.Width),
            //     hasard.Next(ImageCarte.Image.Height));
            graph.Dispose();
        }

        internal static void RecadrerRect(Bitmap imageSource, ref Rectangle rect)
        {
            if (rect.X < 0) rect.X = 0;
            if (rect.Y < 0) rect.Y = 0;
            if (rect.Left + rect.Width + 1 > imageSource.Width)
            {
                rect.Width = imageSource.Width - rect.Left - 1;
            }
            if (rect.Top + rect.Height + 1 > imageSource.Height)
            {
                rect.Height = imageSource.Height - rect.Top - 1;
            }
        }

        public static Bitmap GetImage(Constantes.MODELESCARTE modele)
        {
            switch (modele)
            {
                case Constantes.MODELESCARTE.HISTORIQUE:
                    return m_imageCarteHistorique;
                case Constantes.MODELESCARTE.GRIS:
                    return m_imageCarteGris;
                case Constantes.MODELESCARTE.ZOOM:
                    return m_imageCarteZoom;
                case Constantes.MODELESCARTE.TOPOGRAPHIQUE:
                    return m_imageCarteTopographique;
            }
            return null;
        }

        public static int GetImageLargeur(Constantes.MODELESCARTE modele)
        {
            return GetImage(modele).Width;
        }

        public static int GetImageHauteur(Constantes.MODELESCARTE modele)
        {
            return GetImage(modele).Height;
        }
        /// <summary>
        /// Découpe un fichier et le tourne d'un angle si besoin
        /// </summary>
        /// <param name="imageSource">grande image d'origine</param>
        /// <param name="nomFichierFinal">nom du fichier de sauvegarde de l'image</param>
        /// <param name="rect">rectangle dans l'image finale</param>
        /// <param name="angleRotation">angle en degrés</param>
        /// <returns></returns>
        internal static bool DecoupeFichier(Constantes.MODELESCARTE modele, string nomFichierFinal, Rectangle rect, float angleRotation, float zoom)
        {
            Bitmap imageSource = GetImage(modele);
            Bitmap imageFinale;

            RecadrerRect(imageSource, ref rect);
            if (0 == rect.Height || 0 == rect.Width)
            {
                return true;//rien à faire ?
            }

            if (1 == zoom)
            {
                BitmapData imageCible = new BitmapData();
                imageSource.LockBits(rect, ImageLockMode.ReadOnly, imageSource.PixelFormat, imageCible);
                imageSource.UnlockBits(imageCible);
                imageFinale = new Bitmap((int)(imageCible.Width), (int)(imageCible.Height), imageCible.Stride, imageCible.PixelFormat, imageCible.Scan0);
            }
            else
            {
                imageFinale = new Bitmap((int)(rect.Width * zoom), (int)(rect.Height * zoom), imageSource.PixelFormat);
                imageFinale.SetResolution(imageSource.HorizontalResolution, imageSource.VerticalResolution);
                Graphics graph = Graphics.FromImage(imageFinale);
                graph.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graph.DrawImage(imageSource, new Rectangle(0, 0, (int)(rect.Width * zoom), (int)(rect.Height * zoom)), rect, GraphicsUnit.Pixel);
                graph.Dispose();
            }

            /* ne fonctionne pas visiblement
            if (angleRotation > 0)
            {
                Graphics graph = Graphics.FromImage(imageFinale);
                graph.RotateTransform(angleRotation);
            }
            */
            imageFinale.Save(nomFichierFinal, ImageFormat.Png);
            //imageFinale.Dispose();
            return true;
        }

        /// <summary>
        /// Produit une image entourée d'une autre image (dite Grise)
        /// </summary>
        /// <param name="modele">image source utilisée</param>
        /// <param name="nomFichierFinal">nom du fichier à produire</param>
        /// <param name="rect">zone centrale</param>
        /// <param name="rectGris">zone globale (comprenant le gris)</param>
        /// <returns>true si OK, false si KO</returns>
        internal static bool FichierGrise(Constantes.MODELESCARTE modele, string nomFichierFinal, Rectangle rect, Rectangle rectGris)
        {
            Bitmap imageSource = GetImage(modele);
            Bitmap imageFinale;
            Graphics graph;

            if (null == imageSource)
            {
                LogFile.Notifier(string.Format("Erreur dans FichierGrise, impossible de trouver l'image pour le modele {0}", modele.ToString()));
                return false;
            }
            RecadrerRect(imageSource, ref rect);
            //if (null == m_imageCarteGris)
            //{
                RecadrerRect(m_imageCarteGris, ref rectGris);
                imageFinale = new Bitmap(rectGris.Width, rectGris.Height, m_imageCarteGris.PixelFormat);
                graph = Graphics.FromImage(imageFinale);
                //première copie
                graph.DrawImage(m_imageCarteGris, 0, 0, rectGris, GraphicsUnit.Pixel);
                for (int x = 0; x < rectGris.Width; x++ )
                {
                    for (int y = 0; y < rectGris.Height; y++)
                    {
                        Color couleur = imageFinale.GetPixel(x,y);
                        Double facteurgris= 0.7;
                        Color couleurFinale = Color.FromArgb(
                                                        Math.Min(Math.Max((int)(couleur.R*facteurgris),0),255), 
                                                        Math.Min(Math.Max((int)(couleur.G*facteurgris),0),255), 
                                                        Math.Min(Math.Max((int)(couleur.B*facteurgris),0),255)
                                                            );
                        imageFinale.SetPixel(x, y, couleurFinale);
                    }
                }
                //recopie des pixels non grisés
                graph.DrawImage(imageSource, rect.X - rectGris.X, rect.Y - rectGris.Y, rect, GraphicsUnit.Pixel);
            /*
            }
            else
            {
                LogFile.Notifier(string.Format("FichierGrise rectGris Left={0}, Top={1}, Width={2}, Height={3}", rectGris.Left, rectGris.Top, rectGris.Width, rectGris.Height));
                RecadrerRect(m_imageCarteGris, ref rectGris);

                //Si les images n'ont pas la même résolution, DrawImage refait une mise à l'échelle automatique !
                imageFinale = new Bitmap(rectGris.Width, rectGris.Height, m_imageCarteGris.PixelFormat);
                if (Math.Abs(imageSource.HorizontalResolution - m_imageCarteGris.HorizontalResolution) > 1 ||
                    Math.Abs(m_imageCarteGris.HorizontalResolution - imageFinale.HorizontalResolution) > 1 ||
                    Math.Abs(imageSource.VerticalResolution - m_imageCarteGris.VerticalResolution) > 1 ||
                    Math.Abs(m_imageCarteGris.VerticalResolution - imageFinale.VerticalResolution) > 1)
                {
                    LogFile.Notifier(string.Format("Erreur dans FichierGrise, {0} et {1} DOIVENT avoir une résolution horizontale de {2} dpi, et verticale de {3} dpi au lieu de {4}, {5} dpi et {6},{7} dpi respectivement",
                        modele.ToString(),
                        Donnees.m_donnees.TAB_JEU[0].S_NOM_CARTE_GRIS,
                        imageFinale.HorizontalResolution,
                        imageFinale.VerticalResolution,
                        imageSource.HorizontalResolution,
                        imageSource.VerticalResolution,
                        m_imageCarteGris.HorizontalResolution,
                        m_imageCarteGris.VerticalResolution));
                    return false;
                }

                graph = Graphics.FromImage(imageFinale);
                graph.DrawImage(m_imageCarteGris, 0, 0, rectGris, GraphicsUnit.Pixel);
                graph.DrawImage(imageSource, rect.X - rectGris.X, rect.Y - rectGris.Y, rect, GraphicsUnit.Pixel);
            }
            */
            //sauvegarde
            imageFinale.Save(nomFichierFinal, ImageFormat.Png);

            return true;
        }

        /// <summary>
        /// Construction de la carte en fin de tour pour permettre la génération pour le web et l'affichage ecran
        /// </summary>
        /// <returns>true si ok, false si ko</returns>
        internal static bool ConstructionCarte()
        {
            string message, messageErreur = string.Empty;
            Debug.WriteLine("ConstructionCarteFinale");

            //unité statiques
            if (!PlacerLesUnitesStatiques()) { return false; }

            //unités avec effectif
            int i = 0;
            while (i<Donnees.m_donnees.TAB_PION.Count)//pas de foreach, on peut créer un pion messager en cas de capture
            {
                Donnees.TAB_PIONRow lignePion = Donnees.m_donnees.TAB_PION[i];
                //if (lignePion.B_DETRUIT || lignePion.estEngageeAuCombat()) { continue; }
                if (lignePion.B_DETRUIT || lignePion.estAuCombat) { ++i;  continue; }
                if (lignePion.effectifTotal > 0)
                {
                    Donnees.TAB_ORDRERow ligneOrdre = Donnees.m_donnees.TAB_ORDRE.Mouvement(lignePion.ID_PION);
                    //if (null != ligneOrdre  && lignePion.OrdreActif(ligneOrdre))
                    if (null != ligneOrdre)
                    {
                        //il y a un ordre de mouvement
                        //recherche les modèles de l'unité
                        Donnees.TAB_NATIONRow ligneNation = lignePion.nation;// Donnees.m_donnees.TAB_PION.TrouveNation(lignePion);
                        if (!lignePion.PlacerPionEnRoute(ligneOrdre, ligneNation))
                        {
                            message = "Erreur durant le traitement PlacerPionEnRoute";
                            LogFile.Notifier(message, out messageErreur);
                            return false;
                        }
                    }
                }
                ++i;
            }

            //unités sans effectif
            i = 0;
            while (i<Donnees.m_donnees.TAB_PION.Count)//pas de foreach, on peut créer un pion messager en cas de capture
            {
                Donnees.TAB_PIONRow lignePion = Donnees.m_donnees.TAB_PION[i];
                //if (lignePion.B_DETRUIT || lignePion.estEngageeAuCombat()) { continue; }
                if (lignePion.B_DETRUIT || lignePion.estAuCombat) { ++i; continue; }
                if (lignePion.effectifTotal == 0)
                {
                    Donnees.TAB_CASERow ligneCase = Donnees.m_donnees.TAB_CASE.FindParID_CASE(lignePion.ID_CASE);
                    int nbplacesOccupes = 0;
                    if (!lignePion.RequisitionCase(ligneCase, false, ref nbplacesOccupes)) { return false; }
                }
                ++i;
            }
            return true;
        }

        internal static bool PlacerLesUnitesStatiquesParallele()
        {
            //string message;
            //int i = 0;
            try
            {
                List<Donnees.TAB_PIONRow> liste = new List<Donnees.TAB_PIONRow>();
                foreach (Donnees.TAB_PIONRow lignePion in Donnees.m_donnees.TAB_PION)
                {
                    if (!lignePion.B_DETRUIT)
                    {
                        liste.Add(lignePion);
                    }
                }
                //Parallel.ForEach(liste, item => item.PlacerStatique());
                ParallelLoopResult result = Parallel.ForEach(liste, (item, loopstate) =>
                {
                    if (!item.PlacerStatique()) loopstate.Break();
                });
                return result.IsCompleted;
            }
            catch (AggregateException ae)
            {
                string messageEX = string.Empty;
                foreach (var exAE in ae.InnerExceptions)
                {
                    messageEX += string.Format("exceptionAE PlacerLesUnitesStatiquesParallele {3} : {0} : {1} :{2}",
                           exAE.Message, (null == exAE.InnerException) ? "sans inner exception" : exAE.InnerException.Message,
                           exAE.StackTrace, exAE.GetType().ToString());
                }
                MessageBox.Show(messageEX);
                return false;
            }
            catch (Exception ex)
            {
                string message = string.Format("exception PlacerLesUnitesStatiquesParallele {3} : {0} : {1} :{2}", 
                       ex.Message, (null == ex.InnerException) ? "sans inner exception" : ex.InnerException.Message, 
                       ex.StackTrace, ex.GetType().ToString());
                MessageBox.Show(message);
                return false;
            }
        }

        internal static bool PlacerLesUnitesStatiques()
        {
            return PlacerLesUnitesStatiquesParallele();
            /*
            string message;
            int i = 0;
            while (i<Donnees.m_donnees.TAB_PION.Count)
            {
                Donnees.TAB_PIONRow lignePion = Donnees.m_donnees.TAB_PION[i];
                if (!lignePion.PlacerStatique())
                {
                    message = "Erreur durant le traitement Cartographie::PlacerLesUnitesStatiques";
                    LogFile.Notifier(message);
                    return false;
                }
                ++i;
            }
            return true;
            */
        }

        /// <summary>
        /// Création d'une nouvelle bataille si besoin
        /// </summary>
        /// <param name="ligneCaseBataille">Case sur laquelle est centrée la bataille</param>
        /// <param name="lignePion">pion déclenchant la bataille</param>
        /// <returns>true si ok, false si ko</returns>
        internal static bool NouvelleBataille(Donnees.TAB_CASERow ligneCaseBataille, Donnees.TAB_PIONRow lignePion)
        {
            string message, messageErreur;
            bool nouvelleBataille;
            Donnees.TAB_PIONRow lignePionEnnemi;

            try
            { 
                message = string.Format("NouvelleBataille : en idCASE={0}", ligneCaseBataille.ID_CASE);
                LogFile.Notifier(message, out messageErreur);

                int heureCourante = Donnees.m_donnees.TAB_PARTIE.HeureCourante();
                if (Donnees.m_donnees.TAB_PARTIE.NocturneOuBatailleImpossible())
                {
                    message = string.Format("NouvelleBataille : pas de nouvelle bataille la nuit ou avant que la nuit tombe pour IDPion={0} sur la idcase={1} ", lignePion.ID_PION, ligneCaseBataille.ID_CASE);
                    //oui, cela peut permettre à des troupes de s'échapper en en traversant d'autres... -> donc, finallement, on arrête le mouvement et on envoie un message, géré dans Rencontre
                    LogFile.Notifier(message, out messageErreur);
                    return true;
                }

                lignePionEnnemi = Donnees.m_donnees.TAB_PION.FindByID_PION(ligneCaseBataille.ID_NOUVEAU_PROPRIETAIRE);

                if (null == lignePionEnnemi)
                {
                    lignePionEnnemi = Donnees.m_donnees.TAB_PION.FindByID_PION(ligneCaseBataille.ID_PROPRIETAIRE);
                    if (null == lignePionEnnemi)
                    {
                        message = string.Format("NouvelleBataille : impossible de trouver le pion ennemi id={0}", ligneCaseBataille.ID_PROPRIETAIRE);
                        LogFile.Notifier(message, out messageErreur);
                        return false;
                    }
                }

                //on vérifie que l'ennemi est bien vivant et est une unité de combat
                if (!lignePionEnnemi.estCombattifQG(false, true))
                {
                    message = string.Format("NouvelleBataille : l'ennemi ID={0} : {1} est soit détruit soit sans effectif, pas de bataille", lignePionEnnemi.ID_PION, lignePionEnnemi.S_NOM);
                    LogFile.Notifier(message, out messageErreur);
                    return true;
                }

                // on vérifie si les deux unités sont déjà engageables dans le même combat non terminé
                Monitor.Enter(Donnees.m_donnees.TAB_BATAILLE.Rows.SyncRoot);
                Monitor.Enter(Donnees.m_donnees.TAB_BATAILLE_PIONS.Rows.SyncRoot);
                var result = from BataillePionUn in Donnees.m_donnees.TAB_BATAILLE_PIONS
                             from BataillePionDeux in Donnees.m_donnees.TAB_BATAILLE_PIONS
                             from Bataille in Donnees.m_donnees.TAB_BATAILLE
                             where Bataille.IsI_TOUR_FINNull()
                                && (BataillePionUn.ID_PION == lignePion.ID_PION)
                                && (BataillePionDeux.ID_PION == lignePionEnnemi.ID_PION)
                                && BataillePionUn.ID_BATAILLE == BataillePionDeux.ID_BATAILLE
                                && Bataille.ID_BATAILLE == BataillePionDeux.ID_BATAILLE
                             select Bataille.ID_BATAILLE;
                /*
                var result = from BataillePionUn in Donnees.m_donnees.TAB_BATAILLE_PIONS
                             join BataillePionDeux in Donnees.m_donnees.TAB_BATAILLE_PIONS
                             on BataillePionUn.ID_BATAILLE equals BataillePionDeux.ID_BATAILLE
                             join Bataille in Donnees.m_donnees.TAB_BATAILLE
                             on (BataillePionDeux.ID_BATAILLE == Bataille.ID_BATAILLE) && (BataillePionUn.ID_BATAILLE equals Bataille.ID_BATAILLE) 
                             where (BataillePionUn.ID_PION == lignePion.ID_PION) && (BataillePionDeux.ID_PION == lignePionEnnemi.ID_PION)
                             select BataillePionUn.ID_BATAILLE;
                */
                if (result.Count() > 0)
                {
                    var idBataille = result.ElementAt(0);
                    message = string.Format("NouvelleBataille : les deux unités id={0} et id={1} sont déjà engagées dans la bataille {2}",
                    lignePion.ID_PION, lignePionEnnemi.ID_PION, idBataille.ToString());
                    LogFile.Notifier(message, out messageErreur);
                    Monitor.Exit(Donnees.m_donnees.TAB_BATAILLE_PIONS.Rows.SyncRoot);
                    Monitor.Exit(Donnees.m_donnees.TAB_BATAILLE.Rows.SyncRoot);
                    return true;//pas vraiment une erreur, juste curieux, tout à fait possible en execution parallele, les unités pouvant créer la bataille en même temps
                }

                //la case ou l'une des unités fait-elle déjà partie d'une ou plusieurs bataille ?
                nouvelleBataille = true;
                foreach (Donnees.TAB_BATAILLERow ligneBataille in Donnees.m_donnees.TAB_BATAILLE)
                {
                    if (!ligneBataille.IsI_TOUR_FINNull()) { continue; }
                    if (ligneBataille.I_X_CASE_BAS_DROITE >= ligneCaseBataille.I_X && ligneBataille.I_Y_CASE_BAS_DROITE >= ligneCaseBataille.I_Y
                        && ligneBataille.I_X_CASE_HAUT_GAUCHE <= ligneCaseBataille.I_X && ligneBataille.I_Y_CASE_HAUT_GAUCHE <= ligneCaseBataille.I_Y)
                    {
                        //on ajoute les unités à la bataille courante
                        ligneBataille.AjouterPionDansLaBataille(lignePion, ligneCaseBataille);
                        ligneBataille.AjouterPionDansLaBataille(lignePionEnnemi, ligneCaseBataille);
                        nouvelleBataille = false;
                    }
                    else
                    {
                        int IdBataille = lignePion.ID_BATAILLE;
                        if ((Constantes.NULLENTIER != IdBataille) && IdBataille == ligneBataille.ID_BATAILLE)
                        {
                            if (lignePionEnnemi.IsID_BATAILLENull())
                            {
                                ligneBataille.AjouterPionDansLaBataille(lignePionEnnemi, ligneCaseBataille);
                                nouvelleBataille = false;
                            }
                        }
                        IdBataille = lignePionEnnemi.ID_BATAILLE;
                        if ((Constantes.NULLENTIER != IdBataille) && IdBataille == ligneBataille.ID_BATAILLE)
                        {
                            if (lignePion.IsID_BATAILLENull())
                            {
                                ligneBataille.AjouterPionDansLaBataille(lignePion, ligneCaseBataille);
                                nouvelleBataille = false;
                            }
                        }
                    }
                }

                //si les unités ne sont dans aucune bataille, il faut la créer
                if (nouvelleBataille)
                {
                    Donnees.TAB_BATAILLERow ligneNouvelleBataille = CreationBataille(ligneCaseBataille, lignePion, lignePionEnnemi);
                    //on ajoute systématiquement le pion ayant crée la bataille
                    ligneNouvelleBataille.AjouterPionDansLaBataille(lignePion, ligneCaseBataille, true);
                }

                Monitor.Exit(Donnees.m_donnees.TAB_BATAILLE_PIONS.Rows.SyncRoot);
                Monitor.Exit(Donnees.m_donnees.TAB_BATAILLE.Rows.SyncRoot);
                return true;
            }
            catch (Exception ex)
            {
                string messageEX = string.Format("exception NouvelleBataille {3} : {0} : {1} :{2}",
                       ex.Message, (null == ex.InnerException) ? "sans inner exception" : ex.InnerException.Message,
                       ex.StackTrace, ex.GetType().ToString());
                MessageBox.Show(messageEX);
                return false;
            }
        }

        /// <summary>
        /// Création d'un nouveau lieu de bataille
        /// </summary>
        /// <param name="ligneCaseBataille">case centrale d'où part le combat</param>
        /// <param name="lignePionOrigineBataille">Pion à l'origine de la bataille</param>
        /// <returns>nouvelle bataille si OK, null si KO</returns>
        internal static Donnees.TAB_BATAILLERow CreationBataille(Donnees.TAB_CASERow ligneCaseBataille, Donnees.TAB_PIONRow lignePionOrigineBataille, Donnees.TAB_PIONRow lignePionOrigineBataille2)
        {
            string nomBataille;
            char orientation;
            int[] idModeleTerrain = new int[6];
            int idLeader012 = -1;
            int idLeader345 = -1;
            int xCaseHautGauche;
            int yCaseHautGauche;
            int xCaseBasDroite;
            int yCaseBasDroite;
            char niveauHierarchique;
            string message, messageErreur, requete;
            Donnees.TAB_CASERow ligneCasePion;
            Donnees.TAB_BATAILLERow ligneBataille;
            int idNation0 = -1, idNation1 = -1, izone;
            int idNation012 = -1, idNation345 = -1;
            char niveauHierarchique012 = 'Z', niveauHierarchique345 = 'Z';
            int[] zone0_H = new int[6];
            int[] zone1_H = new int[6];
            int[] zone0_V = new int[6];
            int[] zone1_V = new int[6];
            int i, j, k, nbZoneConflitO, nbZoneConflitV;
            SortedList[] listeModeles = new SortedList[6];
            SortedList[] listeModelesBase = new SortedList[6];
            int[] idObstacle = new int[3];
            Donnees.TAB_PIONRow lignePion;

            //nom de la bataille
            if (!ClassMessager.NomDeBataille(ligneCaseBataille, out nomBataille))
            {
                message = string.Format("CreationBataille : erreur ClassMessager.NomDeBataille renvoie FAUX");
                LogFile.Notifier(message, out messageErreur);
                return null;
            }

            message = string.Format("CreationBataille : bataille de {0}", nomBataille);
            LogFile.Notifier(message, out messageErreur);

            RectangleChampDeBataille(ligneCaseBataille, out xCaseHautGauche, out yCaseHautGauche, out xCaseBasDroite, out yCaseBasDroite);

            #region recherche de l'orientation
            Donnees.TAB_CASERow[] lignesCaseBataille = Donnees.m_donnees.TAB_CASE.CasesCadre(xCaseHautGauche, yCaseHautGauche, xCaseBasDroite, yCaseBasDroite);
            // recherche sur l'horizontale
            // | 0 | 1 | 2 |
            // | 3 | 4 | 5 |
            zone0_H.Initialize();
            zone1_H.Initialize();
            nbZoneConflitO = nbZoneConflitV = 0;
            foreach (Donnees.TAB_CASERow ligneCaseZone in lignesCaseBataille)
            {
                //si la case n'appartient à personne, ce n'est pas un critère
                if (!ligneCaseZone.EstInnocupe()) //.ID_PROPRIETAIRE. != DataSetCoutDonnees.CST_AUCUNPROPRIETAIRE)
                {
                    lignePion = ligneCaseZone.TrouvePionSurCase();
                    if (null == lignePion)
                    {
                        int IdProprietaire = ligneCaseZone.ID_PROPRIETAIRE;
                        string proprio = (Constantes.NULLENTIER == IdProprietaire) ? "null" : IdProprietaire.ToString();
                        int IdNouveauProprietaire = ligneCaseZone.ID_NOUVEAU_PROPRIETAIRE;
                        string nouveauProprio = (Constantes.NULLENTIER == IdNouveauProprietaire) ? "null" : IdNouveauProprietaire.ToString();
                        message = string.Format("CreationBataille : erreur FindByID_PION introuvable sur {0} ou {1}", proprio, nouveauProprio);
                        LogFile.Notifier(message, out messageErreur);
                        return null;
                    }

                    Donnees.TAB_MODELE_PIONRow ligneModelePion = lignePion.modelePion;
                    if (null == ligneModelePion)
                    {
                        message = string.Format("CreationBataille : erreur FindByID_MODELE_PION introuvable sur {0}", lignePion.ID_MODELE_PION);
                        LogFile.Notifier(message, out messageErreur);
                        return null;
                    }

                    //recherche de la zone concernée
                    izone = RechercheZone(Constantes.CST_BATAILLE_HORIZONTALE,
                        ligneCaseZone.I_X, ligneCaseZone.I_Y,
                        xCaseHautGauche, yCaseHautGauche, xCaseBasDroite, yCaseBasDroite);

                    //un point de plus sur la zone pour son camp !
                    if (idNation0 == -1 || idNation0 == ligneModelePion.ID_NATION)
                    {
                        idNation0 = ligneModelePion.ID_NATION;
                        zone0_H[izone]++;
                    }
                    else
                    {
                        idNation1 = ligneModelePion.ID_NATION;
                        zone1_H[izone]++;
                    }
                }
            }
            //bilan sur le nombre de zones en conflit
            for (i = 0; i < 6; i++)
            {
                if ((zone0_H[i] < zone1_H[i] * 2) && (zone0_H[i] * 2 > zone1_H[i]))
                {
                    nbZoneConflitO++;
                }
            }

            // recherche sur la verticale
            // | 3 | 0 |
            // | 4 | 1 |
            // | 5 | 2 |
            zone0_V.Initialize();
            zone1_V.Initialize();
            foreach (Donnees.TAB_CASERow ligneCaseZone in lignesCaseBataille)
            {
                //si la case n'appartient à personne, ce n'est pas un critère
                if (!ligneCaseZone.EstInnocupe()) //.ID_PROPRIETAIRE != DataSetCoutDonnees.CST_AUCUNPROPRIETAIRE)
                {
                    lignePion = ligneCaseZone.TrouvePionSurCase();
                    if (null == lignePion)
                    {
                        int IdProprietaire = ligneCaseZone.ID_PROPRIETAIRE;
                        string proprio = (Constantes.NULLENTIER == IdProprietaire) ? "null" : IdProprietaire.ToString();
                        int IdNouveauProprietaire = ligneCaseZone.ID_NOUVEAU_PROPRIETAIRE;
                        string nouveauProprio = (Constantes.NULLENTIER == IdNouveauProprietaire) ? "null" : IdNouveauProprietaire.ToString();
                        
                        message = string.Format("CreationBataille : erreur FindByID_PION introuvable sur {0} ou {1}", proprio, nouveauProprio);
                        LogFile.Notifier(message, out messageErreur);
                        return null;
                    }
                    Donnees.TAB_MODELE_PIONRow ligneModelePion = lignePion.modelePion;
                    if (null == ligneModelePion)
                    {
                        message = string.Format("CreationBataille : erreur FindByID_MODELE_PION introuvable sur {0}", lignePion.ID_MODELE_PION);
                        LogFile.Notifier(message, out messageErreur);
                        return null;
                    }

                    //recherche de la zone concernée
                    izone = RechercheZone(Constantes.CST_BATAILLE_VERTICALE,
                        ligneCaseZone.I_X, ligneCaseZone.I_Y,
                        xCaseHautGauche, yCaseHautGauche, xCaseBasDroite, yCaseBasDroite);

                    //un point de plus sur la zone pour son camp !
                    if (idNation0 == -1 || idNation0 == ligneModelePion.ID_NATION)
                    {
                        idNation0 = ligneModelePion.ID_NATION;
                        zone0_V[izone]++;
                    }
                    else
                    {
                        idNation1 = ligneModelePion.ID_NATION;
                        zone1_V[izone]++;
                    }
                }
            }
            //bilan sur le nombre de zones en conflit
            for (i = 0; i < 6; i++)
            {
                if ((zone0_V[i] < zone1_V[i] * 2) && (zone0_V[i] * 2 > zone1_V[i]))
                {
                    nbZoneConflitV++;
                }
            }

            //choix de l'orientation quelle nation se trouve sur quelle case ?
            int coherenceH = 0, coherenceV = 0;
            if (nbZoneConflitO == nbZoneConflitV)
            {
                    //pas de choix préalable, on détermine l'orientation par la cohérence dans les zones, moins il y a de zones occupées
                    //plus c'est cohérente et donc meilleur
                    for (i = 0; i < 6; i++)
                    {
                        if (zone0_V[i] > 0) { coherenceV++; }
                        if (zone1_V[i] > 0) { coherenceV++; }
                        if (zone0_H[i] > 0) { coherenceH++; }
                        if (zone1_H[i] > 0) { coherenceH++; }
                    }
            }

            if (nbZoneConflitO > nbZoneConflitV || coherenceV < coherenceH)
            {
                orientation = Constantes.CST_BATAILLE_VERTICALE;
                if (zone0_V[0] + zone0_V[1] + zone0_V[2] > zone0_V[3] + zone0_V[4] + zone0_V[5])
                {
                    idNation012 = idNation0;
                    idNation345 = idNation1;
                }
                else
                {
                    idNation012 = idNation1;
                    idNation345 = idNation0;
                }
            }
            else
            {
                orientation = Constantes.CST_BATAILLE_HORIZONTALE;
                if (zone0_H[0] + zone0_H[1] + zone0_H[2] > zone0_H[3] + zone0_H[4] + zone0_H[5])
                {
                    idNation012 = idNation0;
                    idNation345 = idNation1;
                }
                else
                {
                    idNation012 = idNation1;
                    idNation345 = idNation0;
                }
            }
            //Il peut arriver des cas sur des unités petites ou une nation n'a pas pu être trouvée, il faut alors la forcer
            if (idNation012 < 0) { idNation012 = (0 == idNation345) ? 1 : 0; }
            if (idNation345 < 0) { idNation345 = (0 == idNation012) ? 1 : 0; }
            #endregion

            #region recherche des leaders de chaque bataille
            // je le fais après la création pour utiliser la méthode générique
            foreach (Donnees.TAB_PIONRow lignePionBataille in Donnees.m_donnees.TAB_PION)
            {
                if (lignePionBataille.B_DETRUIT) { continue; }
                //unité QG et non présente dans un autre combat
                if (lignePionBataille.estQGHierarchique(out niveauHierarchique) && !lignePionBataille.estAuCombat && lignePionBataille.estJoueur)
                {
                    ligneCasePion = lignePionBataille.CaseCourante();
                    if (ligneCasePion.I_X <= xCaseBasDroite && ligneCasePion.I_Y <= yCaseBasDroite &&
                        ligneCasePion.I_X >= xCaseHautGauche && ligneCasePion.I_Y >= yCaseHautGauche)
                    {
                        //QG dans le combat, la bataille n'étant pas crée, pour l'instant on cherche le commandant en chef
                        Donnees.TAB_MODELE_PIONRow ligneModelePion = lignePionBataille.modelePion;
                        if (null == ligneModelePion)
                        {
                            message = string.Format("CreationBataille : erreur FindByID_MODELE_PION recherche QG introuvable sur {0}", lignePionBataille.ID_MODELE_PION);
                            LogFile.Notifier(message, out messageErreur);
                            return null;
                        }
                        if (ligneModelePion.ID_NATION == idNation012)
                        {
                            if (lignePionBataille.C_NIVEAU_HIERARCHIQUE < niveauHierarchique012)
                            {
                                niveauHierarchique012 = lignePionBataille.C_NIVEAU_HIERARCHIQUE;
                                idLeader012 = lignePionBataille.ID_PION;
                            }
                        }
                        else
                        {
                            if (lignePionBataille.C_NIVEAU_HIERARCHIQUE < niveauHierarchique345)
                            {
                                niveauHierarchique345 = lignePionBataille.C_NIVEAU_HIERARCHIQUE;
                                idLeader345 = lignePionBataille.ID_PION;
                            }
                        }
                    }
                }
            }
            #endregion

            #region recherche des modèles de terrain sur chaque case
            foreach (Donnees.TAB_CASERow ligneCaseZone in lignesCaseBataille)
            {
                int valeurTerrain = 0;

                Donnees.TAB_MODELE_TERRAINRow ligneTerrain = Donnees.m_donnees.TAB_MODELE_TERRAIN.FindByID_MODELE_TERRAIN(ligneCaseZone.ID_MODELE_TERRAIN);
                if (ligneTerrain.B_ANNULEE_EN_COMBAT)
                {
                    continue;// les cases qui ne représentent pas le terrain réel (route par exemple) ne sont pas comptabilisées
                }
                izone = RechercheZone(orientation,
                    ligneCaseZone.I_X, ligneCaseZone.I_Y,
                    xCaseHautGauche, yCaseHautGauche, xCaseBasDroite, yCaseBasDroite);

                if (null == listeModeles[izone])
                {
                    listeModeles[izone] = new SortedList();
                    listeModelesBase[izone] = new SortedList();
                }
                if (!ligneCaseZone.EstInnocupe() && null!=ligneCaseZone.TrouvePionSurCase())
                {
                    valeurTerrain = Constantes.CST_PONDERATION_CASE_COMBAT;
                }
                else
                {
                    valeurTerrain = 1;
                }

                if (listeModeles[izone].ContainsKey(ligneCaseZone.ID_MODELE_TERRAIN))
                {
                    //modèle de terrain présent une fois de plus
                    listeModeles[izone][ligneCaseZone.ID_MODELE_TERRAIN] = (int)listeModeles[izone][ligneCaseZone.ID_MODELE_TERRAIN] + valeurTerrain;
                    listeModelesBase[izone][ligneCaseZone.ID_MODELE_TERRAIN] = (int)listeModelesBase[izone][ligneCaseZone.ID_MODELE_TERRAIN] + 1;
                }
                else
                {
                    //nouveau modèle de terrain
                    listeModeles[izone].Add(ligneCaseZone.ID_MODELE_TERRAIN, valeurTerrain);
                    listeModelesBase[izone].Add(ligneCaseZone.ID_MODELE_TERRAIN, 1);
                }
            }

            //Recherche du modèle de terrain majoritaire (en valeur) et qui soit une case traversable
            for (i = 0; i < 6; i++)
            {
                //si c'est bien trié, le dernier index est le plus élevé -> en fait non, c'est trié par clef, pas par valeur
                int idTerrain = Convert.ToInt32(listeModeles[i].GetKey(0));
                int valeurMajoritaire = 0;// Convert.ToInt32(listeModeles[i][idTerrain]);
                for (j = 0; j < listeModeles[i].Count; j++)
                {
                    int id = Convert.ToInt32(listeModeles[i].GetKey(j));
                    int val = Convert.ToInt32(listeModeles[i][id]);
                    Donnees.TAB_MODELE_TERRAINRow ligneTerrainMajoritaire = Donnees.m_donnees.TAB_MODELE_TERRAIN.FindByID_MODELE_TERRAIN(id);
                    requete = string.Format("ID_MODELE_MOUVEMENT={0} AND ID_METEO={1} AND ID_MODELE_TERRAIN={2}",
                        0,//ligneModelePion.ID_MODELE_MOUVEMENT,
                        Donnees.m_donnees.TAB_PARTIE[0].ID_METEO,
                        ligneTerrainMajoritaire.ID_MODELE_TERRAIN);
                    Donnees.TAB_MOUVEMENT_COUTRow[] resCout = (Donnees.TAB_MOUVEMENT_COUTRow[])Donnees.m_donnees.TAB_MOUVEMENT_COUT.Select(requete);                    
                    if (val > valeurMajoritaire && resCout[0].I_COUT>0)
                    {
                        idTerrain = id;
                        valeurMajoritaire = val;
                    }
                }
                idModeleTerrain[i] = idTerrain;
            }

            //on regarde si l'un des terrains est un terrain sur lequel on a une seule zone de combat au lieu de trois (cas des forteresses par exemple)
            bool bZoneUnique = false;
            for (i = 0; i < 6; i++)
            {
                Donnees.TAB_MODELE_TERRAINRow ligneModeleTerrain= Donnees.m_donnees.TAB_MODELE_TERRAIN.FindByID_MODELE_TERRAIN(idModeleTerrain[i]);
                if (ligneModeleTerrain.B_BATAILLE_ZONE_UNIQUE)
                {
                    bZoneUnique = true;
                    //dans ce cas le terrain central (le seul joué) est forcement ce terrain unique
                    if (i<3)
                    {
                        idModeleTerrain[1] = idModeleTerrain[i];
                    }
                    else
                    {
                        idModeleTerrain[4] = idModeleTerrain[i];
                    }
                }
            }

            //recherche d'éventuels obstacles (rivière, etc.) entre les zones, d'après le nombre d'éléments présents
            for (i = 0; i < 3; i++)
            {
                //si la somme des obstacles dans une zone est supérieur à 10% aux autres, alors on prend l'obstacle majoritaire
                int nbCases = 0;
                int nbObstacles = 0;
                int idObstacleMajoritaire=-1;
                int valeurMajoritaire=0;
                int idTerrain;
                int nbTerrain;
                bool bObstacle = false;
                idObstacle[i] = -1;

                for (k = 0; k < 2; k++)
                {
                    nbTerrain = 0;
                    nbCases = 0;
                    for (j = 0; j < listeModelesBase[i+k*3].Count; j++)
                    {
                        idTerrain = Convert.ToInt32(listeModelesBase[i + k * 3].GetKey(j));
                        nbTerrain = Convert.ToInt32(listeModelesBase[i + k * 3][idTerrain]);
                        Donnees.TAB_MODELE_TERRAINRow ligneTerrain = Donnees.m_donnees.TAB_MODELE_TERRAIN.FindByID_MODELE_TERRAIN(idTerrain);
                        if (ligneTerrain.B_OBSTACLE_DEFENSIF)
                        {
                            if (nbTerrain > valeurMajoritaire)
                            {
                                idObstacleMajoritaire = idTerrain;
                                valeurMajoritaire = nbTerrain;
                            }
                            nbObstacles += nbTerrain;
                        }
                        nbCases += nbTerrain;
                    }
                    if (nbObstacles * 10 > nbCases)
                    {
                        bObstacle = true;//obstacle présent sur plus de 10%
                    }
                }
                if (bObstacle) { idObstacle[i] = idObstacleMajoritaire; }
            }
            #endregion

            Monitor.Enter(Donnees.m_donnees.TAB_BATAILLE.Rows.SyncRoot); 
            ligneBataille = Donnees.m_donnees.TAB_BATAILLE.AddTAB_BATAILLERow(
                                    Donnees.m_donnees.TAB_BATAILLE.ProchainID_BATAILLE,
                                    nomBataille,
                                    Donnees.m_donnees.TAB_PARTIE[0].I_TOUR,
                                    Donnees.m_donnees.TAB_PARTIE[0].I_PHASE,
                                    Constantes.CST_IDNULL,
                                    Constantes.CST_IDNULL,
                                    orientation,
                                    idModeleTerrain[0],
                                    idModeleTerrain[1],
                                    idModeleTerrain[2],
                                    idModeleTerrain[3],
                                    idModeleTerrain[4],
                                    idModeleTerrain[5],
                                    idObstacle[0],
                                    idObstacle[1],
                                    idObstacle[2],
                                    idNation012,
                                    idNation345,
                                    idLeader012,
                                    idLeader345,
                                    xCaseHautGauche,
                                    yCaseHautGauche,
                                    xCaseBasDroite,
                                    yCaseBasDroite,
                                    1,1,1,1,1,1, //Le niveaux d'engagement (en dés) est minimum par défaut
                                    "Aucun Combat",
                                    "Aucun Combat",
                                    "Aucun Combat",
                                    "Aucun Combat",
                                    "Aucun Combat",
                                    "Aucun Combat",
                                    0,//I_PERTES
                                    0,
                                    0,
                                    0,
                                    0,
                                    0,
                                    bZoneUnique,//B_ZONE_UNIQUE
                                    ""//S_FIN
                                    );
            if (null == ligneBataille)
            {
                message = string.Format("CreationBataille : impossible d'ajouter la bataille sur la case {0}", ligneCaseBataille.ID_CASE);
                LogFile.Notifier(message, out messageErreur);
                return null;
            }
            ligneBataille.SetI_TOUR_FINNull();
            ligneBataille.SetI_PHASE_FINNull();
            Monitor.Exit(Donnees.m_donnees.TAB_BATAILLE.Rows.SyncRoot); 

            //ajout des pions et leaders présents dans la bataille
            //foreach (DataSetCoutDonnees.TAB_PIONRow lignePion in DataSetCoutDonnees.m_donnees.TAB_PION) -> pas possible car on ajouter des pions en cas d'envoie de message dans AjouterPionDansLaBataille
            foreach (Donnees.TAB_CASERow ligneCaseZone in lignesCaseBataille)
            {
                Donnees.TAB_PIONRow lignePionEnBataille = ligneCaseZone.TrouvePionSurCase();
                if (null != lignePionEnBataille)
                {
                    //s'il s'agit d'une des deux unités ayant déclenché la bataille et que celle-ci est en mouvement, l'unité doit être déplace au centre du combat.
                    Donnees.TAB_CASERow ligneCasePlacement =
                        ((lignePionOrigineBataille.ID_PION == lignePionEnBataille.ID_PION && lignePionOrigineBataille.enMouvement)
                        || (lignePionOrigineBataille2.ID_PION == lignePionEnBataille.ID_PION && lignePionOrigineBataille2.enMouvement)) ? ligneCaseBataille : ligneCaseZone;

                    if (!ligneBataille.AjouterPionDansLaBataille(lignePionEnBataille,
                        ligneCasePlacement, 
                        ligneCaseBataille == ligneCaseZone))
                    {
                        return null;
                    }
                }
            }

            //il est possible qu'une unité ne soit pas visible car n'ayant trouvée aucune case pour se placer, on ajoute donc toute unité dont l'emplacement est dans la zone de bataille
            i = 0;
            while (i < Donnees.m_donnees.TAB_PION.Count)
            {
                Donnees.TAB_PIONRow lignePionEnBataille = Donnees.m_donnees.TAB_PION[i++];
                if (lignePionEnBataille.B_DETRUIT) { continue; }
                Donnees.TAB_CASERow ligneCasePionBataille = Donnees.m_donnees.TAB_CASE.FindParID_CASE(lignePionEnBataille.ID_CASE);
                if (ligneCasePionBataille.I_X >= xCaseHautGauche && ligneCasePionBataille.I_Y >= yCaseHautGauche && ligneCasePionBataille.I_X <= xCaseBasDroite && ligneCasePionBataille.I_Y <= yCaseBasDroite)
                {
                    if (!ligneBataille.AjouterPionDansLaBataille(lignePionEnBataille, ligneCasePionBataille, false))
                    {
                        return null;
                    }
                }
            }

            ligneBataille.AjouterDonneesVideo(Donnees.m_donnees.TAB_PARTIE[0].I_TOUR, Donnees.m_donnees.TAB_BATAILLE_VIDEO, Donnees.m_donnees.TAB_BATAILLE_PIONS_VIDEO);
            message = string.Format("Fin de création de la bataille ID={0}", ligneBataille.ID_BATAILLE);
            LogFile.Notifier(message, out messageErreur);
            return ligneBataille;
        }

        /// <summary>
        /// calcul du cadre de la bataille
        /// </summary>
        /// <param name="ligneCaseBataille">Bataille source</param>
        /// <param name="xCaseHautGauche"></param>
        /// <param name="yCaseHautGauche"></param>
        /// <param name="xCaseBasDroite"></param>
        /// <param name="yCaseBasDroite"></param>
        public static void RectangleChampDeBataille(Donnees.TAB_CASERow ligneCaseBataille, out int xCaseHautGauche, out int yCaseHautGauche, out int xCaseBasDroite, out int yCaseBasDroite)
        {
            xCaseHautGauche = ligneCaseBataille.I_X - Constantes.CST_TAILLE_CHAMP_DE_BATAILLE / 2 * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE;
            yCaseHautGauche = ligneCaseBataille.I_Y - Constantes.CST_TAILLE_CHAMP_DE_BATAILLE / 2 * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE;
            xCaseBasDroite = ligneCaseBataille.I_X + Constantes.CST_TAILLE_CHAMP_DE_BATAILLE / 2 * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE;
            yCaseBasDroite = ligneCaseBataille.I_Y + Constantes.CST_TAILLE_CHAMP_DE_BATAILLE / 2 * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE;
            xCaseHautGauche = Math.Max(0, xCaseHautGauche);
            yCaseHautGauche = Math.Max(0, yCaseHautGauche);
            xCaseBasDroite = Math.Min(Donnees.m_donnees.TAB_JEU[0].I_LARGEUR_CARTE - 1, xCaseBasDroite);
            yCaseBasDroite = Math.Min(Donnees.m_donnees.TAB_JEU[0].I_HAUTEUR_CARTE - 1, yCaseBasDroite);
        }

        /// <summary>
        ///  Recherche une zone de combat par rapport à sa coordonnée
        /// </summary>
        /// <param name="orientation">vertical ou horizontal</param>
        /// <param name="x">abscisse du point</param>
        /// <param name="y">ordonnée du point</param>
        /// <param name="xCaseHautGauche">abscisse du point en haut à gauche de la bataille</param>
        /// <param name="yCaseHautGauche">ordonnée du point en haut à gauche de la bataille</param>
        /// <param name="xCaseBasDroite">abscisse du point en bas à droite de la bataille</param>
        /// <param name="yCaseBasDroite">ordonnée du point en bas à droite de la bataille</param>
        /// <returns></returns>
        internal static int RechercheZone(char orientation, int x, int y, int xCaseHautGauche, int yCaseHautGauche, int xCaseBasDroite, int yCaseBasDroite)
        {
            int izone;

            switch (orientation)
            {
                case Constantes.CST_BATAILLE_HORIZONTALE:
                    if (x < xCaseHautGauche + (xCaseBasDroite - xCaseHautGauche) / 3)
                    {
                        izone = 0;
                    }
                    else
                    {
                        if (x < xCaseHautGauche + 2 * (xCaseBasDroite - xCaseHautGauche) / 3)
                        {
                            izone = 1;
                        }
                        else
                        {
                            izone = 2;
                        }
                    }
                    if (y >= yCaseHautGauche + (yCaseBasDroite - yCaseHautGauche) / 2)
                    {
                        izone += 3;
                    }
                    break;
                default://vertical
                    if (y < yCaseHautGauche + (yCaseBasDroite - yCaseHautGauche) / 3)
                    {
                        izone = 0;
                    }
                    else
                    {
                        if (y < yCaseHautGauche + 2 * (yCaseBasDroite - yCaseHautGauche) / 3)
                        {
                            izone = 1;
                        }
                        else
                        {
                            izone = 2;
                        }
                    }
                    if (x < xCaseHautGauche + (xCaseBasDroite - xCaseHautGauche) / 2)
                    {
                        izone += 3;
                    }
                    break;
            }
            return izone;
        }

        static public int AvancementPourRecalcul(Constantes.TYPEPARCOURS tipePacours, Donnees.TAB_PIONRow lignePion, Donnees.TAB_CASERow ligneCaseDepart, Donnees.TAB_CASERow ligneCaseDestination, Donnees.TAB_ORDRERow ligneOrdre, out string erreur)
        {
            string requete, messageErreur, tri;
            int i;
            
            erreur = string.Empty;

            if (null == lignePion || null == ligneCaseDepart || null == ligneCaseDestination)
            {
                erreur = string.Format("PositionPourRecalcul : lignePion ou ligneCaseDepart ou ligneCaseDestination null");
                LogFile.Notifier(erreur, out messageErreur);
                return -1;
            }

            //existe-il déjà un chemin pour le pion sur le trajet demandé ?
            //On ne stocke pas cette information pourles liaisons entre depôt car les parcours changent à chaque test suite aux déplacements des troupes et des ennemis
            if (tipePacours == Constantes.TYPEPARCOURS.RAVITAILLEMENT)
            {
                return -1;
            }

            requete = string.Format("ID_PION={0}", lignePion.ID_PION);
            tri = "I_ORDRE";
            Donnees.TAB_PARCOURSRow[] parcoursExistant = (Donnees.TAB_PARCOURSRow[])Donnees.m_donnees.TAB_PARCOURS.Select(requete, tri);

            if ((null != parcoursExistant) && (0 < parcoursExistant.Length))
            {
                if (ligneCaseDestination.ID_CASE == parcoursExistant[parcoursExistant.Length - 1].ID_CASE)
                {
                    //cherche où l'unité se trouve dans le chemin existant
                    i = 0;
                    //while (i < parcoursExistant.Length && parcoursExistant[i].ID_CASE != ligneCaseDestination.ID_CASE) i++;
                    while (i < parcoursExistant.Length && parcoursExistant[i].ID_CASE != ligneCaseDepart.ID_CASE) i++;
                    if (i < parcoursExistant.Length)
                    {
                        return i;
                    }
                }
            }
            erreur = "chemin introuvable ou incorrect pour l'unité demandée";
            return -1;
        }

        internal static void AfficherUnitesZoom(string nomfichier, int xCaseHautGauche, int yCaseHautGauche, int xCaseBasDroite, int yCaseBasDroite)
        {
            Bitmap imageSource = GetImage(Constantes.MODELESCARTE.HISTORIQUE);

            Rectangle rect = new Rectangle(xCaseHautGauche, yCaseHautGauche, xCaseBasDroite - xCaseHautGauche, yCaseBasDroite - yCaseHautGauche);

            BitmapData imageCible = new BitmapData();
            imageSource.LockBits(rect, ImageLockMode.ReadOnly, imageSource.PixelFormat, imageCible);
            imageSource.UnlockBits(imageCible);
            Bitmap imageFinale;

            imageFinale = new Bitmap((int)(rect.Width * Constantes.CST_FACTEUR_ZOOM), (int)(rect.Height * Constantes.CST_FACTEUR_ZOOM), imageSource.PixelFormat);
            imageFinale.SetResolution(imageSource.HorizontalResolution, imageSource.VerticalResolution);
            Graphics graph = Graphics.FromImage(imageFinale);
            graph.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            graph.DrawImage(imageSource, new Rectangle(0, 0, (int)(rect.Width * Constantes.CST_FACTEUR_ZOOM), (int)(rect.Height * Constantes.CST_FACTEUR_ZOOM)), rect, GraphicsUnit.Pixel);
            graph.Dispose();
            AfficherUnites(imageFinale, Constantes.CST_FACTEUR_ZOOM, xCaseHautGauche, yCaseHautGauche, xCaseBasDroite, yCaseBasDroite);

            imageFinale.Save(nomfichier, ImageFormat.Png);
            imageFinale.Dispose();
        }
    }
}
