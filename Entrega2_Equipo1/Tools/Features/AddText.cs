using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entrega2_Equipo1
{
    // TODO: Implementar
    [Serializable]
    public class AddText : Tool
    {
        public AddText() { }


        public Bitmap InsertText(Bitmap bitmap, string text, int xAxis, int yAxis, float fontSize, 
            Color colorName1, string fontStyle, string fontName 
            , Color colorName2)
        {
            Bitmap temp = (Bitmap)bitmap.Clone();
            Graphics gr = Graphics.FromImage(temp);
            FontStyle fStyle = FontStyle.Regular;
            Font font = new Font(fontName, fontSize);
            switch (fontStyle.ToLower())
            {
                case "bold":
                    fStyle = FontStyle.Bold;
                    break;
                case "italic":
                    fStyle = FontStyle.Italic;
                    break;
                case "underline":
                    fStyle = FontStyle.Underline;
                    break;
                case "strikeout":
                    fStyle = FontStyle.Strikeout;
                    break;
            }
            font = new Font(fontName, fontSize, fStyle);

            if (colorName2 == null)
            {
                colorName2 = colorName1;
            }
            int gW = (int)(text.Length * fontSize);
            gW = gW == 0 ? 10 : gW;
            LinearGradientBrush LGBrush = new LinearGradientBrush(new Rectangle(0,0,gW,(int)fontSize),colorName1,colorName2,LinearGradientMode.Vertical);
            gr.DrawString(text, font, LGBrush, xAxis, yAxis);
            return temp;
        }
    }
}
