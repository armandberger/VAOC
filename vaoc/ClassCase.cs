﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using WaocLib;

namespace vaoc
{
    partial class Donnees
    {
        partial class TAB_CASERow
        {
            //private Object m_verrou = new Object();  

            /*
            static protected AStar m_star = null;
            internal AStar etoile
            {
                get
                {
                    if (null == m_star)
                    {
                        m_star = new AStar();
                    }
                    return m_star;
                }
            }*/

            public double iCOUT
            {
                set
                {
                    I_COUT = value;
                    Donnees.m_donnees.TAB_CASE.AjouterCaseCoutNonMax(this);
                }
            }

            /// <summary>
            /// Renvoie le coût d'une case pour un modèle de mouvement particulier pour la case courante et la météo courante
            /// </summary>
            /// <param name="ID_MODELE_MOUVEMENT"></param>
            /// <returns></returns>
            public int CoutCase(int ID_MODELE_MOUVEMENT)
            {
                string requete = string.Format("ID_MODELE_MOUVEMENT={0} AND ID_METEO={1} AND ID_MODELE_TERRAIN={2}",
                        ID_MODELE_MOUVEMENT,
                        Donnees.m_donnees.TAB_PARTIE[0].ID_METEO,
                        this.ID_MODELE_TERRAIN);
                Donnees.TAB_MOUVEMENT_COUTRow[] resCout = (Donnees.TAB_MOUVEMENT_COUTRow[])Donnees.m_donnees.TAB_MOUVEMENT_COUT.Select(requete);
                return resCout[0].I_COUT;
            }

            /// <summary>
            /// indique si la case est occupable par une unité
            /// </summary>
            /// <returns>true si occupable, false sinon</returns>
            public bool EstMouvementPossible()
            {
                if (CoutCase(0) > 0)
                {
                    return true;
                }
                return false;
            }

            /// <summary>
            /// indique si la case est vide
            /// </summary>
            /// <returns>true si vide, false sinon</returns>
            public bool EstInnocupe()
            {
                return IsID_PROPRIETAIRENull() && IsID_NOUVEAU_PROPRIETAIRENull();
            }

            /// <summary>
            /// indique si la case est occupée par un ennemi bloquant
            /// </summary>
            /// <returns>false si vide ou occupé par le même, true si occupé par une ennemi</returns>
            public bool EstOccupeeOuBloqueParEnnemi(TAB_PIONRow lignePion, bool enMouvement)
            {
                // je stocke les variables pour libérer la table le plus rapidement possible
                Monitor.Enter(Donnees.m_donnees.TAB_CASE.Rows.SyncRoot);
                bool isProprietaireNull = IsID_PROPRIETAIRENull();
                
                bool isNouveauProprietaireNull = IsID_NOUVEAU_PROPRIETAIRENull();
                int? idProprietaire = null;
                if (!isProprietaireNull) idProprietaire = ID_PROPRIETAIRE;
                int? idNouveauProprietaire = null;
                if (!isNouveauProprietaireNull) idNouveauProprietaire = ID_NOUVEAU_PROPRIETAIRE;
                Monitor.Exit(Donnees.m_donnees.TAB_CASE.Rows.SyncRoot);
                if (!isProprietaireNull)
                {
                    if ((lignePion.ID_PION == idProprietaire) && isNouveauProprietaireNull)
                    {
                        return false;
                    }
                    //if (enMouvement && !lignePion.estEnnemi(this, false))//false au lieu de true, 4/4/2019 car sinon une unite en mouvement traverse les depots sans les capturer
                    //{
                    //    //si l'on se déplace sur une route, les occupants peuvent se superposer
                    //    return false;
                    //}
                    if (lignePion.estEnnemi(this, false))//false au lieu de true, 4/4/2019 car sinon une unite en mouvement traverse les depots sans les capturer
                    {
                        return true;
                    }
                    else
                    {
                        //si l'on se déplace sur une route, les occupants peuvent se superposer
                        return !enMouvement;
                    }
                }
                if (isNouveauProprietaireNull) 
                {
                    return false; 
                }
                bool retour = lignePion.ID_PION != idNouveauProprietaire;
                return retour;
            }

            public bool EstOccupeeParAmi(TAB_PIONRow lignePion)
            {
                return lignePion.estAmi(this, false);
            }

            public bool EstOccupeeParEnnemi(TAB_PIONRow lignePion)
            {
                return lignePion.estEnnemi(this);
            }

