using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using WaocLib;

namespace vaoc
{
    public partial class FormPrincipale : Form
    {
        private readonly uint fPreviousExecutionState;//pour emp�cher la mise en veille

        //protected DataSetCoutDonnees DataSetCoutDonnees.m_donnees; mis en static dans BaseVaoc
        protected decimal m_zoom;//niveau de zoom sur l'image
        protected LigneCASE m_departPlusCourtChemin;
        protected LigneCASE m_arriveePlusCourtChemin;
        protected AStar m_etoileHPA;
        protected DateTime m_dateDebut;
        protected bool m_modification;
        protected List<Donnees.TAB_CASERow> m_cheminPCC;
        protected List<LigneCASE> m_cheminHPA;
        protected List<Donnees.TAB_CASERow> m_cheminVille;
        protected List<Donnees.TAB_CASERow> m_cheminHorsRoute;
        protected List<LigneCASE> m_cheminSelection;
        protected List<LigneCASE> m_cheminVerifierTrajet;
        protected Cursor m_ancienCurseur;
        protected Donnees.TAB_PIONRow m_lignePionSelection;
        protected int m_nbBatailles;

        #region gestion du fichier le plus r�cent
        protected MruStripMenuInline mruMenu;
        protected string curFileName;
        #endregion

        public FormPrincipale()
        {
            InitializeComponent();
            // Set new state to prevent system sleep
            fPreviousExecutionState = NativeMethods.SetThreadExecutionState(NativeMethods.ES_CONTINUOUS | NativeMethods.ES_SYSTEM_REQUIRED);
            
            Donnees.m_donnees = new Donnees();
            m_zoom = 1;
            m_departPlusCourtChemin = null;
            m_arriveePlusCourtChemin = null;
            m_modification = false;
            m_lignePionSelection = null;
            buttonRecalculTrajet.Enabled = false;
            TestsDePerformance();
            #region gestion du fichier le plus r�cent
            //RegistryKey regKey = Registry.CurrentUser.OpenSubKey(mruRegKey);
            //if (regKey != null)
            //{
            //    menuClearRegistryOnExit.Checked = (int)regKey.GetValue("delSubkey", 1) != 0;
            //    regKey.Close();
            //}

            mruMenu = new MruStripMenuInline(fichierToolStripMenuItem, fichiersRecentsToolStripMenuItem, new MruStripMenu.ClickedHandler(OnMruFile), Constantes.CST_CLEFDEREGISTRE + "\\MRU");

            #endregion
        }

        #region gestion du fichier le plus r�cent
        private void OnMruFile(int number, String filename)
        {
            InitialiserDonnees();
            if (ChargementPartie(filename))
            {
                mruMenu.SetFirstFile(number);
            }
            else
            {
                mruMenu.RemoveFile(number);
            }
        }

        private void MiseAJourTitreFenetre()
        {
            if (Donnees.m_donnees.TAB_JEU.Count > 0 && Donnees.m_donnees.TAB_PARTIE.Count > 0)
            {
                this.Text = String.Format("VAOC - {0} {1} {2} tour:{3} phase:{4}",
                Donnees.m_donnees.TAB_JEU[0].S_NOM,
                Donnees.m_donnees.TAB_PARTIE[0].S_NOM,
                ClassMessager.DateHeure(false),
                Donnees.m_donnees.TAB_PARTIE[0].I_TOUR,
                Donnees.m_donnees.TAB_PARTIE[0].I_PHASE);
            }
        }

        #endregion

        private bool ChargementPartie(string filename)
        {
            long memoireAvant, memoireApres;

            memoireAvant = GC.GetTotalMemory(false);
            Constantes.repertoireDonnees = filename;
            if (Donnees.m_donnees.ChargerPartie(filename))
            {
                memoireApres = GC.GetTotalMemory(false);//taille m�moire utilis�e par le garbage collector en octets
                //int tailleMemoire = Marshal.SizeOf(Donnees.m_donnees); ->ne marche pas
                //Debug.WriteLine("taille des donn�es 1er methode=" + tailleMemoire);
                Debug.WriteLine("taille des donn�es 2eme methode ko=" + (memoireApres - memoireAvant)/1024);

                // Grosse optimisation pour les d�placements en mode HPA
                Donnees.m_donnees.TAB_PCC_COUTS.Initialisation();

                curFileName = filename;
                m_modification = false;

                if (Donnees.m_donnees.TAB_JEU.Count > 0 && !Donnees.m_donnees.TAB_JEU[0].IsS_NOM_CARTE_TOPOGRAPHIQUENull()
                    && Donnees.m_donnees.TAB_JEU[0].S_NOM_CARTE_TOPOGRAPHIQUE.Length > 0)
                {
                    //chargement du fichier graphique de la carte
                    ConstruireImageCarte();
                }
                MiseAJourTitreFenetre();
                MiseAJourListeUnites();
                Correctifs();
            }
            else
            {
                return false;
            }
            return true;
        }

        private void MiseAJourListeUnites()
        {
            //mise � jour de la liste de selection des unit�s
            comboBoxListeUnites.Items.Clear();

            if (null != this.ImageCarte.Image)
            {
                //TestsDePerformance();
                Donnees.m_donnees.TAB_CASE.InitialisationListeCase(this.ImageCarte.Image.Width, this.ImageCarte.Image.Height);//optimisation m�moire
                Donnees.m_donnees.TAB_CASE.InitialisationListeCaseNonCoutMax();//optimisation de performance pour AStar.SearchSpace
            }

            IEnumerable<Donnees.TAB_PIONRow> requete =
            from pion in Donnees.m_donnees.TAB_PION
            orderby pion.ID_PION
            select pion;

            foreach (Donnees.TAB_PIONRow lignePion in requete)
            {
                comboBoxListeUnites.Items.Add(lignePion);
            }
        }

