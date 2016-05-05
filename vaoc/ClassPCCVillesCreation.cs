using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using WaocLib;

namespace vaoc
{
    struct PCCVillesStatut
    {
        public int m_positionSource;
        public int m_positionDestination;
        public string m_statut;

        public PCCVillesStatut(int positionSource, int positionDestination, string statut)
        {
            m_positionSource = positionSource;
            m_positionDestination = positionDestination;
            m_statut = statut;
        }
    }

    class ClassPCCVillesCreation
    {
        #region Fields
        private AStarVille m_etoile = new AStarVille();
        private int m_idTrajet;
        protected LogFile m_log = null;
        private int m_traitement;//traitement principal
        private int m_sous_traitement;//sous tâche d'un traitement
        private int m_etape;//etape de traitement
        private System.ComponentModel.BackgroundWorker m_travailleur=null;
        private string m_fichierCourant;
        #endregion

        #region Events
        #endregion

        #region Properties
        private System.ComponentModel.BackgroundWorker travailleur
        {
            set { m_travailleur=value; }
            get { return m_travailleur; }
        }
        #endregion

        #region Constructors
        #endregion

        #region Public Methods
        public bool Initialisation(string fichierCourant, System.ComponentModel.BackgroundWorker worker, int etape)
        {
            LogFile.CreationLogFile(fichierCourant, "ClassPCCVillesCreation", 0, 0);
            m_fichierCourant = fichierCourant;
            travailleur = worker;
            LogFile.Notifier("ClassPCCVillesCreation");

            if (0 == etape)
            {
                if (Donnees.m_donnees.TAB_PCC_VILLES.Count > 0)
                {
                    //on interdit l'interuption durant la première étape (étape, très rapide)
                    m_idTrajet = (int)Donnees.m_donnees.TAB_PCC_VILLES.Compute("MAX(ID_TRAJET)", "");//recherche du dernier trajet cree
                    Donnees.TAB_PCC_VILLESRow[] lignePCCVille = (Donnees.TAB_PCC_VILLESRow[])Donnees.m_donnees.TAB_PCC_VILLES.Select("ID_TRAJET=" + m_idTrajet.ToString());
                    //recherche de la ville source en cours
                    int idVille = Math.Min(lignePCCVille[0].ID_VILLE_DEBUT, lignePCCVille[0].ID_VILLE_FIN);
                    int i = 0;
                    while (idVille != Donnees.m_donnees.TAB_NOMS_CARTE[i].ID_NOM) i++;
                    m_traitement = i;
                    //recherche de la ville destination en cours
                    idVille = Math.Max(lignePCCVille[0].ID_VILLE_DEBUT, lignePCCVille[0].ID_VILLE_FIN);
                    i = 0;
                    while (idVille != Donnees.m_donnees.TAB_NOMS_CARTE[i].ID_NOM) i++;
                    m_sous_traitement = i + 1;
                    m_etape = 1;
                }
                else
                {
                    m_idTrajet = m_traitement = m_sous_traitement = m_etape = 0;
                }
            }
            else
            {
                m_idTrajet = m_traitement = m_sous_traitement = 0;
                m_etape = etape;
            }
            return true;
        }

        public bool Traitement()
        {
            if (!TraitementPCC())
            {
                travailleur.ReportProgress(100, new PCCVillesStatut(-1, -1, "terminé"));//c'est fini !
                return false;
            }
            return true;
        }
        #endregion

        #region Protected Methods
        #endregion

        #region Private Methods

