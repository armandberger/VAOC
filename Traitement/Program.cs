using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Traitement
{
    class Program
    {
        static void Main(string[] args)
        {
            Bitmap imageSource = (Bitmap)Bitmap.FromFile("C:\\Users\\Public\\Documents\\vaoc\\1813\\1813_zoom_final.bmp");

            BitmapData imageCible = new BitmapData();
            imageSource.LockBits(new Rectangle(100,100,200,100), ImageLockMode.ReadOnly, imageSource.PixelFormat, imageCible);
            imageSource.UnlockBits(imageCible);
            Bitmap imageFinale = new Bitmap(imageCible.Width, imageCible.Height, imageCible.Stride, imageCible.PixelFormat, imageCible.Scan0);
            imageFinale.Save("C:\\Users\\Public\\Documents\\vaoc\\1813\\1813P_zoom_final.bmp", ImageFormat.Bmp);
        }
    }
}
