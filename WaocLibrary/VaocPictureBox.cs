using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace WaocLib
{
    public class VaocPictureBox : PictureBox
    {
        System.Drawing.Drawing2D.SmoothingMode m_smoothingMode;
        System.Drawing.Drawing2D.CompositingQuality m_compositingQuality;
        System.Drawing.Drawing2D.InterpolationMode m_interpolationMode;

        public VaocPictureBox()
        {
            m_smoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
            m_compositingQuality = CompositingQuality.HighSpeed;
            m_interpolationMode = InterpolationMode.NearestNeighbor;
        }

        public SmoothingMode Smoothing
        {
            get { return m_smoothingMode; }
            set { m_smoothingMode = value; }
        }

        public System.Drawing.Drawing2D.CompositingQuality QualiteDeComposition
        {
            get { return m_compositingQuality; }
            set { m_compositingQuality = value; }
        }

        public System.Drawing.Drawing2D.InterpolationMode Interpolation
        {
            get { return m_interpolationMode; }
            set { m_interpolationMode = value; }
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            pe.Graphics.SmoothingMode = m_smoothingMode;
            pe.Graphics.CompositingQuality = m_compositingQuality;
            pe.Graphics.InterpolationMode = m_interpolationMode;

            // this line is needed for .net to draw the contents.
            base.OnPaint(pe);
        }
    }
}
