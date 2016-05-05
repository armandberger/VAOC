using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vaoc
{
    [Serializable]
    class LigneMODELE_MOUVEMENT
    {
        public int ID_MODELE_MOUVEMENT { get; set; }
        public string S_NOM { get; set; }
        public decimal I_VITESSE_INFANTERIE { get; set; }
        public decimal I_VITESSE_CAVALERIE { get; set; }
        public decimal I_VITESSE_ARTILLERIE { get; set; }

        public LigneMODELE_MOUVEMENT(Donnees.TAB_MODELE_MOUVEMENTRow ligneRow)
        {
            ID_MODELE_MOUVEMENT = ligneRow.ID_MODELE_MOUVEMENT;
            S_NOM = ligneRow.S_NOM;
            I_VITESSE_INFANTERIE = ligneRow.I_VITESSE_INFANTERIE;
            I_VITESSE_CAVALERIE = ligneRow.I_VITESSE_CAVALERIE;
            I_VITESSE_ARTILLERIE = ligneRow.I_VITESSE_ARTILLERIE;
        }

        /// <summary>
        /// Renvoie le coût d'une case pour un modèle de terrain particulier pour le modèle de mouvent courant et la météo courante
        /// </summary>
        /// <param name="ID_MODELE_TERRAIN">modèle de terrain</param>
        /// <returns>cout du terrain</returns>
        public int CoutCase(int ID_MODELE_TERRAIN)
        {
            string requete = string.Format("ID_MODELE_MOUVEMENT={0} AND ID_METEO={1} AND ID_MODELE_TERRAIN={2}",
                    this.ID_MODELE_MOUVEMENT,
                    Donnees.m_donnees.TAB_PARTIE[0].ID_METEO,
                    ID_MODELE_TERRAIN);
            Donnees.TAB_MOUVEMENT_COUTRow[] resCout = (Donnees.TAB_MOUVEMENT_COUTRow[])Donnees.m_donnees.TAB_MOUVEMENT_COUT.Select(requete);
            return resCout[0].I_COUT;
        }
    }
}
