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
    public class Cible
    {
        private Donnees.TAB_CASERow m_case;
        private Donnees.TAB_NOMS_CARTERow m_ville;
        public Donnees.TAB_CASERow Case { get{return m_case;}}
        public Donnees.TAB_NOMS_CARTERow Ville { get { return m_ville; } set { m_ville = value; } }
        public Cible(Donnees.TAB_NOMS_CARTERow villeInit, Donnees.TAB_CASERow caseInit)
        {
            m_case = caseInit;
            m_ville = villeInit;
        }
    }

	/// <summary>
	/// A track is a succession of nodes which have been visited.
	/// Thus when it leads to the target node, it is easy to return the result path.
	/// These objects are contained in Open and Closed lists.
	/// </summary>
	public class TrackVille : IComparable
	{
        private static Cible _Target = null;

        public static Cible Target { set { _Target = value; } get { return _Target; } }

        public static int tailleBloc = 0;

        public Cible EndNode;
        public Donnees.TAB_PCC_VILLESRow EndNodeVille;
        public TrackVille Queue;

		private int _NbArcsVisited;
		public int NbArcsVisited { get { return _NbArcsVisited; } }

        private int _Cost; // un int vaut max 2,147,483,647, ce qui veut dire qu'avec un cout de 1000 par case, on peut encore faire un trajet de 21,474 km donc si on va au-dessus, il y a un gros gros problème quelque part :-)
		public int Cost { get { return _Cost; } }
        private int _OutRoad;
        public int OutRoad { get { return _OutRoad; } }

        public bool Succeed { get { return ((EndNode!=null) && (EndNode.Case == _Target.Case) /*&& (EndNode.Ville == _Target.Ville)*/); } }

        private int _Id;//on doit se baser sur le même Id de case pour tous les arcs
        public int Id { get { return _Id; } }

        public TrackVille(Cible GraphNode)
		{
			_Cost = 0;
            _OutRoad = 0;
			_NbArcsVisited = 0;
			Queue = null;
			EndNode = GraphNode;
            EndNodeVille = null;
            _Id = EndNode.Case.ID_CASE;
        }

        public TrackVille(Donnees.TAB_PCC_VILLESRow GraphNode)
        {
            _Cost = 0;
            _OutRoad = 0;
            _NbArcsVisited = 0;
            Queue = null;
            EndNode = null;
            EndNodeVille = GraphNode;
            Donnees.TAB_CASERow ligneCase = Donnees.m_donnees.TAB_CASE.FindByID_NOM(EndNodeVille.ID_VILLE_FIN);
            _Id = ligneCase.ID_CASE;
        }

        public TrackVille(TrackVille PreviousTrack, Cible caseFinale)
		{
			Queue = PreviousTrack;
            EndNode = caseFinale;
            _Id = EndNode.Case.ID_CASE;
            if (null == Queue.EndNodeVille)
            {
                _Cost = Queue.Cost + AStarVille.Cout(Queue.EndNode.Case, caseFinale.Case);
                _OutRoad = Queue.OutRoad + AStarVille.HorsRoute(Queue.EndNode.Case, caseFinale.Case);
            }
            else
            {
                Donnees.TAB_CASERow ligneCaseSource = Donnees.m_donnees.TAB_CASE.FindByID_NOM(Queue.EndNodeVille.ID_VILLE_FIN);
                _Cost = Queue.Cost + AStarVille.Cout(ligneCaseSource, caseFinale.Case);
                _OutRoad = Queue.OutRoad + AStarVille.HorsRoute(ligneCaseSource, caseFinale.Case);
            }
            _NbArcsVisited = Queue._NbArcsVisited + 1;
        }

        public TrackVille(TrackVille PreviousTrack, TrackVille NextTrack)
        {
            Queue = PreviousTrack;
            _Cost = PreviousTrack.Cost + AStarVille.Cout(PreviousTrack, NextTrack);
            _OutRoad = PreviousTrack.OutRoad + AStarVille.HorsRoute(PreviousTrack, NextTrack);
            _NbArcsVisited = Queue._NbArcsVisited + 1;
            EndNode = NextTrack.EndNode;
            EndNodeVille = NextTrack.EndNodeVille;
            if (null == EndNodeVille)
            {
                _Id = EndNode.Case.ID_CASE;
            }
            else
            {
                Donnees.TAB_CASERow ligneCaseSource = Donnees.m_donnees.TAB_CASE.FindByID_NOM(EndNodeVille.ID_VILLE_FIN);
                _Id = ligneCaseSource.ID_CASE;
            }
        }
        
        public int CompareTo(object Objet)
		{
            TrackVille OtherTrack = (TrackVille)Objet;
            return Cost.CompareTo(OtherTrack.Cost);
		}

		public static bool SameEndNode(object O1, object O2)
		{
            TrackVille P1 = O1 as TrackVille;
            TrackVille P2 = O2 as TrackVille;
			if ( P1==null || P2==null ) throw new ArgumentException("Objects must be of 'TrackVille' type.");
            if (null!=P1.EndNode && null!=P2.EndNode)
			    return P1.EndNode==P2.EndNode;//case à case
            if (null != P1.EndNodeVille && null != P2.EndNodeVille)
                return P1.EndNodeVille == P2.EndNodeVille;
            return false;
		}
	}
}
