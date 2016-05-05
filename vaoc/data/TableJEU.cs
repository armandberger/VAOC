using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vaoc
{
    [Serializable]
    class TableJEU
    {
        private List<LigneJEU> liste;

        public TableJEU()
        {
            liste = new List<LigneJEU>();
        }

        public bool Importer(Donnees.TAB_JEUDataTable tableXML)
        {
            Vider();
            foreach (Donnees.TAB_JEURow ligneXML in tableXML)
            {
                Ajouter(ligneXML);
            }
            return true;
        }

        public bool Exporter(Donnees.TAB_JEUDataTable tableXML)
        {
            tableXML.Clear();
            foreach (LigneJEU ligne in liste)
            {
                Donnees.TAB_JEURow ligneXML = tableXML.AddTAB_JEURow(ligne.ID_JEU,
                    ligne.S_NOM,
                    ligne.I_NOMBRE_TOURS,
                    ligne.I_NOMBRE_PHASES,
                    ligne.DT_INITIALE,
                    -1,
                    ligne.I_LEVER_DU_SOLEIL,
                    ligne.I_COUCHER_DU_SOLEIL,
                    ligne.I_ECHELLE,
                    ligne.ID_MODELE_TERRAIN_DEPLOIEMENT,
                    ligne.I_COUT_DE_BASE,
                    ligne.I_RAYON_BATAILLE,
                    ligne.I_HEURE_INITIALE,
                    ligne.I_HAUTEUR_CARTE,
                    ligne.I_LARGEUR_CARTE,
                    ligne.S_NOM_CARTE_TOPOGRAPHIQUE,
                    ligne.S_NOM_CARTE_ZOOM,
                    ligne.S_NOM_CARTE_GRIS,
                    ligne.S_NOM_CARTE_HISTORIQUE,
                    ligne.I_OBJECTIF,
                    ligne.I_TAILLEBLOC_PCC,
                    -1,
                    -1,
                    ligne.I_VERSION
                    );
                ligneXML.SetID_CARTE_COMPTE_RENDUNull();
                ligneXML.SetI_ZONEVILLE_PCCNull();
                ligneXML.SetI_DISTANCEVILLEMAX_PCCNull();
            }
            return true;
        }

        public void Ajouter(Donnees.TAB_JEURow ligneRow)
        {
            LigneJEU ligne = new LigneJEU(ligneRow);
            Ajouter(ligne);
        }

        public void Ajouter(LigneJEU ligne)
        {
            liste.Add(ligne);
        }

        public void Supprimer(LigneJEU ligne)
        {
            liste.Remove(ligne);
        }

        public void Vider()
        {
            liste.Clear();
        }

        public LigneJEU this[int i]
        {
            get
            {
                return liste[i];
            }
        }
    }
}
