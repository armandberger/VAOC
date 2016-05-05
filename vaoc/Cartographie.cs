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

namespace vaoc
{
    class Cartographie
    {
        static protected AStar m_star = null;
        static protected Bitmap m_imageCarteHistorique;
        static protected Bitmap m_imageCarteGris;
        static protected Bitmap m_imageCarteZoom;
        static protected Bitmap m_imageCarteTopographique;
        static protected float m_rapportZoom;

        internal static AStar etoile
        {
            get
            {
                if (null == m_star)
                {
                    m_star = new AStar();
                }
                return m_star;
            }
        }

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
                LogFile.Notifier(string.Format("Erreur {1} au chargement de l'image : {0}", Donnees.m_donnees.TAB_JEU[0].S_NOM_CARTE_HISTORIQUE, ex.Message));
                return false;
            }
            try
            {
                if (null == m_imageCarteGris || bForcage)
                {
                    if (null != m_imageCarteGris) { m_imageCarteGris.Dispose(); }
                    m_imageCarteGris = (Bitmap)Image.FromFile(Constantes.repertoireDonnees + Donnees.m_donnees.TAB_JEU[0].S_NOM_CARTE_GRIS);
                }
            }
            catch (Exception ex)
            {
                LogFile.Notifier(string.Format("Erreur {1} au chargement de l'image : {0}", Donnees.m_donnees.TAB_JEU[0].S_NOM_CARTE_GRIS, ex.Message));
                return false;
            }
            try
            {
                if (null == m_imageCarteZoom || bForcage)
                {
                    if (null != m_imageCarteZoom) { m_imageCarteZoom.Dispose(); }
                    m_imageCarteZoom = (Bitmap)Image.FromFile(Constantes.repertoireDonnees + Donnees.m_donnees.TAB_JEU[0].S_NOM_CARTE_ZOOM);
                }
            }
            catch (Exception ex)
            {
                LogFile.Notifier(string.Format("Erreur {1} au chargement de l'image : {0}", Donnees.m_donnees.TAB_JEU[0].S_NOM_CARTE_ZOOM, ex.Message));
                return false;
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
                LogFile.Notifier(string.Format("Erreur {1} au chargement de l'image : {0}", Donnees.m_donnees.TAB_JEU[0].S_NOM_CARTE_TOPOGRAPHIQUE, ex.Message));
                return false;
            }
            m_rapportZoom = (float)m_imageCarteZoom.Width / (float)m_imageCarteHistorique.Width;
            
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

