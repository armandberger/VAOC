using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WaocLib;
using System.Diagnostics;

namespace vaoc
{
    public partial class FormVideo : Form
    {
        private Cursor m_oldcurseur;
        private List<LieuRemarquable> m_listeLieux = new List<LieuRemarquable>();
        private List<EffectifEtVictoire> m_effectifsEtVictoires = new List<EffectifEtVictoire>();
        private List<UniteRemarquable> m_unitesRemarquables = new List<UniteRemarquable>();
        private List<UniteRole> m_unitesRoles = new List<UniteRole>();
        private string[] m_texteImages;
        delegate void FinTraitementCallBack(string strErreur);
        private string m_repertoireSource;

        public FormVideo()
        {
            InitializeComponent();
            labelPolice.Text = labelPolice.Font.ToString();
        }

        public string fichierCourant { get; set; }

        public string repertoireSource
        {
            set
            {
                m_repertoireSource = value;
                //initialisation des repertoires et données
                InitialisationRepertoire();
            }
        }

        private void buttonChoixPolice_Click(object sender, EventArgs e)
        {
            fontDialog.Font = labelPolice.Font;
            if (DialogResult.OK == fontDialog.ShowDialog())
            {
                labelPolice.Font = fontDialog.Font;
                labelPolice.Text = labelPolice.Font.ToString();

                //dataGridViewPolice.Rows[e.RowIndex].Cells["Couleur"].Style.BackColor = fontDialog.Color;
                //dataGridViewPolice.Rows[e.RowIndex].Cells["Police"].Value = police.Name;
                //dataGridViewPolice.Rows[e.RowIndex].Cells["Taille"].Value = police.SizeInPoints;
                //dataGridViewPolice.Rows[e.RowIndex].Cells["Italique"].Value = police.Italic;
                //dataGridViewPolice.Rows[e.RowIndex].Cells["Barre"].Value = police.Strikeout;
                //dataGridViewPolice.Rows[e.RowIndex].Cells["Souligne"].Value = police.Underline;
                //dataGridViewPolice.Rows[e.RowIndex].Cells["Gras"].Value = police.Bold;
            }

        }

        private void buttonRepertoireSource_Click(object sender, EventArgs e)
        {
            if (this.folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.textBoxRepertoireImages.Text = this.folderBrowserDialog.SelectedPath;
            }
        }

        private void buttonRepertoireSortie_Click(object sender, EventArgs e)
        {
            if (this.folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.textBoxRepertoireVideo.Text = this.folderBrowserDialog.SelectedPath+"\\video";
            }
        }

