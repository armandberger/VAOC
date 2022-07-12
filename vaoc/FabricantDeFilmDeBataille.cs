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
        private string m_nomCampagne;
        private int m_numeroImage;
        private Brush[] m_brossesTexte = new Brush[6];
        private const int NB_UNITES_LARGEUR = 5;
        private const int NB_UNITES_HAUTEUR = 3;
        private const int EPAISSEUR_BORDURE = 2;
        private const int NB_UNITES_PAR_LIGNE = 4;
        private int m_iNation012;
        private int m_iNation345;
        private int m_maxLongueurChaine;
        // commande dans un .bat ffmpeg -framerate 1 -i imageVideo_%%04d.png -c:v libx264 -r 30 -pix_fmt yuv420p video.mp4

        public FabricantDeFilmDeBataille()
        {
        }

        public string Initialisation(string nomCampagne, string nomFichier, string repertoireVideo, 
                                    string nomBataille, Font police, Font policeTitre, Font policeTitreEffectifs,
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
                m_nomCampagne = nomCampagne;
                m_nomFichier = nomFichier;
                m_numeroImage = 0;
                m_iNation012 = iNation012;
                m_iNation345 = iNation345;

                if (Directory.Exists(repertoireVideo))
                {
                    //on supprime toutes les images qui pourraient exister d'un précédent traitement
                    DirectoryInfo dir = new DirectoryInfo(repertoireVideo);
                    FileInfo[] listeFichiers = dir.GetFiles("*"+nomFichier+"*.png", SearchOption.TopDirectoryOnly);

                    foreach (FileInfo fichier in listeFichiers)
                    {
                        File.Delete(fichier.FullName);
                    }


                    //on supprime également toutes les vidéos précédentes
                    listeFichiers = dir.GetFiles("*" + nomFichier + "*.mp4", SearchOption.TopDirectoryOnly);
                    foreach (FileInfo fichier in listeFichiers)
                    {
                        File.Delete(fichier.FullName);
                    }
                }
                else
                {
                    Directory.CreateDirectory(repertoireVideo);
                }

                TraitementTitre(nomBataille, debut, policeTitre, policeTitreEffectifs);
                for (m_traitement = debut; m_traitement< debut+m_nbEtapes; m_traitement++)
                {
                    //on  vérifie qu'il y a bien une unité à afficher (des fois il n'y a rien car c'est un inter tour de combat)
                    //on compte d'abord le nombre d'unites par zone pour pouvoir les répartir au mieux et s'assurer d'une action
                    int u = 0;
                    while (u < m_unitesBataille.Count && m_unitesBataille[u].iTour != m_traitement) u++;
                    if (u < m_unitesBataille.Count)
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
                    string retour;
                    switch (m_orientation)
                    {
                        case TIPEORIENTATIONBATAILLE.VERTICAL:
                            retour = TraitementVertical(zone, m_fin, true);
                            break;
                        default:
                            retour = TraitementHorizontal(zone, m_fin, true);
                            break;
                    }
                    if (string.Empty != retour)
                    {
                        return retour;
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
                Arguments = string.Format("-framerate 1 -i {0}_{1}_%04d.png -c:v libx264 -r 30 -pix_fmt yuv420p {0}_{1}.mp4", m_nomCampagne.Replace(' ', '_'), m_nomFichier)
            };
            Process.Start(processInfo);
        }

        public string TraitementHorizontal(int iZoneResultats, TIPEFINBATAILLE fin, bool bFin)
        {
            Graphics G;
            int xunite, yunite;
            //pour qu'il y ait action il faut qu'il y ait des unités des deux cotés
            bool baction = false;
            try
            {
                //recherche des leaders à ce moment de la bataille
                //RoleBataille role = null;
                //if (m_rolesBataille.Count > 0)
                //{
                //    int i = 0;
                //    while (m_rolesBataille[i].iTour != m_traitement) i++;
                //    role = m_rolesBataille[i];
                //}

                //recherche des unites
                int[] reserve = new int[2];
                int[] nbReserve = new int[2];
                int[] zone = new int[6];
                int[] nbZone = new int[6];
                //on compte d'abord le nombre d'unites par zone pour pouvoir les répartir au mieux et s'assurer d'une action
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

                if (iZoneResultats < 0)
                {
                    baction = true;
                }
                else
                {
                    //y a t-il une action à representer
                    switch (fin)
                    {
                        case TIPEFINBATAILLE.RETRAITE345:
                        case TIPEFINBATAILLE.VICTOIRE012:
                            if (iZoneResultats < 3 && nbZone[iZoneResultats] > 0)
                            {
                                baction = true;
                            }
                            break;
                        case TIPEFINBATAILLE.RETRAITE012:
                        case TIPEFINBATAILLE.VICTOIRE345:
                            if (iZoneResultats > 2 && nbZone[iZoneResultats] > 0)
                            {
                                baction = true;
                            }
                            break;
                        default:
                            if (nbZone[iZoneResultats] > 0 && nbZone[iZoneResultats % 3 + 3] > 0)
                            {
                                baction = true;
                            }
                            break;
                    }
                }

                //affichage
                if (baction)
                {
                    Bitmap fichierImage = new Bitmap(m_largeur, m_hauteur, PixelFormat.Format24bppRgb);
                    G = Graphics.FromImage(fichierImage);
                    G.PageUnit = GraphicsUnit.Pixel;

                    m_maxLongueurChaine = RechercheLongueurMaximale(G, m_police, m_largeur / 3 / NB_UNITES_PAR_LIGNE);

                    DessineFondBataille(G);

                    if (iZoneResultats >= 0)
                    {
                        foreach (ZoneBataille zoneBataille in m_zonesBataille)
                        {
                            if (zoneBataille.iTour != m_traitement
                                || null == zoneBataille.sCombat[iZoneResultats]
                                || zoneBataille.sCombat[iZoneResultats] == string.Empty
                                ) { continue; }
                            DessineFlecheHorizontale(G, iZoneResultats, zoneBataille, fin);
                        }
                    }

                    //on peut dessiner les unités maintenant
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
                                                (int)((reserve[unite.iNation] + 0.5) * m_largeur / nbReserve[unite.iNation]),
                                                m_hauteur / 9 / 2,
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

                    SauvegardeImage(fichierImage);

                    G.Dispose();
                    fichierImage.Dispose();
                }
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
                return "TraitementHorizontal Exception in: " + e.ToString();
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
                AfficherHeure(G, m_traitement, m_largeur / 2, m_largeur / 9, m_largeur / 9);
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
                AfficherHeure(G, m_traitement, m_hauteur / 9 + 1, m_hauteur / 2, m_hauteur / 9);
            }
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
            unite.nom = ChaineAffiche(m_maxLongueurChaine, unite.nom);
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
                        fleche[f].Y = ((iZoneResultats) * m_hauteur / 3) + m_hauteur / 3 / 4; fleche[f++].X = 13 * m_largeur / 18;//bas gauche
                        fleche[f].Y = ((iZoneResultats) * m_hauteur / 3) + m_hauteur / 3 - m_hauteur / 3 / 4; fleche[f++].X = 13 * m_largeur / 18;
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
            //pour qu'il y ait action il faut qu'il y ait des unités des deux cotés
            bool baction = false;
            try
            {
                //recherche des leaders à ce moment de la bataille
                //int i = 0;
                //while (m_rolesBataille[i].iTour != m_traitement) i++;
                //RoleBataille role = m_rolesBataille[i];

                //recherche des unites
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

                if (iZoneResultats < 0)
                {
                    baction = true;
                }
                else
                {
                    //y a t-il une action à representer
                    switch (fin)
                    {
                        case TIPEFINBATAILLE.RETRAITE345:
                        case TIPEFINBATAILLE.VICTOIRE012:
                            if (iZoneResultats < 3 && nbZone[iZoneResultats] > 0)
                            {
                                baction = true;
                            }
                            break;
                        case TIPEFINBATAILLE.RETRAITE012:
                        case TIPEFINBATAILLE.VICTOIRE345:
                            if (iZoneResultats > 2 && nbZone[iZoneResultats] > 0)
                            {
                                baction = true;
                            }
                            break;
                        default:
                            if (nbZone[iZoneResultats] > 0 && nbZone[iZoneResultats % 3 + 3] > 0)
                            {
                                baction = true;
                            }
                            break;
                    }
                }

                //affichage
                if (baction)
                {
                    Bitmap fichierImage = new Bitmap(m_largeur, m_hauteur, PixelFormat.Format24bppRgb);
                    G = Graphics.FromImage(fichierImage);
                    G.PageUnit = GraphicsUnit.Pixel;
                    m_maxLongueurChaine = RechercheLongueurMaximale(G, m_police, m_largeur / 9 / 2);

                    DessineFondBataille(G);
                    //affichage des flèches
                    if (iZoneResultats >= 0)
                    {
                        foreach (ZoneBataille zoneBataille in m_zonesBataille)
                        {
                            if (zoneBataille.iTour != m_traitement
                                || null == zoneBataille.sCombat[iZoneResultats]
                                || zoneBataille.sCombat[iZoneResultats] == string.Empty
                                ) { continue; }
                            DessineFlecheVerticale(G, iZoneResultats, zoneBataille, fin);
                        }
                    }

                    //on peut dessiner les unités maintenant
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
                                        unite.iZone * m_hauteur / 3 + (int)((zone[unite.iZone] - NB_UNITES_PAR_LIGNE + 0.5) * m_hauteur / 3 / NB_UNITES_PAR_LIGNE) :
                                        unite.iZone * m_hauteur / 3 + (int)((zone[unite.iZone] + 0.5) * m_hauteur / 3 / NB_UNITES_PAR_LIGNE);
                                }
                                DessineUniteImage(G,
                                                unite,
                                                xunite, yunite,
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

                    SauvegardeImage(fichierImage);

                    G.Dispose();
                    fichierImage.Dispose();
                }
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
                return "TraitementVertical Exception in: " + e.ToString();
            }
        }

        private string ChaineAffiche(int lgmax, string texteSource)
        {
            string texte = texteSource;
            int position = 0;
            while (texte.Length > lgmax)
            {
                position = texteSource.IndexOf(' ', position + 1);
                if (position < 0) { return texte.Substring(0,lgmax); }
                texte = texteSource.Substring(position + 1);
            }
            return texte;
        }

        private int RechercheLongueurMaximale(Graphics g, Font police, int taille)
        {
            string texte = "X";
            SizeF tailleTexte = g.MeasureString(texte, police);
            while (tailleTexte.Width < taille)
            {
                texte += "X";
                tailleTexte = g.MeasureString(texte, police);
            }
            return texte.Length - 1;
        }

        private void AfficherHeure(Graphics G, int tour, int x, int y, int taille)
        {
            double[] tableAngle = { 90,60,30,0,330,300,270,240,210,180,150,120 };
            int heure = ClassMessager.DateHeure(tour, 0).Hour % 12;
            //cadran
            Pen styloCadran = new Pen(Color.Black, 8);
            G.FillEllipse(Brushes.White, x - taille/2, y - taille / 2, taille, taille);
            G.DrawEllipse(styloCadran, x - taille / 2, y - taille / 2, taille, taille);
            //aiguille
            Pen styloAiguille = new Pen(Color.Black, 3);
            int xFinAiguille = (int)(x + taille / 3 * Math.Cos(tableAngle[heure] * 2 * Math.PI/360));
            int yFinAiguille = (int)(y + taille / 3 * -1 * Math.Sin(tableAngle[heure] * 2 * Math.PI / 360));//il faut inverser le signe car l'axe des y est du haut vers le bas
            G.DrawLine(styloAiguille, x, y, xFinAiguille, yFinAiguille);
        }

        private string TraitementTitre(string nomBataille, int debut, Font policeTitre, Font policeTitreEffectifs)
        {
            Graphics G;
            int xunite, yunite;
            try
            {
                //recherche des leaders durant la bataille
                List<string>[] rolesBataille = new List<string>[2];
                for (int i = 0; i < 2; i++) { rolesBataille[i] = new List<string>(); }
                foreach (RoleBataille role in m_rolesBataille)
                {
                    if (!role.nomLeader012.Equals(string.Empty) && !rolesBataille[0].Contains(role.nomLeader012)) { rolesBataille[0].Add(role.nomLeader012); }
                    if (!role.nomLeader345.Equals(string.Empty) && !rolesBataille[1].Contains(role.nomLeader345)) { rolesBataille[1].Add(role.nomLeader345); }
                }

                //recherche des effectifs max
                int[] infanterieMax = new int[2];
                int[] cavalerieMax = new int[2];
                int[] artillerieMax = new int[2];
                int[] nationCote = new int[2];

                //on compte d'abord le nombre d'unites par zone pour pouvoir les répartir au mieux
                for (int t = debut; t < debut + m_nbEtapes; t++)
                {
                    int[] infanterie = new int[2];
                    int[] cavalerie = new int[2];
                    int[] artillerie = new int[2];
                    foreach (UniteBataille unite in m_unitesBataille)
                    {
                        if (unite.iTour != t) { continue; }
                        if (unite.iZone < 3)
                        {
                            infanterie[0] += unite.effectifInfanterie;
                            cavalerie[0] += unite.effectifCavalerie;
                            artillerie[0] += unite.effectifArtillerie;
                            nationCote[0] = unite.iNation;
                        }
                        else
                        {
                            infanterie[1] += unite.effectifInfanterie;
                            cavalerie[1] += unite.effectifCavalerie;
                            artillerie[1] += unite.effectifArtillerie;
                            nationCote[1] = unite.iNation;
                        }
                    }
                    for (int i = 0; i < 2; i++)
                    {
                        infanterieMax[i] = Math.Max(infanterieMax[i], infanterie[i]);
                        cavalerieMax[i] = Math.Max(cavalerieMax[i], cavalerie[i]);
                        artillerieMax[i] = Math.Max(artillerieMax[i], artillerie[i]);
                    }
                }

                //affichage
                Bitmap fichierImage = new Bitmap(m_largeur, m_hauteur, PixelFormat.Format24bppRgb);
                G = Graphics.FromImage(fichierImage);
                G.PageUnit = GraphicsUnit.Pixel;

                //mettre la texture sur le fond
                TextureFondBataille(G, "titrebataille.jpg", new Rectangle(0,0, m_largeur, m_hauteur));

                //afficher le titre
                AfficheMultiLigne(G, nomBataille, policeTitre, new Rectangle(0, 0, m_largeur, m_hauteur / 5));

                //afficher la date
                string titreDate = ClassMessager.DateHeure(debut, 0).ToString("dddd d MMMM yyyy");
                AfficheMultiLigne(G, titreDate, policeTitre, new Rectangle(0, 4 * m_hauteur / 5, m_largeur, m_hauteur / 5));

                //afficher les protagonistes + effectifs
                for (int nation=0; nation < 2; nation++)
                {
                    //affichage des leaders
                    if (0==rolesBataille[nation].Count)
                    {
                        AfficheOfficierBataille(G, string.Empty, policeTitreEffectifs,
                            new Rectangle(nation * m_largeur / 2, m_hauteur / 5,
                            m_largeur / 2, 2 * m_hauteur / 5));
                    }
                    else
                    {
                        for (int l = 0; l < rolesBataille[nation].Count; l++)
                        {
                            AfficheOfficierBataille(G, rolesBataille[nation][l], policeTitreEffectifs, 
                                new Rectangle(nation * m_largeur / 2 + l * m_largeur / 2 / rolesBataille[nation].Count, m_hauteur / 5,
                                m_largeur / 2 / rolesBataille[nation].Count, 2 * m_hauteur / 5));
                        }
                    }

                    //affichage des effectifs
                    //on centre par rapport à la taille max des effectifs
                    SizeF tailleTexteBase = G.MeasureString("00 000 ", policeTitreEffectifs);
                    SizeF tailleTexteFinal = G.MeasureString(infanterieMax[nation].ToString("N0") +" ", policeTitreEffectifs);

                    Bitmap image = (0 == nationCote[nation]) ? new Bitmap(vaoc.Properties.Resources.infanterie_0) : new Bitmap(vaoc.Properties.Resources.infanterie_1);
                    xunite = nation* m_largeur / 2 + (m_largeur / 2 - (int)tailleTexteBase.Width - image.Width) /2 + (int)(tailleTexteBase.Width- tailleTexteFinal.Width) / 2;
                    yunite = m_hauteur * 3 / 5 + (int)(tailleTexteBase.Height/2);
                    G.DrawString(infanterieMax[nation].ToString("N0"), policeTitreEffectifs, Brushes.Black, xunite, yunite);
                    G.DrawImage(image, xunite + tailleTexteFinal.Width, yunite);

                    tailleTexteFinal = G.MeasureString(cavalerieMax[nation].ToString("N0") + " ", policeTitreEffectifs);
                    image = (0 == nationCote[nation]) ? new Bitmap(vaoc.Properties.Resources.cavalerie_0) : new Bitmap(vaoc.Properties.Resources.cavalerie_1);
                    xunite = nation * m_largeur / 2 + (m_largeur / 2 - (int)tailleTexteBase.Width - image.Width) / 2 + (int)(tailleTexteBase.Width - tailleTexteFinal.Width) / 2;
                    yunite += (int)(tailleTexteFinal.Height * 3/2);
                    G.DrawString(cavalerieMax[nation].ToString("N0"), policeTitreEffectifs, Brushes.Black, xunite, yunite);
                    G.DrawImage(image, xunite + tailleTexteFinal.Width, yunite);

                    tailleTexteFinal = G.MeasureString(artillerieMax[nation].ToString("N0") + " ", policeTitreEffectifs);
                    image = (0 == nationCote[nation]) ? new Bitmap(vaoc.Properties.Resources.artillerie_0) : new Bitmap(vaoc.Properties.Resources.artillerie_1);
                    xunite = nation * m_largeur / 2 + (m_largeur / 2 - (int)tailleTexteBase.Width - image.Width) / 2 + (int)(tailleTexteBase.Width - tailleTexteFinal.Width) / 2;
                    yunite += (int)(tailleTexteFinal.Height * 3 / 2);
                    G.DrawString(artillerieMax[nation].ToString("N0"), policeTitreEffectifs, Brushes.Black, xunite, yunite);
                    G.DrawImage(image, xunite + tailleTexteFinal.Width, yunite);
                }

                SauvegardeImage(fichierImage);

                G.Dispose();
                fichierImage.Dispose();
                return string.Empty;
            }
            catch (Exception e)
            {
                return "TraitementTitre Exception in: " + e.ToString();
            }
        }

        private void AfficheOfficierBataille(Graphics G, string nom, Font policeNom, Rectangle rect)
        {
            SizeF tailleTexte = G.MeasureString(nom, policeNom);
            Image imageOfficier = Bitmap.FromFile(AppContext.BaseDirectory+"images\\" + NomRoleFichier(nom));
            //calcul des positions, de la taille, centrage
            int largeurImage = rect.Width;
            int hauteurImage = rect.Height - (int)tailleTexte.Height;
            //taille finale image en gardant les proportions
            float rapportLargeur = (float)largeurImage / imageOfficier.Width;
            float rapportHauteur = (float)hauteurImage / imageOfficier.Height;
            if (rapportLargeur > rapportHauteur)
            {
                largeurImage = (int)(imageOfficier.Width * rapportHauteur);
                hauteurImage = (int)(imageOfficier.Height * rapportHauteur);
            }
            else
            {
                largeurImage = (int)(imageOfficier.Width * rapportLargeur);
                hauteurImage = (int)(imageOfficier.Height * rapportLargeur);
            }
            Rectangle rectImage = new Rectangle(rect.X + (rect.Width - largeurImage) / 2, 
                                                rect.Y + (rect.Height - hauteurImage - (int)tailleTexte.Height)/2, 
                                                largeurImage, hauteurImage);
            G.DrawImage(imageOfficier, rectImage);
            G.DrawString(nom, policeNom, Brushes.Black, rect.X + (rect.Width - (int)tailleTexte.Width) / 2, rectImage.Bottom);
        }

        private void AfficheMultiLigne(Graphics G, string texteSource, Font police, Rectangle rect)
        {
            string[] textes = texteSource.Split(' ');
            int largeurLigne = 0;
            int[] largeursLigne = new int[10];//dix lignes max...
            int nbLignes = 1;
            int t = 0;
            //calcul du nombre de lignes et de leur largeur
            while (t<textes.Count())
            {
                int largeurMot = (int)G.MeasureString(textes[t] + " ", police).Width;
                if (largeurLigne + largeurMot >= rect.Width)
                {
                    nbLignes++;
                    largeurLigne = 0;
                }
                largeurLigne += largeurMot;
                largeursLigne[nbLignes - 1] = largeurLigne;
                t++;
            }

            //ecriture du titre
            int pos = 0;
            t = 0;
            int ligne = 0;
            while (t < textes.Count())
            {                
                if (pos + (int)G.MeasureString(textes[t] + " ", police).Width >= rect.Width)
                {
                    ligne++;
                    pos = 0;
                }
                else
                {
                    int x = rect.X + pos + (rect.Width - largeursLigne[ligne]) / 2;
                    int y = rect.Y + ligne * rect.Height / nbLignes;
                    G.DrawString(textes[t], police, Brushes.Black, x, y);
                    pos += (int)G.MeasureString(textes[t] + " ", police).Width;
                    t++;
                }
            }
        }

        private void TextureFondBataille(Graphics G, string fichierTexture, Rectangle rect)
        {
            Image image = Bitmap.FromFile(AppContext.BaseDirectory + "images\\" + fichierTexture);
            TextureBrush brosse = new TextureBrush(image);
            G.FillRectangle(brosse, rect);
        }

        /// <summary>
        /// Détermine le nom du fichier du rôle affiché sur la vidéo
        /// </summary>
        /// <param name="nom">nom de l'unité de base</param>
        /// <returns>>Nom contracté</returns>
        private string NomRoleFichier(string nom)
        {
            if (string.Empty == nom ) { return "aucun_chef.png"; }
            //le dernier mot
            //int pos = Math.Max(nom.LastIndexOf(' '), nom.LastIndexOf('\'')) + 1;
            //return nom.Substring(pos, nom.Length - pos) + ".jpg";

            //les blancs remplacés par des soulignés et tout en minuscule sans accents
            byte[] tempBytes;
            tempBytes = System.Text.Encoding.GetEncoding("ISO-8859-8").GetBytes(nom);
            string asciiStr = System.Text.Encoding.UTF8.GetString(tempBytes);
            return asciiStr.Replace(" ", "_").Replace("'", "_").ToLower() + ".jpg";
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
            fichierImage.Save(m_repertoireVideo + "\\" + m_nomCampagne.Replace(' ','_') + "_" + m_nomFichier
                                + "_" + m_numeroImage++.ToString("0000")
                                + ".png", ImageFormat.Png);
        }

    }
}