            /// <summary>
            /// Renvoie le pion occupant une case
            /// </summary>
            /// <returns>ligne du pion occupant la case</returns>
            public Donnees.TAB_PIONRow TrouvePionSurCase()
            {
                TAB_PIONRow lignePion = null;

                int IdProprietaire = this.ID_PROPRIETAIRE;
                int IdNouveauProprietaire = this.ID_NOUVEAU_PROPRIETAIRE;
                if (Constantes.NULLENTIER != IdNouveauProprietaire)
                {
                    lignePion = m_donnees.TAB_PION.FindByID_PION(IdNouveauProprietaire);
                }
                if (null == lignePion && (Constantes.NULLENTIER != IdProprietaire))
                {
                    lignePion = m_donnees.TAB_PION.FindByID_PION(IdProprietaire);
                }
                return lignePion;
            }

            internal List<Donnees.TAB_CASERow> ListeCasesVoisinesDeMemeType(ref List<Donnees.TAB_CASERow> listeCasesVoisines)
            {
                //List<Donnees.TAB_CASERow> listeCasesVoisines = new List<Donnees.TAB_CASERow>();
                Donnees.TAB_CASERow[] listeVoisins = Donnees.m_donnees.TAB_CASE.CasesVoisines(this);
                foreach (Donnees.TAB_CASERow ligneCaseVoisin in listeVoisins)
                {
                    if (ligneCaseVoisin.ID_MODELE_TERRAIN == ID_MODELE_TERRAIN)
                    {
                        int i = 0;
                        while (i < listeCasesVoisines.Count && listeCasesVoisines[i].ID_CASE != ligneCaseVoisin.ID_CASE) i++;
                        if (listeCasesVoisines.Count <= i)
                        {
                            //valeur non encore en liste
                            listeCasesVoisines.Add(ligneCaseVoisin);
                            ligneCaseVoisin.ListeCasesVoisinesDeMemeType(ref listeCasesVoisines);
                        }
                        //listeCasesVoisines.AddRange(ListeCasesVoisinesDeMemeType(ligneCaseVoisin));
                    }
                }
                return listeCasesVoisines;
            }

            internal bool ConstruirePonton()
            {
                string message;
                List<Bloc> listeBlocsARecomposer = new();

                try
                {
                    Debug.WriteLine(string.Format("Début ConstruirePonton sur la case ID={0}({1},{2})", ID_CASE, I_X, I_Y));
                    DateTime timeStartGlobal = DateTime.Now;
                    ClassHPAStarCreation hpaStarCreation = new(Donnees.m_donnees.TAB_JEU[0].I_TAILLEBLOC_PCC);
                    hpaStarCreation.InitialisationIdTrajet();//pour calculer le prochain IdTrajet en création

                    //recherche de toutes les cases constituant le ponton
                    List<Donnees.TAB_CASERow> listeCasesPont = new()
                    {
                        this //ajout this à la collection
                    };
                    ListeCasesVoisinesDeMemeType(ref listeCasesPont);
                    Debug.WriteLine(string.Format("ConstruirePonton sur la case ID={0}({1},{2}) sur une longueur de {3}", ID_CASE, I_X, I_Y, listeCasesPont.Count));

                    foreach (Donnees.TAB_CASERow ligneCasePont in listeCasesPont)
                    {
                        Donnees.TAB_MODELE_TERRAINRow ligneModeleTerrain = Donnees.m_donnees.TAB_MODELE_TERRAIN.FindByID_MODELE_TERRAIN(ligneCasePont.ID_MODELE_TERRAIN);
                        //dans quels blocs se trouve-t-on ?
                        AStar etoile = new();
                        List<Bloc> listeBlocs = etoile.NuméroBlocParPosition(ligneCasePont.I_X, ligneCasePont.I_Y);

                        //modification des blocs
                        foreach (Bloc bloc in listeBlocs)
                        {
                            if (!listeBlocsARecomposer.Contains(bloc))
                            {
                                listeBlocsARecomposer.Add(new Bloc(bloc.xBloc, bloc.yBloc));
                            }
                        }
                        ligneCasePont.ID_MODELE_TERRAIN = ligneModeleTerrain.ID_MODELE_NOUVEAU_TERRAIN;
                    }

                    foreach (Bloc bloc in listeBlocsARecomposer)
                    {
                        //il faut supprimer tous les trajets du bloc et les recalculer pour tenir des évolutions du modèle du terrain
                        //une destruction de ponton entraine un recalcul global de tous les trajets du bloc
                        if (!hpaStarCreation.RecalculCheminPCCBloc(bloc.xBloc, bloc.yBloc, false))
                        {
                            message = string.Format("ConstruirePonton: Erreur fatale sur RecalculCheminPCCBloc bloc X={0} Y={1}", bloc.xBloc, bloc.yBloc);
                            Debug.WriteLine(message);
                            return false;
                        }
                    }

                    // Il faut recalculer la table d'optimisation HPA
                    Donnees.m_donnees.TAB_PCC_COUTS.Initialisation();
                }
                catch (Exception e)
                {
                    message = string.Format("ConstruirePonton: Erreur fatale sur la case ID={0}({1},{2}) : {3}", ID_CASE, I_X, I_Y, e.Message);
                    Debug.WriteLine(message);
                    LogFile.Notifier(message);
                    return false;
                }

                return true;
            }

