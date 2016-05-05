// Copyright 2003 Eric Marchesin - <eric.marchesin@laposte.net>
//
// This source file(s) may be redistributed by any means PROVIDING they
// are not sold for profit without the authors expressed written consent,
// and providing that this notice and the authors name and all copyright
// notices remain intact.
// THIS SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED. USE IT AT YOUR OWN RISK. THE AUTHOR ACCEPTS NO
// LIABILITY FOR ANY DATA DAMAGE/LOSS THAT THIS PRODUCT MAY CAUSE.
//-----------------------------------------------------------------------
using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using WaocLib;
//using System.Linq.Expressions;


namespace vaoc
{
	/// <summary>
	/// Class to search the best path between two nodes on a graph.
	/// </summary>
	public class AStarVille
	{
		SortableListVille _Open, _Closed;
		TrackVille _LeafToGoBackUp;
		int _NbIterations = -1;

        public const int CST_COUTMAX = Int32.MaxValue;
		SortableListVille.Equality SameNodesReached = new SortableListVille.Equality( TrackVille.SameEndNode );

        //public static long SearchIdMeteo;//ID_METEO pour la recherche en cours
        //public static long SearchIdModeleMouvement;//ID_MODELE_MOUVEMENT pour la recherche en cours
        public static int[] m_tableCoutsMouvementsTerrain;
        private static int m_nombrePixelParCase;
        private const double SQRT2 = 1.4142135623730950488016887242097;
        private int m_minX, m_maxX, m_minY, m_maxY;
        private int m_idVilleDestination;//ville de destination finale
        private bool m_bParcoursVille;//true si on a trouvé une ville sur le parcours, false sinon
        private bool m_bPremiereVille;//true si on cherche seulement la ville la plus proche, false sinon
        private bool m_bParcoursInverse;//renvoie le parcours inverse de l'ordre, optimisation particulière
        private int m_espace;//espace recherché
        private int m_espaceTrouve;//nombre d'espace trouvé

        #region constantes
        public const char CST_CHEMIN='C';
        public const char CST_DEPART = 'D';
        public const char CST_DESTINATION = 'A';
        #endregion
        /// <summary>
		/// AStar Constructor.
		/// </summary>
		/// <param name="G">The graph on which AStar will perform the search.</param>
        public AStarVille(/*Graph G*/)
		{
			_Open = new SortableListVille();
			_Closed = new SortableListVille();
		}

        /// <summary>
        /// Searches for the best path to reach the specified EndNode from the specified StartNode.
        /// </summary>
        /// <exception cref="ArgumentNullException">StartNode and EndNode cannot be null.</exception>
        /// <param name="StartNode">The node from which the path must start.</param>
        /// <param name="EndNode">The node to which the path must end.</param>
        /// <param name="tableCoutsMouvementsTerrain">couts de mouvement suivant les terrains</param>
        /// <returns>'true' if succeeded / 'false' if failed.</returns>
        public bool SearchPath(Donnees.TAB_CASERow StartNode, Donnees.TAB_CASERow EndNode, int[] tableCoutsMouvementsTerrain)
        {
            return SearchPath(StartNode, EndNode, tableCoutsMouvementsTerrain, 0, 0, 0, 0);
        }

        /// <summary>
		/// Searches for the best path to reach the specified EndNode from the specified StartNode.
		/// </summary>
		/// <exception cref="ArgumentNullException">StartNode and EndNode cannot be null.</exception>
		/// <param name="StartNode">The node from which the path must start.</param>
		/// <param name="EndNode">The node to which the path must end.</param>
        /// <param name="tableCoutsMouvementsTerrain">couts de mouvement suivant les terrains</param>
        /// <param name="xmin"> valeur x minimale pour la recherche</param>
        /// <param name="xmax"> valeur x maximale pour la recherche</param>
        /// <param name="ymin"> valeur y minimale pour la recherche</param>
        /// <param name="ymax"> valeur y maximale pour la recherche</param>
        /// <returns>'true' if succeeded / 'false' if failed.</returns>
        public bool SearchPath(Donnees.TAB_CASERow StartNode, Donnees.TAB_CASERow EndNode, int[] tableCoutsMouvementsTerrain, int xmin, int xmax, int ymin, int ymax)
        {
            DateTime timeStart;
            TimeSpan perf;

            m_tableCoutsMouvementsTerrain = tableCoutsMouvementsTerrain;
            m_nombrePixelParCase = Donnees.m_donnees.TAB_JEU[0].I_ECHELLE;
            m_minX = xmin;
            m_minY = ymin;
            m_maxX = xmax;
            m_maxY = ymax;
            m_idVilleDestination = -1;
            //SearchIdMeteo = idMeteo;//ID_METEO pour la recherche en cours
            //SearchIdModeleMouvement = idModeleMouvement;//ID_MODELE_MOUVEMENT pour la recherche en cours

            Debug.WriteLine(string.Format("Début de AStar.SearchPath de {0}({1},{2}) à {3}({4},{5})",
                StartNode.ID_CASE, StartNode.I_X, StartNode.I_Y, EndNode.ID_CASE, EndNode.I_X, EndNode.I_Y));
            timeStart = DateTime.Now;
            Initialize(new Cible(null,StartNode), new Cible(null,EndNode));
			while ( NextStep() ) {}
            perf = DateTime.Now-timeStart;
            Debug.WriteLine(string.Format("AStarVille.SearchPath en {0} heures, {1} minutes, {2} secondes, {3} millisecondes", perf.Hours, perf.Minutes, perf.Seconds, perf.Milliseconds));
            return PathFound;
		}

        //public bool SearchPathVilles(Donnees.TAB_CASERow StartNode, int id_nomCarteDestination, Donnees.TAB_CASERow EndNode, int[] tableCoutsMouvementsTerrain)
        //{
        //    if (id_nomCarteDestination<0)
        //    {
        //        return SearchPathVilles(StartNode, null, EndNode, tableCoutsMouvementsTerrain);
        //    }
        //    Donnees.TAB_NOMS_CARTERow ligneVille = Donnees.m_donnees.TAB_NOMS_CARTE.FindByID_NOM(id_nomCarteDestination);
        //    return SearchPathVilles(StartNode, ligneVille, EndNode, tableCoutsMouvementsTerrain);
        //}

