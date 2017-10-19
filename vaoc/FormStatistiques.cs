using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using WaocLib;

namespace vaoc
{
    public partial class FormStatistiques : Form
    {
        private string m_repertoireSource;

        public FormStatistiques(string fichierCourant)
        {
            InitializeComponent();
            if (null!= fichierCourant && fichierCourant != string.Empty)
            {
                int positionPoint = fichierCourant.LastIndexOf("\\");
                m_repertoireSource = fichierCourant.Substring(0, positionPoint);
            }
            this.buttonCouleurOrdre.BackColor = Color.White;
            this.buttonCouleurOrdre.ForeColor = Color.Black;
            this.buttonCouleurRecu.BackColor = Color.Red;
            this.buttonCouleurRecu.ForeColor = Color.White;
            this.buttonCouleurEmis.BackColor = Color.Blue;
            this.buttonCouleurEmis.ForeColor = Color.White;
        }

        private void Redimensionner()
        {
            //#region positionnement des boutons fermer et generer
            //int espaceboutons = buttonGenerer.Right - buttonFermer.Left;
            //buttonFermer.Left = (Width - espaceboutons) / 2;
            //buttonGenerer.Left = buttonFermer.Left + espaceboutons;
            //#endregion

            #region positionnement des grilles
            dataGridOrdres.Top = groupBoxCouleurs.Bottom + groupBoxCouleurs.Height;
            dataGridOrdres.Left = SystemInformation.VerticalScrollBarWidth;
            dataGridOrdres.Width = (Width - SystemInformation.VerticalScrollBarWidth)/2;
            dataGridOrdres.Height = Height - dataGridOrdres.Top - 2 * SystemInformation.HorizontalScrollBarHeight;

            dataGridMessages.Top = dataGridOrdres.Top;
            dataGridMessages.Left = dataGridOrdres.Right + 2 * SystemInformation.VerticalScrollBarWidth;
            dataGridMessages.Width = dataGridOrdres.Width;
            dataGridMessages.Height = dataGridOrdres.Height;
            #endregion
        }

        private void FormStatistiques_Resize(object sender, EventArgs e)
        {
            Redimensionner();
        }

        private void FormStatistiques_Load(object sender, EventArgs e)
        {
            Redimensionner();

            //chargement des valeurs des tables
            string requete;
            dataGridOrdres.Rows.Clear();
            foreach (Donnees.TAB_PIONRow lignePion in Donnees.m_donnees.TAB_PION)
            {
                if (lignePion.estJoueur)
                {
                    //recherche du nombre d'ordres donnés par le joueur sur son propre pion ou les pions sous ses ordres
                    requete = "ID_PION = " + lignePion.ID_PION;
                    requete += OrdresAuxUnites(lignePion);

                    DataRow[] nbordres = Donnees.m_donnees.TAB_ORDRE.Select(requete);
                    DataRow[] nbordresAncien = Donnees.m_donnees.TAB_ORDRE_ANCIEN.Select(requete);
                    dataGridOrdres.Rows.Add(new string[3] { lignePion.nation.S_NOM, lignePion.S_NOM, (nbordres.Count()+ nbordresAncien.Count()).ToString()});

                    //recherche du nombre de messages envoyés et reçus par le joueur, hors mis le forum initial
                    //quand il y a eut remplacement tous les messages possédés par ce pion sont transférés au nouveau pion par contre
                    //les messages émis par le précédent pion restent marqués comme envoyés par le pion disparus
                    // => ID_PION_PROPRIETAIRE n'a pas à tenir du compte du remplacement mais ID_PION_EMETTEUR oui
                    var result = from message in Donnees.m_donnees.TAB_MESSAGE
                                 where (message.ID_PION_EMETTEUR == lignePion.ID_PION) && (message.I_TOUR_DEPART>0)
                                 group message by message.ID_PION_PROPRIETAIRE into grps
                                 select new { Key = grps.Key, Value = grps };
                    foreach (var valeur in result)
                    {
                        Donnees.TAB_PIONRow lignePionEmetteur = Donnees.m_donnees.TAB_PION.FindByID_PION(valeur.Key);
                        if (!lignePionEmetteur.estMessager && lignePion.ID_PION!=lignePionEmetteur.ID_PION)
                        {
                            requete = string.Format("ID_PION_EMETTEUR={0} AND ID_PION_PROPRIETAIRE={1} AND I_TOUR_DEPART>0", valeur.Key, lignePion.ID_PION);
                            Donnees.TAB_MESSAGERow[] nbMessagesRecs = (Donnees.TAB_MESSAGERow[])Donnees.m_donnees.TAB_MESSAGE.Select(requete);
                            dataGridMessages.Rows.Add(new string[5] { lignePion.nation.S_NOM, lignePion.S_NOM, valeur.Value.Count().ToString(), nbMessagesRecs.Count().ToString(), lignePionEmetteur.S_NOM });
                        }
                    }
                    if (!lignePion.IsID_PION_REMPLACENull() && lignePion.ID_PION_REMPLACE > 0)
                    {
                        Donnees.TAB_PIONRow lignePionRemplace = Donnees.m_donnees.TAB_PION.FindByID_PION(lignePion.ID_PION_REMPLACE);
                        AjouterMessagesEnvoyés(lignePion, lignePionRemplace);
                    }
                }
            }
        }