        private static void Correctifs()
        {
            #region Generation des chaines videos
            //string NomQG(string nom)
            //{
            //    //le dernier mot
            //    int pos = Math.Max(nom.LastIndexOf(' '), nom.LastIndexOf('\'')) + 1;
            //    return nom.Substring(pos, nom.Length - pos);
            //}
            //string ChaineFichier(string source)
            //{
            //    byte[] tempBytes;
            //    tempBytes = System.Text.Encoding.GetEncoding("ISO-8859-8").GetBytes(source);
            //    string asciiStr = System.Text.Encoding.UTF8.GetString(tempBytes);
            //    return asciiStr.Replace(" ", "_").Replace("'", "").ToUpper();
            //}

            //List<String> listeRole = new List<String>();
            //for (int j = 0; j < Donnees.m_donnees.TAB_VIDEO.Count; j++)
            //{
            //    Donnees.TAB_VIDEORow ligneVideo = Donnees.m_donnees.TAB_VIDEO[j];
            //    if (ligneVideo.ID_PION < 0 || ligneVideo.ID_CASE < 0)
            //    {
            //        continue; //case comptant seulement pour les points de victoire
            //    }
            //    if (ligneVideo.B_QG)// || lignePion.estQG)
            //    {
            //        if (!listeRole.Contains(NomQG(ligneVideo.S_NOM)))
            //        {
            //            listeRole.Add(NomQG(ligneVideo.S_NOM));
            //        }
            //    }                
            //}
            //foreach (string role in listeRole)
            //{
            //    String nom = ChaineFichier(Constantes.MinusculeSansAccents(role).Replace(" ", "_").Replace("'", "_"));
            //    Debug.WriteLine(string.Format("ffmpeg -framerate 1 -i {0}_%04d.png -c:v libx264 -r 30 -pix_fmt yuv420p {0}.mp4", nom));
            //}
            #endregion

            #region retrait du rattachement pour toutes les unit�s � l'hopital ou en prison et qui n'ont plus d'effectifs
            //foreach (Donnees.TAB_PIONRow lignePion in Donnees.m_donnees.TAB_PION)
            //{
            //    if (!lignePion.IsID_LIEU_RATTACHEMENTNull() && lignePion.I_CAVALERIE == 0 && lignePion.I_INFANTERIE == 0)
            //    {
            //        lignePion.SetID_LIEU_RATTACHEMENTNull();//pour �viter de conserver des lignes devenues totalement inutiles
            //    }
            //}
            #endregion

            #region suppression des unit�s rattach�s � von bulow sans effectifs
            //foreach (Donnees.TAB_PIONRow lignePion in Donnees.m_donnees.TAB_PION)
            //{
            //    if (lignePion.ID_PION_PROPRIETAIRE==220 && lignePion.effectifTotal==0 && !lignePion.estQG && !lignePion.estMessager && !lignePion.estDepot && !lignePion.estConvoi && !lignePion.estPontonnier)
            //    {
            //        lignePion.DetruirePion();
            //        Donnees.TAB_MESSAGERow ligneMessage = Donnees.m_donnees.TAB_MESSAGE.DernierMessageEmis(lignePion.ID_PION);
            //        if (null!= ligneMessage)
            //        {
            //            ligneMessage.B_DETRUIT = true;
            //        }
            //    }
            //}
            #endregion

            #region mise � zero de fatigue n�gatives
            foreach (Donnees.TAB_PIONRow lignePion in Donnees.m_donnees.TAB_PION)
            {
                if (lignePion.I_FATIGUE<0)
                {
                    lignePion.I_FATIGUE = 0;
                }
            }
            #endregion


            #region Recherche puis suppression d'espaces ou des parcours pions appartenant � des pions detruits
            List<int> listePions = new List<int>();
            foreach(Donnees.TAB_ESPACERow ligneEspace in Donnees.m_donnees.TAB_ESPACE.Rows)
            {
                if (!listePions.Contains(ligneEspace.ID_PION))
                {
                    Donnees.TAB_PIONRow lignePion = Donnees.m_donnees.TAB_PION.FindByID_PION(ligneEspace.ID_PION);
                    if (null != lignePion && lignePion.B_DETRUIT)
                    {
                        listePions.Add(ligneEspace.ID_PION);
                    }
                }
            }

            foreach (Donnees.TAB_PARCOURSRow ligneParcours in Donnees.m_donnees.TAB_PARCOURS.Rows)
            {
                if (!listePions.Contains(ligneParcours.ID_PION))
                {
                    Donnees.TAB_PIONRow lignePion = Donnees.m_donnees.TAB_PION.FindByID_PION(ligneParcours.ID_PION);
                    if (null!= lignePion && lignePion.B_DETRUIT)
                    {
                        listePions.Add(ligneParcours.ID_PION);
                    }
                }
            }
            
            foreach(int ID_PION in listePions)
            {
                Donnees.m_donnees.TAB_ESPACE.SupprimerEspacePion(ID_PION);
                Donnees.m_donnees.TAB_PARCOURS.SupprimerParcoursPion(ID_PION);
            }
            #endregion

            #region dernier message de pions d�truits, doit �tre indiqu� comme tel
            Donnees.TAB_MESSAGERow[] listeMessage = (Donnees.TAB_MESSAGERow[])Donnees.m_donnees.TAB_MESSAGE.Select("I_TOUR_ARRIVEE IS NOT NULL");
            foreach (Donnees.TAB_MESSAGERow ligneMessage in listeMessage)
            {
                Donnees.TAB_PIONRow lignePion = Donnees.m_donnees.TAB_PION.FindByID_PION(ligneMessage.ID_PION_EMETTEUR);
                if (lignePion.B_DETRUIT && !ligneMessage.B_DETRUIT)
                {
                    ligneMessage.B_DETRUIT = true;
                }
            }
            #endregion

            #region correction sur le tipe video de l'unit�
            //MiseAJourDonneesHistorique maj = new MiseAJourDonneesHistorique();
            //foreach(Donnees.TAB_VIDEORow ligneVideo in Donnees.m_donnees.TAB_VIDEO)
            //{
            //    ligneVideo.I_TYPE = (int)maj.tipeVideo(ligneVideo);
            //}
            //int t = 0;
            #endregion

            #region messagers qui n'ont plus d'ordres...
            foreach (Donnees.TAB_PIONRow lignePion in Donnees.m_donnees.TAB_PION)
            {
                if (!lignePion.B_DETRUIT && lignePion.estMessager)
                {
                    int i = 0;
                    while (i < Donnees.m_donnees.TAB_ORDRE.Count)
                    {
                        if (Donnees.m_donnees.TAB_ORDRE[i].ID_PION == lignePion.ID_PION) break;
                        i++;
                    }
                    if (i >= Donnees.m_donnees.TAB_ORDRE.Count)
                    {
                        Debug.WriteLine("suppression du messager " + lignePion.ID_PION + ":" + lignePion.S_NOM);
                        lignePion.B_DETRUIT = true;
                    }
                }
            }
            //int t = 0;
            #endregion

            #region Reprise d'un ordre ancien en ordre courant 
            //ClassTraitementHeure traitement = new ClassTraitementHeure();
            //Donnees.TAB_ORDRE_ANCIENRow ligneOrdreAncien = Donnees.m_donnees.TAB_ORDRE_ANCIEN.FindByID_ORDRE(51951);
            //ligneOrdreAncien.I_TOUR_FIN = Constantes.NULLENTIER;
            //ligneOrdreAncien.I_PHASE_FIN = Constantes.NULLENTIER;
            //traitement.DeplacerOrdreVersCourant(ligneOrdreAncien);

            //ChargementInitial();
            //unites dans des hopitaux sans tours de blessures
            //foreach (Donnees.TAB_PIONRow lignePion in Donnees.m_donnees.TAB_PION)
            //{
            //    if (lignePion.B_BLESSES && lignePion.ID_LIEU_RATTACHEMENT>=0 && lignePion.I_TOUR_BLESSURE<0)
            //    {
            //        if (lignePion.I_TOUR_CONVOI_CREE<0)
            //        {
            //            lignePion.I_TOUR_BLESSURE = Donnees.m_donnees.TAB_PARTIE[0].I_TOUR;
            //        }
            //        else
            //        {
            //            lignePion.I_TOUR_BLESSURE = lignePion.I_TOUR_CONVOI_CREE + 24;
            //        }
            //    }
            //}
            //int t = 0;
            #endregion

            #region messages non arriv�s sur des pions d�truits !!!
            Donnees.TAB_MESSAGERow[] listeMessageD = (Donnees.TAB_MESSAGERow[])Donnees.m_donnees.TAB_MESSAGE.Select("I_TOUR_ARRIVEE IS NULL");
            foreach (Donnees.TAB_MESSAGERow ligneMessage in listeMessageD)
            {
                Donnees.TAB_PIONRow lignePion = Donnees.m_donnees.TAB_PION.FindByID_PION(ligneMessage.ID_PION_PROPRIETAIRE);
                if (null != lignePion && lignePion.B_DETRUIT)
                {
                    Donnees.TAB_ORDRERow ligneOrdre = Donnees.m_donnees.TAB_ORDRE.Premier(lignePion.ID_PION);
                    if (null == ligneOrdre)
                    {
                        ligneMessage.I_TOUR_ARRIVEE = ligneMessage.I_TOUR_DEPART;
                        ligneMessage.I_PHASE_ARRIVEE = ligneMessage.I_PHASE_DEPART;
                    }
                    else
                    {
                        ligneMessage.I_TOUR_ARRIVEE = ligneOrdre.I_TOUR_FIN;
                        ligneMessage.I_PHASE_ARRIVEE = ligneOrdre.I_PHASE_FIN;
                    }
                }
            }
            #endregion

            #region les bless�s ne sont pas marqu�s comme tel...
            //foreach (Donnees.TAB_PIONRow lignePion in Donnees.m_donnees.TAB_PION)
            //{
            //    if (lignePion.S_NOM.StartsWith("Bless"))
            //    {
            //        lignePion.B_BLESSES = true;
            //    }
            //}
            //foreach (Donnees.TAB_PION_ANCIENRow lignePion in Donnees.m_donnees.TAB_PION_ANCIEN)
            //{
            //    if (lignePion.S_NOM.StartsWith("Bless"))
            //    {
            //        lignePion.B_BLESSES = true;
            //    }
            //}

            #endregion
            //Donnees.TAB_PIONRow lignePionBatailleAjout = Donnees.m_donnees.TAB_PION.FindByID_PION(8461);
            //bool test = lignePionBatailleAjout.estCombattifQG(true, true);
            // test = lignePionBatailleAjout.estCombattifQG(true, true);
            // test = lignePionBatailleAjout.estCombattifQG(true, true);
            // test = lignePionBatailleAjout.estCombattifQG(true, true);
            // test = lignePionBatailleAjout.estCombattifQG(true, true);
            // test = lignePionBatailleAjout.estCombattifQG(true, true);


            //test sur l'ordre actif
            /*
            Donnees.TAB_PIONRow lignePionA=Donnees.m_donnees.TAB_PION.FindByID_PION(21);
            Donnees.TAB_ORDRERow ligneOrdreA = Donnees.m_donnees.TAB_ORDRE.Courant(lignePionA.ID_PION);
            for (int t = 25; t < 25+ligneOrdreA.I_DUREE+2; t++)
            {
                Donnees.m_donnees.TAB_PARTIE[0].I_TOUR = t;
                for (int p = 0; p < 100; p++)
                {
                    Donnees.m_donnees.TAB_PARTIE[0].I_PHASE = p;
                    Debug.WriteLine(lignePionA.OrdreActif(ligneOrdreA) +" t="+t+" p="+p+" HeureCourante=" + Donnees.m_donnees.TAB_PARTIE.HeureCourante());
                }
            }
            Debug.WriteLine("Fin");
            */
            //test sur le levee du soleil
            //for (int i=0; i<48; i++)
            //{
            //    Donnees.m_donnees.TAB_PARTIE[0].I_TOUR = i;
            //    Donnees.m_donnees.TAB_PARTIE[0].I_PHASE = 0;
            //    Debug.WriteLine(i+",0");
            //    Debug.WriteLine("HeureCourante=" + Donnees.m_donnees.TAB_PARTIE.HeureCourante());
            //    Debug.WriteLine("HeureBase=" + Donnees.m_donnees.TAB_PARTIE.HeureBase(i));
            //    Debug.WriteLine("Nocturne=" + Donnees.m_donnees.TAB_PARTIE.Nocturne());
            //    Donnees.m_donnees.TAB_PARTIE[0].I_PHASE = 1;
            //    Debug.WriteLine(i+",1");
            //    Debug.WriteLine("HeureCourante=" + Donnees.m_donnees.TAB_PARTIE.HeureCourante());
            //    Debug.WriteLine("HeureBase=" + Donnees.m_donnees.TAB_PARTIE.HeureBase(i));
            //    Debug.WriteLine("Nocturne=" + Donnees.m_donnees.TAB_PARTIE.Nocturne());
            //}
            //ChargementInitial();
            //foreach (Donnees.TAB_PIONRow lignePion in Donnees.m_donnees.TAB_PION)
            //{
            //    if (0 == lignePion.ID_CASE)
            //    { 
            //        lignePion.ID_CASE= (lignePion.estQG) ? 9708619 : 18332258; 
            //    }
            //}

            /* r�cup�ration et mise � jour des anciens noms de villes
            int i = 0;
            while (i < Donnees.m_donnees.TAB_NOMS_CARTE.Count)
            {
                Donnees.TAB_NOMS_CARTERow ligneNom = Donnees.m_donnees.TAB_NOMS_CARTE[i];
                if (ligneNom.B_PONT || ligneNom.I_Y + 1128>=4320)
                {
                    ligneNom.Delete();
                }
                else
                {
                    i++;
                }
            }

            foreach (Donnees.TAB_NOMS_CARTERow ligneNom in Donnees.m_donnees.TAB_NOMS_CARTE)
            {
                //if (ligneNom.S_NOM.IndexOf("Berlin", 0, ligneNom.S_NOM.Length, StringComparison.CurrentCultureIgnoreCase) >=0)
                //{
                //    Debug.WriteLine("berlin x="+ligneNom.I_X + " y=" + ligneNom.I_Y);
                //}
                ligneNom.I_Y += 1128;
                ligneNom.ID_CASE = Donnees.m_donnees.TAB_CASE.FindByXY(ligneNom.I_X, ligneNom.I_Y).ID_CASE;
            }
            */
            //disparation incompr�hensible d'un nom de ville
            //Donnees.TAB_NOMS_CARTERow ligneNom = Donnees.m_donnees.TAB_NOMS_CARTE.FindByID_NOM(206);
            //Donnees.TAB_NOMS_CARTERow ligneNomPlus = Donnees.m_donnees.TAB_NOMS_CARTE.AddTAB_NOMS_CARTERow(
            //    4922413, 0, "Bitterfeld", 0, 0, 0, 0, 1, 0, false, false, 0, 0, 0, 0, 0, 0, false, "Bitterfeld", 1533, 1483, false);
            //ligneNomPlus.ID_NOM = 206;


            #region Tests sur des chemins en doublon
            /*
            AStar etoile = new AStar();
            AstarTerrain[] tableCoutsMouvementsTerrain;
            double cout, coutHorsRoute;
            string messageErreur;
            List<LigneCASE> chemin;
            List<LigneCASE> cheminRaccourci;
            string retour = string.Empty;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            foreach (Donnees.TAB_PIONRow lignePion in Donnees.m_donnees.TAB_PION)
            {
                if (lignePion.B_DETRUIT) { continue; }
                Donnees.TAB_ORDRERow ligneOrdre = Donnees.m_donnees.TAB_ORDRE.Mouvement(lignePion.ID_PION);
                if (null == ligneOrdre) { continue; }
                Donnees.TAB_CASERow ligneCaseDepart = Donnees.m_donnees.TAB_CASE.FindParID_CASE(ligneOrdre.ID_CASE_DEPART);
                Donnees.TAB_CASERow ligneCaseDestination = Donnees.m_donnees.TAB_CASE.FindParID_CASE(ligneOrdre.ID_CASE_DESTINATION);

                if (etoile.RechercheChemin(Constantes.TYPEPARCOURS.MOUVEMENT, lignePion, 
                    ligneCaseDepart, ligneCaseDestination, ligneOrdre, 
                    out chemin, out cout, out coutHorsRoute, out tableCoutsMouvementsTerrain, out messageErreur))
                {
                    cheminRaccourci = etoile.ParcoursOptimise(chemin);
                    if (cheminRaccourci.Count() != chemin.Count())
                    {
                        retour += lignePion.S_NOM + " de " + chemin.Count().ToString() + "cases � " + cheminRaccourci.Count().ToString() + "cases\r\n";
                    }
                }
            }
            sw.Stop();
            Debug.WriteLine(string.Format("Test chemin en {0} heures, {1} minutes, {2} secondes, {3} millisecondes", 
                sw.Elapsed.Hours, sw.Elapsed.Minutes, sw.Elapsed.Seconds, sw.Elapsed.Milliseconds));
                */
            #endregion

            #region ajouts des noms uniques dep�ts et convois
            /*
            Donnees.m_donnees.TAB_NOMS_PIONS.AddTAB_NOMS_PIONSRow("Convoi de ravitaillement du D�p�t de Berlin");
            Donnees.m_donnees.TAB_NOMS_PIONS.AddTAB_NOMS_PIONSRow("Convoi de ravitaillement du D�p�t de Luckau");
            Donnees.m_donnees.TAB_NOMS_PIONS.AddTAB_NOMS_PIONSRow("Convoi de ravitaillement du D�p�t de Naumburg (Leipsic)");
            Donnees.m_donnees.TAB_NOMS_PIONS.AddTAB_NOMS_PIONSRow("Convoi de ravitaillement du D�p�t de Saalfeld");
            Donnees.m_donnees.TAB_NOMS_PIONS.AddTAB_NOMS_PIONSRow("Convoi de ravitaillement du D�p�t de Weimar");
            Donnees.m_donnees.TAB_NOMS_PIONS.AddTAB_NOMS_PIONSRow("Convoi de ravitaillement du D�p�t d'Erfurt");
            Donnees.m_donnees.TAB_NOMS_PIONS.AddTAB_NOMS_PIONSRow("Convoi de ravitaillement n�2 du D�p�t de Berlin");
            Donnees.m_donnees.TAB_NOMS_PIONS.AddTAB_NOMS_PIONSRow("Convoi de ravitaillement n�2 du D�p�t de Luckau");
            Donnees.m_donnees.TAB_NOMS_PIONS.AddTAB_NOMS_PIONSRow("Convoi de ravitaillement n�2 du D�p�t de Naumburg (Leipsic)");
            Donnees.m_donnees.TAB_NOMS_PIONS.AddTAB_NOMS_PIONSRow("Convoi de ravitaillement n�2 du D�p�t de Saalfeld");
            Donnees.m_donnees.TAB_NOMS_PIONS.AddTAB_NOMS_PIONSRow("Convoi de ravitaillement n�2 du D�p�t de Weimar");
            Donnees.m_donnees.TAB_NOMS_PIONS.AddTAB_NOMS_PIONSRow("Convoi de ravitaillement n�2 du D�p�t d'Erfurt");
            Donnees.m_donnees.TAB_NOMS_PIONS.AddTAB_NOMS_PIONSRow("Convoi de ravitaillement n�3 du D�p�t de Berlin");
            Donnees.m_donnees.TAB_NOMS_PIONS.AddTAB_NOMS_PIONSRow("Convoi de ravitaillement n�3 du D�p�t de Luckau");
            Donnees.m_donnees.TAB_NOMS_PIONS.AddTAB_NOMS_PIONSRow("Convoi de ravitaillement n�3 du D�p�t de Naumburg (Leipsic)");
            Donnees.m_donnees.TAB_NOMS_PIONS.AddTAB_NOMS_PIONSRow("Convoi de ravitaillement n�3 du D�p�t de Weimar");
            Donnees.m_donnees.TAB_NOMS_PIONS.AddTAB_NOMS_PIONSRow("Convoi de ravitaillement n�3 du D�p�t d'Erfurt");
            Donnees.m_donnees.TAB_NOMS_PIONS.AddTAB_NOMS_PIONSRow("Convoi de ravitaillement n�4 du D�p�t de Berlin");
            Donnees.m_donnees.TAB_NOMS_PIONS.AddTAB_NOMS_PIONSRow("Convoi de ravitaillement n�4 du D�p�t de Luckau");
            Donnees.m_donnees.TAB_NOMS_PIONS.AddTAB_NOMS_PIONSRow("Convoi de ravitaillement n�4 du D�p�t de Naumburg (Leipsic)");
            Donnees.m_donnees.TAB_NOMS_PIONS.AddTAB_NOMS_PIONSRow("Convoi de ravitaillement n�4 du D�p�t de Weimar");
            Donnees.m_donnees.TAB_NOMS_PIONS.AddTAB_NOMS_PIONSRow("Convoi de ravitaillement n�4 du D�p�t d'Erfurt");
            Donnees.m_donnees.TAB_NOMS_PIONS.AddTAB_NOMS_PIONSRow("Convoi de ravitaillement n�5 du D�p�t de Berlin");
            Donnees.m_donnees.TAB_NOMS_PIONS.AddTAB_NOMS_PIONSRow("Convoi de ravitaillement n�5 du D�p�t de Luckau");
            Donnees.m_donnees.TAB_NOMS_PIONS.AddTAB_NOMS_PIONSRow("Convoi de ravitaillement n�5 du D�p�t de Weimar");
            Donnees.m_donnees.TAB_NOMS_PIONS.AddTAB_NOMS_PIONSRow("Convoi de ravitaillement n�5 du D�p�t d'Erfurt");
            Donnees.m_donnees.TAB_NOMS_PIONS.AddTAB_NOMS_PIONSRow("Convoi de ravitaillement n�6 du D�p�t d'Erfurt");
            Donnees.m_donnees.TAB_NOMS_PIONS.AddTAB_NOMS_PIONSRow("D�p�t d'Altstadt");
            Donnees.m_donnees.TAB_NOMS_PIONS.AddTAB_NOMS_PIONSRow("D�p�t de Berlin");
            Donnees.m_donnees.TAB_NOMS_PIONS.AddTAB_NOMS_PIONSRow("D�p�t de Brandenburg");
            Donnees.m_donnees.TAB_NOMS_PIONS.AddTAB_NOMS_PIONSRow("D�p�t de Leipsic");
            Donnees.m_donnees.TAB_NOMS_PIONS.AddTAB_NOMS_PIONSRow("D�p�t de Luckau");
            Donnees.m_donnees.TAB_NOMS_PIONS.AddTAB_NOMS_PIONSRow("D�p�t de Magdeburg");
            Donnees.m_donnees.TAB_NOMS_PIONS.AddTAB_NOMS_PIONSRow("D�p�t de Naumburg (Leipsic)");
            Donnees.m_donnees.TAB_NOMS_PIONS.AddTAB_NOMS_PIONSRow("D�p�t de Neustadt (Saalfeld)");
            Donnees.m_donnees.TAB_NOMS_PIONS.AddTAB_NOMS_PIONSRow("D�p�t de Nordhausen");
            Donnees.m_donnees.TAB_NOMS_PIONS.AddTAB_NOMS_PIONSRow("D�p�t de Rosslau");
            Donnees.m_donnees.TAB_NOMS_PIONS.AddTAB_NOMS_PIONSRow("D�p�t de Saalfeld");
            Donnees.m_donnees.TAB_NOMS_PIONS.AddTAB_NOMS_PIONSRow("D�p�t de Sangershausen");
            Donnees.m_donnees.TAB_NOMS_PIONS.AddTAB_NOMS_PIONSRow("D�p�t de Torgau");
            Donnees.m_donnees.TAB_NOMS_PIONS.AddTAB_NOMS_PIONSRow("D�p�t de Weimar");
            Donnees.m_donnees.TAB_NOMS_PIONS.AddTAB_NOMS_PIONSRow("D�p�t de Wittemberg");
            Donnees.m_donnees.TAB_NOMS_PIONS.AddTAB_NOMS_PIONSRow("D�p�t de Zalma");
            Donnees.m_donnees.TAB_NOMS_PIONS.AddTAB_NOMS_PIONSRow("D�p�t d'Erfurt");
            */
            #endregion

            #region Mise � jour des rapports de prison et hopitaux sur un seul pion
            /*
            foreach (Donnees.TAB_NOMS_CARTERow ligneNomCarte in Donnees.m_donnees.TAB_NOMS_CARTE)
            {
                if (ligneNomCarte.B_PRISON && !ligneNomCarte.IsID_NATION_CONTROLENull())
                {
                    IEnumerable<Donnees.TAB_PIONRow> listeLeaders = Donnees.m_donnees.TAB_NATION.CommandantEnChef(ligneNomCarte.ID_NATION_CONTROLE);
                    int i = 0;
                    while (i < listeLeaders.Count())
                    {
                        Donnees.TAB_PIONRow lignePionLeader = listeLeaders.ElementAt(i);
                        string nomPrison = "Prison de " + ligneNomCarte.S_NOM;
                        // on v�rifie si un pion de m�me nom et m�me chef n'a pas d�j� �t� cr�e par le pass�
                        Donnees.TAB_PIONRow[] listePionRapport = (Donnees.TAB_PIONRow[])Donnees.m_donnees.TAB_PION.Select("S_NOM = '" + nomPrison+"' AND ID_PION_PROPRIETAIRE="+ lignePionLeader.ID_PION, "ID_PION");
                        if (listePionRapport.Count() > 0)
                        {
                            for (int j=1; j< listePionRapport.Count(); j++)
                            {
                                //on recherche tous les autres messages affect�s � d'autres unit�s du m�me nom
                                Donnees.TAB_MESSAGERow[] listeMessages = (Donnees.TAB_MESSAGERow[])Donnees.m_donnees.TAB_MESSAGE.Select("ID_PION_EMETTEUR = " + listePionRapport[j].ID_PION);
                                foreach (Donnees.TAB_MESSAGERow message in listeMessages)
                                {
                                    message.ID_PION_EMETTEUR = listePionRapport[0].ID_PION;
                                }
                            }
                        }
                        i++;
                    }
                }

                if (ligneNomCarte.B_HOPITAL && !ligneNomCarte.IsID_NATION_CONTROLENull())
                {
                    IEnumerable<Donnees.TAB_PIONRow> listeLeaders = Donnees.m_donnees.TAB_NATION.CommandantEnChef(ligneNomCarte.ID_NATION_CONTROLE);
                    int i = 0;
                    while (i < listeLeaders.Count())
                    {
                        Donnees.TAB_PIONRow lignePionLeader = listeLeaders.ElementAt(i);
                        string nomHopital = "H�pital de " + ligneNomCarte.S_NOM;
                        // on v�rifie si un pion de m�me nom et m�me chef n'a pas d�j� �t� cr�e par le pass�
                        Donnees.TAB_PIONRow[] listePionRapport = (Donnees.TAB_PIONRow[])Donnees.m_donnees.TAB_PION.Select("S_NOM = '" + nomHopital + "' AND ID_PION_PROPRIETAIRE=" + lignePionLeader.ID_PION, "ID_PION");
                        if (listePionRapport.Count() > 0)
                        {
                            for (int j = 1; j < listePionRapport.Count(); j++)
                            {
                                //on recherche tous les autres messages affect�s � d'autres unit�s du m�me nom
                                Donnees.TAB_MESSAGERow[] listeMessages = (Donnees.TAB_MESSAGERow[])Donnees.m_donnees.TAB_MESSAGE.Select("ID_PION_EMETTEUR = " + listePionRapport[j].ID_PION);
                                foreach (Donnees.TAB_MESSAGERow message in listeMessages)
                                {
                                    message.ID_PION_EMETTEUR = listePionRapport[0].ID_PION;
                                }
                            }
                        }
                        i++;
                    }
                }
            }
            */
            #endregion

            #region tests
            //ajout du mode forum pour les pions
            //ClassVaocWebFichier test = new ClassVaocWebFichier("test.test", false);
            //test.SauvegardeForum(Donnees.m_donnees.TAB_PARTIE[0].ID_PARTIE);

            // Test - Il faut pr�venir tous les "r�les" de l'ancienne nation que la ville change de camp
            /*
            Monitor.Enter(Donnees.m_donnees.TAB_ROLE.Rows.SyncRoot);
            Monitor.Enter(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
            Monitor.Enter(Donnees.m_donnees.TAB_MODELE_PION.Rows.SyncRoot);
            var result = from Role in Donnees.m_donnees.TAB_ROLE
                         join Pion in Donnees.m_donnees.TAB_PION
                         on Role.ID_PION equals Pion.ID_PION
                         join Modele in Donnees.m_donnees.TAB_MODELE_PION
                         on Pion.ID_MODELE_PION equals Modele.ID_MODELE_PION
                         where (Modele.ID_NATION == 1)
                            && (Pion.B_DETRUIT == false)
                         select Pion.ID_PION;

            Donnees.TAB_NOMS_CARTERow ligneNomCarte = Donnees.m_donnees.TAB_NOMS_CARTE.FindByID_NOM(211);
            ligneNomCarte.I_VICTOIRE = 1;
            ligneNomCarte.ID_NATION_CONTROLE = 0;
            foreach (var pion in result)
            {
                //Donnees.TAB_PIONRow lignePionRapport = ClassMessager.CreerMessager(lignePionLeader); -> il ne faut pas cr�er un messager sinon on a un mauvais emetteur dans les messages sur le web
                Donnees.TAB_PIONRow lignePionLeader = Donnees.m_donnees.TAB_PION.FindByID_PION(pion);
                Donnees.TAB_PIONRow lignePionRapport = lignePionLeader.CreerConvoi(lignePionLeader, false, false, false);
                lignePionRapport.S_NOM = ligneNomCarte.S_NOM;
                lignePionRapport.ID_CASE = ligneNomCarte.ID_CASE;
                lignePionRapport.I_INFANTERIE = 0;
                lignePionRapport.I_CAVALERIE = 0;
                lignePionRapport.I_ARTILLERIE = 0;

                //oblig� de l'envoy� en imm�diat, sinon le pion prison apparait dans la liste des unit�s !
                lignePionRapport.DetruirePion();
                if (!ClassMessager.EnvoyerMessage(lignePionRapport, ClassMessager.MESSAGES.MESSAGE_LIEU_POINT_DE_VICTOIRE))
                {
                    LogFile.Notifier("ControleDesVilles : erreur lors de l'envoi d'un message MESSAGE_RAVITAILLEMENT_DIRECT_IMPOSSIBLE");
                    Monitor.Exit(Donnees.m_donnees.TAB_ROLE.Rows.SyncRoot);
                    Monitor.Exit(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                    Monitor.Exit(Donnees.m_donnees.TAB_MODELE_PION.Rows.SyncRoot);
                }
            }

            Monitor.Exit(Donnees.m_donnees.TAB_ROLE.Rows.SyncRoot);
            Monitor.Exit(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
            Monitor.Exit(Donnees.m_donnees.TAB_MODELE_PION.Rows.SyncRoot);
            */
            #endregion

            #region mise � jour des tipes Depot/convoi sur les lignes vid�os
            /*
            foreach (Donnees.TAB_PIONRow lignePion in Donnees.m_donnees.TAB_PION)
            {
                if (lignePion.estConvoiDeRavitaillement || lignePion.estDepot)
                {
                    //on recherche tous les ordres de mouvements de l'unite
                    string requeteOrdre = string.Format("(ID_PION={0}) AND I_ORDRE_TYPE = {1}",
                        lignePion.ID_PION, Constantes.ORDRES.MOUVEMENT);
                    Donnees.TAB_ORDRERow[] resOrdre = (Donnees.TAB_ORDRERow[])Donnees.m_donnees.TAB_ORDRE.Select(requeteOrdre);
                    if (resOrdre.Count() > 0)
                    {
                        //par d�faut, c'est un depot (en fait, cela peut etre un convoi au d�but mais comme en d�but de partie
                        //on a que des depots, �a doit marcher :-)
                        string requeteVideo = string.Format("ID_PION={0}", lignePion.ID_PION);
                        Donnees.TAB_VIDEORow[] resVideo = (Donnees.TAB_VIDEORow[])Donnees.m_donnees.TAB_VIDEO.Select(requeteVideo);
                        foreach (Donnees.TAB_VIDEORow ligneVideo in resVideo)
                        {
                            ligneVideo.I_TYPE = (int)TIPEUNITEVIDEO.DEPOT;
                        }

                        foreach (Donnees.TAB_ORDRERow ligneOrdre in resOrdre)
                        {
                            //recherche de toutes les lignes video correspondantes
                            requeteVideo = string.Format("ID_PION={0} AND I_TOUR>={1} AND I_TOUR<={2}", 
                                lignePion.ID_PION, 
                                ligneOrdre.I_TOUR_DEBUT,
                                ligneOrdre.IsI_TOUR_FINNull() ? Donnees.m_donnees.TAB_PARTIE[0].I_TOUR : ligneOrdre.I_TOUR_FIN);
                            resVideo = (Donnees.TAB_VIDEORow[])Donnees.m_donnees.TAB_VIDEO.Select(requeteVideo);
                            foreach (Donnees.TAB_VIDEORow ligneVideo in resVideo)
                            {
                                ligneVideo.I_TYPE = (int)TIPEUNITEVIDEO.CONVOI;
                            }
                        }
                    }
                    else
                    {
                        string requeteVideo= string.Format("ID_PION={0}",lignePion.ID_PION);
                        Donnees.TAB_VIDEORow[] resVideo = (Donnees.TAB_VIDEORow[])Donnees.m_donnees.TAB_VIDEO.Select(requeteVideo);
                        foreach (Donnees.TAB_VIDEORow ligneVideo in resVideo)
                        {
                            ligneVideo.I_TYPE = (lignePion.estDepot) ? (int)TIPEUNITEVIDEO.DEPOT : (int)TIPEUNITEVIDEO.CONVOI;
                        }
                    }
                }
            }
            */
            #endregion

            #region reprise d'un tour de donn�es video
            /*
            int i = 0;
            while (i< Donnees.m_donnees.TAB_VIDEO.Count())
            {
                Donnees.TAB_VIDEORow ligneV = Donnees.m_donnees.TAB_VIDEO[i++];
                if (219 !=ligneV.I_TOUR)
                {
                    continue;
                }
                Donnees.TAB_VIDEORow ligneVideo = Donnees.m_donnees.TAB_VIDEO.AddTAB_VIDEORow(
                    220,
                    ligneV.ID_NATION,
                    ligneV.ID_PION,
                    ligneV.ID_MODELE_PION,
                    ligneV.ID_PION_PROPRIETAIRE,
                    ligneV.S_NOM,
                    ligneV.I_INFANTERIE,
                    ligneV.I_INFANTERIE_INITIALE,
                    ligneV.I_CAVALERIE,
                    ligneV.I_CAVALERIE_INITIALE,
                    ligneV.I_ARTILLERIE,
                    ligneV.I_ARTILLERIE_INITIALE,
                    ligneV.I_FATIGUE,
                    ligneV.I_MORAL,
                    ligneV.ID_CASE,
                    ligneV.IsID_BATAILLENull() ? -1 : ligneV.ID_BATAILLE,
                    ligneV.B_DETRUIT,
                    ligneV.B_FUITE_AU_COMBAT,
                    ligneV.I_MATERIEL,
                    ligneV.I_RAVITAILLEMENT,
                    ligneV.B_BLESSES,
                    ligneV.B_PRISONNIERS,
                    ligneV.C_NIVEAU_DEPOT,
                    ligneV.IsI_VICTOIRENull() ? 0 : ligneV.I_VICTOIRE
                );
                if (ligneV.IsID_BATAILLENull()) ligneVideo.SetID_BATAILLENull();
            }
                */
            #endregion recup�ration d'un tour vid�o

            #region deplacement des fichiers par repertoire
            /*
            for (int tour=0; tour<=222;tour++)
            {
                string repertoireSource = string.Format("{0}cases\\", Constantes.repertoireDonnees, tour);
                string repertoire = string.Format("{0}cases\\{1:D4}_000\\", Constantes.repertoireDonnees,tour);
                if (!Directory.Exists(repertoire))
                {
                    //cr�ation du r�pertoire
                    Directory.CreateDirectory(repertoire);
                }
                else
                {
                    //destruction du contenu
                    string[] fichiers = Directory.GetFiles(repertoire);
                    foreach (string fichier in fichiers) File.Delete(fichier);
                }

                for (int x = 0; x < Donnees.m_donnees.TAB_JEU[0].I_LARGEUR_CARTE; x += Constantes.CST_TAILLE_BLOC_CASES)
                {
                    for (int y = 0; y < Donnees.m_donnees.TAB_JEU[0].I_HAUTEUR_CARTE; y += Constantes.CST_TAILLE_BLOC_CASES)
                    {
                        string nomfichier = string.Format("{0}{00001}_{00002}_{3}_{4}.cases",
                           repertoireSource, (x / Constantes.CST_TAILLE_BLOC_CASES) * Constantes.CST_TAILLE_BLOC_CASES,
                                       (y / Constantes.CST_TAILLE_BLOC_CASES) * Constantes.CST_TAILLE_BLOC_CASES, tour, 0);
                        //on recherche le dernier fichier sauvegard� sur les cases
                        int phaserecherche = 0;
                        int tourrecherche = tour;
                        while (!File.Exists(nomfichier) && tourrecherche >= 0)
                        {
                            phaserecherche -= Constantes.CST_SAUVEGARDE_ECART_PHASES;
                            if (phaserecherche < 0)
                            {
                                phaserecherche = Donnees.m_donnees.TAB_JEU[0].I_NOMBRE_PHASES;
                                tourrecherche--;
                            }
                            nomfichier = string.Format("{0}{00001}_{00002}_{3}_{4}.cases",
                                repertoireSource, (x / Constantes.CST_TAILLE_BLOC_CASES) * Constantes.CST_TAILLE_BLOC_CASES,
                                       (y / Constantes.CST_TAILLE_BLOC_CASES) * Constantes.CST_TAILLE_BLOC_CASES, tourrecherche, phaserecherche);
                        }
                        string nomfichierDestination = string.Format("{0}{1:D5}_{2:D5}.cases",
                                repertoire, (x / Constantes.CST_TAILLE_BLOC_CASES) * Constantes.CST_TAILLE_BLOC_CASES,
                                       (y / Constantes.CST_TAILLE_BLOC_CASES) * Constantes.CST_TAILLE_BLOC_CASES, tourrecherche, phaserecherche);
                        File.Copy(nomfichier, nomfichierDestination);
                    }
                }
            }
            */
            #endregion

            #region test sur le nom de d�p�ts
            //Donnees.TAB_PIONRow ligneTest = Donnees.m_donnees.TAB_PION.FindByID_PION(13);
            //string nom;
            //nom = ligneTest.CreerConvoi(ligneTest.proprietaire, false, false, false).S_NOM;
            //nom = ligneTest.CreerConvoi(ligneTest.proprietaire, false, false, false).S_NOM;
            //nom = ligneTest.CreerConvoi(ligneTest.proprietaire, false, false, false).S_NOM;
            //nom = ligneTest.CreerConvoi(ligneTest.proprietaire, false, false, false).S_NOM;
            //nom = ligneTest.CreerConvoi(ligneTest.proprietaire, false, false, false).S_NOM;
            //nom = ligneTest.CreerConvoi(ligneTest.proprietaire, false, false, false).S_NOM;
            //nom = ligneTest.CreerConvoi(ligneTest.proprietaire, false, false, false).S_NOM;
            //nom = ligneTest.CreerConvoi(ligneTest.proprietaire, false, false, false).S_NOM;
            //nom = ligneTest.CreerConvoi(ligneTest.proprietaire, false, false, false).S_NOM;
            #endregion

            #region controle des parcours
            //Donnees.TAB_PIONRow lignePion = Donnees.m_donnees.TAB_PION.FindByID_PION(4489);
            //Donnees.TAB_ORDRERow ligneOrdre = Donnees.m_donnees.TAB_ORDRE.Mouvement(lignePion.ID_PION);
            //AStar star = new AStar();
            //List<LigneCASE> chemin;
            //List<LigneCASE> cheminOptimise;
            //double coutGlobal, couthorsroute;
            //AstarTerrain[] tableCoutsMouvementsTerrain;
            //string message;
            //star.RechercheChemin(Constantes.TYPEPARCOURS.MOUVEMENT, lignePion,
            //    Donnees.m_donnees.TAB_CASE.FindParID_CASE(ligneOrdre.ID_CASE_DEPART),
            //    Donnees.m_donnees.TAB_CASE.FindParID_CASE(ligneOrdre.ID_CASE_DESTINATION),
            //    ligneOrdre, out chemin, out coutGlobal, out couthorsroute, out tableCoutsMouvementsTerrain, out message);
            //foreach(LigneCASE l in chemin)
            //{
            //    Debug.WriteLine(string.Format("{0}:{1},{2}", l.ID_CASE, l.I_X, l.I_Y));
            //}
            //cheminOptimise=star.ParcoursOptimise(chemin);

            //pour reforcer un calcul de tous les parcours
            /*
            Donnees.m_donnees.TAB_PARCOURS.Clear();
            AStar star = new AStar();
            foreach (Donnees.TAB_PIONRow lignePion in Donnees.m_donnees.TAB_PION)
            {
                if (lignePion.B_DETRUIT) continue;
                Donnees.TAB_ORDRERow ligneOrdre = Donnees.m_donnees.TAB_ORDRE.Mouvement(lignePion.ID_PION);
                if (null== ligneOrdre) continue;
                List<LigneCASE> chemin;
                double coutGlobal, couthorsroute;
                AstarTerrain[] tableCoutsMouvementsTerrain;
                string message;
                star.RechercheChemin(Constantes.TYPEPARCOURS.MOUVEMENT, lignePion,
                    Donnees.m_donnees.TAB_CASE.FindParID_CASE(ligneOrdre.ID_CASE_DEPART),
                    Donnees.m_donnees.TAB_CASE.FindParID_CASE(ligneOrdre.ID_CASE_DESTINATION),
                    ligneOrdre, out chemin, out coutGlobal, out couthorsroute, out tableCoutsMouvementsTerrain, out message);
                int i = 0;
                while (i < chemin.Count && chemin[i].ID_CASE != lignePion.ID_CASE) i++;
                if (i==chemin.Count)
                {
                    //l'unit� �tait sur une position supprim�e
                    //repositionnement au plus pr�t
                    int idcaseposition = -1;
                    double distanceMax = double.MaxValue;
                    Donnees.TAB_CASERow ligneCasePion = Donnees.m_donnees.TAB_CASE.FindParID_CASE(lignePion.ID_CASE);
                    foreach (LigneCASE l in chemin)
                    {
                        double distance = Constantes.Distance(l.I_X, l.I_Y, ligneCasePion.I_X, ligneCasePion.I_Y);
                        if (distance<distanceMax)
                        {
                            distanceMax = distance;
                            idcaseposition = l.ID_CASE;
                        }
                    }
                    lignePion.ID_CASE = idcaseposition;
                }
            }
            */
            #endregion
            //bug meteo
            //Donnees.m_donnees.TAB_PARTIE[0].ID_METEO = 0;            

            #region test a priori absurde sur la copie de cases
            /*
            Donnees.TAB_CASEDataTable tableTestSource = new Donnees.TAB_CASEDataTable();
            try
            {
                Donnees.m_donnees.TAB_CASE.AddTAB_CASERow(0, 1, 0, 0, Constantes.NULLENTIER, Constantes.NULLENTIER, Constantes.NULLENTIER, Constantes.CST_COUTMAX);
                //tableTestSource.AddTAB_CASERow(0, 1, 0, 0, Constantes.NULLENTIER, Constantes.NULLENTIER, Constantes.NULLENTIER, Constantes.CST_COUTMAX);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            try
            {
                Donnees.m_donnees.TAB_CASE.AddTAB_CASERow(0, 1, 0, 0, Constantes.NULLENTIER, Constantes.NULLENTIER, Constantes.NULLENTIER, Constantes.CST_COUTMAX);
                //tableTestSource.AddTAB_CASERow(0, 1, 0, 0, Constantes.NULLENTIER, Constantes.NULLENTIER, Constantes.NULLENTIER, Constantes.CST_COUTMAX);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            try
            {
                Donnees.TAB_CASEDataTable tableTest = new Donnees.TAB_CASEDataTable();
                tableTest.AddTAB_CASERow(0, 1, 0, 0, Constantes.NULLENTIER, Constantes.NULLENTIER, Constantes.NULLENTIER, Constantes.CST_COUTMAX);
                Donnees.m_donnees.TAB_CASE.ImportRow(tableTest[0]);
                //tableTestSource.ImportRow(tableTest[0]);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            */
            #endregion

            #region reaffectation des des points de victoire d'apr�s le dernier tour charg� sur la table video
            //Donnees.TAB_VIDEORow[] lignesVideoVictoire = (Donnees.TAB_VIDEORow[]) Donnees.m_donnees.TAB_VIDEO.Select("I_VICTOIRE>0 AND I_TOUR=0" );//+ Donnees.m_donnees.TAB_PARTIE[0].I_TOUR
            //if (lignesVideoVictoire.Count()>0)
            //{
            //    foreach (Donnees.TAB_VIDEORow ligneVideo in Donnees.m_donnees.TAB_VIDEO)
            //    {
            //        foreach (Donnees.TAB_VIDEORow ligneVideoVictoire in lignesVideoVictoire)
            //        {
            //            if (ligneVideo.ID_PION == ligneVideoVictoire.ID_PION)
            //            {
            //                ligneVideo.I_VICTOIRE = ligneVideoVictoire.I_VICTOIRE;
            //            }
            //        }
            //    }
            //}
            #endregion

            #region calcul des points de victoire
            //ClassTraitementHeure cth = new ClassTraitementHeure();
            //cth.CalculNombreTotalPointsDeVictoire();
            #endregion

            #region remise � 0 de la fatigue pour les prisonniers et l'artillerie
            /*
            int i = 0;
            while (i < Donnees.m_donnees.TAB_PION.Count)
            {
                Donnees.TAB_PIONRow lignePion = Donnees.m_donnees.TAB_PION[i];
                //calcul de la fatigue et du moral
                if (!lignePion.B_DETRUIT 
                    && (lignePion.estDepot || lignePion.estBlesses
                    || lignePion.estConvoiDeRavitaillement || lignePion.estQG || lignePion.estMessager
                    || lignePion.estPatrouille || lignePion.estPontonnier || lignePion.estPrisonniers)
                    )
                {
                    lignePion.I_FATIGUE = 0;
                }
                i++;
            }
            */
            #endregion
            #region Suppression de donn�es dans les tables
            /*
            Donnees.m_donnees.TAB_CASE.Clear();
            Donnees.m_donnees.TAB_MESSAGE.Clear();
            Donnees.m_donnees.TAB_ESPACE.Clear();
            Donnees.m_donnees.TAB_PCC_CASE_BLOCS.Clear();
            Donnees.m_donnees.TAB_PCC_COUTS.Clear();
            Donnees.m_donnees.TAB_PCC_VILLES_INUTILE.Clear();
            Donnees.m_donnees.TAB_PCC_VILLES.Clear();
           /*
          int i = 0;
          while (i< Donnees.m_donnees.TAB_MESSAGE.Count())
          {
              Donnees.TAB_MESSAGERow ligneMessage = Donnees.m_donnees.TAB_MESSAGE[i];
              if (ligneMessage.ID_PION_EMETTEUR == 3389 && ligneMessage.ID_MESSAGE > 5927)
              {
                  ligneMessage.Delete();
              }
              else
              {
                  i++;
              }
          }
          */
            #endregion

            #region correction cibl�e de la carte
            /* C'est revenu apr�s plusieurs tours !!! -> mais �a fait un crash si phase <>0
            //pour cr�er les listeindex, sinon crash au chargement
            Donnees.m_donnees.TAB_CASE.InitialisationListeCase(Donnees.m_donnees.TAB_JEU[0].I_LARGEUR_CARTE, Donnees.m_donnees.TAB_JEU[0].I_HAUTEUR_CARTE);//optimisation m�moire
            //Donnees.m_donnees.TAB_CASE.InitialisationListeCaseNonCoutMax();//optimisation de performance pour AStar.SearchSpace
            List<Donnees.TAB_CASERow> listeCasesModif = new List<Donnees.TAB_CASERow>();
            int x = 1938, y = 594;
            Donnees.m_donnees.TAB_CASE.ChargerCases(x, y, Donnees.m_donnees.TAB_PARTIE[0].I_TOUR, Donnees.m_donnees.TAB_PARTIE[0].I_PHASE);
            listeCasesModif.Add(Donnees.m_donnees.TAB_CASE.FindByXY(x, y));

            x = 1939;
            if(null == Donnees.m_donnees.TAB_CASE.FindByXY(x, y))
            {
                Donnees.m_donnees.TAB_CASE.ChargerCases(x, y, Donnees.m_donnees.TAB_PARTIE[0].I_TOUR, Donnees.m_donnees.TAB_PARTIE[0].I_PHASE);
            }
            listeCasesModif.Add(Donnees.m_donnees.TAB_CASE.FindByXY(x, y));

            x = 1940;
            if (null == Donnees.m_donnees.TAB_CASE.FindByXY(x, y))
            {
                Donnees.m_donnees.TAB_CASE.ChargerCases(x, y, Donnees.m_donnees.TAB_PARTIE[0].I_TOUR, Donnees.m_donnees.TAB_PARTIE[0].I_PHASE);
            }
            listeCasesModif.Add(Donnees.m_donnees.TAB_CASE.FindByXY(x, y));
            AStar etoile = new AStar();
            List<Bloc> listeBlocsARecomposer = new List<Bloc>();
            ClassHPAStarCreation hpaStarCreation = new ClassHPAStarCreation(Donnees.m_donnees.TAB_JEU[0].I_TAILLEBLOC_PCC);
            hpaStarCreation.InitialisationIdTrajet();
            foreach (Donnees.TAB_CASERow ligneCaseModif in listeCasesModif)
            {
                ligneCaseModif.ID_MODELE_TERRAIN = 59;

                List<Bloc> listeBlocs = etoile.Num�roBlocParPosition(ligneCaseModif.I_X, ligneCaseModif.I_Y);
                foreach (Bloc bloc in listeBlocs)
                {
                    if (!listeBlocsARecomposer.Contains(bloc))
                    {
                        listeBlocsARecomposer.Add(new Bloc(bloc.xBloc, bloc.yBloc));
                    }
                }
            }
            foreach (Bloc bloc in listeBlocsARecomposer)
            {
                //il faut supprimer tous les trajets du bloc et les recalculer pour tenir des �volutions du mod�le du terrain
                //une destruction de ponton entraine un recalcul global de tous les trajets du bloc
                if (!hpaStarCreation.RecalculCheminPCCBloc(bloc.xBloc, bloc.yBloc, true))
                {
                    string message = string.Format("Correctifs: Erreur fatale sur RecalculCheminPCCBloc bloc X={0} Y={1}", bloc.xBloc, bloc.yBloc);
                    Debug.WriteLine(message);
                    MessageBox.Show(message, "Correctif", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            // Il faut recalculer la table d'optimisation HPA
            Donnees.m_donnees.TAB_PCC_COUTS.Initialisation();
            */
            #endregion

            #region Changement de prison
            /*
            Donnees.TAB_PIONRow[] lignesPionsPrisons = (Donnees.TAB_PIONRow[]) Donnees.m_donnees.TAB_PION.Select("ID_LIEU_RATTACHEMENT=781");
            foreach (Donnees.TAB_PIONRow lignePion in lignesPionsPrisons)
            {
                lignePion.ID_LIEU_RATTACHEMENT = 140;
            }
            */
            #endregion
            /*
            foreach (Donnees.TAB_ORDRERow ligneOrdre in Donnees.m_donnees.TAB_ORDRE)
            {
                if (ligneOrdre.ID_ORDRE < 0) { ligneOrdre.ID_ORDRE = -ligneOrdre.ID_ORDRE; }
                if (!ligneOrdre.IsID_ORDRE_SUIVANTNull() && ligneOrdre.ID_ORDRE_SUIVANT < 0) { ligneOrdre.ID_ORDRE_SUIVANT = -ligneOrdre.ID_ORDRE_SUIVANT; }
                if (!ligneOrdre.IsID_ORDRE_TRANSMISNull() && ligneOrdre.ID_ORDRE_TRANSMIS < 0) { ligneOrdre.ID_ORDRE_TRANSMIS = -ligneOrdre.ID_ORDRE_TRANSMIS; }
            }

            foreach (Donnees.TAB_NOMS_CARTERow ligneNom in Donnees.m_donnees.TAB_NOMS_CARTE)
            {
                if (ligneNom.IsB_PONTNull()) { ligneNom.B_PONT = false; }
            }

            foreach (Donnees.TAB_PIONRow lignePion in Donnees.m_donnees.TAB_PION)
            {
                if (!lignePion.IsID_PION_REMPLACENull() && 0==lignePion.ID_PION_REMPLACE) { lignePion.SetID_PION_REMPLACENull(); }
            }
            */
            //foreach (Donnees.TAB_PIONRow lignePion in Donnees.m_donnees.TAB_PION)
            //{
            //    lignePion.SetID_BATAILLENull();
            //    lignePion.SetI_TOUR_SANS_RAVITAILLEMENTNull();
            //    lignePion.SetI_ZONE_BATAILLENull();
            //    lignePion.SetI_TOUR_CONVOI_CREENull();
            //    lignePion.SetID_PION_REMPLACENull();
            //    lignePion.SetI_TOUR_BLESSURENull();
            //    lignePion.SetID_LIEU_RATTACHEMENTNull();
            //    lignePion.SetID_NOUVEAU_PION_PROPRIETAIRENull();
            //    lignePion.SetID_ANCIEN_PION_PROPRIETAIRENull();
            //    lignePion.SetID_PION_ESCORTENull();
            //}

            /* 
            Donnees.TAB_PIONRow lignePionCorrectif;
            lignePionCorrectif = Donnees.m_donnees.TAB_PION.FindByID_PION(207);
            Donnees.TAB_CASERow ligneCase = Donnees.m_donnees.TAB_CASE.FindParID_CASE(3480428);
            for (int i = 0; i < 100; i++)
            {
                String test;
                bool btest;
                ClassMessager.PionsEnvironnants(lignePionCorrectif, ClassMessager.MESSAGES.MESSAGE_PATROUILLE_CONTACT_ENNEMI, ligneCase, out test, out btest);
            }
            */

            //recherche de tous les points dans le bloc
            //int[] des = new int[6];
            //int[] score = new int[6];
            //des[1] = 3;
            //des[4] = 3;
            //for (int i = 0; i < 6; i++)
            //{
            //    score[i] = (des[i] > 0) ? Constantes.JetDeDes(des[i]) : 0;
            //}

            /*
            Donnees.TAB_CASERow ligneCaseArrivee;
            Donnees.TAB_PCC_CASE_BLOCSRow[] listeCases;
            listeCases = Donnees.m_donnees.TAB_PCC_CASE_BLOCS.ListeCasesBloc(43, 23);

            for (int i = 0; i < listeCases.Length; i++)
            {
                ligneCaseArrivee = Donnees.m_donnees.TAB_CASE.FindParID_CASE(listeCases[i].ID_CASE);
                Debug.WriteLine(string.Format("case ={0}({1},{2})", 
                    ligneCaseArrivee.ID_CASE, ligneCaseArrivee.I_X, ligneCaseArrivee.I_Y));
            }

            Donnees.TAB_PCC_COUTSRow[] lignesCout = (Donnees.TAB_PCC_COUTSRow[])Donnees.m_donnees.TAB_PCC_COUTS.Select("I_BLOCX=43 AND I_BLOCY=23");
            //Donnees.TAB_PCC_COUTSRow[] lignesCout = (Donnees.TAB_PCC_COUTSRow[])Donnees.m_donnees.TAB_PCC_COUTS.Select("ID_CASE_FIN=4431204");
            Donnees.TAB_CASERow lDebug1, lDebug2;
            foreach (Donnees.TAB_PCC_COUTSRow ligneCout in lignesCout)
            {

                lDebug1 = Donnees.m_donnees.TAB_CASE.FindParID_CASE(ligneCout.ID_CASE_DEBUT);
                lDebug2 = Donnees.m_donnees.TAB_CASE.FindParID_CASE(ligneCout.ID_CASE_FIN);
                Debug.WriteLine(string.Format("CasesVoisinesHPA ajout de ligneCout : ID={0}({1},{2}) bloc :{3},{4} -> ID={5}({6},{7}) bloc :{8},{9}, ID_TRAJET={10}",
    lDebug1.ID_CASE, lDebug1.I_X, lDebug1.I_Y, ligneCout.I_BLOCX, ligneCout.I_BLOCY,
    lDebug2.ID_CASE, lDebug2.I_X, lDebug2.I_Y, ligneCout.I_BLOCX, ligneCout.I_BLOCY,
    ligneCout.ID_TRAJET));
            }
            */
            //test sur messager
            //int idNation=0;
            //Donnees.TAB_MODELE_PIONRow ligneModelePion = Donnees.m_donnees.TAB_MODELE_PION.FindByID_MODELE_PION(1);
            //ligneModelePion.ID_NATION = 0;
            /*
            var result = from Modele in Donnees.m_donnees.TAB_MODELE_PION
                     join AptitudesPion in Donnees.m_donnees.TAB_APTITUDES_PION
                     on new { Modele.ID_MODELE_PION, idNation = Modele.ID_NATION } equals new { AptitudesPion.ID_MODELE_PION, idNation = 0 }
                     join Aptitudes in Donnees.m_donnees.TAB_APTITUDES
                     on new { AptitudesPion.ID_APTITUDE, stipe = "PATROUILLEMESSAGER" } equals new { Aptitudes.ID_APTITUDE, stipe = Aptitudes.S_NOM }
                         select new { Modele.ID_MODELE_PION, Modele.ID_NATION };

            if (result.Count() > 0)
            {
                int r = result.ElementAt(0).ID_MODELE_PION;
                Debug.WriteLine("messager id="+r.ToString());
            }
            //ligneModelePion.ID_NATION = 1;
            result = from Modele in Donnees.m_donnees.TAB_MODELE_PION
                         join AptitudesPion in Donnees.m_donnees.TAB_APTITUDES_PION
                         on new { Modele.ID_MODELE_PION, idNation = Modele.ID_NATION } equals new { AptitudesPion.ID_MODELE_PION, idNation = 1}
                         join Aptitudes in Donnees.m_donnees.TAB_APTITUDES
                         on new { AptitudesPion.ID_APTITUDE, stipe = "PATROUILLEMESSAGER" } equals new { Aptitudes.ID_APTITUDE, stipe = Aptitudes.S_NOM }
                     select new { Modele.ID_MODELE_PION, Modele.ID_NATION };

            if (result.Count() > 0)
            {
                int r = result.ElementAt(0).ID_MODELE_PION;
                Debug.WriteLine("messager id=" + r.ToString());
            }
            */
            //test de ennemi observable
            /*
            foreach (Donnees.TAB_PIONRow lignePion in Donnees.m_donnees.TAB_PION)
            {
                Donnees.TAB_CASERow ligneCase = Donnees.m_donnees.TAB_CASE.FindParID_CASE(lignePion.ID_CASE);
                bool bObservable = Cartographie.EnnemiObservable(lignePion, null);
                string phrase = bObservable ? ClassMessager.GenererPhrase(lignePion, ClassMessager.MESSAGES.MESSAGE_ENNEMI_OBSERVE, 0, 0, 0, 0, 0, null, null, null, null, 0, string.Empty) : ClassMessager.GenererPhrase(lignePion, ClassMessager.MESSAGES.MESSAGE_SANS_ENNEMI_OBSERVE, 0, 0, 0, 0, 0, null, null, null, null, 0, string.Empty);
                Debug.WriteLine(String.Format("pion ID= {0}: {1} EnnemiObservable={2} en {3},{4} : {5}",
                    lignePion.ID_PION,
                    lignePion.S_NOM,
                    bObservable,
                    ligneCase.I_X,
                    ligneCase.I_Y,
                    phrase));
            }
             * */
            #region correctif pour ajout de donn�es

            //Donnees.TAB_ORDRERow ligneOrdre = Donnees.m_donnees.TAB_ORDRE.FindByID_ORDRE(498);
            //ligneOrdre.ID_CASE_DEPART = 4402341;

            /*
            Donnees.TAB_PIONRow lignePionCorrectif;
            lignePionCorrectif = Donnees.m_donnees.TAB_PION.FindByID_PION(128);
            Cartographie.DetruireEspacePion(lignePionCorrectif);//force, par la suite, le recalcul de tous les espaces, parcours, etc.
            lignePionCorrectif = Donnees.m_donnees.TAB_PION.FindByID_PION(126);
            Cartographie.DetruireEspacePion(lignePionCorrectif);//force, par la suite, le recalcul de tous les espaces, parcours, etc.
            lignePionCorrectif = Donnees.m_donnees.TAB_PION.FindByID_PION(127);
            Cartographie.DetruireEspacePion(lignePionCorrectif);//force, par la suite, le recalcul de tous les espaces, parcours, etc.
            lignePionCorrectif = Donnees.m_donnees.TAB_PION.FindByID_PION(129);
            Cartographie.DetruireEspacePion(lignePionCorrectif);//force, par la suite, le recalcul de tous les espaces, parcours, etc.
            lignePionCorrectif = Donnees.m_donnees.TAB_PION.FindByID_PION(130);
            Cartographie.DetruireEspacePion(lignePionCorrectif);//force, par la suite, le recalcul de tous les espaces, parcours, etc.
             * */

            //Donnees.m_donnees.TAB_JEU[0].I_COUCHER_DU_SOLEIL = 21;
            /*
            string nomTest;
            ClassMessager.NomDeBataille(5174910, out nomTest);
            Donnees.m_donnees.TAB_BATAILLE.AddTAB_BATAILLERow(nomTest, 0, 0, 0, 0, 'H', 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            ClassMessager.NomDeBataille(5174910, out nomTest);
            Donnees.m_donnees.TAB_BATAILLE.AddTAB_BATAILLERow(nomTest, 0, 0, 0, 0, 'H', 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            ClassMessager.NomDeBataille(5174910, out nomTest);
            Donnees.m_donnees.TAB_BATAILLE.AddTAB_BATAILLERow(nomTest, 0, 0, 0, 0, 'H', 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            ClassMessager.NomDeBataille(5174910, out nomTest);
            Donnees.m_donnees.TAB_BATAILLE.AddTAB_BATAILLERow(nomTest, 0, 0, 0, 0, 'H', 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            ClassMessager.NomDeBataille(5174910, out nomTest);
            Donnees.m_donnees.TAB_BATAILLE.AddTAB_BATAILLERow(nomTest, 0, 0, 0, 0, 'H', 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            ClassMessager.NomDeBataille(5174910, out nomTest);
            Donnees.m_donnees.TAB_BATAILLE.AddTAB_BATAILLERow(nomTest, 0, 0, 0, 0, 'H', 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            ClassMessager.NomDeBataille(5174910, out nomTest);
            Donnees.m_donnees.TAB_BATAILLE.AddTAB_BATAILLERow(nomTest, 0, 0, 0, 0, 'H', 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            ClassMessager.NomDeBataille(5174910, out nomTest);
            Donnees.m_donnees.TAB_BATAILLE.AddTAB_BATAILLERow(nomTest, 0, 0, 0, 0, 'H', 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            ClassMessager.NomDeBataille(5174910, out nomTest);
            Donnees.m_donnees.TAB_BATAILLE.AddTAB_BATAILLERow(nomTest, 0, 0, 0, 0, 'H', 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            ClassMessager.NomDeBataille(5174910, out nomTest);
            */
            /*
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
            imageCarte.Dispose();

            foreach (Donnees.TAB_MODELE_TERRAINRow ligneModeleTerrain in Donnees.m_donnees.TAB_MODELE_TERRAIN)
            {
                if (ligneModeleTerrain.IsB_PONTNull()) { ligneModeleTerrain.B_PONT = false; };
                if (ligneModeleTerrain.IsB_PONTONNull()) { ligneModeleTerrain.B_PONTON = false; };
                if (ligneModeleTerrain.IsB_DETRUITNull()) { ligneModeleTerrain.B_DETRUIT = false; };
            }

            foreach (Donnees.TAB_PCC_VILLESRow lignePCCVille in Donnees.m_donnees.TAB_PCC_VILLES)
            {
                if (lignePCCVille.IsB_CREATIONNull()) { lignePCCVille.B_CREATION = false; }
            }
            if (Donnees.m_donnees.TAB_JEU[0].IsI_DISTANCEVILLEMAX_PCCNull())
            {
                Donnees.m_donnees.TAB_JEU[0].I_DISTANCEVILLEMAX_PCC = 50;
            }
             * */
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
            /*
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
                    lDebug1 = Donnees.m_donnees.TAB_CASE.FindParID_CASE(lVDebug1.ID_CASE);
                    lDebug2 = Donnees.m_donnees.TAB_CASE.FindParID_CASE(lVDebug2.ID_CASE);
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
                
            }*/
            /*
            if (null != lignePccVilleMax)
            {
                lVDebug1 = Donnees.m_donnees.TAB_NOMS_CARTE.FindByID_NOM(lignePccVilleMax.ID_VILLE_DEBUT);
                lVDebug2 = Donnees.m_donnees.TAB_NOMS_CARTE.FindByID_NOM(lignePccVilleMax.ID_VILLE_FIN);
                lDebug1 = Donnees.m_donnees.TAB_CASE.FindParID_CASE(lVDebug1.ID_CASE);
                lDebug2 = Donnees.m_donnees.TAB_CASE.FindParID_CASE(lVDebug2.ID_CASE);
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
            string requeteDebug = "ID_VILLE_DEBUT=24 AND ID_VILLE_FIN=25";
            Donnees.TAB_PCC_VILLESRow[] lignesDejaTraite = (Donnees.TAB_PCC_VILLESRow[])Donnees.m_donnees.TAB_PCC_VILLES.Select(requeteDebug);
            for (int i = lignesDejaTraite[0].ID_TRAJET - 5; i < lignesDejaTraite[0].ID_TRAJET + 5; i++)
            {
                requeteDebug = "ID_TRAJET=" + i;
                Donnees.TAB_PCC_VILLESRow[] lignesVilles = (Donnees.TAB_PCC_VILLESRow[])Donnees.m_donnees.TAB_PCC_VILLES.Select(requeteDebug);
                lVDebug1 = Donnees.m_donnees.TAB_NOMS_CARTE.FindByID_NOM(lignesVilles[0].ID_VILLE_DEBUT);
                lVDebug2 = Donnees.m_donnees.TAB_NOMS_CARTE.FindByID_NOM(lignesVilles[0].ID_VILLE_FIN);
                    lDebug1 = Donnees.m_donnees.TAB_CASE.FindParID_CASE(lVDebug1.ID_CASE);
                    lDebug2 = Donnees.m_donnees.TAB_CASE.FindParID_CASE(lVDebug2.ID_CASE);
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

            requeteDebug = "ID_TRAJET=316 OR ID_TRAJET=368 OR ID_TRAJET=362";
            Donnees.TAB_PCC_VILLESRow[] lignesTrajets = (Donnees.TAB_PCC_VILLESRow[])Donnees.m_donnees.TAB_PCC_VILLES.Select(requeteDebug);
            foreach (Donnees.TAB_PCC_VILLESRow lignePCCVille in lignesTrajets)
            {
                lVDebug1 = Donnees.m_donnees.TAB_NOMS_CARTE.FindByID_NOM(lignePCCVille.ID_VILLE_DEBUT);
                lVDebug2 = Donnees.m_donnees.TAB_NOMS_CARTE.FindByID_NOM(lignePCCVille.ID_VILLE_FIN);
                lDebug1 = Donnees.m_donnees.TAB_CASE.FindParID_CASE(lVDebug1.ID_CASE);
                lDebug2 = Donnees.m_donnees.TAB_CASE.FindParID_CASE(lVDebug2.ID_CASE);
                Debug.WriteLine(string.Format("trajet ID={9} {0},id={10}:{1}({2},{3}) -> {4},id={11}:{5}({6},{7}), cout={8}",
                lVDebug1.S_NOM,
                lDebug1.ID_CASE,
                lDebug1.I_X,
                lDebug1.I_Y,
                lVDebug2.S_NOM,
                lDebug2.ID_CASE,
                lDebug2.I_X,
                lDebug2.I_Y,
                lignePCCVille.I_COUT,
                lignePCCVille.ID_TRAJET,
                lVDebug1.ID_NOM,
                lVDebug2.ID_NOM
                ));
            }
             * */
            //remet quelques noms de ville sur les cases de villes
            /*
            Donnees.TAB_NOMS_CARTERow ligneNomCarte = Donnees.m_donnees.TAB_NOMS_CARTE.FindByID_NOM(38);
            ligneNomCarte.ID_CASE = 4262028;
            ligneNomCarte = Donnees.m_donnees.TAB_NOMS_CARTE.FindByID_NOM(150);
            ligneNomCarte.ID_CASE = 5720430;
            ligneNomCarte = Donnees.m_donnees.TAB_NOMS_CARTE.FindByID_NOM(162);
            ligneNomCarte.ID_CASE = 5916513;
            */

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
                        chemin.Add(Donnees.m_donnees.TAB_CASE.FindParID_CASE(parcoursExistant[i].ID_CASE));
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
                                //pour �viter d'avoir deux fois la m�me case de suite dans le parcours, possible dans certains cas rares
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
            //Correctif pour envoyer des messages non re�us aux joueurs
            //foreach (Donnees.TAB_ORDRERow ligneOrdre in Donnees.m_donnees.TAB_ORDRE)
            //{
            //    if (ligneOrdre.I_TOUR_FIN >= 155)
            //    {
            //        Donnees.TAB_PIONRow lignePion = Donnees.m_donnees.TAB_PION.FindByID_PION(ligneOrdre.ID_PION);
            //        if (!ligneOrdre.IsID_DESTINATAIRENull())
            //        {
            //            Donnees.TAB_PIONRow lignePionDestinataire = Donnees.m_donnees.TAB_PION.FindByID_PION(ligneOrdre.ID_DESTINATAIRE);
            //            if (null == lignePionDestinataire.proprietaire || null == lignePion.proprietaire
            //                || lignePion.proprietaire.ID_PION != lignePionDestinataire.proprietaire.ID_PION)
            //            {
            //                Donnees.TAB_MESSAGERow ligneMessage = ligneOrdre.messageTransmis;
            //                if (null != ligneMessage)
            //                {
            //                    //il s'agit d'un message textuel
            //                    //donner le message et dispara�tre si le recepteur est un joueur, sinon aller vers le chef recevant le message
            //                    //if (lignePionDestinataire.ID_PION_PROPRIETAIRE == lignePionDestinataire.ID_PION ||
            //                    //    lignePionDestinataire.IsID_PION_PROPRIETAIRENull() || lignePionDestinataire.ID_PION_PROPRIETAIRE <= 0)
            //                    if (null != Donnees.m_donnees.TAB_ROLE.TrouvePion(lignePionDestinataire.ID_PION))
            //                    {
            //                        Monitor.Enter(Donnees.m_donnees.TAB_MESSAGE.Rows.SyncRoot);
            //                        ligneMessage.I_TOUR_ARRIVEE = Donnees.m_donnees.TAB_PARTIE[0].I_TOUR;
            //                        ligneMessage.I_PHASE_ARRIVEE = Donnees.m_donnees.TAB_PARTIE[0].I_PHASE + 1;//pour qu'il soit remis � la prochaine mise � jour
            //                        ligneMessage.ID_PION_PROPRIETAIRE = lignePionDestinataire.ID_PION;
            //                        Monitor.Exit(Donnees.m_donnees.TAB_MESSAGE.Rows.SyncRoot);

            //                        ligneMessage.emetteur.ReceptionMessageTransfert(ligneMessage);
            //                        Debug.WriteLine("Message pour " + lignePionDestinataire.S_NOM + ":" + ligneMessage.S_TEXTE);
            //                    }
            //                }
            //                else
            //                {
            //                    Debug.WriteLine("argh !");
            //                }
            //            }
            //        }
            //    }
            //    //else
            //    //{
            //    //    Donnees.TAB_PIONRow lignePion = Donnees.m_donnees.TAB_PION.FindByID_PION(ligneOrdre.ID_PION);
            //    //    if (!ligneOrdre.IsID_DESTINATAIRENull() && null!=lignePion)
            //    //    {
            //    //        Donnees.TAB_PIONRow lignePionDestinataire = Donnees.m_donnees.TAB_PION.FindByID_PION(ligneOrdre.ID_DESTINATAIRE);
            //    //        if (null == lignePionDestinataire.proprietaire || null == lignePion.proprietaire
            //    //            || lignePion.proprietaire.ID_PION != lignePionDestinataire.proprietaire.ID_PION)
            //    //        {
            //    //            Debug.WriteLine("etrange");
            //    //        }
            //    //    }
            //    //}
            //}
            Debug.WriteLine("Fin Correctifs");
        }

