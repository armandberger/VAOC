using System;
using System.Collections;
using WaocLib;

namespace vaoc
{
	/// <summary>
	/// A heuristic is a function that associates a value with a node to gauge it considering the node to reach.
	/// </summary>
    public delegate double Heuristic(DataSetCoutDonnees.TAB_CASERow NodeToEvaluate, DataSetCoutDonnees.TAB_CASERow TargetNode, DataSetCoutDonnees.TAB_MODELE_MOUVEMENTRow modele);

	/// <summary>
	/// Class to search the best path between two nodes on a graph.
	/// </summary>
	public class AStar
	{
        DataSetCoutDonnees.TAB_CASEDataTable m_tableCase;
        DataSetCoutDonnees.TAB_VOISINS_CASEDataTable m_tableVoisin;
        DataSetCoutDonnees.TAB_MODELE_MOUVEMENTDataTable m_tableModeleMouvement;
        DataSetCoutDonnees.TAB_MOUVEMENT_COUTDataTable m_tableCout;
		SortableList _Open, _Closed;
		Track _LeafToGoBackUp;
		int _NbIterations = -1;

		SortableList.Equality SameNodesReached = new SortableList.Equality( Track.SameEndNode );

		/// <summary>
		/// AStar Constructor.
		/// </summary>
		/// <param name="G">The graph on which AStar will perform the search.</param>
		public AStar(
            DataSetCoutDonnees.TAB_CASEDataTable tableCase,
            DataSetCoutDonnees.TAB_VOISINS_CASEDataTable tableVoisin,
            DataSetCoutDonnees.TAB_MODELE_MOUVEMENTDataTable tableModeleMouvement, 
            DataSetCoutDonnees.TAB_MOUVEMENT_COUTDataTable tableCout
            )
		{
            m_tableCase = tableCase;
            m_tableVoisin = tableVoisin;
            m_tableModeleMouvement = tableModeleMouvement;
            m_tableCout = tableCout;

			_Open = new SortableList();
			_Closed = new SortableList();
		}

		/// <summary>
		/// Searches for the best path to reach the specified EndNode from the specified StartNode.
		/// </summary>
		/// <exception cref="ArgumentNullException">StartNode and EndNode cannot be null.</exception>
		/// <param name="StartNode">The node from which the path must start.</param>
		/// <param name="EndNode">The node to which the path must end.</param>
		/// <returns>'true' if succeeded / 'false' if failed.</returns>
        public bool SearchPath(DataSetCoutDonnees.TAB_CASERow StartNode, DataSetCoutDonnees.TAB_CASERow EndNode)
		{
			lock (_Graph)
			{
				Initialize(StartNode, EndNode);
				while ( NextStep() ) {}
				return PathFound;
			}
		}

		/// <summary>
		/// Use for a 'step by step' search only. This method is alternate to SearchPath.
		/// Initializes AStar before performing search steps manually with NextStep.
		/// </summary>
		/// <exception cref="ArgumentNullException">StartNode and EndNode cannot be null.</exception>
		/// <param name="StartNode">The node from which the path must start.</param>
		/// <param name="EndNode">The node to which the path must end.</param>
        public void Initialize(DataSetCoutDonnees.TAB_CASERow StartNode, DataSetCoutDonnees.TAB_CASERow EndNode)
		{
			if ( StartNode==null || EndNode==null ) throw new ArgumentNullException();
			_Closed.Clear();
			_Open.Clear();
			Track.Target = EndNode;
			_Open.Add( new Track(StartNode) );
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

			int IndexMin = _Open.IndexOfMin();
			Track BestTrack = (Track)_Open[IndexMin];
			_Open.RemoveAt(IndexMin);
			if ( BestTrack.Succeed )
			{
				_LeafToGoBackUp = BestTrack;
				_Open.Clear();
			}
			else
			{
				Propagate(BestTrack);
				_Closed.Add(BestTrack);
			}
			return _Open.Count>0;
		}

		private void Propagate(Track TrackToPropagate)
		{
			foreach ( Arc A in TrackToPropagate.EndNode.OutgoingArcs )
			{
				if ( A.Passable && A.EndNode.Passable )
				{
					Track Successor = new Track(TrackToPropagate, A);
					int PosNF = _Closed.IndexOf(Successor, SameNodesReached);
					int PosNO = _Open.IndexOf(Successor, SameNodesReached);
					if ( PosNF>0 && Successor.Cost>=((Track)_Closed[PosNF]).Cost ) continue;
					if ( PosNO>0 && Successor.Cost>=((Track)_Open[PosNO]).Cost ) continue;
					if ( PosNF>0 ) _Closed.RemoveAt(PosNF);
					if ( PosNO>0 ) _Open.RemoveAt(PosNO);
					_Open.Add(Successor);
				}
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

		/// <summary>
		/// Gets the array of nodes representing the found path.
		/// </summary>
		/// <exception cref="InvalidOperationException">You cannot get a result unless the search has ended.</exception>
        public DataSetCoutDonnees.TAB_CASERow[] PathByNodes
		{
			get 
			{
				CheckSearchHasEnded();
				if ( !PathFound ) return null;
				return GoBackUpNodes(_LeafToGoBackUp);
			}
		}

		private Node[] GoBackUpNodes(Track T)
		{
			int Nb = T.NbArcsVisited;
			Node[] Path = new Node[Nb+1];
			for (int i=Nb; i>=0; i--, T=T.Queue)
			Path[i] = T.EndNode;
			return Path;
		}

		/// <summary>
		/// Gets the array of arcs representing the found path.
		/// </summary>
		/// <exception cref="InvalidOperationException">You cannot get a result unless the search has ended.</exception>
		public Arc[] PathByArcs
		{
			get 
			{
				CheckSearchHasEnded();
				if ( !PathFound ) return null;
				int Nb = _LeafToGoBackUp.NbArcsVisited;
				Arc[] Path = new Arc[Nb];
				Track Cur=_LeafToGoBackUp;
				for (int i=Nb-1; i>=0; i--, Cur = Cur.Queue)
					Path[i] = Cur.Queue.EndNode.ArcGoingTo(Cur.EndNode);
				return Path;
			}
		}

		/// <summary>
		/// Gets the array of points representing the found path.
		/// </summary>
		/// <exception cref="InvalidOperationException">You cannot get a result unless the search has ended.</exception>
		public Point3D[] PathByCoordinates
		{
			get 
			{
				CheckSearchHasEnded();
				if ( !PathFound ) return null;
				int Nb = _LeafToGoBackUp.NbArcsVisited;
				Point3D[] Path = new Point3D[Nb+1];
				Track Cur=_LeafToGoBackUp;
				for (int i=Nb; i>=0; i--, Cur = Cur.Queue)
					Path[i] = Cur.EndNode.Position;
				return Path;
			}
		}
	}
}

