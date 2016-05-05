using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vaoc
{
    [Serializable]
    public class LignePCC_COUTS
    {
        public int I_BLOCX { get; set; }
        public int I_BLOCY { get; set; }
        public int ID_CASE_DEBUT { get; set; }
        public int ID_CASE_FIN { get; set; }
        public int I_COUT { get; set; }
        public int I_COUT_INITIAL { get; set; }
        public int ID_TRAJET { get; set; }
        public bool B_CREATION { get; set; }
        public int? ID_NATION { get; set; }

        public LignePCC_COUTS(int iblocx, int iblocy, int casedebut, int casefin, int cout, int coutInitial, int idTrajet, bool bCreation, int? idNation)
        {
            I_BLOCX = iblocx;
            I_BLOCY = iblocy;
            ID_CASE_DEBUT = casedebut;
            ID_CASE_FIN = casefin;
            I_COUT = cout;
            I_COUT_INITIAL = coutInitial;
            ID_TRAJET = idTrajet;
            B_CREATION = bCreation;
            ID_NATION = idNation;
        }

        public LignePCC_COUTS(Donnees.TAB_PCC_COUTSRow ligneXML)
        {
            I_BLOCX = ligneXML.I_BLOCX;
            I_BLOCY = ligneXML.I_BLOCY;
            ID_CASE_DEBUT = ligneXML.ID_CASE_DEBUT;
            ID_CASE_FIN = ligneXML.ID_CASE_FIN;
            I_COUT = ligneXML.I_COUT;
            I_COUT_INITIAL = ligneXML.I_COUT_INITIAL;
            ID_TRAJET = ligneXML.ID_TRAJET;
            B_CREATION = ligneXML.B_CREATION;
            ID_NATION = ligneXML.IsID_NATIONNull() ? null : (int?)ligneXML.ID_NATION;
        }
    }
}
