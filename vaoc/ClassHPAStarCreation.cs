﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using WaocLib;

namespace vaoc
{
    struct HPACreationStatut
    {
        public int m_xBloc;
        public int m_yBloc;
        public string m_statut;

        public HPACreationStatut(int xBloc, int yBloc, string statut)
        {
            m_xBloc = xBloc;
            m_yBloc = yBloc;
            m_statut = statut;
        }
    }

    class ClassHPAStarCreation
    {
        #region Donnees
        private const int CST_LGPCC = 5;//longueur limite pour laquelle on ajoute deux points de sortie pour le PCC
        //private readonly AStar m_etoile = new();
        private int m_idTrajet;
        static readonly object m_verrouIDTrajet = new();
        static readonly ReaderWriterLockSlim _rwPCCCOUT = new();
        
        //protected LogFile m_log = null;
        private int m_nbBlocsHorizontaux;//nombre de blocs sur l'axe des abscisses
        private int m_nbBlocsVerticaux;//nombre de blocs sur l'axe des ordonnées
        private int m_traitement;//traitement principal
        private int m_sous_traitement;//sous tâche d'un traitement
        private int m_etape;//etape de traitement
        private int m_tailleBloc;
        private System.ComponentModel.BackgroundWorker m_travailleur=null;
        private readonly List<Task<bool>> m_tasks = new();
        public const int NB_TACHES = 1000;//cela n'a pas l'air de changer grand chose de mettre 10 ou 10000 tâches en parallèle.
        private AStar[] m_etoileParallele;
        #endregion

        #region Events
        #endregion

        #region Properties
        private int tailleBloc
        {
            get { return m_tailleBloc; }
            set { m_tailleBloc = value; }
        }

        private System.ComponentModel.BackgroundWorker travailleur
        {
            set { m_travailleur=value; }
            get { return m_travailleur; }
        }
        #endregion

        #region Constructors
        public ClassHPAStarCreation(int nouveauTailleBloc)
        {
            tailleBloc = nouveauTailleBloc;
            LogFile.Notifier("ClassHPAStarCreation");
        }
        #endregion

        #region Public Methods      
        public bool Initialisation(System.ComponentModel.BackgroundWorker worker)
        {
            travailleur = worker;
            LogFile.Notifier("ClassHPAStarCreation : Initialisation");
            m_nbBlocsHorizontaux = (int)Math.Ceiling((decimal)Donnees.m_donnees.TAB_JEU[0].I_LARGEUR_CARTE / (decimal)tailleBloc);
            m_nbBlocsVerticaux = (int)Math.Ceiling((decimal)Donnees.m_donnees.TAB_JEU[0].I_HAUTEUR_CARTE / (decimal)tailleBloc);
            m_etoileParallele = new AStar[NB_TACHES];
            for (int i = 0; i < NB_TACHES; i++) { m_etoileParallele[i] = new AStar(); }

            if (Donnees.m_donnees.TAB_PCC_COUTS.Count > 0)
            {
                //on interdit l'interuption durant la première étape (étape, très rapide)
                InitialisationIdTrajet();
                Donnees.TAB_PCC_COUTSRow[] resPCCout = (Donnees.TAB_PCC_COUTSRow[])Donnees.m_donnees.TAB_PCC_COUTS.Select("ID_TRAJET=" + m_idTrajet);
                m_traitement = resPCCout[0].I_BLOCY;
                m_sous_traitement = resPCCout[0].I_BLOCX;
                m_etape = 1;
            }
            else
            {
                m_traitement = m_sous_traitement = m_etape = 0;
            }
            return true;
        }

        public void InitialisationIdTrajet()
        {
            m_idTrajet = (int)Donnees.m_donnees.TAB_PCC_COUTS.Compute("MAX(ID_TRAJET)", "");
        }

