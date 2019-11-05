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
    public partial class SelectFaceLocationForm : Form
    {
        Bitmap actualImage;
        Bitmap showedImage;
        bool down = false;
		public bool Exit = false;
        public int returningTop = 0, returningLeft = 0, returningHeight = 0, returningWidth = 0;

        // Extras
        Rectangle rect;
        Point LocXY;
        Point LocX1Y1;

        public Bitmap ActualImage { get => this.actualImage; set => this.actualImage = value; }
        public int ReturningTop { get => this.returningTop; set => this.returningTop = value; }
        public int ReturningLeft { get => this.returningLeft; set => this.returningLeft = value; }
        public int ReturningHeight { get => this.returningHeight; set => this.returningHeight = value; }
        public int ReturningWidth { get => this.returningWidth; set => this.returningWidth = value; }

        public SelectFaceLocationForm()
        {
            InitializeComponent();
        }



        private void SelectFaceLocationForm_Load(object sender, EventArgs e)
        {
            this.showedImage = (Bitmap)actualImage.Clone();
            this.ImagePictureBox.Image = showedImage;
            this.ImagePictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
        }


        private void ImagePictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            this.showedImage = (Bitmap)ActualImage.Clone();
            this.ImagePictureBox.Image = showedImage;
            this.SelectedTopData.Text = this.TopData.Text;
            this.SelectedLeftData.Text = this.LeftData.Text;
            down = true;
            LocXY = e.Location;
        }



        private void ImagePictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (down == false)
            {
                int[] position = new int[] { Cursor.Position.X - this.Left - 7, Cursor.Position.Y - (this.Top + 29) };
                int[] realPosition = ToRealImageSize(position[0], position[1]);
                this.TopData.Text = Convert.ToString(realPosition[1]);
                this.LeftData.Text = Convert.ToString(realPosition[0]);
            }
            else
            {
                int[] position = new int[] { Cursor.Position.X - this.Left - 9, Cursor.Position.Y - (this.Top + 32) };
                int[] realPosition = ToRealImageSize(position[0], position[1]);
                this.TopData.Text = Convert.ToString(realPosition[1]);
                this.LeftData.Text = Convert.ToString(realPosition[0]);
                this.WidthData.Text = Convert.ToString(Convert.ToInt32(LeftData.Text) - Convert.ToInt32(SelectedLeftData.Text));
                this.HeightData.Text = Convert.ToString(Convert.ToInt32(TopData.Text) - Convert.ToInt32(SelectedTopData.Text));
            }

            if (down == true)
            {
                LocX1Y1 = e.Location;
                Refresh();
            }
        }


        private void ImagePictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            this.SelectedWidthData.Text = this.WidthData.Text;
            this.SelectedHeightData.Text = this.HeightData.Text;
            
            Graphics gF = this.ImagePictureBox.CreateGraphics();
            if (Convert.ToInt32(this.WidthData.Text) < 0)
            {
                this.WidthData.Text = "0";
                this.SelectedWidthData.Text = "0";
            }
            if (Convert.ToInt32(this.HeightData.Text) < 0)
            {
                this.HeightData.Text = "0";
                this.SelectedHeightData.Text = "0";
            }
            int[] coordinates = ToPictureBoxImageSize(Convert.ToInt32(this.SelectedLeftData.Text), Convert.ToInt32(this.SelectedTopData.Text));
            int[] widthheightcoordinates = ToPictureBoxImageSize(Convert.ToInt32(this.WidthData.Text), Convert.ToInt32(this.HeightData.Text));
            gF.DrawRectangle(Pens.Red, coordinates[0], coordinates[1], widthheightcoordinates[0], widthheightcoordinates[1]);

            this.ReturningLeft = Convert.ToInt32(this.SelectedLeftData.Text);
            this.ReturningTop = Convert.ToInt32(this.SelectedTopData.Text);
            this.ReturningWidth = Convert.ToInt32(this.WidthData.Text);
            this.ReturningHeight = Convert.ToInt32(this.HeightData.Text);

            if (down == true)
            {
                LocX1Y1 = e.Location;
                down = false;
            }
        }


        private void SelectFaceLocationForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ReturningTop == 0 || ReturningLeft == 0 || ReturningWidth == 0 || ReturningHeight == 0)
            {
                if (MessageBox.Show("You didn't select a valid Face Location. Do you want to exit?", "Warning!",
                       MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    e.Cancel = true;
                }
            }
        }

        private void ImagePictureBox_Paint(object sender, PaintEventArgs e)
        {
            if (rect != null)
            {
                e.Graphics.DrawRectangle(Pens.Black, GetRect());
            }
        }

        private Rectangle GetRect()
        {
            rect = new Rectangle();
            rect.X = Math.Min(LocXY.X, LocX1Y1.X);
            rect.Y = Math.Min(LocXY.Y, LocX1Y1.Y);
            rect.Width = Math.Abs(LocXY.X - LocX1Y1.X);
            rect.Height = Math.Abs(LocXY.Y - LocX1Y1.Y);
            return rect;
        }

        private void DoneButton_MouseEnter(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            btn.ForeColor = Color.White;
        }

        private void DoneButton_MouseLeave(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            btn.ForeColor = Color.Black;
        }

        private void DoneButton_Click(object sender, EventArgs e)
        {
			Exit = true;
            this.Close();
        }


        private int[] ToRealImageSize(int x, int y)
        {
            int imageWidth = actualImage.Width;
            int imageHeight = actualImage.Height;
            int pictureBoxWidth = this.ImagePictureBox.Width;
            int pictureBoxHeight = this.ImagePictureBox.Height;
            return new int[] { x*imageWidth/pictureBoxWidth, y*imageHeight/pictureBoxHeight};
        }

        private int[] ToPictureBoxImageSize(int x, int y)
        {
            int imageWidth = actualImage.Width;
            int imageHeight = actualImage.Height;
            int pictureBoxWidth = this.ImagePictureBox.Width;
            int pictureBoxHeight = this.ImagePictureBox.Height;
            return new int[] { x * pictureBoxWidth / imageWidth, y*pictureBoxHeight/imageHeight };
        }
    }
}
