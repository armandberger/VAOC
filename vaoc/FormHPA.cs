using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using Microsoft.Win32;
using WaocLib;

namespace vaoc
{
    public partial class FormHPA : Form
    {
        #region données
        private int m_nbBlocsHorizontaux;//nombre de blocs sur l'axe des abscisses
        private int m_nbBlocsVerticaux;//nombre de blocs sur l'axe des ordonnées
        private Cursor                                              m_oldcurseur;
        private string m_fichierCourant;
        private int m_progressStart;//nombre de valeurs déjà traitées au demmarage du traitement

        //private Donnees.TAB_PCC_TRAJETDataTable m_tablePCCTrajet;
        //private Donnees.TAB_PCC_COUTSDataTable m_tablePCCCouts;
        //private Donnees.TAB_PCC_CASE_BLOCSDataTable m_tableCaseBlocs;
        private DateTime m_dateDebut;
        #endregion

        #region propriétés

        //public Donnees.TAB_PCC_CASE_BLOCSDataTable tableCaseBlocs
        //{
        //    get
        //    {
        //        return (Donnees.TAB_PCC_CASE_BLOCSDataTable)m_tableCaseBlocs;
        //    }
        //    set
        //    {
        //        m_tableCaseBlocs = (Donnees.TAB_PCC_CASE_BLOCSDataTable)value.Copy();
        //    }
        //}

        //public Donnees.TAB_PCC_TRAJETDataTable tablePCCTrajet
        //{
        //    get
        //    {
        //        return (Donnees.TAB_PCC_TRAJETDataTable)m_tablePCCTrajet;
        //    }
        //    set
        //    {
        //        m_tablePCCTrajet = (Donnees.TAB_PCC_TRAJETDataTable)value.Copy();
        //    }
        //}

        //public Donnees.TAB_PCC_COUTSDataTable tablePCCCouts
        //{
        //    get
        //    {
        //        return (Donnees.TAB_PCC_COUTSDataTable)m_tablePCCCouts;
        //    }
        //    set
        //    {
        //        m_tablePCCCouts = (Donnees.TAB_PCC_COUTSDataTable)value.Copy();
        //    }
        //}


        public string fichierCourant
        {
            set { m_fichierCourant=value; }
            get { return m_fichierCourant; }
        }

        public int tailleBloc
        {
		    get { return Convert.ToInt16(this.textBoxBlocPCC.Text);}
            set { this.textBoxBlocPCC.Text = value.ToString(); }
        }
        #endregion

        public FormHPA()
        {
            InitializeComponent();
            labelAvisTailleBloc.Text += Math.Min(Math.Sqrt(Donnees.m_donnees.TAB_JEU[0].I_LARGEUR_CARTE), Math.Sqrt(Donnees.m_donnees.TAB_JEU[0].I_HAUTEUR_CARTE)).ToString();
        }

