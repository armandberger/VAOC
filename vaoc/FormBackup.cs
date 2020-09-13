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
        List<FileInfo> m_fichiersSource;
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
            if (m_traitement > 0)
            {
                Cursor = m_oldcurseur;
                timerTraitement.Enabled = false;
                if (DialogResult.No == MessageBox.Show("Voulez vous arrêter la copie en cours ?", "Copie de Sauvegarde", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    Cursor = Cursors.WaitCursor;
                    timerTraitement.Enabled = true;
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
                int lgRepertoireSource = this.textBoxRepertoireSource.Text.Length;
                int lgRepertoireDestination = this.textBoxRepertoireCible.Text.Length;
                DirectoryInfo dirSource = new DirectoryInfo(this.textBoxRepertoireSource.Text);
                //fichiers du repertoire primaire
                m_fichiersSource = dirSource.GetFiles("*", SearchOption.TopDirectoryOnly).ToList();

                DirectoryInfo dirDestination = new DirectoryInfo(this.textBoxRepertoireCible.Text);
                List<FileInfo> fichiersDestination = dirDestination.GetFiles("*", SearchOption.TopDirectoryOnly).ToList();

                //liste des reepertoires de la partie
                DirectoryInfo[] repertoiresSource = dirSource.GetDirectories("*", SearchOption.AllDirectories);
                foreach (DirectoryInfo repertoire in repertoiresSource)
                {
                    DirectoryInfo repertoireCible = new DirectoryInfo(textBoxRepertoireCible.Text + repertoire.FullName.Substring(lgRepertoireSource));
                    if (Directory.Exists(repertoireCible.FullName))
                    {
                        if (repertoire.LastWriteTime > repertoireCible.LastWriteTime)
                        {
                            //on reteste les fichiers un par un
                            m_fichiersSource.AddRange(repertoire.GetFiles("*", SearchOption.TopDirectoryOnly));
                            fichiersDestination.AddRange(repertoireCible.GetFiles("*", SearchOption.TopDirectoryOnly));
                        }
                    }
                    else
                    {
                        CreationRepertoire(repertoireCible.FullName);
                        m_fichiersSource.AddRange(repertoire.GetFiles("*", SearchOption.TopDirectoryOnly));
                    }
                }

                //on conserve les fichiers sources que l'on doit copier
                int i = 0;
                while (i<m_fichiersSource.Count())
                {
                    FileInfo source = m_fichiersSource[i];
                    string nomSource = source.FullName.Substring(lgRepertoireSource);
                    int j = 0;
                    while (j < fichiersDestination.Count() && fichiersDestination[j].FullName.Substring(lgRepertoireDestination) != nomSource) j++;
                    if (j == fichiersDestination.Count())
                    {
                        //on a pas trouvé de fichier équivalent donc il faut le copier
                        i++;
                    }
                    else
                    {
                        // on converse le plus récent
                        if (source.LastWriteTime < fichiersDestination[j].LastWriteTime)
                        {
                            //fichier source plus vieux donc, on ne le copie pas
                            m_fichiersSource.RemoveAt(i);
                        }
                        else
                        {
                            i++;
                        }
                    }
                }
                //calcul de la taille de fichiers à copier
                m_taille_fichiers = 0;
                foreach (FileInfo fichier in m_fichiersSource) { m_taille_fichiers += fichier.Length; };
                m_taille_fichiers_copies = 0;

                
                m_oldcurseur = Cursor;
                Cursor = Cursors.WaitCursor;
                progressBar.Value = 0;
                progressBar.Maximum = 100;
                timerTraitement.Enabled = true;
            }

        }

        private void timerTraitement_Tick(object sender, EventArgs e)
        {
            if (m_traitement== m_fichiersSource.Count)
            {
                Cursor = m_oldcurseur;
                timerTraitement.Enabled = false;
            }
            else
            {
                int lgRepertoireSource = this.textBoxRepertoireSource.Text.Length;
                try
                {
                    m_fichiersSource[m_traitement].CopyTo(this.textBoxRepertoireCible.Text + m_fichiersSource[m_traitement].FullName.Substring(lgRepertoireSource), true);
                    m_taille_fichiers_copies += m_fichiersSource[m_traitement].Length;
                    progressBar.Value = (int)(m_taille_fichiers_copies * 100 / m_taille_fichiers);
                    m_traitement++;
                }
                catch (DirectoryNotFoundException)
                {
                    //le repertoire n'existe pas il faut le créer
                    CreationRepertoire(this.textBoxRepertoireCible.Text + m_fichiersSource[m_traitement].FullName.Substring(lgRepertoireSource));
                }
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
