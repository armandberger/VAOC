using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Xml;
using Microsoft.Win32;

using WaocLib;
using System.IO;

namespace vaoc
{
    public partial class FormGeneral : Form
    {
        InterfaceVaocWeb iWeb;

        public FormGeneral()
        {
            InitializeComponent();
        }

        private void linkLabelPHP_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //test du site de référence
            PHPService service = new PHPService(e.Link.Name);
            XmlDocument docXML;

            if (service.Version(out docXML))
            {
                if (null != docXML["VERSION"])
                {
                    MessageBox.Show("Appel réussi, numéro de version de l'interface :" + docXML["VERSION"].InnerText, "Interface valide", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Appel réussi, mais numéro de version inconnu", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                if (null != docXML["ERREUR"])
                {
                    MessageBox.Show("linkLabelPHP_LinkClicked :" + docXML["ERREUR"].InnerText, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("linkLabelPHP_LinkClicked : erreur non identifiée sur ll'appel de service.Version", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
           }
       }

       #region propriétés
       public string fichierCourant
       {
           set
           {
               iWeb = ClassVaocWebFactory.CreerVaocWeb(value, false);
               List<ClassDataJeu> liste = iWeb.ListeJeux();
               comboBoxID_JEU.Items.Clear();
               comboBoxID_JEU.DisplayMember = "description";
               foreach (ClassDataJeu jeu in liste)
               {
                   comboBoxID_JEU.Items.Add(jeu);
               }
           }
       }

       public DateTime dateInitiale
       {
            get
            {
                return Convert.ToDateTime(textBoxDateInitiale.Text);
            }
        }

        public int nbTours 
        {
            get
            {
                return Convert.ToInt32(textBoxNbTours.Text);
            }
        }

        public int nbPhases
        {
            get
            {
                return Convert.ToInt32(textBoxNbPhases.Text);
            }
        }

        public string nomScenario
        {
            get
            {
                return textBoxNomScenario.Text;
            }
        }

        public string nomPartie
        {
            get
            {
                return textBoxNomPartie.Text;
            }
            set
            {
                textBoxNomPartie.Text = value;
            }
        }

        public short heureInitiale
        {
            get
            {
                return Convert.ToInt16(numericUpDownHeureInitiale.Value);
            }
        }

        public int echelle
        {
            get
            {
                return Convert.ToInt32(textBoxEchelle.Text);
            }
            set
            {
                textBoxEchelle.Text = Convert.ToString(value);
            }
        }
        
        public int ID_modele_terrain_deploiement
        {
            get
            {
                return Convert.ToInt32(textBoxID_deploiement.Text);
            }
            set
            {
                textBoxID_deploiement.Text = Convert.ToString(value);
            }
        }

        public int coutDeBase
        {
            get
            {
                return Convert.ToInt32(textBoxCoutDeBase.Text);
            }
            set
            {
                textBoxCoutDeBase.Text = Convert.ToString(value);
            }
        }

        public short rayonDeBataille
        {
            get
            {
                return Convert.ToInt16(textBox_rayonDeBataille.Text);
            }
            set
            {
                textBox_rayonDeBataille.Text = Convert.ToString(value);
            }
        }

        public string hostMessagerie
        {
            get
            {
                return textBoxHostMessagerie.Text;
            }
            set
            {
                textBoxHostMessagerie.Text = value;
            }
        }

        public string utilisateurMessagerie
        {
            get
            {
                return textBoxUtilisateurMessagerie.Text;
            }
            set
            {
                textBoxUtilisateurMessagerie.Text = value;
            }
        }

        public string motDePasseMessagerie
        {
            get
            {
                return textBoxMotDePasseMessagerie.Text;
            }
            set
            {
                textBoxMotDePasseMessagerie.Text = value;
            }
        }

        public int meteo
        {
            get
            {
                return Convert.ToInt32(textBoxMeteo.Text);
            }
            set
            {
                textBoxMeteo.Text = Convert.ToString(value);
            }
        }

        public int id_jeu
        {
            get
            {
                ClassDataJeu jeu = (ClassDataJeu)comboBoxID_JEU.SelectedItem;                
                return (null==jeu) ? -1 : jeu.ID_JEU;
            }
            set
            {
                foreach (ClassDataJeu valeur in comboBoxID_JEU.Items)
                {
                    if (valeur.ID_JEU == value)
                    {
                        comboBoxID_JEU.SelectedItem = valeur;
                    }
                }
            }
        }
 
        public int id_partie
        {
            get
            {
                ClassDataPartie partie = (ClassDataPartie)comboBoxID_PARTIE.SelectedItem;
                return (null == partie) ? -1 : partie.ID_PARTIE;
            }
            set
            {
                foreach (ClassDataPartie valeur in comboBoxID_PARTIE.Items)
                {
                    if (valeur.ID_PARTIE == value)
                    {
                        comboBoxID_PARTIE.SelectedItem = valeur;
                    }
                }
            }
        }

        public string nomCarte
        {
            get
            {
                return labelNomCarte.Text;
            }
            set
            {
                labelNomCarte.Text = value;
            }
        }

        public int largeurCarte
        {
            get
            {
                return Convert.ToInt32(labelLargeur.Text);
            }
            set
            {
                labelLargeur.Text = Convert.ToString(value);
            }
        }

        public int hauteurCarte
        {
            get
            {
                return Convert.ToInt32(labelHauteur.Text);
            }
            set
            {
                labelHauteur.Text = Convert.ToString(value);
            }
        }

        public string nomCarteZoom
        {
            get
            {
                return labelNomCarteZoom.Text;
            }
            set
            {
                labelNomCarteZoom.Text = value;
            }
        }

        public string nomCarteGris
        {
            get
            {
                return labelNomCarteGris.Text;
            }
            set
            {
                labelNomCarteGris.Text = value;
            }
        }

        public string nomCarteTopographique
        {
            get
            {
                return labelNomCarteTopographique.Text;
            }
            set
            {
                labelNomCarteTopographique.Text = value;
            }
        }

        public int largeurCarteZoomWeb
        {
            get
            {
                return Convert.ToInt32(textBoxLargeurCarteZoomWeb.Text);
            }
            set
            {
                textBoxLargeurCarteZoomWeb.Text = Convert.ToString(value);
            }
        }

        public int hauteurCarteZoomWeb
        {
            get
            {
                return Convert.ToInt32(textBoxHauteurCarteZoomWeb.Text);
            }
            set
            {
                textBoxHauteurCarteZoomWeb.Text = Convert.ToString(value);
            }
        }

        public int objectifVictoire
        {
            get
            {
                if (comboBoxObjectifVictoire.Text == "Démoralisation de l'adversaire")
                {
                    return Constantes.OBJECTIFS.CST_OBJECTIF_DEMORALISATION;
                }
                return Constantes.CST_IDNULL;
            }
            set
            {
                switch (value)
                {
                    case Constantes.OBJECTIFS.CST_OBJECTIF_DEMORALISATION:
                        comboBoxObjectifVictoire.Text = "Démoralisation de l'adversaire";
                        break;
                    default:
                        comboBoxObjectifVictoire.Text = "objectif inconnu (erreur)";
                        break;
                }
            }
        }
       #endregion

        private void buttonTestMessagerie_Click(object sender, EventArgs e)
        {
            CourrielService.EnvoyerMessage(textBoxTestMessagerie.Text, "VAOC", "test", textBoxHostMessagerie.Text, textBoxUtilisateurMessagerie.Text, textBoxMotDePasseMessagerie.Text);
        }

        private void buttonImageCarte_Click(object sender, EventArgs e)
        {
            Bitmap imageSource;

            if (DialogResult.OK == this.openFileDialog.ShowDialog(this))
            {
                try
                {
                    imageSource = (Bitmap)Image.FromFile(openFileDialog.FileName);
                    labelNomCarte.Text = openFileDialog.FileName;

                    //pour cause de lock, ce qui suit est fait seulement sur OK dans la FormPrincipale
                    //string repertoire = WaocLib.Constantes.repertoireDonnees + labelNomCarte.Text; //Application.ExecutablePath.Substring(0, Application.ExecutablePath.LastIndexOf('\\') + 1);
                    ////destruction d'un eventuel même fichier avec le même nom
                    //File.Delete(repertoire);
                    ////on recopie le fichier image vers le repertoire applicatif des cartes
                    //imageSource.Save(repertoire);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erreur au chargement/sauvegarde de l'image : " + ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                try
                {
                    largeurCarte = imageSource.Width;
                    hauteurCarte = imageSource.Height;
                    imageSource.Dispose();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erreur à l'affectation des tailles de l'image : " + ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
        }

        private void buttonImageCarteZoom_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == this.openFileDialog.ShowDialog(this))
            {
                labelNomCarteZoom.Text = openFileDialog.FileName;
            }
        }

        private void buttonImageCarteGris_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == this.openFileDialog.ShowDialog(this))
            {
                labelNomCarteGris.Text = openFileDialog.FileName;
            }
        }

        private void comboBoxID_JEU_SelectedValueChanged(object sender, EventArgs e)
        {
            ClassDataJeu jeu = (ClassDataJeu)comboBoxID_JEU.SelectedItem;
            List<ClassDataPartie> liste = iWeb.ListeParties(idJeu:jeu.ID_JEU);
            comboBoxID_PARTIE.Items.Clear();
            comboBoxID_PARTIE.DisplayMember = "description";
            foreach (ClassDataPartie partie in liste)
            {
                comboBoxID_PARTIE.Items.Add(partie);
            }
            if (comboBoxID_PARTIE.Items.Count > 0)
            {
                comboBoxID_PARTIE.SelectedIndex = 0;
            }
            textBoxDateInitiale.Text = ClassMessager.DateHeure(jeu.DT_INITIALE);
            numericUpDownHeureInitiale.Value = jeu.I_HEURE_INITIALE;
            textBoxNomScenario.Text = jeu.S_NOM;
            textBoxNbTours.Text = Convert.ToString(jeu.I_NOMBRE_TOURS);
            textBoxNbPhases.Text = Convert.ToString(jeu.I_NOMBRE_PHASES);
        }

        private void comboBoxID_PARTIE_SelectedValueChanged(object sender, EventArgs e)
        {
            ClassDataPartie partie = (ClassDataPartie)comboBoxID_PARTIE.SelectedItem;
            nomPartie = partie.S_NOM;
        }

        private void buttonReinitialiser_Click(object sender, EventArgs e)
        {
        }
    }
}