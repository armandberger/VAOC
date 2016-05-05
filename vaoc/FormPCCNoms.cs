using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using WaocLib;

namespace vaoc
{
    public partial class FormPCCNoms : Form
    {
        #region données
        private Cursor m_oldcurseur;
        private string m_fichierCourant;
        private int m_progressStart;//nombre de valeurs déjà traitées au demmarage du traitement
        private int m_initEtape;//etape de départ;
        private DateTime m_dateDebut;
        #endregion

        #region propriétés

        public string fichierCourant
        {
            set { m_fichierCourant=value; }
            get { return m_fichierCourant; }
        }

        public int tailleVille
        {
		    get { return Convert.ToInt16(textBoxTailleVille.Text);}
            set { textBoxTailleVille.Text = value.ToString(); }
        }

        public int distanceVilleMax
        {
            get { return Convert.ToInt16(this.textBoxDistanceMax.Text); }
            set { this.textBoxDistanceMax.Text = value.ToString(); }
        }
        #endregion

        public FormPCCNoms()
        {
            InitializeComponent();
        }

        private void AfficherTemps()
        {
            TimeSpan tPasse=DateTime.Now.Subtract(m_dateDebut);
            long nbrestant;//nombres de secondes restantes pour finir le traitement

            if (progressBar.Value - m_progressStart > 0)
            {
                nbrestant = (long)(tPasse.TotalSeconds * (progressBar.Maximum - progressBar.Value) / (progressBar.Value - m_progressStart));
            }
            else
            {
                nbrestant = (long)(tPasse.TotalSeconds * (progressBar.Maximum - m_progressStart));
            }

            if (tPasse.TotalHours >= 1)
            {
                labelTempsPasse.Text = String.Format("{0} heures {1} minutes", Math.Floor(tPasse.TotalHours), tPasse.Minutes);
            }
            else
            {
                if (tPasse.TotalMinutes >= 1)
                {
                    labelTempsPasse.Text = String.Format("{0} minutes", tPasse.Minutes);
                }
                else
                {
                    labelTempsPasse.Text = String.Format("{0} secondes", tPasse.Seconds);
                }
            }
            
            if (nbrestant > 3600)
            {
                labelTempsRestant.Text = String.Format("{0} heures {1} minutes", nbrestant / 3600, (nbrestant % 3600)/60);
            }
            else
            {
                if (nbrestant > 60)
                {
                    labelTempsRestant.Text = String.Format("{0} minutes", nbrestant / 60);
                }
                else
                {
                    labelTempsRestant.Text = String.Format("{0} secondes", nbrestant);
                }
            }
        }