        private void AjouterMessagesEnvoyés(Donnees.TAB_PIONRow lignePionSource, Donnees.TAB_PIONRow lignePionRemplace)
        {
            var result = from message in Donnees.m_donnees.TAB_MESSAGE
                         where (message.ID_PION_EMETTEUR == lignePionRemplace.ID_PION) && (message.I_TOUR_DEPART > 0)
                         group message by message.ID_PION_PROPRIETAIRE into grps
                         select new { Key = grps.Key, Value = grps };
            foreach (var valeur in result)
            {
                Donnees.TAB_PIONRow lignePionEmetteur = Donnees.m_donnees.TAB_PION.FindByID_PION(valeur.Key);
                if (!lignePionEmetteur.estMessager && lignePionRemplace.ID_PION != lignePionEmetteur.ID_PION)
                {
                    /* recherche de la ligne correspondante précédente */
                    string requete = string.Format("ID_PION_EMETTEUR={0} AND ID_PION_PROPRIETAIRE={1} AND I_TOUR_DEPART>0", valeur.Key, lignePionRemplace.ID_PION);
                    //normalement doit renvoyé 0 puisque les messages ont été transférés
                    Donnees.TAB_MESSAGERow[] nbMessagesRecs = (Donnees.TAB_MESSAGERow[])Donnees.m_donnees.TAB_MESSAGE.Select(requete);
                    int i = 0;
                    while (i < dataGridMessages.Rows.Count
                        && (dataGridMessages.Rows[i].Cells[0].Value.ToString() != lignePionSource.nation.S_NOM
                        || dataGridMessages.Rows[i].Cells[1].Value.ToString() != lignePionSource.S_NOM
                        || dataGridMessages.Rows[i].Cells[4].Value.ToString() != lignePionEmetteur.S_NOM)
                        ) i++;
                    if (i < dataGridMessages.Rows.Count)
                    {
                        dataGridMessages.Rows[i].Cells[2].Value = (Convert.ToInt32(dataGridMessages.Rows[i].Cells[2].Value.ToString()) + valeur.Value.Count()).ToString();
                        dataGridMessages.Rows[i].Cells[3].Value = (Convert.ToInt32(dataGridMessages.Rows[i].Cells[3].Value.ToString()) + nbMessagesRecs.Count()).ToString();
                    }
                    else
                    {
                        //nouvelle ligne, le dernier remplaçant n'avait rien envoyé à ce joueur mais le précédent si
                        dataGridMessages.Rows.Add(new string[5] { lignePionSource.nation.S_NOM, lignePionSource.S_NOM, valeur.Value.Count().ToString(), nbMessagesRecs.Count().ToString(), lignePionEmetteur.S_NOM });
                    }
                }
            }
            int IdPionRemplace = lignePionRemplace.ID_PION_REMPLACE;
            if ((Constantes.NULLENTIER != IdPionRemplace) && (IdPionRemplace > 0))
            {
                Donnees.TAB_PIONRow lignePionRemplaceSuite = Donnees.m_donnees.TAB_PION.FindByID_PION(IdPionRemplace);
                AjouterMessagesEnvoyés(lignePionSource, lignePionRemplaceSuite);
            }
        }

