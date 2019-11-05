using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Entrega2_Equipo1
{
    [Serializable]
    public class SepiaFilter : Tool
    {
        
        public SepiaFilter() { }

        // Viejo metodo applyfilter
		/*
        public Bitmap ApplyFilter(Bitmap image)
        {
            Bitmap copy = (Bitmap)image.Clone();
            int new_red;
            int new_blue;
            int new_green;
            Color pixelColor;

            for (int i = 0; i < copy.Height; i++)
            {
                for (int x = 0; x < copy.Width; x++)
                {
                    
                    pixelColor = copy.GetPixel(x, i);
                    new_red = (int)Math.Round(.393 * pixelColor.R + .769 * pixelColor.G + .189 * pixelColor.B);
                    new_green = (int)Math.Round(.349 * pixelColor.R + .686 * pixelColor.G + .168 * pixelColor.B);
                    new_blue = (int)Math.Round(.272 * pixelColor.R + .534 * pixelColor.G + .131 * pixelColor.B);

                    if (new_red > 255)
                    {
                        new_red = 255;
                    }

                    if (new_green > 255)
                    {
                        new_green = 255;
                    }

                    if (new_blue > 255)
                    {
                        new_blue = 255;
                    }

                    copy.SetPixel(x, i, Color.FromArgb(new_red, new_green, new_blue));
                }
            }
            return copy;
        }
		*/

        // Nuevo metodo apply filter
		public Bitmap ApplyFilter(Bitmap bitmap)
		{
			Bitmap bmap = (Bitmap)bitmap.Clone();


			unsafe
			{
				BitmapData bitmapData = bmap.LockBits(new Rectangle(0, 0, bmap.Width, bmap.Height), ImageLockMode.ReadWrite, bmap.PixelFormat);

				int bytesPerPixel = Bitmap.GetPixelFormatSize(bmap.PixelFormat) / 8;
				int heightInPixels = bitmapData.Height;
				int widthInBytes = bitmapData.Width * bytesPerPixel;


				byte* PtrFirstPixel = (byte*)bitmapData.Scan0;

				Parallel.For(0, heightInPixels, y =>
				{
					byte* currentLine = PtrFirstPixel + (y * bitmapData.Stride);
					for (int x = 0; x < widthInBytes; x = x + bytesPerPixel)
					{

						int oldBlue = currentLine[x];

						int oldGreen = currentLine[x + 1];
						
						int oldRed = currentLine[x + 2];

						currentLine[x] = (byte)Math.Min((.272 * oldRed) + (.534 * oldGreen) + (.131 * oldBlue), 255.0); ;
						currentLine[x + 1] = (byte)Math.Min((.349 * oldRed) + (.686 * oldGreen) + (.168 * oldBlue), 255.0);
						currentLine[x + 2] = (byte)Math.Min((.393 * oldRed) + (.769 * oldGreen) + (.189 * oldBlue), 255.0);
					}
				});
				bmap.UnlockBits(bitmapData);
                GC.Collect();

			}
			return bmap;
		}
	}
}
