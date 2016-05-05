using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Threading;
using Microsoft.Win32;

using WaocLib;
using System.Collections.Generic;
using System.Runtime.InteropServices;


namespace vaoc
{
    public partial class FormPrincipale : Form
    {
        //protected DataSetCoutDonnees DataSetCoutDonnees.m_donnees; mis en static dans BaseVaoc
        protected decimal m_zoom;//niveau de zoom sur l'image
        protected Donnees.TAB_CASERow m_departPlusCourtChemin;
        protected Donnees.TAB_CASERow m_arriveePlusCourtChemin;
        protected AStarVille m_etoile;
        protected AStar m_etoileHPA;
        protected DateTime m_dateDebut;
        protected bool m_modification;
        protected List<Donnees.TAB_CASERow> m_cheminPCC;
        protected List<Donnees.TAB_CASERow> m_cheminHPA;
        protected List<Donnees.TAB_CASERow> m_cheminVille;
        protected List<Donnees.TAB_CASERow> m_cheminSelection;
        protected List<Donnees.TAB_CASERow> m_cheminVerifierTrajet;
        protected Cursor m_ancienCurseur;
        protected Donnees.TAB_PIONRow m_lignePionSelection;

        enum testBEA { MOUVEMENT = 0, RAVITAILLEMENT = 1 };

        #region gestion du fichier le plus récent
        protected MruStripMenuInline mruMenu;
        protected string curFileName;
        #endregion

        public FormPrincipale()
        {
            InitializeComponent();
            Donnees.m_donnees = new Donnees();
            m_zoom = 1;
            m_departPlusCourtChemin = null;
            m_arriveePlusCourtChemin = null;
            m_etoile = new AStarVille();
            m_modification = false;
            m_lignePionSelection = null;
            buttonRecalculTrajet.Enabled = false;

            #region gestion du fichier le plus récent
            //RegistryKey regKey = Registry.CurrentUser.OpenSubKey(mruRegKey);
            //if (regKey != null)
            //{
            //    menuClearRegistryOnExit.Checked = (int)regKey.GetValue("delSubkey", 1) != 0;
            //    regKey.Close();
            //}

            mruMenu = new MruStripMenuInline(fichierToolStripMenuItem, fichiersRecentsToolStripMenuItem, new MruStripMenu.ClickedHandler(OnMruFile), Constantes.CST_CLEFDEREGISTRE + "\\MRU");

            IncFilename();
            #endregion
        }

        #region gestion du fichier le plus récent
        private int m_curFileNum = 0;

        private void IncFilename()
        {
            m_curFileNum++;
        }

        private void OnMruFile(int number, String filename)
        {
            InitialiserDonnees();
            ChargementPartie(filename, Donnees.m_donnees);
            mruMenu.SetFirstFile(number);
        }

        private void ChargementPartie(string filename, Donnees donnees)
        {
            WaocLib.Dal.ChargerPartie(filename, Donnees.m_donnees);
            curFileName = filename;
            Constantes.repertoireDonnees = curFileName;
            m_modification = false;

            # region correctif pour ajout de données
            Bitmap imageCarte = (Bitmap)Image.FromFile(Constantes.repertoireDonnees + Donnees.m_donnees.TAB_JEU[0].S_NOM_CARTE_TOPOGRAPHIQUE);
            Color nouvelleCouleur = Color.FromArgb(160,160,160);
            for (int x = 0; x < imageCarte.Width; x++)
            {
                //on analyse la ligne de pixels suivants
                for (int y = 0; y < imageCarte.Height; y++)
                {
                    Color pixelColor = imageCarte.GetPixel(x, y);
                    if (pixelColor.R == 100 && pixelColor.G == 100 && pixelColor.B==200)
                    {
                        imageCarte.SetPixel(x, y, nouvelleCouleur);
                    }
                }
            }
            imageCarte.Save(Constantes.repertoireDonnees + Donnees.m_donnees.TAB_JEU[0].S_NOM_CARTE_TOPOGRAPHIQUE + "new");

            foreach (Donnees.TAB_MODELE_TERRAINRow ligneModeleTerrain in Donnees.m_donnees.TAB_MODELE_TERRAIN)
            {
                if (ligneModeleTerrain.IsB_PONTNull()) { ligneModeleTerrain.B_PONT = false; };
                if (ligneModeleTerrain.IsB_GUENull()) { ligneModeleTerrain.B_GUE = false; };
            }

            foreach (Donnees.TAB_PCC_VILLESRow lignePCCVille in Donnees.m_donnees.TAB_PCC_VILLES)
            {
                if (lignePCCVille.IsB_CREATIONNull()) { lignePCCVille.B_CREATION = false; }
            }
            if (Donnees.m_donnees.TAB_JEU[0].IsI_DISTANCEVILLEMAX_PCCNull())
            {
                Donnees.m_donnees.TAB_JEU[0].I_DISTANCEVILLEMAX_PCC = 50;
            }
            /*
            Donnees.TAB_MODELE_TERRAINRow ligneModeleTerrain;
            int nb = 0;
            foreach (Donnees.TAB_CASERow ligneCase in Donnees.m_donnees.TAB_CASE)
            {
                ligneModeleTerrain = Donnees.m_donnees.TAB_MODELE_TERRAIN.FindByID_MODELE_TERRAIN(ligneCase.ID_MODELE_TERRAIN);
                    if (ligneModeleTerrain.B_ANNULEE_SI_OCCUPEE)
                    {
                        ligneCase.SetID_MODELE_TERRAIN_SI_OCCUPENull();
                        nb++;
                    }
            }
            Debug.WriteLine(string.Format("{0} cases ont IsID_MODELE_TERRAIN_SI_OCCUPENull sur un total de {1}", nb, Donnees.m_donnees.TAB_CASE.Count));
            */
            /*
            Donnees.TAB_MODELE_TERRAINRow ligneModeleTerrain;
            int nb = 0;
            foreach (Donnees.TAB_CASERow ligneCase in Donnees.m_donnees.TAB_CASE)
            {
                if (ligneCase.IsID_MODELE_TERRAIN_SI_OCCUPENull())
                {
                    ligneModeleTerrain = Donnees.m_donnees.TAB_MODELE_TERRAIN.FindByID_MODELE_TERRAIN(ligneCase.ID_MODELE_TERRAIN);
                    //if (!ligneModeleTerrain.B_ANNULEE_SI_OCCUPEE)
                    {
                        Debug.WriteLine(string.Format("Case ID ={0} ({1},{2}), modele_terrain={3} a un modele terrain si occupe Null",
                            ligneCase.ID_CASE, ligneCase.I_X, ligneCase.I_Y, ligneCase.ID_MODELE_TERRAIN));
                        //ligneCase.ID_MODELE_TERRAIN_SI_OCCUPE = ligneCase.ID_MODELE_TERRAIN;
                        nb++;
                    }
                }
            }
            Debug.WriteLine(string.Format("{0} cases ont IsID_MODELE_TERRAIN_SI_OCCUPENull sur un total de {1}",nb, Donnees.m_donnees.TAB_CASE.Count));

            ligneModeleTerrain = Donnees.m_donnees.TAB_MODELE_TERRAIN.FindByID_MODELE_TERRAIN(67);
            if (null != ligneModeleTerrain)
            {
                ligneModeleTerrain.ID_MODELE_TERRAIN = 55;
                foreach (Donnees.TAB_CASERow ligneCase in Donnees.m_donnees.TAB_CASE)
                {
                    if (ligneCase.ID_MODELE_TERRAIN == 67)
                    {
                        ligneCase.ID_MODELE_TERRAIN = 55;
                    }
                    if (!ligneCase.IsID_MODELE_TERRAIN_SI_OCCUPENull() && ligneCase.ID_MODELE_TERRAIN_SI_OCCUPE == 67)
                    {
                        ligneCase.ID_MODELE_TERRAIN_SI_OCCUPE = 55;
                    }
                }
            }
            */
            /*
            foreach (Donnees.TAB_PIONRow lignePion in Donnees.m_donnees.TAB_PION)
            {
                lignePion.B_REDITION_RAVITAILLEMENT = false;
                lignePion.B_TELEPORTATION = false;
                lignePion.SetI_TOUR_SANS_RAVITAILLEMENTNull();
                if (!lignePion.IsID_BATAILLENull() && lignePion.ID_BATAILLE <= 0)
                {
                    lignePion.SetID_BATAILLENull();
                    lignePion.SetI_ZONE_BATAILLENull();
                }
                lignePion.I_TOUR_SANS_RAVITAILLEMENT = 0;
            }
            */
            /* Test sur les angles 
            double x1,x2,x3,y1,y2,y3;
            x1=y1=100;
            x3=x1+100; y3=y1;
            x2=y2=200;
            double angle = Constantes.AngleOfView(x1,y1,x2,y2,x3,y3);
            Debug.WriteLine(string.Format("angle={0} x,y={1},{2}  x,y={3},{4}  x,y={5},{6}", angle, x1,y1, x2, y2, x3, y3));

            x2 = x1; y2 = 200;
            angle = Constantes.AngleOfView(x1, y1, x2, y2, x3, y3);
            Debug.WriteLine(string.Format("angle={0} x,y={1},{2}  x,y={3},{4}  x,y={5},{6}", angle, x1, y1, x2, y2, x3, y3));

            x2 = y2 = 0;
            angle = Constantes.AngleOfView(x1, y1, x2, y2, x3, y3);
            Debug.WriteLine(string.Format("angle={0} x,y={1},{2}  x,y={3},{4}  x,y={5},{6}", angle, x1, y1, x2, y2, x3, y3));
            */
            /**/
            Donnees.TAB_NOMS_CARTERow lVDebug1, lVDebug2;
            Donnees.TAB_CASERow lDebug1, lDebug2;
            Debug.WriteLine("TAB_PCC_VILLES #=" + Donnees.m_donnees.TAB_PCC_VILLES.Count);
            Donnees.TAB_PCC_VILLESRow lignePccVilleMax = null;
            foreach (Donnees.TAB_PCC_VILLESRow lignePccVille in Donnees.m_donnees.TAB_PCC_VILLES)
            {
                if (lignePccVille.I_COUT <= 0)
                {
                    lVDebug1 = Donnees.m_donnees.TAB_NOMS_CARTE.FindByID_NOM(lignePccVille.ID_VILLE_DEBUT);
                    lVDebug2 = Donnees.m_donnees.TAB_NOMS_CARTE.FindByID_NOM(lignePccVille.ID_VILLE_FIN);
                    lDebug1 = Donnees.m_donnees.TAB_CASE.FindByID_CASE(lVDebug1.ID_CASE);
                    lDebug2 = Donnees.m_donnees.TAB_CASE.FindByID_CASE(lVDebug2.ID_CASE);
                    Debug.WriteLine(string.Format("trajet impossible {0}:{1}({2},{3}) -> {4}:{5}({6},{7}), cout={8}",
    lVDebug1.S_NOM,
    lDebug1.ID_CASE,
    lDebug1.I_X,
    lDebug1.I_Y,
    lVDebug2.S_NOM,
    lDebug2.ID_CASE,
    lDebug2.I_X,
    lDebug2.I_Y,
    lignePccVille.I_COUT
    ));
                }
                if ((null == lignePccVilleMax) || (lignePccVille.I_COUT > lignePccVilleMax.I_COUT)) { lignePccVilleMax = lignePccVille; }
                
            }/**/
            if (null != lignePccVilleMax)
            {
                lVDebug1 = Donnees.m_donnees.TAB_NOMS_CARTE.FindByID_NOM(lignePccVilleMax.ID_VILLE_DEBUT);
                lVDebug2 = Donnees.m_donnees.TAB_NOMS_CARTE.FindByID_NOM(lignePccVilleMax.ID_VILLE_FIN);
                lDebug1 = Donnees.m_donnees.TAB_CASE.FindByID_CASE(lVDebug1.ID_CASE);
                lDebug2 = Donnees.m_donnees.TAB_CASE.FindByID_CASE(lVDebug2.ID_CASE);
                Debug.WriteLine(string.Format("trajet max {0},id={9} :{1}({2},{3}) -> {4}id={10} :{5}({6},{7}), cout={8}",
                    lVDebug1.S_NOM,
                    lDebug1.ID_CASE,
                    lDebug1.I_X,
                    lDebug1.I_Y,
                    lVDebug2.S_NOM,
                    lDebug2.ID_CASE,
                    lDebug2.I_X,
                    lDebug2.I_Y,
                    lignePccVilleMax.I_COUT,
                    lVDebug1.ID_NOM,
                    lVDebug2.ID_NOM
                    ));
            }
            /*
            string requeteDebug = "ID_VILLE_DEBUT=24 AND ID_VILLE_FIN=25";
            Donnees.TAB_PCC_VILLESRow[] lignesDejaTraite = (Donnees.TAB_PCC_VILLESRow[])Donnees.m_donnees.TAB_PCC_VILLES.Select(requeteDebug);
            for (int i = lignesDejaTraite[0].ID_TRAJET - 5; i < lignesDejaTraite[0].ID_TRAJET + 5; i++)
            {
                requeteDebug = "ID_TRAJET=" + i;
                Donnees.TAB_PCC_VILLESRow[] lignesVilles = (Donnees.TAB_PCC_VILLESRow[])Donnees.m_donnees.TAB_PCC_VILLES.Select(requeteDebug);
                lVDebug1 = Donnees.m_donnees.TAB_NOMS_CARTE.FindByID_NOM(lignesVilles[0].ID_VILLE_DEBUT);
                lVDebug2 = Donnees.m_donnees.TAB_NOMS_CARTE.FindByID_NOM(lignesVilles[0].ID_VILLE_FIN);
                    lDebug1 = Donnees.m_donnees.TAB_CASE.FindByID_CASE(lVDebug1.ID_CASE);
                    lDebug2 = Donnees.m_donnees.TAB_CASE.FindByID_CASE(lVDebug2.ID_CASE);
                    Debug.WriteLine(string.Format("trajet ID={9} {0},id={10}:{1}({2},{3}) -> {4},id={11}:{5}({6},{7}), cout={8}",
                    lVDebug1.S_NOM,
                    lDebug1.ID_CASE,
                    lDebug1.I_X,
                    lDebug1.I_Y,
                    lVDebug2.S_NOM,
                    lDebug2.ID_CASE,
                    lDebug2.I_X,
                    lDebug2.I_Y,
                    lignesVilles[0].I_COUT,
                    lignesVilles[0].ID_TRAJET,
                    lVDebug1.ID_NOM,
                    lVDebug2.ID_NOM
                    ));
            }
            */
            //remet quelques noms de ville sur les cases de villes
            Donnees.TAB_NOMS_CARTERow ligneNomCarte = Donnees.m_donnees.TAB_NOMS_CARTE.FindByID_NOM(38);
            ligneNomCarte.ID_CASE = 4262028;
            ligneNomCarte = Donnees.m_donnees.TAB_NOMS_CARTE.FindByID_NOM(150);
            ligneNomCarte.ID_CASE = 5720430;
            ligneNomCarte = Donnees.m_donnees.TAB_NOMS_CARTE.FindByID_NOM(162);
            ligneNomCarte.ID_CASE = 5916513;

            //foreach (Donnees.TAB_NOMS_CARTERow ligneNomCarte in Donnees.m_donnees.TAB_NOMS_CARTE)
            //{
            //    ligneNomCarte.I_VICTOIRE = 0;
            //}
            //foreach (Donnees.TAB_PARCOURSRow ligneParcours in Donnees.m_donnees.TAB_PARCOURS)
            //{
            //    ligneParcours.I_TYPE = (int)Cartographie.typeParcours.MOUVEMENT;
            //}
            //foreach (DataSetCoutDonnees.TAB_MESSAGERow ligneMessage in DataSetCoutDonnees.m_donnees.TAB_MESSAGE)
            //{
            //    ligneMessage.SetI_ZONE_BATAILLENull();
            //}
            ///*
            //foreach (DataSetCoutDonnees.TAB_MODELE_TERRAINRow ligneTerrain in DataSetCoutDonnees.m_donnees.TAB_MODELE_TERRAIN)
            //{
            //    ligneTerrain.B_OBSTACLE_DEFENSIF = false;
            //    ligneTerrain.B_ANNULEE_EN_COMBAT = false;
            //}
            // * */
            //DataSetCoutDonnees.m_donnees.TAB_PARTIE[0].I_HAUTEUR_CARTE_ZOOM_WEB = 100;
            //DataSetCoutDonnees.m_donnees.TAB_PARTIE[0].I_LARGEUR_CARTE_ZOOM_WEB = 100;

            //DataSetCoutDonnees.m_donnees.TAB_JEU[0].I_OBJECTIF = 1;
            //foreach (DataSetCoutDonnees.TAB_MODELE_TERRAINRow ligneModele in DataSetCoutDonnees.m_donnees.TAB_MODELE_TERRAIN)
            //{
            //    ligneModele.B_CIRCUIT_ROUTIER = false;
            //}
            //DataSetCoutDonnees.m_donnees.TAB_JEU[0].I_RAYON_BATAILLE = 30;
            ////DataSetCoutDonnees.m_donnees.TAB_NATION[1].I_ENCOMBREMENT_ARRET = 10000;
            //Donnees.TAB_PIONRow lignePion11 = Donnees.m_donnees.TAB_PION.FindByID_PION(11);
            //lignePion11.SupprimerTousLesOrdres();

            //Donnees.TAB_PCC_COUTSRow[] test = (Donnees.TAB_PCC_COUTSRow[])Donnees.m_donnees.TAB_PCC_COUTS.Select("", "ID_CASE_DEBUT");
            //int idOldDebut=-1;
            //int idOldFin=-1;
            //foreach (Donnees.TAB_PCC_COUTSRow ligneCout in test)
            //{
            //    if (ligneCout.ID_CASE_FIN == idOldFin && (ligneCout.ID_CASE_DEBUT == idOldDebut))
            //    {
            //        Debug.WriteLine("Bug debut="+idOldDebut+" fin="+idOldFin);
            //    }
            //    idOldFin=ligneCout.ID_CASE_FIN;
            //    idOldDebut=ligneCout.ID_CASE_DEBUT;
            //}

            //List<int> listeCases = new List<int>();
            //foreach (Donnees.TAB_PCC_COUTSRow ligneCout in Donnees.m_donnees.TAB_PCC_COUTS)
            //{
            //    Donnees.TAB_PCC_TRAJETRow[] tableTrajet= (Donnees.TAB_PCC_TRAJETRow[]) Donnees.m_donnees.TAB_PCC_TRAJET.Select("ID_TRAJET="+ligneCout.ID_TRAJET.ToString(),"I_ORDRE");
            //    listeCases.Clear();
            //    foreach (Donnees.TAB_PCC_TRAJETRow ligneTrajet in tableTrajet)
            //        listeCases.Add(ligneTrajet.ID_CASE);
            //    Dal.SauvegarderTrajet(ligneCout.ID_TRAJET, listeCases);
            //}
            //Donnees.m_donnees.TAB_PCC_TRAJET.Clear();

            //MessageBox.Show("Avant Donnees.m_donnees.TAB_PCC_COUTS.Count=" + Donnees.m_donnees.TAB_PCC_COUTS.Count.ToString());
            //int i = Donnees.m_donnees.TAB_PCC_COUTS.Count-1;
            //while (i>=0)
            //{
            //    //il faut copier un doublon du retour
            //    Donnees.TAB_PCC_COUTSRow ligneCout = Donnees.m_donnees.TAB_PCC_COUTS[i];
            //    Donnees.m_donnees.TAB_PCC_COUTS.AddTAB_PCC_COUTSRow(
            //        ligneCout.I_BLOCX,
            //        ligneCout.I_BLOCY,
            //        ligneCout.ID_CASE_FIN,//juste une inversion fin/debut
            //        ligneCout.ID_CASE_DEBUT,
            //        ligneCout.I_COUT,
            //        ligneCout.ID_TRAJET);
            //    i--;
            //}
            //MessageBox.Show("Apres Donnees.m_donnees.TAB_PCC_COUTS.Count=" + Donnees.m_donnees.TAB_PCC_COUTS.Count.ToString());
            //int i = Donnees.m_donnees.TAB_PCC_COUTS.Count - 1;
            //while (i >= 0)
            //{
            //    Donnees.TAB_PCC_COUTSRow ligneCout = Donnees.m_donnees.TAB_PCC_COUTS[i];
            //    if (ligneCout.I_COUT < 0)
            //    {
            //        ligneCout.I_COUT=AStar.CST_COUTMAX;
            //    }
            //    i--;
            //}
            //Donnees.m_donnees.TAB_PARTIE[0].I_TOUR = 9;
            /*
            foreach (Donnees.TAB_PIONRow lignePion in Donnees.m_donnees.TAB_PION)
            {
                if (lignePion.B_DETRUIT) continue;
                //chargement du trajet actuel
                string requete = string.Format("ID_PION={0}", lignePion.ID_PION);
                string tri = "I_ORDRE";
                Donnees.TAB_PARCOURSRow[] parcoursExistant = (Donnees.TAB_PARCOURSRow[])Donnees.m_donnees.TAB_PARCOURS.Select(requete, tri);

                if ((null != parcoursExistant) && (0 < parcoursExistant.Length))
                {
                    List<Donnees.TAB_CASERow> chemin = new List<Donnees.TAB_CASERow>(parcoursExistant.Length);
                    for (int i = 0; i < parcoursExistant.Length; i++)
                    {
                        chemin.Add(Donnees.m_donnees.TAB_CASE.FindByID_CASE(parcoursExistant[i].ID_CASE));
                    }
                    List<Donnees.TAB_CASERow> nouveauChemin = Cartographie.ParcoursOptimise(chemin);
                    if (nouveauChemin.Count < chemin.Count)
                    {
                        //on detruit l'ancien parcours
                        foreach (Donnees.TAB_PARCOURSRow ligneParcours in parcoursExistant)
                        {
                            ligneParcours.Delete();
                        }
                        Donnees.m_donnees.AcceptChanges();
                        //on sauvegarde le nouveau chemin
                        int casePrecedente = -1;
                        int i = 0;
                        foreach (Donnees.TAB_CASERow ligneCase in nouveauChemin)
                        {
                            if (casePrecedente != ligneCase.ID_CASE)
                            {
                                //pour éviter d'avoir deux fois la même case de suite dans le parcours, possible dans certains cas rares
                                Donnees.m_donnees.TAB_PARCOURS.AddTAB_PARCOURSRow(lignePion.ID_PION, i++, ligneCase.ID_CASE);
                            }
                            casePrecedente = ligneCase.ID_CASE;
                        }
                        Donnees.m_donnees.AcceptChanges();
                    }
                }
            }
            */
            #endregion

            if (!Donnees.m_donnees.TAB_JEU[0].IsS_NOM_CARTE_TOPOGRAPHIQUENull()
                && Donnees.m_donnees.TAB_JEU[0].S_NOM_CARTE_TOPOGRAPHIQUE.Length > 0)
            {
                //chargement du fichier graphique de la carte
                ConstruireImageCarte();
            }
            MiseAJourTitreFenetre();
            //mise à jour de la liste de selection des unités
            comboBoxListeUnites.Items.Clear();


            IEnumerable<Donnees.TAB_PIONRow> requete =
            from pion in Donnees.m_donnees.TAB_PION
            orderby pion.ID_PION
            select pion;

            foreach (Donnees.TAB_PIONRow lignePion in requete)
            {
                comboBoxListeUnites.Items.Add(lignePion);
            }
        }