            /// <summary>
            /// En fait, endommager ou reparer un pont, revient toujours à prendre la case de subsitution et à recalculer le cout des trajets qui passent par cette case
            /// </summary>
            /// <param name="ligneCasePontSource">case de pont à modifier</param>
            /// <returns>true si OK, false si KO</returns>
            internal bool EndommagerReparerPont()
            {
                string message;
                int surcoutMouvement;
                List<Bloc> listeBlocsARecomposer = new();

                try
                {
                    //Debug.WriteLine(string.Format("Début EndommagerReparerPont sur la case ID={0}({1},{2})", ID_CASE, I_X, I_Y));
                    DateTime timeStartGlobal = DateTime.Now;
                    ClassHPAStarCreation hpaStarCreation = new(Donnees.m_donnees.TAB_JEU[0].I_TAILLEBLOC_PCC);

                    //recherche de toutes les cases constituant le pont
                    List<Donnees.TAB_CASERow> listeCasesPont = new()
                    {
                        this
                    };
                    ListeCasesVoisinesDeMemeType(ref listeCasesPont);
                    //Debug.WriteLine(string.Format("EndommagerReparerPont sur la case ID={0}({1},{2}) sur une longueur de {3}", ID_CASE, I_X, I_Y, listeCasesPont.Count()));

                    foreach (Donnees.TAB_CASERow ligneCasePont in listeCasesPont)
                    {
                        //Modification du modele suite à l'endommagement/reparation
                        Donnees.TAB_MODELE_TERRAINRow ligneModeleTerrain = Donnees.m_donnees.TAB_MODELE_TERRAIN.FindByID_MODELE_TERRAIN(ligneCasePont.ID_MODELE_TERRAIN);
                        surcoutMouvement = CalculModificationCout(ligneCasePont.ID_MODELE_TERRAIN, ligneModeleTerrain.ID_MODELE_NOUVEAU_TERRAIN);
                        //modification effective du terrain
                        //si la case n'a pas déjà été modifiée, on l'ajoute dans la liste des cases modifiée
                        if (null == Donnees.m_donnees.TAB_CASE_MODIFICATION.FindByID_CASE(ligneCasePont.ID_CASE))
                        {
                            Monitor.Enter(Donnees.m_donnees.TAB_CASE_MODIFICATION.Rows.SyncRoot); 
                            Donnees.m_donnees.TAB_CASE_MODIFICATION.AddTAB_CASE_MODIFICATIONRow(ligneCasePont.ID_CASE, ligneCasePont.ID_MODELE_TERRAIN);
                            Monitor.Exit(Donnees.m_donnees.TAB_CASE_MODIFICATION.Rows.SyncRoot);
                        }

                        //dans quels blocs se trouve-t-on ?
                        AStar etoile = new();
                        List<Bloc> listeBlocs = etoile.NuméroBlocParPosition(ligneCasePont.I_X, ligneCasePont.I_Y);

                        //modification des parcours
                        foreach (Bloc bloc in listeBlocs)
                        {
                            //recherche de tous les trajets du bloc dont la case de pont fait partie
                            string requete = string.Format("I_BLOCX={0} AND I_BLOCY={1}", bloc.xBloc, bloc.yBloc);
                            Donnees.TAB_PCC_COUTSRow[] listeTrajets = (Donnees.TAB_PCC_COUTSRow[])Donnees.m_donnees.TAB_PCC_COUTS.Select(requete);
                            foreach (Donnees.TAB_PCC_COUTSRow trajet in listeTrajets)
                            {
                                //on regarde si la case fait partie du trajet
                                Dal.ChargerTrajet(trajet.ID_TRAJET, out List<int> listeCases);
                                if (listeCases.IndexOf(ligneCasePont.ID_CASE) >= 0)
                                {
                                    if (ligneModeleTerrain.B_PONTON)
                                    {
                                        //si nous sommes sur un ponton, alors le trajet n'est pas simplement modifiée, il est détruit
                                        if (!listeBlocsARecomposer.Contains(bloc))
                                        {
                                            listeBlocsARecomposer.Add(new Bloc(bloc.xBloc, bloc.yBloc));
                                        }
                                    }
                                    else
                                    {
                                        trajet.I_COUT += surcoutMouvement;
                                        if (trajet.I_COUT <= 0)
                                        {
                                            message = string.Format("ERREUR ! EndommagerReparerPont: cout <=0 sur ID trajet = {0}", trajet.ID_TRAJET);
                                            LogFile.Notifier(message);
                                            return false;
                                        }
                                    }
                                }
                            }
                        }
                        ligneCasePont.ID_MODELE_TERRAIN = ligneModeleTerrain.ID_MODELE_NOUVEAU_TERRAIN;
                    }

                    if (listeBlocsARecomposer.Count > 0)
                    {
                        foreach (Bloc bloc in listeBlocsARecomposer)
                        {
                            //il faut supprimer tous les trajets du bloc et les recalculer pour tenir des évolutions du modèle du terrain
                            //une destruction de ponton entraine un recalcul global de tous les trajets du bloc
                            if (!hpaStarCreation.RecalculCheminPCCBloc(bloc.xBloc, bloc.yBloc, false))
                            {
                                message = string.Format("EndommagerReparerPont: Erreur fatale sur RecalculCheminPCCBloc bloc X={0} Y={1}", bloc.xBloc, bloc.yBloc);
                                //Debug.WriteLine(message);
                                return false;
                            }
                        }

                        // Il faut recalculer la table d'optimisation HPA
                        Donnees.m_donnees.TAB_PCC_COUTS.Initialisation();
                    }
                }
                catch (Exception e)
                {
                    message = string.Format("EndommagerReparerPont: Erreur fatale sur la case ID={0}({1},{2}) : {3}", ID_CASE, I_X, I_Y, e.Message);
                    //Debug.WriteLine(message);
                    LogFile.Notifier(message);
                    return false;
                }
                return true;
            }

