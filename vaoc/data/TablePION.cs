using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vaoc
{
    class TablePION : IEnumerable<LignePION>
    {
        private List<LignePION> liste;
        private Hashtable indexID;

        public IEnumerator<LignePION> GetEnumerator()
        {
            return liste.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return liste.GetEnumerator();
        }

        public bool Importer(Donnees.TAB_PIONDataTable tableXML)
        {
            Vider();
            foreach (Donnees.TAB_PIONRow ligneXML in tableXML)
            {
                Ajouter(ligneXML);
            }
            return true;
        }

        public bool Exporter(Donnees.TAB_PIONDataTable tableXML)
        {
            tableXML.Clear();
            foreach (LignePION ligne in liste)
            {
                Donnees.TAB_PIONRow ligneXML = tableXML.AddTAB_PIONRow(ligne.ID_PION,
                    ligne.ID_PION_PROPRIETAIRE ?? -1,
                    ligne.ID_NOUVEAU_PION_PROPRIETAIRE ?? -1,
                    ligne.ID_ANCIEN_PION_PROPRIETAIRE ?? -1,
                    ligne.S_NOM,
                    ligne.I_INFANTERIE,
                    ligne.I_INFANTERIE_INITIALE,
                    ligne.I_CAVALERIE,
                    ligne.I_CAVALERIE_INITIALE,
                    ligne.I_ARTILLERIE,
                    ligne.I_ARTILLERIE_INITIALE,
                    ligne.I_FATIGUE,
                    ligne.I_MORAL,
                    ligne.I_MORAL_MAX,
                    ligne.I_EXPERIENCE,
                    ligne.I_TACTIQUE,
                    ligne.I_STRATEGIQUE,
                    ligne.C_NIVEAU_HIERARCHIQUE,
                    ligne.I_DISTANCE_A_PARCOURIR,
                    ligne.I_NB_PHASES_MARCHE_JOUR,
                    ligne.I_NB_PHASES_MARCHE_NUIT,
                    ligne.I_NB_HEURES_COMBAT,
                    ligne.ID_CASE,
                    ligne.I_TOUR_SANS_RAVITAILLEMENT ?? -1,
                    ligne.ID_BATAILLE ?? -1,
                    ligne.I_ZONE_BATAILLE ?? -1,
                    ligne.I_TOUR_RETRAITE_RESTANT ?? -1,
                    ligne.I_TOUR_FUITE_RESTANT ?? -1,
                    ligne.B_DETRUIT,
                    ligne.B_FUITE_AU_COMBAT,
                    ligne.B_INTERCEPTION,
                    ligne.B_REDITION_RAVITAILLEMENT,
                    ligne.B_TELEPORTATION,
                    ligne.B_ENNEMI_OBSERVABLE,
                    ligne.I_MATERIEL,
                    ligne.I_RAVITAILLEMENT,
                    ligne.B_CAVALERIE_DE_LIGNE,
                    ligne.B_CAVALERIE_LOURDE,
                    ligne.B_GARDE,
                    ligne.B_VIEILLE_GARDE,
                    ligne.I_TOUR_CONVOI_CREE ?? -1,
                    ligne.ID_DEPOT_SOURCE ?? -1,
                    ligne.I_SOLDATS_RAVITAILLES ?? -1,
                    ligne.I_NB_HEURES_FORTIFICATION ?? -1,
                    ligne.I_NIVEAU_FORTIFICATION ?? -1,
                    ligne.ID_PION_REMPLACE ?? -1,
                    ligne.I_DUREE_HORS_COMBAT ?? -1, 
                    ligne.I_TOUR_BLESSURE ?? -1, 
                    ligne.B_BLESSES, 
                    ligne.B_PRISONNIERS, 
                    ligne.B_RENFORT, 
                    ligne.ID_LIEU_RATTACHEMENT ?? -1, 
                    ligne.C_NIVEAU_DEPOT ?? ' ', 
                    ligne.ID_PION_ESCORTE ?? -1,
                    ligne.I_INFANTERIE_ESCORTE ?? -1, 
                    ligne.I_CAVALERIE_ESCORTE ?? -1, 
                    ligne.I_MATERIEL_ESCORTE ?? -1);

                if (!ligne.ID_ANCIEN_PION_PROPRIETAIRE.HasValue) { ligneXML.SetID_ANCIEN_PION_PROPRIETAIRENull(); }
                if (!ligne.ID_NOUVEAU_PION_PROPRIETAIRE.HasValue) { ligneXML.SetID_NOUVEAU_PION_PROPRIETAIRENull(); }
                if (!ligne.ID_ANCIEN_PION_PROPRIETAIRE.HasValue) { ligneXML.SetID_ANCIEN_PION_PROPRIETAIRENull(); }
                if (!ligne.ID_BATAILLE.HasValue) { ligneXML.SetID_BATAILLENull(); }
                if (!ligne.I_ZONE_BATAILLE.HasValue) { ligneXML.SetI_ZONE_BATAILLENull(); }
                if (!ligne.I_TOUR_RETRAITE_RESTANT.HasValue) { ligneXML.SetI_TOUR_RETRAITE_RESTANTNull(); }
                if (!ligne.I_TOUR_FUITE_RESTANT.HasValue) { ligneXML.SetI_TOUR_FUITE_RESTANTNull(); }
                if (!ligne.I_TOUR_CONVOI_CREE.HasValue) { ligneXML.SetI_TOUR_CONVOI_CREENull(); }
                if (!ligne.ID_DEPOT_SOURCE.HasValue) { ligneXML.SetID_DEPOT_SOURCENull(); }
                if (!ligne.I_SOLDATS_RAVITAILLES.HasValue) { ligneXML.SetI_SOLDATS_RAVITAILLESNull(); }
                if (!ligne.I_NB_HEURES_FORTIFICATION.HasValue) { ligneXML.SetI_NB_HEURES_FORTIFICATIONNull(); }
                if (!ligne.I_NIVEAU_FORTIFICATION.HasValue) { ligneXML.SetI_NIVEAU_FORTIFICATIONNull(); }
                if (!ligne.ID_PION_REMPLACE.HasValue) { ligneXML.SetID_PION_REMPLACENull(); }
                if (!ligne.I_DUREE_HORS_COMBAT.HasValue) { ligneXML.SetI_DUREE_HORS_COMBATNull(); }
                if (!ligne.I_TOUR_BLESSURE.HasValue) { ligneXML.SetI_TOUR_BLESSURENull(); }
                if (!ligne.ID_LIEU_RATTACHEMENT.HasValue) { ligneXML.SetID_LIEU_RATTACHEMENTNull(); }
                if (!ligne.C_NIVEAU_DEPOT.HasValue) { ligneXML.SetC_NIVEAU_DEPOTNull(); }
                if (!ligne.ID_PION_ESCORTE.HasValue) { ligneXML.SetID_PION_ESCORTENull(); }
                if (!ligne.I_INFANTERIE_ESCORTE.HasValue) { ligneXML.SetI_INFANTERIE_ESCORTENull(); }
                if (!ligne.I_CAVALERIE_ESCORTE.HasValue) { ligneXML.SetI_CAVALERIE_ESCORTENull(); }
                if (!ligne.I_MATERIEL_ESCORTE.HasValue) { ligneXML.SetI_MATERIEL_ESCORTENull(); }
            }
            return true;
        }

        public TablePION()
        {
            liste = new List<LignePION>();
            indexID = new Hashtable();
        }
        
        public void Ajouter(Donnees.TAB_PIONRow ligneRow)
        {
            LignePION ligne = new LignePION(ligneRow);
            Ajouter(ligne);
        }

        public void Ajouter(LignePION ligne)
        {
            liste.Add(ligne);
            indexID.Add(ligne.ID_PION, ligne);
        }

        public void Supprimer(LignePION ligne)
        {
            indexID.Remove(ligne.ID_PION);
            liste.Remove(ligne);
        }

        public void Vider()
        {
            indexID.Clear();
            liste.Clear();
        }

        public LignePION this[int i]
        {
            get
            {
                //return (LigneCASE)(liste.GetByIndex(i));
                return liste[i];
            }
        }

        public LignePION TrouveParID_PION(int id)
        {
            return (LignePION)indexID[id];
        }

    }
}
