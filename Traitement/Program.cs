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
                Bitmap imageSource = (Bitmap)Bitmap.FromFile("C:\\Users\\Public\\Documents\\vaoc\\1813\\1813_zoom_final.jpg");

                BitmapData imageCible = new BitmapData();
                Rectangle rect = new Rectangle(0, 1128 * 5, imageSource.Width, imageSource.Height - 1128 * 5);
                imageSource.LockBits(rect, ImageLockMode.ReadOnly, imageSource.PixelFormat, imageCible);
                imageSource.UnlockBits(imageCible);
                Bitmap imageFinale = new Bitmap(imageCible.Width, imageCible.Height, imageCible.Stride, imageCible.PixelFormat, imageCible.Scan0);
                imageFinale.Save("C:\\Users\\Public\\Documents\\vaoc\\1813\\1813P_zoom_final.jpg", ImageFormat.Jpeg);
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