        public static void AfficherQG(Bitmap imageSource)
        {
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
                    LogFile.Notifier("AfficherChemin x=" + x1 + " y=" + y1);
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
            //stylo.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            //stylo.DashOffset = 5;
            //System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            x1 = y1 = -1;
            //LogFile.CreationLogFile("C:\\Users\\Public\\Documents\\vaoc\\poudre_et_biere\\test.log");
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
                AfficherUnites(imageSource, m_rapportZoom);
            }
            else
            {
                AfficherUnites(imageSource);
            }
        }

        public static void AfficherUnites(Bitmap imageSource)
        {
            AfficherUnites(imageSource, 1);
        }

        public static void AfficherUnites(Bitmap imageSource, float zoom)
        {
            Color couleur;
            Graphics graph = Graphics.FromImage(imageSource);
            Rectangle rect;
            SolidBrush brosse;
            foreach (Donnees.TAB_CASERow noeud in Donnees.m_donnees.TAB_CASE)
            {
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
                        rect = new Rectangle((int)(noeud.I_X * zoom - zoom / 2), (int)(noeud.I_Y * zoom - zoom / 2), (int)zoom + 1, (int)zoom + 1);
                        graph.FillEllipse(brosse, rect);
                    }
                }
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
            foreach (Donnees.TAB_NOMS_CARTERow ligneNom in
Donnees.m_donnees.TAB_NOMS_CARTE)
            {
                int x = 0, y = 0;

                lignePolice =
Donnees.m_donnees.TAB_POLICE.FindByID_POLICE(ligneNom.ID_POLICE);
                ligneCase =
Donnees.m_donnees.TAB_CASE.FindByID_CASE(ligneNom.ID_CASE);
                if (null == ligneCase)
                {
                    //possible en construction de cartes quand on repart d'un existant
                    //mais  il faut quand meme le signaler car sinon on ne peut pas savoir qu'il reste de vieux noms
                    messageErreur += string.Format("Case {0} de la ville {1} introuvable\r\n", ligneNom.ID_CASE, ligneNom.S_NOM);
                    continue;
                }
                brosse = new
SolidBrush(Color.FromArgb(lignePolice.I_ROUGE, lignePolice.I_VERT, lignePolice.I_BLEU));
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
                        y = ligneCase.I_Y - (int)tailleLigne.Height; ;
                        break;
                    case 3://A droite
                        x = ligneCase.I_X;
                        y = (int)Math.Floor(ligneCase.I_Y - tailleLigne.Height / 2);
                        break;
                    case 4://En bas à droite
                        x = ligneCase.I_X; ;
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
                        y = ligneCase.I_Y - (int)tailleLigne.Height; ;
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

        internal static void MiseAJourProprietaires()
        {
            /* Ce traitement prend 2 minutes ! Mais il n'est pas possible de le faire en Linq, donc pas de solution à moins d'utiliser
            foreach (Donnees.TAB_CASERow ligne in Donnees.m_donnees.TAB_CASE)
            {
                if (ligne.IsID_NOUVEAU_PROPRIETAIRENull())
                {
                    ligne.SetID_PROPRIETAIRENull();
                }
                else
                {
                    ligne.ID_PROPRIETAIRE = ligne.ID_NOUVEAU_PROPRIETAIRE;
                }
                ligne.SetID_NOUVEAU_PROPRIETAIRENull();
            }
             * */
            /* autre solution en 5 secondes, mais ne marche pas car ne remet pas a blanc idProprietaire en fait
            Donnees.TAB_CASEDataTable changeDataSet = (Donnees.TAB_CASEDataTable)Donnees.m_donnees.TAB_CASE.GetChanges();
            if (0==changeDataSet.Rows.Count) { return; }
            foreach (Donnees.TAB_CASERow ligneChange in changeDataSet)
            {
                Donnees.TAB_CASERow ligne = Donnees.m_donnees.TAB_CASE.FindByID_CASE(ligneChange.ID_CASE);
                if (ligne.IsID_NOUVEAU_PROPRIETAIRENull())
                {
                    ligne.SetID_PROPRIETAIRENull();
                }
                else
                {
                    ligne.ID_PROPRIETAIRE = ligne.ID_NOUVEAU_PROPRIETAIRE;
                }
                ligne.SetID_NOUVEAU_PROPRIETAIRENull();
                ligne.AcceptChanges();
            }
             */
            //Donnees.TAB_CASERow ligneCaseD = Donnees.m_donnees.TAB_CASE.FindByXY(174, 482);
            string requete = "(ID_PROPRIETAIRE IS NOT NULL) OR (ID_NOUVEAU_PROPRIETAIRE IS NOT NULL)";
            Donnees.TAB_CASERow[] changeRows = (Donnees.TAB_CASERow[])Donnees.m_donnees.TAB_CASE.Select(requete);
            foreach (Donnees.TAB_CASERow ligneChange in changeRows)
            {
                Donnees.TAB_CASERow ligne = Donnees.m_donnees.TAB_CASE.FindByID_CASE(ligneChange.ID_CASE);
                if (ligne.IsID_NOUVEAU_PROPRIETAIRENull())
                {
                    ligne.SetID_PROPRIETAIRENull();
                }
                else
                {
                    ligne.ID_PROPRIETAIRE = ligne.ID_NOUVEAU_PROPRIETAIRE;
                }
                ligne.SetID_NOUVEAU_PROPRIETAIRENull();
            }
            //Donnees.TAB_CASERow ligneCaseD2 = Donnees.m_donnees.TAB_CASE.FindByXY(174, 482);
        }

        internal static void RecadrerRect(Bitmap imageSource, ref Rectangle rect)
        {
            if (rect.X < 0) rect.X = 0;
            if (rect.Y < 0) rect.Y = 0;
            if (rect.Left + rect.Width > imageSource.Width)
            {
                rect.Width = imageSource.Width - rect.Left;
            }
            if (rect.Top + rect.Height > imageSource.Height)
            {
                rect.Height = imageSource.Height - rect.Top;
            }
        }

        protected static Bitmap GetImage(Constantes.MODELESCARTE modele)
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
        internal static bool DecoupeFichier(Constantes.MODELESCARTE modele, string nomFichierFinal, Rectangle rect, float angleRotation)
        {
            Bitmap imageSource = GetImage(modele);

            RecadrerRect(imageSource, ref rect);
            if (0 == rect.Height || 0 == rect.Width)
            {
                return true;//rien à faire ?
            }

            BitmapData imageCible = new BitmapData();
            imageSource.LockBits(rect, ImageLockMode.ReadOnly, imageSource.PixelFormat, imageCible);
            imageSource.UnlockBits(imageCible);
            Bitmap imageFinale = new Bitmap(imageCible.Width, imageCible.Height, imageCible.Stride, imageCible.PixelFormat, imageCible.Scan0);
            /* ne fonctionne pas visiblement
            if (angleRotation > 0)
            {
                Graphics graph = Graphics.FromImage(imageFinale);
                graph.RotateTransform(angleRotation);
            }
            */
            imageFinale.Save(nomFichierFinal, ImageFormat.Png);
            imageFinale.Dispose();
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

            if (null == imageSource)
            {
                LogFile.Notifier(string.Format("Erreur dans FichierGrise, impossible de trouver l'image pour le modele {0}", modele.ToString()));
                return false;
            }
            RecadrerRect(imageSource, ref rect);
            if (null == m_imageCarteGris)
            {
                LogFile.Notifier("Erreur dans FichierGrise, l'image carte grisée n'est pas chargée");
                return false;
            }
            LogFile.Notifier(string.Format("FichierGrise rectGris Left={0}, Top={1}, Width={2}, Height={3}", rectGris.Left, rectGris.Top, rectGris.Width, rectGris.Height));
            RecadrerRect(m_imageCarteGris, ref rectGris);

            //Si les images n'ont pas la même résolution, DrawImage refait une mise à l'échelle automatique !
            Bitmap imageFinale = new Bitmap(rectGris.Width, rectGris.Height, m_imageCarteGris.PixelFormat);
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

            Graphics graph = Graphics.FromImage(imageFinale);
            graph.DrawImage(m_imageCarteGris, 0, 0, rectGris, GraphicsUnit.Pixel);
            graph.DrawImage(imageSource, rect.X - rectGris.X, rect.Y - rectGris.Y, rect, GraphicsUnit.Pixel);

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
                    Donnees.TAB_CASERow ligneCase = Donnees.m_donnees.TAB_CASE.FindByID_CASE(lignePion.ID_CASE);
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

            List<Donnees.TAB_PIONRow> liste = new List<Donnees.TAB_PIONRow>();
            foreach (Donnees.TAB_PIONRow ligneP in Donnees.m_donnees.TAB_PION)
            {
                liste.Add(ligneP);
            }
            Parallel.ForEach(liste, item => item.PlacerStatique());
            return true;
        }

        internal static bool PlacerLesUnitesStatiques()
        {
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
        }

        internal static void CalculModeleMouvementsPion(out AstarTerrainOBJ[] tableCoutsMouvementsTerrain)
        {
            bool bRoutier, bHorsRoute;
            bRoutier = bHorsRoute = false;
            //int maxNumeroModeleTerrain = (int)Donnees.m_donnees.TAB_MODELE_TERRAIN.Compute("Max(ID_MODELE_TERRAIN)", null);
            int maxNumeroModeleTerrain = BD.Base.ModeleTerrain.MaxID;
            tableCoutsMouvementsTerrain = new AstarTerrainOBJ[maxNumeroModeleTerrain + 1];
            // par défaut on initialise les valeurs à "impassable"
            for (int i = 0; i < maxNumeroModeleTerrain + 1; i++)
            {
                tableCoutsMouvementsTerrain[i] = new AstarTerrainOBJ();
                tableCoutsMouvementsTerrain[i].cout = AStar.CST_COUTMAX;
                tableCoutsMouvementsTerrain[i].route = false;
            }

            //recherche des vrais valeurs et des plus mauvaises valeurs suivant les effectifs présents
            foreach (LigneMODELE_TERRAIN ligneterrain in BD.Base.ModeleTerrain)
            {
                // Toutes les cases coutent la même chose dans ce jeu, le terrain influe seulement sur la vitesse de l'unité (voir CalculVitesseMouvementPion)
                //dans le cas où l'on ne veut que les "routes", les modèles non routiers restent en impassables
                if ((!bRoutier && !bHorsRoute) ||
                    (bRoutier && ligneterrain.B_CIRCUIT_ROUTIER) ||
                    (bHorsRoute && ligneterrain.ID_MODELE_TERRAIN != 60 /*!ligneterrain.B_CIRCUIT_ROUTIER*/))
                {
                    int coutCase = Donnees.m_donnees.TAB_MODELE_MOUVEMENT[0].CoutCase(ligneterrain.ID_MODELE_TERRAIN);
                    tableCoutsMouvementsTerrain[ligneterrain.ID_MODELE_TERRAIN].cout = coutCase; 
                    tableCoutsMouvementsTerrain[ligneterrain.ID_MODELE_TERRAIN].route = ligneterrain.B_CIRCUIT_ROUTIER;
                }
            }
        }

        internal static void CalculModeleMouvementsPion(out AstarTerrain[] tableCoutsMouvementsTerrain)
        {
            CalculModeleMouvementsPion(out tableCoutsMouvementsTerrain, false);
        }

        internal static void CalculModeleMouvementsPion(out AstarTerrain[] tableCoutsMouvementsTerrain, bool bRoutier)
        {
            CalculModeleMouvementsPion(out tableCoutsMouvementsTerrain, bRoutier, false);
        }

        internal static void CalculModeleMouvementsPion(out AstarTerrain[] tableCoutsMouvementsTerrain, bool bRoutier, bool bHorsRoute)
        {
            int maxNumeroModeleTerrain = (int)Donnees.m_donnees.TAB_MODELE_TERRAIN.Compute("Max(ID_MODELE_TERRAIN)", null);
            tableCoutsMouvementsTerrain = new AstarTerrain[maxNumeroModeleTerrain + 1];
            // par défaut on initialise les valeurs à "impassable"
            for (int i = 0; i < maxNumeroModeleTerrain + 1; i++)
            {
                tableCoutsMouvementsTerrain[i] = new AstarTerrain();
                tableCoutsMouvementsTerrain[i].cout = AStar.CST_COUTMAX;
                tableCoutsMouvementsTerrain[i].route = false;
            }

            //recherche des vrais valeurs et des plus mauvaises valeurs suivant les effectifs présents
            foreach (Donnees.TAB_MODELE_TERRAINRow ligneterrain in Donnees.m_donnees.TAB_MODELE_TERRAIN)
            {
                // Toutes les cases coutent la même chose dans ce jeu, le terrain influe seulement sur la vitesse de l'unité (voir CalculVitesseMouvementPion)
                //dans le cas où l'on ne veut que les "routes", les modèles non routiers restent en impassables
                //if (!bRoutier || ligneterrain.B_CIRCUIT_ROUTIER)
                if ((!bRoutier && !bHorsRoute) ||
                    (bRoutier && ligneterrain.B_CIRCUIT_ROUTIER) ||
                    (bHorsRoute && ligneterrain.ID_MODELE_TERRAIN!=60 /*!ligneterrain.B_CIRCUIT_ROUTIER*/))
                {
                    //string requete = string.Format("ID_MODELE_MOUVEMENT={0} AND ID_METEO={1} AND ID_MODELE_TERRAIN={2}",
                    //    Donnees.m_donnees.TAB_MODELE_MOUVEMENT[0].ID_MODELE_MOUVEMENT,
                    //    Donnees.m_donnees.TAB_PARTIE[0].ID_METEO,
                    //    );
                    //Donnees.TAB_MOUVEMENT_COUTRow[] resCout = (Donnees.TAB_MOUVEMENT_COUTRow[])Donnees.m_donnees.TAB_MOUVEMENT_COUT.Select(requete);
                    int coutCase = Donnees.m_donnees.TAB_MODELE_MOUVEMENT[0].CoutCase(ligneterrain.ID_MODELE_TERRAIN);
                    //if (1 != resCout.Length)
                    //{
                    //    tableCoutsMouvementsTerrain[ligneterrain.ID_MODELE_TERRAIN].cout = AStar.CST_COUTMAX;//normalement ne devrait pas arriver
                    //    tableCoutsMouvementsTerrain[ligneterrain.ID_MODELE_TERRAIN].route = false;//normalement ne devrait pas arriver
                    //}
                    //else
                    {
                        tableCoutsMouvementsTerrain[ligneterrain.ID_MODELE_TERRAIN].cout = coutCase; // resCout[0].I_COUT;
                        tableCoutsMouvementsTerrain[ligneterrain.ID_MODELE_TERRAIN].route = ligneterrain.B_CIRCUIT_ROUTIER;
                    }
                }
            }
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

            message = string.Format("NouvelleBataille : en idCASE={0}", ligneCaseBataille.ID_CASE);
            LogFile.Notifier(message, out messageErreur);

            if (Donnees.m_donnees.TAB_PARTIE.Nocturne())
            {
                message = string.Format("NouvelleBataille : pas de nouvelle bataille la nuit pour IDPion={0} sur la idcase={1} ", lignePion.ID_PION, ligneCaseBataille.ID_CASE);
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
                return true;//pas vraiment une erreur, juste curieux
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
                    if (!lignePion.IsID_BATAILLENull() && lignePion.ID_BATAILLE == ligneBataille.ID_BATAILLE)
                    {
                        if (lignePionEnnemi.IsID_BATAILLENull())
                        {
                            ligneBataille.AjouterPionDansLaBataille(lignePionEnnemi, ligneCaseBataille);
                            nouvelleBataille = false;
                        }
                    }
                    if (!lignePionEnnemi.IsID_BATAILLENull() && lignePionEnnemi.ID_BATAILLE == ligneBataille.ID_BATAILLE)
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
                Donnees.TAB_BATAILLERow ligneNouvelleBataille = CreationBataille(ligneCaseBataille);
                //on ajoute systématiquement le pion ayant crée la bataille
                ligneNouvelleBataille.AjouterPionDansLaBataille(lignePion, ligneCaseBataille, true);
            }

            return true;
        }

        /// <summary>
        /// Création d'un nouveau lieu de bataille
        /// </summary>
        /// <param name="ligneCaseBataille">case centrale d'où part le combat</param>
        /// <returns>nouvelle bataille si OK, null si KO</returns>
        internal static Donnees.TAB_BATAILLERow CreationBataille(Donnees.TAB_CASERow ligneCaseBataille)
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

            RectangleChampDeBataille(ligneCaseBataille, out xCaseHautGauche, out yCaseHautGauche, out xCaseBasDroite, out yCaseBasDroite);

            #region recherche de l'orientation
            nbZoneConflitO = nbZoneConflitV = 0;
            requete = string.Format("I_X>={0} AND I_Y>={1} AND I_X<{2} AND I_Y<{3}", 
                xCaseHautGauche, yCaseHautGauche, xCaseBasDroite, yCaseBasDroite);
            Donnees.TAB_CASERow[] lignesCaseBataille = (Donnees.TAB_CASERow[])Donnees.m_donnees.TAB_CASE.Select(requete);

            // recherche sur l'horizontale
            // | 0 | 1 | 2 |
            // | 3 | 4 | 5 |
            zone0_H.Initialize();
            zone1_H.Initialize();
            foreach (Donnees.TAB_CASERow ligneCaseZone in lignesCaseBataille)
            {
                //si la case n'appartient à personne, ce n'est pas un critère
                if (!ligneCaseZone.EstInnocupe()) //.ID_PROPRIETAIRE. != DataSetCoutDonnees.CST_AUCUNPROPRIETAIRE)
                {
                    lignePion = ligneCaseZone.TrouvePionSurCase();
                    if (null == lignePion)
                    {
                        string proprio = ligneCaseZone.IsID_PROPRIETAIRENull() ? "null" : ligneCaseZone.ID_PROPRIETAIRE.ToString();
                        string nouveauProprio = ligneCaseZone.IsID_NOUVEAU_PROPRIETAIRENull() ? "null" : ligneCaseZone.ID_NOUVEAU_PROPRIETAIRE.ToString();
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
                        string proprio = ligneCaseZone.IsID_PROPRIETAIRENull() ? "null" : ligneCaseZone.ID_PROPRIETAIRE.ToString();
                        string nouveauProprio = ligneCaseZone.IsID_NOUVEAU_PROPRIETAIRENull() ? "null" : ligneCaseZone.ID_NOUVEAU_PROPRIETAIRE.ToString();
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

            #endregion

            #region recherche des leaders de chaque bataille
            // je le fais après la création pour utiliser la méthode générique
            foreach (Donnees.TAB_PIONRow lignePionBataille in Donnees.m_donnees.TAB_PION)
            {
                if (lignePionBataille.B_DETRUIT) { continue; }
                //unité QG et non présente dans un autre combat
                if (lignePionBataille.estQGHierarchique(out niveauHierarchique) && !lignePionBataille.estAuCombat && lignePionBataille.I_STRATEGIQUE > 0)
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
                                    "Aucun Combat"
                                    );
            if (null == ligneBataille)
            {
                message = string.Format("CreationBataille : impossible d'ajouter la bataille sur la case {0}", ligneCaseBataille.ID_CASE);
                LogFile.Notifier(message, out messageErreur);
                return null;
            }
            ligneBataille.SetI_TOUR_FINNull();
            ligneBataille.SetI_PHASE_FINNull();

            //ajout des pions et leaders présents dans la bataille
            //foreach (DataSetCoutDonnees.TAB_PIONRow lignePion in DataSetCoutDonnees.m_donnees.TAB_PION) -> pas possible car on ajouter des pions en cas d'envoie de message dans AjouterPionDansLaBataille
            foreach (Donnees.TAB_CASERow ligneCaseZone in lignesCaseBataille)
            {
                Donnees.TAB_PIONRow lignePionEnBataille = ligneCaseZone.TrouvePionSurCase();
                if (null != lignePionEnBataille)
                {
                    if (!ligneBataille.AjouterPionDansLaBataille(lignePionEnBataille, ligneCaseZone, ligneCaseBataille == ligneCaseZone))
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
                Donnees.TAB_CASERow ligneCasePionBataille = Donnees.m_donnees.TAB_CASE.FindByID_CASE(lignePionEnBataille.ID_CASE);
                if (ligneCasePionBataille.I_X >= xCaseHautGauche && ligneCasePionBataille.I_Y >= yCaseHautGauche && ligneCasePionBataille.I_X <= xCaseBasDroite && ligneCasePionBataille.I_Y <= yCaseBasDroite)
                {
                    if (!ligneBataille.AjouterPionDansLaBataille(lignePionEnBataille, ligneCasePionBataille, false))
                    {
                        return null;
                    }
                }
            }

            //i=0;
            //while (i<DataSetCoutDonnees.m_donnees.TAB_PION.Count)
            //{
            //    DataSetCoutDonnees.TAB_PIONRow lignePionEnBataille = DataSetCoutDonnees.m_donnees.TAB_PION[i];
            //    if (!lignePionEnBataille.B_DETRUIT)
            //    {
            //        ligneCasePion = lignePionEnBataille.CaseCourante();
            //        if (ligneCasePion.I_X <= xCaseBasDroite && ligneCasePion.I_Y <= yCaseBasDroite &&
            //            ligneCasePion.I_X >= xCaseHautGauche && ligneCasePion.I_Y >= yCaseHautGauche)
            //        {
            //            if (!AjouterPionDansLaBataille(ligneBataille, lignePionEnBataille))
            //            {
            //                return false;
            //            }
            //        }
            //    }
            //    i++;
            //}
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

        /// <summary>
        /// Initialise la valeur de ID_NATION pour TAB_PCC_COUT
        /// Indispensable avant de faire une recherche sur un parcours dont on ne peut pas traverser les cases occupées par l'adversaire
        /// </summary>
        /// <returns>true si OK, false si KO</returns>
        internal static bool InitialiserProprietairesTrajets()
        {
            return AStar.InitialisationProprietaireTrajet();
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

        /// <summary>
        /// Recherche d'un trajet pour une unité sur la carte
        /// </summary>
        /// <param name="tipePacours">type de parcours typeParcours.MOUVEMENT ou typeParcours.RAVITAILLEMENT</param>
        /// <param name="lignePion">Pion effectuant le trajet</param>
        /// <param name="ligneCaseDepart">case de départ</param>
        /// <param name="ligneCaseDestination">case de destination</param>
        /// <param name="ligneOrdre">ordre de mouvement lié à ce chemin </param>
        /// <param name="chemin">liste de cases formant le chemin trouvé</param>
        /// <param name="coutGlobal">cout global du chemin</param>
        /// <param name="coutHorsRoute">part du cout effecuté en dehors d'une route</param>
        /// <param name="tableCoutsMouvementsTerrain">table du cout de mouvement des cases suivant l'unité et la météo</param>
        /// <param name="erreur">message d'erreur</param>
        /// <returns>true si ok, false si ko</returns>
        internal static bool RechercheChemin(Constantes.TYPEPARCOURS tipePacours, Donnees.TAB_PIONRow lignePion, Donnees.TAB_CASERow ligneCaseDepart, Donnees.TAB_CASERow ligneCaseDestination, Donnees.TAB_ORDRERow ligneOrdre, out List<Donnees.TAB_CASERow> chemin, out double coutGlobal, out double coutHorsRoute, out AstarTerrain[] tableCoutsMouvementsTerrain, out string erreur)
        {
            string requete, message, messageErreur, tri;
            int i;
            DateTime timeStart;
            TimeSpan perf;
            int idNation=-1;//nation du pion, à indiquer dans la recherche du chemin si on ne peut pas traverser les troupes ennemies (ex: ravitaillement)
            Donnees.TAB_PARCOURSRow[] parcoursExistant = null;

            timeStart = DateTime.Now;
            chemin = null;
            tableCoutsMouvementsTerrain = null;
            erreur = string.Empty;
            coutGlobal = coutHorsRoute = 0;

            if (null == lignePion || null == ligneCaseDepart || null == ligneCaseDestination)
            {
                erreur = string.Format("RechercheChemin : lignePion ou ligneCaseDepart ou ligneCaseDestination null");
                LogFile.Notifier(erreur, out messageErreur);
                return false;
            }

            //calcul des couts, à renvoyer pour connaitre le cout pour avancer d'une case supplémentaire
            //il faut faire ce calcul très tot, car cette table peut être utilisée par l'appelant même si l'on ne renvoit effectivement pas de trajet
            CalculModeleMouvementsPion(out tableCoutsMouvementsTerrain);

            //existe-il déjà un chemin pour le pion sur le trajet demandé ?
            //On ne stocke pas cette information pourles liaisons entre depôt car les parcours changent à chaque test suite aux déplacements des troupes et des ennemis
            if (tipePacours != Constantes.TYPEPARCOURS.RAVITAILLEMENT)
            {
                requete = string.Format("ID_PION={0}", lignePion.ID_PION);
                tri = "I_ORDRE";
                parcoursExistant = (Donnees.TAB_PARCOURSRow[])Donnees.m_donnees.TAB_PARCOURS.Select(requete, tri);

                if ((null != parcoursExistant) && (0 < parcoursExistant.Length))
                {
                    //if (lignePion.effectifTotal > 0)
                    //{
                        if ((ligneCaseDepart.ID_CASE == parcoursExistant[0].ID_CASE)
                            && (ligneCaseDestination.ID_CASE == parcoursExistant[parcoursExistant.Length - 1].ID_CASE))
                        {
                            //on renvoie le chemin existant
                            chemin = new List<Donnees.TAB_CASERow>(parcoursExistant.Length);
                            for (i = 0; i < parcoursExistant.Length; i++)
                            {
                                chemin.Add(Donnees.m_donnees.TAB_CASE.FindByID_CASE(parcoursExistant[i].ID_CASE));
                            }
                            perf = DateTime.Now - timeStart;
                            message = string.Format("RechercheChemin : existant en {0} minutes, {1} secondes, {2} millisecondes", perf.Minutes, perf.Seconds, perf.Milliseconds);
                            LogFile.Notifier(message, out messageErreur);
                            return true;
                        }
                        //sinon, ce n'est pas le même chemin, il faut donc le recalculer
                        message = string.Format("RechercheChemin unité avec effectif: différent parcours existant allant de {0} à {1}, chemin demandé de {2} à {3}",
                            parcoursExistant[0].ID_CASE, parcoursExistant[parcoursExistant.Length - 1].ID_CASE,
                            ligneCaseDepart.ID_CASE, ligneCaseDestination.ID_CASE);
                        LogFile.Notifier(message, out messageErreur);
                    //}
                    /* 14/05/2015, cela ne sert surement plus à rien les unités sans effectif suivent la même règle que les autres maintenant
                    else
                    {
                        if (ligneCaseDestination.ID_CASE == parcoursExistant[parcoursExistant.Length - 1].ID_CASE)
                        {
                            //cherche où l'unité se trouve dans le chemin existant
                            i = 0;
                            //while (i < parcoursExistant.Length && parcoursExistant[i].ID_CASE != ligneCaseDestination.ID_CASE) i++;
                            while (i < parcoursExistant.Length && parcoursExistant[i].ID_CASE != ligneCaseDepart.ID_CASE) i++;
                            if (i < parcoursExistant.Length)
                            {
                                chemin = new List<Donnees.TAB_CASERow>(parcoursExistant.Length - i);
                                for (int j = 0; j < parcoursExistant.Length - i; j++)
                                {
                                    chemin.Add(Donnees.m_donnees.TAB_CASE.FindByID_CASE(parcoursExistant[i + j].ID_CASE));
                                }
                                perf = DateTime.Now - timeStart;
                                message = string.Format("RechercheChemin : existant en {0} minutes, {1} secondes, {2} millisecondes", perf.Minutes, perf.Seconds, perf.Milliseconds);
                                LogFile.Notifier(message, out messageErreur);
                                return true;
                            }
                            //sinon, ce n'est pas le même chemin, il faut donc le recalculer
                            message = string.Format("RechercheChemin unité sans effectif: différent parcours existant vers {0}, chemin demandé vers {1}",
                                parcoursExistant[parcoursExistant.Length - 1].ID_CASE,
                                ligneCaseDestination.ID_CASE);
                            LogFile.Notifier(message, out messageErreur);
                        }
                    }
                     * */
                    //destruction de tout autre parcours précédent 14/05/2015: cela semble totalement idiot de faire ça, detruire les parcours mémorisé !!!
                    /*
                    foreach (Donnees.TAB_PARCOURSRow ligneParcours in parcoursExistant)
                    {
                        ligneParcours.Delete();
                    }
                    Donnees.m_donnees.TAB_PARCOURS.AcceptChanges();
                     * */
                }
            }

            if (tipePacours == Constantes.TYPEPARCOURS.RAVITAILLEMENT)
            {
                //Dans le cas d'un ravitaitellement, les troupes ne peuvent pas traverser les lignes ennemies
                idNation = lignePion.idNation;
            }

            //calcul du nouveau parcours
            message = string.Format("RechercheChemin : SearchPath de {0} ({1},{2}) à {3} ({4},{5})",
                ligneCaseDepart.ID_CASE, ligneCaseDepart.I_X, ligneCaseDepart.I_Y, ligneCaseDestination.ID_CASE, ligneCaseDestination.I_X, ligneCaseDestination.I_Y);
            LogFile.Notifier(message, out messageErreur);
            etoile.SearchPathHPA(ligneCaseDepart, ligneCaseDestination, tableCoutsMouvementsTerrain, idNation);
            if (!etoile.PathFound) 
            {
                //en mode ravitaillement, il peut être normal de ne pas trouver de chemin vers un dépôt
                if (tipePacours != Constantes.TYPEPARCOURS.RAVITAILLEMENT)
                {
                    erreur = string.Format("{0}(ID={1}, erreur sur SearchPath dans RechercheChemin idDepart={2}, idDestination={3})",
                        lignePion.S_NOM, lignePion.ID_PION, ligneCaseDepart.ID_CASE, ligneCaseDestination.ID_CASE);
                    LogFile.Notifier(erreur, out messageErreur);
                    return false;
                }
                LogFile.Notifier("RechercheChemin : aucun chemin trouvé");
            }
            else
            {
                //destruction de tout autre parcours précédent
                if (null != parcoursExistant)
                {
                    foreach (Donnees.TAB_PARCOURSRow ligneParcours in parcoursExistant)
                    {
                        ligneParcours.Delete();
                    }
                    Donnees.m_donnees.TAB_PARCOURS.AcceptChanges();
                }

                chemin = etoile.PathByNodes;
                message = string.Format("RechercheChemin : SearchPath longueur={0}", chemin.Count);
                LogFile.Notifier(message, out messageErreur);

                //chemin = ParcoursOptimise(chemin); -> déplacé directement dans AStar
                coutGlobal = etoile.CoutGlobal;
                coutHorsRoute = etoile.HorsRouteGlobal;

                //stockage du chemin en table, sauf pour les recherches de depôt
                int casePrecedente = -1;
                if (tipePacours != Constantes.TYPEPARCOURS.RAVITAILLEMENT)
                {
                    i = 0;
                    foreach (Donnees.TAB_CASERow ligneCase in chemin)
                    {
                        if (casePrecedente != ligneCase.ID_CASE)
                        {
                            //pour éviter d'avoir deux fois la même case de suite dans le parcours, possible dans certains cas rares
                            Donnees.m_donnees.TAB_PARCOURS.AddTAB_PARCOURSRow(lignePion.ID_PION, i++, ligneCase.ID_CASE);
                        }
                        casePrecedente = ligneCase.ID_CASE;
                    }
                    Donnees.m_donnees.TAB_PARCOURS.AcceptChanges();
                }
            }

            perf = DateTime.Now - timeStart;
            message = string.Format("RechercheChemin : nouveau et stockage en {0} minutes, {1} secondes, {2} millisecondes", perf.Minutes, perf.Seconds, perf.Milliseconds);
            LogFile.Notifier(message, out messageErreur);
            return true;
        }

    }
}