        public bool Traitement()
        {
            if (!TraitementPCC())
            {
                travailleur.ReportProgress(100, new HPACreationStatut(-1, -1, "terminé"));//c'est fini !
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
            //if (0 == m_sous_traitement && 0 == m_traitement)
            //{
            //    //initialisation des traitements
            //    if (0 == m_etape)
            //    {
            //        travailleur.ReportProgress(0,  "Points dans les blocs");//toujours zero durant le traitement, car affichage basé differement d'un pourcentage
            //    }
            //    else
            //    {
            //        travailleur.ReportProgress(0, "Calcul des chemins dans les blocs");//toujours zero durant le traitement, car affichage basé differement d'un pourcentage
            //    }
            //}
            try
            {
                if (m_traitement < m_nbBlocsVerticaux)
                {
                    travailleur.ReportProgress(100, new HPACreationStatut(m_sous_traitement, m_traitement, ""));//c'est fini !

                    if (m_sous_traitement < m_nbBlocsHorizontaux)
                    {
                        switch (m_etape)
                        {
                            case 0:
                                //Calcul des points de frontière du bloc
                                travailleur.ReportProgress(0, new HPACreationStatut(-1, -1, "Points dans les blocs"));//toujours zero durant le traitement, car affichage basé differement d'un pourcentage
                                if (!this.CréationsPointsPCCBloc(m_sous_traitement, m_traitement)) { return false; }
                                m_sous_traitement++;
                                break;
                            case 1:
                                //Calcul des PCC entre les points de frontière du bloc
                                //Sur la carte 1813 (5111x4338), le traitement mono prend 430 heures...
                                //1569 heures dans le première version parallèle...
                                travailleur.ReportProgress(0, new HPACreationStatut(-1, -1, "Calcul des chemins dans les blocs"));//toujours zero durant le traitement, car affichage basé differement d'un pourcentage
                                //if (!this.CalculCheminPCCBloc(m_sous_traitement++, m_traitement, 0)) { return false; }
                                if (!this.CalculCheminPCCBlocParallele(ref m_sous_traitement)) { return false; }
                                break;
                            default:
                                throw new Exception("TraitementPCC : On essaye de traiter une étape inexistante !");
                        }
                    }
                    else
                    {
                        if (1 == m_etape)
                        {
                            LogFile.Notifier(string.Format("traitement n°{0} en {1} sous-traitement, Donnees.m_donnees.TAB_PCC_COUTS.Count={2}",
                                m_traitement, m_sous_traitement, Donnees.m_donnees.TAB_PCC_COUTS.Count));
                        }

                        m_sous_traitement = 0;
                        m_traitement++;
                    }
                }
                else
                {
                    //il faut maintenant généré tous les chemins entre les différents points
                    if (0 == m_etape)
                    {
                        m_etape++;
                        m_traitement = m_sous_traitement = 0;
                    }
                    else
                    {
                        return false;//traitement terminé
                    }
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
        /// Calcul de tous les points de sortie/entrée du bloc
        /// </summary>
        /// <param name="xBloc">position en x du bloc</param>
        /// <param name="yBloc">position en y du bloc</param>
        /// <param name="tailleBloc">nombre de points d'un coté de bloc</param>
        /// <returns>true si OK, false si KO</returns>
        private bool CréationsPointsPCCBloc(int xBloc, int yBloc)
        {
            try
            {
                PCCMinMax(xBloc, yBloc, out int xmin, out int xmax, out int ymin, out int ymax);
                //si l'on est sur le bord droit, inutile d'ajouter des points, il n'y a pas de voisins
                if (xmax < Donnees.m_donnees.TAB_JEU[0].I_LARGEUR_CARTE)
                {
                    if (!AjouterPointsPCC(xBloc, yBloc, xmin, xmax, ymax, true))
                    {
                        return false;
                    }
                }

                if (ymax < Donnees.m_donnees.TAB_JEU[0].I_HAUTEUR_CARTE)
                {
                    if (!AjouterPointsPCC(xBloc, yBloc, ymin, ymax, xmax, false))
                    {
                        return false;
                    }
                }
            }
            catch(Exception e)
            {
                LogFile.Notifier("CréationsPointsPCCBloc : exception =" + e.Message);
                return false;
            }
            /*
            //recherche des points avec le cout min sur les entrées
                //on recherche le coût pour ce terrain en prenant pour base la première météo et n'importe quel modèle de mouvement
                int coutMin = int.MaxValue;
                int terrainMin = -1;
                //recherche du terrain avec le cout minimum sur la ligne
                for (int x = xmin; x < xmax; x++)
                {
                    Donnees.TAB_CASERow ligneCase = m_tableCase.FindByXY(x, ymax);
                    int coutCase = Donnees.m_donnees.TAB_MOUVEMENT_COUT.CalculCout(0, ligneCase.ID_MODELE_TERRAIN, 0);
                    if (coutCase < coutMin)
                    {
                        coutMin = coutCase;
                        terrainMin = ligneCase.ID_MODELE_TERRAIN;
                    }
                }
                // liste des points ayant ce modele
                //les frontières étant communes et allant de haut en bas, de gauche vers la droite, 
                //il faut à chaque fois ne traiter que les frontières droite et basse de chaque bloc mais ajouter dans les deux blocs
                int nbCases = 0;
                int nbCasesContinue = 0;//on ne garde que les points en continue s'il sont supérieur à une longueur de CST_LGPCC
                for (int x = xmin; x < xmax; x++)
                {
                    Donnees.TAB_CASERow ligneCase = m_tableCase.FindByXY(x, ymax);
                    if (terrainMin == ligneCase.ID_MODELE_TERRAIN)
                    {
                        //case de sortie possible
                        if (0 == nbCasesContinue || nbCasesContinue == CST_LGPCC)
                        {
                            LogFile.Notifier(string.Format("Point x={0}, y={1}, id={2}", ligneCase.I_X, ligneCase.I_Y, ligneCase.ID_CASE));
                            nbCases++;
                            nbCasesContinue = 0;
                            //ajout des cases dans le bloc courant et dans le bloc x+1,y
                        }
                        else
                        {
                            nbCasesContinue++;
                        }
                    }
                    else
                    {
                        nbCasesContinue = 0;
                    }

                }
            }
            LogFile.Notifier(string.Format("Nombre totals de points={0}", nbCases));
             * */
            return true;
        }

        private void PCCMinMax(int xBloc, int yBloc, out int xmin, out int xmax, out int ymin, out int ymax)
        {
            xmin = xBloc * tailleBloc;
            xmax = Math.Min(xmin + tailleBloc, Donnees.m_donnees.TAB_JEU[0].I_LARGEUR_CARTE - 1);
            ymin = yBloc * tailleBloc;
            ymax = Math.Min(ymin + tailleBloc, Donnees.m_donnees.TAB_JEU[0].I_HAUTEUR_CARTE - 1);
        }

        /// <summary>
        /// Renvoie la liste des blocs auquel appartient un point
        /// </summary>
        /// <param name="x">abscisse du point</param>
        /// <param name="y">ordonnée du point</param>
        //public Dictionary<int,int> NuméroBlocParPosition(int x, int y)
        //{
        //    Dictionary<int, int> listeBlocs = new Dictionary<int, int>();
        //    int xBloc = x / tailleBloc;
        //    int yBloc = y / tailleBloc;

        //    listeBlocs.Add(xBloc, yBloc);

        //    if ((0 == x % tailleBloc) && (x != 0 ))
        //    {
        //        listeBlocs.Add(xBloc - 1, yBloc);
        //        //je suis sur une frontière de blocs
        //        if ((0 == y % tailleBloc) && (y != 0))
        //        {
        //            //je suis sur une frontière de blocs sur x et y
        //            listeBlocs.Add(xBloc, yBloc - 1);
        //            listeBlocs.Add(xBloc - 1, yBloc - 1);
        //        }
        //    }
        //    else
        //    {
        //        if ((0 == y % tailleBloc) && (y != 0))
        //        {
        //            //je suis sur une frontière de blocs
        //            listeBlocs.Add(xBloc, yBloc - 1);
        //        }
        //    }
        //    return listeBlocs;
        //}

        /// <summary>
        /// Ajout des points de liaison inter-blocs pour le PCCC hierarchique
        /// </summary>
        /// <param name="xBloc">abscisse du bloc</param>
        /// <param name="yBloc">ordonnée du bloc</param>
        /// <param name="debut">abscisse ou ordonnée du premier point</param>
        /// <param name="fin">abscisse ou ordonnée du dernier point</param>
        /// <param name="fixe">abscisse ou ordonnée du point fixe</param>
        /// <param name="bAbscisse">true si debut->fin se fait sur l'axe des abscisses, false si le bord est parcouru sur l'axe des ordonnées.</param>
        /// <returns>true si OK, false si KO</returns>
        public bool AjouterPointsPCC(int xBloc, int yBloc, int debut, int fin, int fixe, bool bAbscisse)
        {
            //recherche des points avec le cout min sur les entrées
            //on recherche le coût pour ce terrain en prenant pour base la première météo et n'importe quel modèle de mouvement
            int i;
            int coutMin = Constantes.CST_COUTMAX;
            int terrainMin = -1;
            int i_lgpcc;
            try
            {
                i_lgpcc = Math.Max(CST_LGPCC, tailleBloc / 5);
                //recherche du terrain avec le cout minimum sur la ligne
                for (i = debut; i < fin + 1; i++)//+1 car frontière commune avec le voisin
                {
                    Donnees.TAB_CASERow ligneCase = bAbscisse ? Donnees.m_donnees.TAB_CASE.FindParXY(i, fixe) : Donnees.m_donnees.TAB_CASE.FindParXY(fixe, i);
                    int coutCase = Donnees.m_donnees.TAB_MOUVEMENT_COUT.CalculCout(0, ligneCase.ID_MODELE_TERRAIN, 0);
                    if (coutCase < coutMin && coutCase>0)
                    {
                        //si coutCase <=0 il s'agit d'un terrain intraversable
                        coutMin = coutCase;
                        terrainMin = ligneCase.ID_MODELE_TERRAIN;
                    }
                }
                if (-1 == terrainMin)
                {
                    LogFile.Notifier(string.Format("AjouterPointsPCC:Le bloc {0},{1} n'a aucun point traversable.", xBloc, yBloc));
                    return true;  
                }
                // liste des points ayant ce modele
                //les frontières étant communes et allant de haut en bas, de gauche vers la droite, 
                //il faut à chaque fois ne traiter que les frontières droite et basse de chaque bloc mais ajouter dans les deux blocs
                //LogFile.Notifier(string.Format("AjouterPointsPCC terrainMin={0}", terrainMin));
                int nbCases = 0;
                int nbCasesContinue = 0;//on ne garde que les points en continue s'il sont supérieur à une longueur de CST_LGPCC
                for (i = debut; i < fin + 1; i++)//+1 car frontière commune avec le voisin
                {
                    Donnees.TAB_CASERow ligneCase = bAbscisse ? Donnees.m_donnees.TAB_CASE.FindParXY(i, fixe) : Donnees.m_donnees.TAB_CASE.FindParXY(fixe, i);
                    if (terrainMin == ligneCase.ID_MODELE_TERRAIN)
                    {
                        //case de sortie possible
                        if (0 == nbCasesContinue || nbCasesContinue > i_lgpcc)
                        {
                            //LogFile.Notifier(string.Format("Point x={0}, y={1}, id={2}", ligneCase.I_X, ligneCase.I_Y, ligneCase.ID_CASE));
                            nbCases++;
                            nbCasesContinue = 0;
                            //ajout des cases dans le bloc courant et dans le bloc x+1,y ou le bloc x,y+1
                            Monitor.Enter(Donnees.m_donnees.TAB_PCC_CASE_BLOCS.Rows.SyncRoot);
                            if (null == Donnees.m_donnees.TAB_PCC_CASE_BLOCS.FindByID_CASEI_BLOCXI_BLOCY(ligneCase.ID_CASE, xBloc, yBloc))
                                Donnees.m_donnees.TAB_PCC_CASE_BLOCS.AddTAB_PCC_CASE_BLOCSRow(ligneCase.ID_CASE, xBloc, yBloc);

                            if (bAbscisse)
                            {
                                //parcours sur les abscisses donc sur le bord inférieurs
                                if (null == Donnees.m_donnees.TAB_PCC_CASE_BLOCS.FindByID_CASEI_BLOCXI_BLOCY(ligneCase.ID_CASE, xBloc, yBloc + 1))
                                    Donnees.m_donnees.TAB_PCC_CASE_BLOCS.AddTAB_PCC_CASE_BLOCSRow(ligneCase.ID_CASE, xBloc, yBloc + 1);
                                if (i == debut)
                                {
                                    //cas du coin
                                    if (xBloc > 0 && null == Donnees.m_donnees.TAB_PCC_CASE_BLOCS.FindByID_CASEI_BLOCXI_BLOCY(ligneCase.ID_CASE, xBloc - 1, yBloc))
                                        Donnees.m_donnees.TAB_PCC_CASE_BLOCS.AddTAB_PCC_CASE_BLOCSRow(ligneCase.ID_CASE, xBloc - 1, yBloc);
                                    if (xBloc > 0 && null == Donnees.m_donnees.TAB_PCC_CASE_BLOCS.FindByID_CASEI_BLOCXI_BLOCY(ligneCase.ID_CASE, xBloc - 1, yBloc + 1))
                                        Donnees.m_donnees.TAB_PCC_CASE_BLOCS.AddTAB_PCC_CASE_BLOCSRow(ligneCase.ID_CASE, xBloc - 1, yBloc + 1);

                                }
                                if (i == fin)
                                {
                                    if (null == Donnees.m_donnees.TAB_PCC_CASE_BLOCS.FindByID_CASEI_BLOCXI_BLOCY(ligneCase.ID_CASE, xBloc + 1, yBloc))
                                        Donnees.m_donnees.TAB_PCC_CASE_BLOCS.AddTAB_PCC_CASE_BLOCSRow(ligneCase.ID_CASE, xBloc + 1, yBloc);
                                    if (null == Donnees.m_donnees.TAB_PCC_CASE_BLOCS.FindByID_CASEI_BLOCXI_BLOCY(ligneCase.ID_CASE, xBloc + 1, yBloc+1))
                                        Donnees.m_donnees.TAB_PCC_CASE_BLOCS.AddTAB_PCC_CASE_BLOCSRow(ligneCase.ID_CASE, xBloc + 1, yBloc + 1);
                                }
                            }
                            else
                            {
                                //parcours sur les ordonnées donc sur le bord droit
                                if (null == Donnees.m_donnees.TAB_PCC_CASE_BLOCS.FindByID_CASEI_BLOCXI_BLOCY(ligneCase.ID_CASE, xBloc + 1, yBloc))
                                    Donnees.m_donnees.TAB_PCC_CASE_BLOCS.AddTAB_PCC_CASE_BLOCSRow(ligneCase.ID_CASE, xBloc + 1, yBloc);

                                if (i == debut)
                                {
                                    //cas du coin
                                    if (yBloc > 0 && null == Donnees.m_donnees.TAB_PCC_CASE_BLOCS.FindByID_CASEI_BLOCXI_BLOCY(ligneCase.ID_CASE, xBloc, yBloc - 1))
                                        Donnees.m_donnees.TAB_PCC_CASE_BLOCS.AddTAB_PCC_CASE_BLOCSRow(ligneCase.ID_CASE, xBloc, yBloc - 1);
                                    if (yBloc > 0 && null == Donnees.m_donnees.TAB_PCC_CASE_BLOCS.FindByID_CASEI_BLOCXI_BLOCY(ligneCase.ID_CASE, xBloc + 1, yBloc - 1))
                                        Donnees.m_donnees.TAB_PCC_CASE_BLOCS.AddTAB_PCC_CASE_BLOCSRow(ligneCase.ID_CASE, xBloc + 1, yBloc - 1);

                                }
                                if (i == fin)
                                {
                                    if (null == Donnees.m_donnees.TAB_PCC_CASE_BLOCS.FindByID_CASEI_BLOCXI_BLOCY(ligneCase.ID_CASE, xBloc, yBloc + 1))
                                        Donnees.m_donnees.TAB_PCC_CASE_BLOCS.AddTAB_PCC_CASE_BLOCSRow(ligneCase.ID_CASE, xBloc, yBloc + 1);
                                    if (null == Donnees.m_donnees.TAB_PCC_CASE_BLOCS.FindByID_CASEI_BLOCXI_BLOCY(ligneCase.ID_CASE, xBloc + 1, yBloc + 1))
                                        Donnees.m_donnees.TAB_PCC_CASE_BLOCS.AddTAB_PCC_CASE_BLOCSRow(ligneCase.ID_CASE, xBloc + 1, yBloc + 1);
                                }
                            }
                            Monitor.Exit(Donnees.m_donnees.TAB_PCC_CASE_BLOCS.Rows.SyncRoot);
                        }
                        nbCasesContinue++;
                    }
                    else
                    {
                        nbCasesContinue = 0;
                    }
                }

                //LogFile.Notifier(string.Format("Nombre totals de points={0}", nbCases));
                if (0 == nbCases)
                {
                    LogFile.Notifier(string.Format("AjouterPointsPCC:Le bloc {0},{1} n'a aucun point sur une frontière !", xBloc, yBloc));
                    return false;
                }
            }
            catch (Exception e)
            {
                LogFile.Notifier("AjouterPointsPCC : exception =" + e.Message);
                return false;
            }
            return true;
        }

        private bool CalculCheminPCCBlocParallele(ref int iAvance)
        {
            //semble très très légèrement plus rapide avec des tâches plutot qu'avec Parallel.For
            int nbTasks = Math.Min(NB_TACHES, m_nbBlocsHorizontaux - iAvance); 
            for (int k = 0; k < nbTasks; k++)
            {
                int XBloc = m_sous_traitement + k;
                int YBloc = m_traitement;
                int numeroTache = k; //pour la pile des Astar
                m_tasks.Add(Task<bool>.Factory.StartNew(() =>
                {
                    return CalculCheminPCCBloc(XBloc, YBloc, numeroTache);
                    //return CalculCheminPCCBlocOBJ(XBloc, YBloc, false, numeroTache);
                }));
            }
            Task.WaitAll(m_tasks.ToArray());
            foreach (Task<bool> tache in m_tasks)
            {
                if (!tache.Result)
                {
                    return false;
                }
            }
            //Parallel.For(0, nbTasks, k => CalculCheminPCCBloc(m_sous_traitement + k, m_traitement, k));
            //Parallel.For(0, nbTasks, k => CalculCheminPCCBlocOBJ(m_sous_traitement + k, m_traitement, false, k));
            iAvance += nbTasks;
            //CalculCheminPCCBlocOBJ(m_sous_traitement, m_traitement, false, 0);
            //iAvance++;
            return true;
        }

        /// <summary>
        /// Calcul de tous les chemins entre les points de frontière du bloc et des coûts de parcours entre chaque point
        /// </summary>
        /// <param name="xBloc">position en x du bloc</param>
        /// <param name="yBloc">position en y du bloc</param>
        /// <returns>true si OK, false si KO</returns>
        private bool CalculCheminPCCBloc(int xBloc, int yBloc, int numeroTache)
        {
            //Debug.WriteLine(string.Format("CalculCheminPCCBloc xBloc={0}, yBloc={1}", xBloc, yBloc));
            return CalculCheminPCCBloc(xBloc, yBloc, false, numeroTache);
        }

        private bool CalculCheminPCCBloc(int xBloc, int yBloc, bool bCreation, int numeroTache)
        {
            int i, j, k;
            //DateTime timeStart;
            //TimeSpan perf;
            Donnees.TAB_PCC_CASE_BLOCSRow[] listeCases;
            Donnees.TAB_CASERow ligneCaseDepart, ligneCaseArrivee;
            List<int> listeCasesTrajet = new();
            int nbTrajets = 0; //pour debug

            //LogFile.Notifier(string.Format("CalculCheminPCCBloc xBloc={0}, yBloc={1}, numeroTache={2} ", xBloc, yBloc, numeroTache));
            try
            {
                PCCMinMax(xBloc, yBloc, out int xmin, out int xmax, out int ymin, out int ymax);

                //recherche de tous les points dans le bloc
                listeCases = Donnees.m_donnees.TAB_PCC_CASE_BLOCS.ListeCasesBloc(xBloc, yBloc);
                //if (0==listeCases.Length), cas impossible, testé à l'étape précédente !

                for (i = 0; i < listeCases.Length; i++)
                {
                    ligneCaseDepart = Donnees.m_donnees.TAB_CASE.FindParID_CASE(listeCases[i].ID_CASE);
                    j = i + 1;
                    while (j < listeCases.Length)
                    {
                        ligneCaseArrivee = Donnees.m_donnees.TAB_CASE.FindParID_CASE(listeCases[j].ID_CASE);
                        //if (ligneCaseDepart.I_X == ligneCaseArrivee.I_X || ligneCaseDepart.I_Y == ligneCaseArrivee.I_Y)
                        //{
                        //    //    j++;//pas de trajets entre deux points sur la même ligne
                        //    //    continue;
                        //    //Debug.WriteLine("meme ligne");
                        //}
                        string requete = string.Format("ID_CASE_DEBUT={0} AND ID_CASE_FIN={1} AND I_BLOCX={2} AND I_BLOCY={3}",
                            ligneCaseDepart.ID_CASE, ligneCaseArrivee.ID_CASE, xBloc, yBloc);
                        //Monitor.Enter(Donnees.m_donnees.TAB_PCC_COUTS);//pour éviter toute modification durant la requete
                        _rwPCCCOUT.EnterReadLock();
                        Donnees.TAB_PCC_COUTSRow[] lignesCouts = (Donnees.TAB_PCC_COUTSRow[])Donnees.m_donnees.TAB_PCC_COUTS.Select(requete);
                        _rwPCCCOUT.ExitReadLock();
                        //Monitor.Exit(Donnees.m_donnees.TAB_PCC_COUTS);
                        if (null != lignesCouts && lignesCouts.Length>0)
                        {
                            //le trajet existe déjà à cause d'un traitement précédent (cas d'une reprise)
                            j++;
                            continue;
                        }

                        //recherche du plus court chemin
                        //Donnees.TAB_PIONRow lignePion = Donnees.m_donnees.TAB_PION[0];
                        //ClassTraitementHeure traitementtest = new ClassTraitementHeure();
                        //timeStart = DateTime.Now;
                        //Monitor.Enter(Donnees.m_donnees.TAB_CASE);
                        AStar etoile = (null == m_etoileParallele) ? new AStar() : m_etoileParallele[numeroTache];//m_etoileParallele peut être null si méthode appelée depuis le traitement de tour
                        AStar.CalculModeleMouvementsPion(out AstarTerrain[] tableCoutsMouvementsTerrain);
                        //LogFile.Notifier(string.Format("etoile n°={0} ", numeroTache));
                        if (!etoile.SearchPath(ligneCaseDepart, ligneCaseArrivee, tableCoutsMouvementsTerrain, xmin, xmax, ymin, ymax))
                        {
                            LogFile.Notifier(string.Format("CalculCheminPCCBloc:AStar : Il n'y a aucun chemin possible entre les cases {0}({1},{2}) et {3}({4},{5}), bloc ({6},{7}) posi ({8},{9})",
                                ligneCaseDepart.ID_CASE, ligneCaseDepart.I_X, ligneCaseDepart.I_Y,
                                ligneCaseArrivee.ID_CASE, ligneCaseArrivee.I_X, ligneCaseArrivee.I_Y,
                                xBloc, yBloc, i, j));
                            //dans ce cas on ajouter un chemin de coût maximum
                            //correctif : en fait, non, on s'en fiche de ces trajets
                            //Donnees.m_donnees.TAB_PCC_COUTS.AddTAB_PCC_COUTSRow(xBloc, yBloc, ligneCaseDepart.ID_CASE, ligneCaseArrivee.ID_CASE,AStar.CST_COUTMAX, -1);
                            //Donnees.m_donnees.TAB_PCC_COUTS.AddTAB_PCC_COUTSRow(xBloc, yBloc, ligneCaseArrivee.ID_CASE, ligneCaseDepart.ID_CASE, AStar.CST_COUTMAX, -1);
                        }
                        else
                        {
                            //perf = DateTime.Now - timeStart;
                            //LogFile.Notifier(string.Format("etoile.SearchPath {0} min, {1} sec, {2} mil", perf.Minutes, perf.Seconds, perf.Milliseconds));
                            //ajout des trajets et des couts
                            k = 0;
                            listeCasesTrajet.Clear();
                            //LogFile.Notifier(string.Format("etoile n°={0} m_idTrajet={1}", numeroTache, m_idTrajet));
                            List<LigneCASE> trajet = etoile.PathByNodes;
                            while (k < trajet.Count)
                            {
                                //Donnees.m_donnees.TAB_PCC_TRAJET.AddTAB_PCC_TRAJETRow(m_idTrajet, trajet[k].ID_CASE, k);
                                listeCasesTrajet.Add(trajet[k].ID_CASE);
                                k++;
                            }
                            Monitor.Enter(m_verrouIDTrajet);//m_idTrajet est une ressource partagée, si je ne veux pas que la valeur bouge entre le ++ et l'affectaction, il faut verrouiller
                            m_idTrajet++;
                            int numeroTrajet = m_idTrajet;
                            Monitor.Exit(m_verrouIDTrajet);

                            //timeStart = DateTime.Now;
                            Dal.SauvegarderTrajet(numeroTrajet, listeCasesTrajet);
                            //perf = DateTime.Now - timeStart;
                            //LogFile.Notifier(string.Format("Dal.SauvegarderTrajet {0} min, {1} sec, {2} mil", perf.Minutes, perf.Seconds, perf.Milliseconds));

                            //timeStart = DateTime.Now;
                            //Monitor.Enter(Donnees.m_donnees.TAB_PCC_COUTS);
                            _rwPCCCOUT.EnterWriteLock();
                            Donnees.m_donnees.TAB_PCC_COUTS.AddTAB_PCC_COUTSRow(xBloc, yBloc, ligneCaseDepart.ID_CASE, ligneCaseArrivee.ID_CASE, etoile.CoutGlobal, numeroTrajet, etoile.CoutGlobal, bCreation, Constantes.NULLENTIER);
                            Donnees.m_donnees.TAB_PCC_COUTS.AddTAB_PCC_COUTSRow(xBloc, yBloc, ligneCaseArrivee.ID_CASE, ligneCaseDepart.ID_CASE, etoile.CoutGlobal, numeroTrajet, etoile.CoutGlobal, bCreation, Constantes.NULLENTIER);
                            _rwPCCCOUT.ExitWriteLock();
                            nbTrajets++;
                            //Monitor.Exit(Donnees.m_donnees.TAB_PCC_COUTS);
                            //perf = DateTime.Now - timeStart;
                            //LogFile.Notifier(string.Format("Donnees.m_donnees.TAB_PCC_COUTS.AddTAB_PCC_COUTSRow {0} min, {1} sec, {2} mil", perf.Minutes, perf.Seconds, perf.Milliseconds));

                            //il faut ajouter l'aller et le retour !
                            //if (ligneCaseDepart.ID_CASE < ligneCaseArrivee.ID_CASE)
                            //{
                            //    Donnees.m_donnees.TAB_PCC_COUTS.AddTAB_PCC_COUTSRow(xBloc, yBloc, ligneCaseDepart.ID_CASE, ligneCaseArrivee.ID_CASE, m_etoile.CoutGlobal, m_idTrajet);
                            //}
                            //else
                            //{
                            //    Donnees.m_donnees.TAB_PCC_COUTS.AddTAB_PCC_COUTSRow(xBloc, yBloc, ligneCaseArrivee.ID_CASE, ligneCaseDepart.ID_CASE, m_etoile.CoutGlobal, m_idTrajet);
                            //}
                            //LogFile.Notifier(string.Format("trajet cout={0} #noeuds={1}", m_etoile.CoutGlobal, trajet.Length));
                        }
                        //Monitor.Exit(Donnees.m_donnees.TAB_CASE);
                        j++;
                    }
                }
            }
            catch (Exception e)
            {
                LogFile.Notifier("CalculCheminPCCBloc : exception =" + e.Message + " : " + e.StackTrace);
                return false;
            }
            LogFile.Notifier(string.Format("CalculCheminPCCBloc xBloc={0}, yBloc={1}, numeroTache={2}, nbTrajets={3} ", xBloc, yBloc, numeroTache, nbTrajets));
            return true;
        }

        /// <summary>
        /// Recalcul de tous les chemins entre les points de frontière du bloc et des coûts de parcours entre chaque point
        /// suite à la modification d'un modele de terrain du bloc
        /// </summary>
        /// <param name="xBloc">position en x du bloc</param>
        /// <param name="yBloc">position en y du bloc</param>
        /// <param name="bCorrection">true s'il s'agit d'une correction definitive de la carte, false sinon (par une action d'un joueur, réversible)</param>
        /// <returns>true si OK, false si KO</returns>
        public bool RecalculCheminPCCBloc(int xBloc, int yBloc, bool bCorrection)
        {
            //on supprime tous les chemins du bloc courant
            string requete = string.Format("I_BLOCX={0} AND I_BLOCY={1}", xBloc, yBloc);
            Donnees.TAB_PCC_COUTSRow[] listeTrajets = (Donnees.TAB_PCC_COUTSRow[])Donnees.m_donnees.TAB_PCC_COUTS.Select(requete);
            foreach (Donnees.TAB_PCC_COUTSRow trajet in listeTrajets)
            {
                if (bCorrection)
                {
                    if (null== m_etoileParallele)
                    {
                        m_etoileParallele = new AStar[NB_TACHES];
                        for (int i = 0; i < NB_TACHES; i++) { m_etoileParallele[i] = new AStar(); }
                    }
                }
                else
                {
                    if (trajet.B_CREATION)
                    {
                        //si c'est un trajet crée lors d'une précédente modification, il faut simplement le supprimer
                        Dal.SupprimerTrajet(trajet.ID_TRAJET, "");
                    }
                    else
                    {
                        //si c'est un trajet jamais modifié, il faut le déplacer dans les trajets "supprimés"
                        Donnees.m_donnees.TAB_PCC_COUTS_SUPPRIME.AddTAB_PCC_COUTS_SUPPRIMERow(
                            trajet.I_BLOCX,
                            trajet.I_BLOCY,
                            trajet.ID_CASE_DEBUT,
                            trajet.ID_CASE_FIN,
                            trajet.I_COUT,
                            trajet.ID_TRAJET,
                            trajet.I_COUT_INITIAL);
                    }
                }
                //dans tous les cas on supprime le trajet en base
                trajet.Delete();
            }

            //on recalcule tous les nouveaux trajets
            return CalculCheminPCCBloc(xBloc, yBloc, true, 1);
        }

        #endregion
    }
}
