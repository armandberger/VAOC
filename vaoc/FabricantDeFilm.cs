﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;

namespace vaoc
{
    public enum TIPEUNITEVIDEO { INFANTERIE, CAVALERIE, ARTILLERIE, DEPOT, CONVOI, PONTONNIER, QG, BLESSE, PRISONNIER, AUTRE};

    public class EffectifEtVictoire
    {
        public int iTour;
        public int iNation;
        public int iEffectif;
        public int iVictoire;
    }

    public class LieuRemarquable
    {
        public int iTourDebut;
        public int iTourFin;
        public int i_X_CASE_HAUT_GAUCHE;
        public int i_Y_CASE_HAUT_GAUCHE;
        public int i_X_CASE_BAS_DROITE;
        public int i_Y_CASE_BAS_DROITE;
    }

    public class UniteRemarquable
    {
        public int iTour;
        public int iNation;
        public int i_X_CASE;
        public int i_Y_CASE;
        public TIPEUNITEVIDEO tipe;
        public bool b_blesse;
        public bool b_prisonnier;
    }

    public class UniteRole
    {
        public int iTour;
        public int i_X_CASE;
        public int i_Y_CASE;
        public string nom;
        public int iNation;
        public int ID_ROLE;
        public int iEffectif;
    }

    public class Role
    {
        public string nom;
        public int iNation;
        public int ID_ROLE;
        public int iEffectifMax;
    }
    
    public class Travelling
    {
        public int i_X_CASE;
        public int i_Y_CASE;
        public Travelling(int x, int y) { i_X_CASE = x; i_Y_CASE = y; }
    }

    public class MyCustomComparer : IComparer<FileInfo>
    {
        public int Compare(FileInfo x, FileInfo y)
        {
            // split filename
            string[] parts1 = x.Name.Split('_');
            string[] parts2 = y.Name.Split('_');

            // calculate how much leading zeros we need
            int toPad1 = 10 - parts1[2].Length;//carte_general_XX.png
            int toPad2 = 10 - parts2[2].Length;

            // add the zeros, only for sorting
            parts1[2] = parts1[2].Insert(0, new String('0', toPad1));
            parts2[2] = parts2[2].Insert(0, new String('0', toPad2));

            // create the comparable string
            string toCompare1 = string.Join("", parts1);
            string toCompare2 = string.Join("", parts2);

            // compare
            return toCompare1.CompareTo(toCompare2);
        }
    }

    public class AviWriter
    {
        public class AviException : ApplicationException { }
        public Bitmap Open(string fileName, int frameRate, int width, int height)
        {
            return new Bitmap("erreur");
        }

        public void AddFrame()
        {
        }

        public void AddFrame(BitmapData bmpDat)
        {
        }

        public void Close(){}

        private void CreateStream(){}

        private void SetOptions()
        {

        }//juste pour mémoire
    }


    public class FabricantDeFilm
    {
        private FileInfo[] m_listeFichiers;
        private AviWriter m_aw;
        private int m_traitement;//traitement principal
        private int m_traitementRole;//traitement secondaire si un fichier vidéo par role
        private bool m_bHistoriqueBataille;
        private bool m_bCarteGlobale;
        private bool m_bFilm;
        private string m_repertoireVideo;
        private float m_rapport;
        private List<LieuRemarquable> m_lieuxRemarquables;
        private List<EffectifEtVictoire> m_effectifsEtVictoires;
        private List<UniteRemarquable> m_unitesRemarquables;
        private List<UniteRole> m_unitesRoles;
        private Dictionary<int, Role> m_roles;
        private int m_largeurMax;
        private int m_hauteurMax;
        private int m_largeur;
        private int m_hauteur;
        private string[] m_texteImages;
        private int m_hauteurBandeau;
        private int m_largeurCote;
        private int m_totalvictoire;
        private int m_nbImages;
        private Font m_police;
        private bool m_bTravelling;
        private Dictionary<int, Travelling> m_travelling;
        public const int CST_DEBUT_FILM = 1;//on ne prende pas le premier tour c'est un tour de discussion, les unités sont téléporées ensuite

        System.ComponentModel.BackgroundWorker m_travailleur;
        private const int BARRE_ECART = 2;
        private const int BARRE_EPAISSEUR = 3;
        private const int RATIO_TRAVELLING = 8;
        private const string NOM_FICHIER_VIDEO = "video.mp4";
        private int m_tailleUnite;// Laisser une valeur paire ou cela créer des problèmes d'arrondi
        private int m_epaisseurUnite;//largeur des traits des unites;
        private int m_minX, m_minY, m_maxX, m_maxY;//position extremes des unités sur la carte
        //private BaseVideo m_baseVideo = new BaseVideo();
        private int m_xTravelling = -1;
        private int m_yTravelling = -1;
        private int m_effectifsMax = 0;
        private bool m_videoParRole = false;
        // commande dans un .bat ffmpeg -framerate 1 -i imageVideo_%%04d.png -c:v libx264 -r 30 -pix_fmt yuv420p video.mp4

        public FabricantDeFilm()
        {
        }

