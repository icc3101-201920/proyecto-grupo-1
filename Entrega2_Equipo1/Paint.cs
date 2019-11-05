using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Entrega2_Equipo1
{
	public partial class Paint : Form
	{
		Bitmap actualImage;
		Bitmap showedImage;
		bool down = false;
        private int y = 0;
		private int x = 0;
		private Color mainColor = Color.White;
		private Color secondColor = Color.White;
		Color color;
		private int size = 9;

		public bool Exit = false;

		public Bitmap ActualImage { get => this.actualImage; set => this.actualImage = value; }
		public int X { get => x; set => x = value; }
		public int Y { get => y; set => y = value; }

		public Color MainColor { get => mainColor; set => mainColor = value; }
		public Color SecondColor { get => secondColor; set => secondColor = value; }

		public Paint()
		{
			InitializeComponent();
		}
		private void Paint_Load(object sender, EventArgs e)
		{
			this.showedImage = (Bitmap)actualImage.Clone();
			this.ImagePictureBox.Image = showedImage;
			this.ImagePictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			ColorButton.BackColor = mainColor;
			ColorButton2.BackColor = secondColor;
		}


		private void DoneButton_Click(object sender, EventArgs e)
		{
			actualImage = showedImage;
			Exit = true;
			this.Close();
		}


		private int[] ImagePictureBoxPosition()
		{
			int[] position = new int[] { Cursor.Position.X - this.Left - 9, Cursor.Position.Y - (this.Top + 32) };
			int[] realPosition = ToRealImageSize(position[0], position[1]);
			return realPosition;
		}
		private void MouseClickDown(object sender, MouseEventArgs e)
		{
			down = true;
			if (e.Button == MouseButtons.Left) color = mainColor;
			else color = secondColor;
        }
		private void MouseClickUp(object sender, MouseEventArgs e)
		{
            if (down == true)
            {
                down = false;
            }
        }

        

        private void Brush(object sender, MouseEventArgs e)
		{
            
            if (down)
			{
                int[] realPosition = ImagePictureBoxPosition();
                int x = realPosition[0];
                int y = realPosition[1];
                if ((x + size <= showedImage.Width) && (y + size <= showedImage.Height) && (x > 0) && (y > 0))
                {
                    for (int i = y; i < size + y; i++)
                    {
                        for (int j = x; j < size + x; j++)
                        {
                            showedImage.SetPixel(j, i, color);
                        }
                    }
                    this.ImagePictureBox.Image = showedImage;
                }
            }
		}

		private int[] ToRealImageSize(int x, int y)
		{
			int imageWidth = actualImage.Width;
			int imageHeight = actualImage.Height;
			int pictureBoxWidth = this.ImagePictureBox.Width;
			int pictureBoxHeight = this.ImagePictureBox.Height;
			return new int[] { x * imageWidth / pictureBoxWidth, y * imageHeight / pictureBoxHeight };
		}


		private void ColorButton_Click(object sender, EventArgs e)
		{
			if (colorDialog1.ShowDialog() == DialogResult.OK)
			{
				mainColor = colorDialog1.Color;
				ColorButton.BackColor = mainColor;
			}
		}

		private void NumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			size = Convert.ToInt32(numericUpDown.Value);
		}

		private void ColorButton2_Click(object sender, EventArgs e)
		{
			if (colorDialog1.ShowDialog() == DialogResult.OK)
			{
				secondColor = colorDialog1.Color;
				ColorButton2.BackColor = secondColor;
			}
		}

        private void ImagePictureBox_Click(object sender, EventArgs e)
        {

        }
    }
}