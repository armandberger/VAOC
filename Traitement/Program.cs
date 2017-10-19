using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using WaocLib;
using System.Diagnostics;
using System.IO.Compression;

namespace Traitement
{
    class Program
    {
        static void Main(string[] args)
        {
            //ConversionImage();
            //ConversionBaseNull();
            ConversionFichiersCasesNull();
        }

        private static void ConversionFichiersCasesNull()
        {
            const int NULLENTIER = -int.MaxValue;
            const int CST_COUTMAX = Int32.MaxValue;
            int compteur = 0;
            string[] listefichiers;

            try
            {
                Console.WriteLine("conversion des cases");
                Donnees.TAB_CASEDataTable donneesSource = new Donnees.TAB_CASEDataTable();
                Console.WriteLine("fin de la conversion des cases");
                listefichiers = Directory.GetFiles("C:\\berlin\\cases");
                foreach (string nomfichier in listefichiers)
                {
                    compteur++;
                    Console.WriteLine(string.Format("conversion de {0}, {1}/{2}",nomfichier, compteur, listefichiers.Count()));
                    ZipArchive fichierZip = ZipFile.OpenRead(nomfichier);
                    ZipArchiveEntry fichier = fichierZip.Entries[0];
                    fichier.ExtractToFile("C:\\berlin\\test.xml",true);
                    donneesSource.Rows.Clear();
                    donneesSource.ReadXml(fichier.Open());//m_donnees.TAB_CASE.ReadXml(fichier.Open()); ne marche pas mais je ne sais pas pourquoi !
                    fichierZip.Dispose();

                    foreach (Donnees.TAB_CASERow ligneCase in donneesSource)
                    {
                        if (ligneCase.IsID_MODELE_TERRAINNull()) { ligneCase.ID_MODELE_TERRAIN = NULLENTIER; }
                        if (ligneCase.IsID_MODELE_TERRAIN_SI_OCCUPENull()) { ligneCase.ID_MODELE_TERRAIN_SI_OCCUPE = NULLENTIER; }
                        if (ligneCase.IsID_NOUVEAU_PROPRIETAIRENull()) { ligneCase.ID_NOUVEAU_PROPRIETAIRE = NULLENTIER; }
                        if (ligneCase.IsID_PROPRIETAIRENull()) { ligneCase.ID_PROPRIETAIRE = NULLENTIER; }
                        if (ligneCase.IsI_COUTNull()) { ligneCase.I_COUT = CST_COUTMAX; }
                        if (ligneCase.IsI_XNull()) { ligneCase.I_X = NULLENTIER; }
                        if (ligneCase.IsI_YNull()) { ligneCase.I_Y = NULLENTIER; }
                    }

                    File.Delete(nomfichier);
                    fichierZip = ZipFile.Open(nomfichier, ZipArchiveMode.Create);
                    fichier = fichierZip.CreateEntry(nomfichier);
                    StreamWriter ecrivain = new StreamWriter(fichier.Open());
                    donneesSource.WriteXml(ecrivain);
                    ecrivain.Close();
                    fichierZip.Dispose();
                }
            }
            catch (Exception ex)
            {
                if (null != ex.InnerException)
                {
                    Console.WriteLine("Exception :" + ex.Message + " : " + ex.InnerException.Message);
                }
                else
                {
                    Console.WriteLine("Exception :" + ex.Message + " : sans détail");
                }
            }
            Console.ReadKey();
        }

