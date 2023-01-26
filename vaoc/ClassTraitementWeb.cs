using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;

using WaocLib;

namespace vaoc
{
    class ClassTraitementWeb
    {
        protected string m_repertoireSource;
        //private Bitmap m_imageCarte;
        //private Bitmap m_imageCarteGris;
        //private Bitmap m_imageCarteZoom;

        public ClassTraitementWeb(string fichierCourant)
        {
            LogFile.CreationLogFile("web", Donnees.m_donnees.TAB_PARTIE[0].I_TOUR, Donnees.m_donnees.TAB_PARTIE[0].I_PHASE);
            int positionPoint = fichierCourant.LastIndexOf("\\");
            m_repertoireSource = fichierCourant.Substring(0, positionPoint);
        }

        private string RepertoireTour()
        {
            return RepertoireTour(Donnees.m_donnees.TAB_PARTIE[0].I_TOUR);
        }

        private string RepertoireTour(int tour)
        {
            string repertoireTour = string.Format("{0}\\{1}_{2}_{3}",
                m_repertoireSource,
                Donnees.m_donnees.TAB_JEU[0].S_NOM.Replace(" ", ""),
                Donnees.m_donnees.TAB_PARTIE[0].S_NOM.Replace(" ", ""),
                tour);
            if (!Directory.Exists(repertoireTour))
            {
                Directory.CreateDirectory(repertoireTour);
            }
            return repertoireTour;
        }

