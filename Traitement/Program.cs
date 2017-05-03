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
                    Console.WriteLine("Exception :" + ex.Message + " : " +  ex.InnerException.Message);
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
