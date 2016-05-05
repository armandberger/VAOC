using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vaoc
{
    [Serializable]
    public class LigneCASE
    {
        //private Donnees.TAB_CASERow ligneRow;

        public int ID_CASE { get; set; }
        public int ID_MODELE_TERRAIN { get; set; }
        public int I_X { get; set; }
        public int I_Y { get; set; }
        public int? ID_PROPRIETAIRE { get; set; }
        public int? ID_NOUVEAU_PROPRIETAIRE { get; set; }
        public int? ID_MODELE_TERRAIN_SI_OCCUPE { get; set; }
        public double I_COUT { get; set; }
        //public int? ID_NOM { get; set; }

        public LigneCASE(int idcase, int idModeleTerrain, int iX, int iY, int? proprietaire, int? nouveauProprietaire, int coutInitial, double dCout)
        {
            ID_CASE = idcase;
            ID_MODELE_TERRAIN = idModeleTerrain;
            I_X = iX;
            I_Y = iY;
            ID_PROPRIETAIRE = proprietaire;
            ID_NOUVEAU_PROPRIETAIRE = nouveauProprietaire;
            I_COUT = dCout;
        }

        public LigneCASE(Donnees.TAB_CASERow ligneRow)
        {
            ID_CASE = ligneRow.ID_CASE;
            ID_MODELE_TERRAIN = ligneRow.ID_MODELE_TERRAIN;
            if (ligneRow.IsID_MODELE_TERRAIN_SI_OCCUPENull()) { ID_MODELE_TERRAIN_SI_OCCUPE = null; } else { ID_MODELE_TERRAIN_SI_OCCUPE = ligneRow.ID_MODELE_TERRAIN_SI_OCCUPE; }
            I_X = ligneRow.I_X;
            I_Y = ligneRow.I_Y;
            if (ligneRow.IsID_PROPRIETAIRENull()) { ID_PROPRIETAIRE = null; } else { ID_PROPRIETAIRE = ligneRow.ID_PROPRIETAIRE; }
            if (ligneRow.IsID_NOUVEAU_PROPRIETAIRENull()) { ID_NOUVEAU_PROPRIETAIRE = null; } else { ID_NOUVEAU_PROPRIETAIRE = ligneRow.ID_NOUVEAU_PROPRIETAIRE; }
            I_COUT = ligneRow.I_COUT;
        }
    }
}
