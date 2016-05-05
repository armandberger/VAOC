using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace vaoc
{
    public partial class FormPhrase : Form
    {
        public FormPhrase()
        {
            InitializeComponent();
        }

        #region evenements
        private void FormPhrase_Load(object sender, EventArgs e)
        {
            Redimensionner();
        }

        private void FormPhrase_Resize(object sender, EventArgs e)
        {
            Redimensionner();
        }

        private void Redimensionner()
        {
            #region positionnement des boutons annuler et valider
            buttonValider.Left = (Width - buttonValider.Width - buttonAnnuler.Width) / 3;
            buttonAnnuler.Left = buttonValider.Width + 2 * (Width - buttonValider.Width - buttonAnnuler.Width) / 3;
            buttonValider.Top = buttonAnnuler.Top = Height - 3 * buttonValider.Height;
            #endregion

            #region positionnement de la grille
            dataGridViewPhrases.Width = Width- dataGridViewPhrases.Left*2;
            dataGridViewPhrases.Height = Height - 4 * buttonValider.Height;
            #endregion
        }
        #endregion

        #region propriétés
        public Donnees.TAB_PHRASEDataTable tablePhrase
        {
            get
            {
                int i_tipe;

                Donnees.TAB_PHRASEDataTable table = new Donnees.TAB_PHRASEDataTable();
                foreach (DataGridViewRow ligne in this.dataGridViewPhrases.Rows)
                {
                    if (null != ligne.Cells["I_TYPE"].Value)//cas pour la nouvelle ligne ajoutée automatiquement
                    {
                        i_tipe = -1;
                        foreach (ClassMessager.MESSAGES tipeMessage in Enum.GetValues(typeof(ClassMessager.MESSAGES)))
                        {
                            //this.I_TYPE.Items.Add(tipeMessage.ToString());
                            if (ligne.Cells["I_TYPE"].Value.Equals(tipeMessage.ToString())) 
                            { 
                                i_tipe = (int)tipeMessage;
                                break;
                            }
                        }
                        /*
                        if (ligne.Cells["I_TYPE"].Value.Equals("MESSAGE_FUITE_AU_COMBAT")){i_tipe = ClassMessager.MESSAGE_FUITE_AU_COMBAT;}
                        if (ligne.Cells["I_TYPE"].Value.Equals("MESSAGE_ARRIVE_A_DESTINATION")){i_tipe = ClassMessager.MESSAGE_ARRIVE_A_DESTINATION;}
                        if (ligne.Cells["I_TYPE"].Value.Equals("MESSAGE_FUITE_ET_DETRUITE_AU_COMBAT")) { i_tipe = ClassMessager.MESSAGE_FUITE_ET_DETRUITE_AU_COMBAT; }
                        if (ligne.Cells["I_TYPE"].Value.Equals("MESSAGE_PERTE_MORAL_AU_COMBAT")) { i_tipe = ClassMessager.MESSAGE_PERTE_MORAL_AU_COMBAT; }
                        if (ligne.Cells["I_TYPE"].Value.Equals("MESSAGE_AUCUNE_POURSUITE_POSSIBLE")) { i_tipe = ClassMessager.MESSAGE_AUCUNE_POURSUITE_POSSIBLE; }
                        if (ligne.Cells["I_TYPE"].Value.Equals("MESSAGE_AUCUNE_POURSUITE_RECUE")) { i_tipe = ClassMessager.MESSAGE_AUCUNE_POURSUITE_RECUE; }
                        if (ligne.Cells["I_TYPE"].Value.Equals("MESSAGE_POURSUITE_SANS_EFFET")) { i_tipe = ClassMessager.MESSAGE_POURSUITE_SANS_EFFET; }
                        if (ligne.Cells["I_TYPE"].Value.Equals("MESSAGE_DETRUIT_PAR_POURSUITE")) { i_tipe = ClassMessager.MESSAGE_DETRUIT_PAR_POURSUITE; }
                        if (ligne.Cells["I_TYPE"].Value.Equals("MESSAGE_POURSUITE_DESTRUCTION_TOTALE")) { i_tipe = ClassMessager.MESSAGE_POURSUITE_DESTRUCTION_TOTALE; }
                        if (ligne.Cells["I_TYPE"].Value.Equals("MESSAGE_POURSUITE_PERTES_POURSUIVANT")) { i_tipe = ClassMessager.MESSAGE_POURSUITE_PERTES_POURSUIVANT; }
                        if (ligne.Cells["I_TYPE"].Value.Equals("MESSAGE_POURSUITE_PERTES_POURSUIVI")) { i_tipe = ClassMessager.MESSAGE_POURSUITE_PERTES_POURSUIVI; }
                        if (ligne.Cells["I_TYPE"].Value.Equals("MESSAGE_GAIN_MORAL_MAITRE_TERRAIN")) { i_tipe = ClassMessager.MESSAGE_GAIN_MORAL_MAITRE_TERRAIN; }
                        if (ligne.Cells["I_TYPE"].Value.Equals("MESSAGE_FIN_DE_BATAILLE_A_LA_NUIT")) { i_tipe = ClassMessager.MESSAGE_FIN_DE_BATAILLE_A_LA_NUIT; }
                        if (ligne.Cells["I_TYPE"].Value.Equals("MESSAGE_FIN_DE_BATAILLE_FAUTE_DE_COMBATTANT")) { i_tipe = ClassMessager.MESSAGE_FIN_DE_BATAILLE_FAUTE_DE_COMBATTANT; }
                        if (ligne.Cells["I_TYPE"].Value.Equals("MESSAGE_BILAN_REPOS_MORAL_ET_FATIGUE")) { i_tipe = ClassMessager.MESSAGE_BILAN_REPOS_MORAL_ET_FATIGUE; }
                        if (ligne.Cells["I_TYPE"].Value.Equals("MESSAGE_BILAN_REPOS_MORAL")) { i_tipe = ClassMessager.MESSAGE_BILAN_REPOS_MORAL; }
                        if (ligne.Cells["I_TYPE"].Value.Equals("MESSAGE_BILAN_REPOS_FATIGUE")) { i_tipe = ClassMessager.MESSAGE_BILAN_REPOS_FATIGUE; }
                        if (ligne.Cells["I_TYPE"].Value.Equals("MESSAGE_BILAN_REPOS")) { i_tipe = ClassMessager.MESSAGE_BILAN_REPOS; }
                        if (ligne.Cells["I_TYPE"].Value.Equals("MESSAGE_BILAN_ACTION")) { i_tipe = ClassMessager.MESSAGE_BILAN_ACTION; }
                        if (ligne.Cells["I_TYPE"].Value.Equals("MESSAGE_DETRUITE_AU_COMBAT")) { i_tipe = ClassMessager.MESSAGE_DETRUITE_AU_COMBAT; }
                        if (ligne.Cells["I_TYPE"].Value.Equals("MESSAGE_AUCUN_MESSAGE")) { i_tipe = ClassMessager.MESSAGE_AUCUN_MESSAGE; }
                        if (ligne.Cells["I_TYPE"].Value.Equals("MESSAGE_PERTES_AU_COMBAT")) { i_tipe = ClassMessager.MESSAGE_PERTES_AU_COMBAT; }
                        if (ligne.Cells["I_TYPE"].Value.Equals("MESSAGE_DEPART_VERS_DESTINATION")) { i_tipe = ClassMessager.MESSAGE_DEPART_VERS_DESTINATION; }
                        if (ligne.Cells["I_TYPE"].Value.Equals("MESSAGE_PATROUILLE_CONTACT_ENNEMI")) { i_tipe = ClassMessager.MESSAGE_PATROUILLE_CONTACT_ENNEMI; }
                        if (ligne.Cells["I_TYPE"].Value.Equals("MESSAGE_PATROUILLE_RAPPORT")) { i_tipe = ClassMessager.MESSAGE_PATROUILLE_RAPPORT; }
                        if (ligne.Cells["I_TYPE"].Value.Equals("MESSAGE_CHEMIN_BLOQUE")) { i_tipe = ClassMessager.MESSAGE_CHEMIN_BLOQUE; }
                        if (ligne.Cells["I_TYPE"].Value.Equals("MESSAGE_ORDRE_MOUVEMENT_RECU")) { i_tipe = ClassMessager.MESSAGE_ORDRE_MOUVEMENT_RECU; }
                        if (ligne.Cells["I_TYPE"].Value.Equals("MESSAGE_RENFORT")) { i_tipe = ClassMessager.MESSAGE_RENFORT; }
                        if (ligne.Cells["I_TYPE"].Value.Equals("MESSAGE_ORDRE_MOUVEMENT_IMPOSSIBLE_AU_COMBAT")) { i_tipe = ClassMessager.MESSAGE_ORDRE_MOUVEMENT_IMPOSSIBLE_AU_COMBAT; }
                        if (ligne.Cells["I_TYPE"].Value.Equals("MESSAGE_ORDRE_PATROUILLE_RECU")) { i_tipe = ClassMessager.MESSAGE_ORDRE_PATROUILLE_RECU; }
                        if (ligne.Cells["I_TYPE"].Value.Equals("MESSAGE_BRUIT_DU_CANON")) { i_tipe = ClassMessager.MESSAGE_BRUIT_DU_CANON; }
                        if (ligne.Cells["I_TYPE"].Value.Equals("MESSAGE_ARRIVEE_DANS_BATAILLE")) { i_tipe = ClassMessager.MESSAGE_ARRIVEE_DANS_BATAILLE; }
                        if (ligne.Cells["I_TYPE"].Value.Equals("MESSAGE_ENGAGEMENT_AUTOMATIQUE_DANS_BATAILLE")) { i_tipe = ClassMessager.MESSAGE_ENGAGEMENT_AUTOMATIQUE_DANS_BATAILLE; }
                        if (ligne.Cells["I_TYPE"].Value.Equals("MESSAGE_VICTOIRE_EN_BATAILLE")) { i_tipe = ClassMessager.MESSAGE_VICTOIRE_EN_BATAILLE; }
                        if (ligne.Cells["I_TYPE"].Value.Equals("MESSAGE_DEFAITE_EN_BATAILLE")) { i_tipe = ClassMessager.MESSAGE_DEFAITE_EN_BATAILLE; }
                        if (ligne.Cells["I_TYPE"].Value.Equals("MESSAGE_INTERCEPTION")) { i_tipe = ClassMessager.MESSAGE_INTERCEPTION; }
                        if (ligne.Cells["I_TYPE"].Value.Equals("MESSAGE_TIR_SUR_ENNEMI")) { i_tipe = ClassMessager.MESSAGE_TIR_SUR_ENNEMI; }
                        if (ligne.Cells["I_TYPE"].Value.Equals("MESSAGE_EST_DETRUIT_AU_CONTACT")) { i_tipe = ClassMessager.MESSAGE_EST_DETRUIT_AU_CONTACT; }
                        if (ligne.Cells["I_TYPE"].Value.Equals("MESSAGE_A_DETRUIT_AU_CONTACT")) { i_tipe = ClassMessager.MESSAGE_A_DETRUIT_AU_CONTACT; }
                        if (ligne.Cells["I_TYPE"].Value.Equals("MESSAGE_SANS_RAVITAILLEMENT")) { i_tipe = ClassMessager.MESSAGE_SANS_RAVITAILLEMENT; }
                        if (ligne.Cells["I_TYPE"].Value.Equals("MESSAGE_NOUVEAU_RAVITAILLEMENT")) { i_tipe = ClassMessager.MESSAGE_NOUVEAU_RAVITAILLEMENT; }
                        if (ligne.Cells["I_TYPE"].Value.Equals("MESSAGE_REDITION_POUR_RAVITAILLEMENT")) { i_tipe = ClassMessager.MESSAGE_NOUVEAU_RAVITAILLEMENT; }
                        if (ligne.Cells["I_TYPE"].Value.Equals("MESSAGE_EST_CAPTURE")) { i_tipe = ClassMessager.MESSAGE_EST_CAPTURE; }
                        if (ligne.Cells["I_TYPE"].Value.Equals("MESSAGE_A_CAPTURE")) { i_tipe = ClassMessager.MESSAGE_A_CAPTURE; }
                        */
                        table.AddTAB_PHRASERow(
                            //Convert.ToInt32(ligne.Cells["ID_PHRASE"].Value),
                            i_tipe,
                            Convert.ToString(ligne.Cells["S_PHRASE"].Value)
                            );
                    }
                }
                return table;
            }
            set
            {
                //mise à jour de la liste déroulante des phrases disponibles
                foreach (ClassMessager.MESSAGES tipeMessage in Enum.GetValues(typeof(ClassMessager.MESSAGES)))
                {
                    this.I_TYPE.Items.Add(tipeMessage.ToString());
                }
                this.I_TYPE.Items.Add("MESSAGE_AUCUN_MESSAGE");
                /*
                this.I_TYPE.Items.Add("MESSAGE_FUITE_AU_COMBAT");
                this.I_TYPE.Items.Add("MESSAGE_DEPART_VERS_DESTINATION");
                this.I_TYPE.Items.Add("MESSAGE_ARRIVE_A_DESTINATION");
                this.I_TYPE.Items.Add("MESSAGE_FUITE_ET_DETRUITE_AU_COMBAT");
                this.I_TYPE.Items.Add("MESSAGE_PERTE_MORAL_AU_COMBAT");
                this.I_TYPE.Items.Add("MESSAGE_AUCUNE_POURSUITE_POSSIBLE");
                this.I_TYPE.Items.Add("MESSAGE_AUCUNE_POURSUITE_RECUE");
                this.I_TYPE.Items.Add("MESSAGE_POURSUITE_SANS_EFFET");
                this.I_TYPE.Items.Add("MESSAGE_DETRUIT_PAR_POURSUITE");
                this.I_TYPE.Items.Add("MESSAGE_POURSUITE_DESTRUCTION_TOTALE");
                this.I_TYPE.Items.Add("MESSAGE_POURSUITE_PERTES_POURSUIVANT");
                this.I_TYPE.Items.Add("MESSAGE_POURSUITE_PERTES_POURSUIVI");
                this.I_TYPE.Items.Add("MESSAGE_GAIN_MORAL_MAITRE_TERRAIN");
                this.I_TYPE.Items.Add("MESSAGE_FIN_DE_BATAILLE_A_LA_NUIT");
                this.I_TYPE.Items.Add("MESSAGE_FIN_DE_BATAILLE_FAUTE_DE_COMBATTANT");
                this.I_TYPE.Items.Add("MESSAGE_BILAN_REPOS_MORAL_ET_FATIGUE");
                this.I_TYPE.Items.Add("MESSAGE_BILAN_REPOS_MORAL");
                this.I_TYPE.Items.Add("MESSAGE_BILAN_REPOS_FATIGUE");
                this.I_TYPE.Items.Add("MESSAGE_BILAN_REPOS");
                this.I_TYPE.Items.Add("MESSAGE_BILAN_ACTION");
                this.I_TYPE.Items.Add("MESSAGE_PERTES_AU_COMBAT");
                this.I_TYPE.Items.Add("MESSAGE_DETRUITE_AU_COMBAT");
                this.I_TYPE.Items.Add("MESSAGE_PATROUILLE_CONTACT_ENNEMI");
                this.I_TYPE.Items.Add("MESSAGE_PATROUILLE_RAPPORT");
                this.I_TYPE.Items.Add("MESSAGE_CHEMIN_BLOQUE");
                this.I_TYPE.Items.Add("MESSAGE_ORDRE_MOUVEMENT_RECU");
                this.I_TYPE.Items.Add("MESSAGE_RENFORT");
                this.I_TYPE.Items.Add("MESSAGE_ORDRE_MOUVEMENT_IMPOSSIBLE_AU_COMBAT");
                this.I_TYPE.Items.Add("MESSAGE_ORDRE_PATROUILLE_RECU");
                this.I_TYPE.Items.Add("MESSAGE_BRUIT_DU_CANON");
                this.I_TYPE.Items.Add("MESSAGE_ARRIVEE_DANS_BATAILLE");
                this.I_TYPE.Items.Add("MESSAGE_ENGAGEMENT_AUTOMATIQUE_DANS_BATAILLE");
                this.I_TYPE.Items.Add("MESSAGE_VICTOIRE_EN_BATAILLE");
                this.I_TYPE.Items.Add("MESSAGE_DEFAITE_EN_BATAILLE");
                this.I_TYPE.Items.Add("MESSAGE_INTERCEPTION");
                this.I_TYPE.Items.Add("MESSAGE_TIR_SUR_ENNEMI");
                this.I_TYPE.Items.Add("MESSAGE_EST_DETRUIT_AU_CONTACT");
                this.I_TYPE.Items.Add("MESSAGE_A_DETRUIT_AU_CONTACT");
                this.I_TYPE.Items.Add("MESSAGE_SANS_RAVITAILLEMENT");
                this.I_TYPE.Items.Add("MESSAGE_NOUVEAU_RAVITAILLEMENT");
                this.I_TYPE.Items.Add("MESSAGE_REDITION_POUR_RAVITAILLEMENT");
                this.I_TYPE.Items.Add("MESSAGE_EST_CAPTURE");
                this.I_TYPE.Items.Add("MESSAGE_A_CAPTURE");
                */
                
                foreach (Donnees.TAB_PHRASERow lignePhrase in value)
                {
                    DataGridViewRow ligneGrid = dataGridViewPhrases.Rows[dataGridViewPhrases.Rows.Add()];
                    ligneGrid.Cells["ID_PHRASE"].Value = lignePhrase.ID_PHRASE;
                    ligneGrid.Cells["S_PHRASE"].Value = lignePhrase.S_PHRASE;
                    //recherche du nom de l'id de phrase 
                    foreach (ClassMessager.MESSAGES tipeMessage in Enum.GetValues(typeof(ClassMessager.MESSAGES)))
                    {
                        if (lignePhrase.I_TYPE == (int)tipeMessage)
                        {
                            ligneGrid.Cells["I_TYPE"].Value = tipeMessage.ToString();
                        }
                    }
                    /*
                    switch (lignePhrase.I_TYPE)
                    {
                        case ClassMessager.MESSAGE_FUITE_AU_COMBAT:
                            ligneGrid.Cells["I_TYPE"].Value = "MESSAGE_FUITE_AU_COMBAT";
                            break;
                        case ClassMessager.MESSAGE_ARRIVE_A_DESTINATION:
                            ligneGrid.Cells["I_TYPE"].Value = "MESSAGE_ARRIVE_A_DESTINATION";
                            break;
                        case ClassMessager.MESSAGE_FUITE_ET_DETRUITE_AU_COMBAT:
                            ligneGrid.Cells["I_TYPE"].Value = "MESSAGE_FUITE_ET_DETRUITE_AU_COMBAT";
                            break;
                        case ClassMessager.MESSAGE_PERTE_MORAL_AU_COMBAT:
                            ligneGrid.Cells["I_TYPE"].Value = "MESSAGE_PERTE_MORAL_AU_COMBAT";
                            break;
                        case ClassMessager.MESSAGE_AUCUNE_POURSUITE_POSSIBLE:
                            ligneGrid.Cells["I_TYPE"].Value = "MESSAGE_AUCUNE_POURSUITE_POSSIBLE";
                            break;
                        case ClassMessager.MESSAGE_AUCUNE_POURSUITE_RECUE:
                            ligneGrid.Cells["I_TYPE"].Value = "MESSAGE_AUCUNE_POURSUITE_RECUE";
                            break;
                        case ClassMessager.MESSAGE_POURSUITE_SANS_EFFET:
                            ligneGrid.Cells["I_TYPE"].Value = "MESSAGE_POURSUITE_SANS_EFFET";
                            break;
                        case ClassMessager.MESSAGE_DETRUIT_PAR_POURSUITE:
                            ligneGrid.Cells["I_TYPE"].Value = "MESSAGE_DETRUIT_PAR_POURSUITE";
                            break;
                        case ClassMessager.MESSAGE_POURSUITE_DESTRUCTION_TOTALE:
                            ligneGrid.Cells["I_TYPE"].Value = "MESSAGE_POURSUITE_DESTRUCTION_TOTALE";
                            break;
                        case ClassMessager.MESSAGE_POURSUITE_PERTES_POURSUIVANT:
                            ligneGrid.Cells["I_TYPE"].Value = "MESSAGE_POURSUITE_PERTES_POURSUIVANT";
                            break;
                        case ClassMessager.MESSAGE_POURSUITE_PERTES_POURSUIVI:
                            ligneGrid.Cells["I_TYPE"].Value = "MESSAGE_POURSUITE_PERTES_POURSUIVI";
                            break;
                        case ClassMessager.MESSAGE_GAIN_MORAL_MAITRE_TERRAIN:
                            ligneGrid.Cells["I_TYPE"].Value = "MESSAGE_GAIN_MORAL_MAITRE_TERRAIN";
                            break;
                        case ClassMessager.MESSAGE_FIN_DE_BATAILLE_A_LA_NUIT:
                            ligneGrid.Cells["I_TYPE"].Value = "MESSAGE_FIN_DE_BATAILLE_A_LA_NUIT";
                            break;
                        case ClassMessager.MESSAGE_BILAN_REPOS_MORAL_ET_FATIGUE:
                            ligneGrid.Cells["I_TYPE"].Value = "MESSAGE_BILAN_REPOS_MORAL_ET_FATIGUE";
                            break;
                        case ClassMessager.MESSAGE_BILAN_REPOS_MORAL:
                            ligneGrid.Cells["I_TYPE"].Value = "MESSAGE_BILAN_REPOS_MORAL";
                            break;
                        case ClassMessager.MESSAGE_BILAN_REPOS_FATIGUE:
                            ligneGrid.Cells["I_TYPE"].Value = "MESSAGE_BILAN_REPOS_FATIGUE";
                            break;
                        case ClassMessager.MESSAGE_BILAN_REPOS:
                            ligneGrid.Cells["I_TYPE"].Value = "MESSAGE_BILAN_REPOS";
                            break;
                        case ClassMessager.MESSAGE_BILAN_ACTION:
                            ligneGrid.Cells["I_TYPE"].Value = "MESSAGE_BILAN_ACTION";
                            break;
                        case ClassMessager.MESSAGE_PERTES_AU_COMBAT:
                            ligneGrid.Cells["I_TYPE"].Value = "MESSAGE_PERTES_AU_COMBAT";
                            break;
                        case ClassMessager.MESSAGE_DETRUITE_AU_COMBAT:
                            ligneGrid.Cells["I_TYPE"].Value = "MESSAGE_DETRUITE_AU_COMBAT";
                            break;                            
                        case ClassMessager.MESSAGE_AUCUN_MESSAGE:
                            ligneGrid.Cells["I_TYPE"].Value = "MESSAGE_AUCUN_MESSAGE";
                            break;
                        case ClassMessager.MESSAGE_FIN_DE_BATAILLE_FAUTE_DE_COMBATTANT:
                            ligneGrid.Cells["I_TYPE"].Value = "MESSAGE_FIN_DE_BATAILLE_FAUTE_DE_COMBATTANT";
                            break;
                        case ClassMessager.MESSAGE_DEPART_VERS_DESTINATION:
                            ligneGrid.Cells["I_TYPE"].Value = "MESSAGE_DEPART_VERS_DESTINATION";
                            break;
                        case ClassMessager.MESSAGE_PATROUILLE_CONTACT_ENNEMI:
                            ligneGrid.Cells["I_TYPE"].Value = "MESSAGE_PATROUILLE_CONTACT_ENNEMI";
                            break;
                        case ClassMessager.MESSAGE_PATROUILLE_RAPPORT:
                            ligneGrid.Cells["I_TYPE"].Value = "MESSAGE_PATROUILLE_RAPPORT";
                            break;
                        case ClassMessager.MESSAGE_CHEMIN_BLOQUE:
                            ligneGrid.Cells["I_TYPE"].Value = "MESSAGE_CHEMIN_BLOQUE";
                            break;
                        case ClassMessager.MESSAGE_ORDRE_MOUVEMENT_RECU:
                            ligneGrid.Cells["I_TYPE"].Value = "MESSAGE_ORDRE_MOUVEMENT_RECU";
                            break;
                        case ClassMessager.MESSAGE_RENFORT:
                            ligneGrid.Cells["I_TYPE"].Value = "MESSAGE_RENFORT";
                            break;
                        case ClassMessager.MESSAGE_ORDRE_MOUVEMENT_IMPOSSIBLE_AU_COMBAT:
                            ligneGrid.Cells["I_TYPE"].Value = "MESSAGE_ORDRE_MOUVEMENT_IMPOSSIBLE_AU_COMBAT";
                            break;
                        case ClassMessager.MESSAGE_ORDRE_PATROUILLE_RECU:
                            ligneGrid.Cells["I_TYPE"].Value = "MESSAGE_ORDRE_PATROUILLE_RECU";
                            break;
                        case ClassMessager.MESSAGE_BRUIT_DU_CANON:
                            ligneGrid.Cells["I_TYPE"].Value = "MESSAGE_BRUIT_DU_CANON";
                            break;
                        case ClassMessager.MESSAGE_ARRIVEE_DANS_BATAILLE:
                            ligneGrid.Cells["I_TYPE"].Value = "MESSAGE_ARRIVEE_DANS_BATAILLE";
                            break;
                        case ClassMessager.MESSAGE_ENGAGEMENT_AUTOMATIQUE_DANS_BATAILLE:
                            ligneGrid.Cells["I_TYPE"].Value = "MESSAGE_ENGAGEMENT_AUTOMATIQUE_DANS_BATAILLE";
                            break;
                        case ClassMessager.MESSAGE_VICTOIRE_EN_BATAILLE:
                            ligneGrid.Cells["I_TYPE"].Value = "MESSAGE_VICTOIRE_EN_BATAILLE";
                            break;
                        case ClassMessager.MESSAGE_DEFAITE_EN_BATAILLE:
                            ligneGrid.Cells["I_TYPE"].Value = "MESSAGE_DEFAITE_EN_BATAILLE";
                            break;
                        case ClassMessager.MESSAGE_INTERCEPTION:
                            ligneGrid.Cells["I_TYPE"].Value = "MESSAGE_INTERCEPTION";
                            break;
                        case ClassMessager.MESSAGE_TIR_SUR_ENNEMI:
                            ligneGrid.Cells["I_TYPE"].Value = "MESSAGE_TIR_SUR_ENNEMI";
                            break;
                        case ClassMessager.MESSAGE_EST_DETRUIT_AU_CONTACT:
                            ligneGrid.Cells["I_TYPE"].Value = "MESSAGE_EST_DETRUIT_AU_CONTACT";
                            break;
                        case ClassMessager.MESSAGE_A_DETRUIT_AU_CONTACT:
                            ligneGrid.Cells["I_TYPE"].Value = "MESSAGE_A_DETRUIT_AU_CONTACT";
                            break;
                        case ClassMessager.MESSAGE_SANS_RAVITAILLEMENT:
                            ligneGrid.Cells["I_TYPE"].Value = "MESSAGE_SANS_RAVITAILLEMENT";
                            break;
                        case ClassMessager.MESSAGE_NOUVEAU_RAVITAILLEMENT:
                            ligneGrid.Cells["I_TYPE"].Value = "MESSAGE_NOUVEAU_RAVITAILLEMENT";
                            break;
                        case ClassMessager.MESSAGE_REDITION_POUR_RAVITAILLEMENT:
                            ligneGrid.Cells["I_TYPE"].Value = "MESSAGE_REDITION_POUR_RAVITAILLEMENT";
                            break;                            
                        case ClassMessager.MESSAGE_EST_CAPTURE:
                            ligneGrid.Cells["I_TYPE"].Value = "MESSAGE_EST_CAPTURE";
                            break;                            
                        case ClassMessager.MESSAGE_A_CAPTURE:
                            ligneGrid.Cells["I_TYPE"].Value = "MESSAGE_A_CAPTURE";
                            break;                            
                        default:
                            //ne devrait jamais arrivé
                            ligneGrid.Cells["I_TYPE"].Value = "INCONNU";//crash peut-être car non défini à priori
                            break;
                    }
                     * */
                }
            }
        }
        #endregion
    }
}