        private void MiseAJourTitreFenetre()
        {
            this.Text = String.Format("VAOC - {0} {1} {2} tour:{3} phase:{4}",
                Donnees.m_donnees.TAB_JEU[0].S_NOM,
                Donnees.m_donnees.TAB_PARTIE[0].S_NOM,
                ClassMessager.DateHeure(false),
                Donnees.m_donnees.TAB_PARTIE[0].I_TOUR,
                Donnees.m_donnees.TAB_PARTIE[0].I_PHASE);
        }

        #endregion


        private bool SauvegarderPartie(string nomFichier)
        {
            m_modification = false;
            //Mise à jour de la version du fichier pour de future mise à jour
            Donnees.m_donnees.TAB_JEU[0].I_VERSION = 1;
            return Dal.SauvegarderPartie(nomFichier, Donnees.m_donnees);
        }

        /// <summary>
        /// Tests en tout genre
        /// </summary>
        private void buttonData_Click(object sender, EventArgs e)
        {

            InterfaceVaocWeb iWeb = ClassVaocWebFactory.CreerVaocWeb("toto.txt", true);
            try
            {
                WebRequest requete = WebRequest.Create("http://localhost:8080/vaoc/service.php?op=utilisateurs");
                WebResponse reponse = requete.GetResponse();
                if (reponse is HttpWebResponse)
                {
                    StreamReader sr = new StreamReader(reponse.GetResponseStream(), Encoding.GetEncoding("ISO-8859-1"));
                    string strXML = sr.ReadToEnd();
                    XmlDocument docXML = new XmlDocument();
                    docXML.LoadXml(strXML);
                    XmlElement elem=docXML["UTILISATEURS"];
                    foreach (XmlNode noeud in elem.ChildNodes)
                    {
                        string nom=noeud["S_NOM"].InnerText;
                    }

                }
            }
            catch (Exception exp)
            {
                MessageBox.Show("Erreur sur buttonData_Click :" + exp.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void imprimerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FonctionNonImplementée("imprimerToolStripMenuItem_Click");
        }

        private DialogResult FonctionNonImplementée(string libelle)
        {
            return MessageBox.Show(libelle, "Fonction non implémentée", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void quitterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (null != curFileName)
            {
                try
                {
                    SauvegarderPartie(curFileName);
                    m_modification = false;
                }
                catch (Exception ex)
                {

                    MessageBox.Show("Erreur sur Sauvegarder :" + ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void nouveauToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InitialiserDonnees();
        }

        private void InitialiserDonnees()
        {
            Donnees.m_donnees.Clear();
            if (null != ImageCarte.BackgroundImage)
            {
                ImageCarte.BackgroundImage.Dispose();
                ImageCarte.BackgroundImage = null;
            }
        }

        private void ouvrirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InitialiserDonnees();
            if (DialogResult.OK == this.openFileDialog.ShowDialog(this))
            {
                ChargementPartie(openFileDialog.FileName, Donnees.m_donnees);
                mruMenu.AddFile(curFileName);
                mruMenu.SaveToRegistry();
            }
        }

        private void AProposToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormAPropos apropos = new FormAPropos();
            apropos.ShowDialog();
        }

        private void generalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormGeneral general = new FormGeneral();
            string nomCarte, nomCarteZoom, nomCarteGris;
            string repertoireDest;
            bool fl_demmarage = false;
            string messageArbitre = string.Empty;
            int iTour = 0;
            int iPhase = 0;
            int idVictoire = -1;
            int idMeteo = -1;

            //initialisation des données de la form
            if (Donnees.m_donnees.TAB_JEU.Count > 0)
            {
                general.fichierCourant = curFileName;
                general.echelle = Donnees.m_donnees.TAB_JEU[0].I_ECHELLE;
                general.ID_modele_terrain_deploiement = Donnees.m_donnees.TAB_JEU[0].ID_MODELE_TERRAIN_DEPLOIEMENT;
                general.coutDeBase = Donnees.m_donnees.TAB_JEU[0].I_COUT_DE_BASE;
                general.rayonDeBataille = Donnees.m_donnees.TAB_JEU[0].I_RAYON_BATAILLE;
                general.id_jeu = Donnees.m_donnees.TAB_JEU[0].ID_JEU;
                general.objectifVictoire = Donnees.m_donnees.TAB_JEU[0].I_OBJECTIF;

                if (!Donnees.m_donnees.TAB_JEU[0].IsS_NOM_CARTE_HISTORIQUENull())
                {
                    general.nomCarte = Donnees.m_donnees.TAB_JEU[0].S_NOM_CARTE_HISTORIQUE;
                }
                if (!Donnees.m_donnees.TAB_JEU[0].IsI_LARGEUR_CARTENull() ||
                    !Donnees.m_donnees.TAB_JEU[0].IsI_HAUTEUR_CARTENull())
                {
                    general.largeurCarte = Donnees.m_donnees.TAB_JEU[0].I_LARGEUR_CARTE;
                    general.hauteurCarte = Donnees.m_donnees.TAB_JEU[0].I_HAUTEUR_CARTE;
                }
                if (!Donnees.m_donnees.TAB_JEU[0].IsS_NOM_CARTE_GRISNull())
                {
                    general.nomCarteGris = Donnees.m_donnees.TAB_JEU[0].S_NOM_CARTE_GRIS;
                }
                if (!Donnees.m_donnees.TAB_JEU[0].IsS_NOM_CARTE_TOPOGRAPHIQUENull())
                {
                    general.nomCarteTopographique = Donnees.m_donnees.TAB_JEU[0].S_NOM_CARTE_TOPOGRAPHIQUE;
                }                
                if (!Donnees.m_donnees.TAB_JEU[0].IsS_NOM_CARTE_ZOOMNull())
                {
                    general.nomCarteZoom = Donnees.m_donnees.TAB_JEU[0].S_NOM_CARTE_ZOOM;
                }
            }
            else
            {
                general.id_jeu = -1;
                general.objectifVictoire = Donnees.CST_OBJECTIF_DEMORALISATION;
            }
            nomCarte = general.nomCarte;
            nomCarteGris=general.nomCarteGris;
            nomCarteZoom=general.nomCarteZoom;

            if (Donnees.m_donnees.TAB_PARTIE.Count > 0)
            {
                if (!Donnees.m_donnees.TAB_PARTIE[0].IsS_NOMNull())
                {
                    general.nomPartie = Donnees.m_donnees.TAB_PARTIE[0].S_NOM;
                }
                if (!Donnees.m_donnees.TAB_PARTIE[0].IsS_HOST_COURRIELNull())
                {
                    general.hostMessagerie = Donnees.m_donnees.TAB_PARTIE[0].S_HOST_COURRIEL;
                }
                if (!Donnees.m_donnees.TAB_PARTIE[0].IsS_HOST_MOTDEPASSENull())
                {
                    general.motDePasseMessagerie = Donnees.m_donnees.TAB_PARTIE[0].S_HOST_MOTDEPASSE;
                }
                if (!Donnees.m_donnees.TAB_PARTIE[0].IsS_HOST_UTILISATEURNull())
                {
                    general.utilisateurMessagerie = Donnees.m_donnees.TAB_PARTIE[0].S_HOST_UTILISATEUR;
                }
                if (Donnees.m_donnees.TAB_PARTIE[0].IsID_METEO_INITIALENull())
                {
                    general.meteo = 0;
                }
                else
                {
                    general.meteo = Donnees.m_donnees.TAB_PARTIE[0].ID_METEO_INITIALE;
                }
                if (!Donnees.m_donnees.TAB_PARTIE[0].IsID_VICTOIRENull())
                {
                    idVictoire = Donnees.m_donnees.TAB_PARTIE[0].ID_VICTOIRE;
                }
                if (!Donnees.m_donnees.TAB_PARTIE[0].IsID_METEONull())
                {
                    idMeteo = Donnees.m_donnees.TAB_PARTIE[0].ID_METEO;
                }
                general.id_partie = Donnees.m_donnees.TAB_PARTIE[0].ID_PARTIE;
                general.largeurCarteZoomWeb = Donnees.m_donnees.TAB_PARTIE[0].I_LARGEUR_CARTE_ZOOM_WEB;
                general.hauteurCarteZoomWeb = Donnees.m_donnees.TAB_PARTIE[0].I_HAUTEUR_CARTE_ZOOM_WEB;
                fl_demmarage = Donnees.m_donnees.TAB_PARTIE[0].FL_DEMARRAGE;
                iTour = Donnees.m_donnees.TAB_PARTIE[0].I_TOUR;
                iPhase = Donnees.m_donnees.TAB_PARTIE[0].I_PHASE;
                messageArbitre = Donnees.m_donnees.TAB_PARTIE[0].S_MESSAGE_ARBITRE;
            }
            else
            {
                general.id_partie = -1;
            }

            if (DialogResult.OK == general.ShowDialog())
            {
                m_modification = true;
                //mise à jour des données saisies
                Donnees.m_donnees.TAB_JEU.Clear();
                Donnees.TAB_JEURow ligneJeu = Donnees.m_donnees.TAB_JEU.NewTAB_JEURow();
                ligneJeu.ID_JEU = general.id_jeu;
                ligneJeu.S_NOM_CARTE_HISTORIQUE = nomCarte;
                ligneJeu.S_NOM_CARTE_GRIS = nomCarteGris;
                ligneJeu.S_NOM_CARTE_ZOOM = nomCarteZoom;
                ligneJeu.I_LARGEUR_CARTE = general.largeurCarte;
                ligneJeu.I_HAUTEUR_CARTE = general.hauteurCarte;

                //si les images de carte à changer, il faut les mettre à jour
                if (nomCarte != general.nomCarte)
                {
                    ligneJeu.S_NOM_CARTE_HISTORIQUE = general.nomCarte.Substring(general.nomCarte.LastIndexOf('\\') + 1);
                    repertoireDest = Constantes.repertoireDonnees + ligneJeu.S_NOM_CARTE_HISTORIQUE;

                    if (general.nomCarte != repertoireDest)
                    {
                        // Cela ne doit être fait que pour la carte topographique
                        //if (null != ImageCarte.Image)
                        //{
                        //    //on relache l'image courante pour que l'on puisse, eventuellement, la modifier dans la form Carte
                        //    ImageCarte.Image.Dispose();
                        //}

                        //destruction d'un eventuel même fichier avec le même nom
                        if (File.Exists(repertoireDest))
                        {
                            File.Delete(repertoireDest);
                        }
                        //on recopie le fichier image vers le repertoire applicatif des cartes
                        File.Copy(general.nomCarte, repertoireDest, true);
                        //ImageCarte.Image = (Bitmap)Image.FromFile(repertoireDest);, voir note ci-dessus
                    }
                }

                if (nomCarteZoom != general.nomCarteZoom)
                {
                    ligneJeu.S_NOM_CARTE_ZOOM = general.nomCarteZoom.Substring(general.nomCarteZoom.LastIndexOf('\\') + 1);
                    repertoireDest = Constantes.repertoireDonnees + ligneJeu.S_NOM_CARTE_ZOOM;
                    if (repertoireDest != general.nomCarteZoom)
                    {
                        //destruction d'un eventuel même fichier avec le même nom
                        if (File.Exists(repertoireDest))
                        {
                            File.Delete(repertoireDest);
                        }
                        //on recopie le fichier image vers le repertoire applicatif des cartes
                        File.Copy(general.nomCarteZoom, repertoireDest, true);
                    }
                }

                if (nomCarteGris != general.nomCarteGris)
                {
                    ligneJeu.S_NOM_CARTE_GRIS = general.nomCarteGris.Substring(general.nomCarteGris.LastIndexOf('\\') + 1);
                    repertoireDest = Constantes.repertoireDonnees + ligneJeu.S_NOM_CARTE_GRIS;
                    if (repertoireDest != general.nomCarteGris)
                    {
                        //destruction d'un eventuel même fichier avec le même nom
                        if (File.Exists(repertoireDest))
                        {
                            File.Delete(repertoireDest);
                        }
                        //on recopie le fichier image vers le repertoire applicatif des cartes
                        File.Copy(general.nomCarteGris, repertoireDest, true);
                    }
                }

                ligneJeu.S_NOM_CARTE_TOPOGRAPHIQUE = general.nomCarteTopographique;
                ligneJeu.DT_INITIALE = general.dateInitiale;
                ligneJeu.I_NOMBRE_TOURS = general.nbTours;
                ligneJeu.I_NOMBRE_PHASES = general.nbPhases;
                ligneJeu.S_NOM = general.nomScenario;
                ligneJeu.I_LEVER_DU_SOLEIL = general.leverDuSoleil;
                ligneJeu.I_COUCHER_DU_SOLEIL = general.coucherDuSoleil;
                ligneJeu.I_ECHELLE = general.echelle;
                ligneJeu.ID_MODELE_TERRAIN_DEPLOIEMENT = general.ID_modele_terrain_deploiement;
                ligneJeu.I_COUT_DE_BASE=general.coutDeBase ;
                ligneJeu.I_RAYON_BATAILLE = general.rayonDeBataille;
                ligneJeu.I_HEURE_INITIALE = general.heureInitiale;
                ligneJeu.I_OBJECTIF = general.objectifVictoire;
                Donnees.m_donnees.TAB_JEU.AddTAB_JEURow(ligneJeu);

                Donnees.m_donnees.TAB_PARTIE.Clear();
                Donnees.TAB_PARTIERow lignePartie = Donnees.m_donnees.TAB_PARTIE.NewTAB_PARTIERow();
                lignePartie.ID_JEU = general.id_jeu;
                lignePartie.ID_PARTIE = general.id_partie;

                lignePartie.ID_METEO_INITIALE = general.meteo;
                lignePartie.S_NOM = general.nomPartie;
                lignePartie.S_HOST_COURRIEL = general.hostMessagerie;
                lignePartie.S_HOST_MOTDEPASSE = general.motDePasseMessagerie;
                lignePartie.S_HOST_UTILISATEUR = general.utilisateurMessagerie;
                lignePartie.I_LARGEUR_CARTE_ZOOM_WEB = general.largeurCarteZoomWeb;
                lignePartie.I_HAUTEUR_CARTE_ZOOM_WEB = general.hauteurCarteZoomWeb;
                lignePartie.FL_DEMARRAGE = fl_demmarage;
                lignePartie.S_MESSAGE_ARBITRE = messageArbitre;
                lignePartie.I_TOUR = iTour;
                lignePartie.I_PHASE = iPhase;
                if (idVictoire >= 0) 
                {
                    lignePartie.ID_VICTOIRE = idVictoire;
                }
                if (idMeteo >= 0)
                {
                    lignePartie.ID_METEO = idMeteo;
                }
                Donnees.m_donnees.TAB_PARTIE.AddTAB_PARTIERow(lignePartie);
            }
        }

        private void carteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormCarte fCarte = new FormCarte();

            //affectation des tables
            //fCarte.tableGraphisme = Donnees.m_donnees.TAB_GRAPHISME;
            //fCarte.tablePoint = Donnees.m_donnees.TAB_POINT;
            //fCarte.tableMeteo = Donnees.m_donnees.TAB_METEO;
            //fCarte.tableModelesMouvements = Donnees.m_donnees.TAB_MODELE_MOUVEMENT;
            fCarte.tableCase = Donnees.m_donnees.TAB_CASE;
            //fCarte.tableModelesTerrains = Donnees.m_donnees.TAB_MODELE_TERRAIN;//à mettre après graphisme et point
            //fCarte.imageCarte = (Bitmap)ImageCarte.Image;
            if (!Donnees.m_donnees.TAB_JEU[0].IsS_NOM_CARTE_TOPOGRAPHIQUENull())
            {
                fCarte.nomCarteTopographique = Donnees.m_donnees.TAB_JEU[0].S_NOM_CARTE_TOPOGRAPHIQUE;
            }

            if (DialogResult.OK == fCarte.ShowDialog())
            {
                m_modification = true;
                //mise à jour des modeles de terrain
                Donnees.m_donnees.TAB_CASE.Clear();
                Donnees.m_donnees.TAB_CASE.Merge(fCarte.tableCase, false);
            }
        }

        private void meteoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormMeteo fMeteo = new FormMeteo();

            fMeteo.tableMeteo = Donnees.m_donnees.TAB_METEO;
            if (DialogResult.OK == fMeteo.ShowDialog())
            {
                m_modification = true;
                Donnees.m_donnees.TAB_METEO.Clear();
                Donnees.m_donnees.TAB_METEO.Merge(fMeteo.tableMeteo, false);
            }
        }

        private void modelesDeMOuvementsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormModelesMouvements fModeleMouvement = new FormModelesMouvements();

            fModeleMouvement.tableModeleMouvement = Donnees.m_donnees.TAB_MODELE_MOUVEMENT;
            if (DialogResult.OK == fModeleMouvement.ShowDialog())
            {
                m_modification = true;
                Donnees.m_donnees.TAB_MODELE_MOUVEMENT.Clear();
                Donnees.m_donnees.TAB_MODELE_MOUVEMENT.Merge(fModeleMouvement.tableModeleMouvement, false);
            }
        }

