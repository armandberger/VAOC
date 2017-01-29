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
        private string[] m_texteImages;

        public FormVideo()
        {
            InitializeComponent();
            labelPolice.Text = labelPolice.Font.ToString();
            //initialisation des repertoires et données
            this.textBoxMasque.Text = "carte_general_*.png";
        }

        public string repertoireSource
        {
            set
            {
                string repertoireHistorique = string.Format("{0}{1}_{2}_historique",
                    value,
                    Donnees.m_donnees.TAB_JEU[0].S_NOM.Replace(" ", ""),
                    Donnees.m_donnees.TAB_PARTIE[0].S_NOM.Replace(" ", ""));
                this.textBoxRepertoireImages.Text = repertoireHistorique;
                this.textBoxRepertoireVideo.Text = repertoireHistorique;
            }
        }

        private void buttonCreerFilm_Click(object sender, EventArgs e)
        {
            this.buttonOuvrirFilm.Enabled = false;
            FabricantDeFilm film = new FabricantDeFilm();
            string retour = film.CreerFilm(this.textBoxRepertoireImages.Text, this.textBoxRepertoireVideo.Text, labelPolice.Font,
                                        this.textBoxMasque.Text, null, 
                                        Convert.ToInt32(textBoxLargeurBase.Text), Convert.ToInt32(textBoxHauteurBase.Text), false, null);
            if (string.Empty != retour)
            {
                MessageBox.Show("Erreur lors de la création du film :" + retour);
            }
            MessageBox.Show("film crée");
            this.buttonOuvrirFilm.Enabled = true;
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
                this.textBoxRepertoireVideo.Text = this.folderBrowserDialog.SelectedPath;
            }
        }

        private void buttonCreerFilmHistorique_Click(object sender, EventArgs e)
        {
            m_texteImages = new string[Donnees.m_donnees.TAB_PARTIE[0].I_TOUR+1];
            this.buttonOuvrirFilm.Enabled = false;

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
            m_oldcurseur = Cursor;
            Cursor = Cursors.WaitCursor;
            //m_dateDebut = DateTime.Now;
            progressBar.Value = 0;
            Invalidate();
            backgroundTraitement.RunWorkerAsync();
        }

        private void buttonOuvrirFilm_Click(object sender, EventArgs e)
        {
            Process.Start(this.textBoxRepertoireVideo.Text+"\\video.avi");
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
                erreurTraitement = cineaste.Initialisation(this.textBoxRepertoireImages.Text, this.textBoxRepertoireVideo.Text, labelPolice.Font,
                                        this.textBoxMasque.Text, m_texteImages,
                                        Convert.ToInt32(textBoxLargeurBase.Text), Convert.ToInt32(textBoxHauteurBase.Text),
                                        true, m_listeLieux,
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
                    cineaste.Terminer();
                }
                MessageBox.Show(erreurTraitement, "Fabricant de Film", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                travailleur.CancelAsync();
                MessageBox.Show(ex.Message);
            }

        }
    }
}
