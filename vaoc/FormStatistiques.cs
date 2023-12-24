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
using System.Diagnostics;

namespace vaoc
{
    public partial class FormStatistiques : Form
    {
        private string m_repertoireSource;
        private Donnees.TAB_MESSAGEDataTable m_tableMessage;
        private Donnees.TAB_PIONDataTable m_tablePion;

        public FormStatistiques(string fichierCourant)
        {
            InitializeComponent();
            if (null != fichierCourant && fichierCourant != string.Empty)
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
            dataGridOrdres.Width = (Width - SystemInformation.VerticalScrollBarWidth) / 2;
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

            FusionDesMessagesEtDesPions();

            //chargement des valeurs des tables
            string requete;
            dataGridOrdres.Rows.Clear();
            foreach (Donnees.TAB_PIONRow lignePion in m_tablePion)
            {
                if (lignePion.estRole || lignePion.ID_PION==30 || lignePion.ID_PION == 80 || lignePion.ID_PION == 240 || lignePion.ID_PION == 300 || lignePion.ID_PION == 310 || lignePion.ID_PION == 320)
                {
                    //recherche du nombre d'ordres donnés par le joueur sur son propre pion ou les pions sous ses ordres
                    requete = "ID_PION = " + lignePion.ID_PION;
                    requete += OrdresAuxUnites(lignePion);

                    DataRow[] nbordres = Donnees.m_donnees.TAB_ORDRE.Select(requete);
                    DataRow[] nbordresAncien = Donnees.m_donnees.TAB_ORDRE_ANCIEN.Select(requete);
                    dataGridOrdres.Rows.Add(new string[3] { lignePion.nation.S_NOM, lignePion.S_NOM, (nbordres.Count() + nbordresAncien.Count()).ToString() });

                    //recherche du nombre de messages envoyés et reçus par le joueur, hors mis le forum initial
                    //quand il y a eut remplacement tous les messages possédés par ce pion sont transférés au nouveau pion par contre
                    //les messages émis par le précédent pion restent marqués comme envoyés par le pion disparus
                    // => ID_PION_PROPRIETAIRE n'a pas à tenir du compte du remplacement mais ID_PION_EMETTEUR oui
                    var result = from message in m_tableMessage
                                     where (message.ID_PION_EMETTEUR == lignePion.ID_PION) && (message.I_TOUR_DEPART > 1)//il manque tous les messages directs
                                 //where (message.ID_PION_EMETTEUR == lignePion.ID_PION) && (message.I_PHASE_DEPART == 0)
                                 group message by message.ID_PION_PROPRIETAIRE into grps
                                 select new { Key = grps.Key, Value = grps };
                    foreach (var valeur in result)
                    {
                        Donnees.TAB_PIONRow lignePionEmetteur = m_tablePion.FindByID_PION(valeur.Key);
                        if (!lignePionEmetteur.estMessager && lignePion.ID_PION != lignePionEmetteur.ID_PION)
                        {
                            requete = string.Format("ID_PION_EMETTEUR={0} AND ID_PION_PROPRIETAIRE={1} AND I_TOUR_DEPART>0", valeur.Key, lignePion.ID_PION);
                            Donnees.TAB_MESSAGERow[] nbMessagesRecs = (Donnees.TAB_MESSAGERow[])m_tableMessage.Select(requete);
                            dataGridMessages.Rows.Add(new string[5] { lignePion.nation.S_NOM, lignePion.S_NOM, valeur.Value.Count().ToString(), nbMessagesRecs.Count().ToString(), lignePionEmetteur.S_NOM });
                        }
                    }
                    if (!lignePion.IsID_PION_REMPLACENull() && lignePion.ID_PION_REMPLACE > 0)
                    {
                        Donnees.TAB_PIONRow lignePionRemplace = m_tablePion.FindByID_PION(lignePion.ID_PION_REMPLACE);
                        AjouterMessagesEnvoyés(lignePion, lignePionRemplace);
                    }
                }
            }

        }

