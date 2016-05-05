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
    public class AstarTerrain
    {
        public int cout { get; set; }
        public bool route { get; set; }
    }

    public class Bloc : IEquatable<Bloc>
    {
        public int xBloc { get; set; }
        public int yBloc { get; set; }

        public Bloc(int x, int y)
        {
            xBloc = x;
            yBloc = y;
        }

        //public override bool Equals(object obj)
        //{
        //    Bloc bloc = (Bloc)obj;
        //    return (bloc.xBloc == this.xBloc && bloc.yBloc == this.yBloc) ? true : false;
        //}

        public bool Equals(Bloc bloc)
        {
            return (bloc.xBloc == this.xBloc && bloc.yBloc == this.yBloc) ? true : false;
        }

    //    public override int GetHashCode()
    //    {
    //        return base.GetHashCode();
    //    }
    }

	/// <summary>
	/// Class to search the best path between two nodes on a graph.
	/// </summary>
	public class AStar
	{
		SortableList _Open, _Closed;
		Track _LeafToGoBackUp;
		int _NbIterations = -1;

        public const int CST_COUTMAX = Int32.MaxValue;
        public const int CST_PLUSIEURSNATION = Int32.MaxValue;
		SortableList.Equality SameNodesReached = new SortableList.Equality( Track.SameEndNode );

        //public static long SearchIdMeteo;//ID_METEO pour la recherche en cours
        //public static long SearchIdModeleMouvement;//ID_MODELE_MOUVEMENT pour la recherche en cours
        public static AstarTerrain[] m_tableCoutsMouvementsTerrain;
        private static int m_nombrePixelParCase;
        private const double SQRT2 = 1.4142135623730950488016887242097;
        private int m_minX, m_maxX, m_minY, m_maxY;
        private int m_tailleBloc;
        private int m_xBlocEnd;
        private int m_yBlocEnd;
        private int xminBlocEnd;
        private int xmaxBlocEnd;
        private int yminBlocEnd;
        private int ymaxBlocEnd;
        private int m_espace;//espace recherché
        private int m_espaceTrouve;//nombre d'espace trouvé
        private static int m_idNation;//nation autorisé à se déplacer sur le parcours

        #region constantes
        public const char CST_CHEMIN='C';
        public const char CST_DEPART = 'D';
        public const char CST_DESTINATION = 'A';
        #endregion
        /// <summary>
		/// AStar Constructor.
		/// </summary>
		/// <param name="G">The graph on which AStar will perform the search.</param>
		public AStar(/*Graph G*/)
		{
			_Open = new SortableList();
			_Closed = new SortableList();
            m_tailleBloc = -1;//eviter la division par zero
		}

        /// <summary>
        /// Renvoie la liste des blocs auquel appartient un point
        /// </summary>
        /// <param name="x">abscisse du point</param>
        /// <param name="y">ordonnée du point</param>
        public List<Bloc> NuméroBlocParPosition(int x, int y)
        {
            if (m_tailleBloc <= 0)
            {
                m_tailleBloc = Donnees.m_donnees.TAB_JEU[0].I_TAILLEBLOC_PCC;
            }

            List<Bloc> listeBlocs = new List<Bloc>();
            int xBloc = x / m_tailleBloc;
            int yBloc = y / m_tailleBloc;

            listeBlocs.Add(new Bloc(xBloc, yBloc));

            if ((0 == x % m_tailleBloc) && (x != 0))
            {
                listeBlocs.Add(new Bloc(xBloc - 1, yBloc));
                //je suis sur une frontière de blocs
                if ((0 == y % m_tailleBloc) && (y != 0))
                {
                    //je suis sur une frontière de blocs sur x et y
                    listeBlocs.Add(new Bloc(xBloc, yBloc - 1));
                    listeBlocs.Add(new Bloc(xBloc - 1, yBloc - 1));
                }
            }
            else
            {
                if ((0 == y % m_tailleBloc) && (y != 0))
                {
                    //je suis sur une frontière de blocs
                    listeBlocs.Add(new Bloc(xBloc, yBloc - 1));
                }
            }
            return listeBlocs;
        }

        /// <summary>
        /// Searches for the best path to reach the specified EndNode from the specified StartNode.
        /// </summary>
        /// <exception cref="ArgumentNullException">StartNode and EndNode cannot be null.</exception>
        /// <param name="StartNode">The node from which the path must start.</param>
        /// <param name="EndNode">The node to which the path must end.</param>
        /// <param name="tableCoutsMouvementsTerrain">couts de mouvement suivant les terrains</param>
        /// <returns>'true' if succeeded / 'false' if failed.</returns>
        public bool SearchPath(Donnees.TAB_CASERow StartNode, Donnees.TAB_CASERow EndNode, AstarTerrain[] tableCoutsMouvementsTerrain)
        {
            return SearchPath(StartNode, EndNode, tableCoutsMouvementsTerrain, 0,0,0,0);
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
        public bool SearchPath(Donnees.TAB_CASERow StartNode, Donnees.TAB_CASERow EndNode, AstarTerrain[] tableCoutsMouvementsTerrain, int xmin, int xmax, int ymin, int ymax)
		{
            DateTime timeStart;
            TimeSpan perf;

            m_tableCoutsMouvementsTerrain = tableCoutsMouvementsTerrain;
            m_nombrePixelParCase = Donnees.m_donnees.TAB_JEU[0].I_ECHELLE;
            m_minX = xmin;
            m_minY = ymin;
            m_maxX = xmax;
            m_maxY = ymax;
            //SearchIdMeteo = idMeteo;//ID_METEO pour la recherche en cours
            //SearchIdModeleMouvement = idModeleMouvement;//ID_MODELE_MOUVEMENT pour la recherche en cours

            Debug.WriteLine(string.Format("Début de AStar.SearchPath de {0} à {1}", StartNode.ID_CASE, EndNode.ID_CASE));
            m_idNation = -1;
            timeStart = DateTime.Now;
            Initialize(StartNode, EndNode);
			while ( NextStep() ) {}
            perf = DateTime.Now-timeStart;
            Debug.WriteLine(string.Format("AStar.SearchPath en {0} heures, {1} minutes, {2} secondes, {3} millisecondes", perf.Hours, perf.Minutes, perf.Seconds, perf.Milliseconds));
            return PathFound;
		}

        /// <summary>
        /// Searches for the best path to reach the specified EndNode from the specified StartNode. avec une recherche hierarchique
        /// </summary>
        /// <exception cref="ArgumentNullException">StartNode and EndNode cannot be null.</exception>
        /// <param name="StartNode">The node from which the path must start.</param>
        /// <param name="EndNode">The node to which the path must end.</param>
        /// <param name="tableCoutsMouvementsTerrain">couts de mouvement suivant les terrains</param>
        /// <returns>'true' if succeeded / 'false' if failed.</returns>
        public bool SearchPathHPA(Donnees.TAB_CASERow StartNode, Donnees.TAB_CASERow EndNode, AstarTerrain[] tableCoutsMouvementsTerrain)
        {
            return SearchPathHPA(StartNode, EndNode, tableCoutsMouvementsTerrain, -1);
        }

        /// <summary>
        /// Searches for the best path to reach the specified EndNode from the specified StartNode. avec une recherche hierarchique
        /// </summary>
        /// <exception cref="ArgumentNullException">StartNode and EndNode cannot be null.</exception>
        /// <param name="StartNode">The node from which the path must start.</param>
        /// <param name="EndNode">The node to which the path must end.</param>
        /// <param name="tableCoutsMouvementsTerrain">couts de mouvement suivant les terrains</param>
        /// <param name="idNation">Nation de l'unité qui fait la recherche, celle-ci ne peut pas traverser les unités ennemies (uniquement pour les dépôts), -1 sinon.</param>
        /// <returns>'true' if succeeded / 'false' if failed.</returns>
        public bool SearchPathHPA(Donnees.TAB_CASERow StartNode, Donnees.TAB_CASERow EndNode, AstarTerrain[] tableCoutsMouvementsTerrain, int idNation)
        {
            DateTime timeStart;
            TimeSpan perf;

            m_tableCoutsMouvementsTerrain = tableCoutsMouvementsTerrain;
            m_nombrePixelParCase = Donnees.m_donnees.TAB_JEU[0].I_ECHELLE;
            m_tailleBloc = Donnees.m_donnees.TAB_JEU[0].I_TAILLEBLOC_PCC;
            m_minX = 0;
            m_minY = 0;
            m_maxX = Donnees.m_donnees.TAB_JEU[0].I_LARGEUR_CARTE;
            m_maxY = Donnees.m_donnees.TAB_JEU[0].I_HAUTEUR_CARTE;
            m_idNation = idNation;

            //SearchIdMeteo = idMeteo;//ID_METEO pour la recherche en cours
            //SearchIdModeleMouvement = idModeleMouvement;//ID_MODELE_MOUVEMENT pour la recherche en cours

            Donnees.TAB_CASERow lDebug1 = Donnees.m_donnees.TAB_CASE.FindByID_CASE(StartNode.ID_CASE);
            Donnees.TAB_CASERow lDebug2 = Donnees.m_donnees.TAB_CASE.FindByID_CASE(EndNode.ID_CASE);
            Debug.WriteLine(string.Format("Début de AStar.SearchPathHPA de {0}({1},{2}) -> {3}({4},{5})",
                lDebug1.ID_CASE,
                lDebug1.I_X,
                lDebug1.I_Y,
                lDebug2.ID_CASE,
                lDebug2.I_X,
                lDebug2.I_Y
                ));
            timeStart = DateTime.Now;
            Initialize(StartNode, EndNode);
            while (NextStepHPA()) { }
            perf = DateTime.Now - timeStart;
            Debug.WriteLine(string.Format("AStar.SearchPathHPA en {0} heures, {1} minutes, {2} secondes, {3} millisecondes", perf.Hours, perf.Minutes, perf.Seconds, perf.Milliseconds));
            return PathFound;
        }
        
        /// <summary>
		/// Use for a 'step by step' search only. This method is alternate to SearchPath.
		/// Initializes AStar before performing search steps manually with NextStep.
		/// </summary>
		/// <exception cref="ArgumentNullException">StartNode and EndNode cannot be null.</exception>
		/// <param name="StartNode">The node from which the path must start.</param>
		/// <param name="EndNode">The node to which the path must end.</param>
        public void Initialize(Donnees.TAB_CASERow StartNode, Donnees.TAB_CASERow EndNode)
		{
			if ( StartNode==null || EndNode==null ) throw new ArgumentNullException();
			_Closed.Clear();
			_Open.Clear();
			Track.Target = EndNode;
            m_xBlocEnd = EndNode.I_X / m_tailleBloc; ;
            m_yBlocEnd = EndNode.I_Y / m_tailleBloc; ;
            xminBlocEnd = m_xBlocEnd * m_tailleBloc;
            xmaxBlocEnd = xminBlocEnd + m_tailleBloc;
            yminBlocEnd = m_yBlocEnd * m_tailleBloc;
            ymaxBlocEnd = yminBlocEnd + m_tailleBloc;

            Track.tailleBloc = m_tailleBloc;

            _Open.Add(new Track(StartNode));
			_NbIterations = 0;
			_LeafToGoBackUp = null;
		}

		/// <summary>
		/// Use for a 'step by step' search only. This method is alternate to SearchPath.
		/// The algorithm must have been initialize before.
		/// </summary>
		/// <exception cref="InvalidOperationException">You must initialize AStar before using NextStep().</exception>
		/// <returns>'true' unless the search ended.</returns>
		public bool NextStep()
		{
			if ( !Initialized ) throw new InvalidOperationException("You must initialize AStar before launching the algorithm.");
			if ( _Open.Count==0 ) return false;
			_NbIterations++;
            Debug.WriteLineIf(_NbIterations % 10000 == 0, "_NbIterations=" + _NbIterations.ToString());

			int IndexMin = _Open.IndexOfMin();
			//Track BestTrack = (Track)_Open[IndexMin];//on repart toujours du moins couteux
            Track BestTrack = (Track)_Open.ValueOfCout(IndexMin);
            if (_NbIterations % 10000 == 0)
            {
                Debug.WriteLine("BestTrack.Cost=" + BestTrack.Cost.ToString() + " BestTrack.EndNode.ID_CASE=" + BestTrack.EndNode.ID_CASE.ToString());
                Debug.WriteLine("_Open.Count=" + _Open.Count.ToString() + "_Open.CountCout=" + _Open.CountCout.ToString());
                Debug.WriteLine("_Closed.Count=" + _Closed.Count.ToString() + "_Closed.CountCout=" + _Closed.CountCout.ToString());
            }
            //_Open.RemoveAt(IndexMin);
            _Open.Remove(BestTrack);
			if ( BestTrack.Succeed )
			{
                Debug.WriteLine(string.Format("BestTrack Case {0}({1},{2}), cout={3} en _NbIterations={4}",
                    BestTrack.EndNode.ID_CASE,
                    BestTrack.EndNode.I_X,
                    BestTrack.EndNode.I_Y,
                    BestTrack.Cost,
                    _NbIterations));
                _LeafToGoBackUp = BestTrack;
				_Open.Clear();
			}
			else
			{
				Propagate(BestTrack);
				_Closed.Add(BestTrack);
			}
            //traces de Debug
            //foreach (Track noeud in _Open)
            //{
            //    Debug.WriteLine(string.Format("Open : X:{0} Y:{1} C:{2}", noeud.EndNode.I_X, noeud.EndNode.I_Y, noeud.Cost));
            //}
            //foreach (Track noeud in _Closed)
            //{
            //    Debug.WriteLine(string.Format("Close : X:{0} Y:{1} C:{2}", noeud.EndNode.I_X, noeud.EndNode.I_Y, noeud.Cost));
            //}
            //Debug.WriteLine("");
            return _Open.Count > 0;
		}

        /// <summary>
        /// Use for a 'step by step' search only. This method is alternate to SearchPath.
        /// The algorithm must have been initialize before.
        /// </summary>
        /// <exception cref="InvalidOperationException">You must initialize AStar before using NextStep().</exception>
        /// <returns>'true' unless the search ended.</returns>
        public bool NextStepHPA()
        {
            if (!Initialized) throw new InvalidOperationException("You must initialize AStar before launching the algorithm.");
            if (_Open.Count == 0) return false;
            _NbIterations++;
            Debug.WriteLineIf(_NbIterations % 10000 == 0, "_NbIterations=" + _NbIterations.ToString());

            int IndexMin = _Open.IndexOfMin();
            //Track BestTrack = (Track)_Open[IndexMin];//on repart toujours du moins couteux
            Track BestTrack = (Track)_Open.ValueOfCout(IndexMin);
            /* debug 
            if (null==BestTrack.EndNodeHPA)
                Debug.WriteLine(string.Format("BestTrack Case {0}({1},{2}), cout={3}",
                    BestTrack.EndNode.ID_CASE,
                    BestTrack.EndNode.I_X,
                    BestTrack.EndNode.I_Y,
                    BestTrack.Cost));
            else
            {
                Donnees.TAB_CASERow lDebug1 = Donnees.m_donnees.TAB_CASE.FindByID_CASE(BestTrack.EndNodeHPA.ID_CASE_DEBUT);
                Donnees.TAB_CASERow lDebug2 = Donnees.m_donnees.TAB_CASE.FindByID_CASE(BestTrack.EndNodeHPA.ID_CASE_FIN);
                Debug.WriteLine(string.Format("BestTrack trajet {0}({1},{2}) -> {4}({5},{6}), cout={3}",
                    lDebug1.ID_CASE,
                    lDebug1.I_X,
                    lDebug1.I_Y,
                    BestTrack.Cost,
                    lDebug2.ID_CASE,
                    lDebug2.I_X,
                    lDebug2.I_Y
                    ));
            }
           */

            if (_NbIterations % 10000 == 0)
            {
                Debug.WriteLine("BestTrack.Cost=" + BestTrack.Cost.ToString());
                Debug.WriteLine("_Open.Count=" + _Open.Count.ToString() + "_Open.CountCout=" + _Open.CountCout.ToString());
                Debug.WriteLine("_Closed.Count=" + _Closed.Count.ToString() + "_Closed.CountCout=" + _Closed.CountCout.ToString());
            }
            //_Open.RemoveAt(IndexMin);
            _Open.Remove(BestTrack);
            if (BestTrack.Succeed)
            {
                Debug.WriteLine(string.Format("BestTrack cout={0} en _NbIterations={1}",
                    BestTrack.Cost,
                    _NbIterations));
                _LeafToGoBackUp = BestTrack;
                _Open.Clear();
            }
            else
            {
                PropagateHPA(BestTrack);
                _Closed.Add(BestTrack);
            }
            //traces de Debug
            //foreach (Track noeud in _Open)
            //{
            //    Debug.WriteLine(string.Format("Open : X:{0} Y:{1} C:{2}", noeud.EndNode.I_X, noeud.EndNode.I_Y, noeud.Cost));
            //}
            //foreach (Track noeud in _Closed)
            //{
            //    Debug.WriteLine(string.Format("Close : X:{0} Y:{1} C:{2}", noeud.EndNode.I_X, noeud.EndNode.I_Y, noeud.Cost));
            //}
            //Debug.WriteLine("");
            return _Open.Count > 0;
        }

        private void PropagateHPA(Track TrackToPropagate)
        {
            /* debug 
            if (null == TrackToPropagate.EndNodeHPA)
            {
                Debug.WriteLine(string.Format("TrackToPropagate Case {0}({1},{2}), cout={3}",
                    TrackToPropagate.EndNode.ID_CASE,
                    TrackToPropagate.EndNode.I_X,
                    TrackToPropagate.EndNode.I_Y,
                    TrackToPropagate.Cost));
            }
             * */
            foreach (Track A in CasesVoisinesHPA(TrackToPropagate))
            {
                if (CST_COUTMAX == Cout(TrackToPropagate, A))
                {
                    continue;//case intraversable
                }
                //if ((m_minX != m_maxX) && (A.I_X < m_minX || A.I_Y < m_minY || A.I_X > m_maxX || A.I_Y > m_maxY))
                //{
                //    continue;
                //}
                Track Successor = new Track(TrackToPropagate, A);// le cout est calculé dans le new
                int PosNF = _Closed.IndexOf(Successor, SameNodesReached);
                int PosNO = _Open.IndexOf(Successor, SameNodesReached);
                if (PosNF >= 0 && Successor.Cost >= ((Track)_Closed[PosNF]).Cost) continue;
                if (PosNO >= 0 && Successor.Cost >= ((Track)_Open[PosNO]).Cost) continue;
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
                if (null == Successor.EndNodeHPA)
                    Debug.WriteLine(string.Format("Successor Case {0}({1},{2}), cout={3}",
                        Successor.EndNode.ID_CASE,
                        Successor.EndNode.I_X,
                        Successor.EndNode.I_Y,
                        Successor.Cost));
                else
                {
                    Donnees.TAB_CASERow lDebug1 = Donnees.m_donnees.TAB_CASE.FindByID_CASE(Successor.EndNodeHPA.ID_CASE_DEBUT);
                    Donnees.TAB_CASERow lDebug2 = Donnees.m_donnees.TAB_CASE.FindByID_CASE(Successor.EndNodeHPA.ID_CASE_FIN);
                    Debug.WriteLine(string.Format("Successor trajet {0}({1},{2}) -> {4}({5},{6}), cout={3}",
                        lDebug1.ID_CASE,
                        lDebug1.I_X,
                        lDebug1.I_Y,
                        Successor.Cost,
                        lDebug2.ID_CASE,
                        lDebug2.I_X,
                        lDebug2.I_Y
                        ));
                }
                  */
            }
        }

        private void Propagate(Track TrackToPropagate)
		{
            foreach (Donnees.TAB_CASERow A in Donnees.m_donnees.TAB_CASE.CasesVoisines(TrackToPropagate.EndNode))
			{
                if (CST_COUTMAX == Cout(TrackToPropagate.EndNode, A))
                {
                    continue;//case intraversable
                }
                if ((m_minX!=m_maxX) && (A.I_X < m_minX || A.I_Y < m_minY || A.I_X > m_maxX || A.I_Y > m_maxY))
                {
                    continue;
                }
                Track Successor = new Track(TrackToPropagate, A);// le cout est calculé dans le new
				int PosNF = _Closed.IndexOf(Successor, SameNodesReached);
				int PosNO = _Open.IndexOf(Successor, SameNodesReached);
				if ( PosNF>=0 && Successor.Cost>=((Track)_Closed[PosNF]).Cost ) continue;
				if ( PosNO>=0 && Successor.Cost>=((Track)_Open[PosNO]).Cost ) continue;
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
        /// <summary>
        /// Returns information on the result.
        /// </summary>
        /// <param name="NbArcsOfPath">The number of arcs in the result path / -1 if no result.</param>
        /// <param name="CostOfPath">The cost of the result path / -1 if no result.</param>
        /// <param name="nbOutRoad">The cost of the result path out road / -1 if no result.</param>
        /// <returns>'true' if the search succeeded / 'false' if it failed.</returns>
        public bool ResultInformation(out int NbArcsOfPath, out int CostOfPath, out int nbOutRoad)
        {
            CheckSearchHasEnded();
            if (!PathFound)
            {
                NbArcsOfPath = -1;
                CostOfPath = -1;
                nbOutRoad = -1;
                return false;
            }
            else
            {
                NbArcsOfPath = _LeafToGoBackUp.NbArcsVisited;
                CostOfPath = _LeafToGoBackUp.Cost;
                nbOutRoad = _LeafToGoBackUp.OutRoad;
                return true;
            }
        }

        public int CoutGlobal
        {
            get
            {
                int NbArcsOfPath; 
                int CostOfPath;
                int outRoad;
                ResultInformation(out NbArcsOfPath, out CostOfPath, out outRoad);
                return CostOfPath;
            }
        }

        public int HorsRouteGlobal
        {
            get
            {
                int NbArcsOfPath;
                int CostOfPath;
                int outRoad;
                ResultInformation(out NbArcsOfPath, out CostOfPath, out outRoad);
                return outRoad;
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
				return ParcoursOptimise(GoBackUpNodes(_LeafToGoBackUp));
			}
		}

        private List<Donnees.TAB_CASERow> GoBackUpNodes(Track T)
		{
			int Nb = T.NbArcsVisited,j;
            List<Donnees.TAB_CASERow> Path = new List<Donnees.TAB_CASERow>();
            List<int> listeCases;

            for (int i = Nb; i >= 0; i--, T = T.Queue)
            {
                if (null != T.EndNode)
                {
                    Path.Insert(0, T.EndNode);
                    /*
                    Debug.WriteLine(string.Format("GoBackUpNodes ajout de T.EndNode : ID={0}({1},{2}) ",
                        T.EndNode.ID_CASE, T.EndNode.I_X, T.EndNode.I_Y
                        ));
                     * */
                }
                else
                {
                    /*
                    Donnees.TAB_CASERow lDebug1, lDebug2;
                    lDebug1 = Donnees.m_donnees.TAB_CASE.FindByID_CASE(T.EndNodeHPA.ID_CASE_DEBUT);
                    lDebug2 = Donnees.m_donnees.TAB_CASE.FindByID_CASE(T.EndNodeHPA.ID_CASE_FIN);
                    Debug.WriteLine(string.Format("GoBackUpNodes ajout de ligneCout : ID={0}({1},{2}) -> ID={3}({4},{5}) ",
                        lDebug1.ID_CASE, lDebug1.I_X, lDebug1.I_Y,
                        lDebug2.ID_CASE, lDebug2.I_X, lDebug2.I_Y
                        ));
                     * */
                    if (Dal.ChargerTrajet(T.EndNodeHPA.ID_TRAJET, out listeCases))
                    {
                        if (listeCases[0] == T.EndNodeHPA.ID_CASE_FIN)
                        {
                            j = 0;
                            while (j < listeCases.Count)
                            {
                                Donnees.TAB_CASERow ligneCase = Donnees.m_donnees.TAB_CASE.FindByID_CASE(listeCases[j++]);
                                Path.Insert(0, ligneCase);
                                /*
                                Debug.WriteLine(string.Format("GoBackUpNodes ajout de ligneCase : ID={0}({1},{2}) ",
                                    ligneCase.ID_CASE, ligneCase.I_X, ligneCase.I_Y
                                    ));
                                 * */
                            }
                        }
                        else
                        {
                            j = listeCases.Count - 1;
                            while (j >= 0)
                            {
                                Donnees.TAB_CASERow ligneCase = Donnees.m_donnees.TAB_CASE.FindByID_CASE(listeCases[j--]);
                                Path.Insert(0, ligneCase);
                                /*
                                Debug.WriteLine(string.Format("GoBackUpNodes ajout de ligneCase : ID={0}({1},{2}) ",
                                    ligneCase.ID_CASE, ligneCase.I_X, ligneCase.I_Y
                                    ));
                                 * */
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("Astar : GoBackUpNodes : erreur sur Dal.ChargerTrajet pour idtrajet=" + T.EndNodeHPA.ID_TRAJET.ToString());
                    }
                }
            }
			return Path;
		}

        /// <summary>
        /// Retire toute boucle qui pourrait se produire dans le trajet et qui empèche un pion d'arrive à sa destination
        /// En effet, on recherche sa position dans le parcours à chaque fois
        /// </summary>
        /// <param name="chemin"></param>
        /// <returns></returns>
        public static List<Donnees.TAB_CASERow> ParcoursOptimise(List<Donnees.TAB_CASERow> chemin)
        {
            if (null == chemin) { return null; }
            List<Donnees.TAB_CASERow> cheminRetour = new List<Donnees.TAB_CASERow>();
            int i = 0;
            while (i < chemin.Count)
            {
                //on regarde, si avant,on passe après par la même case, si oui, on part de ce point
                int j = i + 1;
                while (j < chemin.Count && chemin[i] != chemin[j]) j++;
                cheminRetour.Add(chemin[i]);
                if (j < chemin.Count)
                {
                    //on a trouvé une case équivalente, on repart de là
                    i = j + 1;
                }
                else
                {
                    //on ne passe pas deux fois par cette case
                    i++;
                }
            }
            return cheminRetour;
        }

        private System.Collections.Generic.IList<Track> CasesVoisinesHPA(Track trackSource)
        {
            System.Collections.Generic.IList<Track> listeRetour;
            //Donnees.TAB_CASERow lDebug1, lDebug2;
            //DateTime timeStart;
            //TimeSpan perf;
            //timeStart = DateTime.Now;

            listeRetour = new List<Track>();
            if (null != trackSource.EndNode)
            {
                //si le noeud est sur une frontière de bloc, soit c'est un point de jonction et on renvoit les Tracks de blocs,
                //soit ce n'est pas une frontière de bloc et on renvoit rien, soit ce n'est pas une frontière et on renvoit les points voisins
                if ( (m_minX==trackSource.EndNode.I_X) || (m_minY==trackSource.EndNode.I_Y) || 
                    (m_maxX==trackSource.EndNode.I_X) || (m_maxY==trackSource.EndNode.I_Y))
                {
                    //sur une bordure de carte, donc pas une frontière
                    return listeRetour;//liste vide
                }

                //sommes nous sur une bordure ?
                if ((0 == trackSource.EndNode.I_X % m_tailleBloc) || (0 == trackSource.EndNode.I_Y % m_tailleBloc))
                {
                    //sommes nous sur le bloc de destination
                    if (trackSource.EndNode.I_X >= xminBlocEnd && trackSource.EndNode.I_X <= xmaxBlocEnd && trackSource.EndNode.I_Y >= yminBlocEnd && trackSource.EndNode.I_Y <= ymaxBlocEnd)
                    {
                        IList<Donnees.TAB_CASERow> tableCases = Donnees.m_donnees.TAB_CASE.CasesVoisinesInBloc(trackSource.EndNode, m_xBlocEnd, m_yBlocEnd, m_tailleBloc);
                        foreach (Donnees.TAB_CASERow ligneCase in tableCases)
                        {
                            /* debug 
                            Debug.WriteLine(string.Format("CasesVoisinesHPA bordure ajout de la case : ID={0}({1},{2}) ",
                                ligneCase.ID_CASE, ligneCase.I_X, ligneCase.I_Y));
                            /**/
                            listeRetour.Add(new Track(trackSource, ligneCase));
                        }
                    }
                    else
                    {
                        //nous sommes dans le bloc source
                        //sur une frontière, on recherche s'il s'agit bien d'un point connu d'une frontière, donc avec des trajets
                        List<Bloc> listeBlocs = NuméroBlocParPosition(trackSource.EndNode.I_X, trackSource.EndNode.I_Y);

                        //modification des blocs
                        foreach (Bloc bloc in listeBlocs)
                        {
                            string requete = string.Format("ID_CASE_FIN={0} AND I_BLOCX={1} AND I_BLOCY={2}",
                                trackSource.EndNode.ID_CASE, bloc.xBloc, bloc.yBloc);

                            Donnees.TAB_PCC_COUTSRow[] lignesCout = (Donnees.TAB_PCC_COUTSRow[])Donnees.m_donnees.TAB_PCC_COUTS.Select(requete);
                            if (null != lignesCout && lignesCout.Length > 0)
                            {
                                /*
                                lDebug1 = Donnees.m_donnees.TAB_CASE.FindByID_CASE(trackSource.EndNode.ID_CASE);
                                Debug.WriteLine(string.Format("case frontière : ID={0}({1},{2}) bloc :{3},{4}", lDebug1.ID_CASE, lDebug1.I_X, lDebug1.I_Y, trackSource.blocX, trackSource.blocY));
                                 */
                                //c'est une case case de frontière valide
                                List<Donnees.TAB_PCC_COUTSRow> tableCout = Donnees.m_donnees.TAB_PCC_COUTS.CasesVoisines(lignesCout[0], true);
                                foreach (Donnees.TAB_PCC_COUTSRow ligneCout in tableCout)
                                {
                                    AjouterBlocHPA(trackSource, ligneCout, ref listeRetour);
                                    /* debug 
                                    lDebug1 = Donnees.m_donnees.TAB_CASE.FindByID_CASE(ligneCout.ID_CASE_DEBUT);
                                    lDebug2 = Donnees.m_donnees.TAB_CASE.FindByID_CASE(ligneCout.ID_CASE_FIN);

                                    Debug.WriteLine(string.Format("CasesVoisinesHPA ajout de ligneCout : ID={0}({1},{2}) bloc :{3},{4} -> ID={5}({6},{7}) bloc :{8},{9} Cout={10}",
                                    lDebug1.ID_CASE, lDebug1.I_X, lDebug1.I_Y, ligneCout.I_BLOCX, ligneCout.I_BLOCY,
                                    lDebug2.ID_CASE, lDebug2.I_X, lDebug2.I_Y, ligneCout.I_BLOCX, ligneCout.I_BLOCY,
                                    listeRetour[listeRetour.Count - 1].Cost));
                                    //listeRetour.Add(new Track(ligneCout));
                                    */
                                }
                            }
                            else
                            {
                                //possible si l'on demarre d'une case de bordure qui n'est pas un point de liaison par exemple
                                //ou que le point final est sur une bordure
                                IList<Donnees.TAB_CASERow> tableCases = Donnees.m_donnees.TAB_CASE.CasesVoisinesInBloc(trackSource.EndNode, trackSource.blocX, trackSource.blocY, m_tailleBloc);
                                foreach (Donnees.TAB_CASERow ligneCase in tableCases)
                                {
                                    /* debug 
                                    Debug.WriteLine(string.Format("CasesVoisinesHPA ajout de la case : ID={0}({1},{2}) ",
                                        ligneCase.ID_CASE, ligneCase.I_X, ligneCase.I_Y));
                                    /**/
                                    listeRetour.Add(new Track(trackSource, ligneCase));
                                }
                            }
                        }
                    }
                }
                else
                {
                    //on est pas sur une frontière, on revient à une case voisine de points
                    IList<Donnees.TAB_CASERow> tableCases = Donnees.m_donnees.TAB_CASE.CasesVoisines(trackSource.EndNode);
                    foreach (Donnees.TAB_CASERow ligneCase in tableCases)
                    {
                        /* debug
                        Debug.WriteLine(string.Format("CasesVoisinesHPA ajout de la case : ID={0}({1},{2}) ",
                            ligneCase.ID_CASE, ligneCase.I_X, ligneCase.I_Y));
                         */
                        listeRetour.Add(new Track(trackSource, ligneCase));
                    }
                }
            }
            //on se trouve sur un parcours de blocs ?
            if (null != trackSource.EndNodeHPA)
            {
                /* debug 
                lDebug1 = Donnees.m_donnees.TAB_CASE.FindByID_CASE(trackSource.EndNodeHPA.ID_CASE_DEBUT);
                lDebug2 = Donnees.m_donnees.TAB_CASE.FindByID_CASE(trackSource.EndNodeHPA.ID_CASE_FIN);
                Debug.WriteLine(string.Format("CasesVoisinesHPA Blocs voisins de : ID={0}({1},{2}) bloc :{3},{4} -> ID={5}({6},{7}) bloc :{8},{9}",
                lDebug1.ID_CASE, lDebug1.I_X, lDebug1.I_Y, trackSource.EndNodeHPA.I_BLOCX, trackSource.EndNodeHPA.I_BLOCY,
                lDebug2.ID_CASE, lDebug2.I_X, lDebug2.I_Y, trackSource.EndNodeHPA.I_BLOCX, trackSource.EndNodeHPA.I_BLOCY));
                /**/
                List<Donnees.TAB_PCC_COUTSRow> tableCout = Donnees.m_donnees.TAB_PCC_COUTS.CasesVoisines(trackSource.EndNodeHPA, false);
                foreach (Donnees.TAB_PCC_COUTSRow ligneCout in tableCout)
                {
                    AjouterBlocHPA(trackSource, ligneCout, ref listeRetour);
                }

                //on regarde si le chemin ne conduit pas dans le bloc de destination
                Donnees.TAB_CASERow ligneCaseFin = Donnees.m_donnees.TAB_CASE.FindByID_CASE(trackSource.EndNodeHPA.ID_CASE_FIN);
                //if (ligneCout.I_BLOCX == m_xBlocEnd && ligneCout.I_BLOCY == m_yBlocEnd)
                if (ligneCaseFin.I_X >= xminBlocEnd && ligneCaseFin.I_X <= xmaxBlocEnd && ligneCaseFin.I_Y >= yminBlocEnd && ligneCaseFin.I_Y <= ymaxBlocEnd)
                {
                    listeRetour.Add(new Track(ligneCaseFin));
                }
            }
            return listeRetour;
        }

        private void AjouterBlocHPA(Track trackSource, Donnees.TAB_PCC_COUTSRow ligneCout, ref System.Collections.Generic.IList<Track> listeRetour)
        {
            //Donnees.TAB_CASERow lDebug1, lDebug2;
            Track nouveauPoint;

            nouveauPoint = new Track(ligneCout);
            listeRetour.Add(nouveauPoint);
            return;

            //int xmin = m_xBlocEnd * m_tailleBloc;
            //int xmax = xmin + m_tailleBloc;
            //int ymin = m_yBlocEnd * m_tailleBloc;
            //int ymax = ymin + m_tailleBloc;

            //Donnees.TAB_CASERow ligneCaseFin = Donnees.m_donnees.TAB_CASE.FindByID_CASE(ligneCout.ID_CASE_FIN);
            ////if (ligneCout.I_BLOCX == m_xBlocEnd && ligneCout.I_BLOCY == m_yBlocEnd)
            //if (ligneCaseFin.I_X >= xmin && ligneCaseFin.I_X <= xmax && ligneCaseFin.I_Y >= ymin && ligneCaseFin.I_Y <= ymax)
            //{
            //    /* debug 
            //    lDebug1 = Donnees.m_donnees.TAB_CASE.FindByID_CASE(ligneCout.ID_CASE_DEBUT);
            //    lDebug2 = Donnees.m_donnees.TAB_CASE.FindByID_CASE(ligneCout.ID_CASE_FIN);
            //    Debug.WriteLine(string.Format("AjouterBlocHPA ajout ligneCout : ID={0}({1},{2}) bloc :{3},{4} -> ID={5}({6},{7}) bloc :{8},{9} cout={10}",
            //    lDebug1.ID_CASE, lDebug1.I_X, lDebug1.I_Y, ligneCout.I_BLOCX, ligneCout.I_BLOCY,
            //    lDebug2.ID_CASE, lDebug2.I_X, lDebug2.I_Y, ligneCout.I_BLOCX, ligneCout.I_BLOCY, nouveauPoint.Cost));
            //    */
            //    Donnees.TAB_CASERow ligneCaseSource = ligneCaseFin;
            //    //Donnees.TAB_CASERow ligneCaseSource = Donnees.m_donnees.TAB_CASE.FindByID_CASE(ligneCout.ID_CASE_DEBUT);
            //                    /* debug 
            //    Debug.WriteLine(string.Format("AjouterBlocHPA Cases voisines de : ID={0}({1},{2}) bloc :{3},{4}",
            //        ligneCaseSource.ID_CASE, ligneCaseSource.I_X, ligneCaseSource.I_Y, ligneCout.I_BLOCX, ligneCout.I_BLOCY));
            //                      */
            //    IList<Donnees.TAB_CASERow> tableCases = Donnees.m_donnees.TAB_CASE.CasesVoisinesInBloc(ligneCaseSource, m_xBlocEnd, m_yBlocEnd, m_tailleBloc);

            //    foreach (Donnees.TAB_CASERow ligneCase in tableCases)
            //    {
            //        //on ajoute que les cases dans le bloc et hors frontière
            //        nouveauPoint = new Track(ligneCase);
            //        listeRetour.Add(nouveauPoint);
            //                        /* debug

            //        Debug.WriteLine(string.Format("AjouterBlocHPA:CasesVoisinesInBloc Ajout de la case : ID={0}({1},{2}) bloc :{3},{4} -> ID={5}({6},{7}) Cout={8}",
            //            ligneCaseSource.ID_CASE, ligneCaseSource.I_X, ligneCaseSource.I_Y, ligneCout.I_BLOCX, ligneCout.I_BLOCY, ligneCase.ID_CASE, ligneCase.I_X, ligneCase.I_Y, nouveauPoint.Cost));
            //                          */
            //    }
            //}
            //else
            //{
            //    nouveauPoint = new Track(ligneCout);
            //                    /* debug 

            //    lDebug1 = Donnees.m_donnees.TAB_CASE.FindByID_CASE(ligneCout.ID_CASE_DEBUT);
            //    lDebug2 = Donnees.m_donnees.TAB_CASE.FindByID_CASE(ligneCout.ID_CASE_FIN);
            //    Debug.WriteLine(string.Format("AjouterBlocHPA ajout ligneCout : ID={0}({1},{2}) bloc :{3},{4} -> ID={5}({6},{7}) bloc :{8},{9} cout={10}",
            //        lDebug1.ID_CASE, lDebug1.I_X, lDebug1.I_Y, ligneCout.I_BLOCX, ligneCout.I_BLOCY,
            //        lDebug2.ID_CASE, lDebug2.I_X, lDebug2.I_Y, ligneCout.I_BLOCX, ligneCout.I_BLOCY, nouveauPoint.Cost));
            //                      */
            //    listeRetour.Add(nouveauPoint);
            //}
        }

        //private System.Collections.Generic.IList<Donnees.TAB_CASERow> CasesVoisines(Donnees.TAB_CASERow caseSource)
        //{
        //    System.Collections.Generic.IList<Donnees.TAB_CASERow> listeRetour;

        //    //DateTime timeStart;
        //    //TimeSpan perf;
        //    //timeStart = DateTime.Now;

        //    listeRetour = Donnees.m_donnees.TAB_CASE.CasesVoisines(caseSource);
        //    //perf = DateTime.Now - timeStart;
        //    //Debug.WriteLine(string.Format("CasesVoisines en {0} minutes, {1} secondes, {2} millisecondes", perf.Minutes, perf.Seconds, perf.Milliseconds));
        //    return listeRetour;
        //    //return DataSetCoutDonnees.m_donnees.TAB_CASE.CasesVoisines(caseSource);
        //}

        static public int Cout(Track trackSource, Track trackDestination)
        {
            if (null != trackSource.EndNode && null != trackDestination.EndNode)
            {
                //case vers case
                return Cout(trackSource.EndNode, trackDestination.EndNode);
            }
            if (null != trackSource.EndNodeHPA && null != trackDestination.EndNodeHPA)
            {
                //bloc vers bloc
                Debug.WriteLineIf(trackSource.EndNodeHPA.ID_CASE_FIN != trackDestination.EndNodeHPA.ID_CASE_DEBUT,
                    "Cout : trackSource.EndNodeHPA.ID_CASE_FIN != trackDestination.EndNodeHPA.ID_CASE_DEBUT");
                if (m_idNation >= 0 && !trackDestination.EndNodeHPA.IsID_NATIONNull() && trackDestination.EndNodeHPA.ID_NATION!= m_idNation)
                {
                    return AStar.CST_COUTMAX;//trajet intraversable
                }
                return trackDestination.EndNodeHPA.I_COUT;
            }
            if (null != trackSource.EndNode && null != trackDestination.EndNodeHPA)
            {
                //case vers bloc
                Debug.WriteLineIf(trackSource.EndNode.ID_CASE != trackDestination.EndNodeHPA.ID_CASE_DEBUT,
                    "Cout : null != trackSource.EndNode && null != trackDestination.EndNodeHPA");
                Donnees.TAB_CASERow caseBloc = Donnees.m_donnees.TAB_CASE.FindByID_CASE(trackDestination.EndNodeHPA.ID_CASE_DEBUT);
                //return Cout(trackSource.EndNode, caseBloc);
                if (m_idNation >= 0 && !trackDestination.EndNodeHPA.IsID_NATIONNull() && trackDestination.EndNodeHPA.ID_NATION != m_idNation)
                {
                    return AStar.CST_COUTMAX;//trajet intraversable
                }
                return trackDestination.EndNodeHPA.I_COUT;
            }
            if (null != trackSource.EndNodeHPA && null != trackDestination.EndNode)
            {
                //bloc vers case
                //Debug.WriteLineIf(trackSource.EndNodeHPA.ID_CASE_FIN != trackDestination.EndNode.ID_CASE,
                //    "Cout : null != trackSource.EndNodeHPA && null != trackDestination.EndNode");
                // -> pas anormal sur des cases voisines de la bordure
                Donnees.TAB_CASERow caseSource = Donnees.m_donnees.TAB_CASE.FindByID_CASE(trackSource.EndNodeHPA.ID_CASE_FIN);
                return Cout(caseSource, trackDestination.EndNode);
            }
            throw new NotSupportedException("Erreur : Cout pour HPA non défini");//normalement, on ne devrait jamais arriver là
        }

        //cout de déplacement entre deux cases
        static public int Cout(Donnees.TAB_CASERow caseSource, Donnees.TAB_CASERow caseFinale)
        {
            //DateTime timeStart;
            //TimeSpan perf;
            //timeStart = DateTime.Now;
            //if (caseFinale.ID_PROPRIETAIRE >= 0) -> Ne pas faire cela, sinon une unité entourée d'unités amies ne pourra plus se déplacer
            //{
            //    return Int32.MaxValue;//case déjà occupée
            //}

            int retour = -1;

            if (m_idNation >= 0)
            {
                if (!caseFinale.IsID_NOUVEAU_PROPRIETAIRENull() && caseFinale.ID_NOUVEAU_PROPRIETAIRE >= 0)
                {
                    Donnees.TAB_PIONRow lignePionProprietaire = Donnees.m_donnees.TAB_PION.FindByID_PION(caseFinale.ID_NOUVEAU_PROPRIETAIRE);
                    if (lignePionProprietaire.idNation != m_idNation)
                    {
                        return AStar.CST_COUTMAX;//case intraversable
                    }
                }
                if (!caseFinale.IsID_PROPRIETAIRENull() && caseFinale.ID_PROPRIETAIRE >= 0)
                {
                    Donnees.TAB_PIONRow lignePionProprietaire = Donnees.m_donnees.TAB_PION.FindByID_PION(caseFinale.ID_PROPRIETAIRE);
                    if (lignePionProprietaire.idNation != m_idNation)
                    {
                        return AStar.CST_COUTMAX;//case intraversable
                    }
                }
            }

            if (caseSource.I_X == caseFinale.I_X || caseSource.I_Y == caseFinale.I_Y)
            {
                //ligne droite
                retour = m_nombrePixelParCase * m_tableCoutsMouvementsTerrain[caseFinale.ID_MODELE_TERRAIN].cout;//resCout[0].I_COUT;
            }
            else
            {
                //diagonale
                retour = (int)(m_nombrePixelParCase * SQRT2 * m_tableCoutsMouvementsTerrain[caseFinale.ID_MODELE_TERRAIN].cout);
            }

            //cout de la case d'arrivée
            //string requete = string.Format("ID_MODELE_MOUVEMENT={0} AND ID_METEO={1} AND ID_MODELE_TERRAIN={2}",
            //    SearchIdModeleMouvement, SearchIdMeteo, caseFinale.ID_MODELE_TERRAIN);
            //DataSetCoutDonnees.TAB_MOUVEMENT_COUTRow[] resCout = (DataSetCoutDonnees.TAB_MOUVEMENT_COUTRow[])DataSetCoutDonnees.m_donnees.TAB_MOUVEMENT_COUT.Select(requete);
            //if (1!=resCout.Length)
            //{
            //    throw new InvalidOperationException("AStar.Cout, aucun ou plusieurs résultats de cout dans TAB_MOUVEMENT_COUT avec "+requete);
            //}
            //else
            //{
            //    retour = distance * resCout[0].I_COUT;
            //}
            //retour = distance * m_tableCoutsMouvementsTerrain[caseFinale.ID_MODELE_TERRAIN];//resCout[0].I_COUT;
            //if (caseFinale.I_X == 100 && caseFinale.I_Y == 100)
            //{
            //    Debug.WriteLine(string.Format("AStar.Cout de 100,100 à  {0},{1} = {2}", caseSource.I_X, caseSource.I_Y, retour));
            //}

            //perf = DateTime.Now - timeStart;
            //Debug.WriteLine(string.Format("AStar.Cout en {0} minutes, {1} secondes, {2} millisecondes", perf.Minutes, perf.Seconds, perf.Milliseconds));
            if (retour <= 0)
            {
                return CST_COUTMAX;//case intraversable
            }
            return retour;
        }

        /// <summary>
        /// Renvoie le cout des cases hors route
        /// </summary>
        /// <param name="caseSource">case source</param>
        /// <param name="caseFinale">case finale</param>
        /// <returns></returns>
        static public int HorsRoute(Donnees.TAB_CASERow caseSource, Donnees.TAB_CASERow caseFinale)
        {
            return (m_tableCoutsMouvementsTerrain[caseFinale.ID_MODELE_TERRAIN].route) ? 0 : (int)Cout(caseSource, caseFinale);
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
        internal bool SearchSpace(Donnees.TAB_CASERow ligneCase, int espace, AstarTerrain[] tableCoutsMouvementsTerrain, int nombrePixelParCase, int idNation, out Donnees.TAB_CASERow[] listeCaseEspace, out string erreur)
        {
            DateTime    timeStart;
            TimeSpan    perf;
            string      requete;
            bool        bEspaceFound;
            //DataSetCoutDonnees.TAB_CASERow[] retour;

            erreur = string.Empty;
            Debug.WriteLine(string.Format("AStar.SearchSpace sur {0} espaces ", espace));
            m_tableCoutsMouvementsTerrain = tableCoutsMouvementsTerrain;
            m_nombrePixelParCase = nombrePixelParCase;
            //SearchIdMeteo = idMeteo;//ID_METEO pour la recherche en cours
            //SearchIdModeleMouvement = idModeleMouvement;//ID_MODELE_MOUVEMENT pour la recherche en cours
            m_minX = 0;
            m_minY = 0;
            m_maxX = Donnees.m_donnees.TAB_JEU[0].I_LARGEUR_CARTE;
            m_maxY = Donnees.m_donnees.TAB_JEU[0].I_HAUTEUR_CARTE;
            m_espace = espace * 2;//on double l'espace recherchée, une grande partie pouvant déjà être occupée par d'autres unités.
            m_idNation = idNation;

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
                //m_minX= Math.Max(0, ligneCase.I_X-espace);
                //m_minY= Math.Max(0, ligneCase.I_Y-espace);
                //m_maxX= Math.Min(Donnees.m_donnees.TAB_JEU[0].I_LARGEUR_CARTE, ligneCase.I_X+espace);
                //m_maxY = Math.Min(Donnees.m_donnees.TAB_JEU[0].I_HAUTEUR_CARTE, ligneCase.I_Y + espace);

                while (NextSpaceStep()) { }
                perf = DateTime.Now - timeStart;
                Debug.WriteLine(string.Format("AStar.SearchSpace en {0} minutes, {1} secondes, {2} millisecondes", perf.Minutes, perf.Seconds, perf.Milliseconds));

                //on vérifie qu'il y a assez de cases disponibles pour remplir l'encombrement
                requete = string.Format("I_COUT<{0}", CST_COUTMAX);
                listeCaseEspace = (Donnees.TAB_CASERow[])Donnees.m_donnees.TAB_CASE.Select(requete, "I_COUT");
                if (listeCaseEspace.Count() < m_espace)
                {
                    erreur = string.Format("SearchSpace ne trouve que {0} cases disponibles alors que l'unité en a besoin de {1} sur un espace global de {2}", listeCaseEspace.Count(), espace, m_espace);
                    Debug.WriteLine(erreur);
                    // on double l'espace et on recommence BEA, en fait ça sert pas à grand chose a priori
                    //m_espace *= 2;
                }
                //else
                //{
                    bEspaceFound=true;
                //}
            }
            return bEspaceFound;
        }

        private void InitializeSpace(Donnees.TAB_CASERow ligneCase)
        {
            if (ligneCase == null) throw new ArgumentNullException();
            _Closed.Clear();//on utilise pas la liste mais...
            Track.Target = ligneCase;//on utilise pas la liste mais il faut l'affecter car c'est testé de nombreuse fois dans Track
            Track.tailleBloc = m_tailleBloc;
            _Open.Clear();
            _Open.Add(new Track(ligneCase));
            _NbIterations = 0;
            m_espaceTrouve = 0;
            _LeafToGoBackUp = null;//on utilise pas la variable mais...

            foreach (Donnees.TAB_CASERow ligne in Donnees.m_donnees.TAB_CASE)
            {
                ligne.I_COUT = CST_COUTMAX;
            }
            ligneCase.I_COUT = 0;
        }

        private bool NextSpaceStep()
        {
            //if (!Initialized) throw new InvalidOperationException("You must initialize AStar before launching the algorithm.");
            if (_Open.Count == 0)
            {
                return false;//c'est fini
            }
            if (m_espaceTrouve > m_espace * 3)
            {
                return false; //c'est bon on a la zone (et un peu plus des fois que certaines cases seraient occupées)
            }
            _NbIterations++;

            int IndexMin = _Open.IndexOfMin();
            //Track BestTrack = (Track)_Open[IndexMin];//on repart toujours du moins couteux
            Track BestTrack = (Track)_Open.ValueOfCout(IndexMin);

            _Open.Remove(BestTrack);
            PropagateSpace(BestTrack);
            //traces de Debug
            //foreach (Track noeud in _Open)
            //{
            //    Debug.WriteLine(string.Format("Open : X:{0} Y:{1} C:{2}", noeud.EndNode.I_X, noeud.EndNode.I_Y, noeud.Cost));
            //}
            //foreach (Track noeud in _Closed)
            //{
            //    Debug.WriteLine(string.Format("Close : X:{0} Y:{1} C:{2}", noeud.EndNode.I_X, noeud.EndNode.I_Y, noeud.Cost));
            //}
            //Debug.WriteLine("");
            if (_NbIterations % 10000 == 0)
            {
                Debug.WriteLine("NextSpaceStep BestTrack.Cost=" + BestTrack.Cost.ToString() + " BestTrack.EndNode.ID_CASE=" + BestTrack.EndNode.ID_CASE.ToString());
                Debug.WriteLine("_Open.Count=" + _Open.Count.ToString() + "_Open.CountCout=" + _Open.CountCout.ToString());
                Debug.WriteLine("_Closed.Count=" + _Closed.Count.ToString() + "_Closed.CountCout=" + _Closed.CountCout.ToString());
            }
            return _Open.Count > 0;
        }

        private void PropagateSpace(Track TrackToPropagate)
        {
            //DateTime timeStart;
            //TimeSpan perf;

            //timeStart = DateTime.Now;
            foreach (Donnees.TAB_CASERow A in Donnees.m_donnees.TAB_CASE.CasesVoisines(TrackToPropagate.EndNode))
            {
                if (A.I_X < m_minX || A.I_Y < m_minY || A.I_X > m_maxX || A.I_Y > m_maxY)
                {
                    continue;
                }

                int cout = Cout(TrackToPropagate.EndNode, A);
                if (cout != CST_COUTMAX)
                {
                    //DateTime timeStart2 = DateTime.Now;
                    ///if (A.I_COUT == CST_COUTMAX || (TrackToPropagate.EndNode.I_COUT + cout < A.I_COUT))
                    if ((TrackToPropagate.EndNode.I_COUT + cout < A.I_COUT))//pas sur que ce soit bien A.I_COUT == CST_COUTMAX, BEA 28/11/2011
                    {
                        if (A.I_COUT == AStar.CST_COUTMAX) m_espaceTrouve++;//on vient de trouver une case de plus
                        A.I_COUT = TrackToPropagate.EndNode.I_COUT + cout;
                        if (A.I_COUT < 0)
                        {
                            Debug.WriteLine("AStar.PropagateSpace cout négatif !!!");
                        }
                        //if (A.I_X==100 && A.I_Y==100)
                        //{
                        //    Debug.WriteLine(string.Format("AStar.PropagateSpace 100,100 cout final= {0}, cout={1} de {2},{3} avec un cout de {4}",
                        //        A.I_COUT, cout, TrackToPropagate.EndNode.I_X, TrackToPropagate.EndNode.I_Y, TrackToPropagate.EndNode.I_COUT));
                        //}
                        Track Successor = new Track(TrackToPropagate, A);// le cout est calculé dans le new
                        _Open.Add(Successor);
                    }
                }
                //perf = DateTime.Now - timeStart2;
                //Debug.WriteLine(string.Format("AStar.PropagateSpace interne en {0} minutes, {1} secondes, {2} millisecondes", perf.Minutes, perf.Seconds, perf.Milliseconds));
            }
            //perf = DateTime.Now - timeStart;
            //Debug.WriteLine(string.Format("AStar.PropagateSpace en {0} minutes, {1} secondes, {2} millisecondes", perf.Minutes, perf.Seconds, perf.Milliseconds));
        }

        /// <summary>
        /// met à jour les nations controllant les différents trajets
        /// ceci afin que les trajets ne puissent pas être empruntés par des nations qui ne les controllent pas (cas des ravitaillement par dépôt)
        /// </summary>
        /// <returns>true si Ok, false si KO</returns>
        static public bool InitialisationProprietaireTrajet()
        {
            List<int> listeCases;
            foreach (Donnees.TAB_PCC_COUTSRow lignePCCCout in Donnees.m_donnees.TAB_PCC_COUTS)
            {
                lignePCCCout.SetID_NATIONNull();//par défaut, ça n'appartient à personne
                //on charge le trajet correspondant
                if (!Dal.ChargerTrajet(lignePCCCout.ID_TRAJET, out listeCases)) {return false;}
                int i=0;
                int proprio = -1;
                while (i < listeCases.Count)
                {
                    Donnees.TAB_CASERow ligneCase = Donnees.m_donnees.TAB_CASE.FindByID_CASE(listeCases[i]);
                    if (null == ligneCase) { return false; }

                    if (!ligneCase.IsID_NOUVEAU_PROPRIETAIRENull() && ligneCase.ID_NOUVEAU_PROPRIETAIRE >= 0)
                    {
                        if (proprio < 0)
                        {
                            proprio = ligneCase.ID_NOUVEAU_PROPRIETAIRE;
                        }
                        else
                        {
                            if (proprio != ligneCase.ID_NOUVEAU_PROPRIETAIRE)
                            {
                                lignePCCCout.ID_NATION = CST_PLUSIEURSNATION; //plusieurs proprietaires
                                break;//cela ne sert à rien de continuer
                            }
                        }
                    }
                    if (!ligneCase.IsID_PROPRIETAIRENull() && ligneCase.ID_PROPRIETAIRE >= 0)
                    {
                        if (proprio < 0)
                        {
                            proprio = ligneCase.ID_PROPRIETAIRE;
                        }
                        else
                        {
                            if (proprio != ligneCase.ID_PROPRIETAIRE)
                            {
                                lignePCCCout.ID_NATION = CST_PLUSIEURSNATION; //plusieurs proprietaires
                                break;//cela ne sert à rien de continuer
                            }
                        }
                    }
                    i++;
                }
            }

            return true;
        }
    }
}