        /*
        private int NombreMessagesEnvoyes(Donnees.TAB_PIONRow lignePion, ref int nbMessagesRecus)
        {
            var result = from message in Donnees.m_donnees.TAB_MESSAGE
                         where (message.ID_PION_EMETTEUR == lignePion.ID_PION) && (message.I_TOUR_DEPART > 0)
                         group message by message.ID_PION_PROPRIETAIRE into grps
                         select new { Key = grps.Key, Value = grps };
            foreach (var valeur in result)
            {
                Donnees.TAB_PIONRow lignePionEmetteur = Donnees.m_donnees.TAB_PION.FindByID_PION(valeur.Key);
                if (!lignePionEmetteur.estMessager && lignePion.ID_PION != lignePionEmetteur.ID_PION)
                {
                    string requete = string.Format("ID_PION_EMETTEUR={0} AND ID_PION_PROPRIETAIRE={1} AND I_TOUR_DEPART>0", valeur.Key, lignePion.ID_PION);
                    Donnees.TAB_MESSAGERow[] nbMessagesRecs = (Donnees.TAB_MESSAGERow[])Donnees.m_donnees.TAB_MESSAGE.Select(requete);
                    nbMessagesRecus += nbMessagesRecs.Count();
                }
            }
        }

        private int NombreMessagesRecues(Donnees.TAB_PIONRow lignePion, int idPIONEMETTEUR)
        {
            int iRetour = 0;
            string requete = string.Format("ID_PION_EMETTEUR={0} AND ID_PION_PROPRIETAIRE={1} AND I_TOUR_DEPART>0", idPIONEMETTEUR, lignePion.ID_PION);
            Donnees.TAB_MESSAGERow[] nbMessagesRecs = (Donnees.TAB_MESSAGERow[])Donnees.m_donnees.TAB_MESSAGE.Select(requete);
            iRetour = nbMessagesRecs.Count();
            if (!lignePion.IsID_PION_REMPLACENull() && lignePion.ID_PION_REMPLACE > 0)
            {
                Donnees.TAB_PIONRow lignePionRemplace = Donnees.m_donnees.TAB_PION.FindByID_PION(lignePion.ID_PION_REMPLACE);
                iRetour += NombreMessagesRecues(lignePionRemplace, idPIONEMETTEUR);
            }
            return iRetour; 
        }
        */

        private string OrdresAuxUnites(Donnees.TAB_PIONRow lignePion)
        {
            string requete = string.Empty;
            foreach (Donnees.TAB_PIONRow lignePionSous in Donnees.m_donnees.TAB_PION)
            {
                if (!lignePionSous.estJoueur && !lignePionSous.estMessager && lignePionSous.ID_PION_PROPRIETAIRE == lignePion.ID_PION)
                {
                    requete += " OR ID_PION = " + lignePionSous.ID_PION;
                }
            }
            if (!lignePion.IsID_PION_REMPLACENull() &&  lignePion.ID_PION_REMPLACE>0)
            {
                requete += " OR ID_PION = " + lignePion.ID_PION_REMPLACE;
                /* ce qui suit ne sert à rien puisque tous les pions ont été transférés lors du remplacement
                Donnees.TAB_PIONRow lignePionRemplace = Donnees.m_donnees.TAB_PION.FindByID_PION(lignePion.ID_PION_REMPLACE);
                requete += OrdresAuxUnites(lignePionRemplace);
                */
            }
            return requete;
        }