        public bool GenerationWeb()
        {
            string repertoireTour, repertoireCarte, repertoireHistorique;
            string nomfichier;
            Rectangle rect, rectGris;
            int vision, visionGris;
            int i, j;
            int largeur, hauteur;

            LogFile.Notifier("Début de GenerationWeb");
            //création du repertoire dedié au tour
            repertoireTour = RepertoireTour();

            //création du repertoire dans lequel on met la carte générale            
            repertoireCarte = string.Format("{0}\\{1}_{2}_carte",
                m_repertoireSource,
                Donnees.m_donnees.TAB_JEU[0].S_NOM.Replace(" ", ""),
                Donnees.m_donnees.TAB_PARTIE[0].S_NOM.Replace(" ", ""));
            if (!Directory.Exists(repertoireCarte))
            {
                Directory.CreateDirectory(repertoireCarte);
            }

            //création du repertoire dans lequel on met la carte générale pour l'historique de fin de partie
            repertoireHistorique = string.Format("{0}\\{1}_{2}_historique",
                m_repertoireSource,
                Donnees.m_donnees.TAB_JEU[0].S_NOM.Replace(" ", ""),
                Donnees.m_donnees.TAB_PARTIE[0].S_NOM.Replace(" ", ""));
            if (!Directory.Exists(repertoireHistorique))
            {
                Directory.CreateDirectory(repertoireHistorique);
            }

            
            if (!Cartographie.ChargerLesFichiers()) { return false; }

            #region fichier général segmenté
            //fichier standard historique
            largeur = Cartographie.GetImageLargeur(Constantes.MODELESCARTE.HISTORIQUE);
            hauteur = Cartographie.GetImageHauteur(Constantes.MODELESCARTE.HISTORIQUE);
            //for (i = 0; i < (largeur / Donnees.m_donnees.TAB_PARTIE[0].I_LARGEUR_CARTE_ZOOM_WEB) + 1; i++)
            //il faut la carte soit un multiple parfait de la taille web sinon cela s'affiche mal sur internet
            for (i = 0; i < (largeur / Donnees.m_donnees.TAB_PARTIE[0].I_LARGEUR_CARTE_ZOOM_WEB); i++)
            {
                for (j = 0; j < (hauteur / Donnees.m_donnees.TAB_PARTIE[0].I_HAUTEUR_CARTE_ZOOM_WEB); j++)
                //for (j = 0; j < (hauteur / Donnees.m_donnees.TAB_PARTIE[0].I_HAUTEUR_CARTE_ZOOM_WEB) + 1; j++)
                {
                    rect = new Rectangle(i * Donnees.m_donnees.TAB_PARTIE[0].I_LARGEUR_CARTE_ZOOM_WEB,
                            j * Donnees.m_donnees.TAB_PARTIE[0].I_HAUTEUR_CARTE_ZOOM_WEB,
                            Donnees.m_donnees.TAB_PARTIE[0].I_LARGEUR_CARTE_ZOOM_WEB,
                            Donnees.m_donnees.TAB_PARTIE[0].I_HAUTEUR_CARTE_ZOOM_WEB);

                    nomfichier = string.Format("{0}\\carte_{1}_{2}.png",
                        repertoireCarte,
                        i, j);
                    if (!Cartographie.DecoupeFichier(Constantes.MODELESCARTE.HISTORIQUE, nomfichier, rect, 1)) { return false; }
                }
            }

            //fichier zoom
            if (null == Cartographie.GetImage(Constantes.MODELESCARTE.ZOOM))
            {
                largeur = Cartographie.GetImageLargeur(Constantes.MODELESCARTE.HISTORIQUE) * Constantes.CST_FACTEUR_ZOOM;
                hauteur = Cartographie.GetImageHauteur(Constantes.MODELESCARTE.HISTORIQUE) * Constantes.CST_FACTEUR_ZOOM;
                //for (i = 0; i < (largeur / Donnees.m_donnees.TAB_PARTIE[0].I_LARGEUR_CARTE_ZOOM_WEB) + 1; i++)
                //il faut la carte soit un multiple parfait de la taille web sinon cela s'affiche mal sur internet
                for (i = 0; i < (largeur / Donnees.m_donnees.TAB_PARTIE[0].I_LARGEUR_CARTE_ZOOM_WEB); i++)
                {
                    for (j = 0; j < (hauteur / Donnees.m_donnees.TAB_PARTIE[0].I_HAUTEUR_CARTE_ZOOM_WEB); j++)
                    {
                        rect = new Rectangle(i * Donnees.m_donnees.TAB_PARTIE[0].I_LARGEUR_CARTE_ZOOM_WEB / Constantes.CST_FACTEUR_ZOOM,
                                j * Donnees.m_donnees.TAB_PARTIE[0].I_HAUTEUR_CARTE_ZOOM_WEB / Constantes.CST_FACTEUR_ZOOM,
                                Donnees.m_donnees.TAB_PARTIE[0].I_LARGEUR_CARTE_ZOOM_WEB / Constantes.CST_FACTEUR_ZOOM,
                                Donnees.m_donnees.TAB_PARTIE[0].I_HAUTEUR_CARTE_ZOOM_WEB / Constantes.CST_FACTEUR_ZOOM);

                        nomfichier = string.Format("{0}\\carteZoom_{1}_{2}.png",
                            repertoireCarte,
                            i, j);
                        if (!Cartographie.DecoupeFichier(Constantes.MODELESCARTE.HISTORIQUE, nomfichier, rect, Constantes.CST_FACTEUR_ZOOM)) { return false; }
                    }
                }
            }
            else
            {
                largeur = Cartographie.GetImageLargeur(Constantes.MODELESCARTE.ZOOM);
                hauteur = Cartographie.GetImageHauteur(Constantes.MODELESCARTE.ZOOM);
                //il faut la carte soit un multiple parfait de la taille web sinon cela s'affiche mal sur internet
                //for (i = 0; i < (largeur / Donnees.m_donnees.TAB_PARTIE[0].I_LARGEUR_CARTE_ZOOM_WEB); i++)
                for (i = 0; i < (largeur / Donnees.m_donnees.TAB_PARTIE[0].I_LARGEUR_CARTE_ZOOM_WEB); i++)
                {
                    for (j = 0; j < (hauteur / Donnees.m_donnees.TAB_PARTIE[0].I_HAUTEUR_CARTE_ZOOM_WEB); j++)
                    {
                        rect = new Rectangle(i * Donnees.m_donnees.TAB_PARTIE[0].I_LARGEUR_CARTE_ZOOM_WEB,
                                j * Donnees.m_donnees.TAB_PARTIE[0].I_HAUTEUR_CARTE_ZOOM_WEB,
                                Donnees.m_donnees.TAB_PARTIE[0].I_LARGEUR_CARTE_ZOOM_WEB,
                                Donnees.m_donnees.TAB_PARTIE[0].I_HAUTEUR_CARTE_ZOOM_WEB);

                        nomfichier = string.Format("{0}\\carteZoom_{1}_{2}.png",
                            repertoireCarte,
                            i, j);
                        if (!Cartographie.DecoupeFichier(Constantes.MODELESCARTE.ZOOM, nomfichier, rect, 1)) { return false; }
                    }
                }
            }
            #endregion

            //ajout des unités et des noms sur les cartes
            //Cartographie.InitialisationProprietaires(); -> devenu inutile je pense puisque les propriétaires sont conservés
            //Cartographie.ConstructionCarte(); -> fait à la fin du tour, et met le bazar ensuite, car fait comme si on était au début du tour suivant
            Cartographie.AfficherUnites(Constantes.MODELESCARTE.HISTORIQUE);
            Cartographie.AfficherUnites(Constantes.MODELESCARTE.ZOOM);

            #region fichiers des unités
            foreach (Donnees.TAB_PIONRow lignePion in Donnees.m_donnees.TAB_PION)
            {
                //Donnees.TAB_MESSAGERow ligneMessage = Donnees.m_donnees.TAB_MESSAGE.DernierMessageRecu(lignePion.ID_PION, lignePion.IsID_PION_PROPRIETAIRENull() ? -1 : lignePion.ID_PION_PROPRIETAIRE);
                Donnees.TAB_MESSAGERow ligneMessage = Donnees.m_donnees.TAB_MESSAGE.DernierMessageRecu(lignePion.ID_PION, lignePion.ID_PION_PROPRIETAIRE);

                //il faut générer un fichier avec la position courante à tous les tours
                //et ne fournir au joueur que celui correspondant à celle du dernier message reçu
                if (lignePion.estMessager || lignePion.estPatrouille)
                {
                    continue;//on ne donne pas la position des messagers et des patrouilles
                }

                LogFile.Notifier(string.Format("Images pour le pion ID_PION={0} : {1}",
                    lignePion.ID_PION, lignePion.S_NOM));
                if (null!= ligneMessage && ligneMessage.B_DETRUIT) { continue; }
                Donnees.TAB_MODELE_PIONRow ligneModelePion = lignePion.modelePion;
                vision = Math.Max(ligneModelePion.I_VISION_NUIT, ligneModelePion.I_VISION_JOUR);
                if (Donnees.m_donnees.TAB_PARTIE.Nocturne())
                {
                    vision = ligneModelePion.I_VISION_NUIT;
                }
                else
                {
                    vision = ligneModelePion.I_VISION_JOUR;
                }
                visionGris = 2 * Math.Max(ligneModelePion.I_VISION_NUIT, ligneModelePion.I_VISION_JOUR);

                //fichier standard du pion (historique)
                //recopie du fichier généré lors du dernier message

                string nomfichierBackup = string.Format("{0}\\pion_{1}.png",
                    repertoireTour,
                    lignePion.ID_PION);

                //fichier pion (historique) qui sera peut-être repris par un autre message plus tard
                Donnees.TAB_CASERow ligneCase = Donnees.m_donnees.TAB_CASE.FindParID_CASE(lignePion.ID_CASE);
                rect = new Rectangle(ligneCase.I_X - vision * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE,
                        ligneCase.I_Y - vision * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE,
                        vision * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE * 2,
                        vision * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE * 2);
                rectGris = new Rectangle(ligneCase.I_X - visionGris * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE,
                        ligneCase.I_Y - visionGris * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE,
                        visionGris * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE * 2,
                        visionGris * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE * 2);
                if (!Cartographie.FichierGrise(Constantes.MODELESCARTE.HISTORIQUE, nomfichierBackup, rect, rectGris)) { return false; }

                string nomfichierSource;
                if (null != Donnees.m_donnees.TAB_ROLE.TrouvePion(lignePion.ID_PION))
                {
                    //s'il y a un role, c'est un joueur, il faut afficher sa position reelle
                    nomfichier = string.Format("{0}\\cartepion_{1}.png",
                        repertoireTour,
                        lignePion.ID_PION);

                    nomfichierSource = string.Format("{0}\\pion_{1}.png",
                        repertoireTour,
                        lignePion.ID_PION);

                    //fichier cartepion pour le tour
                    File.Copy(nomfichierSource, nomfichier, true);
                }
                else
                {
                    if (lignePion.IsID_NOUVEAU_PION_PROPRIETAIRENull())
                    {
                        //Tant qu'id nouveau proprietaire est renseigné, le nouveau propriétaire ne doit pas voir l'unité dans son bilan
                        //la valeur est remise à vide quand l'ancien proprietaire reçoit le message/ordre du transfert
                        //Donnees.TAB_MESSAGERow ligneMessage = Donnees.m_donnees.TAB_MESSAGE.DernierMessageRecu(lignePion.ID_PION, lignePion.IsID_PION_PROPRIETAIRENull() ? -1 : lignePion.ID_PION_PROPRIETAIRE);
                        GenererFichierPion(repertoireTour, lignePion, ligneMessage, lignePion.ID_PION_PROPRIETAIRE);
                    }

                    int IdAncienProprietaire = lignePion.ID_ANCIEN_PION_PROPRIETAIRE;
                    if (Constantes.NULLENTIER != IdAncienProprietaire)
                    {
                        //Tant qu'id ancien proprietaire est renseigné, l'ancien propriétaire doit continuer à voir l'unité dans son bilan
                        //la valeur est remise à vide quand l'ancien proprietaire reçoit le message/ordre du transfert
                        ligneMessage = Donnees.m_donnees.TAB_MESSAGE.DernierMessageRecu(lignePion.ID_PION, IdAncienProprietaire);
                        GenererFichierPion(repertoireTour, lignePion, ligneMessage, IdAncienProprietaire);
                    }
                }

            }
            #endregion

            #region fichiers de vue des joueurs
            if (!GenerationWebFichiersRoles(false))
            {
                LogFile.Notifier("Erreur dans GenerationWebFichiersRoles");
                return false;
            }

            if (!GenerationWebFilmRoles())
            {
                LogFile.Notifier("Erreur dans GenerationWebFilmRoles");
                return false;
            }
            #endregion

            #region fichiers des batailles
            foreach (Donnees.TAB_BATAILLERow ligneBataille in Donnees.m_donnees.TAB_BATAILLE)
            {
                LogFile.Notifier(string.Format("Images pour la bataille ID_BATAILLE={0} : {1}",
                    ligneBataille.ID_BATAILLE, ligneBataille.S_NOM));
                rect = new Rectangle(ligneBataille.I_X_CASE_HAUT_GAUCHE,
                        ligneBataille.I_Y_CASE_HAUT_GAUCHE,
                        ligneBataille.I_X_CASE_BAS_DROITE - ligneBataille.I_X_CASE_HAUT_GAUCHE,
                        ligneBataille.I_Y_CASE_BAS_DROITE - ligneBataille.I_Y_CASE_HAUT_GAUCHE);
                //fichier vision historique de la bataille
                nomfichier = string.Format("{0}\\bataille_{1}.png",
                    repertoireTour,ligneBataille.ID_BATAILLE);
                if (!Cartographie.DecoupeFichier(Constantes.MODELESCARTE.HISTORIQUE, nomfichier, rect, 1)) { return false; }

                //fichier topographique de la bataille
                nomfichier = string.Format("{0}\\bataille_{1}_topographique.png",
                    repertoireTour, ligneBataille.ID_BATAILLE);
                if (!Cartographie.DecoupeFichier(Constantes.MODELESCARTE.TOPOGRAPHIQUE, nomfichier, rect, 1)) { return false; }
            }
            #endregion

            #region Carte général pour afficher en fin de partie
            nomfichier = string.Format("{0}\\carte_general_{1}.png", repertoireHistorique, Donnees.m_donnees.TAB_PARTIE[0].I_TOUR);
            rect = new Rectangle(0, 0, Cartographie.GetImageLargeur(Constantes.MODELESCARTE.HISTORIQUE), Cartographie.GetImageHauteur(Constantes.MODELESCARTE.HISTORIQUE));
            if (!Cartographie.DecoupeFichier(Constantes.MODELESCARTE.HISTORIQUE, nomfichier, rect, 1)) { return false; }

            if (null != Cartographie.GetImage(Constantes.MODELESCARTE.ZOOM))
            {
                nomfichier = string.Format("{0}\\carte_generalzoom_{1}.png", repertoireHistorique, Donnees.m_donnees.TAB_PARTIE[0].I_TOUR);
                rect = new Rectangle(0, 0, Cartographie.GetImageLargeur(Constantes.MODELESCARTE.ZOOM), Cartographie.GetImageHauteur(Constantes.MODELESCARTE.ZOOM));
                if (!Cartographie.DecoupeFichier(Constantes.MODELESCARTE.ZOOM, nomfichier, rect, 1)) { return false; }
            }
            #endregion

            #region Fichiers annexes et techniques
            //Fichier pour empécher le webbrowsing dans le repertoire
            string html = "<html><head><meta http-equiv=\"REFRESH\" content=\"0; url=http://vaoc.free.fr/\"></head></html>";
            nomfichier = string.Format("{0}\\index.html", repertoireTour);
            File.WriteAllText(nomfichier,html);
            nomfichier = string.Format("{0}\\index.html", repertoireCarte);
            File.WriteAllText(nomfichier, html);
            nomfichier = string.Format("{0}\\index.html", repertoireHistorique);
            File.WriteAllText(nomfichier, html);
            #endregion
            //suppression des tables d'optimisation -> pourquoi faire 20/05/2014
            //Donnees.m_donnees.NettoyageBase();

            LogFile.Notifier("Fin de GenerationWeb");
            return true;
        }

