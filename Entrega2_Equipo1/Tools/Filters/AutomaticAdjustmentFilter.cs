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
    public class AutomaticAdjustmentFilter : Tool
    {
        // We set a better contrast and better brightness
        public Bitmap ApplyFilter(Bitmap image)
        {
            Bitmap copy = (Bitmap)image.Clone();
            SetContrast(20, copy);
            BrightnessFilter filter = new BrightnessFilter();
            Bitmap returningImage = filter.ApplyFilter(copy, 30);
            return returningImage;
        }

		public Bitmap ApplyContrast(Bitmap bitmap , double contrastD)
		{
			Bitmap copy = (Bitmap)bitmap.Clone();
			float contrast = (float)contrastD;
			copy = SetContrast(contrast, copy);
			return copy;
		}
		/*
        public void SetContrast(double contrast, Bitmap bitmap)
        {
            if (contrast < -100) contrast = -100;
            if (contrast > 100) contrast = 100;
            contrast = (100.0 + contrast) / 100.0;
            contrast *= contrast;
            Color color;
            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    color = bitmap.GetPixel(i, j);
                    double pR = color.R / 255.0;
                    pR -= 0.5;
                    pR *= contrast;
                    pR += 0.5;
                    pR *= 255;
                    if (pR < 0) pR = 0;
                    if (pR > 255) pR = 255;

                    double pG = color.G / 255.0;
                    pG -= 0.5;
                    pG *= contrast;
                    pG += 0.5;
                    pG *= 255;
                    if (pG < 0) pG = 0;
                    if (pG > 255) pG = 255;

                    double pB = color.B / 255.0;
                    pB -= 0.5;
                    pB *= contrast;
                    pB += 0.5;
                    pB *= 255;
                    if (pB < 0) pB = 0;
                    if (pB > 255) pB = 255;

                    bitmap.SetPixel(i, j, Color.FromArgb((byte)pR, (byte)pG, (byte)pB));
                }
            }
        }
		*/

		public Bitmap SetContrast(float Value, Bitmap bitmap)
		{

			Value = (100.0f + Value) / 100.0f;
			Value *= Value;
			Bitmap NewBitmap = (Bitmap)bitmap.Clone();
			BitmapData data = NewBitmap.LockBits(
				new Rectangle(0, 0, NewBitmap.Width, NewBitmap.Height),
				ImageLockMode.ReadWrite,
				NewBitmap.PixelFormat);
			int Height = NewBitmap.Height;
			int Width = NewBitmap.Width;

			unsafe
			{
				for (int y = 0; y < Height; ++y)
				{
					byte* row = (byte*)data.Scan0 + (y * data.Stride);
					int columnOffset = 0;
					for (int x = 0; x < Width; ++x)
					{
						byte B = row[columnOffset];
						byte G = row[columnOffset + 1];
						byte R = row[columnOffset + 2];

						float Red = R / 255.0f;
						float Green = G / 255.0f;
						float Blue = B / 255.0f;
						Red = (((Red - 0.5f) * Value) + 0.5f) * 255.0f;
						Green = (((Green - 0.5f) * Value) + 0.5f) * 255.0f;
						Blue = (((Blue - 0.5f) * Value) + 0.5f) * 255.0f;

						int iR = (int)Red;
						iR = iR > 255 ? 255 : iR;
						iR = iR < 0 ? 0 : iR;
						int iG = (int)Green;
						iG = iG > 255 ? 255 : iG;
						iG = iG < 0 ? 0 : iG;
						int iB = (int)Blue;
						iB = iB > 255 ? 255 : iB;
						iB = iB < 0 ? 0 : iB;

						row[columnOffset] = (byte)iB;
						row[columnOffset + 1] = (byte)iG;
						row[columnOffset + 2] = (byte)iR;

						columnOffset += 4;
					}
				}
			}

			NewBitmap.UnlockBits(data);

			return NewBitmap;
		}
	}
}
