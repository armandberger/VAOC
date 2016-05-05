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


namespace vaoc
{
	/// <summary>
	/// A track is a succession of nodes which have been visited.
	/// Thus when it leads to the target node, it is easy to return the result path.
	/// These objects are contained in Open and Closed lists.
	/// </summary>
	public class Track : IComparable
	{
        private static LigneCASE _Target = null;
        //private static Donnees.TAB_PCC_COUTSRow _TargetHPA = null;

        public static LigneCASE Target { set { _Target = value; } get { return _Target; } }
        //public static Donnees.TAB_PCC_COUTSRow Target { set { _TargetHPA = value; } get { return _TargetHPA; } }

        private static long idGlobal = 0;
        public static int tailleBloc = 0;

        public LigneCASE EndNode;
        public LignePCC_COUTS EndNodeHPA;
        public Track Queue;

		private int _NbArcsVisited;
		public int NbArcsVisited { get { return _NbArcsVisited; } }

        private int _Cost; // un int vaut max 2,147,483,647, ce qui veut dire qu'avec un cout de 1000 par case, on peut encore faire un trajet de 21,474 km donc si on va au-dessus, il y a un gros gros problème quelque part :-)
        public int Cost { get { return _Cost; } }
        private int _OutRoad;
        public int OutRoad { get { return _OutRoad; } }

        public bool Succeed { get { if (null==EndNodeHPA) return (EndNode == _Target); else return (EndNodeHPA.ID_CASE_FIN == _Target.ID_CASE); } }

        public long Id { get; set; }
        public int blocX { get; set; }
        public int blocY { get; set; }

        public Track(LigneCASE GraphNode)
		{
            //if ( _Target==null ) throw new InvalidOperationException("You must specify a target Node for the Track class.");
			_Cost = 0;
            _OutRoad = 0;
            _NbArcsVisited = 0;
			Queue = null;
			EndNode = GraphNode;
            EndNodeHPA = null;
            blocX = (0 == EndNode.I_X % tailleBloc) ? (EndNode.I_X / tailleBloc) - 1 : EndNode.I_X / tailleBloc;
            blocY = (0 == EndNode.I_Y % tailleBloc) ? (EndNode.I_Y / tailleBloc) - 1 : EndNode.I_Y / tailleBloc;
            Id = idGlobal++;
        }

        public Track(LignePCC_COUTS GraphNode)
        {
            if (_Target == null) throw new InvalidOperationException("You must specify a target Node for the Track class.");
            _Cost = 0;
            _OutRoad = 0;
            _NbArcsVisited = 0;
            Queue = null;
            EndNode = null;
            EndNodeHPA = GraphNode;
            blocX = EndNodeHPA.I_BLOCX;
            blocY = EndNodeHPA.I_BLOCY;
            Id = idGlobal++;
        }

        //public Track(Track PreviousTrack, Donnees.TAB_PCC_COUTSRow ligneCout)
        //{
        //    //if (_Target == null) throw new InvalidOperationException("You must specify a target Node for the Track class.");
        //    Queue = PreviousTrack;

        //    //if (null == Queue.EndNodeHPA)
        //    //{
        //    //    _Cost = Queue.Cost;//passage d'une case frontière a une case de zone
        //    //}
        //    //else
        //    //{
        //        _Cost = Queue.Cost + ligneCout.I_COUT;
        //    //}
            
        //    _NbArcsVisited = Queue._NbArcsVisited + 1;
        //    EndNode = null;
        //    EndNodeHPA = ligneCout;
        //    blocX = EndNodeHPA.I_BLOCX;
        //    blocY = EndNodeHPA.I_BLOCY;
        //    Id = idGlobal++;
        //}

        public Track(Track PreviousTrack, LigneCASE caseFinale)
		{
            //if (_Target==null) throw new InvalidOperationException("You must specify a target Node for the Track class.");
			Queue = PreviousTrack;
            if (null == Queue.EndNodeHPA)
            {
                _Cost = Queue.Cost+AStar.Cout(Queue.EndNode, caseFinale);
                _OutRoad = Queue.OutRoad + AStar.HorsRoute(Queue.EndNode, caseFinale);
            }
            else
            {
                LigneCASE ligneCaseSource = BD.Base.Case.TrouveParID_CASE(Queue.EndNodeHPA.ID_CASE_FIN);                
                _Cost = Queue.Cost + AStar.Cout(ligneCaseSource, caseFinale);
                _OutRoad = Queue.OutRoad + AStar.HorsRoute(ligneCaseSource, caseFinale);
            }
			_NbArcsVisited = Queue._NbArcsVisited + 1;
			EndNode = caseFinale;
            blocX = PreviousTrack.blocX;
            blocY = PreviousTrack.blocY;
            Id = idGlobal++;
        }

        public Track(Track PreviousTrack, Track NextTrack)
        {
            //if (_Target == null) throw new InvalidOperationException("You must specify a target Node for the Track class.");
            Queue = PreviousTrack;
            //if (null == Queue.EndNodeHPA)
            //{
            //    //le noeud précédent est une case
            //    if (null == NextTrack.EndNodeHPA)
            //    {
            //        //le noeud suivant est une case
            //        _Cost = Queue.Cost + AStar.Cout(Queue.EndNode, NextTrack.EndNode);
            //    }
            //    else
            //    {
            //        //le noeud suivant est un bloc
            //        _Cost = Queue.Cost;//passage d'une case frontière a une case de zone
            //    }
            //}
            //else
            //{
            //    //le noeud précédent est un bloc
            //    _Cost = Queue.Cost + Queue.EndNodeHPA.I_COUT;
            //}
            _Cost = PreviousTrack.Cost + AStar.Cout(PreviousTrack, NextTrack);
            _NbArcsVisited = Queue._NbArcsVisited + 1;
            EndNode = NextTrack.EndNode;
            EndNodeHPA = NextTrack.EndNodeHPA;
            blocX = NextTrack.blocX;
            blocY = NextTrack.blocY;
            Id = idGlobal++;
        }
        
        public int CompareTo(object Objet)
		{
			Track OtherTrack = (Track) Objet;
            return Cost.CompareTo(OtherTrack.Cost);
		}

		public static bool SameEndNode(object O1, object O2)
		{
			Track P1 = O1 as Track;
			Track P2 = O2 as Track;
			if ( P1==null || P2==null ) throw new ArgumentException("Objects must be of 'Track' type.");
            if (null!=P1.EndNode && null!=P2.EndNode)
			    return P1.EndNode==P2.EndNode;//case à case
            if (null != P1.EndNodeHPA && null != P2.EndNodeHPA)
                return P1.EndNodeHPA == P2.EndNodeHPA;//bloc à bloc
            return false;
		}
	}
}
