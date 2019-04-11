using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vaoc
{
    public class ClassDataPion
    {
        public int ID_PION { get; set; }
        public int ID_PARTIE { get; set; }
        public int ID_PION_PROPRIETAIRE { get; set; }
        public int ID_PION_REMPLACE { get; set; }
        public int ID_MODELE_PION { get; set; }
        public string S_NOM  { get; set; }
        public int I_INFANTERIE { get; set; }
        public int I_INFANTERIE_REEL { get; set; }
        public int I_INFANTERIE_INITIALE  { get; set; }
        public int I_CAVALERIE { get; set; }
        public int I_CAVALERIE_REEL { get; set; }
        public int I_CAVALERIE_INITIALE { get; set; }
        public int I_ARTILLERIE { get; set; }
        public int I_ARTILLERIE_REEL { get; set; }
        public int I_ARTILLERIE_INITIALE { get; set; }
        public int I_FATIGUE { get; set; }
        public int I_FATIGUE_REEL { get; set; }
        public int I_MORAL { get; set; }
        public int I_MORAL_REEL { get; set; }
        public int I_MORAL_MAX { get; set; }
        public int I_EXPERIENCE { get; set; }
        public int I_TACTIQUE { get; set; }
        public bool B_QG { get; set; }
        public int I_STRATEGIQUE { get; set; }
        public char C_NIVEAU_HIERARCHIQUE  { get; set; }
        public int I_MATERIEL { get; set; }
        public int I_RAVITAILLEMENT { get; set; }
        public int I_NIVEAU_FORTIFICATION { get; set; }
        public int I_RETRAITE { get; set; }
        public int ID_BATAILLE { get; set; }
        public int I_ZONE_BATAILLE { get; set; }
        public string S_POSITION { get; set; }
        public bool B_DETRUIT { get; set; }
        public bool B_FUITE_AU_COMBAT { get; set; }
        public bool B_REDITION_RAVITAILLEMENT { get; set; }
        public bool B_DEPOT { get; set; }
        public char? C_NIVEAU_DEPOT { get; set; }
        public int I_TOUR_CONVOI_CREE { get; set; }
        public int I_SOLDATS_RAVITAILLES { get; set; }
        public int ID_DEPOT_SOURCE { get; set; }
        public bool B_CAVALERIE_DE_LIGNE { get; set; }
        public bool B_CAVALERIE_LOURDE { get; set; }
        public bool B_GARDE { get; set; }
        public bool B_VIEILLE_GARDE { get; set; }
        public bool B_PONTONNIER { get; set; }
        public bool B_CONVOI { get; set; }
        public bool B_RENFORT { get; set; }
        public bool B_BLESSES { get; set; }
        public bool B_PRISONNIERS { get; set; }
        public int I_PATROUILLES_DISPONIBLES { get; set; }
        public int I_PATROUILLES_MAX { get; set; }
        public decimal I_VITESSE  { get; set; }
        public int I_X { get; set; }
        public int I_Y { get; set; }
        public string S_ORDRE_COURANT { get; set; }
        public int I_TRI { get; set; }
    }
}
