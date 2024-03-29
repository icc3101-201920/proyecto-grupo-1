﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace Entrega2_Equipo1
{
    [Serializable]
    public class InvertFilter : Tool
    {
        public InvertFilter() { }
		/*
        public Bitmap ApplyFilterUS(Bitmap image)
        {
            Bitmap copy = (Bitmap)image.Clone();
            Color color;
            for (int i = 0; i < copy.Height; i++)
            {
                for (int x = 0; x < copy.Width; x++)
                {
                    color = copy.GetPixel(x, i);
                    copy.SetPixel(x, i, Color.FromArgb(255-color.R, 255-color.G, 255-color.B));
                }
            }
            return copy;
        }
		*/
		public Bitmap ApplyFilter(Bitmap bmap)
		{
			Bitmap b = (Bitmap)bmap.Clone();
			// GDI+ still lies to us - the return format is BGR, NOT RGB. 
			BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height),
				ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
			int stride = bmData.Stride;
			System.IntPtr Scan0 = bmData.Scan0;
			unsafe
			{
				byte* p = (byte*)(void*)Scan0;
				int nOffset = stride - b.Width * 3;
				int nWidth = b.Width * 3;
				for (int y = 0; y < b.Height; ++y)
				{
					for (int x = 0; x < nWidth; ++x)
					{
						p[0] = (byte)(255 - p[0]);
						++p;
					}
					p += nOffset;
				}
			}

			b.UnlockBits(bmData);

			return b;
		}
    }
}
