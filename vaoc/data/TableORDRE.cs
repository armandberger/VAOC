using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WaocLib;

namespace vaoc
{
    [Serializable]
    class TableORDRE
    {
        private List<LigneORDRE> liste;
        private Hashtable indexID;

        public TableORDRE()
        {
            liste = new List<LigneORDRE>();
            indexID = new Hashtable();
        }

        public bool Importer(Donnees.TAB_ORDREDataTable tableXML)
        {
            Vider();
            foreach (Donnees.TAB_ORDRERow ligneXML in tableXML)
            {
                Ajouter(ligneXML);
            }
            return true;
        }

        public bool Exporter(Donnees.TAB_ORDREDataTable tableXML)
        {
            tableXML.Clear();
            foreach (LigneORDRE ligne in liste)
            {
                Donnees.TAB_ORDRERow ligneXML = tableXML.AddTAB_ORDRERow(
                    //ligne.ID_ORDRE,
                    ligne.ID_ORDRE_TRANSMIS ?? -1,
                    ligne.ID_ORDRE_SUIVANT ?? -1,
                    ligne.ID_ORDRE_WEB,
                    ligne.I_ORDRE_TYPE,
                    ligne.ID_PION,
                    ligne.ID_CASE_DEPART,
                    ligne.I_EFFECTIF_DEPART,
                    ligne.ID_CASE_DESTINATION,
                    ligne.ID_NOM_DESTINATION,
                    ligne.I_EFFECTIF_DESTINATION,
                    ligne.I_TOUR_DEBUT,
                    ligne.I_PHASE_DEBUT,
                    ligne.I_TOUR_FIN ?? -1,
                    ligne.I_PHASE_FIN ?? -1,
                    ligne.ID_MESSAGE ?? -1,
                    ligne.ID_DESTINATAIRE,
                    ligne.ID_CIBLE ?? -1,
                    ligne.ID_DESTINATAIRE_CIBLE ?? -1,
                    ligne.ID_BATAILLE ?? -1,
                    ligne.I_ZONE_BATAILLE ?? -1,
                    ligne.I_HEURE_DEBUT,
                    ligne.I_DUREE,
                    ligne.I_ENGAGEMENT
                    );
                if (!ligne.ID_ORDRE_TRANSMIS.HasValue) { ligneXML.SetID_ORDRE_TRANSMISNull(); }
                if (!ligne.ID_ORDRE_SUIVANT.HasValue) { ligneXML.SetID_ORDRE_SUIVANTNull(); }
                if (!ligne.I_TOUR_FIN.HasValue) { ligneXML.SetI_TOUR_FINNull(); }
                if (!ligne.I_PHASE_FIN.HasValue) { ligneXML.SetI_PHASE_FINNull(); }
                if (!ligne.ID_MESSAGE.HasValue) { ligneXML.SetID_MESSAGENull(); }
                if (!ligne.ID_CIBLE.HasValue) { ligneXML.SetID_CIBLENull(); }
                if (!ligne.ID_DESTINATAIRE_CIBLE.HasValue) { ligneXML.SetID_DESTINATAIRE_CIBLENull(); }
                if (!ligne.ID_BATAILLE.HasValue) { ligneXML.SetID_BATAILLENull(); }
                if (!ligne.I_ZONE_BATAILLE.HasValue) { ligneXML.SetI_ZONE_BATAILLENull(); }
            }
            return true;
        }

        /// <summary>
        /// Crée une nouveau element avec son index, reste à faire ensuite à remplir les valeurs
        /// </summary>
        /// <returns>nouveau element </returns>
        public LigneORDRE Nouveau()
        {
            LigneORDRE retour = new LigneORDRE();
            retour.ID_ORDRE = ProchainID_ORDRE;
            Ajouter(retour);
            return retour;
        }

        public void Ajouter(Donnees.TAB_ORDRERow ligneRow)
        {
            LigneORDRE ligne = new LigneORDRE(ligneRow);
            Ajouter(ligne);
        }

        public void Ajouter(LigneORDRE ligne)
        {
            liste.Add(ligne);
            indexID.Add(ligne.ID_ORDRE, ligne);
        }

        public void Supprimer(LigneORDRE ligne)
        {
            indexID.Remove(ligne.ID_ORDRE);
            liste.Remove(ligne);
        }

        public void Vider()
        {
            indexID.Clear();
            liste.Clear();
        }

        public LigneORDRE this[int i]
        {
            get
            {
                return liste[i];
            }
        }

