using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vaoc
{
    [Serializable]
    class TablePARTIE
    {
        private List<LignePARTIE> liste;

        public TablePARTIE()
        {
            liste = new List<LignePARTIE>();
        }

        public bool Importer(Donnees.TAB_PARTIEDataTable tableXML)
        {
            Vider();
            foreach (Donnees.TAB_PARTIERow ligneXML in tableXML)
            {
                Ajouter(ligneXML);
            }
            return true;
        }

        public bool Exporter(Donnees.TAB_PARTIEDataTable tableXML)
        {
            tableXML.Clear();
            foreach (LignePARTIE ligne in liste)
            {
                Donnees.TAB_PARTIERow ligneXML = tableXML.AddTAB_PARTIERow(ligne.ID_PARTIE,
                    ligne.ID_JEU,
                    ligne.S_NOM,
                    ligne.I_TOUR,
                    ligne.I_PHASE,
                    ligne.S_MESSAGE_ARBITRE,
                    ligne.S_HOST_COURRIEL,
                    ligne.S_HOST_UTILISATEUR,
                    ligne.S_HOST_MOTDEPASSE,
                    ligne.ID_METEO_INITIALE,
                    ligne.ID_METEO,
                    ligne.I_LARGEUR_CARTE_ZOOM_WEB,
                    ligne.I_HAUTEUR_CARTE_ZOOM_WEB,
                    ligne.ID_VICTOIRE,
                    ligne.FL_DEMARRAGE,
                    ligne.I_TOUR_NOTIFICATION
                    );
            }
            return true;
        }

        public void Ajouter(Donnees.TAB_PARTIERow ligneRow)
        {
            LignePARTIE ligne = new LignePARTIE(ligneRow);
            Ajouter(ligne);
        }

        public void Ajouter(LignePARTIE ligne)
        {
            liste.Add(ligne);
        }

        public void Supprimer(LignePARTIE ligne)
        {
            liste.Remove(ligne);
        }

        public void Vider()
        {
            liste.Clear();
        }

        public LignePARTIE this[int i]
        {
            get
            {
                return liste[i];
            }
        }
        /// <summary>
        /// indique s'il fait jour ou nuit
        /// </summary>
        /// <returns>true il fait nuit, false, il fait jour</returns>
        public bool Nocturne()
        {
            return Nocturne(HeureCourante());
        }

        /// <summary>
        /// indique s'il fait jour ou nuit pour une heure donnée
        /// </summary>
        /// <returns>true il fait nuit, false, il fait jour</returns>
        public bool Nocturne(int heure)
        {
            if (heure < BD.Base.Jeu[0].I_LEVER_DU_SOLEIL
                || heure >= BD.Base.Jeu[0].I_COUCHER_DU_SOLEIL)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Renvoie l'heure courante sur une base 0-23
        /// </summary>
        /// <returns>heure en cours</returns>
        public int HeureCourante()
        {
            return HeureBase(liste[0].I_TOUR);
        }

        /// <summary>
        /// Renvoie l'heure courante sur une base 0-23
        /// </summary>
        /// <returns>heure en cours</returns>
        public int HeureBase(int heure)
        {
            return (BD.Base.Jeu[0].I_HEURE_INITIALE + heure) % 24;
        }

        /// <summary>
        /// renvoie le nombre d'heures effectuées de nuit dans un délai
        /// </summary>
        /// <param name="tourDebut">tour du début du délai</param>
        /// <param name="duree">durée du délai</param>
        /// <returns></returns>
        public int DureeNocturne(int tourDebut, int duree)
        {
            int lNocturne = 0;
            //methode de bourrin !
            for (int i = tourDebut; i < duree; i++)
            {
                if (Nocturne(HeureBase(i))) { lNocturne++; }
            }
            return lNocturne;
        }
    }
}
