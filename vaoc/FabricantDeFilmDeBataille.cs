using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Linq;

namespace vaoc
{
    public enum TIPEUNITEBATAILLE { INFANTERIE, CAVALERIE, ARTILLERIE, DEPOT, CONVOI, PONTONNIER, QG, BLESSE, PRISONNIER, AUTRE};
    public enum TIPETERRAINBATAILLE { PLAINE, FORET, VILLE, FORTERESSE, COLLINE, RIVIERE, FLEUVE, AUCUN};
    public enum TIPEFINBATAILLE { RETRAITE012, RETRAITE345, VICTOIRE012, VICTOIRE345, NUIT};
    public enum TIPEORIENTATIONBATAILLE { HORIZONTAL, VERTICAL };
    public class UniteBataille
    {
        public int iTour;
        public int iNation;
        public int iZone;
        public TIPEUNITEBATAILLE tipe;
        public int effectifInfanterie;
        public int effectifCavalerie;
        public int effectifArtillerie;
        public int ID;
        public string nom;
    }

    public class ZoneBataille
    {
        public int iTour;
        /// <summary>
        /// Résultat des jets de combats
        /// </summary>
        public string[] sCombat = new string[6];
        public int[] iPertes = new int[6];
    }

    /// <summary>
    /// Role dirigeant la bataille par tour
    /// </summary>
    public class RoleBataille
    {
        public int iTour;
        public string nomLeader012;
        public string nomLeader345;
        public int iNation012;
        public int iNation345;
    }

    public class FabricantDeFilmDeBataille
    {
        //private FileInfo[] m_listeFichiers;
        private int m_traitement;//traitement principal
        private string m_repertoireVideo;
        private List<ZoneBataille> m_zonesBataille;
        private List<UniteBataille> m_unitesBataille;
        private List<RoleBataille> m_rolesBataille;
        private TIPETERRAINBATAILLE[] m_terrains;
        private TIPETERRAINBATAILLE[] m_obstacles;
        private TIPEORIENTATIONBATAILLE m_orientation;
        private TIPEFINBATAILLE m_fin;
        private int m_largeur;
        private int m_hauteur;
        private int m_nbEtapes;
        private Font m_police;
        private string m_nomFichier;
        private Brush[] m_brossesTexte = new Brush[6];
        private const int NB_UNITES_LARGEUR = 5;
        private const int NB_UNITES_HAUTEUR = 3;
        private const int EPAISSEUR_BORDURE = 2;
        private const int NB_UNITES_PAR_LIGNE = 4;
        private int m_iNation012;
        private int m_iNation345;
        // commande dans un .bat ffmpeg -framerate 1 -i imageVideo_%%04d.png -c:v libx264 -r 30 -pix_fmt yuv420p video.mp4

        public FabricantDeFilmDeBataille()
        {
        }

        public string Initialisation(string nomFichier, string repertoireVideo, Font police, 
                                    int largeur, int hauteur,
                                    int iNation012,
                                    int iNation345,
                                    List<UniteBataille> unitesBataille, 
                                    List<RoleBataille> rolesBataille,
                                    List<ZoneBataille> zonesBataille,
                                    TIPETERRAINBATAILLE[] iTerrain,
                                    TIPETERRAINBATAILLE[] iObstacle,
                                    TIPEORIENTATIONBATAILLE cOrientation,
                                    TIPEFINBATAILLE iFin,
                                    int nbEtapes,
                                    int debut
                                    )
        {
            try
            {
                m_largeur = largeur;
                m_hauteur = hauteur;
                m_repertoireVideo = repertoireVideo;
                m_zonesBataille= zonesBataille;
                m_unitesBataille = unitesBataille;
                m_rolesBataille = rolesBataille;
                m_terrains = iTerrain;
                m_obstacles = iObstacle;
                m_orientation = cOrientation;
                m_fin = iFin;
                m_nbEtapes = nbEtapes;
                m_police = police;
                m_nomFichier= nomFichier;
                m_iNation012 = iNation012;
                m_iNation345 = iNation345;

                if (Directory.Exists(repertoireVideo))
                {
                    //on supprime toutes les images qui pourraient exister d'un précédent traitement
                    DirectoryInfo dir = new DirectoryInfo(repertoireVideo);
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

                for (m_traitement = debut; m_traitement< debut+m_nbEtapes; m_traitement++)
                {
                    switch (m_orientation)
                    {
                        case TIPEORIENTATIONBATAILLE.VERTICAL:
                            TraitementVertical(-1, TIPEFINBATAILLE.NUIT, false);
                            break;
                        default:
                            TraitementHorizontal(-1, TIPEFINBATAILLE.NUIT, false);
                            break;
                    }
                }
                if (m_fin != TIPEFINBATAILLE.NUIT)
                {
                    //afficher les valeurs de poursuite en cas de retraite ou de victoire directe
                    int zone;
                    switch(m_fin)
                    {
                        case TIPEFINBATAILLE.RETRAITE012:
                        case TIPEFINBATAILLE.VICTOIRE345:
                            zone = 4;
                            break;
                        case TIPEFINBATAILLE.RETRAITE345:
                        case TIPEFINBATAILLE.VICTOIRE012:
                            zone = 1;
                            break;
                        default:
                            zone = 0;
                            break;
                    }
                    switch (m_orientation)
                    {
                        case TIPEORIENTATIONBATAILLE.VERTICAL:
                            TraitementVertical(zone, m_fin, true);
                            break;
                        default:
                            TraitementHorizontal(zone, m_fin, true);
                            break;
                    }
                }
                Terminer();
                return string.Empty;
            }
            catch (Exception e)
            {
                return "Initialisation - Exception in: " + e.ToString();
            }
        }

        /// <summary>
        /// Lancement la génération effective du fichier final
        /// </summary>
        /// <returns>chaine vide si ok, chaine d'erreur sinon</returns>
        public string Terminer()
        {
            try
            {
                FilmMpeg(m_nomFichier);
            }
            catch (Exception e)
            {
                return "Terminer - Exception in: " + e.ToString();
            }
            return string.Empty;
        }

        private void FilmMpeg(string nom)
        {
            ProcessStartInfo processInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Normal,
                FileName = "ffmpeg.exe",
                WorkingDirectory = m_repertoireVideo, //Path.GetDirectoryName(YourApplicationPath);
                Arguments = string.Format("-framerate 1 -i {0}_%04d_%04d.png -c:v libx264 -r 30 -pix_fmt yuv420p {0}.mp4", nom)
            };
            Process.Start(processInfo);
        }

