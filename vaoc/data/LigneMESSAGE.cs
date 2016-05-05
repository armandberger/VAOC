using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vaoc
{
    [Serializable]
    class LigneMESSAGE
    {
        public int ID_MESSAGE { get; set; }
        public int ID_PION_EMETTEUR { get; set; }
        public int ID_PION_PROPRIETAIRE { get; set; }
        public int I_TYPE { get; set; }
        public int I_TOUR_ARRIVEE { get; set; }
        public int I_PHASE_ARRIVEE { get; set; }
        public int I_TOUR_DEPART { get; set; }
        public int I_PHASE_DEPART { get; set; }
        public string S_TEXTE { get; set; }
        public int I_INFANTERIE { get; set; }
        public int I_CAVALERIE { get; set; }
        public int I_ARTILLERIE { get; set; }
        public int I_FATIGUE { get; set; }
        public int I_MORAL { get; set; }
        public int I_TOUR_SANS_RAVITAILLEMENT { get; set; }
        public int ID_BATAILLE { get; set; }
        public int I_ZONE_BATAILLE { get; set; }
        public int I_RETRAITE { get; set; }
        public bool B_DETRUIT { get; set; }
        public int ID_CASE { get; set; }
        public int ID_CASE_DEBUT { get; set; }
        public int ID_CASE_FIN { get; set; }
        public int I_NB_PHASES_MARCHE_JOUR { get; set; }
        public int I_NB_PHASES_MARCHE_NUIT { get; set; }
        public int I_NB_HEURES_COMBAT { get; set; }
        public int I_MATERIEL { get; set; }
        public int I_RAVITAILLEMENT { get; set; }
        public int I_SOLDATS_RAVITAILLES { get; set; }
        public int I_NB_HEURES_FORTIFICATION { get; set; }
        public int I_NIVEAU_FORTIFICATION { get; set; }
        public int I_DUREE_HORS_COMBAT { get; set; }
        public int I_TOUR_BLESSURE { get; set; }
        public char C_NIVEAU_DEPOT { get; set; }

        public LigneMESSAGE()
        {
        }

        public LigneMESSAGE(Donnees.TAB_MESSAGERow ligneXML)
        {
            ID_MESSAGE = ligneXML.ID_MESSAGE;
        }
    }
}