        public string Initialisation(string repertoireImages, string repertoireVideo, Font police, string texteMasqueImage, 
                                    string[] texteImages, int largeurOptimale, int HauteurOptimale, int tailleUnite, int epaisseurUnite,
                                    bool bHistoriqueBataille, bool bCarteGlobale, bool bFilm, bool bTravelling, bool videoParRole,
                                    List<LieuRemarquable> lieuxRemarquables, List<UniteRemarquable> unitesRemarquables, 
                                    List<EffectifEtVictoire> effectifsEtVictoires, List<UniteRole> unitesRoles,
                                    int totalvictoire, int nbImages, 
                                    System.ComponentModel.BackgroundWorker worker)
        {
            try
            {
                //pour test
                //Donnees.m_donnees.TAB_TRAVELLING.AddTAB_TRAVELLINGRow(0, 1700, 1200);//Wittenberg
                //Donnees.m_donnees.TAB_TRAVELLING.AddTAB_TRAVELLINGRow(37, 1080, 780);//Magdebourg
                //Donnees.m_donnees.TAB_TRAVELLING.AddTAB_TRAVELLINGRow(122, 1660, 1800);//Leipsiz
                //Donnees.m_donnees.TAB_TRAVELLING.AddTAB_TRAVELLINGRow(154, 1770, 1660);//Eilenburg

                SizeF tailleTexte;
                Graphics G;
                Bitmap fichierImageSource;
                m_largeur = int.MaxValue;
                m_hauteur = int.MaxValue;
                m_hauteurMax = 0;
                m_largeurMax = 0;
                m_bHistoriqueBataille = bHistoriqueBataille;
                m_bCarteGlobale = bCarteGlobale;
                m_bFilm = bFilm;
                m_repertoireVideo = repertoireVideo;
                m_lieuxRemarquables = lieuxRemarquables;
                m_unitesRemarquables = unitesRemarquables;
                m_effectifsEtVictoires = effectifsEtVictoires;
                m_texteImages = texteImages;
                m_hauteurBandeau = 0;
                m_largeurCote = 0;
                m_nbImages = nbImages;
                m_police = police;
                m_totalvictoire = totalvictoire;
                m_tailleUnite = tailleUnite;
                m_epaisseurUnite = epaisseurUnite;
                m_travailleur = worker;
                float largeurTexte, hauteurTexte;
                m_bTravelling = bTravelling;
                m_videoParRole = videoParRole;
                if (m_videoParRole)
                {
                    m_unitesRoles = unitesRoles;
                    m_roles = new Dictionary<int, Role>();
                    m_travelling = new Dictionary<int, Travelling>();
                    foreach (UniteRole roleUnite in m_unitesRoles)
                    {
                        if (!m_roles.ContainsKey(roleUnite.ID_ROLE)) 
                        {
                            Role role = new Role();
                            role.ID_ROLE = roleUnite.ID_ROLE;
                            role.iNation = roleUnite.iNation;
                            role.nom = roleUnite.nom;
                            role.iEffectifMax = roleUnite.iEffectif;
                            m_roles.Add(role.ID_ROLE,role); 
                            m_travelling.Add(role.ID_ROLE, new Travelling(-1, -1)); 
                        }
                        else
                        {
                            Role role = m_roles[roleUnite.ID_ROLE];
                            if (role.iEffectifMax< roleUnite.iEffectif) { role.iEffectifMax = roleUnite.iEffectif; }
                        }
                    }
                }

                //recherche le nombre d'images et leur taille
                DirectoryInfo dir = new DirectoryInfo(repertoireImages);
                m_listeFichiers = dir.GetFiles(texteMasqueImage, SearchOption.TopDirectoryOnly);
                if (0 == m_listeFichiers.Length) { return "le repertoire source ne contient aucune image " + texteMasqueImage; }

                Array.Sort(m_listeFichiers, new MyCustomComparer());//tri par nom
                foreach (FileInfo fichier in m_listeFichiers)
                {
                    Bitmap fichierImage = (Bitmap)Image.FromFile(fichier.FullName);
                    if (m_largeurMax < fichierImage.Width) { m_largeurMax = fichierImage.Width; }
                    if (m_hauteurMax < fichierImage.Height) { m_hauteurMax = fichierImage.Height; }
                    if (m_largeur > fichierImage.Width) { m_largeur = fichierImage.Width; }
                    if (m_hauteur > fichierImage.Height) { m_hauteur = fichierImage.Height; }
                }

                foreach(EffectifEtVictoire ev in effectifsEtVictoires)
                { m_effectifsMax = Math.Max(m_effectifsMax, ev.iEffectif); }

                if (m_largeurMax != m_largeur || m_hauteurMax != m_hauteur)
                {
                    return string.Format("Toutes les images n'ont pas la même taille, celles-ci vont de ({0},{1}) à ({2},{3}). Le traitement ne peut être effectué",
                        m_hauteur, m_largeur, m_hauteurMax, m_largeurMax);
                }

                if (!m_bCarteGlobale && !m_bTravelling)
                {
                    //on cherche la taille d'affichage par rapport aux positions extremes des unités sur la carte
                    m_minX = m_minY = int.MaxValue;
                    m_maxX = m_maxY = int.MinValue;
                    foreach(Donnees.TAB_VIDEORow ligneVideo in Donnees.m_donnees.TAB_VIDEO)
                    {
                        if (ligneVideo.ID_CASE<0 || (0==ligneVideo.EffectifTotal)) { continue; }//ca peut arriver visiblement...
                        //Donnees.TAB_CASERow ligneCase = Donnees.m_donnees.TAB_CASE.FindParID_CASE(ligneVideo.ID_CASE);
                        int x, y;
                        Donnees.m_donnees.TAB_CASE.ID_CASE_Vers_XY(ligneVideo.ID_CASE, out x, out y);
                        m_minX = Math.Min(m_minX, x);
                        m_minY = Math.Min(m_minY, y);
                        m_maxX = Math.Max(m_maxX, x);
                        m_maxY = Math.Max(m_maxY, y);
                    }
                }

                //calcul de la hauteur du bandeau et de la largeur du bandeau (au cas où cela dépassererait la largeur min)
                if (null != texteImages && texteImages.Length > 0)
                {
                    fichierImageSource = (Bitmap)Image.FromFile(m_listeFichiers[0].FullName);
                    G = Graphics.FromImage(fichierImageSource);
                    largeurTexte = hauteurTexte = 0;
                    foreach(string texte in texteImages)
                    {
                        tailleTexte = G.MeasureString(texte, police);
                        largeurTexte = Math.Max(largeurTexte, tailleTexte.Width);
                        hauteurTexte = Math.Max(hauteurTexte, tailleTexte.Height);
                    }

                    //ajout des chiffres d'effectif à droite et à gauche
                    tailleTexte = G.MeasureString(m_effectifsMax.ToString("000,000"), police);
                    m_largeurCote = (int)tailleTexte.Width+1;
                    hauteurTexte = Math.Max(hauteurTexte, tailleTexte.Height);

                    //v1 - m_hauteurBandeau = (int)(hauteurTexte * 1.5);
                    //v1 - m_largeur = Math.Max((int)(largeurTexte * 1)+1, m_largeur); // non, car on inclue pas les bords dans la taille d'image +2 * (int)(tailleTexte.Width * 2);
                    m_hauteurBandeau = (int)(hauteurTexte * 3);
                    m_largeur = Math.Max((int)(largeurTexte * 1)+1+ m_hauteurBandeau+ m_largeurCote, m_largeur); // non, car on inclue pas les bords dans la taille d'image +2 * (int)(tailleTexte.Width * 2);
                    fichierImageSource.Dispose();
                }

                //calcul de la taille optimale
                if (m_bTravelling)
                {
                    m_hauteur = HauteurOptimale - m_hauteurBandeau;
                    m_largeur = largeurOptimale;
                    m_rapport = 1; // (float)m_hauteur / m_largeur;
                }
                else
                {
                    if ((float)m_largeur / largeurOptimale > (float)m_hauteur / (HauteurOptimale - m_hauteurBandeau))
                    {
                        //on se cale donc sur la largeur (effort le plus grand)
                        m_rapport = (float)largeurOptimale / m_largeur;
                        m_hauteur = (int)(m_hauteur * m_rapport);
                        m_largeur = largeurOptimale;
                    }
                    else
                    {
                        m_rapport = (float)(HauteurOptimale - m_hauteurBandeau) / m_hauteur;
                        m_largeur = (int)(m_largeur * m_rapport);
                        m_hauteur = HauteurOptimale - m_hauteurBandeau;
                    }
                }

                if (m_bFilm)
                {
                    m_aw = new AviWriter();
                    Bitmap bmp = m_aw.Open(repertoireVideo + "\\" + "video.avi", 1, m_largeur + 2 * m_largeurCote, m_hauteur + m_hauteurBandeau);
                }
                else
                {
                    if (Directory.Exists(repertoireVideo))
                    {
                        //on supprime toutes les images qui pourraient exister d'un précédent traitement
                        dir = new DirectoryInfo(repertoireVideo);
                        FileInfo[] listeFichiers = dir.GetFiles("*.png", SearchOption.TopDirectoryOnly);

                        foreach (FileInfo fichier in listeFichiers)
                        {
                            File.Delete(fichier.FullName);
                        }
                        //on supprime également toutes les vidéos précédentes
                        listeFichiers = dir.GetFiles("*.mp4", SearchOption.TopDirectoryOnly);
                        foreach (FileInfo fichier in listeFichiers)
                        {
                            File.Delete(fichier.FullName);
                        }

                        foreach (FileInfo fichier in listeFichiers)
                        {
                            File.Delete(fichier.FullName);
                        }
                    }
                    else
                    {
                        Directory.CreateDirectory(repertoireVideo);
                    }
                }

                m_traitement = CST_DEBUT_FILM;
                if (m_videoParRole) 
                {
                    Dictionary<int, Role>.Enumerator listeRole = m_roles.GetEnumerator();
                    listeRole.MoveNext();
                    m_traitementRole = listeRole.Current.Key;
                }
                else
                {
                    m_traitementRole = 0;
                }
                return string.Empty;
            }
            catch (AviWriter.AviException e)
            {
                return "AVI Exception in: " + e.ToString();
            }
        }

        public string Terminer()
        {
            try
            {
                if (m_bFilm) { m_aw.Close(); }
                if (m_bTravelling)
                {
                    //on lance la commande DOS de création du film
                    //string YourApplicationPath = m_repertoireVideo + "\\ffmpeg.exe";
                    if (m_videoParRole)
                    {
                        foreach (Role role in m_roles.Values)
                        {
                            FilmMpeg(ChaineFichier(role.nom));
                        }
                    }
                    else
                    {
                        FilmMpeg("imageVideo");
                    }
                }
                m_travailleur.ReportProgress(100);
                return string.Empty;
            }
            catch (AviWriter.AviException e)
            {
                return "AVI Exception in: " + e.ToString();
            }
        }

