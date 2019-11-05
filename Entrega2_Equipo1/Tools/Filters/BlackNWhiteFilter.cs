using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace Entrega2_Equipo1
{
    [Serializable]
    public class BlackNWhiteFilter : Tool
    {
        public BlackNWhiteFilter() { }
		/*
        public Bitmap ApplyFilter(Bitmap image)
        {
            Bitmap copy = (Bitmap)image.Clone();
            int aux;
            Color color;

            for (int i = 0; i < copy.Height; i++)
            {
                for (int x = 0; x < copy.Width; x++)
                {
                    color = copy.GetPixel(x, i);
                    aux = (int)(0.59 * color.G + 0.11 * color.B + 0.29 * color.R);
                    copy.SetPixel(x, i, Color.FromArgb(aux, aux, aux));
                }
            }
            return copy;
        }
		*/
		public Bitmap ApplyFilter(Bitmap bmap)
		{
			Bitmap b = (Bitmap)bmap.Clone();
			BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height),
			  ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
			int stride = bmData.Stride;
			System.IntPtr Scan0 = bmData.Scan0;

			unsafe
			{
				byte* p = (byte*)(void*)Scan0;

				int nOffset = stride - b.Width * 3;

				byte red, green, blue;

				for (int y = 0; y < b.Height; ++y)
				{
					for (int x = 0; x < b.Width; ++x)
					{
						blue = p[0];
						green = p[1];
						red = p[2];

						p[0] = p[1] = p[2] = (byte)(.299 * red
							+ .587 * green
							+ .114 * blue);

						p += 3;
					}
					p += nOffset;
				}
			}


			// unlock the bits when done or when 
			// an exception has been thrown.
			b.UnlockBits(bmData);
			
			return b;
		}
	}
}