        /// <summary>
        /// Searches for the best path to reach the specified EndNode from the specified StartNode. avec une ville destination
        /// </summary>
        /// <exception cref="ArgumentNullException">StartNode and EndNode cannot be null.</exception>
        /// <param name="StartNode">The node from which the path must start.</param>
        /// <param name="EndTown">La ville parlaquelle on doit arriver avant d'aller au EndNode.</param>
        /// <param name="EndNode">The node to which the path must end.</param>
        /// <param name="tableCoutsMouvementsTerrain">couts de mouvement suivant les terrains</param>
        /// <returns>'true' if succeeded / 'false' if failed.</returns>
        public bool SearchPathVilles(Donnees.TAB_CASERow StartNode, Donnees.TAB_NOMS_CARTERow EndTown, Donnees.TAB_CASERow EndNode, int[] tableCoutsMouvementsTerrain)
        {
            DateTime timeStart;
            TimeSpan perf;
            Donnees.TAB_NOMS_CARTERow villeFinale;

            Debug.WriteLine(string.Format("Début de AStarVille.SearchPathVilles de {0}({1},{2}) à {3}({4},{5})",
                StartNode.ID_CASE, StartNode.I_X, StartNode.I_Y, EndNode.ID_CASE, EndNode.I_X, EndNode.I_Y));
            timeStart = DateTime.Now;

            m_tableCoutsMouvementsTerrain = tableCoutsMouvementsTerrain;
            m_nombrePixelParCase = Donnees.m_donnees.TAB_JEU[0].I_ECHELLE;
            m_minX = 0;
            m_minY = 0;
            m_maxX = Donnees.m_donnees.TAB_JEU[0].I_LARGEUR_CARTE;
            m_maxY = Donnees.m_donnees.TAB_JEU[0].I_HAUTEUR_CARTE;

            if (null == EndTown)
            {
                // dans ce cas on recherche la ville la plus proche de la ville finale, donc on remonte dans le sens inverse demandé
                //ClassMessager.CaseVersZoneGeographique(EndNode, out villeFinale); -> rapide mais ne marche pas si la ville la plus proche est dérrière un fleuve par exemple
                Initialize(new Cible(null, EndNode), new Cible(null, StartNode));
                m_bPremiereVille = true;//mis à false dans l'initialize, on s'arrête  la première ville rencontrée
                while (NextStepVille()) { }
                if (!PathFound) return PathFound;//il n'y a pas de chemin entre les deux points
                //on recherche la premiere ville trouvée sur le parcours
                List<Donnees.TAB_CASERow> listeCase = this.PathByNodes;
                Donnees.TAB_CASERow ligneCaseVille = listeCase[listeCase.Count-1];
                if (ligneCaseVille.ID_CASE == StartNode.ID_CASE)
                {
                    //il n'y a pas de villes sur le parcours puisque l'on a trouvé la case finale de destination !
                    //inutile de le refaire, par contre, il faut l'inverser
                    m_bParcoursInverse = true;
                    return PathFound;
                }
                if (ligneCaseVille.IsID_NOMNull()) return false;//ne devrait jamais arrivé !
                villeFinale = Donnees.m_donnees.TAB_NOMS_CARTE.FindByID_NOM(ligneCaseVille.ID_NOM);
            }
            else
            {
                villeFinale = EndTown;
            }

            Initialize(new Cible(null, StartNode), new Cible(villeFinale, EndNode));
            while (NextStepVille()) { }

            perf = DateTime.Now - timeStart;
            Debug.WriteLine(string.Format("AStar.SearchPathVilles en {0} heures, {1} minutes, {2} secondes, {3} millisecondes", perf.Hours, perf.Minutes, perf.Seconds, perf.Milliseconds));
            return PathFound;
        }
        
        /// <summary>
		/// Use for a 'step by step' search only. This method is alternate to SearchPath.
		/// Initializes AStar before performing search steps manually with NextStep.
		/// </summary>
		/// <exception cref="ArgumentNullException">StartNode and EndNode cannot be null.</exception>
		/// <param name="StartNode">The node from which the path must start.</param>
		/// <param name="EndNode">The node to which the path must end.</param>
        public void Initialize(Cible StartNode, Cible EndNode)
		{
			if ( StartNode==null || EndNode==null ) throw new ArgumentNullException();
			_Closed.Clear();
			_Open.Clear();
			TrackVille.Target = EndNode;
            if (null != EndNode.Ville)
            {
                m_idVilleDestination = EndNode.Ville.ID_NOM;
            }
            else
            {
                m_idVilleDestination = -1;
            }
            _Open.Add(new TrackVille(StartNode));
			_NbIterations = 0;
			_LeafToGoBackUp = null;
            m_bParcoursVille = false;
            m_bPremiereVille = false;
            m_bParcoursInverse = false;
        }

		/// <summary>
		/// Use for a 'step by step' search only. This method is alternate to SearchPath.
		/// The algorithm must have been initialize before.
		/// </summary>
		/// <exception cref="InvalidOperationException">You must initialize AStar before using NextStep().</exception>
		/// <returns>'true' unless the search ended.</returns>
		private bool NextStep()
		{
			//if ( !Initialized ) throw new InvalidOperationException("You must initialize AStar before launching the algorithm.");
			if ( _Open.Count==0 ) return false;
			_NbIterations++;
            Debug.WriteLineIf(_NbIterations % 10000 == 0, "_NbIterations=" + _NbIterations.ToString());

			int IndexMin = _Open.IndexOfMin();
			//TrackVille BestTrackVille = (TrackVille)_Open[IndexMin];//on repart toujours du moins couteux
            TrackVille BestTrackVille = (TrackVille)_Open.ValueOfCout(IndexMin);
            if (_NbIterations % 10000 == 0)
            {
                Debug.WriteLine("BestTrack.Cost=" + BestTrackVille.Cost.ToString() + " BestTrack.EndNode.ID_CASE=" + BestTrackVille.EndNode.Case.ID_CASE.ToString());
                Debug.WriteLine("_Open.Count=" + _Open.Count.ToString() + "_Open.CountCout=" + _Open.CountCout.ToString());
                Debug.WriteLine("_Closed.Count=" + _Closed.Count.ToString() + "_Closed.CountCout=" + _Closed.CountCout.ToString());
            }
            //_Open.RemoveAt(IndexMin);
            _Open.Remove(BestTrackVille);
			if ( BestTrackVille.Succeed )
			{
                Debug.WriteLine(string.Format("BestTrack Case {0}({1},{2}), cout={3} en _NbIterations={4}",
                    BestTrackVille.EndNode.Case.ID_CASE,
                    BestTrackVille.EndNode.Case.I_X,
                    BestTrackVille.EndNode.Case.I_Y,
                    BestTrackVille.Cost,
                    _NbIterations));
                _LeafToGoBackUp = BestTrackVille;
				_Open.Clear();
			}
			else
			{
				Propagate(BestTrackVille);
				_Closed.Add(BestTrackVille);
			}
            return _Open.Count > 0;
		}