        private void FilmMpeg(string nom)
        {
            ProcessStartInfo processInfo = new ProcessStartInfo();
            processInfo.WindowStyle = ProcessWindowStyle.Normal;
            processInfo.FileName = "ffmpeg.exe";
            processInfo.WorkingDirectory = m_repertoireVideo; //Path.GetDirectoryName(YourApplicationPath);
            processInfo.Arguments = string.Format("-framerate 1 -i {0}_%04d.png -c:v libx264 -r 30 -pix_fmt yuv420p {0}.mp4", nom);
            Process.Start(processInfo);
        }

        public string Traitement()
        {
            SizeF tailleTexte;
            Graphics G;
            Bitmap fichierImageSource;
            UniteRemarquable uniteQG = new UniteRemarquable();
            uniteQG.tipe = TIPEUNITEVIDEO.QG;

            try
            {
                //Debug.WriteLine("FabricantDeFilm:Traitement n°" + m_traitement);
                FileInfo fichier = 1 == m_listeFichiers.Length ? m_listeFichiers[0] : m_listeFichiers[m_traitement];
                fichierImageSource = (Bitmap)Image.FromFile(fichier.FullName);
                Bitmap fichierImage = new Bitmap(m_largeur, m_hauteur + m_hauteurBandeau, fichierImageSource.PixelFormat);
                G = Graphics.FromImage(fichierImage);
                G.PageUnit = GraphicsUnit.Pixel;
                //bandes blanches en bas et sur les côtés
                //bas
                Rectangle rectBas = new Rectangle(0, m_hauteur, m_largeur, m_hauteurBandeau);
                G.FillRectangle(Brushes.White, rectBas);

                //indicateur de victoires sous forme de camembert et ligne des effectifs
                int largeurGraphEffectifs = rectBas.Width - m_largeurCote - m_hauteurBandeau;
                if (m_traitement > 0)
                {
                    if (m_videoParRole)
                    {
                        if (m_roles[m_traitementRole].iEffectifMax > 0)
                        {
                            Pen styloEff;
                            int iNationRole = -1;
                            Point[] pointsEff = new Point[m_traitement + 1];
                            int nbTours = m_effectifsEtVictoires.Count / 2;
                            foreach (UniteRole role in m_unitesRoles)
                            {
                                if (role.iTour > m_traitement)
                                {
                                    continue;
                                }

                                if (m_roles[m_traitementRole].ID_ROLE == role.ID_ROLE)
                                {
                                    iNationRole = role.iNation;
                                    pointsEff[role.iTour].X = rectBas.X + m_largeurCote + (role.iTour * largeurGraphEffectifs / nbTours);
                                    pointsEff[role.iTour].Y = rectBas.Y + rectBas.Height + BARRE_ECART - (role.iEffectif * (rectBas.Height - BARRE_ECART) / m_roles[m_traitementRole].iEffectifMax);
                                }
                            }
                            //on trace les lignes des effectifs
                            styloEff = (0 == iNationRole) ? new Pen(Color.Blue, 3) : new Pen(Color.Red, 3);
                            G.DrawLines(styloEff, pointsEff);
                        }
                    }
                    else
                    {
                        int victoire0 = 0, victoire1 = 0;
                        Point[] pointsEff0 = new Point[m_traitement + 1];
                        Point[] pointsEff1 = new Point[m_traitement + 1];
                        int nbTours = m_effectifsEtVictoires.Count / 2;
                        if (null != m_effectifsEtVictoires && m_effectifsEtVictoires.Count > 0)
                        {
                            foreach (EffectifEtVictoire eev in m_effectifsEtVictoires)
                            {
                                if (eev.iTour == m_traitement)
                                {
                                    if (0 == eev.iNation)
                                    {
                                        victoire0 = eev.iVictoire;
                                    }
                                    else
                                    {
                                        victoire1 = eev.iVictoire;
                                    }
                                }
                                if (eev.iTour <= m_traitement)
                                {
                                    if (0 == eev.iNation)
                                    {
                                        pointsEff0[eev.iTour].X = rectBas.X + m_largeurCote + (eev.iTour * largeurGraphEffectifs / nbTours);
                                        pointsEff0[eev.iTour].Y = rectBas.Y + rectBas.Height + BARRE_ECART - (eev.iEffectif * (rectBas.Height - BARRE_ECART) / m_effectifsMax);
                                    }
                                    else
                                    {
                                        pointsEff1[eev.iTour].X = rectBas.X + m_largeurCote + (eev.iTour * largeurGraphEffectifs / nbTours);
                                        pointsEff1[eev.iTour].Y = rectBas.Y + rectBas.Height + BARRE_ECART - (eev.iEffectif * (rectBas.Height - BARRE_ECART) / m_effectifsMax);
                                    }
                                }
                            }
                        }
                        Rectangle RectangleVictoire = new Rectangle(rectBas.Width - BARRE_ECART - rectBas.Height,
                                                                    rectBas.Y + BARRE_ECART,
                                                                    rectBas.Height - 2 * BARRE_ECART, rectBas.Height - 2 * BARRE_ECART);
                        //on pose un cercle complet dans le fond en premier
                        G.FillEllipse(Brushes.Blue, RectangleVictoire);
                        //on complète par le camembert
                        int degresVictoire = (victoire1 * 360 / (victoire0 + victoire1));
                        G.FillPie(Brushes.Red, RectangleVictoire, 90 - degresVictoire, degresVictoire);
                        //on trace les lignes des effectifs
                        Pen styloEff0 = new Pen(Color.Blue, 3);
                        Pen styloEff1 = new Pen(Color.Red, 3);
                        G.DrawLines(styloEff1, pointsEff1);
                        G.DrawLines(styloEff0, pointsEff0);
                    }
                }

                //bandeau avec texte
                if (null != m_texteImages && m_texteImages.Length > 0)
                {
                    tailleTexte = G.MeasureString(m_texteImages[m_traitement], m_police);
                    Rectangle rectText = new Rectangle(m_largeurCote + (m_largeur - m_largeurCote - m_hauteurBandeau - (int)tailleTexte.Width) / 2,
                                            m_hauteur + (m_hauteurBandeau - (int)tailleTexte.Height) / 2,
                                            (int)tailleTexte.Width + 1,
                                            (int)tailleTexte.Height + 1);
                    G.DrawString(m_texteImages[m_traitement], m_police, Brushes.Black, rectText);
                }

                // effectifs et indicateur de victoire par camp ou par role
                if (m_videoParRole)
                {
                    foreach (UniteRole role in m_unitesRoles)
                    {
                        if (role.iTour != m_traitement)
                        {
                            continue;
                        }
                        
                        if (m_roles[m_traitementRole].ID_ROLE == role.ID_ROLE)
                        {
                            string strEffectif = role.iEffectif.ToString("###,000");
                            tailleTexte = G.MeasureString(strEffectif, m_police);
                            Brush brosse = (0 == role.iNation) ? Brushes.Blue : Brushes.Red;
                            //affichage des effectifs
                            G.DrawString(strEffectif, m_police, brosse,
                                new Rectangle(BARRE_ECART + rectBas.X,
                                                rectBas.Y + (int)(tailleTexte.Height),
                                                (int)tailleTexte.Width + 1,
                                                (int)tailleTexte.Height + 1));
                            break;
                        }
                    }
                }
                else
                {
                    if (null != m_effectifsEtVictoires && m_effectifsEtVictoires.Count > 0)
                    {
                        foreach (EffectifEtVictoire eev in m_effectifsEtVictoires)
                        {
                            if (eev.iTour != m_traitement)
                            {
                                continue;
                            }

                            string strEffectif = eev.iEffectif.ToString("###,000");
                            tailleTexte = G.MeasureString(strEffectif, m_police);
                            Brush brosse = (0 == eev.iNation) ? Brushes.Blue : Brushes.Red;
                            int y = rectBas.Y;
                            y += (0 == eev.iNation) ? (int)(tailleTexte.Height * 1 / 2) : (int)(tailleTexte.Height * 3 / 2);
                            //affichage des effectifs
                            G.DrawString(strEffectif, m_police, brosse,
                                new Rectangle(BARRE_ECART + rectBas.X,
                                                y,
                                                (int)tailleTexte.Width + 1,
                                                (int)tailleTexte.Height + 1));
                        }
                    }
                }

                //image de base
                if (m_bTravelling)
                {
                    int xCentreTravelling=0, yCentreTravelling=0;
                    if (m_videoParRole)
                    {
                        foreach (UniteRole role in m_unitesRoles)
                        {
                            if (m_roles[m_traitementRole].ID_ROLE == role.ID_ROLE && role.iTour == m_traitement)
                            {
                                xCentreTravelling = role.i_X_CASE - m_largeur / 2;
                                yCentreTravelling = role.i_Y_CASE - m_hauteur / 2;
                                uniteQG.i_X_CASE = role.i_X_CASE;
                                uniteQG.i_Y_CASE = role.i_Y_CASE;
                                uniteQG.iNation = role.iNation;
                                break;
                            }
                        }
                        if (m_travelling[m_traitementRole].i_X_CASE < 0 || m_travelling[m_traitementRole].i_Y_CASE < 0)
                        {
                            m_travelling[m_traitementRole].i_X_CASE = xCentreTravelling;
                            m_travelling[m_traitementRole].i_Y_CASE = yCentreTravelling;
                        }
                        else
                        {
                            if (m_travelling[m_traitementRole].i_X_CASE != xCentreTravelling)
                            {
                                //on fait un travelling pour approcher la position
                                m_travelling[m_traitementRole].i_X_CASE = (Math.Abs(m_travelling[m_traitementRole].i_X_CASE - xCentreTravelling) < m_largeur / RATIO_TRAVELLING) ? xCentreTravelling : m_travelling[m_traitementRole].i_X_CASE + Math.Sign(xCentreTravelling - m_travelling[m_traitementRole].i_X_CASE) * m_largeur / RATIO_TRAVELLING;
                            }
                            if (m_travelling[m_traitementRole].i_Y_CASE != yCentreTravelling)
                            {
                                //on fait un travelling pour approcher la position
                                m_travelling[m_traitementRole].i_Y_CASE = (Math.Abs(m_travelling[m_traitementRole].i_Y_CASE - yCentreTravelling) < m_hauteur / RATIO_TRAVELLING) ? yCentreTravelling : m_travelling[m_traitementRole].i_Y_CASE + Math.Sign(yCentreTravelling - m_travelling[m_traitementRole].i_Y_CASE) * m_hauteur / RATIO_TRAVELLING;
                            }
                        }
                        m_xTravelling = m_travelling[m_traitementRole].i_X_CASE;
                        m_yTravelling = m_travelling[m_traitementRole].i_Y_CASE;
                    }
                    else
                    {
                        string requete = string.Format("I_TOUR<={0}", m_traitement);
                        Donnees.TAB_TRAVELLINGRow[] resultatTravelling = (Donnees.TAB_TRAVELLINGRow[])Donnees.m_donnees.TAB_TRAVELLING.Select(requete, "I_TOUR DESC");
                        xCentreTravelling = resultatTravelling[0].I_X - m_largeur / 2;
                        yCentreTravelling = resultatTravelling[0].I_Y - m_hauteur / 2;
                        if (m_xTravelling < 0 || m_yTravelling < 0)
                        {
                            m_xTravelling = xCentreTravelling;
                            m_yTravelling = yCentreTravelling;
                        }
                        else
                        {
                            if (m_xTravelling != xCentreTravelling)
                            {
                                //on fait un travelling pour approcher la position
                                m_xTravelling = (Math.Abs(m_xTravelling - xCentreTravelling) < m_largeur / RATIO_TRAVELLING) ? xCentreTravelling : m_xTravelling + Math.Sign(xCentreTravelling - m_xTravelling) * m_largeur / RATIO_TRAVELLING;
                            }
                            if (m_yTravelling != yCentreTravelling)
                            {
                                //on fait un travelling pour approcher la position
                                m_yTravelling = (Math.Abs(m_yTravelling - yCentreTravelling) < m_hauteur / RATIO_TRAVELLING) ? yCentreTravelling : m_yTravelling + Math.Sign(yCentreTravelling - m_yTravelling) * m_hauteur / RATIO_TRAVELLING;
                            }
                        }
                    }
                    //Bitmap imageVideo = new Bitmap(m_largeur, m_hauteur, fichierImageSource.PixelFormat);
                    //Graphics graph = Graphics.FromImage(imageVideo);

                    G.DrawImage(fichierImageSource, -1, -1, new Rectangle(m_xTravelling, m_yTravelling, m_largeur, m_hauteur), GraphicsUnit.Pixel);
                    //imageVideo.Save(m_repertoireVideo + "\\" + "test.png", ImageFormat.Png);
                    //G.DrawImageUnscaledAndClipped(imageVideo, new Rectangle(0,0, m_largeur, m_hauteur));
                    //graph.Dispose();
                    //imageVideo.Dispose();
                }
                else
                {
                    G.DrawImage(fichierImageSource, 0, 0, m_largeur, m_hauteur);
                }

                if (m_bHistoriqueBataille)
                {
                    Pen styloExterieur = new Pen(Color.Black, 4);
                    Pen styloInterieur = new Pen(Color.White, 1);
                    //on ajoute les batailles s'il y en a
                    foreach (LieuRemarquable ligneLieu in m_lieuxRemarquables)
                    {
                        if (m_traitement >= ligneLieu.iTourDebut && m_traitement <= ligneLieu.iTourFin)
                        {
                            G.DrawRectangle(styloExterieur,
                                (ligneLieu.i_X_CASE_HAUT_GAUCHE- m_xTravelling) * m_rapport,
                                (ligneLieu.i_Y_CASE_HAUT_GAUCHE - m_yTravelling) * m_rapport,
                                (ligneLieu.i_X_CASE_BAS_DROITE - ligneLieu.i_X_CASE_HAUT_GAUCHE) * m_rapport,
                                (ligneLieu.i_Y_CASE_BAS_DROITE - ligneLieu.i_Y_CASE_HAUT_GAUCHE) * m_rapport);

                            G.DrawRectangle(styloInterieur,
                                (ligneLieu.i_X_CASE_HAUT_GAUCHE - m_xTravelling ) * m_rapport,
                                (ligneLieu.i_Y_CASE_HAUT_GAUCHE - m_yTravelling) * m_rapport,
                                (ligneLieu.i_X_CASE_BAS_DROITE - ligneLieu.i_X_CASE_HAUT_GAUCHE) * m_rapport,
                                (ligneLieu.i_Y_CASE_BAS_DROITE - ligneLieu.i_Y_CASE_HAUT_GAUCHE) * m_rapport);
                        }
                    }

                    //on ajoute les unités
                    // d'abord tous les dépôts et convois
                    foreach (UniteRemarquable unite in m_unitesRemarquables)
                    {
                        if (unite.iTour == m_traitement &&
                            (unite.tipe != TIPEUNITEVIDEO.INFANTERIE && unite.tipe != TIPEUNITEVIDEO.CAVALERIE &&
                            unite.tipe != TIPEUNITEVIDEO.ARTILLERIE && unite.tipe != TIPEUNITEVIDEO.QG))
                        {
                            DessineUnite(G, unite, m_xTravelling, m_yTravelling);
                        }
                    }

                    //maintenant les unités combattantes
                    foreach (UniteRemarquable unite in m_unitesRemarquables)
                    {
                        if (unite.iTour == m_traitement &&
                            (unite.tipe == TIPEUNITEVIDEO.INFANTERIE || unite.tipe == TIPEUNITEVIDEO.CAVALERIE ||
                            unite.tipe == TIPEUNITEVIDEO.ARTILLERIE || unite.tipe == TIPEUNITEVIDEO.QG))
                        {
                            DessineUnite(G, unite, m_xTravelling, m_yTravelling);
                        }
                    }
                }
                //Le leader à la fin
                if (m_bTravelling && m_videoParRole)
                {
                    DessineUnite(G, uniteQG, m_xTravelling, m_yTravelling);
                }
                //G.DrawImageUnscaled(fichierImageSource, 0, 0);
                if (m_bFilm)
                {
                    fichierImage.Save(m_repertoireVideo + "\\test.png", ImageFormat.Png);
                    fichierImage.RotateFlip(RotateFlipType.RotateNoneFlipY);//il faut retourner l'image sinon, elle apparait inversée dans la vidéo
                    BitmapData bmpDat = fichierImage.LockBits(
                        new Rectangle(0, 0, m_largeur + 2 * m_largeurCote, m_hauteur + m_hauteurBandeau), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);                    
                    //BitmapData imageCible = new BitmapData();
                    //m_imageCarte.LockBits(rect, ImageLockMode.ReadOnly, m_imageCarte.PixelFormat, imageCible);
                    //m_imageCarte.UnlockBits(imageCible);
                    //Bitmap imageFinale = new Bitmap(imageCible.Width, imageCible.Height, imageCible.Stride, imageCible.PixelFormat, imageCible.Scan0);
                    //imageFinale.Save(nomFichierFinal);
                    m_aw.AddFrame(bmpDat);
                    fichierImage.UnlockBits(bmpDat);
                }
                else
                {
                    if (m_videoParRole)
                    {
                        fichierImage.Save(ChaineFichier(m_repertoireVideo + "\\" + m_roles[m_traitementRole].nom + "_" + m_traitement.ToString("0000") + ".png"), ImageFormat.Png);
                    }
                    else
                    {
                        fichierImage.Save(m_repertoireVideo + "\\" + "imageVideo_" + m_traitement.ToString("0000") + ".png", ImageFormat.Png);
                    }
                }

                G.Dispose();
                fichierImage.Dispose();
                fichierImageSource.Dispose();
                if (m_videoParRole)
                {
                    // on recherche la clef suivante
                    Dictionary<int, Role>.Enumerator listeRole = m_roles.GetEnumerator();
                    listeRole.MoveNext();
                    while (listeRole.Current.Key!= m_traitementRole) listeRole.MoveNext();
                    if (!listeRole.MoveNext())
                    {
                        m_traitementRole = 0;
                        m_traitement++;
                    }
                    else
                    {
                        m_traitementRole = listeRole.Current.Key;
                    }                        
                }
                else
                {
                    m_traitement++;
                }
                m_travailleur.ReportProgress(m_traitement*100/m_nbImages);
                //m_travailleur.ReportProgress(m_traitement * 100 / m_listeFichiers.Length);
                //if (m_traitement == m_listeFichiers.Length)
                if (m_traitement == m_nbImages)
                {
                    return "film crée";
                }
                return string.Empty;
            }
            catch (AviWriter.AviException e)
            {
                return "AVI Exception in: " + e.ToString();
            }
        }

