using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WaocLib;

namespace vaoc
{
    public partial class FormPion : Form
    {
        public FormPion()
        {
            InitializeComponent();
        }

        #region propri�t�s
        public Donnees.TAB_PIONDataTable tablePions
        {
            get
            {
                //string requete;
                Donnees.TAB_PIONDataTable table = new Donnees.TAB_PIONDataTable();

                foreach (DataGridViewRow ligne in this.dataGridViewPions.Rows)
                {
                    if (null != ligne.Cells["S_NOM"].Value)//cas pour la nouvelle ligne ajout�e automatiquement
                    {
                        //recherche de l'id du mod�le de pion en fonction de son nom et de sa nation
                        //requete = string.Format("S_NOM='{0}' AND ID_NATION={1}", ligne.Cells["MODELE_PION"].Value, lignePion.nation.ID_NATION);
                        //Donnees.TAB_MODELE_PIONRow[] resDataModele = (Donnees.TAB_MODELE_PIONRow[])Donnees.m_donnees.TAB_MODELE_PION.Select(requete);
                        //lignePion.ID_MODELE_PION = resDataModele[0].ID_MODELE_PION;

                        //Donnees.TAB_MODELE_PIONRow ligneModelePion = (Donnees.TAB_MODELE_PIONRow)ligne.Cells["MODELE_PION"].Value;
                        if (null == ligne.Cells["ID_BATAILLE"].Value) ligne.Cells["ID_BATAILLE"].Value="-1";
                        if (null == ligne.Cells["I_TOUR_SANS_RAVITAILLEMENT"].Value) ligne.Cells["I_TOUR_SANS_RAVITAILLEMENT"].Value = "-1";
                        if (null == ligne.Cells["I_ZONE_BATAILLE"].Value) ligne.Cells["I_ZONE_BATAILLE"].Value = "-1";
                        if (null == ligne.Cells["I_TOUR_CONVOI_CREE"].Value) ligne.Cells["I_TOUR_CONVOI_CREE"].Value = "-1";
                        if (null == ligne.Cells["ID_PION_REMPLACE"].Value) ligne.Cells["ID_PION_REMPLACE"].Value = "-1";
                        if (null == ligne.Cells["I_TOUR_BLESSURE"].Value) ligne.Cells["I_TOUR_BLESSURE"].Value = "-1";
                        if (null == ligne.Cells["ID_LIEU_RATTACHEMENT"].Value) ligne.Cells["ID_LIEU_RATTACHEMENT"].Value = "-1";
                        if (null == ligne.Cells["ID_NOUVEAU_PION_PROPRIETAIRE"].Value) ligne.Cells["ID_NOUVEAU_PION_PROPRIETAIRE"].Value = "-1";
                        if (null == ligne.Cells["ID_ANCIEN_PION_PROPRIETAIRE"].Value) ligne.Cells["ID_ANCIEN_PION_PROPRIETAIRE"].Value = "-1";
                        if (null == ligne.Cells["ID_PION_ESCORTE"].Value) ligne.Cells["ID_PION_ESCORTE"].Value = "-1";
                        if (null == ligne.Cells["C_NIVEAU_HIERARCHIQUE"].Value || !char.IsLetter(Convert.ToChar(ligne.Cells["C_NIVEAU_HIERARCHIQUE"].Value))) ligne.Cells["C_NIVEAU_HIERARCHIQUE"].Value = 'Z';
                        if (null == ligne.Cells["C_NIVEAU_DEPOT"].Value || !char.IsLetter(Convert.ToChar(ligne.Cells["C_NIVEAU_DEPOT"].Value))) ligne.Cells["C_NIVEAU_DEPOT"].Value = ' ';
                        if (null == ligne.Cells["ID_DEPOT_SOURCE"].Value) ligne.Cells["ID_DEPOT_SOURCE"].Value = "-1";
                        if (null == ligne.Cells["I_TRI"].Value) ligne.Cells["I_TRI"].Value = "-1";
                        if (null == ligne.Cells["S_ENNEMI_OBSERVABLE"].Value) ligne.Cells["S_ENNEMI_OBSERVABLE"].Value = string.Empty;
                        if (null == ligne.Cells["I_TOUR_ENNEMI_OBSERVABLE"].Value) ligne.Cells["I_TOUR_ENNEMI_OBSERVABLE"].Value = "-1"; 

                        Donnees.TAB_PIONRow lignePion = table.AddTAB_PIONRow(
                            Convert.ToInt32(ligne.Cells["ID_PION"].Value),
                            //Convert.ToInt32(((Donnees.TAB_MODELE_PIONRow)ligne.Cells["MODELE_PION"].Value).ID_MODELE_PION),
                            Convert.ToInt32(ligne.Cells["MODELE_PION"].Value),
                            //ligneModelePion.ID_MODELE_PION,
                            Convert.ToInt32((ligne.Cells["ID_PION_PROPRIETAIRE"].Value)),
                            Convert.ToInt32((ligne.Cells["ID_NOUVEAU_PION_PROPRIETAIRE"].Value)),
                            Convert.ToInt32((ligne.Cells["ID_ANCIEN_PION_PROPRIETAIRE"].Value)),                            
                            Convert.ToString(ligne.Cells["S_NOM"].Value),
                            Convert.ToInt32(ligne.Cells["I_INFANTERIE"].Value),
                            Convert.ToInt32(ligne.Cells["I_INFANTERIE_INITIALE"].Value),
                            Convert.ToInt32(ligne.Cells["I_CAVALERIE"].Value),
                            Convert.ToInt32(ligne.Cells["I_CAVALERIE_INITIALE"].Value),
                            Convert.ToInt32(ligne.Cells["I_ARTILLERIE"].Value),
                            Convert.ToInt32(ligne.Cells["I_ARTILLERIE_INITIALE"].Value),
                            Convert.ToInt32(ligne.Cells["I_FATIGUE"].Value),
                            Convert.ToInt32(ligne.Cells["I_MORAL"].Value),
                            Convert.ToInt32(ligne.Cells["I_MORAL_MAX"].Value),
                            Convert.ToDecimal(ligne.Cells["I_EXPERIENCE"].Value),
                            Convert.ToInt32(ligne.Cells["I_TACTIQUE"].Value),
                            Convert.ToInt32(ligne.Cells["I_STRATEGIQUE"].Value),
                            Convert.ToChar(ligne.Cells["C_NIVEAU_HIERARCHIQUE"].Value),
                            Convert.ToInt32(ligne.Cells["I_DISTANCE_A_PARCOURIR"].Value),
                            Convert.ToInt32(ligne.Cells["I_NB_PHASES_MARCHE_JOUR"].Value),
                            Convert.ToInt32(ligne.Cells["I_NB_PHASES_MARCHE_NUIT"].Value),
                            Convert.ToInt32(ligne.Cells["I_NB_HEURES_COMBAT"].Value),                            
                            Convert.ToInt32(ligne.Cells["ID_CASE"].Value),
                            Convert.ToInt32(ligne.Cells["I_TOUR_SANS_RAVITAILLEMENT"].Value),
                            Convert.ToInt32(ligne.Cells["ID_BATAILLE"].Value),
                            Convert.ToInt32(ligne.Cells["I_ZONE_BATAILLE"].Value),
                            Convert.ToInt32(ligne.Cells["I_TOUR_RETRAITE_RESTANT"].Value),
                            Convert.ToInt32(ligne.Cells["I_TOUR_FUITE_RESTANT"].Value),
                            Convert.ToBoolean(ligne.Cells["B_DETRUIT"].Value),
                            Convert.ToBoolean(ligne.Cells["B_FUITE_AU_COMBAT"].Value),
                            Convert.ToBoolean(ligne.Cells["B_INTERCEPTION"].Value),
                            Convert.ToBoolean(ligne.Cells["B_REDITION_RAVITAILLEMENT"].Value),
                            Convert.ToBoolean(ligne.Cells["B_TELEPORTATION"].Value),
                            Convert.ToBoolean(ligne.Cells["B_ENNEMI_OBSERVABLE"].Value),                      
                            Convert.ToInt32(ligne.Cells["I_MATERIEL"].Value),
                            Convert.ToInt32(ligne.Cells["I_RAVITAILLEMENT"].Value),
                            Convert.ToBoolean(ligne.Cells["B_CAVALERIE_DE_LIGNE"].Value),
                            Convert.ToBoolean(ligne.Cells["B_CAVALERIE_LOURDE"].Value),
                            Convert.ToBoolean(ligne.Cells["B_GARDE"].Value),
                            Convert.ToBoolean(ligne.Cells["B_VIEILLE_GARDE"].Value),
                            Convert.ToInt32(ligne.Cells["I_TOUR_CONVOI_CREE"].Value),
                            Convert.ToInt32(ligne.Cells["ID_DEPOT_SOURCE"].Value),
                            Convert.ToInt32(ligne.Cells["I_SOLDATS_RAVITAILLES"].Value),
                            Convert.ToInt32(ligne.Cells["I_NB_HEURES_FORTIFICATION"].Value),
                            Convert.ToInt32(ligne.Cells["I_NIVEAU_FORTIFICATION"].Value),
                            Convert.ToInt32(ligne.Cells["ID_PION_REMPLACE"].Value),
                            Convert.ToInt32(ligne.Cells["I_DUREE_HORS_COMBAT"].Value),
                            Convert.ToInt32(ligne.Cells["I_TOUR_BLESSURE"].Value),
                            Convert.ToBoolean(ligne.Cells["B_BLESSES"].Value),
                            Convert.ToBoolean(ligne.Cells["B_PRISONNIERS"].Value),
                            Convert.ToBoolean(ligne.Cells["B_RENFORT"].Value),                            
                            Convert.ToInt32(ligne.Cells["ID_LIEU_RATTACHEMENT"].Value),
                            Convert.ToChar(ligne.Cells["C_NIVEAU_DEPOT"].Value),
                            Convert.ToInt32(ligne.Cells["ID_PION_ESCORTE"].Value),
                            Convert.ToInt32(ligne.Cells["I_INFANTERIE_ESCORTE"].Value),
                            Convert.ToInt32(ligne.Cells["I_CAVALERIE_ESCORTE"].Value),
                            Convert.ToInt32(ligne.Cells["I_MATERIEL_ESCORTE"].Value),
                            Convert.ToInt32(ligne.Cells["I_TOUR_DERNIER_RAVITAILLEMENT_DIRECT"].Value),
                            Convert.ToInt32(ligne.Cells["I_VICTOIRE"].Value),
                            Convert.ToInt32(ligne.Cells["I_TRI"].Value),
                            Convert.ToString(ligne.Cells["S_ENNEMI_OBSERVABLE"].Value),
                            Convert.ToInt32(ligne.Cells["I_TOUR_ENNEMI_OBSERVABLE"].Value)
                            );
                        if (lignePion.ID_BATAILLE < 0) lignePion.SetID_BATAILLENull();
                        if (lignePion.I_TOUR_SANS_RAVITAILLEMENT < 0) lignePion.I_TOUR_SANS_RAVITAILLEMENT=0;
                        if (lignePion.I_ZONE_BATAILLE < 0) lignePion.SetI_ZONE_BATAILLENull();
                        if (lignePion.I_TOUR_CONVOI_CREE < 0) lignePion.I_TOUR_CONVOI_CREE=0;
                        if (lignePion.ID_PION_REMPLACE < 0) lignePion.SetID_PION_REMPLACENull();
                        if (lignePion.I_TOUR_BLESSURE < 0) lignePion.SetI_TOUR_BLESSURENull();
                        if (lignePion.ID_LIEU_RATTACHEMENT < 0) lignePion.SetID_LIEU_RATTACHEMENTNull();
                        if (lignePion.ID_NOUVEAU_PION_PROPRIETAIRE < 0) lignePion.SetID_NOUVEAU_PION_PROPRIETAIRENull();
                        if (lignePion.ID_ANCIEN_PION_PROPRIETAIRE < 0) lignePion.SetID_ANCIEN_PION_PROPRIETAIRENull();
                        if (lignePion.ID_PION_ESCORTE < 0) lignePion.SetID_PION_ESCORTENull();
                        if (lignePion.ID_DEPOT_SOURCE < 0) lignePion.SetID_DEPOT_SOURCENull();
                        if (lignePion.I_TOUR_DERNIER_RAVITAILLEMENT_DIRECT < 0) lignePion.SetI_TOUR_DERNIER_RAVITAILLEMENT_DIRECTNull();
                        if (lignePion.I_VICTOIRE < 0) lignePion.I_VICTOIRE=0;
                        if (lignePion.I_TRI < 0) lignePion.SetI_TRINull();
                        if (lignePion.I_TOUR_ENNEMI_OBSERVABLE < 0) lignePion.SetI_TOUR_ENNEMI_OBSERVABLENull();
                    }
                }
                return table;
            }
            set
            {
                dataGridViewPions.Rows.Clear();

                //mise � jour de la liste d�roulante des mod�les de pions disponibles
                foreach (Donnees.TAB_MODELE_PIONRow ligneModelePion in Donnees.m_donnees.TAB_MODELE_PION)
                {
                    this.MODELE_PION.Items.Add(ligneModelePion);
                }
                this.MODELE_PION.DisplayMember = "S_NOM";
                this.MODELE_PION.ValueMember = "ID_MODELE_PION";

                foreach (Donnees.TAB_PIONRow lignePion in value)
                {
                    DataGridViewRow ligneGrid = dataGridViewPions.Rows[dataGridViewPions.Rows.Add()];

                    //recherche du mod�le de pion coupl� � l'id s�l�ctionn�
                    Donnees.TAB_MODELE_PIONRow[] resDataModelePion = (Donnees.TAB_MODELE_PIONRow[])Donnees.m_donnees.TAB_MODELE_PION.Select("ID_MODELE_PION=" + lignePion.ID_MODELE_PION);
                    //recherche des coordonn�es de la case du pion
                    ligneGrid.Cells["ID_PION"].Value = lignePion.ID_PION.ToString("00000");
                    ligneGrid.Cells["S_NOM"].Value = lignePion.S_NOM;
                    ligneGrid.Cells["MODELE_PION"].Value = resDataModelePion[0].ID_MODELE_PION;
                    ligneGrid.Cells["ID_PION_PROPRIETAIRE"].Value = lignePion.ID_PION_PROPRIETAIRE;
                    ligneGrid.Cells["ID_NOUVEAU_PION_PROPRIETAIRE"].Value = lignePion.IsID_NOUVEAU_PION_PROPRIETAIRENull() ? -1 : lignePion.ID_NOUVEAU_PION_PROPRIETAIRE;
                    ligneGrid.Cells["ID_ANCIEN_PION_PROPRIETAIRE"].Value = lignePion.IsID_ANCIEN_PION_PROPRIETAIRENull() ? -1 : lignePion.ID_ANCIEN_PION_PROPRIETAIRE;                    
                    ligneGrid.Cells["I_INFANTERIE"].Value = lignePion.I_INFANTERIE;
                    ligneGrid.Cells["I_INFANTERIE_INITIALE"].Value = lignePion.I_INFANTERIE_INITIALE;
                    ligneGrid.Cells["I_CAVALERIE"].Value = lignePion.I_CAVALERIE;
                    ligneGrid.Cells["I_CAVALERIE_INITIALE"].Value = lignePion.I_CAVALERIE_INITIALE;
                    ligneGrid.Cells["I_ARTILLERIE"].Value = lignePion.I_ARTILLERIE;
                    ligneGrid.Cells["I_ARTILLERIE_INITIALE"].Value = lignePion.I_ARTILLERIE_INITIALE;
                    ligneGrid.Cells["I_FATIGUE"].Value = lignePion.I_FATIGUE;
                    ligneGrid.Cells["I_MORAL"].Value = lignePion.I_MORAL;
                    ligneGrid.Cells["I_MORAL_MAX"].Value = lignePion.I_MORAL_MAX;
                    ligneGrid.Cells["I_EXPERIENCE"].Value = lignePion.I_EXPERIENCE;
                    ligneGrid.Cells["I_TACTIQUE"].Value = lignePion.I_TACTIQUE;
                    ligneGrid.Cells["I_STRATEGIQUE"].Value = lignePion.I_STRATEGIQUE;
                    ligneGrid.Cells["C_NIVEAU_HIERARCHIQUE"].Value = lignePion.IsC_NIVEAU_HIERARCHIQUENull() ? ' ' : lignePion.C_NIVEAU_HIERARCHIQUE;
                    ligneGrid.Cells["I_DISTANCE_A_PARCOURIR"].Value = lignePion.I_DISTANCE_A_PARCOURIR;
                    ligneGrid.Cells["I_NB_PHASES_MARCHE_JOUR"].Value = lignePion.I_NB_PHASES_MARCHE_JOUR;
                    ligneGrid.Cells["I_NB_PHASES_MARCHE_NUIT"].Value = lignePion.I_NB_PHASES_MARCHE_NUIT;
                    ligneGrid.Cells["I_NB_HEURES_COMBAT"].Value = lignePion.I_NB_HEURES_COMBAT;                    
                    ligneGrid.Cells["ID_CASE"].Value = lignePion.ID_CASE;
                    ligneGrid.Cells["ID_BATAILLE"].Value = lignePion.IsID_BATAILLENull() ? -1 : lignePion.ID_BATAILLE;
                    ligneGrid.Cells["I_TOUR_SANS_RAVITAILLEMENT"].Value = lignePion.I_TOUR_SANS_RAVITAILLEMENT;
                    ligneGrid.Cells["I_ZONE_BATAILLE"].Value = lignePion.IsI_ZONE_BATAILLENull() ? -1 : lignePion.I_ZONE_BATAILLE;
                    ligneGrid.Cells["I_TOUR_RETRAITE_RESTANT"].Value = lignePion.IsI_TOUR_RETRAITE_RESTANTNull() ? 0 : lignePion.I_TOUR_RETRAITE_RESTANT;
                    ligneGrid.Cells["I_TOUR_FUITE_RESTANT"].Value = lignePion.I_TOUR_FUITE_RESTANT;
                    ligneGrid.Cells["B_DETRUIT"].Value = lignePion.B_DETRUIT;
                    ligneGrid.Cells["B_FUITE_AU_COMBAT"].Value = lignePion.B_FUITE_AU_COMBAT;
                    ligneGrid.Cells["B_INTERCEPTION"].Value = lignePion.B_INTERCEPTION;
                    ligneGrid.Cells["B_TELEPORTATION"].Value = lignePion.B_TELEPORTATION;
                    ligneGrid.Cells["B_REDITION_RAVITAILLEMENT"].Value = lignePion.B_REDITION_RAVITAILLEMENT;
                    ligneGrid.Cells["B_ENNEMI_OBSERVABLE"].Value = lignePion.B_ENNEMI_OBSERVABLE;
                    ligneGrid.Cells["I_MATERIEL"].Value = lignePion.I_MATERIEL;
                    ligneGrid.Cells["I_RAVITAILLEMENT"].Value = lignePion.I_RAVITAILLEMENT;
                    ligneGrid.Cells["B_CAVALERIE_DE_LIGNE"].Value = lignePion.B_CAVALERIE_DE_LIGNE;
                    ligneGrid.Cells["B_CAVALERIE_LOURDE"].Value = lignePion.B_CAVALERIE_LOURDE;
                    ligneGrid.Cells["B_GARDE"].Value = lignePion.B_GARDE;
                    ligneGrid.Cells["B_VIEILLE_GARDE"].Value = lignePion.B_VIEILLE_GARDE;
                    ligneGrid.Cells["I_TOUR_CONVOI_CREE"].Value = lignePion.I_TOUR_CONVOI_CREE;
                    ligneGrid.Cells["I_SOLDATS_RAVITAILLES"].Value = lignePion.I_SOLDATS_RAVITAILLES;
                    ligneGrid.Cells["I_NB_HEURES_FORTIFICATION"].Value = lignePion.I_NB_HEURES_FORTIFICATION;
                    ligneGrid.Cells["I_NIVEAU_FORTIFICATION"].Value = lignePion.I_NIVEAU_FORTIFICATION;
                    ligneGrid.Cells["ID_PION_REMPLACE"].Value = lignePion.IsID_PION_REMPLACENull() ? -1 : lignePion.ID_PION_REMPLACE;
                    ligneGrid.Cells["I_DUREE_HORS_COMBAT"].Value = lignePion.I_DUREE_HORS_COMBAT;
                    ligneGrid.Cells["I_TOUR_BLESSURE"].Value = lignePion.IsI_TOUR_BLESSURENull() ? -1 : lignePion.I_TOUR_BLESSURE;
                    ligneGrid.Cells["B_BLESSES"].Value = lignePion.B_BLESSES;
                    ligneGrid.Cells["B_PRISONNIERS"].Value = lignePion.B_PRISONNIERS;
                    ligneGrid.Cells["B_RENFORT"].Value = lignePion.B_RENFORT;                    
                    ligneGrid.Cells["ID_LIEU_RATTACHEMENT"].Value = lignePion.IsID_LIEU_RATTACHEMENTNull() ? -1 : lignePion.ID_LIEU_RATTACHEMENT;
                    ligneGrid.Cells["C_NIVEAU_DEPOT"].Value = lignePion.C_NIVEAU_DEPOT;                    
                    ligneGrid.Cells["ID_PION_ESCORTE"].Value = lignePion.IsID_PION_ESCORTENull() ? -1 : lignePion.ID_PION_ESCORTE;
                    ligneGrid.Cells["I_INFANTERIE_ESCORTE"].Value = lignePion.I_INFANTERIE_ESCORTE;                    
                    ligneGrid.Cells["I_CAVALERIE_ESCORTE"].Value = lignePion.I_CAVALERIE_ESCORTE;
                    ligneGrid.Cells["I_MATERIEL_ESCORTE"].Value = lignePion.I_MATERIEL_ESCORTE;
                    ligneGrid.Cells["ID_DEPOT_SOURCE"].Value = lignePion.IsID_DEPOT_SOURCENull() ? -1 : lignePion.ID_DEPOT_SOURCE;
                    ligneGrid.Cells["I_TOUR_DERNIER_RAVITAILLEMENT_DIRECT"].Value = lignePion.IsI_TOUR_DERNIER_RAVITAILLEMENT_DIRECTNull() ? -1 : lignePion.I_TOUR_DERNIER_RAVITAILLEMENT_DIRECT;
                    ligneGrid.Cells["I_VICTOIRE"].Value = lignePion.IsI_VICTOIRENull() ? -1 : lignePion.I_VICTOIRE;
                    ligneGrid.Cells["I_TRI"].Value = lignePion.IsI_TRINull() ? -1 : lignePion.I_TRI;
                    ligneGrid.Cells["I_TOUR_ENNEMI_OBSERVABLE"].Value = lignePion.IsI_TOUR_ENNEMI_OBSERVABLENull() ? -1 : lignePion.I_TOUR_ENNEMI_OBSERVABLE;
                    ligneGrid.Cells["S_ENNEMI_OBSERVABLE"].Value = lignePion.IsS_ENNEMI_OBSERVABLENull() ? string.Empty : lignePion.S_ENNEMI_OBSERVABLE;
                    ligneGrid.Cells["I_X"].Value = -1;
                    ligneGrid.Cells["I_Y"].Value = -1;
                }
                dataGridViewPions.Sort(dataGridViewPions.Columns["ID_PION"], ListSortDirection.Ascending);
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
            #region positionnement des boutons
            buttonValider.Left = (Width - buttonValider.Width - buttonAnnuler.Width - buttonRenfort.Width - buttonExportCSV.Width) / 6;
            buttonAnnuler.Left = buttonValider.Width + 2 * (Width - buttonValider.Width - buttonAnnuler.Width - buttonRenfort.Width - buttonExportCSV.Width) / 6;
            buttonRenfort.Left = buttonValider.Width + 3 * (Width - buttonValider.Width - buttonAnnuler.Width - buttonRenfort.Width - buttonExportCSV.Width) / 6;
            buttonExportCSV.Left = buttonValider.Width + 4 * (Width - buttonValider.Width - buttonAnnuler.Width - buttonRenfort.Width - buttonExportCSV.Width) / 6;
            buttonChargementXY.Left = buttonValider.Width + 5 * (Width - buttonValider.Width - buttonAnnuler.Width - buttonRenfort.Width - buttonExportCSV.Width) / 6;
            buttonValider.Top = buttonAnnuler.Top = buttonRenfort.Top = buttonExportCSV.Top = buttonChargementXY.Top = Height - 3 * buttonValider.Height;
            #endregion

            #region positionnement du champ commentaire
            labelCommentaire.Left = buttonValider.Left;
            labelCommentaire.Top = buttonAnnuler.Top - 2 * labelCommentaire.Height;
            #endregion

            #region positionnement de la grille
            dataGridViewPions.Width = Width - SystemInformation.VerticalScrollBarWidth;
            dataGridViewPions.Height = Height - 4 * buttonValider.Height - 3 * labelCommentaire.Height - SystemInformation.HorizontalScrollBarHeight;
            #endregion
        }

        private void FormPion_Load(object sender, EventArgs e)
        {
            Redimensionner();
        }
        #endregion

        private void buttonRenfort_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Action inutile, les lignes de renforts ne sont jamais supprim�es, par contre il faut supprimer les renforts arriv�s -> fait dans Initialisation", "buttonRenfort", MessageBoxButtons.OK, MessageBoxIcon.Information);
            /*
            if (dataGridViewPions.SelectedRows.Count <= 0)
            {
                MessageBox.Show("Selectionnez les lignes des unit�s que vous souhaitez mettre en renfort", "buttonRenfort", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            while (dataGridViewPions.SelectedRows.Count > 0)
            {
                DataGridViewRow ligneGrid = dataGridViewPions.SelectedRows[0];

                //recherche de l'id du mod�le de pion
                string requete = string.Format("S_NOM='{0}'", ligneGrid.Cells["MODELE_PION"].Value);
                Donnees.TAB_MODELE_PIONRow[] resDataModele = (Donnees.TAB_MODELE_PIONRow[])Donnees.m_donnees.TAB_MODELE_PION.Select(requete);

                Donnees.m_donnees.TAB_RENFORT.AddTAB_RENFORTRow(
                    Convert.ToInt32(ligneGrid.Cells["ID_PION"].Value),
                    resDataModele[0].ID_MODELE_PION,
                    Convert.ToInt32(ligneGrid.Cells["ID_PION_PROPRIETAIRE"].Value),
                    Convert.ToString(ligneGrid.Cells["S_NOM"].Value),
                    Convert.ToInt32(ligneGrid.Cells["I_INFANTERIE"].Value),
                    Convert.ToInt32(ligneGrid.Cells["I_CAVALERIE"].Value),
                    Convert.ToInt32(ligneGrid.Cells["I_ARTILLERIE"].Value),
                    Convert.ToInt32(ligneGrid.Cells["I_FATIGUE"].Value),
                    Convert.ToInt32(ligneGrid.Cells["I_MORAL"].Value),
                    Convert.ToInt32(ligneGrid.Cells["I_MORAL_MAX"].Value),
                    Convert.ToInt32(ligneGrid.Cells["I_EXPERIENCE"].Value),
                    Convert.ToInt32(ligneGrid.Cells["I_TACTIQUE"].Value),
                    Convert.ToInt32(ligneGrid.Cells["I_STRATEGIQUE"].Value),
                    Convert.ToChar(ligneGrid.Cells["C_NIVEAU_HIERARCHIQUE"].Value),
                    Convert.ToInt32(ligneGrid.Cells["ID_CASE"].Value),
                    0
                );

                dataGridViewPions.Rows.Remove(ligneGrid);//supprimer la ligne selectionn�
            }
             * */
        }