        private void AfficherTemps()
        {
            TimeSpan tPasse=DateTime.Now.Subtract(m_dateDebut);
            long nbrestant;

            if (progressBar.Value > 0)
            {
                nbrestant = (long)(tPasse.TotalSeconds * (progressBar.Maximum - progressBar.Value) / progressBar.Value);
            }
            else
            {
                nbrestant = (long)(tPasse.TotalSeconds * progressBar.Maximum);
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
            bool bNouveau;

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
                m_nbBlocsHorizontaux = (int)Math.Ceiling((decimal)Donnees.m_donnees.TAB_JEU[0].I_LARGEUR_CARTE / (decimal)tailleBloc);
                m_nbBlocsVerticaux = (int)Math.Ceiling((decimal)Donnees.m_donnees.TAB_JEU[0].I_HAUTEUR_CARTE / (decimal)tailleBloc);
                bNouveau = true;
                if (Donnees.m_donnees.TAB_PCC_COUTS.Count > 0)
                {
                    DialogResult retourMessage = MessageBox.Show("Voulez vous reprendre le traitement à partir de zéro (sinon reprend à partir du point d'arrêt)", "Traitement PCC", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                    if (DialogResult.No == retourMessage)
                    {
                        bNouveau = false;
                    }
                }

                if (bNouveau)
                {
                    //ok, on calcule le nombre de blocs pour s'assurer que c'est ok pour le traitement
                    decimal nbBlocs = m_nbBlocsHorizontaux * m_nbBlocsVerticaux;
                    if (DialogResult.No ==
                        MessageBox.Show("Confirmez vous la mise en place d'un PCC hierarchique sur " + nbBlocs.ToString() + " blocs ?", "PCC Hierarchique", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                    {
                        return;
                    }
                    Donnees.m_donnees.TAB_PCC_CASE_BLOCS.Clear();
                    Donnees.m_donnees.TAB_PCC_COUTS.Clear();
                    //Donnees.m_donnees.TAB_PCC_TRAJET.Clear(); inutilisé, trop lourd en base, sous forme de fichiers
                    m_progressStart = 0;
                    progressBar.Maximum = m_nbBlocsHorizontaux * m_nbBlocsVerticaux;
                }
                else
                {
                    int maxTrajet = (int)Donnees.m_donnees.TAB_PCC_COUTS.Compute("MAX(ID_TRAJET)", "");
                    Donnees.TAB_PCC_COUTSRow[] resPCCout = (Donnees.TAB_PCC_COUTSRow[])Donnees.m_donnees.TAB_PCC_COUTS.Select("ID_TRAJET=" + maxTrajet);
                    m_progressStart = m_nbBlocsHorizontaux * resPCCout[0].I_BLOCY + resPCCout[0].I_BLOCX;
                    progressBar.Maximum = (m_nbBlocsHorizontaux * m_nbBlocsVerticaux) - m_progressStart;
                }
            }
            
            //vérifie que les conditions sont requises pour un PCC
            if (tailleBloc < 10 || tailleBloc > 60)
            {
                MessageBox.Show("Tous les auteurs recommandent une taille de bloc entre 10 et 60. La valeur indiquée est " + this.textBoxBlocPCC.Text, "PCC Hierarchique", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            //vérifie que tous les modèles de mouvements ont le même coût pour le terrain
            //pour une même météo, pour un même terrain, le coût doit être le même pour tous les modèles de mouvement
            //si ce n'est pas le cas, il faut faire une table PCC hierarchique par modèle de mouvement ce qui 
            //occuperait beaucoup plus d'espace mémoire et disque.
            foreach (Donnees.TAB_MODELE_TERRAINRow ligneModeleTerrain in Donnees.m_donnees.TAB_MODELE_TERRAIN)
            {
                foreach (Donnees.TAB_METEORow ligneMeteo in Donnees.m_donnees.TAB_METEO)
                {
                    int cout=-100;
                    foreach (Donnees.TAB_MODELE_MOUVEMENTRow ligneModeleMouvement in Donnees.m_donnees.TAB_MODELE_MOUVEMENT)
                    {
                        int coutLocal = Donnees.m_donnees.TAB_MOUVEMENT_COUT.CalculCout(ligneMeteo.ID_METEO, 
                            ligneModeleTerrain.ID_MODELE_TERRAIN, ligneModeleMouvement.ID_MODELE_MOUVEMENT);
                        if (cout <0)
                        {
                            cout = coutLocal;
                        }
                        else
                        {
                            if (cout != coutLocal)
                            {
                                MessageBox.Show(string.Format("Ecart de cout {0}<>{1} pour modele_mouvement={2}, modele_teerrain={3}, meteo={4}. PCC hierarchique impossible", 
                                    cout,
                                    coutLocal,
                                    ligneModeleMouvement.ID_MODELE_MOUVEMENT,
                                    ligneModeleTerrain.ID_MODELE_TERRAIN,
                                    ligneMeteo.ID_METEO), 
                                    "PCC Hierarchique", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                return;
                            }
                        }
                    }
                }
            }
            
            //lancement du traitement
            this.buttonValider.Enabled = false;
            this.buttonPCC.Text = "Arrêt";
            m_oldcurseur = Cursor;
            Cursor = Cursors.WaitCursor;
            m_dateDebut = DateTime.Now;
            progressBar.Value = 0;
            Invalidate();
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
            ClassHPAStarCreation traitementHPA = new ClassHPAStarCreation(fichierCourant, this.tailleBloc);
            BackgroundWorker travailleur = sender as BackgroundWorker;
            bool bTraitement = true;
            try
            {
                if (!traitementHPA.Initialisation(travailleur))
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
                        bTraitement = traitementHPA.Traitement();
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
                    HPACreationStatut statut = (HPACreationStatut)e.UserState;
                    if (statut.m_statut.Length>0)
                        labelTraitement.Text = statut.m_statut;
                    if (statut.m_xBloc >= 0 && statut.m_yBloc >= 0)
                    {
                        this.labelXbloc.Text = statut.m_xBloc + " / " + m_nbBlocsHorizontaux;
                        this.labelYbloc.Text = statut.m_yBloc + " / " + m_nbBlocsVerticaux;
                        progressBar.Value = (m_nbBlocsHorizontaux * statut.m_yBloc + statut.m_xBloc) - m_progressStart;
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
                this.buttonPCC.Text = "Fin de Generation du PCC hierarchique";
                Invalidate();
                MessageBox.Show("Nb de points dans les blocs =" + Donnees.m_donnees.TAB_PCC_CASE_BLOCS.Count.ToString());
                MessageBox.Show("Nb lignes de couts =" + Donnees.m_donnees.TAB_PCC_COUTS.Count.ToString());
                MessageBox.Show("Nb points dans les trajets =" + Donnees.m_donnees.TAB_PCC_TRAJET.Count.ToString());
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
        /// Des problèmes étant survenus sur la génération des coins, ce script sert à vérifier qu'ils sont tous là et avec les trajets qu'il faut
        /// </summary>
        private void buttonControleCoins_Click(object sender, EventArgs e)
        {
            Donnees.TAB_CASERow ligneCase;
            int tailleBloc = Donnees.m_donnees.TAB_JEU[0].I_TAILLEBLOC_PCC;
            int m_nbBlocsHorizontaux = (int)Math.Ceiling((decimal)Donnees.m_donnees.TAB_JEU[0].I_LARGEUR_CARTE / (decimal)tailleBloc);
            int m_nbBlocsVerticaux = (int)Math.Ceiling((decimal)Donnees.m_donnees.TAB_JEU[0].I_HAUTEUR_CARTE / (decimal)tailleBloc);
            string requete;
            for (int xBloc = 0; xBloc < m_nbBlocsHorizontaux; xBloc++)
            {
                Debug.WriteLine("xbloc=" + xBloc + " / " + m_nbBlocsHorizontaux);
                for (int yBloc = 0; yBloc < m_nbBlocsHorizontaux; yBloc++)
                {
                    Debug.WriteLine("bloc=" + xBloc + "," + yBloc);
                    for (int c = 0; c < 4; c++)
                    {
                        int x = Math.Min((xBloc + c / 2) * tailleBloc, Donnees.m_donnees.TAB_JEU[0].I_LARGEUR_CARTE - 1);
                        int y = Math.Min((yBloc + c % 2) * tailleBloc, Donnees.m_donnees.TAB_JEU[0].I_HAUTEUR_CARTE - 1);
                        // on regarde si le coin fait partie du bloc, si oui, on verifie qu'il est bien présent pour les 3 blocs voisins
                        ligneCase = Donnees.m_donnees.TAB_CASE.FindByXY(x, y);
                        if (null != ligneCase)
                        {
                            requete = string.Format("I_BLOCX={0} AND I_BLOCY={1} AND ID_CASE={2}", xBloc, yBloc, ligneCase.ID_CASE);
                            Donnees.TAB_PCC_CASE_BLOCSRow[] lignesCaseBloc = (Donnees.TAB_PCC_CASE_BLOCSRow[])Donnees.m_donnees.TAB_PCC_CASE_BLOCS.Select(requete);
                            if (null != lignesCaseBloc && lignesCaseBloc.Length > 0)
                            {
                                requete = string.Format("I_BLOCX={0} AND I_BLOCY={1} AND ID_CASE_DEBUT={2}", xBloc, yBloc, ligneCase.ID_CASE);
                                Donnees.TAB_PCC_COUTSRow[] lignesCout = (Donnees.TAB_PCC_COUTSRow[])Donnees.m_donnees.TAB_PCC_COUTS.Select(requete);
                                if (null == lignesCout || lignesCout.Length == 0)
                                {
                                    Debug.WriteLine(string.Format("pas de ligne cout associée pour ID={0}({1},{2}) sur bloc {3},{4}",
                                        ligneCase.ID_CASE, ligneCase.I_X, ligneCase.I_Y, xBloc, yBloc));
                                    GenerationTrajets(ligneCase, xBloc, yBloc);
                                }

                                switch (c)
                                {
                                    case 0://coin supérieur gauche
                                        if (!TestCoin(ligneCase, xBloc - 1, yBloc - 1))
                                            AjouterCoin(ligneCase, xBloc - 1, yBloc - 1);
                                        if (!TestCoin(ligneCase, xBloc - 1, yBloc))
                                            AjouterCoin(ligneCase, xBloc - 1, yBloc);
                                        if (!TestCoin(ligneCase, xBloc, yBloc - 1))
                                            AjouterCoin(ligneCase, xBloc, yBloc - 1);
                                        break;
                                    case 1://coin inférieur gauche
                                        if (!TestCoin(ligneCase, xBloc - 1, yBloc))
                                            AjouterCoin(ligneCase, xBloc - 1, yBloc);
                                        if (!TestCoin(ligneCase, xBloc - 1, yBloc + 1))
                                            AjouterCoin(ligneCase, xBloc - 1, yBloc + 1);
                                        if (!TestCoin(ligneCase, xBloc, yBloc + 1))
                                            AjouterCoin(ligneCase, xBloc, yBloc + 1);
                                        break;
                                    case 2://coin supérieur droit
                                        if (!TestCoin(ligneCase, xBloc + 1, yBloc - 1))
                                            AjouterCoin(ligneCase, xBloc + 1, yBloc - 1);
                                        if (!TestCoin(ligneCase, xBloc + 1, yBloc))
                                            AjouterCoin(ligneCase, xBloc + 1, yBloc);
                                        if (!TestCoin(ligneCase, xBloc, yBloc - 1))
                                            AjouterCoin(ligneCase, xBloc, yBloc - 1);
                                        break;
                                    case 3://coin inférieur droit
                                        if (!TestCoin(ligneCase, xBloc + 1, yBloc))
                                            AjouterCoin(ligneCase, xBloc + 1, yBloc);
                                        if (!TestCoin(ligneCase, xBloc + 1, yBloc + 1))
                                            AjouterCoin(ligneCase, xBloc + 1, yBloc + 1);
                                        if (!TestCoin(ligneCase, xBloc, yBloc + 1))
                                            AjouterCoin(ligneCase, xBloc, yBloc + 1);
                                        break;
                                }
                            }
                        }
                    }

                }
            }
        }

        private void PCCMinMax(int xBloc, int yBloc, out int xmin, out int xmax, out int ymin, out int ymax)
        {
            xmin = xBloc * tailleBloc;
            xmax = Math.Min(xmin + tailleBloc, Donnees.m_donnees.TAB_JEU[0].I_LARGEUR_CARTE - 1);
            ymin = yBloc * tailleBloc;
            ymax = Math.Min(ymin + tailleBloc, Donnees.m_donnees.TAB_JEU[0].I_HAUTEUR_CARTE - 1);
        }

        private bool GenerationTrajets(Donnees.TAB_CASERow ligneCaseDepart, int xBloc, int yBloc)
        {
            int i, k;
            Donnees.TAB_PCC_CASE_BLOCSRow[] listeCases;
            Donnees.TAB_CASERow ligneCaseArrivee;
            int xmin, xmax, ymin, ymax;
            List<int> listeCasesTrajet = new List<int>();
            AStar m_etoile = new AStar();
            int m_idTrajet;

            Debug.WriteLine(string.Format("GenerationTrajets xBloc={0}, yBloc={1}, case ={2}({3},{4})", 
                xBloc, yBloc, ligneCaseDepart.ID_CASE, ligneCaseDepart.I_X, ligneCaseDepart.I_Y));
            m_idTrajet = (int)Donnees.m_donnees.TAB_PCC_COUTS.Compute("MAX(ID_TRAJET)", "");
            try
            {
                PCCMinMax(xBloc, yBloc, out xmin, out xmax, out ymin, out ymax);

                //recherche de tous les points dans le bloc
                listeCases = Donnees.m_donnees.TAB_PCC_CASE_BLOCS.ListeCasesBloc(xBloc, yBloc);

                for (i = 0; i < listeCases.Length; i++)
                {
                    ligneCaseArrivee = Donnees.m_donnees.TAB_CASE.FindByID_CASE(listeCases[i].ID_CASE);
                    if (ligneCaseDepart.ID_CASE == ligneCaseArrivee.ID_CASE)
                    {
                        continue;
                    }

                    //recherche du plus court chemin
                    AstarTerrain[] tableCoutsMouvementsTerrain;
                    //Donnees.TAB_PIONRow lignePion = Donnees.m_donnees.TAB_PION[0];
                    ClassTraitementHeure traitementtest = new ClassTraitementHeure();
                    Cartographie.CalculModeleMouvementsPion(out tableCoutsMouvementsTerrain);
                    if (!m_etoile.SearchPath(ligneCaseDepart, ligneCaseArrivee, tableCoutsMouvementsTerrain, xmin, xmax, ymin, ymax))
                    {
                        Debug.WriteLine(string.Format("CalculCheminPCCBloc:AStar : Il n'y a aucun chemin possible entre les cases {0}({1},{2}) et {3}({4},{5}), bloc ({6},{7}) posi {8}",
                            ligneCaseDepart.ID_CASE, ligneCaseDepart.I_X, ligneCaseDepart.I_Y,
                            ligneCaseArrivee.ID_CASE, ligneCaseArrivee.I_X, ligneCaseArrivee.I_Y,
                            xBloc, yBloc, i));
                        //dans ce cas on ajouter un chemin de coût maximum
                        Donnees.m_donnees.TAB_PCC_COUTS.AddTAB_PCC_COUTSRow(xBloc, yBloc, ligneCaseDepart.ID_CASE, ligneCaseArrivee.ID_CASE, AStar.CST_COUTMAX, -1, AStar.CST_COUTMAX, false, -1);
                        Donnees.m_donnees.TAB_PCC_COUTS.AddTAB_PCC_COUTSRow(xBloc, yBloc, ligneCaseArrivee.ID_CASE, ligneCaseDepart.ID_CASE, AStar.CST_COUTMAX, -1, AStar.CST_COUTMAX, false, -1);
                    }
                    else
                    {
                        //ajout des trajets et des couts
                        m_idTrajet++;
                        k = 0;
                        listeCasesTrajet.Clear();
                        List<Donnees.TAB_CASERow> trajet = m_etoile.PathByNodes;
                        while (k < trajet.Count)
                        {
                            //Donnees.m_donnees.TAB_PCC_TRAJET.AddTAB_PCC_TRAJETRow(m_idTrajet, trajet[k].ID_CASE, k);
                            listeCasesTrajet.Add(trajet[k].ID_CASE);
                            k++;
                        }
                        Dal.SauvegarderTrajet(m_idTrajet, listeCasesTrajet);
                        Donnees.m_donnees.TAB_PCC_COUTS.AddTAB_PCC_COUTSRow(xBloc, yBloc, ligneCaseDepart.ID_CASE, ligneCaseArrivee.ID_CASE, m_etoile.CoutGlobal, m_idTrajet, m_etoile.CoutGlobal, false, -1);
                        Donnees.m_donnees.TAB_PCC_COUTS.AddTAB_PCC_COUTSRow(xBloc, yBloc, ligneCaseArrivee.ID_CASE, ligneCaseDepart.ID_CASE, m_etoile.CoutGlobal, m_idTrajet, m_etoile.CoutGlobal, false, -1);

                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("GenerationTrajets : exception =" + e.Message);
                //return false;
            }
            return true;
        }

        /// <summary>
        /// il manque au moins un des trajet du coin, on regenere les trajets manquants
        /// </summary>
        private void AjouterCoin(Donnees.TAB_CASERow ligneCase, int xBloc, int yBloc)
        {
            Donnees.m_donnees.TAB_PCC_CASE_BLOCS.AddTAB_PCC_CASE_BLOCSRow(ligneCase.ID_CASE, xBloc, yBloc);
        }

        /// <summary>
        /// Vérifie si un coin est présent ou pas
        /// </summary>
        private bool TestCoin(Donnees.TAB_CASERow ligneCase, int xBloc, int yBloc)
        {
            if (xBloc < 0 || yBloc < 0) return true;
            string requete = string.Format("I_BLOCX={0} AND I_BLOCY={1} AND ID_CASE={2}", xBloc, yBloc, ligneCase.ID_CASE);
            Donnees.TAB_PCC_CASE_BLOCSRow[] lignesCaseBlocTest = (Donnees.TAB_PCC_CASE_BLOCSRow[])Donnees.m_donnees.TAB_PCC_CASE_BLOCS.Select(requete);
            if ((null == lignesCaseBlocTest || lignesCaseBlocTest.Length == 0)
                && ligneCase.I_X != 0 && ligneCase.I_Y != 0
                && ligneCase.I_X != Donnees.m_donnees.TAB_JEU[0].I_LARGEUR_CARTE - 1
                && ligneCase.I_Y != Donnees.m_donnees.TAB_JEU[0].I_HAUTEUR_CARTE - 1)
            {
                Debug.WriteLine(string.Format("coin {0}({1},{2})absent sur le bloc {3},{4}", ligneCase.ID_CASE, ligneCase.I_X, ligneCase.I_Y,
                        xBloc, yBloc));
                return false;
            }
            return true;
        }

    }
}