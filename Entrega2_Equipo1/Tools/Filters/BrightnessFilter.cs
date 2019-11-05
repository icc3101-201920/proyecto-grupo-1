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
    public class BrightnessFilter : Tool
    {
        public BrightnessFilter() { }

        // brightness should be in range (-255, 255)
        // From 0 to 255, the image gets brighter
        // From -255 to 0, the image gets darker
		/*
        public Bitmap ApplyFilter(Bitmap image, int brightness)
        {
            
            Bitmap copy = (Bitmap)image.Clone();
            Color color;
            int new_red;
            int new_green;
            int new_blue;
            if (brightness < -255) brightness = -255;
            if (brightness > 255) brightness = 255;

            for (int i = 0; i < copy.Height; i++)
            {
                for (int x = 0; x < copy.Width; x++)
                {
                    color = copy.GetPixel(x, i);
                    new_red = color.R + brightness;
                    if (new_red > 255) new_red = 255;
                    if (new_red < 0) new_red = 0;
                    new_green = color.G + brightness;
                    if (new_green > 255) new_green = 255;
                    if (new_green < 0) new_green = 0;
                    new_blue = color.B + brightness;
                    if (new_blue > 255) new_blue = 255;
                    if (new_blue < 0) new_blue = 0;
                    copy.SetPixel(x, i, Color.FromArgb(new_red, new_green, new_blue));
                }
            }
            return copy;
        }
		*/
		public Bitmap ApplyFilter(Bitmap bitmap, int brightness)
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

						int oldBlue = currentLine[x] + brightness;
						if (oldBlue > 255) oldBlue = 255;
						if (oldBlue < 0) oldBlue = 0;
						int oldGreen = currentLine[x + 1] + brightness;
						if (oldGreen > 255) oldGreen = 255;
						if (oldGreen < 0) oldGreen = 0;
						int oldRed = currentLine[x + 2] + brightness;
						if (oldRed > 255) oldRed = 255;
						if (oldRed < 0) oldRed = 0;

						currentLine[x] = (byte)((oldBlue) );
						currentLine[x + 1] = (byte)((oldGreen ));
						currentLine[x + 2] = (byte)((oldRed ));
					}
				});
				bmap.UnlockBits(bitmapData);


			}
			return bmap;
		}
	}
}