        private void toolStripButtonZoomPlus_Click(object sender, EventArgs e)
        {
            if (null != ImageCarte.Image)
            {
                m_zoom *= 2;
                ImageCarte.Width = Convert.ToInt32(Donnees.m_donnees.TAB_JEU[0].I_LARGEUR_CARTE * m_zoom);
                ImageCarte.Height = Convert.ToInt32(Donnees.m_donnees.TAB_JEU[0].I_HAUTEUR_CARTE * m_zoom);
                ImageCarte.Invalidate();
            }
        }

        private void toolStripButtonZoomMoins_Click(object sender, EventArgs e)
        {
            if (null != ImageCarte.Image)
            {
                m_zoom /= 2;
                ImageCarte.Width = Convert.ToInt32(Donnees.m_donnees.TAB_JEU[0].I_LARGEUR_CARTE * m_zoom);
                ImageCarte.Height = Convert.ToInt32(Donnees.m_donnees.TAB_JEU[0].I_HAUTEUR_CARTE * m_zoom);
                ImageCarte.Invalidate();
            }

        }

        //private void ImageCarte_Paint(object sender, PaintEventArgs e)
        //{
        //    Random hasard = new Random();
        //    //Bitmap imageCarte;
        //    Color couleur;

        //    if (null != ImageCarte.Image)
        //    {

