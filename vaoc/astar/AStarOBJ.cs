using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaocLib;//pour la Dal

namespace vaoc
{
    public class AstarTerrainOBJ
    {
        public int cout { get; set; }
        public bool route { get; set; }
    }

    internal class BlocOBJ : IEquatable<BlocOBJ>
    {
        public int xBloc { get; set; }
        public int yBloc { get; set; }

        public BlocOBJ(int x, int y)
        {
            xBloc = x;
            yBloc = y;
        }

        public bool Equals(BlocOBJ bloc)
        {
            return (bloc.xBloc == this.xBloc && bloc.yBloc == this.yBloc) ? true : false;
        }

    }

	/// <summary>
	/// Class to search the best path between two nodes on a graph.
	/// </summary>
    public class AStarOBJ
    {
		SortableListOBJ _Open, _Closed;
		TrackOBJ _LeafToGoBackUp;
		int _NbIterations = -1;

        public const double CST_COUTMAX = double.MaxValue;
        public const int CST_PLUSIEURSNATION = Int32.MaxValue;
		SortableListOBJ.Equality SameNodesReached = new SortableListOBJ.Equality( TrackOBJ.SameEndNode );

        //public static long SearchIdMeteo;//ID_METEO pour la recherche en cours
        //public static long SearchIdModeleMouvement;//ID_MODELE_MOUVEMENT pour la recherche en cours
        public static AstarTerrainOBJ[] m_tableCoutsMouvementsTerrain;
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
		public AStarOBJ(/*Graph G*/)
		{
			_Open = new SortableListOBJ();
			_Closed = new SortableListOBJ();
            m_tailleBloc = -1;//eviter la division par zero
		}

