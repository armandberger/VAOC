using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace vaoc
{
    public partial class FormModelesPions : Form
    {
        public FormModelesPions()
        {
            InitializeComponent();
        }

        #region propriétés
        public Donnees.TAB_MODELE_PIONDataTable tableModelesPions
        {
            get
            {
                string requete;
                Donnees.TAB_MODELE_PIONDataTable table = new Donnees.TAB_MODELE_PIONDataTable();
                foreach (DataGridViewRow ligne in this.dataGridViewModelesPions.Rows)
                {
                    if (null != ligne.Cells["S_NOM"].Value)//cas pour la nouvelle ligne ajoutée automatiquement
                    {
                        //recherche de l'id de la nation associée couplé au nom séléctionné
                        requete = string.Format("S_NOM='{0}'", ligne.Cells["ID_NATION"].Value);
                        Donnees.TAB_NATIONRow[] resDataNation = (Donnees.TAB_NATIONRow[])Donnees.m_donnees.TAB_NATION.Select(requete);

                        //recherche de l'id du modèle de mouvement couplé au nom séléctionné
                        requete = string.Format("S_NOM='{0}'", ligne.Cells["MODELE_DE_MOUVEMENT"].Value);
                        Donnees.TAB_MODELE_MOUVEMENTRow[] resDataMouvement = (Donnees.TAB_MODELE_MOUVEMENTRow[])Donnees.m_donnees.TAB_MODELE_MOUVEMENT.Select(requete);

                        Color pixelColor = ligne.Cells["Couleur"].Style.BackColor;
                        table.AddTAB_MODELE_PIONRow(
                            Convert.ToInt32(ligne.Cells["ID_MODELE_PION"].Value),
                            Convert.ToString(ligne.Cells["S_NOM"].Value),
                            resDataMouvement[0].ID_MODELE_MOUVEMENT,
                            resDataNation[0].ID_NATION,
                            pixelColor.R, pixelColor.G, pixelColor.B,
                            Convert.ToInt32(ligne.Cells["I_VISION_JOUR"].Value),
                            Convert.ToInt32(ligne.Cells["I_VISION_NUIT"].Value)
                            );
                    }
                }
                return table;
            }
            set
            {
                dataGridViewModelesPions.Rows.Clear();

                //mise à jour de la liste déroulante des modèles de combat disponibles
                foreach (Donnees.TAB_NATIONRow ligneNation in Donnees.m_donnees.TAB_NATION)
                {
                    this.ID_NATION.Items.Add(ligneNation.S_NOM);
                }

                //mise à jour de la liste déroulante des modèles de mouvements disponibles
                foreach (Donnees.TAB_MODELE_MOUVEMENTRow ligneMouvement in Donnees.m_donnees.TAB_MODELE_MOUVEMENT)
                {
                    this.MODELE_DE_MOUVEMENT.Items.Add(ligneMouvement.S_NOM);
                }

                foreach (Donnees.TAB_MODELE_PIONRow ligneModele in value)
                {
                    //recherche du modèle de combat couplé à l'id séléctionné
                    Donnees.TAB_NATIONRow[] resDataNation = (Donnees.TAB_NATIONRow[])Donnees.m_donnees.TAB_NATION.Select("ID_NATION=" + ligneModele.ID_NATION);

                    //recherche du modèle de mouvement couplé à l'id séléctionné
                    Donnees.TAB_MODELE_MOUVEMENTRow[] resDataMouvement = (Donnees.TAB_MODELE_MOUVEMENTRow[])Donnees.m_donnees.TAB_MODELE_MOUVEMENT.Select("ID_MODELE_MOUVEMENT=" + ligneModele.ID_MODELE_MOUVEMENT);

                    //recherche de la couleur utilisée
                    Color pixelColor = Color.FromArgb(ligneModele.I_ROUGE, ligneModele.I_VERT, ligneModele.I_BLEU);

                    DataGridViewRow ligneGrid = dataGridViewModelesPions.Rows[dataGridViewModelesPions.Rows.Add()];
                    ligneGrid.Cells["S_NOM"].Value = ligneModele.S_NOM;
                    ligneGrid.Cells["ID_MODELE_PION"].Value = ligneModele.ID_MODELE_PION;
                    ligneGrid.Cells["ID_NATION"].Value = resDataNation[0].S_NOM;
                    ligneGrid.Cells["MODELE_DE_MOUVEMENT"].Value = resDataMouvement[0].S_NOM;
                    ligneGrid.Cells["Couleur"].Style.BackColor = pixelColor;
                    ligneGrid.Cells["I_VISION_JOUR"].Value = ligneModele.I_VISION_JOUR;
                    ligneGrid.Cells["I_VISION_NUIT"].Value = ligneModele.I_VISION_NUIT;
                }
            }
        }
        #endregion

        private void dataGridViewModelesPions_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (4 == e.ColumnIndex)
            {
                couleurDialog.Color = dataGridViewModelesPions.Rows[e.RowIndex].Cells["Couleur"].Style.BackColor;
                if (DialogResult.OK == couleurDialog.ShowDialog())
                {
                    dataGridViewModelesPions.Rows[e.RowIndex].Cells["Couleur"].Style.BackColor = couleurDialog.Color;
                }
            }
        }

        private void FormModelesPions_Load(object sender, EventArgs e)
        {
            Redimensionner();
        }

        private void FormModelesPions_Resize(object sender, EventArgs e)
        {
            Redimensionner();
        }

        private void Redimensionner()
        {
            buttonValider.Left = (Width - buttonValider.Width - buttonAnnuler.Width - 20 - labelModele.Width -20) / 2;
            labelModele.Left = buttonValider.Left + buttonValider.Width + 20;
            buttonAnnuler.Left = labelModele.Left + labelModele.Width + 20;
            buttonValider.Top = Height - 3 * buttonAnnuler.Height;
            labelModele.Top = buttonValider.Top;
            buttonAnnuler.Top = buttonValider.Top;

            dataGridViewModelesPions.Width = Width - dataGridViewModelesPions.Left * 2;
            dataGridViewModelesPions.Height = Height - 4 * buttonAnnuler.Height;
        }
    }
}