        /// <summary>
        /// Crée un fichier gif d'animation  pour chaque rôle
        /// </summary>
        /// <returns>true = Ok, false = KO</returns>
        private bool GenerationWebFilmRoles()
        {
            string nomfichier;
            string repertoireTourSource;
            string repertoireTourDestination = RepertoireTour();
            int nbPhases = Donnees.m_donnees.TAB_JEU[0].I_NOMBRE_PHASES;
            //int tour = Donnees.m_donnees.TAB_PARTIE[0].I_TOUR-1;
            int itourDebut = (Constantes.NULLENTIER == Donnees.m_donnees.TAB_PARTIE[0].I_TOUR_NOTIFICATION) ? 1 : Donnees.m_donnees.TAB_PARTIE[0].I_TOUR_NOTIFICATION;

            foreach (Donnees.TAB_ROLERow ligneRole in Donnees.m_donnees.TAB_ROLE)
            {
                LogFile.Notifier(string.Format("GenerationWebFilmRoles : Film pour le rôle ID_ROLE={0} : {1}",
                    ligneRole.ID_ROLE, ligneRole.S_NOM));
                GifAnime gif = new(string.Format("{0}\\filmrole_{1}_zoom.gif",
                        repertoireTourDestination,
                        ligneRole.ID_ROLE));
                for (int tour = itourDebut; tour < Donnees.m_donnees.TAB_PARTIE[0].I_TOUR; tour++)
                {
                    repertoireTourSource = RepertoireTour(tour);
                    for (int phase = 0; phase < nbPhases; phase += Constantes.CST_SAUVEGARDE_ECART_PHASES)
                    {
                        //fichier zoom
                        nomfichier = string.Format("{0}\\carterole_{1}_{2}_zoom.png",
                            repertoireTourSource,
                            ligneRole.ID_ROLE,
                            phase);
                        if (File.Exists(nomfichier))
                        {
                            Bitmap im = new(nomfichier);
                            AjouterHeure(im, ClassMessager.DateHeure(tour, phase).ToShortTimeString());
                            gif.WriteFrame(im);
                            im.Dispose();
                        }
                        else
                        {
                            LogFile.Notifier(string.Format("GenerationWebFilmRoles : pas d'image de film le rôle ID_ROLE={0} : {1} : {2}",
                                ligneRole.ID_ROLE, ligneRole.S_NOM, nomfichier));
                        }
                    }
                }
                gif.Dispose();
            }
            return true;
        }

