﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace vaoc
{
    public partial class FormTests : Form
    {
        public FormTests()
        {
            InitializeComponent();
        }

        private void AlimentationDesMessages(string[] chaineDeDonnees)
        {
            string phrase, stype;
            Random de = new Random();
            Dictionary<string, int> cptTypes = new Dictionary<string, int>();
            textBoxResultat.Text = string.Empty;

            foreach (ClassMessager.MESSAGES tipeMessage in Enum.GetValues(typeof(ClassMessager.MESSAGES)))
            {
                cptTypes.Add(tipeMessage.ToString(), 0);
            }

            Donnees.TAB_PHRASERow[] lignesPhrases = (Donnees.TAB_PHRASERow[])Donnees.m_donnees.TAB_PHRASE.Select("", "I_TYPE");
            foreach (Donnees.TAB_PHRASERow lignePhrase in lignesPhrases)
            {
                try
                {
                    stype = "INCONNU";//crash peut-être car non défini à priori
                    foreach (ClassMessager.MESSAGES tipeMessage in Enum.GetValues(typeof(ClassMessager.MESSAGES)))
                    {
                        if ((int)tipeMessage == lignePhrase.I_TYPE)
                        {
                            stype = tipeMessage.ToString();
                        }
                    }
                    cptTypes[stype] = cptTypes[stype] + 1;

                    //on tire des données au hasard pour génerer une phrase "type" ->trop complexe, revient à faire le module de phrase
                    //Donnees.TAB_PIONRow lignePion = Donnees.m_donnees.TAB_PION[de.Next(Donnees.m_donnees.TAB_PION.Count)];
                    //Donnees.TAB_BATAILLERow ligneBataille = Donnees.m_donnees.TAB_BATAILLE[de.Next(Donnees.m_donnees.TAB_BATAILLE.Count)];
                    //Donnees.TAB_METEORow ligneMeteo = Donnees.m_donnees.TAB_METEO[de.Next(Donnees.m_donnees.TAB_METEO.Count)];
                    phrase = string.Format("I_type={0}({1})", lignePhrase.I_TYPE, stype) +
                        string.Format(lignePhrase.S_PHRASE,chaineDeDonnees);
                    textBoxResultat.Text += string.Format("Phrase ID={0} : {1}\r\n",
                        lignePhrase.ID_PHRASE, phrase);
                }
                catch (Exception ex)
                {
                    textBoxResultat.Text += string.Format("Exception phrase ID={0} : {1}\r\n",
                        lignePhrase.ID_PHRASE, ex.Message);
                }
            }

            //affiche le nombre de phrases écrites par type de message
            try
            {
                textBoxResultat.Text += "\r\n";
                foreach (KeyValuePair<string, int> kvp in cptTypes)
                {
                    textBoxResultat.Text += string.Format("{0} phrases de type {1}\r\n",
                        kvp.Value, kvp.Key);
                }
            }
            catch (Exception ex)
            {
                textBoxResultat.Text += string.Format("Exception dans le parcours du nombre de phrases écrites : {0}\r\n", ex.Message);
            }



            MessageBox.Show("Fin des tests Messages", "Tests Messages", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// on balaye tous les messages avec leur code de remplissage pour voir ce que
        /// donne les messages
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonMessages_Click(object sender, EventArgs e)
        {
            AlimentationDesMessages(new string[] {
"<0:DateHeure>",
"<1:nom du pion>",
"<2:nomZoneGeographique>",
"<3:nomBataille>",
"<4:iPertesInfanterie>",
"<5:lignePion.I_INFANTERIE>",
"<6:iPertesCavalerie>",
"<7:lignePion.I_CAVALERIE>",
"<8:iPertesArtillerie>",
"<9:lignePion.I_ARTILLERIE>",
"<10:moralPerduOuGagne>",
"<11:lignePion.I_MORAL>",
"<12:lignePion.I_MORAL_MAX>",
"<13:fatiguePerduOuGagne>",
"<14:lignePion.I_FATIGUE>",
"<15:lignePion.I_TOUR_SANS_RAVITAILLEMENT>",
"<16:cri de ralliement>",
"<17:nom du supérieur>",
"<18:unités attaquantes>",
"<19:nations attaquantes>",
"<20:météo>",
"<21:type d'ordre>",
"<22:unités voisines>",
"<23:nomZoneGeographiqueOrdre>",
"<24:ordreHeureDebut>",
"<25:ordreHeureFin>",
"<26:distanceBataille>",
"<27:message intercepté>",
"<28:effectifs>",
"<29:nom unité cible>",
"<30:distance depot>",
"<31:nom depot>",
"<32:type terrain>",
"<33:effectifsPerdus>",
"<34:ravitaillement>",
"<35:materiel>",
"<36:ravitaillementPerduGagne>",
"<37:materielPerduGagne>",
"<38:nomDuChefRemplace>",
"<39:Modèle unité>",
            }
                        );
        }

        private void buttonCasesNoms_Click(object sender, EventArgs e)
        {
            textBoxResultat.Text = string.Empty;

            Cursor oldCursor = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;
            string nomPrecedent = string.Empty;
            foreach (Donnees.TAB_NOMS_CARTERow ligneNomCarte in Donnees.m_donnees.TAB_NOMS_CARTE.OrderBy(u => u["S_NOM"]))
            {
                Donnees.TAB_CASERow ligneCase = Donnees.m_donnees.TAB_CASE.FindParID_CASE(ligneNomCarte.ID_CASE);
                Donnees.TAB_MODELE_TERRAINRow ligneModeleTerrain = Donnees.m_donnees.TAB_MODELE_TERRAIN.FindByID_MODELE_TERRAIN(ligneCase.ID_MODELE_TERRAIN);

                if (!this.checkBoxHorsVille.Checked || ligneModeleTerrain.S_NOM.IndexOf("ville", 0, StringComparison.CurrentCultureIgnoreCase) < 0)
                {
                    string doublon = (nomPrecedent == ligneNomCarte.S_NOM) ? "Doublon !" : string.Empty;
                    textBoxResultat.Text += string.Format("{0} en {1} : {2},{3} PV={4} {5}\r\n",
                        ligneNomCarte.S_NOM,
                        ligneModeleTerrain.S_NOM,
                        ligneCase.I_X,
                        ligneCase.I_Y,
                        ligneNomCarte.I_VICTOIRE,
                        doublon);
                }
                nomPrecedent = ligneNomCarte.S_NOM;
            }
            Cursor.Current = oldCursor;
        }

        /// <summary>
        /// on balaye tous les messages avec un libelle réel pour voir ce que
        /// donne les messages
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonMessageTexteFictif_Click(object sender, EventArgs e)
        {
            string nomZoneGeographique;
            Cursor oldCursor = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;
            Donnees.TAB_CASERow ligneCase = Donnees.m_donnees.TAB_CASE.FindParID_CASE(0);

            ClassMessager.CaseVersZoneGeographique(ligneCase.ID_CASE, out nomZoneGeographique);

            AlimentationDesMessages(new string[] {
"<0:"+ClassMessager.DateHeure(Donnees.m_donnees.TAB_PARTIE[0].I_TOUR, Donnees.m_donnees.TAB_PARTIE[0].I_PHASE)+">",
"<1:5 div:Hussards>",
"<2:"+nomZoneGeographique+">",
"<3:la bataille de Paris>",
"<4:1244>",
"<5:5000>",
"<6:888>",
"<7:2000>",
"<8:7>",
"<9:15>",
"<10:9>",
"<11:22>",
"<12:40>",
"<13:23>",
"<14:44>",
"<15:2>",
"<16:vive nous !>",
"<17:Maréchal Machin>",
"<18:1 div:Gag, 2div:Truc et 3div:Machin>",
"<19:"+Donnees.m_donnees.TAB_NATION[0].S_NOM+">",
"<20:"+Donnees.m_donnees.TAB_METEO[0].S_NOM+">",
"<21:mouvement>",
"<22:1 div:Gag, 2div:Truc et 3div:Machin>",
"<23:"+nomZoneGeographique+">",
"<24:8>",
"<25:18>",
"<26:5 km au Sud-Ouest>",
"<27:je vais attaquer bientôt, ne le dites à personnes>",
"<28:8000 fantassins, 2000 cavaliers, 15 canons>",
"<29:5div:Tiraillon>",
"<30:35>",
"<31:Dépôt du Graillon>",
"<32:"+Donnees.m_donnees.TAB_MODELE_TERRAIN[0].S_NOM+">",
"<33:2540 fantassins, 670 cavaliers, 2 canons>",
"<33:65>",
"<34:75>",
"<35:14>",
"<36:11>",
"<37:-10>",
"<38:Général Bidon>",
"<39:Division française>"
            }
                        );
            Cursor.Current = oldCursor;
        }

        /// <summary>
        /// affiche toutes les cases d'hopitaux de la carte par ordre alphabetique
        /// </summary>
        private void buttonHopitaux_Click(object sender, EventArgs e)
        {
            Donnees.TAB_NOMS_CARTERow[] liste;
            bool bPremier;
            textBoxResultat.Text = string.Empty;
            Cursor oldCursor = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;

            textBoxResultat.Text += string.Format("Hôpitaux :\r\n");
            bPremier = true;
            liste = (Donnees.TAB_NOMS_CARTERow[])Donnees.m_donnees.TAB_NOMS_CARTE.Select("B_HOPITAL=true", "S_NOM");
            foreach (Donnees.TAB_NOMS_CARTERow ligneNomCarte in liste)
            {
                if (bPremier) { bPremier = false; } else { textBoxResultat.Text += ", "; }
                textBoxResultat.Text += ligneNomCarte.S_NOM;
            }
            textBoxResultat.Text += ".\r\n\r\n";

            textBoxResultat.Text += string.Format("Prisons :\r\n");
            bPremier = true;
            liste = (Donnees.TAB_NOMS_CARTERow[])Donnees.m_donnees.TAB_NOMS_CARTE.Select("B_PRISON=true", "S_NOM");
            foreach (Donnees.TAB_NOMS_CARTERow ligneNomCarte in liste)
            {
                if (bPremier) { bPremier = false; } else { textBoxResultat.Text += ", "; }
                textBoxResultat.Text += ligneNomCarte.S_NOM;
            }
            textBoxResultat.Text += ".\r\n\r\n";

            textBoxResultat.Text += string.Format("Création de dépôts possible :\r\n");
            bPremier = true;
            liste = (Donnees.TAB_NOMS_CARTERow[])Donnees.m_donnees.TAB_NOMS_CARTE.Select("B_CREATION_DEPOT=true", "S_NOM");
            foreach (Donnees.TAB_NOMS_CARTERow ligneNomCarte in liste)
            {
                if (bPremier) { bPremier = false; } else { textBoxResultat.Text += ", "; }
                textBoxResultat.Text += ligneNomCarte.S_NOM;
            }
            textBoxResultat.Text += ".";
            Cursor.Current = oldCursor;
        }
    }
}
