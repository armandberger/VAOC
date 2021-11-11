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
using System.Threading;
using System.Windows.Forms;
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
            return (bloc.xBloc == this.xBloc && bloc.yBloc == this.yBloc);
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
		private SortableList _Open, _Closed;
		private Track _LeafToGoBackUp;
		private int _NbIterations = -1;

		private SortableList.Equality SameNodesReached = new SortableList.Equality( Track.SameEndNode );

        //public static long SearchIdMeteo;//ID_METEO pour la recherche en cours
        //public static long SearchIdModeleMouvement;//ID_MODELE_MOUVEMENT pour la recherche en cours
        public AstarTerrain[] m_tableCoutsMouvementsTerrain;
        private int m_nombrePixelParCase;
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
        private int m_idNation;//nation autorisé à se déplacer sur le parcours
        private Node m_cible = null;//private static Donnees.TAB_CASERow _Target = null; de Track

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
            //DateTime timeStart;
            //TimeSpan perf;

            m_tableCoutsMouvementsTerrain = tableCoutsMouvementsTerrain;
            m_nombrePixelParCase = Donnees.m_donnees.TAB_JEU[0].I_ECHELLE;
            m_minX = xmin;
            m_minY = ymin;
            m_maxX = xmax;
            m_maxY = ymax;
            //SearchIdMeteo = idMeteo;//ID_METEO pour la recherche en cours
            //SearchIdModeleMouvement = idModeleMouvement;//ID_MODELE_MOUVEMENT pour la recherche en cours

            //Debug.WriteLine(string.Format("Début de AStar.SearchPath de {0} à {1}", StartNode.ID_CASE, EndNode.ID_CASE));
            m_idNation = -1;
            //timeStart = DateTime.Now;
            Initialize(StartNode, EndNode);
			while ( NextStep() ) {}
            //perf = DateTime.Now-timeStart;
            //Debug.WriteLine(string.Format("AStar.SearchPath en {0} heures, {1} minutes, {2} secondes, {3} millisecondes", perf.Hours, perf.Minutes, perf.Seconds, perf.Milliseconds));
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
            //DateTime timeStart;
            //TimeSpan perf;

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

            //Donnees.TAB_CASERow lDebug1 = Donnees.m_donnees.TAB_CASE.FindParID_CASE(StartNode.ID_CASE);
            //Donnees.TAB_CASERow lDebug2 = Donnees.m_donnees.TAB_CASE.FindParID_CASE(EndNode.ID_CASE);
            //Debug.WriteLine(string.Format("Début de AStar.SearchPathHPA de {0}({1},{2}) -> {3}({4},{5})",
            //    lDebug1.ID_CASE,
            //    lDebug1.I_X,
            //    lDebug1.I_Y,
            //    lDebug2.ID_CASE,
            //    lDebug2.I_X,
            //    lDebug2.I_Y
            //    ));
            //timeStart = DateTime.Now;
            Initialize(StartNode, EndNode);
            while (NextStepHPA()) { }
            //perf = DateTime.Now - timeStart;
            //Debug.WriteLine(string.Format("AStar.SearchPathHPA en {0} heures, {1} minutes, {2} secondes, {3} millisecondes", perf.Hours, perf.Minutes, perf.Seconds, perf.Milliseconds));
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
            m_cible = new Node(EndNode);
            m_xBlocEnd = EndNode.I_X / m_tailleBloc; ;
            m_yBlocEnd = EndNode.I_Y / m_tailleBloc; ;
            xminBlocEnd = m_xBlocEnd * m_tailleBloc;
            xmaxBlocEnd = xminBlocEnd + m_tailleBloc;
            yminBlocEnd = m_yBlocEnd * m_tailleBloc;
            ymaxBlocEnd = yminBlocEnd + m_tailleBloc;

            //Track.tailleBloc = m_tailleBloc;

            _Open.Add(new Track(StartNode, m_tailleBloc));
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
            //Debug.WriteLineIf(_NbIterations % 10000 == 0, "_NbIterations=" + _NbIterations.ToString());

			int IndexMin = _Open.IndexOfMinCout();
			//Track BestTrack = (Track)_Open[IndexMin];//on repart toujours du moins couteux
            Track BestTrack = (Track)_Open.ValueOfCout(IndexMin);
            //if (_NbIterations % 10000 == 0)
            //{
            //    Debug.WriteLine("BestTrack.Cost=" + BestTrack.Cost.ToString() + " BestTrack.EndNode.ID_CASE=" + BestTrack.EndNode.ID_CASE.ToString());
            //    Debug.WriteLine("_Open.Count=" + _Open.Count.ToString() /*+ "_Open.CountCout=" + _Open.CountCout.ToString()*/);
            //    Debug.WriteLine("_Closed.Count=" + _Closed.Count.ToString() /*+ "_Closed.CountCout=" + _Closed.CountCout.ToString()*/);
            //}
            //_Open.RemoveAt(IndexMin);
            _Open.Remove(BestTrack);
			if ( BestTrack.Succeed(m_cible) )
			{
                //Debug.WriteLine(string.Format("BestTrack Case {0}({1},{2}), cout={3} en _NbIterations={4}",
                //    BestTrack.EndNode.ID_CASE,
                //    BestTrack.EndNode.I_X,
                //    BestTrack.EndNode.I_Y,
                //    BestTrack.Cost,
                //    _NbIterations));
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

            int IndexMin = _Open.IndexOfMinCout();
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
                Donnees.TAB_CASERow lDebug1 = Donnees.m_donnees.TAB_CASE.FindParID_CASE(BestTrack.EndNodeHPA.ID_CASE_DEBUT);
                Donnees.TAB_CASERow lDebug2 = Donnees.m_donnees.TAB_CASE.FindParID_CASE(BestTrack.EndNodeHPA.ID_CASE_FIN);
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
                Debug.WriteLine("_Open.Count=" + _Open.Count.ToString() /*+ "_Open.CountCout=" + _Open.CountCout.ToString()*/);
                Debug.WriteLine("_Closed.Count=" + _Closed.Count.ToString() /*+ "_Closed.CountCout=" + _Closed.CountCout.ToString()*/);
            }
            //_Open.RemoveAt(IndexMin);
            _Open.Remove(BestTrack);
            if (BestTrack.Succeed(m_cible))
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
            IList<Track> liste = CasesVoisinesHPA(TrackToPropagate); 
            foreach (Track A in liste)
            {
                if (Constantes.CST_COUTMAX == Cout(TrackToPropagate, A))
                {
                    //Debug.Write("X");
                    continue;//case intraversable
                }
                //if ((m_minX != m_maxX) && (A.I_X < m_minX || A.I_Y < m_minY || A.I_X > m_maxX || A.I_Y > m_maxY))
                //{
                //    continue;
                //}
                Track Successor = new Track(TrackToPropagate, A, this);// le cout est calculé dans le new
                int PosNF = _Closed.IndexOf(Successor, SameNodesReached);
                if (PosNF >= 0 && Successor.Cost >= ((Track)_Closed[PosNF]).Cost) continue;
                int PosNO = _Open.IndexOf(Successor, SameNodesReached);
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
                    Donnees.TAB_CASERow lDebug1 = Donnees.m_donnees.TAB_CASE.FindParID_CASE(Successor.EndNodeHPA.ID_CASE_DEBUT);
                    Donnees.TAB_CASERow lDebug2 = Donnees.m_donnees.TAB_CASE.FindParID_CASE(Successor.EndNodeHPA.ID_CASE_FIN);
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
            try
            {
                //foreach (Donnees.TAB_CASERow A in Donnees.m_donnees.TAB_CASE.CasesVoisines(TrackToPropagate.EndNode))
                foreach (Node A in Donnees.m_donnees.TAB_CASE.CasesVoisines(TrackToPropagate.EndNode))
                {
                    if (Constantes.CST_COUTMAX == Cout(TrackToPropagate.EndNode, A))
                    {
                        continue;//case intraversable
                    }
                    if ((m_minX != m_maxX) && (A.I_X < m_minX || A.I_Y < m_minY || A.I_X > m_maxX || A.I_Y > m_maxY))
                    {
                        continue;
                    }
                    Track Successor = new Track(TrackToPropagate, A, this);// le cout est calculé dans le new
                    int PosNF = _Closed.IndexOf(Successor, SameNodesReached);
                    if (PosNF >= 0 && Successor.Cost >= ((Track)_Closed[PosNF]).Cost) continue;
                    int PosNO = _Open.IndexOf(Successor, SameNodesReached);
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
                }
            }
            catch (Exception ex)
            {
                string messageEX = string.Format("exception Propagate {3} : {0} : {1} :{2}",
                       ex.Message, (null == ex.InnerException) ? "sans inner exception" : ex.InnerException.Message,
                       ex.StackTrace, ex.GetType().ToString());
                LogFile.Notifier(messageEX);
                throw ex;
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
            //si on renvoit un parcours il dit que la recherche n'est pas terminée (même pas commencée en fait)
			//if ( !SearchEnded ) throw new InvalidOperationException("You cannot get a result unless the search has ended.");
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
                int CostOfPath;
                ResultInformation(out _, out CostOfPath, out _);
                return CostOfPath;
            }
        }

        public int HorsRouteGlobal
        {
            get
            {
                int outRoad;
                ResultInformation(out _, out _, out outRoad);
                return outRoad;
            }
        }

        /*
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
                    Path.Insert(0, Donnees.m_donnees.TAB_CASE.FindParID_CASE(T.EndNode.ID_CASE));
                    //Debug.WriteLine(string.Format("GoBackUpNodes ajout de T.EndNode : ID={0}({1},{2}) ",
                    //    T.EndNode.ID_CASE, T.EndNode.I_X, T.EndNode.I_Y
                    //    ));
                }
                else
                {
                    //Donnees.TAB_CASERow lDebug1, lDebug2;
                    //lDebug1 = Donnees.m_donnees.TAB_CASE.FindParID_CASE(T.EndNodeHPA.ID_CASE_DEBUT);
                    //lDebug2 = Donnees.m_donnees.TAB_CASE.FindParID_CASE(T.EndNodeHPA.ID_CASE_FIN);
                    //Debug.WriteLine(string.Format("GoBackUpNodes ajout de ligneCout : ID={0}({1},{2}) -> ID={3}({4},{5}) ",
                    //    lDebug1.ID_CASE, lDebug1.I_X, lDebug1.I_Y,
                    //    lDebug2.ID_CASE, lDebug2.I_X, lDebug2.I_Y
                    //    ));
                    if (Dal.ChargerTrajet(T.EndNodeHPA.ID_TRAJET, out listeCases))
                    {
                        if (listeCases[0] == T.EndNodeHPA.ID_CASE_FIN)
                        {
                            j = 0;
                            while (j < listeCases.Count)
                            {
                                Donnees.TAB_CASERow ligneCase = Donnees.m_donnees.TAB_CASE.FindParID_CASE(listeCases[j++]);
                                Path.Insert(0, ligneCase);
                                //Debug.WriteLine(string.Format("GoBackUpNodes ajout de ligneCase : ID={0}({1},{2}) ",
                                //    ligneCase.ID_CASE, ligneCase.I_X, ligneCase.I_Y
                                //    ));
                            }
                        }
                        else
                        {
                            j = listeCases.Count - 1;
                            while (j >= 0)
                            {
                                Donnees.TAB_CASERow ligneCase = Donnees.m_donnees.TAB_CASE.FindParID_CASE(listeCases[j--]);
                                Path.Insert(0, ligneCase);
                                //Debug.WriteLine(string.Format("GoBackUpNodes ajout de ligneCase : ID={0}({1},{2}) ",
                                //    ligneCase.ID_CASE, ligneCase.I_X, ligneCase.I_Y
                                //    ));
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
        */
        /// <summary>
		/// Gets the array of nodes representing the found path.
		/// </summary>
		/// <exception cref="InvalidOperationException">You cannot get a result unless the search has ended.</exception>
        public List<LigneCASE> PathByNodes
        {
            get
            {
                CheckSearchHasEnded();
                if (!PathFound) return null;
                return ParcoursOptimise(GoBackUpNodes(_LeafToGoBackUp));
            }
        }

        private List<LigneCASE> GoBackUpNodes(Track T)
        {
            int Nb = T.NbArcsVisited, j;
            List<LigneCASE> Path = new List<LigneCASE>();
            List<int> listeCases;

            for (int i = Nb; i >= 0; i--, T = T.Queue)
            {
                if (null != T.EndNode)
                {
                    Path.Insert(0, new LigneCASE(Donnees.m_donnees.TAB_CASE.FindParID_CASE(T.EndNode.ID_CASE)));
                }
                else
                {
                    if (Dal.ChargerTrajet(T.EndNodeHPA.ID_TRAJET, out listeCases))
                    {
                        if (listeCases[0] == T.EndNodeHPA.ID_CASE_FIN)
                        {
                            j = 0;
                            while (j < listeCases.Count)
                            {
                                Donnees.TAB_CASERow ligneCase = Donnees.m_donnees.TAB_CASE.FindParID_CASE(listeCases[j++]);
                                Path.Insert(0, new LigneCASE(ligneCase));
                            }
                        }
                        else
                        {
                            j = listeCases.Count - 1;
                            while (j >= 0)
                            {
                                Donnees.TAB_CASERow ligneCase = Donnees.m_donnees.TAB_CASE.FindParID_CASE(listeCases[j--]);
                                Path.Insert(0, new LigneCASE(ligneCase));
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
        public List<Donnees.TAB_CASERow> ParcoursOptimise(List<Donnees.TAB_CASERow> chemin)
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

        /// <summary>
        /// Retire toute boucle qui pourrait se produire dans le trajet et qui empèche un pion d'arrive à sa destination
        /// En effet, on recherche sa position dans le parcours à chaque fois
        /// </summary>
        /// <param name="chemin"></param>
        /// <returns></returns>
        public List<LigneCASE> ParcoursOptimise(List<LigneCASE> chemin)
        {
            if (null == chemin) { return null; }
            List<LigneCASE> cheminRetour = new List<LigneCASE>();
            int i = 0;
            while (i < chemin.Count)
            {
                //on regarde, si avant,on passe après par la même case, si oui, on part de ce point
                int j = i + 1;
                while (j < chemin.Count && chemin[i].ID_CASE != chemin[j].ID_CASE) j++;
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
                        IList<Node> tableCases = Donnees.m_donnees.TAB_CASE.CasesVoisinesInBloc(trackSource.EndNode, m_xBlocEnd, m_yBlocEnd, m_tailleBloc);
                        foreach (Node ligneCase in tableCases)
                        {
                            /* debug 
                            Debug.WriteLine(string.Format("CasesVoisinesHPA bordure ajout de la case : ID={0}({1},{2}) ",
                                ligneCase.ID_CASE, ligneCase.I_X, ligneCase.I_Y));
                            /**/
                            listeRetour.Add(new Track(trackSource, ligneCase, this));
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
                            Monitor.Enter(Donnees.m_donnees.TAB_PCC_COUTS.Rows.SyncRoot);
                            Donnees.TAB_PCC_COUTSRow[] lignesCout = (Donnees.TAB_PCC_COUTSRow[])Donnees.m_donnees.TAB_PCC_COUTS.Select(requete);
                            Monitor.Exit(Donnees.m_donnees.TAB_PCC_COUTS.Rows.SyncRoot);
                            if (null != lignesCout && lignesCout.Length > 0)
                            {
                                /*
                                lDebug1 = Donnees.m_donnees.TAB_CASE.FindParID_CASE(trackSource.EndNode.ID_CASE);
                                Debug.WriteLine(string.Format("case frontière : ID={0}({1},{2}) bloc :{3},{4}", lDebug1.ID_CASE, lDebug1.I_X, lDebug1.I_Y, trackSource.blocX, trackSource.blocY));
                                 */
                                //c'est une case case de frontière valide
                                List<Donnees.TAB_PCC_COUTSRow> tableCout = Donnees.m_donnees.TAB_PCC_COUTS.CasesVoisines(lignesCout[0], true);
                                foreach (Donnees.TAB_PCC_COUTSRow ligneCout in tableCout)
                                {
                                    AjouterBlocHPA(ligneCout, ref listeRetour);
                                    /* debug 
                                    lDebug1 = Donnees.m_donnees.TAB_CASE.FindParID_CASE(ligneCout.ID_CASE_DEBUT);
                                    lDebug2 = Donnees.m_donnees.TAB_CASE.FindParID_CASE(ligneCout.ID_CASE_FIN);

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
                                IList<Node> tableCases = Donnees.m_donnees.TAB_CASE.CasesVoisinesInBloc(trackSource.EndNode, trackSource.blocX, trackSource.blocY, m_tailleBloc);
                                foreach (Node ligneCase in tableCases)
                                {
                                    /* debug 
                                    Debug.WriteLine(string.Format("CasesVoisinesHPA ajout de la case : ID={0}({1},{2}) ",
                                        ligneCase.ID_CASE, ligneCase.I_X, ligneCase.I_Y));
                                    /**/
                                    listeRetour.Add(new Track(trackSource, ligneCase, this));
                                }
                            }
                        }
                    }
                }
                else
                {
                    //on est pas sur une frontière, on revient à une case voisine de points
                    IList<Node> tableCases = Donnees.m_donnees.TAB_CASE.CasesVoisines(trackSource.EndNode);
                    foreach (Node ligneCase in tableCases)
                    {
                        /* debug
                        Debug.WriteLine(string.Format("CasesVoisinesHPA ajout de la case : ID={0}({1},{2}) ",
                            ligneCase.ID_CASE, ligneCase.I_X, ligneCase.I_Y));
                         */
                        listeRetour.Add(new Track(trackSource, ligneCase, this));
                    }
                }
            }
            //on se trouve sur un parcours de blocs ?
            if (null != trackSource.EndNodeHPA)
            {
                /* debug 
                lDebug1 = Donnees.m_donnees.TAB_CASE.FindParID_CASE(trackSource.EndNodeHPA.ID_CASE_DEBUT);
                lDebug2 = Donnees.m_donnees.TAB_CASE.FindParID_CASE(trackSource.EndNodeHPA.ID_CASE_FIN);
                Debug.WriteLine(string.Format("CasesVoisinesHPA Blocs voisins de : ID={0}({1},{2}) bloc :{3},{4} -> ID={5}({6},{7}) bloc :{8},{9}",
                lDebug1.ID_CASE, lDebug1.I_X, lDebug1.I_Y, trackSource.EndNodeHPA.I_BLOCX, trackSource.EndNodeHPA.I_BLOCY,
                lDebug2.ID_CASE, lDebug2.I_X, lDebug2.I_Y, trackSource.EndNodeHPA.I_BLOCX, trackSource.EndNodeHPA.I_BLOCY));
                /**/
                List<Donnees.TAB_PCC_COUTSRow> tableCout = Donnees.m_donnees.TAB_PCC_COUTS.CasesVoisines(trackSource.EndNodeHPA, false);
                foreach (Donnees.TAB_PCC_COUTSRow ligneCout in tableCout)
                {
                    AjouterBlocHPA(ligneCout, ref listeRetour);
                }

                //on regarde si le chemin ne conduit pas dans le bloc de destination
                Donnees.TAB_CASERow ligneCaseFin = Donnees.m_donnees.TAB_CASE.FindParID_CASE(trackSource.EndNodeHPA.ID_CASE_FIN);
                //if (ligneCout.I_BLOCX == m_xBlocEnd && ligneCout.I_BLOCY == m_yBlocEnd)
                if (ligneCaseFin.I_X >= xminBlocEnd && ligneCaseFin.I_X <= xmaxBlocEnd && ligneCaseFin.I_Y >= yminBlocEnd && ligneCaseFin.I_Y <= ymaxBlocEnd)
                {
                    listeRetour.Add(new Track(ligneCaseFin, m_tailleBloc));
                }
            }
            return listeRetour;
        }

        private void AjouterBlocHPA(Donnees.TAB_PCC_COUTSRow ligneCout, ref System.Collections.Generic.IList<Track> listeRetour)
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

            //Donnees.TAB_CASERow ligneCaseFin = Donnees.m_donnees.TAB_CASE.FindParID_CASE(ligneCout.ID_CASE_FIN);
            ////if (ligneCout.I_BLOCX == m_xBlocEnd && ligneCout.I_BLOCY == m_yBlocEnd)
            //if (ligneCaseFin.I_X >= xmin && ligneCaseFin.I_X <= xmax && ligneCaseFin.I_Y >= ymin && ligneCaseFin.I_Y <= ymax)
            //{
            //    /* debug 
            //    lDebug1 = Donnees.m_donnees.TAB_CASE.FindParID_CASE(ligneCout.ID_CASE_DEBUT);
            //    lDebug2 = Donnees.m_donnees.TAB_CASE.FindParID_CASE(ligneCout.ID_CASE_FIN);
            //    Debug.WriteLine(string.Format("AjouterBlocHPA ajout ligneCout : ID={0}({1},{2}) bloc :{3},{4} -> ID={5}({6},{7}) bloc :{8},{9} cout={10}",
            //    lDebug1.ID_CASE, lDebug1.I_X, lDebug1.I_Y, ligneCout.I_BLOCX, ligneCout.I_BLOCY,
            //    lDebug2.ID_CASE, lDebug2.I_X, lDebug2.I_Y, ligneCout.I_BLOCX, ligneCout.I_BLOCY, nouveauPoint.Cost));
            //    */
            //    Donnees.TAB_CASERow ligneCaseSource = ligneCaseFin;
            //    //Donnees.TAB_CASERow ligneCaseSource = Donnees.m_donnees.TAB_CASE.FindParID_CASE(ligneCout.ID_CASE_DEBUT);
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

            //    lDebug1 = Donnees.m_donnees.TAB_CASE.FindParID_CASE(ligneCout.ID_CASE_DEBUT);
            //    lDebug2 = Donnees.m_donnees.TAB_CASE.FindParID_CASE(ligneCout.ID_CASE_FIN);
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

        public int Cout(Track trackSource, Track trackDestination)
        {
            if (null != trackSource.EndNode && null != trackDestination.EndNode)
            {
                //case vers case
                //pas de notion de nation sur les cases, donc ce qui suit n'est pas vraiment utilisable -> Fait dans la méthode "cout"
                //if (m_idNation >= 0 && !trackDestination.EndNode.IsID_NATIONNull() && trackDestination.EndNode.ID_NATION != m_idNation)
                //{
                //    return AStar.CST_COUTMAX;//trajet intraversable
                //}
                return Cout(trackSource.EndNode, trackDestination.EndNode);
            }
            if (null != trackSource.EndNodeHPA && null != trackDestination.EndNodeHPA)
            {
                //bloc vers bloc
                //Debug.WriteLineIf(trackSource.EndNodeHPA.ID_CASE_FIN != trackDestination.EndNodeHPA.ID_CASE_DEBUT,
                //    "Cout : trackSource.EndNodeHPA.ID_CASE_FIN != trackDestination.EndNodeHPA.ID_CASE_DEBUT");
                if (m_idNation >= 0 && (Constantes.NULLENTIER != trackDestination.EndNodeHPA.ID_NATION) && (trackDestination.EndNodeHPA.ID_NATION != m_idNation))
                {
                    return Constantes.CST_COUTMAX;//trajet intraversable
                }
                return trackDestination.EndNodeHPA.I_COUT;
            }
            if (null != trackSource.EndNode && null != trackDestination.EndNodeHPA)
            {
                //case vers bloc
                //Debug.WriteLineIf(trackSource.EndNode.ID_CASE != trackDestination.EndNodeHPA.ID_CASE_DEBUT,
                //    "Cout : null != trackSource.EndNode && null != trackDestination.EndNodeHPA");
                //Donnees.TAB_CASERow caseBloc = Donnees.m_donnees.TAB_CASE.FindParID_CASE(trackDestination.EndNodeHPA.ID_CASE_DEBUT);
                //if (m_idNation >= 0 && !trackDestination.EndNodeHPA.IsID_NATIONNull() && trackDestination.EndNodeHPA.ID_NATION != m_idNation)
                //{
                //    return AStar.CST_COUTMAX;//trajet intraversable
                //}
                return trackDestination.EndNodeHPA.I_COUT;
            }
            if (null != trackSource.EndNodeHPA && null != trackDestination.EndNode)
            {
                //bloc vers case
                //Debug.WriteLineIf(trackSource.EndNodeHPA.ID_CASE_FIN != trackDestination.EndNode.ID_CASE,
                //    "Cout : null != trackSource.EndNodeHPA && null != trackDestination.EndNode");
                // -> pas anormal sur des cases voisines de la bordure

                //pas de notion de nation sur les cases, donc ce qui suit n'est pas vraiment utilisable -> Fait dans la méthode "cout"
                //if (m_idNation >= 0 && !trackDestination.EndNode.IsID_NATIONNull() && trackDestination.EndNode.ID_NATION != m_idNation)
                //{
                //    return AStar.CST_COUTMAX;//trajet intraversable
                //}
                Donnees.TAB_CASERow caseSource = Donnees.m_donnees.TAB_CASE.FindParID_CASE(trackSource.EndNodeHPA.ID_CASE_FIN);
                return Cout(new Node(caseSource), trackDestination.EndNode);
            }
            throw new NotSupportedException("Erreur : Cout pour HPA non défini");//normalement, on ne devrait jamais arriver là
        }

        //cout de déplacement entre deux cases
        public int Cout(Donnees.TAB_CASERow caseSource, Donnees.TAB_CASERow caseFinale)
        {
            //DateTime timeStart;
            //TimeSpan perf;
            //timeStart = DateTime.Now;
            //if (caseFinale.ID_PROPRIETAIRE >= 0) -> Ne pas faire cela, sinon une unité entourée d'unités amies ne pourra plus se déplacer
            //{
            //    return Int32.MaxValue;//case déjà occupée
            //}

            int retour;

            if (m_idNation >= 0)
            {
                int IdNouveauProprietaire = caseFinale.ID_NOUVEAU_PROPRIETAIRE;//pour éviter une modification dans le multithreading
                if ((Constantes.NULLENTIER!=IdNouveauProprietaire) && IdNouveauProprietaire >= 0)
                {
                    Donnees.TAB_PIONRow lignePionProprietaire = Donnees.m_donnees.TAB_PION.FindByID_PION(caseFinale.ID_NOUVEAU_PROPRIETAIRE);
                    if (lignePionProprietaire.estCombattif && lignePionProprietaire.idNation != m_idNation)
                    {
                        return Constantes.CST_COUTMAX;//case intraversable
                    }
                }

                int IdProprietaire = caseFinale.ID_PROPRIETAIRE;//pour éviter une modification dans le multithreading
                if ((Constantes.NULLENTIER != IdProprietaire) && IdProprietaire >= 0)
                {
                    Donnees.TAB_PIONRow lignePionProprietaire = Donnees.m_donnees.TAB_PION.FindByID_PION(caseFinale.ID_PROPRIETAIRE);
                    if (lignePionProprietaire.estCombattif && lignePionProprietaire.idNation != m_idNation)
                    {
                        return Constantes.CST_COUTMAX;//case intraversable
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
                return Constantes.CST_COUTMAX;//case intraversable
            }
            return retour;
        }

        public int Cout(Node caseSource, Node caseFinale)
        {
            int retour;

            try
            {
                if (m_idNation >= 0)
                {
                    if (caseFinale.ID_NOUVEAU_PROPRIETAIRE >= 0)
                    {
                        Donnees.TAB_PIONRow lignePionProprietaire = Donnees.m_donnees.TAB_PION.FindByID_PION(caseFinale.ID_NOUVEAU_PROPRIETAIRE);
                        if (null!=lignePionProprietaire && lignePionProprietaire.estCombattif && lignePionProprietaire.idNation != m_idNation)
                        {
                            return Constantes.CST_COUTMAX;//case intraversable
                        }
                    }
                    if (caseFinale.ID_PROPRIETAIRE >= 0)
                    {
                        Donnees.TAB_PIONRow lignePionProprietaire = Donnees.m_donnees.TAB_PION.FindByID_PION(caseFinale.ID_PROPRIETAIRE);
                        if (null != lignePionProprietaire && lignePionProprietaire.estCombattif && lignePionProprietaire.idNation != m_idNation)
                        {
                            return Constantes.CST_COUTMAX;//case intraversable
                        }
                    }
                }

                if (caseSource.I_X == caseFinale.I_X || caseSource.I_Y == caseFinale.I_Y)
                {
                    //ligne droite
                    retour = m_nombrePixelParCase * m_tableCoutsMouvementsTerrain[caseFinale.ID_MODELE_TERRAIN].cout;
                }
                else
                {
                    //diagonale
                    retour = (int)(m_nombrePixelParCase * SQRT2 * m_tableCoutsMouvementsTerrain[caseFinale.ID_MODELE_TERRAIN].cout);
                }

                if (retour <= 0)
                {
                    return Constantes.CST_COUTMAX;//case intraversable
                }
            }
            catch (Exception ex)
            {
                string messageEX = string.Format("exception Cout : Compare1 {3} : {0} : {1} :{2}",
                       ex.Message, (null == ex.InnerException) ? "sans inner exception" : ex.InnerException.Message,
                       ex.StackTrace, ex.GetType().ToString());
                LogFile.Notifier(messageEX);
                throw ex;
            }
            return retour;
        }

        /// <summary>
        /// Renvoie le cout des cases hors route
        /// </summary>
        /// <param name="caseSource">case source</param>
        /// <param name="caseFinale">case finale</param>
        /// <returns></returns>
        public int HorsRoute(Donnees.TAB_CASERow caseSource, Donnees.TAB_CASERow caseFinale)
        {
            return (m_tableCoutsMouvementsTerrain[caseFinale.ID_MODELE_TERRAIN].route) ? 0 : (int)Cout(caseSource, caseFinale);
        }

        public int HorsRoute(Node caseSource, Node caseFinale)
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
        internal bool SearchSpace0(Donnees.TAB_CASERow ligneCase, int espace, AstarTerrain[] tableCoutsMouvementsTerrain, int nombrePixelParCase, int idNation, out Donnees.TAB_CASERow[] listeCaseEspace, out string erreur)
        {
            //DateTime    timeStart;
            //TimeSpan    perf;
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
                //timeStart = DateTime.Now;
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
                //perf = DateTime.Now - timeStart;
                //Debug.WriteLine(string.Format("AStar.SearchSpace en {0} minutes, {1} secondes, {2} millisecondes", perf.Minutes, perf.Seconds, perf.Milliseconds));

                //on vérifie qu'il y a assez de cases disponibles pour remplir l'encombrement
                requete = string.Format("I_COUT<{0}", Constantes.CST_COUTMAX);
                Monitor.Enter(Donnees.m_donnees.TAB_CASE.Rows.SyncRoot);
                listeCaseEspace = (Donnees.TAB_CASERow[])Donnees.m_donnees.TAB_CASE.Select(requete, "I_COUT");
                Monitor.Exit(Donnees.m_donnees.TAB_CASE.Rows.SyncRoot);
                if (listeCaseEspace.Count() < m_espace)
                {
                    erreur = string.Format("SearchSpace ne trouve que {0} cases disponibles alors que l'unité en a besoin de {1} sur un espace global de {2}", listeCaseEspace.Count(), espace, m_espace);
                    Debug.WriteLine(erreur);
                    // on double l'espace et on recommence, en fait ça sert pas à grand chose a priori
                    //m_espace *= 2;
                }
                //else
                //{
                    bEspaceFound=true;
                //}
            }
            return bEspaceFound;
        }

        internal void CalculModeleMouvementsPion(out AstarTerrain[] tableCoutsMouvementsTerrain)
        {
            CalculModeleMouvementsPion(out tableCoutsMouvementsTerrain, false);
        }

        internal void CalculModeleMouvementsPion(out AstarTerrain[] tableCoutsMouvementsTerrain, bool bRoutier)
        {
            CalculModeleMouvementsPion(out tableCoutsMouvementsTerrain, bRoutier, false);
        }

        internal void CalculModeleMouvementsPion(out AstarTerrain[] tableCoutsMouvementsTerrain, bool bRoutier, bool bHorsRoute)
        {
            int maxNumeroModeleTerrain = (int)Donnees.m_donnees.TAB_MODELE_TERRAIN.Compute("Max(ID_MODELE_TERRAIN)", null);
            tableCoutsMouvementsTerrain = new AstarTerrain[maxNumeroModeleTerrain + 1];
            // par défaut on initialise les valeurs à "impassable"
            for (int i = 0; i < maxNumeroModeleTerrain + 1; i++)
            {
                tableCoutsMouvementsTerrain[i] = new AstarTerrain
                {
                    cout = Constantes.CST_COUTMAX,
                    route = false
                };
            }

            //recherche des vrais valeurs et des plus mauvaises valeurs suivant les effectifs présents
            foreach (Donnees.TAB_MODELE_TERRAINRow ligneterrain in Donnees.m_donnees.TAB_MODELE_TERRAIN)
            {
                // Toutes les cases coutent la même chose dans ce jeu, le terrain influe seulement sur la vitesse de l'unité (voir CalculVitesseMouvementPion)
                //dans le cas où l'on ne veut que les "routes", les modèles non routiers restent en impassables
                //if (!bRoutier || ligneterrain.B_CIRCUIT_ROUTIER)
                if ((!bRoutier && !bHorsRoute) ||
                    (bRoutier && ligneterrain.B_CIRCUIT_ROUTIER) ||
                    (bHorsRoute && ligneterrain.ID_MODELE_TERRAIN != 60 /*!ligneterrain.B_CIRCUIT_ROUTIER*/))
                {
                    //string requete = string.Format("ID_MODELE_MOUVEMENT={0} AND ID_METEO={1} AND ID_MODELE_TERRAIN={2}",
                    //    Donnees.m_donnees.TAB_MODELE_MOUVEMENT[0].ID_MODELE_MOUVEMENT,
                    //    Donnees.m_donnees.TAB_PARTIE[0].ID_METEO,
                    //    );
                    //Donnees.TAB_MOUVEMENT_COUTRow[] resCout = (Donnees.TAB_MOUVEMENT_COUTRow[])Donnees.m_donnees.TAB_MOUVEMENT_COUT.Select(requete);
                    int coutCase = Donnees.m_donnees.TAB_MODELE_MOUVEMENT[0].CoutCase(ligneterrain.ID_MODELE_TERRAIN);
                    //if (1 != resCout.Length)
                    //{
                    //    tableCoutsMouvementsTerrain[ligneterrain.ID_MODELE_TERRAIN].cout = AStar.CST_COUTMAX;//normalement ne devrait pas arriver
                    //    tableCoutsMouvementsTerrain[ligneterrain.ID_MODELE_TERRAIN].route = false;//normalement ne devrait pas arriver
                    //}
                    //else
                    {
                        tableCoutsMouvementsTerrain[ligneterrain.ID_MODELE_TERRAIN].cout = coutCase; // resCout[0].I_COUT;
                        tableCoutsMouvementsTerrain[ligneterrain.ID_MODELE_TERRAIN].route = ligneterrain.B_CIRCUIT_ROUTIER;
                    }
                }
            }
        }

        public bool RechercheChemin(Constantes.TYPEPARCOURS tipePacours, Donnees.TAB_PIONRow lignePion, LigneCASE ligneCaseDepart, LigneCASE ligneCaseDestination, Donnees.TAB_ORDRERow ligneOrdre, out List<LigneCASE> chemin, out double coutGlobal, out double coutHorsRoute, out AstarTerrain[] tableCoutsMouvementsTerrain, out string erreur)
        {
            return RechercheChemin(tipePacours, lignePion,
                Donnees.m_donnees.TAB_CASE.FindParID_CASE(ligneCaseDepart.ID_CASE),
                Donnees.m_donnees.TAB_CASE.FindParID_CASE(ligneCaseDestination.ID_CASE),
                out chemin, out coutGlobal, out coutHorsRoute, out tableCoutsMouvementsTerrain, out erreur);
        }

        public bool RechercheChemin(Constantes.TYPEPARCOURS tipePacours, Donnees.TAB_PIONRow lignePion, LigneCASE ligneCaseDepart, Donnees.TAB_CASERow ligneCaseDestination, Donnees.TAB_ORDRERow ligneOrdre, out List<LigneCASE> chemin, out double coutGlobal, out double coutHorsRoute, out AstarTerrain[] tableCoutsMouvementsTerrain, out string erreur)
        {
            return RechercheChemin(tipePacours, lignePion,
                Donnees.m_donnees.TAB_CASE.FindParID_CASE(ligneCaseDepart.ID_CASE),
                ligneCaseDestination,
                out chemin, out coutGlobal, out coutHorsRoute, out tableCoutsMouvementsTerrain, out erreur);
        }
        /// <summary>
        /// Recherche d'un trajet pour une unité sur la carte
        /// </summary>
        /// <param name="tipePacours">type de parcours typeParcours.MOUVEMENT ou typeParcours.RAVITAILLEMENT</param>
        /// <param name="lignePion">Pion effectuant le trajet</param>
        /// <param name="ligneCaseDepart">case de départ</param>
        /// <param name="ligneCaseDestination">case de destination</param>
        /// <param name="chemin">liste de cases formant le chemin trouvé</param>
        /// <param name="coutGlobal">cout global du chemin</param>
        /// <param name="coutHorsRoute">part du cout effecuté en dehors d'une route</param>
        /// <param name="tableCoutsMouvementsTerrain">table du cout de mouvement des cases suivant l'unité et la météo</param>
        /// <param name="erreur">message d'erreur</param>
        /// <returns>true si ok, false si ko</returns>
        public bool RechercheChemin(Constantes.TYPEPARCOURS tipePacours, Donnees.TAB_PIONRow lignePion, Donnees.TAB_CASERow ligneCaseDepart, Donnees.TAB_CASERow ligneCaseDestination, out List<LigneCASE> chemin, out double coutGlobal, out double coutHorsRoute, out AstarTerrain[] tableCoutsMouvementsTerrain, out string erreur)
        {
            string requete, message, tri;
            int i;
            DateTime timeStart;
            TimeSpan perf;
            int idNation = -1;//nation du pion, à indiquer dans la recherche du chemin si on ne peut pas traverser les troupes ennemies (ex: ravitaillement) -> ne fonctionne, pas de nation sur les cases
            Donnees.TAB_PARCOURSRow[] parcoursExistant = null;

            timeStart = DateTime.Now;
            chemin = null;
            tableCoutsMouvementsTerrain = null;
            erreur = string.Empty;
            coutGlobal = coutHorsRoute = 0;

            if (null == lignePion || null == ligneCaseDepart || null == ligneCaseDestination)
            {
                erreur = string.Format("RechercheChemin : lignePion ou ligneCaseDepart ou ligneCaseDestination null");
                LogFile.Notifier(erreur);
                return false;
            }

            //calcul des couts, à renvoyer pour connaitre le cout pour avancer d'une case supplémentaire
            //il faut faire ce calcul très tot, car cette table peut être utilisée par l'appelant même si l'on ne renvoit effectivement pas de trajet
            CalculModeleMouvementsPion(out tableCoutsMouvementsTerrain);

            //existe-il déjà un chemin pour le pion sur le trajet demandé ?
            //On ne stocke pas cette information pourles liaisons entre depôt car les parcours changent à chaque test suite aux déplacements des troupes et des ennemis
            if (tipePacours != Constantes.TYPEPARCOURS.RAVITAILLEMENT)
            {
                requete = string.Format("ID_PION={0}", lignePion.ID_PION);
                tri = "I_ORDRE";
                Monitor.Enter(Donnees.m_donnees.TAB_PARCOURS.Rows.SyncRoot);
                parcoursExistant = (Donnees.TAB_PARCOURSRow[])Donnees.m_donnees.TAB_PARCOURS.Select(requete, tri);

                if ((null != parcoursExistant) && (0 < parcoursExistant.Length))
                {
                    //if (lignePion.effectifTotal > 0)
                    //{
                    if ((ligneCaseDepart.ID_CASE == parcoursExistant[0].ID_CASE)
                        && (ligneCaseDestination.ID_CASE == parcoursExistant[parcoursExistant.Length - 1].ID_CASE))
                    {
                        //on renvoie le chemin existant
                        chemin = new List<LigneCASE>(parcoursExistant.Length);
                        Monitor.Enter(Donnees.m_donnees.TAB_CASE.Rows.SyncRoot);//BEA, normalement inutile mais ça crash avec un id_case non accessible
                        for (i = 0; i < parcoursExistant.Length; i++)
                        {
                            chemin.Add(new LigneCASE(Donnees.m_donnees.TAB_CASE.FindParID_CASE(parcoursExistant[i].ID_CASE)));
                        }
                        Monitor.Exit(Donnees.m_donnees.TAB_CASE.Rows.SyncRoot);//BEA, normalement inutile mais ça crash avec un id_case non accessible
                        perf = DateTime.Now - timeStart;
                        Monitor.Exit(Donnees.m_donnees.TAB_PARCOURS.Rows.SyncRoot);
                        message = string.Format("{0},ID={1}, RechercheChemin : existant en {2} minutes, {3} secondes, {4} millisecondes, lg={5}", 
                            lignePion.S_NOM, lignePion.ID_PION, perf.Minutes, perf.Seconds, perf.Milliseconds, parcoursExistant.Length);
                        LogFile.Notifier(message);
                        return true;
                    }
                    //sinon, ce n'est pas le même chemin, il faut donc le recalculer
                    message = string.Format("{0},ID={1}, RechercheChemin unité avec effectif: différent parcours existant allant de {2} à {3}, chemin demandé de {4} à {5}",
                        lignePion.S_NOM, lignePion.ID_PION, parcoursExistant[0].ID_CASE, parcoursExistant[parcoursExistant.Length - 1].ID_CASE,
                        ligneCaseDepart.ID_CASE, ligneCaseDestination.ID_CASE);
                    LogFile.Notifier(message);

                    //}
                    /* 14/05/2015, cela ne sert surement plus à rien les unités sans effectif suivent la même règle que les autres maintenant
                    else
                    {
                        if (ligneCaseDestination.ID_CASE == parcoursExistant[parcoursExistant.Length - 1].ID_CASE)
                        {
                            //cherche où l'unité se trouve dans le chemin existant
                            i = 0;
                            //while (i < parcoursExistant.Length && parcoursExistant[i].ID_CASE != ligneCaseDestination.ID_CASE) i++;
                            while (i < parcoursExistant.Length && parcoursExistant[i].ID_CASE != ligneCaseDepart.ID_CASE) i++;
                            if (i < parcoursExistant.Length)
                            {
                                chemin = new List<Donnees.TAB_CASERow>(parcoursExistant.Length - i);
                                for (int j = 0; j < parcoursExistant.Length - i; j++)
                                {
                                    chemin.Add(Donnees.m_donnees.TAB_CASE.FindParID_CASE(parcoursExistant[i + j].ID_CASE));
                                }
                                perf = DateTime.Now - timeStart;
                                message = string.Format("RechercheChemin : existant en {0} minutes, {1} secondes, {2} millisecondes", perf.Minutes, perf.Seconds, perf.Milliseconds);
                                LogFile.Notifier(message, out messageErreur);
                                return true;
                            }
                            //sinon, ce n'est pas le même chemin, il faut donc le recalculer
                            message = string.Format("RechercheChemin unité sans effectif: différent parcours existant vers {0}, chemin demandé vers {1}",
                                parcoursExistant[parcoursExistant.Length - 1].ID_CASE,
                                ligneCaseDestination.ID_CASE);
                            LogFile.Notifier(message, out messageErreur);
                        }
                    }
                     * */
                    //destruction de tout autre parcours précédent 14/05/2015: cela semble totalement idiot de faire ça, detruire les parcours mémorisé !!!
                    /*
                    foreach (Donnees.TAB_PARCOURSRow ligneParcours in parcoursExistant)
                    {
                        ligneParcours.Delete();
                    }
                    Donnees.m_donnees.TAB_PARCOURS.AcceptChanges();
                     * */
                }
                Monitor.Exit(Donnees.m_donnees.TAB_PARCOURS.Rows.SyncRoot);
            }

            if (tipePacours == Constantes.TYPEPARCOURS.RAVITAILLEMENT)
            {
                //Dans le cas d'un ravitaitellement, les troupes ne peuvent pas traverser les lignes ennemies
                idNation = lignePion.idNation;
            }

            //calcul du nouveau parcours
            if (ligneCaseDepart.ID_CASE != lignePion.ID_CASE)
            {
                lignePion.ID_CASE = ligneCaseDepart.ID_CASE;//cas qui devrait être réglé avant l'appel à la fonction normalement
                //message = string.Format("{0},ID={1}, RechercheChemin : Astar: ligneCaseDepart.ID_CASE != lignePion.ID_CASE de {2} ({3},{4}) à {5} ({6},{7})",
                //    lignePion.S_NOM, lignePion.ID_PION, ligneCaseDepart.ID_CASE, ligneCaseDepart.I_X, ligneCaseDepart.I_Y, ligneCaseDestination.ID_CASE, ligneCaseDestination.I_X, ligneCaseDestination.I_Y);
                //LogFile.Notifier(message, out messageErreur);
                //throw new Exception("Astar: ligneCaseDepart.ID_CASE != lignePion.ID_CASE");
            }
            message = string.Format("{0},ID={1}, RechercheChemin : SearchPath de {2} ({3},{4}) à {5} ({6},{7})",
                lignePion.S_NOM, lignePion.ID_PION, ligneCaseDepart.ID_CASE, ligneCaseDepart.I_X, ligneCaseDepart.I_Y, ligneCaseDestination.ID_CASE, ligneCaseDestination.I_X, ligneCaseDestination.I_Y);
            LogFile.Notifier(message);
            this.SearchPathHPA(ligneCaseDepart, ligneCaseDestination, tableCoutsMouvementsTerrain, idNation);
            if (!this.PathFound)
            {
                //en mode ravitaillement, il peut être normal de ne pas trouver de chemin vers un dépôt
                if (tipePacours != Constantes.TYPEPARCOURS.RAVITAILLEMENT)
                {
                    erreur = string.Format("{0},ID={1}, erreur sur SearchPath dans RechercheChemin idDepart={2}, idDestination={3})",
                        lignePion.S_NOM, lignePion.ID_PION, ligneCaseDepart.ID_CASE, ligneCaseDestination.ID_CASE);
                    LogFile.Notifier(erreur);
                    return false;
                }
                LogFile.Notifier(string.Format("{0},ID={1}, RechercheChemin : aucun chemin trouvé", lignePion.S_NOM, lignePion.ID_PION));
            }
            else
            {
                //destruction de tout autre parcours précédent
                if (null != parcoursExistant)
                {
                    Monitor.Enter(Donnees.m_donnees.TAB_PARCOURS.Rows.SyncRoot);
                    foreach (Donnees.TAB_PARCOURSRow ligneParcours in parcoursExistant)
                    {
                        ligneParcours.Delete();
                    }
                    Monitor.Exit(Donnees.m_donnees.TAB_PARCOURS.Rows.SyncRoot);
                    //AcceptChanges only updates your rows in the (in memory) dataset, that is - marks them as "not needed for actual database update".
                    //Donnees.m_donnees.TAB_PARCOURS.AcceptChanges();
                }

                chemin = this.PathByNodes;
                message = string.Format("RechercheChemin : SearchPath longueur={0}", chemin.Count);
                LogFile.Notifier(message);

                //chemin = ParcoursOptimise(chemin); -> déplacé directement dans AStar
                coutGlobal = this.CoutGlobal;
                coutHorsRoute = this.HorsRouteGlobal;

                //stockage du chemin en table, sauf pour les recherches de depôt
                int casePrecedente = -1;
                if (tipePacours != Constantes.TYPEPARCOURS.RAVITAILLEMENT)
                {
                    Monitor.Enter(Donnees.m_donnees.TAB_PARCOURS.Rows.SyncRoot);
                    i = 0;
                    foreach (LigneCASE ligneCase in chemin)
                    {
                        if (casePrecedente != ligneCase.ID_CASE)
                        {
                            //pour éviter d'avoir deux fois la même case de suite dans le parcours, possible dans certains cas rares
                            Donnees.m_donnees.TAB_PARCOURS.AddTAB_PARCOURSRow(lignePion.ID_PION, i++, ligneCase.ID_CASE);
                        }
                        casePrecedente = ligneCase.ID_CASE;
                    }
                    //AcceptChanges only updates your rows in the (in memory) dataset, that is - marks them as "not needed for actual database update".
                    //Donnees.m_donnees.TAB_PARCOURS.AcceptChanges();
                    Monitor.Exit(Donnees.m_donnees.TAB_PARCOURS.Rows.SyncRoot);
                }
            }

            perf = DateTime.Now - timeStart;
            message = string.Format("RechercheChemin : nouveau et stockage en {0} minutes, {1} secondes, {2} millisecondes", perf.Minutes, perf.Seconds, perf.Milliseconds);
            LogFile.Notifier(message);
            return true;
        }

        /// <summary>
        /// Recherche toutes les cases avec un cout de mouvement minimum autour d'un point donné. Les cases déjà occupées ne sont pas considérées
        /// lors du calcul de coût mais uniquement lors de la recherche de cases disponibles.
        /// </summary>
        /// <param name="ligneCase">Case d'origine</param>
        /// <param name="espace">Nombre de cases à trouver</param>
        /// <param name="tableCoutsMouvementsTerrain">couts de mouvement suivant les terrains</param>
        /// <param name="nombrePixelParCase>echelle en pixels par case</param>
        /// <param name="idNation"></param>
        /// <param name="listeCaseEspace>liste des cases du parcours, ordonnées par I_COUT</param>
        /// <returns>true si OK, false si KO</returns>
        internal bool SearchSpace(Donnees.TAB_CASERow ligneCase, int espace, AstarTerrain[] tableCoutsMouvementsTerrain, int nombrePixelParCase, int idNation, out List<int> listeCaseEspace, out string erreur)
        {
            DateTime timeStart;
            TimeSpan perf;
            //string requete;
            bool bEspaceFound;
            //DataSetCoutDonnees.TAB_CASERow[] retour;
            listeCaseEspace = new List<int>();

            try
            {
                erreur = string.Empty;
                //Debug.WriteLine(string.Format("AStar.SearchSpace sur {0} espaces ", espace));
                m_tableCoutsMouvementsTerrain = tableCoutsMouvementsTerrain;
                m_nombrePixelParCase = nombrePixelParCase;
                //SearchIdMeteo = idMeteo;//ID_METEO pour la recherche en cours
                //SearchIdModeleMouvement = idModeleMouvement;//ID_MODELE_MOUVEMENT pour la recherche en cours
                m_minX = 0;
                m_minY = 0;
                m_maxX = Donnees.m_donnees.TAB_JEU[0].I_LARGEUR_CARTE;
                m_maxY = Donnees.m_donnees.TAB_JEU[0].I_HAUTEUR_CARTE;
                m_espace = espace * 3;//on triple l'espace recherché, une grande partie pouvant déjà être occupée par d'autres unités.
                m_idNation = idNation;

                bEspaceFound = false;
                while (!bEspaceFound)
                {
                    //recherche des coûts à partir de la case source
                    timeStart = DateTime.Now;
                    InitializeSpace(ligneCase);
                    if (m_espace <= 1)
                    {
                        return true;
                    }
                    //m_minX= Math.Max(0, ligneCase.I_X-espace);
                    //m_minY= Math.Max(0, ligneCase.I_Y-espace);
                    //m_maxX= Math.Min(Donnees.m_donnees.TAB_JEU[0].I_LARGEUR_CARTE, ligneCase.I_X+espace);
                    //m_maxY = Math.Min(Donnees.m_donnees.TAB_JEU[0].I_HAUTEUR_CARTE, ligneCase.I_Y + espace);

                    while (NextSpaceStep()) { }
                    perf = DateTime.Now - timeStart;
                    //Debug.WriteLine(string.Format("AStar.SearchSpace en {0} minutes, {1} secondes, {2} millisecondes", perf.Minutes, perf.Seconds, perf.Milliseconds));

                    int i = 0;
                    while (i < _Closed.Count)
                    {
                        Track caseEspace = (Track)_Closed.ValueOfCout(i++);
                        //test qui suit inutile, de toute façon les CST_COUTMAX ne sont pas ajoutées dans le Closed
                        //if (caseEspace.Cost == AStar.CST_COUTMAX)
                        //{ 
                        //    break; //on ne renvoie pas dans la liste les cases intraversables 
                        //}
                        listeCaseEspace.Add(caseEspace.EndNode.ID_CASE);
                    }
                    //on vérifie qu'il y a assez de cases disponibles pour remplir l'encombrement
                    /*
                    requete = string.Format("I_COUT<{0}", CST_COUTMAX);
                    listeCaseEspace = (Donnees.TAB_CASERow[])Donnees.m_donnees.TAB_CASE.Select(requete, "I_COUT");
                    if (listeCaseEspace.Count() < m_espace)
                    {
                        erreur = string.Format("SearchSpace ne trouve que {0} cases disponibles alors que l'unité en a besoin de {1} sur un espace global de {2}", listeCaseEspace.Count(), espace, m_espace);
                        Debug.WriteLine(erreur);
                        // on double l'espace et on recommence, en fait ça sert pas à grand chose a priori
                        //m_espace *= 2;
                    }
                     * */
                    //else
                    //{
                    bEspaceFound = true;
                    //}
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("AStar.SearchSpace {3} : {0} : {1} :{2}",
                       ex.Message, (null == ex.InnerException) ? "sans inner exception" : ex.InnerException.Message,
                       ex.StackTrace, ex.GetType().ToString());
                LogFile.Notifier(message);
                throw ex;
            }
            return bEspaceFound;
        }

        private void InitializeSpace(Donnees.TAB_CASERow ligneCase)
        {
            if (ligneCase == null) throw new ArgumentNullException();
            _Closed.Clear();//on utilise pas la liste mais...
            m_cible = new Node(ligneCase);//on utilise pas la liste mais il faut l'affecter car c'est testé de nombreuse fois dans Track
            //Track.tailleBloc = m_tailleBloc;
            _Open.Clear();
            _Open.Add(new Track(ligneCase, m_tailleBloc));
            _NbIterations = 0;
            m_espaceTrouve = 0;
            _LeafToGoBackUp = null;//on utilise pas la variable mais...

            //foreach (Donnees.TAB_CASERow ligne in Donnees.m_donnees.TAB_CASE)
            //{
            //    ligne.I_COUT = CST_COUTMAX;
            //}
            //ligneCase.I_COUT = 0;
        }

        private bool NextSpaceStep()
        {
            //if (!Initialized) throw new InvalidOperationException("You must initialize AStar before launching the algorithm.");
            if (_Open.Count == 0)
            {
                return false;//c'est fini
            }
            //if (m_espaceTrouve > m_espace * 3)
            if (_Closed.Count > m_espace)
            {
                return false; //c'est bon on a la zone (et un peu plus des fois que certaines cases seraient occupées)
            }
            _NbIterations++;

            int IndexMin = _Open.IndexOfMinCout();
            Track BestTrack = (Track)_Open.ValueOfCout(IndexMin);//on repart toujours du moins couteux
            //Track BestTrack = (Track)_Open.ValueOfCout(IndexMin);

            _Open.Remove(BestTrack);
            //PropagateSpace2(BestTrack); -> il me semble qu'il suffit de faire comme Propagate (après juste pareil mais en tenant de l'occupation de case
            Propagate(BestTrack);
            _Closed.Add(BestTrack);

            //traces de Debug
            //C'est "normal" de ne pas voir les noeud triés car le foreach renvoit les noeuds par rapport à _List qui n'est pas trié, seul _ListCout est trié
            //Pour le chemin on se base sur la remontée des queues de chaque noeud donc ce n'est pas grave
            //pour l'espace on va chercher les éléments par l'indexeur [] qui utilise _ListCout et donc est trié
            //foreach (Track noeud in _Open)
            //{
            //    Debug.WriteLine(string.Format("Open : X:{0} Y:{1} C:{2}", noeud.EndNode.I_X, noeud.EndNode.I_Y, noeud.Cost));
            //}
            //foreach (Track noeud in _Closed)
            //{
            //    Debug.WriteLine(string.Format("Close : X:{0} Y:{1} C:{2}", noeud.EndNode.I_X, noeud.EndNode.I_Y, noeud.Cost));
            //}
            //Debug.WriteLine("");
            //if (_NbIterations % 10000 == 0)
            //{
            //    Debug.WriteLine("NextSpaceStep BestTrack.Cost=" + BestTrack.Cost.ToString() + " BestTrack.EndNode.ID_CASE=" + BestTrack.EndNode.ID_CASE.ToString());
            //    Debug.WriteLine("_Open.Count=" + _Open.Count.ToString() /*+ "_Open.CountCout=" + _Open.CountCout.ToString()*/);
            //    Debug.WriteLine("_Closed.Count=" + _Closed.Count.ToString() /*+ "_Closed.CountCout=" + _Closed.CountCout.ToString()*/);
            //}
            return _Open.Count > 0;
        }

        private void PropagateSpace(Track TrackToPropagate)
        {
            //DateTime timeStart;
            //TimeSpan perf;

            //timeStart = DateTime.Now;
            foreach (Node A in Donnees.m_donnees.TAB_CASE.CasesVoisines(TrackToPropagate.EndNode))
            {
                //BEA, je ne vois plus l'intéret du test qui suit, effectivement, le test est déjà fait dans CasesVoisines
                //if (A.I_X < m_minX || A.I_Y < m_minY || A.I_X > m_maxX || A.I_Y > m_maxY)
                //{
                //    continue;
                //}

                int cout = Cout(TrackToPropagate.EndNode, A);
                if (cout != Constantes.CST_COUTMAX)
                {
                    //DateTime timeStart2 = DateTime.Now;
                    ///if (A.I_COUT == CST_COUTMAX || (TrackToPropagate.EndNode.I_COUT + cout < A.I_COUT))
                    //if ((TrackToPropagate.EndNode.I_COUT + cout < A.I_COUT))//pas sur que ce soit bien A.I_COUT == CST_COUTMAX, 28/11/2011
                    //{
                        m_espaceTrouve++;//on vient de trouver une case de plus
                        //A.I_COUT = TrackToPropagate.EndNode.I_COUT + cout;
                        //if (A.I_COUT < 0)
                        //{
                        //    Debug.WriteLine("AStar.PropagateSpace cout négatif !!!");
                        //}
                        //if (A.I_X==100 && A.I_Y==100)
                        //{
                        //    Debug.WriteLine(string.Format("AStar.PropagateSpace 100,100 cout final= {0}, cout={1} de {2},{3} avec un cout de {4}",
                        //        A.I_COUT, cout, TrackToPropagate.EndNode.I_X, TrackToPropagate.EndNode.I_Y, TrackToPropagate.EndNode.I_COUT));
                        //}
                        Track Successor = new Track(TrackToPropagate, A, this);// le cout est calculé dans le new
                        _Open.Add(Successor);
                    //}
                }
                //perf = DateTime.Now - timeStart2;
                //Debug.WriteLine(string.Format("AStar.PropagateSpace interne en {0} minutes, {1} secondes, {2} millisecondes", perf.Minutes, perf.Seconds, perf.Milliseconds));
            }
            //perf = DateTime.Now - timeStart;
            //Debug.WriteLine(string.Format("AStar.PropagateSpace en {0} minutes, {1} secondes, {2} millisecondes", perf.Minutes, perf.Seconds, perf.Milliseconds));
        }

    }
}
