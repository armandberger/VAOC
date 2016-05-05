using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vaoc
{
    [Serializable]
    class TableMODELE_TERRAIN : IEnumerable<LigneMODELE_TERRAIN>
    {
        private List<LigneMODELE_TERRAIN> liste;

        public TableMODELE_TERRAIN()
        {
            liste = new List<LigneMODELE_TERRAIN>();
        }

        public IEnumerator<LigneMODELE_TERRAIN> GetEnumerator()
        {
            return liste.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return liste.GetEnumerator();
        }

        public bool Importer(Donnees.TAB_MODELE_TERRAINDataTable tableXML)
        {
            Vider();
            foreach (Donnees.TAB_MODELE_TERRAINRow ligneXML in tableXML)
            {
                Ajouter(ligneXML);
            }
            return true;
        }

        public bool Exporter(Donnees.TAB_MODELE_TERRAINDataTable tableXML)
        {
            tableXML.Clear();
            foreach (LigneMODELE_TERRAIN ligne in liste)
            {
                Donnees.TAB_MODELE_TERRAINRow ligneXML = tableXML.AddTAB_MODELE_TERRAINRow(
                    ligne.ID_MODELE_TERRAIN,
                    ligne.S_NOM,
                    ligne.ID_GRAPHIQUE,
                    ligne.ID_MODELE_NOUVEAU_TERRAIN,
                    ligne.I_MODIFICATEUR_DEFENSE,
                    ligne.B_ANNULEE_SI_OCCUPEE,
                    ligne.B_CIRCUIT_ROUTIER,
                    ligne.B_OBSTACLE_DEFENSIF,
                    ligne.B_ANNULEE_EN_COMBAT,
                    ligne.B_PONT,
                    ligne.B_PONTON,
                    ligne.B_DETRUIT
                );
            }
            return true;
        }

        public void Ajouter(Donnees.TAB_MODELE_TERRAINRow ligneRow)
        {
            LigneMODELE_TERRAIN ligne = new LigneMODELE_TERRAIN(ligneRow);
            Ajouter(ligne);
        }

        public void Ajouter(LigneMODELE_TERRAIN ligne)
        {
            liste.Add(ligne);
        }

        public void Supprimer(LigneMODELE_TERRAIN ligne)
        {
            liste.Remove(ligne);
        }

        public void Vider()
        {
            liste.Clear();
        }

        public LigneMODELE_TERRAIN this[int i]
        {
            get
            {
                return liste[i];
            }
        }

        /// <summary>
        /// renvoie la valeur d'identifiant max de la table
        /// </summary>
        public int MaxID
        {
            get
            {
                int retour = -1;
                foreach (LigneMODELE_TERRAIN ligne in liste)
                {
                    if (ligne.ID_MODELE_TERRAIN > retour) { retour = ligne.ID_MODELE_TERRAIN; }
                }
                return retour;
            }
        }
    }
}