        public string TraitementHorizontal(int iZoneResultats, TIPEFINBATAILLE fin, bool bFin)
        {
            Graphics G;
            int xunite, yunite;
            try
            {
                //Debug.WriteLine("FabricantDeFilm:Traitement n°" + m_traitement);
                Bitmap fichierImage = new Bitmap(m_largeur, m_hauteur, PixelFormat.Format24bppRgb);
                G = Graphics.FromImage(fichierImage);
                G.PageUnit = GraphicsUnit.Pixel;

                DessineFondBataille(G);

                //recherche des leaders à ce moment de la bataille
                RoleBataille role = null;
                if (m_rolesBataille.Count > 0)
                {
                    int i = 0;
                    while (m_rolesBataille[i].iTour != m_traitement) i++;
                    role = m_rolesBataille[i];
                }

                //affichage des flèches
                if (iZoneResultats >= 0)
                {
                    foreach (ZoneBataille zoneBataille in m_zonesBataille)
                    {
                        if (zoneBataille.iTour != m_traitement
                            || null == zoneBataille.sCombat[iZoneResultats]
                            || zoneBataille.sCombat[iZoneResultats] == string.Empty) { continue; }
                        DessineFlecheHorizontale(G, iZoneResultats, zoneBataille, fin);
                    }
                }

                //unites
                int[] reserve = new int[2];
                int[] nbReserve = new int[2];
                int[] zone = new int[6];
                int[] nbZone = new int[6];
                //on compte d'abord le nombre d'unites par zone pour pouvoir les répartir au mieux
                foreach (UniteBataille unite in m_unitesBataille)
                {
                    if (unite.iTour != m_traitement) { continue; }
                    if (unite.iZone < 0)
                    {
                        nbReserve[unite.iNation]++;
                    }
                    else
                    {
                        nbZone[unite.iZone]++;
                    }
                }

                //on peut les dessiner maintenant
                foreach (UniteBataille unite in m_unitesBataille)
                {
                    if (unite.iTour != m_traitement) { continue; }
                    if (unite.iNation == m_iNation012)
                    {
                        if (unite.iZone<0)
                        {
                            //unite en réserve
                            DessineUniteImage(G, 
                                            unite, 
                                            (int)((reserve[unite.iNation] + 0.5) * m_largeur/ nbReserve[unite.iNation]), 
                                            m_hauteur/9/2,
                                            Brushes.Black);
                            reserve[unite.iNation]++;
                        }
                        else
                        {
                            if (nbZone[unite.iZone] <= NB_UNITES_PAR_LIGNE)
                            {
                                //affichage sur une seule colonne
                                xunite = unite.iZone * m_largeur / 3 + (int)((zone[unite.iZone] + 0.5) * m_largeur / 3 / nbZone[unite.iZone]);
                                yunite = 2 * m_hauteur / 9 + m_hauteur / 9 / 2;
                            }
                            else
                            {
                                //affichage sur deux lignes
                                xunite = (zone[unite.iZone] >= NB_UNITES_PAR_LIGNE) ?
                                    unite.iZone * m_largeur / 3 + (int)((zone[unite.iZone] - NB_UNITES_PAR_LIGNE + 0.5) * m_largeur / 3 / NB_UNITES_PAR_LIGNE) :
                                    unite.iZone * m_largeur / 3 + (int)((zone[unite.iZone] + 0.5) * m_largeur / 3 / NB_UNITES_PAR_LIGNE);
                                yunite = (zone[unite.iZone] >= NB_UNITES_PAR_LIGNE) ? 3 * m_hauteur / 9 + m_hauteur / 9 / 2 : 2 * m_hauteur / 9 + m_hauteur / 9 / 2;
                            }
                            DessineUniteImage(G, 
                                            unite, 
                                            xunite, 
                                            yunite,
                                            m_brossesTexte[unite.iZone]);
                            zone[unite.iZone]++;
                        }
                    }
                    else
                    {
                        if (unite.iZone < 0)
                        {
                            //unite en réserve
                            DessineUniteImage(G, 
                                            unite, 
                                            (int)((reserve[unite.iNation] + 0.5) * m_largeur / nbReserve[unite.iNation]), 
                                            7 * m_hauteur / 9 + m_hauteur / 9 / 2,
                                            Brushes.Black);
                            reserve[unite.iNation]++;
                        }
                        else
                        {
                            if (nbZone[unite.iZone] <= NB_UNITES_PAR_LIGNE)
                            {
                                //affichage sur une seule colonne
                                xunite = (unite.iZone - 3) * m_largeur / 3 + (int)((zone[unite.iZone] + 0.5) * m_largeur / 3 / nbZone[unite.iZone]);
                                yunite = 5 * m_hauteur / 9 + m_hauteur / 9 / 2;
                            }
                            else
                            {
                                //affichage sur deux lignes
                                xunite = (zone[unite.iZone] >= NB_UNITES_PAR_LIGNE) ?
                                    (unite.iZone - 3) * m_largeur / 3 + (int)((zone[unite.iZone] - NB_UNITES_PAR_LIGNE + 0.5) * m_largeur / 3 / NB_UNITES_PAR_LIGNE) :
                                    (unite.iZone - 3) * m_largeur / 3 + (int)((zone[unite.iZone] + 0.5) * m_largeur / 3 / NB_UNITES_PAR_LIGNE);
                                yunite = (zone[unite.iZone] >= NB_UNITES_PAR_LIGNE) ? 6 * m_hauteur / 9 + m_hauteur / 9 / 2 : 5 * m_hauteur / 9 + m_hauteur / 9 / 2;
                            }
                            DessineUniteImage(G, 
                                            unite,
                                            xunite, yunite,
                                            m_brossesTexte[unite.iZone]);
                            zone[unite.iZone]++;
                        }
                    }
                }

                fichierImage.Save(m_repertoireVideo + "\\" + m_nomFichier 
                                    + "_" + m_traitement.ToString("0000") 
                                    + "_" + (iZoneResultats+1).ToString("0000") + ".png", ImageFormat.Png);
                
                G.Dispose();
                fichierImage.Dispose();
                iZoneResultats++;
                if (iZoneResultats<6 && !bFin)
                {
                    //on refait la même image mais avec les résultats de l'attaque cette fois !
                    TraitementHorizontal(iZoneResultats, fin, bFin);
                }
                return string.Empty;
            }
            catch (Exception e)
            {
                return "Traitement Exception in: " + e.ToString();
            }
        }

