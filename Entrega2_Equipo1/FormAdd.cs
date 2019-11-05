using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Entrega2_Equipo1
{
	public partial class FormAdd : Form
	{
		Bitmap actualImage;
		Bitmap showedImage;
		private int y = 0;
		private int x = 0;
		private string text = "";
		private string fontStyle = "bold";
		private string fontName = "Times New Roman";
		private float fontSize = 10.0F;
		private Color mainColor = Color.Black;
		private Color secondaryColor = Color.Empty;
		public bool Exit = false;

		public Bitmap ActualImage { get => this.actualImage; set => this.actualImage = value; }
		public int X { get => x; set => x = value; }
		public int Y { get => y; set => y = value; }
		public override string Text { get => text; set => text = value; }
		public string FontName { get => fontName; set => fontName = value; }
		public string FontStyle { get => fontStyle; set => fontStyle = value; }
		public Color MainColor { get => mainColor; set => mainColor = value; }
		public Color SecondaryColor { get => secondaryColor; set => secondaryColor = value; }

		public float FontSize { get => fontSize; set => fontSize = value; }

		public FormAdd()
		{
			InitializeComponent();
		}

		private void FormAdd_Load(object sender, EventArgs e)
		{
			this.showedImage = (Bitmap)actualImage.Clone();
			this.ImagePictureBox.Image = showedImage;
			this.ImagePictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			foreach (FontFamily font in System.Drawing.FontFamily.Families)
			{
				comboFontName.Items.Add(font.Name);
			}
			comboFontName.Text = "Times New Roman";
			comboBoxFontStyle.Items.Add("bold"); comboBoxFontStyle.Items.Add("italic"); comboBoxFontStyle.Items.Add("underline"); comboBoxFontStyle.Items.Add("strikeout");
			comboBoxFontStyle.Text = "bold";
			for (int i = 10; i < 51; i++)
			{
				comboBox3.Items.Add((double)i);
			}
			comboBox3.Text = "10";
		}


		private void ComboFontName_SelectedIndexChanged(object sender, EventArgs e)
		{
			FontName = (string)comboFontName.SelectedItem;
		}

		private void DoneButton_Click(object sender, EventArgs e)
		{
			Exit = true;
			this.Close();
		}

		private void ImagePictureBox_MouseClick(object sender, MouseEventArgs e)
		{
			int[] position = new int[] { Cursor.Position.X - this.Left - 9, Cursor.Position.Y - (this.Top + 32) };
			int[] realPosition = ToRealImageSize(position[0], position[1]);
			x = realPosition[0];
			y = realPosition[1];
		}


		private int[] ToRealImageSize(int x, int y)
		{
			int imageWidth = actualImage.Width;
			int imageHeight = actualImage.Height;
			int pictureBoxWidth = this.ImagePictureBox.Width;
			int pictureBoxHeight = this.ImagePictureBox.Height;
			return new int[] { x * imageWidth / pictureBoxWidth, y * imageHeight / pictureBoxHeight };
		}

		private void TextBox1_TextChanged(object sender, EventArgs e)
		{
			text = textBox1.Text;
		}

		private void ComboBoxFontStyle_SelectedIndexChanged(object sender, EventArgs e)
		{
			fontStyle = (string)comboBoxFontStyle.SelectedItem;
		}

		private void Button1_Click(object sender, EventArgs e)
		{
			if (colorDialog1.ShowDialog() == DialogResult.OK)
			{
				MainColor = colorDialog1.Color;
			}
		}

		private void Button2_Click(object sender, EventArgs e)
		{
			if (colorDialog1.ShowDialog() == DialogResult.OK)
			{
				SecondaryColor = colorDialog1.Color;
			}
		}

		private void ComboBox3_SelectedIndexChanged(object sender, EventArgs e)
		{
			double Temp = (double)comboBox3.SelectedItem;
			FontSize = (float)Temp;
		}
	}
}
