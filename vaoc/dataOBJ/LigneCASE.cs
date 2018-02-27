using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vaoc
{
    [Serializable]
    public class LigneCASE
    {
        private int m_ID_CASE;
        public int ID_CASE { get { return m_ID_CASE; } }

        private int m_ID_MODELE_TERRAIN;
        public int ID_MODELE_TERRAIN { get { return m_ID_MODELE_TERRAIN; } }

        private int m_I_X;
        public int I_X { get { return m_I_X; } }

        private int m_I_Y;
        public int I_Y { get { return m_I_Y; } }

        private int m_ID_PROPRIETAIRE;
        public int ID_PROPRIETAIRE { get { return m_ID_PROPRIETAIRE; } }

        private int m_ID_NOUVEAU_PROPRIETAIRE;
        public int ID_NOUVEAU_PROPRIETAIRE { get { return m_ID_NOUVEAU_PROPRIETAIRE; } }

        private double m_I_COUT;
        public double I_COUT { get { return m_I_COUT; } }
        //public int? ID_NOM { get; set; }

        private int m_ID_MODELE_TERRAIN_SI_OCCUPE;
        public int ID_MODELE_TERRAIN_SI_OCCUPE { get { return m_ID_MODELE_TERRAIN_SI_OCCUPE; } }

        public LigneCASE(int idcase, int idModeleTerrain, int idModeleTerrainSiOccupe, int iX, int iY, int proprietaire, int nouveauProprietaire, int coutInitial, double dCout)
        {
            m_ID_CASE = idcase;
            m_ID_MODELE_TERRAIN = idModeleTerrain;
            m_ID_MODELE_TERRAIN_SI_OCCUPE = idModeleTerrainSiOccupe;
            m_I_X = iX;
            m_I_Y = iY;
            m_ID_PROPRIETAIRE = proprietaire;
            m_ID_NOUVEAU_PROPRIETAIRE = nouveauProprietaire;
            m_I_COUT = dCout;
        }

        public LigneCASE(Donnees.TAB_CASERow ligneRow)
        {
            m_ID_CASE = ligneRow.ID_CASE;
            m_ID_MODELE_TERRAIN = ligneRow.ID_MODELE_TERRAIN;
            m_ID_MODELE_TERRAIN_SI_OCCUPE = ligneRow.ID_MODELE_TERRAIN_SI_OCCUPE;
            m_I_X = ligneRow.I_X;
            m_I_Y = ligneRow.I_Y;
            m_ID_PROPRIETAIRE = ligneRow.ID_PROPRIETAIRE;
            m_ID_NOUVEAU_PROPRIETAIRE = ligneRow.ID_NOUVEAU_PROPRIETAIRE;
            m_I_COUT = ligneRow.I_COUT;
        }
    }
}
