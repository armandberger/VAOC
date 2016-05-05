using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace vaoc
{
    public partial class FormRenfort : Form
    {
        public FormRenfort()
        {
            InitializeComponent();
        }

        #region propriétés
        public Donnees.TAB_RENFORTDataTable tableRenfort
        {
            get
            {
                string requete;
                Donnees.TAB_RENFORTDataTable table = new Donnees.TAB_RENFORTDataTable();
                foreach (DataGridViewRow ligne in this.dataGridViewRenfort.Rows)
                {
                    if (null != ligne.Cells["S_NOM"].Value)//cas pour la nouvelle ligne ajoutée automatiquement
                    {
                        //recherche de l'id du modèle de pion
                        requete = string.Format("S_NOM='{0}'", ligne.Cells["MODELE_PION"].Value);
                        Donnees.TAB_MODELE_PIONRow[] resDataModele = (Donnees.TAB_MODELE_PIONRow[])Donnees.m_donnees.TAB_MODELE_PION.Select(requete);

                        if (null == ligne.Cells["C_NIVEAU_HIERARCHIQUE"].Value || !char.IsLetter(Convert.ToChar(ligne.Cells["C_NIVEAU_HIERARCHIQUE"].Value))) ligne.Cells["C_NIVEAU_HIERARCHIQUE"].Value = 'Z';
                        if (null == ligne.Cells["C_NIVEAU_DEPOT"].Value || !char.IsLetter(Convert.ToChar(ligne.Cells["C_NIVEAU_DEPOT"].Value))) ligne.Cells["C_NIVEAU_DEPOT"].Value = ' ';
                        table.AddTAB_RENFORTRow(
                            Convert.ToInt32(ligne.Cells["ID_PION"].Value),
                            resDataModele[0].ID_MODELE_PION,
                            Convert.ToInt32(ligne.Cells["ID_PION_PROPRIETAIRE"].Value),
                            Convert.ToString(ligne.Cells["S_NOM"].Value),
                            Convert.ToInt32(ligne.Cells["ID_CASE"].Value),
                            Convert.ToInt32(ligne.Cells["I_TOUR_ARRIVEE"].Value),
                            Convert.ToInt32(ligne.Cells["I_INFANTERIE"].Value),
                            Convert.ToInt32(ligne.Cells["I_CAVALERIE"].Value),
                            Convert.ToInt32(ligne.Cells["I_ARTILLERIE"].Value),
                            Convert.ToInt32(ligne.Cells["I_FATIGUE"].Value),
                            Convert.ToInt32(ligne.Cells["I_MORAL"].Value),
                            Convert.ToInt32(ligne.Cells["I_MORAL_MAX"].Value),
                            Convert.ToInt32(ligne.Cells["I_EXPERIENCE"].Value),
                            Convert.ToInt32(ligne.Cells["I_TACTIQUE"].Value),
                            Convert.ToInt32(ligne.Cells["I_STRATEGIQUE"].Value),
                            Convert.ToChar(ligne.Cells["C_NIVEAU_HIERARCHIQUE"].Value),
                            Convert.ToInt32(ligne.Cells["I_MATERIEL"].Value),
                            Convert.ToInt32(ligne.Cells["I_RAVITAILLEMENT"].Value),
                            Convert.ToBoolean(ligne.Cells["B_CAVALERIE_DE_LIGNE"].Value),
                            Convert.ToBoolean(ligne.Cells["B_CAVALERIE_LOURDE"].Value),
                            Convert.ToBoolean(ligne.Cells["B_GARDE"].Value),
                            Convert.ToBoolean(ligne.Cells["B_VIEILLE_GARDE"].Value),
                            Convert.ToChar(ligne.Cells["C_NIVEAU_DEPOT"].Value),
                            Convert.ToBoolean(ligne.Cells["B_RENFORT"].Value),
                            Convert.ToString(ligne.Cells["S_MESSAGE_ARRIVEE"].Value)                            
                            );
                    }
                }
                return table;
            }
            set
            {
                dataGridViewRenfort.Rows.Clear();

                //mise à jour de la liste déroulante des modèles de pions disponibles
                foreach (Donnees.TAB_MODELE_PIONRow ligneModelePion in Donnees.m_donnees.TAB_MODELE_PION)
                {
                    this.MODELE_PION.Items.Add(ligneModelePion.S_NOM);
                }

                foreach (Donnees.TAB_RENFORTRow ligneRenfort in value)
                {
                    //recherche du modèle de pion couplé à l'id séléctionné
                    Donnees.TAB_MODELE_PIONRow[] resDataModelePion = (Donnees.TAB_MODELE_PIONRow[])Donnees.m_donnees.TAB_MODELE_PION.Select("ID_MODELE_PION=" + ligneRenfort.ID_MODELE_PION);

                    DataGridViewRow ligneGrid = dataGridViewRenfort.Rows[dataGridViewRenfort.Rows.Add()];
                    ligneGrid.Cells["ID_PION"].Value = ligneRenfort.ID_PION;
                    ligneGrid.Cells["S_NOM"].Value = ligneRenfort.S_NOM;
                    ligneGrid.Cells["MODELE_PION"].Value = resDataModelePion[0].S_NOM;
                    ligneGrid.Cells["ID_PION_PROPRIETAIRE"].Value = ligneRenfort.ID_PION_PROPRIETAIRE;
                    ligneGrid.Cells["ID_CASE"].Value = ligneRenfort.ID_CASE;
                    ligneGrid.Cells["I_TOUR_ARRIVEE"].Value = ligneRenfort.I_TOUR_ARRIVEE;
                    ligneGrid.Cells["I_INFANTERIE"].Value = ligneRenfort.I_INFANTERIE;
                    ligneGrid.Cells["I_CAVALERIE"].Value = ligneRenfort.I_CAVALERIE;
                    ligneGrid.Cells["I_ARTILLERIE"].Value = ligneRenfort.I_ARTILLERIE;
                    ligneGrid.Cells["I_FATIGUE"].Value = ligneRenfort.I_FATIGUE;
                    ligneGrid.Cells["I_MORAL"].Value = ligneRenfort.I_MORAL;
                    ligneGrid.Cells["I_MORAL_MAX"].Value = ligneRenfort.I_MORAL_MAX;
                    ligneGrid.Cells["I_EXPERIENCE"].Value = ligneRenfort.I_EXPERIENCE;
                    ligneGrid.Cells["I_TACTIQUE"].Value = ligneRenfort.I_TACTIQUE;
                    ligneGrid.Cells["I_STRATEGIQUE"].Value = ligneRenfort.I_STRATEGIQUE;
                    ligneGrid.Cells["C_NIVEAU_HIERARCHIQUE"].Value = ligneRenfort.C_NIVEAU_HIERARCHIQUE;
                    //ligneGrid.Cells["I_DISTANCE_A_PARCOURIR"].Value = lignePion.I_DISTANCE_A_PARCOURIR;
                    //ligneGrid.Cells["I_NB_PHASES_MARCHE_JOUR"].Value = lignePion.I_NB_PHASES_MARCHE_JOUR;
                    //ligneGrid.Cells["I_NB_PHASES_MARCHE_NUIT"].Value = lignePion.I_NB_PHASES_MARCHE_NUIT;
                //    if (!lignePion.IsID_BATAILLENull())
                //    {
                //        ligneGrid.Cells["ID_BATAILLE"].Value = lignePion.ID_BATAILLE;
                //    }
                //    ligneGrid.Cells["I_TOUR_SANS_RAVITAILLEMENT"].Value = lignePion.I_TOUR_SANS_RAVITAILLEMENT;
                //    if (!lignePion.IsI_ZONE_BATAILLENull())
                //    {
                //        ligneGrid.Cells["I_ZONE_BATAILLE"].Value = lignePion.I_ZONE_BATAILLE;
                //    }
                //    ligneGrid.Cells["I_TOUR_FUITE_RESTANT"].Value = lignePion.I_TOUR_FUITE_RESTANT;
                //    ligneGrid.Cells["B_DETRUIT"].Value = lignePion.B_DETRUIT;
                //
                    ligneGrid.Cells["I_MATERIEL"].Value = ligneRenfort.I_MATERIEL;
                    ligneGrid.Cells["I_RAVITAILLEMENT"].Value = ligneRenfort.I_RAVITAILLEMENT;
                    ligneGrid.Cells["B_CAVALERIE_DE_LIGNE"].Value = ligneRenfort.B_CAVALERIE_DE_LIGNE;
                    ligneGrid.Cells["B_CAVALERIE_LOURDE"].Value = ligneRenfort.B_CAVALERIE_LOURDE;
                    ligneGrid.Cells["B_GARDE"].Value = ligneRenfort.B_GARDE;
                    ligneGrid.Cells["B_VIEILLE_GARDE"].Value = ligneRenfort.B_VIEILLE_GARDE;
                    ligneGrid.Cells["C_NIVEAU_DEPOT"].Value = ligneRenfort.C_NIVEAU_DEPOT;
                    ligneGrid.Cells["B_RENFORT"].Value = ligneRenfort.B_RENFORT;
                    ligneGrid.Cells["S_MESSAGE_ARRIVEE"].Value = ligneRenfort.S_MESSAGE_ARRIVEE;                    
                }
            }
        }
        #endregion

        #region evenements
        private void FormPion_Resize(object sender, EventArgs e)
        {
            //repositionnement des composants
            //Control control = (Control)sender;
            Redimensionner();
        }

        private void Redimensionner()
        {
            #region positionnement des boutons annuler et valider
            buttonValider.Left = (Width - buttonValider.Width - buttonAnnuler.Width) / 3;
            buttonAnnuler.Left = buttonValider.Width + 2 * (Width - buttonValider.Width - buttonAnnuler.Width) / 3;
            buttonValider.Top = buttonAnnuler.Top = Height - 3 * buttonValider.Height;
            #endregion

            #region positionnement du champ commentaire
            labelCommentaire.Left = buttonValider.Left;
            labelCommentaire.Top = buttonAnnuler.Top - 2 * labelCommentaire.Height;
            #endregion

            #region positionnement de la grille
            dataGridViewRenfort.Width = Width - SystemInformation.VerticalScrollBarWidth;
            dataGridViewRenfort.Height = Height - 4 * buttonValider.Height - 3 * labelCommentaire.Height;
            #endregion
        }

        private void FormPion_Load(object sender, EventArgs e)
        {
            Redimensionner();
        }
        #endregion

    }
}