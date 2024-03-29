﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

namespace WaocLib
{
    public class Dal
    {
        public struct IconInfo
        {
            public bool fIcon;
            public int xHotspot;
            public int yHotspot;
            public IntPtr hbmMask;
            public IntPtr hbmColor;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetIconInfo(IntPtr hIcon, ref IconInfo pIconInfo);

        [DllImport("user32.dll")]
        public static extern IntPtr CreateIconIndirect(ref IconInfo icon);

        [DllImport("user32.dll")]
        static extern IntPtr CreateIconFromResource(byte[] presbits, uint dwResSize, bool fIcon, uint dwVer);

        public static Cursor Create(byte[] resource)
        {
            IntPtr myNew_Animated_hCursor;

            //byte[] resource = Properties.Resources.flower_anim;

            myNew_Animated_hCursor = CreateIconFromResource(resource, (uint)resource.Length, false, 0x00030000);

            if (!IntPtr.Zero.Equals(myNew_Animated_hCursor))
            {
                // all is good
                return new Cursor(myNew_Animated_hCursor);
            }
            else
            {  // resource wrong type or memory error occurred
                // normally this resource exists since you had to put  Properties.Resources. and a resource would appear and you selected it
                // the animate cursor desired  is the error generator since this call is not required for simple cursors
                throw new ApplicationException("Could not create cursor from Embedded resource ");
            }
        }
        
        /// <summary>
        /// Renvoie le nom du fichier sans les indications de tour ou de phase
        /// </summary>
        /// <param name="nomfichier">nom d'origine</param>
        /// <returns>chaine filtrée</returns>
        static public string NomFichierTourPhase(string nomfichier, int tour, int phase, bool bSuperieur)
        {
            //recopie de la chaine avant l'extension
            int positionPoint = nomfichier.LastIndexOf(".");
            string nomfichierTourPhase = nomfichier.Substring(0, positionPoint);

            //ajout du tour et de la phase
            //amélioration pour que la toute dernière sauvegarde mette directement le bon nom de fichier
            int tourfichier = -1;//indication du tour courant dans le nom du fichier
            int i = nomfichierTourPhase.Length - 1;
            while (char.IsDigit(nomfichierTourPhase[i])) i--;
            //string test = nomfichierTourPhase.Substring(i + 1, nomfichierTourPhase.Length - i - 1);
            if (i != nomfichierTourPhase.Length - 1) tourfichier = Convert.ToInt32(nomfichierTourPhase.Substring(i + 1, nomfichierTourPhase.Length - i - 1));
            if ((tourfichier > 0 && tour > tourfichier && 0 == phase) || !bSuperieur)
            {
                nomfichierTourPhase = string.Format("{0}{1}{2}",
                        nomfichierTourPhase.Substring(0, i + 1),
                        tour,
                        nomfichier.Substring(positionPoint, nomfichier.Length - positionPoint));
            }
            else
            {
                nomfichierTourPhase = string.Format("{0}_{001}_{002}{3}", nomfichierTourPhase, tour, phase, nomfichier.Substring(positionPoint, nomfichier.Length - positionPoint));
            }
            return nomfichierTourPhase;
        }

