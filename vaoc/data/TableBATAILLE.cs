using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WaocLib;

namespace vaoc
{
    [Serializable]
    class TableBATAILLE
    {
        private List<LigneBATAILLE> liste;
        private Hashtable indexID;

        public TableBATAILLE()
        {
            liste = new List<LigneBATAILLE>();
            indexID = new Hashtable();
        }

        public bool Importer(Donnees.TAB_BATAILLEDataTable tableXML)
        {
            Vider();
            foreach (Donnees.TAB_BATAILLERow ligneXML in tableXML)
            {
                Ajouter(ligneXML);
            }
            return true;
        }

        public bool Exporter(Donnees.TAB_BATAILLEDataTable tableXML)
        {
            tableXML.Clear();
            foreach (LigneBATAILLE ligne in liste)
            {
                Donnees.TAB_BATAILLERow ligneXML = tableXML.AddTAB_BATAILLERow(
                    ligne.ID_BATAILLE,
                    ligne.S_NOM,
                    ligne.I_TOUR_DEBUT,
                    ligne.I_PHASE_DEBUT,
                    ligne.I_TOUR_FIN,
                    ligne.I_PHASE_FIN,
                    ligne.C_ORIENTATION,
                    ligne.ID_TERRAIN_0,
                    ligne.ID_TERRAIN_1,
                    ligne.ID_TERRAIN_2,
                    ligne.ID_TERRAIN_3,
                    ligne.ID_TERRAIN_4,
                    ligne.ID_TERRAIN_5,
                    ligne.ID_OBSTACLE_03,
                    ligne.ID_OBSTACLE_14,
                    ligne.ID_OBSTACLE_25,
                    ligne.ID_NATION_012,
                    ligne.ID_NATION_345,
                    ligne.ID_LEADER_012,
                    ligne.ID_LEADER_345,
                    ligne.I_X_CASE_HAUT_GAUCHE,
                    ligne.I_Y_CASE_HAUT_GAUCHE,
                    ligne.I_X_CASE_BAS_DROITE,
                    ligne.I_Y_CASE_BAS_DROITE,
                    ligne.I_ENGAGEMENT_0,
                    ligne.I_ENGAGEMENT_1,
                    ligne.I_ENGAGEMENT_2,
                    ligne.I_ENGAGEMENT_3,
                    ligne.I_ENGAGEMENT_4,
                    ligne.I_ENGAGEMENT_5,
                    ligne.S_COMBAT_0,
                    ligne.S_COMBAT_1,
                    ligne.S_COMBAT_2,
                    ligne.S_COMBAT_3,
                    ligne.S_COMBAT_4,
                    ligne.S_COMBAT_5
                //    ligne.ID_BATAILLE_TRANSMIS ?? -1,
                    );
                //if (!ligne.ID_BATAILLE_TRANSMIS.HasValue) { ligneXML.SetID_BATAILLE_TRANSMISNull(); }
            }
            return true;
        }

        public int ProchainID_BATAILLE
        {
            get
            {
                /*
                if (liste.Count > 0)
                {
                    System.Nullable<int> maxIdOrdre =
                        (from ordre in this
                         select ordre.ID_ORDRE)
                        .Max();
                    return (int)maxIdOrdre;
                }
                return 0;
                */
                return liste.Count;
            }
        }

        /// <summary>
        /// Crée une nouvelle bataille avec son index, reste à faire ensuite à remplir les valeurs
        /// </summary>
        /// <returns>nouvelle bataille </returns>
        public LigneBATAILLE Nouveau()
        {
            LigneBATAILLE retour = new LigneBATAILLE();
            retour.ID_BATAILLE = ProchainID_BATAILLE;
            Ajouter(retour);
            return retour;
        }

        public void Ajouter(Donnees.TAB_BATAILLERow ligneRow)
        {
            LigneBATAILLE ligne = new LigneBATAILLE(ligneRow);
            Ajouter(ligne);
        }

        public void Ajouter(LigneBATAILLE ligne)
        {
            liste.Add(ligne);
            indexID.Add(ligne.ID_BATAILLE, ligne);
        }

        public void Supprimer(LigneBATAILLE ligne)
        {
            indexID.Remove(ligne.ID_BATAILLE);
            liste.Remove(ligne);
        }

        public void Vider()
        {
            indexID.Clear();
            liste.Clear();
        }

        public LigneBATAILLE this[int i]
        {
            get
            {
                return liste[i];
            }
        }

        public LigneBATAILLE TrouveParID_BATAILLE(int id)
        {
            return (LigneBATAILLE)indexID[id];
        }
    }
}
