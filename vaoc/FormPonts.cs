using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WaocLib;

namespace vaoc
{
    public partial class FormPonts : Form
    {
        public FormPonts()
        {
            InitializeComponent();
        }

        public List<NomDePont> GenererLesNomsDePont()
        {
            int i;
            List<NomDePont> liste = new List<NomDePont>();
            List<Donnees.TAB_CASERow> casesVoisines = new List<Donnees.TAB_CASERow>();

            //string requete = ;
            //Donnees.TAB_CASERow[] listeCasesPonts = (Donnees.TAB_CASERow[]) Donnees.m_donnees.TAB_CASE.Select(requete).ToList<Donnees.TAB_CASERow>;
            List<Donnees.TAB_CASERow> listeCasesPont = (from Case in Donnees.m_donnees.TAB_CASE
                                                       from ModeleTerrain in Donnees.m_donnees.TAB_MODELE_TERRAIN
                                                        where ModeleTerrain.B_PONT == true
                                                        && (Case.ID_MODELE_TERRAIN == ModeleTerrain.ID_MODELE_TERRAIN)
                                                        select Case).ToList();
            i = 0;
            while (i<listeCasesPont.Count())
            {
                //quel est le nom du pont ?
                decimal distance;
                ClassMessager.COMPAS compas;
                int id_nom;
                string nomLieu;

                if (!ClassMessager.CaseVersZoneGeographique(listeCasesPont[i].ID_CASE, true, out distance, out compas, out id_nom))
                {
                    nomLieu = "Erreur sur CaseVersZoneGeographique";
                }
                else
                {
                    ClassMessager.NomZoneGeographique(distance, compas, id_nom, out nomLieu);
                }

                Donnees.TAB_NOMS_CARTERow ligneNomCarte = Donnees.m_donnees.TAB_NOMS_CARTE.FindByID_NOM(id_nom);
                string strDirection = ClassMessager.DirectionOrdreVersCompasString(compas, true);
                string nom, nomIndexWeb, chaineFormat;
                //limite pour laquelle on indique pas la distance
                if (distance < (decimal)1)
                {
                    //Vous êtes actuellement à 4 km à l'Est de Cobourg, pont à 12 km au Nord-Ouest 
                    nomIndexWeb = string.Format("{0}, le pont {1}", ligneNomCarte.S_NOM, strDirection);
                    //Vous êtes actuellement à 4 km à l'Est du pont à 12 km au Nord-Ouest de Cobourg
                    if (Constantes.DebuteParUneVoyelle(ligneNomCarte.S_NOM))
                    {
                        chaineFormat = "pont {1} de l'{0}";
                    }
                    else
                    {
                        chaineFormat = ClassMessager.EstNomLieuMasculin(ligneNomCarte.S_NOM) ? "pont {1} du {0}" : "pont {1} de {0}";
                    }
                    nom = string.Format(chaineFormat, ligneNomCarte.S_NOM, strDirection);
                }
                else
                {
                    nomIndexWeb = string.Format("{0}, pont à {1} km {2} ", ligneNomCarte.S_NOM, Math.Round(distance), strDirection);
                    //Vous êtes actuellement à 4 km à l'Est du pont à 12 km au Nord-Ouest de Cobourg
                    if (Constantes.DebuteParUneVoyelle(ligneNomCarte.S_NOM))
                    {
                        chaineFormat = "pont à {1} km {2} de l'{0}";
                    }
                    else
                    {
                        chaineFormat = ClassMessager.EstNomLieuMasculin(ligneNomCarte.S_NOM) ? "pont à {1} km {2} du {0}" : "pont à {1} km {2} de {0}";
                    }
                    nom = string.Format(chaineFormat, ligneNomCarte.S_NOM, Math.Round(distance), strDirection);
                }
                liste.Add(new NomDePont(nom, nomIndexWeb, listeCasesPont[i].ID_CASE));

                //on retire de la liste toutes les autres cases faisant partie du même pont
                casesVoisines.Clear();
                listeCasesPont[i].ListeCasesVoisinesDeMemeType(ref casesVoisines);
                foreach (Donnees.TAB_CASERow ligneCase in casesVoisines)
                {
                    if (listeCasesPont.IndexOf(ligneCase) > i)
                    {
                        listeCasesPont.Remove(ligneCase);
                    }
                }
                i++;
            }

            return liste;
        }

        private void FormPonts_Load(object sender, EventArgs e)
        {
            Redimensionner();
        }

        private void FormPonts_Resize(object sender, EventArgs e)
        {
            Redimensionner();
        }

        private void Redimensionner()
        {
            #region positionnement des résultats
            textBoxResultat.Top = buttonValider.Bottom + buttonValider.Height;
            textBoxResultat.Width = Width - 3*textBoxResultat.Left;
            textBoxResultat.Height = Height - 6 * buttonValider.Height;
            #endregion
        }

        private void buttonGeneration_Click(object sender, EventArgs e)
        {
            textBoxResultat.Clear();
            List<NomDePont> liste = GenererLesNomsDePont();
            int i=0;
            foreach (NomDePont nom in liste)
            {
                Donnees.TAB_CASERow ligneCase = Donnees.m_donnees.TAB_CASE.FindByID_CASE(nom.ID_CASE);
                textBoxResultat.Text += string.Format("{4}-{0} / {5} : {1} ({2},{3}) \r\n",
                    nom.S_NOM, nom.ID_CASE, ligneCase.I_X, ligneCase.I_Y, i++, nom.S_NOM_INDEX);
            }
        }
    }

    public class NomDePont
    {
        public string S_NOM { get; set; }
        public string S_NOM_INDEX { get; set; }
        public int ID_CASE { get; set; }
        public NomDePont(string nom, string nomIndex, int idCase) { S_NOM = nom;  S_NOM_INDEX = nomIndex;  ID_CASE = idCase; }
    }

}
