using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using Microsoft.Win32;
using WaocLib;

namespace vaoc
{
    public partial class FormCarte : Form
    {
        #region donn�es
        private int                                     m_traitement;//traitement principal
        private int                                     m_sous_traitement;//sous t�che d'un traitement
        private Cursor                                  m_oldcurseur;
        private Bitmap                                  m_imageCarte;
        private Donnees.TAB_CASEDataTable               m_tableCase;
        private Donnees.TAB_CASERow[]                   m_tableCaseTraitement;
        private DateTime                                m_dateDebut;
        #endregion

        #region propri�t�s

        public Donnees.TAB_CASEDataTable tableCase
        {
            get
            {
                return (Donnees.TAB_CASEDataTable)m_tableCase;
            }
            set
            {
                m_tableCase = (Donnees.TAB_CASEDataTable)value.Copy();
            }
        }

        public string nomCarteTopographique
        {
            set
            {
                labelNomCarteTopographique.Text = value;
                string repertoireDest = Constantes.repertoireDonnees + labelNomCarteTopographique.Text;
                try
                {
                    m_imageCarte = (Bitmap)Image.FromFile(repertoireDest);
                    labelLargeur.Text = m_imageCarte.Width.ToString();
                    labelHauteur.Text = m_imageCarte.Height.ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erreur au chargement de l'image : " + repertoireDest + ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        #endregion
 
        public FormCarte()
        {
            InitializeComponent();
        }


        private void timerTraitement_Tick(object sender, EventArgs e)
        {
            if (!Traitement())
            {
                timerTraitement.Enabled = false;
                Cursor = m_oldcurseur;
            }
        }

        /// <summary>
        /// G�n�ration d'un nouvelle carte en base d'apr�s l'image
        /// </summary>
        private void buttonGeneration_Click(object sender, EventArgs e)
        {
            if (m_traitement > 0 || m_sous_traitement > 0)
            {
                Cursor = m_oldcurseur;
                timerTraitement.Enabled = false;
                if (DialogResult.No == MessageBox.Show("Voulez vous suspendre le traitement en cours ?", "G�n�ration Carte", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    Cursor = Cursors.WaitCursor;
                    timerTraitement.Enabled = true;
                }
                else
                {
                    buttonGeneration.Text = "G�n�rer la carte";
                }
            }
            else
            {
                m_traitement = m_sous_traitement = 0;
                if (m_tableCase.Rows.Count > 0)
                {
                    DialogResult retourMessage=MessageBox.Show("Voulez vous reprendre l'analyse � partir de z�ro (sinon reprend � partir du point d'arr�t)", "Traitement Carte", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                    switch (retourMessage)
                    {
                        case DialogResult.Yes :
                            m_tableCase.Rows.Clear();
                            break;
                        case DialogResult.No :
                            //on interdit l'interuption durant la premi�re �tape (trop complexe � g�rer)
                            m_traitement = 1;
                            break;
                        default:
                            //cancel
                            return;
                    }
                }

                m_oldcurseur = Cursor;
                Cursor = Cursors.WaitCursor;
                timerTraitement.Enabled = true;
            }
        }
        
        /// <summary>
        /// Traitement en cours jusqu'� l'�tape suivante
        /// </summary>
        /// <returns>false si un probl�me a eut lieu ou si le traitement est termin�</returns>
        private bool Traitement()
        {
            int y;

            switch (m_traitement)
            {
                case 0:
                    if (m_sous_traitement < m_imageCarte.Width)
                    {
                        if (0 == m_sous_traitement)
                        {
                            labelTraitement.Text = "construction des cases";
                            progressBar.Maximum = m_imageCarte.Width;
                            tableCase.Clear();
                            tableCase.AcceptChanges();
                            m_dateDebut = DateTime.Now;
                        }
                        //on analyse la ligne de pixels suivants
                        for (y = 0; y < m_imageCarte.Height; y++)
                        {
                            Color pixelColor = m_imageCarte.GetPixel(m_sous_traitement, y);
                            if (!CreationCase(m_sous_traitement* m_imageCarte.Height + y, m_sous_traitement, y, pixelColor))
                            {
                                labelTraitement.Text = "erreur";
                                return false;//traitement termin�
                            }
                        }
                        progressBar.Value = m_sous_traitement;
                        m_sous_traitement++;
                        AfficherTemps();
                        Invalidate();
                    }
                    else
                    {
                        m_sous_traitement = 0;
                        m_traitement++;
                    }
                    break;
                case 1:
                    //on parcours toutes les cases pour d�terminer les cases de remplacement (cas des routes)
                    //on optimise les traitements en metteant les cases en m�moire
                    if (0 == m_sous_traitement)
                    {
                        tableCase.InitialisationListeCase(m_imageCarte.Width, m_imageCarte.Height);//optimisation m�moire
                        tableCase.AcceptChanges();//on valide les ajouts du traitement pr�c�dent
                        labelTraitement.Text = "cases de remplacement";
                        m_tableCaseTraitement = (Donnees.TAB_CASERow[])m_tableCase.Select("ID_MODELE_TERRAIN_SI_OCCUPE IS NULL");
                        progressBar.Maximum = m_tableCaseTraitement.Length;
                        m_dateDebut = DateTime.Now;
                        buttonGeneration.Text = "Arr�t";
                    }

                    if (m_sous_traitement < m_tableCaseTraitement.Length)
                    {
                        //CreationsVoisins(m_tableCase[m_sous_traitement]); -> inutile a priori, facile a d�duire durant le traitement
                        CreationCasesDeRemplacement(m_tableCaseTraitement[m_sous_traitement]);
                        if (0 == m_sous_traitement % 100)
                        {
                            Debug.WriteLine("");
                            progressBar.Value = m_sous_traitement;
                            AfficherTemps();
                            Invalidate();
                        }
                        m_sous_traitement++;
                    }
                    else
                    {
                        m_sous_traitement = 0;
                        m_traitement++;
                    }
                    break;
                case 3:
                    //on met seulement � jour les modeles de terrain suite � une modification mineure de l'image source
                    if (m_sous_traitement < m_imageCarte.Width)
                    {
                        if (0 == m_sous_traitement)
                        {
                            labelTraitement.Text = "mise � jour des mod�les des cases";
                            progressBar.Maximum = m_imageCarte.Width;
                            m_dateDebut = DateTime.Now;
                        }
                        //on analyse la ligne de pixels suivants
                        for (y = 0; y < m_imageCarte.Height; y++)
                        {
                            Color pixelColor = m_imageCarte.GetPixel(m_sous_traitement, y);
                            if (!MiseAJourCase(m_sous_traitement, y, pixelColor))
                            {
                                labelTraitement.Text = "erreur";
                                return false;//traitement termin�
                            }
                        }
                        progressBar.Value = m_sous_traitement;
                        m_sous_traitement++;
                        AfficherTemps();
                        Invalidate();
                    }
                    else
                    {
                        m_sous_traitement = 0;
                        m_traitement++;
                    }
                    break;
                case 2:
                case 4:
                    tableCase.AcceptChanges();
                    buttonGeneration.Text = "G�n�rer la carte";
                    labelTraitement.Text = "termin�";
                    return false;//traitement termin�
            }
            return true;
        }

        private void AfficherTemps()
        {
            TimeSpan tPasse=DateTime.Now.Subtract(m_dateDebut);
            long nbrestant;

            if (progressBar.Value > 0)
            {
                nbrestant = (long)(tPasse.TotalSeconds * (progressBar.Maximum - progressBar.Value) / progressBar.Value);
            }
            else
            {
                nbrestant = (long)(tPasse.TotalSeconds * progressBar.Maximum);
            }

            if (tPasse.TotalHours >= 1)
            {
                labelTempsPasse.Text = String.Format("{0} heures {1} minutes", Math.Floor(tPasse.TotalHours), tPasse.Minutes);
            }
            else
            {
                if (tPasse.TotalMinutes >= 1)
                {
                    labelTempsPasse.Text = String.Format("{0} minutes", tPasse.Minutes);
                }
                else
                {
                    labelTempsPasse.Text = String.Format("{0} secondes", tPasse.Seconds);
                }
            }
            
            if (nbrestant > 3600)
            {
                labelTempsRestant.Text = String.Format("{0} heures {1} minutes", nbrestant / 3600, (nbrestant % 3600)/60);
            }
            else
            {
                if (nbrestant > 60)
                {
                    labelTempsRestant.Text = String.Format("{0} minutes", nbrestant / 60);
                }
                else
                {
                    labelTempsRestant.Text = String.Format("{0} secondes", nbrestant);
                }
            }
        }

        private bool CreationCase(int idCase, int x, int y, Color pixelColor)
        {
            int j, k;
            string requete,message;
            Donnees.TAB_POINTRow[] resPoint;
            Donnees.TAB_GRAPHISMERow[] resGraphique;
            Donnees.TAB_MODELE_TERRAINRow[] resModeleTerrain=null;
            bool btrouve;

            //recherche du mod�le de terrain associ� � cette couleur
            btrouve = false;
            requete = String.Format("I_ROUGE={0} AND I_VERT={1} AND I_BLEU={2}", pixelColor.R, pixelColor.G, pixelColor.B);
            resPoint = (Donnees.TAB_POINTRow[])Donnees.m_donnees.TAB_POINT.Select(requete);
            j = 0;
            while (!btrouve && j < resPoint.Length)
            {
                //on cherche le graphique associ� au point de couleur
                resGraphique = (Donnees.TAB_GRAPHISMERow[])Donnees.m_donnees.TAB_GRAPHISME.Select("ID_POINT=" + resPoint[j].ID_POINT);
                k = 0;
                while (!btrouve && k < resGraphique.Length)
                {
                    resModeleTerrain = (Donnees.TAB_MODELE_TERRAINRow[])Donnees.m_donnees.TAB_MODELE_TERRAIN.Select("ID_GRAPHIQUE=" + resGraphique[k].ID_GRAPHIQUE);
                    if (resModeleTerrain.Length > 0)
                    {
                        btrouve = true;
                    }
                    else
                    {
                        message = String.Format("Impossible de trouver le mod�le de terrain pour le graphique ID_GRAPHIQUE : {0}", resGraphique[k].ID_GRAPHIQUE);
                        throw new Exception(message);
                    }
                    k++;
                }
                j++;
            }

            if (!btrouve)
            {
                message = String.Format("Impossible de trouver le mod�le de terrain pour la couleur I_ROUGE={0} AND I_VERT={1} AND I_BLEU={2} ", 
                    pixelColor.R, pixelColor.G, pixelColor.B);
                throw new Exception(message);
            }
            
            //creation de la case
            Donnees.TAB_CASERow ligneCase=m_tableCase.AddTAB_CASERow(
                idCase,
                resModeleTerrain[0].ID_MODELE_TERRAIN, 
                x, y,-1,-1, -1,
                int.MaxValue);
            ligneCase.SetID_MODELE_TERRAIN_SI_OCCUPENull();
            ligneCase.SetID_PROPRIETAIRENull();
            ligneCase.SetID_NOUVEAU_PROPRIETAIRENull();
            //ligneCase.SetID_NOMNull();
            return true;
        }

        /// <summary>
        /// Ajoute les voisins de la case dans la table TAB_VOISINS_CASE en tenant compte de la distance en ligne ou en diagonale
        /// </summary>
        /// <param name="ligneCase">case source</param>
        /* recherche en table inneficace
        private void CreationsVoisins(DataSetCoutDonnees.TAB_CASERow ligneCase)
        {
            Collection<int> x_voisins;
            Collection<int> y_voisins;
            int i;

            m_tableCarte.TrouveVoisins(ligneCase, out x_voisins, out y_voisins);
            for (i = 0; i < x_voisins.Count; i++)
            {
                DataSetCoutDonnees.TAB_CASERow ligneVoisin = m_tableCase.TrouveCase(x_voisins[i], y_voisins[i]);
                if (x_voisins[i] == ligneCase.I_X || y_voisins[i] == ligneCase.I_Y)
                {
                    m_tableVoisins.AddTAB_VOISINS_CASERow(ligneCase.ID_CASE, ligneVoisin.ID_CASE,10);
                }
                else
                {
                    m_tableVoisins.AddTAB_VOISINS_CASERow(ligneCase.ID_CASE, ligneVoisin.ID_CASE, 14);
                }
            }
            
        }
        */

        /// <summary>
        /// D�termine le type de case qui sera utilis�e si la case est occup�e par une autre unit�
        /// L'id�e c'est que pour une route, une seule unit� peut la franchir mais pour simplifier l'algorithme de parcours
        /// des unit�s peuvent s'en "servir" (aller dessus) mais sans avoir alors les bonus de la route
        /// La case de remplacement est determin�e en prenant le type de case voisines majoritaires
        /// </summary>
        /// <param name="ligneCase">case source</param>
        private void CreationCasesDeRemplacement(Donnees.TAB_CASERow ligneCase)
        {
            int                     valeur,maximum;
            Int32                   modeleCible;

               SortedList<Int32, int> liste = new SortedList<Int32, int>();

               //analyse de toutes les cases voisines pour trouver le mod�le de terrain appropri�
               foreach (Donnees.TAB_CASERow ligneVoisin in tableCase.CasesVoisines(ligneCase))
               {
                   //si la case voisine est elle-m�me inutilisable si occup�e, on n'en tient pas compte
                   Donnees.TAB_MODELE_TERRAINRow ligneModeleTerrainVoisin = Donnees.m_donnees.TAB_MODELE_TERRAIN.FindByID_MODELE_TERRAIN(ligneVoisin.ID_MODELE_TERRAIN);
                   if (null == ligneModeleTerrainVoisin)
                   {
                       string message = string.Format("CreationCasesDeRemplacement ne trouve pas de mod�le de terrain pour la case voisine id={0} x={1} et y={2}",
                           ligneVoisin.ID_MODELE_TERRAIN, ligneVoisin.I_X, ligneVoisin.I_Y);
                       throw new Exception(message);
                   }
                   if (false == ligneModeleTerrainVoisin.B_ANNULEE_SI_OCCUPEE)
                   {
                       if (liste.TryGetValue(ligneVoisin.ID_MODELE_TERRAIN, out valeur))
                       {
                           liste[ligneVoisin.ID_MODELE_TERRAIN] = ++valeur;
                       }
                       else
                       {
                           liste.Add(ligneVoisin.ID_MODELE_TERRAIN,1);
                       }                       
                   }
               }

               //maintenant on affecte le mod�le le plus souvent rencontr�
               if (0 == liste.Count)
               {
                   //le terrain n'est entour� que par des terrains eux aussi modifiables, tant pis, on garde le m�me (cela n'arrivera pas souvent !)
                   ligneCase.ID_MODELE_TERRAIN_SI_OCCUPE = ligneCase.ID_MODELE_TERRAIN;
               }
               else
               {
                   maximum = 0;
                   modeleCible = -1;
                   foreach (Int32 clef in liste.Keys)
                   {
                       liste.TryGetValue(clef, out valeur);
                       if (valeur > maximum)
                       {
                           maximum = valeur;
                           modeleCible = clef;
                       }
                   }
                   ligneCase.ID_MODELE_TERRAIN_SI_OCCUPE = modeleCible;
               }
        }

        /// <summary>
        /// D�termine le type de case qui sera utilis�e si la case est occup�e par une autre unit�
        /// L'id�e c'est que pour une route, une seule unit� peut la franchir mais pour simplifier l'algorithme de parcours
        /// des unit�s peuvent s'en "servir" (aller dessus) mais sans avoir alors les bonus de la route
        /// La case de remplacement est determin�e en prenant le type de case voisines majoritaires
        /// </summary>
        /// <param name="ligneCase">case source</param>
        private void CreationCasesDeRemplacementOld(Donnees.TAB_CASERow ligneCase)
        {
            int valeur, maximum;
            Int32 modeleCible;

            /* inutile, tri des cases null fait avant
            if (!ligneCase.IsID_MODELE_TERRAIN_SI_OCCUPENull())
            {
                Debug.Write("N");
                return; //traitement d�j� effectu�
            }
             * */

            // Donnees.TAB_MODELE_TERRAINRow[] resModeleTerrain = (Donnees.TAB_MODELE_TERRAINRow[])Donnees.m_donnees.TAB_MODELE_TERRAIN.Select("ID_MODELE_TERRAIN=" + ligneCase.ID_MODELE_TERRAIN);
            // if (0==resModeleTerrain.Length)
            // {
            //     string message = string.Format("CreationCasesDeRemplacement ne trouve pas de mod�le de terrain pour la case id={0} x={1} et y={2}",
            //         ligneCase.ID_MODELE_TERRAIN, ligneCase.I_X, ligneCase.I_Y);
            //     throw new Exception(message);
            //}

            //if (resModeleTerrain[0].B_ANNULEE_SI_OCCUPEE)
            //{
            //    ligneCase.SetID_MODELE_TERRAIN_SI_OCCUPENull();
            //    Debug.Write("B");
            //}
            //else
            //{
            SortedList<Int32, int> liste = new SortedList<Int32, int>();

            //analyse de toutes les cases voisines pour trouver le mod�le de terrain appropri�
            foreach (Donnees.TAB_CASERow ligneVoisin in tableCase.CasesVoisines(ligneCase))
            {
                //si la case voisine est elle-m�me inutilisable si occup�e, on n'en tient pas compte
                Donnees.TAB_MODELE_TERRAINRow[] resModeleTerrainVoisin = (Donnees.TAB_MODELE_TERRAINRow[])Donnees.m_donnees.TAB_MODELE_TERRAIN.Select("ID_MODELE_TERRAIN=" + ligneVoisin.ID_MODELE_TERRAIN);
                if (0 == resModeleTerrainVoisin.Length)
                {
                    string message = string.Format("CreationCasesDeRemplacement ne trouve pas de mod�le de terrain pour la case voisine id={0} x={1} et y={2}",
                        ligneVoisin.ID_MODELE_TERRAIN, ligneVoisin.I_X, ligneVoisin.I_Y);
                    throw new Exception(message);
                }
                if (false == resModeleTerrainVoisin[0].B_ANNULEE_SI_OCCUPEE)
                {
                    if (liste.TryGetValue(ligneVoisin.ID_MODELE_TERRAIN, out valeur))
                    {
                        liste[ligneVoisin.ID_MODELE_TERRAIN] = ++valeur;
                    }
                    else
                    {
                        liste.Add(ligneVoisin.ID_MODELE_TERRAIN, 1);
                    }
                }
            }
            //maintenant on affecte le mod�le le plus souvent rencontr�
            if (0 == liste.Count)
            {
                //le terrain n'est entour� que par des terrains eux aussi modifiables, tant pis, on garde le m�me (cela n'arrivera pas souvent !)
                ligneCase.ID_MODELE_TERRAIN_SI_OCCUPE = ligneCase.ID_MODELE_TERRAIN;
                Debug.Write("S");
            }
            else
            {
                Debug.Write("D");
                maximum = 0;
                modeleCible = -1;
                foreach (Int32 clef in liste.Keys)
                {
                    liste.TryGetValue(clef, out valeur);
                    if (valeur > maximum)
                    {
                        maximum = valeur;
                        modeleCible = clef;
                    }
                }
                ligneCase.ID_MODELE_TERRAIN_SI_OCCUPE = modeleCible;
            }
            //}
        }

        private void FormCarte_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (null != m_imageCarte)
            {
                m_imageCarte.Dispose();
            }
        }

        private void buttonNettoyage_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show("Voulez vous vraiment nettoyer toutes les cases de remplacement ?", "Nettoyage", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                Cursor oldcurseur = this.Cursor;
                Cursor = Cursors.WaitCursor;
                foreach (Donnees.TAB_CASERow ligneCase in m_tableCase)
                {
                    ligneCase.SetID_MODELE_TERRAIN_SI_OCCUPENull();
                }
                Cursor = oldcurseur;
            }
        }

        private bool MiseAJourCase(int x, int y, Color pixelColor)
        {
            int j, k;
            string requete,message;
            Donnees.TAB_POINTRow[] resPoint;
            Donnees.TAB_GRAPHISMERow[] resGraphique;
            Donnees.TAB_MODELE_TERRAINRow[] resModeleTerrain=null;
            bool btrouve;

            //recherche du mod�le de terrain associ� � cette couleur
            btrouve = false;
            requete = String.Format("I_ROUGE={0} AND I_VERT={1} AND I_BLEU={2}", pixelColor.R, pixelColor.G, pixelColor.B);
            resPoint = (Donnees.TAB_POINTRow[])Donnees.m_donnees.TAB_POINT.Select(requete);
            j = 0;
            while (!btrouve && j < resPoint.Length)
            {
                //on cherche le graphique associ� au point de couleur
                resGraphique = (Donnees.TAB_GRAPHISMERow[])Donnees.m_donnees.TAB_GRAPHISME.Select("ID_POINT=" + resPoint[j].ID_POINT);
                k = 0;
                while (!btrouve && k < resGraphique.Length)
                {
                    resModeleTerrain = (Donnees.TAB_MODELE_TERRAINRow[])Donnees.m_donnees.TAB_MODELE_TERRAIN.Select("ID_GRAPHIQUE=" + resGraphique[k].ID_GRAPHIQUE);
                    if (resModeleTerrain.Length > 0)
                    {
                        btrouve = true;
                    }
                    else
                    {
                        message = String.Format("Impossible de trouver le mod�le de terrain pour le graphique ID_GRAPHIQUE : {0}", resGraphique[k].ID_GRAPHIQUE);
                        throw new Exception(message);
                    }
                    k++;
                }
                j++;
            }

            if (!btrouve)
            {
                message = String.Format("Impossible de trouver le mod�le de terrain pour la couleur I_ROUGE={0} AND I_VERT={1} AND I_BLEU={2} ", 
                    pixelColor.R, pixelColor.G, pixelColor.B);
                throw new Exception(message);
            }
            
            //Mise a jour de la case
            Donnees.TAB_CASERow ligneCase=m_tableCase.FindByXY(x,y);
            ligneCase.ID_MODELE_TERRAIN=resModeleTerrain[0].ID_MODELE_TERRAIN;
            return true;
        }

        private void buttonMiseAJourModelesTerrain_Click(object sender, EventArgs e)
        {
            m_traitement = 3;
            m_sous_traitement = 0;

            m_oldcurseur = Cursor;
            Cursor = Cursors.WaitCursor;
            timerTraitement.Enabled = true;
        }
    }
}