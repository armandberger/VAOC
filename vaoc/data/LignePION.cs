using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vaoc
{
    [Serializable]
    class LignePION
    {
        public int ID_PION { get; set; }
        public int? ID_MODELE_PION { get; set; }
        public int? ID_PION_PROPRIETAIRE { get; set; }
        public int? ID_NOUVEAU_PION_PROPRIETAIRE { get; set; }
        public int? ID_ANCIEN_PION_PROPRIETAIRE { get; set; }
        public string S_NOM { get; set; }
        public int I_INFANTERIE { get; set; }
        public int I_INFANTERIE_INITIALE { get; set; }
        public int I_CAVALERIE { get; set; }
        public int I_CAVALERIE_INITIALE { get; set; }
        public int I_ARTILLERIE { get; set; }
        public int I_ARTILLERIE_INITIALE { get; set; }
        public int I_FATIGUE { get; set; }
        public int I_MORAL { get; set; }
        public int I_MORAL_MAX { get; set; }
        public decimal I_EXPERIENCE { get; set; }
        public int I_TACTIQUE { get; set; }
        public int I_STRATEGIQUE { get; set; }
        public char C_NIVEAU_HIERARCHIQUE { get; set; }
        public decimal I_DISTANCE_A_PARCOURIR { get; set; }
        public int I_NB_PHASES_MARCHE_JOUR { get; set; }
        public int I_NB_PHASES_MARCHE_NUIT { get; set; }
        public int I_NB_HEURES_COMBAT { get; set; }
        public int ID_CASE { get; set; }
        public int? I_TOUR_SANS_RAVITAILLEMENT { get; set; }
        public int? ID_BATAILLE { get; set; }
        public int? I_ZONE_BATAILLE { get; set; }
        public int? I_TOUR_RETRAITE_RESTANT { get; set; }
        public int? I_TOUR_FUITE_RESTANT { get; set; }
        public bool B_DETRUIT { get; set; }
        public bool B_FUITE_AU_COMBAT { get; set; }
        public bool B_INTERCEPTION { get; set; }
        public bool B_REDITION_RAVITAILLEMENT { get; set; }
        public bool B_TELEPORTATION { get; set; }
        public bool B_ENNEMI_OBSERVABLE { get; set; }
        public int I_MATERIEL { get; set; }
        public int I_RAVITAILLEMENT { get; set; }
        public bool B_CAVALERIE_DE_LIGNE { get; set; }
        public bool B_CAVALERIE_LOURDE { get; set; }
        public bool B_GARDE { get; set; }
        public bool B_VIEILLE_GARDE { get; set; }
        public int? I_TOUR_CONVOI_CREE { get; set; }
        public int? ID_DEPOT_SOURCE { get; set; }
        public int? I_SOLDATS_RAVITAILLES { get; set; }
        public int? I_NB_HEURES_FORTIFICATION { get; set; }
        public int? I_NIVEAU_FORTIFICATION { get; set; }
        public int? ID_PION_REMPLACE { get; set; }
        public int? I_DUREE_HORS_COMBAT { get; set; }
        public int? I_TOUR_BLESSURE { get; set; }
        public bool B_BLESSES { get; set; }
        public bool B_PRISONNIERS { get; set; }
        public bool B_RENFORT { get; set; }
        public int? ID_LIEU_RATTACHEMENT { get; set; }
        public char? C_NIVEAU_DEPOT { get; set; }
        public int? ID_PION_ESCORTE { get; set; }
        public int? I_INFANTERIE_ESCORTE { get; set; }
        public int? I_CAVALERIE_ESCORTE { get; set; }
        public int? I_MATERIEL_ESCORTE { get; set; }

        public LignePION(Donnees.TAB_PIONRow ligneRow)
        {
            ID_PION = ligneRow.ID_PION;
            if (ligneRow.IsID_MODELE_PIONNull()) { ID_MODELE_PION = null; } else { ID_MODELE_PION = ligneRow.ID_MODELE_PION; }
            if (ligneRow.IsID_PION_PROPRIETAIRENull()) { ID_PION_PROPRIETAIRE = null; } else { ID_PION_PROPRIETAIRE = ligneRow.ID_PION_PROPRIETAIRE; }
            if (ligneRow.IsID_NOUVEAU_PION_PROPRIETAIRENull()) { ID_NOUVEAU_PION_PROPRIETAIRE = null; } else { ID_NOUVEAU_PION_PROPRIETAIRE = ligneRow.ID_NOUVEAU_PION_PROPRIETAIRE; }
            if (ligneRow.IsID_ANCIEN_PION_PROPRIETAIRENull()) { ID_ANCIEN_PION_PROPRIETAIRE = null; } else { ID_ANCIEN_PION_PROPRIETAIRE = ligneRow.ID_ANCIEN_PION_PROPRIETAIRE; }
            if (ligneRow.IsS_NOMNull()) { S_NOM = null; } else { S_NOM = ligneRow.S_NOM; }        
            I_INFANTERIE = ligneRow.I_INFANTERIE;
            I_INFANTERIE_INITIALE = ligneRow.I_INFANTERIE_INITIALE;
            I_CAVALERIE = ligneRow.I_CAVALERIE;
            I_CAVALERIE_INITIALE = ligneRow.I_CAVALERIE_INITIALE;
            I_ARTILLERIE = ligneRow.I_ARTILLERIE;
            I_ARTILLERIE_INITIALE = ligneRow.I_ARTILLERIE_INITIALE;
            I_FATIGUE = ligneRow.I_FATIGUE;
            I_MORAL = ligneRow.I_MORAL;
            I_MORAL_MAX = ligneRow.I_MORAL_MAX;
            I_EXPERIENCE = ligneRow.I_EXPERIENCE;
            I_TACTIQUE = ligneRow.I_TACTIQUE;
            I_STRATEGIQUE = ligneRow.I_STRATEGIQUE;
            C_NIVEAU_HIERARCHIQUE = ligneRow.C_NIVEAU_HIERARCHIQUE;
            I_DISTANCE_A_PARCOURIR = ligneRow.I_DISTANCE_A_PARCOURIR;
            I_NB_PHASES_MARCHE_JOUR = ligneRow.I_NB_PHASES_MARCHE_JOUR;
            I_NB_PHASES_MARCHE_NUIT = ligneRow.I_NB_PHASES_MARCHE_JOUR;
            I_NB_HEURES_COMBAT = ligneRow.I_NB_HEURES_COMBAT;
            ID_CASE = ligneRow.ID_CASE;
            if (ligneRow.IsI_TOUR_SANS_RAVITAILLEMENTNull()) { I_TOUR_SANS_RAVITAILLEMENT = null; } else { I_TOUR_SANS_RAVITAILLEMENT = ligneRow.I_TOUR_SANS_RAVITAILLEMENT; }
            if (ligneRow.IsID_BATAILLENull()) { ID_BATAILLE = null; } else { ID_BATAILLE = ligneRow.ID_BATAILLE; }
            if (ligneRow.IsI_ZONE_BATAILLENull()) { I_ZONE_BATAILLE = null; } else { I_ZONE_BATAILLE = ligneRow.I_ZONE_BATAILLE; }
            if (ligneRow.IsI_TOUR_RETRAITE_RESTANTNull()) { I_TOUR_RETRAITE_RESTANT = null; } else { I_TOUR_RETRAITE_RESTANT = ligneRow.I_TOUR_RETRAITE_RESTANT; }
            if (ligneRow.IsI_TOUR_FUITE_RESTANTNull()) { I_TOUR_FUITE_RESTANT = null; } else { I_TOUR_FUITE_RESTANT = ligneRow.I_TOUR_FUITE_RESTANT; }
            B_DETRUIT = ligneRow.B_DETRUIT;
            B_FUITE_AU_COMBAT = ligneRow.B_FUITE_AU_COMBAT;
            B_INTERCEPTION = ligneRow.B_INTERCEPTION;
            B_REDITION_RAVITAILLEMENT = ligneRow.B_REDITION_RAVITAILLEMENT;
            B_TELEPORTATION = ligneRow.B_TELEPORTATION;
            B_ENNEMI_OBSERVABLE = ligneRow.B_ENNEMI_OBSERVABLE;
            I_MATERIEL =ligneRow.I_MATERIEL;
            I_RAVITAILLEMENT = ligneRow.I_RAVITAILLEMENT;
            B_CAVALERIE_DE_LIGNE = ligneRow.B_CAVALERIE_DE_LIGNE;
            B_CAVALERIE_LOURDE = ligneRow.B_CAVALERIE_LOURDE;
            B_GARDE = ligneRow.B_GARDE;
            B_VIEILLE_GARDE =ligneRow.B_VIEILLE_GARDE;
            if (ligneRow.IsI_TOUR_CONVOI_CREENull()) { I_TOUR_CONVOI_CREE = null; } else { I_TOUR_CONVOI_CREE = ligneRow.I_TOUR_CONVOI_CREE; }
            if (ligneRow.IsID_DEPOT_SOURCENull()) { ID_DEPOT_SOURCE = null; } else { ID_DEPOT_SOURCE = ligneRow.ID_DEPOT_SOURCE; }
            if (ligneRow.IsI_SOLDATS_RAVITAILLESNull()) { I_SOLDATS_RAVITAILLES = null; } else { I_SOLDATS_RAVITAILLES = ligneRow.I_SOLDATS_RAVITAILLES; }
            if (ligneRow.IsI_NB_HEURES_FORTIFICATIONNull()) { I_NB_HEURES_FORTIFICATION = null; } else { I_NB_HEURES_FORTIFICATION = ligneRow.I_NB_HEURES_FORTIFICATION; }
            if (ligneRow.IsI_NIVEAU_FORTIFICATIONNull()) { I_NIVEAU_FORTIFICATION = null; } else { I_NIVEAU_FORTIFICATION = ligneRow.I_NIVEAU_FORTIFICATION; }
            if (ligneRow.IsID_PION_REMPLACENull()) { ID_PION_REMPLACE = null; } else { ID_PION_REMPLACE = ligneRow.ID_PION_REMPLACE; }
            if (ligneRow.IsI_DUREE_HORS_COMBATNull()) { I_DUREE_HORS_COMBAT = null; } else { I_DUREE_HORS_COMBAT = ligneRow.I_DUREE_HORS_COMBAT; }
            if (ligneRow.IsI_TOUR_BLESSURENull()) { I_TOUR_BLESSURE = null; } else { I_TOUR_BLESSURE = ligneRow.I_TOUR_BLESSURE; }
            B_BLESSES = ligneRow.B_BLESSES;
            B_PRISONNIERS = ligneRow.B_PRISONNIERS;
            B_RENFORT = ligneRow.B_RENFORT;
            if (ligneRow.IsID_LIEU_RATTACHEMENTNull()) { ID_LIEU_RATTACHEMENT = null; } else { ID_LIEU_RATTACHEMENT = ligneRow.ID_LIEU_RATTACHEMENT; }
            if (ligneRow.IsC_NIVEAU_DEPOTNull()) { C_NIVEAU_DEPOT = null; } else { C_NIVEAU_DEPOT = ligneRow.C_NIVEAU_DEPOT; }
            if (ligneRow.IsID_PION_ESCORTENull()) { ID_PION_ESCORTE = null; } else { ID_PION_ESCORTE = ligneRow.ID_PION_ESCORTE; }
            if (ligneRow.IsI_INFANTERIE_ESCORTENull()) { I_INFANTERIE_ESCORTE = null; } else { I_INFANTERIE_ESCORTE = ligneRow.I_INFANTERIE_ESCORTE; }
            if (ligneRow.IsI_CAVALERIE_ESCORTENull()) { I_CAVALERIE_ESCORTE = null; } else { I_CAVALERIE_ESCORTE = ligneRow.I_CAVALERIE_ESCORTE; }
            if (ligneRow.IsI_MATERIEL_ESCORTENull()) { I_MATERIEL_ESCORTE = null; } else { I_MATERIEL_ESCORTE = ligneRow.I_MATERIEL_ESCORTE; }
        }

        /// <summary>
        /// renvoie la nation du pion
        /// </summary>
        /// <returns>identifiant de la nation du pion, -1 si non trouvé</returns>
        public int idNation
        {
            get
            {
                Donnees.TAB_MODELE_PIONRow ligneModele = Donnees.m_donnees.TAB_MODELE_PION.FindByID_MODELE_PION((int)ID_MODELE_PION);
                if (null == ligneModele) { return -1; }
                return ligneModele.ID_NATION;
            }
        }
    }
}