        /// <summary>
        /// Use for a 'step by step' search only. This method is alternate to SearchPath.
        /// The algorithm must have been initialize before.
        /// </summary>
        /// <exception cref="InvalidOperationException">You must initialize AStar before using NextStep().</exception>
        /// <returns>'true' unless the search ended.</returns>
        private bool NextStepVille()
        {
            if (!Initialized) throw new InvalidOperationException("You must initialize AStar before launching the algorithm.");
            if (_Open.Count == 0) return false;
            _NbIterations++;
            Debug.WriteLineIf(_NbIterations % 10000 == 0, "_NbIterations=" + _NbIterations.ToString());

            int IndexMin = _Open.IndexOfMin();
            //TrackVille BestTrackVille = (TrackVille)_Open[IndexMin];//on repart toujours du moins couteux
            TrackVille BestTrackVille = (TrackVille)_Open.ValueOfCout(IndexMin);
            /* debug
            if (null==BestTrackVille.EndNodeVille)
                Debug.WriteLine(string.Format("BestTrackVille Case {0}({1},{2}), cout={3}",
                    BestTrackVille.EndNode.Case.ID_CASE,
                    BestTrackVille.EndNode.Case.I_X,
                    BestTrackVille.EndNode.Case.I_Y,
                    BestTrackVille.Cost));
            else
            {
                Donnees.TAB_NOMS_CARTERow lVDebug1 = Donnees.m_donnees.TAB_NOMS_CARTE.FindByID_NOM(BestTrackVille.EndNodeVille.ID_VILLE_DEBUT);
                Donnees.TAB_NOMS_CARTERow lVDebug2 = Donnees.m_donnees.TAB_NOMS_CARTE.FindByID_NOM(BestTrackVille.EndNodeVille.ID_VILLE_FIN);
                Donnees.TAB_CASERow lDebug1 = Donnees.m_donnees.TAB_CASE.FindByID_CASE(lVDebug1.ID_CASE);
                Donnees.TAB_CASERow lDebug2 = Donnees.m_donnees.TAB_CASE.FindByID_CASE(lVDebug2.ID_CASE);
                Debug.WriteLine(string.Format("BestTrackVille trajet {0}:{1}({2},{3}) -> {4}:{5}({6},{7}), cout={8}",
                    lVDebug1.S_NOM,
                    lDebug1.ID_CASE,
                    lDebug1.I_X,
                    lDebug1.I_Y,
                    lVDebug2.S_NOM,
                    lDebug2.ID_CASE,
                    lDebug2.I_X,
                    lDebug2.I_Y,
                    BestTrackVille.Cost
                    ));
            }
             /**/

            if (_NbIterations % 10000 == 0)
            {
                Debug.WriteLine("BestTrackVille.Cost=" + BestTrackVille.Cost.ToString());
                Debug.WriteLine("_Open.Count=" + _Open.Count.ToString() + "_Open.CountCout=" + _Open.CountCout.ToString());
                Debug.WriteLine("_Closed.Count=" + _Closed.Count.ToString() + "_Closed.CountCout=" + _Closed.CountCout.ToString());
            }
            //_Open.RemoveAt(IndexMin);
            _Open.Remove(BestTrackVille);
            if (BestTrackVille.Succeed)
            {
                Debug.WriteLine(string.Format("BestTrackVille cout={0} en _NbIterations={1}",
                    BestTrackVille.Cost,
                    _NbIterations));
                _LeafToGoBackUp = BestTrackVille;
                _Open.Clear();
            }
            else
            {
                PropagateVille(BestTrackVille);
                _Closed.Add(BestTrackVille);
            }
            return _Open.Count > 0;
        }

        private void PropagateVille(TrackVille TrackVilleToPropagate)
        {
            foreach (TrackVille A in CasesVoisinesVille(TrackVilleToPropagate))
            {
                if (CST_COUTMAX == Cout(TrackVilleToPropagate, A))
                {
                    continue;//case intraversable
                }
                TrackVille Successor = new TrackVille(TrackVilleToPropagate, A);// le cout est calculé dans le new
                int PosNF = _Closed.IndexOf(Successor, SameNodesReached);
                int PosNO = _Open.IndexOf(Successor, SameNodesReached);
                if (PosNF >= 0 && Successor.Cost >= ((TrackVille)_Closed[PosNF]).Cost) continue;
                if (PosNO >= 0 && Successor.Cost >= ((TrackVille)_Open[PosNO]).Cost) continue;
                if (PosNF >= 0)
                {
                    _Closed.RemoveAt(PosNF);
                }
                if (PosNO >= 0)
                {
                    _Open.RemoveAt(PosNO);
                }
                _Open.Add(Successor);
                /* debug
                if (null == Successor.EndNodeVille)
                    Debug.WriteLine(string.Format("Successor Case {0}({1},{2}), cout={3}",
                        Successor.EndNode.Case.ID_CASE,
                        Successor.EndNode.Case.I_X,
                        Successor.EndNode.Case.I_Y,
                        Successor.Cost));
                else
                {
                    Donnees.TAB_NOMS_CARTERow lVDebug1 = Donnees.m_donnees.TAB_NOMS_CARTE.FindByID_NOM(Successor.EndNodeVille.ID_VILLE_DEBUT);
                    Donnees.TAB_NOMS_CARTERow lVDebug2 = Donnees.m_donnees.TAB_NOMS_CARTE.FindByID_NOM(Successor.EndNodeVille.ID_VILLE_FIN);
                    Donnees.TAB_CASERow lDebug1 = Donnees.m_donnees.TAB_CASE.FindByID_CASE(lVDebug1.ID_CASE);
                    Donnees.TAB_CASERow lDebug2 = Donnees.m_donnees.TAB_CASE.FindByID_CASE(lVDebug2.ID_CASE);
                    Debug.WriteLine(string.Format("Successor trajet {0}:{1}({2},{3}) -> {4}:{5}({6},{7}), cout={8}",
                        lVDebug1.S_NOM,
                        lDebug1.ID_CASE,
                        lDebug1.I_X,
                        lDebug1.I_Y,
                        lVDebug2.S_NOM,
                        lDebug2.ID_CASE,
                        lDebug2.I_X,
                        lDebug2.I_Y,
                        Successor.Cost
                        ));
                }
                  /* */
            }
        }