        private static void TestsDePerformance()
        {
            //DateTime timeStart;
            //TimeSpan perf;

            #region Performances sur table et mod�le de donn�es � aucune diff�rence entre les deux m�thodes qui suivent
            /*
            for (int i = 0; i < 1000000; i++)
            {
                Donnees.TAB_CASERow ligne = Donnees.m_donnees.TAB_CASE.AddTAB_CASERow(i, 0, i, i, -1, -1, 0, 0);
                //Donnees.m_listeNouveauProprietaire.Add(ligne);
            }
            timeStart = DateTime.Now;
            for (int i = 0; i < 1000000; i++)
            {
                Donnees.TAB_CASERow ligne = Donnees.m_donnees.TAB_CASE.FindParID_CASE(i);
            }
            perf = DateTime.Now - timeStart;
            Debug.WriteLine(string.Format("Find en {0} heures, {1} minutes, {2} secondes, {3} millisecondes", perf.Hours, perf.Minutes, perf.Seconds, perf.Milliseconds));
            
            timeStart = DateTime.Now;
            foreach (Donnees.TAB_CASERow ligne in Donnees.m_listeNouveauProprietaire)
            {
                Debug.WriteLine("ligne" + ligne.ID_CASE);
            }
            perf = DateTime.Now - timeStart;
            Debug.WriteLine(string.Format("m_listeNouveauProprietaire en {0} heures, {1} minutes, {2} secondes, {3} millisecondes", perf.Hours, perf.Minutes, perf.Seconds, perf.Milliseconds));

            timeStart = DateTime.Now;
            foreach (Donnees.TAB_CASERow ligne in Donnees.m_donnees.TAB_CASE)
            {
                Debug.WriteLine("ligne" + ligne.ID_CASE);
            }
            perf = DateTime.Now - timeStart;
            Debug.WriteLine(string.Format("TAB_CASE en {0} heures, {1} minutes, {2} secondes, {3} millisecondes", perf.Hours, perf.Minutes, perf.Seconds, perf.Milliseconds));
            */
            #endregion


            #region Performances sur chargement en mod�le OBJ � Au final, et contre toute attente, c�est beaucoup plus long !        
            //BD.Base.Initialisation(ImageCarte.Image.Width, ImageCarte.Image.Height);
            //BD.Base.Importer(Donnees.m_donnees);

            //timeStart = DateTime.Now;

            //Stream TestFileStream = File.Create("L:\\testvaoc.bin");
            //BinaryFormatter serializer = new BinaryFormatter();
            //serializer.Serialize(TestFileStream, BD.Base);
            //TestFileStream.Close();
            //perf = DateTime.Now - timeStart;
            //Debug.WriteLine(string.Format("Serialize en m�moire en {0} heures, {1} minutes, {2} secondes, {3} millisecondes", perf.Hours, perf.Minutes, perf.Seconds, perf.Milliseconds));
            //timeStart = DateTime.Now;

            //perf = DateTime.Now - timeStart;
            //TestFileStream = File.OpenRead("L:\\testvaoc.bin");
            //BinaryFormatter serializer2 = new BinaryFormatter();
            //BD.Base = (BD)serializer2.Deserialize(TestFileStream);
            //TestFileStream.Close();
            //perf = DateTime.Now - timeStart;
            //Debug.WriteLine(string.Format("DeSerialize en m�moire en {0} heures, {1} minutes, {2} secondes, {3} millisecondes", perf.Hours, perf.Minutes, perf.Seconds, perf.Milliseconds));
            #endregion

            //Transfert dans la base m�moire
            #region Performances sur linq Dataobject plutot que XML --> et il semblerait que ce soit 20 fois plus rapide en DataObject (mais c'est quand m�me tr�s rapide dans l'autre cas) -> faux, c'est 20 fois plus rapide si on ne l'excute pas, sinon c'est 3 fois plus lent !
            /* 

            string requete;
            int i;
            int xBloc = 10;
            int yBloc = 10;
            Donnees.TAB_PCC_COUTSRow[] lignesCout;

            //je cr�e la liste des dataObjects correspondants
            foreach (Donnees.TAB_CASERow ligneCase in Donnees.m_donnees.TAB_CASE)
            {
                BD.Base.Case.Ajouter(new LigneCASE(
                    ligneCase.ID_CASE,
                    ligneCase.ID_MODELE_TERRAIN,
                    ligneCase.I_X,
                    ligneCase.I_Y,
                    ligneCase.IsID_PROPRIETAIRENull() ? null : (int?)ligneCase.ID_PROPRIETAIRE,
                    ligneCase.IsID_NOUVEAU_PROPRIETAIRENull() ? null : (int?)ligneCase.ID_NOUVEAU_PROPRIETAIRE,
                    ligneCase.ID_MODELE_TERRAIN_SI_OCCUPE,
                    ligneCase.I_COUT,
                    ligneCase.IsID_NOMNull() ? null : (int?)ligneCase.ID_NOM
                 ));
            }

            foreach (Donnees.TAB_PCC_COUTSRow ligneCout in Donnees.m_donnees.TAB_PCC_COUTS)
            {
                BD.Base.PccCouts.Ajouter(new LignePCC_COUTS(ligneCout.I_BLOCX, ligneCout.I_BLOCY,
                    ligneCout.ID_CASE_DEBUT, ligneCout.ID_CASE_FIN,
                    ligneCout.I_COUT,
                    ligneCout.I_COUT_INITIAL,
                    ligneCout.ID_TRAJET,
                    ligneCout.B_CREATION));
            }

            foreach (Donnees.TAB_JEURow ligneJeu in Donnees.m_donnees.TAB_JEU)
            {
                BD.Base.Jeu.Ajouter(new LigneJEU(
                    ligneJeu.ID_JEU,
                    ligneJeu.S_NOM,
                    ligneJeu.I_NOMBRE_TOURS,
                    ligneJeu.I_NOMBRE_PHASES,
                    ligneJeu.DT_INITIALE,
                    //ligneJeu.IsID_CARTE_COMPTE_RENDUNull() ? null : (int?)ligneJeu.ID_CARTE_COMPTE_RENDU, -> on ne s'en sert pas
                    ligneJeu.I_LEVER_DU_SOLEIL,
                    ligneJeu.I_COUCHER_DU_SOLEIL,
                    ligneJeu.I_ECHELLE,
                    ligneJeu.ID_MODELE_TERRAIN_DEPLOIEMENT,
                    ligneJeu.I_COUT_DE_BASE,
                    ligneJeu.I_RAYON_BATAILLE,
                    ligneJeu.I_HEURE_INITIALE,
                    ligneJeu.I_HAUTEUR_CARTE,
                    ligneJeu.I_LARGEUR_CARTE,
                    ligneJeu.S_NOM_CARTE_TOPOGRAPHIQUE,
                    ligneJeu.S_NOM_CARTE_ZOOM,
                    ligneJeu.S_NOM_CARTE_GRIS,
                    ligneJeu.S_NOM_CARTE_HISTORIQUE,
                    ligneJeu.I_OBJECTIF,
                    ligneJeu.I_TAILLEBLOC_PCC,
                    //ligneJeu.I_ZONEVILLE_PCC,
                    //ligneJeu.I_DISTANCEVILLEMAX_PCC,
                    ligneJeu.I_VERSION));
            }

            BD.Base.PccCouts.Initialisation();

            ////sauvegarde sur fichier (test de serialisation)
            //Stream fichier = File.Create("c:\\toto.bin");
            //BinaryFormatter ecrivain = new BinaryFormatter();
            //ecrivain.Serialize(fichier, tablePCCCouts);
            //fichier.Close();

            ////on le lit pour v�rifier
            //tablePCCCouts.Vider();
            //Stream fichierLecture = File.OpenRead("c:\\toto.bin");
            //tablePCCCouts = (TablePCC_COUTS)ecrivain.Deserialize(fichierLecture);
            //fichierLecture.Close();

            //je cherche juste une case valide sur le bloc
            requete = string.Format("I_BLOCX={0} AND I_BLOCY={1}", xBloc, yBloc);
            lignesCout = (Donnees.TAB_PCC_COUTSRow[])Donnees.m_donnees.TAB_PCC_COUTS.Select(requete);
            int idcase = lignesCout[0].ID_CASE_FIN;

            requete = string.Format("ID_CASE_FIN={0} AND I_BLOCX={1} AND I_BLOCY={2}", idcase, xBloc, yBloc);

            timeStart = DateTime.Now;
            for (i = 0; i < 10000; i++)
            {
                lignesCout = (Donnees.TAB_PCC_COUTSRow[])Donnees.m_donnees.TAB_PCC_COUTS.Select(requete);
            }
            perf = DateTime.Now - timeStart;
            Debug.WriteLine(string.Format("Methode 1 (XML) en {0} heures, {1} minutes, {2} secondes, {3} millisecondes", perf.Hours, perf.Minutes, perf.Seconds, perf.Milliseconds));

            List<LignePCC_COUTS> listeCout = new List<LignePCC_COUTS>();
            timeStart = DateTime.Now;
            for (i = 0; i < 10000; i++)
            {
                //listeCout.Clear();

                IEnumerable<LignePCC_COUTS> listeCouts = BD.Base.PccCouts.ListeLiensPointBloc(xBloc, yBloc, idcase);
                int nb = listeCouts.Count();//juste pour forcer l'execution de la requete Linq, sinon, bien sur, c'est tr�s tr�s rapide !!!

                //foreach (ClassPCC_COUTS pccCout in requeteLinq)
                //{
                //    listeCout.Add(pccCout);
                //}
            }

            perf = DateTime.Now - timeStart;
            Debug.WriteLine(string.Format("Methode 2 (DataObjet) en {0} heures, {1} minutes, {2} secondes, {3} millisecondes", perf.Hours, perf.Minutes, perf.Seconds, perf.Milliseconds));
             * */
            #endregion

            #region Performances sur Datatable.Select par rapport � foreach -> Bilan, le foreach est plus long (beaucoup plus long !)
            /* 
            string requete;
            int i;
            int xBloc = 10;
            int yBloc = 10;
            Donnees.TAB_PCC_COUTSRow[] lignesCout;
            
            //je cherche juste une case valide sur le bloc
            requete = string.Format("I_BLOCX={0} AND I_BLOCY={1}", xBloc, yBloc);
            lignesCout = (Donnees.TAB_PCC_COUTSRow[])Donnees.m_donnees.TAB_PCC_COUTS.Select(requete);
            int idcase2 = lignesCout[0].ID_CASE_FIN;

            requete = string.Format("ID_CASE_FIN={0} AND I_BLOCX={1} AND I_BLOCY={2}", idcase2, xBloc, yBloc);

            timeStart = DateTime.Now;
            for (i = 0; i < 100; i++)
            {
                lignesCout = (Donnees.TAB_PCC_COUTSRow[])Donnees.m_donnees.TAB_PCC_COUTS.Select(requete);
            }
            perf = DateTime.Now - timeStart;
            Debug.WriteLine(string.Format("Methode 1 en {0} heures, {1} minutes, {2} secondes, {3} millisecondes", perf.Hours, perf.Minutes, perf.Seconds, perf.Milliseconds));

            List<Donnees.TAB_PCC_COUTSRow> listeCoutPCC = new List<Donnees.TAB_PCC_COUTSRow>();

            timeStart = DateTime.Now;
            for (i = 0; i < 100; i++)
            {
                //listeCoutPCC.Clear();
                foreach (Donnees.TAB_PCC_COUTSRow ligneCout in Donnees.m_donnees.TAB_PCC_COUTS)
                {
                    if ((ligneCout.I_BLOCX == xBloc) && (ligneCout.I_BLOCY == yBloc) && (ligneCout.ID_CASE_FIN == idcase2))
                    {
                        //listeCoutPCC.Add(ligneCout);
                    }
                }
            }

            perf = DateTime.Now - timeStart;
            Debug.WriteLine(string.Format("Methode 2 en {0} heures, {1} minutes, {2} secondes, {3} millisecondes", perf.Hours, perf.Minutes, perf.Seconds, perf.Milliseconds));
              */
            #endregion

            #region Performances sur CasesVoisines
            /*
            timeStart = DateTime.Now;
            //foreach (Donnees.TAB_CASERow ligneCase in Donnees.m_donnees.TAB_CASE)
            for (int i=0; i<100000; i++)
            {
                Donnees.TAB_CASERow[] retourCases = Donnees.m_donnees.TAB_CASE.CasesVoisines(Donnees.m_donnees.TAB_CASE[i]);
            }
            perf = DateTime.Now - timeStart;
            Debug.WriteLine(string.Format("Methode 1 en {0} heures, {1} minutes, {2} secondes, {3} millisecondes", perf.Hours, perf.Minutes, perf.Seconds, perf.Milliseconds));


            timeStart = DateTime.Now;
            for (int i = 0; i < 100000; i++)
            {
                List<Donnees.TAB_CASERow> listeCasesVoisins2 = Donnees.m_donnees.TAB_CASE.CasesVoisines2(Donnees.m_donnees.TAB_CASE[i]);
            }
            perf = DateTime.Now - timeStart;
            Debug.WriteLine(string.Format("Methode 2 en {0} heures, {1} minutes, {2} secondes, {3} millisecondes", perf.Hours, perf.Minutes, perf.Seconds, perf.Milliseconds));
             */
            #endregion

            #region Performances sur tablecase m�moire
            /*
            BD.Base.Initialisation(ImageCarte.Image.Width, ImageCarte.Image.Height);
            timeStart = DateTime.Now;
            BD.Base.Case.Importer(Donnees.m_donnees.TAB_CASE);
            perf = DateTime.Now - timeStart;
            Debug.WriteLine(string.Format("Chargement des cases en m�moire en {0} heures, {1} minutes, {2} secondes, {3} millisecondes", perf.Hours, perf.Minutes, perf.Seconds, perf.Milliseconds));
            */
            //timeStart = DateTime.Now;
            //BD.Base.Case.Exporter(Donnees.m_donnees.TAB_CASE);
            //perf = DateTime.Now - timeStart;
            //Debug.WriteLine(string.Format("Exporter les cases vers XML en {0} heures, {1} minutes, {2} secondes, {3} millisecondes", perf.Hours, perf.Minutes, perf.Seconds, perf.Milliseconds));

            //timeStart = DateTime.Now;
            //foreach (Donnees.TAB_CASERow ligneCase in Donnees.m_donnees.TAB_CASE)
            //{
            //    ligneCase.I_COUT = AStar.CST_COUTMAX;
            //}
            //perf = DateTime.Now - timeStart;
            //Debug.WriteLine(string.Format("CST_COUTMAX XML en {0} heures, {1} minutes, {2} secondes, {3} millisecondes", perf.Hours, perf.Minutes, perf.Seconds, perf.Milliseconds));

            //timeStart = DateTime.Now;
            //foreach (LigneCASE ligneCase in BD.Base.Case)
            //{
            //    ligneCase.I_COUT = AStar.CST_COUTMAX;
            //}
            //perf = DateTime.Now - timeStart;
            //Debug.WriteLine(string.Format("CST_COUTMAX XML en {0} heures, {1} minutes, {2} secondes, {3} millisecondes", perf.Hours, perf.Minutes, perf.Seconds, perf.Milliseconds));
            #endregion

            #region Performances sur EnnemiObservable
            /*
            string requete;
            int xCaseHautGauche, yCaseHautGauche, xCaseBasDroite, yCaseBasDroite;
            xCaseHautGauche = 0;
            yCaseHautGauche = 0;
            xCaseBasDroite = 100;
            yCaseBasDroite = 100;
            Donnees.TAB_PIONRow lignePion = Donnees.m_donnees.TAB_PION[0];
            Donnees.TAB_MODELE_PIONRow ligneModelePion = lignePion.modelePion;
            int nb1, nb2, nb3;
            nb1 = nb2 = nb3 = 0;

            timeStart = DateTime.Now;
            requete = string.Format("I_X>={0} AND I_Y>={1} AND I_X<={2} AND I_Y<={3}", xCaseHautGauche, yCaseHautGauche, xCaseBasDroite, yCaseBasDroite);
            Donnees.TAB_CASERow[] ligneCaseVues = (Donnees.TAB_CASERow[])Donnees.m_donnees.TAB_CASE.Select(requete);
            foreach (Donnees.TAB_CASERow ligneCaseVue in ligneCaseVues)
            {
                //lignePion.estEnnemi(ligneCaseVue, ligneModelePion, true, true);
                nb1++;
            }
            perf = DateTime.Now - timeStart;
            Debug.WriteLine(string.Format("Requ�te en {0} heures, {1} minutes, {2} secondes, {3} millisecondes", perf.Hours, perf.Minutes, perf.Seconds, perf.Milliseconds));

            timeStart = DateTime.Now;
            for (int x = xCaseHautGauche; x <= xCaseBasDroite; x++)
            {
                for (int y = yCaseHautGauche; y <= yCaseBasDroite; y++)
                {
                    Donnees.TAB_CASERow ligneCaseVue = Donnees.m_donnees.TAB_CASE.FindByXY(x, y);
                    //lignePion.estEnnemi(ligneCaseVue, ligneModelePion, true, true);
                    nb2++;
                }
            }
            perf = DateTime.Now - timeStart;
            Debug.WriteLine(string.Format("boucle en {0} heures, {1} minutes, {2} secondes, {3} millisecondes", perf.Hours, perf.Minutes, perf.Seconds, perf.Milliseconds));

            BD.Base.Initialisation(ImageCarte.Image.Width, ImageCarte.Image.Height);
            BD.Base.Case.Importer(Donnees.m_donnees.TAB_CASE);
            timeStart = DateTime.Now;
            for (int x = xCaseHautGauche; x <= xCaseBasDroite; x++)
            {
                for (int y = yCaseHautGauche; y <= yCaseBasDroite; y++)
                {
                    LigneCASE ligneCase = BD.Base.Case.FindByXY(x, y);
                    //lignePion.estEnnemi(ligneCase, ligneModelePion, true, true);
                }
            }
            perf = DateTime.Now - timeStart;
            Debug.WriteLine(string.Format("boucle case m�moire en {0} heures, {1} minutes, {2} secondes, {3} millisecondes", perf.Hours, perf.Minutes, perf.Seconds, perf.Milliseconds));

            timeStart = DateTime.Now;
            var requeteOBJ = from LigneCASE ligne in BD.Base.Case
                          where ligne.I_X>= xCaseHautGauche && ligne.I_Y>= yCaseHautGauche && ligne.I_X<= xCaseBasDroite && ligne.I_Y<= yCaseBasDroite
                             select ligne;
            foreach (LigneCASE ligneCaseVue in requeteOBJ.ToList())
            {
                nb3++;
            }
            perf = DateTime.Now - timeStart;
            Debug.WriteLine(string.Format("requ�te case m�moire en {0} heures, {1} minutes, {2} secondes, {3} millisecondes", perf.Hours, perf.Minutes, perf.Seconds, perf.Milliseconds));
            */
            //Requ�te en 0 heures, 0 minutes, 0 secondes, 46 millisecondes
            //boucle en 0 heures, 0 minutes, 0 secondes, 338 millisecondes
            //boucle case m�moire en 0 heures, 0 minutes, 0 secondes, 3 millisecondes
            //requ�te case m�moire en 0 heures, 0 minutes, 0 secondes, 28 millisecondes
            #endregion

            #region Performances SearchSpace
            //BD.Base.Initialisation(ImageCarte.Image.Width, ImageCarte.Image.Height);
            //BD.Base.Importer(Donnees.m_donnees);

            /*
            if (null == m_etoileHPA) m_etoileHPA = new AStar();
            if (null == m_etoileOBJ) m_etoileOBJ = new AStarOBJ();
            AstarTerrainOBJ[] tableCoutsMouvementsTerrainOBJ;
            AstarTerrain[] tableCoutsMouvementsTerrain;
            Cartographie.CalculModeleMouvementsPion(out tableCoutsMouvementsTerrainOBJ);
            Cartographie.CalculModeleMouvementsPion(out tableCoutsMouvementsTerrain);
            Donnees.TAB_PIONRow lignePion = Donnees.m_donnees.TAB_PION[0];
            int nombrePixelParCase = Donnees.m_donnees.TAB_JEU[0].I_ECHELLE;
            int espace = 100;
            string erreur;
            Donnees.TAB_CASERow[] listeCaseEspace;
            IEnumerable<LigneCASE> listeCaseEspaceOBJ;
            Donnees.TAB_CASERow ligneCase = Donnees.m_donnees.TAB_CASE.FindParID_CASE(lignePion.ID_CASE);
            LigneCASE ligneCaseOBJ = BD.Base.Case.FindByXY(ligneCase.I_X, ligneCase.I_Y);
            timeStart = DateTime.Now;

            if (!m_etoileHPA.SearchSpace(ligneCase, espace, tableCoutsMouvementsTerrain, nombrePixelParCase, lignePion.nation.ID_NATION, out listeCaseEspace, out erreur))
            {
                MessageBox.Show("erreur m_etoileHPA.SearchSpace", "AStarHPA", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            perf = DateTime.Now - timeStart;
            Debug.WriteLine(string.Format("m_etoileHPA.SearchSpace en {0} heures, {1} minutes, {2} secondes, {3} millisecondes", perf.Hours, perf.Minutes, perf.Seconds, perf.Milliseconds));

            timeStart = DateTime.Now;
            if (!m_etoileOBJ.SearchSpace(ligneCaseOBJ, espace, tableCoutsMouvementsTerrainOBJ, nombrePixelParCase, lignePion.nation.ID_NATION, out listeCaseEspaceOBJ, out erreur))
            {
                MessageBox.Show("erreur m_etoileHPA.SearchSpace", "AStarHPA", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            perf = DateTime.Now - timeStart;
            Debug.WriteLine(string.Format("m_etoileOBJ.SearchSpace en {0} heures, {1} minutes, {2} secondes, {3} millisecondes", perf.Hours, perf.Minutes, perf.Seconds, perf.Milliseconds));

            timeStart = DateTime.Now;

            if (!m_etoileHPA.SearchSpace(ligneCase, espace, tableCoutsMouvementsTerrain, nombrePixelParCase, lignePion.nation.ID_NATION, out listeCaseEspace, out erreur))
            {
                MessageBox.Show("erreur m_etoileHPA.SearchSpace", "AStarHPA", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            perf = DateTime.Now - timeStart;
            Debug.WriteLine(string.Format("m_etoileHPA.SearchSpace II en {0} heures, {1} minutes, {2} secondes, {3} millisecondes", perf.Hours, perf.Minutes, perf.Seconds, perf.Milliseconds));
            */
            //m_etoileOBJ.SearchSpace en 0 heures, 0 minutes, 0 secondes, 65 millisecondes
            //AStar.SearchSpace en 0 minutes, 2 secondes, 389 millisecondes

            #endregion
        }

