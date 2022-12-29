using System;
using System.Diagnostics;
using System.Threading;
using WaocLib;

namespace vaoc
{
    /// <summary>
    /// Noeud fait pour remplacer Donnees.TAB_CASERow dans Track.cs car en multi-threading
    /// les cases peuvent changer et cela provoque des crash
    /// </summary>
	public class Node
	{
        private int m_idcase;
        private int m_x;
        private int m_y;
        private int m_modeleTerrain;
        private int m_proprietaire;
        private int m_nouveauProprietaire;

        public int ID_CASE { get { return m_idcase; } }
        public int I_X { get { return m_x; } }
        public int I_Y { get { return m_y; } }
        public int ID_MODELE_TERRAIN { get { return m_modeleTerrain; } }
        public int ID_PROPRIETAIRE { get { return m_proprietaire; } }
        public int ID_NOUVEAU_PROPRIETAIRE { get { return m_nouveauProprietaire; } }
        

        public Node(Donnees.TAB_CASERow ligneCase)
        {
            int IdProprietaire;
            int IdNouveauProprietaire;
            try
            {
                m_idcase = ligneCase.ID_CASE;
                m_x = ligneCase.I_X;
                m_y = ligneCase.I_Y;
                m_modeleTerrain = ligneCase.ID_MODELE_TERRAIN;
                //Monitor.Enter(Donnees.m_donnees.TAB_CASE.Rows.SyncRoot);//DBNull is not thread safe...
                IdProprietaire = ligneCase.ID_PROPRIETAIRE;
                m_proprietaire = IdProprietaire;
                IdNouveauProprietaire = ligneCase.ID_NOUVEAU_PROPRIETAIRE;
                m_nouveauProprietaire = IdNouveauProprietaire;
                //Monitor.Exit(Donnees.m_donnees.TAB_CASE.Rows.SyncRoot);
            }
            catch (Exception ex)
            {
                string messageEX = string.Format("exception Node {3} : {0} : {1} :{2}",
                       ex.Message, (null == ex.InnerException) ? "sans inner exception" : ex.InnerException.Message,
                       ex.StackTrace, ex.GetType().ToString());
                Debug.WriteLine(messageEX);
                throw;
            }
        }
    }
}

