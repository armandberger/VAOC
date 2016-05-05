using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using Microsoft.Win32;
using WaocLib;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace vaoc
{
    public partial class FormFondDeCarte : Form
    {
        #region données
        private int         m_traitement;//traitement principal
        private int         m_sous_traitement;//sous tâche d'un traitement
        private Cursor      m_oldcurseur;
        private SortedList  m_tableCouleur;
        private Bitmap      m_imageCarte;
        private Donnees.TAB_GRAPHISMEDataTable           m_tableGraphisme;//table des graphismes
        private Donnees.TAB_POINTDataTable               m_tablePoint;//table des points liés aux graphismes
        private Donnees.TAB_METEODataTable               m_tableMeteo;//table des points liés aux graphismes
        private Donnees.TAB_MODELE_MOUVEMENTDataTable    m_tableModelesMouvements;
        private Donnees.TAB_MODELE_TERRAINDataTable      m_tableModelesTerrains;
        private ListDictionary                          m_identifiantsModelesTerrains; //stocke les valeurs avant, après des identifiants
        //private DataSetCoutDonnees.TAB_MOUVEMENT_COUTDataTable      m_tableMouvementsCout;

        #endregion

        #region propriétés
        /// <summary>
        /// hauteur de le carte en pixels
        /// </summary>
        public int hauteur
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

        /// <summary>
        /// largeur de le carte en pixels
        /// </summary>
        public int largeur
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

        /// <summary>
        /// cout de base d'une case, c'est à dire 
        /// la valeur pour laquelle une case équivaut à la valeur '1'
        /// </summary>
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

        public string nomCarteTopographique
        {
            get
            {
                return labelNom.Text;
            }
            set
            {
                labelNom.Text = value;
            }
        }

        public DataGridView grilleDonnees
        {
            get
            {
                return dataGridViewCouleurs;
            }
        }

        public ListDictionary tableIdentifiansModelesTerrains
        {
            get
            {
                return m_identifiantsModelesTerrains;
            }
        }

        public Donnees.TAB_MODELE_TERRAINDataTable tableModelesTerrains
        {
            get{
                m_tableModelesTerrains.Clear();
                foreach (DataGridViewRow ligne in dataGridViewModelesTerrain.Rows)
                {
                    Donnees.TAB_MODELE_TERRAINRow ligneTerrain = m_tableModelesTerrains.NewTAB_MODELE_TERRAINRow();
                    ligneTerrain.ID_MODELE_TERRAIN = Convert.ToInt32(ligne.Cells["ID_MODELE_TERRAIN"].Value);
                    ligneTerrain.S_NOM = Convert.ToString(ligne.Cells["NOM_TERRAIN"].Value);
                    ligneTerrain.B_ANNULEE_SI_OCCUPEE = Convert.ToBoolean(ligne.Cells["B_ANNULEE_SI_OCCUPEE"].Value);
                    ligneTerrain.I_MODIFICATEUR_DEFENSE = Convert.ToInt32(ligne.Cells["I_MODIFICATEUR_DEFENSE"].Value);
                    ligneTerrain.ID_GRAPHIQUE = Convert.ToInt32(ligne.Cells["ID_GRAPHIQUE"].Value);
                    ligneTerrain.B_PONT = Convert.ToBoolean(ligne.Cells["Pont"].Value);
                    ligneTerrain.B_PONTON = Convert.ToBoolean(ligne.Cells["Ponton"].Value);
                    ligneTerrain.B_DETRUIT = Convert.ToBoolean(ligne.Cells["Detruit"].Value);
                    ligneTerrain.B_CIRCUIT_ROUTIER = Convert.ToBoolean(ligne.Cells["Routier"].Value);
                    ligneTerrain.ID_MODELE_NOUVEAU_TERRAIN = Convert.ToInt32(ligne.Cells["ID_NOUVEAU_TERRAIN"].Value);
                    ligneTerrain.B_OBSTACLE_DEFENSIF = Convert.ToBoolean(ligne.Cells["B_OBSTACLE_DEFENSIF"].Value);
                    ligneTerrain.B_ANNULEE_EN_COMBAT = Convert.ToBoolean(ligne.Cells["B_ANNULEE_EN_COMBAT"].Value);

                    m_tableModelesTerrains.AddTAB_MODELE_TERRAINRow(ligneTerrain);
                }
                //return (DataSetCoutDonnees.TAB_MODELE_TERRAINDataTable)dataGridViewModelesTerrain.DataSource;
                return m_tableModelesTerrains;
            }
            set
            {
                m_identifiantsModelesTerrains = new ListDictionary();
                m_tableModelesTerrains = (Donnees.TAB_MODELE_TERRAINDataTable)value.Copy();
                foreach (Donnees.TAB_MODELE_TERRAINRow ligneTerrain in m_tableModelesTerrains)
                {
                    m_identifiantsModelesTerrains.Add(ligneTerrain.ID_MODELE_TERRAIN, ligneTerrain.ID_MODELE_TERRAIN);

                    Donnees.TAB_GRAPHISMERow[] resGraphique = (Donnees.TAB_GRAPHISMERow[])m_tableGraphisme.Select("ID_GRAPHIQUE=" + ligneTerrain.ID_GRAPHIQUE);
                    if (resGraphique.Length > 0)
                    {
                        Donnees.TAB_POINTRow[] resPoint = (Donnees.TAB_POINTRow[])m_tablePoint.Select("ID_POINT=" + resGraphique[0].ID_POINT);
                        if (resPoint.Length > 0)
                        {
                            Color pixelColor = Color.FromArgb(resPoint[0].I_ROUGE, resPoint[0].I_VERT, resPoint[0].I_BLEU);
                            DataGridViewRow ligne = new DataGridViewRow();
                            ligne.CreateCells(dataGridViewModelesTerrain);
                            ligne.Cells[0].Value = ligneTerrain.ID_MODELE_TERRAIN;//"ID_MODELE_TERRAIN"
                            ligne.Cells[1].Value = ligneTerrain.S_NOM;//"NOM_TERRAIN"
                            ligne.Cells[2].Value = ligneTerrain.B_ANNULEE_SI_OCCUPEE;
                            ligne.Cells[3].Value = ligneTerrain.I_MODIFICATEUR_DEFENSE;//"I_MODIFICATEUR_DEFENSE"
                            ligne.Cells[4].Style.BackColor = pixelColor;//"TERRAIN_COULEUR"
                            ligne.Cells[5].Value = string.Format("{0},{1},{2}", pixelColor.R, pixelColor.G, pixelColor.B);//"RGB"

                            Donnees.TAB_CASERow[] resCase = (Donnees.TAB_CASERow[])Donnees.m_donnees.TAB_CASE.Select("ID_MODELE_TERRAIN=" + ligneTerrain.ID_MODELE_TERRAIN);
                            if (null == resCase)
                            {
                                ligne.Cells[6].Value = "Générer la carte";
                            }
                            else
                            {
                                ligne.Cells[6].Value = resCase.Length.ToString();//NB_CASES
                            }
                            ligne.Cells[7].Value = ligneTerrain.ID_GRAPHIQUE;//"ID_GRAPHIQUE"
                            ligne.Cells[8].Value = ligneTerrain.B_PONT;
                            ligne.Cells[9].Value = ligneTerrain.B_PONTON;
                            ligne.Cells[10].Value = ligneTerrain.B_DETRUIT;
                            ligne.Cells[11].Value = ligneTerrain.B_CIRCUIT_ROUTIER;//"Routier"
                            ligne.Cells[12].Value = ligneTerrain.ID_MODELE_NOUVEAU_TERRAIN;//"ID_MODELE_NOUVEAU_TERRAIN"
                            ligne.Cells[13].Value = ligneTerrain.B_OBSTACLE_DEFENSIF;
                            ligne.Cells[14].Value = ligneTerrain.B_ANNULEE_EN_COMBAT;
                            dataGridViewModelesTerrain.Rows.Add(ligne);
                        }
                    }
                }
                //dataGridViewModelesTerrain.Rows..DataSource = m_tableModelesTerrains; marche pas, duplique la structure
            }
        }

        public Donnees.TAB_GRAPHISMEDataTable tableGraphisme
        {
            get
            {
                return (Donnees.TAB_GRAPHISMEDataTable)m_tableGraphisme;
            }
            set
            {
                m_tableGraphisme = (Donnees.TAB_GRAPHISMEDataTable)value.Copy();
            }
        }

        public Donnees.TAB_POINTDataTable tablePoint
        {
            get
            {
                return (Donnees.TAB_POINTDataTable)m_tablePoint;
            }
            set
            {
                m_tablePoint = (Donnees.TAB_POINTDataTable)value.Copy();
            }
        }

        public Donnees.TAB_METEODataTable tableMeteo
        {
            set
            {
                m_tableMeteo = (Donnees.TAB_METEODataTable)value.Copy();
            }
        }

        public Donnees.TAB_MODELE_MOUVEMENTDataTable tableModelesMouvements
        {
            set
            {
                m_tableModelesMouvements = (Donnees.TAB_MODELE_MOUVEMENTDataTable)value.Copy();
            }
        }

        public Donnees.TAB_MOUVEMENT_COUTDataTable tableMouvementCout
        {
            get
            {
                Donnees.TAB_MOUVEMENT_COUTDataTable table = new Donnees.TAB_MOUVEMENT_COUTDataTable();
                foreach (DataGridViewRow ligne in dataGridViewCouleurs.Rows)
                {
                    table.AddTAB_MOUVEMENT_COUTRow(
                        Convert.ToInt32(ligne.Cells["ID_MODELE_MOUVEMENT"].Value),
                        Convert.ToInt32(ligne.Cells["ID_MODELE_DU_TERRAIN"].Value),
                        Convert.ToInt32(ligne.Cells["ID_METEO"].Value),
                        Convert.ToInt32(ligne.Cells["Cout"].Value));
                }
                return table;
            }
            set
            {
                //on vérifie s'il n'y aurait pas de nouveaux modèles à ajouter
                foreach (Donnees.TAB_MODELE_MOUVEMENTRow ligneModeleMouvement in m_tableModelesMouvements)
                {
                    foreach (Donnees.TAB_METEORow ligneMeteo in m_tableMeteo)
                    {
                        foreach (Donnees.TAB_MODELE_TERRAINRow ligneModeleTerrain in m_tableModelesTerrains)
                        {
                            bool bExiste = false;
                            int i = 0;
                            while(i<value.Count)
                            {
                                Donnees.TAB_MOUVEMENT_COUTRow ligneCout = value[i];
                                if (ligneCout.ID_MODELE_MOUVEMENT == ligneModeleMouvement.ID_MODELE_MOUVEMENT
                                    && ligneCout.ID_METEO == ligneMeteo.ID_METEO
                                    && ligneCout.ID_MODELE_TERRAIN == ligneModeleTerrain.ID_MODELE_TERRAIN)
                                {
                                    bExiste = true;
                                    break;
                                }
                                i++;
                            }
                            if (!bExiste)
                            {
                                //ajout des lignes de cout
                                value.AddTAB_MOUVEMENT_COUTRow(ligneModeleMouvement.ID_MODELE_MOUVEMENT,
                                    ligneModeleTerrain.ID_MODELE_TERRAIN,
                                    ligneMeteo.ID_METEO,
                                    -1);
                            }
                        }
                    }
                }

                //m_tableMouvementsCout = (DataSetCoutDonnees.TAB_MOUVEMENT_COUTDataTable)value.Copy();
                dataGridViewCouleurs.Rows.Clear();
                foreach (Donnees.TAB_MOUVEMENT_COUTRow ligneCout in value)
                {
                    Donnees.TAB_MODELE_MOUVEMENTRow resMouvement = m_tableModelesMouvements.FindByID_MODELE_MOUVEMENT(ligneCout.ID_MODELE_MOUVEMENT);
                    Donnees.TAB_METEORow[] resMeteo = (Donnees.TAB_METEORow[])m_tableMeteo.Select("ID_METEO=" + ligneCout.ID_METEO);
                    Donnees.TAB_MODELE_TERRAINRow[] resTerrain = (Donnees.TAB_MODELE_TERRAINRow[])m_tableModelesTerrains.Select("ID_MODELE_TERRAIN=" + ligneCout.ID_MODELE_TERRAIN);
                    if (null != resMouvement && resMeteo.Length > 0 && resTerrain.Length > 0)
                    {
                        Donnees.TAB_GRAPHISMERow resGraphique = m_tableGraphisme.FindByID_GRAPHIQUE(resTerrain[0].ID_GRAPHIQUE);
                        if (null != resGraphique)
                        {
                            Donnees.TAB_POINTRow[] resPoint = (Donnees.TAB_POINTRow[])m_tablePoint.Select("ID_POINT=" + resGraphique.ID_POINT);
                            if (resPoint.Length > 0)
                            {
                                Color pixelColor = Color.FromArgb(resPoint[0].I_ROUGE, resPoint[0].I_VERT, resPoint[0].I_BLEU);
                                DataGridViewRow ligneGrid = dataGridViewCouleurs.Rows[dataGridViewCouleurs.Rows.Add()];
                                ligneGrid.Cells["ID_MODELE_MOUVEMENT"].Value = ligneCout.ID_MODELE_MOUVEMENT;
                                ligneGrid.Cells["ID_MODELE_DU_TERRAIN"].Value = ligneCout.ID_MODELE_TERRAIN;
                                ligneGrid.Cells["ID_METEO"].Value = ligneCout.ID_METEO;
                                ligneGrid.Cells["Meteo"].Value = resMeteo[0].S_NOM;
                                ligneGrid.Cells["Modele"].Value = resMouvement.S_NOM;
                                ligneGrid.Cells["NOM_DU_TERRAIN"].Value = resTerrain[0].S_NOM;
                                ligneGrid.Cells["Couleur"].Value = String.Format("R:{0} G:{1} B{2}", pixelColor.R, pixelColor.G, pixelColor.B);
                                ligneGrid.Cells["Cout"].Value = ligneCout.I_COUT;
                                ligneGrid.Cells["Vision"].Style.BackColor = pixelColor;
                            }
                        }
                    }
                    else
                    {
                        Debug.WriteLine("Erreur dans le chargement de tableMouvementCout, modele, mét");
                    }
                }
            }
        }

        #endregion
 
        public FormFondDeCarte()
        {
            InitializeComponent();
            m_tableCouleur = new SortedList();
        }

        private void buttonImageCarte_Click(object sender, EventArgs e)
        {
            m_oldcurseur = Cursor;
            
            if (DialogResult.OK == this.openFileDialog.ShowDialog(this))
            {
                try
                {
                    Cursor = Cursors.WaitCursor;
                    m_imageCarte = (Bitmap)Image.FromFile(openFileDialog.FileName);
                    //labelNom.Text = openFileDialog.FileName.Substring(openFileDialog.FileName.LastIndexOf('\\') + 1);
                    labelNom.Text = openFileDialog.FileName;

                    //pour cause de lock, ce qui suit est fait seulement sur OK
                    /*
                    string repertoire = WaocLib.Constantes.repertoireDonnees + labelNom.Text; //Application.ExecutablePath.Substring(0, Application.ExecutablePath.LastIndexOf('\\') + 1);
                    //destruction d'un eventuel même fichier avec le même nom
                    File.Delete(repertoire);
                    //on recopie le fichier image vers le repertoire applicatif des cartes
                    m_imageCarte.Save(repertoire);
                     * */
                }
                catch (Exception ex)
                {
                    Cursor = m_oldcurseur;
                    MessageBox.Show("Erreur au chargement/sauvegarde de l'image : " + ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                try
                {
                    labelLargeur.Text = Convert.ToString(m_imageCarte.Width);
                    labelHauteur.Text = Convert.ToString(m_imageCarte.Height);
                }
                catch (Exception ex)
                {
                    Cursor = m_oldcurseur;
                    MessageBox.Show("Erreur à l'affectation des tailles de l'image : " + ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                Cursor = m_oldcurseur;
            }
        }

        private void timerTraitement_Tick(object sender, EventArgs e)
        {
            if (!Traitement())
            {
                progressBar.Value = progressBar.Maximum;
                timerTraitement.Enabled = false;
                Cursor = m_oldcurseur;
                MessageBox.Show("Traitement terminé", "Traitement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                if (null != m_imageCarte)
                {
                    m_imageCarte.Dispose();
                }
            }
        }

        /// <summary>
        /// Traitement en cours jusqu'à l'étape suivante
        /// </summary>
        /// <returns>false si un problème a eut lieu ou si le traitement est terminé</returns>
        private bool Traitement()
        {
            int y;

            switch (m_traitement)
            {
            case 0:
                if (m_sous_traitement < m_imageCarte.Width)
                {
                    if (0==m_sous_traitement)
                    {
                        labelTraitement.Text = "analyse des couleurs";
                        progressBar.Maximum = m_imageCarte.Width;
                        m_tableCouleur.Clear();
                    }
                    //on analyse la ligne de pixels suivants
                    for (y = 0; y < m_imageCarte.Height; y++)
                    {
                        Color pixelColor = m_imageCarte.GetPixel(m_sous_traitement, y);
                        if (!m_tableCouleur.Contains(pixelColor.Name))
                        {
                            m_tableCouleur.Add(pixelColor.Name, pixelColor);
                        }
                    }
                    progressBar.Value = m_sous_traitement;
                    Invalidate();
                    m_sous_traitement++;
                }
                else{
                    m_sous_traitement=0;
                    m_traitement++;
                }
                break;
            case 1:
                labelTraitement.Text = "affectation de la table";
                AffectationsDesTables();
                m_traitement++;
                break;
            default:
                labelTraitement.Text = "terminé";
                return false;//traitement terminé
            }
            return true;
        }

        private bool AffectationsDesTables()
        {
            int i, j,k;
            bool btrouve;
            Donnees.TAB_POINTRow[] resPoint;
            Donnees.TAB_GRAPHISMERow[] resGraphique;
            //DataSetCoutDonnees.TAB_MODELE_TERRAINRow[] resModeleTerrain;
            Donnees.TAB_POINTRow lignePoint;
            Donnees.TAB_GRAPHISMERow ligneGraphisme;
            Donnees.TAB_MODELE_TERRAINRow ligneTerrain;
            Color pixelColor=Color.Empty;
            String requete;
            DataGridViewRow ligne;

            #region suppression dans la table de toutes les couleurs qui ne se trouve plus dans le nouveau fichier
            try
            {
                i = 0;
                while (i < dataGridViewModelesTerrain.Rows.Count)
                {
                    //recherche de la couleur du graphique utilisé
                    btrouve = false;
                    resGraphique = (Donnees.TAB_GRAPHISMERow[])m_tableGraphisme.Select("ID_GRAPHIQUE=" + dataGridViewModelesTerrain.Rows[i].Cells[7].Value);
                    if (resGraphique.Length > 0)
                    {
                        lignePoint = m_tablePoint.FindByID_POINT(resGraphique[0].ID_POINT);
                        Debug.Write(string.Format("Couleur R:{0}, G:{1}, B:{2} ", lignePoint.I_ROUGE, lignePoint.I_VERT, lignePoint.I_BLEU));
                        if (null != lignePoint)
                        {
                            pixelColor = Color.FromArgb(lignePoint.I_ROUGE, lignePoint.I_VERT, lignePoint.I_BLEU);
                            if (m_tableCouleur.IndexOfKey(pixelColor.Name) >= 0)
                            {
                                btrouve = true;
                            }
                        }
                    }

                    if (btrouve)
                    {
                        //la couleur existe toujours, on la retire de la liste des couleurs à ajouter
                        Debug.WriteLine("la couleur existe toujours " + pixelColor.Name);
                        m_tableCouleur.Remove(pixelColor.Name);
                        i++;
                    }
                    else
                    {
                        i++;
                        //suppression de la couleur
                        /* Il ne faut pas le faire, car certaines couleurs modifiables ne sont pas forcément présent sur la carte, les ponts détruits en particulier
                        Debug.WriteLine("suppression de la couleur existante " + dataGridViewModelesTerrain.Rows[i].Cells[5].Value);
                        //suppression dans dataGridViewCouleurs
                        k = 0;
                        while (k<dataGridViewCouleurs.Rows.Count)
                        {
                            if (Convert.ToInt32(dataGridViewCouleurs.Rows[k].Cells["ID_MODELE_DU_TERRAIN"].Value) == m_tableModelesTerrains[i].ID_MODELE_TERRAIN)
                            {
                                dataGridViewCouleurs.Rows.RemoveAt(k);
                            }
                            else{
                                k++;
                            }
                        }
                        //suppression dans m_tablePoint, m_tableGraphisme, m_tableModelesTerrains
                        ligneTerrain = m_tableModelesTerrains.FindByID_MODELE_TERRAIN(Convert.ToInt32(dataGridViewModelesTerrain.Rows[i].Cells["ID_MODELE_TERRAIN"].Value));
                        ligneGraphisme = m_tableGraphisme.FindByID_GRAPHIQUE(ligneTerrain.ID_GRAPHIQUE);
                        lignePoint = m_tablePoint.FindByID_POINT(ligneGraphisme.ID_POINT);
                        m_tablePoint.RemoveTAB_POINTRow(lignePoint);
                        m_tableGraphisme.RemoveTAB_GRAPHISMERow(ligneGraphisme);
                        m_tableModelesTerrains.RemoveTAB_MODELE_TERRAINRow(ligneTerrain);

                        //suppression dans dataGridViewModelesTerrain
                        dataGridViewModelesTerrain.Rows.RemoveAt(i);
                         * */
                    }
                }
            }
            catch (Exception ex)
            {
                this.timerTraitement.Enabled = false;
                Cursor = m_oldcurseur;
                MessageBox.Show("AffectationsDesTables - I : " + ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            #endregion

            #region ajout des nouvelles couleurs
            try
            {
                for (i = 0; i < m_tableCouleur.Count; i++)
                {
                    pixelColor = (Color)m_tableCouleur.GetByIndex(i);

                    Debug.WriteLine("ajout de la couleur " + pixelColor.Name);
                    btrouve = false;
                    //on recherche le point équivalent
                    requete = String.Format("I_ROUGE={0} AND I_VERT={1} AND I_BLEU={2}", pixelColor.R, pixelColor.G, pixelColor.B);
                    resPoint = (Donnees.TAB_POINTRow[])m_tablePoint.Select(requete);
                    j = 0;
                    while (!btrouve && j < resPoint.Length)
                    {
                        //on vérifie que le point trouvé fait bien partie des modèles
                        resGraphique = (Donnees.TAB_GRAPHISMERow[])m_tableGraphisme.Select("ID_POINT=" + resPoint[j].ID_POINT);
                        if (resGraphique.Length > 0)
                        {
                            k = 0;
                            while (!btrouve && k < dataGridViewModelesTerrain.Rows.Count)
                            {
                                //resModeleTerrain = (DataSetCoutDonnees.TAB_MODELE_TERRAINRow[])dataGridViewModelesTerrain.BindingContext.d.C.Select("ID_GRAPHIQUE=" + resGraphique[0].ID_GRAPHIQUE);
                                //resModeleTerrain = (DataSetCoutDonnees.TAB_MODELE_TERRAINRow[])(((DataSetCoutDonnees.TAB_MODELE_TERRAINDataTable)dataGridViewModelesTerrain.Select("ID_GRAPHIQUE=" + resGraphique[0].ID_GRAPHIQUE));

                                if (dataGridViewModelesTerrain.Rows[k].Cells["TERRAIN_COULEUR"].Style.BackColor == pixelColor)
                                {
                                    btrouve = true;
                                }
                                k++;
                            }
                        }
                        j++;
                    }
                    if (!btrouve)
                    {
                        //création du nouveau point si necessaire
                        requete = String.Format("I_ROUGE={0} AND I_VERT={1} AND I_BLEU={2}", pixelColor.R, pixelColor.G, pixelColor.B);
                        resPoint = (Donnees.TAB_POINTRow[])m_tablePoint.Select(requete);
                        if (resPoint.Length > 0)
                        {
                            lignePoint = resPoint[0];
                        }
                        else
                        {
                            lignePoint = m_tablePoint.AddTAB_POINTRow(255, pixelColor.R, pixelColor.G, pixelColor.B);
                        }
                        ligneGraphisme = m_tableGraphisme.AddTAB_GRAPHISMERow(1, "Point Carte", Constantes.CST_IDNULL, Constantes.CST_IDNULL, lignePoint.ID_POINT);

                        //recherche de l'id modele suivant
                        /*
                        string tri = "ID_MODELE_TERRAIN DESC";
                        int id_modeleTerrain = 0;
                        DataSetCoutDonnees.TAB_MODELE_TERRAINRow[] resModele = (DataSetCoutDonnees.TAB_MODELE_TERRAINRow[])DataSetCoutDonnees.m_donnees.TAB_MODELE_TERRAIN.Select(string.Empty, tri);
                        if (resModele.Length >0)
                        {
                            id_modeleTerrain = resModele[0].ID_MODELE_TERRAIN+1;
                        }
                         * */
                        int id_modeleTerrain = (int)Donnees.m_donnees.TAB_MODELE_TERRAIN.Compute("Max(ID_MODELE_TERRAIN)", null) +1;
                        ligneTerrain = m_tableModelesTerrains.AddTAB_MODELE_TERRAINRow( id_modeleTerrain,
                                                                                        "", 
                                                                                        ligneGraphisme.ID_GRAPHIQUE,
                                                                                        Constantes.CST_IDNULL,
                                                                                        0,//modificateur de défense
                                                                                        false,//B_ANNULEE_SI_OCCUPEE
                                                                                        false,//B_CIRCUIT_ROUTIER
                                                                                        false,//B_OBSTACLE_DEFENSIF
                                                                                        false,
                                                                                        false, //B_PONT
                                                                                        false, //B_PONTON
                                                                                        false //B_DETRUIT
                                                                                        );//B_ANNULEE_EN_COMBAT
                        m_identifiantsModelesTerrains.Add(ligneTerrain.ID_MODELE_TERRAIN, ligneTerrain.ID_MODELE_TERRAIN);
                        //création de la nouvelle ligne dans la table des modeles de terrain
                        ligne = new DataGridViewRow();
                        ligne.CreateCells(dataGridViewModelesTerrain);
                        MiseAjourLigneModele(ligne, ligneTerrain, pixelColor);
                        /*
                        ligne.Cells[0].Value = ligneTerrain.ID_MODELE_TERRAIN;//"ID_MODELE_TERRAIN"
                        ligne.Cells[1].Value = "";//"NOM_TERRAIN"
                        ligne.Cells[2].Value = false;//B_ANNULEE_SI_OCCUPEE
                        ligne.Cells[3].Value = "0";//I_MODIFICATEUR_DEFENSE
                        ligne.Cells[4].Style.BackColor = pixelColor;//"TERRAIN_COULEUR"
                        ligne.Cells[5].Value = string.Format("{0},{1},{2}", pixelColor.R, pixelColor.G, pixelColor.B);//"RGB"
                        Donnees.TAB_CASERow[] resCase = (Donnees.TAB_CASERow[])Donnees.m_donnees.TAB_CASE.Select("ID_MODELE_TERRAIN=" + ligneTerrain.ID_MODELE_TERRAIN);
                        if (null == resCase)
                        {
                            ligne.Cells[6].Value = "Générer la carte";
                        }
                        else
                        {
                            ligne.Cells[6].Value = resCase.Length.ToString();//NB_CASES
                        }
                        ligne.Cells[7].Value = ligneGraphisme.ID_GRAPHIQUE;//"ID_GRAPHIQUE"
                        ligne.Cells[8].Value = false;//B_MODIFIABLE
                        ligne.Cells[9].Value = false;//B_CIRCUIT_ROUTIER
                        ligne.Cells[10].Value = Constantes.CST_IDNULL;//ID_MODELE_NOUVEAU_TERRAIN
                        ligne.Cells[11].Value = false;//B_OBSTACLE_DEFENSIF
                        ligne.Cells[12].Value = false;//B_ANNULEE_EN_COMBAT
                        */
                        dataGridViewModelesTerrain.Rows.Add(ligne);

                        //((DataSetCoutDonnees.TAB_MODELE_TERRAINDataTable)dataGridViewModelesTerrain.DataSource).Rows.Add(ligne);
                        //DataSetCoutDonnees.TAB_MODELE_TERRAINRow ligneTerrain = ((DataSetCoutDonnees.TAB_MODELE_TERRAINDataTable)dataGridViewModelesTerrain.DataSource).NewTAB_MODELE_TERRAINRow();
                        //ligneTerrain.ID_GRAPHIQUE=ligneGraphisme.ID_GRAPHIQUE;
                        //ligneTerrain.S_NOM="";

                        //dataGridViewModelesTerrain.DataSource = m_tableModelesTerrains;

                        //création des lignes dans la table d'affectation des couts
                        foreach (Donnees.TAB_MODELE_MOUVEMENTRow ligneModele in m_tableModelesMouvements)
                        {
                            foreach (Donnees.TAB_METEORow ligneMeteo in m_tableMeteo)
                            {
                                //ajout de la nouvelle ligne de modèle, par couleur, il faut ajouter une ligne par modèle et par météo
                                ligne = new DataGridViewRow();
                                ligne.CreateCells(dataGridViewCouleurs);
                                MiseAjourLigneMeteo(ligne, ligneMeteo, ligneModele, ligneTerrain, pixelColor);
                                dataGridViewCouleurs.Rows.Add(ligne);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.timerTraitement.Enabled = false;
                Cursor = m_oldcurseur;
                MessageBox.Show("AffectationsDesTables - II : " + ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            #endregion
            return true;
        }

        private Donnees.TAB_GRAPHISMERow RechercheGraphisme(Color pixelColor)
        {
            string requete;
            Donnees.TAB_POINTRow[] resPoint;
            Donnees.TAB_GRAPHISMERow ligneGraphisme;
            Donnees.TAB_POINTRow lignePoint;

            //on recherche le point équivalent
            requete = String.Format("I_ROUGE={0} AND I_VERT={1} AND I_BLEU={2}", pixelColor.R, pixelColor.G, pixelColor.B);
            resPoint = (Donnees.TAB_POINTRow[])m_tablePoint.Select(requete);
            if (resPoint.Length > 0) 
            { 
                //recherche du graphisme equivalent
                requete = String.Format("ID_POINT={0}", resPoint[0].ID_POINT);
                ligneGraphisme = (Donnees.TAB_GRAPHISMERow) m_tableGraphisme.Select(requete)[0];                
            }
            else
            {
                //création du nouveau point
                lignePoint = m_tablePoint.AddTAB_POINTRow(255, pixelColor.R, pixelColor.G, pixelColor.B);
                ligneGraphisme = m_tableGraphisme.AddTAB_GRAPHISMERow(1, "Point Carte", -1, -1, lignePoint.ID_POINT);
                ligneGraphisme.SetID_BROSSENull();
                ligneGraphisme.SetID_STYLONull();
            }
            return ligneGraphisme;
        }

        private void dataGridViewModelesTerrain_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                //il faut modifier les noms dans la table des couts pour le nouveau nom donné
                foreach (DataGridViewRow ligne in dataGridViewCouleurs.Rows)
                {
                    if (e.RowIndex<m_tableModelesTerrains.Rows.Count && 
                        Convert.ToInt32(ligne.Cells["ID_MODELE_DU_TERRAIN"].Value) == m_tableModelesTerrains[e.RowIndex].ID_MODELE_TERRAIN)
                    {
                        ligne.Cells["NOM_DU_TERRAIN"].Value = dataGridViewModelesTerrain.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;// m_tableModelesTerrains[e.RowIndex].S_NOM;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("dataGridViewModelesTerrain_CellValueChanged : " + ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        /// <summary>
        /// On vérifie si les valeurs saisies permettent une bonne execution du programme
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridViewCouleurs_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            TestValidite();
        }

        private void textBoxCoutDeBase_TextChanged(object sender, EventArgs e)
        {
            if (coutDeBase < 10)
            {
                //il faut au moins que cout de base vaille 10 pour tenir des mouvements en diagonales
                MessageBox.Show("Cout de base DOIT être supérieur ou égal à 10", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            TestValidite();
        }

        private void TestValidite()
        {
            try
            {
                if (null != m_tableModelesMouvements && null != m_tableModelesMouvements && null != tableMouvementCout)
                {
                    decimal vitessemin = m_tableModelesMouvements.VitesseMinimale();
                    decimal vitessemax = m_tableModelesMouvements.VitesseMaximale();
                    int coutmin = tableMouvementCout.CoutMinimum();

                    if (vitessemin >= 0 && coutDeBase >= 0)
                    {
                        if (vitessemin * coutDeBase * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE < Donnees.m_donnees.TAB_JEU[0].I_NOMBRE_PHASES)
                        {
                            //il faut au moins que l'unité la plus lente progresse de 1 à chaque phase
                            labelValiditéBase.Text = string.Format("Il faut que vitesse min*cout de base d'une case* echelle>= nombre de phases. On a {0}*{1}*{2}={3} <{4}",
                                vitessemin,
                                coutDeBase,
                                Donnees.m_donnees.TAB_JEU[0].I_ECHELLE,
                                vitessemin * coutDeBase * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE,
                                Donnees.m_donnees.TAB_JEU[0].I_NOMBRE_PHASES);
                            labelValiditéBase.ForeColor = Color.Red;
                        }
                        else
                        {
                            labelValiditéBase.Text = string.Format("valeurs valides : vitesse min*cout de base d'une case* echelle>= nombre de phases. On a {0}*{1}*{2}={3} >={4}",
                                vitessemin,
                                coutDeBase,
                                Donnees.m_donnees.TAB_JEU[0].I_ECHELLE,
                                vitessemin * coutDeBase * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE,
                                Donnees.m_donnees.TAB_JEU[0].I_NOMBRE_PHASES);
                            labelValiditéBase.ForeColor = System.Drawing.SystemColors.ControlText;
                            //labelValiditeMax
                        }
                    }

                    if (vitessemax >= 0 && coutmin >= 0 && coutDeBase >= 0)
                    {
                        //il ne faut pas qu'une unité puisse progresser de plus d'un pixel en une phase
                        if (vitessemax * coutDeBase / Donnees.m_donnees.TAB_JEU[0].I_NOMBRE_PHASES > coutmin)
                        {
                            labelValiditeMax.Text = string.Format("Il faut que vitesse max*cout de base/nombre de phases<= coutmin. On a {0}*{1}/{2}={3} >{4}",
                                vitessemax,
                                coutDeBase,
                                Donnees.m_donnees.TAB_JEU[0].I_NOMBRE_PHASES,
                                vitessemax * coutDeBase / Donnees.m_donnees.TAB_JEU[0].I_NOMBRE_PHASES,
                                coutmin);
                            labelValiditeMax.ForeColor = Color.Red;
                        }
                        else
                        {
                            labelValiditeMax.Text = string.Format("valeurs valides : vitesse max*cout de base/nombre de phases<= coutmin. On a {0}*{1}/{2}={3} <={4}",
                                vitessemax,
                                coutDeBase,
                                Donnees.m_donnees.TAB_JEU[0].I_NOMBRE_PHASES,
                                vitessemax * coutDeBase / Donnees.m_donnees.TAB_JEU[0].I_NOMBRE_PHASES,
                                coutmin);
                            labelValiditeMax.ForeColor = System.Drawing.SystemColors.ControlText;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur dans la valeur des coûts : " + ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonEffacer_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Attention, voulez vous vraiment effacer tous les modèles ?", "Modèles de terrain", MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.Yes)
            {
                /* en cas de problème sur les tables pour tout remettre à zéro */
                m_tablePoint.Clear();
                m_tableGraphisme.Clear();
                m_identifiantsModelesTerrains.Clear();
                m_tableModelesTerrains.Clear();
                dataGridViewModelesTerrain.Rows.Clear();
                dataGridViewCouleurs.Rows.Clear();
                MessageBox.Show("Effacement effectué", "Modèles de terrain", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Recherche la position d'une couleur au cas où l'on ait qq pixels anormaux dans l'image de fond de carte
        /// </summary>
        private void buttonRechercherCouleur_Click(object sender, EventArgs e)
        {
            m_oldcurseur = Cursor;
            Cursor = Cursors.WaitCursor;
            int r= Convert.ToInt32(textBoxR.Text);
            int g= Convert.ToInt32(textBoxG.Text);
            int b= Convert.ToInt32(textBoxB.Text);
            string resultat= "Couleur trouvée sur ";

            if (null == m_imageCarte)
            {
                if (nomCarteTopographique == string.Empty)
                {
                    MessageBox.Show("Il faut ouvrir une image de fond avant traitement", "Recherche Couleur", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                else
                {
                    m_imageCarte = (Bitmap)Image.FromFile(Constantes.repertoireDonnees + nomCarteTopographique);
                }
            }

            for (int x = 0; x < m_imageCarte.Width; x++)
            {
                //on analyse la ligne de pixels suivants
                for (int y = 0; y < m_imageCarte.Height; y++)
                {
                    Color pixelColor = m_imageCarte.GetPixel(x, y);
                    if (pixelColor.R == r && pixelColor.G == g && pixelColor.B==b)
                    {
                        resultat += string.Format("({0},{1}) ", x, y);
                    }
                }
            }
            if (resultat == "Couleur trouvée sur ")
            {
                resultat = "Couleur non trouvée dans l'image";
            }
            Cursor = m_oldcurseur;
            MessageBox.Show(resultat, "Recherche Couleur", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        private void buttonTraitement_Click(object sender, EventArgs e)
        {
            if (null == m_imageCarte)
            {
                if (nomCarteTopographique == string.Empty)
                {
                    MessageBox.Show("Il faut ouvrir une image de fond avant traitement", "Traitement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                else
                {
                    m_imageCarte = (Bitmap)Image.FromFile(Constantes.repertoireDonnees + nomCarteTopographique);
                }
            }

            m_oldcurseur = Cursor;
            Cursor = Cursors.WaitCursor;
            m_traitement = m_sous_traitement = 0;
            timerTraitement.Enabled = true;
        }

        private void FormFondDeCarte_Load(object sender, EventArgs e)
        {
            Redimensionner();
        }

        private void FormFondDeCarte_Resize(object sender, EventArgs e)
        {
            Redimensionner();
        }

        private void Redimensionner()
        {
            #region positionnement de la grille des modèles
            dataGridViewModelesTerrain.Left = 0;
            dataGridViewModelesTerrain.Width = Width - SystemInformation.VerticalScrollBarWidth;
            dataGridViewModelesTerrain.Top = buttonAjouterModele.Bottom + 5;
            dataGridViewModelesTerrain.Height = (Height - buttonAjouterModele.Bottom - 5) / 2 - SystemInformation.HorizontalScrollBarHeight;
            #endregion

            #region positionnement de la grille de la météo
            dataGridViewCouleurs.Left = 0;
            dataGridViewCouleurs.Top = dataGridViewModelesTerrain.Bottom + 1;
            dataGridViewCouleurs.Width = dataGridViewModelesTerrain.Width;
            dataGridViewCouleurs.Height = dataGridViewModelesTerrain.Height;
            #endregion
        }

        private void dataGridViewModelesTerrain_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            int i;

            //s'il y a des cases utilisant ce modèle, on informe l'utilisateur et on interdit la suppression.
            if (Convert.ToInt32(dataGridViewModelesTerrain[6, e.Row.Index].Value) > 0)
            {
                MessageBox.Show("On ne peut supprimer un modèle utilisé sur des cases", "Suppression", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                e.Cancel = true;
            }
            else
            {
                //il faut supprimer les lignes associés dans la grille des couleurs
                i = 0;
                while (i < dataGridViewCouleurs.Rows.Count)
                {
                    DataGridViewRow ligneMeteo = dataGridViewCouleurs.Rows[i];
                    if ((int)ligneMeteo.Cells["ID_MODELE_DU_TERRAIN"].Value == (int)dataGridViewModelesTerrain[0, e.Row.Index].Value)
                    {
                        dataGridViewCouleurs.Rows.Remove(ligneMeteo);
                        //Debug.WriteLine("OK=" + ligneMeteo.Cells["ID_MODELE_DU_TERRAIN"].Value);
                    }
                    else
                    {
                        i++;
                        Debug.WriteLine("KO="+ligneMeteo.Cells["ID_MODELE_DU_TERRAIN"].Value);
                    }
                }
                //il faut supprimer la ligne dans m_tableModelesTerrains
                Donnees.TAB_MODELE_TERRAINRow ligneModeleTerrain =
                m_tableModelesTerrains.FindByID_MODELE_TERRAIN((int)dataGridViewModelesTerrain[0, e.Row.Index].Value);
                if (null != ligneModeleTerrain)
                {
                    ligneModeleTerrain.Delete();
                }
            }
        }

        private void buttonAjouterModele_Click(object sender, EventArgs e)
        {
            FormNouveauModeleTerrain fNouveauModeleTerrain = new FormNouveauModeleTerrain();
            fNouveauModeleTerrain.enCreation = true;
            if (fNouveauModeleTerrain.ShowDialog(this) == DialogResult.OK)
            {
                //on recherche le graphique équivalent
                Color pixelColor = Color.FromArgb(fNouveauModeleTerrain.CouleurR,
                    fNouveauModeleTerrain.CouleurG,
                    fNouveauModeleTerrain.CouleurB);
                Donnees.TAB_GRAPHISMERow ligneGraphisme = RechercheGraphisme(pixelColor);

                //créer la nouvelle ligne dans m_tableModelesTerrains
                int id_modeleTerrain = (int)Donnees.m_donnees.TAB_MODELE_TERRAIN.Compute("Max(ID_MODELE_TERRAIN)", null) + 1;
                Donnees.TAB_MODELE_TERRAINRow ligneModeleTerrain =m_tableModelesTerrains.AddTAB_MODELE_TERRAINRow(
                    id_modeleTerrain,
                    fNouveauModeleTerrain.nomTerrain, //S_NOM
                    ligneGraphisme.ID_GRAPHIQUE,//ID_GRAPHIQUE                    
                    -1,//ID_MODELE_NOUVEAU_TERRAIN,
                    fNouveauModeleTerrain.modificateurDefense,//I_MODIFICATEUR_DEFENSE,
                    fNouveauModeleTerrain.bAnnuleeSiOcccupee,//B_ANNULEE_SI_OCCUPEE,
                    fNouveauModeleTerrain.bRoutier,//B_CIRCUIT_ROUTIER
                    fNouveauModeleTerrain.bObstacle,
                    fNouveauModeleTerrain.bAnnuleEnCombat,
                    fNouveauModeleTerrain.bPont,
                    fNouveauModeleTerrain.bPonton,
                    fNouveauModeleTerrain.bDetruit
                    );

                m_identifiantsModelesTerrains.Add(ligneModeleTerrain.ID_MODELE_TERRAIN, ligneModeleTerrain.ID_MODELE_TERRAIN);
                //ajouter la ligne sur la datagrid
                DataGridViewRow ligne = new DataGridViewRow();
                ligne.CreateCells(dataGridViewModelesTerrain);
                MiseAjourLigneModele(ligne, ligneModeleTerrain, pixelColor);
                dataGridViewModelesTerrain.Rows.Add(ligne);

                //ajouter les météos correspondantes
                foreach (Donnees.TAB_MODELE_MOUVEMENTRow ligneModeleMouvement in m_tableModelesMouvements)
                {
                    foreach(Donnees.TAB_METEORow ligneMeteo in m_tableMeteo)
                    {
                        DataGridViewRow ligneGrid = dataGridViewCouleurs.Rows[dataGridViewCouleurs.Rows.Add()];
                        MiseAjourLigneMeteo(ligneGrid, ligneMeteo, ligneModeleMouvement, ligneModeleTerrain, pixelColor);
                    }
                }
            }
        }

        private void MiseAjourLigneMeteo(DataGridViewRow ligne, Donnees.TAB_METEORow ligneMeteo, Donnees.TAB_MODELE_MOUVEMENTRow ligneModeleMouvement, Donnees.TAB_MODELE_TERRAINRow ligneModeleTerrain, Color pixelColor)
        {
            ligne.Cells[0].Value = ligneMeteo.ID_METEO;//"ID_METEO"
            ligne.Cells[1].Value = ligneModeleMouvement.ID_MODELE_MOUVEMENT;//"ID_MODELE_MOUVEMENT"
            ligne.Cells[2].Value = ligneModeleTerrain.ID_MODELE_TERRAIN;//"ID_MODELE_DU_TERRAIN"
            ligne.Cells[3].Value = ligneMeteo.S_NOM;//"Meteo"
            ligne.Cells[4].Value = ligneModeleMouvement.S_NOM;//"Modele"
            ligne.Cells[5].Value = ligneModeleTerrain.S_NOM;//"NOM_DU_TERRAIN"
            ligne.Cells[6].Value = String.Format("R:{0} G:{1} B{2}", pixelColor.R, pixelColor.G, pixelColor.B);//"Couleur"
            ligne.Cells[7].Style.BackColor = pixelColor;//"Vision"
            //ligne.Cells[8].Value = 0;//"Cout"*/
            /*
            ligneGrid.Cells["ID_MODELE_MOUVEMENT"].Value = ligneModeleMouvement.ID_MODELE_MOUVEMENT;
            ligneGrid.Cells["ID_MODELE_DU_TERRAIN"].Value = ligneModeleTerrain.ID_MODELE_TERRAIN;
            ligneGrid.Cells["ID_METEO"].Value = ligneMeteo.ID_METEO;
            ligneGrid.Cells["Meteo"].Value = ligneMeteo.S_NOM;
            ligneGrid.Cells["Modele"].Value = ligneModeleMouvement.S_NOM;
            ligneGrid.Cells["NOM_DU_TERRAIN"].Value = ligneModeleTerrain.S_NOM;
            ligneGrid.Cells["Couleur"].Value = String.Format("R:{0} G:{1} B{2}", pixelColor.R, pixelColor.G, pixelColor.B);
            ligneGrid.Cells["Cout"].Value = 1;
            ligneGrid.Cells["Vision"].Style.BackColor = pixelColor;
             * */
        }

        private void MiseAjourLigneModele(DataGridViewRow ligne, Donnees.TAB_MODELE_TERRAINRow ligneModeleTerrain, Color pixelColor)
        {
            ligne.Cells[0].Value = ligneModeleTerrain.ID_MODELE_TERRAIN;//"ID_MODELE_TERRAIN"
            ligne.Cells[1].Value = ligneModeleTerrain.S_NOM;//"NOM_TERRAIN"
            ligne.Cells[2].Value = ligneModeleTerrain.B_ANNULEE_SI_OCCUPEE;
            ligne.Cells[3].Value = ligneModeleTerrain.I_MODIFICATEUR_DEFENSE;//"I_MODIFICATEUR_DEFENSE"
            ligne.Cells[4].Style.BackColor = pixelColor;//"TERRAIN_COULEUR"
            ligne.Cells[5].Value = string.Format("{0},{1},{2}", pixelColor.R, pixelColor.G, pixelColor.B);//"RGB"
            ligne.Cells[6].Value = 0;//Nombre de cases
            ligne.Cells[7].Value = ligneModeleTerrain.ID_GRAPHIQUE;//"ID_GRAPHIQUE"
            ligne.Cells[8].Value = ligneModeleTerrain.B_PONT;
            ligne.Cells[9].Value = ligneModeleTerrain.B_PONTON;
            ligne.Cells[10].Value = ligneModeleTerrain.B_DETRUIT;
            ligne.Cells[11].Value = ligneModeleTerrain.B_CIRCUIT_ROUTIER;//"Routier"
            ligne.Cells[12].Value = ligneModeleTerrain.ID_MODELE_NOUVEAU_TERRAIN;//"ID_MODELE_NOUVEAU_TERRAIN"
            ligne.Cells[13].Value = ligneModeleTerrain.B_OBSTACLE_DEFENSIF;
            ligne.Cells[14].Value = ligneModeleTerrain.B_ANNULEE_EN_COMBAT;
        }

        private void buttonSupprimerModele_Click(object sender, EventArgs e)
        {
            if (null == dataGridViewModelesTerrain.CurrentRow)
            {
                MessageBox.Show("Vous devez selectionner une ligne de modèle avant de pouvoir la supprimer", "Fond de Carte", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }

            if (DialogResult.Yes == MessageBox.Show("Etes vous sur de vouloir supprimer cette ligne de modèle ?", "Fond de Carte", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                DataGridViewRow ligne = dataGridViewModelesTerrain.CurrentRow;
                Donnees.TAB_MODELE_TERRAINRow ligneModeleTerrain = m_tableModelesTerrains.FindByID_MODELE_TERRAIN(Convert.ToInt32(ligne.Cells[0].Value));//"ID_MODELE_TERRAIN"
                Donnees.TAB_GRAPHISMERow ligneGraphisme = m_tableGraphisme.FindByID_GRAPHIQUE(Convert.ToInt32(ligne.Cells[7].Value));//"ID_GRAPHIQUE"

                //supprimer les météos correspondantes
                int i = 0;
                while (i < dataGridViewCouleurs.Rows.Count)
                {
                    DataGridViewRow ligneGrid = dataGridViewCouleurs.Rows[i];
                    if (Convert.ToInt32(ligneGrid.Cells["ID_MODELE_DU_TERRAIN"].Value) == ligneModeleTerrain.ID_MODELE_TERRAIN)
                    {
                        dataGridViewCouleurs.Rows.Remove(ligneGrid);
                    }
                    else
                    {
                        i++;
                    }
                }

                //suppression de la ligne dans m_tableModelesTerrains
                if (null != ligneModeleTerrain)
                {
                    m_identifiantsModelesTerrains.Remove(ligneModeleTerrain.ID_MODELE_TERRAIN);
                    m_tableModelesTerrains.RemoveTAB_MODELE_TERRAINRow(ligneModeleTerrain);
                }

                //suppression du graphisme
                if (null != ligneGraphisme)
                {
                    //on ne supprimer la ligne que si celui-ci n'est utilisé par aucun autre modele
                    bool btrouve = false;
                    i++;
                    while (!btrouve && i < dataGridViewModelesTerrain.Rows.Count)
                    {
                        DataGridViewRow ligneRecherche = dataGridViewModelesTerrain.Rows[i];
                        if (ligneRecherche != ligne && ligneGraphisme.ID_GRAPHIQUE == Convert.ToInt32(ligneRecherche.Cells[7].Value))
                        {
                            btrouve = true;
                        }
                        i++;
                    }
                    if (!btrouve)
                    {
                        m_tableGraphisme.RemoveTAB_GRAPHISMERow(ligneGraphisme);
                    }
                }

                dataGridViewModelesTerrain.Rows.Remove(ligne);
            }
        }

        private void buttonModifierModele_Click(object sender, EventArgs e)
        {
            if (null == dataGridViewModelesTerrain.CurrentRow)
            {
                MessageBox.Show("Vous devez selectionner une ligne de modèle avant de pouvoir la modifier", "Fond de Carte", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            DataGridViewRow ligne = dataGridViewModelesTerrain.CurrentRow;
            Donnees.TAB_MODELE_TERRAINRow ligneModeleTerrain = m_tableModelesTerrains.FindByID_MODELE_TERRAIN(Convert.ToInt32(ligne.Cells[0].Value));//"ID_MODELE_TERRAIN"
            Donnees.TAB_GRAPHISMERow ligneGraphisme = m_tableGraphisme.FindByID_GRAPHIQUE(Convert.ToInt32(ligne.Cells[7].Value));//"ID_GRAPHIQUE"
            Donnees.TAB_POINTRow lignePoint = m_tablePoint.FindByID_POINT(ligneGraphisme.ID_POINT);

            FormNouveauModeleTerrain fNouveauModeleTerrain = new FormNouveauModeleTerrain();
            fNouveauModeleTerrain.enCreation = false;
            fNouveauModeleTerrain.identifiant = ligneModeleTerrain.ID_MODELE_TERRAIN;
            fNouveauModeleTerrain.nomTerrain = ligneModeleTerrain.S_NOM;
            fNouveauModeleTerrain.bPont = ligneModeleTerrain.B_PONT;
            fNouveauModeleTerrain.bPonton = ligneModeleTerrain.B_PONTON;
            fNouveauModeleTerrain.bDetruit = ligneModeleTerrain.B_DETRUIT;
            fNouveauModeleTerrain.bAnnuleeSiOcccupee = ligneModeleTerrain.B_ANNULEE_SI_OCCUPEE;
            fNouveauModeleTerrain.bRoutier = ligneModeleTerrain.B_CIRCUIT_ROUTIER;
            fNouveauModeleTerrain.bObstacle = ligneModeleTerrain.B_OBSTACLE_DEFENSIF;
            fNouveauModeleTerrain.bAnnuleEnCombat = ligneModeleTerrain.B_ANNULEE_EN_COMBAT;
            fNouveauModeleTerrain.modificateurDefense = ligneModeleTerrain.I_MODIFICATEUR_DEFENSE;
            fNouveauModeleTerrain.CouleurB = lignePoint.I_BLEU;
            fNouveauModeleTerrain.CouleurR = lignePoint.I_ROUGE;
            fNouveauModeleTerrain.CouleurG = lignePoint.I_VERT;
            if (fNouveauModeleTerrain.ShowDialog(this) == DialogResult.OK)
            {
                //mise à jour des données
                if (ligneModeleTerrain.ID_MODELE_TERRAIN != fNouveauModeleTerrain.identifiant)
                {
                    //il faut mettre à jour les identifiants de la table météo avant de valider la modification
                    foreach (DataGridViewRow ligneGrid in dataGridViewCouleurs.Rows)
                    {
                        if (Convert.ToInt32(ligneGrid.Cells["ID_MODELE_DU_TERRAIN"].Value) == ligneModeleTerrain.ID_MODELE_TERRAIN)
                        {
                            ligneGrid.Cells["ID_MODELE_DU_TERRAIN"].Value = fNouveauModeleTerrain.identifiant;
                        }
                    }
                }

                m_identifiantsModelesTerrains[ligneModeleTerrain.ID_MODELE_TERRAIN] = fNouveauModeleTerrain.identifiant;
                if (fNouveauModeleTerrain.identifiant != fNouveauModeleTerrain.identifiant)
                {
                    m_identifiantsModelesTerrains.Add(fNouveauModeleTerrain.identifiant, fNouveauModeleTerrain.identifiant);
                }
                ligneModeleTerrain.ID_MODELE_TERRAIN = fNouveauModeleTerrain.identifiant;
                ligneModeleTerrain.S_NOM = fNouveauModeleTerrain.nomTerrain;
                ligneModeleTerrain.B_PONT = fNouveauModeleTerrain.bPont;
                ligneModeleTerrain.B_PONTON = fNouveauModeleTerrain.bPonton;
                ligneModeleTerrain.B_DETRUIT = fNouveauModeleTerrain.bDetruit;
                ligneModeleTerrain.B_ANNULEE_SI_OCCUPEE = fNouveauModeleTerrain.bAnnuleeSiOcccupee;
                ligneModeleTerrain.B_CIRCUIT_ROUTIER = fNouveauModeleTerrain.bRoutier;
                ligneModeleTerrain.B_OBSTACLE_DEFENSIF = fNouveauModeleTerrain.bObstacle;
                ligneModeleTerrain.B_ANNULEE_EN_COMBAT = fNouveauModeleTerrain.bAnnuleEnCombat;
                ligneModeleTerrain.I_MODIFICATEUR_DEFENSE = fNouveauModeleTerrain.modificateurDefense;
                lignePoint.I_BLEU = (short)fNouveauModeleTerrain.CouleurB;
                lignePoint.I_ROUGE = (short)fNouveauModeleTerrain.CouleurR;
                lignePoint.I_VERT = (short)fNouveauModeleTerrain.CouleurG;

                //maintenant il faut mettre à jour la datagrid modele
                Color pixelColor = Color.FromArgb(fNouveauModeleTerrain.CouleurR,
                   fNouveauModeleTerrain.CouleurG,
                   fNouveauModeleTerrain.CouleurB);
                MiseAjourLigneModele(ligne, ligneModeleTerrain, pixelColor);

                //maintenant il faut mettre à jour la datagrid meteo
                foreach (DataGridViewRow ligneGrid in dataGridViewCouleurs.Rows)
                {
                    if (Convert.ToInt32(ligneGrid.Cells["ID_MODELE_DU_TERRAIN"].Value) == ligneModeleTerrain.ID_MODELE_TERRAIN)
                    {
                        Donnees.TAB_MODELE_MOUVEMENTRow ligneModeleMouvement = m_tableModelesMouvements.FindByID_MODELE_MOUVEMENT(Convert.ToInt32(ligneGrid.Cells["ID_MODELE_MOUVEMENT"].Value));
                        Donnees.TAB_METEORow ligneMeteo = m_tableMeteo.FindByID_METEO(Convert.ToInt32(ligneGrid.Cells["ID_METEO"].Value));
                        MiseAjourLigneMeteo(ligneGrid, ligneMeteo, ligneModeleMouvement, ligneModeleTerrain, pixelColor);
                    }
                }                    
            }
        }
    }
}