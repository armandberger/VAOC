using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.ComponentModel;
using System.Windows.Forms;

namespace WaocLib
{
    public class EffectifEtVictoire
    {
        public int iTour;
        public int iNation;
        public int iEffectif;
        public int iVictoire;
    }

    public class LieuRemarquable
    {
        public int iTourDebut;
        public int iTourFin;
        public int i_X_CASE_HAUT_GAUCHE;
        public int i_Y_CASE_HAUT_GAUCHE;
        public int i_X_CASE_BAS_DROITE;
        public int i_Y_CASE_BAS_DROITE;
    }

    public class MyCustomComparer : IComparer<FileInfo>
    {
        public int Compare(FileInfo x, FileInfo y)
        {
            // split filename
            string[] parts1 = x.Name.Split('_');
            string[] parts2 = y.Name.Split('_');

            // calculate how much leading zeros we need
            int toPad1 = 10 - parts1[2].Length;//carte_general_XX.png
            int toPad2 = 10 - parts2[2].Length;

            // add the zeros, only for sorting
            parts1[2] = parts1[2].Insert(0, new String('0', toPad1));
            parts2[2] = parts2[2].Insert(0, new String('0', toPad2));

            // create the comparable string
            string toCompare1 = string.Join("", parts1);
            string toCompare2 = string.Join("", parts2);

            // compare
            return toCompare1.CompareTo(toCompare2);
        }
    }
    
    public class FabricantDeFilm
    {
        private FileInfo[] m_listeFichiers;
        private AviWriter m_aw;
        private int m_traitement;//traitement principal
        private bool m_bHistoriqueBataille;
        private string m_repertoireVideo;
        private float m_rapport;
        private List<LieuRemarquable> m_lieuxRemarquables;
        private List<EffectifEtVictoire> m_effectifsEtVictoires;
        private int m_largeurMax;
        private int m_hauteurMax;
        private int m_largeur;
        private int m_hauteur;
        private string[] m_texteImages;
        private int m_hauteurBandeau;
        private Font m_police;
        System.ComponentModel.BackgroundWorker m_travailleur;

        public FabricantDeFilm()
        {
        }

