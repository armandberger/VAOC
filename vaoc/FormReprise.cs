﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace vaoc
{
    public partial class FormReprise : Form
    {
        public string fichierCourant { get; set; }
        public Cursor m_oldcurseur;

        public FormReprise()
        {
            InitializeComponent();
        }

        private void buttonMajDonnees_Click(object sender, EventArgs e)
        {
            //lancement du traitement
            this.buttonOK.Enabled = false;
            m_oldcurseur = Cursor;
            Cursor = Cursors.WaitCursor;
            progressBar.Value = 0;
            Invalidate();
            backgroundTraitement.RunWorkerAsync();
            this.buttonOK.Enabled = true;
        }

        private void backgroundTraitement_DoWork(object sender, DoWorkEventArgs e)
        {
            MiseAJourDonneesHistorique majHistorique = new MiseAJourDonneesHistorique();
            BackgroundWorker travailleur = sender as BackgroundWorker;
            string erreurTraitement = string.Empty;
            int tourDebut;
            try
            {
                if (string.Empty==this.textBoxTour.Text)
                {
                    tourDebut = 0;
                }
                else
                {
                    try
                    {
                        tourDebut = Convert.ToInt16(this.textBoxTour.Text);
                    }
                    catch
                    {
                        tourDebut = 0;
                    }

                }
                erreurTraitement = majHistorique.Initialisation(this.fichierCourant, 
                                                                this.checkBoxSauvegarde.Checked,
                                                                tourDebut, 
                                                                travailleur);
                if (string.Empty != erreurTraitement)
                {
                    e.Cancel = true;
                    MessageBox.Show(erreurTraitement, "Reprise des données : Initialisation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                while (string.Empty == erreurTraitement)
                {
                    if (travailleur.CancellationPending)
                    {
                        e.Cancel = true;
                        erreurTraitement = "Reprise des données terminée";
                    }
                    else
                    {
                        erreurTraitement = majHistorique.Traitement();
                    }
                }
                Cursor = m_oldcurseur;
                MessageBox.Show(erreurTraitement, "Reprise des données", MessageBoxButtons.OK, MessageBoxIcon.Information);
                majHistorique.Terminer();
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
    }
}