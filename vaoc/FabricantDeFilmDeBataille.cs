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
        private const int EPAISSEUR_BORDURE = 2;

        // commande dans un .bat ffmpeg -framerate 1 -i imageVideo_%%04d.png -c:v libx264 -r 30 -pix_fmt yuv420p video.mp4

        public FabricantDeFilmDeBataille()
        {
        }

        public string Initialisation(string nomFichier, string repertoireVideo, Font police, 
                                    int largeur, int hauteur,
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
            try
            {
                //Debug.WriteLine("FabricantDeFilm:Traitement n°" + m_traitement);
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
                        DessineFlecheHorizontale(G, role, iZoneResultats, zoneBataille, fin);
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
                    if (unite.iNation == role.iNation012)
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
                            DessineUniteImage(G, 
                                            unite, 
                                            unite.iZone * m_largeur / 3 + (int)((zone[unite.iZone]+0.5) * m_largeur / 3 / nbZone[unite.iZone]), 
                                            2*m_hauteur/9 + m_hauteur/9/2,
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
                            DessineUniteImage(G, 
                                            unite,
                                            (unite.iZone - 3)  * m_largeur / 3 + (int)((zone[unite.iZone] + 0.5) * m_largeur / 3 / nbZone[unite.iZone]),
                                            5 * m_hauteur / 9 + m_hauteur / 9 / 2,
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

        private void DessineFlecheHorizontale(Graphics G, RoleBataille role, int iZoneResultats, ZoneBataille zoneBataille, TIPEFINBATAILLE fin)
        {
            //on dessine la flèche
            StringFormat format = new StringFormat();
            format.LineAlignment = StringAlignment.Center;
            format.Alignment = StringAlignment.Center;
            if (iZoneResultats < 3)
            {
                Brush brosse = (0 == role.iNation012) ? Brushes.Blue : Brushes.OrangeRed;
                Point[] fleche = new Point[8];
                int f = 0;
                switch(fin)
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
                fleche[f].X = fleche[0].X; fleche[f++].Y = fleche[0].Y;
                G.FillPolygon(brosse, fleche);
                G.DrawString(zoneBataille.sCombat[iZoneResultats], m_police, Brushes.Black,
                    new Rectangle(fleche[0], new Size(fleche[5].X - fleche[0].X, fleche[5].Y - fleche[0].Y)), format);
                int xtriangle = (fleche[2].X + fleche[4].X) / 2;
                int ytriangle = fleche[2].Y;
                G.DrawString(zoneBataille.iPertes[iZoneResultats].ToString() + " †", m_police, Brushes.Black,
                    xtriangle, ytriangle, format);
            }
            else
            {
                Brush brosse = (0 == role.iNation345) ? Brushes.Blue : Brushes.OrangeRed;
                Point[] fleche = new Point[8];
                int f = 0;
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
                fleche[f].X = fleche[0].X; fleche[f++].Y = fleche[0].Y;
                G.FillPolygon(brosse, fleche);
                G.DrawString(zoneBataille.sCombat[iZoneResultats], m_police, Brushes.Black,
                        new Rectangle(fleche[2], new Size(fleche[4].X - fleche[2].X, fleche[4].Y - fleche[2].Y)), format);
                int xtriangle = (fleche[1].X + fleche[6].X) / 2;
                int ytriangle = fleche[1].Y;
                G.DrawString(zoneBataille.iPertes[iZoneResultats].ToString() + " †", m_police, Brushes.Black,
                    xtriangle, ytriangle, format);
            }
        }

        private void DessineFondBataille(Graphics G)
        {
            if (TIPEORIENTATIONBATAILLE.VERTICAL == m_orientation)
            {
                //Bataille verticale
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

        private void TraitementVertical(int iZoneResultats, TIPEFINBATAILLE fin, bool bFin)
        { 
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
            int ximage=-1, yimage=-1;

            //emplacement du texte
            SizeF tailleTexte = G.MeasureString(unite.nom, m_police);
            Rectangle rectText = new Rectangle(x - (int)tailleTexte.Width / 2,
                                    y + image.Height * 3, // on considère qu'il y a toujours 3 lignes au maximum
                                    (int)tailleTexte.Width + 1,
                                    (int)tailleTexte.Height + 1);
            //cadre de l'unité
            Rectangle rectCadre = new Rectangle(Math.Min(rectText.Left, x - (nb_unites_largeur * image.Width) / 2) - EPAISSEUR_BORDURE,
                y - EPAISSEUR_BORDURE,
                Math.Max(rectText.Right, x + (nb_unites_largeur * image.Width) / 2) - Math.Min(rectText.Left, x - (nb_unites_largeur * image.Width) / 2) + EPAISSEUR_BORDURE * 2,
                rectText.Bottom - y + EPAISSEUR_BORDURE * 2);
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
                    yimage = y + image.Height * (pos / NB_UNITES_LARGEUR);
                    G.DrawImage(image, ximage, yimage);
                    pos++;
                }

                image = (0 == unite.iNation) ? new Bitmap(vaoc.Properties.Resources.cavalerie_0) : new Bitmap(vaoc.Properties.Resources.cavalerie_1);
                for (int i = 0; i < nb_unites_cavalerie; i++)
                {
                    ximage = x - (nb_unites_largeur * image.Width) / 2 + (pos % nb_unites_largeur) * image.Width;
                    yimage = y + image.Height * (pos / NB_UNITES_LARGEUR);
                    G.DrawImage(image, ximage, yimage);
                    pos++;
                }
                image = (0 == unite.iNation) ? new Bitmap(vaoc.Properties.Resources.artillerie_0) : new Bitmap(vaoc.Properties.Resources.artillerie_1);
                for (int i = 0; i < nb_unites_artillerie; i++)
                {
                    ximage = x - (nb_unites_largeur * image.Width) / 2 + (pos % nb_unites_largeur) * image.Width;
                    yimage = y + image.Height * (pos / NB_UNITES_LARGEUR);
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
    }
}
