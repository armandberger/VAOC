using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;

namespace WaocLib
{
    public class AviWriter
    {
        //public static readonly int streamtypeVIDEO = mmioFOURCC('v', 'i', 'd', 's');
        public const UInt32 ICMF_CHOOSE_KEYFRAME = 0x0001;
        public const UInt32 ICMF_CHOOSE_DATARATE = 0x0002;
        public const UInt32 ICMF_CHOOSE_PREVIEW = 0x0004;
        public const int OF_WRITE = 1;
        public const int OF_READWRITE = 2;
        public const int OF_CREATE = 4096;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct AVISTREAMINFOW
        {
            public UInt32 fccType, fccHandler, dwFlags, dwCaps;

            public UInt16 wPriority, wLanguage;

            public UInt32 dwScale, dwRate,
                             dwStart, dwLength, dwInitialFrames, dwSuggestedBufferSize,
                             dwQuality, dwSampleSize, rect_left, rect_top,
                             rect_right, rect_bottom, dwEditCount, dwFormatChangeCount;

            public UInt16 szName0, szName1, szName2, szName3, szName4, szName5,
                             szName6, szName7, szName8, szName9, szName10, szName11,
                             szName12, szName13, szName14, szName15, szName16, szName17,
                             szName18, szName19, szName20, szName21, szName22, szName23,
                             szName24, szName25, szName26, szName27, szName28, szName29,
                             szName30, szName31, szName32, szName33, szName34, szName35,
                             szName36, szName37, szName38, szName39, szName40, szName41,
                             szName42, szName43, szName44, szName45, szName46, szName47,
                             szName48, szName49, szName50, szName51, szName52, szName53,
                             szName54, szName55, szName56, szName57, szName58, szName59,
                             szName60, szName61, szName62, szName63;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public UInt32 left;
            public UInt32 top;
            public UInt32 right;
            public UInt32 bottom;
        }
        
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct AVISTREAMINFO
        {
            public Int32 fccType;
            public Int32 fccHandler;
            public Int32 dwFlags;
            public Int32 dwCaps;
            public Int16 wPriority;
            public Int16 wLanguage;
            public Int32 dwScale;
            public Int32 dwRate;
            public Int32 dwStart;
            public Int32 dwLength;
            public Int32 dwInitialFrames;
            public Int32 dwSuggestedBufferSize;
            public Int32 dwQuality;
            public Int32 dwSampleSize;
            public RECT rcFrame;
            public Int32 dwEditCount;
            public Int32 dwFormatChangeCount;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public String szName;
        }