        private void AjouterHeure(Bitmap im, string heure)
        {
            Graphics G;
            SizeF tailleTexte;
            float taillePolice;
            const float ecartTaillePolice = 2;
            const string nomPolice = "Arial";
            Font police;

            G = Graphics.FromImage(im);
            G.PageUnit = GraphicsUnit.Pixel;
            //bandeau avec texte
            tailleTexte = new SizeF(0, 0);
            taillePolice = 6;
            while (tailleTexte.Height < im.Height / 5)
            {
                taillePolice += ecartTaillePolice;
                police = new Font(nomPolice, taillePolice, FontStyle.Bold);
                tailleTexte = G.MeasureString("08:36", police);
            }

            police = new Font(nomPolice, taillePolice - ecartTaillePolice, FontStyle.Bold);
            tailleTexte = G.MeasureString(heure, police);

            int w = im.Width;
            int h = im.Height;
            //G.FillRectangle(Brushes.White,
            //    new Rectangle((w - (int)tailleTexte.Width) / 2, h - (int)tailleTexte.Height, (int)tailleTexte.Width, (int)tailleTexte.Height)
            //    );

            G.DrawString(heure, police, Brushes.Black, (w - (int)tailleTexte.Width) / 2, h - (int)tailleTexte.Height);
            G.Dispose();
        }

