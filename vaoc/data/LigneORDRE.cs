using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vaoc
{
    [Serializable]
    class LigneORDRE
    {
        public int ID_ORDRE { get; set; }
        public int? ID_ORDRE_TRANSMIS { get; set; }
        public int? ID_ORDRE_SUIVANT { get; set; }
        public int ID_ORDRE_WEB { get; set; }
        public int I_ORDRE_TYPE { get; set; }
        public int ID_PION { get; set; }
        public int ID_CASE_DEPART { get; set; }
        public int I_EFFECTIF_DEPART { get; set; }
        public int ID_CASE_DESTINATION { get; set; }
        public int ID_NOM_DESTINATION { get; set; }
        public int I_EFFECTIF_DESTINATION { get; set; }
        public int I_TOUR_DEBUT { get; set; }
        public int I_PHASE_DEBUT { get; set; }
        public int? I_TOUR_FIN { get; set; }
        public int? I_PHASE_FIN { get; set; }
        public int? ID_MESSAGE { get; set; }
        public int ID_DESTINATAIRE { get; set; }
        public int? ID_CIBLE { get; set; }
        public int? ID_DESTINATAIRE_CIBLE { get; set; }
        public int? ID_BATAILLE { get; set; }
        public int? I_ZONE_BATAILLE { get; set; }
        public int I_HEURE_DEBUT { get; set; }
        public int I_DUREE { get; set; }
        public int I_ENGAGEMENT { get; set; }

        public LigneORDRE() { }

        public LigneORDRE(Donnees.TAB_ORDRERow ligneXML)
        {
            ID_ORDRE = ligneXML.ID_ORDRE;
            if (ligneXML.IsID_ORDRE_TRANSMISNull()) { ID_ORDRE_TRANSMIS = null; } else { ID_ORDRE_TRANSMIS = ligneXML.ID_ORDRE_TRANSMIS; };
            if (ligneXML.IsID_ORDRE_SUIVANTNull()) { ID_ORDRE_SUIVANT = null; } else { ID_ORDRE_SUIVANT = ligneXML.ID_ORDRE_SUIVANT; };
            ID_ORDRE_WEB = ligneXML.ID_ORDRE_WEB;
            I_ORDRE_TYPE = ligneXML.I_ORDRE_TYPE;
            ID_PION = ligneXML.ID_PION;
            ID_CASE_DEPART = ligneXML.ID_CASE_DEPART;
            I_EFFECTIF_DEPART = ligneXML.I_EFFECTIF_DEPART;
            ID_CASE_DESTINATION = ligneXML.ID_CASE_DESTINATION;
            ID_NOM_DESTINATION = ligneXML.ID_NOM_DESTINATION;
            I_EFFECTIF_DESTINATION = ligneXML.I_EFFECTIF_DESTINATION;
            I_TOUR_DEBUT = ligneXML.I_TOUR_DEBUT;
            I_PHASE_DEBUT = ligneXML.I_PHASE_DEBUT;
            if (ligneXML.IsI_TOUR_FINNull()) { I_TOUR_FIN = null; } else { I_TOUR_FIN = ligneXML.I_TOUR_FIN; };
            if (ligneXML.IsI_PHASE_FINNull()) { I_PHASE_FIN = null; } else { I_PHASE_FIN = ligneXML.I_PHASE_FIN; };
            if (ligneXML.IsID_MESSAGENull()) { ID_MESSAGE = null; } else { ID_MESSAGE = ligneXML.ID_MESSAGE; };
            ID_DESTINATAIRE = ligneXML.ID_DESTINATAIRE;
            if (ligneXML.IsID_CIBLENull()) { ID_CIBLE = null; } else { ID_CIBLE = ligneXML.ID_CIBLE; };
            if (ligneXML.IsID_DESTINATAIRE_CIBLENull()) { ID_DESTINATAIRE_CIBLE = null; } else { ID_DESTINATAIRE_CIBLE = ligneXML.ID_DESTINATAIRE_CIBLE; };
            if (ligneXML.IsID_BATAILLENull()) { ID_BATAILLE = null; } else { ID_BATAILLE = ligneXML.ID_BATAILLE; };
            if (ligneXML.IsI_ZONE_BATAILLENull()) { I_ZONE_BATAILLE = null; } else { I_ZONE_BATAILLE = ligneXML.I_ZONE_BATAILLE; };
            I_HEURE_DEBUT = ligneXML.I_HEURE_DEBUT;
            I_DUREE = ligneXML.I_DUREE;
            I_ENGAGEMENT = ligneXML.I_ENGAGEMENT;
        }

        /// <summary>
        /// renvoie l'ordre suivant de l'ordre courant
        /// </summary>
        public LigneORDRE ordreSuivant
        {
            get
            {
                if (!ID_ORDRE_SUIVANT.HasValue || this.ID_ORDRE_SUIVANT < 0) { return null; }
                return BD.Base.Ordre.TrouveParID_ORDRE((int)this.ID_ORDRE_SUIVANT);
            }
        }

        public LignePION cible
        {
            get
            {
                if (!ID_CIBLE.HasValue) { return null; }
                return BD.Base.Pion.TrouveParID_PION((int)ID_CIBLE);
            }
        }

        /// <summary>
        /// renvoie l'ordre transmis par une unité (de messager)
        /// </summary>
        public LigneORDRE ordreTransmis
        {
            get
            {
                if (!ID_ORDRE_TRANSMIS.HasValue) { return null; }
                return BD.Base.Ordre.TrouveParID_ORDRE((int)this.ID_ORDRE_TRANSMIS);
            }
        }

        /// <summary>
        /// renvoie le message transmis par une unité (de messager)
        /// </summary>
        public LigneMESSAGE messageTransmis
        {
            get
            {
                if (!ID_MESSAGE.HasValue) { return null; }
                return BD.Base.Message.TrouveParID_MESSAGE((int)ID_MESSAGE);
            }
        }
    }
}