        private void buttonCreerFilm_Click(object sender, EventArgs e)
        {
            m_texteImages = new string[Donnees.m_donnees.TAB_PARTIE[0].I_TOUR+1];
            this.buttonOuvrirFilm.Enabled = false;
            m_oldcurseur = Cursor;
            Cursor = Cursors.WaitCursor;

            for (int i=0; i<=Donnees.m_donnees.TAB_PARTIE[0].I_TOUR; i++)
            {
                m_texteImages[i] = ClassMessager.DateHeure(i, 0, false);
            }

            //on ajoute les batailles s'il y en a
            m_listeLieux.Clear();
            foreach (Donnees.TAB_BATAILLERow ligneBataille in Donnees.m_donnees.TAB_BATAILLE)
            {
                LieuRemarquable lieu =new LieuRemarquable();
                lieu.iTourDebut = ligneBataille.I_TOUR_DEBUT;
                lieu.iTourFin = (ligneBataille.IsI_TOUR_FINNull() ? Donnees.m_donnees.TAB_PARTIE[0].I_TOUR : ligneBataille.I_TOUR_FIN);
                lieu.i_X_CASE_BAS_DROITE = ligneBataille.I_X_CASE_BAS_DROITE;
                lieu.i_X_CASE_HAUT_GAUCHE = ligneBataille.I_X_CASE_HAUT_GAUCHE;
                lieu.i_Y_CASE_BAS_DROITE =ligneBataille.I_Y_CASE_BAS_DROITE;
                lieu.i_Y_CASE_HAUT_GAUCHE = ligneBataille.I_Y_CASE_HAUT_GAUCHE;
                m_listeLieux.Add(lieu);
            }

            //on ajoute les effectifs et les points de victoire
            for (int i=0; i<=Donnees.m_donnees.TAB_PARTIE[0].I_TOUR; i++)
            {
                for (int j=0; j<Donnees.m_donnees.TAB_NATION.Count; j++)
                {
                    System.Nullable<int> effectifs =
                        (from video in Donnees.m_donnees.TAB_VIDEO
                         where (video.I_TOUR == i) 
                            && (video.ID_NATION == Donnees.m_donnees.TAB_NATION[j].ID_NATION)
                            && (false == video.B_DETRUIT)
                            && (false == video.B_PRISONNIERS)
                            && (false == video.B_BLESSES)
                         select (video.I_INFANTERIE+video.I_CAVALERIE)*(100-video.I_FATIGUE)/100)
                        .Sum();

                    System.Nullable<int> victoires =
                        (from video in Donnees.m_donnees.TAB_VIDEO
                         where (video.I_TOUR == i)
                            && (((video.ID_NATION != Donnees.m_donnees.TAB_NATION[j].ID_NATION)
                                    && (true == video.B_DETRUIT || true == video.B_FUITE_AU_COMBAT))
                                || ((video.ID_NATION == Donnees.m_donnees.TAB_NATION[j].ID_NATION)
                                    && (false == video.B_DETRUIT && false == video.B_FUITE_AU_COMBAT))
                                    )
                         select video.I_VICTOIRE)
                        .Sum();

                    //var test1 =
                    //    (from video in Donnees.m_donnees.TAB_VIDEO
                    //     where (video.I_TOUR == i)
                    //        && (((video.ID_NATION != Donnees.m_donnees.TAB_NATION[j].ID_NATION)
                    //                && (true == video.B_DETRUIT || true == video.B_FUITE_AU_COMBAT))
                    //                && video.I_VICTOIRE>0)
                    //     select video);

                    //var test2 =
                    //    (from video in Donnees.m_donnees.TAB_VIDEO
                    //     where (video.I_TOUR == i)
                    //        && (((video.ID_NATION == Donnees.m_donnees.TAB_NATION[j].ID_NATION)
                    //                && (false == video.B_DETRUIT && false == video.B_FUITE_AU_COMBAT))
                    //                && video.I_VICTOIRE > 0)
                    //     select video);

                    EffectifEtVictoire effV = new EffectifEtVictoire();
                    effV.iTour = i;
                    effV.iNation = Donnees.m_donnees.TAB_NATION[j].ID_NATION;
                    effV.iEffectif = effectifs ?? 0;
                    effV.iVictoire = victoires ?? 0;
                    m_effectifsEtVictoires.Add(effV);
                }
                /*
                string requete = string.Format("ID_CASE_FIN={0} AND I_BLOCX={1} AND I_BLOCY={2}",
                    trackSource.EndNode.ID_CASE, bloc.xBloc, bloc.yBloc);

                Donnees.TAB_VIDEORow[] lignesCout = (Donnees.TAB_PCC_COUTSRow[])Donnees.m_donnees.TAB_PCC_COUTS.Select(requete);
                if (null != lignesCout && lignesCout.Length > 0)
                {
                 * */
            }

            //On ajoute les unites à représenter à l'écran
            LogFile.CreationLogFile("DonneesVideo.csv");
            for (int j=0; j<Donnees.m_donnees.TAB_VIDEO.Count; j++)
            {
                Donnees.TAB_VIDEORow ligneVideo = Donnees.m_donnees.TAB_VIDEO[j];
                if (ligneVideo.ID_PION < 0 || ligneVideo.ID_CASE<0) 
                { 
                    continue; //case comptant seulement pour les points de victoire
                }
                //ligneVideo.B_QG a été ajouté à la fin et n'est pas fiable
                Donnees.TAB_PIONRow lignePion = Donnees.m_donnees.TAB_PION.FindByID_PION(ligneVideo.ID_PION);
                if (lignePion.estQG && !ligneVideo.B_QG) { 
                    Debug.Write("bug indicateur"); }
                if (ligneVideo.B_QG)// || lignePion.estQG)
                {
                    UniteRole role = new UniteRole();
                    role.iTour = ligneVideo.I_TOUR;
                    role.nom = ligneVideo.S_NOM;
                    role.iNation = ligneVideo.ID_NATION;
                    role.ID_ROLE = ligneVideo.ID_PION;
                    Donnees.m_donnees.TAB_CASE.ID_CASE_Vers_XY(ligneVideo.ID_CASE, out role.i_X_CASE, out role.i_Y_CASE);
                    System.Nullable<int> effectifs = (from video in Donnees.m_donnees.TAB_VIDEO
                         where (video.I_TOUR == role.iTour)
                            && (video.ID_PION_PROPRIETAIRE == ligneVideo.ID_PION)
                            && (false == video.B_DETRUIT)
                            && (false == video.B_PRISONNIERS)
                            && (false == video.B_BLESSES)
                         select (video.I_INFANTERIE + video.I_CAVALERIE) * (100 - video.I_FATIGUE) / 100)
                        .Sum();
                    role.iEffectif = effectifs ?? 0;
                    LogFile.Notifier(string.Format("{0};{1};{2}", role.nom,role.iTour,role.iEffectif)); ;
                    m_unitesRoles.Add(role);
                }

                if (ligneVideo.B_QG || ligneVideo.B_PRISONNIERS || ligneVideo.B_BLESSES || ligneVideo.B_DETRUIT)
                {
                    continue; //on n'affiche pas les QG, les prisonniers, les blesses, les unités détruites
                }
                
                UniteRemarquable unite = new UniteRemarquable();
                unite.iNation = ligneVideo.ID_NATION;
                unite.iTour = ligneVideo.I_TOUR;
                //unite.tipe = (ligneVideo.IsI_TYPENull() || Constantes.NULLENTIER==ligneVideo.I_TYPE) ? lignePion.tipeVideo(ligneVideo) : (TIPEUNITEVIDEO)ligneVideo.I_TYPE;
                unite.tipe = (TIPEUNITEVIDEO)ligneVideo.I_TYPE;
                //Donnees.TAB_CASERow ligneCase = Donnees.m_donnees.TAB_CASE.FindParID_CASE(ligneVideo.ID_CASE);
                Donnees.m_donnees.TAB_CASE.ID_CASE_Vers_XY(ligneVideo.ID_CASE, out unite.i_X_CASE, out unite.i_Y_CASE);
                unite.b_blesse = ligneVideo.B_BLESSES;
                unite.b_prisonnier = ligneVideo.B_PRISONNIERS;
                //unite.i_X_CASE = ligneCase.I_X;
                //unite.i_Y_CASE = ligneCase.I_Y;
                m_unitesRemarquables.Add(unite);
            }            

            /* -> deporté dans un traitement background ci-dessous
            FabricantDeFilm film = new FabricantDeFilm();
            string retour = film.CreerFilm(this.textBoxRepertoireImages.Text, this.textBoxRepertoireVideo.Text, labelPolice.Font,
                                        this.textBoxMasque.Text, m_texteImages,
                                        Convert.ToInt32(textBoxLargeurBase.Text), Convert.ToInt32(textBoxHauteurBase.Text), 
                                        true, m_listeLieux);
            if (string.Empty != retour)
            {
                MessageBox.Show("Erreur lors de la création du film :" + retour);
            }
            MessageBox.Show("film crée");
            this.buttonOuvrirFilm.Enabled = true;
            */

            //lancement du traitement
            this.buttonOuvrirFilm.Enabled = false;
            //m_dateDebut = DateTime.Now;
            progressBar.Value = 0;
            Invalidate();
            backgroundTraitement.RunWorkerAsync();
        }