        static public bool SauvegarderPartie(string nomfichier, int tour, int phase, DataSet donnees, bool bSuperieur)
        {
            Cursor oldCursor = Cursor.Current;
            try
            {
                
                return SauvegarderPartie(NomFichierTourPhase(nomfichier, tour, phase, bSuperieur), donnees);
            }
            catch (Exception e)
            {
                Cursor.Current = oldCursor;
                MessageBox.Show("Erreur sur SauvegarderPartie :" + e.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        static private bool SauvegarderPartie(string nomfichier, DataSet donnees)
        {
            Cursor oldCursor = Cursor.Current;
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                //Monitor.Enter(donnees);//au cas où il y aurait un chargement de case par la souris, la collection va changée, provoquant un crash ->marche pas ce lock
                if (File.Exists(nomfichier))
                {
                    File.Delete(nomfichier);
                }
                ZipArchive fichierZip = ZipFile.Open(nomfichier, ZipArchiveMode.Create);
                ZipArchiveEntry fichier = fichierZip.CreateEntry(nomfichier);
                StreamWriter ecrivain = new StreamWriter(fichier.Open());
                //cette méthode renvoie Erreur sur ChargerPartie :Données non valides au niveau racine. Ligne 1, position 1.
                /*
                bool bpremier = true;
                foreach (DataTable table in donnees.Tables)
                {
                    if (table.TableName != "TAB_CASE")
                    {
                        if (bpremier)
                        {
                            table.WriteXml(ecrivain);
                            bpremier = false;
                        }
                        else
                            table.WriteXml(ecrivain, XmlWriteMode.IgnoreSchema);
                    }
                }
                */
                donnees.WriteXml(ecrivain);
                ecrivain.Close();
                fichierZip.Dispose();
                //donnees.WriteXml(nomfichier);
                //Monitor.Exit(donnees);
                Cursor.Current = oldCursor;
            }
            catch (Exception e)
            {
                Cursor.Current = oldCursor;
                MessageBox.Show("Erreur sur SauvegarderPartie :" + e.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        static public bool ChargerPartie(string nomfichier, DataSet donneesSource)
        {
            Cursor oldCursor = Cursor.Current;

            try
            {
                Cursor.Current = Cursors.WaitCursor;
                donneesSource.Clear();
                //foreach (DataTable table in donneesSource.Tables)
                //    table.BeginLoadData();//accelère le chargement en retirant les constructions d'index, etc.
                try
                {
                    ZipArchive fichierZip = ZipFile.OpenRead(nomfichier);
                    ZipArchiveEntry fichier = fichierZip.Entries[0];
                    donneesSource.ReadXml(fichier.Open());
                    fichierZip.Dispose();
                }
                catch
                {
                    //il s'agit probablement d'un fichier non zippé, on le charge en direct
                    donneesSource.ReadXml(nomfichier);
                }
                foreach (DataTable table in donneesSource.Tables)
                    Debug.WriteLine(table.TableName + " : "+table.Rows.Count.ToString());
                bool bException = false;
                Exception derniereErreur = new Exception();//le new juste pour eviter le warning de non affectation
                foreach (DataTable table in donneesSource.Tables)
                {
                    /*
                    DataRow[] rowErrors = table.GetErrors();

                    Debug.WriteLine(table.TableName + "Errors:"+ rowErrors.Length);

                    for (int i = 0; i < rowErrors.Length; i++)
                    {
                        Debug.WriteLine(rowErrors[i].RowError);

                        foreach (DataColumn col in rowErrors[i].GetColumnsInError())
                        {
                            Debug.WriteLine(col.ColumnName
                                + ":" + rowErrors[i].GetColumnError(col));
                        }
                    }
                    */
                    Debug.WriteLine("EndLoadData:" + table.TableName + " : " + table.Rows.Count.ToString());
                    try
                    {
                        //table.EndLoadData(); -> Gros problème, ne rétablit pas les contraintes, on peut donc ensuite insérer des données en double
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("Exception sur EndLoadData:" + table.TableName + " : " + table.Rows.Count.ToString() + " = " + e.Message);
                        derniereErreur = e;
                        bException = true;
                    }                    
                }
                if (bException)
                {
                    Cursor.Current = oldCursor;
                    MessageBox.Show("Erreur sur ChargerPartie :" + derniereErreur.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                Cursor.Current = oldCursor;
            }
            catch (Exception e)
            {
                Cursor.Current = oldCursor;
                MessageBox.Show("Erreur sur ChargerPartie :" + e.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        static public bool ChargerTrajet(int idTrajet, out List<int> listeCase)
        {
            return ChargerTrajet(idTrajet, "", out listeCase);
        }

        static public bool ChargerTrajet(int idTrajet, string tipe, out List<int> listeCase)
        {
            listeCase = new List<int>();
            try
            {
                string nomFichier = nomFichierTrajet(idTrajet, tipe);
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(nomFichier);
                foreach (XmlNode noeud in xDoc.SelectNodes("/TRAJET/TAB_TRAJET"))//attention, sensible, majuscule/minuscule
                {
                    listeCase.Add(Convert.ToInt32(noeud.Attributes["ID_CASE"].Value));
                }
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show("Erreur sur ChargerTrajet :" + e.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        static public bool SupprimerTrajet(int idTrajet, string tipe)
        {
            if (!Directory.Exists(nomRepertoireTrajet(idTrajet, tipe)))
            {
                return false;
            }
            string nomFichier = nomFichierTrajet(idTrajet, tipe);
            if (!File.Exists(nomFichier))
            {
                return false;
            }
            File.Delete(nomFichier);
            return true;
        }

        static public bool SauvegarderTrajet(int idTrajet, List<int> listeCase)
        {
            return SauvegarderTrajet(idTrajet, "", listeCase);
        }

        static public bool SauvegarderTrajet(int idTrajet, string tipe, List<int> listeCase)
        {
            XmlDocument xDoc = new XmlDocument();
            XmlElement enfant, racine;
            XmlAttribute attribut;
            int i=0;
            try
            {
                if (!Directory.Exists(nomRepertoireTrajet(idTrajet, tipe)))
                {
                    Directory.CreateDirectory(nomRepertoireTrajet(idTrajet, tipe));
                }

                racine = xDoc.CreateElement("TRAJET");
                xDoc.AppendChild(racine);
                XmlDeclaration xmlDeclaration = xDoc.CreateXmlDeclaration("1.0", null, null);
                xmlDeclaration.Encoding = "UTF-8";
                xDoc.InsertBefore(xmlDeclaration, racine);

                while (i<listeCase.Count)
                {
                    enfant = xDoc.CreateElement("TAB_TRAJET");
                    attribut = xDoc.CreateAttribute("ID_CASE");
                    attribut.Value = listeCase[i].ToString();
                    enfant.Attributes.Append(attribut);
                    attribut = xDoc.CreateAttribute("I_ORDRE");
                    attribut.Value = i.ToString();
                    enfant.Attributes.Append(attribut);

                    racine.AppendChild(enfant);
                    i++;
                }
                string nomFichier = nomFichierTrajet(idTrajet, tipe);
                xDoc.Save(nomFichier);
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show("Erreur sur ChargerTrajet :" + e.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public static string nomRepertoireTrajets()
        {
            return string.Format("{0}trajets\\",Constantes.repertoireDonnees);
        }

        private static string nomRepertoireTrajet(int idTrajet, string tipe)
        {
            return string.Format("{0}\\trajet{1}{2}",
                nomRepertoireTrajets(), ((idTrajet / 10000) * 10000).ToString("000000"), tipe);
        }

        private static string nomFichierTrajet(int idTrajet, string tipe)
        {
            return nomRepertoireTrajet(idTrajet, tipe) + string.Format("\\trajet{0}{1}.xml", idTrajet.ToString("000000"), tipe);
        }

        public static string exportCSV(DataTable table, out string nomfichier)
        {
            string messageErreur = string.Empty;
            StringBuilder texteFichier = new StringBuilder();
            char separateur = ';';
            nomfichier = "non affecté";
            try
            {
                Cursor oldCursor = Cursor.Current;
                Cursor.Current = Cursors.WaitCursor;
                nomfichier = string.Format("{0}{1}.csv", Constantes.repertoireDonnees, table.TableName);
                //en-tête de colonnes
                foreach(DataColumn colonne in table.Columns)
                {
                    texteFichier.Append(colonne.ColumnName + separateur);
                }
                texteFichier.AppendLine();

                //lignes
                foreach (DataRow ligne in table.Rows)
                {
                    foreach (object valeur in ligne.ItemArray)
                    {
                        texteFichier.Append(
                            (valeur.GetType().Equals(typeof(string)) || valeur.GetType().Equals(typeof(char))) ? "\"" + valeur.ToString() + "\"" + separateur : valeur.ToString() + separateur);
                    }
                    texteFichier.AppendLine();
                }
                texteFichier.AppendLine();
                //sauvegarde du fichier final
                StreamWriter file;
                file = new StreamWriter(nomfichier, false);
                file.WriteLine(texteFichier);
                file.Close();
                Cursor.Current = oldCursor;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return messageErreur;
        }

        public static string exportCSV(DataGridView table, string nomtable, out string nomfichier)
        {
            string messageErreur = string.Empty;
            StringBuilder texteFichier = new StringBuilder();
            char separateur = ';';
            nomfichier = "non affecté";
            try
            {
                Cursor oldCursor = Cursor.Current;
                Cursor.Current = Cursors.WaitCursor;
                nomfichier = string.Format("{0}{1}.csv", Constantes.repertoireDonnees, nomtable);
                //en-tête de colonnes
                foreach (DataGridViewColumn colonne in table.Columns)
                {
                    texteFichier.Append(colonne.HeaderText + separateur);
                }
                texteFichier.AppendLine();

                //lignes
                foreach (DataGridViewRow ligne in table.Rows)
                {
                    foreach (DataGridViewCell cellule in ligne.Cells)
                    {
                        object valeur = cellule.Value;
                        if (null==valeur)
                        {
                            texteFichier.Append(separateur);
                        }
                        else
                        {
                            texteFichier.Append("\"" + valeur.ToString() + "\"" + separateur);
                        }
                    }
                    texteFichier.AppendLine();
                }
                texteFichier.AppendLine();
                //sauvegarde du fichier final
                StreamWriter file;
                file = new StreamWriter(nomfichier, false);
                file.WriteLine(texteFichier);
                file.Close();
                Cursor.Current = oldCursor;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return messageErreur;
        }
        /*
                public static string nomRepertoireCases()
                {
                    return string.Format("{0}cases\\", Constantes.repertoireDonnees);
                }

                private static string NomFichierCases(int x, int y, int tour, int phase, string repertoire)
                {
                    string nomfichier = string.Format("{0}{00001}_{00002}_{3}_{4}.cases",
                        repertoire, (x / Constantes.CST_TAILLE_BLOC_CASES) * Constantes.CST_TAILLE_BLOC_CASES,
                                    (y / Constantes.CST_TAILLE_BLOC_CASES) * Constantes.CST_TAILLE_BLOC_CASES, tour, phase);
                    return nomfichier;
                }

                static public bool SauvegarderCases(DataSet donnees, int x, int y, int tour, int phase)
                {
                    Cursor oldCursor = Cursor.Current;
                    try
                    {
                        Cursor.Current = Cursors.WaitCursor;
                        string repertoire = nomRepertoireCases();
                        if (!Directory.Exists(repertoire))
                        {
                            Directory.CreateDirectory(repertoire);
                        }
                        string nomfichier = NomFichierCases(x, y, tour, phase, repertoire);

                        if (File.Exists(nomfichier))
                        {
                            File.Delete(nomfichier);
                        }
                        ZipArchive fichierZip = ZipFile.Open(nomfichier, ZipArchiveMode.Create);
                        ZipArchiveEntry fichier = fichierZip.CreateEntry(nomfichier);
                        StreamWriter ecrivain = new StreamWriter(fichier.Open());
                        donnees.WriteXml(ecrivain);
                        ecrivain.Close();
                        fichierZip.Dispose();
                        //donnees.WriteXml(nomfichier);
                        Cursor.Current = oldCursor;
                    }
                    catch (Exception e)
                    {
                        Cursor.Current = oldCursor;
                        MessageBox.Show("Erreur sur SauvegarderCases :" + e.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    return true;
                }

                static public bool ChargerCases(int x, int y, int tour, int phase, out DataSet donneesSource)
                {
                    Cursor oldCursor = Cursor.Current;
                    donneesSource = new DataSet();
                    try
                    {
                        Cursor.Current = Cursors.WaitCursor;
                        string repertoire = nomRepertoireCases();
                        string nomfichier = NomFichierCases(x, y, tour, phase, repertoire);

                        //on recherche le dernier fichier sauvegardé sur les cases
                        int phaserecherche = phase;
                        while (!File.Exists(nomfichier) && phaserecherche >= 0)
                        {
                            nomfichier = NomFichierCases(x, y, tour, phaserecherche, repertoire);
                            phaserecherche -= Constantes.CST_SAUVEGARDE_ECART_PHASES;
                        }
                        if (phase < 0) 
                        {
                            Cursor.Current = oldCursor;
                            MessageBox.Show(string.Format("Erreur sur ChargerCases : Impossible de trouver un fichiers de cases pour x={0}, y={1}, tour={2}, phase={3}",
                                x, y, tour, phase)
                                , "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false; 
                        }

                        ZipArchive fichierZip = ZipFile.OpenRead(nomfichier);
                        ZipArchiveEntry fichier = fichierZip.Entries[0];
                        donneesSource.ReadXml(fichier.Open());
                        fichierZip.Dispose();
                        Cursor.Current = oldCursor;
                    }
                    catch (Exception e)
                    {
                        Cursor.Current = oldCursor;
                        MessageBox.Show("Erreur sur Dal.ChargerCases :" + e.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    return true;
                }
                 * */
    }
}