        /// <summary>
        /// 2 -  2  - 1 -  2  - 2
        /// R - 345 - O - 012 - R
        /// </summary>
        /// <param name="G"></param>
        private void DessineFondBataille(Graphics G)
        {
            if (TIPEORIENTATIONBATAILLE.VERTICAL == m_orientation)
            {
                //Bataille verticale
                //Reserve droite
                Rectangle rectReserveDroite = new Rectangle(7 * m_largeur / 9, 0, 2 * m_largeur / 9, m_hauteur);
                G.FillRectangle(Brushes.White, rectReserveDroite);

                //Reserve gauche
                Rectangle rectReserveGauche = new Rectangle(0, 0, 2 * m_largeur / 9, m_hauteur);
                G.FillRectangle(Brushes.White, rectReserveGauche);

                //Terrains
                for (int t = 0; t < m_terrains.Count(); t++)
                {
                    Bitmap tuile;
                    switch (m_terrains[t])
                    {
                        case TIPETERRAINBATAILLE.PLAINE:
                            tuile = new Bitmap(vaoc.Properties.Resources.plaine);
                            m_brossesTexte[t] = Brushes.Black;
                            break;
                        case TIPETERRAINBATAILLE.FORET:
                            tuile = new Bitmap(vaoc.Properties.Resources.foret);
                            m_brossesTexte[t] = Brushes.Black;
                            break;
                        case TIPETERRAINBATAILLE.VILLE:
                            tuile = new Bitmap(vaoc.Properties.Resources.ville);
                            m_brossesTexte[t] = Brushes.White;
                            break;
                        case TIPETERRAINBATAILLE.FORTERESSE:
                            tuile = new Bitmap(vaoc.Properties.Resources.forteresse);
                            m_brossesTexte[t] = Brushes.Black;
                            break;
                        case TIPETERRAINBATAILLE.COLLINE:
                            tuile = new Bitmap(vaoc.Properties.Resources.colline);
                            m_brossesTexte[t] = Brushes.White;
                            break;
                        default:
                            tuile = new Bitmap(vaoc.Properties.Resources.blesse_1);
                            m_brossesTexte[t] = Brushes.Red;
                            break;
                    }
                    Rectangle rectTerrain;
                    if (t < 3)
                    {
                        rectTerrain = new Rectangle(5 * m_largeur / 9, t * m_hauteur / 3, 2 * m_largeur / 9, m_hauteur / 3);
                    }
                    else
                    {
                        rectTerrain = new Rectangle(2 * m_largeur / 9, (t-3) * m_hauteur / 3, 2 * m_largeur / 9, m_hauteur / 3);
                    }

                    TextureBrush bt = new TextureBrush(tuile, System.Drawing.Drawing2D.WrapMode.Tile,
                        new Rectangle(0, 0, tuile.Width, tuile.Height));
                    G.FillRectangle(bt, rectTerrain);
                    bt.Dispose();
                }

                //Obstacles
                for (int t = 0; t < m_obstacles.Count(); t++)
                {
                    Brush brosse = Brushes.HotPink;
                    Bitmap tuile = new Bitmap(vaoc.Properties.Resources.zoomMoins);
                    switch (m_obstacles[t])
                    {
                        case TIPETERRAINBATAILLE.RIVIERE:
                            tuile = new Bitmap(vaoc.Properties.Resources.riviere);
                            break;
                        case TIPETERRAINBATAILLE.FLEUVE:
                            tuile = new Bitmap(vaoc.Properties.Resources.fleuve);
                            break;
                        case TIPETERRAINBATAILLE.AUCUN:
                            brosse = Brushes.White;
                            break;
                        default:
                            tuile = new Bitmap(vaoc.Properties.Resources.zoomPlus);
                            break;
                    }
                    Rectangle rectObstacle;
                    rectObstacle = new Rectangle(4 * m_largeur / 9, t * m_hauteur / 3, m_largeur / 9, m_hauteur / 3);
                    if (brosse != Brushes.HotPink)
                    {
                        G.FillRectangle(brosse, rectObstacle);
                    }
                    else
                    {
                        TextureBrush bt = new TextureBrush(tuile, System.Drawing.Drawing2D.WrapMode.Tile,
                            new Rectangle(0, 0, tuile.Width, tuile.Height));
                        G.FillRectangle(bt, rectObstacle);
                        bt.Dispose();
                    }
                }
            }
            else
            {
                //Bataille horizontale
                //Reserve haute
                Rectangle rectReserveHaut = new Rectangle(0, 0, m_largeur, 2 * m_hauteur / 9);
                G.FillRectangle(Brushes.White, rectReserveHaut);

                //Reserve basse
                Rectangle rectReserveBas = new Rectangle(0, 7 * m_hauteur / 9, m_largeur, 2 * m_hauteur / 9);
                G.FillRectangle(Brushes.White, rectReserveBas);

                //Terrains
                for (int t = 0; t < m_terrains.Count(); t++)
                {
                    /*
                    Brush brosse;
                    switch (m_terrains[t])
                    {
                        case TIPETERRAINBATAILLE.PLAINE:
                            brosse = Brushes.White;
                            m_brossesTexte[t] = Brushes.Black;
                            break;
                        case TIPETERRAINBATAILLE.FORET:
                            brosse = Brushes.Green;
                            m_brossesTexte[t] = Brushes.Black;
                            break;
                        case TIPETERRAINBATAILLE.VILLE:
                            brosse = Brushes.Black;
                            m_brossesTexte[t] = Brushes.White;
                            break;
                        case TIPETERRAINBATAILLE.FORTERESSE:
                            brosse = Brushes.Red;
                            m_brossesTexte[t] = Brushes.Black;
                            break;
                        case TIPETERRAINBATAILLE.COLLINE:
                            brosse = Brushes.Brown;
                            m_brossesTexte[t] = Brushes.White;
                            break;
                        default:
                            brosse = Brushes.Yellow;
                            m_brossesTexte[t] = Brushes.Red;
                            break;
                    }
                    */
                    Bitmap tuile;
                    switch (m_terrains[t])
                    {
                        case TIPETERRAINBATAILLE.PLAINE:
                            tuile = new Bitmap(vaoc.Properties.Resources.plaine);
                            m_brossesTexte[t] = Brushes.Black;
                            break;
                        case TIPETERRAINBATAILLE.FORET:
                            tuile = new Bitmap(vaoc.Properties.Resources.foret);
                            m_brossesTexte[t] = Brushes.Black;
                            break;
                        case TIPETERRAINBATAILLE.VILLE:
                            tuile = new Bitmap(vaoc.Properties.Resources.ville);
                            m_brossesTexte[t] = Brushes.White;
                            break;
                        case TIPETERRAINBATAILLE.FORTERESSE:
                            tuile = new Bitmap(vaoc.Properties.Resources.forteresse);
                            m_brossesTexte[t] = Brushes.Black;
                            break;
                        case TIPETERRAINBATAILLE.COLLINE:
                            tuile = new Bitmap(vaoc.Properties.Resources.colline);
                            m_brossesTexte[t] = Brushes.White;
                            break;
                        default:
                            tuile = new Bitmap(vaoc.Properties.Resources.blesse_1);
                            m_brossesTexte[t] = Brushes.Red;
                            break;
                    }
                    Rectangle rectTerrain;
                    if (t < 3)
                    {
                        rectTerrain = new Rectangle(t * m_largeur / 3, 2 * m_hauteur / 9, m_largeur / 3, 2 * m_hauteur / 9);
                    }
                    else
                    {
                        rectTerrain = new Rectangle((t - 3) * m_largeur / 3, 5 * m_hauteur / 9, m_largeur / 3, 2 * m_hauteur / 9);
                    }
                    //G.FillRectangle(brosse, rectTerrain);                    
                    TextureBrush bt = new TextureBrush(tuile, System.Drawing.Drawing2D.WrapMode.Tile, 
                        new Rectangle(0,0,tuile.Width,tuile.Height));
                    G.FillRectangle(bt, rectTerrain);
                    bt.Dispose();
                }

                //Obstacles
                for (int t = 0; t < m_obstacles.Count(); t++)
                {
                    /*
                    Brush brosse;
                    switch (m_obstacles[t])
                    {
                        case TIPETERRAINBATAILLE.RIVIERE:
                            brosse = Brushes.LightBlue;
                            break;
                        case TIPETERRAINBATAILLE.FLEUVE:
                            brosse = Brushes.DarkBlue;
                            break;
                        case TIPETERRAINBATAILLE.AUCUN:
                            brosse = Brushes.White;
                            break;
                        default:
                            brosse = Brushes.Yellow;
                            break;
                    }
                    */
                    Brush brosse = Brushes.HotPink;
                    Bitmap tuile = new Bitmap(vaoc.Properties.Resources.zoomMoins);
                    switch (m_obstacles[t])
                    {
                        case TIPETERRAINBATAILLE.RIVIERE:
                            tuile = new Bitmap(vaoc.Properties.Resources.riviere);
                            break;
                        case TIPETERRAINBATAILLE.FLEUVE:
                            tuile = new Bitmap(vaoc.Properties.Resources.fleuve);
                            break;
                        case TIPETERRAINBATAILLE.AUCUN:
                            brosse = Brushes.White;
                            break;
                        default:
                            tuile = new Bitmap(vaoc.Properties.Resources.zoomPlus);
                            break;
                    }
                    Rectangle rectObstacle;
                    rectObstacle = new Rectangle(t * m_largeur / 3, 4 * m_hauteur / 9, m_largeur / 3, m_hauteur / 9);
                    if (brosse != Brushes.HotPink)
                    {
                        G.FillRectangle(brosse, rectObstacle);
                    }
                    else
                    {
                        TextureBrush bt = new TextureBrush(tuile, System.Drawing.Drawing2D.WrapMode.Tile,
                            new Rectangle(0, 0, tuile.Width, tuile.Height));
                        G.FillRectangle(bt, rectObstacle);
                        bt.Dispose();
                    }
                }
            }
        }

