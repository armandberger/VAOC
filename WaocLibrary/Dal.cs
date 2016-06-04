using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Diagnostics;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Runtime.InteropServices;

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
        
        static public bool SauvegarderPartie(string nomfichier, int tour, int phase, DataSet donnees)
        {
            string nomfichierTourPhase;

            Cursor oldCursor = Cursor.Current;
            try
            {
                //recopie de la chaine avant l'extension
                int positionPoint = nomfichier.LastIndexOf(".");
                nomfichierTourPhase = nomfichier.Substring(0, positionPoint);

                //ajout du tour et de la phase
                //amélioration pour que la toute dernière sauvegarde mette directement le bon nom de fichier
                int tourfichier = -1;//indication du tour courant dans le nom du fichier
                int i = nomfichierTourPhase.Length - 1;
                while (char.IsDigit(nomfichierTourPhase[i])) i--;
                //string test = nomfichierTourPhase.Substring(i + 1, nomfichierTourPhase.Length - i - 1);
                if (i != nomfichierTourPhase.Length - 1) tourfichier = Convert.ToInt32(nomfichierTourPhase.Substring(i + 1, nomfichierTourPhase.Length - i - 1));
                if (tourfichier > 0 && tour > tourfichier && 0 == phase)
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

                return SauvegarderPartie(nomfichierTourPhase, donnees);
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
                donnees.WriteXml(nomfichier);
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
                foreach (DataTable table in donneesSource.Tables)
                    table.BeginLoadData();//accelère le chargement en retirant les constructions d'index, etc.
                donneesSource.ReadXml(nomfichier);
                foreach (DataTable table in donneesSource.Tables)
                    Debug.WriteLine(table.TableName + " : "+table.Rows.Count.ToString());
                //DataTable tableTest = donneesSource.Tables[1];
                //tableTest.EndLoadData();
                bool bException = false;
                Exception derniereErreur = new Exception();//le new juste pour eviter le warning de non affectation
                foreach (DataTable table in donneesSource.Tables)
                {
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

                    Debug.WriteLine("EndLoadData:" + table.TableName + " : " + table.Rows.Count.ToString());
                    try
                    {
                        table.EndLoadData();
                        Debug.WriteLine("Exception sur EndLoadData:" + table.TableName + " : " + table.Rows.Count.ToString());
                    }
                    catch (Exception e)
                    {
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

        private static string nomRepertoireTrajet(int idTrajet, string tipe)
        {
            return string.Format("{0}trajet{1}{2}",
                Constantes.repertoireDonnees, ((idTrajet / 10000) * 10000).ToString("000000"), tipe);
        }

        private static string nomFichierTrajet(int idTrajet, string tipe)
        {
            return nomRepertoireTrajet(idTrajet, tipe) + string.Format("\\trajet{0}{1}.xml", idTrajet.ToString("000000"), tipe);
        }

    }
}