        /// <summary>
        /// Traitement en cours jusqu'à l'étape suivante pour la mise en place des hierarchies
        /// </summary>
        /// <returns>false si un problème a eut lieu ou si le traitement est terminé</returns>
        private bool TraitementPCC()
        {
            try
            {
                switch (m_etape)
                {
                case 0://Etalement des zones de ville standard
                case 3://reparation de fichier, on commence par refaire l'étalement
                    /*int i;
                    i = 0;
                    while (i < Donnees.m_donnees.TAB_NOMS_CARTE.Count && Donnees.m_donnees.TAB_NOMS_CARTE[i].ID_NOM != 26) i++;
                    int posiFreising = i;
                    i = 0;
                    while (i < Donnees.m_donnees.TAB_NOMS_CARTE.Count && Donnees.m_donnees.TAB_NOMS_CARTE[i].ID_NOM != 50) i++;
                    int posiOberding = i;

                    this.CalculCheminVilles(posiFreising, posiOberding);
                    return false; */
                    
                    if (m_traitement < Donnees.m_donnees.TAB_NOMS_CARTE.Count)
                    {
                        if (0 == m_traitement) { travailleur.ReportProgress(0, new PCCVillesStatut(0, 0, "Etalement des villes")); }
                        if (!this.CreationEtalementVille(m_traitement)) { return false; }
                        travailleur.ReportProgress(0, new PCCVillesStatut(m_traitement, m_sous_traitement, ""));
                        m_traitement++;
                    }
                    else
                    {
                        m_etape++;
                        m_traitement = 0;
                        m_sous_traitement = m_traitement + 1;
                    }
                    break;
                case 1:
                    //Calcul des PCC entre les points de frontière du bloc
                    if (0 == m_traitement && m_sous_traitement <= 1) { travailleur.ReportProgress(0, new PCCVillesStatut(0, 0, "Calcul des chemins entre villes")); }
                    if (m_traitement < Donnees.m_donnees.TAB_NOMS_CARTE.Count)
                    {
                        if (m_sous_traitement < Donnees.m_donnees.TAB_NOMS_CARTE.Count)
                        {
                            if (!this.CalculCheminVilles(m_traitement, m_sous_traitement)) { return false; }
                            travailleur.ReportProgress(0, new PCCVillesStatut(m_traitement, m_sous_traitement, ""));
                            m_sous_traitement++;
                        }
                        else
                        {
                            m_traitement++;
                            m_sous_traitement = m_traitement + 1;
                            //sauvegarde du backup en cas de crash
                            Dal.SauvegarderPartie(m_fichierCourant, Donnees.m_donnees.TAB_PARTIE[0].I_TOUR, m_traitement, Donnees.m_donnees);
                            LogFile.CreationLogFile(m_fichierCourant, "ClassPCCVillesCreation", m_traitement, 0);
                            travailleur.ReportProgress(0, new PCCVillesStatut(m_traitement, m_sous_traitement, "Calcul des chemins entre villes"));
                        }
                    }
                    else
                    {
                        m_etape++;
                        m_traitement = m_sous_traitement = 0;
                    }
                    break;
                case 2:
                    return false;//traitement terminé
                case 4:
                    if (0 == m_traitement) 
                    {
                        Donnees.m_donnees.TAB_PCC_VILLES.Clear();
                        m_traitement++;//idtrajet commence à 1
                    }
                    int progression = ReparerTablePCC(m_traitement);
                    if (progression < 0)
                    {
                        //on a traité le dernier fichier
                        m_etape++;
                        m_traitement = m_sous_traitement = 0;
                    }
                    else
                    {
                        travailleur.ReportProgress(0, new PCCVillesStatut(progression, 0, "Reparer la table PCC"));
                        m_traitement++;
                    }
                    break;
                case 5:
                    return false;//traitement terminé
                default:
                    throw new Exception("TraitementPCC : On essaye de traiter une étape inexistante !");
                }
            }
            catch(Exception e)
            {
                LogFile.Notifier("TraitementPCC : exception =" + e.Message);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Etalement des points de la ville autour de celle-ci
        /// </summary>
        /// <param name="idVille">ville etalée</param>
        /// <returns>true si OK, false si KO</returns>
        private bool CreationEtalementVille(int posiVille)
        {
            LogFile.Notifier(string.Format("CreationEtalementVille idVille={0}", posiVille));
            int x,y;
            int xmin, xmax, ymin, ymax;
            try
            {
                Donnees.TAB_NOMS_CARTERow ligneVille=Donnees.m_donnees.TAB_NOMS_CARTE[posiVille];
                Donnees.TAB_CASERow ligneCaseVille = Donnees.m_donnees.TAB_CASE.FindByID_CASE(ligneVille.ID_CASE);
                //on affecte toutes les cases autour du nom, (du même modèle que l'emplacement du nom ? pas pour l'instant)
                xmin = Math.Max(0,ligneCaseVille.I_X - Donnees.m_donnees.TAB_JEU[0].I_ZONEVILLE_PCC);
                xmax = Math.Min(Donnees.m_donnees.TAB_JEU[0].I_LARGEUR_CARTE, ligneCaseVille.I_X + Donnees.m_donnees.TAB_JEU[0].I_ZONEVILLE_PCC);
                ymin = Math.Max(0,ligneCaseVille.I_Y - Donnees.m_donnees.TAB_JEU[0].I_ZONEVILLE_PCC);
                ymax = Math.Min(Donnees.m_donnees.TAB_JEU[0].I_HAUTEUR_CARTE,ligneCaseVille.I_Y + Donnees.m_donnees.TAB_JEU[0].I_ZONEVILLE_PCC);
                for (x = xmin; x <= xmax; x++)
                {
                    for (y = ymin; y <= ymax; y++)
                    {
                        Donnees.TAB_CASERow ligneCase = Donnees.m_donnees.TAB_CASE.FindByXY(x,y);
                        ligneCase.ID_NOM = ligneVille.ID_NOM;
                    }
                }
            }
            catch(Exception e)
            {
                LogFile.Notifier("CreationEtalementVille : exception =" + e.Message);
                return false;
            }
            return true;
        }

          /// <summary>
        /// Calcul d'un chemin, d'une ville, vers une autre
        /// </summary>
        /// <param name="posiVilleSource">position du nom de départ dans la table</param>
        /// <param name="posiVilleDestination">position du nom d'arrivée dans la table</param>
        /// <returns>true si OK, false si KO</returns>
        private bool CalculCheminVilles(int posiVilleSource, int posiVilleDestination)
        {
            Donnees.TAB_NOMS_CARTERow ligneVilleS, ligneVilleD;
            ligneVilleS = Donnees.m_donnees.TAB_NOMS_CARTE[posiVilleSource];
            ligneVilleD = Donnees.m_donnees.TAB_NOMS_CARTE[posiVilleDestination];
            return CalculCheminVilles(ligneVilleS, ligneVilleD);
        }

        private bool CalculCheminVilles(Donnees.TAB_NOMS_CARTERow ligneVilleS, Donnees.TAB_NOMS_CARTERow ligneVilleD)
        {
            int k, i, j;
            DateTime timeStart;
            TimeSpan perf;
            Donnees.TAB_CASERow ligneCaseVilleS, ligneCaseVilleD;//ligneCaseVilleI, ligneCaseVilleP
            List<int> listeCasesTrajet = new List<int>();
            List<Donnees.TAB_NOMS_CARTERow> listeVilles = new List<Donnees.TAB_NOMS_CARTERow>();
            double distance;
            double maxdistance=Donnees.m_donnees.TAB_JEU[0].I_DISTANCEVILLEMAX_PCC * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE;
            Donnees.TAB_NOMS_CARTERow ligneVilleP;//ligneVilleI
            bool bRoutier;
            string requete;

            try
            {
                if (ligneVilleS.ID_NOM == 9 && ligneVilleD.ID_NOM == 156)
                {
                    Debug.WriteLine("exception ?");
                }
                //on regarde si les deux villes sont trop éloignées pour tenter une recherche
                ligneCaseVilleS = Donnees.m_donnees.TAB_CASE.FindByID_CASE(ligneVilleS.ID_CASE);
                ligneCaseVilleD = Donnees.m_donnees.TAB_CASE.FindByID_CASE(ligneVilleD.ID_CASE);
                LogFile.Notifier(string.Format("CalculCheminVilles source={0}({1}), destination={2}({3})",
                    ligneVilleS.S_NOM, ligneVilleS.ID_NOM, ligneVilleD.S_NOM, ligneVilleD.ID_NOM));
                
                distance = Constantes.Distance(ligneCaseVilleS.I_X, ligneCaseVilleS.I_Y, ligneCaseVilleD.I_X, ligneCaseVilleD.I_Y);
                if (distance > maxdistance)
                {
                    LogFile.Notifier(string.Format("Distance trop importante, {0} pixels pour un maximum autorisé de {1}",
                        distance, maxdistance));
                    return true;
                }

                //On regarde si le chemin n'a pas déjà été traité et mis en echec
                requete = string.Format("ID_VILLE_DEBUT={0} AND ID_VILLE_FIN={1} ",
                    ligneVilleS.ID_NOM, ligneVilleD.ID_NOM);

                //on regarde si le chemin n'a pas déjà été identifié comme inutile car comprenant des villes intermédiaires
                Donnees.TAB_PCC_VILLES_INUTILERow[] lignesInutile = (Donnees.TAB_PCC_VILLES_INUTILERow[])Donnees.m_donnees.TAB_PCC_VILLES_INUTILE.Select(requete);
                if (null != lignesInutile && lignesInutile.Length > 0)
                {
                    LogFile.Notifier("Chemin déjà indiqué comme comprenant des villes intermédiaires");
                    return true;
                }

                //on regarde si le chemin n'a pas déjà été traité avec succès
                Donnees.TAB_PCC_VILLESRow[] lignesAvec = (Donnees.TAB_PCC_VILLESRow[])Donnees.m_donnees.TAB_PCC_VILLES.Select(requete);
                if (null != lignesAvec && lignesAvec.Length > 0)
                {
                    LogFile.Notifier("Chemin déjà indiqué dans les parcours traités");
                    return true;
                }

                //recherche du plus court chemin
                AstarTerrain[] tableCoutsMouvementsTerrain;
                Donnees.TAB_PIONRow lignePion = Donnees.m_donnees.TAB_PION[0];
                Donnees.TAB_MODELE_TERRAINRow ligneterrainS = Donnees.m_donnees.TAB_MODELE_TERRAIN.FindByID_MODELE_TERRAIN(ligneCaseVilleS.ID_MODELE_TERRAIN);
                Donnees.TAB_MODELE_TERRAINRow ligneterrainD = Donnees.m_donnees.TAB_MODELE_TERRAIN.FindByID_MODELE_TERRAIN(ligneCaseVilleD.ID_MODELE_TERRAIN);
                bRoutier = (ligneterrainS.B_CIRCUIT_ROUTIER && ligneterrainD.B_CIRCUIT_ROUTIER) ? true : false;
                if (bRoutier) {LogFile.Notifier("Chemin routier");} else {LogFile.Notifier("Chemin non routier");}
                Cartographie.CalculModeleMouvementsPion(out tableCoutsMouvementsTerrain, bRoutier);
                timeStart = DateTime.Now;
                //if (!m_etoile.SearchPathVilles(ligneCaseDepart, ligneVilleD, ligneCaseArrivee, tableCoutsMouvementsTerrain, true))
                if (!m_etoile.SearchPath(ligneCaseVilleS, ligneCaseVilleD, tableCoutsMouvementsTerrain))
                {
                    // il est possible que le nom soit sur une case "routiere" mais qu'aucune route n'y conduise.
                    //on refait un test mais sans le mode routier
                    if (bRoutier)
                    {
                        LogFile.Notifier("pas de chemin routier, recherche en non routier");
                        Cartographie.CalculModeleMouvementsPion(out tableCoutsMouvementsTerrain, false);
                        m_etoile.SearchPath(ligneCaseVilleS, ligneCaseVilleD, tableCoutsMouvementsTerrain);
                    }
                    if (!m_etoile.PathFound)
                    {
                        LogFile.Notifier(string.Format("CalculCheminVilles:AStar : Il n'y a aucun chemin possible entre les villes {0}:{1}({2}:{3},{4}) et {5}:{6}({7}:{8},{9})",
                            ligneVilleS.ID_NOM, ligneVilleS.S_NOM,
                            ligneCaseVilleS.ID_CASE, ligneCaseVilleS.I_X, ligneCaseVilleS.I_Y,
                            ligneVilleD.ID_NOM, ligneVilleD.S_NOM,
                            ligneCaseVilleD.ID_CASE, ligneCaseVilleD.I_X, ligneCaseVilleD.I_Y));
                    }
                }
                if (m_etoile.PathFound)
                {
                    perf = DateTime.Now - timeStart;
                    LogFile.Notifier(string.Format("{0} heures, {1} min, {2} sec, {3} mil", perf.Hours, perf.Minutes, perf.Seconds, perf.Milliseconds));
                    //ajout des trajets et des couts
                    List<Donnees.TAB_CASERow> trajet = m_etoile.PathByNodes;
                    listeVilles.Clear();
                    //on recherche toutes les villes présente sur le trajet
                    ligneVilleP = ligneVilleS;
                    //ligneCaseVilleP = ligneCaseVilleS;
                    k = 0;
                    listeVilles.Add(ligneVilleP);
                    while (k < trajet.Count)
                    {
                        if (!trajet[k].IsID_NOMNull() && trajet[k].ID_NOM != ligneVilleP.ID_NOM)
                        {
                            ligneVilleP = Donnees.m_donnees.TAB_NOMS_CARTE.FindByID_NOM(trajet[k].ID_NOM);
                            listeVilles.Add(ligneVilleP);
                        }
                        k++;
                    }

                    if (2 == listeVilles.Count())
                    {
                        //On a trouvé aucune autre ville sur le parcours, on ajoute le trajet
                        LogFile.Notifier("ajout de ce nouveau trajet");
                        AjouterTrajet(ligneVilleS, ligneVilleD, trajet, m_etoile.CoutGlobal, m_etoile.HorsRouteGlobal);
                    }
                    else
                    {
                        //trace du parcours de villes trouvées
                        string message = "parcours multivilles";
                        i = 0;
                        while (i < listeVilles.Count())
                        {
                            message += (0 == i) ? string.Format(" {0}:{1}", listeVilles[i].ID_NOM, listeVilles[i].S_NOM) : string.Format(" -> {0}:{1}", listeVilles[i].ID_NOM, listeVilles[i].S_NOM);
                            i++;
                        }
                        LogFile.Notifier(message);

                        //On crée d'abord tous les parcours unitaires entre toutes les villes intermédiaires
                        i = 0;
                        while (i < listeVilles.Count()-1)
                        {
                            //on regarde si le chemin n'a pas déjà été traité avec succès
                            requete = string.Format("ID_VILLE_DEBUT={0} AND ID_VILLE_FIN={1} ",
                                                    listeVilles[i].ID_NOM, listeVilles[i+1].ID_NOM);
                            Donnees.TAB_PCC_VILLESRow[] lignesDejaTraite = (Donnees.TAB_PCC_VILLESRow[])Donnees.m_donnees.TAB_PCC_VILLES.Select(requete);
                            if (null != lignesDejaTraite && lignesDejaTraite.Length == 0)
                            {
                                //on peut ajouter ce chemin
                                LogFile.Notifier(string.Format("ajout de trajet intermédiaire de {0}:{1} vers {2}:{3}",
                                    listeVilles[i].ID_NOM, listeVilles[i].S_NOM, listeVilles[i + 1].ID_NOM, listeVilles[i+1].S_NOM));
                                Donnees.TAB_CASERow ligneCaseVilleIS = Donnees.m_donnees.TAB_CASE.FindByID_CASE(listeVilles[i].ID_CASE);
                                Donnees.TAB_CASERow ligneCaseVilleID = Donnees.m_donnees.TAB_CASE.FindByID_CASE(listeVilles[i + 1].ID_CASE);
                                //on est obligé de tout recalculer car sinon on ne part pas des centres des villes
                                if (!m_etoile.SearchPath(ligneCaseVilleIS, ligneCaseVilleID, tableCoutsMouvementsTerrain))
                                {
                                    //oui, ça peut arriver si la ville est malencontreusement placée hors route !
                                    if (bRoutier)
                                    {
                                        LogFile.Notifier("pas de chemin routier, recherche en non routier");
                                        Cartographie.CalculModeleMouvementsPion(out tableCoutsMouvementsTerrain, false);
                                        m_etoile.SearchPath(ligneCaseVilleS, ligneCaseVilleD, tableCoutsMouvementsTerrain);
                                        //on remet comme avant !
                                        Cartographie.CalculModeleMouvementsPion(out tableCoutsMouvementsTerrain, true);
                                    }
                                }
                                AjouterTrajet(listeVilles[i], listeVilles[i + 1], m_etoile.PathByNodes, m_etoile.CoutGlobal, m_etoile.HorsRouteGlobal);
                            }
                            else
                            {
                                LogFile.Notifier(string.Format("Trajet intermédiaire de {0}:{1} vers {2}:{3} déjà existant ID = {4}",
                                    listeVilles[i].ID_NOM, listeVilles[i].S_NOM, listeVilles[i + 1].ID_NOM, listeVilles[i + 1].S_NOM, lignesDejaTraite[0].ID_TRAJET));
                            }
                            i++;
                        }

                        //ensuite on "elimine" tous les parcours entre les villes qui contiennent des villes intermédiares
                        i = 0;
                        while (i < listeVilles.Count())
                        {
                            j = i + 2;
                            while (j < listeVilles.Count())
                            {
                                requete = string.Format("ID_VILLE_DEBUT={0} AND ID_VILLE_FIN={1} ",
                                                        listeVilles[i].ID_NOM, listeVilles[j].ID_NOM);

                                //on regarde si le chemin n'a pas déjà été traité avec succès
                                Donnees.TAB_PCC_VILLES_INUTILERow[] lignesDejaTraite = (Donnees.TAB_PCC_VILLES_INUTILERow[])Donnees.m_donnees.TAB_PCC_VILLES_INUTILE.Select(requete);
                                if (null != lignesDejaTraite && lignesDejaTraite.Length == 0)
                                {
                                    //on peut ajouter ce chemin comme inutile
                                    LogFile.Notifier(string.Format("ajout de trajet inutile de {0}:{1} vers {2}:{3}",
                                        listeVilles[i].ID_NOM, listeVilles[i].S_NOM, listeVilles[j].ID_NOM, listeVilles[j].S_NOM));
                                    Donnees.m_donnees.TAB_PCC_VILLES_INUTILE.AddTAB_PCC_VILLES_INUTILERow(listeVilles[i].ID_NOM, listeVilles[j].ID_NOM);
                                    Donnees.m_donnees.TAB_PCC_VILLES_INUTILE.AddTAB_PCC_VILLES_INUTILERow(listeVilles[j].ID_NOM, listeVilles[i].ID_NOM);
                                }
                                else
                                {
                                    LogFile.Notifier(string.Format("trajet inutile déjà exitant de {0}:{1} vers {2}:{3}",
                                        listeVilles[i].ID_NOM, listeVilles[i].S_NOM, listeVilles[j].ID_NOM, listeVilles[j].S_NOM));
                                }
                                j++;
                            }
                            i++;
                        }
                    }
                    /*
                    ligneVilleI = null;
                    ligneVilleP = ligneVilleS;
                    ligneCaseVilleP = ligneCaseVilleS;
                    k=0;
                    while (k < trajet.Count)
                    {
                        //on regarde si pour aller entre les deux villes on passe par une autre ville
                        if (!trajet[k].IsID_NOMNull() && trajet[k].ID_NOM != ligneVilleS.ID_NOM)
                        {
                            ligneVilleI = Donnees.m_donnees.TAB_NOMS_CARTE.FindByID_NOM(trajet[k].ID_NOM);
                            if (ligneVilleI != null && ligneVilleI.ID_NOM != ligneVilleP.ID_NOM)
                            {
                                //On a trouvé une ville avant sur le parcours, on ne conserve que les trajets vers les villes les plus proches
                                //car ces villes seront forcement traitées ensuite
                                LogFile.Notifier(string.Format("Chemin de {0}:{1} vers {2}:{3} passe par {4}:{5}",
                                    ligneVilleS.ID_NOM, ligneVilleS.S_NOM, ligneVilleD.ID_NOM, ligneVilleD.S_NOM, ligneVilleI.ID_NOM, ligneVilleI.S_NOM));

                                ligneCaseVilleI = trajet[k];
                                //Optimisation : si le trajet n'est pas déjà présent, on en profite pour l'ajouter
                                requete = string.Format("ID_VILLE_DEBUT={0} AND ID_VILLE_FIN={1} ",
                                                        ligneVilleP.ID_NOM, ligneVilleI.ID_NOM);

                                //on regarde si le chemin n'a pas déjà été traité avec succès
                                Donnees.TAB_PCC_VILLESRow[] lignesDejaTraite = (Donnees.TAB_PCC_VILLESRow[])Donnees.m_donnees.TAB_PCC_VILLES.Select(requete);
                                if (null != lignesDejaTraite && lignesDejaTraite.Length == 0)
                                {
                                    //on peut ajouter ce chemin
                                    LogFile.Notifier(string.Format("ajout de trajet intermédiaire de {0}:{1} vers {2}:{3}",
                                        ligneVilleP.ID_NOM, ligneVilleP.S_NOM, ligneVilleI.ID_NOM, ligneVilleI.S_NOM));
                                    double CostOfPath;
                                    long outRoad;
                                    m_etoile.CoutPartiels(ligneCaseVilleP, ligneCaseVilleI, out CostOfPath, out outRoad);
                                    AjouterTrajet(ligneVilleP, ligneVilleI, trajet, (long)CostOfPath, outRoad);
                                }
                                else
                                {
                                    LogFile.Notifier(string.Format("Trajet déjà existant ID = {0}",
                                        lignesDejaTraite[0].ID_TRAJET));
                                }
                                ligneVilleP = ligneVilleI;
                                ligneCaseVilleP = ligneCaseVilleI;
                            }                    
                        }
                        k++;
                    }

                    if (ligneVilleI == null)
                    {
                        //On a trouvé aucune autre ville sur le parcours, on ajoute le trajet
                        LogFile.Notifier("ajout de ce nouveau trajet");
                        AjouterTrajet(ligneVilleS, ligneVilleD, trajet, m_etoile.CoutGlobal, m_etoile.HorsRouteGlobal);
                    }
                    */
                }
                //Mise à jour de la progression pour pouvoir repartir du dernier trajet traité correctement en cas de crash
                Donnees.m_donnees.TAB_PCC_VILLES_PROGRESSION[0].ID_VILLE_DEBUT=ligneVilleS.ID_NOM;
                Donnees.m_donnees.TAB_PCC_VILLES_PROGRESSION[0].ID_VILLE_FIN=ligneVilleD.ID_NOM;
            }
            catch (Exception e)
            {
                LogFile.Notifier("CalculCheminVilles : exception =" + e.Message);
                return false;
            }
            return true;
        }

        private void AjouterTrajet(Donnees.TAB_NOMS_CARTERow ligneVilleS, Donnees.TAB_NOMS_CARTERow ligneVilleD, List<Donnees.TAB_CASERow> trajet, int coutGlobal, int coutGlobalHorsRoute)
        {
            if (coutGlobal < 0 || coutGlobalHorsRoute < 0)
            {
                Debug.WriteLine("bug sur l'ajout de trajet");
            }
            List<int> listeCasesTrajet = new List<int>();

            m_idTrajet++;
            LogFile.Notifier(string.Format("Ajout du trajet de {0}:{1} vers {2}:{3} ID={4} avec un coutGlobal={5} et coutGlobalHorsRoute={6}",
                ligneVilleS.ID_NOM, ligneVilleS.S_NOM, ligneVilleD.ID_NOM, ligneVilleD.S_NOM, m_idTrajet, coutGlobal, coutGlobalHorsRoute));
            int k = 0;
            while (k < trajet.Count)
            {
                listeCasesTrajet.Add(trajet[k].ID_CASE);
                k++;
            }
            Dal.SauvegarderTrajet(m_idTrajet, Constantes.CST_TRAJET_VILLE, listeCasesTrajet);
            Donnees.m_donnees.TAB_PCC_VILLES.AddTAB_PCC_VILLESRow(ligneVilleS.ID_NOM, ligneVilleD.ID_NOM, m_etoile.CoutGlobal, m_idTrajet, m_etoile.HorsRouteGlobal, Constantes.CST_TRAJET_INOCCUPE, false);
            Donnees.m_donnees.TAB_PCC_VILLES.AddTAB_PCC_VILLESRow(ligneVilleD.ID_NOM, ligneVilleS.ID_NOM, m_etoile.CoutGlobal, m_idTrajet, m_etoile.HorsRouteGlobal, Constantes.CST_TRAJET_INOCCUPE, false);
        }

        private int ReparerTablePCC(int idTrajet)
        {
            List<Donnees.TAB_CASERow> listeCase = new List<Donnees.TAB_CASERow>(); ;
            List<int> listeIDCases;
            int cout, horsRoute;
            AstarTerrain[] tableCoutsMouvementsTerrain;

            if (Dal.ChargerTrajet(idTrajet, Constantes.CST_TRAJET_VILLE, out listeIDCases))
            {
                listeCase.Clear();
                foreach (int idCase in listeIDCases)
                {
                    listeCase.Add(Donnees.m_donnees.TAB_CASE.FindByID_CASE(idCase));
                }

                Donnees.TAB_PIONRow lignePion = Donnees.m_donnees.TAB_PION[0];
                Cartographie.CalculModeleMouvementsPion(out tableCoutsMouvementsTerrain, false);
                cout = AStarVille.CalculCout(listeCase, tableCoutsMouvementsTerrain);
                horsRoute = AStarVille.CalculHorsRoute(listeCase, tableCoutsMouvementsTerrain);
                Donnees.m_donnees.TAB_PCC_VILLES.AddTAB_PCC_VILLESRow(
                    listeCase[0].ID_NOM,
                    listeCase[listeCase.Count - 1].ID_NOM,
                    cout,
                    idTrajet,
                    horsRoute,
                    Constantes.CST_TRAJET_INOCCUPE, 
                    false);

                Donnees.m_donnees.TAB_PCC_VILLES.AddTAB_PCC_VILLESRow(
                    listeCase[listeCase.Count - 1].ID_NOM,
                    listeCase[0].ID_NOM,
                    cout,
                    idTrajet,
                    horsRoute,
                    Constantes.CST_TRAJET_INOCCUPE, 
                    false);
                return Math.Min(listeCase[0].ID_NOM, listeCase[listeCase.Count - 1].ID_NOM);
            }
            return -1;
        }
        #endregion
    }
}