        private void DessineUniteImage0(Graphics G, UniteBataille unite, int x, int y, Brush brosseTexte)
        {
            Image image;
            switch (unite.tipe)
            {
                case TIPEUNITEBATAILLE.INFANTERIE:
                    //barre haut gauche, bas droite
                    image = (0 == unite.iNation) ? new Bitmap(vaoc.Properties.Resources.infanterie_0) : new Bitmap(vaoc.Properties.Resources.infanterie_1);
                    break;
                case TIPEUNITEBATAILLE.CAVALERIE:
                    //barre haut gauche, bas droite
                    image = (0 == unite.iNation) ? new Bitmap(vaoc.Properties.Resources.cavalerie_0) : new Bitmap(vaoc.Properties.Resources.cavalerie_1);
                    break;
                case TIPEUNITEBATAILLE.ARTILLERIE:
                    image = (0 == unite.iNation) ? new Bitmap(vaoc.Properties.Resources.artillerie_0) : new Bitmap(vaoc.Properties.Resources.artillerie_1);
                    break;
                case TIPEUNITEBATAILLE.CONVOI:
                    image = (0 == unite.iNation) ? new Bitmap(vaoc.Properties.Resources.convoi_0) : new Bitmap(vaoc.Properties.Resources.convoi_1);
                    break;
                case TIPEUNITEBATAILLE.DEPOT:
                    image = (0 == unite.iNation) ? new Bitmap(vaoc.Properties.Resources.depot_0) : new Bitmap(vaoc.Properties.Resources.depot_1);
                    break;
                case TIPEUNITEBATAILLE.PONTONNIER:
                    image = (0 == unite.iNation) ? new Bitmap(vaoc.Properties.Resources.genie_0) : new Bitmap(vaoc.Properties.Resources.genie_1);
                    break;
                case TIPEUNITEBATAILLE.QG:
                    image = (0 == unite.iNation) ? new Bitmap(vaoc.Properties.Resources.qg_0) : new Bitmap(vaoc.Properties.Resources.qg_1);
                    break;
                case TIPEUNITEBATAILLE.PRISONNIER:
                    image = (0 == unite.iNation) ? new Bitmap(vaoc.Properties.Resources.prisonnier_0) : new Bitmap(vaoc.Properties.Resources.prisonnier_1);
                    break;
                case TIPEUNITEBATAILLE.BLESSE:
                    image = (0 == unite.iNation) ? new Bitmap(vaoc.Properties.Resources.blesse_0) : new Bitmap(vaoc.Properties.Resources.blesse_1);
                    break;
                default:
                    image = new Bitmap(vaoc.Properties.Resources.zoomMoins);
                    break;
            }
            G.DrawImage(image, x - image.Width/2, y - image.Height/2);
            SizeF tailleTexte = G.MeasureString(unite.nom, m_police);
            StringFormat format = new StringFormat();
            format.LineAlignment = StringAlignment.Center;
            Rectangle rectText = new Rectangle(x - (int)tailleTexte.Width / 2,
                                    y + image.Height / 2,
                                    (int)tailleTexte.Width + 1,
                                    (int)tailleTexte.Height + 1);
            G.DrawString(unite.nom, m_police, brosseTexte, rectText, format);
        }

        private void DessineUniteImage(Graphics G, UniteBataille unite, int x, int y, Brush brosseTexte)
        {
            Image image= new Bitmap(vaoc.Properties.Resources.qg_0);//toutes les images doivent avoir la même taille
            int pos = 0;
            int nb_unites_infanterie = (int) Math.Ceiling((decimal)unite.effectifInfanterie / 1000);
            int nb_unites_cavalerie = (int)Math.Ceiling((decimal)unite.effectifCavalerie / 1000);
            int nb_unites_artillerie = (int)Math.Ceiling((decimal)unite.effectifArtillerie / 10);
            int nb_unites_largeur = Math.Min(NB_UNITES_LARGEUR, nb_unites_infanterie + nb_unites_cavalerie + nb_unites_artillerie);
            int nb_unites_hauteur = NB_UNITES_HAUTEUR;// on considère qu'il y a toujours le même nombre lignes au maximum
            int ximage=-1, yimage=-1;

            //emplacement du texte
            SizeF tailleTexte = G.MeasureString(unite.nom, m_police);
            Rectangle rectText = new Rectangle(x - (int)tailleTexte.Width / 2,
                                    y - (image.Height * nb_unites_hauteur) / 2 + image.Height * nb_unites_hauteur, 
                                    (int)tailleTexte.Width + 1,
                                    (int)tailleTexte.Height + 1);
            //cadre de l'unité
            Rectangle rectCadre = new Rectangle(Math.Min(rectText.Left, x - (nb_unites_largeur * image.Width) / 2) - EPAISSEUR_BORDURE,
                y - (image.Height * nb_unites_hauteur) /2 - EPAISSEUR_BORDURE,
                Math.Max(rectText.Right, x + (nb_unites_largeur * image.Width) / 2) - Math.Min(rectText.Left, 
                x - (nb_unites_largeur * image.Width) / 2) + EPAISSEUR_BORDURE * 2,
                rectText.Bottom - y + (image.Height * nb_unites_hauteur) / 2 + EPAISSEUR_BORDURE * 2);
            G.FillRectangle(Brushes.White, rectCadre);
            G.DrawRectangle(new Pen(Color.Black, EPAISSEUR_BORDURE), rectCadre);

            if (unite.tipe == TIPEUNITEBATAILLE.QG)
            {
                image = (0 == unite.iNation) ? new Bitmap(vaoc.Properties.Resources.qg_0) : new Bitmap(vaoc.Properties.Resources.qg_1);
                ximage = x - image.Width / 2;
                yimage = y;
                G.DrawImage(image, ximage, yimage);
            }
            else
            {
                image = (0 == unite.iNation) ? new Bitmap(vaoc.Properties.Resources.infanterie_0) : new Bitmap(vaoc.Properties.Resources.infanterie_1);
                for (int i = 0; i < nb_unites_infanterie; i++)
                {
                    ximage = x - (nb_unites_largeur * image.Width) / 2 + (pos % nb_unites_largeur) * image.Width;
                    yimage = y - (image.Height * nb_unites_hauteur) / 2 + image.Height * (pos / NB_UNITES_LARGEUR);
                    G.DrawImage(image, ximage, yimage);
                    pos++;
                }

                image = (0 == unite.iNation) ? new Bitmap(vaoc.Properties.Resources.cavalerie_0) : new Bitmap(vaoc.Properties.Resources.cavalerie_1);
                for (int i = 0; i < nb_unites_cavalerie; i++)
                {
                    ximage = x - (nb_unites_largeur * image.Width) / 2 + (pos % nb_unites_largeur) * image.Width;
                    yimage = y - (image.Height * nb_unites_hauteur) / 2 + image.Height * (pos / NB_UNITES_LARGEUR);
                    G.DrawImage(image, ximage, yimage);
                    pos++;
                }
                image = (0 == unite.iNation) ? new Bitmap(vaoc.Properties.Resources.artillerie_0) : new Bitmap(vaoc.Properties.Resources.artillerie_1);
                for (int i = 0; i < nb_unites_artillerie; i++)
                {
                    ximage = x - (nb_unites_largeur * image.Width) / 2 + (pos % nb_unites_largeur) * image.Width;
                    yimage = y - (image.Height * nb_unites_hauteur) / 2 + image.Height * (pos / NB_UNITES_LARGEUR);
                    G.DrawImage(image, ximage, yimage);
                    pos++;
                }
            }
            StringFormat format = new StringFormat();
            format.LineAlignment = StringAlignment.Center;
            //G.DrawString(unite.nom, m_police, brosseTexte, rectText, format);
            G.DrawString(unite.nom, m_police, Brushes.Black, rectText, format);
        }