        /// <summary>
        /// On dessine des disques avec un cercle pour le nombre d'ordres, un pour le nombre de messages émis, un pour le nombre de messages reçus
        /// </summary>
        private void buttonGenerer_Click(object sender, EventArgs e)
        {
            int nbOrdres, nbEmis, nbRecus;
            int epaisseur = Convert.ToInt32(this.textBoxRayon.Text);
            foreach (Donnees.TAB_PIONRow lignePion in Donnees.m_donnees.TAB_PION)
            {
                if (lignePion.estJoueur)
                {
                    nbOrdres = nbEmis = nbRecus = 0;
                    foreach (DataGridViewRow ligne in dataGridOrdres.Rows)
                    {
                        if (lignePion.S_NOM == ligne.Cells[1].Value.ToString())
                        {
                            nbOrdres = Convert.ToInt32(ligne.Cells[2].Value);
                        }
                    }

                    foreach (DataGridViewRow ligne in dataGridMessages.Rows)
                    {
                        if (lignePion.S_NOM == ligne.Cells[1].Value.ToString())
                        {
                            nbEmis += Convert.ToInt32(ligne.Cells[2].Value);
                            nbRecus += Convert.ToInt32(ligne.Cells[3].Value);
                        }
                    }

                    //on trace le disque statistique
                    Pen stylo = new Pen(Color.Black, 1);
                    int tailleImage = epaisseur * (nbEmis + nbRecus + nbOrdres) + 2 * (int)stylo.Width;
                    Bitmap image = new Bitmap(tailleImage, tailleImage);
                    Graphics graph = Graphics.FromImage(image);

                    Brush brosse = new SolidBrush(this.buttonCouleurEmis.BackColor);
                    int taille = epaisseur * (nbEmis + nbRecus + nbOrdres);
                    graph.FillEllipse(brosse, (tailleImage - taille) / 2, (tailleImage - taille) / 2, taille, taille);
                    graph.DrawEllipse(stylo, (tailleImage - taille) / 2, (tailleImage - taille) / 2, taille, taille);

                    taille = epaisseur * (nbRecus + nbOrdres);
                    brosse = new SolidBrush(this.buttonCouleurRecu.BackColor);
                    graph.FillEllipse(brosse, (tailleImage - taille) / 2, (tailleImage - taille) / 2, taille, taille);
                    graph.DrawEllipse(stylo, (tailleImage - taille) / 2, (tailleImage - taille) / 2, taille, taille);

                    taille = epaisseur * nbOrdres;
                    brosse = new SolidBrush(this.buttonCouleurOrdre.BackColor);
                    graph.FillEllipse(brosse, (tailleImage - taille) / 2, (tailleImage - taille) / 2, taille, taille);
                    graph.DrawEllipse(stylo, (tailleImage - taille) / 2, (tailleImage - taille) / 2, taille, taille);
                    
                    string nomPion = lignePion.S_NOM;
                    char []caracteresIncorrects = Path.GetInvalidFileNameChars();
                    foreach (char caractere in caracteresIncorrects)
                    {
                        nomPion = nomPion.Replace(caractere, '_');
                    }
                    string nomImage = string.Format("{0}\\disque_statistique_{1}.png",
                                    m_repertoireSource,nomPion); 
                    image.Save(nomImage, System.Drawing.Imaging.ImageFormat.Png);
                    image.Dispose();
                }
            }
            MessageBox.Show("Images statistiques générées sans erreur", this.Name, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void buttonCouleurOrdre_Click(object sender, EventArgs e)
        {
            couleurDialog.Color = buttonCouleurOrdre.BackColor;
            if (DialogResult.OK == couleurDialog.ShowDialog())
            {
                buttonCouleurOrdre.BackColor = couleurDialog.Color;
                buttonCouleurOrdre.ForeColor = (buttonCouleurOrdre.BackColor.R + buttonCouleurOrdre.BackColor.G + buttonCouleurOrdre.BackColor.B > 3 * 127) ? Color.Black : Color.White;
            }
        }

        private void buttonCouleurRecu_Click(object sender, EventArgs e)
        {
            couleurDialog.Color = buttonCouleurRecu.BackColor;
            if (DialogResult.OK == couleurDialog.ShowDialog())
            {
                buttonCouleurRecu.BackColor = couleurDialog.Color;
                buttonCouleurRecu.ForeColor = (buttonCouleurRecu.BackColor.R + buttonCouleurRecu.BackColor.G + buttonCouleurRecu.BackColor.B > 3 * 127) ? Color.Black : Color.White;
            }
        }

        private void buttonCouleurEmis_Click(object sender, EventArgs e)
        {
            couleurDialog.Color = buttonCouleurEmis.BackColor;
            if (DialogResult.OK == couleurDialog.ShowDialog())
            {
                buttonCouleurEmis.BackColor = couleurDialog.Color;
                buttonCouleurEmis.ForeColor = (buttonCouleurEmis.BackColor.R + buttonCouleurEmis.BackColor.G + buttonCouleurEmis.BackColor.B > 3 * 127) ? Color.Black : Color.White;
            }
        }
    }
}