        public string Initialisation(string repertoireImages, string repertoireVideo, Font police, string texteMasqueImage, 
                                    string[] texteImages, int largeurOptimale, int HauteurOptimale, 
                                    bool bHistoriqueBataille, List<LieuRemarquable> lieuxRemarquables, List<EffectifEtVictoire> effectifsEtVictoires,
                                    System.ComponentModel.BackgroundWorker worker)
        {
            try
            {
                SizeF tailleTexte;
                Graphics G;
                Bitmap fichierImageSource;
                m_largeur = int.MaxValue;
                m_hauteur = int.MaxValue;
                m_hauteurMax = 0;
                m_largeurMax = 0;
                m_bHistoriqueBataille = bHistoriqueBataille;
                m_repertoireVideo = repertoireVideo;
                m_lieuxRemarquables = lieuxRemarquables;
                m_effectifsEtVictoires = effectifsEtVictoires;
                m_texteImages = texteImages;
                m_hauteurBandeau = 0;
                m_police = police;
                m_travailleur = worker;
                float largeurTexte, hauteurTexte;

                //recherche le nombre d'images et leur taille
                DirectoryInfo dir = new DirectoryInfo(repertoireImages);
                m_listeFichiers = dir.GetFiles(texteMasqueImage, SearchOption.TopDirectoryOnly);
                if (0 == m_listeFichiers.Length) { return "le repertoire source ne contient aucune image " + texteMasqueImage; }

                Array.Sort(m_listeFichiers, new MyCustomComparer());//tri par nom
                foreach (FileInfo fichier in m_listeFichiers)
                {
                    Bitmap fichierImage = (Bitmap)Image.FromFile(fichier.FullName);
                    if (m_largeurMax < fichierImage.Width) { m_largeurMax = fichierImage.Width; }
                    if (m_hauteurMax < fichierImage.Height) { m_hauteurMax = fichierImage.Height; }
                    if (m_largeur > fichierImage.Width) { m_largeur = fichierImage.Width; }
                    if (m_hauteur > fichierImage.Height) { m_hauteur = fichierImage.Height; }
                }

                if (m_largeurMax != m_largeur || m_hauteurMax != m_hauteur)
                {
                    return string.Format("Toutes les images n'ont pas la même taille, celles-ci vont de ({0},{1}) à ({2},{3}). Le traitement ne peut être effectué",
                        m_hauteur, m_largeur, m_hauteurMax, m_largeurMax);
                }

                //calcul de la hauteur du bandeau = 2 fois la hauteur de la police, et de la largeur du bandeau (au cas où cela dépassererait la largeur min)
                if (null != texteImages && texteImages.Length > 0)
                {
                    fichierImageSource = (Bitmap)Image.FromFile(m_listeFichiers[0].FullName);
                    G = Graphics.FromImage(fichierImageSource);
                    largeurTexte = hauteurTexte = 0;
                    foreach(string texte in texteImages)
                    {
                        tailleTexte = G.MeasureString(texte, police);
                        largeurTexte = Math.Max(largeurTexte, tailleTexte.Width);
                        hauteurTexte = Math.Max(hauteurTexte, tailleTexte.Height);
                    }

                    //ajout des chiffres d'effectif à droite et à gauche
                    tailleTexte = G.MeasureString("000.000", police);
                    hauteurTexte = Math.Max(hauteurTexte, tailleTexte.Height);

                    m_hauteurBandeau = (int)(hauteurTexte * 2);
                    m_largeur = Math.Max((int)(largeurTexte * 1), m_largeur) + 2 * (int)(tailleTexte.Width * 2);
                    fichierImageSource.Dispose();
                }

                //calcul de la taille optimale
                if ((float)m_largeur / largeurOptimale > (float)m_hauteur / (HauteurOptimale - m_hauteurBandeau))
                {
                    //on se cale donc sur la largeur (effort le plus grand)
                    m_rapport = (float)largeurOptimale / m_largeur;
                    m_hauteur = (int)(m_hauteur * m_rapport);
                    m_largeur = largeurOptimale;
                }
                else
                {
                    m_rapport = (float)(HauteurOptimale - m_hauteurBandeau) / m_hauteur;
                    m_largeur = (int)(m_largeur * m_rapport);
                    m_hauteur = HauteurOptimale - m_hauteurBandeau;
                }

                m_aw = new AviWriter();
                Bitmap bmp = m_aw.Open(repertoireVideo + "\\" + "video.avi", 1, m_largeur, m_hauteur + m_hauteurBandeau);
                m_traitement = 0;
                return string.Empty;
            }
            catch (AviWriter.AviException e)
            {
                return "AVI Exception in: " + e.ToString();
            }
        }

        public string Terminer()
        {
            try
            {
                m_aw.Close();
                m_travailleur.ReportProgress(100);
                return string.Empty;
            }
            catch (AviWriter.AviException e)
            {
                return "AVI Exception in: " + e.ToString();
            }
        }