        public string ChaineFichier(string source)
        {
            byte[] tempBytes;
            tempBytes = System.Text.Encoding.GetEncoding("ISO-8859-8").GetBytes(source);
            string asciiStr = System.Text.Encoding.UTF8.GetString(tempBytes);
            return asciiStr.Replace(" ", "_").Replace("'","").ToUpper();
        }

        private void DessineFlecheHorizontale(Graphics G, int iZoneResultats, ZoneBataille zoneBataille, TIPEFINBATAILLE fin)
        {
            Brush brosse, couleurTexte;
            Point[] fleche;
            int f;
            int xtexte;

            //on dessine la flèche
            StringFormat format = new StringFormat();
            format.LineAlignment = StringAlignment.Center;
            format.Alignment = StringAlignment.Center;
            if (iZoneResultats < 3)
            {
                brosse = (0 == m_iNation012) ? Brushes.Blue : Brushes.OrangeRed;
                couleurTexte = (0 == m_iNation012) ? Brushes.White : Brushes.Black;
                fleche = new Point[8];
                f = 0;
                xtexte = (iZoneResultats * m_largeur / 3) + m_largeur / 6;
                switch (fin)
                {
                    case TIPEFINBATAILLE.RETRAITE345:
                        fleche[f].X = (iZoneResultats * m_largeur / 3) + m_largeur / 3 / 4; fleche[f++].Y = 5 * m_hauteur / 18;//haut, gauche
                        fleche[f].X = (iZoneResultats * m_largeur / 3) + m_largeur / 3 / 4; fleche[f++].Y = 5 * m_hauteur / 9;
                        fleche[f].X = (iZoneResultats * m_largeur / 3) + m_largeur / 3 / 6; fleche[f++].Y = 5 * m_hauteur / 9;
                        fleche[f].X = (iZoneResultats * m_largeur / 3) + m_largeur / 3 / 2; fleche[f++].Y = 9 * m_hauteur / 9;//pointe
                        fleche[f].X = (iZoneResultats * m_largeur / 3) + m_largeur / 3 - m_largeur / 3 / 6; fleche[f++].Y = 5 * m_hauteur / 9;
                        fleche[f].X = (iZoneResultats * m_largeur / 3) + m_largeur / 3 - m_largeur / 3 / 4; fleche[f++].Y = 5 * m_hauteur / 9;
                        fleche[f].X = (iZoneResultats * m_largeur / 3) + m_largeur / 3 - m_largeur / 3 / 4; fleche[f++].Y = 5 * m_hauteur / 18;
                        break;
                    case TIPEFINBATAILLE.VICTOIRE012:
                        fleche[f].X = (iZoneResultats * m_largeur / 3) + m_largeur / 3 / 4; fleche[f++].Y = 11 * m_hauteur / 18;//haut, gauche
                        fleche[f].X = (iZoneResultats * m_largeur / 3) + m_largeur / 3 / 4; fleche[f++].Y = 7 * m_hauteur / 9;
                        fleche[f].X = (iZoneResultats * m_largeur / 3) + m_largeur / 3 / 6; fleche[f++].Y = 7 * m_hauteur / 9;
                        fleche[f].X = (iZoneResultats * m_largeur / 3) + m_largeur / 3 / 2; fleche[f++].Y = 9 * m_hauteur / 9;//pointe
                        fleche[f].X = (iZoneResultats * m_largeur / 3) + m_largeur / 3 - m_largeur / 3 / 6; fleche[f++].Y = 7 * m_hauteur / 9;
                        fleche[f].X = (iZoneResultats * m_largeur / 3) + m_largeur / 3 - m_largeur / 3 / 4; fleche[f++].Y = 7 * m_hauteur / 9;
                        fleche[f].X = (iZoneResultats * m_largeur / 3) + m_largeur / 3 - m_largeur / 3 / 4; fleche[f++].Y = 11 * m_hauteur / 18;
                        break;
                    default:
                        fleche[f].X = (iZoneResultats * m_largeur / 3) + m_largeur / 3 / 4; fleche[f++].Y = 5 * m_hauteur / 18;//haut, gauche
                        fleche[f].X = (iZoneResultats * m_largeur / 3) + m_largeur / 3 / 4; fleche[f++].Y = 5 * m_hauteur / 9;
                        fleche[f].X = (iZoneResultats * m_largeur / 3) + m_largeur / 3 / 6; fleche[f++].Y = 5 * m_hauteur / 9;
                        fleche[f].X = (iZoneResultats * m_largeur / 3) + m_largeur / 3 / 2; fleche[f++].Y = 7 * m_hauteur / 9;//pointe
                        fleche[f].X = (iZoneResultats * m_largeur / 3) + m_largeur / 3 - m_largeur / 3 / 6; fleche[f++].Y = 5 * m_hauteur / 9;
                        fleche[f].X = (iZoneResultats * m_largeur / 3) + m_largeur / 3 - m_largeur / 3 / 4; fleche[f++].Y = 5 * m_hauteur / 9;
                        fleche[f].X = (iZoneResultats * m_largeur / 3) + m_largeur / 3 - m_largeur / 3 / 4; fleche[f++].Y = 5 * m_hauteur / 18;
                        break;
                }
            }
            else
            {
                brosse = (0 == m_iNation345) ? Brushes.Blue : Brushes.OrangeRed;
                couleurTexte = (0 == m_iNation345) ? Brushes.White : Brushes.Black;
                fleche = new Point[8];
                f = 0;
                xtexte = ((iZoneResultats - 3) * m_largeur / 3) + m_largeur / 6;
                switch (fin)
                {
                    case TIPEFINBATAILLE.RETRAITE012:
                        fleche[f].X = ((iZoneResultats - 3) * m_largeur / 3) + m_largeur / 3 / 2; fleche[f++].Y = 0;//pointe
                        fleche[f].X = ((iZoneResultats - 3) * m_largeur / 3) + m_largeur / 3 / 6; fleche[f++].Y = 4 * m_hauteur / 9;
                        fleche[f].X = ((iZoneResultats - 3) * m_largeur / 3) + m_largeur / 3 / 4; fleche[f++].Y = 4 * m_hauteur / 9;
                        fleche[f].X = ((iZoneResultats - 3) * m_largeur / 3) + m_largeur / 3 / 4; fleche[f++].Y = 13 * m_hauteur / 18;//bas gauche
                        fleche[f].X = ((iZoneResultats - 3) * m_largeur / 3) + m_largeur / 3 - m_largeur / 3 / 4; fleche[f++].Y = 13 * m_hauteur / 18;
                        fleche[f].X = ((iZoneResultats - 3) * m_largeur / 3) + m_largeur / 3 - m_largeur / 3 / 4; fleche[f++].Y = 4 * m_hauteur / 9;
                        fleche[f].X = ((iZoneResultats - 3) * m_largeur / 3) + m_largeur / 3 - m_largeur / 3 / 6; fleche[f++].Y = 4 * m_hauteur / 9;
                        break;
                    case TIPEFINBATAILLE.VICTOIRE345:
                        fleche[f].X = ((iZoneResultats - 3) * m_largeur / 3) + m_largeur / 3 / 2; fleche[f++].Y = 0;//pointe
                        fleche[f].X = ((iZoneResultats - 3) * m_largeur / 3) + m_largeur / 3 / 6; fleche[f++].Y = 2 * m_hauteur / 9;
                        fleche[f].X = ((iZoneResultats - 3) * m_largeur / 3) + m_largeur / 3 / 4; fleche[f++].Y = 2 * m_hauteur / 9;
                        fleche[f].X = ((iZoneResultats - 3) * m_largeur / 3) + m_largeur / 3 / 4; fleche[f++].Y = 3 * m_hauteur / 18;//bas gauche
                        fleche[f].X = ((iZoneResultats - 3) * m_largeur / 3) + m_largeur / 3 - m_largeur / 3 / 4; fleche[f++].Y = 3 * m_hauteur / 18;
                        fleche[f].X = ((iZoneResultats - 3) * m_largeur / 3) + m_largeur / 3 - m_largeur / 3 / 4; fleche[f++].Y = 2 * m_hauteur / 9;
                        fleche[f].X = ((iZoneResultats - 3) * m_largeur / 3) + m_largeur / 3 - m_largeur / 3 / 6; fleche[f++].Y = 2 * m_hauteur / 9;
                        break;
                    default:
                        fleche[f].X = ((iZoneResultats - 3) * m_largeur / 3) + m_largeur / 3 / 2; fleche[f++].Y = 2 * m_hauteur / 9;//pointe
                        fleche[f].X = ((iZoneResultats - 3) * m_largeur / 3) + m_largeur / 3 / 6; fleche[f++].Y = 4 * m_hauteur / 9;
                        fleche[f].X = ((iZoneResultats - 3) * m_largeur / 3) + m_largeur / 3 / 4; fleche[f++].Y = 4 * m_hauteur / 9;
                        fleche[f].X = ((iZoneResultats - 3) * m_largeur / 3) + m_largeur / 3 / 4; fleche[f++].Y = 13 * m_hauteur / 18;//bas gauche
                        fleche[f].X = ((iZoneResultats - 3) * m_largeur / 3) + m_largeur / 3 - m_largeur / 3 / 4; fleche[f++].Y = 13 * m_hauteur / 18;
                        fleche[f].X = ((iZoneResultats - 3) * m_largeur / 3) + m_largeur / 3 - m_largeur / 3 / 4; fleche[f++].Y = 4 * m_hauteur / 9;
                        fleche[f].X = ((iZoneResultats - 3) * m_largeur / 3) + m_largeur / 3 - m_largeur / 3 / 6; fleche[f++].Y = 4 * m_hauteur / 9;
                        break;
                }
            }
            fleche[f].X = fleche[0].X; fleche[f++].Y = fleche[0].Y;
            G.FillPolygon(brosse, fleche);

            SizeF tailleTexte = G.MeasureString(zoneBataille.sCombat[iZoneResultats], m_police);
            G.DrawString(zoneBataille.sCombat[iZoneResultats], m_police, couleurTexte,
                new Rectangle(xtexte - (int)tailleTexte.Width / 2, m_hauteur / 2 - 2 * (int)tailleTexte.Height, (int)tailleTexte.Width + 2, (int)tailleTexte.Height), format);
            tailleTexte = G.MeasureString(zoneBataille.iPertes[iZoneResultats].ToString() + " †", m_police);
            G.DrawString(zoneBataille.iPertes[iZoneResultats].ToString() + " †", m_police, couleurTexte,
                new Rectangle(xtexte - (int)tailleTexte.Width / 2, m_hauteur / 2 + 2 * (int)tailleTexte.Height, (int)tailleTexte.Width + 2, (int)tailleTexte.Height), format);
        }