        private bool SauvegarderPartie(string nomFichier)
        {
            m_modification = false;
            //il faut remettre tous les chemins � null, toutes les cases �tant vid�es, cela provoque un crash sur s�lection d'un pion sinon ensuite
            m_cheminHorsRoute = null;
            m_cheminPCC = null;
            m_cheminVille = null;
            m_cheminHPA = null;

            //Mise � jour de la version du fichier pour de future mise � jour
            return Donnees.m_donnees.SauvegarderPartie(nomFichier, false);
        }

        /// <summary>
        /// Tests en tout genre
        /// </summary>
        private void buttonData_Click(object sender, EventArgs e)
        {

            string url = string.Format("http://{0}/vaocservicemiseajour.php?id_partie={1}&date_prochaintour={2}",
                Donnees.m_donnees.TAB_PARTIE[0].S_HOST_SITEWEB,
                Donnees.m_donnees.TAB_PARTIE[0].ID_PARTIE,
                Constantes.DateHeureSQL(Donnees.m_donnees.TAB_PARTIE[0].DT_PROCHAINTOUR));
            HttpClient client = new();
            HttpRequestMessage message = new(HttpMethod.Get, url);
            HttpResponseMessage response = client.Send(message);
            //Task<HttpResponseMessage> response = client..GetAsync(url).Wait(new TimeSpan(0, 0, 3, 0, 0));
            if (!response.IsSuccessStatusCode)
            {
                MessageBox.Show("Timeout durant l'envoi de la date de mise � jour. Tout est correct sinon.",
                    "Attention", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            //    InterfaceVaocWeb iWeb = ClassVaocWebFactory.CreerVaocWeb("toto.txt", string.Empty, true);
            //    try
            //    {
            //        WebRequest requete = WebRequest.Create("http://localhost:8080/vaoc/service.php?op=utilisateurs");
            //        WebResponse reponse = requete.GetResponse();
            //        if (reponse is HttpWebResponse)
            //        {
            //            StreamReader sr = new StreamReader(reponse.GetResponseStream(), Encoding.GetEncoding("ISO-8859-1"));
            //            string strXML = sr.ReadToEnd();
            //            XmlDocument docXML = new XmlDocument();
            //            docXML.LoadXml(strXML);
            //            XmlElement elem=docXML["UTILISATEURS"];
            //            foreach (XmlNode noeud in elem.ChildNodes)
            //            {
            //                string nom=noeud["S_NOM"].InnerText;
            //            }

            //        }
            //    }
            //    catch (Exception exp)
            //    {
            //        MessageBox.Show("Erreur sur buttonData_Click :" + exp.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    }

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
                if (ChargementPartie(openFileDialog.FileName))
                {
                    mruMenu.AddFile(curFileName);
                    mruMenu.SaveToRegistry();
                }
            }
        }

        private void AProposToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormAPropos apropos = new();
            apropos.ShowDialog();
        }

        private void generalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormGeneral general = new();
            string nomCarte, nomCarteZoom, nomCarteGris, nomCarteTopographique;
            string repertoireDest;
            bool fl_demmarage = false;
            string messageArbitre = string.Empty;
            int iTour = 0;
            int iPhase = 0;
            int idVictoire = -1;
            int idMeteo = -1;
            int iTailleZonePCC = -1;
            int iTourNotification = 0;
            int iNbMeteoSuccessive = 0;

            //initialisation des donn�es de la form
            general.fichierCourant = curFileName;
            if (Donnees.m_donnees.TAB_JEU.Count > 0)
            {
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
                iTailleZonePCC = Donnees.m_donnees.TAB_JEU[0].IsI_TAILLEBLOC_PCCNull() ? -1 : Donnees.m_donnees.TAB_JEU[0].I_TAILLEBLOC_PCC;
            }
            else
            {
                general.id_jeu = -1;
                general.objectifVictoire = Constantes.OBJECTIFS.CST_OBJECTIF_DEMORALISATION;
            }
            nomCarte = general.nomCarte;
            nomCarteGris=general.nomCarteGris;
            nomCarteZoom=general.nomCarteZoom;
            nomCarteTopographique = general.nomCarteTopographique;

            if (Donnees.m_donnees.TAB_PARTIE.Count > 0)
            {
                if (!Donnees.m_donnees.TAB_PARTIE[0].IsI_NB_TOTAL_VICTOIRENull())
                {
                    general.nbPointsTotalVictoire = Donnees.m_donnees.TAB_PARTIE[0].I_NB_TOTAL_VICTOIRE;
                }
                if (Constantes.NULLCHAINE != Donnees.m_donnees.TAB_PARTIE[0].S_NOM)
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
                general.siteWeb = Donnees.m_donnees.TAB_PARTIE[0].S_HOST_SITEWEB;
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
                iTourNotification = Donnees.m_donnees.TAB_PARTIE[0].I_TOUR_NOTIFICATION;
                iNbMeteoSuccessive = Donnees.m_donnees.TAB_PARTIE[0].I_NB_METEO_SUCCESSIVE;
            }
            else
            {
                general.id_partie = -1;
            }

            if (DialogResult.OK == general.ShowDialog())
            {
                m_modification = true;
                //mise � jour des donn�es saisies
                Donnees.m_donnees.TAB_JEU.Clear();
                Donnees.TAB_JEURow ligneJeu = Donnees.m_donnees.TAB_JEU.NewTAB_JEURow();
                ligneJeu.ID_JEU = general.id_jeu;
                ligneJeu.S_NOM_CARTE_HISTORIQUE = nomCarte;
                ligneJeu.S_NOM_CARTE_GRIS = nomCarteGris;
                ligneJeu.S_NOM_CARTE_ZOOM = nomCarteZoom;
                ligneJeu.S_NOM_CARTE_TOPOGRAPHIQUE = nomCarteTopographique;
                ligneJeu.I_LARGEUR_CARTE = general.largeurCarte;
                ligneJeu.I_HAUTEUR_CARTE = general.hauteurCarte;
                if (-1 == iTailleZonePCC) { ligneJeu.SetI_TAILLEBLOC_PCCNull(); } else { ligneJeu.I_TAILLEBLOC_PCC = iTailleZonePCC; }

                //si les images de carte � changer, il faut les mettre � jour
                if (nomCarte != general.nomCarte)
                {
                    ligneJeu.S_NOM_CARTE_HISTORIQUE = general.nomCarte[(general.nomCarte.LastIndexOf('\\') + 1)..];
                    repertoireDest = Constantes.repertoireDonnees + ligneJeu.S_NOM_CARTE_HISTORIQUE;

                    if (general.nomCarte != repertoireDest)
                    {
                        // Cela ne doit �tre fait que pour la carte topographique
                        //if (null != ImageCarte.Image)
                        //{
                        //    //on relache l'image courante pour que l'on puisse, eventuellement, la modifier dans la form Carte
                        //    ImageCarte.Image.Dispose();
                        //}

                        //destruction d'un eventuel m�me fichier avec le m�me nom
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
                    if (string.Empty == general.nomCarteZoom)
                    {
                        ligneJeu.SetS_NOM_CARTE_ZOOMNull();
                    }
                    else
                    {
                        ligneJeu.S_NOM_CARTE_ZOOM = general.nomCarteZoom.Substring(general.nomCarteZoom.LastIndexOf('\\') + 1);
                        repertoireDest = Constantes.repertoireDonnees + ligneJeu.S_NOM_CARTE_ZOOM;
                        if (repertoireDest != general.nomCarteZoom)
                        {
                            //destruction d'un eventuel m�me fichier avec le m�me nom
                            if (File.Exists(repertoireDest))
                            {
                                File.Delete(repertoireDest);
                            }
                            //on recopie le fichier image vers le repertoire applicatif des cartes
                            File.Copy(general.nomCarteZoom, repertoireDest, true);
                        }
                    }
                }

                if (nomCarteGris != general.nomCarteGris)
                {
                    if (string.Empty == general.nomCarteGris)
                    {
                        ligneJeu.SetS_NOM_CARTE_GRISNull();
                    }
                    else
                    {
                        ligneJeu.S_NOM_CARTE_GRIS = general.nomCarteGris.Substring(general.nomCarteGris.LastIndexOf('\\') + 1);
                        repertoireDest = Constantes.repertoireDonnees + ligneJeu.S_NOM_CARTE_GRIS;
                        if (repertoireDest != general.nomCarteGris)
                        {
                            //destruction d'un eventuel m�me fichier avec le m�me nom
                            if (File.Exists(repertoireDest))
                            {
                                File.Delete(repertoireDest);
                            }
                            //on recopie le fichier image vers le repertoire applicatif des cartes
                            File.Copy(general.nomCarteGris, repertoireDest, true);
                        }
                    }
                }

                if (nomCarteTopographique != general.nomCarteTopographique)
                {
                    ligneJeu.S_NOM_CARTE_TOPOGRAPHIQUE = general.nomCarteTopographique.Substring(general.nomCarteTopographique.LastIndexOf('\\') + 1);
                    repertoireDest = Constantes.repertoireDonnees + ligneJeu.S_NOM_CARTE_TOPOGRAPHIQUE;
                    if (repertoireDest != general.nomCarteTopographique)
                    {
                        //destruction d'un eventuel m�me fichier avec le m�me nom
                        if (File.Exists(repertoireDest))
                        {
                            File.Delete(repertoireDest);
                        }
                        //on recopie le fichier image vers le repertoire applicatif des cartes
                        File.Copy(general.nomCarteTopographique, repertoireDest, true);
                    }
                }

                
                ligneJeu.DT_INITIALE = general.dateInitiale;
                ligneJeu.I_NOMBRE_TOURS = general.nbTours;
                ligneJeu.I_NOMBRE_PHASES = general.nbPhases;
                ligneJeu.S_NOM = general.nomScenario;
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
                lignePartie.S_HOST_SITEWEB = general.siteWeb;
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
                lignePartie.I_TOUR_NOTIFICATION = iTourNotification;
                lignePartie.I_NB_METEO_SUCCESSIVE = iNbMeteoSuccessive;
                //lignePartie.SetID_VICTOIRENull();//BEA, � retirer
                Donnees.m_donnees.TAB_PARTIE.AddTAB_PARTIERow(lignePartie);

                //On determine l'heure de lev�e et de coucher du soleil d'apr�s le mois en cours, le calcul se base sur les donn�es de TAB_PARTIE
                //-> faux, maintenant c'est automatique suivant le mois en cours, et cela peut donc varier � chaque tour -> deplacer dans traitementHeure
                int moisEnCours = ClassMessager.DateHeure().Month;
                Donnees.m_donnees.TAB_JEU[0].I_LEVER_DU_SOLEIL = Constantes.tableHeuresLeveeDuSoleil[moisEnCours - 1];
                Donnees.m_donnees.TAB_JEU[0].I_COUCHER_DU_SOLEIL = Constantes.tableHeuresCoucherDuSoleil[moisEnCours - 1];
            }
        }