        private void buttonOuvrirFilm_Click(object sender, EventArgs e)
        {
            if (this.checkBoxTravelling.Checked)
            {
                Process.Start(this.textBoxRepertoireVideo.Text + "\\video.mp4");
            }
            else
            {
                Process.Start(this.textBoxRepertoireVideo.Text + "\\video.avi");
            }
        }

        private void backgroundTraitement_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                if (null != e)
                {
                    progressBar.Value = e.ProgressPercentage;
                }
                //AfficherTemps();
            }
            catch (Exception ex)
            {
                BackgroundWorker travailleur = sender as BackgroundWorker;
                travailleur.CancelAsync();
                MessageBox.Show(ex.Message);
            }
        }

        private void backgroundTraitement_DoWork(object sender, DoWorkEventArgs e)
        {
            FabricantDeFilm cineaste = new FabricantDeFilm();
            BackgroundWorker travailleur = sender as BackgroundWorker;
            string erreurTraitement = string.Empty;
            try
            {
                erreurTraitement = cineaste.Initialisation(this.textBoxRepertoireImages.Text, this.textBoxRepertoireVideo.Text, 
                                        labelPolice.Font,
                                        this.textBoxMasque.Text, m_texteImages,
                                        Convert.ToInt32(textBoxLargeurBase.Text), Convert.ToInt32(textBoxHauteurBase.Text),
                                        Convert.ToInt32(textBoxTailleUnite.Text), Convert.ToInt32(textBoxEpaisseurUnite.Text),
                                        true, checkBoxCarteGlobale.Checked, checkBoxFilm.Checked, checkBoxTravelling.Checked,
                                        checkBoxVideoParRole.Checked,
                                        m_listeLieux, m_unitesRemarquables, m_effectifsEtVictoires, m_unitesRoles,
                                        Donnees.m_donnees.TAB_PARTIE[0].I_NB_TOTAL_VICTOIRE, Donnees.m_donnees.TAB_PARTIE[0].I_TOUR,
                                        travailleur);
                if (string.Empty != erreurTraitement)
                {
                    e.Cancel = true;
                    MessageBox.Show(erreurTraitement, "Fabricant de Film : Initialisation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                while (string.Empty == erreurTraitement)
                {
                    if (travailleur.CancellationPending)
                    {
                        e.Cancel = true;
                        erreurTraitement = "film crée";
                    }
                    else
                    {
                        erreurTraitement = cineaste.Traitement();
                    }
                }
                cineaste.Terminer();
                FinTraitementCallBack cb = new FinTraitementCallBack(FinTraitement);
                this.Invoke(cb, new object[] { erreurTraitement });
            }
            catch (Exception ex)
            {
                travailleur.CancelAsync();
                MessageBox.Show(ex.Message);
            }
        }

        private void FinTraitement(string strErreur)
        {
            this.buttonOuvrirFilm.Enabled = true;
            Cursor = m_oldcurseur;
            MessageBox.Show(strErreur, "Fabricant de Film", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void InitialisationRepertoire()
        {
            if (checkBoxCarteUnites.Checked)
            {
                textBoxMasque.Text = "*.png";
                string repertoireHistorique = string.Format("{0}{1}_{2}_historique",
                    m_repertoireSource,
                    Donnees.m_donnees.TAB_JEU[0].S_NOM.Replace(" ", ""),
                    Donnees.m_donnees.TAB_PARTIE[0].S_NOM.Replace(" ", ""));
                this.textBoxRepertoireImages.Text = repertoireHistorique;
            }
            else
            {
                textBoxMasque.Text = Donnees.m_donnees.TAB_JEU[0].S_NOM_CARTE_HISTORIQUE;
                this.textBoxRepertoireImages.Text = m_repertoireSource[m_repertoireSource.Length -1] == '\\' ? m_repertoireSource.Substring(0, m_repertoireSource.Length-1) : m_repertoireSource;
            }
            this.textBoxRepertoireVideo.Text = this.textBoxRepertoireImages.Text + "\\video";
        }

        private void checkBoxCarteUnites_CheckedChanged(object sender, EventArgs e)
        {
            InitialisationRepertoire();
        }

        private void FormVideo_Load(object sender, EventArgs e)
        {
            //Donnees.m_donnees = new Donnees();
        }

        private void buttonChargerPartie_Click(object sender, EventArgs e)
        {
            //il faut déjà chargé un fichier
            if (DialogResult.OK == this.openFileDialog.ShowDialog(this))
            {
                if (Donnees.m_donnees.ChargerPartie(openFileDialog.FileName))
                {
                    LogFile.CreationLogFile("vaocVideo");
                    Donnees.m_donnees.TAB_CASE.InitialisationListeCase(Donnees.m_donnees.TAB_JEU[0].I_LARGEUR_CARTE, Donnees.m_donnees.TAB_JEU[0].I_HAUTEUR_CARTE);
                    this.fichierCourant = openFileDialog.FileName;
                    Constantes.repertoireDonnees = this.fichierCourant;
                    repertoireSource = Constantes.repertoireDonnees;
                    Bitmap fichierImage = (Bitmap)Image.FromFile(m_repertoireSource + Donnees.m_donnees.TAB_JEU[0].S_NOM_CARTE_HISTORIQUE);
                    textBoxLargeurBase.Text = fichierImage.Width.ToString();
                    textBoxHauteurBase.Text = fichierImage.Height.ToString();
                    buttonDonnees.Enabled = true;
                }
            }
        }

        private void buttonDonnees_Click(object sender, EventArgs e)
        {
            FormVideoTable fVideoTable = new FormVideoTable();

            fVideoTable.tableVideo = Donnees.m_donnees.TAB_VIDEO;
            if (fVideoTable.ShowDialog() == DialogResult.OK)
            {
                Donnees.m_donnees.TAB_VIDEO.Clear();
                Donnees.m_donnees.TAB_VIDEO.Merge(fVideoTable.tableVideo.Copy(), false);
            }
        }

        private void checkBoxFilm_CheckedChanged(object sender, EventArgs e)
        {
            buttonCreerFilm.Text = (checkBoxFilm.Checked) ? "Créer Film" : "Créer Images";
        }

        private void buttonDonneesTravelling_Click(object sender, EventArgs e)
        {
            FormVideoTravellingTable fVideoTable = new FormVideoTravellingTable();

            fVideoTable.tableTravelling = Donnees.m_donnees.TAB_TRAVELLING;
            if (fVideoTable.ShowDialog() == DialogResult.OK)
            {
                Donnees.m_donnees.TAB_TRAVELLING.Clear();
                Donnees.m_donnees.TAB_TRAVELLING.Merge(fVideoTable.tableTravelling.Copy(), false);
            }
        }
    }
}