        public string TraitementV1()
        {
            SizeF tailleTexte;
            Graphics G;
            Bitmap fichierImageSource;

            try
            {
                //Debug.WriteLine("FabricantDeFilm:Traitement n°" + m_traitement);
                FileInfo fichier = 1 == m_listeFichiers.Length ? m_listeFichiers[0] : m_listeFichiers[m_traitement];
                fichierImageSource = (Bitmap)Image.FromFile(fichier.FullName);
                Bitmap fichierImage = new Bitmap(m_largeur + 2 * m_largeurCote, m_hauteur + m_hauteurBandeau, fichierImageSource.PixelFormat);
                G = Graphics.FromImage(fichierImage);
                G.PageUnit = GraphicsUnit.Pixel;
                //bandes blanches en bas et sur les côtés
                //bas
                G.FillRectangle(Brushes.White, new Rectangle(0, m_hauteur, m_largeur + 2 * m_largeurCote, m_hauteurBandeau));
                //gauche
                G.FillRectangle(Brushes.White, new Rectangle(0, 0, m_largeurCote, m_hauteur));
                //droite
                G.FillRectangle(Brushes.White, new Rectangle(m_largeur + m_largeurCote, 0, m_largeurCote, m_hauteur));

                //bandeau avec texte
                if (null != m_texteImages && m_texteImages.Length > 0)
                {
                    tailleTexte = G.MeasureString(m_texteImages[m_traitement], m_police);
                    G.DrawString(m_texteImages[m_traitement], m_police, Brushes.Black,
                        new Rectangle(m_largeurCote + (m_largeur - (int)tailleTexte.Width) / 2,
                                        m_hauteur + (m_hauteurBandeau - (int)tailleTexte.Height) / 2,
                                        (int)tailleTexte.Width + 1,
                                        (int)tailleTexte.Height + 1));
                    //m_largeur - (m_largeur - (int)tailleTexte.Width) / 2, 
                    //m_hauteurBandeau - (m_hauteurBandeau - (int)tailleTexte.Height) / 2));
                }

                //indicateur de victoires sous forme de camembert
                int victoire0 = 0, victoire1 = 0;
                if (null != m_effectifsEtVictoires && m_effectifsEtVictoires.Count > 0)
                {
                    foreach (EffectifEtVictoire eev in m_effectifsEtVictoires)
                    {
                        if (eev.iTour != m_traitement)
                        {
                            continue;
                        }
                        if (0 == eev.iNation) { victoire0 = eev.iVictoire; } else { victoire1 = eev.iVictoire; }

                        Brush brosseNation = (0 == eev.iNation) ? Brushes.Blue : Brushes.Red;
                    }
                }

                // effectifs et indicateur de victoire par camp
                if (null != m_effectifsEtVictoires && m_effectifsEtVictoires.Count > 0)
                {
                    foreach (EffectifEtVictoire eev in m_effectifsEtVictoires)
                    {
                        if (eev.iTour != m_traitement)
                        {
                            continue;
                        }
                        string strEffectif = eev.iEffectif.ToString("000,000");
                        tailleTexte = G.MeasureString(strEffectif, m_police);
                        if (0 == eev.iNation)
                        {
                            //affichage des effectifs
                            G.DrawString(strEffectif, m_police, Brushes.Blue,
                                new Rectangle((m_largeurCote - (int)tailleTexte.Width) / 2,
                                                m_hauteur + (m_hauteurBandeau - (int)tailleTexte.Height) / 2,
                                                (int)tailleTexte.Width + 1,
                                                (int)tailleTexte.Height + 1));
                            //affichage de la barre de victoire
                            //barre inférieure
                            G.FillRectangle(Brushes.Blue, new Rectangle(BARRE_ECART, m_hauteur - BARRE_EPAISSEUR, m_largeurCote - 2 * BARRE_ECART, BARRE_EPAISSEUR));
                            //barre verticale (points)
                            int h = eev.iVictoire * (m_hauteur - BARRE_EPAISSEUR) / m_totalvictoire;
                            G.FillRectangle(Brushes.Blue, new Rectangle(BARRE_ECART + m_largeurCote / 4,
                                                                        m_hauteur - BARRE_EPAISSEUR - h,
                                                                        m_largeurCote / 2 - 2 * BARRE_ECART,
                                                                        h));
                        }
                        else
                        {
                            //affichage des effectifs
                            G.DrawString(strEffectif, m_police, Brushes.Red,
                                new Rectangle(m_largeur + m_largeurCote - (m_largeurCote - (int)tailleTexte.Width) / 2,
                                                m_hauteur + (m_hauteurBandeau - (int)tailleTexte.Height) / 2,
                                                (int)tailleTexte.Width + 1,
                                                (int)tailleTexte.Height + 1));

                            //affichage de la barre de victoire
                            //barre inférieure
                            G.FillRectangle(Brushes.Red, new Rectangle(m_largeur + m_largeurCote + BARRE_ECART, m_hauteur - BARRE_EPAISSEUR, m_largeurCote - 2 * BARRE_ECART, BARRE_EPAISSEUR));
                            //barre verticale (points)
                            int h = eev.iVictoire * (m_hauteur - BARRE_EPAISSEUR) / m_totalvictoire;
                            G.FillRectangle(Brushes.Red, new Rectangle(m_largeur + m_largeurCote + BARRE_ECART + m_largeurCote / 4,
                                                                        m_hauteur - BARRE_EPAISSEUR - h,
                                                                        m_largeurCote / 2 - 2 * BARRE_ECART,
                                                                        h));
                        }
                    }
                }

                //image de base
                if (m_bTravelling)
                {
                    string requete = string.Format("I_TOUR<={0}", m_traitement);
                    Donnees.TAB_TRAVELLINGRow[] resultatTravelling = (Donnees.TAB_TRAVELLINGRow[])Donnees.m_donnees.TAB_TRAVELLING.Select(requete, "I_TOUR DESC");
                    int xCentreTravelling = resultatTravelling[0].I_X - m_largeur / 2;
                    int yCentreTravelling = resultatTravelling[0].I_Y - m_hauteur / 2;
                    if (m_xTravelling < 0 || m_yTravelling < 0)
                    {
                        m_xTravelling = xCentreTravelling;
                        m_yTravelling = yCentreTravelling;
                    }
                    else
                    {
                        if (m_yTravelling != yCentreTravelling)
                        {
                            //on fait un travelling pour approcher la position
                            m_xTravelling = (Math.Abs(m_xTravelling - xCentreTravelling) < m_largeur / RATIO_TRAVELLING) ? xCentreTravelling : m_xTravelling + Math.Sign(xCentreTravelling - m_xTravelling) * m_largeur / RATIO_TRAVELLING;
                        }
                        if (m_yTravelling != yCentreTravelling)
                        {
                            //on fait un travelling pour approcher la position
                            m_yTravelling = (Math.Abs(m_yTravelling - yCentreTravelling) < m_hauteur / RATIO_TRAVELLING) ? yCentreTravelling : m_yTravelling + Math.Sign(yCentreTravelling - m_yTravelling) * m_hauteur / RATIO_TRAVELLING;
                        }
                    }
                    //Bitmap imageVideo = new Bitmap(m_largeur, m_hauteur, fichierImageSource.PixelFormat);
                    //Graphics graph = Graphics.FromImage(imageVideo);

                    G.DrawImage(fichierImageSource, m_largeurCote, 0, new Rectangle(m_xTravelling, m_yTravelling, m_largeur, m_hauteur), GraphicsUnit.Pixel);
                    //imageVideo.Save(m_repertoireVideo + "\\" + "test.png", ImageFormat.Png);
                    //G.DrawImageUnscaledAndClipped(imageVideo, new Rectangle(0,0, m_largeur, m_hauteur));
                    //graph.Dispose();
                    //imageVideo.Dispose();
                }
                else
                {
                    G.DrawImage(fichierImageSource, m_largeurCote, 0, m_largeur, m_hauteur);
                }

                if (m_bHistoriqueBataille)
                {
                    Pen styloExterieur = new Pen(Color.Black, 4);
                    Pen styloInterieur = new Pen(Color.White, 1);
                    //on ajoute les batailles s'il y en a
                    foreach (LieuRemarquable ligneLieu in m_lieuxRemarquables)
                    {
                        if (m_traitement >= ligneLieu.iTourDebut && m_traitement <= ligneLieu.iTourFin)
                        {
                            G.DrawRectangle(styloExterieur,
                                m_largeurCote + (ligneLieu.i_X_CASE_HAUT_GAUCHE - m_xTravelling) * m_rapport,
                                (ligneLieu.i_Y_CASE_HAUT_GAUCHE - m_yTravelling) * m_rapport,
                                (ligneLieu.i_X_CASE_BAS_DROITE - ligneLieu.i_X_CASE_HAUT_GAUCHE) * m_rapport,
                                (ligneLieu.i_Y_CASE_BAS_DROITE - ligneLieu.i_Y_CASE_HAUT_GAUCHE) * m_rapport);

                            G.DrawRectangle(styloInterieur,
                                m_largeurCote + (ligneLieu.i_X_CASE_HAUT_GAUCHE - m_xTravelling) * m_rapport,
                                (ligneLieu.i_Y_CASE_HAUT_GAUCHE - m_yTravelling) * m_rapport,
                                (ligneLieu.i_X_CASE_BAS_DROITE - ligneLieu.i_X_CASE_HAUT_GAUCHE) * m_rapport,
                                (ligneLieu.i_Y_CASE_BAS_DROITE - ligneLieu.i_Y_CASE_HAUT_GAUCHE) * m_rapport);
                        }
                    }

                    //on ajoute les unités
                    foreach (UniteRemarquable unite in m_unitesRemarquables)
                    {
                        if (unite.iTour != m_traitement)
                        {
                            continue;
                        }
                        DessineUnite(G, unite, m_xTravelling, m_yTravelling);
                    }
                }
                //G.DrawImageUnscaled(fichierImageSource, 0, 0);
                if (m_bFilm)
                {
                    fichierImage.Save(m_repertoireVideo + "\\test.png", ImageFormat.Png);
                    fichierImage.RotateFlip(RotateFlipType.RotateNoneFlipY);//il faut retourner l'image sinon, elle apparait inversée dans la vidéo
                    BitmapData bmpDat = fichierImage.LockBits(
                        new Rectangle(0, 0, m_largeur + 2 * m_largeurCote, m_hauteur + m_hauteurBandeau), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

                    //BitmapData imageCible = new BitmapData();
                    //m_imageCarte.LockBits(rect, ImageLockMode.ReadOnly, m_imageCarte.PixelFormat, imageCible);
                    //m_imageCarte.UnlockBits(imageCible);
                    //Bitmap imageFinale = new Bitmap(imageCible.Width, imageCible.Height, imageCible.Stride, imageCible.PixelFormat, imageCible.Scan0);
                    //imageFinale.Save(nomFichierFinal);
                    m_aw.AddFrame(bmpDat);
                    fichierImage.UnlockBits(bmpDat);
                }
                else
                {
                    fichierImage.Save(m_repertoireVideo + "\\" + "imageVideo_" + m_traitement.ToString("0000") + ".png", ImageFormat.Png);
                }

                G.Dispose();
                fichierImage.Dispose();
                fichierImageSource.Dispose();
                m_traitement++;
                m_travailleur.ReportProgress(m_traitement * 100 / m_nbImages);
                //m_travailleur.ReportProgress(m_traitement * 100 / m_listeFichiers.Length);
                //if (m_traitement == m_listeFichiers.Length)
                if (m_traitement == m_nbImages)
                {
                    return "film crée";
                }
                return string.Empty;
            }
            catch (AviWriter.AviException e)
            {
                return "AVI Exception in: " + e.ToString();
            }
        }

        private void DessineUnite(Graphics G, UniteRemarquable unite, int xTravelling, int yTravelling)
        {
            //si on est pas dans le cadre, inutile de continuer
            if (m_bTravelling)
            {
                if (
                       ((unite.i_X_CASE - xTravelling) * m_rapport - m_tailleUnite / 2) < 0
                    || ((unite.i_X_CASE - xTravelling) * m_rapport + m_tailleUnite / 2) > m_largeur
                    || ((unite.i_Y_CASE - yTravelling) * m_rapport - m_tailleUnite / 2) < 0
                    || ((unite.i_Y_CASE - yTravelling) * m_rapport + m_tailleUnite / 2) > m_hauteur
                    )
                {
                    return;
                }
            }
            //DessineUniteFilaire(G, unite, xTravelling, yTravelling);
            //Autre = villes, hopitaux, etc.
            if (unite.tipe != TIPEUNITEVIDEO.AUTRE)
            {
                DessineUniteImage(G, unite, xTravelling, yTravelling);
            }
        }

        private void DessineUniteImage(Graphics G, UniteRemarquable unite, int xTravelling, int yTravelling)
        {
            Image image;
            if (unite.b_blesse)
            {
                image = (0 == unite.iNation) ? new Bitmap(vaoc.Properties.Resources.blesse_0) : new Bitmap(vaoc.Properties.Resources.blesse_1);
            }
            else
            {
                if (unite.b_prisonnier)
                {
                    image = (0 == unite.iNation) ? new Bitmap(vaoc.Properties.Resources.prisonnier_0) : new Bitmap(vaoc.Properties.Resources.prisonnier_1);
                }
                else
                {
                    switch (unite.tipe)
                    {
                        case TIPEUNITEVIDEO.INFANTERIE:
                            //barre haut gauche, bas droite
                            image = (0 == unite.iNation) ? new Bitmap(vaoc.Properties.Resources.infanterie_0) : new Bitmap(vaoc.Properties.Resources.infanterie_1);
                            break;
                        case TIPEUNITEVIDEO.CAVALERIE:
                            //barre haut gauche, bas droite
                            image = (0 == unite.iNation) ? new Bitmap(vaoc.Properties.Resources.cavalerie_0) : new Bitmap(vaoc.Properties.Resources.cavalerie_1);
                            break;
                        case TIPEUNITEVIDEO.ARTILLERIE:
                            image = (0 == unite.iNation) ? new Bitmap(vaoc.Properties.Resources.artillerie_0) : new Bitmap(vaoc.Properties.Resources.artillerie_1);
                            break;
                        case TIPEUNITEVIDEO.CONVOI:
                            image = (0 == unite.iNation) ? new Bitmap(vaoc.Properties.Resources.convoi_0) : new Bitmap(vaoc.Properties.Resources.convoi_1);
                            break;
                        case TIPEUNITEVIDEO.DEPOT:
                            image = (0 == unite.iNation) ? new Bitmap(vaoc.Properties.Resources.depot_0) : new Bitmap(vaoc.Properties.Resources.depot_1);
                            break;
                        case TIPEUNITEVIDEO.PONTONNIER:
                            image = (0 == unite.iNation) ? new Bitmap(vaoc.Properties.Resources.genie_0) : new Bitmap(vaoc.Properties.Resources.genie_1);
                            break;
                        case TIPEUNITEVIDEO.QG:
                            image = (0 == unite.iNation) ? new Bitmap(vaoc.Properties.Resources.qg_0) : new Bitmap(vaoc.Properties.Resources.qg_1);
                            break;
                        case TIPEUNITEVIDEO.PRISONNIER:
                            image = (0 == unite.iNation) ? new Bitmap(vaoc.Properties.Resources.prisonnier_0) : new Bitmap(vaoc.Properties.Resources.prisonnier_1);
                            break;
                        case TIPEUNITEVIDEO.BLESSE:
                            image = (0 == unite.iNation) ? new Bitmap(vaoc.Properties.Resources.blesse_0) : new Bitmap(vaoc.Properties.Resources.blesse_1);
                            break;
                        default:
                            image = new Bitmap(vaoc.Properties.Resources.zoomMoins);
                            break;
                    }
                }
            }
            G.DrawImage(image,
                        (unite.i_X_CASE - xTravelling) * m_rapport - image.Width/2, 
                        (unite.i_Y_CASE - yTravelling) * m_rapport - image.Height/2);
        }

        private void DessineUniteFilaire(Graphics G, UniteRemarquable unite, int xTravelling, int yTravelling)
        {
            Pen styloUnite = new Pen((unite.iNation == 0) ? Color.Blue : Color.Red, m_epaisseurUnite);
            Brush brosseUnite = new SolidBrush((unite.iNation == 0) ? Color.Blue : Color.Red);
            switch (unite.tipe)
            {
                case TIPEUNITEVIDEO.INFANTERIE:
                    //barre haut gauche, bas droite
                    G.DrawLine(styloUnite,
                        m_largeurCote + (unite.i_X_CASE - xTravelling) * m_rapport - m_tailleUnite / 2,
                        (unite.i_Y_CASE - yTravelling) * m_rapport - m_tailleUnite / 2,
                        m_largeurCote + (unite.i_X_CASE - xTravelling) * m_rapport + m_tailleUnite / 2,
                        (unite.i_Y_CASE - yTravelling) * m_rapport + m_tailleUnite / 2
                        );
                    //barre haut droite , bas gauche
                    G.DrawLine(styloUnite,
                        m_largeurCote + (unite.i_X_CASE - xTravelling) * m_rapport + m_tailleUnite / 2,
                        (unite.i_Y_CASE - yTravelling) * m_rapport - m_tailleUnite / 2,
                        m_largeurCote + (unite.i_X_CASE - xTravelling) * m_rapport - m_tailleUnite / 2,
                        (unite.i_Y_CASE - yTravelling) * m_rapport + m_tailleUnite / 2
                        );
                    //finir par le cadre pour éviter des problèmes de points de fin de ligne
                    G.DrawRectangle(styloUnite,
                        m_largeurCote + (unite.i_X_CASE - xTravelling) * m_rapport - m_tailleUnite / 2,
                        (unite.i_Y_CASE - yTravelling) * m_rapport - m_tailleUnite / 2,
                        m_tailleUnite,
                        m_tailleUnite);
                    break;
                case TIPEUNITEVIDEO.CAVALERIE:
                    //barre haut gauche, bas droite
                    G.DrawLine(styloUnite,
                        m_largeurCote + (unite.i_X_CASE - xTravelling) * m_rapport - m_tailleUnite / 2,
                        (unite.i_Y_CASE - yTravelling) * m_rapport - m_tailleUnite / 2,
                        m_largeurCote + (unite.i_X_CASE - xTravelling) * m_rapport + m_tailleUnite / 2,
                        (unite.i_Y_CASE - yTravelling) * m_rapport + m_tailleUnite / 2
                        );
                    //finir par le cadre pour éviter des problèmes de points de fin de ligne
                    G.DrawRectangle(styloUnite,
                        m_largeurCote + (unite.i_X_CASE - xTravelling) * m_rapport - m_tailleUnite / 2,
                        (unite.i_Y_CASE - yTravelling) * m_rapport - m_tailleUnite / 2,
                        m_tailleUnite,
                        m_tailleUnite);
                    break;
                case TIPEUNITEVIDEO.ARTILLERIE:
                    G.FillEllipse(brosseUnite,
                        m_largeurCote + (unite.i_X_CASE - xTravelling) * m_rapport - m_tailleUnite / 4,
                        (unite.i_Y_CASE - yTravelling) * m_rapport - m_tailleUnite / 4,
                        m_tailleUnite / 2,
                        m_tailleUnite / 2);
                    //finir par le cadre pour éviter des problèmes de points de fin de ligne
                    G.DrawRectangle(styloUnite,
                        m_largeurCote + (unite.i_X_CASE - xTravelling) * m_rapport - m_tailleUnite / 2,
                        (unite.i_Y_CASE - yTravelling) * m_rapport - m_tailleUnite / 2,
                        m_tailleUnite,
                        m_tailleUnite);
                    break;
                case TIPEUNITEVIDEO.CONVOI:
                    G.DrawEllipse(styloUnite,
                        m_largeurCote + (unite.i_X_CASE - xTravelling) * m_rapport - m_tailleUnite / 2,
                        (unite.i_Y_CASE - yTravelling) * m_rapport - m_tailleUnite / 2,
                        m_tailleUnite,
                        m_tailleUnite);
                    G.FillPie(brosseUnite,
                        m_largeurCote + (unite.i_X_CASE - xTravelling) * m_rapport - m_tailleUnite / 2,
                        (unite.i_Y_CASE - yTravelling) * m_rapport - m_tailleUnite / 2,
                        m_tailleUnite,
                        m_tailleUnite,
                        0, 180);
                    break;
                case TIPEUNITEVIDEO.DEPOT:
                    G.FillEllipse(brosseUnite,
                        m_largeurCote + (unite.i_X_CASE - xTravelling) * m_rapport - m_tailleUnite / 2,
                        (unite.i_Y_CASE - yTravelling) * m_rapport - m_tailleUnite / 2,
                        m_tailleUnite,
                        m_tailleUnite);
                    break;
                case TIPEUNITEVIDEO.PONTONNIER:
                    break;
                default:
                    //carre vide
                    G.DrawRectangle(styloUnite,
                        m_largeurCote + (unite.i_X_CASE - xTravelling) * m_rapport - m_tailleUnite / 2,
                        (unite.i_Y_CASE - yTravelling) * m_rapport - m_tailleUnite / 2,
                        m_tailleUnite,
                        m_tailleUnite);
                    break;
            }
        }
        /*
public string CreerFilm(string repertoireImages, string repertoireVideo, Font police, string texteMasqueImage, string[] texteImages
                       ,int largeurOptimale, int HauteurOptimale, bool bHistoriqueBataille, List<LieuRemarquable> lieuxRemarquables)
{
   try
   {
       SizeF tailleTexte;
       Graphics G;
       Bitmap fichierImageSource;
       int w = int.MaxValue;
       int h = int.MaxValue;
       int hmax = 0;
       int wmax = 0;
       int hauteurBandeau = 0;
       float rapport;

       //recherche le nombre d'images et leur taille
       DirectoryInfo dir = new DirectoryInfo(repertoireImages);
       m_listeFichiers = dir.GetFiles(texteMasqueImage,SearchOption.TopDirectoryOnly);
       if (0 == m_listeFichiers.Length) {return "le repertoire source ne contient aucune image "+texteMasqueImage;}

       Array.Sort(m_listeFichiers, new MyCustomComparer());//tri par nom
       foreach (FileInfo fichier in m_listeFichiers)
       {
           Bitmap fichierImage = (Bitmap)Image.FromFile(fichier.FullName);
           if (wmax < fichierImage.Width) { wmax = fichierImage.Width; }
           if (hmax < fichierImage.Height) { hmax = fichierImage.Height; }
           if (w > fichierImage.Width) { w = fichierImage.Width; }
           if (h > fichierImage.Height) { h = fichierImage.Height; }
       }

       if (wmax != w || hmax != h)
       {
           return string.Format("Toutes les images n'ont pas la même taille, celles-ci vont de ({0},{1}) à ({2},{3}). Le traitement ne peut être effectué", h,w,hmax,wmax);
       }

       //calcul de la hauteur du bandeau = 2 fois la hauteur de la police
       if (null != texteImages && texteImages.Length > 0)
       {
           fichierImageSource = (Bitmap)Image.FromFile(m_listeFichiers[0].FullName);
           G = Graphics.FromImage(fichierImageSource);
           tailleTexte = G.MeasureString("XX", police);
           hauteurBandeau = (int)(tailleTexte.Height * 1);
           fichierImageSource.Dispose();
       }

       //calcul de la taille optimale
       if ((float)w / largeurOptimale > (float)h / (HauteurOptimale - hauteurBandeau))
       {
           //on se cale donc sur la largeur (effort le plus grand)
           rapport = (float)largeurOptimale / w;
           h = (int)(h * rapport);
           w = largeurOptimale;
       }
       else
       {
           rapport = (float)(HauteurOptimale - hauteurBandeau) / h;
           w = (int)(w * rapport);
           h = HauteurOptimale - hauteurBandeau;
       }

       m_aw = new AviWriter();
       Bitmap bmp = m_aw.Open(repertoireVideo + "\\" + "video.avi", 1, w, h + hauteurBandeau);

       m_traitement = 0;
       foreach (FileInfo fichier in m_listeFichiers)
       {
           fichierImageSource = (Bitmap)Image.FromFile(fichier.FullName);
           Bitmap fichierImage = new Bitmap(w, h + hauteurBandeau, fichierImageSource.PixelFormat);
           G = Graphics.FromImage(fichierImage);
           G.PageUnit = GraphicsUnit.Pixel;
           //bandeau avec texte
           if (null != texteImages && texteImages.Length > 0)
           {
               G.FillRectangle(Brushes.White, new Rectangle(0, h, w, hauteurBandeau));
               tailleTexte = G.MeasureString(texteImages[m_traitement], police);
               G.DrawString(texteImages[m_traitement], police, Brushes.Black, new Rectangle((w - (int)tailleTexte.Width) / 2, h + (hauteurBandeau - (int)tailleTexte.Height) / 2,
                   w - (w - (int)tailleTexte.Width) / 2, hauteurBandeau - (hauteurBandeau - (int)tailleTexte.Height) / 2));
               m_traitement++;
           }

           //image de base
           G.DrawImage(fichierImageSource, 0, 0, w, h);

           if (bHistoriqueBataille)
           {
               Pen styloExterieur = new Pen(Color.Black, 3);
               Pen styloInterieur = new Pen(Color.White, 1);
               //on ajoute les batailles s'il y en a
               foreach (LieuRemarquable ligneLieu in lieuxRemarquables)
               {
                   if (m_traitement >= ligneLieu.iTourDebut && m_traitement <= ligneLieu.iTourFin)
                   {
                       G.DrawRectangle(styloExterieur,
                           ligneLieu.i_X_CASE_HAUT_GAUCHE * rapport,
                           ligneLieu.i_Y_CASE_HAUT_GAUCHE * rapport,
                           (ligneLieu.i_X_CASE_BAS_DROITE - ligneLieu.i_X_CASE_HAUT_GAUCHE) * rapport,
                           (ligneLieu.i_Y_CASE_BAS_DROITE - ligneLieu.i_Y_CASE_HAUT_GAUCHE) * rapport);

                       G.DrawRectangle(styloInterieur,
                           ligneLieu.i_X_CASE_HAUT_GAUCHE * rapport,
                           ligneLieu.i_Y_CASE_HAUT_GAUCHE * rapport,
                           (ligneLieu.i_X_CASE_BAS_DROITE - ligneLieu.i_X_CASE_HAUT_GAUCHE) * rapport,
                           (ligneLieu.i_Y_CASE_BAS_DROITE - ligneLieu.i_Y_CASE_HAUT_GAUCHE) * rapport);
                   }
               }
           }
           //G.DrawImageUnscaled(fichierImageSource, 0, 0);
           fichierImage.Save(repertoireVideo+"\\test.png", ImageFormat.Png);
           fichierImage.RotateFlip(RotateFlipType.RotateNoneFlipY);//il faut retourner l'image sinon, elle apparait inversée dans la vidéo
           BitmapData bmpDat = fichierImage.LockBits(
             new Rectangle(0, 0, w, h + hauteurBandeau), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);


           //BitmapData imageCible = new BitmapData();
           //m_imageCarte.LockBits(rect, ImageLockMode.ReadOnly, m_imageCarte.PixelFormat, imageCible);
           //m_imageCarte.UnlockBits(imageCible);
           //Bitmap imageFinale = new Bitmap(imageCible.Width, imageCible.Height, imageCible.Stride, imageCible.PixelFormat, imageCible.Scan0);
           //imageFinale.Save(nomFichierFinal);
           m_aw.AddFrame(bmpDat);
           fichierImage.UnlockBits(bmpDat);
           G.Dispose();
           fichierImage.Dispose();
           fichierImageSource.Dispose();
       }
       m_aw.Close();
       return string.Empty;
   }
   catch (AviWriter.AviException e)
   {
       return "AVI Exception in: " + e.ToString();
   }
}
*/
        public string ChaineFichier(string source)
        {
            byte[] tempBytes;
            tempBytes = System.Text.Encoding.GetEncoding("ISO-8859-8").GetBytes(source);
            string asciiStr = System.Text.Encoding.UTF8.GetString(tempBytes);
            return asciiStr.Replace(" ", "_").Replace("'","").ToUpper();
        }
    }
}
