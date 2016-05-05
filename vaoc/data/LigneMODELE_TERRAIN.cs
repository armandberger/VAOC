using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vaoc
{
    class LigneMODELE_TERRAIN
    {
        public int ID_MODELE_TERRAIN { get; set; }
        public string S_NOM { get; set; }
        public int ID_GRAPHIQUE { get; set; }
        public int ID_MODELE_NOUVEAU_TERRAIN { get; set; }
        public int I_MODIFICATEUR_DEFENSE { get; set; }
        public bool B_ANNULEE_SI_OCCUPEE { get; set; }
        public bool B_CIRCUIT_ROUTIER { get; set; }
        public bool B_OBSTACLE_DEFENSIF { get; set; }
        public bool B_ANNULEE_EN_COMBAT { get; set; }
        public bool B_PONT { get; set; }
        public bool B_PONTON { get; set; }
        public bool B_DETRUIT { get; set; }

        public LigneMODELE_TERRAIN(Donnees.TAB_MODELE_TERRAINRow ligneXML)
        {
            ID_MODELE_TERRAIN = ligneXML.ID_MODELE_TERRAIN;
            S_NOM = ligneXML.S_NOM;
            ID_GRAPHIQUE = ligneXML.ID_GRAPHIQUE;
            ID_MODELE_NOUVEAU_TERRAIN = ligneXML.ID_MODELE_NOUVEAU_TERRAIN;
            I_MODIFICATEUR_DEFENSE = ligneXML.I_MODIFICATEUR_DEFENSE;
            B_ANNULEE_SI_OCCUPEE = ligneXML.B_ANNULEE_SI_OCCUPEE;
            B_CIRCUIT_ROUTIER = ligneXML.B_CIRCUIT_ROUTIER;
            B_OBSTACLE_DEFENSIF = ligneXML.B_OBSTACLE_DEFENSIF;
            B_ANNULEE_EN_COMBAT = ligneXML.B_ANNULEE_EN_COMBAT;
            B_PONT = ligneXML.B_PONT;
            B_PONTON = ligneXML.B_PONTON;
            B_DETRUIT = ligneXML.B_DETRUIT;
        }
    }
}
