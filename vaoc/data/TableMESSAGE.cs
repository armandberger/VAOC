using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WaocLib;

namespace vaoc
{
    [Serializable]
    class TableMESSAGE
    {
        private List<LigneMESSAGE> liste;
        private Hashtable indexID;

        public TableMESSAGE()
        {
            liste = new List<LigneMESSAGE>();
            indexID = new Hashtable();
        }

        public bool Importer(Donnees.TAB_MESSAGEDataTable tableXML)
        {
            Vider();
            foreach (Donnees.TAB_MESSAGERow ligneXML in tableXML)
            {
                Ajouter(ligneXML);
            }
            return true;
        }

        public bool Exporter(Donnees.TAB_MESSAGEDataTable tableXML)
        {
            tableXML.Clear();
            foreach (LigneMESSAGE ligne in liste)
            {
                Donnees.TAB_MESSAGERow ligneXML = tableXML.AddTAB_MESSAGERow(
                    ligne.ID_MESSAGE,
                    ligne.ID_PION_EMETTEUR,
                    ligne.ID_PION_PROPRIETAIRE,
                    ligne.I_TYPE,
                    ligne.I_TOUR_ARRIVEE,
                    ligne.I_PHASE_ARRIVEE,
                    ligne.I_TOUR_DEPART,
                    ligne.I_PHASE_DEPART,
                    ligne.S_TEXTE,
                    ligne.I_INFANTERIE,
                    ligne.I_CAVALERIE,
                    ligne.I_ARTILLERIE,
                    ligne.I_FATIGUE,
                    ligne.I_MORAL,
                    ligne.I_TOUR_SANS_RAVITAILLEMENT,
                    ligne.ID_BATAILLE,
                    ligne.I_ZONE_BATAILLE,
                    ligne.I_RETRAITE,
                    ligne.B_DETRUIT,
                    ligne.ID_CASE,
                    ligne.ID_CASE_DEBUT,
                    ligne.ID_CASE_FIN,
                    ligne.I_NB_PHASES_MARCHE_JOUR,
                    ligne.I_NB_PHASES_MARCHE_NUIT,
                    ligne.I_NB_HEURES_COMBAT,
                    ligne.I_MATERIEL,
                    ligne.I_RAVITAILLEMENT,
                    ligne.I_SOLDATS_RAVITAILLES,
                    ligne.I_NB_HEURES_FORTIFICATION,
                    ligne.I_NIVEAU_FORTIFICATION,
                    ligne.I_DUREE_HORS_COMBAT,
                    ligne.I_TOUR_BLESSURE,
                    ligne.C_NIVEAU_DEPOT
                //    ligne.ID_MESSAGE_TRANSMIS ?? -1,
                    );
                //if (!ligne.ID_MESSAGE_TRANSMIS.HasValue) { ligneXML.SetID_MESSAGE_TRANSMISNull(); }
            }
            return true;
        }

        /// <summary>
        /// Crée une nouveau  message avec son index, reste à faire ensuite à remplir les valeurs
        /// </summary>
        /// <returns>nouveau message </returns>
        public LigneMESSAGE Nouveau()
        {
            LigneMESSAGE retour = new LigneMESSAGE();
            retour.ID_MESSAGE = ProchainID_MESSAGE;
            Ajouter(retour);
            return retour;
        }

        public int ProchainID_MESSAGE
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

        public void Ajouter(Donnees.TAB_MESSAGERow ligneRow)
        {
            LigneMESSAGE ligne = new LigneMESSAGE(ligneRow);
            Ajouter(ligne);
        }

        public void Ajouter(LigneMESSAGE ligne)
        {
            liste.Add(ligne);
            indexID.Add(ligne.ID_MESSAGE, ligne);
        }

        public void Supprimer(LigneMESSAGE ligne)
        {
            indexID.Remove(ligne.ID_MESSAGE);
            liste.Remove(ligne);
        }

        public void Vider()
        {
            indexID.Clear();
            liste.Clear();
        }

        public LigneMESSAGE this[int i]
        {
            get
            {
                return liste[i];
            }
        }

        public LigneMESSAGE TrouveParID_MESSAGE(int id)
        {
            return (LigneMESSAGE)indexID[id];
        }
    }
}