        public string Traitement()
        {
            SizeF tailleTexte;
            int largeurCote;
            Graphics G;
            Bitmap fichierImageSource;
            try
            {
                FileInfo fichier = m_listeFichiers[m_traitement];
                fichierImageSource = (Bitmap)Image.FromFile(fichier.FullName);
                Bitmap fichierImage = new Bitmap(m_largeur, m_hauteur + m_hauteurBandeau, fichierImageSource.PixelFormat);
                G = Graphics.FromImage(fichierImage);
                G.PageUnit = GraphicsUnit.Pixel;
                //bandes blanches en bas et sur les côtés
                //bas
                G.FillRectangle(Brushes.White, new Rectangle(0, m_hauteur, m_largeur, m_hauteurBandeau));
                //gauche
                largeurCote = (m_largeur - fichierImageSource.Width)/2;
                G.FillRectangle(Brushes.White, new Rectangle(0, 0, largeurCote, m_hauteur));
                //droite
                G.FillRectangle(Brushes.White, new Rectangle(m_largeur - largeurCote, 0, largeurCote, m_hauteur));

                //bandeau avec texte
                if (null != m_texteImages && m_texteImages.Length > 0)
                {
                    tailleTexte = G.MeasureString(m_texteImages[m_traitement], m_police);
                    G.DrawString(m_texteImages[m_traitement], m_police, Brushes.Black,
                        new Rectangle((m_largeur - (int)tailleTexte.Width) / 2,
                                        m_hauteur + (m_hauteurBandeau - (int)tailleTexte.Height) / 2,
                                        (int)tailleTexte.Width, 
                                        (int)tailleTexte.Height));
                                        //m_largeur - (m_largeur - (int)tailleTexte.Width) / 2, 
                                        //m_hauteurBandeau - (m_hauteurBandeau - (int)tailleTexte.Height) / 2));
                }

                // effectifs et indicateur de victoire par camp
                if (null != m_effectifsEtVictoires && m_effectifsEtVictoires.Count > 0)
                {
                    foreach (EffectifEtVictoire eev in m_effectifsEtVictoires)
                    {
                        if (eev.iTour!=m_traitement)
                        {
                            continue;
                        }
                        string strEffectif = eev.iEffectif.ToString("000,000");
                        tailleTexte = G.MeasureString(strEffectif, m_police);
                        if (0==eev.iNation)
                        {
                            G.DrawString(strEffectif, m_police, Brushes.Blue,
                                new Rectangle((largeurCote - (int)tailleTexte.Width) / 2,
                                                m_hauteur + (m_hauteurBandeau - (int)tailleTexte.Height) / 2,
                                                (int)tailleTexte.Width, 
                                                (int)tailleTexte.Height));
                        }
                        else
                        {
                            G.DrawString(strEffectif, m_police, Brushes.Red,
                                new Rectangle(m_largeur - (largeurCote - (int)tailleTexte.Width) / 2,
                                                m_hauteur + (m_hauteurBandeau - (int)tailleTexte.Height) / 2,
                                                (int)tailleTexte.Width, 
                                                (int)tailleTexte.Height));
                        }
                    }
                }

                //image de base
                G.DrawImage(fichierImageSource, 0, 0, m_largeur, m_hauteur);

                if (m_bHistoriqueBataille)
                {
                    Pen styloExterieur = new Pen(Color.Black, 3);
                    Pen styloInterieur = new Pen(Color.White, 1);
                    //on ajoute les batailles s'il y en a
                    foreach (LieuRemarquable ligneLieu in m_lieuxRemarquables)
                    {
                        if (m_traitement >= ligneLieu.iTourDebut && m_traitement <= ligneLieu.iTourFin)
                        {
                            G.DrawRectangle(styloExterieur,
                                ligneLieu.i_X_CASE_HAUT_GAUCHE * m_rapport,
                                ligneLieu.i_Y_CASE_HAUT_GAUCHE * m_rapport,
                                (ligneLieu.i_X_CASE_BAS_DROITE - ligneLieu.i_X_CASE_HAUT_GAUCHE) * m_rapport,
                                (ligneLieu.i_Y_CASE_BAS_DROITE - ligneLieu.i_Y_CASE_HAUT_GAUCHE) * m_rapport);

                            G.DrawRectangle(styloInterieur,
                                ligneLieu.i_X_CASE_HAUT_GAUCHE * m_rapport,
                                ligneLieu.i_Y_CASE_HAUT_GAUCHE * m_rapport,
                                (ligneLieu.i_X_CASE_BAS_DROITE - ligneLieu.i_X_CASE_HAUT_GAUCHE) * m_rapport,
                                (ligneLieu.i_Y_CASE_BAS_DROITE - ligneLieu.i_Y_CASE_HAUT_GAUCHE) * m_rapport);
                        }
                    }
                }
                //G.DrawImageUnscaled(fichierImageSource, 0, 0);
                fichierImage.Save(m_repertoireVideo + "\\test.png", ImageFormat.Png);
                fichierImage.RotateFlip(RotateFlipType.RotateNoneFlipY);//il faut retourner l'image sinon, elle apparait inversée dans la vidéo
                BitmapData bmpDat = fichierImage.LockBits(
                    new Rectangle(0, 0, m_largeur, m_hauteur + m_hauteurBandeau), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);


                //BitmapData imageCible = new BitmapData();
                //m_imageCarte.LockBits(rect, ImageLockMode.ReadOnly, m_imageCarte.PixelFormat, imageCible);
                //m_imageCarte.UnlockBits(imageCible);
                //Bitmap imageFinale = new Bitmap(imageCible.Width, imageCible.Height, imageCible.Stride, imageCible.PixelFormat, imageCible.Scan0);
                //imageFinale.Save(nomFichierFinal);
                m_aw.AddFrame(bmpDat);
                fichierImage.UnlockBits(bmpDat);
                G.Dispose();
                fichierImage.Dispose();
                fichierImageSource.Dispose();
                m_traitement++;
                m_travailleur.ReportProgress(m_traitement*100/m_listeFichiers.Length);
                if (m_traitement == m_listeFichiers.Length)
                {
                    return "film crée";
                }
                return string.Empty;
            }
            catch (AviWriter.AviException e)
            {
                return "AVI Exception in: " + e.ToString();
            }
        }

