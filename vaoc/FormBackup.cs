using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace vaoc
{
    public partial class FormBackup : Form
    {
        const string registryKeyName = "VAOC";
        const string registryValueName = "repertoireBackup";
        private Cursor m_oldcurseur;
        private int m_traitement;
        private int m_soustraitement;
        List<FileInfo> m_fichiersDestination;
        List<FileInfo> m_fichiersSource;
        DirectoryInfo[] m_repertoiresSource;
        int m_lgRepertoireSource;
        int m_lgRepertoireDestination;
        long m_taille_fichiers;
        long m_taille_fichiers_copies;

        public string fichierCourant
        {
            set
            {
                int positionPoint = value.LastIndexOf("\\");
                this.textBoxRepertoireSource.Text = value.Substring(0, positionPoint);
            }
        }

        public FormBackup()
        {
            InitializeComponent();
        }

        private void FormBackup_Load(object sender, EventArgs e)
        {
            RegistryKey regKey = Registry.CurrentUser.OpenSubKey(registryKeyName);
            if (regKey != null)
            {
                this.textBoxRepertoireCible.Text = (null == regKey.GetValue(registryValueName)) ? string.Empty : regKey.GetValue(registryValueName).ToString();
                regKey.Close();
            }
        }
        private void buttonCible_Click(object sender, EventArgs e)
        {
            if (this.folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.textBoxRepertoireCible.Text = this.folderBrowserDialog.SelectedPath;
            }
        }

        private void buttonValider_Click(object sender, EventArgs e)
        {
            SauvegardeRegistryCible();
            this.Close();
        }

        private void SauvegardeRegistryCible()
        {
            //sauvegarde du répertoire en registry
            RegistryKey regKey = Registry.CurrentUser.CreateSubKey(registryKeyName);
            if (regKey != null)
            {
                regKey.SetValue(registryValueName, this.textBoxRepertoireCible.Text);
                regKey.Close();
            }
        }

        private void buttonSynchroniser_Click(object sender, EventArgs e)
        {
            if (m_soustraitement > 0)
            {
                Cursor = m_oldcurseur;
                timerTraitement.Enabled = false;
                if (DialogResult.No == MessageBox.Show("Voulez vous arrêter la copie en cours ?", "Copie de Sauvegarde", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    Cursor = Cursors.WaitCursor;
                    timerTraitement.Enabled = true;
                }
                else
                {
                    labelStatut.Text = "Traitement annulé";
                    progressBar.Value = 0;
                }
            }
            else
            {
                if (0 == Donnees.m_donnees.TAB_PARTIE.Count)
                {
                    MessageBox.Show("Vous devez charger une partie pour pouvoir en faire la copie !", "Copie de Sauvegarde", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (string.Empty == this.textBoxRepertoireCible.Text)
                {
                    MessageBox.Show("Vous devez indiquer le répertoire de copie !", "Copie de Sauvegarde", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                SauvegardeRegistryCible();
                m_traitement = 0;
                m_soustraitement = 0;
                m_lgRepertoireSource = this.textBoxRepertoireSource.Text.Length;
                m_lgRepertoireDestination = this.textBoxRepertoireCible.Text.Length;
                DirectoryInfo dirSource = new DirectoryInfo(this.textBoxRepertoireSource.Text);
                //fichiers du repertoire primaire
                m_fichiersSource = dirSource.GetFiles("*", SearchOption.TopDirectoryOnly).ToList();

                DirectoryInfo dirDestination = new DirectoryInfo(this.textBoxRepertoireCible.Text);
                m_fichiersDestination = dirDestination.GetFiles("*", SearchOption.TopDirectoryOnly).ToList();

                //liste des repertoires de la partie
                m_repertoiresSource = dirSource.GetDirectories("*", SearchOption.AllDirectories);
                labelStatut.Text = "Analyse des repertoires";
                
                m_oldcurseur = Cursor;
                Cursor = Cursors.WaitCursor;
                progressBar.Value = 0;
                progressBar.Maximum = 100;
                timerTraitement.Enabled = true;
            }

        }

        private void timerTraitement_Tick(object sender, EventArgs e)
        {
            switch (m_traitement)
            {
                case 0:
                    if (m_soustraitement == m_repertoiresSource.Length)
                    {
                        //traitement terminé
                        labelStatut.Text = "Analyse des fichiers";
                        m_soustraitement = 0;
                        progressBar.Value = 0;
                        m_traitement++;
                    }
                    else
                    {
                        DirectoryInfo repertoire = m_repertoiresSource[m_soustraitement];
                        DirectoryInfo repertoireCible = new DirectoryInfo(textBoxRepertoireCible.Text + m_repertoiresSource[m_soustraitement].FullName.Substring(m_lgRepertoireSource));
                        if (Directory.Exists(repertoireCible.FullName))
                        {
                            if (repertoire.LastWriteTime > repertoireCible.LastWriteTime)
                            {
                                //on reteste les fichiers un par un
                                m_fichiersSource.AddRange(repertoire.GetFiles("*", SearchOption.TopDirectoryOnly));
                                m_fichiersDestination.AddRange(repertoireCible.GetFiles("*", SearchOption.TopDirectoryOnly));
                            }
                        }
                        else
                        {
                            CreationRepertoire(repertoireCible.FullName);
                            m_fichiersSource.AddRange(repertoire.GetFiles("*", SearchOption.TopDirectoryOnly));
                        }
                        //progression en pourcentage
                        progressBar.Value = (int)(m_soustraitement * 100 / m_repertoiresSource.Length);
                        m_soustraitement++;
                    }

                    break;
                case 1:
                    //on conserve les fichiers sources que l'on doit copier
                    if (m_soustraitement == m_fichiersSource.Count)
                    {
                        //traitement terminé
                        //calcul de la taille de fichiers à copier
                        m_taille_fichiers = 0;
                        foreach (FileInfo fichier in m_fichiersSource) { m_taille_fichiers += fichier.Length; };
                        m_taille_fichiers_copies = 0;

                        labelStatut.Text = "Copie des fichiers";
                        m_soustraitement = 0;
                        progressBar.Value = 0;
                        m_traitement++;
                    }
                    else
                    {
                        FileInfo source = m_fichiersSource[m_soustraitement];
                        string nomSource = source.FullName.Substring(m_lgRepertoireSource);
                        int j = 0;
                        while (j < m_fichiersDestination.Count() && m_fichiersDestination[j].FullName.Substring(m_lgRepertoireDestination) != nomSource) j++;
                        if (j == m_fichiersDestination.Count())
                        {
                            //on a pas trouvé de fichier équivalent donc il faut le copier
                            m_soustraitement++;
                        }
                        else
                        {
                            // on converse le plus récent
                            if (source.LastWriteTime <= m_fichiersDestination[j].LastWriteTime)
                            {
                                //fichier source plus vieux ou équivalent donc, on ne le copie pas
                                m_fichiersSource.RemoveAt(m_soustraitement);
                            }
                            else
                            {
                                
                                m_soustraitement++;
                            }
                        }
                        //progression en pourcentage
                        progressBar.Value = (0 == m_fichiersSource.Count) ? 100 : (int)(m_soustraitement * 100 / m_fichiersSource.Count);
                    }
                    break;
                case 2:
                    if (m_soustraitement == m_fichiersSource.Count)
                    {
                        //traitement terminé
                        m_soustraitement = 0;
                        Cursor = m_oldcurseur;
                        timerTraitement.Enabled = false;
                        labelStatut.Text = (0== m_fichiersSource.Count) ? "Aucun fichier à copier" : "Terminé";
                    }
                    else
                    {
                        try
                        {
                            string fichierDestination = this.textBoxRepertoireCible.Text + m_fichiersSource[m_soustraitement].FullName.Substring(m_lgRepertoireSource);
                            labelStatut.Text = m_fichiersSource[m_soustraitement].FullName + " -> " + fichierDestination;
                            m_fichiersSource[m_soustraitement].CopyTo(fichierDestination, true);
                            m_taille_fichiers_copies += m_fichiersSource[m_soustraitement].Length;
                            progressBar.Value = (int)(m_taille_fichiers_copies * 100 / m_taille_fichiers);
                            m_soustraitement++;
                        }
                        catch (DirectoryNotFoundException)
                        {
                            //le repertoire n'existe pas il faut le créer
                            CreationRepertoire(this.textBoxRepertoireCible.Text + m_fichiersSource[m_soustraitement].FullName.Substring(m_lgRepertoireSource));
                        }
                    }
                    break;
                default:
                    throw new Exception("valeur de m_traitement non prévue");
            }
        }

        private void CreationRepertoire(string source)
        {
            
            int pos = 3;//D:\\
            while(pos>0)
            {
                pos = source.IndexOf('\\', pos);
                string repertoireCible = (pos < 0) ? source : source.Substring(0, pos);
                if (!Directory.Exists(repertoireCible))
                {
                    Directory.CreateDirectory(repertoireCible);
                }
                pos++;
            }
        }
    } 
}
