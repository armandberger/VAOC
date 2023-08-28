using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;
using System.Linq;
using WaocLib;

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
        public int iEffectif;
        public int ID;
        public int ID_ROLE;//ID du role proprietaire du pion
        public bool bInclusDansLeCorps;//true s'il est dans la zone du corps pour un affichage corps, false sinon
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
        public int i_X_CASE_CORPS;
        public int i_Y_CASE_CORPS;
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

    public class FabricantDeFilm
    {
        private FileInfo[] m_listeFichiers;
        private int m_traitement;//traitement principal
        private int m_traitementRole;//traitement secondaire si un fichier vidéo par role
        private string m_repertoireVideo;
        private float m_rapport;
        private List<LieuRemarquable> m_lieuxRemarquables;
        private List<EffectifEtVictoire> m_effectifsEtVictoires;
        private List<UniteRemarquable> m_unitesRemarquables;
        private List<UniteRole> m_unitesRoles;
        private Donnees.TAB_BATAILLERow[] m_batailles;
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
        private int m_hauteurCorps;
        private int m_largeurCorps;
        private const int CST_DEBUT_FILM = 1;//on ne prende pas le premier tour c'est un tour de discussion, les unités sont téléportées ensuite
        private const int CST_TAILLE_NOM_CORPS = 4;
        private const int CST_RAYON_CORPS = 4;//rayon autour de la taille du corps qui inclue les unités
        private const int CST_DESSIN_ALPHA = 100;
        private int m_numeroImage;
        private string m_nomCampagne;
        private string m_nomFichier;

        System.ComponentModel.BackgroundWorker m_travailleur;
        private const int BARRE_ECART = 2;
        private const int BARRE_EPAISSEUR = 3;
        private const int RATIO_TRAVELLING = 8;
        private int m_tailleUnite;// Laisser une valeur paire ou cela créer des problèmes d'arrondi
        private int m_epaisseurUnite;//largeur des traits des unites;
        //private int m_minX, m_minY, m_maxX, m_maxY;//position extremes des unités sur la carte
        //private BaseVideo m_baseVideo = new BaseVideo();
        private int m_xTravelling = -1;
        private int m_yTravelling = -1;
        private int m_effectifsMax = 0;
        private bool m_videoParRole = false;
        private bool m_affichageCorps = false;
        private bool m_affichageDepots = true;
        private int m_effectifsRoleMoyen;
        private Dictionary<int, SolidBrush> m_couleursCorps;
        private readonly Color[,] couleursNations = new Color[2, 19]
        {   { Color.Blue, Color.LightBlue, Color.DarkBlue, Color.AliceBlue, Color.Aquamarine, 
                Color.BlueViolet, Color.Aqua, Color.CornflowerBlue, Color.Cyan, Color.DarkSlateBlue,
                Color.DeepSkyBlue, Color.DodgerBlue, Color.Lavender, Color.LightSteelBlue, Color.Indigo,
            Color.MediumBlue, Color.DarkSeaGreen, Color.CadetBlue, Color.LightSkyBlue},
            { Color.Red, Color.DarkRed, Color.DarkOrange, Color.Firebrick, Color.DeepPink,
                Color.IndianRed, Color.Orange, Color.OrangeRed, Color.Magenta, Color.MediumVioletRed,
                Color.MistyRose, Color.PaleVioletRed, Color.Peru, Color.DeepPink, Color.Tomato,
            Color.Yellow, Color.LightGoldenrodYellow, Color.YellowGreen, Color.LightYellow},
        };
        // commande dans un .bat ffmpeg -framerate 1 -i imageVideo_%%04d.png -c:v libx264 -r 30 -pix_fmt yuv420p video.mp4

        public FabricantDeFilm()
        {
        }

        public string Initialisation(string repertoireImages, string repertoireVideo, Font police, string texteMasqueImage, 
                                    string[] texteImages, int largeurOptimale, int HauteurOptimale, int tailleUnite, int epaisseurUnite,
                                    bool bTravelling, bool bvideoParRole, bool baffichageCorps, bool baffichageDepots,
                                    List<LieuRemarquable> lieuxRemarquables, List<UniteRemarquable> unitesRemarquables, 
                                    List<EffectifEtVictoire> effectifsEtVictoires, List<UniteRole> unitesRoles,
                                    Donnees.TAB_BATAILLERow[] batailles,
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
                m_numeroImage = 0;
                m_nomCampagne = Donnees.m_donnees.TAB_JEU[0].S_NOM.Replace(" ", "");
                m_nomFichier = "video";
                m_largeur = int.MaxValue;
                m_hauteur = int.MaxValue;
                m_hauteurMax = 0;
                m_largeurMax = 0;
                m_repertoireVideo = repertoireVideo;
                m_lieuxRemarquables = lieuxRemarquables;
                m_unitesRemarquables = unitesRemarquables;
                m_batailles = batailles;
                m_effectifsEtVictoires = effectifsEtVictoires;
                m_unitesRoles = unitesRoles;
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
                m_videoParRole = bvideoParRole;
                m_affichageCorps = baffichageCorps;
                m_affichageDepots = baffichageDepots;
                if (m_videoParRole)
                {
                    m_unitesRoles = unitesRoles;
                    m_roles = new Dictionary<int, Role>();
                    m_travelling = new Dictionary<int, Travelling>();
                    foreach (UniteRole roleUnite in m_unitesRoles)
                    {
                        if (!m_roles.ContainsKey(roleUnite.ID_ROLE)) 
                        {
                            Role role = new Role
                            {
                                ID_ROLE = roleUnite.ID_ROLE,
                                iNation = roleUnite.iNation,
                                nom = roleUnite.nom,
                                iEffectifMax = roleUnite.iEffectif
                            };
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

                //if (!m_bCarteGlobale && !m_bTravelling)
                //{
                //    //on cherche la taille d'affichage par rapport aux positions extremes des unités sur la carte
                //    m_minX = m_minY = int.MaxValue;
                //    m_maxX = m_maxY = int.MinValue;
                //    foreach(Donnees.TAB_VIDEORow ligneVideo in Donnees.m_donnees.TAB_VIDEO)
                //    {
                //        if (ligneVideo.ID_CASE<0 || (0==ligneVideo.EffectifTotal)) { continue; }//ca peut arriver visiblement...
                //        //Donnees.TAB_CASERow ligneCase = Donnees.m_donnees.TAB_CASE.FindParID_CASE(ligneVideo.ID_CASE);
                //        int x, y;
                //        Donnees.m_donnees.TAB_CASE.ID_CASE_Vers_XY(ligneVideo.ID_CASE, out x, out y);
                //        m_minX = Math.Min(m_minX, x);
                //        m_minY = Math.Min(m_minY, y);
                //        m_maxX = Math.Max(m_maxX, x);
                //        m_maxY = Math.Max(m_maxY, y);
                //    }
                //}

                //calcul de la hauteur du bandeau et de la largeur du bandeau (au cas où cela dépassererait la largeur min)
                fichierImageSource = (Bitmap)Image.FromFile(m_listeFichiers[0].FullName);
                G = Graphics.FromImage(fichierImageSource);
                if (null != texteImages && texteImages.Length > 0)
                {
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

                //fmpeg n'accepte pas les tailles impairs, donc on ajoute 1 si c'est le cas
                if (m_hauteur%2==1)
                {
                    m_hauteur++;
                }
                if (m_largeur % 2 == 1)
                {
                    m_largeur++;
                }

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

                m_hauteurCorps = -1;
                m_largeurCorps = -1;
                if (m_affichageCorps)
                {
                    m_couleursCorps = new Dictionary<int, SolidBrush>();
                    int c0 = 0;
                    int c1 = 0;
                    //calcul des hauteurs et largeur pour les corps, déterminera ensuite l'espacement d'inclusion des unités dans un corps pour l'affichage
                    m_effectifsRoleMoyen = 0;
                    foreach (UniteRole role in m_unitesRoles)
                    {
                        tailleTexte = G.MeasureString(role.nom.Substring(0, Math.Min(CST_TAILLE_NOM_CORPS, role.nom.Length)), m_police);
                        m_hauteurCorps = Math.Max(m_hauteurCorps, (int)tailleTexte.Height);
                        m_largeurCorps = Math.Max(m_largeurCorps, (int)tailleTexte.Width);
                        m_effectifsRoleMoyen += role.iEffectif;

                        // affectation des couleurs par corps
                        if (!m_couleursCorps.ContainsKey(role.ID_ROLE))
                        {
                            
                            if (0 == role.iNation)
                            {
                                //m_couleursCorps.Add(role.ID_ROLE, new SolidBrush(Color.FromArgb(20, 5*c0, 0, 255-10*c0)));
                                m_couleursCorps.Add(role.ID_ROLE, new SolidBrush(Color.FromArgb(CST_DESSIN_ALPHA, couleursNations[role.iNation, c0++])));
                            }
                            else
                            {
                                m_couleursCorps.Add(role.ID_ROLE, new SolidBrush(Color.FromArgb(CST_DESSIN_ALPHA, couleursNations[role.iNation, c1++])));
                            }
                        }
                    }
                    m_hauteurCorps += 2*m_epaisseurUnite;
                    m_largeurCorps += 2*m_epaisseurUnite;
                    m_effectifsRoleMoyen /= m_unitesRoles.Count;
                    CalculPositionCorps();

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
            catch (Exception e)
            {
                return "Initialisation - Exception in: " + e.ToString();
            }
        }

        public string Terminer()
        {
            try
            {
                //if (m_bTravelling) -> je ne comprends pas le test, on fait une vidéo dans tous les cas...
                {
                    //on lance la commande DOS de création du film
                    //string YourApplicationPath = m_repertoireVideo + "\\ffmpeg.exe";
                    if (m_videoParRole)
                    {
                        foreach (Role role in m_roles.Values)
                        {
                            FilmMpeg(ChaineFichier(Constantes.MinusculeSansAccents(role.nom).Replace(" ", "_").Replace("'", "_")));
                        }
                    }
                    else
                    {
                        FilmMpeg(m_nomCampagne.Replace(' ', '_') + "_" + m_nomFichier);
                    }
                }
                m_travailleur.ReportProgress(100);
                return string.Empty;
            }
            catch (Exception e)
            {
                return "Terminer - Exception in: " + e.ToString();
            }
        }

        private void FilmMpeg(string nom)
        {
            ProcessStartInfo processInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Normal,
                FileName = m_repertoireVideo+"\\ffmpeg.exe",
                WorkingDirectory = m_repertoireVideo, //Path.GetDirectoryName(YourApplicationPath);
                Arguments = string.Format("-framerate 1 -i {0}_%04d.png -c:v libx264 -r 30 -pix_fmt yuv420p {0}.mp4", nom)
            };
            Process.Start(processInfo);
        }

        public string Traitement()
        {
            SizeF tailleTexte;
            Graphics G;
            Bitmap fichierImageSource;
            UniteRemarquable uniteQG = new UniteRemarquable
            {
                tipe = TIPEUNITEVIDEO.QG
            };

            try
            {
                if (null != m_batailles[m_traitement] && !m_videoParRole)
                {
                    TraitementBataille(m_batailles[m_traitement]);
                    m_traitement += m_batailles[m_traitement].I_TOUR_FIN - m_batailles[m_traitement].I_TOUR_DEBUT;
                }
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
                        TraitementVictoireVideoParRole(G, rectBas, largeurGraphEffectifs);
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
                    TraitementEffectifsVideoParRole(G, rectBas);
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

                if (m_affichageCorps)
                {
                    // afficher toutes les unités seules puis tous les chefs de corps puis les batailles.
                    foreach (UniteRemarquable unite in m_unitesRemarquables)
                    {
                        if (unite.iTour == m_traitement &&
                            (unite.tipe != TIPEUNITEVIDEO.INFANTERIE && unite.tipe != TIPEUNITEVIDEO.CAVALERIE &&
                            unite.tipe != TIPEUNITEVIDEO.ARTILLERIE && unite.tipe != TIPEUNITEVIDEO.QG))
                        {
                            DessineUniteFilaire(G, unite, m_xTravelling, m_yTravelling);
                        }
                    }
                    foreach (UniteRemarquable unite in m_unitesRemarquables)
                    {
                        if (unite.iTour == m_traitement &&
                            !unite.bInclusDansLeCorps &&
                            (unite.tipe == TIPEUNITEVIDEO.INFANTERIE || unite.tipe == TIPEUNITEVIDEO.CAVALERIE ||
                            unite.tipe == TIPEUNITEVIDEO.ARTILLERIE || unite.tipe == TIPEUNITEVIDEO.QG))
                        {
                            DessineUniteFilaire(G, unite, m_xTravelling, m_yTravelling);
                        }
                    }
                    //Le leader à la fin
                    if (m_bTravelling && m_videoParRole)
                    {
                        DessineUnite(G, uniteQG, m_xTravelling, m_yTravelling);
                    }
                    foreach (UniteRole role in m_unitesRoles)
                    {
                        //if (m_traitement >= 285 && role.ID_ROLE == 0 && m_traitement == role.iTour)
                        //{
                        //    int debug = 0;
                        //}
                        if (role.iTour != m_traitement || role.i_X_CASE_CORPS < 0 || role.i_Y_CASE_CORPS < 0)
                        {
                            continue;
                        }
                        //if (role.ID_ROLE==0)
                        DessineCorps(G, role, m_xTravelling, m_yTravelling);
                    }
                }
                else
                {
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

                    //Le leader à la fin
                    if (m_bTravelling && m_videoParRole)
                    {
                        DessineUnite(G, uniteQG, m_xTravelling, m_yTravelling);
                    }
                }
                //G.DrawImageUnscaled(fichierImageSource, 0, 0);

                //on ajoute les batailles s'il y en a
                foreach (LieuRemarquable ligneLieu in m_lieuxRemarquables)
                {
                    if (m_traitement >= ligneLieu.iTourDebut && m_traitement <= ligneLieu.iTourFin)
                    {
                        DessineBataille(G, ligneLieu, m_xTravelling, m_yTravelling);
                    }
                }

                if (m_videoParRole)
                {
                    fichierImage.Save(ChaineFichier(m_repertoireVideo + "\\" + Constantes.MinusculeSansAccents(m_roles[m_traitementRole].nom) + "_" + m_traitement.ToString("0000") + ".png"), ImageFormat.Png);
                }
                else
                {
                    SauvegardeImage(fichierImage);
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
            catch (Exception e)
            {
                return "Traitement Exception in: " + e.ToString();
            }
        }

        private void TraitementEffectifsVideoParRole(Graphics G, Rectangle rectAffichage)
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
                    SizeF tailleTexte = G.MeasureString(strEffectif, m_police);
                    Brush brosse = (0 == role.iNation) ? Brushes.Blue : Brushes.Red;
                    //affichage des effectifs
                    G.DrawString(strEffectif, m_police, brosse,
                        new Rectangle(BARRE_ECART + rectAffichage.X,
                                        rectAffichage.Y + (int)(tailleTexte.Height),
                                        (int)tailleTexte.Width + 1,
                                        (int)tailleTexte.Height + 1));
                    break;
                }
            }
        }

        private void TraitementVictoireVideoParRole(Graphics G, Rectangle rectAffichage, int largeurGraphEffectifs)
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
                        pointsEff[role.iTour].X = rectAffichage.X + m_largeurCote + (role.iTour * largeurGraphEffectifs / nbTours);
                        pointsEff[role.iTour].Y = rectAffichage.Y + rectAffichage.Height + BARRE_ECART - (role.iEffectif * (rectAffichage.Height - BARRE_ECART) / m_roles[m_traitementRole].iEffectifMax);
                    }
                }
                //on trace les lignes des effectifs
                styloEff = (0 == iNationRole) ? new Pen(Color.Blue, 3) : new Pen(Color.Red, 3);
                G.DrawLines(styloEff, pointsEff);
            }
        }

        private void TraitementBataille(Donnees.TAB_BATAILLERow ligneBataille)
        {
            //zoom progressif sur la zone de bataille
            int xzoom, yzoom, xbataille, ybataille, largeur, hauteur;
            FileInfo fichier = 1 == m_listeFichiers.Length ? m_listeFichiers[0] : m_listeFichiers[m_traitement];
            Bitmap fichierImageSource = (Bitmap)Image.FromFile(fichier.FullName);
            Bitmap fichierImage = new Bitmap(m_largeur, m_hauteur + m_hauteurBandeau, fichierImageSource.PixelFormat);
            Graphics G = Graphics.FromImage(fichierImage);
            G.PageUnit = GraphicsUnit.Pixel;

            largeur = m_largeur;
            hauteur = (m_hauteur + m_hauteurBandeau);
            xbataille = (int)(1 * (ligneBataille.I_X_CASE_HAUT_GAUCHE + (ligneBataille.I_X_CASE_BAS_DROITE - ligneBataille.I_X_CASE_HAUT_GAUCHE) / 2));
            ybataille = (int)(1 * (ligneBataille.I_Y_CASE_HAUT_GAUCHE + (ligneBataille.I_Y_CASE_BAS_DROITE - ligneBataille.I_Y_CASE_HAUT_GAUCHE) / 2));

            xzoom = Math.Max(0, xbataille - largeur);
            yzoom = Math.Max(0, ybataille - hauteur);
            G.DrawImage(fichierImageSource, new Rectangle(0, 0, largeur, hauteur), new Rectangle(xzoom, yzoom, largeur*2, hauteur*2), GraphicsUnit.Pixel);
            SauvegardeImage(fichierImage);

            xzoom = Math.Max(0, xbataille - largeur / 2);
            yzoom = Math.Max(0, ybataille - hauteur / 2);
            G.DrawImage(fichierImageSource, new Rectangle(0, 0, largeur, hauteur), new Rectangle(xzoom, yzoom, largeur, hauteur), GraphicsUnit.Pixel);
            SauvegardeImage(fichierImage);

            xzoom = Math.Max(0, xbataille - largeur / 4);
            yzoom = Math.Max(0, ybataille - hauteur / 4);
            G.DrawImage(fichierImageSource, new Rectangle(0, 0, largeur, hauteur), new Rectangle(xzoom, yzoom, largeur / 2, hauteur / 2), GraphicsUnit.Pixel);
            SauvegardeImage(fichierImage);

            xzoom = Math.Max(0, xbataille - largeur / 8);
            yzoom = Math.Max(0, ybataille - hauteur / 8);
            G.DrawImage(fichierImageSource, new Rectangle(0, 0, largeur, hauteur), new Rectangle(xzoom, yzoom, largeur / 4, hauteur / 4), GraphicsUnit.Pixel);
            SauvegardeImage(fichierImage);

            //insertion de la bataille
            ligneBataille.GenererFilm(m_repertoireVideo, m_nomFichier, m_repertoireVideo, ref m_numeroImage, m_hauteur, m_largeur, false);
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
                
                //G.DrawImageUnscaled(fichierImageSource, 0, 0);

                fichierImage.Save(m_repertoireVideo + "\\" + "imageVideo_" + m_traitement.ToString("0000") + ".png", ImageFormat.Png);

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
            catch (Exception e)
            {
                return "Traitement V1 Exception in: " + e.ToString();
            }
        }

        private bool EstDansLeCadre(int x, int y, int xTravelling, int yTravelling)
        {
            if (
                   ((x - xTravelling) * m_rapport - m_tailleUnite / 2) < 0
                || ((x - xTravelling) * m_rapport + m_tailleUnite / 2) > m_largeur
                || ((y - yTravelling) * m_rapport - m_tailleUnite / 2) < 0
                || ((y - yTravelling) * m_rapport + m_tailleUnite / 2) > m_hauteur
                )
            {
                return false;
            }
            return true;
        }

        private void DessineCorps(Graphics G, UniteRole role, int xTravelling, int yTravelling)
        {
            if (role.i_X_CASE_CORPS<0 || role.i_Y_CASE_CORPS<0) { return; }
            int xtravel = (xTravelling < 0) ? 0 : xTravelling;
            int ytravel = (yTravelling < 0) ? 0 : yTravelling;
            //si on est pas dans le cadre, inutile de continuer
            if (m_bTravelling)
            {
                if (!EstDansLeCadre(role.i_X_CASE_CORPS, role.i_Y_CASE_CORPS, xtravel, ytravel))
                {
                    return;
                }
            }
            Brush brosse = (0 == role.iNation) ? Brushes.Blue : Brushes.Red;
            decimal rapportTaille = (decimal)role.iEffectif/m_effectifsRoleMoyen;
            int largeur = (int)(m_largeurCorps * rapportTaille);
            int hauteur = (int)(m_hauteurCorps * rapportTaille);
            Rectangle rectCorps = new Rectangle(
                (int)Math.Max(0,((role.i_X_CASE_CORPS - xtravel) * m_rapport - largeur / 2)),
                (int)Math.Max(0, ((role.i_Y_CASE_CORPS - ytravel) * m_rapport - hauteur / 2)),
                (int)largeur + 1,
                (int)hauteur + 1);

            SolidBrush semiTransBrush = m_couleursCorps[role.ID_ROLE];
            G.FillRectangle(semiTransBrush, rectCorps);
            G.DrawRectangle(Pens.Black, rectCorps);
            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;
            string nomRoleAffiche = role.nom.Substring(0, Math.Min(CST_TAILLE_NOM_CORPS, role.nom.Length));
            SizeF tailleTexte = G.MeasureString(nomRoleAffiche, m_police);
            G.DrawString(nomRoleAffiche, m_police, brosse,
                (int)((role.i_X_CASE_CORPS - xtravel) * m_rapport) - tailleTexte.Width/2, 
                (int)((role.i_Y_CASE_CORPS - ytravel) * m_rapport) - tailleTexte.Height/2);
            //G.DrawString(role.nom.Substring(0, Math.Min(CST_TAILLE_NOM_CORPS, role.nom.Length)),  m_police, brosse, rectCorps, format);
        }

        private void DessineUnite(Graphics G, UniteRemarquable unite, int xTravelling, int yTravelling)
        {
            //si on est pas dans le cadre, inutile de continuer
            if (m_bTravelling)
            {
                if (!EstDansLeCadre(unite.i_X_CASE, unite.i_Y_CASE, xTravelling, yTravelling))
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
                    if (!m_affichageDepots && TIPEUNITEVIDEO.DEPOT==unite.tipe) { return; }
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

        private void DessineBataille(Graphics G, LieuRemarquable lieu, int xTravelling, int yTravelling)
        {
            int xtravel = (xTravelling < 0) ? 0 : xTravelling;
            int ytravel = (yTravelling < 0) ? 0 : yTravelling;

            if (m_bTravelling)
            {
                if (!EstDansLeCadre(lieu.i_X_CASE_HAUT_GAUCHE, lieu.i_Y_CASE_HAUT_GAUCHE, xtravel, ytravel)
                    || !EstDansLeCadre(lieu.i_X_CASE_BAS_DROITE, lieu.i_Y_CASE_BAS_DROITE, xtravel, ytravel))
                {
                    return;
                }
            }
            if (m_affichageCorps)
            {
                //dessin d'une "explosion" de la hauteur/largeur d'un corps sur une grille 8x8
                Pen stylo = new Pen(Color.Black, 3);
                PointF[] points = new PointF[24];
                int p = 0;
                int x = (int)(m_rapport * (lieu.i_X_CASE_BAS_DROITE + lieu.i_X_CASE_HAUT_GAUCHE) / 2 - m_largeurCorps / 2);
                int y = (int)(m_rapport * (lieu.i_Y_CASE_BAS_DROITE + lieu.i_Y_CASE_HAUT_GAUCHE) / 2 - m_hauteurCorps / 2);
                int lbord = m_largeurCorps / 8;
                int hbord = m_hauteurCorps / 8;
                points[p].X = x;
                points[p++].Y = y;
                points[p].X = x + 3 * lbord;
                points[p++].Y = y + 2 * hbord;
                points[p].X = x + 3 * lbord;
                points[p++].Y = y + 1 * hbord;
                points[p].X = x + 4 * lbord;//3
                points[p++].Y = y + 2 * hbord;
                points[p].X = x + 6 * lbord;//4
                points[p++].Y = y + 0 * hbord;
                points[p].X = x + 5 * lbord;//5
                points[p++].Y = y + 2 * hbord;
                points[p].X = x + 7 * lbord;//6
                points[p++].Y = y + 1 * hbord;
                points[p].X = x + 6 * lbord;//7
                points[p++].Y = y + 4 * hbord;
                points[p].X = x + 8 * lbord;//8
                points[p++].Y = y + 3 * hbord;
                points[p].X = x + 7 * lbord;//9
                points[p++].Y = y + 4 * hbord;
                points[p].X = x + 8 * lbord;//10
                points[p++].Y = y + 5 * hbord;
                points[p].X = x + 6 * lbord;//11
                points[p++].Y = y + 5 * hbord;
                points[p].X = x + 7 * lbord;//12
                points[p++].Y = y + 7 * hbord;
                points[p].X = x + 5 * lbord;//13
                points[p++].Y = y + 5 * hbord;
                points[p].X = x + 5 * lbord;//14
                points[p++].Y = y + 7 * hbord;
                points[p].X = x + 4 * lbord;//15
                points[p++].Y = y + 6 * hbord;
                points[p].X = x + 3 * lbord;//16
                points[p++].Y = y + 8 * hbord;
                points[p].X = x + 3 * lbord;//17
                points[p++].Y = y + 6 * hbord;
                points[p].X = x + 1 * lbord;//18
                points[p++].Y = y + 7 * hbord;
                points[p].X = x + 2 * lbord;//19
                points[p++].Y = y + 4 * hbord;
                points[p].X = x + 0 * lbord;//20
                points[p++].Y = y + 5 * hbord;
                points[p].X = x + 1 * lbord;//21
                points[p++].Y = y + 4 * hbord;
                points[p].X = x + 0 * lbord;//22
                points[p++].Y = y + 3 * hbord;
                points[p].X = x + 2 * lbord;//23
                points[p++].Y = y + 3 * hbord;

                G.DrawLines(stylo, points);
            }
            else
            {
                Pen styloExterieur = new Pen(Color.Black, 4);
                Pen styloInterieur = new Pen(Color.White, 1);
                G.DrawRectangle(styloExterieur,
                    (lieu.i_X_CASE_HAUT_GAUCHE - xtravel) * m_rapport,
                    (lieu.i_Y_CASE_HAUT_GAUCHE - ytravel) * m_rapport,
                    (lieu.i_X_CASE_BAS_DROITE - lieu.i_X_CASE_HAUT_GAUCHE) * m_rapport,
                    (lieu.i_Y_CASE_BAS_DROITE - lieu.i_Y_CASE_HAUT_GAUCHE) * m_rapport);

                G.DrawRectangle(styloInterieur,
                    (lieu.i_X_CASE_HAUT_GAUCHE - xtravel) * m_rapport,
                    (lieu.i_Y_CASE_HAUT_GAUCHE - ytravel) * m_rapport,
                    (lieu.i_X_CASE_BAS_DROITE - lieu.i_X_CASE_HAUT_GAUCHE) * m_rapport,
                    (lieu.i_Y_CASE_BAS_DROITE - lieu.i_Y_CASE_HAUT_GAUCHE) * m_rapport);
            }
        }

        private void DessineUniteFilaire(Graphics G, UniteRemarquable unite, int xTravelling, int yTravelling)
        {
            Pen styloUnite = new Pen((unite.iNation == 0) ? Color.Blue : Color.Red, m_epaisseurUnite);
            Brush brosseUnite = new SolidBrush((unite.iNation == 0) ? Color.Blue : Color.Red);
            SolidBrush brosseRectangleUnite = m_couleursCorps[unite.ID_ROLE];
            int xtravel = (xTravelling < 0) ? 0 : xTravelling;
            int ytravel = (yTravelling < 0) ? 0 : yTravelling;

            switch (unite.tipe)
            {
                case TIPEUNITEVIDEO.INFANTERIE:
                    DessineUniteFilaireInfanterie(G, unite, styloUnite, brosseRectangleUnite, xtravel, ytravel);
                    break;
                case TIPEUNITEVIDEO.CAVALERIE:
                    DessineUniteFilaireCavalerie(G, unite, styloUnite, brosseRectangleUnite, xtravel, ytravel);
                    break;
                case TIPEUNITEVIDEO.ARTILLERIE:
                    DessineUniteFilaireArtillerie(G, unite, styloUnite, brosseRectangleUnite, brosseUnite, xtravel, ytravel);
                    break;
                case TIPEUNITEVIDEO.CONVOI:
                    if (m_affichageDepots)
                    {
                        DessineUniteFilaireConvoi(G, unite, styloUnite, brosseRectangleUnite, brosseUnite, xtravel, ytravel);
                    }
                    break;
                case TIPEUNITEVIDEO.DEPOT:
                    if (m_affichageDepots)
                    {
                        DessineUniteFilaireDepot(G, unite, styloUnite, brosseRectangleUnite, brosseUnite, xtravel, ytravel);
                    }
                    break;
                case TIPEUNITEVIDEO.PONTONNIER:
                case TIPEUNITEVIDEO.BLESSE:
                case TIPEUNITEVIDEO.PRISONNIER:
                    break;
                default:
                    //carre vide
                    //DessineFondUniteFilaire(G, unite, brosseRectangleUnite, xtravel, ytravel);
                    break;
            }
            //pour debug, afficher l'identifiant de l'unité dans le cadre
            //if (m_affichageDepots || (TIPEUNITEVIDEO.DEPOT != unite.tipe && TIPEUNITEVIDEO.CONVOI != unite.tipe))
            //{
            //    Font policeDebug = new Font(FontFamily.GenericSansSerif, 7);
            //    G.DrawString(unite.ID.ToString(), policeDebug, Brushes.DarkGreen,
            //                (unite.i_X_CASE - xtravel) * m_rapport - m_tailleUnite / 2,
            //                (unite.i_Y_CASE - ytravel) * m_rapport - m_tailleUnite / 2);
            //}
        }
        void DessineFondUniteFilaire(Graphics G, UniteRemarquable unite, SolidBrush brosseRectangleUnite, int xtravel, int ytravel)
        {
            G.FillRectangle(brosseRectangleUnite,
                (unite.i_X_CASE - xtravel) * m_rapport - m_tailleUnite / 2,
                (unite.i_Y_CASE - ytravel) * m_rapport - m_tailleUnite / 2,
                m_tailleUnite,
                m_tailleUnite);
        }

        void DessineUniteFilaireInfanterie(Graphics G, UniteRemarquable unite, Pen styloUnite, SolidBrush brosseRectangleUnite, int xtravel, int ytravel)
        {
            DessineFondUniteFilaire(G, unite, brosseRectangleUnite, xtravel, ytravel);

            //barre haut gauche, bas droite
            G.DrawLine(styloUnite,
                (unite.i_X_CASE - xtravel) * m_rapport - m_tailleUnite / 2,
                (unite.i_Y_CASE - ytravel) * m_rapport - m_tailleUnite / 2,
                (unite.i_X_CASE - xtravel) * m_rapport + m_tailleUnite / 2,
                (unite.i_Y_CASE - ytravel) * m_rapport + m_tailleUnite / 2
                );
            //barre haut droite , bas gauche
            G.DrawLine(styloUnite,
                (unite.i_X_CASE - xtravel) * m_rapport + m_tailleUnite / 2,
                (unite.i_Y_CASE - ytravel) * m_rapport - m_tailleUnite / 2,
                (unite.i_X_CASE - xtravel) * m_rapport - m_tailleUnite / 2,
                (unite.i_Y_CASE - ytravel) * m_rapport + m_tailleUnite / 2
                );
            //finir par le cadre pour éviter des problèmes de points de fin de ligne
            G.DrawRectangle(styloUnite,
                (unite.i_X_CASE - xtravel) * m_rapport - m_tailleUnite / 2,
                (unite.i_Y_CASE - ytravel) * m_rapport - m_tailleUnite / 2,
                m_tailleUnite,
                m_tailleUnite);
        }

        void DessineUniteFilaireCavalerie(Graphics G, UniteRemarquable unite, Pen styloUnite, SolidBrush brosseRectangleUnite, int xtravel, int ytravel)
        {
            DessineFondUniteFilaire(G, unite, brosseRectangleUnite, xtravel, ytravel);

            //barre haut gauche, bas droite
            G.DrawLine(styloUnite,
                (unite.i_X_CASE - xtravel) * m_rapport - m_tailleUnite / 2,
                (unite.i_Y_CASE - ytravel) * m_rapport - m_tailleUnite / 2,
                (unite.i_X_CASE - xtravel) * m_rapport + m_tailleUnite / 2,
                (unite.i_Y_CASE - ytravel) * m_rapport + m_tailleUnite / 2
                );
            //finir par le cadre pour éviter des problèmes de points de fin de ligne
            G.DrawRectangle(styloUnite,
                (unite.i_X_CASE - xtravel) * m_rapport - m_tailleUnite / 2,
                (unite.i_Y_CASE - ytravel) * m_rapport - m_tailleUnite / 2,
                m_tailleUnite,
                m_tailleUnite);
        }

        void DessineUniteFilaireArtillerie(Graphics G, UniteRemarquable unite, Pen styloUnite, SolidBrush brosseRectangleUnite, Brush brosseUnite, int xtravel, int ytravel)
        {
            DessineFondUniteFilaire(G, unite, brosseRectangleUnite, xtravel, ytravel);

            G.FillEllipse(brosseUnite,
                (unite.i_X_CASE - xtravel) * m_rapport - m_tailleUnite / 4,
                (unite.i_Y_CASE - ytravel) * m_rapport - m_tailleUnite / 4,
                m_tailleUnite / 2,
                m_tailleUnite / 2);
            //finir par le cadre pour éviter des problèmes de points de fin de ligne
            G.DrawRectangle(styloUnite,
                (unite.i_X_CASE - xtravel) * m_rapport - m_tailleUnite / 2,
                (unite.i_Y_CASE - ytravel) * m_rapport - m_tailleUnite / 2,
                m_tailleUnite,
                m_tailleUnite);
        }
        void DessineUniteFilaireConvoi(Graphics G, UniteRemarquable unite, Pen styloUnite, SolidBrush brosseRectangleUnite, Brush brosseUnite, int xtravel, int ytravel)
        {
            DessineFondUniteFilaire(G, unite, brosseRectangleUnite, xtravel, ytravel);

            G.DrawEllipse(styloUnite,
                (unite.i_X_CASE - xtravel) * m_rapport - m_tailleUnite / 2,
                (unite.i_Y_CASE - ytravel) * m_rapport - m_tailleUnite / 2,
                m_tailleUnite,
                m_tailleUnite);
            G.FillPie(brosseUnite,
                (unite.i_X_CASE - xtravel) * m_rapport - m_tailleUnite / 2,
                (unite.i_Y_CASE - ytravel) * m_rapport - m_tailleUnite / 2,
                m_tailleUnite,
                m_tailleUnite,
                0, 180);
        }
        void DessineUniteFilaireDepot(Graphics G, UniteRemarquable unite, Pen styloUnite, SolidBrush brosseRectangleUnite, Brush brosseUnite, int xtravel, int ytravel)
        {
            DessineFondUniteFilaire(G, unite, brosseRectangleUnite, xtravel, ytravel);

            G.FillEllipse(brosseUnite,
                (unite.i_X_CASE - xtravel) * m_rapport - m_tailleUnite / 2,
                (unite.i_Y_CASE - ytravel) * m_rapport - m_tailleUnite / 2,
                m_tailleUnite,
                m_tailleUnite);
        }
        /// <summary>
        /// Déterminer les positions des corps
        /// - pour toutes les unités d'un corps, calculer le poids de sa position en prenant ses effectifs+tous ceux des unités situé dans le "rayon"
        /// - pour toutes les unités dans le "rayon" du poids le plus fort, mettre un lien vers l'unité de référence (juste pour n'afficher le corps qu'une fois) et indiquer la position du corps sur le chef de corps
        /// </summary>
        private void CalculPositionCorps()
        {
            for (int tour = CST_DEBUT_FILM; tour < m_nbImages; tour++)
            {
                foreach (UniteRole roleUnite in m_unitesRoles)
                {
                    if (roleUnite.iTour != tour) { continue; }

                    //recherche de la position la division qui regroupe le plus de poids avec ses voisines
                    List<UniteRemarquable> divisions =
                        (from a in m_unitesRemarquables
                         where a.iTour == tour
                                 && (a.tipe == TIPEUNITEVIDEO.INFANTERIE || a.tipe == TIPEUNITEVIDEO.CAVALERIE || a.tipe == TIPEUNITEVIDEO.ARTILLERIE)
                                 && a.ID_ROLE == roleUnite.ID_ROLE
                         select a).ToList();
                    int poidsMax = -1;
                    UniteRemarquable uniteMax = null;
                    foreach (UniteRemarquable unitePoid in divisions)
                    {
                        int poids = 0;
                        foreach (UniteRemarquable unite in divisions)
                        {
                            if ((Math.Abs(unitePoid.i_X_CASE - unite.i_X_CASE) <= m_largeurCorps * CST_RAYON_CORPS)
                                && (Math.Abs(unitePoid.i_Y_CASE - unite.i_Y_CASE) <= m_hauteurCorps * CST_RAYON_CORPS))
                            {
                                poids += unite.iEffectif;
                            }
                        }
                        if (poids > poidsMax)
                        {
                            poidsMax = poids;
                            uniteMax = unitePoid;
                        }
                    }

                    //position de la division pondérée par le poids de chaque division
                    if (poidsMax > 0)
                    {
                        int x_division=0, y_division=0;
                        foreach (UniteRemarquable unite in divisions)
                        {
                            if ((Math.Abs(uniteMax.i_X_CASE - unite.i_X_CASE) <= m_largeurCorps * CST_RAYON_CORPS)
                                && (Math.Abs(uniteMax.i_Y_CASE - unite.i_Y_CASE) <= m_hauteurCorps * CST_RAYON_CORPS))
                            {
                                x_division += unite.i_X_CASE * unite.iEffectif;
                                y_division += unite.i_Y_CASE * unite.iEffectif;
                                unite.bInclusDansLeCorps = true;
                            }
                            else
                            {
                                unite.bInclusDansLeCorps = false;
                            }
                        }
                        x_division /= poidsMax;
                        y_division /= poidsMax;

                        //affectation finale sur le chef de corps
                        roleUnite.i_X_CASE_CORPS = x_division;
                        roleUnite.i_Y_CASE_CORPS = y_division;
                        //la taille du corps affichée n'est égale qu'à la somme des unités dans le cadre, sinon, on voit les corps de chef d'armée surdimensionné par
                        //toutes les garnison
                        roleUnite.iEffectif = poidsMax;
                    }
                    else
                    {
                        roleUnite.i_X_CASE_CORPS = -1;
                        roleUnite.i_Y_CASE_CORPS = -1;
                    }                    
                }
            }
        }

        public string ChaineFichier(string source)
        {
            byte[] tempBytes;
            tempBytes = System.Text.Encoding.GetEncoding("ISO-8859-8").GetBytes(source);
            string asciiStr = System.Text.Encoding.UTF8.GetString(tempBytes);
            return asciiStr.Replace(" ", "_").Replace("'","").ToUpper();
        }

        /// <summary>
        /// ffmpeg -framerate 1 -i Automne_1813_Bataille118_%04d.png -c:v libx264 -r 30 -pix_fmt yuv420p video.mp4
        /// Pour utiliser %04d dans ffmpeg il faut que les sequences ce suivent
        /// La solution glob non supporté sur windows (globbing is not supported by this libavformat build), %4d ne supporte aucune rupture de sequence
        /// Je n'arrive pas à le faire avec un fichier de concat media comme ffmpeg -r 5 -f concat -safe 0 -i liste.txt -c:v libx264 output.mp4
        /// avec dans liste.txt
        /// file Bataille118_0000_0000.png
        /// file Bataille118_0440_0000.png
        /// file Bataille118_0442_0000.png
        /// </summary>
        /// <param name="fichierImage">Image à sauvegarder</param>
        private void SauvegardeImage(Bitmap fichierImage)
        {
            fichierImage.Save(m_repertoireVideo + "\\" + m_nomCampagne.Replace(' ', '_') + "_" + m_nomFichier
                                + "_" + m_numeroImage++.ToString("0000")
                                + ".png", ImageFormat.Png);
        }

    }
}
