using System;
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
    public partial class FormVideoBatailles : Form
    {
        public string m_nomfichier;

        public FormVideoBatailles()
        {
            InitializeComponent();
        }

        private void FormVideoBatailles_Load(object sender, EventArgs e)
        {
            foreach(Donnees.TAB_BATAILLERow ligneBataille in Donnees.m_donnees.TAB_BATAILLE)
            {
                comboBoxBatailles.Items.Add(ligneBataille);
            }
            comboBoxBatailles.SelectedIndex = comboBoxBatailles.Items.Add("Toutes");
        }

        private void buttonGenerer_Click(object sender, EventArgs e)
        {
            string retour = string.Empty;
            if(comboBoxBatailles.SelectedItem.ToString().Equals("Toutes"))
            {
                foreach (Donnees.TAB_BATAILLERow ligneBataille in Donnees.m_donnees.TAB_BATAILLE)
                {
                    if (!ligneBataille.IsI_TOUR_FINNull())
                    {
                        retour +=ligneBataille.GenererFilm(m_nomfichier);
                    }
                }
            }
            else
            {
                ((Donnees.TAB_BATAILLERow)comboBoxBatailles.SelectedItem).GenererFilm(m_nomfichier);
            }
            if (string.Empty == retour)
            {
                MessageBox.Show(this, "Génération terminée", "Génération film de bataille", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(this, retour, "Génération film de bataille", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