        /// <summary>
        /// Renvoie la liste des blocs auquel appartient un point
        /// </summary>
        /// <param name="x">abscisse du point</param>
        /// <param name="y">ordonnée du point</param>
        internal List<BlocOBJ> NuméroBlocParPosition(int x, int y)
        {
            if (m_tailleBloc <= 0)
            {
                m_tailleBloc = BD.Base.Jeu[0].I_TAILLEBLOC_PCC;
            }

            List<BlocOBJ> listeBlocs = new List<BlocOBJ>();
            int xBloc = x / m_tailleBloc;
            int yBloc = y / m_tailleBloc;

            listeBlocs.Add(new BlocOBJ(xBloc, yBloc));

            if ((0 == x % m_tailleBloc) && (x != 0))
            {
                listeBlocs.Add(new BlocOBJ(xBloc - 1, yBloc));
                //je suis sur une frontière de blocs
                if ((0 == y % m_tailleBloc) && (y != 0))
                {
                    //je suis sur une frontière de blocs sur x et y
                    listeBlocs.Add(new BlocOBJ(xBloc, yBloc - 1));
                    listeBlocs.Add(new BlocOBJ(xBloc - 1, yBloc - 1));
                }
            }
            else
            {
                if ((0 == y % m_tailleBloc) && (y != 0))
                {
                    //je suis sur une frontière de blocs
                    listeBlocs.Add(new BlocOBJ(xBloc, yBloc - 1));
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
        public bool SearchPath(LigneCASE StartNode, LigneCASE EndNode, AstarTerrainOBJ[] tableCoutsMouvementsTerrain)
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
        public bool SearchPath(LigneCASE StartNode, LigneCASE EndNode, AstarTerrainOBJ[] tableCoutsMouvementsTerrain, int xmin, int xmax, int ymin, int ymax)
		{
            DateTime timeStart;
            TimeSpan perf;

            m_tableCoutsMouvementsTerrain = tableCoutsMouvementsTerrain;
            m_nombrePixelParCase = BD.Base.Jeu[0].I_ECHELLE;
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
        public bool SearchPathHPA(LigneCASE StartNode, LigneCASE EndNode, AstarTerrainOBJ[] tableCoutsMouvementsTerrain)
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
        public bool SearchPathHPA(LigneCASE StartNode, LigneCASE EndNode, AstarTerrainOBJ[] tableCoutsMouvementsTerrain, int idNation)
        {
            DateTime timeStart;
            TimeSpan perf;

            m_tableCoutsMouvementsTerrain = tableCoutsMouvementsTerrain;
            m_nombrePixelParCase = BD.Base.Jeu[0].I_ECHELLE;
            m_tailleBloc = BD.Base.Jeu[0].I_TAILLEBLOC_PCC;
            m_minX = 0;
            m_minY = 0;
            m_maxX = BD.Base.Jeu[0].I_LARGEUR_CARTE;
            m_maxY = BD.Base.Jeu[0].I_HAUTEUR_CARTE;
            m_idNation = idNation;

            //SearchIdMeteo = idMeteo;//ID_METEO pour la recherche en cours
            //SearchIdModeleMouvement = idModeleMouvement;//ID_MODELE_MOUVEMENT pour la recherche en cours

            LigneCASE lDebug1 = BD.Base.Case.TrouveParID_CASE(StartNode.ID_CASE);
            LigneCASE lDebug2 = BD.Base.Case.TrouveParID_CASE(EndNode.ID_CASE);
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
        public void Initialize(LigneCASE StartNode, LigneCASE EndNode)
		{
			if ( StartNode==null || EndNode==null ) throw new ArgumentNullException();
			_Closed.Clear();
			_Open.Clear();
			TrackOBJ.Target = EndNode;
            m_xBlocEnd = EndNode.I_X / m_tailleBloc; ;
            m_yBlocEnd = EndNode.I_Y / m_tailleBloc; ;
            xminBlocEnd = m_xBlocEnd * m_tailleBloc;
            xmaxBlocEnd = xminBlocEnd + m_tailleBloc;
            yminBlocEnd = m_yBlocEnd * m_tailleBloc;
            ymaxBlocEnd = yminBlocEnd + m_tailleBloc;

            TrackOBJ.tailleBloc = m_tailleBloc;

            _Open.Add(new TrackOBJ(StartNode));
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
			//TrackOBJ BestTrackOBJ = (TrackOBJ)_Open[IndexMin];//on repart toujours du moins couteux
            TrackOBJ BestTrackOBJ = (TrackOBJ)_Open.ValueOfCout(IndexMin);
            if (_NbIterations % 10000 == 0)
            {
                Debug.WriteLine("BestTrackOBJ.Cost=" + BestTrackOBJ.Cost.ToString() + " BestTrackOBJ.EndNode.ID_CASE=" + BestTrackOBJ.EndNode.ID_CASE.ToString());
                Debug.WriteLine("_Open.Count=" + _Open.Count.ToString() + "_Open.CountCout=" + _Open.CountCout.ToString());
                Debug.WriteLine("_Closed.Count=" + _Closed.Count.ToString() + "_Closed.CountCout=" + _Closed.CountCout.ToString());
            }
            //_Open.RemoveAt(IndexMin);
            _Open.Remove(BestTrackOBJ);
			if ( BestTrackOBJ.Succeed )
			{
                Debug.WriteLine(string.Format("BestTrackOBJ Case {0}({1},{2}), cout={3} en _NbIterations={4}",
                    BestTrackOBJ.EndNode.ID_CASE,
                    BestTrackOBJ.EndNode.I_X,
                    BestTrackOBJ.EndNode.I_Y,
                    BestTrackOBJ.Cost,
                    _NbIterations));
                _LeafToGoBackUp = BestTrackOBJ;
				_Open.Clear();
			}
			else
			{
				Propagate(BestTrackOBJ);
				_Closed.Add(BestTrackOBJ);
			}
            //traces de Debug
            //foreach (TrackOBJ noeud in _Open)
            //{
            //    Debug.WriteLine(string.Format("Open : X:{0} Y:{1} C:{2}", noeud.EndNode.I_X, noeud.EndNode.I_Y, noeud.Cost));
            //}
            //foreach (TrackOBJ noeud in _Closed)
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
            //TrackOBJ BestTrackOBJ = (TrackOBJ)_Open[IndexMin];//on repart toujours du moins couteux
            TrackOBJ BestTrackOBJ = (TrackOBJ)_Open.ValueOfCout(IndexMin);
            /* debug 
            if (null==BestTrackOBJ.EndNodeHPA)
                Debug.WriteLine(string.Format("BestTrackOBJ Case {0}({1},{2}), cout={3}",
                    BestTrackOBJ.EndNode.ID_CASE,
                    BestTrackOBJ.EndNode.I_X,
                    BestTrackOBJ.EndNode.I_Y,
                    BestTrackOBJ.Cost));
            else
            {
                LigneCASE lDebug1 = Donnees.m_donnees.TAB_CASE.FindByID_CASE(BestTrackOBJ.EndNodeHPA.ID_CASE_DEBUT);
                LigneCASE lDebug2 = Donnees.m_donnees.TAB_CASE.FindByID_CASE(BestTrackOBJ.EndNodeHPA.ID_CASE_FIN);
                Debug.WriteLine(string.Format("BestTrackOBJ trajet {0}({1},{2}) -> {4}({5},{6}), cout={3}",
                    lDebug1.ID_CASE,
                    lDebug1.I_X,
                    lDebug1.I_Y,
                    BestTrackOBJ.Cost,
                    lDebug2.ID_CASE,
                    lDebug2.I_X,
                    lDebug2.I_Y
                    ));
            }
           */

            if (_NbIterations % 10000 == 0)
            {
                Debug.WriteLine("BestTrackOBJ.Cost=" + BestTrackOBJ.Cost.ToString());
                Debug.WriteLine("_Open.Count=" + _Open.Count.ToString() + "_Open.CountCout=" + _Open.CountCout.ToString());
                Debug.WriteLine("_Closed.Count=" + _Closed.Count.ToString() + "_Closed.CountCout=" + _Closed.CountCout.ToString());
            }
            //_Open.RemoveAt(IndexMin);
            _Open.Remove(BestTrackOBJ);
            if (BestTrackOBJ.Succeed)
            {
                Debug.WriteLine(string.Format("BestTrackOBJ cout={0} en _NbIterations={1}",
                    BestTrackOBJ.Cost,
                    _NbIterations));
                _LeafToGoBackUp = BestTrackOBJ;
                _Open.Clear();
            }
            else
            {
                PropagateHPA(BestTrackOBJ);
                _Closed.Add(BestTrackOBJ);
            }
            //traces de Debug
            //foreach (TrackOBJ noeud in _Open)
            //{
            //    Debug.WriteLine(string.Format("Open : X:{0} Y:{1} C:{2}", noeud.EndNode.I_X, noeud.EndNode.I_Y, noeud.Cost));
            //}
            //foreach (TrackOBJ noeud in _Closed)
            //{
            //    Debug.WriteLine(string.Format("Close : X:{0} Y:{1} C:{2}", noeud.EndNode.I_X, noeud.EndNode.I_Y, noeud.Cost));
            //}
            //Debug.WriteLine("");
            return _Open.Count > 0;
        }

        private void PropagateHPA(TrackOBJ TrackOBJToPropagate)
        {
            /* debug 
            if (null == TrackOBJToPropagate.EndNodeHPA)
            {
                Debug.WriteLine(string.Format("TrackOBJToPropagate Case {0}({1},{2}), cout={3}",
                    TrackOBJToPropagate.EndNode.ID_CASE,
                    TrackOBJToPropagate.EndNode.I_X,
                    TrackOBJToPropagate.EndNode.I_Y,
                    TrackOBJToPropagate.Cost));
            }
             * */
            foreach (TrackOBJ A in CasesVoisinesHPA(TrackOBJToPropagate))
            {
                if (CST_COUTMAX == Cout(TrackOBJToPropagate, A))
                {
                    continue;//case intraversable
                }
                //if ((m_minX != m_maxX) && (A.I_X < m_minX || A.I_Y < m_minY || A.I_X > m_maxX || A.I_Y > m_maxY))
                //{
                //    continue;
                //}
                TrackOBJ Successor = new TrackOBJ(TrackOBJToPropagate, A);// le cout est calculé dans le new
                int PosNF = _Closed.IndexOf(Successor, SameNodesReached);
                int PosNO = _Open.IndexOf(Successor, SameNodesReached);
                if (PosNF >= 0 && Successor.Cost >= ((TrackOBJ)_Closed[PosNF]).Cost) continue;
                if (PosNO >= 0 && Successor.Cost >= ((TrackOBJ)_Open[PosNO]).Cost) continue;
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
                    LigneCASE lDebug1 = Donnees.m_donnees.TAB_CASE.FindByID_CASE(Successor.EndNodeHPA.ID_CASE_DEBUT);
                    LigneCASE lDebug2 = Donnees.m_donnees.TAB_CASE.FindByID_CASE(Successor.EndNodeHPA.ID_CASE_FIN);
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

        private void Propagate(TrackOBJ TrackOBJToPropagate)
		{
            foreach (LigneCASE A in BD.Base.Case.CasesVoisines(TrackOBJToPropagate.EndNode))
			{
                if (CST_COUTMAX == Cout(TrackOBJToPropagate.EndNode, A))
                {
                    continue;//case intraversable
                }
                if ((m_minX!=m_maxX) && (A.I_X < m_minX || A.I_Y < m_minY || A.I_X > m_maxX || A.I_Y > m_maxY))
                {
                    continue;
                }
                TrackOBJ Successor = new TrackOBJ(TrackOBJToPropagate, A);// le cout est calculé dans le new
				int PosNF = _Closed.IndexOf(Successor, SameNodesReached);
				int PosNO = _Open.IndexOf(Successor, SameNodesReached);
				if ( PosNF>=0 && Successor.Cost>=((TrackOBJ)_Closed[PosNF]).Cost ) continue;
				if ( PosNO>=0 && Successor.Cost>=((TrackOBJ)_Open[PosNO]).Cost ) continue;
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
        public bool ResultInformation(out int NbArcsOfPath, out double CostOfPath, out int nbOutRoad)
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

        public double CoutGlobal
        {
            get
            {
                int NbArcsOfPath; 
                double CostOfPath;
                int outRoad;
                ResultInformation(out NbArcsOfPath, out CostOfPath, out outRoad);
                return CostOfPath;
            }
        }

        public double HorsRouteGlobal
        {
            get
            {
                int NbArcsOfPath;
                double CostOfPath;
                int outRoad;
                ResultInformation(out NbArcsOfPath, out CostOfPath, out outRoad);
                return outRoad;
            }
        }

        /// <summary>
		/// Gets the array of nodes representing the found path.
		/// </summary>
		/// <exception cref="InvalidOperationException">You cannot get a result unless the search has ended.</exception>
        public List<LigneCASE> PathByNodes
		{
			get 
			{
				CheckSearchHasEnded();
				if ( !PathFound ) return null;
				return ParcoursOptimise(GoBackUpNodes(_LeafToGoBackUp));
			}
		}

        private List<LigneCASE> GoBackUpNodes(TrackOBJ T)
		{
			int Nb = T.NbArcsVisited,j;
            List<LigneCASE> Path = new List<LigneCASE>();
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
                    LigneCASE lDebug1, lDebug2;
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
                                LigneCASE ligneCase = BD.Base.Case.TrouveParID_CASE(listeCases[j++]);
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
                                LigneCASE ligneCase = BD.Base.Case.TrouveParID_CASE(listeCases[j--]);
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
        public static List<LigneCASE> ParcoursOptimise(List<LigneCASE> chemin)
        {
            if (null == chemin) { return null; }
            List<LigneCASE> cheminRetour = new List<LigneCASE>();
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

        private System.Collections.Generic.IList<TrackOBJ> CasesVoisinesHPA(TrackOBJ TrackOBJSource)
        {
            System.Collections.Generic.IList<TrackOBJ> listeRetour;
            //LigneCASE lDebug1, lDebug2;
            //DateTime timeStart;
            //TimeSpan perf;
            //timeStart = DateTime.Now;

            listeRetour = new List<TrackOBJ>();
            if (null != TrackOBJSource.EndNode)
            {
                //si le noeud est sur une frontière de bloc, soit c'est un point de jonction et on renvoit les TrackOBJs de blocs,
                //soit ce n'est pas une frontière de bloc et on renvoit rien, soit ce n'est pas une frontière et on renvoit les points voisins
                if ( (m_minX==TrackOBJSource.EndNode.I_X) || (m_minY==TrackOBJSource.EndNode.I_Y) || 
                    (m_maxX==TrackOBJSource.EndNode.I_X) || (m_maxY==TrackOBJSource.EndNode.I_Y))
                {
                    //sur une bordure de carte, donc pas une frontière
                    return listeRetour;//liste vide
                }

                //sommes nous sur une bordure ?
                if ((0 == TrackOBJSource.EndNode.I_X % m_tailleBloc) || (0 == TrackOBJSource.EndNode.I_Y % m_tailleBloc))
                {
                    //sommes nous sur le bloc de destination
                    if (TrackOBJSource.EndNode.I_X >= xminBlocEnd && TrackOBJSource.EndNode.I_X <= xmaxBlocEnd && TrackOBJSource.EndNode.I_Y >= yminBlocEnd && TrackOBJSource.EndNode.I_Y <= ymaxBlocEnd)
                    {
                        IList<LigneCASE> tableCases = BD.Base.Case.CasesVoisinesInBloc(TrackOBJSource.EndNode, m_xBlocEnd, m_yBlocEnd, m_tailleBloc);
                        foreach (LigneCASE ligneCase in tableCases)
                        {
                            /* debug 
                            Debug.WriteLine(string.Format("CasesVoisinesHPA bordure ajout de la case : ID={0}({1},{2}) ",
                                ligneCase.ID_CASE, ligneCase.I_X, ligneCase.I_Y));
                            /**/
                            listeRetour.Add(new TrackOBJ(TrackOBJSource, ligneCase));
                        }
                    }
                    else
                    {
                        //nous sommes dans le bloc source
                        //sur une frontière, on recherche s'il s'agit bien d'un point connu d'une frontière, donc avec des trajets
                        List<BlocOBJ> listeBlocs = NuméroBlocParPosition(TrackOBJSource.EndNode.I_X, TrackOBJSource.EndNode.I_Y);

                        //modification des blocs
                        foreach (BlocOBJ bloc in listeBlocs)
                        {
                            string requete = string.Format("ID_CASE_FIN={0} AND I_BLOCX={1} AND I_BLOCY={2}",
                                TrackOBJSource.EndNode.ID_CASE, bloc.xBloc, bloc.yBloc);

                            Donnees.TAB_PCC_COUTSRow[] lignesCout1 = (Donnees.TAB_PCC_COUTSRow[])Donnees.m_donnees.TAB_PCC_COUTS.Select(requete);
                            IEnumerable<LignePCC_COUTS> lignesCout = BD.Base.PccCouts.ListeLiensPointBloc(TrackOBJSource.EndNode.ID_CASE, bloc.xBloc, bloc.yBloc);
                                                              

                            if (null != lignesCout && lignesCout.Count() > 0)
                            {
                                /*
                                lDebug1 = Donnees.m_donnees.TAB_CASE.FindByID_CASE(TrackOBJSource.EndNode.ID_CASE);
                                Debug.WriteLine(string.Format("case frontière : ID={0}({1},{2}) bloc :{3},{4}", lDebug1.ID_CASE, lDebug1.I_X, lDebug1.I_Y, TrackOBJSource.blocX, TrackOBJSource.blocY));
                                 */
                                //c'est une case case de frontière valide
                                List<LignePCC_COUTS> tableCout = BD.Base.PccCouts.CasesVoisines(lignesCout.ElementAt(0), true);
                                foreach (LignePCC_COUTS ligneCout in tableCout)
                                {
                                    AjouterBlocHPA(TrackOBJSource, ligneCout, ref listeRetour);
                                    /* debug 
                                    lDebug1 = Donnees.m_donnees.TAB_CASE.FindByID_CASE(ligneCout.ID_CASE_DEBUT);
                                    lDebug2 = Donnees.m_donnees.TAB_CASE.FindByID_CASE(ligneCout.ID_CASE_FIN);

                                    Debug.WriteLine(string.Format("CasesVoisinesHPA ajout de ligneCout : ID={0}({1},{2}) bloc :{3},{4} -> ID={5}({6},{7}) bloc :{8},{9} Cout={10}",
                                    lDebug1.ID_CASE, lDebug1.I_X, lDebug1.I_Y, ligneCout.I_BLOCX, ligneCout.I_BLOCY,
                                    lDebug2.ID_CASE, lDebug2.I_X, lDebug2.I_Y, ligneCout.I_BLOCX, ligneCout.I_BLOCY,
                                    listeRetour[listeRetour.Count - 1].Cost));
                                    //listeRetour.Add(new TrackOBJ(ligneCout));
                                    */
                                }
                            }
                            else
                            {
                                //possible si l'on demarre d'une case de bordure qui n'est pas un point de liaison par exemple
                                //ou que le point final est sur une bordure
                                IList<LigneCASE> tableCases = BD.Base.Case.CasesVoisinesInBloc(TrackOBJSource.EndNode, TrackOBJSource.blocX, TrackOBJSource.blocY, m_tailleBloc);
                                foreach (LigneCASE ligneCase in tableCases)
                                {
                                    /* debug 
                                    Debug.WriteLine(string.Format("CasesVoisinesHPA ajout de la case : ID={0}({1},{2}) ",
                                        ligneCase.ID_CASE, ligneCase.I_X, ligneCase.I_Y));
                                    /**/
                                    listeRetour.Add(new TrackOBJ(TrackOBJSource, ligneCase));
                                }
                            }
                        }
                    }
                }
                else
                {
                    //on est pas sur une frontière, on revient à une case voisine de points
                    IList<LigneCASE> tableCases = BD.Base.Case.CasesVoisines(TrackOBJSource.EndNode);
                    foreach (LigneCASE ligneCase in tableCases)
                    {
                        /* debug
                        Debug.WriteLine(string.Format("CasesVoisinesHPA ajout de la case : ID={0}({1},{2}) ",
                            ligneCase.ID_CASE, ligneCase.I_X, ligneCase.I_Y));
                         */
                        listeRetour.Add(new TrackOBJ(TrackOBJSource, ligneCase));
                    }
                }
            }
            //on se trouve sur un parcours de blocs ?
            if (null != TrackOBJSource.EndNodeHPA)
            {
                /* debug 
                lDebug1 = Donnees.m_donnees.TAB_CASE.FindByID_CASE(TrackOBJSource.EndNodeHPA.ID_CASE_DEBUT);
                lDebug2 = Donnees.m_donnees.TAB_CASE.FindByID_CASE(TrackOBJSource.EndNodeHPA.ID_CASE_FIN);
                Debug.WriteLine(string.Format("CasesVoisinesHPA Blocs voisins de : ID={0}({1},{2}) bloc :{3},{4} -> ID={5}({6},{7}) bloc :{8},{9}",
                lDebug1.ID_CASE, lDebug1.I_X, lDebug1.I_Y, TrackOBJSource.EndNodeHPA.I_BLOCX, TrackOBJSource.EndNodeHPA.I_BLOCY,
                lDebug2.ID_CASE, lDebug2.I_X, lDebug2.I_Y, TrackOBJSource.EndNodeHPA.I_BLOCX, TrackOBJSource.EndNodeHPA.I_BLOCY));
                /**/
                List<LignePCC_COUTS> tableCout = BD.Base.PccCouts.CasesVoisines(TrackOBJSource.EndNodeHPA, false);
                foreach (LignePCC_COUTS ligneCout in tableCout)
                {
                    AjouterBlocHPA(TrackOBJSource, ligneCout, ref listeRetour);
                }

                //on regarde si le chemin ne conduit pas dans le bloc de destination
                LigneCASE ligneCaseFin = BD.Base.Case.TrouveParID_CASE(TrackOBJSource.EndNodeHPA.ID_CASE_FIN);
                //if (ligneCout.I_BLOCX == m_xBlocEnd && ligneCout.I_BLOCY == m_yBlocEnd)
                if (ligneCaseFin.I_X >= xminBlocEnd && ligneCaseFin.I_X <= xmaxBlocEnd && ligneCaseFin.I_Y >= yminBlocEnd && ligneCaseFin.I_Y <= ymaxBlocEnd)
                {
                    listeRetour.Add(new TrackOBJ(ligneCaseFin));
                }
            }
            return listeRetour;
        }

        private void AjouterBlocHPA(TrackOBJ TrackOBJSource, LignePCC_COUTS ligneCout, ref System.Collections.Generic.IList<TrackOBJ> listeRetour)
        {
            //LigneCASE lDebug1, lDebug2;
            TrackOBJ nouveauPoint;

            nouveauPoint = new TrackOBJ(ligneCout);
            listeRetour.Add(nouveauPoint);
            return;

            //int xmin = m_xBlocEnd * m_tailleBloc;
            //int xmax = xmin + m_tailleBloc;
            //int ymin = m_yBlocEnd * m_tailleBloc;
            //int ymax = ymin + m_tailleBloc;

            //LigneCASE ligneCaseFin = Donnees.m_donnees.TAB_CASE.FindByID_CASE(ligneCout.ID_CASE_FIN);
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
            //    LigneCASE ligneCaseSource = ligneCaseFin;
            //    //LigneCASE ligneCaseSource = Donnees.m_donnees.TAB_CASE.FindByID_CASE(ligneCout.ID_CASE_DEBUT);
            //                    /* debug 
            //    Debug.WriteLine(string.Format("AjouterBlocHPA Cases voisines de : ID={0}({1},{2}) bloc :{3},{4}",
            //        ligneCaseSource.ID_CASE, ligneCaseSource.I_X, ligneCaseSource.I_Y, ligneCout.I_BLOCX, ligneCout.I_BLOCY));
            //                      */
            //    IList<LigneCASE> tableCases = Donnees.m_donnees.TAB_CASE.CasesVoisinesInBloc(ligneCaseSource, m_xBlocEnd, m_yBlocEnd, m_tailleBloc);

            //    foreach (LigneCASE ligneCase in tableCases)
            //    {
            //        //on ajoute que les cases dans le bloc et hors frontière
            //        nouveauPoint = new TrackOBJ(ligneCase);
            //        listeRetour.Add(nouveauPoint);
            //                        /* debug

            //        Debug.WriteLine(string.Format("AjouterBlocHPA:CasesVoisinesInBloc Ajout de la case : ID={0}({1},{2}) bloc :{3},{4} -> ID={5}({6},{7}) Cout={8}",
            //            ligneCaseSource.ID_CASE, ligneCaseSource.I_X, ligneCaseSource.I_Y, ligneCout.I_BLOCX, ligneCout.I_BLOCY, ligneCase.ID_CASE, ligneCase.I_X, ligneCase.I_Y, nouveauPoint.Cost));
            //                          */
            //    }
            //}
            //else
            //{
            //    nouveauPoint = new TrackOBJ(ligneCout);
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

        //private System.Collections.Generic.IList<LigneCASE> CasesVoisines(LigneCASE caseSource)
        //{
        //    System.Collections.Generic.IList<LigneCASE> listeRetour;

        //    //DateTime timeStart;
        //    //TimeSpan perf;
        //    //timeStart = DateTime.Now;

        //    listeRetour = Donnees.m_donnees.TAB_CASE.CasesVoisines(caseSource);
        //    //perf = DateTime.Now - timeStart;
        //    //Debug.WriteLine(string.Format("CasesVoisines en {0} minutes, {1} secondes, {2} millisecondes", perf.Minutes, perf.Seconds, perf.Milliseconds));
        //    return listeRetour;
        //    //return DataSetCoutDonnees.m_donnees.TAB_CASE.CasesVoisines(caseSource);
        //}

        static public double Cout(TrackOBJ TrackOBJSource, TrackOBJ TrackOBJDestination)
        {
            if (null != TrackOBJSource.EndNode && null != TrackOBJDestination.EndNode)
            {
                //case vers case
                return Cout(TrackOBJSource.EndNode, TrackOBJDestination.EndNode);
            }
            if (null != TrackOBJSource.EndNodeHPA && null != TrackOBJDestination.EndNodeHPA)
            {
                //bloc vers bloc
                Debug.WriteLineIf(TrackOBJSource.EndNodeHPA.ID_CASE_FIN != TrackOBJDestination.EndNodeHPA.ID_CASE_DEBUT,
                    "Cout : TrackOBJSource.EndNodeHPA.ID_CASE_FIN != TrackOBJDestination.EndNodeHPA.ID_CASE_DEBUT");
                if (m_idNation >= 0 && (null != TrackOBJDestination.EndNodeHPA.ID_NATION) && TrackOBJDestination.EndNodeHPA.ID_NATION!= m_idNation)
                {
                    return CST_COUTMAX;//trajet intraversable
                }
                return TrackOBJDestination.EndNodeHPA.I_COUT;
            }
            if (null != TrackOBJSource.EndNode && null != TrackOBJDestination.EndNodeHPA)
            {
                //case vers bloc
                Debug.WriteLineIf(TrackOBJSource.EndNode.ID_CASE != TrackOBJDestination.EndNodeHPA.ID_CASE_DEBUT,
                    "Cout : null != TrackOBJSource.EndNode && null != TrackOBJDestination.EndNodeHPA");
                LigneCASE caseBloc = BD.Base.Case.TrouveParID_CASE(TrackOBJDestination.EndNodeHPA.ID_CASE_DEBUT);
                //return Cout(TrackOBJSource.EndNode, caseBloc);
                if (m_idNation >= 0 && (null!=TrackOBJDestination.EndNodeHPA.ID_NATION) && TrackOBJDestination.EndNodeHPA.ID_NATION != m_idNation)
                {
                    return CST_COUTMAX;//trajet intraversable
                }
                return TrackOBJDestination.EndNodeHPA.I_COUT;
            }
            if (null != TrackOBJSource.EndNodeHPA && null != TrackOBJDestination.EndNode)
            {
                //bloc vers case
                //Debug.WriteLineIf(TrackOBJSource.EndNodeHPA.ID_CASE_FIN != TrackOBJDestination.EndNode.ID_CASE,
                //    "Cout : null != TrackOBJSource.EndNodeHPA && null != TrackOBJDestination.EndNode");
                // -> pas anormal sur des cases voisines de la bordure
                LigneCASE caseSource = BD.Base.Case.TrouveParID_CASE(TrackOBJSource.EndNodeHPA.ID_CASE_FIN);
                return Cout(caseSource, TrackOBJDestination.EndNode);
            }
            throw new NotSupportedException("Erreur : Cout pour HPA non défini");//normalement, on ne devrait jamais arriver là
        }

        //cout de déplacement entre deux cases
        static public double Cout(LigneCASE caseSource, LigneCASE caseFinale)
        {
            int retour = -1;

            if (m_idNation >= 0)
            {
                if (caseFinale.ID_NOUVEAU_PROPRIETAIRE.HasValue && caseFinale.ID_NOUVEAU_PROPRIETAIRE >= 0)
                {
                    LignePION lignePionProprietaire = BD.Base.Pion.TrouveParID_PION((int)caseFinale.ID_NOUVEAU_PROPRIETAIRE);
                    if (lignePionProprietaire.idNation != m_idNation)
                    {
                        return CST_COUTMAX;//case intraversable
                    }
                }
                if (caseFinale.ID_PROPRIETAIRE.HasValue && caseFinale.ID_PROPRIETAIRE >= 0)
                {
                    LignePION lignePionProprietaire = BD.Base.Pion.TrouveParID_PION((int)caseFinale.ID_PROPRIETAIRE);
                    if (lignePionProprietaire.idNation != m_idNation)
                    {
                        return CST_COUTMAX;//case intraversable
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
        static public int HorsRoute(LigneCASE caseSource, LigneCASE caseFinale)
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
        internal bool SearchSpace(LigneCASE ligneCase, int espace, AstarTerrainOBJ[] tableCoutsMouvementsTerrain, int nombrePixelParCase, int idNation, out IEnumerable<LigneCASE> listeCaseEspace, out string erreur)
        {
            DateTime    timeStart;
            TimeSpan    perf;
            //string      requete;
            bool        bEspaceFound;

            erreur = string.Empty;
            Debug.WriteLine(string.Format("AStarOBJ.SearchSpace sur {0} espaces ", espace));
            m_tableCoutsMouvementsTerrain = tableCoutsMouvementsTerrain;
            m_nombrePixelParCase = nombrePixelParCase;
            m_minX = 0;
            m_minY = 0;
            m_maxX = BD.Base.Jeu[0].I_LARGEUR_CARTE;
            m_maxY = BD.Base.Jeu[0].I_HAUTEUR_CARTE;
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
                    listeCaseEspace = new LigneCASE[0];
                    return true;
                }
                while (NextSpaceStep()) { }
                perf = DateTime.Now - timeStart;
                Debug.WriteLine(string.Format("AStarOBJ.SearchSpace en {0} minutes, {1} secondes, {2} millisecondes", perf.Minutes, perf.Seconds, perf.Milliseconds));

                //on vérifie qu'il y a assez de cases disponibles pour remplir l'encombrement
                //requete = string.Format("I_COUT<{0}", CST_COUTMAX);
                //listeCaseEspace = (LigneCASE[])BD.Base.Case.Select(requete, "I_COUT");
                var requete = from LigneCASE ligne in BD.Base.Case
                                  where ligne.I_COUT<CST_COUTMAX
                                  select ligne;

                listeCaseEspace = requete.OrderBy(LigneCASE => LigneCASE.I_COUT);

                if (listeCaseEspace.Count() < m_espace)
                {
                    erreur = string.Format("SearchSpace ne trouve que {0} cases disponibles alors que l'unité en a besoin de {1} sur un espace global de {2}", listeCaseEspace.Count(), espace, m_espace);
                    Debug.WriteLine(erreur);
                    // on double l'espace et on recommence BEA, en fait ça sert pas à grand chose a priori
                    //m_espace *= 2;
                }
                bEspaceFound=true;
            }
            return bEspaceFound;
        }

        private void InitializeSpace(LigneCASE ligneCase)
        {
            if (ligneCase == null) throw new ArgumentNullException();
            _Closed.Clear();//on utilise pas la liste mais...
            TrackOBJ.Target = ligneCase;//on utilise pas la liste mais il faut l'affecter car c'est testé de nombreuse fois dans TrackOBJ
            TrackOBJ.tailleBloc = m_tailleBloc;
            _Open.Clear();
            _Open.Add(new TrackOBJ(ligneCase));
            _NbIterations = 0;
            m_espaceTrouve = 0;
            _LeafToGoBackUp = null;//on utilise pas la variable mais...

            foreach (LigneCASE ligne in BD.Base.Case)
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
            //TrackOBJ BestTrackOBJ = (TrackOBJ)_Open[IndexMin];//on repart toujours du moins couteux
            TrackOBJ BestTrackOBJ = (TrackOBJ)_Open.ValueOfCout(IndexMin);

            _Open.Remove(BestTrackOBJ);
            PropagateSpace(BestTrackOBJ);
            //traces de Debug
            //foreach (TrackOBJ noeud in _Open)
            //{
            //    Debug.WriteLine(string.Format("Open : X:{0} Y:{1} C:{2}", noeud.EndNode.I_X, noeud.EndNode.I_Y, noeud.Cost));
            //}
            //foreach (TrackOBJ noeud in _Closed)
            //{
            //    Debug.WriteLine(string.Format("Close : X:{0} Y:{1} C:{2}", noeud.EndNode.I_X, noeud.EndNode.I_Y, noeud.Cost));
            //}
            //Debug.WriteLine("");
            if (_NbIterations % 10000 == 0)
            {
                Debug.WriteLine("NextSpaceStepOBJ BestTrackOBJ.Cost=" + BestTrackOBJ.Cost.ToString() + " BestTrackOBJ.EndNode.ID_CASE=" + BestTrackOBJ.EndNode.ID_CASE.ToString());
                Debug.WriteLine("_Open.Count=" + _Open.Count.ToString() + "_Open.CountCout=" + _Open.CountCout.ToString());
                Debug.WriteLine("_Closed.Count=" + _Closed.Count.ToString() + "_Closed.CountCout=" + _Closed.CountCout.ToString());
            }
            return _Open.Count > 0;
        }

        private void PropagateSpace(TrackOBJ TrackOBJToPropagate)
        {
            //DateTime timeStart;
            //TimeSpan perf;

            //timeStart = DateTime.Now;
            foreach (LigneCASE A in BD.Base.Case.CasesVoisines(TrackOBJToPropagate.EndNode))
            {
                if (A.I_X < m_minX || A.I_Y < m_minY || A.I_X > m_maxX || A.I_Y > m_maxY)
                {
                    continue;
                }

                double cout = Cout(TrackOBJToPropagate.EndNode, A);
                if (cout != CST_COUTMAX)
                {
                    //DateTime timeStart2 = DateTime.Now;
                    ///if (A.I_COUT == CST_COUTMAX || (TrackOBJToPropagate.EndNode.I_COUT + cout < A.I_COUT))
                    if ((TrackOBJToPropagate.EndNode.I_COUT + cout < A.I_COUT))//pas sur que ce soit bien A.I_COUT == CST_COUTMAX, BEA 28/11/2011
                    {
                        if (A.I_COUT == CST_COUTMAX) m_espaceTrouve++;//on vient de trouver une case de plus
                        A.I_COUT = TrackOBJToPropagate.EndNode.I_COUT + cout;
                        if (A.I_COUT < 0)
                        {
                            Debug.WriteLine("AStar.PropagateSpace cout négatif !!!");
                        }
                        //if (A.I_X==100 && A.I_Y==100)
                        //{
                        //    Debug.WriteLine(string.Format("AStar.PropagateSpace 100,100 cout final= {0}, cout={1} de {2},{3} avec un cout de {4}",
                        //        A.I_COUT, cout, TrackOBJToPropagate.EndNode.I_X, TrackOBJToPropagate.EndNode.I_Y, TrackOBJToPropagate.EndNode.I_COUT));
                        //}
                        TrackOBJ Successor = new TrackOBJ(TrackOBJToPropagate, A);// le cout est calculé dans le new
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
            foreach (LignePCC_COUTS lignePCCCout in BD.Base.PccCouts)
            {
                lignePCCCout.ID_NATION = null;//par défaut, ça n'appartient à personne
                //on charge le trajet correspondant
                if (!Dal.ChargerTrajet(lignePCCCout.ID_TRAJET, out listeCases)) {return false;}
                int i=0;
                int proprio = -1;
                while (i < listeCases.Count)
                {
                    LigneCASE ligneCase = BD.Base.Case.TrouveParID_CASE(listeCases[i]);
                    if (null == ligneCase) { return false; }

                    if (ligneCase.ID_NOUVEAU_PROPRIETAIRE.HasValue && ligneCase.ID_NOUVEAU_PROPRIETAIRE >= 0)
                    {
                        if (proprio < 0)
                        {
                            proprio = (int)ligneCase.ID_NOUVEAU_PROPRIETAIRE;
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
                    if (ligneCase.ID_PROPRIETAIRE.HasValue && ligneCase.ID_PROPRIETAIRE >= 0)
                    {
                        if (proprio < 0)
                        {
                            proprio = (int)ligneCase.ID_PROPRIETAIRE;
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