        /// <summary>
        /// Conversion les données d'une base autorisant les DBNulls en ajoutant des valeurs dans les colonnes
        /// </summary>
        private static void ConversionBaseNull()
        {
            try
            {
                const int NULLENTIER = -int.MaxValue;
                const string NULLCHAINE = "NAZE";
                const char NULLCHAR = '?';
                //string nomfichierSource = "C:\\Users\\Public\\Documents\\vaoc\\1813\\berlin_printemps_3.vaoc";
                string nomfichierSource = "C:\\berlin\\berlin_printemps_6.vaoc";
                //string nomfichierDestination = "C:\\Users\\Public\\Documents\\vaoc\\1813\\berlin_printemps_3_converti.vaoc"; //"C:\\berlin\\berlin_printemps_3.vaoc";
                string nomfichierDestination = "C:\\berlin\\berlin_printemps_corrige_6.vaoc";

                Donnees m_base = new Donnees();
                Constantes.repertoireDonnees = nomfichierSource;
                Console.WriteLine("Charger la partie :"+nomfichierSource);
                Dal.ChargerPartie(nomfichierSource, m_base);
                Console.WriteLine("Conversion des pions");
                foreach (Donnees.TAB_PIONRow lignePion in m_base.TAB_PION)
                {
                    if (lignePion.IsC_NIVEAU_DEPOTNull()) { lignePion.C_NIVEAU_DEPOT = NULLCHAR; }
                    if (lignePion.IsC_NIVEAU_HIERARCHIQUENull()) { lignePion.C_NIVEAU_HIERARCHIQUE = NULLCHAR; }
                    if (lignePion.IsID_ANCIEN_PION_PROPRIETAIRENull()) { lignePion.ID_ANCIEN_PION_PROPRIETAIRE = NULLENTIER; }
                    if (lignePion.IsID_BATAILLENull()) { lignePion.ID_BATAILLE = NULLENTIER; }
                    if (lignePion.IsID_DEPOT_SOURCENull()) { lignePion.ID_DEPOT_SOURCE = NULLENTIER; }
                    if (lignePion.IsID_LIEU_RATTACHEMENTNull()) { lignePion.ID_LIEU_RATTACHEMENT = NULLENTIER; }
                    if (lignePion.IsID_NOUVEAU_PION_PROPRIETAIRENull()) { lignePion.ID_NOUVEAU_PION_PROPRIETAIRE = NULLENTIER; }
                    if (lignePion.IsID_PION_ESCORTENull()) { lignePion.ID_PION_ESCORTE = NULLENTIER; }
                    if (lignePion.IsID_PION_REMPLACENull()) { lignePion.ID_PION_REMPLACE = NULLENTIER; }
                    if (lignePion.IsI_CAVALERIE_ESCORTENull()) { lignePion.I_CAVALERIE_ESCORTE = 0; }
                    if (lignePion.IsI_DISTANCE_A_PARCOURIRNull()) { lignePion.I_DISTANCE_A_PARCOURIR = 0; }
                    if (lignePion.IsI_DUREE_HORS_COMBATNull()) { lignePion.I_DUREE_HORS_COMBAT = 0; }
                    if (lignePion.IsI_INFANTERIE_ESCORTENull()) { lignePion.I_INFANTERIE_ESCORTE = 0; }
                    if (lignePion.IsI_MATERIELNull()) { lignePion.I_MATERIEL = 0; }
                    if (lignePion.IsI_MATERIEL_ESCORTENull()) { lignePion.I_MATERIEL_ESCORTE = 0; }
                    if (lignePion.IsI_NB_HEURES_COMBATNull()) { lignePion.I_NB_HEURES_COMBAT = 0; }
                    if (lignePion.IsI_NB_HEURES_FORTIFICATIONNull()) { lignePion.I_NB_HEURES_FORTIFICATION = 0; }
                    if (lignePion.IsI_NB_PHASES_MARCHE_JOURNull()) { lignePion.I_NB_PHASES_MARCHE_JOUR = 0; }
                    if (lignePion.IsI_NB_PHASES_MARCHE_NUITNull()) { lignePion.I_NB_PHASES_MARCHE_NUIT = 0; }
                    if (lignePion.IsI_NIVEAU_FORTIFICATIONNull()) { lignePion.I_NIVEAU_FORTIFICATION = 0; }
                    if (lignePion.IsI_RAVITAILLEMENTNull()) { lignePion.I_RAVITAILLEMENT = 0; }
                    if (lignePion.IsI_SOLDATS_RAVITAILLESNull()) { lignePion.I_SOLDATS_RAVITAILLES = 0; }
                    if (lignePion.IsI_STRATEGIQUENull()) { lignePion.I_STRATEGIQUE = 0; }
                    if (lignePion.IsI_TACTIQUENull()) { lignePion.I_TACTIQUE = 0; }
                    if (lignePion.IsI_TOUR_BLESSURENull()) { lignePion.I_TOUR_BLESSURE = NULLENTIER; }
                    if (lignePion.IsI_TOUR_CONVOI_CREENull()) { lignePion.I_TOUR_CONVOI_CREE = NULLENTIER; }
                    if (lignePion.IsI_TOUR_DERNIER_RAVITAILLEMENT_DIRECTNull()) { lignePion.I_TOUR_DERNIER_RAVITAILLEMENT_DIRECT = NULLENTIER; }
                    if (lignePion.IsI_TOUR_FUITE_RESTANTNull()) { lignePion.I_TOUR_FUITE_RESTANT = 0; }
                    if (lignePion.IsI_TOUR_RETRAITE_RESTANTNull()) { lignePion.I_TOUR_RETRAITE_RESTANT = 0; }
                    if (lignePion.IsI_TOUR_SANS_RAVITAILLEMENTNull()) { lignePion.I_TOUR_SANS_RAVITAILLEMENT = 0; }
                    if (lignePion.IsI_VICTOIRENull()) { lignePion.I_VICTOIRE = 0; }
                    if (lignePion.IsI_ZONE_BATAILLENull()) { lignePion.I_ZONE_BATAILLE = NULLENTIER; }
                }

                /* out of memory !
                Console.WriteLine("chargement des cases");
                if (!ChargerToutesLesCases(m_base))
                {
                    Console.WriteLine("erreur dans ChargerToutesLesCases");
                }
                Console.WriteLine("Conversion des cases");
                foreach (Donnees.TAB_CASERow ligneCase in m_base.TAB_CASE)
                {
                    if (ligneCase.IsID_MODELE_TERRAIN_SI_OCCUPENull()) { ligneCase.ID_MODELE_TERRAIN = NULLENTIER; }
                    if (ligneCase.IsID_NOUVEAU_PROPRIETAIRENull()) { ligneCase.ID_NOUVEAU_PROPRIETAIRE = NULLENTIER; }
                    if (ligneCase.IsID_PROPRIETAIRENull()) { ligneCase.ID_PROPRIETAIRE = NULLENTIER; }
                    if (ligneCase.IsI_COUTNull()) { ligneCase.I_COUT = CST_COUTMAX; }
                    if (ligneCase.IsI_XNull()) { ligneCase.I_X = NULLENTIER; }
                    if (ligneCase.IsI_YNull()) { ligneCase.I_Y = NULLENTIER; }
                }
                */
                Console.WriteLine("sauvegarde finale");
                Dal.SauvegarderPartie(nomfichierDestination, m_base.TAB_PARTIE[0].I_TOUR, m_base.TAB_PARTIE[0].I_PHASE, m_base, true);
                Console.WriteLine("Conversion terminée :" + nomfichierDestination);
            }
            catch (Exception ex)
            {
                if (null != ex.InnerException)
                {
                    Console.WriteLine("Exception :" + ex.Message + " : " + ex.InnerException.Message);
                }
                else
                {
                    Console.WriteLine("Exception :" + ex.Message + " : sans détail");
                }
            }
            Console.ReadKey();
        }