        public int ProchainID_ORDRE
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
        ///  Renvoi l'ordre correspondant à un identifiant du web
        /// </summary>
        /// <param name="ID_ORDREWEB">identifiant de l'ordre du web</param>
        /// <returns>ordre Web, null si aucun</returns>
        public LigneORDRE OrdreWeb(int ID_ORDREWEB)
        {     
            //string requete = string.Format("ID_ORDRE_WEB={0}", ID_ORDREWEB);
            //TAB_ORDRERow[] resOrdre = (TAB_ORDRERow[])Select(requete);
            //if (0 == resOrdre.Length)
            //{
            //    return null;
            //}
            //return resOrdre[0];

            IEnumerable<LigneORDRE> requete =
                (from ligne in liste
                 where (ligne.ID_ORDRE_WEB == ID_ORDREWEB)
                 select ligne);
            if (0 == requete.Count()) { return null; }
            return requete.ElementAt(0);
        }

        /// <summary>
        ///  Renvoi l'ordre de mouvement affecté à l'unité, null si aucun
        /// </summary>
        /// <param name="ID_PION">Pion recherchant son mouvement</param>
        /// <returns>ordre de mouvement du pion, null si aucun</returns>
        public LigneORDRE Mouvement(int ID_PION)
        {
            //On cherche l'ordre actuellement actif s'il existe, on vérifie qu'il s'agit bien d'un ordre de mouvement

            //string tri = "ID_ORDRE";
            //string requete = string.Format("(ID_PION={0}) AND (I_TOUR_FIN IS NULL) AND (I_PHASE_FIN IS NULL)", ID_PION);
            //TAB_ORDRERow[] resOrdre = (TAB_ORDRERow[])Select(requete, tri);
            //if (0 == resOrdre.Length)
            //{
            //    resOrdre = (TAB_ORDRERow[])Select("ID_PION=31", tri);
            //    return null;
            //}
            //if ((resOrdre[0].I_ORDRE_TYPE == Constantes.ORDRES.MOUVEMENT) || (resOrdre[0].I_ORDRE_TYPE == Constantes.ORDRES.MESSAGE) || (resOrdre[0].I_ORDRE_TYPE == Constantes.ORDRES.PATROUILLE))
            //{
            //    return resOrdre[0];
            //}
            //return null;//l'ordre actif n'est pas un ordre de mouvement
            IEnumerable<LigneORDRE> requete =
                (from ligne in liste
                 where ((ligne.ID_PION == ID_PION) && ligne.I_TOUR_FIN.HasValue && ligne.I_PHASE_FIN.HasValue)
                 select ligne).OrderBy(ligne => ligne.ID_ORDRE);
            if (0 == requete.Count()) { return null; }
            LigneORDRE ligneOrdre = requete.ElementAt(0);
            if ((ligneOrdre.I_ORDRE_TYPE == Constantes.ORDRES.MOUVEMENT) || (ligneOrdre.I_ORDRE_TYPE == Constantes.ORDRES.MESSAGE) || (ligneOrdre.I_ORDRE_TYPE == Constantes.ORDRES.PATROUILLE))
            {
                return ligneOrdre;
            }
            return null;//l'ordre actif n'est pas un ordre de mouvement
        }

        /// <summary>
        ///  Renvoi l'ordre courant affecté à l'unité, null si aucun
        /// </summary>
        /// <param name="ID_PION">identifiant du pion sur lequel on cherche l'ordre</param>
        /// <returns>ordre courant du pion, null si aucun</returns>
        public LigneORDRE Courant(int ID_PION)
        {
            //string requete = string.Format("ID_PION={0} AND I_TOUR_FIN IS NULL AND I_PHASE_FIN IS NULL", ID_PION);
            //string tri = "ID_ORDRE";
            //TAB_ORDRERow[] resOrdre = (TAB_ORDRERow[])Select(requete, tri);
            //if (0 == resOrdre.Length)
            //{
            //    return null;
            //}
            //return resOrdre[0];

            IEnumerable<LigneORDRE> requete =
                (from ligne in liste
                 where ((ligne.ID_PION == ID_PION) && ligne.I_TOUR_FIN.HasValue && ligne.I_PHASE_FIN.HasValue)
                 select ligne).OrderBy(ligne => ligne.ID_ORDRE);
            if (0 == requete.Count()) { return null; }
            return requete.ElementAt(0); 
        }

        public LigneORDRE TrouveParID_ORDRE(int id)
        {
            return (LigneORDRE)indexID[id];
        }
    }
}
