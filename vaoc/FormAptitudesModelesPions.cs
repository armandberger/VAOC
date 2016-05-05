using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace vaoc
{
    public partial class FormAptitudesModelesPions : Form
    {
        private Donnees.TAB_APTITUDES_PIONDataTable m_tableAptitudesPions;

        public FormAptitudesModelesPions()
        {
            InitializeComponent();
        }

        #region propriétés
        public Donnees.TAB_APTITUDES_PIONDataTable tableAptitudesModelesPions
        {
            get
            {
                //string requete;
                //DataSetCoutDonnees.TAB_MODELE_PIONDataTable table = new DataSetCoutDonnees.TAB_MODELE_PIONDataTable();
                //foreach (DataGridViewRow ligne in this.dataGridViewModelesPions.Rows)
                //{
                //    if (null != ligne.Cells["S_NOM"].Value)//cas pour la nouvelle ligne ajoutée automatiquement
                //    {
                //        //recherche de l'id du modèle de combat couplé au nom séléctionné
                //        requete = string.Format("S_NOM='{0}'", ligne.Cells["MODELE_DE_COMBAT"].Value);
                //        DataSetCoutDonnees.TAB_MODELE_COMBATRow[] resDataCombat = (DataSetCoutDonnees.TAB_MODELE_COMBATRow[])DataSetCoutDonnees.m_donnees.TAB_MODELE_COMBAT.Select(requete);

                //        //recherche de l'id du modèle de mouvement couplé au nom séléctionné
                //        requete = string.Format("S_NOM='{0}'", ligne.Cells["MODELE_DE_MOUVEMENT"].Value);
                //        DataSetCoutDonnees.TAB_MODELE_MOUVEMENTRow[] resDataMouvement = (DataSetCoutDonnees.TAB_MODELE_MOUVEMENTRow[])DataSetCoutDonnees.m_donnees.TAB_MODELE_MOUVEMENT.Select(requete);

                //        Color pixelColor = ligne.Cells["Couleur"].Style.BackColor;
                //        table.AddTAB_MODELE_PIONRow(
                //            Convert.ToInt32(ligne.Cells["ID_MODELE_PION"].Value),
                //            Convert.ToString(ligne.Cells["S_NOM"].Value),
                //            resDataMouvement[0].ID_MODELE_MOUVEMENT,
                //            resDataCombat[0].ID_MODELE_COMBAT,
                //            pixelColor.R, pixelColor.G, pixelColor.B
                //            );
                //    }
                //}
                int ligne, colonne;
                m_tableAptitudesPions.Clear();
                for (ligne=0;ligne<Donnees.m_donnees.TAB_MODELE_PION.Count;ligne++)
                {
                    for (colonne=1;colonne<Donnees.m_donnees.TAB_APTITUDES.Count+1;colonne++)
                    {
                        CheckBox aptitude = (CheckBox)tablePlacement.GetControlFromPosition(colonne, ligne);
                        if (aptitude.Checked)
                        {
                            //insertion de l'aptitude en base
                            string modeleName=tablePlacement.GetControlFromPosition(0, ligne).Name;
                            int id_modele = Convert.ToInt32(modeleName.Substring(modeleName.Length-3,3));
                            int id_aptitude = Convert.ToInt32(aptitude.Name.Substring(aptitude.Name.Length - 3, 3));

                            m_tableAptitudesPions.AddTAB_APTITUDES_PIONRow(id_modele,id_aptitude);
                        }
                    }
                }

                return m_tableAptitudesPions;
            }
            set
            {
                int ligne, colonne;
                string requete;

                tablePlacement.Controls.Clear();
                tablePlacement.RowCount = Donnees.m_donnees.TAB_MODELE_PION.Count;
                tablePlacement.ColumnCount = Donnees.m_donnees.TAB_APTITUDES.Count+1;

                m_tableAptitudesPions = (Donnees.TAB_APTITUDES_PIONDataTable)value.Copy();

                //construction du tableau
                ligne=0;
                foreach (Donnees.TAB_MODELE_PIONRow ligneModele in Donnees.m_donnees.TAB_MODELE_PION)
                {
                    Label textModele=new Label();
                    textModele.Text = ligneModele.S_NOM;
                    textModele.Name = string.Format("Modele{0:000}", ligneModele.ID_MODELE_PION);

                    colonne = 0;
                    tablePlacement.Controls.Add(textModele, colonne++, ligne);
                    foreach (Donnees.TAB_APTITUDESRow ligneAptitude in Donnees.m_donnees.TAB_APTITUDES)
                    {
                        CheckBox cocheAptitude = new CheckBox();
                        cocheAptitude.Text = ligneAptitude.S_NOM;
                        cocheAptitude.Name = string.Format("Aptitude{0:000}{1:000}", ligneModele.ID_MODELE_PION, ligneAptitude.ID_APTITUDE);
                        requete = string.Format("ID_MODELE_PION='{0}' AND ID_APTITUDE='{1}'", ligneModele.ID_MODELE_PION, ligneAptitude.ID_APTITUDE);
                        Donnees.TAB_APTITUDES_PIONRow[] resData = (Donnees.TAB_APTITUDES_PIONRow[])m_tableAptitudesPions.Select(requete);
                        if (resData.Length > 0)
                        {
                            cocheAptitude.Checked = true;
                        }
                        tablePlacement.Controls.Add(cocheAptitude, colonne++, ligne);
                    }
                    ligne++;
                }

                ////mise à jour de la liste déroulante des modèles de mouvements disponibles
                //foreach (DataSetCoutDonnees.TAB_MODELE_MOUVEMENTRow ligneMouvement in DataSetCoutDonnees.m_donnees.TAB_MODELE_MOUVEMENT)
                //{
                //    this.MODELE_DE_MOUVEMENT.Items.Add(ligneMouvement.S_NOM);
                //}

                //foreach (DataSetCoutDonnees.TAB_MODELE_PIONRow ligneModele in value)
                //{
                //    //recherche du modèle de combat couplé à l'id séléctionné
                //    DataSetCoutDonnees.TAB_MODELE_COMBATRow[] resDataCombat = (DataSetCoutDonnees.TAB_MODELE_COMBATRow[])DataSetCoutDonnees.m_donnees.TAB_MODELE_COMBAT.Select("ID_MODELE_COMBAT=" + ligneModele.ID_MODELE_COMBAT);

                //    //recherche du modèle de mouvement couplé à l'id séléctionné
                //    DataSetCoutDonnees.TAB_MODELE_MOUVEMENTRow[] resDataMouvement = (DataSetCoutDonnees.TAB_MODELE_MOUVEMENTRow[])DataSetCoutDonnees.m_donnees.TAB_MODELE_MOUVEMENT.Select("ID_MODELE_MOUVEMENT=" + ligneModele.ID_MODELE_MOUVEMENT);

                //    //recherche de la couleur utilisée
                //    Color pixelColor = Color.FromArgb(ligneModele.I_ROUGE, ligneModele.I_VERT, ligneModele.I_BLEU);

                //    DataGridViewRow ligneGrid = dataGridViewModelesPions.Rows[dataGridViewModelesPions.Rows.Add()];
                //    ligneGrid.Cells["S_NOM"].Value = ligneModele.S_NOM;
                //    ligneGrid.Cells["ID_MODELE_PION"].Value = ligneModele.ID_MODELE_PION;
                //    ligneGrid.Cells["MODELE_DE_COMBAT"].Value = resDataCombat[0].S_NOM;
                //    ligneGrid.Cells["MODELE_DE_MOUVEMENT"].Value = resDataMouvement[0].S_NOM;
                //    ligneGrid.Cells["Couleur"].Style.BackColor = pixelColor;
                //}

            }
        }
        #endregion

        private void FormAptitudesModelesPions_SizeChanged(object sender, EventArgs e)
        {
            Redimensionner();
        }

        private void FormAptitudesModelesPions_Load(object sender, EventArgs e)
        {
            Redimensionner();
        }

        private void Redimensionner()
        {
            tablePlacement.Top = buttonValider.Bottom + 15;
            tablePlacement.Left = 15;
            tablePlacement.Width = this.Width - tablePlacement.Left - SystemInformation.VerticalScrollBarWidth - 15;
            tablePlacement.Height = this.Height - tablePlacement.Top- SystemInformation.HorizontalScrollBarHeight -15;
        }

    }
}