        private static bool ChargerToutesLesCases(Donnees maBase)
        {
            if (0 == maBase.TAB_PARTIE.Count || 0 == maBase.TAB_JEU.Count)
            {
                return true;
            }
            Donnees.TAB_CASEDataTable donneesSource = new Donnees.TAB_CASEDataTable();
            maBase.TAB_CASE.Clear();
            for (int x = 0; x < maBase.TAB_JEU[0].I_LARGEUR_CARTE; x += Constantes.CST_TAILLE_BLOC_CASES)
            {
                for (int y = 0; y < maBase.TAB_JEU[0].I_HAUTEUR_CARTE; y += Constantes.CST_TAILLE_BLOC_CASES)
                {
                    //on vérifie que le chargement n'a pas déjà été fait ->appelé uniquement en génération de cartes, faux, appeler pour les noms de ponts par exemple
                    string requete = string.Format("I_X={0} AND I_Y={1}", x, y);
                    Donnees.TAB_CASERow[] listeCases = (Donnees.TAB_CASERow[])maBase.TAB_CASE.Select(requete);
                    if (listeCases.Count() > 0)
                    {
                        continue; //cases déjà chargées
                    }
                    if (!ChargerDonnneesCases(ref donneesSource, maBase.TAB_JEU[0].I_NOMBRE_PHASES, x, y, maBase.TAB_PARTIE[0].I_TOUR, maBase.TAB_PARTIE[0].I_PHASE))
                    { return false; }
                }
            }
            maBase.TAB_CASE.Merge(donneesSource, false);
            return true;
        }

