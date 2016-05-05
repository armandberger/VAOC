using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vaoc
{
    [Serializable]
    class TableMOUVEMENT_COUT
    {
        private List<LigneMOUVEMENT_COUT> liste;

        public TableMOUVEMENT_COUT()
        {
            liste = new List<LigneMOUVEMENT_COUT>();
        }

        public bool Importer(Donnees.TAB_MOUVEMENT_COUTDataTable tableXML)
        {
            Vider();
            foreach (Donnees.TAB_MOUVEMENT_COUTRow ligneXML in tableXML)
            {
                Ajouter(ligneXML);
            }
            return true;
        }

        public bool Exporter(Donnees.TAB_MOUVEMENT_COUTDataTable tableXML)
        {
            tableXML.Clear();
            foreach (LigneMOUVEMENT_COUT ligne in liste)
            {
                Donnees.TAB_MOUVEMENT_COUTRow ligneXML = tableXML.AddTAB_MOUVEMENT_COUTRow(
                    ligne.ID_MODELE_MOUVEMENT,
                    ligne.ID_MODELE_TERRAIN,
                    ligne.ID_METEO,
                    ligne.I_COUT);
                //if (!ligne.ID_ANCIEN_MODELE_MOUVEMENT_PROPRIETAIRE.HasValue) { ligneXML.SetID_ANCIEN_MODELE_MOUVEMENT_PROPRIETAIRENull(); }
            }
            return true;
        }

        public void Ajouter(Donnees.TAB_MOUVEMENT_COUTRow ligneRow)
        {
            LigneMOUVEMENT_COUT ligne = new LigneMOUVEMENT_COUT(ligneRow);
            Ajouter(ligne);
        }

        public void Ajouter(LigneMOUVEMENT_COUT ligne)
        {
            liste.Add(ligne);
        }

        public void Supprimer(LigneMOUVEMENT_COUT ligne)
        {
            liste.Remove(ligne);
        }

        public void Vider()
        {
            liste.Clear();
        }

        public LigneMOUVEMENT_COUT this[int i]
        {
            get
            {
                return liste[i];
            }
        }
    }
}