            /// <summary>
            /// Calcul du surcout de modification d'une case, utiliser pour un pont détruit
            /// </summary>
            /// <param name="idModeleTerrainAvant"></param>
            /// <param name="idModeleTerrainApres"></param>
            /// <returns></returns>
            internal static int CalculModificationCout(int idModeleTerrainAvant, int idModeleTerrainApres)
            {
                //cacul du surcout
                int coutCaseAvant = Donnees.m_donnees.TAB_MODELE_MOUVEMENT[0].CoutCase(idModeleTerrainAvant);
                int coutCaseApres = Donnees.m_donnees.TAB_MODELE_MOUVEMENT[0].CoutCase(idModeleTerrainApres);
                return (coutCaseApres - coutCaseAvant) * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE;
            }

            /// <summary>
            /// Recherche un point un gué se trouvant à CST_DISTANCE_RECHERCHE_PONT_GUET de la case de base
            /// </summary>
            /// <param name="ligneCase">case de base</param>
            /// <param name="unPont">true=pont, false= ponton</param>
            /// <param name="bEndommage">true=détruit (pont détruit, gué), false=non détruit (pont, ponton)</param>
            /// <param name="tailleDuPontOuGue">nombre de cases constituant le gué ou le pont</param>
            /// <returns>case de pont ou du gué la plus proche de la case de base</returns>
            internal Donnees.TAB_CASERow RecherchePontouPonton(bool unPont, bool bEndommage, out int tailleDuPontOuGue)
            {

                int xCaseHautGauche = Math.Max(0, I_X - Constantes.CST_DISTANCE_RECHERCHE_PONT_GUET * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE);
                int yCaseHautGauche = Math.Max(0, I_Y - Constantes.CST_DISTANCE_RECHERCHE_PONT_GUET * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE);
                int xCaseBasDroite = Math.Min(Donnees.m_donnees.TAB_JEU[0].I_LARGEUR_CARTE - 1, I_X + Constantes.CST_DISTANCE_RECHERCHE_PONT_GUET * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE);
                int yCaseBasDroite = Math.Min(Donnees.m_donnees.TAB_JEU[0].I_HAUTEUR_CARTE - 1, I_Y + Constantes.CST_DISTANCE_RECHERCHE_PONT_GUET * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE);
                Donnees.TAB_CASERow[] lignesCaseRecherche = Donnees.m_donnees.TAB_CASE.CasesCadre(xCaseHautGauche, yCaseHautGauche, xCaseBasDroite, yCaseBasDroite);

                //on recherche le pont ou le guet le plus proche de la case indiquée
                tailleDuPontOuGue = -1;
                int distancePontGue = int.MaxValue;
                Donnees.TAB_CASERow ligneCasePontGue = null;

                foreach (Donnees.TAB_CASERow ligneCaseRecherche in lignesCaseRecherche)
                {
                    Donnees.TAB_MODELE_TERRAINRow ligneModeleTerrain = Donnees.m_donnees.TAB_MODELE_TERRAIN.FindByID_MODELE_TERRAIN(ligneCaseRecherche.ID_MODELE_TERRAIN);
                    //if (ligneModeleTerrain.ID_MODELE_TERRAIN != 63 && ligneModeleTerrain.ID_MODELE_TERRAIN != 60
                    //    && ligneModeleTerrain.ID_MODELE_TERRAIN != 55 && ligneModeleTerrain.ID_MODELE_TERRAIN != 57
                    //     && ligneModeleTerrain.ID_MODELE_TERRAIN != 24)
                    //{
                    //    int desbugs = 0;
                    //    desbugs++;
                    //}

                    if (((unPont && ligneModeleTerrain.B_PONT) || (!unPont && ligneModeleTerrain.B_PONTON)) && (ligneModeleTerrain.B_DETRUIT == bEndommage))
                    {
                        int distance = (int)Constantes.Distance(I_X, I_Y, ligneCaseRecherche.I_X, ligneCaseRecherche.I_Y);
                        if (distance < distancePontGue)
                        {
                            distancePontGue = distance;
                            ligneCasePontGue = ligneCaseRecherche;
                        }
                    }
                }

                //si la case est trouvé on recherche toutes les cases de même type contigues
                if (null != ligneCasePontGue)
                {
                    List<Donnees.TAB_CASERow> listeCasesVoisines = new()
                    {
                        ligneCasePontGue
                    };
                    ligneCasePontGue.ListeCasesVoisinesDeMemeType(ref listeCasesVoisines);
                    tailleDuPontOuGue = listeCasesVoisines.Count;
                }
                return ligneCasePontGue;
            }