        public bool GenerationWebFichiersRoles(bool bAvecPhase)
        {
            string nomfichier;
            Rectangle rect, rectGris;
            int vision, visionGris;
            string repertoireTour = RepertoireTour();

            foreach (Donnees.TAB_ROLERow ligneRole in Donnees.m_donnees.TAB_ROLE)
            {
                LogFile.Notifier(string.Format("GenerationWebFichiersRoles : Images pour le rôle ID_ROLE={0} : {1}",
                    ligneRole.ID_ROLE, ligneRole.S_NOM));
                Donnees.TAB_PIONRow lignePion = Donnees.m_donnees.TAB_PION.FindByID_PION(ligneRole.ID_PION);
                if (null == lignePion)
                {
                    //le pion arrive peut-être en renfort mais on le signale quand même.
                    LogFile.Notifier(string.Format("GenerationWebFichiersRoles : Impossible de trouver le pion ID_PION={0} pour le rôle ID_ROLE={1}. Ce pion arrive en renfort ?",
                        ligneRole.ID_PION, ligneRole.ID_ROLE));
                }
                else
                {
                    Donnees.TAB_CASERow ligneCase = Donnees.m_donnees.TAB_CASE.FindParID_CASE(lignePion.ID_CASE);
                    Donnees.TAB_MODELE_PIONRow ligneModelePion = lignePion.modelePion;
                    if (Donnees.m_donnees.TAB_PARTIE.Nocturne())
                    {
                        vision = ligneModelePion.I_VISION_NUIT;
                    }
                    else
                    {
                        vision = ligneModelePion.I_VISION_JOUR;
                    }
                    visionGris = 2 * Math.Max(ligneModelePion.I_VISION_NUIT, ligneModelePion.I_VISION_JOUR);

                    //fichier standard du role (historique)
                    nomfichier = (bAvecPhase) ?
                        string.Format("{0}\\carterole_{1}_{2}.png",
                        repertoireTour,
                        ligneRole.ID_ROLE,
                        Donnees.m_donnees.TAB_PARTIE[0].I_PHASE)
                        :
                        string.Format("{0}\\carterole_{1}.png",
                        repertoireTour,
                        ligneRole.ID_ROLE);
                    rect = new Rectangle(ligneCase.I_X - vision * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE,
                            ligneCase.I_Y - vision * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE,
                            vision * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE * 2,
                            vision * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE * 2);
                    rectGris = new Rectangle(ligneCase.I_X - visionGris * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE,
                            ligneCase.I_Y - visionGris * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE,
                            visionGris * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE * 2,
                            visionGris * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE * 2);
                    if (!Cartographie.FichierGrise(Constantes.MODELESCARTE.HISTORIQUE, nomfichier, rect, rectGris)) { return false; }

                    //fichier topographique
                    nomfichier = (bAvecPhase) ?
                        string.Format("{0}\\carterole_{1}_{2}_topographie.png",
                        repertoireTour,
                        ligneRole.ID_ROLE,
                        Donnees.m_donnees.TAB_PARTIE[0].I_PHASE)
                        :
                        string.Format("{0}\\carterole_{1}_topographie.png",
                        repertoireTour,
                        ligneRole.ID_ROLE);
                    if (!Cartographie.FichierGrise(Constantes.MODELESCARTE.TOPOGRAPHIQUE, nomfichier, rect, rectGris)) { return false; }

                    //fichier zoom
                    nomfichier = (bAvecPhase) ?
                        string.Format("{0}\\carterole_{1}_{2}_zoom.png",
                        repertoireTour,
                        ligneRole.ID_ROLE,
                        Donnees.m_donnees.TAB_PARTIE[0].I_PHASE)
                        : 
                        string.Format("{0}\\carterole_{1}_zoom.png",
                        repertoireTour,
                        ligneRole.ID_ROLE);
                    if (null == Cartographie.GetImage(Constantes.MODELESCARTE.ZOOM))
                    {
                        //cas d'un fichier trop gros pour être chargé
                        Cartographie.AfficherUnitesZoom(nomfichier,
                                                        Math.Max(0, ligneCase.I_X - vision * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE),
                                                        Math.Max(0, ligneCase.I_Y - vision * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE),
                                                        Math.Min(Donnees.m_donnees.TAB_JEU[0].I_LARGEUR_CARTE - 1, ligneCase.I_X + vision * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE),
                                                        Math.Min(Donnees.m_donnees.TAB_JEU[0].I_HAUTEUR_CARTE - 1, ligneCase.I_Y + vision * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE)
                                                        );
                    }
                    else
                    {
                        rect = new Rectangle((int)(Cartographie.rapportZoom * (ligneCase.I_X - vision * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE)),
                                 (int)(Cartographie.rapportZoom * (ligneCase.I_Y - vision * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE)),
                                (int)(vision * Cartographie.rapportZoom * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE * 2),
                                (int)(vision * Cartographie.rapportZoom * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE * 2));
                        if (!Cartographie.DecoupeFichier(Constantes.MODELESCARTE.ZOOM, nomfichier, rect, 1)) { return false; }
                    }
                }
            }
            return true;
        }