        //        couleur = Color.FromArgb(hasard.Next(256), hasard.Next(256), hasard.Next(256));

        //        //pour utiliser des méthodes avancées
        //        Pen stylo = new Pen(couleur, hasard.Next(5));

        //        Graphics graph = e.Graphics;
        //        graph.DrawLine(stylo, hasard.Next(ImageCarte.Image.Width),
        //            hasard.Next(ImageCarte.Image.Height),
        //            hasard.Next(ImageCarte.Image.Width),
        //            hasard.Next(ImageCarte.Image.Height));

        //        graph.DrawLine(stylo, -ImageCarte.Left, -ImageCarte.Top, ImageCarte.Image.Width, ImageCarte.Image.Height);
        //        Debug.WriteLine("left=" + ImageCarte.Left + " Top=" + ImageCarte.Top + " X=" + ImageCarte.Location.X + " Y=" + ImageCarte.Location.Y);
        //    }
        //}

        private void toolStripAfficherVilles_Click(object sender, EventArgs e)
        {
            ConstruireImageCarte();
        }

        private void toolStripButtonAffichierTopographie_Click(object sender, EventArgs e)
        {
            ConstruireImageCarte();
        }

        private void toolStripButtonTrajets_Click(object sender, EventArgs e)
        {
            ConstruireImageCarte();
        }

        /// <summary>
        /// reconstruit une image de carte à partir du fond et des données à afficher
        /// </summary>
        private void ConstruireImageCarte()
        {
            //rechargement du fond
            if (null != ImageCarte.Image)
            {
                ImageCarte.Image.Dispose();
                ImageCarte.Image = null;
            }
            if (0 == Donnees.m_donnees.TAB_JEU.Count)
            {
                //partie non crée, ou chargée, on ne peut rien afficher
                return;
            }
            if (toolStripAffichierTopographie.CheckState == CheckState.Checked)
            {
                if (!Donnees.m_donnees.TAB_JEU[0].IsS_NOM_CARTE_TOPOGRAPHIQUENull()
                    && Donnees.m_donnees.TAB_JEU[0].S_NOM_CARTE_TOPOGRAPHIQUE.Length > 0)
                {
                    ImageCarte.Image = Image.FromFile(Constantes.repertoireDonnees + Donnees.m_donnees.TAB_JEU[0].S_NOM_CARTE_TOPOGRAPHIQUE);
                }
            }
            else
            {
                if (!Donnees.m_donnees.TAB_JEU[0].IsS_NOM_CARTE_HISTORIQUENull() 
                    && Donnees.m_donnees.TAB_JEU[0].S_NOM_CARTE_HISTORIQUE.Length > 0)
                {
                    ImageCarte.Image = Image.FromFile(Constantes.repertoireDonnees + Donnees.m_donnees.TAB_JEU[0].S_NOM_CARTE_HISTORIQUE);
                }
            }
            if (null == ImageCarte.Image)
            {
                return;
            }
            ImageCarte.Width = (int)Math.Floor(Donnees.m_donnees.TAB_JEU[0].I_LARGEUR_CARTE * m_zoom);
            ImageCarte.Height = (int)Math.Floor(Donnees.m_donnees.TAB_JEU[0].I_HAUTEUR_CARTE * m_zoom);

            if ( toolStripAfficherBatailles.CheckState == CheckState.Unchecked)
            {
                Cartographie.AfficherBatailles((Bitmap)ImageCarte.Image);
            }
            if (toolStripAfficherUnites.CheckState == CheckState.Unchecked)
            {
                Cartographie.AfficherUnites((Bitmap)ImageCarte.Image);//ajout des unités
            }
            if (toolStripAfficherQG.CheckState == CheckState.Unchecked)
            {
                Cartographie.AfficherQG((Bitmap)ImageCarte.Image);//ajout des QG
            }
            if (toolStripAfficherVilles.CheckState == CheckState.Unchecked)
            {
                Cartographie.AfficherNoms((Bitmap)ImageCarte.Image);//ajout des noms de villes
            }

            if (this.toolStripButtonTrajets.CheckState == CheckState.Checked)
            {
                List<int> listeCases;// = new List<int>;
                List<Donnees.TAB_CASERow> cheminPCCTrajet = new List<Donnees.TAB_CASERow>();
                foreach (Donnees.TAB_PCC_VILLESRow lignePCCVille in Donnees.m_donnees.TAB_PCC_VILLES)
                {
                    cheminPCCTrajet.Clear();
                    Dal.ChargerTrajet(lignePCCVille.ID_TRAJET, Constantes.CST_TRAJET_VILLE, out listeCases);
                    cheminPCCTrajet.Capacity = listeCases.Count;
                    int j = 0;
                    while (j < listeCases.Count)
                    {
                        Donnees.TAB_CASERow ligneCase = Donnees.m_donnees.TAB_CASE.FindByID_CASE(listeCases[j++]);
                        ligneCase.I_COUT = lignePCCVille.ID_TRAJET;//pour debug, je met l'id de trajet dans le cout pour l'afficher
                        cheminPCCTrajet.Add(ligneCase);
                    }
                    Cartographie.AfficherChemin((Bitmap)ImageCarte.Image, cheminPCCTrajet, Color.Orange, 1);//chemin parcouru par une unité
                }                
            }

            if (toolStripPlusCourtChemin.CheckState == CheckState.Checked)
            {
                if (null!=m_cheminPCC && m_cheminPCC.Count > 0)
                {
                    Cartographie.AfficherChemin((Bitmap)ImageCarte.Image, m_cheminPCC, Color.Red, 5);//chemin parcouru par une unité
                }
                if (null != m_cheminHPA && m_cheminHPA.Count > 0)
                {
                    Cartographie.AfficherChemin((Bitmap)ImageCarte.Image, m_cheminHPA, Color.DeepSkyBlue,3 );//chemin parcouru par une unité
                }
                if (null != m_cheminVille && m_cheminVille.Count > 0)
                {
                    Cartographie.AfficherChemin((Bitmap)ImageCarte.Image, m_cheminVille, Color.Yellow, 1);//chemin parcouru par une unité
                }                
            }

            if (null != m_lignePionSelection)
            {
                if( null != m_cheminSelection && m_cheminSelection.Count > 0)
                {
                    Cartographie.AfficherChemin((Bitmap)ImageCarte.Image, m_cheminSelection, Color.DarkGreen, 2);//chemin parcouru par une unité
                    if (null != m_cheminVerifierTrajet && m_cheminVerifierTrajet.Count > 0)
                    {
                        Cartographie.AfficherChemin((Bitmap)ImageCarte.Image, m_cheminSelection, Color.LavenderBlush, 2);
                    }
                    Cartographie.AfficherArriveeDepart((Bitmap)ImageCarte.Image, m_cheminSelection[0], m_cheminSelection[m_cheminSelection.Count-1], Color.DarkGreen, 1);
                }
                else
                {
                    Cartographie.AfficherArriveeDepart((Bitmap)ImageCarte.Image,null,Donnees.m_donnees.TAB_CASE.FindByID_CASE(m_lignePionSelection.ID_CASE), Color.DarkGreen, 1);
                }
            }
            ImageCarte.Invalidate();
        }

        private void policeDeCaractèresToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormPolice fPolice = new FormPolice();

            fPolice.tablePolice = Donnees.m_donnees.TAB_POLICE;
            if (DialogResult.OK == fPolice.ShowDialog())
            {
                m_modification = true;
                Donnees.m_donnees.TAB_POLICE.Clear();
                Donnees.m_donnees.TAB_POLICE.Merge(fPolice.tablePolice, false);
            }
        }

        private void fondDeCarteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormFondDeCarte fFondDeCarte = new FormFondDeCarte();

            //initialisation des données de la form
            if (!Donnees.m_donnees.TAB_JEU[0].IsS_NOM_CARTE_TOPOGRAPHIQUENull()
                && Donnees.m_donnees.TAB_JEU[0].S_NOM_CARTE_TOPOGRAPHIQUE.Length > 0)
            {
                fFondDeCarte.largeur = Donnees.m_donnees.TAB_JEU[0].I_LARGEUR_CARTE;
                fFondDeCarte.hauteur = Donnees.m_donnees.TAB_JEU[0].I_HAUTEUR_CARTE;
                fFondDeCarte.nomCarteTopographique = Donnees.m_donnees.TAB_JEU[0].S_NOM_CARTE_TOPOGRAPHIQUE;
            }
            else
            {
                fFondDeCarte.largeur = 0;
                fFondDeCarte.hauteur = 0;
                fFondDeCarte.nomCarteTopographique = string.Empty;
            }

            fFondDeCarte.coutDeBase = Donnees.m_donnees.TAB_JEU[0].I_COUT_DE_BASE;