        private void AjouterMessagesEnvoyés(Donnees.TAB_PIONRow lignePionSource, Donnees.TAB_PIONRow lignePionRemplace)
        {
            var result = from message in m_tableMessage
                         where (message.ID_PION_EMETTEUR == lignePionRemplace.ID_PION) && (message.I_TOUR_DEPART > 1)
                         //where (message.ID_PION_EMETTEUR == lignePionRemplace.ID_PION) && (message.I_PHASE_DEPART == 0)//il manque tous les messages directs
                         group message by message.ID_PION_PROPRIETAIRE into grps
                         select new { Key = grps.Key, Value = grps };
            foreach (var valeur in result)
            {
                Donnees.TAB_PIONRow lignePionEmetteur = m_tablePion.FindByID_PION(valeur.Key);
                if (!lignePionEmetteur.estMessager && lignePionRemplace.ID_PION != lignePionEmetteur.ID_PION)
                {
                    /* recherche de la ligne correspondante précédente */
                    string requete = string.Format("ID_PION_EMETTEUR={0} AND ID_PION_PROPRIETAIRE={1} AND I_TOUR_DEPART>0", valeur.Key, lignePionRemplace.ID_PION);
                    //normalement doit renvoyé 0 puisque les messages ont été transférés
                    Donnees.TAB_MESSAGERow[] nbMessagesRecs = (Donnees.TAB_MESSAGERow[])m_tableMessage.Select(requete);
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
                Donnees.TAB_PIONRow lignePionRemplaceSuite = m_tablePion.FindByID_PION(IdPionRemplace);
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
            foreach (Donnees.TAB_PIONRow lignePionSous in m_tablePion)
            {
                if (!lignePionSous.estJoueur && !lignePionSous.estMessager && lignePionSous.ID_PION_PROPRIETAIRE == lignePion.ID_PION)
                {
                    requete += " OR ID_PION = " + lignePionSous.ID_PION;
                }
            }
            if (!lignePion.IsID_PION_REMPLACENull() && lignePion.ID_PION_REMPLACE > 0)
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
                    char[] caracteresIncorrects = Path.GetInvalidFileNameChars();
                    foreach (char caractere in caracteresIncorrects)
                    {
                        nomPion = nomPion.Replace(caractere, '_');
                    }
                    string nomImage = string.Format("{0}\\disque_statistique_{1}.png",
                                    m_repertoireSource, nomPion);
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

        /// <summary>
        /// =MOYENNE.SI.ENS($Donnees.$T$2:$T$600000;$Donnees.$A$2:$A$600000;"="&A2;$Donnees.$B$2:$B$600000;"=0";$Donnees.$Z$2:$Z$600000;"=True")
        /// </summary>
        /// <returns>table de donnnées</returns>
        private System.Windows.Forms.DataGridView StatistiquesRavitaillement()
        {
            System.Windows.Forms.DataGridView table = new DataGridView();
            table.Columns.Add("tour", "Tour");
            table.Columns.Add("date", "Date");
            table.Columns.Add("camp0", "0");
            table.Columns.Add("camp1", "1");
            Decimal[] ravitaillement = new Decimal[2];

            for (int tour = 0; tour < Donnees.m_donnees.TAB_PARTIE[0].I_TOUR; tour += 24)
            {

                for (int nation = 0; nation < 2; nation++)
                {
                    var result = from ligne in Donnees.m_donnees.TAB_VIDEO
                                 where (ligne.ID_NATION == nation) && (ligne.I_TOUR == tour)
                                    && ((ligne.I_INFANTERIE_INITIALE > 0) || (ligne.I_CAVALERIE_INITIALE > 0))
                                    && (!ligne.B_DETRUIT)
                                 select ligne.I_RAVITAILLEMENT;
                    ravitaillement[nation] = (Decimal)result.ToList().Sum() / result.Count();
                }
                table.Rows.Add(tour, ClassMessager.DateHeure(tour, 0, false), ravitaillement[0], ravitaillement[1]);
            }
            return table;
        }

        /// <summary>
        /// =MOYENNE.SI.ENS($Donnees.$S$2:$S$600000;$Donnees.$A$2:$A$600000;$A2;$Donnees.$B$2:$B$600000;"0";$Donnees.$Z$2:$Z$600000;"True")
        /// </summary>
        /// <returns>table de donnnées</returns>
        private System.Windows.Forms.DataGridView StatistiquesMateriel()
        {
            System.Windows.Forms.DataGridView table = new DataGridView();
            table.Columns.Add("tour", "Tour");
            table.Columns.Add("date", "Date");
            table.Columns.Add("camp0", "0");
            table.Columns.Add("camp1", "1");
            Decimal[] materiel = new Decimal[2];

            for (int tour = 0; tour < Donnees.m_donnees.TAB_PARTIE[0].I_TOUR; tour += 24)
            {

                for (int nation = 0; nation < 2; nation++)
                {
                    var result = from ligne in Donnees.m_donnees.TAB_VIDEO
                                 where (ligne.ID_NATION == nation) && (ligne.I_TOUR == tour)
                                    && ((ligne.I_INFANTERIE_INITIALE > 0) || (ligne.I_CAVALERIE_INITIALE > 0))
                                    && (!ligne.B_DETRUIT)
                                 select ligne.I_MATERIEL;
                    materiel[nation] = (Decimal)result.ToList().Sum() / result.Count();
                }
                table.Rows.Add(tour, ClassMessager.DateHeure(tour, 0, false), materiel[0], materiel[1]);
            }
            return table;
        }

        /// <summary>
        /// =MOYENNE.SI.ENS($Donnees.$M$2:$M$600000;$Donnees.$A$2:$A$600000;"="&A2;$Donnees.$B$2:$B$600000;"=0";$Donnees.$Z$2:$Z$600000;"=True")
        /// </summary>
        /// <returns>table de donnnées</returns>
        private System.Windows.Forms.DataGridView StatistiquesFatigue()
        {
            System.Windows.Forms.DataGridView table = new DataGridView();
            table.Columns.Add("tour", "Tour");
            table.Columns.Add("date", "Date");
            table.Columns.Add("camp0", "0");
            table.Columns.Add("camp1", "1");
            Decimal[] fatigue = new Decimal[2];

            for (int tour = 0; tour < Donnees.m_donnees.TAB_PARTIE[0].I_TOUR; tour += 24)
            {

                for (int nation = 0; nation < 2; nation++)
                {
                    var result = from ligne in Donnees.m_donnees.TAB_VIDEO
                                 where (ligne.ID_NATION == nation) && (ligne.I_TOUR == tour)
                                    && ((ligne.I_INFANTERIE_INITIALE > 0) || (ligne.I_CAVALERIE_INITIALE > 0))
                                    && (!ligne.B_DETRUIT)
                                 select ligne.I_FATIGUE;
                    fatigue[nation] = (Decimal)result.ToList().Sum() / result.Count();
                }
                table.Rows.Add(tour, ClassMessager.DateHeure(tour, 0, false), fatigue[0], fatigue[1]);
            }
            return table;
        }

        /// <summary>
        /// =MOYENNE.SI.ENS($Donnees.$N$2:$N$600000;$Donnees.$A$2:$A$600000;A2;$Donnees.$B$2:$B$600000;"0";$Donnees.$Z$2:$Z$600000;"True")
        /// </summary>
        /// <returns>table de donnnées</returns>
        private System.Windows.Forms.DataGridView StatistiquesMoral()
        {
            System.Windows.Forms.DataGridView table = new DataGridView();
            table.Columns.Add("tour", "Tour");
            table.Columns.Add("date", "Date");
            table.Columns.Add("camp0", "0");
            table.Columns.Add("camp1", "1");
            Decimal[] fatigue = new Decimal[2];

            for (int tour = 0; tour < Donnees.m_donnees.TAB_PARTIE[0].I_TOUR; tour += 24)
            {

                for (int nation = 0; nation < 2; nation++)
                {
                    var result = from ligne in Donnees.m_donnees.TAB_VIDEO
                                 where (ligne.ID_NATION == nation) && (ligne.I_TOUR == tour)
                                    && ((ligne.I_INFANTERIE_INITIALE > 0) || (ligne.I_CAVALERIE_INITIALE > 0))
                                    && (!ligne.B_DETRUIT)
                                 select ligne.I_MORAL;
                    fatigue[nation] = (Decimal)result.ToList().Sum() / result.Count();
                }
                table.Rows.Add(tour, ClassMessager.DateHeure(tour, 0, false), fatigue[0], fatigue[1]);
            }
            return table;
        }

        /// <summary>
        /// =MOYENNE.SI.ENS($Donnees.$N$2:$N$600000;$Donnees.$A$2:$A$600000;A2;$Donnees.$B$2:$B$600000;"0";$Donnees.$Z$2:$Z$600000;"True")
        /// </summary>
        /// <returns>table de donnnées</returns>
        private System.Windows.Forms.DataGridView StatistiquesDepots()
        {
            System.Windows.Forms.DataGridView table = new DataGridView();
            table.Columns.Add("tour", "Tour");
            table.Columns.Add("date", "Date");
            table.Columns.Add("depotA-0", "depotA-0");
            table.Columns.Add("depotA-1", "depotA-1");
            table.Columns.Add("depotB-0", "depotB-0");
            table.Columns.Add("depotB-1", "depotB-1");
            table.Columns.Add("depotC-0", "depotC-0");
            table.Columns.Add("depotC-1", "depotC-1");
            table.Columns.Add("depotD-0", "depotD-0");
            table.Columns.Add("depotD-1", "depotD-1");
            int[] nbDepots = new int[8];

            for (int tour = 0; tour < Donnees.m_donnees.TAB_PARTIE[0].I_TOUR; tour += 24)
            {

                for (int nation = 0; nation < 2; nation++)
                {
                    var result = from ligne in Donnees.m_donnees.TAB_VIDEO
                                 where (ligne.ID_NATION == nation) && (ligne.I_TOUR == tour)
                                    && (ligne.B_DEPOT)
                                    && (!ligne.B_DETRUIT)
                                 select new { niveau = ligne.C_NIVEAU_DEPOT };
                    nbDepots[nation] = result.ToList().Count(x => x.niveau == 'A');
                    nbDepots[nation + 2] = result.ToList().Count(x => x.niveau == 'B');
                    nbDepots[nation + 4] = result.ToList().Count(x => x.niveau == 'C');
                    nbDepots[nation + 6] = result.ToList().Count(x => x.niveau == 'D');
                }
                table.Rows.Add(tour, ClassMessager.DateHeure(tour, 0, false),
                    nbDepots[0], nbDepots[1], nbDepots[2], nbDepots[3],
                    nbDepots[4], nbDepots[5], nbDepots[6], nbDepots[7]);
            }
            return table;
        }

        /// <summary>
        /// Recherche des joueurs les plus communiquants
        /// </summary>
        /// <returns></returns>
        private System.Windows.Forms.DataGridView StatistiquesVolubile()
        {
            System.Windows.Forms.DataGridView table = new DataGridView();
            table.Columns.Add("role", "Role");
            table.Columns.Add("nbmessages", "Nombre Messages");
            table.Columns.Add("pctmessages", "% Nb messages");
            table.Columns.Add("volumemessages", "Volumes Messages");
            table.Columns.Add("volmessages", "% Volumes messages");

            int c = 0;
            string[] nomRole = new string[Donnees.m_donnees.TAB_ROLE.Count];
            int[] nbMessages = new int[Donnees.m_donnees.TAB_ROLE.Count];
            int[] lgMessages = new int[Donnees.m_donnees.TAB_ROLE.Count];
            foreach (Donnees.TAB_ROLERow ligneRole in Donnees.m_donnees.TAB_ROLE)
            {
                Donnees.TAB_PIONRow lignePion = Donnees.m_donnees.TAB_PION.FindByID_PION(ligneRole.ID_PION);
                nomRole[c] = ligneRole.S_NOM;

                foreach (Donnees.TAB_MESSAGERow ligneMessage in Donnees.m_donnees.TAB_MESSAGE)
                {
                    if (ligneMessage.ID_PION_EMETTEUR == lignePion.ID_PION)
                    {
                        nbMessages[c]++;
                        lgMessages[c] += ligneMessage.S_TEXTE.Length;
                    }
                }
                foreach (Donnees.TAB_MESSAGE_ANCIENRow ligneMessage in Donnees.m_donnees.TAB_MESSAGE_ANCIEN)
                {
                    if (ligneMessage.ID_PION_EMETTEUR == lignePion.ID_PION)
                    {
                        nbMessages[c]++;
                        lgMessages[c] += ligneMessage.S_TEXTE.Length;
                    }
                }
                c++;
            }

            //on fait la somme des totaux
            int totalNbMessages = 0;
            int totalLgMessages = 0;
            for (int i = 0; i < Donnees.m_donnees.TAB_ROLE.Count; i++)
            {
                totalNbMessages += nbMessages[i];
                totalLgMessages += lgMessages[i];
            }

            //on ajoute les lignes
            for (int i = 0; i < Donnees.m_donnees.TAB_ROLE.Count; i++)
            {
                table.Rows.Add(nomRole[i],
                                nbMessages[i],
                                Math.Round((decimal)nbMessages[i] * 100 / totalNbMessages, 2, MidpointRounding.AwayFromZero),
                                lgMessages[i], Math.Round((decimal)lgMessages[i] * 100 / totalLgMessages, 2, MidpointRounding.AwayFromZero)
                    );
            }

            return table;
        }

        private void buttonFichiers_Click(object sender, EventArgs e)
        {
            //on exporte déjà les données des tables au format csv
            string nomfichier;
            string nomsfichier = string.Empty;
            string messageErreur = string.Empty;
            messageErreur += Dal.exportCSV(this.dataGridMessages, "Messages", out nomfichier);
            nomsfichier += nomfichier + ",";
            messageErreur += Dal.exportCSV(this.dataGridOrdres, "Ordres", out nomfichier);
            nomsfichier += nomfichier + ",";

            //puis on ajoute des fichiers de statistiques pour le ravitaillement, etc
            messageErreur += Dal.exportCSV(StatistiquesRavitaillement(), "RavitaillementMoyen", out nomfichier);
            nomsfichier += nomfichier + ",";
            messageErreur += Dal.exportCSV(StatistiquesMateriel(), "MaterielMoyen", out nomfichier);
            nomsfichier += nomfichier + ",";
            messageErreur += Dal.exportCSV(StatistiquesFatigue(), "FatigueMoyen", out nomfichier);
            nomsfichier += nomfichier + ",";
            messageErreur += Dal.exportCSV(StatistiquesMoral(), "MoralMoyen", out nomfichier);
            nomsfichier += nomfichier + ",";
            messageErreur += Dal.exportCSV(StatistiquesDepots(), "Depots", out nomfichier);
            nomsfichier += nomfichier + ",";
            messageErreur += Dal.exportCSV(StatistiquesVolubile(), "Volubile", out nomfichier);
            nomsfichier += nomfichier + ",";

            if (string.Empty == messageErreur)
            {
                MessageBox.Show("Fichiers CSV exportés : " + nomsfichier, "Export CSV", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Erreur sur l'export du fichier CSV : " + nomsfichier + " : " + messageErreur,
                    "FormPion", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FusionDesMessagesEtDesPions()
        {
            // C'est très très long !
            //fusion de tous les messages dans une table
            m_tableMessage = (Donnees.TAB_MESSAGEDataTable)Donnees.m_donnees.TAB_MESSAGE.Copy();
            int i = 0;
            while (i < Donnees.m_donnees.TAB_MESSAGE_ANCIEN.Count())
            {
                Donnees.TAB_MESSAGE_ANCIENRow ligneMessage = Donnees.m_donnees.TAB_MESSAGE_ANCIEN[i++];
                Donnees.TAB_MESSAGERow ligneMessageNouveau = m_tableMessage.AddTAB_MESSAGERow(
                    ligneMessage.ID_MESSAGE,
                    ligneMessage.ID_PION_EMETTEUR,
                    ligneMessage.ID_PION_PROPRIETAIRE,
                    ligneMessage.I_TYPE,
                    ligneMessage.IsI_TOUR_ARRIVEENull() ? -1 : ligneMessage.I_TOUR_ARRIVEE,
                    ligneMessage.IsI_PHASE_ARRIVEENull() ? -1 : ligneMessage.I_PHASE_ARRIVEE,
                    ligneMessage.IsI_TOUR_DEPARTNull() ? -1 : ligneMessage.I_TOUR_DEPART,
                    ligneMessage.IsI_PHASE_DEPARTNull() ? -1 : ligneMessage.I_PHASE_DEPART,
                    ligneMessage.IsS_TEXTENull() ? "" : ligneMessage.S_TEXTE,
                    ligneMessage.IsI_INFANTERIENull() ? -1 : ligneMessage.I_INFANTERIE,
                    ligneMessage.IsI_CAVALERIENull() ? -1 : ligneMessage.I_CAVALERIE,
                    ligneMessage.IsI_ARTILLERIENull() ? -1 : ligneMessage.I_ARTILLERIE,
                    ligneMessage.IsI_FATIGUENull() ? -1 : ligneMessage.I_FATIGUE,
                    ligneMessage.IsI_MORALNull() ? -1 : ligneMessage.I_MORAL,
                    ligneMessage.IsI_TOUR_SANS_RAVITAILLEMENTNull() ? -1 : ligneMessage.I_TOUR_SANS_RAVITAILLEMENT,
                    ligneMessage.IsID_BATAILLENull() ? -1 : ligneMessage.ID_BATAILLE,
                    ligneMessage.IsI_ZONE_BATAILLENull() ? -1 : ligneMessage.I_ZONE_BATAILLE,
                    ligneMessage.IsI_RETRAITENull() ? -1 : ligneMessage.I_RETRAITE,
                    ligneMessage.B_DETRUIT,
                    ligneMessage.ID_CASE,
                    ligneMessage.IsID_CASE_DEBUTNull() ? -1 : ligneMessage.ID_CASE_DEBUT,
                    ligneMessage.IsID_CASE_FINNull() ? -1 : ligneMessage.ID_CASE_FIN,
                    ligneMessage.IsI_NB_PHASES_MARCHE_JOURNull() ? -1 : ligneMessage.I_NB_PHASES_MARCHE_JOUR,
                    ligneMessage.IsI_NB_PHASES_MARCHE_NUITNull() ? -1 : ligneMessage.I_NB_PHASES_MARCHE_NUIT,
                    ligneMessage.IsI_NB_HEURES_COMBATNull() ? -1 : ligneMessage.I_NB_HEURES_COMBAT,
                    ligneMessage.IsI_MATERIELNull() ? -1 : ligneMessage.I_MATERIEL,
                    ligneMessage.IsI_RAVITAILLEMENTNull() ? -1 : ligneMessage.I_RAVITAILLEMENT,
                    ligneMessage.IsI_SOLDATS_RAVITAILLESNull() ? -1 : ligneMessage.I_SOLDATS_RAVITAILLES,
                    ligneMessage.IsI_NB_HEURES_FORTIFICATIONNull() ? -1 : ligneMessage.I_NB_HEURES_FORTIFICATION,
                    ligneMessage.IsI_NIVEAU_FORTIFICATIONNull() ? -1 : ligneMessage.I_NIVEAU_FORTIFICATION,
                    ligneMessage.IsI_DUREE_HORS_COMBATNull() ? -1 : ligneMessage.I_DUREE_HORS_COMBAT,
                    ligneMessage.IsI_TOUR_BLESSURENull() ? -1 : ligneMessage.I_TOUR_BLESSURE,
                    ligneMessage.IsC_NIVEAU_DEPOTNull() ? 'X' : ligneMessage.C_NIVEAU_DEPOT
                    );

                if (ligneMessage.IsI_TOUR_ARRIVEENull()) { ligneMessageNouveau.SetI_TOUR_ARRIVEENull(); }
                if (ligneMessage.IsI_PHASE_ARRIVEENull()) { ligneMessageNouveau.SetI_PHASE_ARRIVEENull(); }
                if (ligneMessage.IsI_TOUR_DEPARTNull()) { ligneMessageNouveau.SetI_TOUR_DEPARTNull(); }
                if (ligneMessage.IsI_PHASE_DEPARTNull()) { ligneMessageNouveau.SetI_PHASE_DEPARTNull(); }
                if (ligneMessage.IsS_TEXTENull()) { ligneMessageNouveau.SetS_TEXTENull(); }
                if (ligneMessage.IsI_INFANTERIENull()) { ligneMessageNouveau.SetI_INFANTERIENull(); }
                if (ligneMessage.IsI_CAVALERIENull()) { ligneMessageNouveau.SetI_CAVALERIENull(); }
                if (ligneMessage.IsI_ARTILLERIENull()) { ligneMessageNouveau.SetI_ARTILLERIENull(); }
                if (ligneMessage.IsI_FATIGUENull()) { ligneMessageNouveau.SetI_FATIGUENull(); }
                if (ligneMessage.IsI_MORALNull()) { ligneMessageNouveau.SetI_MORALNull(); }
                if (ligneMessage.IsI_TOUR_SANS_RAVITAILLEMENTNull()) { ligneMessageNouveau.SetI_TOUR_SANS_RAVITAILLEMENTNull(); }
                if (ligneMessage.IsID_BATAILLENull()) { ligneMessageNouveau.SetID_BATAILLENull(); }
                if (ligneMessage.IsI_ZONE_BATAILLENull()) { ligneMessageNouveau.SetI_ZONE_BATAILLENull(); }
                if (ligneMessage.IsI_RETRAITENull()) { ligneMessageNouveau.SetI_RETRAITENull(); }
                if (ligneMessage.IsID_CASE_DEBUTNull()) { ligneMessageNouveau.SetID_CASE_DEBUTNull(); }
                if (ligneMessage.IsID_CASE_FINNull()) { ligneMessageNouveau.SetID_CASE_FINNull(); }
                if (ligneMessage.IsI_NB_PHASES_MARCHE_JOURNull()) { ligneMessageNouveau.SetI_NB_PHASES_MARCHE_JOURNull(); }
                if (ligneMessage.IsI_NB_PHASES_MARCHE_NUITNull()) { ligneMessageNouveau.SetI_NB_PHASES_MARCHE_NUITNull(); }
                if (ligneMessage.IsI_NB_HEURES_COMBATNull()) { ligneMessageNouveau.SetI_NB_HEURES_COMBATNull(); }
                if (ligneMessage.IsI_MATERIELNull()) { ligneMessageNouveau.SetI_MATERIELNull(); }
                if (ligneMessage.IsI_RAVITAILLEMENTNull()) { ligneMessageNouveau.SetI_DUREE_HORS_COMBATNull(); }
                if (ligneMessage.IsI_SOLDATS_RAVITAILLESNull()) { ligneMessageNouveau.SetI_SOLDATS_RAVITAILLESNull(); }
                if (ligneMessage.IsI_NB_HEURES_FORTIFICATIONNull()) { ligneMessageNouveau.SetI_NB_HEURES_FORTIFICATIONNull(); }
                if (ligneMessage.IsI_NIVEAU_FORTIFICATIONNull()) { ligneMessageNouveau.SetI_NIVEAU_FORTIFICATIONNull(); }
                if (ligneMessage.IsI_DUREE_HORS_COMBATNull()) { ligneMessageNouveau.SetI_DUREE_HORS_COMBATNull(); }
                if (ligneMessage.IsI_TOUR_BLESSURENull()) { ligneMessageNouveau.SetI_TOUR_BLESSURENull(); }
                if (ligneMessage.IsC_NIVEAU_DEPOTNull()) { ligneMessageNouveau.SetC_NIVEAU_DEPOTNull(); }
            }

            //fusion de tous les pions dans une seule table
            m_tablePion = (Donnees.TAB_PIONDataTable)Donnees.m_donnees.TAB_PION.Copy();
            int j = 0;
            while (j < Donnees.m_donnees.TAB_PION_ANCIEN.Count())
            {
                //Debug.WriteLine("j=" + j+ "/"+ Donnees.m_donnees.TAB_PION_ANCIEN.Count()+" ajouts="+ m_tablePion.Count());
                Donnees.TAB_PION_ANCIENRow lignePionAncien = Donnees.m_donnees.TAB_PION_ANCIEN[j++];
                if (null == m_tablePion.FindByID_PION(lignePionAncien.ID_PION))//normalement inutile, mais, visiblement, il y a des bugs...
                {
                    Donnees.TAB_PIONRow lignePion = m_tablePion.AddTAB_PIONRow(
                        lignePionAncien.ID_PION,
                        lignePionAncien.ID_MODELE_PION,
                        lignePionAncien.ID_PION_PROPRIETAIRE,
                        lignePionAncien.ID_NOUVEAU_PION_PROPRIETAIRE,
                        lignePionAncien.ID_ANCIEN_PION_PROPRIETAIRE,
                        lignePionAncien.S_NOM,
                        lignePionAncien.I_INFANTERIE,
                        lignePionAncien.I_INFANTERIE_INITIALE,
                        lignePionAncien.I_CAVALERIE,
                        lignePionAncien.I_CAVALERIE_INITIALE,
                        lignePionAncien.I_ARTILLERIE,
                        lignePionAncien.I_ARTILLERIE_INITIALE,
                        lignePionAncien.I_FATIGUE,
                        lignePionAncien.I_MORAL,
                        lignePionAncien.I_MORAL_MAX,
                        lignePionAncien.I_EXPERIENCE,
                        lignePionAncien.I_TACTIQUE,
                        lignePionAncien.I_STRATEGIQUE,
                        lignePionAncien.C_NIVEAU_HIERARCHIQUE,
                        lignePionAncien.I_DISTANCE_A_PARCOURIR,
                        lignePionAncien.I_NB_PHASES_MARCHE_JOUR,
                        lignePionAncien.I_NB_PHASES_MARCHE_NUIT,
                        lignePionAncien.I_NB_HEURES_COMBAT,
                        lignePionAncien.ID_CASE,
                        lignePionAncien.I_TOUR_SANS_RAVITAILLEMENT,
                        lignePionAncien.ID_BATAILLE,
                        lignePionAncien.I_ZONE_BATAILLE,
                        lignePionAncien.I_TOUR_RETRAITE_RESTANT,
                        lignePionAncien.I_TOUR_FUITE_RESTANT,
                        lignePionAncien.B_DETRUIT,
                        lignePionAncien.B_FUITE_AU_COMBAT,
                        lignePionAncien.B_INTERCEPTION,
                        lignePionAncien.B_REDITION_RAVITAILLEMENT,
                        lignePionAncien.B_TELEPORTATION,
                        lignePionAncien.B_ENNEMI_OBSERVABLE,
                        lignePionAncien.I_MATERIEL,
                        lignePionAncien.I_RAVITAILLEMENT,
                        lignePionAncien.B_CAVALERIE_DE_LIGNE,
                        lignePionAncien.B_CAVALERIE_LOURDE,
                        lignePionAncien.B_GARDE,
                        lignePionAncien.B_VIEILLE_GARDE,
                        lignePionAncien.I_TOUR_CONVOI_CREE,
                        lignePionAncien.ID_DEPOT_SOURCE,
                        lignePionAncien.I_SOLDATS_RAVITAILLES,
                        lignePionAncien.I_NB_HEURES_FORTIFICATION,
                        lignePionAncien.I_NIVEAU_FORTIFICATION,
                        lignePionAncien.ID_PION_REMPLACE,
                        lignePionAncien.I_DUREE_HORS_COMBAT,
                        lignePionAncien.I_TOUR_BLESSURE,
                        lignePionAncien.B_BLESSES,
                        lignePionAncien.B_PRISONNIERS,
                        lignePionAncien.B_RENFORT,
                        lignePionAncien.ID_LIEU_RATTACHEMENT,
                        lignePionAncien.C_NIVEAU_DEPOT,
                        lignePionAncien.ID_PION_ESCORTE,
                        lignePionAncien.I_INFANTERIE_ESCORTE,
                        lignePionAncien.I_CAVALERIE_ESCORTE,
                        lignePionAncien.I_MATERIEL_ESCORTE,
                        -1, 0, 0, string.Empty, -1);
                }
            }
        }
    }
}
