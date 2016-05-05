using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vaoc
{
    [Serializable]
    class LigneMOUVEMENT_COUT
    {
        public int ID_MODELE_MOUVEMENT { get; set; }
        public int ID_MODELE_TERRAIN { get; set; }
        public int ID_METEO { get; set; }
        public int I_COUT { get; set; }

        public LigneMOUVEMENT_COUT(Donnees.TAB_MOUVEMENT_COUTRow ligneRow)
        {
            ID_MODELE_MOUVEMENT = ligneRow.ID_MODELE_MOUVEMENT;
            ID_MODELE_TERRAIN = ligneRow.ID_MODELE_TERRAIN;
            ID_METEO = ligneRow.ID_METEO;
            I_COUT = ligneRow.I_COUT;
        }
    }
}
