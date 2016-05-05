using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace vaoc
{
    public partial class FormOutils : Form
    {
        private Bitmap m_imageCarte;
        private Bitmap m_imageCarteFond;
        private Donnees donneesImport;


        public FormOutils()
        {
            InitializeComponent();
            donneesImport = new Donnees();
        }

        private void buttonImage_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == this.openFileDialog.ShowDialog(this))
            {
                textBoxFichierImage.Text = openFileDialog.FileName;
            }
        }

        private void buttonTraitement_Click(object sender, EventArgs e)
        {
        }

        private void SauvegardeDecoupeFichier(string nomFichierFinal, Rectangle rect)
        {
            BitmapData imageCible = new BitmapData();
            m_imageCarte.LockBits(rect, ImageLockMode.ReadOnly, m_imageCarte.PixelFormat, imageCible);
            m_imageCarte.UnlockBits(imageCible);
            Bitmap imageFinale = new Bitmap(imageCible.Width, imageCible.Height, imageCible.Stride, imageCible.PixelFormat, imageCible.Scan0);
            imageFinale.Save(nomFichierFinal);
        }

        private void buttonDecoupe_Click(object sender, EventArgs e)
        {
            Cursor oldcurseur = Cursor;
            string nomFichier, extensionFichier;
            int i,j;

            textBoxFichierImage.Text = textBoxFichierImage.Text;
            try
            {
                Cursor = Cursors.WaitCursor;
                m_imageCarte = (Bitmap)Image.FromFile(textBoxFichierImage.Text);
                nomFichier = textBoxFichierImage.Text.Substring(0, textBoxFichierImage.Text.LastIndexOf('.'));
                extensionFichier = textBoxFichierImage.Text.Substring(textBoxFichierImage.Text.LastIndexOf('.'));
                //labelNom.Text = openFileDialog.FileName.Substring(openFileDialog.FileName.LastIndexOf('\\') + 1);

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
                Cursor = oldcurseur;
                MessageBox.Show("Erreur au chargement de l'image : " + ex.Message, "Decoupe de fichiers", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                string nomFichierFinal;
                Rectangle rect;

                int l= Convert.ToInt32(textBoxLargeur.Text);
                int h= Convert.ToInt32(textBoxHauteur.Text);
                for (i = 0; i < l; i++)
                {
                    for (j = 0; j < h; j++)
                    {
                        int x = i * m_imageCarte.Width / l;
                        int y = j * m_imageCarte.Height / h;
                        rect = new Rectangle(x, y, m_imageCarte.Width / l, m_imageCarte.Height / h);
                        nomFichierFinal = string.Format("{0}_{1}{2}", nomFichier, i*h+j, extensionFichier);
                        SauvegardeDecoupeFichier(nomFichierFinal, rect);
                    }
                }
                #region coupure en 4
                //rect = new Rectangle(0, 0, m_imageCarte.Width / 2, m_imageCarte.Height / 2);
                //nomFichierFinal = string.Format("{0}_{1}{2}", nomFichier, 1, extensionFichier);
                //SauvegardeDecoupeFichier(nomFichierFinal, rect);

                //rect = new Rectangle(m_imageCarte.Width / 2, 0, m_imageCarte.Width / 2, m_imageCarte.Height / 2);
                //nomFichierFinal = string.Format("{0}_{1}{2}", nomFichier, 2, extensionFichier);
                //SauvegardeDecoupeFichier(nomFichierFinal, rect);

                //rect = new Rectangle(0, m_imageCarte.Height / 2, m_imageCarte.Width / 2, m_imageCarte.Height / 2);
                //nomFichierFinal = string.Format("{0}_{1}{2}", nomFichier, 3, extensionFichier);
                //SauvegardeDecoupeFichier(nomFichierFinal, rect);

                //rect = new Rectangle(m_imageCarte.Width / 2, m_imageCarte.Height / 2, m_imageCarte.Width / 2, m_imageCarte.Height / 2);
                //nomFichierFinal = string.Format("{0}_{1}{2}", nomFichier, 4, extensionFichier);
                //SauvegardeDecoupeFichier(nomFichierFinal, rect);
                #endregion

                #region coupure en 5
                //rect = new Rectangle(0, 0, m_imageCarte.Width, m_imageCarte.Height / 5);
                //nomFichierFinal = string.Format("{0}_{1}{2}", nomFichier, 1, extensionFichier);
                //SauvegardeDecoupeFichier(nomFichierFinal, rect);

                //rect = new Rectangle(0, m_imageCarte.Height / 5, m_imageCarte.Width, m_imageCarte.Height / 5);
                //nomFichierFinal = string.Format("{0}_{1}{2}", nomFichier, 2, extensionFichier);
                //SauvegardeDecoupeFichier(nomFichierFinal, rect);

                //rect = new Rectangle(0, 2 * m_imageCarte.Height / 5, m_imageCarte.Width, m_imageCarte.Height / 5);
                //nomFichierFinal = string.Format("{0}_{1}{2}", nomFichier, 3, extensionFichier);
                //SauvegardeDecoupeFichier(nomFichierFinal, rect);

                //rect = new Rectangle(0, 3 * m_imageCarte.Height / 5, m_imageCarte.Width, m_imageCarte.Height / 5);
                //nomFichierFinal = string.Format("{0}_{1}{2}", nomFichier, 4, extensionFichier);
                //SauvegardeDecoupeFichier(nomFichierFinal, rect);

                //rect = new Rectangle(0, 4 * m_imageCarte.Height / 5, m_imageCarte.Width, m_imageCarte.Height / 5);
                //nomFichierFinal = string.Format("{0}_{1}{2}", nomFichier, 5, extensionFichier);
                //SauvegardeDecoupeFichier(nomFichierFinal, rect);

                #endregion

                #region coupure en 6
                //rect = new Rectangle(0, 0, m_imageCarte.Width / 3, m_imageCarte.Height / 2);
                //nomFichierFinal = string.Format("{0}_{1}{2}", nomFichier, 1, extensionFichier);
                //SauvegardeDecoupeFichier(nomFichierFinal, rect);

                //rect = new Rectangle(m_imageCarte.Width / 3, 0, m_imageCarte.Width / 3, m_imageCarte.Height / 2);
                //nomFichierFinal = string.Format("{0}_{1}{2}", nomFichier, 2, extensionFichier);
                //SauvegardeDecoupeFichier(nomFichierFinal, rect);

                //rect = new Rectangle(2 * m_imageCarte.Width / 3, 0, m_imageCarte.Width / 3, m_imageCarte.Height / 2);
                //nomFichierFinal = string.Format("{0}_{1}{2}", nomFichier, 3, extensionFichier);
                //SauvegardeDecoupeFichier(nomFichierFinal, rect);

                //rect = new Rectangle(0, m_imageCarte.Height / 2, m_imageCarte.Width / 3, m_imageCarte.Height / 2);
                //nomFichierFinal = string.Format("{0}_{1}{2}", nomFichier, 4, extensionFichier);
                //SauvegardeDecoupeFichier(nomFichierFinal, rect);

                //rect = new Rectangle(m_imageCarte.Width / 3, m_imageCarte.Height / 2, m_imageCarte.Width / 3, m_imageCarte.Height / 2);
                //nomFichierFinal = string.Format("{0}_{1}{2}", nomFichier, 5, extensionFichier);
                //SauvegardeDecoupeFichier(nomFichierFinal, rect);

                //rect = new Rectangle(2 * m_imageCarte.Width / 3, m_imageCarte.Height / 2, m_imageCarte.Width / 3, m_imageCarte.Height / 2);
                //nomFichierFinal = string.Format("{0}_{1}{2}", nomFichier, 6, extensionFichier);
                //SauvegardeDecoupeFichier(nomFichierFinal, rect);
                #endregion

                #region coupure en 3
                //rect = new Rectangle(0, 0, m_imageCarte.Width / 3, m_imageCarte.Height);
                //nomFichierFinal = string.Format("{0}_{1}{2}", nomFichier, 1, extensionFichier);
                //SauvegardeDecoupeFichier(nomFichierFinal, rect);

                //rect = new Rectangle(m_imageCarte.Width / 3, 0, m_imageCarte.Width / 3, m_imageCarte.Height);
                //nomFichierFinal = string.Format("{0}_{1}{2}", nomFichier, 2, extensionFichier);
                //SauvegardeDecoupeFichier(nomFichierFinal, rect);

                //rect = new Rectangle(2 * m_imageCarte.Width / 3, 0, m_imageCarte.Width / 3, m_imageCarte.Height);
                //nomFichierFinal = string.Format("{0}_{1}{2}", nomFichier, 3, extensionFichier);
                //SauvegardeDecoupeFichier(nomFichierFinal, rect);
                #endregion
            }
            catch (Exception ex)
            {
                Cursor = oldcurseur;
                MessageBox.Show("Erreur durant la découpe/sauvegarde de l'image : " + ex.Message, "Decoupe de fichiers", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            MessageBox.Show("Le traitement s'est terminé avec succès.", "Decoupe de fichiers", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Cursor = oldcurseur;
        }

        private void buttonImageFond_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == this.openFileDialog.ShowDialog(this))
            {
                textBoxFichierImageFond.Text = openFileDialog.FileName;
            }
        }

        private void buttonZoom_Click(object sender, EventArgs e)
        {
            Cursor oldcurseur = Cursor;
            string nomFichier, extensionFichier;

            textBoxFichierImage.Text = textBoxFichierImage.Text;
            try
            {
                Cursor = Cursors.WaitCursor;
                m_imageCarte = (Bitmap)Image.FromFile(textBoxFichierImage.Text);
                nomFichier = textBoxFichierImage.Text.Substring(0, textBoxFichierImage.Text.LastIndexOf('.'));
                extensionFichier = textBoxFichierImage.Text.Substring(textBoxFichierImage.Text.LastIndexOf('.'));
            }
            catch (Exception ex)
            {
                Cursor = oldcurseur;
                MessageBox.Show("Erreur au chargement de l'image : " + ex.Message, "Zoom de fichiers", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                m_imageCarteFond = (Bitmap)Image.FromFile(textBoxFichierImageFond.Text);
            }
            catch (Exception ex)
            {
                Cursor = oldcurseur;
                MessageBox.Show("Erreur au chargement de l'image de Fond: " + ex.Message, "Zoom de fichiers", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                string nomFichierFinal;
                Point ptCible= new Point(m_imageCarte.Width/2, m_imageCarte.Height/2);
                Rectangle rect;

                //prend la zone max entourant la cible dans la carte grisée
                rect = new Rectangle(ptCible.X -m_imageCarte.Width / 4, 
                                    ptCible.Y- m_imageCarte.Width / 4, 
                                    m_imageCarte.Width / 2,
                                    m_imageCarte.Height / 2);
                BitmapData imageZoneMax = new BitmapData();
                m_imageCarteFond.LockBits(rect, ImageLockMode.ReadOnly, m_imageCarte.PixelFormat, imageZoneMax);
                m_imageCarteFond.UnlockBits(imageZoneMax);


                //prend la zone autour de la cible dans la carte non grisée
                rect = new Rectangle(ptCible.X -m_imageCarte.Width / 8, 
                                    ptCible.Y- m_imageCarte.Width / 8, 
                                    m_imageCarte.Width / 4,
                                    m_imageCarte.Height / 4);
                BitmapData imageZoneCible = new BitmapData();
                m_imageCarte.LockBits(rect, ImageLockMode.ReadOnly, m_imageCarte.PixelFormat, imageZoneCible);
                m_imageCarte.UnlockBits(imageZoneCible);
                Bitmap imageCible = new Bitmap(imageZoneCible.Width, imageZoneCible.Height, imageZoneCible.Stride, imageZoneCible.PixelFormat, imageZoneCible.Scan0);

                //copy la zone autour de la cible dans la zone max
                Bitmap imageFinale = new Bitmap(imageZoneMax.Width, imageZoneMax.Height, imageZoneMax.Stride, imageZoneMax.PixelFormat, imageZoneMax.Scan0);
                Graphics graph = Graphics.FromImage(imageFinale);
                graph.DrawImageUnscaled(imageCible, m_imageCarte.Width / 8, m_imageCarte.Width / 8);

                //sauvegarde
                nomFichierFinal = string.Format("{0}_Zoom{1}", nomFichier, extensionFichier);
                imageFinale.Save(nomFichierFinal);

            }
            catch (Exception ex)
            {
                Cursor = oldcurseur;
                MessageBox.Show("Erreur durant le zoom/sauvegarde de l'image : " + ex.Message, "Decoupe de fichiers", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            MessageBox.Show("Le traitement s'est terminé avec succès.", "zoom de fichiers", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Cursor = oldcurseur;

        }

        private void buttonRepertoireSource_Click(object sender, EventArgs e)
        {
            if (this.folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.textBoxRepertoireSources.Text = this.folderBrowserDialog.SelectedPath;
            }
        }

        /// <summary>
        /// Recherche de tous les fichiers .php dans le repertoire indiqué, remplacement des noms de tables en minuscules, sauvegarde de l'ancier fichier et écriture du nouveau
        /// </summary>
        private void buttonMinMaj_Click(object sender, EventArgs e)
        {
            string[] tablesSQL = new string[] {
            "tab_utilisateurs", "tab_vaoc_aptitudes", "tab_vaoc_aptitudes_modele", "tab_vaoc_bataille", "tab_vaoc_bataille_pions",
             "tab_vaoc_campagne", "tab_vaoc_jeu",  "tab_vaoc_message", "tab_vaoc_meteo", "tab_vaoc_modele_mouvement",
             "tab_vaoc_modele_pion", "tab_vaoc_nation", "tab_vaoc_noms_carte", "tab_vaoc_objectifs", "tab_vaoc_ordre", "tab_vaoc_parametre",
             "tab_vaoc_partie", "tab_vaoc_pion", "tab_vaoc_role"
            };
            string nomFichierDestination, nomFichierBackup;

            //parcours de tous les fichiers
            string[] fichiersPHP = Directory.GetFiles(this.textBoxRepertoireSources.Text, "*.php", SearchOption.TopDirectoryOnly);
            if (0 == fichiersPHP.Length)
            {
                MessageBox.Show("Le repertoire " + this.textBoxRepertoireSources.Text + " ne contient aucun fichier PHP", "buttonMinMaj_Click", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            foreach (string nomFichierSource in fichiersPHP)
            {
                int positionPoint = nomFichierSource.LastIndexOf(".");

                //recopie de la chaine avant l'extension
                nomFichierDestination = nomFichierSource.Substring(0, positionPoint) + ".new";
                nomFichierBackup = nomFichierSource.Substring(0, positionPoint) + ".bak";

                StreamReader fichier = new StreamReader(nomFichierSource, Encoding.GetEncoding("ISO-8859-1"));
                string contenuPHP = fichier.ReadToEnd();
                fichier.Close();
                //on modifie les lignes des tables pour les mettres en minuscule
                foreach (string ligneTable in tablesSQL)
                {
                    contenuPHP = contenuPHP.Replace(ligneTable.ToUpperInvariant(), ligneTable);
                }
                //ecriture du nouveau fichier
                File.WriteAllText(nomFichierDestination, contenuPHP, Encoding.GetEncoding("ISO-8859-1"));
                //remplacement de l'ancien fichier par le nouveau et backup
                File.Replace(nomFichierDestination, nomFichierSource, nomFichierBackup);
            }
            MessageBox.Show("Traitement terminé avec succès", "buttonMinMaj_Click", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void buttonChoixFichierImport_Click(object sender, EventArgs evenements)
        {
            this.openFileDialog.Filter = "VAOC|*.vaoc|tous|*.*";
            if (DialogResult.OK == this.openFileDialog.ShowDialog(this))
            {
                textBoxChoixFichierImport.Text = openFileDialog.FileName;
            }

            //mise à jour de la liste des tables
            Cursor oldCursor = Cursor.Current;
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                this.listBoxImport.Items.Clear();

                donneesImport.Clear();
                donneesImport.ReadXml(textBoxChoixFichierImport.Text);
                foreach (DataTable table in donneesImport.Tables)
                {
                    this.listBoxImport.Items.Add(table.TableName + " : " + table.Rows.Count.ToString() + " lignes");
                }
                Cursor.Current = oldCursor;
            }
            catch (Exception e)
            {
                Cursor.Current = oldCursor;
                MessageBox.Show("buttonChoixFichierImport_Click, Erreur sur la lecture des tables :" + e.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonImport_Click(object sender, EventArgs evenements)
        {
            if (0 == this.listBoxImport.SelectedItems.Count)
            {
                MessageBox.Show("Il faut selectionner au moins une table pour pouvoir l'importer", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            if (DialogResult.Yes == MessageBox.Show("Etes vous certains de vouloir importer ces tables ? Toutes les données précédentes seront détruites", "Demande de Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                try
                {
                    foreach (string element in this.listBoxImport.SelectedItems)
                    {
                        string nomTable;
                        nomTable = element.Substring(0, element.IndexOf(':')-1);
                        Donnees.m_donnees.Tables[nomTable].Clear();
                        Donnees.m_donnees.Tables[nomTable].Merge(donneesImport.Tables[nomTable],false);                    
                    }
                    MessageBox.Show("Tables importés avec succès", "Fin de l'imporation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception e)
                {
                    MessageBox.Show("buttonImport_Click, Erreur sur l'import des tables :" + e.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void buttonCoupe_Click(object sender, EventArgs e)
        {
            Cursor oldcurseur = Cursor;
            string nomFichier, extensionFichier;
            int i, j;            

            textBoxFichierImage.Text = textBoxFichierImage.Text;
            try
            {
                Cursor = Cursors.WaitCursor;
                m_imageCarte = (Bitmap)Image.FromFile(textBoxFichierImage.Text);
                nomFichier = textBoxFichierImage.Text.Substring(0, textBoxFichierImage.Text.LastIndexOf('.'));
                extensionFichier = textBoxFichierImage.Text.Substring(textBoxFichierImage.Text.LastIndexOf('.'));

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
                Cursor = oldcurseur;
                MessageBox.Show("Erreur au chargement de l'image : " + ex.Message, "Coupe de fichiers", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                string nomFichierFinal;
                Rectangle rect;

                int cote = Convert.ToInt16(textBoxNbCoteCarre.Text);
                for (i = 0; i < cote; i++)
                {
                    for (j = 0; j < cote; j++)
                    {
                        int x = i * m_imageCarte.Width / cote;
                        int y = j * m_imageCarte.Height / cote;
                        int h, l;

                        l = (i == cote - 1) ? m_imageCarte.Width - i * m_imageCarte.Width / cote : m_imageCarte.Width / cote;
                        h = (j == cote - 1) ? m_imageCarte.Height - j * m_imageCarte.Height / cote : m_imageCarte.Height / cote;
                        rect = new Rectangle(x, y, l, h);
                        nomFichierFinal = string.Format("{0}_{1:000}{2}", nomFichier, (i * cote) + j, extensionFichier);
                        SauvegardeDecoupeFichier(nomFichierFinal, rect);
                    }
                }
            }
            catch (Exception ex)
            {
                Cursor = oldcurseur;
                MessageBox.Show("Erreur durant la découpe/sauvegarde de l'image : " + ex.Message, "Coupe de fichiers", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            MessageBox.Show("Le traitement s'est terminé avec succès.", "Coupe de fichiers", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Cursor = oldcurseur;
        }

        private void buttonFusion_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Et pourquoi ne pas faire plutôt un zip multi fichiers ?", "Coupe de fichiers", MessageBoxButtons.OK, MessageBoxIcon.Information);
            /*
            Cursor oldcurseur = Cursor;
            string nomFichier, extensionFichier;
            int i, j;

                Cursor = Cursors.WaitCursor;

            try
            {
                textBoxFichierImage.Text = textBoxFichierImage.Text;
                nomFichier = textBoxFichierImage.Text.Substring(0, textBoxFichierImage.Text.LastIndexOf('.'));
                extensionFichier = textBoxFichierImage.Text.Substring(textBoxFichierImage.Text.LastIndexOf('.'));
    
                //recherche du nombre de fichiers concernés.
                string nomFichierEtoile = nomFichier.Substring(0, nomFichier.LastIndexOf('_'))+ "*";
                string[] listeFichier = Directory.GetFiles(nomFichierEtoile);

                string nomFichierFinal;
                Rectangle rect;

                BitmapData imageCible = new BitmapData();
                m_imageCarte.LockBits(rect, ImageLockMode.ReadOnly, m_imageCarte.PixelFormat, imageCible);
                m_imageCarte.UnlockBits(imageCible);
                Bitmap imageFinale = new Bitmap(imageCible.Width, imageCible.Height, imageCible.Stride, imageCible.PixelFormat, imageCible.Scan0);
                imageFinale.Save(nomFichierFinal);

                //creation du fichier fusion
                m_imageCarte = (Bitmap)Image.FromFile(textBoxFichierImage.Text);

                int cote = (int)Math.Round(Math.Sqrt(listeFichier.Count()));
                for (i = 0; i < cote; i++)
                {
                    for (j = 0; j < cote; j++)
                    {
                        int x = i * m_imageCarte.Width / cote;
                        int y = j * m_imageCarte.Height / cote;
                        int h, l;

                        l = (i == cote - 1) ? m_imageCarte.Width - i * m_imageCarte.Width / cote : m_imageCarte.Width / cote;
                        h = (j == cote - 1) ? m_imageCarte.Height - j * m_imageCarte.Height / cote : m_imageCarte.Height / cote;
                        rect = new Rectangle(x, y, l, h);
                        nomFichierFinal = string.Format("{0}_{1:000}{2}", nomFichier, (i * cote) + j, extensionFichier);
                        SauvegardeDecoupeFichier(nomFichierFinal, rect);
                    }
                }
            }
            catch (Exception ex)
            {
                Cursor = oldcurseur;
                MessageBox.Show("Erreur durant la fusion/sauvegarde de l'image : " + ex.Message, "Fusion de fichiers", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            MessageBox.Show("Le traitement s'est terminé avec succès.", "Coupe de fichiers", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Cursor = oldcurseur;
             * */
        }
    }
}