        private void carteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormCarte fCarte = new()
            {

                //affectation des tables
                //fCarte.tableGraphisme = Donnees.m_donnees.TAB_GRAPHISME;
                //fCarte.tablePoint = Donnees.m_donnees.TAB_POINT;
                //fCarte.tableMeteo = Donnees.m_donnees.TAB_METEO;
                //fCarte.tableModelesMouvements = Donnees.m_donnees.TAB_MODELE_MOUVEMENT;
                tableCase = Donnees.m_donnees.TAB_CASE
            };
            //fCarte.tableModelesTerrains = Donnees.m_donnees.TAB_MODELE_TERRAIN;//� mettre apr�s graphisme et point
            //fCarte.imageCarte = (Bitmap)ImageCarte.Image;
            if (!Donnees.m_donnees.TAB_JEU[0].IsS_NOM_CARTE_TOPOGRAPHIQUENull())
            {
                fCarte.nomCarteTopographique = Donnees.m_donnees.TAB_JEU[0].S_NOM_CARTE_TOPOGRAPHIQUE;
            }

            if (DialogResult.OK == fCarte.ShowDialog())
            {
                m_modification = true;
                //mise � jour des modeles de terrain
                Cursor oldcurseur = this.Cursor;
                this.Cursor = Cursors.WaitCursor;
                if (fCarte.traitementEffectue.HasFlag(FormCarte.TRAITEMENTCARTE.GENERATION) || fCarte.traitementEffectue.HasFlag(FormCarte.TRAITEMENTCARTE.REMPLACEMENT))
                {
                    Donnees.m_donnees.TAB_CASE.Clear();
                    Donnees.m_donnees.TAB_CASE.Merge(fCarte.tableCase, false);
                }
                //sauvegarde des cases cr�es
                if (!Donnees.m_donnees.SauvegarderCases())
                {
                    MessageBox.Show(this, "Erreur � la sauvegarde des cases", "FormCarte", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                this.Cursor = oldcurseur;
            }
        }

        private void meteoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormMeteo fMeteo = new()
            {
                tableMeteo = Donnees.m_donnees.TAB_METEO
            };
            if (DialogResult.OK == fMeteo.ShowDialog())
            {
                m_modification = true;
                Donnees.m_donnees.TAB_METEO.Clear();
                Donnees.m_donnees.TAB_METEO.Merge(fMeteo.tableMeteo, false);
                if (Donnees.m_donnees.TAB_PARTIE[0].IsID_METEONull() && Donnees.m_donnees.TAB_METEO.Count > 0)
                {
                    Donnees.m_donnees.TAB_PARTIE[0].ID_METEO = Donnees.m_donnees.TAB_METEO[0].ID_METEO;
                }
            }
        }

        private void modelesDeMOuvementsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormModelesMouvements fModeleMouvement = new()
            {
                tableModeleMouvement = Donnees.m_donnees.TAB_MODELE_MOUVEMENT
            };
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

                Point pt = this.panelImage.AutoScrollPosition;
                ImageCarte.Width = Convert.ToInt32(Donnees.m_donnees.TAB_JEU[0].I_LARGEUR_CARTE * m_zoom);
                ImageCarte.Height = Convert.ToInt32(Donnees.m_donnees.TAB_JEU[0].I_HAUTEUR_CARTE * m_zoom);
                /* GetPreferredSize renvoie juste la taille de l'image d'origine, on ne peut donc pas s'en servir pour �viter un crash sur un zoom trop important
                Size prop = new System.Drawing.Size(ImageCarte.Width, ImageCarte.Height);
                Size test = ImageCarte.GetPreferredSize(prop);
                 * */