        // vfw.h
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct AVICOMPRESSOPTIONS
        {
            public UInt32 fccType;
            public UInt32 fccHandler;
            public UInt32 dwKeyFrameEvery;  // only used with AVICOMRPESSF_KEYFRAMES
            public UInt32 dwQuality;
            public UInt32 dwBytesPerSecond; // only used with AVICOMPRESSF_DATARATE
            public UInt32 dwFlags;
            public IntPtr lpFormat;
            public UInt32 cbFormat;
            public IntPtr lpParms;
            public UInt32 cbParms;
            public UInt32 dwInterleaveEvery;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct BITMAPINFOHEADER
        {
            public UInt32 biSize;
            public Int32 biWidth;
            public Int32 biHeight;
            public Int16 biPlanes;
            public Int16 biBitCount;
            public UInt32 biCompression;
            public UInt32 biSizeImage;
            public Int32 biXPelsPerMeter;
            public Int32 biYPelsPerMeter;
            public UInt32 biClrUsed;
            public UInt32 biClrImportant;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class AVICOMPRESSOPTIONS_CLASS
        {
            public UInt32 fccType;
            public UInt32 fccHandler;
            public UInt32 dwKeyFrameEvery;
            public UInt32 dwQuality;
            public UInt32 dwBytesPerSecond;
            public UInt32 dwFlags;
            public IntPtr lpFormat;
            public UInt32 cbFormat;
            public IntPtr lpParms;
            public UInt32 cbParms;
            public UInt32 dwInterleaveEvery;
        }

        private uint mmioFOURCC(char ch0, char ch1, char ch2, char ch3)
        {
            return (uint)((Int32)(byte)(ch0) | ((byte)(ch1) << 8) |
                ((byte)(ch2) << 16) | ((byte)(ch3) << 24));
        }
        
        public class AviException : ApplicationException
        {
            public AviException(string s) : base(s) { }
            public AviException(string s, Int32 hr)
                : base(s)
            {

                if (hr == AVIERR_BADPARAM)
                {
                    err_msg = "AVIERR_BADPARAM";
                }
                else
                {
                    err_msg = "unknown";
                }
            }

            public string ErrMsg()
            {
                return err_msg;
            }
            private const Int32 AVIERR_BADPARAM = -2147205018;
            private string err_msg;
        }

        public Bitmap Open(string fileName, int frameRate, int width, int height)
        {
            frameRate_ = frameRate;
            width_ = width;
            height_ = height;
            bmp_ = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            BitmapData bmpDat = bmp_.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            stride_ = bmpDat.Stride;
            bmp_.UnlockBits(bmpDat);
            AVIFileInit();
            //int hr = AVIFileOpenW(ref pfile_, fileName, 4097 /* OF_WRITE | OF_CREATE (winbase.h) */, 0);
            int hr = AVIFileOpen(out pfile_, fileName, OF_WRITE | OF_CREATE, IntPtr.Zero);
            if (hr != 0)
            {
                throw new AviException("error for AVIFileOpenW", hr);
            }

            CreateStream();
            SetOptions();

            return bmp_;
        }

        public void AddFrame()
        {
            BitmapData bmpDat = bmp_.LockBits(
              new Rectangle(0, 0, (int)width_, (int)height_), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

            int hr = AVIStreamWrite(psCompressed_, count_, 1,
               bmpDat.Scan0, // pointer to data
               (Int32)(stride_ * height_),
               0, // 16 = AVIIF_KEYFRAMe
               0,
               0);

            if (hr != 0)
            {
                throw new AviException("AVIStreamWrite");
            }

            bmp_.UnlockBits(bmpDat);

            count_++;
        }

        public void AddFrame(BitmapData bmpDat)
        {
            int hr = AVIStreamWrite(psCompressed_, count_, 1,
               bmpDat.Scan0, // pointer to data
               (Int32)(stride_ * height_),
               0, // 16 = AVIIF_KEYFRAMe
               0,
               0);

            if (hr != 0)
            {
                throw new AviException("AVIStreamWrite");
            }

            count_++;
        }

        public void Close()
        {
            AVIStreamRelease(ps_);
            AVIStreamRelease(psCompressed_);

            AVIFileRelease(pfile_);
            AVIFileExit();
        }

        private void CreateStream()
        {
            AVISTREAMINFO strhdr = new AVISTREAMINFO();
            strhdr.fccType = mmioStringToFOURCC("vids", 0); //fccType_;
            strhdr.fccHandler = mmioStringToFOURCC("CVID", 0); //fccHandler_;
            strhdr.dwFlags = 0;
            strhdr.dwCaps = 0;
            strhdr.wPriority = 0;
            strhdr.wLanguage = 0;
            strhdr.dwScale = 1;
            strhdr.dwRate = frameRate_; // Frames per Second
            strhdr.dwStart = 0;
            strhdr.dwLength = 0;
            strhdr.dwInitialFrames = 0;
            strhdr.dwSuggestedBufferSize = height_ * stride_;
            strhdr.dwQuality = -1;// 0xffffffff; //-1;         // Use default
            strhdr.dwSampleSize = 0;
            strhdr.rcFrame.top = 0; //.rect_top = 0;
            strhdr.rcFrame.left = 0;// rect_left = 0;
            strhdr.rcFrame.bottom = (uint)height_;// rect_bottom = height_;
            strhdr.rcFrame.right = (uint)width_;// rect_right = width_;
            strhdr.dwEditCount = 0;
            strhdr.dwFormatChangeCount = 0;
            strhdr.szName = "";
            //strhdr.szName0 = 0;
            //strhdr.szName1 = 0;

            int hr = AVIFileCreateStream(pfile_, out ps_, ref strhdr);

            if (hr != 0)
            {
                throw new AviException("AVIFileCreateStream");
            }
        }

        unsafe private void SetOptions()
        {
    //public static readonly int DIVX = FOURCC.mmioFOURCC('d', 'i', 'v', 'x');
    //public static readonly int MP42 = FOURCC.mmioFOURCC('M', 'P', '4', '2');
    //public static readonly int streamtypeVIDEO = mmioFOURCC('v', 'i', 'd', 's');
    //public static readonly int streamtypeAUDIO = mmioFOURCC('a', 'u', 'd', 's');
    //public static readonly int streamtypeMIDI = mmioFOURCC('m', 'i', 'd', 's');
    //public static readonly int streamtypeTEXT = mmioFOURCC('t', 'x', 't', 's');
    //public static readonly int ICTYPE_VIDEO = mmioFOURCC('v', 'i', 'd', 'c');
    //public static readonly int ICTYPE_AUDIO = mmioFOURCC('a', 'u', 'd', 'c');
    //public static readonly int ICM_FRAMERATE = mmioFOURCC('F', 'r', 'm', 'R');
    //public static readonly int ICM_KEYFRAMERATE = mmioFOURCC('K', 'e', 'y', 'R');
            //int ICM_KEYFRAMERATE = mmioFOURCC('K', 'e', 'y', 'R');
            AVICOMPRESSOPTIONS opts = new AVICOMPRESSOPTIONS();
            opts.fccType = 0; //fccType_;
            opts.fccHandler = mmioFOURCC('D', 'I', 'B', ' '); // Uncompressed, note: si on met 0, affiche une boite de dialogue des choix            
            opts.dwKeyFrameEvery = 0;
            opts.dwQuality = 0;  // 0 .. 10000
            opts.dwFlags = 4;  // AVICOMRPESSF_KEYFRAMES = 4
            opts.dwBytesPerSecond = 0;
            opts.lpFormat = IntPtr.Zero;//(IntPtr)0; //new IntPtr(0);
            opts.cbFormat = 0;
            opts.lpParms = IntPtr.Zero;//(IntPtr)0; //new IntPtr(0);
            opts.cbParms = 0;
            opts.dwInterleaveEvery = 0;

            AVICOMPRESSOPTIONS* p = &opts;
            AVICOMPRESSOPTIONS** pp = &p;

            IntPtr x = ps_;
            IntPtr* ptr_ps = &x;

            //AVISaveOptions(0, 0, 1, ptr_ps, pp); -> pour afficher le choix de codec ou de compression

            // TODO: AVISaveOptionsFree(...)

            int hr = AVIMakeCompressedStream(out psCompressed_, ps_, ref opts, 0);
            if (hr != 0)
            {
                throw new AviException("AVIMakeCompressedStream");
            }

            BITMAPINFOHEADER bi = new BITMAPINFOHEADER();
            bi.biSize = 40;
            bi.biWidth = (Int32)width_;
            bi.biHeight = (Int32)height_;
            bi.biPlanes = 1;
            bi.biBitCount = 24;
            bi.biCompression = 0;  // 0 = BI_RGB
            bi.biSizeImage = (uint)(stride_ * height_);
            bi.biXPelsPerMeter = 0;
            bi.biYPelsPerMeter = 0;
            bi.biClrUsed = 0;
            bi.biClrImportant = 0;

            hr = AVIStreamSetFormat(psCompressed_, 0, ref bi, 40);
            if (hr != 0)
            {
                throw new AviException("AVIStreamSetFormat", hr);
            }
        }

        [DllImport("winmm.dll", CharSet = CharSet.Auto)]
        public static extern int mmioStringToFOURCC([MarshalAs(UnmanagedType.LPWStr)] String sz, int uFlags);

        [DllImport("avifil32.dll")]
        private static extern void AVIFileInit();

        [DllImport("avifil32.dll")]
        private static extern int AVIFileOpenW(ref int ptr_pfile, [MarshalAs(UnmanagedType.LPWStr)]string fileName, int flags, int dummy);

        [DllImport("avifil32.dll", PreserveSig = true, CharSet = CharSet.Auto)]
        public static extern int AVIFileOpen(out IntPtr ppfile, String szFile, int uMode, IntPtr pclsidHandler);

        [DllImport("avifil32.dll", CharSet = CharSet.Auto)]
        public static extern int AVIFileCreateStream(IntPtr pfile, out IntPtr ppavi, ref AVISTREAMINFO ptr_streaminfo);

        //[DllImport("avifil32.dll")]
        //private static extern int AVIFileCreateStream(
        //  int ptr_pfile, out IntPtr ptr_ptr_avi, ref AVISTREAMINFOW ptr_streaminfo);

        [DllImport("avifil32.dll")]
        private static extern int AVIMakeCompressedStream(
          out IntPtr ppsCompressed, IntPtr aviStream, ref AVICOMPRESSOPTIONS ao, int dummy);

        [DllImport("avifil32.dll")]
        private static extern int AVIStreamSetFormat(
          IntPtr aviStream, Int32 lPos, ref BITMAPINFOHEADER lpFormat, Int32 cbFormat);

        [DllImport("avifil32.dll")]
        unsafe private static extern int AVISaveOptions(
          int hwnd, UInt32 flags, int nStreams, IntPtr* ptr_ptr_avi, AVICOMPRESSOPTIONS** ao);

        [DllImport("avifil32.dll")]
        private static extern int AVIStreamWrite(
          IntPtr aviStream, Int32 lStart, Int32 lSamples, IntPtr lpBuffer,
          Int32 cbBuffer, Int32 dwFlags, Int32 dummy1, Int32 dummy2);

        [DllImport("avifil32.dll")]
        private static extern int AVIStreamRelease(IntPtr aviStream);

        [DllImport("avifil32.dll")]
        private static extern int AVIFileRelease(IntPtr pfile);

        [DllImport("avifil32.dll")]
        private static extern void AVIFileExit();

        //private int pfile_ = 0;
        private IntPtr pfile_ = IntPtr.Zero;
        private IntPtr ps_ = new IntPtr(0);
        private IntPtr psCompressed_ = new IntPtr(0);
        private int frameRate_ = 0;
        private int count_ = 0;
        private int width_ = 0;
        private int stride_ = 0;
        private int height_ = 0;
        //private UInt32 fccType_ = 1935960438;  // vids
        //private UInt32 fccHandler_ = 808810089;// IV50
        //1145656899;  // CVID
        private Bitmap bmp_;
    }
}