        private void GenererFichierPion(string repertoireTour, Donnees.TAB_PIONRow lignePion, Donnees.TAB_MESSAGERow ligneMessage, int id_pion_proprietaire)
        {
            string nomfichierSource, nomfichierDestination;
            int iTourDepart;

            //ligneMessage ne doit jamais être null car il y a le premier message d'arrivée des renforts
            //en fait si, c'est possible, pour le leader général, qui, lui n'a pas de chef à qui envoyer le message !                    
            if (0 == Donnees.m_donnees.TAB_PARTIE[0].I_PHASE)
            {
                iTourDepart = (null == ligneMessage) ? Donnees.m_donnees.TAB_PARTIE[0].I_TOUR : Math.Min(ligneMessage.I_TOUR_DEPART + 1, Donnees.m_donnees.TAB_PARTIE[0].I_TOUR);//debut du tour suivant mais l'image doit bien être l'image actuelle
                //pour être totalement rigoureux, il faudrait générer une image de position basé sur l'identifiant de message, au moment où le message est généré
            }
            else
            {
                iTourDepart = (null == ligneMessage) ? Donnees.m_donnees.TAB_PARTIE[0].I_TOUR : ligneMessage.I_TOUR_DEPART;
            }

            //repertoire source de l'image du message
            string repertoireSource = string.Format("{0}\\{1}_{2}_{3}",
                m_repertoireSource,
                Donnees.m_donnees.TAB_JEU[0].S_NOM.Replace(" ", ""),
                Donnees.m_donnees.TAB_PARTIE[0].S_NOM.Replace(" ", ""),
                iTourDepart);

            nomfichierSource = string.Format("{0}\\pion_{1}.png",
                repertoireSource,
                lignePion.ID_PION);

            //fichier visible par le joueur sur le web
            nomfichierDestination = string.Format("{0}\\cartepion_{1}_{2}.png",
                repertoireTour,
                lignePion.ID_PION,
                id_pion_proprietaire);

            //fichier cartepion pour le tour, c'est à dire le fichier de pion visible pour le joueur
            File.Copy(nomfichierSource, nomfichierDestination, true);
        }
    }
}
