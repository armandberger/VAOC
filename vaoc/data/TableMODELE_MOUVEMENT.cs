using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vaoc
{
    class TableMODELE_MOUVEMENT
    {
        private List<LigneMODELE_MOUVEMENT> liste;
        private Hashtable indexID;

        public bool Importer(Donnees.TAB_MODELE_MOUVEMENTDataTable tableXML)
        {
            Vider();
            foreach (Donnees.TAB_MODELE_MOUVEMENTRow ligneXML in tableXML)
            {
                Ajouter(ligneXML);
            }
            return true;
        }

        public bool Exporter(Donnees.TAB_MODELE_MOUVEMENTDataTable tableXML)
        {
            tableXML.Clear();
            foreach (LigneMODELE_MOUVEMENT ligne in liste)
            {
                Donnees.TAB_MODELE_MOUVEMENTRow ligneXML = tableXML.AddTAB_MODELE_MOUVEMENTRow(
                    ligne.ID_MODELE_MOUVEMENT,
                    ligne.S_NOM,
                    ligne.I_VITESSE_INFANTERIE,
                    ligne.I_VITESSE_CAVALERIE,
                    ligne.I_VITESSE_ARTILLERIE);
                //if (!ligne.ID_ANCIEN_MODELE_MOUVEMENT_PROPRIETAIRE.HasValue) { ligneXML.SetID_ANCIEN_MODELE_MOUVEMENT_PROPRIETAIRENull(); }
            }
            return true;
        }

        public bool Importer(TableMODELE_MOUVEMENT table)
        {
            Vider();
            foreach (LigneMODELE_MOUVEMENT ligne in table.liste)
            {
                Ajouter(ligne);
            }
            return true;
        }

        public TableMODELE_MOUVEMENT()
        {
            liste = new List<LigneMODELE_MOUVEMENT>();
            indexID = new Hashtable();
        }

        public void Ajouter(Donnees.TAB_MODELE_MOUVEMENTRow ligneRow)
        {
            LigneMODELE_MOUVEMENT ligne = new LigneMODELE_MOUVEMENT(ligneRow);
            Ajouter(ligne);
        }

        public void Ajouter(LigneMODELE_MOUVEMENT ligne)
        {
            liste.Add(ligne);
            indexID.Add(ligne.ID_MODELE_MOUVEMENT, ligne);
        }

        public void Supprimer(LigneMODELE_MOUVEMENT ligne)
        {
            indexID.Remove(ligne.ID_MODELE_MOUVEMENT);
            liste.Remove(ligne);
        }

        public void Vider()
        {
            indexID.Clear();
            liste.Clear();
        }

        public LigneMODELE_MOUVEMENT this[int i]
        {
            get
            {
                return liste[i];
            }
        }

        public LigneMODELE_MOUVEMENT TrouveParID_MODELE_MOUVEMENT(int id)
        {
            return (LigneMODELE_MOUVEMENT)indexID[id];
        }

        /// <summary>
        /// calcul de la vitesse minimale de tous les modèles définis
        /// </summary>
        /// <returns>la vitesse minimale définie, -1 si non trouvé</returns>
        public decimal VitesseMinimale()
        {
            decimal vitesse = decimal.MaxValue;
            foreach (LigneMODELE_MOUVEMENT ligneModeleMouvement in liste)
            {
                if (ligneModeleMouvement.I_VITESSE_CAVALERIE > 0)
                {
                    vitesse = Math.Min(vitesse, ligneModeleMouvement.I_VITESSE_CAVALERIE);
                }
                if (ligneModeleMouvement.I_VITESSE_INFANTERIE > 0)
                {
                    vitesse = Math.Min(vitesse, ligneModeleMouvement.I_VITESSE_INFANTERIE);
                }
                if (ligneModeleMouvement.I_VITESSE_ARTILLERIE > 0)
                {
                    vitesse = Math.Min(vitesse, ligneModeleMouvement.I_VITESSE_ARTILLERIE);
                }
            }
            if (vitesse == int.MaxValue)
            {
                vitesse = 1;//vitesse d'un convoi
            }
            return vitesse;
        }

        /// <summary>
        /// calcul de la vitesse maximale de tous les modèles définis
        /// </summary>
        /// <returns>la vitesse maximale définie, -1 si non trouvé</returns>
        public decimal VitesseMaximale()
        {
            decimal vitesse = 1;//vitesse d'un convoi
            foreach (LigneMODELE_MOUVEMENT ligneModeleMouvement in liste)
            {
                if (ligneModeleMouvement.I_VITESSE_CAVALERIE > 0)
                {
                    vitesse = Math.Max(vitesse, ligneModeleMouvement.I_VITESSE_CAVALERIE);
                }
                if (ligneModeleMouvement.I_VITESSE_INFANTERIE > 0)
                {
                    vitesse = Math.Max(vitesse, ligneModeleMouvement.I_VITESSE_INFANTERIE);
                }
                if (ligneModeleMouvement.I_VITESSE_ARTILLERIE > 0)
                {
                    vitesse = Math.Max(vitesse, ligneModeleMouvement.I_VITESSE_ARTILLERIE);
                }
            }
            return vitesse;
        }
    }
}