            internal void SetID_MODELE_TERRAIN_SI_OCCUPENull()
            {
                Monitor.Enter(Donnees.m_donnees.TAB_CASE.Rows.SyncRoot);
                this.ID_MODELE_TERRAIN_SI_OCCUPE = Constantes.NULLENTIER;
                Monitor.Exit(Donnees.m_donnees.TAB_CASE.Rows.SyncRoot);
            }

            internal void SetID_PROPRIETAIRENull()
            {
                Monitor.Enter(Donnees.m_donnees.TAB_CASE.Rows.SyncRoot);
                initialisationID_PROPRIETAIRENull();
                if (null == Donnees.m_donnees.TAB_MAJ_PROPRIO.FindByID_CASE(this.ID_CASE)) { Donnees.m_donnees.TAB_MAJ_PROPRIO.AddTAB_MAJ_PROPRIORow(this.ID_CASE); }
                Monitor.Exit(Donnees.m_donnees.TAB_CASE.Rows.SyncRoot);
            }

            internal void initialisationID_PROPRIETAIRENull()
            {
                this.ID_PROPRIETAIRE = Constantes.NULLENTIER;
            }
            internal void SetID_NOUVEAU_PROPRIETAIRENull()
            {
                Monitor.Enter(Donnees.m_donnees.TAB_CASE.Rows.SyncRoot);
                initialisationID_NOUVEAU_PROPRIETAIRENull();
                if (null == Donnees.m_donnees.TAB_MAJ_PROPRIO.FindByID_CASE(this.ID_CASE)) { Donnees.m_donnees.TAB_MAJ_PROPRIO.AddTAB_MAJ_PROPRIORow(this.ID_CASE); }
                Monitor.Exit(Donnees.m_donnees.TAB_CASE.Rows.SyncRoot);
            }

            internal void initialisationID_NOUVEAU_PROPRIETAIRENull()
            {
                this.ID_NOUVEAU_PROPRIETAIRE = Constantes.NULLENTIER;
            }

            internal bool IsID_NOUVEAU_PROPRIETAIRENull()
            {
                return this.ID_NOUVEAU_PROPRIETAIRE == Constantes.NULLENTIER;
            }

            internal bool IsID_PROPRIETAIRENull()
            {
                return this.ID_PROPRIETAIRE == Constantes.NULLENTIER;
            }

            internal bool IsID_MODELE_TERRAIN_SI_OCCUPENull()
            {
                return this.ID_MODELE_TERRAIN_SI_OCCUPE == Constantes.NULLENTIER;
            }

            internal void BlocCourant(out int xBloc, out int yBloc)
            {
                xBloc = this.I_X / Constantes.CST_TAILLE_BLOC_CASES;
                yBloc = this.I_Y / Constantes.CST_TAILLE_BLOC_CASES;
            }
        }    
    }
}
