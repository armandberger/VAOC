using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vaoc
{
    class ClassDataOrdre
    {
        public int ID_ORDRE { get; set; }
        public int ID_ORDRE_SUIVANT { get; set; }
        public int ID_PION { get; set; }
        /// <summary>
        /// identifianty du pion destinataire de l'ordre
        /// </summary>
        public int ID_PION_DESTINATAIRE { get; set; }
        public int ID_PARTIE { get; set; }
        public int I_TOUR { get; set; }
        public int I_TYPE { get; set; }
        public string S_MESSAGE { get; set; }
        public int I_DISTANCE { get; set; }
        public int I_DIRECTION { get; set; }
        public int ID_NOM_LIEU { get; set; }
        public int I_HEURE { get; set; }
        public int I_DUREE { get; set; }
        public int I_DISTANCE_DESTINATAIRE { get; set; }
        public int I_DIRECTION_DESTINATAIRE { get; set; }
        public int ID_NOM_LIEU_DESTINATAIRE { get; set; }
        public int ID_BATAILLE { get; set; }
        public int I_ZONE_BATAILLE { get; set; }
        /// <summary>
        /// Pion cible de l'ordre
        /// par exemple pour un transfert, l'ordre est envoyé par ID_PION vers ID_PION_DESTINATAIRE pour transferer ID_PION_CIBLE a ID_PION_DESTINATAIRE_CIBLE
        /// </summary>
        public int ID_PION_CIBLE { get; set; }
        /// <summary>
        /// Pion cible de l'ordre
        /// par exemple pour un transfert, l'ordre est envoyé par ID_PION vers ID_PION_DESTINATAIRE pour transferer ID_PION_CIBLE a ID_PION_DESTINATAIRE_CIBLE
        /// </summary>
        public int ID_PION_DESTINATAIRE_CIBLE { get; set; }
        public int I_ENGAGEMENT { get; set; }
    }
}
