using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vaoc
{
    [Serializable]
    class LignePARTIE
    {
        public int ID_PARTIE { get; set; }
        public int ID_JEU { get; set; }
        public string S_NOM { get; set; }
        public int I_TOUR { get; set; }
        public int I_PHASE { get; set; }
        public string S_MESSAGE_ARBITRE { get; set; }
        public string S_HOST_COURRIEL { get; set; }
        public string S_HOST_UTILISATEUR { get; set; }
        public string S_HOST_MOTDEPASSE { get; set; }
        public int ID_METEO_INITIALE { get; set; }
        public int ID_METEO { get; set; }
        public int I_LARGEUR_CARTE_ZOOM_WEB { get; set; }
        public int I_HAUTEUR_CARTE_ZOOM_WEB { get; set; }
        public int ID_VICTOIRE { get; set; }
        public bool FL_DEMARRAGE { get; set; }
        public int I_TOUR_NOTIFICATION { get; set; }

        public LignePARTIE(Donnees.TAB_PARTIERow ligneXML)
        {
            ID_PARTIE = ligneXML.ID_PARTIE;
            ID_JEU = ligneXML.ID_JEU;
            S_NOM = ligneXML.S_NOM;
            I_TOUR = ligneXML.I_TOUR;
            I_PHASE = ligneXML.I_PHASE;
            S_MESSAGE_ARBITRE = ligneXML.S_MESSAGE_ARBITRE;
            S_HOST_COURRIEL = ligneXML.S_HOST_COURRIEL;
            S_HOST_UTILISATEUR = ligneXML.S_HOST_UTILISATEUR;
            S_HOST_MOTDEPASSE = ligneXML.S_HOST_MOTDEPASSE;
            ID_METEO_INITIALE = ligneXML.ID_METEO_INITIALE;
            ID_METEO = ligneXML.ID_METEO;
            I_LARGEUR_CARTE_ZOOM_WEB = ligneXML.I_LARGEUR_CARTE_ZOOM_WEB;
            I_HAUTEUR_CARTE_ZOOM_WEB = ligneXML.I_HAUTEUR_CARTE_ZOOM_WEB;
            ID_VICTOIRE = ligneXML.ID_VICTOIRE;
            FL_DEMARRAGE = ligneXML.FL_DEMARRAGE;
            I_TOUR_NOTIFICATION = ligneXML.I_TOUR_NOTIFICATION;
        }
    }
}
