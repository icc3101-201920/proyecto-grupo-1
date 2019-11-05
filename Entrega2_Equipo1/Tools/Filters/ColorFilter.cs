using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace Entrega2_Equipo1
{
    [Serializable]
    public class ColorFilter : Tool
    {
        public Bitmap ApplyFilter(Bitmap image, EColorFilterTypes type)
        {
            Bitmap copy = (Bitmap)image.Clone();
            Color color;
            int new_red;
            int new_green;
            int new_blue;
            for (int i = 0; i < copy.Height; i++)
            {
                for (int x = 0; x < copy.Width; x++)
                {
                    color = copy.GetPixel(x, i);
                    if (type == EColorFilterTypes.Red)
                    {
                        new_red = color.R;
                        new_green = 0;
                        new_blue = 0;
                        copy.SetPixel(x, i, Color.FromArgb(new_red, new_green, new_blue));
                    }
                    else if (type == EColorFilterTypes.Green)
                    {
                        new_red = 0;
                        new_green = color.G;
                        new_blue = 0;
                        copy.SetPixel(x, i, Color.FromArgb(new_red, new_green, new_blue));
                    }
                    else if (type == EColorFilterTypes.Blue)
                    {
                        new_red = 0;
                        new_green = 0;
                        new_blue = color.B;
                        copy.SetPixel(x, i, Color.FromArgb(new_red, new_green, new_blue));
                    }
                    else if (type == EColorFilterTypes.Yellow)
                    {
                        new_red = color.R;
                        new_green = color.G;
                        new_blue = 0;
                        copy.SetPixel(x, i, Color.FromArgb(new_red, new_green, new_blue));
                    }
                }
            }
            return copy;
        }
		/*
        public Bitmap ApplyFilter(Bitmap image, Color usrColor)
        {
            Bitmap copy = (Bitmap)image.Clone();
            Color imgColor;
            int new_red;
            int new_green;
            int new_blue;

            for (int i = 0; i < copy.Height; i++)
            {
                for (int x = 0; x < copy.Width; x++)
                {
                    imgColor = copy.GetPixel(x, i);
                    new_red = (usrColor.R * imgColor.R) / 255;
                    new_green = (usrColor.G * imgColor.G) / 255;
                    new_blue = (usrColor.B * imgColor.B) / 255;
                    copy.SetPixel(x, i, Color.FromArgb(new_red, new_green, new_blue));
                }
            }
            return copy;
        }
		*/

		public Bitmap ApplyFilter(Bitmap bitmap, Color usrColor)
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

						currentLine[x] = (byte)((oldBlue * usrColor.B) / 255);
						currentLine[x + 1] = (byte)((oldGreen * usrColor.G)/ 255);
						currentLine[x + 2] = (byte)((oldRed * usrColor.R)/ 255);
					}
				});
				bmap.UnlockBits(bitmapData);


			}
			return bmap;
		}
    }
}