        private void buttonExportCSV_Click(object sender, EventArgs e)
        {
            string nomfichier;
            //string messageErreur = Dal.exportCSV(tablePions, out nomfichier);
            string messageErreur = Dal.exportCSV(dataGridViewPions, tablePions.TableName, out nomfichier);
            
            if (string.Empty == messageErreur)
            {
                MessageBox.Show("Fichier CSV export� : " + nomfichier, "FormPion", MessageBoxButtons.OK, MessageBoxIcon.Information);    
            }
            else
            {
                MessageBox.Show("Erreur sur l'export du fichier CSV : " + nomfichier + " : "+ messageErreur, 
                    "FormPion", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonChargementXY_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow ligneGrid in dataGridViewPions.Rows)
            {
                Donnees.TAB_PIONRow lignePion = Donnees.m_donnees.TAB_PION.FindByID_PION(Convert.ToInt32(ligneGrid.Cells["ID_PION"].Value));
                //recherche des coordonn�es de la case du pion
                if (null != lignePion && lignePion.ID_CASE >= 0)
                {
                    Donnees.TAB_CASERow ligneCase = Donnees.m_donnees.TAB_CASE.FindParID_CASE(lignePion.ID_CASE);
                    if (null != ligneCase)
                    {
                        ligneGrid.Cells["I_X"].Value = ligneCase.I_X;
                        ligneGrid.Cells["I_Y"].Value = ligneCase.I_Y;
                    }
                }
            }
            dataGridViewPions.Refresh();
        }
    }
}