        private void DessineFlecheVerticale(Graphics G, int iZoneResultats, ZoneBataille zoneBataille, TIPEFINBATAILLE fin)
        {
            Brush brosse, couleurTexte;
            Point[] fleche;
            int f;
            int ytexte;

            //on dessine la flèche
            StringFormat format = new StringFormat();
            format.LineAlignment = StringAlignment.Center;
            format.Alignment = StringAlignment.Center;
            if (iZoneResultats < 3)
            {
                brosse = (0 == m_iNation012) ? Brushes.Blue : Brushes.OrangeRed;
                couleurTexte = (0 == m_iNation012) ? Brushes.White : Brushes.Black;
                fleche = new Point[8];
                f = 0;
                ytexte = iZoneResultats * m_hauteur / 3 + m_hauteur / 6;
                switch (fin)
                {
                    case TIPEFINBATAILLE.RETRAITE345:
                        fleche[f].Y = ((iZoneResultats) * m_hauteur / 3) + m_hauteur / 3 / 2; fleche[f++].X = 0;//pointe
                        fleche[f].Y = ((iZoneResultats) * m_hauteur / 3) + m_hauteur / 3 / 6; fleche[f++].X = 4 * m_largeur / 9;
                        fleche[f].Y = ((iZoneResultats) * m_hauteur / 3) + m_hauteur / 3 / 4; fleche[f++].X = 4 * m_largeur / 9;
                        fleche[f].Y = ((iZoneResultats) * m_hauteur / 3) + m_hauteur / 3 / 4; fleche[f++].X = 13 * m_largeur / 18;//bas gauche
                        fleche[f].Y = ((iZoneResultats) * m_hauteur / 3) + m_hauteur / 3 - m_hauteur / 3 / 4; fleche[f++].X = 13 * m_largeur / 18;
                        fleche[f].Y = ((iZoneResultats) * m_hauteur / 3) + m_hauteur / 3 - m_hauteur / 3 / 4; fleche[f++].X = 4 * m_largeur / 9;
                        fleche[f].Y = ((iZoneResultats) * m_hauteur / 3) + m_hauteur / 3 - m_hauteur / 3 / 6; fleche[f++].X = 4 * m_largeur / 9;
                        break;
                    case TIPEFINBATAILLE.VICTOIRE012:
                        fleche[f].Y = ((iZoneResultats) * m_hauteur / 3) + m_hauteur / 3 / 2; fleche[f++].X = 0;//pointe
                        fleche[f].Y = ((iZoneResultats) * m_hauteur / 3) + m_hauteur / 3 / 6; fleche[f++].X = 2 * m_largeur / 9;
                        fleche[f].Y = ((iZoneResultats) * m_hauteur / 3) + m_hauteur / 3 / 4; fleche[f++].X = 2 * m_largeur / 9;
                        fleche[f].Y = ((iZoneResultats) * m_hauteur / 3) + m_hauteur / 3 / 4; fleche[f++].X = 3 * m_largeur / 18;//bas gauche
                        fleche[f].Y = ((iZoneResultats) * m_hauteur / 3) + m_hauteur / 3 - m_hauteur / 3 / 4; fleche[f++].X = 3 * m_largeur / 18;
                        fleche[f].Y = ((iZoneResultats) * m_hauteur / 3) + m_hauteur / 3 - m_hauteur / 3 / 4; fleche[f++].X = 2 * m_largeur / 9;
                        fleche[f].Y = ((iZoneResultats) * m_hauteur / 3) + m_hauteur / 3 - m_hauteur / 3 / 6; fleche[f++].X = 2 * m_largeur / 9;
                        break;
                    default:
                        fleche[f].Y = ((iZoneResultats) * m_hauteur / 3) + m_hauteur / 3 / 2; fleche[f++].X = 2 * m_largeur / 9;//pointe
                        fleche[f].Y = ((iZoneResultats) * m_hauteur / 3) + m_hauteur / 3 / 6; fleche[f++].X = 4 * m_largeur / 9;
                        fleche[f].Y = ((iZoneResultats) * m_hauteur / 3) + m_hauteur / 3 / 4; fleche[f++].X = 4 * m_largeur / 9;
                        fleche[f].Y = ((iZoneResultats) * m_hauteur / 3) + m_hauteur / 3 / 4; fleche[f++].X = 13 * m_largeur / 18;//bas gauche
                        fleche[f].Y = ((iZoneResultats) * m_hauteur / 3) + m_hauteur / 3 - m_hauteur / 3 / 4; fleche[f++].X = 13 * m_largeur / 18;
                        fleche[f].Y = ((iZoneResultats) * m_hauteur / 3) + m_hauteur / 3 - m_hauteur / 3 / 4; fleche[f++].X = 4 * m_largeur / 9;
                        fleche[f].Y = ((iZoneResultats) * m_hauteur / 3) + m_hauteur / 3 - m_hauteur / 3 / 6; fleche[f++].X = 4 * m_largeur / 9;
                        break;
                }
            }
            else
            {
                brosse = (0 == m_iNation345) ? Brushes.Blue : Brushes.OrangeRed;
                couleurTexte = (0 == m_iNation345) ? Brushes.White : Brushes.Black;
                fleche = new Point[8];
                f = 0;
                ytexte = (iZoneResultats  - 3) * m_hauteur / 3 + m_hauteur / 6; 
                switch (fin)
                {
                    case TIPEFINBATAILLE.RETRAITE012:
                        fleche[f].Y = ((iZoneResultats - 3) * m_hauteur / 3) + m_hauteur / 3 / 4; fleche[f++].X = 5 * m_largeur / 18;//haut, gauche
                        fleche[f].Y = ((iZoneResultats - 3) * m_hauteur / 3) + m_hauteur / 3 / 4; fleche[f++].X = 5 * m_largeur / 9;
                        fleche[f].Y = ((iZoneResultats - 3) * m_hauteur / 3) + m_hauteur / 3 / 6; fleche[f++].X = 5 * m_largeur / 9;
                        fleche[f].Y = ((iZoneResultats - 3) * m_hauteur / 3) + m_hauteur / 3 / 2; fleche[f++].X = 9 * m_largeur / 9;//pointe
                        fleche[f].Y = ((iZoneResultats - 3) * m_hauteur / 3) + m_hauteur / 3 - m_hauteur / 3 / 6; fleche[f++].X = 5 * m_largeur / 9;
                        fleche[f].Y = ((iZoneResultats - 3) * m_hauteur / 3) + m_hauteur / 3 - m_hauteur / 3 / 4; fleche[f++].X = 5 * m_largeur / 9;
                        fleche[f].Y = ((iZoneResultats - 3) * m_hauteur / 3) + m_hauteur / 3 - m_hauteur / 3 / 4; fleche[f++].X = 5 * m_largeur / 18;
                        break;
                    case TIPEFINBATAILLE.VICTOIRE345:
                        fleche[f].Y = ((iZoneResultats - 3) * m_hauteur / 3) + m_hauteur / 3 / 4; fleche[f++].X = 11 * m_largeur / 18;//haut, gauche
                        fleche[f].Y = ((iZoneResultats - 3) * m_hauteur / 3) + m_hauteur / 3 / 4; fleche[f++].X = 7 * m_largeur / 9;
                        fleche[f].Y = ((iZoneResultats - 3) * m_hauteur / 3) + m_hauteur / 3 / 6; fleche[f++].X = 7 * m_largeur / 9;
                        fleche[f].Y = ((iZoneResultats - 3) * m_hauteur / 3) + m_hauteur / 3 / 2; fleche[f++].X = 9 * m_largeur / 9;//pointe
                        fleche[f].Y = ((iZoneResultats - 3) * m_hauteur / 3) + m_hauteur / 3 - m_hauteur / 3 / 6; fleche[f++].X = 7 * m_largeur / 9;
                        fleche[f].Y = ((iZoneResultats - 3) * m_hauteur / 3) + m_hauteur / 3 - m_hauteur / 3 / 4; fleche[f++].X = 7 * m_largeur / 9;
                        fleche[f].Y = ((iZoneResultats - 3) * m_hauteur / 3) + m_hauteur / 3 - m_hauteur / 3 / 4; fleche[f++].X = 11 * m_largeur / 18;
                        break;
                    default:
                        fleche[f].Y = ((iZoneResultats - 3) * m_hauteur / 3) + m_hauteur / 3 / 4; fleche[f++].X = 5 * m_largeur / 18;//haut, gauche
                        fleche[f].Y = ((iZoneResultats - 3) * m_hauteur / 3) + m_hauteur / 3 / 4; fleche[f++].X = 5 * m_largeur / 9;
                        fleche[f].Y = ((iZoneResultats - 3) * m_hauteur / 3) + m_hauteur / 3 / 6; fleche[f++].X = 5 * m_largeur / 9;
                        fleche[f].Y = ((iZoneResultats - 3) * m_hauteur / 3) + m_hauteur / 3 / 2; fleche[f++].X = 7 * m_largeur / 9;//pointe
                        fleche[f].Y = ((iZoneResultats - 3) * m_hauteur / 3) + m_hauteur / 3 - m_hauteur / 3 / 6; fleche[f++].X = 5 * m_largeur / 9;
                        fleche[f].Y = ((iZoneResultats - 3) * m_hauteur / 3) + m_hauteur / 3 - m_hauteur / 3 / 4; fleche[f++].X = 5 * m_largeur / 9;
                        fleche[f].Y = ((iZoneResultats - 3) * m_hauteur / 3) + m_hauteur / 3 - m_hauteur / 3 / 4; fleche[f++].X = 5 * m_largeur / 18;
                        break;
                }
            }
            fleche[f].Y = fleche[0].Y; fleche[f++].X = fleche[0].X;
            G.FillPolygon(brosse, fleche);
            SizeF tailleTexte = G.MeasureString(zoneBataille.sCombat[iZoneResultats], m_police);
            G.DrawString(zoneBataille.sCombat[iZoneResultats], m_police, couleurTexte,
                new Rectangle(m_largeur / 2 - (int)tailleTexte.Width / 2, 
                            ytexte - 2 * (int)tailleTexte.Height, 
                            (int)tailleTexte.Width + 2, (int)tailleTexte.Height),
                format);
            tailleTexte = G.MeasureString(zoneBataille.iPertes[iZoneResultats].ToString() + " †", m_police);
            G.DrawString(zoneBataille.iPertes[iZoneResultats].ToString() + " †", m_police, couleurTexte,
                new Rectangle(m_largeur / 2 - (int)tailleTexte.Width / 2,
                            ytexte + 2 * (int)tailleTexte.Height, 
                            (int)tailleTexte.Width + 2, (int)tailleTexte.Height),
                format);
        }

