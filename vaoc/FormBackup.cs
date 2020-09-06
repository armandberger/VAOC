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
            //sauvegarde du répertoire en registry
            RegistryKey regKey = Registry.CurrentUser.CreateSubKey(registryKeyName);
            if (regKey != null)
            {
                regKey.SetValue(registryValueName, this.textBoxRepertoireCible.Text);
                regKey.Close();
            }
            this.Close();
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
                m_traitement = 0;
                //liste des fichiers de la partie
                DirectoryInfo dirSource = new DirectoryInfo(this.textBoxRepertoireSource.Text);
                FileInfo[] fichiersSource = dirSource.GetFiles("*",SearchOption.AllDirectories);

                //liste des fichiers du backup
                DirectoryInfo dirDestination = new DirectoryInfo(this.textBoxRepertoireCible.Text);
                FileInfo[] fichiersDestination = dirDestination.GetFiles("*", SearchOption.AllDirectories);

                m_oldcurseur = Cursor;
                Cursor = Cursors.WaitCursor;
                timerTraitement.Enabled = true;
            }

        }
    }
}