        private void Propagate(TrackVille TrackVilleToPropagate)
		{
            foreach (Donnees.TAB_CASERow A in Donnees.m_donnees.TAB_CASE.CasesVoisines(TrackVilleToPropagate.EndNode.Case))
			{
                if (CST_COUTMAX == Cout(TrackVilleToPropagate.EndNode.Case, A))
                {
                    continue;//case intraversable
                }
                if ((m_minX!=m_maxX) && (A.I_X < m_minX || A.I_Y < m_minY || A.I_X > m_maxX || A.I_Y > m_maxY))
                {
                    continue;
                }
                TrackVille Successor = new TrackVille(TrackVilleToPropagate, new Cible(null,A));// le cout est calculé dans le new
				int PosNF = _Closed.IndexOf(Successor, SameNodesReached);
				int PosNO = _Open.IndexOf(Successor, SameNodesReached);
				if ( PosNF>=0 && Successor.Cost>=((TrackVille)_Closed[PosNF]).Cost ) continue;
				if ( PosNO>=0 && Successor.Cost>=((TrackVille)_Open[PosNO]).Cost ) continue;
                if (PosNF >= 0)
                {
                    _Closed.RemoveAt(PosNF);
                }
                if (PosNO >= 0)
                {
                    _Open.RemoveAt(PosNO);
                }
				_Open.Add(Successor);
			}
		}

		/// <summary>
		/// To know if the search has been initialized.
		/// </summary>
		public bool Initialized { get { return _NbIterations>=0; } }

		/// <summary>
		/// To know if the search has been started.
		/// </summary>
		public bool SearchStarted { get { return _NbIterations>0; } }

		/// <summary>
		/// To know if the search has ended.
		/// </summary>
		public bool SearchEnded { get { return SearchStarted && _Open.Count==0; } }

		/// <summary>
		/// To know if a path has been found.
		/// </summary>
		public bool PathFound { get { return _LeafToGoBackUp!=null; } }

		/// <summary>
		/// Use for a 'step by step' search only.
		/// Gets the number of the current step.
		/// -1 if the search has not been initialized.
		/// 0 if it has not been started.
		/// </summary>
		public int StepCounter { get { return _NbIterations; } }

		private void CheckSearchHasEnded()
		{
			if ( !SearchEnded ) throw new InvalidOperationException("You cannot get a result unless the search has ended.");
		}

		/// <summary>
		/// Returns information on the result.
		/// </summary>
		/// <param name="NbArcsOfPath">The number of arcs in the result path / -1 if no result.</param>
		/// <param name="CostOfPath">The cost of the result path / -1 if no result.</param>
		/// <returns>'true' if the search succeeded / 'false' if it failed.</returns>
		public bool ResultInformation(out int NbArcsOfPath, out double CostOfPath)
		{
			CheckSearchHasEnded();
			if ( !PathFound )
			{
				NbArcsOfPath = -1;
				CostOfPath = -1;
				return false;
			}
			else
			{
				NbArcsOfPath = _LeafToGoBackUp.NbArcsVisited;
				CostOfPath = _LeafToGoBackUp.Cost;
				return true;
			}
		}

        public int CoutGlobal
        {
            get
            {
                int NbArcsOfPath; 
                double CostOfPath;
                if (!ResultInformation(out NbArcsOfPath, out CostOfPath))
                {
                    return -1;
                }
                return Convert.ToInt32(CostOfPath);
            }
        }

		/// <summary>
		/// Gets the array of nodes representing the found path.
		/// </summary>
		/// <exception cref="InvalidOperationException">You cannot get a result unless the search has ended.</exception>
        public List<Donnees.TAB_CASERow> PathByNodes
		{
			get 
			{
				CheckSearchHasEnded();
				if ( !PathFound ) return null;
				return GoBackUpNodes(_LeafToGoBackUp);
			}
		}

