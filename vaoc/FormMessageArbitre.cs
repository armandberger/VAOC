using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace vaoc
{
    public partial class FormMessageArbitre : Form
    {
        protected ClassNotificationJoueurs notification;

        public FormMessageArbitre()
        {
            InitializeComponent();
        }

        public string fichierCourant
        {
            set
            {
                notification = new ClassNotificationJoueurs(value);

                //initialisation de la liste des utilisateurs
                InterfaceVaocWeb iWeb = ClassVaocWebFactory.CreerVaocWeb(value, false);
                List<ClassDataUtilisateur> liste = iWeb.ListeUtilisateurs(true);
                dataGridViewUtilisateur.DataSource = liste;

                //initialisation de la listbox des roles
                foreach (ClassDataUtilisateur utilisateur in liste)
                {
                    string requete = string.Format("ID_UTILISATEUR={0}", utilisateur.ID_UTILISATEUR);
                    Donnees.TAB_ROLERow[] ligneRoleResultat = (Donnees.TAB_ROLERow[])Donnees.m_donnees.TAB_ROLE.Select(requete);
                    if (0 == ligneRoleResultat.Length)
                    {
                        continue;//c'est juste un utilisateur qui ne joue pas cette partie
                    }

                    foreach (Donnees.TAB_ROLERow ligneRole in ligneRoleResultat)
                    {
                        listBoxRoles.Items.Add(ligneRole);
                    }
                }
                listBoxRoles.DisplayMember = "S_NOM";
            }
        }
        
        
        private void FormMessageArbitre_Load(object sender, EventArgs e)
        {
            Redimensionner();
            //Au chargement si un joueur semble ONR on met une popup d'alerte
            string message = string.Empty;
            List<ClassDataUtilisateur> liste = (List<ClassDataUtilisateur>)dataGridViewUtilisateur.DataSource;

            foreach (ClassDataUtilisateur utilisateur in liste)
            {
                string requete = string.Format("ID_UTILISATEUR={0}", utilisateur.ID_UTILISATEUR);
                Donnees.TAB_ROLERow[] ligneRoleResultat = (Donnees.TAB_ROLERow[])Donnees.m_donnees.TAB_ROLE.Select(requete);
                if (0 == ligneRoleResultat.Length)
                {
                    continue;//c'est juste un utilisateur qui ne joue pas cette partie
                }

                if (utilisateur.I_ONR >= 10 && (DateTime.Now - utilisateur.DT_DERNIERECONNEXION).Days >= 30)
                {
                    if (message == string.Empty) { message = "Les joueurs suivants sont ONR:\r\n"; }
                    message += string.Format("{0} ({1} {2}) {4} tours sans ordre et {5} jours sans se connecter : {3}\r\n",
                        utilisateur.S_LOGIN, utilisateur.S_PRENOM, utilisateur.S_NOM, utilisateur.S_COURRIEL, utilisateur.I_ONR, (DateTime.Now - utilisateur.DT_DERNIERECONNEXION).Days);
                }

                textBoxDernierNotification.Text = Convert.ToString(Donnees.m_donnees.TAB_PARTIE[0].IsI_TOUR_NOTIFICATIONNull() ? 0 : Donnees.m_donnees.TAB_PARTIE[0].I_TOUR_NOTIFICATION);
            }

            if (message != string.Empty)
            {
                MessageBox.Show(message, "Joueurs ONR", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void FormMessageArbitre_Resize(object sender, EventArgs e)
        {
            Redimensionner();
        }

        private void Redimensionner()
        {
            const int ECARTH = 10;

            labelMessageArbitre.Left = textBoxMessage.Left;
            textBoxMessage.Width = Width - 2 * textBoxMessage.Left;
            textBoxMessage.Top = this.labelMessageArbitre.Top + 2 * this.labelMessageArbitre.Height;
            textBoxMessage.Height = Height / 4;

            labelDernierNotification.Left = textBoxMessage.Left;
            labelDernierNotification.Top = textBoxMessage.Top + textBoxMessage.Height + ECARTH;
            textBoxDernierNotification.Left = labelDernierNotification.Left + labelDernierNotification.Width + 20;
            textBoxDernierNotification.Top = labelDernierNotification.Top;

            buttonOK.Left = (Width - buttonOK.Width - buttonAnnuler.Width -20) / 2;
            buttonOK.Top = Height - 3 * buttonOK.Height;
            buttonAnnuler.Left = buttonOK.Left + buttonOK.Width + 20;
            buttonAnnuler.Top = buttonOK.Top;

            dataGridViewUtilisateur.Left = textBoxMessage.Left;
            dataGridViewUtilisateur.Width = textBoxMessage.Width;
            dataGridViewUtilisateur.Height = textBoxMessage.Height;
            dataGridViewUtilisateur.Top = textBoxDernierNotification.Top + textBoxDernierNotification.Height + ECARTH;

            webBrowserNotification.Left = textBoxMessage.Left;
            webBrowserNotification.Width = textBoxMessage.Width/2 - ECARTH;
            webBrowserNotification.Height = textBoxMessage.Height;
            webBrowserNotification.Top = dataGridViewUtilisateur.Top + dataGridViewUtilisateur.Height + ECARTH;

            listBoxRoles.Left = textBoxMessage.Left + webBrowserNotification.Width + 20;
            listBoxRoles.Width = textBoxMessage.Width / 2 - ECARTH;
            listBoxRoles.Height = webBrowserNotification.Height;
            listBoxRoles.Top = webBrowserNotification.Top;
        }

        private void listBoxRoles_SelectedIndexChanged(object sender, EventArgs e)
        {
            string titre;
            StringBuilder texte;
            //chargement du message de notification dans le Navigateur embarqué
            Donnees.TAB_ROLERow ligneRole = (Donnees.TAB_ROLERow) listBoxRoles.SelectedItem;
            if (!notification.NotificationRole(ligneRole, Convert.ToInt32(textBoxDernierNotification.Text), out titre, out texte))
            {
                webBrowserNotification.DocumentText = "<html>Erreur rencontrée dans NotificationRole</html>";
            }
            else
            {
                webBrowserNotification.DocumentText = texte.ToString();
            }

        }
    }
}