        private string TraitementVertical(int iZoneResultats, TIPEFINBATAILLE fin, bool bFin)
        {
            Graphics G;
            int xunite, yunite;
            try
            {
                Bitmap fichierImage = new Bitmap(m_largeur, m_hauteur, PixelFormat.Format24bppRgb);
                G = Graphics.FromImage(fichierImage);
                G.PageUnit = GraphicsUnit.Pixel;

                DessineFondBataille(G);

                //recherche des leaders à ce moment de la bataille
                int i = 0;
                while (m_rolesBataille[i].iTour != m_traitement) i++;
                RoleBataille role = m_rolesBataille[i];

                //affichage des flèches
                if (iZoneResultats >= 0)
                {
                    foreach (ZoneBataille zoneBataille in m_zonesBataille)
                    {
                        if (zoneBataille.iTour != m_traitement
                            || null == zoneBataille.sCombat[iZoneResultats]
                            || zoneBataille.sCombat[iZoneResultats] == string.Empty) { continue; }
                        DessineFlecheVerticale(G, iZoneResultats, zoneBataille, fin);
                    }
                }

                //unites
                int[] reserve = new int[2];
                int[] nbReserve = new int[2];
                int[] zone = new int[6];
                int[] nbZone = new int[6];
                //on compte d'abord le nombre d'unites par zone pour pouvoir les répartir au mieux
                foreach (UniteBataille unite in m_unitesBataille)
                {
                    if (unite.iTour != m_traitement) { continue; }
                    if (unite.iZone < 0)
                    {
                        nbReserve[unite.iNation]++;
                    }
                    else
                    {
                        nbZone[unite.iZone]++;
                    }
                }

                //on peut les dessiner maintenant
                foreach (UniteBataille unite in m_unitesBataille)
                {
                    if (unite.iTour != m_traitement) { continue; }
                    if (unite.iNation == m_iNation012)
                    {
                        if (unite.iZone < 0)
                        {
                            //unite en réserve
                            DessineUniteImage(G,
                                            unite,
                                            16 * m_largeur / 9 / 2,
                                            (int)((reserve[unite.iNation] + 0.5) * m_hauteur / nbReserve[unite.iNation]),
                                            Brushes.Black);
                            reserve[unite.iNation]++;
                        }
                        else
                        {
                            if (nbZone[unite.iZone] <= NB_UNITES_PAR_LIGNE)
                            {
                                //affichage sur une seule colonne
                                xunite = 12 * m_largeur / 9 / 2;
                                yunite = unite.iZone * m_hauteur / 3 + (int)((zone[unite.iZone] + 0.5) * m_hauteur / 3 / nbZone[unite.iZone]);
                            }
                            else
                            {
                                //affichage sur deux colonnes
                                xunite = (zone[unite.iZone] >= NB_UNITES_PAR_LIGNE) ? 13 * m_largeur / 9 / 2 : 11 * m_largeur / 9 / 2;
                                yunite = (zone[unite.iZone] >= NB_UNITES_PAR_LIGNE) ? 
                                    unite.iZone  * m_hauteur / 3 + (int)((zone[unite.iZone] - NB_UNITES_PAR_LIGNE + 0.5) * m_hauteur / 3 / NB_UNITES_PAR_LIGNE) :
                                    unite.iZone * m_hauteur / 3 + (int)((zone[unite.iZone] + 0.5) * m_hauteur / 3 / NB_UNITES_PAR_LIGNE);
                            }
                            DessineUniteImage(G,
                                            unite,
                                            xunite,yunite,
                                            m_brossesTexte[unite.iZone]);
                            zone[unite.iZone]++;
                        }
                    }
                    else
                    {
                        if (unite.iZone < 0)
                        {
                            //unite en réserve
                            DessineUniteImage(G,
                                            unite,
                                            2 * m_largeur / 9 / 2,
                                            (int)((reserve[unite.iNation] + 0.5) * m_hauteur / nbReserve[unite.iNation]),
                                            Brushes.Black);
                            reserve[unite.iNation]++;
                        }
                        else
                        {
                            if (nbZone[unite.iZone] <= NB_UNITES_PAR_LIGNE)
                            {
                                //affichage sur une seule colonne
                                xunite = 6 * m_largeur / 9 / 2;
                                yunite = (unite.iZone - 3) * m_hauteur / 3 + (int)((zone[unite.iZone] + 0.5) * m_hauteur / 3 / nbZone[unite.iZone]);
                            }
                            else
                            {
                                //affichage sur deux colonnes
                                xunite = (zone[unite.iZone] >= NB_UNITES_PAR_LIGNE) ? 7 * m_largeur / 9 / 2 : 5 * m_largeur / 9 / 2;
                                yunite = (zone[unite.iZone] >= NB_UNITES_PAR_LIGNE) ? 
                                    (unite.iZone - 3) * m_hauteur / 3 + (int)((zone[unite.iZone] - NB_UNITES_PAR_LIGNE + 0.5) * m_hauteur / 3 / NB_UNITES_PAR_LIGNE) :
                                    (unite.iZone - 3) * m_hauteur / 3 + (int)((zone[unite.iZone] + 0.5) * m_hauteur / 3 / NB_UNITES_PAR_LIGNE);
                            }
                            DessineUniteImage(G,
                                            unite,
                                            xunite, yunite,
                                            m_brossesTexte[unite.iZone]);
                            zone[unite.iZone]++;
                        }
                    }
                }

                fichierImage.Save(m_repertoireVideo + "\\" + m_nomFichier
                                    + "_" + m_traitement.ToString("0000")
                                    + "_" + (iZoneResultats + 1).ToString("0000") + ".png", ImageFormat.Png);

                G.Dispose();
                fichierImage.Dispose();
                iZoneResultats++;
                if (iZoneResultats < 6 && !bFin)
                {
                    //on refait la même image mais avec les résultats de l'attaque cette fois !
                    TraitementVertical(iZoneResultats, fin, bFin);
                }
                return string.Empty;
            }
            catch (Exception e)
            {
                return "Traitement Exception in: " + e.ToString();
            }
        }
    }
}
