using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vaoc
{
    [Serializable]
    class LigneBATAILLE
    {
        public int ID_BATAILLE { get; set; }
        public string S_NOM { get; set; }
        public int I_TOUR_DEBUT { get; set; }
        public int I_PHASE_DEBUT { get; set; }
        public int I_TOUR_FIN { get; set; }
        public int I_PHASE_FIN { get; set; }
        public char C_ORIENTATION { get; set; }
        public int ID_TERRAIN_0 { get; set; }
        public int ID_TERRAIN_1 { get; set; }
        public int ID_TERRAIN_2 { get; set; }
        public int ID_TERRAIN_3 { get; set; }
        public int ID_TERRAIN_4 { get; set; }
        public int ID_TERRAIN_5 { get; set; }
        public int ID_OBSTACLE_03 { get; set; }
        public int ID_OBSTACLE_14 { get; set; }
        public int ID_OBSTACLE_25 { get; set; }
        public int ID_NATION_012 { get; set; }
        public int ID_NATION_345 { get; set; }
        public int ID_LEADER_012 { get; set; }
        public int ID_LEADER_345 { get; set; }
        public int I_X_CASE_HAUT_GAUCHE { get; set; }
        public int I_Y_CASE_HAUT_GAUCHE { get; set; }
        public int I_X_CASE_BAS_DROITE { get; set; }
        public int I_Y_CASE_BAS_DROITE { get; set; }
        public int I_ENGAGEMENT_0 { get; set; }
        public int I_ENGAGEMENT_1 { get; set; }
        public int I_ENGAGEMENT_2 { get; set; }
        public int I_ENGAGEMENT_3 { get; set; }
        public int I_ENGAGEMENT_4 { get; set; }
        public int I_ENGAGEMENT_5 { get; set; }
        public string S_COMBAT_0 { get; set; }
        public string S_COMBAT_1 { get; set; }
        public string S_COMBAT_2 { get; set; }
        public string S_COMBAT_3 { get; set; }
        public string S_COMBAT_4 { get; set; }
        public string S_COMBAT_5 { get; set; }

        public LigneBATAILLE()
        {
        }

        public LigneBATAILLE(Donnees.TAB_BATAILLERow ligneXML)
        {
            ID_BATAILLE = ligneXML.ID_BATAILLE;
            S_NOM = ligneXML.S_NOM;
            I_TOUR_DEBUT = ligneXML.I_TOUR_DEBUT;
            I_PHASE_DEBUT = ligneXML.I_PHASE_DEBUT;
            I_TOUR_FIN = ligneXML.I_TOUR_FIN;
            I_PHASE_FIN = ligneXML.I_PHASE_FIN;
            C_ORIENTATION = ligneXML.C_ORIENTATION;
            ID_TERRAIN_0 = ligneXML.ID_TERRAIN_0;
            ID_TERRAIN_1 = ligneXML.ID_TERRAIN_1;
            ID_TERRAIN_2 = ligneXML.ID_TERRAIN_2;
            ID_TERRAIN_3 = ligneXML.ID_TERRAIN_3;
            ID_TERRAIN_4 = ligneXML.ID_TERRAIN_4;
            ID_TERRAIN_5 = ligneXML.ID_TERRAIN_5;
            ID_OBSTACLE_03 = ligneXML.ID_OBSTACLE_03;
            ID_OBSTACLE_14 = ligneXML.ID_OBSTACLE_14;
            ID_OBSTACLE_25 = ligneXML.ID_OBSTACLE_25;
            ID_NATION_012 = ligneXML.ID_NATION_012;
            ID_NATION_345 = ligneXML.ID_NATION_345;
            ID_LEADER_012 = ligneXML.ID_LEADER_012;
            ID_LEADER_345 = ligneXML.ID_LEADER_345;
            I_X_CASE_HAUT_GAUCHE = ligneXML.I_X_CASE_HAUT_GAUCHE;
            I_Y_CASE_HAUT_GAUCHE = ligneXML.I_Y_CASE_HAUT_GAUCHE;
            I_X_CASE_BAS_DROITE = ligneXML.I_X_CASE_BAS_DROITE;
            I_Y_CASE_BAS_DROITE = ligneXML.I_Y_CASE_BAS_DROITE;
            I_ENGAGEMENT_0 = ligneXML.I_ENGAGEMENT_0;
            I_ENGAGEMENT_1 = ligneXML.I_ENGAGEMENT_1;
            I_ENGAGEMENT_2 = ligneXML.I_ENGAGEMENT_2;
            I_ENGAGEMENT_3 = ligneXML.I_ENGAGEMENT_3;
            I_ENGAGEMENT_4 = ligneXML.I_ENGAGEMENT_4;
            I_ENGAGEMENT_5 = ligneXML.I_ENGAGEMENT_5;
            S_COMBAT_0 = ligneXML.S_COMBAT_0;
            S_COMBAT_1 = ligneXML.S_COMBAT_1;
            S_COMBAT_2 = ligneXML.S_COMBAT_2;
            S_COMBAT_3 = ligneXML.S_COMBAT_3;
            S_COMBAT_4 = ligneXML.S_COMBAT_4;
            S_COMBAT_5 = ligneXML.S_COMBAT_5;
        }

        public LigneBATAILLE(        
            int ID_BATAILLEp,
            string S_NOMp,
            int I_TOUR_DEBUTp,
            int I_PHASE_DEBUTp,
            int I_TOUR_FINp,
            int I_PHASE_FINp,
            char C_ORIENTATIONp,
            int ID_TERRAIN_0p,
            int ID_TERRAIN_1p,
            int ID_TERRAIN_2p,
            int ID_TERRAIN_3p,
            int ID_TERRAIN_4p,
            int ID_TERRAIN_5p,
            int ID_OBSTACLE_03p,
            int ID_OBSTACLE_14p,
            int ID_OBSTACLE_25p,
            int ID_NATION_012p,
            int ID_NATION_345p,
            int ID_LEADER_012p,
            int ID_LEADER_345p,
            int I_X_CASE_HAUT_GAUCHEp,
            int I_Y_CASE_HAUT_GAUCHEp,
            int I_X_CASE_BAS_DROITEp,
            int I_Y_CASE_BAS_DROITEp,
            int I_ENGAGEMENT_0p,
            int I_ENGAGEMENT_1p,
            int I_ENGAGEMENT_2p,
            int I_ENGAGEMENT_3p,
            int I_ENGAGEMENT_4p,
            int I_ENGAGEMENT_5p,
            string S_COMBAT_0p,
            string S_COMBAT_1p,
            string S_COMBAT_2p,
            string S_COMBAT_3p,
            string S_COMBAT_4p,
            string S_COMBAT_5p
            )
        {
            ID_BATAILLE = ID_BATAILLEp;
            S_NOM = S_NOMp;
            I_TOUR_DEBUT = I_TOUR_DEBUTp;
            I_PHASE_DEBUT = I_PHASE_DEBUTp;
            I_TOUR_FIN = I_TOUR_FINp;
            I_PHASE_FIN = I_PHASE_FINp;
            C_ORIENTATION = C_ORIENTATIONp;
            ID_TERRAIN_0 = ID_TERRAIN_0p;
            ID_TERRAIN_1 = ID_TERRAIN_1p;
            ID_TERRAIN_2 = ID_TERRAIN_2p;
            ID_TERRAIN_3 = ID_TERRAIN_3p;
            ID_TERRAIN_4 = ID_TERRAIN_4p;
            ID_TERRAIN_5 = ID_TERRAIN_5p;
            ID_OBSTACLE_03 = ID_OBSTACLE_03p;
            ID_OBSTACLE_14 = ID_OBSTACLE_14p;
            ID_OBSTACLE_25 = ID_OBSTACLE_25p;
            ID_NATION_012 = ID_NATION_012p;
            ID_NATION_345 = ID_NATION_345p;
            ID_LEADER_012 = ID_LEADER_012p;
            ID_LEADER_345 = ID_LEADER_345p;
            I_X_CASE_HAUT_GAUCHE = I_X_CASE_HAUT_GAUCHEp;
            I_Y_CASE_HAUT_GAUCHE = I_Y_CASE_HAUT_GAUCHEp;
            I_X_CASE_BAS_DROITE = I_X_CASE_BAS_DROITEp;
            I_Y_CASE_BAS_DROITE = I_Y_CASE_BAS_DROITEp;
            I_ENGAGEMENT_0 = I_ENGAGEMENT_0p;
            I_ENGAGEMENT_1 = I_ENGAGEMENT_1p;
            I_ENGAGEMENT_2 = I_ENGAGEMENT_2p;
            I_ENGAGEMENT_3 = I_ENGAGEMENT_3p;
            I_ENGAGEMENT_4 = I_ENGAGEMENT_4p;
            I_ENGAGEMENT_5 = I_ENGAGEMENT_5p;
            S_COMBAT_0 = S_COMBAT_0p;
            S_COMBAT_1 = S_COMBAT_1p;
            S_COMBAT_2 = S_COMBAT_2p;
            S_COMBAT_3 = S_COMBAT_3p;
            S_COMBAT_4 = S_COMBAT_4p;
            S_COMBAT_5 = S_COMBAT_5p;
        }
    }
}