        private static bool ChargerDonnneesCases(ref Donnees.TAB_CASEDataTable donneesSource, int nbPhases, int x, int y, int tour, int phase)
        {
            string repertoire, nomfichier;

            try
            {
                repertoire = nomRepertoireCases();
                nomfichier = NomFichierCases(x, y, tour, phase, repertoire);
                //nomfichier = "C:\\Users\\Public\\Documents\\vaoc\\1813\\cases\\1300_200_0_90.cases";

                //on recherche le dernier fichier sauvegardé sur les cases
                int phaserecherche = phase;
                int tourrecherche = tour;
                while (!File.Exists(nomfichier) && tourrecherche >= 0)
                {
                    phaserecherche -= Constantes.CST_SAUVEGARDE_ECART_PHASES;
                    if (phaserecherche < 0)
                    {
                        phaserecherche = nbPhases;
                        tourrecherche--;
                    }
                    nomfichier = NomFichierCases(x, y, tourrecherche, phaserecherche, repertoire);
                    Debug.WriteLine("ChargerCases, test sur le fichier " + nomfichier);
                }

                if (tourrecherche < 0)
                {
                    Console.WriteLine(string.Format("Erreur sur ChargerDonnneesCases : Impossible de trouver un fichiers de cases pour x={0}, y={1}, tour={2}, phase={3}",
                        x, y, tour, phase));
                    return false;
                }

                ZipArchive fichierZip = ZipFile.OpenRead(nomfichier);
                ZipArchiveEntry fichier = fichierZip.Entries[0];
                donneesSource.ReadXml(fichier.Open());//m_donnees.TAB_CASE.ReadXml(fichier.Open()); ne marche pas mais je ne sais pas pourquoi !
                fichierZip.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine("Erreur sur TABCASEDataTables.ChargerDonnneesCases :" + e.Message);
                return false;
            }
            return true;
        }

        private static string nomRepertoireCases()
        {
            return string.Format("{0}cases\\", Constantes.repertoireDonnees);
        }

        private static string NomFichierCases(int x, int y, int tour, int phase, string repertoire)
        {
            string nomfichier = string.Format("{0}{00001}_{00002}_{3}_{4}.cases",
                repertoire, (x / Constantes.CST_TAILLE_BLOC_CASES) * Constantes.CST_TAILLE_BLOC_CASES,
                            (y / Constantes.CST_TAILLE_BLOC_CASES) * Constantes.CST_TAILLE_BLOC_CASES, tour, phase - (phase % Constantes.CST_SAUVEGARDE_ECART_PHASES));
            return nomfichier;
        }


        private static void ConversionImage()
        {
            try
            {
                Bitmap imageSource = (Bitmap)Bitmap.FromFile("C:\\berlin\\1813_terrain.bmp");

                BitmapData imageCible = new BitmapData();
                Rectangle rect = new Rectangle(0, 1128, imageSource.Width, imageSource.Height - 1128);
                imageSource.LockBits(rect, ImageLockMode.ReadOnly, imageSource.PixelFormat, imageCible);
                imageSource.UnlockBits(imageCible);
                Bitmap imageFinale = new Bitmap(imageCible.Width, imageCible.Height, imageCible.Stride, imageCible.PixelFormat, imageCible.Scan0);
                imageFinale.Save("C:\\berlin\\1813P_terrain.bmp", ImageFormat.Bmp);
            }
            catch (Exception ex)
            {
                if (null != ex.InnerException)
                {
                    Console.WriteLine("Exception :" + ex.Message + " : " + ex.InnerException.Message);
                }
                else
                {
                    Console.WriteLine("Exception :" + ex.Message + " : sans détail");
                }
                Console.ReadKey();
            }
        }
    }
}