        private void buttonPCC_Click(object sender, EventArgs e)
        {
            FormPCCNomsChoix.PCCNomsChoix bChoix;

            if (backgroundTraitement.IsBusy)
            {
                Cursor = m_oldcurseur;
                if (DialogResult.Yes == MessageBox.Show("Voulez vous suspendre le traitement en cours ?", "Génération PCC", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    backgroundTraitement.CancelAsync();
                }
                return;
            }
            else
            {
                //vérifie que tous les modèles de mouvements ont le même coût pour le terrain
                //pour une même météo, pour un même terrain, le coût doit être le même pour tous les modèles de mouvement
                //si ce n'est pas le cas, il faut faire une table PCC hierarchique par modèle de mouvement ce qui 
                //occuperait beaucoup plus d'espace mémoire et disque.
                foreach (Donnees.TAB_MODELE_TERRAINRow ligneModeleTerrain in Donnees.m_donnees.TAB_MODELE_TERRAIN)
                {
                    foreach (Donnees.TAB_METEORow ligneMeteo in Donnees.m_donnees.TAB_METEO)
                    {
                        int cout = -100;
                        foreach (Donnees.TAB_MODELE_MOUVEMENTRow ligneModeleMouvement in Donnees.m_donnees.TAB_MODELE_MOUVEMENT)
                        {
                            int coutLocal = Donnees.m_donnees.TAB_MOUVEMENT_COUT.CalculCout(ligneMeteo.ID_METEO,
                                ligneModeleTerrain.ID_MODELE_TERRAIN, ligneModeleMouvement.ID_MODELE_MOUVEMENT);
                            if (cout < 0)
                            {
                                cout = coutLocal;
                            }
                            else
                            {
                                if (cout != coutLocal)
                                {
                                    MessageBox.Show(string.Format("Ecart de cout {0}<>{1} pour modele_mouvement={2}, modele_terrain={3}, meteo={4}. PCC hierarchique impossible",
                                        cout,
                                        coutLocal,
                                        ligneModeleMouvement.ID_MODELE_MOUVEMENT,
                                        ligneModeleTerrain.ID_MODELE_TERRAIN,
                                        ligneMeteo.ID_METEO),
                                        "PCC Villes", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                    return;
                                }
                            }
                        }
                    }
                }

                bChoix = FormPCCNomsChoix.PCCNomsChoix.VERIFICATION;
                Donnees.m_donnees.TAB_JEU[0].I_ZONEVILLE_PCC = tailleVille;
                Donnees.m_donnees.TAB_JEU[0].I_DISTANCEVILLEMAX_PCC = distanceVilleMax;
                if (Donnees.m_donnees.TAB_PCC_VILLES.Count > 0)
                {
                    FormPCCNomsChoix formChoix = new FormPCCNomsChoix();
                    formChoix.ShowDialog(this);
                    bChoix = formChoix.choix;
                    //DialogResult retourMessage = MessageBox.Show("Voulez vous reprendre le traitement à partir de zéro (sinon reprend à partir du point d'arrêt)", "Traitement PCC", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                    //if (DialogResult.No == retourMessage)
                    //{
                    //    bNouveau = false;
                    //}
                }

                if (FormPCCNomsChoix.PCCNomsChoix.INITIAL == bChoix)
                {
                    Donnees.m_donnees.TAB_PCC_CASE_BLOCS.Clear();//pour HPA, détruit par sureté
                    Donnees.m_donnees.TAB_PCC_COUTS.Clear();//pour HPA, détruit par sureté
                    Donnees.m_donnees.TAB_PCC_TRAJET.Clear();//inutilisé car trop gourmand en mémoire, detruit par sureté
                    Donnees.m_donnees.TAB_PCC_VILLES.Clear();
                    Donnees.m_donnees.TAB_PCC_VILLES_INUTILE.Clear();
                    Donnees.m_donnees.TAB_PCC_VILLES_PROGRESSION.Clear();
                    Donnees.m_donnees.TAB_PCC_VILLES_PROGRESSION.AddTAB_PCC_VILLES_PROGRESSIONRow(0, 0);
                    foreach (Donnees.TAB_CASERow ligneCase in Donnees.m_donnees.TAB_CASE)
                        ligneCase.SetID_NOMNull();

                    //Donnees.m_donnees.TAB_NOMS_TRAJET.Clear(); inutilisé, trop lourd en base, sous forme de fichiers
                    m_progressStart = 0;
                }
                else
                {
                    /* ce qui suit ne marche plus car les villes intermédiaires peuvent avoir n'importe quel identifiant*/
                    //int idTrajet = (int)Donnees.m_donnees.TAB_PCC_VILLES.Compute("MAX(ID_TRAJET)", "");//recherche du dernier trajet cree
                    if (FormPCCNomsChoix.PCCNomsChoix.VERIFICATION == bChoix)
                    {
                        m_progressStart = 0;
                        MessageBox.Show("Mode de traitement probablement sans intérêt, s'il y a une erreur dans les trajets, il faut tout recommencer.", "Traitement PCC", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return;
                    }
                    else
                    {
                        //recherche de la ville source en cours
                        int idVille = Donnees.m_donnees.TAB_PCC_VILLES_PROGRESSION[0].ID_VILLE_DEBUT;
                        int i = 0;
                        while (idVille != Donnees.m_donnees.TAB_NOMS_CARTE[i].ID_NOM) i++;
                        idVille = Donnees.m_donnees.TAB_PCC_VILLES_PROGRESSION[0].ID_VILLE_FIN;
                        int j = 0;
                        while (idVille != Donnees.m_donnees.TAB_NOMS_CARTE[j].ID_NOM) j++;
                        m_progressStart = (Donnees.m_donnees.TAB_NOMS_CARTE.Count * i + j + 1) / 2; //+1 car on va commencer à la ville suivante
                    }
                }
                progressBar.Maximum = (Donnees.m_donnees.TAB_NOMS_CARTE.Count * Donnees.m_donnees.TAB_NOMS_CARTE.Count + Donnees.m_donnees.TAB_NOMS_CARTE.Count) / 2;//somme arithmétique de raison 1
            }
            
            //lancement du traitement
            this.buttonValider.Enabled = false;
            this.buttonPCC.Text = "Arrêt";
            m_oldcurseur = Cursor;
            Cursor = Cursors.WaitCursor;
            m_dateDebut = DateTime.Now;
            progressBar.Value = 0;
            Invalidate();
            m_initEtape = 0;
            backgroundTraitement.RunWorkerAsync();
        }

        //private void timerTraitementPCC_Tick(object sender, EventArgs e)
        //{
        //    if (!TraitementPCC())
        //    {
        //        timerTraitementPCC.Enabled = false;
        //        Cursor = m_oldcurseur;
        //    }
        //}

        private void backgroundTraitement_DoWork(object sender, DoWorkEventArgs e)
        {
            ClassPCCVillesCreation traitementPCCVilles = new ClassPCCVillesCreation();
            BackgroundWorker travailleur = sender as BackgroundWorker;
            bool bTraitement = true;
            try
            {
                if (!traitementPCCVilles.Initialisation(fichierCourant, travailleur, m_initEtape))
                {
                    e.Cancel = true;
                    return;
                }

                while (bTraitement)
                {
                    if (travailleur.CancellationPending)
                    {
                        e.Cancel = true;
                        bTraitement = false;
                    }
                    else
                    {
                        bTraitement = traitementPCCVilles.Traitement();
                    }
                }
            }
            catch (Exception ex)
            {
                travailleur.CancelAsync();
                MessageBox.Show(ex.Message);
            }

        }

        private void backgroundTraitement_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                if (null!=e)
                {
                    PCCVillesStatut statut = (PCCVillesStatut)e.UserState;
                    if (statut.m_statut.Length>0)
                        labelTraitement.Text = statut.m_statut;
                    if (statut.m_positionSource >= 0 && statut.m_positionDestination >= 0)
                    {
                        this.labelNbVillesSource.Text = statut.m_positionSource + " / " + Donnees.m_donnees.TAB_NOMS_CARTE.Count;
                        this.labelNbVillesDestination.Text = statut.m_positionDestination + " / " + Donnees.m_donnees.TAB_NOMS_CARTE.Count;
                        progressBar.Value = (statut.m_positionSource * Donnees.m_donnees.TAB_NOMS_CARTE.Count + statut.m_positionDestination)/2;
                    }
                }
                AfficherTemps();
            }
            catch (Exception ex)
            {
                BackgroundWorker travailleur = sender as BackgroundWorker;
                travailleur.CancelAsync();
                MessageBox.Show(ex.Message);
            }
        }

        private void backgroundTraitement_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                Cursor = m_oldcurseur;
                this.buttonValider.Enabled = true;
                this.buttonPCC.Enabled = false;
                this.buttonPCC.Text = "Fin de Generation du PCC Villes";
                Invalidate();
                MessageBox.Show(string.Format("Nb lignes de couts = {0}", 
                    Donnees.m_donnees.TAB_PCC_VILLES.Count.ToString()));
                if (null != e.Error)
                {
                    MessageBox.Show("Erreur durant le traitement :" + e.Error.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (e.Cancelled)
                {
                    MessageBox.Show("Création annulée:" + e.Result.ToString(), "Annulation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show("Traitement terminé avec succès", "Fin de traitement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Recherche la distance maximum entre deux villes dans tous les parcours recherchés
        /// et analyse de la majorité des distances calculées
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonDistanceMax_Click(object sender, EventArgs e)
        {
            double distanceMax = 0;
            Donnees.TAB_PCC_VILLESRow lignePCCVilleMax=null;
            Donnees.TAB_CASERow ligneCaseD;
            Donnees.TAB_CASERow ligneCaseF;
            Donnees.TAB_NOMS_CARTERow ligneNomD;
            Donnees.TAB_NOMS_CARTERow ligneNomF;
            int[] tableStats = new int[distanceVilleMax/ 10 + 1];

            tableStats.Initialize();
            foreach (Donnees.TAB_PCC_VILLESRow lignePCCVille in Donnees.m_donnees.TAB_PCC_VILLES)
            {
                ligneNomD = Donnees.m_donnees.TAB_NOMS_CARTE.FindByID_NOM(lignePCCVille.ID_VILLE_DEBUT);
                ligneNomF = Donnees.m_donnees.TAB_NOMS_CARTE.FindByID_NOM(lignePCCVille.ID_VILLE_FIN);
                ligneCaseD = Donnees.m_donnees.TAB_CASE.FindByID_CASE(ligneNomD.ID_CASE);
                ligneCaseF = Donnees.m_donnees.TAB_CASE.FindByID_CASE(ligneNomF.ID_CASE);
                double distance = Constantes.Distance(ligneCaseD.I_X, ligneCaseD.I_Y, ligneCaseF.I_X, ligneCaseF.I_Y) / Donnees.m_donnees.TAB_JEU[0].I_ECHELLE;//distance en km
                if (distance > distanceMax)
                {
                    distanceMax = distance;
                    lignePCCVilleMax = lignePCCVille;
                }
                tableStats[(int)distance / 10 ]++;
            }
            //affichage du resultat
            if (lignePCCVilleMax == null)
            {
                MessageBox.Show("Aucun trajet actuellement calculé. Impossible de trouver la distance max", "PCCNoms",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
            }
            else
            {
                ligneNomD = Donnees.m_donnees.TAB_NOMS_CARTE.FindByID_NOM(lignePCCVilleMax.ID_VILLE_DEBUT);
                ligneNomF = Donnees.m_donnees.TAB_NOMS_CARTE.FindByID_NOM(lignePCCVilleMax.ID_VILLE_FIN);
                ligneCaseD = Donnees.m_donnees.TAB_CASE.FindByID_CASE(ligneNomD.ID_CASE);
                ligneCaseF = Donnees.m_donnees.TAB_CASE.FindByID_CASE(ligneNomF.ID_CASE);
                string message = string.Format("Distance Max de {0} km entre {1}:{2},{3} et {4}:{5},{6}",
                    distanceMax,
                    ligneNomD.S_NOM,
                    ligneCaseD.I_X,
                    ligneCaseD.I_Y,
                    ligneNomF.S_NOM,
                    ligneCaseF.I_X,
                    ligneCaseF.I_Y
                    );
                for (int i = 0; i < tableStats.Length; i++)
                {
                    if (tableStats[i] > 0)
                    {
                        message += string.Format("\r\n {0} noms entre {1} et {2} km", tableStats[i], i * 10, i * 10 + 10);
                    }
                }
                MessageBox.Show(message,"PCCNoms", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void buttonReparerTablePCC_Click(object sender, EventArgs e)
        {
            DialogResult retourMessage = MessageBox.Show("Voulez vous reconstituer la table TAB_PCC_VILLESRow à partir des fichiers trajets déjà constitués ?", "Traitement PCC", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (DialogResult.No == retourMessage)
            {
                return;
            }
            Donnees.m_donnees.TAB_JEU[0].I_ZONEVILLE_PCC = tailleVille;
            Donnees.m_donnees.TAB_JEU[0].I_DISTANCEVILLEMAX_PCC = distanceVilleMax;
            m_progressStart = 0;
            progressBar.Maximum = (Donnees.m_donnees.TAB_NOMS_CARTE.Count * Donnees.m_donnees.TAB_NOMS_CARTE.Count);//nombre maximum théorique, on sera toujours très en dessous !
            
            this.buttonValider.Enabled = false;
            this.buttonPCC.Text = "Arrêt";
            m_oldcurseur = Cursor;
            Cursor = Cursors.WaitCursor;
            m_dateDebut = DateTime.Now;
            progressBar.Value = 0;
            Invalidate();
            m_initEtape = 3;
            backgroundTraitement.RunWorkerAsync();
        }
    }
}