        public string CreerFilm(string repertoireImages, string repertoireVideo, Font police, string texteMasqueImage, string[] texteImages
                                ,int largeurOptimale, int HauteurOptimale, bool bHistoriqueBataille, List<LieuRemarquable> lieuxRemarquables)
        {
            try
            {
                SizeF tailleTexte;
                Graphics G;
                Bitmap fichierImageSource;
                int w = int.MaxValue;
                int h = int.MaxValue;
                int hmax = 0;
                int wmax = 0;
                int hauteurBandeau = 0;
                float rapport;

                //recherche le nombre d'images et leur taille
                DirectoryInfo dir = new DirectoryInfo(repertoireImages);
                m_listeFichiers = dir.GetFiles(texteMasqueImage,SearchOption.TopDirectoryOnly);
                if (0 == m_listeFichiers.Length) {return "le repertoire source ne contient aucune image "+texteMasqueImage;}

                Array.Sort(m_listeFichiers, new MyCustomComparer());//tri par nom
                foreach (FileInfo fichier in m_listeFichiers)
                {
                    Bitmap fichierImage = (Bitmap)Image.FromFile(fichier.FullName);
                    if (wmax < fichierImage.Width) { wmax = fichierImage.Width; }
                    if (hmax < fichierImage.Height) { hmax = fichierImage.Height; }
                    if (w > fichierImage.Width) { w = fichierImage.Width; }
                    if (h > fichierImage.Height) { h = fichierImage.Height; }
                }

                if (wmax != w || hmax != h)
                {
                    return string.Format("Toutes les images n'ont pas la même taille, celles-ci vont de ({0},{1}) à ({2},{3}). Le traitement ne peut être effectué", h,w,hmax,wmax);
                }

                //calcul de la hauteur du bandeau = 2 fois la hauteur de la police
                if (null != texteImages && texteImages.Length > 0)
                {
                    fichierImageSource = (Bitmap)Image.FromFile(m_listeFichiers[0].FullName);
                    G = Graphics.FromImage(fichierImageSource);
                    tailleTexte = G.MeasureString("XX", police);
                    hauteurBandeau = (int)(tailleTexte.Height * 1);
                    fichierImageSource.Dispose();
                }

                //calcul de la taille optimale
                if ((float)w / largeurOptimale > (float)h / (HauteurOptimale - hauteurBandeau))
                {
                    //on se cale donc sur la largeur (effort le plus grand)
                    rapport = (float)largeurOptimale / w;
                    h = (int)(h * rapport);
                    w = largeurOptimale;
                }
                else
                {
                    rapport = (float)(HauteurOptimale - hauteurBandeau) / h;
                    w = (int)(w * rapport);
                    h = HauteurOptimale - hauteurBandeau;
                }

                m_aw = new AviWriter();
                Bitmap bmp = m_aw.Open(repertoireVideo + "\\" + "video.avi", 1, w, h + hauteurBandeau);

                m_traitement = 0;
                foreach (FileInfo fichier in m_listeFichiers)
                {
                    fichierImageSource = (Bitmap)Image.FromFile(fichier.FullName);
                    Bitmap fichierImage = new Bitmap(w, h + hauteurBandeau, fichierImageSource.PixelFormat);
                    G = Graphics.FromImage(fichierImage);
                    G.PageUnit = GraphicsUnit.Pixel;
                    //bandeau avec texte
                    if (null != texteImages && texteImages.Length > 0)
                    {
                        G.FillRectangle(Brushes.White, new Rectangle(0, h, w, hauteurBandeau));
                        tailleTexte = G.MeasureString(texteImages[m_traitement], police);
                        G.DrawString(texteImages[m_traitement], police, Brushes.Black, new Rectangle((w - (int)tailleTexte.Width) / 2, h + (hauteurBandeau - (int)tailleTexte.Height) / 2,
                            w - (w - (int)tailleTexte.Width) / 2, hauteurBandeau - (hauteurBandeau - (int)tailleTexte.Height) / 2));
                        m_traitement++;
                    }

                    //image de base
                    G.DrawImage(fichierImageSource, 0, 0, w, h);

                    if (bHistoriqueBataille)
                    {
                        Pen styloExterieur = new Pen(Color.Black, 3);
                        Pen styloInterieur = new Pen(Color.White, 1);
                        //on ajoute les batailles s'il y en a
                        foreach (LieuRemarquable ligneLieu in lieuxRemarquables)
                        {
                            if (m_traitement >= ligneLieu.iTourDebut && m_traitement <= ligneLieu.iTourFin)
                            {
                                G.DrawRectangle(styloExterieur,
                                    ligneLieu.i_X_CASE_HAUT_GAUCHE * rapport,
                                    ligneLieu.i_Y_CASE_HAUT_GAUCHE * rapport,
                                    (ligneLieu.i_X_CASE_BAS_DROITE - ligneLieu.i_X_CASE_HAUT_GAUCHE) * rapport,
                                    (ligneLieu.i_Y_CASE_BAS_DROITE - ligneLieu.i_Y_CASE_HAUT_GAUCHE) * rapport);

                                G.DrawRectangle(styloInterieur,
                                    ligneLieu.i_X_CASE_HAUT_GAUCHE * rapport,
                                    ligneLieu.i_Y_CASE_HAUT_GAUCHE * rapport,
                                    (ligneLieu.i_X_CASE_BAS_DROITE - ligneLieu.i_X_CASE_HAUT_GAUCHE) * rapport,
                                    (ligneLieu.i_Y_CASE_BAS_DROITE - ligneLieu.i_Y_CASE_HAUT_GAUCHE) * rapport);
                            }
                        }
                    }
                    //G.DrawImageUnscaled(fichierImageSource, 0, 0);
                    fichierImage.Save(repertoireVideo+"\\test.png", ImageFormat.Png);
                    fichierImage.RotateFlip(RotateFlipType.RotateNoneFlipY);//il faut retourner l'image sinon, elle apparait inversée dans la vidéo
                    BitmapData bmpDat = fichierImage.LockBits(
                      new Rectangle(0, 0, w, h + hauteurBandeau), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);


                    //BitmapData imageCible = new BitmapData();
                    //m_imageCarte.LockBits(rect, ImageLockMode.ReadOnly, m_imageCarte.PixelFormat, imageCible);
                    //m_imageCarte.UnlockBits(imageCible);
                    //Bitmap imageFinale = new Bitmap(imageCible.Width, imageCible.Height, imageCible.Stride, imageCible.PixelFormat, imageCible.Scan0);
                    //imageFinale.Save(nomFichierFinal);
                    m_aw.AddFrame(bmpDat);
                    fichierImage.UnlockBits(bmpDat);
                    G.Dispose();
                    fichierImage.Dispose();
                    fichierImageSource.Dispose();
                }
                /*
                Color[] palette = new Color[50];
                for (int i = 0; i < 50; i++)
                {
                    palette[i] = Color.FromArgb(i * 4, 255 - i * 4, 50 + i * 2);
                }
                AviWriter aw = new AviWriter();
                Bitmap bmp = aw.Open(repertoireVideo + "\\" + "test.avi", 1, w, h);

                double f = 1.2;
                double centerX = -0.7454333;
                double centerY = -0.1130211;
                double pctAreaNewImage = 0.9;
                double endWidth_times_2 = 0.0001;

                while (f > endWidth_times_2)
                {
                    MandelbrotCalc.CalcMandelBrot(
                      bmp,
                      centerX - f, centerY - f,
                      centerX + f, centerY + f,
                      palette);

                    f = Math.Sqrt(pctAreaNewImage * f * f);
                    aw.AddFrame();
                    Console.Write(".");
                }
                */
                m_aw.Close();
                return string.Empty;
            }
            catch (AviWriter.AviException e)
            {
                return "AVI Exception in: " + e.ToString();
            }
        }
    }
}