                //AutoScrollPosition renvoie la position en n�gatif mais il faut l'affecter en positif
                this.panelImage.AutoScrollPosition = new Point(Math.Abs(pt.X * 2) + this.panelImage.ClientRectangle.Width/2, Math.Abs(pt.Y * 2) + this.panelImage.ClientRectangle.Height/2);
                ImageCarte.Invalidate();
            }
        }

        private void toolStripButtonZoomMoins_Click(object sender, EventArgs e)
        {
            if (null != ImageCarte.Image)
            {
                m_zoom /= 2;

                Point pt = this.panelImage.AutoScrollPosition;
                ImageCarte.Width = Convert.ToInt32(Donnees.m_donnees.TAB_JEU[0].I_LARGEUR_CARTE * m_zoom);
                ImageCarte.Height = Convert.ToInt32(Donnees.m_donnees.TAB_JEU[0].I_HAUTEUR_CARTE * m_zoom);
                this.panelImage.AutoScrollPosition = new Point(Math.Abs(pt.X / 2) - this.panelImage.ClientRectangle.Width / 4, Math.Abs(pt.Y / 2) - this.panelImage.ClientRectangle.Height / 4);
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

        //        //pour utiliser des m�thodes avanc�es
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
        /// reconstruit une image de carte � partir du fond et des donn�es � afficher
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
                //partie non cr�e, ou charg�e, on ne peut rien afficher
                return;
            }

            if (toolStripAffichierTopographie.CheckState == CheckState.Checked)
            {
                if (Donnees.m_donnees.TAB_JEU.Count > 0 && !Donnees.m_donnees.TAB_JEU[0].IsS_NOM_CARTE_TOPOGRAPHIQUENull()
                    && Donnees.m_donnees.TAB_JEU[0].S_NOM_CARTE_TOPOGRAPHIQUE.Length > 0)
                {
                    if (File.Exists(Constantes.repertoireDonnees + Donnees.m_donnees.TAB_JEU[0].S_NOM_CARTE_TOPOGRAPHIQUE))
                    {
                        ImageCarte.Image = Image.FromFile(Constantes.repertoireDonnees + Donnees.m_donnees.TAB_JEU[0].S_NOM_CARTE_TOPOGRAPHIQUE);
                    }
                    else
                    {
                        ImageCarte.Image = null;
                    }
                }
            }
            else
            {
                if (!Donnees.m_donnees.TAB_JEU[0].IsS_NOM_CARTE_HISTORIQUENull() 
                    && Donnees.m_donnees.TAB_JEU[0].S_NOM_CARTE_HISTORIQUE.Length > 0)
                {
                    if (File.Exists(Constantes.repertoireDonnees + Donnees.m_donnees.TAB_JEU[0].S_NOM_CARTE_HISTORIQUE))
                    {
                        ImageCarte.Image = Image.FromFile(Constantes.repertoireDonnees + Donnees.m_donnees.TAB_JEU[0].S_NOM_CARTE_HISTORIQUE);
                    }
                    else
                    {
                        ImageCarte.Image = null;
                    }
                }
            }
            if (null == ImageCarte.Image)
            {
                return;
            }

            ImageCarte.Width = (int)Math.Floor(Donnees.m_donnees.TAB_JEU[0].I_LARGEUR_CARTE * m_zoom);
            ImageCarte.Height = (int)Math.Floor(Donnees.m_donnees.TAB_JEU[0].I_HAUTEUR_CARTE * m_zoom);

            if (toolStripButtonMemoire.CheckState == CheckState.Checked)
            {
                //on affiche les zones des cases non charg�es
                Graphics graph = Graphics.FromImage(ImageCarte.Image);
                Pen styloCadrillage = new(Color.DarkSeaGreen, 5);
                //styloCadrillage.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                //styloCadrillage.DashCap = System.Drawing.Drawing2D.DashCap.Flat;
                //float[] traits = new float[2];
                //traits[0] = 2;
                //traits[1] = 8;
                //styloCadrillage.DashPattern = traits;
                for (int x = 0; x < Donnees.m_donnees.TAB_JEU[0].I_LARGEUR_CARTE; x += Constantes.CST_TAILLE_BLOC_CASES)
                {
                    for (int y = 0; y < Donnees.m_donnees.TAB_JEU[0].I_HAUTEUR_CARTE; y += Constantes.CST_TAILLE_BLOC_CASES)
                    {
                        if (!Donnees.m_donnees.TAB_CASE.estCaseChargee(x,y))
                        {
                            int largeur = Math.Min(Constantes.CST_TAILLE_BLOC_CASES, Donnees.m_donnees.TAB_JEU[0].I_LARGEUR_CARTE - 1 - x);
                            int hauteur = Math.Min(Constantes.CST_TAILLE_BLOC_CASES, Donnees.m_donnees.TAB_JEU[0].I_HAUTEUR_CARTE - 1 - y);
                            //graph.DrawRectangle(styloCadrillage, x, y, largeur, hauteur);
                            graph.DrawEllipse(styloCadrillage, x, y, largeur, hauteur);
                        }
                    }
                }
                graph.Dispose();
            }

            if (toolStripAfficherBatailles.CheckState == CheckState.Checked)
            {
                Cartographie.AfficherBatailles((Bitmap)ImageCarte.Image);
            }
            if (toolStripAfficherUnites.CheckState == CheckState.Checked)
            {
                Cartographie.AfficherUnites((Bitmap)ImageCarte.Image);//ajout des unit�s
            }
            if (toolStripAfficherVilles.CheckState == CheckState.Checked)
            {
                Cartographie.AfficherNoms((Bitmap)ImageCarte.Image);//ajout des noms de villes
            }
            if (toolStripButtonDepots.CheckState == CheckState.Checked)
            {
                Cartographie.AfficherDepots((Bitmap)ImageCarte.Image);
            }

            if (this.toolStripButtonTrajets.CheckState == CheckState.Checked)
            {
                //on trace le cadrillage des blocs
                Graphics graph = Graphics.FromImage(ImageCarte.Image);
                int tailleBloc = Donnees.m_donnees.TAB_JEU[0].I_TAILLEBLOC_PCC;
                //int nbBlocsHorizontaux = (int)Math.Ceiling((decimal)Donnees.m_donnees.TAB_JEU[0].I_LARGEUR_CARTE / (decimal)tailleBloc);
                //int nbBlocsVerticaux = (int)Math.Ceiling((decimal)Donnees.m_donnees.TAB_JEU[0].I_HAUTEUR_CARTE / (decimal)tailleBloc);
                Pen styloCadrillage = new(Color.DarkSeaGreen, 1)
                {
                    DashStyle = System.Drawing.Drawing2D.DashStyle.Dash,
                    DashCap = System.Drawing.Drawing2D.DashCap.Flat
                };
                float[] traits= new float[2];
                traits[0]=2;
                traits[1]=8;
                styloCadrillage.DashPattern = traits;

                int xBloc = 43;
                int yBloc = 23;
                //for (int xBloc = 0; xBloc < nbBlocsHorizontaux; xBloc++)
                //{
                //    for (int yBloc = 0; yBloc < nbBlocsVerticaux; yBloc++)
                    {
                        int xmin = xBloc * tailleBloc;
                        int xmax = Math.Max(tailleBloc, Donnees.m_donnees.TAB_JEU[0].I_LARGEUR_CARTE - 1 - xmin);
                        int ymin = yBloc * tailleBloc;
                        int ymax = Math.Max(tailleBloc, Donnees.m_donnees.TAB_JEU[0].I_HAUTEUR_CARTE - 1 - ymin);
                        graph.DrawRectangle(styloCadrillage, xmin, ymin, xmax, ymax);
                    }
                //}
                graph.Dispose();

                // = new List<int>;
                List<Donnees.TAB_CASERow> cheminPCCTrajet = new();
                foreach (Donnees.TAB_PCC_COUTSRow lignePCCTrajet in Donnees.m_donnees.TAB_PCC_COUTS)
                //foreach(Donnees.TAB_PCC_CASE_BLOCSRow lignePCCTrajet in Donnees.m_donnees.TAB_PCC_CASE_BLOCS)
                {
                    if (lignePCCTrajet.I_BLOCX != xBloc || lignePCCTrajet.I_BLOCY != yBloc) continue;
                    Donnees.TAB_CASERow lDebug1, lDebug2;
                    lDebug1 = Donnees.m_donnees.TAB_CASE.FindParID_CASE(lignePCCTrajet.ID_CASE_DEBUT);
                    lDebug2 = Donnees.m_donnees.TAB_CASE.FindParID_CASE(lignePCCTrajet.ID_CASE_FIN);
                    Debug.WriteLine(string.Format("GoBackUpNodes ajout de ligneCout : ID={0}({1},{2}) -> ID={3}({4},{5}) ",
                        lDebug1.ID_CASE, lDebug1.I_X, lDebug1.I_Y,
                        lDebug2.ID_CASE, lDebug2.I_X, lDebug2.I_Y
                        ));
                    Debug.WriteLine(string.Format("PCC ID={0}({1},{2}) -> ID={3}({4},{5}) ID Trajet= {6}, cout={7}, ", 
                        lDebug1.ID_CASE, lDebug1.I_X, lDebug1.I_Y,
                        lDebug2.ID_CASE, lDebug2.I_X, lDebug2.I_Y,
                        lignePCCTrajet.ID_TRAJET, 
                        lignePCCTrajet.I_COUT));

                    cheminPCCTrajet.Clear();
                    Dal.ChargerTrajet(lignePCCTrajet.ID_TRAJET, out List<int> listeCases);
                    cheminPCCTrajet.Capacity = listeCases.Count;
                    int j = 0;
                    while (j < listeCases.Count)
                    {
                        Donnees.TAB_CASERow ligneCase = Donnees.m_donnees.TAB_CASE.FindParID_CASE(listeCases[j++]);
                        ligneCase.I_COUT = lignePCCTrajet.ID_TRAJET;//pour debug, je met l'id de trajet dans le cout pour l'afficher
                        cheminPCCTrajet.Add(ligneCase);
                    }
                    Cartographie.AfficherChemin((Bitmap)ImageCarte.Image, cheminPCCTrajet, Color.DarkSalmon, 1);
                }                
            }

            if (this.toolStripButtonTrajetsVilles.CheckState == CheckState.Checked)
            {
                List<Donnees.TAB_CASERow> cheminPCCTrajet = new();
                foreach (Donnees.TAB_PCC_VILLESRow lignePCCVille in Donnees.m_donnees.TAB_PCC_VILLES)
                {
                    cheminPCCTrajet.Clear();
                    Dal.ChargerTrajet(lignePCCVille.ID_TRAJET, Constantes.CST_TRAJET_VILLE, out List<int> listeCases);
                    cheminPCCTrajet.Capacity = listeCases.Count;
                    int j = 0;
                    while (j < listeCases.Count)
                    {
                        Donnees.TAB_CASERow ligneCase = Donnees.m_donnees.TAB_CASE.FindParID_CASE(listeCases[j++]);
                        ligneCase.I_COUT = lignePCCVille.ID_TRAJET;//pour debug, je met l'id de trajet dans le cout pour l'afficher
                        cheminPCCTrajet.Add(ligneCase);
                    }
                    Cartographie.AfficherChemin((Bitmap)ImageCarte.Image, cheminPCCTrajet, Color.Orange, 1);//chemin parcouru par une unit�
                }
            }

            if (toolStripPlusCourtChemin.CheckState == CheckState.Checked)
            {
                if (null != m_cheminHorsRoute && m_cheminHorsRoute.Count > 0)
                {
                    Cartographie.AfficherChemin((Bitmap)ImageCarte.Image, m_cheminHorsRoute, Color.LavenderBlush, 1);//chemin parcouru par une unit�
                }
                if (null != m_cheminPCC && m_cheminPCC.Count > 0)
                {
                    Cartographie.AfficherChemin((Bitmap)ImageCarte.Image, m_cheminPCC, Color.Red, 5);//chemin parcouru par une unit�
                }
                if (null != m_cheminVille && m_cheminVille.Count > 0)
                {
                    Cartographie.AfficherChemin((Bitmap)ImageCarte.Image, m_cheminVille, Color.Yellow, 3);//chemin parcouru par une unit�
                }
                if (null != m_cheminHPA && m_cheminHPA.Count > 0)
                {
                    Cartographie.AfficherChemin((Bitmap)ImageCarte.Image, m_cheminHPA, Color.DeepSkyBlue, 1);//chemin parcouru par une unit�
                }
            }

            if (null != m_lignePionSelection)
            {
                if( null != m_cheminSelection && m_cheminSelection.Count > 0)
                {
                    Cartographie.AfficherChemin((Bitmap)ImageCarte.Image, m_cheminSelection, Color.DarkGreen, 1);//chemin parcouru par une unit�
                    if (null != m_cheminVerifierTrajet && m_cheminVerifierTrajet.Count > 0)
                    {
                        Cartographie.AfficherChemin((Bitmap)ImageCarte.Image, m_cheminVerifierTrajet, Color.LavenderBlush, 2);
                    }
                    Cartographie.AfficherArriveeDepart((Bitmap)ImageCarte.Image, m_cheminSelection[0], m_cheminSelection[^1], Color.DarkGreen, 1);
                }
                else
                {
                    Cartographie.AfficherArriveeDepart((Bitmap)ImageCarte.Image,null,Donnees.m_donnees.TAB_CASE.FindParID_CASE(m_lignePionSelection.ID_CASE), Color.DarkGreen, 1);
                }
            }
            ImageCarte.Invalidate();
        }

        private void policeDeCaract�resToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormPolice fPolice = new()
            {
                tablePolice = Donnees.m_donnees.TAB_POLICE
            };
            if (DialogResult.OK == fPolice.ShowDialog())
            {
                m_modification = true;
                Donnees.m_donnees.TAB_POLICE.Clear();
                Donnees.m_donnees.TAB_POLICE.Merge(fPolice.tablePolice, false);
            }
        }

        private void fondDeCarteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormFondDeCarte fFondDeCarte = new();

            //initialisation des donn�es de la form
            if (Donnees.m_donnees.TAB_JEU.Count > 0 && !Donnees.m_donnees.TAB_JEU[0].IsS_NOM_CARTE_TOPOGRAPHIQUENull()
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
            fFondDeCarte.tableModelesTerrains = Donnees.m_donnees.TAB_MODELE_TERRAIN;//� mettre apr�s graphisme et point
            fFondDeCarte.tableMouvementCout = Donnees.m_donnees.TAB_MOUVEMENT_COUT;//attention, � mettre en derni�re affectation
            if (DialogResult.OK == fFondDeCarte.ShowDialog())
            {
                m_modification = true;
                //si l'image de carte � changer, il faut la mettre � jour
                string nomCarteTopographique = fFondDeCarte.nomCarteTopographique.Substring(fFondDeCarte.nomCarteTopographique.LastIndexOf('\\') + 1);
                if ((Donnees.m_donnees.TAB_JEU[0].IsS_NOM_CARTE_TOPOGRAPHIQUENull()
                    && !string.IsNullOrEmpty(fFondDeCarte.nomCarteTopographique))
                    || (!Donnees.m_donnees.TAB_JEU[0].IsS_NOM_CARTE_TOPOGRAPHIQUENull() &&
                    Donnees.m_donnees.TAB_JEU[0].S_NOM_CARTE_TOPOGRAPHIQUE != nomCarteTopographique))
                {
                    //on relache l'image courante pour que l'on puisse, eventuellement, la modifier dans la form Carte
                    ImageCarte.Image?.Dispose();

                    Donnees.m_donnees.TAB_JEU[0].S_NOM_CARTE_TOPOGRAPHIQUE = nomCarteTopographique;

                    string repertoireDest = WaocLib.Constantes.repertoireDonnees + Donnees.m_donnees.TAB_JEU[0].S_NOM_CARTE_TOPOGRAPHIQUE;
                    //destruction d'un eventuel m�me fichier avec le m�me nom
                    /* ->fichier g�n�ralement pas disponible
                    if (File.Exists(repertoireDest))
                    {
                        File.Delete(repertoireDest);
                    }
                    //on recopie le fichier image vers le repertoire applicatif des cartes
                    File.Copy(fFondDeCarte.nomCarteTopographique, repertoireDest, true);
                    */
                    ImageCarte.Image = (Bitmap)Image.FromFile(repertoireDest);

                    Donnees.m_donnees.TAB_JEU[0].I_LARGEUR_CARTE = fFondDeCarte.largeur;
                    Donnees.m_donnees.TAB_JEU[0].I_HAUTEUR_CARTE = fFondDeCarte.hauteur;
                }

                //mise � jour des points
                Donnees.m_donnees.TAB_POINT.Clear();
                Donnees.m_donnees.TAB_POINT.Merge(fFondDeCarte.tablePoint, false);

                //mise � jour des graphiques
                Donnees.m_donnees.TAB_GRAPHISME.Clear();
                Donnees.m_donnees.TAB_GRAPHISME.Merge(fFondDeCarte.tableGraphisme, false);

                //mise � jour des modeles de terrain
                Donnees.m_donnees.TAB_MODELE_TERRAIN.Clear();
                Donnees.m_donnees.TAB_MODELE_TERRAIN.Merge(fFondDeCarte.tableModelesTerrains, false);

                System.Collections.Specialized.ListDictionary tableIdentifiansModelesTerrains = fFondDeCarte.tableIdentifiansModelesTerrains;
                foreach (int clef in tableIdentifiansModelesTerrains.Keys)
                {
                    if ((int)tableIdentifiansModelesTerrains[clef] != clef)
                    {
                        //un identifiant d'origine (clef) a �t� modifi� (valeur), il faut mettre � jour toutes les cases
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

                //mise � jour des couts de mouvement
                Donnees.m_donnees.TAB_MOUVEMENT_COUT.Clear();
                Donnees.m_donnees.TAB_MOUVEMENT_COUT.Merge(fFondDeCarte.tableMouvementCout, false);

                //divers
                Donnees.m_donnees.TAB_JEU[0].I_COUT_DE_BASE = fFondDeCarte.coutDeBase;
            }
            ConstruireImageCarte();
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
            RemiseEnVeille();
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
        /// modification de l'emplacement du split container car le "dock" n'arrive pas � le faire correctement
        /// </summary>
        private void ResizeSplitContainer()
        {
            splitContainer.Left = toolStripCarte.Left;
            splitContainer.Width = toolStripCarte.Width;
            splitContainer.Top = toolStripCarte.Bottom;
            splitContainer.Height = this.Bottom - toolStripCarte.Bottom - SystemInformation.HorizontalScrollBarHeight;
            splitContainer.SplitterDistance = panelInformation.Width + 20;
        }

        private void utilisateursToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormUtilisateur fUtilisateur = new()
            {
                fichierCourant = curFileName
            };
            fUtilisateur.ShowDialog();
        }

        private void rolesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormRole fRole = new()
            {
                fichierCourant = curFileName,
                tableRole = Donnees.m_donnees.TAB_ROLE
            };
            if (DialogResult.OK == fRole.ShowDialog())
            {
                m_modification = true;
                Donnees.m_donnees.TAB_ROLE.Clear();
                Donnees.m_donnees.TAB_ROLE.Merge(fRole.tableRole, false);
            }
        }

        private void aptitudesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormAptitude fAptitude = new()
            {
                tableAptitudes = Donnees.m_donnees.TAB_APTITUDES
            };
            if (DialogResult.OK == fAptitude.ShowDialog())
            {
                m_modification = true;
                Donnees.m_donnees.TAB_APTITUDES.Clear();
                Donnees.m_donnees.TAB_APTITUDES.Merge(fAptitude.tableAptitudes, false);
            }
        }

        private void modelesDeCombatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormNation fNation = new()
            {
                tableNation = Donnees.m_donnees.TAB_NATION
            };
            if (DialogResult.OK == fNation.ShowDialog())
            {
                m_modification = true;
                Donnees.m_donnees.TAB_NATION.Clear();
                Donnees.m_donnees.TAB_NATION.Merge(fNation.tableNation, false);
            }
        }

        private void modelesDePIonsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormModelesPions fModelePions = new()
            {
                tableModelesPions = Donnees.m_donnees.TAB_MODELE_PION
            };
            if (DialogResult.OK == fModelePions.ShowDialog())
            {
                m_modification = true;
                Donnees.m_donnees.TAB_MODELE_PION.Clear();
                Donnees.m_donnees.TAB_MODELE_PION.Merge(fModelePions.tableModelesPions, false);
            }
        }

        private void aptitudesModelesDePIonsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormAptitudesModelesPions fAptitudesModelePions = new()
            {
                tableAptitudesModelesPions = Donnees.m_donnees.TAB_APTITUDES_PION
            };
            if (DialogResult.OK == fAptitudesModelePions.ShowDialog())
            {
                m_modification = true;
                Donnees.m_donnees.TAB_APTITUDES_PION.Clear();
                Donnees.m_donnees.TAB_APTITUDES_PION.Merge(fAptitudesModelePions.tableAptitudesModelesPions, false);
            }
        }

        private void pionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormPion fPion = new()
            {
                tablePions = Donnees.m_donnees.TAB_PION
            };
            if (DialogResult.OK == fPion.ShowDialog())
            {
                m_modification = true;

                //tous les pions qui ont chang�s de position sont "t�l�port�s"
                Donnees.TAB_PIONDataTable tableForm = (Donnees.TAB_PIONDataTable) fPion.tablePions.Copy();
                foreach (Donnees.TAB_PIONRow lignePionForm in tableForm)
                {
                    Donnees.TAB_PIONRow lignePion = Donnees.m_donnees.TAB_PION.FindByID_PION(lignePionForm.ID_PION);
                    if (null == lignePion || lignePion.ID_CASE != lignePionForm.ID_CASE)
                    {
                        //pion nouvellement cr�e ou qui a chang� de position
                        lignePionForm.B_TELEPORTATION = true;
                    }
                }
                Donnees.m_donnees.TAB_PION.Clear();
                Donnees.m_donnees.TAB_PION.Merge(tableForm, false);
                foreach (Donnees.TAB_PIONRow lignePion in Donnees.m_donnees.TAB_PION)
                {
                    if (lignePion.estMessager || lignePion.estPatrouille)
                    {
                        Donnees.TAB_MODELE_PIONRow ligneModeleProprietaire = lignePion.modelePion;
                        if (lignePion.possedeAptitude("MESSAGER"))
                        {
                        //    if (0 == ligneModeleProprietaire.ID_NATION && lignePion.ID_MODELE_PION != 5)
                        //    {
                        //        int pb = 0;
                        //        pb++;
                        //    }
                        //    if (0 != ligneModeleProprietaire.ID_NATION && lignePion.ID_MODELE_PION != 6)
                        //    {
                        //        int pb = 0;
                        //        pb++;
                        //    }
                            lignePion.ID_MODELE_PION = (0 == ligneModeleProprietaire.ID_NATION) ? 5 : 6;
                        }
                    }
                }
                //Il faut remettre � jour la liste car les r�f�rences ont pu changer
                MiseAJourListeUnites();
            }
        }

        private void ordresToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormOrdre fOrdre = new()
            {
                tableOrdre = Donnees.m_donnees.TAB_ORDRE
            };
            if (DialogResult.OK == fOrdre.ShowDialog())
            {
                m_modification = true;
                Donnees.m_donnees.TAB_ORDRE.Clear();
                Donnees.m_donnees.TAB_ORDRE.Merge(fOrdre.tableOrdre, false);
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
            labelCoutTerrain.Text = (Constantes.CST_COUTMAX == ligneCase.I_COUT) ? "Cout:?" : "Cout:" + ligneCase.I_COUT.ToString();
            if (ligneCase.EstInnocupe())
            {
                labelProprietaire.Text = "Propri�taire: ?";
            }
            else
            {
                lignePion = ligneCase.TrouvePionSurCase();
                labelProprietaire.Text = string.Format("Propri�taire: {0} ({1})", lignePion.S_NOM, lignePion.ID_PION);
            }
        }

        public bool AfficherTemps()
        {
            TimeSpan tPasse = DateTime.Now.Subtract(m_dateDebut);
            long nbrestant = (long)(tPasse.TotalSeconds * (Donnees.m_donnees.TAB_JEU[0].I_NOMBRE_PHASES - (Donnees.m_donnees.TAB_PARTIE[0].I_PHASE + 1)) / (Donnees.m_donnees.TAB_PARTIE[0].I_PHASE + 1));

            labelTour.Text = string.Format("{000:0}", Donnees.m_donnees.TAB_PARTIE[0].I_TOUR);
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
            return true;
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
            //DataSetCoutDonnees.m_donnees.TAB_PARTIE[0].I_PHASE=0; ne pas le faire � cause de la reprise possible sur fichier

            //La partie est-elle commenc�e ?
            if (!Donnees.m_donnees.TAB_PARTIE[0].FL_DEMARRAGE)
            {
                if (System.Windows.Forms.DialogResult.Yes == MessageBox.Show("La partie est actuellement en phase forum. Souhaitez vous que la partie d�bute pleinement maintenant ?", "Partie non commenc�e", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                Donnees.m_donnees.TAB_PARTIE[0].FL_DEMARRAGE = true;
                Donnees.m_donnees.TAB_PARTIE[0].I_TOUR_NOTIFICATION = 0;
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
            ClassTraitementHeure traitement = new();
            BackgroundWorker travailleur = sender as BackgroundWorker;

            //si la partie est termin�e, on ne devrait pas appeler cette m�thode !
            if (!Donnees.m_donnees.TAB_PARTIE[0].IsID_VICTOIRENull() && Donnees.m_donnees.TAB_PARTIE[0].ID_VICTOIRE >= 0)
            {
                MessageBox.Show("La partie est d�j� termin�e avec la victoire de la nation ID=" + Donnees.m_donnees.TAB_PARTIE[0].ID_VICTOIRE, "Partie termin�e", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                e.Cancel = false;
                e.Result = "La partie est d�j� termin�e avec la victoire de la nation ID=" + Donnees.m_donnees.TAB_PARTIE[0].ID_VICTOIRE;
            }

            e.Cancel = false;
            m_nbBatailles = Donnees.m_donnees.TAB_BATAILLE.Count;
            if (!traitement.TraitementHeure(fichierCourant, travailleur, out string messageErreur))
            {
                e.Cancel = true;
                e.Result = messageErreur;
            }
            else
            {
                e.Cancel = false;
                e.Result = string.Empty;
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
                string message = e.Error.Message;
                if (null != e.Error.InnerException) { message += e.Error.InnerException.Message; }
                message += "\r\n"+e.Error.StackTrace; 
                MessageBox.Show("Erreur durant le traitement :" + message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (e.Cancelled)
            {
                string message = (e.Error == null) ? e.ToString() : e.Error.Message;
                MessageBox.Show("Traitement annul�:" + message, "Annulation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                RemiseEnVeille();
                //recherche du nombre de nouvelles batailles 
                int nbNouvellesBatailles = (from nb in Donnees.m_donnees.TAB_BATAILLE
                             where nb.I_TOUR_DEBUT >= Donnees.m_donnees.TAB_PARTIE[0].I_TOUR_NOTIFICATION
                             select nb.ID_BATAILLE).Count();

                //recherche du nombre de batailles termin�es
                int nbAnciennesBatailles = (from nb in Donnees.m_donnees.TAB_BATAILLE
                                            where !nb.IsI_TOUR_FINNull() &&  nb.I_TOUR_FIN >= Donnees.m_donnees.TAB_PARTIE[0].I_TOUR_NOTIFICATION
                                            select nb.ID_BATAILLE).Count();

                string message;
                if (0 == nbNouvellesBatailles && 0 == nbAnciennesBatailles)
                {
                    message = "Traitement termin� avec succ�s";
                }
                else
                {
                    message = string.Format("Traitement termin� avec succ�s avec {0} nouvelles batailles et {1} batailles termin�es !", nbNouvellesBatailles, nbAnciennesBatailles);
                }
                MessageBox.Show(message, "Fin de traitement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //ConstruireImageCarte(); ne peut pas �tre appel� depuis un autre process
            }
        }

        private void RemiseEnVeille()
        {
            // Restore previous state
            if (NativeMethods.SetThreadExecutionState(fPreviousExecutionState) == 0)
            {
                // No way to recover; already exiting
            }
        }

        private void toolStripAfficherUnites_Click(object sender, EventArgs e)
        {
            AfficherUnites();
        }

        private void AfficherUnites()
        {
            //on s'assure que toutes les cases des unit�s sont bien charg�es
            int i = 0;
            while (i< Donnees.m_donnees.TAB_PION.Count) 
            {
                Donnees.TAB_PIONRow lignePion = Donnees.m_donnees.TAB_PION[i++];
                if (lignePion.B_DETRUIT)
                { continue; }

                //juste pour "forcer" le chargmement des cases si besoin
                Donnees.TAB_ORDRERow ligneOrdre = Donnees.m_donnees.TAB_ORDRE.Mouvement(lignePion.ID_PION);

                if (null == ligneOrdre)
                {
                    Donnees.m_donnees.TAB_CASE.FindParID_CASE(lignePion.ID_CASE);
                }
                else
                {
                    if (ligneOrdre.ID_CASE_DEPART >= 0)
                    { 
                    Donnees.m_donnees.TAB_CASE.FindParID_CASE(ligneOrdre.ID_CASE_DEPART);
                    }
                    if (ligneOrdre.ID_CASE_DESTINATION >=0)
                    {
                        Donnees.m_donnees.TAB_CASE.FindParID_CASE(ligneOrdre.ID_CASE_DESTINATION);
                    }
                }
            }
            ConstruireImageCarte();
        }

        private void toolStripButtonAfficherBatailles_Click(object sender, EventArgs e)
        {
            ConstruireImageCarte();
        }

        private void phrasesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormPhrase fPhrase = new()
            {
                tablePhrase = Donnees.m_donnees.TAB_PHRASE
            };
            if (DialogResult.OK == fPhrase.ShowDialog())
            {
                m_modification = true;
                Donnees.m_donnees.TAB_PHRASE.Clear();
                Donnees.m_donnees.TAB_PHRASE.Merge(fPhrase.tablePhrase, false);
            }
        }

        private void bataillesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormBatailles fBatailles = new()
            {
                tableBataille = Donnees.m_donnees.TAB_BATAILLE,
                tableBataillePions = Donnees.m_donnees.TAB_BATAILLE_PIONS
            };
            if (DialogResult.OK == fBatailles.ShowDialog())
            {
                m_modification = true;
                Donnees.m_donnees.TAB_BATAILLE.Clear();
                Donnees.m_donnees.TAB_BATAILLE.Merge(fBatailles.tableBataille, false);

                Donnees.m_donnees.TAB_BATAILLE_PIONS.Clear();
                Donnees.m_donnees.TAB_BATAILLE_PIONS.Merge(fBatailles.tableBataillePions, false);
                foreach(Donnees.TAB_BATAILLE_PIONSRow ligneBataillePion in Donnees.m_donnees.TAB_BATAILLE_PIONS)
                {
                    if (ligneBataillePion.IsB_ENGAGEENull()) { ligneBataillePion.B_ENGAGEE = false; }
                    if (ligneBataillePion.IsB_ENGAGEMENTNull()) { ligneBataillePion.B_ENGAGEMENT = false; }
                    if (ligneBataillePion.IsB_EN_DEFENSENull()) { ligneBataillePion.B_EN_DEFENSE = false; }
                    if (ligneBataillePion.IsB_RETRAITENull()) { ligneBataillePion.B_RETRAITE = false; }
                    if (ligneBataillePion.IsI_INFANTERIE_DEBUTNull()) { ligneBataillePion.I_INFANTERIE_DEBUT = 0; }
                    if (ligneBataillePion.IsI_CAVALERIE_DEBUTNull()) { ligneBataillePion.I_CAVALERIE_DEBUT = 0; }
                    if (ligneBataillePion.IsI_ARTILLERIE_DEBUTNull()) { ligneBataillePion.I_ARTILLERIE_DEBUT = 0; }
                    if (ligneBataillePion.IsI_FATIGUE_DEBUTNull()) { ligneBataillePion.I_FATIGUE_DEBUT = 0; }
                    if (ligneBataillePion.IsI_MORAL_DEBUTNull()) { ligneBataillePion.I_MORAL_DEBUT = 0; }
                }
            }
        }

        private void mEssagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormMessage fMessages = new()
            {
                tableMessage = Donnees.m_donnees.TAB_MESSAGE
            };
            if (DialogResult.OK == fMessages.ShowDialog())
            {
                m_modification = true;
                Donnees.m_donnees.TAB_MESSAGE.Clear();
                Donnees.m_donnees.TAB_MESSAGE.Merge(fMessages.tableMessage, false);
            }
        }

        private void testsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormTests fTests = new();
            fTests.ShowDialog();
        }

        private void outilsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormOutils fOutils = new();
            fOutils.ShowDialog(this);
        }

        private void creationInternetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!toolStripAfficherUnites.Checked)
            {
                //on remet � jour la carte avant de generer les fichiers
                toolStripAfficherUnites.Checked = true;
                AfficherUnites();
            }
            ClassTraitementWeb web = new(fichierCourant);
            if (!web.GenerationWeb())
            {
                MessageBox.Show("Erreur durant la g�n�ration des fichiers Web. Consultez le fichier de log", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show("G�n�ration des fichiers Web termin�e sans erreur", "G�n�ration Web", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void notificationAuxJoueursToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormMessageArbitre formMessageArbitre = new();
            formMessageArbitre.textBoxMessage.Text = Donnees.m_donnees.TAB_PARTIE[0].S_MESSAGE_ARBITRE;
            formMessageArbitre.fichierCourant = this.curFileName;
            string erreur;
            if (formMessageArbitre.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Donnees.m_donnees.TAB_PARTIE[0].S_MESSAGE_ARBITRE = formMessageArbitre.textBoxMessage.Text;
                Donnees.m_donnees.TAB_PARTIE[0].DT_PROCHAINTOUR = formMessageArbitre.dateEtHeure.Value;

                ClassNotificationJoueurs notification = new(fichierCourant);
                erreur = notification.NotificationJoueurs();
                if (string.Empty != erreur)
                {
                    MessageBox.Show("Erreur durant les notifications :"+ erreur, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("Notifications envoy�es sans erreur", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                //on met � jour la date du prochain tour dans tous les cas
                try
                {
                    string url = string.Format("http://{0}/vaocservicemiseajour.php?id_partie={1}&date_prochaintour={2}",
                        Donnees.m_donnees.TAB_PARTIE[0].S_HOST_SITEWEB,
                        Donnees.m_donnees.TAB_PARTIE[0].ID_PARTIE,
                        Constantes.DateHeureSQL(Donnees.m_donnees.TAB_PARTIE[0].DT_PROCHAINTOUR));
                    HttpClient client = new();
                    HttpRequestMessage message = new(HttpMethod.Get, url);
                    HttpResponseMessage response = client.Send(message);
                    if (!response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Timeout durant l'envoi de la date de mise � jour. Tout est correct sinon.",
                            "Attention", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch(Exception exWeb)
                {
                    MessageBox.Show("Erreur durant l'envoi de la date de mise � jour. Tout est correct sinon"+exWeb.Message, 
                        "Attention", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                Donnees.m_donnees.TAB_PARTIE[0].I_TOUR_NOTIFICATION = Donnees.m_donnees.TAB_PARTIE[0].I_TOUR;
                Donnees.m_donnees.SauvegarderPartie(fichierCourant, false);
            }            
        }

        private void renfortsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormRenfort fRenfort = new()
            {
                tableRenfort = Donnees.m_donnees.TAB_RENFORT
            };
            if (DialogResult.OK == fRenfort.ShowDialog())
            {
                m_modification = true;
                Donnees.m_donnees.TAB_RENFORT.Clear();
                Donnees.m_donnees.TAB_RENFORT.Merge(fRenfort.tableRenfort, false);
            }

        }

        private void mise�JourInternetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!ClassTraitementHeure.mise�JourInternet(fichierCourant, out string messageErreur))
            {
                MessageBox.Show("Erreur durant la mise � jour du Web. Consultez le fichier de log : " + messageErreur, "mise�JourInternet", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show("G�n�ration des mises � jour Web termin�e sans erreur", "mise�JourInternet", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            m_modification = true;
        }

        private void buttonCaseID_Click(object sender, EventArgs e)
        {
            if (null == Donnees.m_donnees)
            {
                MessageBox.Show("Aucun fichier charg�", "buttonCaseID_Click", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Donnees.TAB_CASERow ligneCase = Donnees.m_donnees.TAB_CASE.FindParID_CASE(Convert.ToInt32(this.textBoxCaseID.Text));
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
                Donnees.TAB_CASERow ligneCase = Donnees.m_donnees.TAB_CASE.FindParID_CASE(m_lignePionSelection.ID_CASE);
                labelInformationX.Text = ligneCase.I_X.ToString();
                labelInformationY.Text = ligneCase.I_Y.ToString();
                labelInformationIDCASE.Text = String.Format("ID_CASE:{0:00000}", ligneCase.ID_CASE);

                //recherche du trajet de l'unit� en m�moire s'il existe pour l'affichage
                m_cheminSelection?.Clear();
                this.buttonRecalculTrajet.Enabled = false;
                this.buttonVerifierTrajet.Enabled = false;
                Donnees.TAB_ORDRERow ligneOrdre = Donnees.m_donnees.TAB_ORDRE.Mouvement(m_lignePionSelection.ID_PION);
                if (null != ligneOrdre)
                {
                    Donnees.TAB_CASERow ligneCaseDestination;
                    Donnees.TAB_CASERow ligneCaseDepart;
                    if ((m_lignePionSelection.I_INFANTERIE > 0 || m_lignePionSelection.I_CAVALERIE > 0 || m_lignePionSelection.I_ARTILLERIE > 0) 
                        || (Constantes.ORDRES.SUIVRE_UNITE != ligneOrdre.I_ORDRE_TYPE))
                    {
                        ligneCaseDestination = Donnees.m_donnees.TAB_CASE.FindParID_CASE(ligneOrdre.ID_CASE_DESTINATION);
                        ligneCaseDepart = Donnees.m_donnees.TAB_CASE.FindParID_CASE(ligneOrdre.ID_CASE_DEPART);
                    }
                    else
                    {
                        ligneCaseDestination = Donnees.m_donnees.TAB_CASE.FindParID_CASE(m_lignePionSelection.ID_CASE);
                        ligneCaseDepart = Donnees.m_donnees.TAB_CASE.FindParID_CASE(m_lignePionSelection.ID_CASE);
                    }
                    AStar etoile = new();

                    if (etoile.RechercheChemin(Constantes.TYPEPARCOURS.MOUVEMENT,
                        m_lignePionSelection, ligneCaseDepart, ligneCaseDestination, out m_cheminSelection, out _, out _, out _, out _))
                    {
                        this.buttonVerifierTrajet.Enabled = true;
                        this.buttonRecalculTrajet.Enabled = true;
                        int dx=-1, dy=-1;
                        foreach (LigneCASE ligneC in m_cheminSelection)
                        {
                            Debug.WriteLine(string.Format("selection = {0}, {1}", ligneC.I_X, ligneC.I_Y));
                            if ((dx > 0 && Math.Abs(dx - ligneC.I_X) > 1) || (dy > 0 && Math.Abs(dy - ligneC.I_Y) > 1))
                            {
                                Debug.WriteLine(string.Format("dx = {0}, {1}", dx, dy));
                            }
                            dx = ligneC.I_X;
                            dy = ligneC.I_Y;
                        }
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
            FormHPA fHPA = new()
            {
                //affectation des tables
                //fHPA.tableCaseBlocs = Donnees.m_donnees.TAB_PCC_CASE_BLOCS;
                tailleBloc = Donnees.m_donnees.TAB_JEU[0].IsI_TAILLEBLOC_PCCNull() ? 20 : Donnees.m_donnees.TAB_JEU[0].I_TAILLEBLOC_PCC,
                //fHPA.tablePCCCouts = Donnees.m_donnees.TAB_PCC_COUTS;
                //fHPA.tablePCCTrajet = Donnees.m_donnees.TAB_PCC_TRAJET;
                fichierCourant = this.fichierCourant
            };

            fHPA.ShowDialog();
            m_modification = true;
            Donnees.m_donnees.TAB_JEU[0].I_TAILLEBLOC_PCC = fHPA.tailleBloc;
                //mise � jour des modeles de terrain
                //Donnees.m_donnees.TAB_PCC_CASE_BLOCS.Clear();
                //Donnees.m_donnees.TAB_PCC_CASE_BLOCS.Merge(fHPA.tableCaseBlocs, false);
                //Donnees.m_donnees.TAB_PCC_COUTS.Merge(fHPA.tablePCCCouts, false);
                //Donnees.m_donnees.TAB_PCC_TRAJET.Merge(fHPA.tablePCCTrajet, false);

        }

        private void initilisationPartieToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == MessageBox.Show("Cette action va supprimer toutes les tables temporaires, les messages, les messages et patrouilles, les renforts et remettre les effectifs des unit�s. N'oubliez pas de remettre, � la main, la position initiale des troupes.",
                "Reinitialisation de la partie", MessageBoxButtons.OKCancel))
            {
                Cursor oldcurseur = this.Cursor;
                this.Cursor = Cursors.WaitCursor;
                Donnees.m_donnees.TAB_BATAILLE.Clear();
                Donnees.m_donnees.TAB_BATAILLE_PIONS.Clear();
                Donnees.m_donnees.TAB_MESSAGE.Clear();
                Donnees.m_donnees.TAB_ORDRE_ANCIEN.Clear();
                Donnees.m_donnees.TAB_MESSAGE_ANCIEN.Clear();
                Donnees.m_donnees.TAB_PION_ANCIEN.Clear();
                Donnees.m_donnees.TAB_ORDRE.Clear();
                Donnees.m_donnees.TAB_PARCOURS.Clear();
                Donnees.m_donnees.TAB_ESPACE.Clear();
                Donnees.m_donnees.TAB_VIDEO.Clear();
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

                //suppression des unit�s arriv�es en renfort (les lignes de renforts restant inchang�s)
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

                //remise � zero des autres unit�s
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
                    lignePion.I_TOUR_RETRAITE_RESTANT = 0;
                    lignePion.I_TOUR_SANS_RAVITAILLEMENT = 0;
                    lignePion.SetI_ZONE_BATAILLENull();
                    lignePion.SetID_BATAILLENull();
                    lignePion.B_DETRUIT = false;
                    lignePion.B_FUITE_AU_COMBAT = false;
                    lignePion.B_INTERCEPTION = false;
                }
                //remise � "blanc" du placement de toutes les unit�s sur la carte
                foreach (Donnees.TAB_CASERow ligneCase in Donnees.m_donnees.TAB_CASE)
                {
                    ligneCase.initialisationID_PROPRIETAIRENull();
                    ligneCase.initialisationID_NOUVEAU_PROPRIETAIRENull();
                }

                //Suppression de tous les trajets cr�es suite � la destruction ou � la construction d'un pont
                i = 0;
                while (i<Donnees.m_donnees.TAB_PCC_COUTS.Count)
                {
                    Donnees.TAB_PCC_COUTSRow ligneCout = Donnees.m_donnees.TAB_PCC_COUTS[i];
                    if (ligneCout.B_CREATION)
                    {
                        Dal.SupprimerTrajet(ligneCout.ID_TRAJET, "");
                        ligneCout.Delete();
                    }
                    else
                    {
                        i++;
                    }
                }
                //foreach (Donnees.TAB_PCC_VILLESRow lignePCCVille in Donnees.m_donnees.TAB_PCC_VILLES)
                //{
                //    if (lignePCCVille.B_CREATION)
                //    {
                //        Dal.SupprimerTrajet(lignePCCVille.ID_TRAJET, Constantes.CST_TRAJET_VILLE);
                //        lignePCCVille.Delete();
                //    }
                //}

                //on d�place en table tous les trajets d'origine remplac� par de nouveaux suite � la destruction ou � la construction d'un pont
                foreach (Donnees.TAB_PCC_COUTS_SUPPRIMERow ligneCoutSupprimer in Donnees.m_donnees.TAB_PCC_COUTS_SUPPRIME)
                {
                    Donnees.m_donnees.TAB_PCC_COUTS.AddTAB_PCC_COUTSRow(
                        ligneCoutSupprimer.I_BLOCX,
                        ligneCoutSupprimer.I_BLOCY,
                        ligneCoutSupprimer.ID_CASE_DEBUT,
                        ligneCoutSupprimer.ID_CASE_FIN,
                        ligneCoutSupprimer.I_COUT_INITIAL,
                        ligneCoutSupprimer.ID_TRAJET,
                        ligneCoutSupprimer.I_COUT_INITIAL,
                        false,//B_CREATION
                        -1//ID_NATION
                        );
                }
                Donnees.m_donnees.TAB_PCC_COUTS_SUPPRIME.Clear();

                //on remet les couts initiaux au cas o� ceux-ci auraient chang�s suite � des ponts endommag�s
                foreach (Donnees.TAB_PCC_COUTSRow ligneCout in Donnees.m_donnees.TAB_PCC_COUTS)
                {
                    ligneCout.I_COUT = ligneCout.I_COUT_INITIAL;
                }
                
                //de m�me on remet en place les mod�les de terrains des cases au moment du d�but du sc�nario
                foreach (Donnees.TAB_CASE_MODIFICATIONRow ligneCaseModif in Donnees.m_donnees.TAB_CASE_MODIFICATION)
                {
                    Donnees.TAB_CASERow ligneCase = Donnees.m_donnees.TAB_CASE.FindParID_CASE(ligneCaseModif.ID_CASE);
                    if (null != ligneCase)
                    {
                        ligneCase.ID_MODELE_TERRAIN = ligneCaseModif.ID_MODELE_TERRAIN_SOURCE;
                    }
                }
                Donnees.m_donnees.TAB_CASE_MODIFICATION.Clear();
                //foreach (Donnees.TAB_PCC_VILLES_SUPPRIMERow lignePCCVilleSupprime in Donnees.m_donnees.TAB_PCC_VILLES_SUPPRIME)
                //{
                //    Donnees.m_donnees.TAB_PCC_VILLES.AddTAB_PCC_VILLESRow(
                //        lignePCCVilleSupprime.ID_VILLE_DEBUT,
                //        lignePCCVilleSupprime.ID_VILLE_FIN,
                //        lignePCCVilleSupprime.I_COUT,
                //        lignePCCVilleSupprime.ID_TRAJET,
                //        lignePCCVilleSupprime.I_HORSROUTE,
                //        Donnees.CST_TRAJET_INOCCUPE,
                //        lignePCCVilleSupprime.B_CREATION);
                //}

                //on remet � false tous les noms des rempla�ants des chefs bless�s
                foreach (Donnees.TAB_NOMS_PROMUSRow ligneNomPromu in Donnees.m_donnees.TAB_NOMS_PROMUS)
                {
                    ligneNomPromu.B_NOM_PROMU = false;
                }

                //elements g�n�raux
                Donnees.m_donnees.TAB_PARTIE[0].I_TOUR = 0;
                Donnees.m_donnees.TAB_PARTIE[0].I_PHASE = 0;
                Donnees.m_donnees.TAB_PARTIE[0].ID_METEO = Donnees.m_donnees.TAB_PARTIE[0].ID_METEO_INITIALE;
                Donnees.m_donnees.TAB_PARTIE[0].SetID_VICTOIRENull();
                Donnees.m_donnees.TAB_PARTIE[0].FL_DEMARRAGE = false;
                MiseAJourTitreFenetre();
                this.Cursor = oldcurseur;
                ConstruireImageCarte();
            }
        }

        /// <summary>
        /// Affichage du point sur lequel se trouve la souris
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImageCarte_MouseMove(object sender, MouseEventArgs e)
        {
            bool bTrouveCase = false;
            Donnees.TAB_CASERow ligneCase = null;
            int clicX = (int)Math.Round(e.X / m_zoom, 0);
            int clicY = (int)Math.Round(e.Y / m_zoom, 0);

            if (Donnees.m_donnees.TAB_JEU.Count > 0)
            {
                if (clicX >= 0 && clicY >= 0 && clicX < Donnees.m_donnees.TAB_JEU[0].I_LARGEUR_CARTE && clicY < Donnees.m_donnees.TAB_JEU[0].I_HAUTEUR_CARTE)
                {
                    ligneCase = Donnees.m_donnees.TAB_CASE.FindParXY(clicX, clicY);
                    if (null != ligneCase)
                    {
                        bTrouveCase = true;
                    }
                }
            }

            if (bTrouveCase)
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
            //Donnees.TAB_NOMS_CARTERow ligneNom = null;

            //int id_case_destination;
            //ClassMessager.ZoneGeographiqueVersCase(lignePion, 2, ClassMessager.COMPAS.CST_SUD, 120, out id_case_destination);
            //return;

            #region test plus court chemin
            if (toolStripPlusCourtChemin.CheckState == CheckState.Checked)
            {
                Donnees.TAB_PIONRow lignePion = Donnees.m_donnees.TAB_PION.FindByID_PION(0);

                if (MouseButtons.Left == e.Button)
                {
                    m_departPlusCourtChemin = new LigneCASE(Donnees.m_donnees.TAB_CASE.FindParXY((int)Math.Round(e.X / m_zoom, 0), (int)Math.Round(e.Y / m_zoom, 0)));
                    //m_departPlusCourtChemin = Donnees.m_donnees.TAB_CASE.FindByXY(931, 180);//bug poentiel, d�part sur une bordure
                    //m_departPlusCourtChemin = Donnees.m_donnees.TAB_CASE.FindByXY(378, 720);//bug poentiel, d�part sur une bordure
                    //m_departPlusCourtChemin = Donnees.m_donnees.TAB_CASE.FindByXY(622, 577);
                    //m_departPlusCourtChemin = Donnees.m_donnees.TAB_CASE.FindParID_CASE(4373903);
                    //m_departPlusCourtChemin = new LigneCASE(Donnees.m_donnees.TAB_CASE.FindParID_CASE(7232577));
                    labelDepartX.Text = Convert.ToString(m_departPlusCourtChemin.I_X);
                    labelDepartY.Text = Convert.ToString(m_departPlusCourtChemin.I_Y);
                    labelDepartIDCASE.Text = Convert.ToString(m_departPlusCourtChemin.ID_CASE);
                    ligneModeleTerrain = Donnees.m_donnees.TAB_MODELE_TERRAIN.FindByID_MODELE_TERRAIN(m_departPlusCourtChemin.ID_MODELE_TERRAIN);
                    labelDepartTerrain.Text = ligneModeleTerrain.S_NOM;
                }
                else
                {
                    m_arriveePlusCourtChemin = new LigneCASE(Donnees.m_donnees.TAB_CASE.FindParXY((int)Math.Round(e.X / m_zoom, 0), (int)Math.Round(e.Y / m_zoom, 0)));
                    //m_arriveePlusCourtChemin = Donnees.m_donnees.TAB_CASE.FindByXY(933, 180);
                    //m_arriveePlusCourtChemin = Donnees.m_donnees.TAB_CASE.FindByXY(1596, 561);
                    //m_arriveePlusCourtChemin = new LigneCASE(Donnees.m_donnees.TAB_CASE.FindParID_CASE(3515734));
                    //m_arriveePlusCourtChemin = Donnees.m_donnees.TAB_CASE.FindByXY(661, 560);
                    //ligneNom = Donnees.m_donnees.TAB_NOMS_CARTE.FindByID_NOM(120);
                    //m_arriveePlusCourtChemin = Donnees.m_donnees.TAB_CASE.FindParID_CASE(4431204);//bug potentiel emp�chant de trouver un chemin pour aller jusqu'� cette case
                    labelArriveeX.Text = Convert.ToString(m_arriveePlusCourtChemin.I_X);
                    labelArriveeY.Text = Convert.ToString(m_arriveePlusCourtChemin.I_Y);
                    labelArriveeIDCASE.Text = Convert.ToString(m_arriveePlusCourtChemin.ID_CASE);
                    ligneModeleTerrain = Donnees.m_donnees.TAB_MODELE_TERRAIN.FindByID_MODELE_TERRAIN(m_arriveePlusCourtChemin.ID_MODELE_TERRAIN);
                    labelArriveeTerrain.Text = ligneModeleTerrain.S_NOM;
                }

                if (null != m_departPlusCourtChemin && null != m_arriveePlusCourtChemin)
                {
                    //recherche de la m�t�o
                    if (0 == Donnees.m_donnees.TAB_METEO.Count)
                    {
                        MessageBox.Show("Il n'y a aucune m�t�o de d�finie", "AStar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    //recherche du mod�le de mouvement
                    if (0 == Donnees.m_donnees.TAB_MODELE_MOUVEMENT.Count)
                    {
                        MessageBox.Show("Il n'y a aucune mod�le de mouvement de d�fini", "AStar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    //recherche du plus court chemin
                    labelInformationTempsPasse.Text = string.Empty;
                    //AstarTerrain[] tableCoutsMouvementsTerrain;

                    #region version Hors Route
                    /* fonctionne pas du tout !
                    timeStart = DateTime.Now;
                    Cartographie.CalculModeleMouvementsPion(out tableCoutsMouvementsTerrain, false, true);
                    //if (!Cartographie.RechercheChemin(Cartographie.typeParcours.RAVITAILLEMENT, lignePion, m_departPlusCourtChemin, m_arriveePlusCourtChemin, null, out chemin, out cout, out coutHorsRoute, out tableCoutsMouvementsTerrain, out messageErreur))
                    if (!m_etoile.SearchPath(m_departPlusCourtChemin, m_arriveePlusCourtChemin, tableCoutsMouvementsTerrain))
                    {
                        MessageBox.Show("Il n'y a aucun chemin possible entre les deux points", "HorsRoute", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    perf = DateTime.Now - timeStart;
                    labelInformationTempsPasse.Text += string.Format("\r\n Hors Route : {0} min {1} sec {2} mil", perf.Minutes, perf.Seconds, perf.Milliseconds);
                    m_cheminHorsRoute = m_etoile.PathByNodes;
                     * */
                    #endregion

                    #region version standard star*
                    /* 
                    if (null == m_etoileHPA) m_etoileHPA = new AStar();
                    timeStart = DateTime.Now;
                    Cartographie.CalculModeleMouvementsPion(out tableCoutsMouvementsTerrain);
                    if (!m_etoileHPA.SearchPath(m_departPlusCourtChemin, m_arriveePlusCourtChemin, tableCoutsMouvementsTerrain))
                    {
                        MessageBox.Show("Il n'y a aucun chemin possible entre les deux points", "AStar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    perf = DateTime.Now - timeStart;
                    labelInformationTempsPasse.Text = string.Format("{0} min {1} sec {2} mil cout:{3}", perf.Minutes, perf.Seconds, perf.Milliseconds, m_etoileHPA.CoutGlobal);
                    m_cheminPCC = m_etoileHPA.PathByNodes;
                     */
                    #endregion

                    #region version HPA
                    /* */
                    //maintenant on compare avec la version HPA
                    //Donnees.m_donnees.TAB_PCC_COUTS.Initialisation(); -> deja fait au chargement
                    if (null == m_etoileHPA) m_etoileHPA = new AStar();
                    AStar.CalculModeleMouvementsPion(out _);
                    timeStart = DateTime.Now;
                    //if (!m_etoileHPA.InitialisationProprietaireTrajet())
                    //{
                    //    MessageBox.Show("Erreur dans m_etoileHPA.InitialisationProprietaireTrajet()", "ImageCarte_MouseClick", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //    return;
                    //}
                    //if (!m_etoileHPA.SearchPathHPA(m_departPlusCourtChemin, m_arriveePlusCourtChemin, tableCoutsMouvementsTerrain, -1))

                    int id_case = lignePion.ID_CASE;
                    lignePion.ID_CASE = m_departPlusCourtChemin.ID_CASE;
                    if (!m_etoileHPA.RechercheChemin(Constantes.TYPEPARCOURS.MOUVEMENT, lignePion,m_departPlusCourtChemin, m_arriveePlusCourtChemin, 
                        out List<LigneCASE> chemin, out _, out _, out _, out _))
                    {
                            MessageBox.Show(string.Format("Il n'y a aucun chemin possible entre les points {0}:{1},{2} -> {3}:{4},{5}", 
                            m_departPlusCourtChemin.ID_CASE, m_departPlusCourtChemin.I_X, m_departPlusCourtChemin.I_Y,
                            m_arriveePlusCourtChemin.ID_CASE, m_arriveePlusCourtChemin.I_X, m_arriveePlusCourtChemin.I_Y), 
                            "AStarHPA", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        perf = DateTime.Now - timeStart;
                        labelInformationTempsPasse.Text += string.Format("\r\n HPA : {0} min {1} sec {2} mil cout:{3} distance:{4}", 
                            perf.Minutes, perf.Seconds, perf.Milliseconds, m_etoileHPA.CoutGlobal, (decimal)chemin.Count / Donnees.m_donnees.TAB_JEU[0].I_ECHELLE);
                        m_cheminHPA = m_etoileHPA.PathByNodes;
                    }
                    lignePion.ID_CASE = id_case;
                    //int pos = 0;
                    //while (chemin[pos].ID_CASE != lignePion.ID_CASE) pos++;                    
                    /* */
                    #endregion

                    #region version ville
                    //maintenant on compare avec la version Ville
                    /*
                    Debug.WriteLine("TAB_PCC_VILLES #=" + Donnees.m_donnees.TAB_PCC_VILLES.Count);
                    foreach (Donnees.TAB_PCC_VILLESRow lignePccVille in Donnees.m_donnees.TAB_PCC_VILLES)
                    {
                        if (lignePccVille.I_COUT < 0)
                        {
                            Donnees.TAB_NOMS_CARTERow lVDebug1 = Donnees.m_donnees.TAB_NOMS_CARTE.FindByID_NOM(lignePccVille.ID_VILLE_DEBUT);
                            Donnees.TAB_NOMS_CARTERow lVDebug2 = Donnees.m_donnees.TAB_NOMS_CARTE.FindByID_NOM(lignePccVille.ID_VILLE_FIN);
                            Donnees.TAB_CASERow lDebug1 = Donnees.m_donnees.TAB_CASE.FindParID_CASE(lVDebug1.ID_CASE);
                            Donnees.TAB_CASERow lDebug2 = Donnees.m_donnees.TAB_CASE.FindParID_CASE(lVDebug2.ID_CASE);
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
                    Cartographie.CalculModeleMouvementsPion(out tableCoutsMouvementsTerrain);
                    Cartographie.InitialiserProprietairesTrajets();
                    List<Donnees.TAB_CASERow> chemin = null;
                    //ligneNom = Donnees.m_donnees.TAB_NOMS_CARTE.FindByID_NOM(19);
                    //double cout, coutHorsRoute;
                    //string messageErreur;
                    //if (!Cartographie.RechercheChemin(Cartographie.typeParcours.MOUVEMENT, lignePion, m_departPlusCourtChemin, m_arriveePlusCourtChemin, null, out chemin, out cout, out coutHorsRoute, out tableCoutsMouvementsTerrain, out messageErreur))
                    LogFile.CreationLogFile(fichierCourant, "tour", Donnees.m_donnees.TAB_PARTIE[0].I_TOUR, -1);
                    //if (!Cartographie.RechercheChemin(Cartographie.typeParcours.RAVITAILLEMENT, lignePion, m_departPlusCourtChemin, m_arriveePlusCourtChemin, null, out chemin, out cout, out coutHorsRoute, out tableCoutsMouvementsTerrain, out messageErreur))
                    if (!m_etoile.SearchPathVilles(m_departPlusCourtChemin, ligneNom, m_arriveePlusCourtChemin, tableCoutsMouvementsTerrain))
                    {
                        m_cheminVille = null;
                        MessageBox.Show("Il n'y a aucun chemin possible entre les deux points", "AStarVille", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        perf = DateTime.Now - timeStart;
                        labelInformationTempsPasse.Text += string.Format("\r\n Villes : {0} min {1} sec {2} mil", perf.Minutes, perf.Seconds, perf.Milliseconds);
                        if (null != chemin || m_etoile.PathByNodes.Count>0)
                        {
                            labelInformationTempsPasse.Text += string.Format("\r\n Villes : {0}/{1} hors route", m_etoile.HorsRouteGlobal, m_etoile.CoutGlobal);
                            m_cheminVille = m_etoile.PathByNodes;
                        }
                        else
                        {
                            labelInformationTempsPasse.Text += "\r\n Villes : Il n'y a aucun chemin possible entre les deux points";
                            m_cheminVille = null;
                        }
                        //m_cheminVille = chemin;
                    }
                     */
                    #endregion
                    ConstruireImageCarte();
                }
            }
            #endregion

            #region ajout, suppression, modifications de noms sur la carte
            if (toolStripPlusCourtChemin.CheckState == CheckState.Unchecked
                && toolStripButtonReparerPont.CheckState == CheckState.Unchecked
                && toolStripButtonPontDetruit.CheckState == CheckState.Unchecked
                && toolStripButtonConstruirePonton.CheckState == CheckState.Unchecked
                && toolStripButtonPontEndommage.CheckState == CheckState.Unchecked)
            {
                AjouterModifierNomSurCarte(clicX, clicY);
            }
            #endregion

            #region Test construction de ponton
            int lgPontOuGue;
            if (this.toolStripButtonConstruirePonton.CheckState == CheckState.Checked)
            {
                Donnees.TAB_CASERow ligneCase = Donnees.m_donnees.TAB_CASE.FindParXY((int)Math.Round(e.X / m_zoom, 0), (int)Math.Round(e.Y / m_zoom, 0));
                //un guet, c'est un ponton d�truit
                Donnees.TAB_CASERow ligneCasePont = ligneCase.RecherchePontouPonton(false, true, out lgPontOuGue);
                if (null == ligneCasePont)
                {
                    MessageBox.Show("Il n'y a aucun gu� � proximit�", "Construction de ponton", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
                else
                {
                    if (!ligneCasePont.ConstruirePonton())
                    {
                        MessageBox.Show("Erreur lors de la construction du ponton", "construction de ponton", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        MessageBox.Show(string.Format("Construction de ponton sur une longueur de {0} en {1}({2},{3})", lgPontOuGue.ToString(), ligneCasePont.ID_CASE, ligneCasePont.I_X, ligneCasePont.I_Y), 
                        "Construction de ponton", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            #endregion

            if (toolStripButtonPontDetruit.CheckState == CheckState.Checked)
            {
                //pour d�truire un pont, c'est comme pour d�truire un ponton, sauf qu'on le fait sur un pont et que l'on recherche le mod�le "guet", c'est � dire un ponton/d�truit
                //mais, dans ce cas, ce n'est pas une vraie destruction, mais vraiment detruire le pont, ne d�truirait que sur la case "pont" (je laisse des routes sur les ponts longs)
                //et mettrait une rivi�re (traversable), ce qui n'est pas un �cart par rappport � un endommagement sauf que seul un pontonnier peut reconstruire, bof...
            }

            #region Test endommager un pont / detruire un ponton
            if (toolStripButtonPontEndommage.CheckState == CheckState.Checked)
            {
                Donnees.TAB_CASERow ligneCase = Donnees.m_donnees.TAB_CASE.FindParXY((int)Math.Round(e.X / m_zoom, 0), (int)Math.Round(e.Y / m_zoom, 0));
                Donnees.TAB_CASERow ligneCasePont = ligneCase.RecherchePontouPonton(true, false, out lgPontOuGue);
                Donnees.TAB_CASERow ligneCasePonton = ligneCase.RecherchePontouPonton(false, false, out int lgPonton);
                if (null == ligneCasePont && null == ligneCasePonton)
                {
                    MessageBox.Show("Il n'y a aucun pont ou ponton � proximit�", "Destruction de pont", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
                else
                {
                    if (Constantes.Distance(ligneCase.I_X, ligneCase.I_Y, ligneCasePont.I_X, ligneCasePont.I_Y) > Constantes.Distance(ligneCase.I_X, ligneCase.I_Y, ligneCasePonton.I_X, ligneCasePonton.I_Y))
                    {
                        if (!ligneCasePonton.EndommagerReparerPont())
                        {
                            MessageBox.Show("Erreur lors de la destruction du ponton", "Ponton d�truit", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            MessageBox.Show("ponton d�truit sur une longueur de " + lgPonton.ToString(), "Pont Endommag�/Ponton d�truit", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else
                    {
                        if (!ligneCasePont.EndommagerReparerPont())
                        {
                            MessageBox.Show("Erreur lors de l'endommagement (sic)", "Pont Endommag�", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            MessageBox.Show("pont endommag� sur une longueur de " + lgPontOuGue.ToString(), "Pont Endommag�/Ponton d�truit", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            #endregion

            #region Test reparation de pont
            if (toolStripButtonReparerPont.CheckState == CheckState.Checked)
            {
                Donnees.TAB_CASERow ligneCase = Donnees.m_donnees.TAB_CASE.FindParXY((int)Math.Round(e.X / m_zoom, 0), (int)Math.Round(e.Y / m_zoom, 0));
                Donnees.TAB_CASERow ligneCasePont = ligneCase.RecherchePontouPonton(true, true, out lgPontOuGue);
                if (null == ligneCasePont)
                {
                    MessageBox.Show("Il n'y a aucun pont endommag� � proximit�", "Construction de pont", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
                else
                {
                    if (!ligneCasePont.EndommagerReparerPont())
                    {
                        MessageBox.Show("Erreur lors de la r�paration", "R�paration de pont", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        MessageBox.Show("Pont construit", "R�paration de pont sur une longueur de " + lgPontOuGue.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            #endregion
        }

        private void AjouterModifierNomSurCarte(int clicX, int clicY)
        {
            //ajout, suppression, modifications de noms sur la carte
            FormNomCarte fNomCarte = new()
            {
                X = clicX,
                Y = clicY,
                tablePolice = Donnees.m_donnees.TAB_POLICE,
                id_nom = Constantes.CST_IDNULL
            };
            int id_case = Constantes.CST_IDNULL;
            int i = 0;
            Donnees.TAB_CASERow ligneCase = null;
            Donnees.TAB_NOMS_CARTERow ligneNomCarte;

            //recherche d'un nom de carte d�j� existant � proximit�
            while (i < Donnees.m_donnees.TAB_NOMS_CARTE.Rows.Count && Constantes.CST_IDNULL == id_case)
            {
                if (Constantes.Distance(clicX, clicY, Donnees.m_donnees.TAB_NOMS_CARTE[i].I_X, Donnees.m_donnees.TAB_NOMS_CARTE[i].I_Y) < Convert.ToInt32(Properties.Resources.distanceRechercheNom))
                {
                    ligneCase = Donnees.m_donnees.TAB_CASE.FindParID_CASE(Donnees.m_donnees.TAB_NOMS_CARTE[i].ID_CASE);
                    id_case = ligneCase.ID_CASE;
                    fNomCarte.id_nom = Donnees.m_donnees.TAB_NOMS_CARTE[i].ID_NOM;
                    fNomCarte.victoire = Donnees.m_donnees.TAB_NOMS_CARTE[i].I_VICTOIRE;
                    fNomCarte.id_police = Donnees.m_donnees.TAB_NOMS_CARTE[i].ID_POLICE;
                    //fNomCarte.nom = Donnees.m_donnees.TAB_NOMS_CARTE[i].S_NOM; recherche faite dans la FormNomCarte
                    fNomCarte.chanceRenfort  = Donnees.m_donnees.TAB_NOMS_CARTE[i].I_CHANCE_RENFORT;
                    fNomCarte.position = Donnees.m_donnees.TAB_NOMS_CARTE[i].I_POSITION;
                    fNomCarte.hopital = Donnees.m_donnees.TAB_NOMS_CARTE[i].B_HOPITAL;
                    fNomCarte.prison = Donnees.m_donnees.TAB_NOMS_CARTE[i].B_PRISON;
                    fNomCarte.infanterie = Donnees.m_donnees.TAB_NOMS_CARTE[i].I_INFANTERIE_RENFORT;
                    fNomCarte.cavalerie = Donnees.m_donnees.TAB_NOMS_CARTE[i].I_CAVALERIE_RENFORT;
                    fNomCarte.artillerie = Donnees.m_donnees.TAB_NOMS_CARTE[i].I_ARTILLERIE_RENFORT;
                    fNomCarte.moral = Donnees.m_donnees.TAB_NOMS_CARTE[i].I_MORAL_RENFORT;
                    fNomCarte.materiel = Donnees.m_donnees.TAB_NOMS_CARTE[i].I_MATERIEL_RENFORT;
                    fNomCarte.ravitaillement = Donnees.m_donnees.TAB_NOMS_CARTE[i].I_RAVITAILLEMENT_RENFORT;
                    fNomCarte.modelePionRenfort = Donnees.m_donnees.TAB_NOMS_CARTE[i].IsID_MODELE_PION_RENFORTNull() ? null : Donnees.m_donnees.TAB_MODELE_PION.FindByID_MODELE_PION(Donnees.m_donnees.TAB_NOMS_CARTE[i].ID_MODELE_PION_RENFORT);
                    fNomCarte.creationDeDepots = Donnees.m_donnees.TAB_NOMS_CARTE[i].B_CREATION_DEPOT;
                    if (Donnees.m_donnees.TAB_NOMS_CARTE[i].IsID_PION_PROPRIETAIRE_RENFORTNull())
                    {
                        fNomCarte.pionProprietaireRenfort = null;
                    }
                    else
                    {
                        fNomCarte.pionProprietaireRenfort = Donnees.m_donnees.TAB_PION.FindByID_PION(Donnees.m_donnees.TAB_NOMS_CARTE[i].ID_PION_PROPRIETAIRE_RENFORT);
                    }

                    if (Donnees.m_donnees.TAB_NOMS_CARTE[i].IsID_NATION_CONTROLENull())
                    {
                        fNomCarte.proprietaire = null;
                    }
                    else
                    {
                        fNomCarte.proprietaire = Donnees.m_donnees.TAB_NATION.FindByID_NATION(Donnees.m_donnees.TAB_NOMS_CARTE[i].ID_NATION_CONTROLE);
                    }
                }
                
                i++;
            }

            //s'il n'y en pas, on prend la case du clic
            if (Constantes.CST_IDNULL == id_case)
            {
                ligneCase = Donnees.m_donnees.TAB_CASE.FindParXY(clicX, clicY);
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
                        ligneNomCarte.I_CHANCE_RENFORT = fNomCarte.chanceRenfort;
                        ligneNomCarte.B_HOPITAL = fNomCarte.hopital;
                        ligneNomCarte.B_PRISON = fNomCarte.prison;
                        ligneNomCarte.I_INFANTERIE_RENFORT = fNomCarte.infanterie;
                        ligneNomCarte.I_CAVALERIE_RENFORT = fNomCarte.cavalerie;
                        ligneNomCarte.I_ARTILLERIE_RENFORT = fNomCarte.artillerie;
                        ligneNomCarte.I_MORAL_RENFORT = fNomCarte.moral;
                        ligneNomCarte.I_MATERIEL_RENFORT = fNomCarte.materiel;
                        ligneNomCarte.I_RAVITAILLEMENT_RENFORT = fNomCarte.ravitaillement;
                        ligneNomCarte.B_CREATION_DEPOT = fNomCarte.creationDeDepots;
                        if (null == fNomCarte.modelePionRenfort)
                        {
                            ligneNomCarte.SetID_MODELE_PION_RENFORTNull();
                        }
                        else
                        {
                            ligneNomCarte.ID_MODELE_PION_RENFORT = fNomCarte.modelePionRenfort.ID_MODELE_PION;
                        }
                        if (null == fNomCarte.pionProprietaireRenfort)
                        {
                            ligneNomCarte.SetID_PION_PROPRIETAIRE_RENFORTNull();
                        }
                        else
                        {
                            ligneNomCarte.ID_PION_PROPRIETAIRE_RENFORT = fNomCarte.pionProprietaireRenfort.ID_PION;
                        }
                        if (null == fNomCarte.pionProprietaireRenfort)
                        {
                            ligneNomCarte.SetID_PION_PROPRIETAIRE_RENFORTNull();
                        }
                        else
                        {
                            ligneNomCarte.ID_PION_PROPRIETAIRE_RENFORT = fNomCarte.pionProprietaireRenfort.ID_PION;
                        }
                        if (null == fNomCarte.proprietaire)
                        {
                            ligneNomCarte.SetID_NATION_CONTROLENull();
                        }
                        else
                        {
                            ligneNomCarte.ID_NATION_CONTROLE = fNomCarte.proprietaire.ID_NATION;
                        }
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
                            (fNomCarte.proprietaire == null) ? -1 : fNomCarte.proprietaire.ID_NATION,
                            fNomCarte.chanceRenfort,
                            (fNomCarte.modelePionRenfort == null) ? -1 : fNomCarte.modelePionRenfort.ID_MODELE_PION,
                            (fNomCarte.pionProprietaireRenfort==null) ? -1 : fNomCarte.pionProprietaireRenfort.ID_PION,
                            fNomCarte.hopital,
                            fNomCarte.prison,
                            fNomCarte.infanterie,
                            fNomCarte.cavalerie,
                            fNomCarte.artillerie,
                            fNomCarte.moral,
                            fNomCarte.materiel,
                            fNomCarte.ravitaillement,
                            false,
                            fNomCarte.nom,
                            ligneCase.I_X,
                            ligneCase.I_Y,
                            fNomCarte.creationDeDepots
                            );
                        ligneNomCarte.SetID_NATION_CONTROLENull();
                        if (null == fNomCarte.pionProprietaireRenfort) ligneNomCarte.SetID_PION_PROPRIETAIRE_RENFORTNull();
                        if (null == fNomCarte.modelePionRenfort) ligneNomCarte.SetID_MODELE_PION_RENFORTNull();
                    }
                }
            }
            ConstruireImageCarte();
        }

        private void buttonVerifierTrajet_Click(object sender, EventArgs e)
        {
            Donnees.TAB_ORDRERow ligneOrdre = Donnees.m_donnees.TAB_ORDRE.Mouvement(m_lignePionSelection.ID_PION);
            Donnees.TAB_CASERow ligneCaseDestination = Donnees.m_donnees.TAB_CASE.FindParID_CASE(ligneOrdre.ID_CASE_DESTINATION);
            Donnees.TAB_CASERow ligneCaseDepart = (m_lignePionSelection.I_INFANTERIE > 0 || m_lignePionSelection.I_CAVALERIE > 0 || m_lignePionSelection.I_ARTILLERIE > 0) ? Donnees.m_donnees.TAB_CASE.FindParID_CASE(ligneOrdre.ID_CASE_DEPART) : Donnees.m_donnees.TAB_CASE.FindParID_CASE(m_lignePionSelection.ID_CASE);

            /*string requete = string.Format("ID_VILLE_DEBUT=26 AND ID_VILLE_FIN=50");
            Donnees.TAB_PCC_VILLESRow[] lignesCout = (Donnees.TAB_PCC_VILLESRow[])Donnees.m_donnees.TAB_PCC_VILLES.Select(requete);
            requete = string.Format("ID_VILLE_DEBUT=50 AND ID_VILLE_FIN=26");
            Donnees.TAB_PCC_VILLESRow[] lignesCout2 = (Donnees.TAB_PCC_VILLESRow[])Donnees.m_donnees.TAB_PCC_VILLES.Select(requete);*/

            AStar.CalculModeleMouvementsPion(out AstarTerrain[] tableCoutsMouvementsTerrain);
            if (!m_etoileHPA.SearchPathHPA(ligneCaseDepart, ligneCaseDestination, tableCoutsMouvementsTerrain))
            {
                MessageBox.Show("Il n'y a aucun chemin possible entre les deux points", "buttonVerifierTrajet_Click", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                m_cheminVerifierTrajet = m_etoileHPA.PathByNodes;
            }
            ConstruireImageCarte();
        }

        private void buttonRecalculTrajet_Click(object sender, EventArgs e)
        {
            Donnees.TAB_ORDRERow ligneOrdre = Donnees.m_donnees.TAB_ORDRE.Mouvement(m_lignePionSelection.ID_PION);
            Donnees.TAB_CASERow ligneCaseDestination = Donnees.m_donnees.TAB_CASE.FindParID_CASE(ligneOrdre.ID_CASE_DESTINATION);
            Donnees.TAB_CASERow ligneCaseDepart = (m_lignePionSelection.effectifTotal > 0) ? Donnees.m_donnees.TAB_CASE.FindParID_CASE(ligneOrdre.ID_CASE_DEPART) : Donnees.m_donnees.TAB_CASE.FindParID_CASE(m_lignePionSelection.ID_CASE);
            Donnees.TAB_NATIONRow ligneNation = m_lignePionSelection.nation; // Donnees.m_donnees.TAB_PION.TrouveNation(m_lignePionSelection);
            //Donnees.TAB_NOMS_CARTERow ligneNom=null;
            int avancementPion;

            //if (!ligneOrdre.IsID_NOM_DESTINATIONNull())
            //{
            //    ligneNom = Donnees.m_donnees.TAB_NOMS_CARTE.FindByID_NOM(ligneOrdre.ID_NOM_DESTINATION);
            //}

            //quel quantit� du trajet l'unit� avait-elle parcouru ?
            if (m_lignePionSelection.effectifTotal > 0)
            {
                avancementPion = m_lignePionSelection.CalculPionPositionRelativeAvancement(ligneOrdre, ligneNation, out m_cheminSelection);
            }
            else
            {
                avancementPion = Cartographie.AvancementPourRecalcul(Constantes.TYPEPARCOURS.MOUVEMENT, m_lignePionSelection, ligneCaseDepart, ligneCaseDestination, out _);
            }

            if (avancementPion < 0)
            {
                MessageBox.Show("Erreur dans le calcul de l'avancement.", "buttonRecalculTrajet_Click", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //l'unit� change d'emplacement de facto
            Donnees.m_donnees.TAB_ESPACE.SupprimerEspacePion(m_lignePionSelection.ID_PION);
            Donnees.m_donnees.TAB_PARCOURS.SupprimerParcoursPion(m_lignePionSelection.ID_PION);
            //Recalcul du nouveau trajet
            AStar etoile = new();
            if (!etoile.RechercheChemin(Constantes.TYPEPARCOURS.MOUVEMENT,
                m_lignePionSelection, Donnees.m_donnees.TAB_CASE.FindParID_CASE(ligneOrdre.ID_CASE_DEPART), ligneCaseDestination, 
                out m_cheminSelection, out _, out _, out _, out string messageErreur))
            {
                MessageBox.Show("Erreur dans RechercheChemin : "+ messageErreur, "buttonRecalculTrajet_Click", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (m_lignePionSelection.effectifTotal > 0)
            {
                //il faut repositionner l'unit� suivant son avancement
                if (avancementPion >= m_cheminSelection.Count)
                {
                    //l'unit� devrait d�j� �tre arriv�e
                    m_lignePionSelection.ID_CASE = m_cheminSelection[^1].ID_CASE;
                    ligneOrdre.I_EFFECTIF_DEPART = 0;
                    ligneOrdre.I_EFFECTIF_DESTINATION = m_lignePionSelection.effectifTotal;
                }
                else
                {
                    // On repositionne l'unit� comme si elle �tait en bivouac, il serait complexe de faire un calcul relatif aux troupes arriv�es ou pas suivant la nouvelle longueur du chemin 
                    // par rapport � sa destination si celle-ci est plus courte que la pr�c�dente
                    m_lignePionSelection.PlacerPionEnBivouac(ligneOrdre, ligneNation);
                }
            }
            else
            {
                if (avancementPion >= m_cheminSelection.Count)
                {
                    m_lignePionSelection.ID_CASE = m_cheminSelection[^1].ID_CASE;
                }
                else
                {
                    m_lignePionSelection.ID_CASE = m_cheminSelection[avancementPion].ID_CASE;
                }
            }
            MessageBox.Show("Recalcul du trajet effectu�.", "buttonRecalculTrajet_Click", MessageBoxButtons.OK, MessageBoxIcon.Information);

            ConstruireImageCarte();
        }

        private void genererLeFilmToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("La g�n�ration de vid�o se fait sur un projet s�par� car il utilise des m�thodes qui ne sont pas 'safe'.", "Generation de Vid�o", MessageBoxButtons.OK, MessageBoxIcon.Information);
            FormVideo frmVideo = new()
            {
                repertoireSource = Constantes.repertoireDonnees,
                fichierCourant = this.fichierCourant
            };
            frmVideo.Show(this);
        }

        private void toolStripButtonTrajetsVilles_Click(object sender, EventArgs e)
        {
            ConstruireImageCarte();
        }

        private void UncheckButtons(ToolStripButton boutonCheck)
        {
            if (toolStripPlusCourtChemin != boutonCheck) { toolStripPlusCourtChemin.CheckState = CheckState.Unchecked; }
            if (toolStripButtonReparerPont != boutonCheck) { toolStripButtonReparerPont.CheckState = CheckState.Unchecked; }
            if (toolStripButtonPontDetruit != boutonCheck) { toolStripButtonPontDetruit.CheckState = CheckState.Unchecked; }
            if (toolStripButtonConstruirePonton != boutonCheck) { toolStripButtonConstruirePonton.CheckState = CheckState.Unchecked; }
            if (toolStripButtonPontEndommage != boutonCheck) { toolStripButtonPontEndommage.CheckState = CheckState.Unchecked; }            
        }

        private void toolStripButtonPlusCourtChemin_Click(object sender, EventArgs e)
        {
            UncheckButtons(toolStripPlusCourtChemin);

            if (toolStripPlusCourtChemin.CheckState == CheckState.Unchecked)
            {
                panelTestPlusCourtChemin.Visible = false;
            }
            else
            {
                panelTestPlusCourtChemin.Visible = true;
                //panelTestPlusCourtChemin.Dock = DockStyle.Fill;

                //remise � z�ro des valeurs de test
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

        private void toolStripButtonConstruirePonton_Click(object sender, EventArgs e)
        {
            UncheckButtons(toolStripButtonConstruirePonton);
        }

        private void toolStripButtonPont_Click(object sender, EventArgs e)
        {
            UncheckButtons(toolStripButtonReparerPont);
        }

        private void toolStripButtonPontDetruit_Click(object sender, EventArgs e)
        {
            UncheckButtons(toolStripButtonPontDetruit);
        }

        private void toolStripButtonPontEndommage_Click(object sender, EventArgs e)
        {
            UncheckButtons(toolStripButtonPontEndommage);
        }

        private void nomsDesLeadesPromusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormNomsPromus fNomsPromus = new()
            {
                tableNomsPromus = Donnees.m_donnees.TAB_NOMS_PROMUS
            };
            if (DialogResult.OK == fNomsPromus.ShowDialog())
            {
                m_modification = true;
                Donnees.m_donnees.TAB_NOMS_PROMUS.Clear();
                Donnees.m_donnees.TAB_NOMS_PROMUS.Merge(fNomsPromus.tableNomsPromus, false);
            }
        }

        private void statistiquesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormStatistiques fStatistiques = new(fichierCourant);
            fStatistiques.ShowDialog();
        }

        private void pontsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormPonts fPonts = new();
            if (DialogResult.OK == fPonts.ShowDialog(this))
            {
                //Mise � jour des noms des ponts
                //on supprise tous les anciens ponts
                int i = 0;
                while (i< Donnees.m_donnees.TAB_NOMS_CARTE.Count)
                {
                    Donnees.TAB_NOMS_CARTERow ligneNom = Donnees.m_donnees.TAB_NOMS_CARTE[i];
                    if (ligneNom.B_PONT)
                    {
                        ligneNom.Delete();
                    }
                    else
                    {
                        i++;
                    }
                }

                //on ajoute les noms des ponts
                List<NomDePont> liste = fPonts.GenererLesNomsDePont();
                foreach (NomDePont nom in liste)
                {
                    bool bNouveau = true;
                    //on regarde s'il existe d�j� un nom sur la m�me case avec le mot "pont"
                    Donnees.TAB_NOMS_CARTERow[] listeNomCarte = (Donnees.TAB_NOMS_CARTERow[])Donnees.m_donnees.TAB_NOMS_CARTE.Select(string.Format("ID_CASE={0}",nom.ID_CASE));
                    if (listeNomCarte.Length > 0)
                    {
                        foreach (Donnees.TAB_NOMS_CARTERow ligneNomCarte in listeNomCarte)
                        {
                            if (ligneNomCarte.S_NOM.ToUpper().Contains("PONT"))
                            {
                                bNouveau = false;
                                //on fait une mise � jour des noms au cas o� la r�gle de g�n�ration aurait chang�e
                                ligneNomCarte.S_NOM = nom.S_NOM;
                            }
                        }
                    }

                    if (bNouveau)
                    {
                        Donnees.TAB_NOMS_CARTERow ligneNomCarte = Donnees.m_donnees.TAB_NOMS_CARTE.AddTAB_NOMS_CARTERow(
                            nom.ID_CASE, //int ID_CASE,
                            Donnees.m_donnees.TAB_NOMS_CARTE[0].ID_POLICE, //int ID_POLICE,
                            nom.S_NOM,  //string S_NOM,
                            0,   //short I_POSITION,
                            0,  //int I_VICTOIRE,
                            -1, //int ID_NATION_CONTROLE,
                            0,  //int I_CHANCE_RENFORT,
                            Donnees.m_donnees.TAB_MODELE_PION[0].ID_MODELE_PION, //int ID_MODELE_PION_RENFORT,
                            -1, //int ID_PION_PROPRIETAIRE_RENFORT,
                            false, //bool B_HOPITAL,
                            false, //bool B_PRISON,
                            0,                                //int I_INFANTERIE_RENFORT,
                            0,                                //int I_CAVALERIE_RENFORT,
                            0,                                //int I_ARTILLERIE_RENFORT,
                            0,                                //int I_MORAL_RENFORT,
                            0,                                //int I_MATERIEL_RENFORT,
                            0,                                //int I_RAVITAILLEMENT_RENFORT,
                            true,//bool B_PONT
                            nom.S_NOM_INDEX,
                            nom.I_X,
                            nom.I_Y,
                            false//bool B_CREATION_DEPOT
                            );
                        ligneNomCarte.SetID_NATION_CONTROLENull();
                        //ligneNomCarte.SetID_MODELE_PION_RENFORTNull(); -> affiche toujours un r�sultat dans la liste d�roulante
                        ligneNomCarte.SetID_PION_PROPRIETAIRE_RENFORTNull();
                    }
                }
            }
        }

        private void messagesAnciensToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormMessageAncien fMessagesAncien = new()
            {
                tableMessage = Donnees.m_donnees.TAB_MESSAGE_ANCIEN
            };
            if (DialogResult.OK == fMessagesAncien.ShowDialog())
            {
                m_modification = true;
                Donnees.m_donnees.TAB_MESSAGE_ANCIEN.Clear();
                Donnees.m_donnees.TAB_MESSAGE_ANCIEN.Merge(fMessagesAncien.tableMessage, false);
            }
        }

        private void pionsAnciensToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormPionAncien fPion = new()
            {
                tablePions = Donnees.m_donnees.TAB_PION_ANCIEN
            };
            if (DialogResult.OK == fPion.ShowDialog())
            {
                m_modification = true;

                Donnees.m_donnees.TAB_PION_ANCIEN.Clear();
                Donnees.m_donnees.TAB_PION_ANCIEN.Merge(fPion.tablePions, false);
            }
        }

        private void ordresAnciensToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormOrdreAncien fOrdre = new()
            {
                tableOrdre = Donnees.m_donnees.TAB_ORDRE_ANCIEN
            };
            if (DialogResult.OK == fOrdre.ShowDialog())
            {
                m_modification = true;
                Donnees.m_donnees.TAB_ORDRE_ANCIEN.Clear();
                Donnees.m_donnees.TAB_ORDRE_ANCIEN.Merge(fOrdre.tableOrdre, false);
            }
        }

        private void repriseDeDonn�esToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormReprise fReprise = new()
            {
                fichierCourant = this.fichierCourant
            };
            if (DialogResult.OK == fReprise.ShowDialog())
            {
                m_modification = true;
            }
        }

        private void donneesVid�oToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormVideoTable fVideoTable = new()
            {
                tableVideo = Donnees.m_donnees.TAB_VIDEO
            };
            if (DialogResult.OK == fVideoTable.ShowDialog())
            {
                m_modification = true;
                Donnees.m_donnees.TAB_VIDEO.Clear();
                Donnees.m_donnees.TAB_VIDEO.Merge(fVideoTable.tableVideo, false);
            }
        }

        private void toolStripButtonMemoire_Click(object sender, EventArgs e)
        {
            ConstruireImageCarte();
        }

        private void actuelsAnciensToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int nbPions = Donnees.m_donnees.TAB_PION.Count;
            int nbMessage = Donnees.m_donnees.TAB_MESSAGE.Count;
            int nbOrdres = Donnees.m_donnees.TAB_ORDRE.Count;

            ClassTraitementHeure traitement = new();
            traitement.ActuelsVersAnciens();

            MessageBox.Show(string.Format("Apr�s actuelsAnciens #pions avant={0} apr�s={1} #messages avant={2} apr�s={3} #ordres avant={4} apr�s={5}",
                nbPions,
                Donnees.m_donnees.TAB_PION.Count,
                nbMessage,
                Donnees.m_donnees.TAB_MESSAGE.Count,
                nbOrdres,
                Donnees.m_donnees.TAB_ORDRE.Count),"R�sultat",MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void anciensActuelsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int nbPions = Donnees.m_donnees.TAB_PION.Count;
            int nbMessage = Donnees.m_donnees.TAB_MESSAGE.Count;
            int nbOrdres = Donnees.m_donnees.TAB_ORDRE.Count;

            #region Backup des messages, ordres et pions d�truits ou pass�s
            /* Retrait de l'optimisation sur les messages, en effets, certains pouvant �tre cr�es sur le web d'autres par l'interface, cela cr�e des bugs d'identifiants. */
            /*
            int i = 0;
            while (i<Donnees.m_donnees.TAB_MESSAGE_ANCIEN.Count())
            {
                Donnees.TAB_MESSAGE_ANCIENRow ligneMessage = Donnees.m_donnees.TAB_MESSAGE_ANCIEN[i];
                Donnees.TAB_MESSAGERow ligneMessageAncien = Donnees.m_donnees.TAB_MESSAGE.AddTAB_MESSAGERow(
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

                if (ligneMessage.IsI_TOUR_ARRIVEENull()) { ligneMessageAncien.SetI_TOUR_ARRIVEENull(); }
                if (ligneMessage.IsI_PHASE_ARRIVEENull()) { ligneMessageAncien.SetI_PHASE_ARRIVEENull(); }
                if (ligneMessage.IsI_TOUR_DEPARTNull()) { ligneMessageAncien.SetI_TOUR_DEPARTNull(); }
                if (ligneMessage.IsI_PHASE_DEPARTNull()) { ligneMessageAncien.SetI_PHASE_DEPARTNull(); }
                if (ligneMessage.IsS_TEXTENull()) { ligneMessageAncien.SetS_TEXTENull(); }
                if (ligneMessage.IsI_INFANTERIENull()) { ligneMessageAncien.SetI_INFANTERIENull(); }
                if (ligneMessage.IsI_CAVALERIENull()) { ligneMessageAncien.SetI_CAVALERIENull(); }
                if (ligneMessage.IsI_ARTILLERIENull()) { ligneMessageAncien.SetI_ARTILLERIENull(); }
                if (ligneMessage.IsI_FATIGUENull()) { ligneMessageAncien.SetI_FATIGUENull(); }
                if (ligneMessage.IsI_MORALNull()) { ligneMessageAncien.SetI_MORALNull(); }
                if (ligneMessage.IsI_TOUR_SANS_RAVITAILLEMENTNull()) { ligneMessageAncien.SetI_TOUR_SANS_RAVITAILLEMENTNull(); }
                if (ligneMessage.IsID_BATAILLENull()) { ligneMessageAncien.SetID_BATAILLENull(); }
                if (ligneMessage.IsI_ZONE_BATAILLENull()) { ligneMessageAncien.SetI_ZONE_BATAILLENull(); }
                if (ligneMessage.IsI_RETRAITENull()) { ligneMessageAncien.SetI_RETRAITENull(); }
                if (ligneMessage.IsID_CASE_DEBUTNull()) { ligneMessageAncien.SetID_CASE_DEBUTNull(); }
                if (ligneMessage.IsID_CASE_FINNull()) { ligneMessageAncien.SetID_CASE_FINNull(); }
                if (ligneMessage.IsI_NB_PHASES_MARCHE_JOURNull()) { ligneMessageAncien.SetI_NB_PHASES_MARCHE_JOURNull(); }
                if (ligneMessage.IsI_NB_PHASES_MARCHE_NUITNull()) { ligneMessageAncien.SetI_NB_PHASES_MARCHE_NUITNull(); }
                if (ligneMessage.IsI_NB_HEURES_COMBATNull()) { ligneMessageAncien.SetI_NB_HEURES_COMBATNull(); }
                if (ligneMessage.IsI_MATERIELNull()) { ligneMessageAncien.SetI_MATERIELNull(); }
                if (ligneMessage.IsI_RAVITAILLEMENTNull()) { ligneMessageAncien.SetI_DUREE_HORS_COMBATNull(); }
                if (ligneMessage.IsI_SOLDATS_RAVITAILLESNull()) { ligneMessageAncien.SetI_SOLDATS_RAVITAILLESNull(); }
                if (ligneMessage.IsI_NB_HEURES_FORTIFICATIONNull()) { ligneMessageAncien.SetI_NB_HEURES_FORTIFICATIONNull(); }
                if (ligneMessage.IsI_NIVEAU_FORTIFICATIONNull()) { ligneMessageAncien.SetI_NIVEAU_FORTIFICATIONNull(); }
                if (ligneMessage.IsI_DUREE_HORS_COMBATNull()) { ligneMessageAncien.SetI_DUREE_HORS_COMBATNull(); }
                if (ligneMessage.IsI_TOUR_BLESSURENull()) { ligneMessageAncien.SetI_TOUR_BLESSURENull(); }
                if (ligneMessage.IsC_NIVEAU_DEPOTNull()) { ligneMessageAncien.SetC_NIVEAU_DEPOTNull(); }

                ligneMessage.Delete();                
            }
            */
            #region r�cup�raton des ordres anciens
            int i = 0;
            while (i<Donnees.m_donnees.TAB_ORDRE_ANCIEN.Count)
            {
                Donnees.TAB_ORDRE_ANCIENRow ligneOrdre = Donnees.m_donnees.TAB_ORDRE_ANCIEN[i];
                if (ligneOrdre.I_ORDRE_TYPE != Constantes.ORDRES.RAVITAILLEMENT_DIRECT)
                {
                    i++;
                }
                else
                {
                    //Donnees.TAB_ORDRERow ligneOrdreAncien = 
                    Donnees.m_donnees.TAB_ORDRE.AddTAB_ORDRERow(
                    //ligneOrdre.ID_ORDRE,
                    ligneOrdre.ID_ORDRE_TRANSMIS,
                    ligneOrdre.ID_ORDRE_SUIVANT,
                    ligneOrdre.ID_ORDRE_WEB,
                    ligneOrdre.I_ORDRE_TYPE,
                    ligneOrdre.ID_PION,
                    ligneOrdre.ID_CASE_DEPART,
                    ligneOrdre.I_EFFECTIF_DEPART,
                    ligneOrdre.ID_CASE_DESTINATION,
                    ligneOrdre.ID_NOM_DESTINATION,
                    ligneOrdre.I_EFFECTIF_DESTINATION,
                    ligneOrdre.I_TOUR_DEBUT,
                    ligneOrdre.I_PHASE_DEBUT,
                    ligneOrdre.I_TOUR_FIN,
                    ligneOrdre.I_PHASE_FIN,
                    ligneOrdre.ID_MESSAGE,
                    ligneOrdre.ID_DESTINATAIRE,
                    ligneOrdre.ID_CIBLE,
                    ligneOrdre.ID_DESTINATAIRE_CIBLE,
                    ligneOrdre.ID_BATAILLE,
                    ligneOrdre.I_ZONE_BATAILLE,
                    ligneOrdre.I_HEURE_DEBUT,
                    ligneOrdre.I_DUREE,
                    ligneOrdre.I_ENGAGEMENT);

                    ligneOrdre.Delete();
                }
            }
            
            #endregion
            /* complexe aussi sur les pions, quand on remonte sur proprietaire dans sauvegardeMesage de ClassVaocWebFichier, il faut des fois �tre sur un ancien des fois non et pas possible
             *  a priori de faire un cast automatique entre les deux */
            /*
           while (Donnees.m_donnees.TAB_PION.Count()>0)
           {
               Donnees.TAB_PION_ANCIENRow lignePion = Donnees.m_donnees.TAB_PION_ANCIEN[0];
               Donnees.TAB_PIONRow lignePionAncien = Donnees.m_donnees.TAB_PION.AddTAB_PIONRow(
                   lignePion.ID_PION,
                   lignePion.ID_MODELE_PION,
                   lignePion.ID_PION_PROPRIETAIRE,
                   lignePion.ID_NOUVEAU_PION_PROPRIETAIRE,
                   lignePion.ID_ANCIEN_PION_PROPRIETAIRE,
                   lignePion.S_NOM,
                   lignePion.I_INFANTERIE,
                   lignePion.I_INFANTERIE_INITIALE,
                   lignePion.I_CAVALERIE,
                   lignePion.I_CAVALERIE_INITIALE,
                   lignePion.I_ARTILLERIE,
                   lignePion.I_ARTILLERIE_INITIALE,
                   lignePion.I_FATIGUE,
                   lignePion.I_MORAL,
                   lignePion.I_MORAL_MAX,
                   lignePion.I_EXPERIENCE,
                   lignePion.I_TACTIQUE,
                   lignePion.I_STRATEGIQUE,
                   lignePion.C_NIVEAU_HIERARCHIQUE,
                   lignePion.I_DISTANCE_A_PARCOURIR,
                   lignePion.I_NB_PHASES_MARCHE_JOUR,
                   lignePion.I_NB_PHASES_MARCHE_NUIT,
                   lignePion.I_NB_HEURES_COMBAT,
                   lignePion.ID_CASE,
                   lignePion.I_TOUR_SANS_RAVITAILLEMENT,
                   lignePion.ID_BATAILLE,
                   lignePion.I_ZONE_BATAILLE,
                   lignePion.I_TOUR_RETRAITE_RESTANT,
                   lignePion.I_TOUR_FUITE_RESTANT,
                   lignePion.B_DETRUIT,
                   lignePion.B_FUITE_AU_COMBAT,
                   lignePion.B_INTERCEPTION,
                   lignePion.B_REDITION_RAVITAILLEMENT,
                   lignePion.B_TELEPORTATION,
                   lignePion.B_ENNEMI_OBSERVABLE,
                   lignePion.I_MATERIEL,
                   lignePion.I_RAVITAILLEMENT,
                   lignePion.B_CAVALERIE_DE_LIGNE,
                   lignePion.B_CAVALERIE_LOURDE,
                   lignePion.B_GARDE,
                   lignePion.B_VIEILLE_GARDE,
                   lignePion.I_TOUR_CONVOI_CREE,
                   lignePion.ID_DEPOT_SOURCE,
                   lignePion.I_SOLDATS_RAVITAILLES,
                   lignePion.I_NB_HEURES_FORTIFICATION,
                   lignePion.I_NIVEAU_FORTIFICATION,
                   lignePion.ID_PION_REMPLACE,
                   lignePion.I_DUREE_HORS_COMBAT,
                   lignePion.I_TOUR_BLESSURE,
                   lignePion.B_BLESSES,
                   lignePion.B_PRISONNIERS,
                   lignePion.B_RENFORT,
                   lignePion.ID_LIEU_RATTACHEMENT,
                   lignePion.C_NIVEAU_DEPOT,
                   lignePion.ID_PION_ESCORTE,
                   lignePion.I_INFANTERIE_ESCORTE,
                   lignePion.I_CAVALERIE_ESCORTE,
                   lignePion.I_MATERIEL_ESCORTE);

               lignePion.Delete();
           }
           */
            MessageBox.Show(string.Format("Apr�s anciensActuels #pions avant={0} apr�s={1} #messages avant={2} apr�s={3} #ordres avant={4} apr�s={5}",
                nbPions,
                Donnees.m_donnees.TAB_PION.Count,
                nbMessage,
                Donnees.m_donnees.TAB_MESSAGE.Count,
                nbOrdres,
                Donnees.m_donnees.TAB_ORDRE.Count), "R�sultat", MessageBoxButtons.OK, MessageBoxIcon.Information);

            #endregion
        }

        private void nomsPionsUniquesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormNomsPions fNompsPionsTable = new()
            {
                tableNomsPions = Donnees.m_donnees.TAB_NOMS_PIONS
            };
            if (DialogResult.OK == fNompsPionsTable.ShowDialog())
            {
                m_modification = true;
                Donnees.m_donnees.TAB_NOMS_PIONS.Clear();
                Donnees.m_donnees.TAB_NOMS_PIONS.Merge(fNompsPionsTable.tableNomsPions, false);
            }
        }

        private void extractionDeLaBaseEnCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string nomsfichier = string.Empty;
            string messageErreur = string.Empty;
            foreach (DataTable table in Donnees.m_donnees.Tables)
            {
                messageErreur += Dal.exportCSV(table, out string nomfichier);
                nomsfichier += nomfichier + ",";
            }

            if (string.Empty == messageErreur)
            {
                MessageBox.Show("Fichiers CSV export� : " + nomsfichier, "Export CSV", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Erreur sur l'export du fichier CSV : " + nomsfichier + " : " + messageErreur,
                    "FormPion", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void sQLDeTousLesMessagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Donnees.m_donnees.TAB_PARTIE.Count <= 0)
            {
                MessageBox.Show("Vous devez charger une partie pour pouvoir en exporter les messages.","SQL Messages", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                InterfaceVaocWeb iWeb;
                iWeb = ClassVaocWebFactory.CreerVaocWeb(fichierCourant, "_messages", true);
                iWeb.SauvegardeMessage(Donnees.m_donnees.TAB_PARTIE[0].ID_PARTIE, -1);
                MessageBox.Show("Fichier message export� : " + iWeb.fileNameSQL, "SQL Messages", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void broderiequadrillageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormBroderie broderie = new();
            broderie.ShowDialog();
        }

        private void copieDeSauvegardeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormBackup copieDeSauvegarde = new()
            {
                fichierCourant = fichierCourant
            };
            copieDeSauvegarde.ShowDialog();
        }

        private void forcesInitialesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormForcesInitiales forcesInitiales = new();
            forcesInitiales.ShowDialog();
        }

        private void mise�JourProprietairesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormMajCases majDesCases = new();
            majDesCases.ShowDialog();
        }

        private void toolStripButtonDepots_Click(object sender, EventArgs e)
        {
            ConstruireImageCarte();
        }

        private void bataillesVideoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormBataillesVideo fBataillesVideo = new()
            {
                tableBatailleVideo = Donnees.m_donnees.TAB_BATAILLE_VIDEO,
                tableBataillePionsVideo = Donnees.m_donnees.TAB_BATAILLE_PIONS_VIDEO
            };
            if (DialogResult.OK == fBataillesVideo.ShowDialog())
            {
                m_modification = true;
                Donnees.m_donnees.TAB_BATAILLE_VIDEO.Clear();
                Donnees.m_donnees.TAB_BATAILLE_VIDEO.Merge(fBataillesVideo.tableBatailleVideo, false);

                Donnees.m_donnees.TAB_BATAILLE_PIONS_VIDEO.Clear();
                Donnees.m_donnees.TAB_BATAILLE_PIONS_VIDEO.Merge(fBataillesVideo.tableBataillePionsVideo, false);
                foreach (Donnees.TAB_BATAILLE_PIONS_VIDEORow ligneBataillePionVideo in Donnees.m_donnees.TAB_BATAILLE_PIONS_VIDEO)
                {
                    if (ligneBataillePionVideo.IsB_ENGAGEENull()) { ligneBataillePionVideo.B_ENGAGEE = false; }
                    if (ligneBataillePionVideo.IsB_ENGAGEMENTNull()) { ligneBataillePionVideo.B_ENGAGEMENT = false; }
                    if (ligneBataillePionVideo.IsB_EN_DEFENSENull()) { ligneBataillePionVideo.B_EN_DEFENSE = false; }
                    if (ligneBataillePionVideo.IsB_RETRAITENull()) { ligneBataillePionVideo.B_RETRAITE = false; }
                    if (ligneBataillePionVideo.IsI_INFANTERIENull()) { ligneBataillePionVideo.I_INFANTERIE = 0; }
                    if (ligneBataillePionVideo.IsI_CAVALERIENull()) { ligneBataillePionVideo.I_CAVALERIE = 0; }
                    if (ligneBataillePionVideo.IsI_ARTILLERIENull()) { ligneBataillePionVideo.I_ARTILLERIE = 0; }
                    if (ligneBataillePionVideo.IsI_FATIGUENull()) { ligneBataillePionVideo.I_FATIGUE = 0; }
                    if (ligneBataillePionVideo.IsI_MORALNull()) { ligneBataillePionVideo.I_MORAL = 0; }
                }
            }
        }

        private void genererLesFilmsDeBatailleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormVideoBatailles fVideoBatailles = new()
            {
                m_nomfichier = this.curFileName
            };
            fVideoBatailles.ShowDialog();
        }
    }

    internal static partial class NativeMethods
    {
        // Import SetThreadExecutionState Win32 API and necessary flags
        [LibraryImport("kernel32.dll")]
        public static partial uint SetThreadExecutionState(uint esFlags);
        public const uint ES_CONTINUOUS = 0x80000000;
        public const uint ES_SYSTEM_REQUIRED = 0x00000001;
    }
}