        /// <summary>
        /// On redonne le parcours de case à partir de la track de destination jusqu'au point de départ
        /// </summary>
        /// <param name="T">track de destination</param>
        /// <returns>liste des cases du trajet de la source à la destination</returns>
        private List<Donnees.TAB_CASERow> GoBackUpNodes(TrackVille T)
		{
			int Nb = T.NbArcsVisited,j;
            List<Donnees.TAB_CASERow> Path = new List<Donnees.TAB_CASERow>();
            List<int> listeCases;
            for (int i = Nb; i >= 0; i--, T = T.Queue)
            {
                if (null != T.EndNode)
                {
                    if (m_bParcoursInverse)
                    {
                        Path.Add(T.EndNode.Case);
                    }
                    else
                    {
                        Path.Insert(0, T.EndNode.Case);
                    }
                }
                else
                {
                    /* debug
                    Donnees.TAB_NOMS_CARTERow lVDebug1 = Donnees.m_donnees.TAB_NOMS_CARTE.FindByID_NOM(T.EndNodeVille.ID_VILLE_DEBUT);
                    Donnees.TAB_NOMS_CARTERow lVDebug2 = Donnees.m_donnees.TAB_NOMS_CARTE.FindByID_NOM(T.EndNodeVille.ID_VILLE_FIN);
                    Donnees.TAB_CASERow lDebug1 = Donnees.m_donnees.TAB_CASE.FindByID_CASE(lVDebug1.ID_CASE);
                    Donnees.TAB_CASERow lDebug2 = Donnees.m_donnees.TAB_CASE.FindByID_CASE(lVDebug2.ID_CASE);
                    Debug.WriteLine(string.Format("GoBackUpNodes ajout de trajet {0}:{1}({2},{3}) -> {4}:{5}({6},{7})",
                        lVDebug1.S_NOM,
                        lDebug1.ID_CASE,
                        lDebug1.I_X,
                        lDebug1.I_Y,
                        lVDebug2.S_NOM,
                        lDebug2.ID_CASE,
                        lDebug2.I_X,
                        lDebug2.I_Y
                        ));
                     /* */
                    if (Dal.ChargerTrajet(T.EndNodeVille.ID_TRAJET,Constantes.CST_TRAJET_VILLE, out listeCases))
                    {
                        //un seul trajet quel que soit le sens de parcours
                        Donnees.TAB_NOMS_CARTERow ligneVille = Donnees.m_donnees.TAB_NOMS_CARTE.FindByID_NOM(T.EndNodeVille.ID_VILLE_DEBUT);
                        Donnees.TAB_CASERow ligneCaseVille = Donnees.m_donnees.TAB_CASE.FindByID_CASE(ligneVille.ID_CASE);
                        if (listeCases[0] == ligneCaseVille.ID_CASE)
                        {
                            j = listeCases.Count - 1;
                            while (j >= 0)
                            {
                                Donnees.TAB_CASERow ligneCase = Donnees.m_donnees.TAB_CASE.FindByID_CASE(listeCases[j--]);
                                if (m_bParcoursInverse)
                                {
                                    Path.Add(ligneCase);
                                }
                                else
                                {
                                    Path.Insert(0, ligneCase);
                                }
                            }
                        }
                        else
                        {
                            //ajout de toutes les cases du trajet
                            j = 0;
                            while (j < listeCases.Count)
                            {
                                Donnees.TAB_CASERow ligneCase = Donnees.m_donnees.TAB_CASE.FindByID_CASE(listeCases[j++]);
                                if (m_bParcoursInverse)
                                {
                                    Path.Add(ligneCase);
                                }
                                else
                                {
                                    Path.Insert(0, ligneCase);
                                }
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("Astar : GoBackUpNodes : erreur sur Dal.ChargerTrajet pour idtrajet=" + T.EndNodeVille.ID_TRAJET.ToString());
                    }
                }
            }
			return Path;
		}

        private System.Collections.Generic.IList<TrackVille> CasesVoisinesVille(TrackVille TrackVilleSource)
        {
            System.Collections.Generic.IList<TrackVille> listeRetour;
            /* debug
            Donnees.TAB_CASERow lDebug1, lDebug2;
            Donnees.TAB_NOMS_CARTERow lVDebug1, lVDebug2;
            /* */

            listeRetour = new List<TrackVille>();
            if (null != TrackVilleSource.EndNode)
            {
                //on se trouve sur une case
                if (null == TrackVilleSource.EndNode.Ville)
                {
                    //la ville cible n'a pas encore été trouvée, on cherche toujours à tomber sur une première ville
                    if (!TrackVilleSource.EndNode.Case.IsID_NOMNull()) /*&& 
                        (TrackVilleSource.EndNode.Case.ID_NOM != m_idVilleDestination))//test necessaire dans le cas où la première ville rencontrée est la ville de départ */
                                                                                        //test sur 648, 244 ->665, 215
                    {
                        if (m_bPremiereVille)
                        {
                            //on ne souhaite pas aller plus loin, on arrête là le parcours car c'est le parcours pour trouver la ville
                            //de destination la plus proche si elle n'est pas fournie dans l'appel initial
                            TrackVille.Target = new Cible(null,TrackVilleSource.EndNode.Case);
                            _Open.Clear();
                            _Open.Add(TrackVilleSource);
                            return listeRetour;
                        }
                        if (TrackVilleSource.EndNode.Case.ID_NOM == m_idVilleDestination)
                        {
                            //ville la plus proche de la case source est la même que la ville la plus proche de la case destination
                            //autant donc faire une recherche classique
                            //pour simuler cela, on indique que la ville cible a été trouvée à toutes les cases en cours
                            //et on ajoute toutes les cases voisines !
                            Donnees.TAB_NOMS_CARTERow lignesVille = Donnees.m_donnees.TAB_NOMS_CARTE.FindByID_NOM(m_idVilleDestination);
                            foreach (TrackVille trackVille in _Open)
                            {
                                trackVille.EndNode.Ville = lignesVille;
                            }
                            
                            IList<Donnees.TAB_CASERow> tableCases = Donnees.m_donnees.TAB_CASE.CasesVoisines(TrackVilleSource.EndNode.Case);
                            foreach (Donnees.TAB_CASERow ligneCase in tableCases)
                            {
                                /* debug
                                Debug.WriteLine(string.Format("CasesVoisinesVille ajout de la case : ID={0}({1},{2}) ",
                                    ligneCase.ID_CASE, ligneCase.I_X, ligneCase.I_Y));
                                /**/
                                listeRetour.Add(new TrackVille(TrackVilleSource, new Cible(lignesVille, ligneCase)));
                            }
                        }
                        else
                        {

                            //On ajoute tous les parcours avec les villes voisines
                            string requete = string.Format("ID_VILLE_DEBUT={0}",
                                TrackVilleSource.EndNode.Case.ID_NOM);

                            Donnees.TAB_PCC_VILLESRow[] lignesCout = (Donnees.TAB_PCC_VILLESRow[])Donnees.m_donnees.TAB_PCC_VILLES.Select(requete);
                            foreach (Donnees.TAB_PCC_VILLESRow ligneCout in lignesCout)
                            {
                                listeRetour.Add(new TrackVille(ligneCout));
                                /* debug
                                lVDebug1 = Donnees.m_donnees.TAB_NOMS_CARTE.FindByID_NOM(TrackVilleSource.EndNode.Case.ID_NOM);
                                lVDebug2 = Donnees.m_donnees.TAB_NOMS_CARTE.FindByID_NOM(ligneCout.ID_VILLE_FIN);
                                lDebug1 = Donnees.m_donnees.TAB_CASE.FindByID_CASE(lVDebug1.ID_CASE);
                                lDebug2 = Donnees.m_donnees.TAB_CASE.FindByID_CASE(lVDebug2.ID_CASE);
                                Debug.WriteLine(string.Format("CasesVoisinesVille ajout de trajet {0},{8}:{1}({2},{3}) -> {4},{9}:{5}({6},{7})",
                                    lVDebug1.S_NOM,
                                    lDebug1.ID_CASE,
                                    lDebug1.I_X,
                                    lDebug1.I_Y,
                                    lVDebug2.S_NOM,
                                    lDebug2.ID_CASE,
                                    lDebug2.I_X,
                                    lDebug2.I_Y,
                                    lVDebug1.ID_NOM,
                                    lVDebug2.ID_NOM
                                    ));
                                /* */
                            }
                            m_bParcoursVille = true;
                        }
                    }
                    else
                    {
                        if (m_bParcoursVille) return listeRetour; //on est passé en mode pure ville dés que l'on en rencontre une
                        IList<Donnees.TAB_CASERow> tableCases = Donnees.m_donnees.TAB_CASE.CasesVoisines(TrackVilleSource.EndNode.Case);
                        foreach (Donnees.TAB_CASERow ligneCase in tableCases)
                        {
                            /* debug
                            Debug.WriteLine(string.Format("CasesVoisinesVille ajout de la case : ID={0}({1},{2}) ",
                                ligneCase.ID_CASE, ligneCase.I_X, ligneCase.I_Y));
                             /**/
                            listeRetour.Add(new TrackVille(TrackVilleSource, new Cible(null, ligneCase)));
                        }
                    }
                }
                else
                {
                    //la ville cible a déjà été trouvée, on cherche maintenant à rejoindre la case finale
                    IList<Donnees.TAB_CASERow> tableCases = Donnees.m_donnees.TAB_CASE.CasesVoisines(TrackVilleSource.EndNode.Case);
                    foreach (Donnees.TAB_CASERow ligneCase in tableCases)
                    {
                        /* debug
                        Debug.WriteLine(string.Format("CasesVoisinesVille ajout de la case : ID={0}({1},{2}) ",
                            ligneCase.ID_CASE, ligneCase.I_X, ligneCase.I_Y));
                         /**/
                        listeRetour.Add(new TrackVille(TrackVilleSource, new Cible(TrackVilleSource.EndNode.Ville, ligneCase)));
                    }
                }
            }
            if (null != TrackVilleSource.EndNodeVille)
            {
                //on se trouve sur un parcours de ville
                if (TrackVilleSource.EndNodeVille.ID_VILLE_FIN == TrackVille.Target.Ville.ID_NOM)
                {
                    //on est arrivé à la ville cible, on cherche maintenant les cases pour rejoindre la case finale
                    Donnees.TAB_CASERow ligneCaseVille = Donnees.m_donnees.TAB_CASE.FindByID_CASE(TrackVille.Target.Ville.ID_CASE);
                    IList<Donnees.TAB_CASERow> tableCases = Donnees.m_donnees.TAB_CASE.CasesVoisines(ligneCaseVille);
                    foreach (Donnees.TAB_CASERow ligneCase in tableCases)
                    {
                        /* debug
                        Debug.WriteLine(string.Format("CasesVoisinesVille ajout de la case : ID={0}({1},{2}) ",
                            ligneCase.ID_CASE, ligneCase.I_X, ligneCase.I_Y));
                         /**/
                        listeRetour.Add(new TrackVille(TrackVilleSource, new Cible(TrackVille.Target.Ville, ligneCase)));
                    }
                    //on ajoute également la case cible au cas où l'on arrive exactement sur la case cible
                    listeRetour.Add(new TrackVille(TrackVilleSource, new Cible(TrackVille.Target.Ville, ligneCaseVille)));
                    /* debug
                    Debug.WriteLine(string.Format("CasesVoisinesVille ajout de la case : ID={0}({1},{2}) ",
                        ligneCaseVille.ID_CASE, ligneCaseVille.I_X, ligneCaseVille.I_Y));
                    /**/
                }
                else
                {
                    //on est pas encore sur la ville finale, il faut donc ajouter les villes voisines suivantes
                    string requete = string.Format("ID_VILLE_DEBUT={0}",
                        TrackVilleSource.EndNodeVille.ID_VILLE_FIN);

                    Donnees.TAB_PCC_VILLESRow[] lignesCout = (Donnees.TAB_PCC_VILLESRow[])Donnees.m_donnees.TAB_PCC_VILLES.Select(requete);
                    foreach (Donnees.TAB_PCC_VILLESRow ligneCout in lignesCout)
                    {
                        listeRetour.Add(new TrackVille(ligneCout));
                        /* debug
                        lVDebug1 = Donnees.m_donnees.TAB_NOMS_CARTE.FindByID_NOM(ligneCout.ID_VILLE_DEBUT);
                        lVDebug2 = Donnees.m_donnees.TAB_NOMS_CARTE.FindByID_NOM(ligneCout.ID_VILLE_FIN);
                        lDebug1 = Donnees.m_donnees.TAB_CASE.FindByID_CASE(lVDebug1.ID_CASE);
                        lDebug2 = Donnees.m_donnees.TAB_CASE.FindByID_CASE(lVDebug2.ID_CASE);
                        Debug.WriteLine(string.Format("CasesVoisinesVille ajout de trajet {0}:{1}({2},{3}) -> {4}:{5}({6},{7})",
                            lVDebug1.S_NOM,
                            lDebug1.ID_CASE,
                            lDebug1.I_X,
                            lDebug1.I_Y,
                            lVDebug2.S_NOM,
                            lDebug2.ID_CASE,
                            lDebug2.I_X,
                            lDebug2.I_Y
                            ));
                         /* */
                    }
                }
            }
            return listeRetour;
        }

        /// <summary>
        /// Calcul du cout entre un track de départ et d'arrivée
        /// </summary>
        /// <param name="TrackVilleSource">Track de départ</param>
        /// <param name="TrackVilleDestination">Track d'arrivée</param>
        /// <returns></returns>
        static public double Cout(TrackVille TrackVilleSource, TrackVille TrackVilleDestination)
        {
            if (null != TrackVilleSource.EndNode && null != TrackVilleDestination.EndNode)
            {
                //case vers case
                return Cout(TrackVilleSource.EndNode.Case, TrackVilleDestination.EndNode.Case);
            }
            if (null != TrackVilleSource.EndNodeVille && null != TrackVilleDestination.EndNodeVille)
            {
                //ville vers ville
                return TrackVilleDestination.EndNodeVille.I_COUT;
            }
            if (null != TrackVilleSource.EndNode && null != TrackVilleDestination.EndNodeVille)
            {
                //case vers ville
                //Donnees.TAB_NOMS_CARTERow ligneVille = Donnees.m_donnees.TAB_NOMS_CARTE.FindByID_NOM(TrackVilleDestination.EndNodeVille.ID_VILLE_FIN);
                //Donnees.TAB_CASERow caseVille = Donnees.m_donnees.TAB_CASE.FindByID_CASE(ligneVille.ID_CASE);
                //return Cout(TrackVilleSource.EndNode.Case, caseVille);
                return TrackVilleDestination.EndNodeVille.I_COUT;
            }
            if (null != TrackVilleSource.EndNodeVille && null != TrackVilleDestination.EndNode)
            {
                //ville vers case
                Donnees.TAB_NOMS_CARTERow ligneVille = Donnees.m_donnees.TAB_NOMS_CARTE.FindByID_NOM(TrackVilleSource.EndNodeVille.ID_VILLE_FIN);
                Donnees.TAB_CASERow caseVille = Donnees.m_donnees.TAB_CASE.FindByID_CASE(ligneVille.ID_CASE);
                return Cout(caseVille, TrackVilleDestination.EndNode.Case);
            }
            throw new NotSupportedException("Erreur : Cout pour AStarVille non défini");//normalement, on ne devrait jamais arriver là
        }

        //cout de déplacement entre deux cases
        static public double Cout(Donnees.TAB_CASERow caseSource, Donnees.TAB_CASERow caseFinale)
        {
            //DateTime timeStart;
            //TimeSpan perf;
            //timeStart = DateTime.Now;
            //if (caseFinale.ID_PROPRIETAIRE >= 0) -> Ne pas faire cela, sinon une unité entourée d'unités amies ne pourra plus se déplacer
            //{
            //    return Int32.MaxValue;//case déjà occupée
            //}

            double retour = -1;

            if (caseSource.I_X == caseFinale.I_X || caseSource.I_Y == caseFinale.I_Y)
            {
                //ligne droite
                retour = m_nombrePixelParCase * m_tableCoutsMouvementsTerrain[caseFinale.ID_MODELE_TERRAIN];//resCout[0].I_COUT;
            }
            else
            {
                //diagonale
                retour = (int)(m_nombrePixelParCase * SQRT2 * m_tableCoutsMouvementsTerrain[caseFinale.ID_MODELE_TERRAIN]);
            }

            if (retour <= 0)
            {
                return CST_COUTMAX;//case intraversable
            }
            return retour;
        }

        /// <summary>
        /// Recherche toutes les cases avec un cout de mouvement minimum autour d'un point donné. Les cases déjà occupées ne sont pas considérées
        /// lors du calcul de coût mais uniquement lors de la recherche de cases disponibles.
        /// </summary>
        /// <param name="ligneCase">Case d'origine</param>
        /// <param name="espace">Nombre de cases à trouver</param>
        /// <param name="tableCoutsMouvementsTerrain">couts de mouvement suivant les terrains</param>
        /// <param name="nombrePixelParCase>echelle en pixels par case</param>
        /// <param name="listeCaseEspace>liste des cases du parcours, ordonnées par I_COUT</param>
        /// <returns>true si OK, false si KO</returns>
        internal bool SearchSpace(Donnees.TAB_CASERow ligneCase, int espace, int[] tableCoutsMouvementsTerrain, int nombrePixelParCase, out Donnees.TAB_CASERow[] listeCaseEspace, out string erreur)
        {
            DateTime    timeStart;
            TimeSpan    perf;
            string      requete;
            bool bEspaceFound;

            erreur = string.Empty;
            Debug.WriteLine(string.Format("AStar.SearchSpace sur {0} espaces ", espace));
            m_tableCoutsMouvementsTerrain = tableCoutsMouvementsTerrain;
            m_nombrePixelParCase = nombrePixelParCase;
            m_minX = 0;
            m_minY = 0;
            m_maxX = Donnees.m_donnees.TAB_JEU[0].I_LARGEUR_CARTE;
            m_maxY = Donnees.m_donnees.TAB_JEU[0].I_HAUTEUR_CARTE;
            m_espace = espace;

            listeCaseEspace = null;
            bEspaceFound = false;
            while (!bEspaceFound)
            {
                //recherche des coûts à partir de la case source
                timeStart = DateTime.Now;
                InitializeSpace(ligneCase);
                if (m_espace <= 1)
                {
                    listeCaseEspace = new Donnees.TAB_CASERow[0];
                    return true;
                }
                //m_minX = Math.Max(0, ligneCase.I_X - espace);
                //m_minY = Math.Max(0, ligneCase.I_Y - espace);
                //m_maxX = Math.Min(Donnees.m_donnees.TAB_JEU[0].I_LARGEUR_CARTE, ligneCase.I_X + espace);
                //m_maxY = Math.Min(Donnees.m_donnees.TAB_JEU[0].I_HAUTEUR_CARTE, ligneCase.I_Y + espace);

                while (NextSpaceStep()) { }
                perf = DateTime.Now - timeStart;
                Debug.WriteLine(string.Format("AStar.SearchSpace en {0} minutes, {1} secondes, {2} millisecondes", 
                    perf.Minutes, perf.Seconds, perf.Milliseconds));

                //on vérifie qu'il y a assez de cases disponibles pour remplir l'encombrement
                requete = string.Format("I_COUT<{0}", CST_COUTMAX);
                listeCaseEspace = (Donnees.TAB_CASERow[])Donnees.m_donnees.TAB_CASE.Select(requete, "I_COUT");
                if (listeCaseEspace.Count() < m_espace)
                {
                    erreur = string.Format("SearchSpace ne trouve que {0} cases disponibles alors que l'unité en a besoin de {1}. On double l'espace et on continue.", listeCaseEspace.Count(), espace);
                    Debug.WriteLine(erreur);
                    // on double l'espace et on recommence
                    m_espace *= 2;
                }
                else
                {
                    bEspaceFound = true;
                }
            }
            return bEspaceFound;
        }

        private void InitializeSpace(Donnees.TAB_CASERow ligneCase)
        {
            if (ligneCase == null) throw new ArgumentNullException();
            _Closed.Clear();//on utilise pas la liste mais...
            TrackVille.Target = new Cible(null,ligneCase);//on utilise pas la liste mais il faut l'affecter car c'est testé de nombreuse fois dans TrackVille
            _Open.Clear();
            _Open.Add(new TrackVille(new Cible(null,ligneCase)));
            _NbIterations = 0;
            m_espaceTrouve = 0;
            _LeafToGoBackUp = null;//on utilise pas la variable mais...

            foreach (Donnees.TAB_CASERow ligne in Donnees.m_donnees.TAB_CASE)
            {
                ligne.I_COUT = CST_COUTMAX;
            }
            ligneCase.I_COUT = 0;
        }

        /// <summary>
        /// recherche les espaces libres suivants
        /// </summary>
        /// <returns>true s'il y a encore des cases à visiter, false s'il n'y en a plus ou si l'espace recherché est trouvé</returns>
        private bool NextSpaceStep()
        {
            //if (!Initialized) throw new InvalidOperationException("You must initialize AStar before launching the algorithm.");
            if (_Open.Count == 0) return false;//c'est fini, on a parcouru toutes les cases possibles du terrain (bref, on a pas trouvé)
            if (m_espaceTrouve > m_espace*5) return false; //c'est bon on a la zone (et un peu plus des fois que certaines cases seraient occupées)
            _NbIterations++;

            int IndexMin = _Open.IndexOfMin();
            TrackVille BestTrackVille = (TrackVille)_Open.ValueOfCout(IndexMin);//on repart toujours du moins couteux

            _Open.Remove(BestTrackVille);
            PropagateSpace(BestTrackVille);

            if (_NbIterations % 10000 == 0)
            {
                Debug.WriteLine("NextSpaceStep BestTrackVille.Cost=" + BestTrackVille.Cost.ToString() + " BestTrackVille.EndNode.ID_CASE=" + BestTrackVille.EndNode.Case.ID_CASE.ToString());
                Debug.WriteLine("_Open.Count=" + _Open.Count.ToString() + "_Open.CountCout=" + _Open.CountCout.ToString());
                Debug.WriteLine("_Closed.Count=" + _Closed.Count.ToString() + "_Closed.CountCout=" + _Closed.CountCout.ToString());
            }
            return _Open.Count > 0;
        }

        private void PropagateSpace(TrackVille TrackVilleToPropagate)
        {
            //DateTime timeStart;
            //TimeSpan perf;

            //timeStart = DateTime.Now;
            foreach (Donnees.TAB_CASERow A in Donnees.m_donnees.TAB_CASE.CasesVoisines(TrackVilleToPropagate.EndNode.Case))
            {
                if (A.I_X < m_minX || A.I_Y < m_minY || A.I_X > m_maxX || A.I_Y > m_maxY)
                {
                    continue;
                }

                double cout = Cout(TrackVilleToPropagate.EndNode.Case, A);
                if (cout != CST_COUTMAX)
                {
                    //DateTime timeStart2 = DateTime.Now;
                    ///if (A.I_COUT == CST_COUTMAX || (TrackVilleToPropagate.EndNode.I_COUT + cout < A.I_COUT))
                    if ((TrackVilleToPropagate.EndNode.Case.I_COUT + cout < A.I_COUT))//pas sur que ce soit bien A.I_COUT == CST_COUTMAX, BEA 28/11/2011
                    {
                        if (A.I_COUT == CST_COUTMAX) m_espaceTrouve++;//on vient de trouver une case de plus
                        A.I_COUT = TrackVilleToPropagate.EndNode.Case.I_COUT + Convert.ToInt32(cout);
                        Debug.WriteLineIf(A.I_COUT < 0,"AStar.PropagateSpace cout négatif !!!");

                        //if (A.I_X==100 && A.I_Y==100)
                        //{
                        //    Debug.WriteLine(string.Format("AStar.PropagateSpace 100,100 cout final= {0}, cout={1} de {2},{3} avec un cout de {4}",
                        //        A.I_COUT, cout, TrackVilleToPropagate.EndNode.I_X, TrackVilleToPropagate.EndNode.I_Y, TrackVilleToPropagate.EndNode.I_COUT));
                        //}
                        TrackVille Successor = new TrackVille(TrackVilleToPropagate, new Cible(null,A));// le cout est calculé dans le new
                        _Open.Add(Successor);
                    }
                }
                //perf = DateTime.Now - timeStart2;
                //Debug.WriteLine(string.Format("AStar.PropagateSpace interne en {0} minutes, {1} secondes, {2} millisecondes", perf.Minutes, perf.Seconds, perf.Milliseconds));
            }
            //perf = DateTime.Now - timeStart;
            //Debug.WriteLine(string.Format("AStar.PropagateSpace en {0} minutes, {1} secondes, {2} millisecondes", perf.Minutes, perf.Seconds, perf.Milliseconds));
        }

        public static double CalculCout(List<Donnees.TAB_CASERow> listeCase, int[] tableCoutsMouvementsTerrain)
        {
            double cout = 0;
            Donnees.TAB_CASERow ligneCasePred=null;
            m_tableCoutsMouvementsTerrain = tableCoutsMouvementsTerrain;
            m_nombrePixelParCase = Donnees.m_donnees.TAB_JEU[0].I_ECHELLE;
            foreach (Donnees.TAB_CASERow ligneCase in listeCase)
            {
                if (null!=ligneCasePred)
                {
                    cout += Cout(ligneCase, ligneCasePred);
                }
                ligneCasePred= ligneCase;
            }
            return cout;
        }
    }
}