            //affectation des tables
            fFondDeCarte.tableGraphisme = Donnees.m_donnees.TAB_GRAPHISME;
            fFondDeCarte.tablePoint = Donnees.m_donnees.TAB_POINT;
            fFondDeCarte.tableMeteo = Donnees.m_donnees.TAB_METEO;
            fFondDeCarte.tableModelesMouvements = Donnees.m_donnees.TAB_MODELE_MOUVEMENT;
            fFondDeCarte.tableModelesTerrains = Donnees.m_donnees.TAB_MODELE_TERRAIN;//à mettre après graphisme et point
            fFondDeCarte.tableMouvementCout = Donnees.m_donnees.TAB_MOUVEMENT_COUT;//attention, à mettre en dernière affectation
            string nomCarteTopographique = fFondDeCarte.nomCarteTopographique.Substring(fFondDeCarte.nomCarteTopographique.LastIndexOf('\\') + 1);
            if (DialogResult.OK == fFondDeCarte.ShowDialog())
            {
                m_modification = true;
                //si l'image de carte à changer, il faut la mettre à jour
                if ((Donnees.m_donnees.TAB_JEU[0].IsS_NOM_CARTE_TOPOGRAPHIQUENull()
                    && !string.IsNullOrEmpty(fFondDeCarte.nomCarteTopographique))
                    || (!Donnees.m_donnees.TAB_JEU[0].IsS_NOM_CARTE_TOPOGRAPHIQUENull() &&
                    Donnees.m_donnees.TAB_JEU[0].S_NOM_CARTE_TOPOGRAPHIQUE != nomCarteTopographique))
                {
                    if (null != ImageCarte.Image)
                    {
                        //on relache l'image courante pour que l'on puisse, eventuellement, la modifier dans la form Carte
                        ImageCarte.Image.Dispose();
                    }

                    Donnees.m_donnees.TAB_JEU[0].S_NOM_CARTE_TOPOGRAPHIQUE = nomCarteTopographique;

                    string repertoireDest = WaocLib.Constantes.repertoireDonnees + Donnees.m_donnees.TAB_JEU[0].S_NOM_CARTE_TOPOGRAPHIQUE;
                    //destruction d'un eventuel même fichier avec le même nom
                    if (File.Exists(repertoireDest))
                    {
                        File.Delete(repertoireDest);
                    }
                    //on recopie le fichier image vers le repertoire applicatif des cartes
                    File.Copy(fFondDeCarte.nomCarteTopographique, repertoireDest, true);
                    ImageCarte.Image = (Bitmap)Image.FromFile(repertoireDest);

                    Donnees.m_donnees.TAB_JEU[0].I_LARGEUR_CARTE = fFondDeCarte.largeur;
                    Donnees.m_donnees.TAB_JEU[0].I_HAUTEUR_CARTE = fFondDeCarte.hauteur;
                }

                //mise à jour des points
                Donnees.m_donnees.TAB_POINT.Clear();
                Donnees.m_donnees.TAB_POINT.Merge(fFondDeCarte.tablePoint, false);

                //mise à jour des graphiques
                Donnees.m_donnees.TAB_GRAPHISME.Clear();
                Donnees.m_donnees.TAB_GRAPHISME.Merge(fFondDeCarte.tableGraphisme, false);

                //mise à jour des modeles de terrain
                Donnees.m_donnees.TAB_MODELE_TERRAIN.Clear();
                Donnees.m_donnees.TAB_MODELE_TERRAIN.Merge(fFondDeCarte.tableModelesTerrains, false);

                System.Collections.Specialized.ListDictionary tableIdentifiansModelesTerrains = fFondDeCarte.tableIdentifiansModelesTerrains;
                foreach (int clef in tableIdentifiansModelesTerrains.Keys)
                {
                    if ((int)tableIdentifiansModelesTerrains[clef] != clef)
                    {
                        //un identifiant d'origine (clef) a été modifié (valeur), il faut mettre à jour toutes les cases
                        foreach (Donnees.TAB_CASERow ligneCase in Donnees.m_donnees.TAB_CASE)
                        {
                            if (ligneCase.ID_MODELE_TERRAIN == clef)
                            {
                                ligneCase.ID_MODELE_TERRAIN = (int)tableIdentifiansModelesTerrains[clef];
                            }
                            if (ligneCase.ID_MODELE_TERRAIN_SI_OCCUPE == clef)
                            {
                                ligneCase.ID_MODELE_TERRAIN_SI_OCCUPE = (int)tableIdentifiansModelesTerrains[clef];
                            }
                        }
                    }
                }

                //mise à jour des couts de mouvement
                Donnees.m_donnees.TAB_MOUVEMENT_COUT.Clear();
                Donnees.m_donnees.TAB_MOUVEMENT_COUT.Merge(fFondDeCarte.tableMouvementCout, false);

                //divers
                Donnees.m_donnees.TAB_JEU[0].I_COUT_DE_BASE = fFondDeCarte.coutDeBase;
            }
            ConstruireImageCarte();
        }

        private void toolStripButtonPlusCourtChemin_Click(object sender, EventArgs e)
        {
            toolStripButtonPont.CheckState = CheckState.Unchecked;
            toolStripButtonGue.CheckState = CheckState.Unchecked;

            if (toolStripPlusCourtChemin.CheckState == CheckState.Unchecked)
            {
                panelTestPlusCourtChemin.Visible = false;
            }
            else
            {
                panelTestPlusCourtChemin.Visible = true;
                //panelTestPlusCourtChemin.Dock = DockStyle.Fill;

                //remise à zéro des valeurs de test
                labelDepartX.Text = "X:000";
                labelDepartY.Text = "Y:000";
                labelDepartIDCASE.Text = "ID_CASE:00000";
                labelDepartTerrain.Text = "Terrain:xxxxxxxxx";

                labelArriveeX.Text = "X:000";
                labelArriveeY.Text = "Y:000";
                labelArriveeIDCASE.Text = "ID_CASE:00000";
                labelArriveeTerrain.Text = "Terrain:xxxxxxxxx";
            }
        }

        private void saveasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == saveFileDialog.ShowDialog(this))
            {
                SauvegarderPartie(saveFileDialog.FileName);
                curFileName = saveFileDialog.FileName;
                Constantes.repertoireDonnees = curFileName;
                mruMenu.AddFile(curFileName);
                mruMenu.SaveToRegistry();
                m_modification = false;
                MiseAJourTitreFenetre();
            }
        }

        private void FormPrincipale_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (null != curFileName && 
                m_modification && 
                DialogResult.Yes == MessageBox.Show("Voulez vous faire une sauvegarde avant de quitter ?", "Sauvegarde", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                SauvegarderPartie(curFileName);
            }
        }

        private void FormPrincipale_Resize(object sender, EventArgs e)
        {
            ResizeSplitContainer();
        }

        private void FormPrincipale_Shown(object sender, EventArgs e)
        {
            ResizeSplitContainer();
        }

        /// <summary>
        /// modification de l'emplacement du split container car le "dock" n'arrive pas à le faire correctement
        /// </summary>
        private void ResizeSplitContainer()
        {
            splitContainer.Left = toolStripCarte.Left;
            splitContainer.Width = toolStripCarte.Width;
            splitContainer.Top = toolStripCarte.Bottom;
            splitContainer.Height = this.Bottom - toolStripCarte.Bottom - SystemInformation.HorizontalScrollBarHeight;
        }

        private void utilisateursToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormUtilisateur fUtilisateur = new FormUtilisateur();

            fUtilisateur.fichierCourant = curFileName;
            fUtilisateur.ShowDialog();
        }

        private void rolesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormRole fRole = new FormRole();

            fRole.fichierCourant = curFileName;
            fRole.tableRole = Donnees.m_donnees.TAB_ROLE;
            if (DialogResult.OK == fRole.ShowDialog())
            {
                m_modification = true;
                Donnees.m_donnees.TAB_ROLE.Clear();
                Donnees.m_donnees.TAB_ROLE.Merge(fRole.tableRole, false);
            }
        }

        private void aptitudesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormAptitude fAptitude = new FormAptitude();

            fAptitude.tableAptitudes = Donnees.m_donnees.TAB_APTITUDES;
            if (DialogResult.OK == fAptitude.ShowDialog())
            {
                m_modification = true;
                Donnees.m_donnees.TAB_APTITUDES.Clear();
                Donnees.m_donnees.TAB_APTITUDES.Merge(fAptitude.tableAptitudes, false);
            }
        }

        private void modelesDeCombatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormNation fNation = new FormNation();

            fNation.tableNation = Donnees.m_donnees.TAB_NATION;
            if (DialogResult.OK == fNation.ShowDialog())
            {
                m_modification = true;
                Donnees.m_donnees.TAB_NATION.Clear();
                Donnees.m_donnees.TAB_NATION.Merge(fNation.tableNation, false);
            }
        }

        private void modelesDePIonsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormModelesPions fModelePions = new FormModelesPions();

            fModelePions.tableModelesPions = Donnees.m_donnees.TAB_MODELE_PION;
            if (DialogResult.OK == fModelePions.ShowDialog())
            {
                m_modification = true;
                Donnees.m_donnees.TAB_MODELE_PION.Clear();
                Donnees.m_donnees.TAB_MODELE_PION.Merge(fModelePions.tableModelesPions, false);
            }
        }

        private void aptitudesModelesDePIonsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormAptitudesModelesPions fAptitudesModelePions = new FormAptitudesModelesPions();

            fAptitudesModelePions.tableAptitudesModelesPions = Donnees.m_donnees.TAB_APTITUDES_PION;
            if (DialogResult.OK == fAptitudesModelePions.ShowDialog())
            {
                m_modification = true;
                Donnees.m_donnees.TAB_APTITUDES_PION.Clear();
                Donnees.m_donnees.TAB_APTITUDES_PION.Merge(fAptitudesModelePions.tableAptitudesModelesPions, false);
            }
        }

        private void pionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormPion fPion = new FormPion();

            fPion.tablePions = Donnees.m_donnees.TAB_PION;
            if (DialogResult.OK == fPion.ShowDialog())
            {
                m_modification = true;

                //tous les pions qui ont changés de position sont "téléportés"
                Donnees.TAB_PIONDataTable tableForm = (Donnees.TAB_PIONDataTable) fPion.tablePions.Copy();
                foreach (Donnees.TAB_PIONRow lignePionForm in tableForm)
                {
                    Donnees.TAB_PIONRow lignePion = Donnees.m_donnees.TAB_PION.FindByID_PION(lignePionForm.ID_PION);
                    if (null == lignePion || lignePion.ID_CASE != lignePionForm.ID_CASE)
                    {
                        //pion nouvellement crée ou qui a changé de position
                        lignePionForm.B_TELEPORTATION = true;
                    }
                }
                Donnees.m_donnees.TAB_PION.Clear();
                Donnees.m_donnees.TAB_PION.Merge(tableForm, false);
            }
        }

        private void ordresToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormOrdre fOrdre = new FormOrdre();

            fOrdre.tableOrdre = Donnees.m_donnees.TAB_ORDRE;
            if (DialogResult.OK == fOrdre.ShowDialog())
            {
                m_modification = true;
                Donnees.m_donnees.TAB_ORDRE.Clear();
                Donnees.m_donnees.TAB_ORDRE.Merge(fOrdre.tableOrdre, false);
            }
        }

        private void libellesOrdresToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormLibelleOrdre fLibelleOrdre = new FormLibelleOrdre();

            fLibelleOrdre.tableLibelleOrdre = Donnees.m_donnees.TAB_ORDRE_LIBELLE;
            if (DialogResult.OK == fLibelleOrdre.ShowDialog())
            {
                m_modification = true;
                Donnees.m_donnees.TAB_ORDRE_LIBELLE.Clear();
                Donnees.m_donnees.TAB_ORDRE_LIBELLE.Merge(fLibelleOrdre.tableLibelleOrdre, false);
            }
        }

        private void MiseAJourInformationCase(Donnees.TAB_CASERow ligneCase)
        {
            Donnees.TAB_MODELE_TERRAINRow ligneModeleTerrain;
            Donnees.TAB_PIONRow lignePion;

            labelInformationX.Text = String.Format("X:{000:0}", ligneCase.I_X);
            labelInformationY.Text = String.Format("Y:{000:0}", ligneCase.I_Y);
            labelInformationIDCASE.Text = String.Format("ID_CASE:{0:00000}", ligneCase.ID_CASE);
            ligneModeleTerrain = Donnees.m_donnees.TAB_MODELE_TERRAIN.FindByID_MODELE_TERRAIN(ligneCase.ID_MODELE_TERRAIN);
            if (null != ligneModeleTerrain)
            {
                labelInformationTerrain.Text = String.Format("Terrain:{0}", ligneModeleTerrain.S_NOM);
            }
            else
            {
                labelInformationTerrain.Text = "Terrain:xxxxxxxxx";
            }
            labelCoutTerrain.Text = ligneCase.IsI_COUTNull() ? "Cout:?" : "Cout:" + ligneCase.I_COUT.ToString();
            if (ligneCase.EstInnocupe())
            {
                labelProprietaire.Text = "Propriétaire: ?";
            }
            else
            {
                lignePion = ligneCase.TrouvePionSurCase();
                labelProprietaire.Text = string.Format("Propriétaire: {0} ({1})", lignePion.S_NOM, lignePion.ID_PION);
            }
        }

        //public void AfficherTempsSafe()
        //{
        //    if (this.InvokeRequired)
        //    {
        //        EventArgs evt = new EventArgs();
        //        EvenementAfficherTempsHandler d = new EvenementAfficherTempsHandler(this, evt);
        //        this.Invoke(d, new object[] { this,evt });
        //    }
        //    else
        //    {
        //        AfficherTemps();
        //    }
        //}

        public void AfficherTemps()
        {
            TimeSpan tPasse = DateTime.Now.Subtract(m_dateDebut);
            long nbrestant = (long)(tPasse.TotalSeconds * (Donnees.m_donnees.TAB_JEU[0].I_NOMBRE_PHASES - (Donnees.m_donnees.TAB_PARTIE[0].I_PHASE + 1)) / (Donnees.m_donnees.TAB_PARTIE[0].I_PHASE + 1));

            labelPhase.Text = string.Format("{000:0}", Donnees.m_donnees.TAB_PARTIE[0].I_PHASE);
            if (tPasse.TotalHours >= 1)
            {
                labelInformationTempsPasse.Text = String.Format("{0} heures {1} minutes", Math.Floor(tPasse.TotalHours), tPasse.Minutes);
            }
            else
            {
                labelInformationTempsPasse.Text = (tPasse.TotalMinutes >= 1) ? String.Format("{0} minutes", tPasse.Minutes) : String.Format("{0} secondes", tPasse.Seconds); 
            }

            if (nbrestant > 3600)
            {
                labelInformationTempsRestant.Text = String.Format("{0} heures {1} minutes", nbrestant / 3600, (nbrestant - (nbrestant / 3600) * 3600) / 60);
            }
            else
            {
                if (nbrestant > 60)
                {
                    labelInformationTempsRestant.Text = String.Format("{0} minutes", nbrestant / 60);
                }
                else
                {
                    labelInformationTempsRestant.Text = String.Format("{0} secondes", nbrestant);
                }
            }
            ConstruireImageCarte();
        }
        
        private void heureSuivanteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_dateDebut = DateTime.Now;

            m_ancienCurseur = this.Cursor;
            // from file
            /*
            byte[] Embeded_Cursor_Resource = Path.Combine(Application.StartupPath, "tail2.ani");
             * */
            // from resouces   modification here is :   byte[] resource in the call
            byte[] Embeded_Cursor_Resource = Properties.Resources.tail2;  // the animate cursor desired
            this.Cursor = Dal.Create(Embeded_Cursor_Resource);

            //Pour toutes les phases d'une heure
            //DataSetCoutDonnees.m_donnees.TAB_PARTIE[0].I_PHASE=0; ne pas le faire à cause de la reprise possible sur fichier

            //La partie est-elle commencée ?
            if (!Donnees.m_donnees.TAB_PARTIE[0].FL_DEMARRAGE)
            {
                if (System.Windows.Forms.DialogResult.Yes == MessageBox.Show("La partie est actuellement en phase forum. Souhaitez vous que la partie débute pleinement maintenant ?", "Partie non commencée", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                Donnees.m_donnees.TAB_PARTIE[0].FL_DEMARRAGE = true;
            }

            //on commence les traitements
            heureSuivanteToolStripMenuItem.Enabled = false;
            backgroundTraitement.RunWorkerAsync(this.fichierCourant);
        }

        public string fichierCourant
        {
            get { return curFileName; }
        }

        private void backgroundTraitement_DoWork(object sender, DoWorkEventArgs e)
        {
            ClassTraitementHeure traitement = new ClassTraitementHeure();
            BackgroundWorker travailleur = sender as BackgroundWorker;

            //si la partie est terminée, on ne devrait pas appeler cette méthode !
            if (!Donnees.m_donnees.TAB_PARTIE[0].IsID_VICTOIRENull() && Donnees.m_donnees.TAB_PARTIE[0].ID_VICTOIRE >= 0)
            {
                MessageBox.Show("La partie est déjà terminée avec la victoire de la nation ID=" + Donnees.m_donnees.TAB_PARTIE[0].ID_VICTOIRE, "Partie terminée", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                e.Cancel = false;
                e.Result = "La partie est déjà terminée avec la victoire de la nation ID=" + Donnees.m_donnees.TAB_PARTIE[0].ID_VICTOIRE;
            }

            string messageErreur = string.Empty;
            e.Cancel = false;
            if (!traitement.TraitementHeure(fichierCourant, travailleur, out messageErreur))
            {
                e.Cancel = false;
                e.Result = messageErreur;
            }
        }

        private void backgroundTraitement_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            AfficherTemps();
        }

        private void backgroundTraitement_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            heureSuivanteToolStripMenuItem.Enabled = true;
            this.Cursor = m_ancienCurseur;
            if (null != e.Error)
            {
                MessageBox.Show("Erreur durant le traitement :" + e.Error.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (e.Cancelled)
            {
                MessageBox.Show("Traitement annulé:" + e.Result.ToString(), "Annulation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                MessageBox.Show("Traitement terminé avec succès", "Fin de traitement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //ConstruireImageCarte(); ne peut pas être appelé depuis un autre process
            }
        }

        private void toolStripAfficherUnites_Click(object sender, EventArgs e)
        {
            ConstruireImageCarte();
        }

        private void toolStripButtonAfficherBatailles_Click(object sender, EventArgs e)
        {
            ConstruireImageCarte();
        }

        private void phrasesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormPhrase fPhrase = new FormPhrase();

            fPhrase.tablePhrase = Donnees.m_donnees.TAB_PHRASE;
            if (DialogResult.OK == fPhrase.ShowDialog())
            {
                m_modification = true;
                Donnees.m_donnees.TAB_PHRASE.Clear();
                Donnees.m_donnees.TAB_PHRASE.Merge(fPhrase.tablePhrase, false);
            }
        }

        private void bataillesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormBatailles fBatailles = new FormBatailles();

            fBatailles.tableBataille = Donnees.m_donnees.TAB_BATAILLE;
            fBatailles.tableBataillePions = Donnees.m_donnees.TAB_BATAILLE_PIONS;
            if (DialogResult.OK == fBatailles.ShowDialog())
            {
                m_modification = true;
                Donnees.m_donnees.TAB_BATAILLE.Clear();
                Donnees.m_donnees.TAB_BATAILLE.Merge(fBatailles.tableBataille, false);

                Donnees.m_donnees.TAB_BATAILLE_PIONS.Clear();
                Donnees.m_donnees.TAB_BATAILLE_PIONS.Merge(fBatailles.tableBataillePions, false);
            }
        }

        private void mEssagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormMessage fMessages = new FormMessage();

            fMessages.tableMessage = Donnees.m_donnees.TAB_MESSAGE;
            if (DialogResult.OK == fMessages.ShowDialog())
            {
                m_modification = true;
                Donnees.m_donnees.TAB_MESSAGE.Clear();
                Donnees.m_donnees.TAB_MESSAGE.Merge(fMessages.tableMessage, false);
            }

        }

        private void testsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormTests fTests = new FormTests();
            fTests.ShowDialog();
        }

        private void outilsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormOutils fOutils = new FormOutils();
            fOutils.ShowDialog(this);
        }

        private void creationInternetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClassTraitementWeb web = new ClassTraitementWeb(fichierCourant);
            if (!web.GenerationWeb())
            {
                MessageBox.Show("Erreur durant la génération des fichiers Web. Consultez le fichier de log", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show("Génération des fichiers Web terminée sans erreur", "Génération Web", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void notificationAuxJoueursToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormMessageArbitre formMessageArbitre = new FormMessageArbitre();
            formMessageArbitre.textBoxMessage.Text = Donnees.m_donnees.TAB_PARTIE[0].S_MESSAGE_ARBITRE;
            formMessageArbitre.fichierCourant = this.curFileName;
            if (formMessageArbitre.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Donnees.m_donnees.TAB_PARTIE[0].S_MESSAGE_ARBITRE = formMessageArbitre.textBoxMessage.Text;
                ClassNotificationJoueurs notification = new ClassNotificationJoueurs(fichierCourant);
                if (!notification.NotificationJoueurs())
                {
                    MessageBox.Show("Erreur durant les notifications", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("Notifications envoyées sans erreur", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }            
        }

        private void renfortsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormRenfort fRenfort = new FormRenfort();

            fRenfort.tableRenfort = Donnees.m_donnees.TAB_RENFORT;
            if (DialogResult.OK == fRenfort.ShowDialog())
            {
                m_modification = true;
                Donnees.m_donnees.TAB_RENFORT.Clear();
                Donnees.m_donnees.TAB_RENFORT.Merge(fRenfort.tableRenfort, false);
            }

        }

        private void miseÀJourInternetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClassTraitementHeure traitement = new ClassTraitementHeure();

            string messageErreur = string.Empty;
            if (!traitement.miseÀJourInternet(fichierCourant, out messageErreur))
            {
                MessageBox.Show("Erreur durant la mise à jour du Web. Consultez le fichier de log", "miseÀJourInternet", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show("Génération des mises à jour Web terminée sans erreur", "miseÀJourInternet", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            m_modification = true;
        }

        private void buttonCaseID_Click(object sender, EventArgs e)
        {
            if (null == Donnees.m_donnees)
            {
                MessageBox.Show("Aucun fichier chargé", "buttonCaseID_Click", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Donnees.TAB_CASERow ligneCase = Donnees.m_donnees.TAB_CASE.FindByID_CASE(Convert.ToInt32(this.textBoxCaseID.Text));
            if (null == ligneCase)
            {
                MessageBox.Show("ID inconnu sur la carte", "buttonCaseID_Click", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            MiseAJourInformationCase(ligneCase);
        }

        private void comboBoxListeUnites_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxListeUnites.SelectedIndex != -1)
            {
                m_lignePionSelection = (Donnees.TAB_PIONRow)comboBoxListeUnites.SelectedItem;
                Donnees.TAB_CASERow ligneCase = Donnees.m_donnees.TAB_CASE.FindByID_CASE(m_lignePionSelection.ID_CASE);
                labelInformationX.Text = ligneCase.I_X.ToString();
                labelInformationY.Text = ligneCase.I_Y.ToString();
                labelInformationIDCASE.Text = String.Format("ID_CASE:{0:00000}", ligneCase.ID_CASE);

                //recherche du trajet de l'unité en mémoire s'il existe pour l'affichage
                if (null != m_cheminSelection) { m_cheminSelection.Clear(); }
                string messageErreur;
                this.buttonRecalculTrajet.Enabled = false;
                this.buttonVerifierTrajet.Enabled = false;
                Donnees.TAB_ORDRERow ligneOrdre = Donnees.m_donnees.TAB_ORDRE.Mouvement(m_lignePionSelection.ID_PION);
                if (null != ligneOrdre)
                {
                    Donnees.TAB_CASERow ligneCaseDestination = Donnees.m_donnees.TAB_CASE.FindByID_CASE(ligneOrdre.ID_CASE_DESTINATION);
                    Donnees.TAB_CASERow ligneCaseDepart = (m_lignePionSelection.I_INFANTERIE > 0 || m_lignePionSelection.I_CAVALERIE > 0 || m_lignePionSelection.I_ARTILLERIE > 0) ? Donnees.m_donnees.TAB_CASE.FindByID_CASE(ligneOrdre.ID_CASE_DEPART) : Donnees.m_donnees.TAB_CASE.FindByID_CASE(m_lignePionSelection.ID_CASE);
                    AstarTerrain[] tableCoutsMouvementsTerrain;
                    double cout, coutHorsRoute;

                    if (Cartographie.RechercheChemin(Cartographie.typeParcours.MOUVEMENT,
                        m_lignePionSelection, ligneCaseDepart, ligneCaseDestination, ligneOrdre, out m_cheminSelection, out cout, out coutHorsRoute, out tableCoutsMouvementsTerrain, out messageErreur))
                    {
                        this.buttonVerifierTrajet.Enabled = true;
                        this.buttonRecalculTrajet.Enabled = true;
                    }
                }
                ConstruireImageCarte();
            }
            else
            {
                m_lignePionSelection = null;
                this.buttonRecalculTrajet.Enabled = false;
                this.buttonVerifierTrajet.Enabled = false;
            }
        }

        private void hPAStarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormHPA fHPA = new FormHPA();

            //affectation des tables
            //fHPA.tableCaseBlocs = Donnees.m_donnees.TAB_PCC_CASE_BLOCS;
            fHPA.tailleBloc = Donnees.m_donnees.TAB_JEU[0].IsI_TAILLEBLOC_PCCNull() ? 20 : Donnees.m_donnees.TAB_JEU[0].I_TAILLEBLOC_PCC;
            //fHPA.tablePCCCouts = Donnees.m_donnees.TAB_PCC_COUTS;
            //fHPA.tablePCCTrajet = Donnees.m_donnees.TAB_PCC_TRAJET;
            fHPA.fichierCourant = this.fichierCourant;

            fHPA.ShowDialog();
            m_modification = true;
            Donnees.m_donnees.TAB_JEU[0].I_TAILLEBLOC_PCC = fHPA.tailleBloc;
                //mise à jour des modeles de terrain
                //Donnees.m_donnees.TAB_PCC_CASE_BLOCS.Clear();
                //Donnees.m_donnees.TAB_PCC_CASE_BLOCS.Merge(fHPA.tableCaseBlocs, false);
                //Donnees.m_donnees.TAB_PCC_COUTS.Merge(fHPA.tablePCCCouts, false);
                //Donnees.m_donnees.TAB_PCC_TRAJET.Merge(fHPA.tablePCCTrajet, false);

        }

        private void PCCVillestoolStrip_Click(object sender, EventArgs e)
        {
            FormPCCNoms fPCCNoms = new FormPCCNoms();

            //affectation des tables
            //fHPA.tableCaseBlocs = Donnees.m_donnees.TAB_PCC_CASE_BLOCS;
            fPCCNoms.tailleVille = Donnees.m_donnees.TAB_JEU[0].IsI_ZONEVILLE_PCCNull() ? 3 : Donnees.m_donnees.TAB_JEU[0].I_ZONEVILLE_PCC;
            fPCCNoms.distanceVilleMax = Donnees.m_donnees.TAB_JEU[0].IsI_DISTANCEVILLEMAX_PCCNull() ? 50 : Donnees.m_donnees.TAB_JEU[0].I_DISTANCEVILLEMAX_PCC;
            //fHPA.tablePCCCouts = Donnees.m_donnees.TAB_PCC_COUTS;
            //fHPA.tablePCCTrajet = Donnees.m_donnees.TAB_PCC_TRAJET;
            fPCCNoms.fichierCourant = this.fichierCourant;

            fPCCNoms.ShowDialog();
            m_modification = true;
            Donnees.m_donnees.TAB_JEU[0].I_ZONEVILLE_PCC = fPCCNoms.tailleVille;
            Donnees.m_donnees.TAB_JEU[0].I_DISTANCEVILLEMAX_PCC = fPCCNoms.distanceVilleMax;
            //mise à jour des modeles de terrain
            //Donnees.m_donnees.TAB_PCC_CASE_BLOCS.Clear();
            //Donnees.m_donnees.TAB_PCC_CASE_BLOCS.Merge(fHPA.tableCaseBlocs, false);
            //Donnees.m_donnees.TAB_PCC_COUTS.Merge(fHPA.tablePCCCouts, false);
            //Donnees.m_donnees.TAB_PCC_TRAJET.Merge(fHPA.tablePCCTrajet, false);

            Debug.WriteLine("TAB_PCC_VILLES #=" + Donnees.m_donnees.TAB_PCC_VILLES.Count);
            foreach (Donnees.TAB_PCC_VILLESRow lignePccVille in Donnees.m_donnees.TAB_PCC_VILLES)
            {
                if (lignePccVille.I_COUT < 0)
                {
                    Donnees.TAB_NOMS_CARTERow lVDebug1 = Donnees.m_donnees.TAB_NOMS_CARTE.FindByID_NOM(lignePccVille.ID_VILLE_DEBUT);
                    Donnees.TAB_NOMS_CARTERow lVDebug2 = Donnees.m_donnees.TAB_NOMS_CARTE.FindByID_NOM(lignePccVille.ID_VILLE_FIN);
                    Donnees.TAB_CASERow lDebug1 = Donnees.m_donnees.TAB_CASE.FindByID_CASE(lVDebug1.ID_CASE);
                    Donnees.TAB_CASERow lDebug2 = Donnees.m_donnees.TAB_CASE.FindByID_CASE(lVDebug2.ID_CASE);
                    Debug.WriteLine(string.Format("trajet toujours impossible {0}:{1}({2},{3}) -> {4}:{5}({6},{7}), cout={8}",
                        lVDebug1.S_NOM,
                        lDebug1.ID_CASE,
                        lDebug1.I_X,
                        lDebug1.I_Y,
                        lVDebug2.S_NOM,
                        lDebug2.ID_CASE,
                        lDebug2.I_X,
                        lDebug2.I_Y,
                        lignePccVille.I_COUT
                        ));
                }
            }

        }

        private void initilisationPartieToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == MessageBox.Show("Cette action va supprimer toutes les tables temporaires, les messages, les messages et patrouilles, les renforts et remettre les effectifs des unités. N'oubliez pas de remettre, à la main, la position initiale des troupes.",
                "Reinitialisation de la partie", MessageBoxButtons.OKCancel))
            {
                Cursor oldcurseur = this.Cursor;
                this.Cursor = Cursors.WaitCursor;
                Donnees.m_donnees.TAB_BATAILLE.Clear();
                Donnees.m_donnees.TAB_BATAILLE_PIONS.Clear();
                Donnees.m_donnees.TAB_LOG.Clear();
                Donnees.m_donnees.TAB_MESSAGE.Clear();
                Donnees.m_donnees.TAB_ORDRE.Clear();
                Donnees.m_donnees.TAB_PARCOURS.Clear();
                Donnees.m_donnees.TAB_ESPACE.Clear();
                //suppression des patrouilles et des messages
                int i = 0;
                while (i < Donnees.m_donnees.TAB_PION.Count)
                {
                    Donnees.TAB_PIONRow lignePion = Donnees.m_donnees.TAB_PION[i];
                    if (lignePion.estMessager || lignePion.estPatrouille)
                    {
                        lignePion.Delete();
                    }
                    else
                    {
                        i++;
                    }
                }

                //suppression des unités arrivées en renfort (les lignes de renforts restant inchangés)
                i = 0;
                while (i < Donnees.m_donnees.TAB_PION.Count)
                {
                    Donnees.TAB_PIONRow lignePion = Donnees.m_donnees.TAB_PION[i];
                    if (null!=Donnees.m_donnees.TAB_RENFORT.FindByID_PION(lignePion.ID_PION))
                    {
                        lignePion.Delete();
                    }
                    else
                    {
                        i++;
                    }
                }

                //remise à zero des autres unités
                foreach (Donnees.TAB_PIONRow lignePion in Donnees.m_donnees.TAB_PION)
                {
                    lignePion.I_INFANTERIE = lignePion.I_INFANTERIE_INITIALE;
                    lignePion.I_CAVALERIE = lignePion.I_CAVALERIE_INITIALE;
                    lignePion.I_ARTILLERIE = lignePion.I_ARTILLERIE_INITIALE;
                    lignePion.I_MORAL = lignePion.I_MORAL_MAX;
                    lignePion.I_FATIGUE = 0;
                    lignePion.I_DISTANCE_A_PARCOURIR = 0;
                    lignePion.I_NB_HEURES_COMBAT = 0;
                    lignePion.I_NB_PHASES_MARCHE_JOUR = 0;
                    lignePion.I_NB_PHASES_MARCHE_NUIT = 0;
                    lignePion.I_TOUR_FUITE_RESTANT = 0;
                    lignePion.I_TOUR_SANS_RAVITAILLEMENT = 0;
                    lignePion.SetI_ZONE_BATAILLENull();
                    lignePion.SetID_BATAILLENull();
                    lignePion.B_DETRUIT = false;
                    lignePion.B_FUITE_AU_COMBAT = false;
                    lignePion.B_INTERCEPTION = false;
                }
                //remise à "blanc" du placement de toutes les unités sur la carte
                foreach (Donnees.TAB_CASERow ligneCase in Donnees.m_donnees.TAB_CASE)
                {
                    ligneCase.SetID_PROPRIETAIRENull();
                    ligneCase.SetID_NOUVEAU_PROPRIETAIRENull();
                }

                //Suppression de tous les trajets crées suite à la destruction ou à la construction d'un pont
                foreach (Donnees.TAB_PCC_VILLESRow lignePCCVille in Donnees.m_donnees.TAB_PCC_VILLES)
                {
                    if (lignePCCVille.B_CREATION) 
                    {
                        Dal.SupprimerTrajet(lignePCCVille.ID_TRAJET, Constantes.CST_TRAJET_VILLE);
                        lignePCCVille.Delete(); 
                    }
                }
                //on déplace en table tous les trajets d'origine remplacé par de nouveaux suite à la destruction ou à la construction d'un pont
                foreach (Donnees.TAB_PCC_VILLES_SUPPRIMERow lignePCCVilleSupprime in Donnees.m_donnees.TAB_PCC_VILLES_SUPPRIME)
                {
                    Donnees.m_donnees.TAB_PCC_VILLES.AddTAB_PCC_VILLESRow(
                        lignePCCVilleSupprime.ID_VILLE_DEBUT,
                        lignePCCVilleSupprime.ID_VILLE_FIN,
                        lignePCCVilleSupprime.I_COUT,
                        lignePCCVilleSupprime.ID_TRAJET,
                        lignePCCVilleSupprime.I_HORSROUTE,
                        Donnees.CST_TRAJET_INOCCUPE,
                        lignePCCVilleSupprime.B_CREATION);
                }

                //elements généraux
                Donnees.m_donnees.TAB_PARTIE[0].I_TOUR = 0;
                Donnees.m_donnees.TAB_PARTIE[0].I_PHASE = 0;
                Donnees.m_donnees.TAB_PARTIE[0].ID_METEO = Donnees.m_donnees.TAB_PARTIE[0].ID_METEO_INITIALE;
                Donnees.m_donnees.TAB_PARTIE[0].SetID_VICTOIRENull();
                Donnees.m_donnees.TAB_PARTIE[0].FL_DEMARRAGE = false;
                MiseAJourTitreFenetre();
                this.Cursor = oldcurseur;
            }
        }

        private void toolStripButtonPont_Click(object sender, EventArgs e)
        {
            toolStripPlusCourtChemin.CheckState = CheckState.Unchecked;
            toolStripButtonGue.CheckState = CheckState.Unchecked;
        }

        private void toolStripButtonGue_Click(object sender, EventArgs e)
        {
            toolStripPlusCourtChemin.CheckState = CheckState.Unchecked;
            toolStripButtonPont.CheckState = CheckState.Unchecked;
        }

        /// <summary>
        /// Affichage du point sur lequel se trouve la souris
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImageCarte_MouseMove(object sender, MouseEventArgs e)
        {
            Donnees.TAB_CASERow ligneCase;
            int clicX = (int)Math.Round(e.X / m_zoom, 0);
            int clicY = (int)Math.Round(e.Y / m_zoom, 0);

            ligneCase = Donnees.m_donnees.TAB_CASE.FindByXY(clicX, clicY);
            if (null != ligneCase)
            {
                MiseAJourInformationCase(ligneCase);
            }
            else
            {
                labelInformationX.Text = "X:000";
                labelInformationY.Text = "Y:000";
                labelInformationIDCASE.Text = "ID_CASE:00000";
                labelInformationTerrain.Text = "Terrain:xxxxxxxxx";
                labelCoutTerrain.Text = "Cout:?";
            }
        }

        private void ImageCarte_MouseClick(object sender, MouseEventArgs e)
        {
            Donnees.TAB_MODELE_TERRAINRow ligneModeleTerrain;
            int clicX = (int)Math.Floor(e.X / m_zoom);
            int clicY = (int)Math.Floor(e.Y / m_zoom);
            DateTime timeStart;
            TimeSpan perf;
            Donnees.TAB_NOMS_CARTERow ligneNom = null;
            Donnees.TAB_PIONRow lignePion = Donnees.m_donnees.TAB_PION[0];

            //int id_case_destination;
            //ClassMessager.ZoneGeographiqueVersCase(lignePion, 2, ClassMessager.COMPAS.CST_SUD, 120, out id_case_destination);
            //return;

            #region test plus court chemin
            if (toolStripPlusCourtChemin.CheckState == CheckState.Checked)
            {
                if (MouseButtons.Left == e.Button)
                {
                    m_departPlusCourtChemin = Donnees.m_donnees.TAB_CASE.FindByXY((int)Math.Round(e.X / m_zoom, 0), (int)Math.Round(e.Y / m_zoom, 0));
                    //m_departPlusCourtChemin = Donnees.m_donnees.TAB_CASE.FindByXY(622, 577);
                    //m_departPlusCourtChemin = Donnees.m_donnees.TAB_CASE.FindByID_CASE(4373903);
                    labelDepartX.Text = Convert.ToString(m_departPlusCourtChemin.I_X);
                    labelDepartY.Text = Convert.ToString(m_departPlusCourtChemin.I_Y);
                    labelDepartIDCASE.Text = Convert.ToString(m_departPlusCourtChemin.ID_CASE);
                    ligneModeleTerrain = Donnees.m_donnees.TAB_MODELE_TERRAIN.FindByID_MODELE_TERRAIN(m_departPlusCourtChemin.ID_MODELE_TERRAIN);
                    labelDepartTerrain.Text = ligneModeleTerrain.S_NOM;
                }
                else
                {
                    m_arriveePlusCourtChemin = Donnees.m_donnees.TAB_CASE.FindByXY((int)Math.Round(e.X / m_zoom, 0), (int)Math.Round(e.Y / m_zoom, 0));
                    //m_arriveePlusCourtChemin = Donnees.m_donnees.TAB_CASE.FindByID_CASE(4606596);
                    //m_arriveePlusCourtChemin = Donnees.m_donnees.TAB_CASE.FindByXY(661, 560);
                    //ligneNom = Donnees.m_donnees.TAB_NOMS_CARTE.FindByID_NOM(120);
                    labelArriveeX.Text = Convert.ToString(m_arriveePlusCourtChemin.I_X);
                    labelArriveeY.Text = Convert.ToString(m_arriveePlusCourtChemin.I_Y);
                    labelArriveeIDCASE.Text = Convert.ToString(m_arriveePlusCourtChemin.ID_CASE);
                    ligneModeleTerrain = Donnees.m_donnees.TAB_MODELE_TERRAIN.FindByID_MODELE_TERRAIN(m_arriveePlusCourtChemin.ID_MODELE_TERRAIN);
                    labelArriveeTerrain.Text = ligneModeleTerrain.S_NOM;
                }

                if (null != m_departPlusCourtChemin && null != m_arriveePlusCourtChemin)
                {
                    //recherche de la météo
                    if (0 == Donnees.m_donnees.TAB_METEO.Count)
                    {
                        MessageBox.Show("Il n'y a aucune météo de définie", "AStar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    //recherche du modèle de mouvement
                    if (0 == Donnees.m_donnees.TAB_MODELE_MOUVEMENT.Count)
                    {
                        MessageBox.Show("Il n'y a aucune modèle de mouvement de défini", "AStar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    //recherche du plus court chemin
                    labelInformationTempsPasse.Text = string.Empty;
                    AstarTerrain[] tableCoutsMouvementsTerrain;
                    ClassTraitementHeure traitementtest = new ClassTraitementHeure();
                    Cartographie.CalculModeleMouvementsPion(out tableCoutsMouvementsTerrain);
                    
                    timeStart = DateTime.Now;
                    if (!m_etoile.SearchPath(m_departPlusCourtChemin, m_arriveePlusCourtChemin, tableCoutsMouvementsTerrain))
                    {
                        MessageBox.Show("Il n'y a aucun chemin possible entre les deux points", "AStar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    perf = DateTime.Now - timeStart;
                    labelInformationTempsPasse.Text = string.Format("{0} min {1} sec {2} mil", perf.Minutes, perf.Seconds, perf.Milliseconds);
                    m_cheminPCC = m_etoile.PathByNodes;

                    //maintenant on compare avec la version HPA
                    if (null == m_etoileHPA) m_etoileHPA = new AStar();
                    timeStart = DateTime.Now;
                    if (!m_etoileHPA.SearchPath(m_departPlusCourtChemin, m_arriveePlusCourtChemin, tableCoutsMouvementsTerrain))
                    {
                        MessageBox.Show("Il n'y a aucun chemin possible entre les deux points", "AStarHPA", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        perf = DateTime.Now - timeStart;
                        labelInformationTempsPasse.Text += string.Format("\r\n HPA : {0} min {1} sec {2} mil", perf.Minutes, perf.Seconds, perf.Milliseconds);
                        m_cheminHPA = m_etoileHPA.PathByNodes;
                    }

                    //maintenant on compare avec la version Ville
                    Debug.WriteLine("TAB_PCC_VILLES #=" + Donnees.m_donnees.TAB_PCC_VILLES.Count);
                    foreach (Donnees.TAB_PCC_VILLESRow lignePccVille in Donnees.m_donnees.TAB_PCC_VILLES)
                    {
                        if (lignePccVille.I_COUT < 0)
                        {
                            Donnees.TAB_NOMS_CARTERow lVDebug1 = Donnees.m_donnees.TAB_NOMS_CARTE.FindByID_NOM(lignePccVille.ID_VILLE_DEBUT);
                            Donnees.TAB_NOMS_CARTERow lVDebug2 = Donnees.m_donnees.TAB_NOMS_CARTE.FindByID_NOM(lignePccVille.ID_VILLE_FIN);
                            Donnees.TAB_CASERow lDebug1 = Donnees.m_donnees.TAB_CASE.FindByID_CASE(lVDebug1.ID_CASE);
                            Donnees.TAB_CASERow lDebug2 = Donnees.m_donnees.TAB_CASE.FindByID_CASE(lVDebug2.ID_CASE);
                            Debug.WriteLine(string.Format("trajet impossible de {0}:{1}({2},{3}) vers {4}:{5}({6},{7}), cout={8}",
                                lVDebug1.S_NOM,
                                lDebug1.ID_CASE,
                                lDebug1.I_X,
                                lDebug1.I_Y,
                                lVDebug2.S_NOM,
                                lDebug2.ID_CASE,
                                lDebug2.I_X,
                                lDebug2.I_Y,
                                lignePccVille.I_COUT
                                ));
                        }
                    }
                    
                    timeStart = DateTime.Now;
                    Cartographie.InitialiserProprietairesTrajets();
                    //List<Donnees.TAB_CASERow> chemin;
                    //int cout, coutHorsRoute;
                    //string messageErreur;
                    //if (!Cartographie.RechercheChemin(Cartographie.typeParcours.MOUVEMENT, lignePion, m_departPlusCourtChemin, m_arriveePlusCourtChemin, null, out chemin, out cout, out coutHorsRoute, out tableCoutsMouvementsTerrain, out messageErreur))
                    if (!m_etoile.SearchPathVilles(m_departPlusCourtChemin, ligneNom, m_arriveePlusCourtChemin, 1, tableCoutsMouvementsTerrain))
                    {
                        MessageBox.Show("Il n'y a aucun chemin possible entre les deux points", "AStarVille", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        perf = DateTime.Now - timeStart;
                        labelInformationTempsPasse.Text += string.Format("\r\n Villes : {0} min {1} sec {2} mil", perf.Minutes, perf.Seconds, perf.Milliseconds);
                        labelInformationTempsPasse.Text += string.Format("\r\n Villes : {0}/{1} hors route", m_etoile.HorsRouteGlobal, m_etoile.CoutGlobal);
                        m_cheminVille = m_etoile.PathByNodes;
                        //m_cheminVille = chemin;
                    }
                    ConstruireImageCarte();
                }
            }
            #endregion

            #region ajout, suppression, modifications de noms sur la carte
            if (toolStripPlusCourtChemin.CheckState == CheckState.Unchecked
                && toolStripButtonPont.CheckState == CheckState.Unchecked
                && toolStripButtonGue.CheckState == CheckState.Unchecked)
            {
                //ajout, suppression, modifications de noms sur la carte
                FormNomCarte fNomCarte = new FormNomCarte();
                fNomCarte.X = clicX;
                fNomCarte.Y = clicY;
                fNomCarte.tablePolice = Donnees.m_donnees.TAB_POLICE;
                fNomCarte.id_nom = Constantes.CST_IDNULL;
                int id_case = Constantes.CST_IDNULL;
                int i = 0;
                Donnees.TAB_CASERow ligneCase;
                Donnees.TAB_NOMS_CARTERow ligneNomCarte;

                //recherche d'un nom de carte déjà existant à proximité
                while (i < Donnees.m_donnees.TAB_NOMS_CARTE.Rows.Count && Constantes.CST_IDNULL == id_case)
                {
                    ligneCase = Donnees.m_donnees.TAB_CASE.FindByID_CASE(Donnees.m_donnees.TAB_NOMS_CARTE[i].ID_CASE);
                    if (null != ligneCase)
                    {
                        if (Constantes.Distance(clicX, clicY, ligneCase.I_X, ligneCase.I_Y) < Properties.Settings.Default.distanceRechercheNom)
                        {
                            id_case = ligneCase.ID_CASE;
                            fNomCarte.id_nom = Donnees.m_donnees.TAB_NOMS_CARTE[i].ID_NOM;
                            fNomCarte.victoire = Donnees.m_donnees.TAB_NOMS_CARTE[i].I_VICTOIRE;
                            fNomCarte.id_police = Donnees.m_donnees.TAB_NOMS_CARTE[i].ID_POLICE;
                            //fNomCarte.nom = Donnees.m_donnees.TAB_NOMS_CARTE[i].S_NOM; recherche faite dans la FormNomCarte
                            fNomCarte.position = Donnees.m_donnees.TAB_NOMS_CARTE[i].I_POSITION;
                        }
                    }

                    i++;
                }

                //s'il n'y en pas, on prend la case du clic
                if (Constantes.CST_IDNULL == id_case)
                {
                    ligneCase = Donnees.m_donnees.TAB_CASE.FindByXY(clicX, clicY);
                    if (null != ligneCase)
                    {
                        id_case = ligneCase.ID_CASE;
                    }
                    else
                    {
                        MessageBox.Show("Vous avez cliquer en dehors de la carte", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                fNomCarte.idcase = id_case;
                if (DialogResult.OK == fNomCarte.ShowDialog(this))
                {
                    m_modification = true;

                    if (fNomCarte.suppression)
                    {
                        //suppression du nom existant
                        ligneNomCarte = Donnees.m_donnees.TAB_NOMS_CARTE.FindByID_NOM(fNomCarte.id_nom);
                        Donnees.m_donnees.TAB_NOMS_CARTE.RemoveTAB_NOMS_CARTERow(ligneNomCarte);
                    }
                    else
                    {
                        if (fNomCarte.id_nom != Constantes.CST_IDNULL)
                        {
                            //modification
                            ligneNomCarte = Donnees.m_donnees.TAB_NOMS_CARTE.FindByID_NOM(fNomCarte.id_nom);
                            ligneNomCarte.ID_POLICE = fNomCarte.id_police;
                            ligneNomCarte.S_NOM = fNomCarte.nom;
                            ligneNomCarte.I_POSITION = fNomCarte.position;
                            ligneNomCarte.I_VICTOIRE = fNomCarte.victoire;
                        }
                        else
                        {
                            //ajout du nouveau nom
                            ligneNomCarte = Donnees.m_donnees.TAB_NOMS_CARTE.AddTAB_NOMS_CARTERow(
                                id_case,
                                fNomCarte.id_police,
                                fNomCarte.nom,
                                fNomCarte.position,
                                fNomCarte.victoire,
                                -1);
                            ligneNomCarte.SetID_NATION_CONTROLENull();
                        }
                    }
                    ConstruireImageCarte();
                }
            }
            #endregion

            #region Test construction de pont
            if (toolStripButtonPont.CheckState == CheckState.Checked)
            {
                Donnees.TAB_CASERow ligneCase = Donnees.m_donnees.TAB_CASE.FindByXY((int)Math.Round(e.X / m_zoom, 0), (int)Math.Round(e.Y / m_zoom, 0));
                Donnees.TAB_CASERow ligneCaseGue = Cartographie.RecherchePontouGue(ligneCase, false);
                if (null == ligneCaseGue)
                {
                    MessageBox.Show("Il n'y a aucun gué à proximité", "Construction de pont", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }

            }
            #endregion

            #region Test destruction de pont
            if (toolStripButtonGue.CheckState == CheckState.Checked)
            {
                Donnees.TAB_CASERow ligneCase = Donnees.m_donnees.TAB_CASE.FindByXY((int)Math.Round(e.X / m_zoom, 0), (int)Math.Round(e.Y / m_zoom, 0));
                Donnees.TAB_CASERow ligneCasePont = Cartographie.RecherchePontouGue(ligneCase, true);
                if (null == ligneCasePont)
                {
                    MessageBox.Show("Il n'y a aucun pont à proximité", "Destruction de pont", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
                if (!Cartographie.DetruirePont(ligneCasePont))
                {
                    MessageBox.Show("Erreur lors de la destruction", "Destruction de pont", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                MessageBox.Show("Pont détruit", "Destruction de pont", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            #endregion

            #region Test construction de pont
            if (toolStripButtonPont.CheckState == CheckState.Checked)
            {
                Donnees.TAB_CASERow ligneCase = Donnees.m_donnees.TAB_CASE.FindByXY((int)Math.Round(e.X / m_zoom, 0), (int)Math.Round(e.Y / m_zoom, 0));
                Donnees.TAB_CASERow ligneCasePont = Cartographie.RecherchePontouGue(ligneCase,false);
                if (null == ligneCasePont)
                {
                    MessageBox.Show("Il n'y a aucun gué à proximité", "Construction de pont", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
                if (!Cartographie.ConstruirePont(ligneCasePont))
                {
                    MessageBox.Show("Erreur lors de la construction", "Construction de pont", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                MessageBox.Show("Pont construit", "Construction de pont", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            #endregion
        }

        private void buttonVerifierTrajet_Click(object sender, EventArgs e)
        {
            AstarTerrain[] tableCoutsMouvementsTerrain;
            Donnees.TAB_ORDRERow ligneOrdre = Donnees.m_donnees.TAB_ORDRE.Mouvement(m_lignePionSelection.ID_PION);
            Donnees.TAB_CASERow ligneCaseDestination = Donnees.m_donnees.TAB_CASE.FindByID_CASE(ligneOrdre.ID_CASE_DESTINATION);
            Donnees.TAB_CASERow ligneCaseDepart = (m_lignePionSelection.I_INFANTERIE > 0 || m_lignePionSelection.I_CAVALERIE > 0 || m_lignePionSelection.I_ARTILLERIE > 0) ? Donnees.m_donnees.TAB_CASE.FindByID_CASE(ligneOrdre.ID_CASE_DEPART) : Donnees.m_donnees.TAB_CASE.FindByID_CASE(m_lignePionSelection.ID_CASE);
            Donnees.TAB_NOMS_CARTERow ligneNom = null;

            string requete = string.Format("ID_VILLE_DEBUT=26 AND ID_VILLE_FIN=50");
            Donnees.TAB_PCC_VILLESRow[] lignesCout = (Donnees.TAB_PCC_VILLESRow[])Donnees.m_donnees.TAB_PCC_VILLES.Select(requete);
            requete = string.Format("ID_VILLE_DEBUT=50 AND ID_VILLE_FIN=26");
            Donnees.TAB_PCC_VILLESRow[] lignesCout2 = (Donnees.TAB_PCC_VILLESRow[])Donnees.m_donnees.TAB_PCC_VILLES.Select(requete);
            
            if (!ligneOrdre.IsID_NOM_DESTINATIONNull())
            {
                ligneNom = Donnees.m_donnees.TAB_NOMS_CARTE.FindByID_NOM(ligneOrdre.ID_NOM_DESTINATION);
            }
            Cartographie.CalculModeleMouvementsPion(out tableCoutsMouvementsTerrain);
            if (!m_etoile.SearchPathVilles(ligneCaseDepart, ligneNom, ligneCaseDestination, 1, tableCoutsMouvementsTerrain))
            {
                MessageBox.Show("Il n'y a aucun chemin possible entre les deux points", "buttonVerifierTrajet_Click", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                m_cheminVerifierTrajet = m_etoile.PathByNodes;
            }
            ConstruireImageCarte();
        }

        private void buttonRecalculTrajet_Click(object sender, EventArgs e)
        {
            string messageErreur;
            Donnees.TAB_ORDRERow ligneOrdre = Donnees.m_donnees.TAB_ORDRE.Mouvement(m_lignePionSelection.ID_PION);
            Donnees.TAB_CASERow ligneCaseDestination = Donnees.m_donnees.TAB_CASE.FindByID_CASE(ligneOrdre.ID_CASE_DESTINATION);
            Donnees.TAB_CASERow ligneCaseDepart = (m_lignePionSelection.effectifTotal > 0) ? Donnees.m_donnees.TAB_CASE.FindByID_CASE(ligneOrdre.ID_CASE_DEPART) : Donnees.m_donnees.TAB_CASE.FindByID_CASE(m_lignePionSelection.ID_CASE);
            Donnees.TAB_NATIONRow ligneNation = Donnees.m_donnees.TAB_PION.TrouveNation(m_lignePionSelection);
            Donnees.TAB_NOMS_CARTERow ligneNom=null;
            AstarTerrain[] tableCoutsMouvementsTerrain;
            double cout, coutHorsRoute;
            int avancementPion;

            if (!ligneOrdre.IsID_NOM_DESTINATIONNull())
            {
                ligneNom = Donnees.m_donnees.TAB_NOMS_CARTE.FindByID_NOM(ligneOrdre.ID_NOM_DESTINATION);
            }

            //quel quantité du trajet l'unité avait-elle parcouru ?
            if (m_lignePionSelection.effectifTotal > 0)
            {
                avancementPion = Cartographie.CalculPionPositionRelativeAvancement(m_lignePionSelection, ligneOrdre, ligneNation, out m_cheminSelection);
            }
            else
            {
                avancementPion = Cartographie.AvancementPourRecalcul(Cartographie.typeParcours.MOUVEMENT, m_lignePionSelection, ligneCaseDepart, ligneCaseDestination, ligneOrdre, out messageErreur);
            }

            if (avancementPion < 0)
            {
                MessageBox.Show("Erreur dans le calcul de l'avancement.", "buttonRecalculTrajet_Click", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //l'unité change d'emplacement de facto
            Donnees.m_donnees.TAB_ESPACE.SupprimerEspacePion(m_lignePionSelection.ID_PION);
            Donnees.m_donnees.TAB_PARCOURS.SupprimerParcoursPion(m_lignePionSelection.ID_PION);
            //Recalcul du nouveau trajet
            if (!Cartographie.RechercheChemin(Cartographie.typeParcours.MOUVEMENT,
                m_lignePionSelection, Donnees.m_donnees.TAB_CASE.FindByID_CASE(ligneOrdre.ID_CASE_DEPART), ligneCaseDestination, ligneOrdre,
                out m_cheminSelection, out cout, out coutHorsRoute, out tableCoutsMouvementsTerrain, out messageErreur))
            {
                MessageBox.Show("Erreur dans RechercheChemin.", "buttonRecalculTrajet_Click", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (m_lignePionSelection.effectifTotal > 0)
            {
                //il faut repositionner l'unité suivant son avancement
                if (avancementPion >= m_cheminSelection.Count)
                {
                    //l'unité devrait déjà être arrivée
                    m_lignePionSelection.ID_CASE = m_cheminSelection.Count - 1;
                    ligneOrdre.I_EFFECTIF_DEPART = 0;
                    ligneOrdre.I_EFFECTIF_DESTINATION = m_lignePionSelection.effectifTotal;
                }
                else
                {
                    // On repositionne l'unité comme si elle était en bivouac, il serait complexe de faire un calcul relatif aux troupes arrivées ou pas suivant la nouvelle longueur du chemin 
                    // par rapport à sa destination si celle-ci est plus courte que la précédente
                    Cartographie.PlacerPionEnBivouac(m_lignePionSelection, ligneOrdre, ligneNation);
                }
            }
            else
            {
                if (avancementPion >= m_cheminSelection.Count)
                {
                    m_lignePionSelection.ID_CASE = m_cheminSelection.Count - 1;
                }
                else
                {
                    m_lignePionSelection.ID_CASE = m_cheminSelection[avancementPion].ID_CASE;
                }
            }
            MessageBox.Show("Recalcul du trajet effectué.", "buttonRecalculTrajet_Click", MessageBoxButtons.OK, MessageBoxIcon.Information);

            ConstruireImageCarte();
        }

    }   
}