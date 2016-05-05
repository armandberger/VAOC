using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vaoc
{
    [Serializable]
    class LigneMODELE_PION
    {
        public int ID_MODELE_PION { get; set; }
        public string S_NOM { get; set; }
        public int ID_MODELE_MOUVEMENT { get; set; }
        public int ID_NATION { get; set; }
        public int I_ROUGE { get; set; }
        public int I_VERT { get; set; }
        public int I_BLEU { get; set; }
        public int I_VISION_JOUR { get; set; }
        public int I_VISION_NUIT { get; set; }

        public LigneMODELE_PION(Donnees.TAB_MODELE_PIONRow ligneRow)
        {
            ID_MODELE_PION = ligneRow.ID_MODELE_PION;
            ID_MODELE_MOUVEMENT = ligneRow.ID_MODELE_MOUVEMENT;
            S_NOM = ligneRow.S_NOM;
            ID_NATION = ligneRow.ID_NATION;
            I_ROUGE = ligneRow.I_ROUGE;
            I_VERT = ligneRow.I_VERT;
            I_BLEU = ligneRow.I_BLEU;
            I_VISION_JOUR = ligneRow.I_VISION_JOUR;
            I_VISION_NUIT = ligneRow.I_VISION_NUIT;
        }
    }
}
