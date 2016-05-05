using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
//using WaocLib;

namespace WaocLib
{
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
        public FabricantDeFilm()
        {
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
                int iTexte;
                float rapport;

                //recherche le nombre d'images et leur taille
                DirectoryInfo dir = new DirectoryInfo(repertoireImages);
                FileInfo[] listeFichiers = dir.GetFiles(texteMasqueImage,SearchOption.TopDirectoryOnly);
                if (0 == listeFichiers.Length) {return "le repertoire source ne contient aucune image "+texteMasqueImage;}

                Array.Sort(listeFichiers, new MyCustomComparer());//tri par nom
                foreach (FileInfo fichier in listeFichiers)
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
                    fichierImageSource = (Bitmap)Image.FromFile(listeFichiers[0].FullName);
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

                AviWriter aw = new AviWriter();
                Bitmap bmp = aw.Open(repertoireVideo + "\\" + "video.avi", 1, w, h + hauteurBandeau);

                iTexte = 0;
                foreach (FileInfo fichier in listeFichiers)
                {
                    fichierImageSource = (Bitmap)Image.FromFile(fichier.FullName);
                    Bitmap fichierImage = new Bitmap(w, h + hauteurBandeau, fichierImageSource.PixelFormat);
                    G = Graphics.FromImage(fichierImage);
                    G.PageUnit = GraphicsUnit.Pixel;
                    //bandeau avec texte
                    if (null != texteImages && texteImages.Length > 0)
                    {
                        G.FillRectangle(Brushes.White, new Rectangle(0, h, w, hauteurBandeau));
                        tailleTexte = G.MeasureString(texteImages[iTexte], police);
                        G.DrawString(texteImages[iTexte], police, Brushes.Black, new Rectangle((w - (int)tailleTexte.Width) / 2, h + (hauteurBandeau - (int)tailleTexte.Height) / 2,
                            w - (w - (int)tailleTexte.Width) / 2, hauteurBandeau - (hauteurBandeau - (int)tailleTexte.Height) / 2));
                        iTexte++;
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
                            if (iTexte >= ligneLieu.iTourDebut && iTexte <= ligneLieu.iTourFin)
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
                    aw.AddFrame(bmpDat);
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
                aw.Close();
                return string.Empty;
            }
            catch (AviWriter.AviException e)
            {
                return "AVI Exception in: " + e.ToString();
            }
        }

    }
}
