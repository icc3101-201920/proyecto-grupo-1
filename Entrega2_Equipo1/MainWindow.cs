using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Windows.Forms;

namespace Entrega2_Equipo1
{
    public partial class MainWindow : Form
    {
		private string[] files;
		private BackgroundWorker worker = new BackgroundWorker();
		Library library;
        Producer producer;
        ProgramManager PM = new ProgramManager();
        bool Saved = true;
        PictureBox chosenImage = null;
        PictureBox chosenEditingImage = null;
        List<Image> featuresImage = new List<Image>();
        Label createdLabel;
        Image imagetoaddlabel;
        Size formsizewithrightpanel = new Size(1702, 822);
        Size formsizewithoutrightpanel = new Size(558, 822);
        User userLoggedIn = null;
        bool exit = true;
        bool deleteaccount = false;
        Bitmap chooseUserPictureBitmap;
        Searcher mainSearcher;
        List<Image> imagestoaddlabel;
        StringBuilder pattern;
        PictureBox selectedSmartBox = null;

        public User UserLoggedIn { get => this.userLoggedIn; set => this.userLoggedIn = value; }
        public bool Exit { get => this.exit; set => this.exit = value; }
        public bool Deleteaccount { get => this.deleteaccount; set => this.deleteaccount = value; }


        public MainWindow()
        {
            InitializeComponent();
            this.menuStrip1.Renderer = new MyRenderer();
			worker.DoWork += WorkerImportFiles;
			worker.ProgressChanged += WorkerImportFilesProgess;
			worker.WorkerReportsProgress = true;
			worker.RunWorkerCompleted += WorkerImportFileCompleted;
            FiltroComboBox.Items.Clear();
            FiltroComboBox.DataSource = Enum.GetValues(typeof(EFilter));
            YesNo.Items.Clear();YesNo.Items.Add(true);YesNo.Items.Add(false);
            SexComboBox.Items.Clear();
            SexComboBox.DataSource = Enum.GetValues(typeof(ESex));
            ColorComboBox.Items.Clear();
            ColorComboBox.DataSource = Enum.GetValues(typeof(EColor));
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            library = PM.LoadingUsersLibraryManager(UserLoggedIn.Usrname);
            producer = PM.LoadingUsersProducerManager(UserLoggedIn.Usrname);
            PanelImages_AddImages();
            comboRotate.DataSource = Enum.GetValues(typeof(RotateFlipType));
            comboCensor.Items.Add("Black bar"); comboCensor.Items.Add("Pixel blur"); comboCensor.Text = "Black bar";
            AddLabelPanel.Location = panelImages.Location;
            AddLabelPanel.Visible = false;
            this.Size = formsizewithoutrightpanel;
            string arrowiconslocation = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + @"\logos\";
            Bitmap rightarrow = (Bitmap)Bitmap.FromFile(arrowiconslocation + "rightarrow.png");
            OpenRightPanelButton.BackgroundImage = rightarrow;
            OpenRightPanelButton.BackgroundImageLayout = ImageLayout.Zoom;
            string imageLocation = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + @"\logos\";
            Bitmap image = (Bitmap)Bitmap.FromFile(imageLocation + "changeuserpicture.png");
            Resizer res = new Resizer();
            int x = 150;
            int y = 150;
            this.chooseUserPictureBitmap = res.ResizeImage(image, x, y);
            this.mainSearcher = new Searcher();
            pattern = new StringBuilder();
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!Saved)
            {
                if (MessageBox.Show("Are you sure you want to close without saving?", "Exit without save", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                {
                    e.Cancel = true;
                }
            }
        }

        private void ImportOnlyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select the desired images";
            ofd.Multiselect = true;
            ofd.Filter = "Supported formats |*.jpg;*.jpeg;*.png;*.bmp";
            DialogResult dr = ofd.ShowDialog();
            if (dr == DialogResult.OK)
            {
                files = ofd.FileNames;
				this.ToolbarProgressBar.Value = 0;
				this.ToolbarProgressBar.Visible = true;
				this.Cursor = Cursors.WaitCursor;
				worker.RunWorkerAsync();
				this.Cursor = Cursors.Arrow;
			}
        }

		private void WorkerImportFiles(object sender, DoWorkEventArgs e)
		{
			int cont = 0;
			foreach (string path in files)
			{
				string name = Path.GetFileNameWithoutExtension(path);
				Image returningImage = new Image(path, new List<Label>(), -1);
				returningImage.Name = name;
				library.AddImage(returningImage);
				int percent = (cont * 100) / files.Length;
				worker.ReportProgress(percent);
				cont++;
			}
		}

		private void WorkerImportFilesProgess(object sender, ProgressChangedEventArgs e)
		{
			this.ToolbarProgressBar.Value = e.ProgressPercentage;
		}

		private void WorkerImportFileCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			ReLoadPanelImage(sender, e);
			Saved = false;
			this.ToolbarProgressBar.Value = 0;
            this.ToolbarProgressBar.Visible = false;
            SmartList_Click(this, EventArgs.Empty);
        }

        private void RemoveFromLibraryToolStripMenuItem_Click(object sender, EventArgs e)
        {

            ToolStripItem menuItem = sender as ToolStripItem;
            if (menuItem != null)
            {
                // Retrieve the ContextMenuStrip that owns this ToolStripItem
                ContextMenuStrip owner = menuItem.Owner as ContextMenuStrip;
                if (owner != null)
                {
                    // Get the control that is displaying this context menu
                    Control sourceControl = owner.SourceControl;
                    PictureBox PIC = (PictureBox)sourceControl;
                    Image im = (Image)PIC.Tag;
                    library.RemoveImage(im);
                    ReLoadPanelImage(sender, e);
                    SmartList_Click(this, EventArgs.Empty);
                    Saved = false;
                    if (PIC == chosenImage) chosenImage = null;
                }
            }
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PM.SavingUsersLibraryManager(UserLoggedIn.Usrname, library);
            Saved = true;
        }

        
        private void ExportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (chosenImage != null)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Images |*.jpg;*.jpeg;*.png;*.bmp";
                ImageFormat format = ImageFormat.Jpeg;
                if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    Image img = (Image)chosenImage.Tag;
                    Bitmap bm = img.BitmapImage;
                    bm.Save(sfd.FileName, format);
                }
            }
            else NoPictureChosen(sender, e);
        }

        private void ExportAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (chosenImage != null)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = ".jpg|*.jpg|.bmp|*.bmp|.png|*.png";
                ImageFormat format = ImageFormat.Jpeg;
                if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string ext = System.IO.Path.GetExtension(sfd.FileName);
                    switch (ext)
                    {
                        case ".png":
                            format = ImageFormat.Png;
                            break;
                        case ".bmp":
                            format = ImageFormat.Bmp;
                            break;
                    }
                    Image img = (Image)chosenImage.Tag;
                    Bitmap bm = img.BitmapImage;
                    bm.Save(sfd.FileName, format);
                }
            }
            else NoPictureChosen(sender, e);
        }

        private void CleanLibraryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (library.Images.Count != 0)
            {
                if (MessageBox.Show("Are you sure you want to clean the library?", "Warning!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    library.ResetImages();
                    ReLoadPanelImage(sender, e);
                    Saved = false;
                }
            }
            else MessageBox.Show("There are no pictures in library", "Clean library error.", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void AddToEditingAreaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripItem menuItem = sender as ToolStripItem;
            if (menuItem != null)
            {
                ContextMenuStrip owner = menuItem.Owner as ContextMenuStrip;
                if (owner != null)
                {
                    Control sourceControl = owner.SourceControl;
                    PictureBox PIC = (PictureBox)sourceControl;
                    pictureChosen.Tag = (Image)PIC.Tag;
                    producer.LoadImagesToWorkingArea(new List<Image>() { (Image)PIC.Tag });
                    EditingPanel_Paint(sender, e);
                    if (RightPanel.Visible == false) OpenRightPanelButton_Click(this, EventArgs.Empty);
                }
            }
        }

        private void AddLabelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripItem menuItem = sender as ToolStripItem;
            if (menuItem != null)
            {
                ContextMenuStrip owner = menuItem.Owner as ContextMenuStrip;
                if (owner != null)
                {
                    Control sourceControl = owner.SourceControl;
                    PictureBox PIC = (PictureBox)sourceControl;
                    Image imagetoaddlabel = (Image)PIC.Tag;
                    AddLabelPanel.Visible = true;
                    this.imagetoaddlabel = imagetoaddlabel;
                    string arrowiconslocation = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + @"\logos\";
                    InfoSettingPanel.Visible = false;
                    ImageInfoPanel.Dock = DockStyle.Fill;
                    Bitmap uparrow = (Bitmap)Bitmap.FromFile(arrowiconslocation + "uparrow.png");
                    CollapseInfoPanelButton.BackgroundImage = uparrow;
                    this.SearchTextBox.Enabled = false;
                    AddLabelController();
                }
            }
            else return;
        }

        private void PanelImages_Paint(object sender, EventArgs e)
        {
			//METODO QUE CARGA IMAGENES AUTOMATICAMENTE
		}

        private void PanelImages_AddImages()
        {
            int x = 20;
            int y = 20;
            int maxHeight = -1;
            int count = 1;
            //this.ToolbarProgressBar.Value = 0;
            //this.ToolbarProgressBar.Visible = true;

            foreach (Image image in library.Images)
            {
                PictureBox pic = new PictureBox();
                pic.Image = NewThumbnailImage(image.BitmapImage);
                pic.Location = new Point(x, y);
                pic.Tag = image;
                pic.Size = new Size(150, 100);
                pic.SizeMode = PictureBoxSizeMode.Zoom;
                pic.BackColor = Color.Transparent;
                pic.Click += ImageDetailClick;
                pic.Click += ImageBorderClick;
                pic.ContextMenuStrip = contextMenuStripImage;
                pic.Name = image.Name;
                pic.Cursor = Cursors.Hand;
                if (chosenImage != null)
                {
                    if (pic.Image == chosenImage.Image && pic.Tag == chosenImage.Tag && pic.Name == chosenImage.Name)
                    {
                        chosenImage.Location = pic.Location;
                        pic = chosenImage;
                        chosenImage.BorderStyle = BorderStyle.Fixed3D;
                    }
                }
                x += pic.Width + 10;
                maxHeight = Math.Max(pic.Height, maxHeight);
                if (x > this.panelImages.Width - 100)
                {
                    x = 20;
                    y += maxHeight + 10;
                }
                this.panelImages.Controls.Add(pic);
                //this.ToolbarProgressBar.Increment((count * 100) / library.Images.Count);
                count++;
            }
            //this.ToolbarProgressBar.Visible = false;
            //this.ToolbarProgressBar.Value = 0;
        }

        private System.Drawing.Image NewThumbnailImage(System.Drawing.Image image)
        {
            System.Drawing.Image thumb = image.GetThumbnailImage(image.Width/4, image.Height/4, () => false, IntPtr.Zero);
            return thumb;
        }

        private System.Drawing.Image NewThumbnailMainImage(System.Drawing.Image image)
        {
            System.Drawing.Image thumb = image.GetThumbnailImage(image.Width, image.Height, () => false, IntPtr.Zero);
            if (image.Width < 1000 || image.Height < 1000)
            {
                thumb = image.GetThumbnailImage(image.Width, image.Height, () => false, IntPtr.Zero);
            }
            else
            {
                thumb = image.GetThumbnailImage(image.Width / 2, image.Height / 2, () => false, IntPtr.Zero);
            }
            return thumb;
        }

        private void PanelImages_PaintSearchResult(List<Image> result)
        {
            int x = 20;
            int y = 20;
            int maxHeight = -1;
            foreach (Image image in result)
            {
                PictureBox pic = new PictureBox();
                pic.Image = NewThumbnailImage(image.BitmapImage);
                pic.Size = new Size(150, 100);
                pic.Location = new Point(x, y);
                pic.BackColor = Color.Transparent;
                pic.Tag = image;
                pic.SizeMode = PictureBoxSizeMode.Zoom;
                pic.Click += ImageDetailClick;
                pic.Click += ImageBorderClick;
                pic.ContextMenuStrip = contextMenuStripImage;
                pic.Name = image.Name;
                pic.Cursor = Cursors.Hand;
                x += pic.Width + 10;
                maxHeight = Math.Max(pic.Height, maxHeight);
                if (x > this.panelImages.Width - 100)
                {
                    x = 20;
                    y += maxHeight + 10;
                }
                this.panelImages.Controls.Add(pic);
            }
        }

        private void ImageDetailClick(object sender, EventArgs e)
        {
            PictureBox PIC = (PictureBox)sender;
            Image image = (Image)PIC.Tag;
            // Esto estaba del property grid
            this.calificationUpDown.Enabled = true;
            this.SetCalificationButton.Enabled = true;
            this.setNewNameButton.Enabled = true;
            this.nameTextBox.Enabled = true;
            this.DeleteLabelButton.Enabled = true;
            this.imagetoaddlabel = image;
            // Mostrar los datos en el tree view
            RefreshInfoTreeView();
            PIC.BackColor = Color.White;
            if(chosenImage != null)chosenImage.BackColor = Color.Transparent;
        }

        private void ImageBorderClick(object sender, EventArgs e)
        {
            PictureBox PIC = (PictureBox)sender;
            if (chosenImage != PIC && chosenImage != null)
            {
                chosenImage.BorderStyle = BorderStyle.None;
                chosenImage = PIC;
            }
            else chosenImage = PIC;
            PIC.BorderStyle = BorderStyle.FixedSingle;
        }

        private void NoPictureChosen(object sender, EventArgs e)
        {
            MessageBox.Show("A picture has to be chosen in order to export.", "No picture chosen", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ReLoadPanelImage(object sender, EventArgs e)
        {
            panelImages.Controls.Clear();
            PanelImages_AddImages();
        }

        private void PanelImages_Click(object sender, EventArgs e)
        {
            if (chosenImage != null)
            {
                chosenImage.BorderStyle = BorderStyle.None;
                chosenImage.BackColor = Color.Transparent;
                chosenImage = null;
                this.calificationUpDown.Enabled = false;
                this.SetCalificationButton.Enabled = false;
                this.setNewNameButton.Enabled = false;
                this.DeleteLabelButton.Enabled = false;
                this.nameTextBox.Enabled = false;
                InfoTreeView.Nodes.Clear();
                chosenImage = null;
            }
        }

        private void SetCalificationButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.imagetoaddlabel.Calification = (calificationUpDown.Value == 0) ? -1 : Convert.ToInt32(calificationUpDown.Value);
                RefreshInfoTreeView();
                SmartList_Click(this, EventArgs.Empty);
            }
            catch
            {
                return;
            }
        }

        private void SetNewNameButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.imagetoaddlabel.Name = nameTextBox.Text;
                RefreshInfoTreeView();
                SmartList_Click(this, EventArgs.Empty);
            }
            catch
            {
                return;
            }
        }


        private void RefreshInfoTreeView()
        {
            InfoTreeView.Nodes.Clear();
            InfoTreeView.Nodes.Add("Image information");
            InfoTreeView.Nodes[0].Nodes.Add("Name: " + imagetoaddlabel.Name);
            string cal = imagetoaddlabel.Calification == -1 ? "" : Convert.ToString(imagetoaddlabel.Calification);
            InfoTreeView.Nodes[0].Nodes.Add("Calification: " + cal);
            InfoTreeView.Nodes[0].Nodes.Add("Resolution: " + Convert.ToString(imagetoaddlabel.Resolution[0]) + "x" + Convert.ToString(imagetoaddlabel.Resolution[1]));
            InfoTreeView.Nodes[0].Nodes.Add("Aspect ratio: " + Convert.ToString(imagetoaddlabel.AspectRatio[0]) + ":" + Convert.ToString(imagetoaddlabel.AspectRatio[1]));
            InfoTreeView.Nodes[0].Nodes.Add("Saturation: " + Convert.ToString(imagetoaddlabel.Saturation));
            string clear = imagetoaddlabel.DarkClear == true ? "Yes" : "No";
            InfoTreeView.Nodes[0].Nodes.Add("Clear: " + clear);
            InfoTreeView.Nodes.Add("Metadata information");
            if (imagetoaddlabel.Metadata != null)
            {
                foreach (string directoryname in imagetoaddlabel.Metadata.Keys)
                {
                    int count = 0;
                    InfoTreeView.Nodes[1].Nodes.Add(directoryname);
                    foreach (string tagname in imagetoaddlabel.Metadata[directoryname].Keys)
                    {
                        InfoTreeView.Nodes[1].Nodes[count].Nodes.Add(tagname + ": " + imagetoaddlabel.Metadata[directoryname][tagname]);
                    }
                    count++;
                }
            }
            InfoTreeView.Nodes.Add("Labels information");
            InfoTreeView.Nodes[2].Nodes.Add("Simple Labels");
            InfoTreeView.Nodes[2].Nodes.Add("Person Labels");
            InfoTreeView.Nodes[2].Nodes.Add("Special Labels");
            int simplecounter = 0, specialcounter = 0, personcounter = 0;
            foreach (Label label in imagetoaddlabel.Labels)
            {
                switch (label.labelType)
                {
                    case "SimpleLabel":
                        SimpleLabel label2 = (SimpleLabel)label;
                        InfoTreeView.Nodes[2].Nodes[0].Nodes.Add("Simple " + Convert.ToInt32(simplecounter + 1));
                        InfoTreeView.Nodes[2].Nodes[0].Nodes[simplecounter].Nodes.Add("Tag: " + label2.Sentence);
                        simplecounter++;
                        break;
                    case "PersonLabel":
                        PersonLabel label3 = (PersonLabel)label;
                        InfoTreeView.Nodes[2].Nodes[1].Nodes.Add("Person " + Convert.ToInt32(personcounter + 1));
                        if (label3.Name != null) InfoTreeView.Nodes[2].Nodes[1].Nodes[personcounter].Nodes.Add("Name: " + label3.Name);
                        else InfoTreeView.Nodes[2].Nodes[1].Nodes[personcounter].Nodes.Add("Name: None");
                        if (label3.Surname != null) InfoTreeView.Nodes[2].Nodes[1].Nodes[personcounter].Nodes.Add("Surname: " + label3.Surname);
                        else InfoTreeView.Nodes[2].Nodes[1].Nodes[personcounter].Nodes.Add("Surname: None");
                        InfoTreeView.Nodes[2].Nodes[1].Nodes[personcounter].Nodes.Add("Sex: " + label3.Sex);
                        InfoTreeView.Nodes[2].Nodes[1].Nodes[personcounter].Nodes.Add("Country: " + label3.Nationality);
                        InfoTreeView.Nodes[2].Nodes[1].Nodes[personcounter].Nodes.Add("Eyes Color: " + label3.EyesColor);
                        InfoTreeView.Nodes[2].Nodes[1].Nodes[personcounter].Nodes.Add("Hair Color: " + label3.HairColor);
                        if (label3.BirthDate != "") InfoTreeView.Nodes[2].Nodes[1].Nodes[personcounter].Nodes.Add("Birth Date: " + label3.BirthDate);
                        else InfoTreeView.Nodes[2].Nodes[1].Nodes[personcounter].Nodes.Add("Birth Date: None");
                        if (label3.FaceLocation != null) InfoTreeView.Nodes[2].Nodes[1].Nodes[personcounter].Nodes.Add("Face Location: " + label3.FaceLocation[0].ToString() + "," + label3.FaceLocation[1].ToString() + "," + label3.FaceLocation[2].ToString() + "," + label3.FaceLocation[3].ToString());
                        else InfoTreeView.Nodes[2].Nodes[1].Nodes[personcounter].Nodes.Add("Face Location: None");
                        personcounter++;
                        break;
                    case "SpecialLabel":
                        SpecialLabel label4 = (SpecialLabel)label;
                        InfoTreeView.Nodes[2].Nodes[2].Nodes.Add("Special " + Convert.ToInt32(specialcounter + 1));
                        if (label4.GeographicLocation != null) InfoTreeView.Nodes[2].Nodes[2].Nodes[specialcounter].Nodes.Add("Geo Location: " + label4.GeographicLocation[0].ToString() + "," + label4.GeographicLocation[1].ToString());
                        else InfoTreeView.Nodes[2].Nodes[2].Nodes[specialcounter].Nodes.Add("Geo Location: None");
                        if (label4.Address != null) InfoTreeView.Nodes[2].Nodes[2].Nodes[specialcounter].Nodes.Add("Address: " + label4.Address);
                        else InfoTreeView.Nodes[2].Nodes[2].Nodes[specialcounter].Nodes.Add("Address: None");
                        if (label4.Photographer != null) InfoTreeView.Nodes[2].Nodes[2].Nodes[specialcounter].Nodes.Add("Photographer: " + label4.Photographer);
                        else InfoTreeView.Nodes[2].Nodes[2].Nodes[specialcounter].Nodes.Add("Photographer: None");
                        if (label4.PhotoMotive != null) InfoTreeView.Nodes[2].Nodes[2].Nodes[specialcounter].Nodes.Add("Photo Motive: " + label4.PhotoMotive);
                        else InfoTreeView.Nodes[2].Nodes[2].Nodes[specialcounter].Nodes.Add("Photo Motive: None");
                        if (label4.Selfie != false) InfoTreeView.Nodes[2].Nodes[2].Nodes[specialcounter].Nodes.Add("Selfie: Yes");
                        else InfoTreeView.Nodes[2].Nodes[2].Nodes[specialcounter].Nodes.Add("Selfie: No");
                        specialcounter++;
                        break;
                }
            }
            InfoTreeView.CollapseAll();
        }


        private void SaveFilterApplyed(EFilter filter, Image img)
        {
            img.ApplyedFilters[filter] = true;
        }

        private void EditingPanel_Paint(object sender, EventArgs e)
        {
            int x = 20;
            int y = 20;
            int maxHeight = -1;
            List<Image> editingImages = PM.producer.imagesInTheWorkingArea();
            foreach (Image image in editingImages)
            {
                PictureBox pic = new PictureBox();
                pic.Image = NewThumbnailImage(image.BitmapImage);
                pic.Size = new Size(100, 50);
                pic.Location = new Point(x, y);
                pic.Tag = image;
                pic.SizeMode = PictureBoxSizeMode.Zoom;
                pic.BackColor = Color.Transparent;
                pic.Click += ImageEditingBorderClick;
                pic.Click += MainEditingImage;
                pic.Cursor = Cursors.Hand;
                pic.ContextMenuStrip = contextMenuStripEditing;
                pic.Name = image.Name;
				if (chosenEditingImage != null)
				{
					if (pic.Image == chosenEditingImage.Image && pic.Tag == chosenEditingImage.Tag && pic.Name == chosenEditingImage.Name)
					{
						chosenEditingImage.Location = pic.Location;
						pic = chosenEditingImage;
						chosenEditingImage.BorderStyle = BorderStyle.Fixed3D;
					}
				}
				x += pic.Width + 10;
                maxHeight = Math.Max(pic.Height, maxHeight);
                if (x > this.topauxlabel.Width - 100)
                {
                    x = 20;
                    y += maxHeight + 10;
                }
                this.topauxlabel.Controls.Add(pic);
            }

        }


        private void MainEditingImage(object sender, EventArgs e)
        {
            PictureBox image = (PictureBox)sender;
            pictureChosen.SizeMode = PictureBoxSizeMode.Zoom;
            Image img = (Image)image.Tag;
            Bitmap pic = (Bitmap)img.BitmapImage;
            pictureChosen.Image = NewThumbnailMainImage(pic);
            brightnessBar.Value = 0;
			ContrastBar.Value = 0;
        }

        

        private void ImageEditingBorderClick(object sender, EventArgs e)
        {
            PictureBox PIC = (PictureBox)sender;
            if (chosenEditingImage != PIC && chosenEditingImage != null)
            {
                chosenEditingImage.BorderStyle = BorderStyle.None;
                chosenEditingImage = PIC;
            }
            else chosenEditingImage = PIC;
            MosaicpictureBox.Image = chosenEditingImage.Image;
            pictureCollageImage.Image = chosenEditingImage.Image;
            PIC.BorderStyle = BorderStyle.Fixed3D;
        }

        private void ReLoadEditingPanelImage(object sender, EventArgs e)
        {
            topauxlabel.Controls.Clear();
            EditingPanel_Paint(sender, e);
        }

        private void RemoveFromEditingAreaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete this image from the editing area?", "Warning!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                ToolStripItem menuItem = sender as ToolStripItem;
                if (menuItem != null)
                {
                    ContextMenuStrip owner = menuItem.Owner as ContextMenuStrip;
                    if (owner != null)
                    {
                        Control sourceControl = owner.SourceControl;
                        PictureBox PIC = (PictureBox)sourceControl;
                        Image im = (Image)PIC.Tag;
                        PM.producer.RemoveImage(im);
                        ReLoadEditingPanelImage(sender, e);
                        Saved = false;
                        if (PIC == chosenEditingImage)
                        {
                            pictureChosen.SizeMode = PictureBoxSizeMode.CenterImage;
                            pictureChosen.Image = pictureChosen.ErrorImage;
                            chosenEditingImage = null;
                        }
                    }
                }
            }

        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if (chosenEditingImage != null)
            {
                Image image = (Image)chosenEditingImage.Tag;
                image.BitmapImage = producer.ApplyFilter((Image)chosenEditingImage.Tag, EFilter.Grayscale);
                SaveFilterApplyed(EFilter.Grayscale, image);
                chosenEditingImage.Image = image.BitmapImage;
                pictureChosen.Image = chosenEditingImage.Image;
            }
        }

        private void Button9_Click(object sender, EventArgs e)
        {
            if (chosenEditingImage != null)
            {
                Image image = (Image)chosenEditingImage.Tag;
                image.BitmapImage = producer.ApplyFilter((Image)chosenEditingImage.Tag, EFilter.Sepia);
                SaveFilterApplyed(EFilter.Sepia, image);
                chosenEditingImage.Image = image.BitmapImage;
                pictureChosen.Image = chosenEditingImage.Image;
            }
        }

        private void ExportToLibraryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripItem menuItem = sender as ToolStripItem;
            if (menuItem != null)
            {
                ContextMenuStrip owner = menuItem.Owner as ContextMenuStrip;
                if (owner != null)
                {
                    Control sourceControl = owner.SourceControl;
                    PictureBox PIC = (PictureBox)sourceControl;
                    Image im = (Image)PIC.Tag;
                    PM.producer.RemoveImage(im);
                    library.AddImage(im);
                    ReLoadPanelImage(sender, e);
                    ReLoadEditingPanelImage(sender, e);
                    Saved = false;
                    if (PIC == chosenEditingImage)
                    {
                        pictureChosen.SizeMode = PictureBoxSizeMode.CenterImage;
                        pictureChosen.Image = pictureChosen.ErrorImage;
                        chosenEditingImage = null;
                    }
                    SmartList_Click(this, EventArgs.Empty);
                }
            }
        }

        private void Button10_Click(object sender, EventArgs e)
        {
            if (chosenEditingImage != null)
            {
                Image image = (Image)chosenEditingImage.Tag;
                image.BitmapImage = producer.ApplyFilter((Image)chosenEditingImage.Tag, EFilter.Windows);
                SaveFilterApplyed(EFilter.Windows, image);
                image.ResetFaceLocation();
                chosenEditingImage.Image = image.BitmapImage;
                pictureChosen.Image = chosenEditingImage.Image;
            }
        }

        private void Button7_Click(object sender, EventArgs e)
        {
            if (chosenEditingImage != null)
            {
                Image image = (Image)chosenEditingImage.Tag;
                image.BitmapImage = producer.ApplyFilter((Image)chosenEditingImage.Tag, EFilter.OldFilm);
                SaveFilterApplyed(EFilter.OldFilm, image);
                chosenEditingImage.Image = image.BitmapImage;
                pictureChosen.Image = chosenEditingImage.Image;
            }
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            if (chosenEditingImage != null)
            {
                Image image = (Image)chosenEditingImage.Tag;
                image.BitmapImage = producer.ApplyFilter((Image)chosenEditingImage.Tag, EFilter.Invert);
                SaveFilterApplyed(EFilter.Invert, image);
                chosenEditingImage.Image = image.BitmapImage;
                pictureChosen.Image = chosenEditingImage.Image;
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (chosenEditingImage != null)
            {
                Image image = (Image)chosenEditingImage.Tag;
                image.BitmapImage = producer.ApplyFilter((Image)chosenEditingImage.Tag, EFilter.AutomaticAdjustment);
                SaveFilterApplyed(EFilter.AutomaticAdjustment, image);
                chosenEditingImage.Image = image.BitmapImage;
                pictureChosen.Image = chosenEditingImage.Image;
            }
        }

        private void Button6_Click(object sender, EventArgs e)
        {
            if (chosenEditingImage != null)
            {
                Image image = (Image)chosenEditingImage.Tag;
                image.BitmapImage = producer.ApplyFilter((Image)chosenEditingImage.Tag, EFilter.Mirror);
                SaveFilterApplyed(EFilter.Mirror, image);
                image.ResetFaceLocation();
                chosenEditingImage.Image = image.BitmapImage;
                pictureChosen.Image = chosenEditingImage.Image;
            }
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            if (chosenEditingImage != null)
            {
                if (colorDialogFilter.ShowDialog() == DialogResult.OK)
                {
                    Image image = (Image)chosenEditingImage.Tag;
                    image.BitmapImage = producer.ApplyFilter((Image)chosenEditingImage.Tag, EFilter.Color, colorDialogFilter.Color);
                    SaveFilterApplyed(EFilter.Color, image);
                    chosenEditingImage.Image = image.BitmapImage;
                    pictureChosen.Image = chosenEditingImage.Image;
                }
            }
        }

        private void ComboRotate_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (chosenEditingImage != null)
            {
                Image image = (Image)chosenEditingImage.Tag;
                image.BitmapImage = producer.ApplyFilter((Image)chosenEditingImage.Tag, EFilter.RotateFlip, Color.Empty, 0, 60, (RotateFlipType)comboRotate.SelectedItem);
                SaveFilterApplyed(EFilter.RotateFlip, image);
                image.ResetFaceLocation();
                chosenEditingImage.Image = image.BitmapImage;
                pictureChosen.Image = chosenEditingImage.Image;
            }
        }

		private void ComboCensor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (chosenEditingImage != null)
            {
                Image image = (Image)chosenEditingImage.Tag;
                SelectFaceLocationForm newForm = new SelectFaceLocationForm();
                newForm.ActualImage = image.BitmapImage;
                newForm.Text = comboCensor.SelectedItem.ToString();
                newForm.ShowDialog();
                int newLeft = newForm.ReturningLeft;
                int newTop = newForm.ReturningTop;
                int newHeight = newForm.ReturningHeight;
                int newWidth = newForm.ReturningWidth;
                switch (comboCensor.SelectedItem)
                {
                    case "Black bar":
                        if (newForm.Exit)
                        {
                            int[] coordinatesBlack = { newWidth, newHeight, newTop, newLeft };
                            image.BitmapImage = producer.BlackCensorship(image, coordinatesBlack);
                            chosenEditingImage.Image = image.BitmapImage;
                            pictureChosen.Image = chosenEditingImage.Image;
                        }
                        break;
                    case "Pixel blur":
                        if (newForm.Exit)
                        {
                            int[] coordinatesBlur = { newLeft, newTop, newWidth, newHeight };
                            image.BitmapImage = producer.PixelCensorship(image, coordinatesBlur);
                            chosenEditingImage.Image = image.BitmapImage;
                            pictureChosen.Image = chosenEditingImage.Image;
                        }
                        break;
                }
            }
        }

        private void DoneButton_Click(object sender, EventArgs e)
        {
            if (createdLabel == null)
            {
                if (MessageBox.Show("You didn't create any new Label. Do you want to exit?", "Warning!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    AddLabelPanel.Visible = false;
                }
            }
            AddLabelPanel.Visible = false;
            panelImages.Visible = true;
            this.importToolStripMenuItem1.Enabled = true;
            this.exportToolStripMenuItem.Enabled = true;
            this.saveToolStripMenuItem.Enabled = true;
            this.cleanLibraryToolStripMenuItem.Enabled = true;
            this.myAccountToolStripMenuItem.Enabled = true;
            this.exitToolStripMenuItem.Enabled = true;
            SmartList_Click(this, EventArgs.Empty);
            this.SearchTextBox.Enabled = true;
            ResetAddLabelEntries();
        }

        private void ResetAddLabelEntries()
        {
            PersonLabelNameBox.Text = "";
            PersonLabelSurnameBox.Text = "";
            PersonLabelSexComboBox.SelectedItem = null;
            PersonLabelHairColorComboBox.SelectedItem = null;
            PersonLabelEyesColorComboBox.SelectedItem = null;
            PersonLabelNationalityComboBox.SelectedItem = null;
            PersonLabelBirthDatePicker.Value = new DateTime(1930, 1, 1);
            SpecialLabelAddressTextBox.Text = "";
            SpecialLabelPhotographerTextBox.Text = "";
            SpecialLabelPhotoMotiveTextBox.Text = "";
            SpecialLabelLatitudeUpDown.Value = 0;
            SpecialLabelLongitudeUpDown.Value = 0;
            SpecialLabelSelfieComboxBox.SelectedItem = null;
            SimpleLabelTagBox.Text = "";
            WatsonRecommendationsComboBox.SelectedItem = null;
            WatsonRecommendationsComboBox.Items.Clear();
            FaceLocationHeightTag.Text = "0";
            FaceLocationLeftTag.Text = "0";
            FaceLocationWidthTag.Text = "0";
            FaceLocationTopTag.Text = "0";
        }

        private void AddLabelController()
        {
            this.createdLabel = null;
            this.AddLabelImageBox.Image = NewThumbnailMainImage((Bitmap)this.imagetoaddlabel.BitmapImage.Clone());
            this.AddLabelImageBox.Tag = this.imagetoaddlabel;
            this.AddLabelImageBox.SizeMode = PictureBoxSizeMode.Zoom;
            this.PersonLabelNationalityComboBox.DataSource = Enum.GetValues(typeof(ENationality));
            this.PersonLabelHairColorComboBox.DataSource = Enum.GetValues(typeof(EColor));
            this.PersonLabelEyesColorComboBox.DataSource = Enum.GetValues(typeof(EColor));
            this.PersonLabelSexComboBox.DataSource = Enum.GetValues(typeof(ESex));
            this.PersonalizedTagCheck.Checked = true;
            this.SpecialLabelSelfieComboxBox.SelectedIndex = 0;
            this.importToolStripMenuItem1.Enabled = false;
            this.exportToolStripMenuItem.Enabled = false;
            this.saveToolStripMenuItem.Enabled = false;
            this.myAccountToolStripMenuItem.Enabled = false;
            this.exitToolStripMenuItem.Enabled = false;
            this.cleanLibraryToolStripMenuItem.Enabled = false;
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox combo = (ComboBox)sender;
            switch (combo.SelectedIndex)
            {
                case 0:
                    AuxiliarEnablerDisabler("SimpleLabel");
                    break;
                case 1:
                    AuxiliarEnablerDisabler("PersonLabel");
                    break;
                case 2:
                    AuxiliarEnablerDisabler("SpecialLabel");
                    break;
            }
        }

        private void AuxiliarEnablerDisabler(string type)
        {
            switch (type)
            {
                case "SimpleLabel":
                    AddSimpleLabelPanel.Visible = true;
                    AddPersonLabelPanel.Visible = false;
                    AddSpecialLabelPanel.Visible = false;
                    break;
                case "PersonLabel":
                    AddPersonLabelPanel.Visible = true;
                    AddSimpleLabelPanel.Visible = false;
                    AddSpecialLabelPanel.Visible = false;
                    break;
                case "SpecialLabel":
                    AddPersonLabelPanel.Visible = false;
                    AddSimpleLabelPanel.Visible = false;
                    AddSpecialLabelPanel.Visible = true;
                    break;
            }
        }

        private void AddLabelButton_Click(object sender, EventArgs e)
        {
            switch (this.SelectedLabelComboBox1.SelectedIndex)
            {
                case 0:
                    if (this.PersonalizedTagCheck.Checked)
                    {
                        if (this.SimpleLabelTagBox.Text == "")
                        {
                            MessageBox.Show("You cant add an empty tag", "Add label error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {

                            this.createdLabel = new SimpleLabel(this.SimpleLabelTagBox.Text);
                            this.imagetoaddlabel.AddLabel(createdLabel);
                        }
                    }
                    else
                    {
                        if (Convert.ToString(this.WatsonRecommendationsComboBox.SelectedItem) == "")
                        {
                            MessageBox.Show("You cant add an empty tag", "Add label error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            this.createdLabel = new SimpleLabel(Convert.ToString(this.WatsonRecommendationsComboBox.SelectedItem));
                            this.imagetoaddlabel.AddLabel(createdLabel);
                        }
                    }
                    break;
                case 1:
                    this.createdLabel = new PersonLabel(this.PersonLabelNameBox.Text == "" ? null : this.PersonLabelNameBox.Text,
                    (FaceLocationLeftTag.Text == "0" && FaceLocationTopTag.Text == "0" && FaceLocationWidthTag.Text == "0" && FaceLocationHeightTag.Text == "0") ? null : new double[] { Convert.ToInt32(FaceLocationLeftTag.Text), Convert.ToInt32(FaceLocationTopTag.Text), Convert.ToInt32(FaceLocationWidthTag.Text), Convert.ToInt32(FaceLocationHeightTag.Text) },
                    this.PersonLabelSurnameBox.Text == "" ? null : this.PersonLabelSurnameBox.Text, (ENationality)this.PersonLabelNationalityComboBox.SelectedItem,
                    (EColor)this.PersonLabelEyesColorComboBox.SelectedItem, (EColor)this.PersonLabelHairColorComboBox.SelectedItem, (ESex)this.PersonLabelSexComboBox.SelectedItem,
                    this.PersonLabelBirthDatePicker.Value.Date.ToString() == "01-01-1930 0:00:00" ? "" : this.PersonLabelBirthDatePicker.Value.Date.ToString());
                    this.imagetoaddlabel.AddLabel(createdLabel);
                    break;
                case 2:
                    this.createdLabel = new SpecialLabel((this.SpecialLabelLatitudeUpDown.Value == 0 && this.SpecialLabelLongitudeUpDown.Value == 0) ? null : new double[] { Convert.ToDouble(this.SpecialLabelLatitudeUpDown.Value), Convert.ToDouble(this.SpecialLabelLatitudeUpDown.Value) },
                        (this.SpecialLabelAddressTextBox.Text == "") ? null : this.SpecialLabelAddressTextBox.Text, (this.SpecialLabelPhotographerTextBox.Text == "") ? null : this.SpecialLabelPhotographerTextBox.Text,
                        (this.SpecialLabelPhotoMotiveTextBox.Text == "") ? null : this.SpecialLabelPhotoMotiveTextBox.Text, (this.SpecialLabelSelfieComboxBox.SelectedItem.ToString() == "Si") ? true : false);
                    this.imagetoaddlabel.AddLabel(createdLabel);
                    break;
            }
            RefreshInfoTreeView();
        }

        private void PersonalizedTagCheck_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox box = (CheckBox)sender;
            if (box.Checked)
            {
                SimpleLabelTagBox.Enabled = true;
                WatsonTagCheck.Checked = false;
            }
            else
            {
                SimpleLabelTagBox.Enabled = false;
                WatsonTagCheck.Checked = true;
            }
        }

        private void WatsonTagCheck_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox box = (CheckBox)sender;
            if (box.Checked)
            {
                PersonalizedTagCheck.Checked = false;
                LoadWatsonRecommendationsButton.Enabled = true;
                WatsonRecommendationsComboBox.Enabled = true;
            }
            else
            {
                PersonalizedTagCheck.Checked = true;
                LoadWatsonRecommendationsButton.Enabled = false;
                WatsonRecommendationsComboBox.Enabled = false;
                LoadingWatsonRecommendationsLabel.Text = "";
            }
        }

        private void LoadWatsonRecommendationsButton_Click(object sender, EventArgs e)
        {
            this.WatsonRecommendationsComboBox.Items.Clear();
            this.LoadingWatsonRecommendationsLabel.Text = "Loading...";
            this.LoadingWatsonRecommendationsLabel.Visible = true;
            try
            {
                List<string> options = this.PM.LoadWatsonRecommendations(this.imagetoaddlabel, this.producer);
                foreach (string option in options)
                {
                    this.WatsonRecommendationsComboBox.Items.Add(option);
                }
                this.LoadingWatsonRecommendationsLabel.Text = "Done!";
            }
            catch
            {
                this.LoadingWatsonRecommendationsLabel.Text = "[!] ERROR";
            }
        }

        private void SelectFaceLocationButton_Click(object sender, EventArgs e)
        {
            SelectFaceLocationForm newForm = new SelectFaceLocationForm();
            Image image = (Image)this.AddLabelImageBox.Tag;
            newForm.ActualImage = image.BitmapImage;
            var result = newForm.ShowDialog();
            int newLeft = newForm.ReturningLeft;
            int newTop = newForm.ReturningTop;
            int newHeight = newForm.ReturningHeight;
            int newWidth = newForm.ReturningWidth;
            if (newLeft != 0 && newTop != 0 && newHeight != 0 && newWidth != 0)
            {
                this.FaceLocationTopTag.Text = Convert.ToString(newTop);
                this.FaceLocationLeftTag.Text = Convert.ToString(newLeft);
                this.FaceLocationWidthTag.Text = Convert.ToString(newWidth);
                this.FaceLocationHeightTag.Text = Convert.ToString(newHeight);
            }
        }

        private void PutZeroButton_Click(object sender, EventArgs e)
        {
            this.FaceLocationTopTag.Text = "0";
            this.FaceLocationLeftTag.Text = "0";
            this.FaceLocationWidthTag.Text = "0";
            this.FaceLocationHeightTag.Text = "0";
        }

        private void Label8_Click(object sender, EventArgs e)
        {

        }

        private void Asdfbutton_Click(object sender, EventArgs e)
        {
            AddLabelPanel.Visible = true;
            panelImages.Visible = false;
            AddLabelController();
        }

        private void Button13_Click(object sender, EventArgs e)
        {
            if (chosenEditingImage != null)
            {
                Image image = (Image)chosenEditingImage.Tag;
                FormAdd form = new FormAdd();
                form.ActualImage = image.BitmapImage;
                form.ShowDialog();
                int x = form.X;
                int y = form.Y;
                string Text = form.Text;
                string FontStyle = form.FontStyle;
                string FontName = form.FontName;
                float FontSize = form.FontSize;
                Color MainColor = form.MainColor;
                Color SecondColor = form.SecondaryColor;
                if (SecondColor == Color.Empty)
                {
                    SecondColor = MainColor;
                }
                if (form.Exit)
                {
                    image.BitmapImage = producer.AddText(image.BitmapImage, Text, x, y, FontSize, MainColor, FontStyle, FontName, SecondColor);
                    chosenEditingImage.Image = image.BitmapImage;
                    pictureChosen.Image = chosenEditingImage.Image;
                }
            }
        }

        private void Button14_Click(object sender, EventArgs e)
        {
            if (chosenEditingImage != null)
            {
                panelResize.Visible = true;
                Image image = (Image)chosenEditingImage.Tag;
                image.ResetFaceLocation();
                XText.Text = image.Resolution[0].ToString();
                YText.Text = image.Resolution[1].ToString();
            }
        }

        private void ResizeDone_Click(object sender, EventArgs e)
        {
            panelResize.Visible = false;
            Resizer res = new Resizer();
            int x = Convert.ToInt32(XText.Text);
            int y = Convert.ToInt32(YText.Text);
            Image image = (Image)chosenEditingImage.Tag;
            image.BitmapImage = res.ResizeImage(image.BitmapImage, x, y);
            image.ResetFaceLocation();
            chosenEditingImage.Image = image.BitmapImage;
            pictureChosen.Image = chosenEditingImage.Image;
        }

        private void AddToFeaturesListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripItem menuItem = sender as ToolStripItem;
            if (menuItem != null)
            {
                ContextMenuStrip owner = menuItem.Owner as ContextMenuStrip;
                if (owner != null)
                {
                    Control sourceControl = owner.SourceControl;
                    PictureBox PIC = (PictureBox)sourceControl;
                    Image im = (Image)PIC.Tag;
                    featuresImage.Add(im);
                    Refresh_FeatureListPanel();
                }
            }
        }

        private void Button8_Click(object sender, EventArgs e)
        {
            if (featuresImage.Count > 1)
            {
                Bitmap merged = producer.Merge(featuresImage);
                Image MergeImage = new Image(merged, new List<Label>(), -1);
                producer.LoadImagesToWorkingArea(new List<Image>() { MergeImage });
                EditingPanel_Paint(sender, e);
                pictureChosen.Image = merged;
            }
            else
            {
                MessageBox.Show("Select at least two images", "Not enough images.", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void RemoveFromFeaturesListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripItem menuItem = sender as ToolStripItem;
            if (menuItem != null)
            {
                ContextMenuStrip owner = menuItem.Owner as ContextMenuStrip;
                if (owner != null)
                {
                    Control sourceControl = owner.SourceControl;
                    PictureBox PIC = (PictureBox)sourceControl;
                    Image im = (Image)PIC.Tag;
                    featuresImage.Remove(im);
                }
            }
        }

        private void Button15_Click(object sender, EventArgs e)
        {
            if (featuresImage.Count > 0)
            {
                MosaicPanel.Visible = true;
                try
                {
                    MosaicpictureBox.Image = chosenEditingImage.Image;
                    MosaicpictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                }
                catch
                {
                    MosaicpictureBox.Image = MosaicpictureBox.ErrorImage;
                    MosaicpictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                }
            }
            else
            {
                MessageBox.Show("There has to be at least one picture selected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            if (featuresImage.Count > 1)
            {
                panelCollage.Visible = true;
                try
                {
                    pictureCollageImage.Image = chosenEditingImage.Image;
                    pictureCollageImage.SizeMode = PictureBoxSizeMode.StretchImage;
                }
                catch
                {
                    pictureCollageImage.Image = pictureCollageImage.ErrorImage;
                    pictureCollageImage.SizeMode = PictureBoxSizeMode.StretchImage;
                }
            }
            else
            {
                MessageBox.Show("Select at least two images", "Not enough images.", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ButtonCollage_Click(object sender, EventArgs e)
        {
            try
            {
                int BaseW = Convert.ToInt32(textBaseW.Text);
                int BaseH = Convert.ToInt32(textBaseH.Text);
                int InsertW = Convert.ToInt32(textInsertW.Text);
                int InsertH = Convert.ToInt32(textInsertH.Text);
                if (radioButtonSolid.Checked)
                {
                    if (colorDialogFilter.ShowDialog() == DialogResult.OK)
                    {

                        Color color = colorDialogFilter.Color;
                        Bitmap collage = producer.Collage(featuresImage, BaseW, BaseH, InsertW, InsertH, null, color.R, color.G, color.B);
                        Image MergeImage = new Image(collage, new List<Label>(), -1);
                        producer.LoadImagesToWorkingArea(new List<Image>() { MergeImage });
                        EditingPanel_Paint(sender, e);
                        pictureChosen.Image = collage;
                        panelCollage.Visible = false;
                        textBaseW.Text = "";
                        textBaseH.Text = "";
                        textInsertW.Text = "";
                        textInsertH.Text = "";
                    }
                }
                else if (radioButtonImage.Checked && chosenEditingImage != null)
                {
                    Image im = (Image)chosenEditingImage.Tag;
                    Bitmap collage = producer.Collage(featuresImage, BaseW, BaseH, InsertW, InsertH, im.BitmapImage);
                    Image MergeImage = new Image(collage, new List<Label>(), -1);
                    producer.LoadImagesToWorkingArea(new List<Image>() { MergeImage });
                    EditingPanel_Paint(sender, e);
                    pictureChosen.Image = collage;
                    panelCollage.Visible = false;
                    textBaseW.Text = "";
                    textBaseH.Text = "";
                    textInsertW.Text = "";
                    textInsertH.Text = "";
                }
                else
                {
                    MessageBox.Show("There is no picture selected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch
            {
                MessageBox.Show("Wrong Parameters", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ImportToolStripMenuItem1_MouseEnter(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Hand;
            var selecteditem = (ToolStripMenuItem)sender;
            selecteditem.Font = new Font(selecteditem.Font, FontStyle.Bold);
        }

        private void ImportToolStripMenuItem1_MouseLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Arrow;
            var selecteditem = (ToolStripMenuItem)sender;
            selecteditem.Font = new Font(selecteditem.Font, FontStyle.Regular);
        }

        private void ImportWithLabelsToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Hand;
            var selecteditem = (ToolStripMenuItem)sender;
            selecteditem.Font = new Font(selecteditem.Font, FontStyle.Bold);
        }

        private void ImportWithLabelsToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Arrow;
            this.Cursor = Cursors.Arrow;
            var selecteditem = (ToolStripMenuItem)sender;
            selecteditem.Font = new Font(selecteditem.Font, FontStyle.Regular);
        }

        private void ExportToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Hand;
            var selecteditem = (ToolStripMenuItem)sender;
            selecteditem.Font = new Font(selecteditem.Font, FontStyle.Bold);
        }

        private void ExportToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Arrow;
            this.Cursor = Cursors.Arrow;
            var selecteditem = (ToolStripMenuItem)sender;
            selecteditem.Font = new Font(selecteditem.Font, FontStyle.Regular);
        }

        private void ExportAsToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Hand;
            var selecteditem = (ToolStripMenuItem)sender;
            selecteditem.Font = new Font(selecteditem.Font, FontStyle.Bold);
        }

        private void ExportAsToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Arrow;
            this.Cursor = Cursors.Arrow;
            var selecteditem = (ToolStripMenuItem)sender;
            selecteditem.Font = new Font(selecteditem.Font, FontStyle.Regular);
        }

        private void SaveToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Hand;
            var selecteditem = (ToolStripMenuItem)sender;
            selecteditem.Font = new Font(selecteditem.Font, FontStyle.Bold);
        }

        private void SaveToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Arrow;
            this.Cursor = Cursors.Arrow;
            var selecteditem = (ToolStripMenuItem)sender;
            selecteditem.Font = new Font(selecteditem.Font, FontStyle.Regular);
        }

        private void CleanLibraryToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Hand;
            var selecteditem = (ToolStripMenuItem)sender;
            selecteditem.Font = new Font(selecteditem.Font, FontStyle.Bold);
        }

        private void CleanLibraryToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Hand;
            this.Cursor = Cursors.Arrow;
            var selecteditem = (ToolStripMenuItem)sender;
            selecteditem.Font = new Font(selecteditem.Font, FontStyle.Regular);
        }

        private void PanelImages_DragEnter(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            panelImages.BackColor = Color.FromArgb(35, 32, 39);
            this.Cursor = Cursors.WaitCursor;
            this.ToolbarProgressBar.Value = 0;
            this.ToolbarProgressBar.Visible = true;
            int count = 1;
            foreach (string path in files)
            {
                string name = Path.GetFileNameWithoutExtension(path);
                Image returningImage = new Image(path, new List<Label>(), -1);
                returningImage.Name = name;
                library.AddImage(returningImage);
                this.ToolbarProgressBar.Increment((count * 100) / files.Length);
            }
            this.ToolbarProgressBar.Visible = false;
            this.ToolbarProgressBar.Value = 0;
            ReLoadPanelImage(sender, e);
            Saved = false;
            this.Cursor = Cursors.Arrow;
        }

        private void PanelImages_DragLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Arrow;
            panelImages.BackColor = Color.FromArgb(11, 7, 17);
        }

        private void OpenRightPanelButton_Click(object sender, EventArgs e)
        {
            string arrowiconslocation = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + @"\logos\";
            if (RightPanel.Visible == false)
            {
                RightPanel.Visible = true;
                this.Size = formsizewithrightpanel;
                Bitmap leftarrow = (Bitmap)Bitmap.FromFile(arrowiconslocation + "leftarrow.png");
                OpenRightPanelButton.BackgroundImage = leftarrow;

            }
            else
            {
                RightPanel.Visible = false;
                this.Size = formsizewithoutrightpanel;
                Bitmap rightarrow = (Bitmap)Bitmap.FromFile(arrowiconslocation + "rightarrow.png");
                OpenRightPanelButton.BackgroundImage = rightarrow;
            }
            OpenRightPanelButton.BackgroundImageLayout = ImageLayout.Zoom;
        }

        private void ExitCollageButton_Click(object sender, EventArgs e)
        {
            panelCollage.Visible = false;
        }

        private void ExitResizeButton_Click(object sender, EventArgs e)
        {
            panelResize.Visible = false;
        }

        private void SlideShowButton_Click(object sender, EventArgs e)
        {
            if (featuresImage.Count > 0)
            {
                SlideNPresentation slide = new SlideNPresentation(featuresImage, true);
                slide.ShowDialog();
            }
            else
            {
                MessageBox.Show("There has to be at least one picture selected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PresentationButton_Click(object sender, EventArgs e)
        {
            if (featuresImage.Count > 0)
            {
                SlideNPresentation slide = new SlideNPresentation(featuresImage, false);
                slide.ShowDialog();
            }
            else
            {
                MessageBox.Show("There has to be at least one picture selected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ExitToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Hand;
            var selecteditem = (ToolStripMenuItem)sender;
            selecteditem.Font = new Font(selecteditem.Font, FontStyle.Bold);
        }

        private void ExitToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Arrow;
            var selecteditem = (ToolStripMenuItem)sender;
            selecteditem.Font = new Font(selecteditem.Font, FontStyle.Regular);
        }

        private void MyAccountToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Hand;
            var selecteditem = (ToolStripMenuItem)sender;
            selecteditem.Font = new Font(selecteditem.Font, FontStyle.Bold);
        }

        private void MyAccountToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Arrow;
            var selecteditem = (ToolStripMenuItem)sender;
            selecteditem.Font = new Font(selecteditem.Font, FontStyle.Regular);
        }

        private void MyAccountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (AccountPanel.Visible == false)
            {
                AccountPanel.Visible = true;
                panelImages.Visible = false;
                AddLabelPanel.Visible = true;
                UsernameLabel.Text = userLoggedIn.Usrname;
                UserPicturePictureBox.BackgroundImage = userLoggedIn.UsrImage;
                this.MemberSinceLabel.Text = "Member since " + userLoggedIn.Membersince.Date.ToString("MM/dd/yyyy");
                if (UserLoggedIn.Name != null && UserLoggedIn.Name != "") this.RealNameTextBox.Text = UserLoggedIn.Name;
                if (UserLoggedIn.Surname != null && UserLoggedIn.Surname != "") this.RealSurnameTextBox.Text = UserLoggedIn.Surname;
                if (UserLoggedIn.Nationality != ENationality.None) this.RealNationalityComboBox.SelectedItem = UserLoggedIn.Nationality;
                if (UserLoggedIn.Description != null && UserLoggedIn.Description != "") this.DescriptionTextBox.Text = UserLoggedIn.Description;
                if (UserLoggedIn.BirthDate != new DateTime(1, 1, 1, 0, 0, 0)) this.UserDateTimePicker.Value = UserLoggedIn.BirthDate;
                RealNationalityComboBox.DataSource = Enum.GetValues(typeof(ENationality));
            }
            else
            {
                AccountPanel.Visible = false;
                panelImages.Visible = true;
                AddLabelPanel.Visible = false;
            }
        }

        private void UserPicturePictureBox_MouseEnter(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Hand;
            UserPicturePictureBox.Image = null;
            UserPicturePictureBox.BackgroundImage = chooseUserPictureBitmap;
        }

        private void UserPicturePictureBox_MouseLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Arrow;
            UserPicturePictureBox.BackgroundImage = UserLoggedIn.UsrImage;
        }

        private void UserPicturePictureBox_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select your profile picture";
            ofd.Multiselect = false;
            ofd.Filter = "Supported formats |*.jpg;*.jpeg;*.png;*.bmp";
            DialogResult dr = ofd.ShowDialog();
            if (dr == DialogResult.OK)
            {
                Resizer res = new Resizer();
                UserLoggedIn.UsrImage = res.ResizeImage((Bitmap)Bitmap.FromFile(ofd.FileNames[0]), 100, 100);
                UserPicturePictureBox.BackgroundImage = UserLoggedIn.UsrImage;
            }
        }

        private void LogOutButton_Click(object sender, EventArgs e)
        {
            this.exit = false;
            this.UserLoggedIn.CurrentUser = false;
            this.Close();
        }

        private void RealNameTextBox_TextChanged(object sender, EventArgs e)
        {
            this.UserLoggedIn.Name = RealNameTextBox.Text;
        }

        private void RealSurnameTextBox_TextChanged(object sender, EventArgs e)
        {
            this.UserLoggedIn.Surname = RealSurnameTextBox.Text;
        }

        private void RealNationalityComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.UserLoggedIn.Nationality = (ENationality)RealNationalityComboBox.SelectedItem;
        }

        private void DescriptionTextBox_TextChanged(object sender, EventArgs e)
        {
            this.UserLoggedIn.Description = DescriptionTextBox.Text;
        }

        private void UserDateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            this.UserLoggedIn.BirthDate = UserDateTimePicker.Value.Date;
        }

        private void ChangePassButton_Click(object sender, EventArgs e)
        {
            if (ChangePasswordPanel.Visible == false) ChangePasswordPanel.Visible = true;
            else ChangePasswordPanel.Visible = false;
        }

        private void OldPasswordTextBox_MouseEnter(object sender, EventArgs e)
        {
            if (OldPasswordTextBox.Text == "old password")
            {
                OldPasswordTextBox.Text = "";
                OldPasswordTextBox.ForeColor = Color.White;
            }
        }

        private void OldPasswordTextBox_MouseLeave(object sender, EventArgs e)
        {
            if (OldPasswordTextBox.Text == "")
            {
                OldPasswordTextBox.Text = "old password";
                OldPasswordTextBox.ForeColor = Color.Silver;
            }
        }

        private void NewPasswordTextBox_MouseEnter(object sender, EventArgs e)
        {
            if (NewPasswordTextBox.Text == "new password")
            {
                NewPasswordTextBox.Text = "";
                NewPasswordTextBox.ForeColor = Color.White;
            }
        }

        private void NewPasswordTextBox_MouseLeave(object sender, EventArgs e)
        {
            if (NewPasswordTextBox.Text == "")
            {
                NewPasswordTextBox.Text = "new password";
                NewPasswordTextBox.ForeColor = Color.Silver;
            }
        }

        private void OldPasswordTextBox_TextChanged(object sender, EventArgs e)
        {
            if (OldPasswordTextBox.Text != userLoggedIn.Password && OldPasswordTextBox.Text != "OLD PASSWORD" && OldPasswordTextBox.Text != "") WrongOldPassword.Visible = true;
            else WrongOldPassword.Visible = false;
        }

        private void Button11_Click(object sender, EventArgs e)
        {
            userLoggedIn.Password = NewPasswordTextBox.Text;
            OldPasswordTextBox.Text = "OLD PASSWORD";
            OldPasswordTextBox.ForeColor = Color.DarkGray;
            NewPasswordTextBox.Text = "NEW PASSWORD";
            NewPasswordTextBox.ForeColor = Color.DarkGray;
            ChangePasswordPanel.Visible = false;
        }

        private void DeleteAccountButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete your account?", "Delete account", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {
                this.exit = false;
                this.UserLoggedIn.CurrentUser = false;
                this.Deleteaccount = true;
                this.Close();
            }
        }

        private void SearchTextBox_Enter(object sender, EventArgs e)
        {
            if (SearchTextBox.Text == "SEARCH PATTERN")
            {
                SearchTextBox.Text = "";
                SearchTextBox.ForeColor = Color.White;
            }
        }

        private void SearchTextBox_Leave(object sender, EventArgs e)
        {
            if (SearchTextBox.Text == "")
            {
                SearchTextBox.Text = "SEARCH PATTERN";
                SearchTextBox.ForeColor = Color.DarkGray;
            }
        }

        private void SearchTextBox_TextChanged(object sender, EventArgs e)
        {
            this.panelImages.Controls.Clear();
            this.ValidNotValidPatternLabel.Text = "";
            try
            {
                if (SearchTextBox.Text != "")
                {
                    List<Image> result = mainSearcher.Search(library.Images, SearchTextBox.Text);
                    if (result.Count != 0)
                    {
                        PanelImages_PaintSearchResult(result);
                        ValidNotValidPatternLabel.Text = "Results";
                        ValidNotValidPatternLabel.ForeColor = Color.FromArgb(34, 160, 182);
                    }
                    else
                    {
                        ValidNotValidPatternLabel.Text = "No results";
                        ValidNotValidPatternLabel.ForeColor = Color.FromArgb(34,160,182);
                    }
                }
                else
                {
                    ReLoadPanelImage(sender, e);
                }
            }
            catch
            {
                ValidNotValidPatternLabel.Text = "Not valid pattern";
                ReLoadPanelImage(this, EventArgs.Empty);
                ValidNotValidPatternLabel.ForeColor = Color.FromArgb(203, 12, 89);
                return;
            }
        }

        private void TextBoxNumberOnly(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void NameTextBox_Enter(object sender, EventArgs e)
        {
            if (nameTextBox.Text == "NEW NAME")
            {
                nameTextBox.Text = "";
                nameTextBox.ForeColor = Color.White;
            }
        }

        private void NameTextBox_Leave(object sender, EventArgs e)
        {
            if (nameTextBox.Text == "")
            {
                nameTextBox.Text = "NEW NAME";
                nameTextBox.ForeColor = Color.DarkGray;
            }
        }

        private void GoBackButton_Click(object sender, EventArgs e)
        {
            MyAccountToolStripMenuItem_Click(this, EventArgs.Empty);
        }

        private void MosaicButton_Click(object sender, EventArgs e)
        {
            try
            {
                int width = Convert.ToInt32(InsertWidthText.Text);
                int height = Convert.ToInt32(InsertHeightText.Text);
				int BWidth = Convert.ToInt32(BaseWMosaicText.Text);
				int BHeight = Convert.ToInt32(BaseHMosaicText.Text);
				if (chosenEditingImage != null)
                {
                    Bitmap mosaic = PM.producer.Mosaic((Image)chosenEditingImage.Tag, featuresImage, width, height, BWidth, BHeight, feauturesProgressBar);
                    Image mosaicImage = new Image(mosaic, new List<Label>(), -1);
                    producer.LoadImagesToWorkingArea(new List<Image>() { mosaicImage });
                    EditingPanel_Paint(sender, e);
                    pictureChosen.Image = mosaic;
                    InsertWidthText.Text = "";
                    InsertHeightText.Text = "";
                    MosaicPanel.Visible = false;
                }
                else MessageBox.Show("Select a base Image", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch
            {
                MessageBox.Show("Wrong parameters", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExitMosaic_Click(object sender, EventArgs e)
        {
            MosaicPanel.Visible = false;
        }

        private void Button12_Click(object sender, EventArgs e)
        {
            string arrowiconslocation = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + @"\logos\";
            if (InfoSettingPanel.Visible == true)
            {
                InfoSettingPanel.Visible = false;
                ImageInfoPanel.Dock = DockStyle.Fill;
                Bitmap uparrow = (Bitmap)Bitmap.FromFile(arrowiconslocation + "uparrow.png");
                CollapseInfoPanelButton.BackgroundImage = uparrow;
            }
            else
            {
                InfoSettingPanel.Visible = true;
                ImageInfoPanel.Dock = DockStyle.Top;
                Bitmap downarrow = (Bitmap)Bitmap.FromFile(arrowiconslocation + "downarrow.png");
                CollapseInfoPanelButton.BackgroundImage = downarrow;
            }
            CollapseInfoPanelButton.BackgroundImageLayout = ImageLayout.Zoom;
        }

        private void DeleteLabelButton_Click(object sender, EventArgs e)
        {
            if (InfoTreeView.SelectedNode != null)
            {
                string selectedNode = InfoTreeView.SelectedNode.Text;
                string[] splitted = selectedNode.Split(' ');
                List<string> splittedlist = new List<string>() { splitted[0], splitted[1] };
                try
                {
                    int labelnumber = Convert.ToInt32(splitted[1]);
                    if (splittedlist[0] != "Simple" && splittedlist[0] != "Person" && splittedlist[0] != "Special") throw new Exception();
                    switch (splitted[0])
                    {
                        case "Simple":
                            InfoTreeView.Nodes[1].Nodes[0].Nodes.RemoveAt(labelnumber - 1);
                            imagetoaddlabel.RemoveLabel("SimpleLabel", labelnumber - 1);
                            break;
                        case "Person":
                            InfoTreeView.Nodes[1].Nodes[1].Nodes.RemoveAt(labelnumber - 1);
                            imagetoaddlabel.RemoveLabel("PersonLabel", labelnumber - 1);
                            break;
                        case "Special":
                            InfoTreeView.Nodes[1].Nodes[2].Nodes.RemoveAt(labelnumber - 1);
                            imagetoaddlabel.RemoveLabel("SpecialLabel", labelnumber - 1);
                            break;
                    }
                    RefreshInfoTreeView();
                    PM.SavingUsersLibraryManager(UserLoggedIn.Usrname, library);
                    SmartList_Click(this, EventArgs.Empty);
                }
                catch
                {
                    MessageBox.Show("Didn't select a Label to delete", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ImportWithLabelsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            List<Image> importedImages = new List<Image>();
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select the desired images";
            ofd.Multiselect = true;
            ofd.Filter = "Supported formats |*.jpg;*.jpeg;*.png;*.bmp";
            DialogResult dr = ofd.ShowDialog();
            if (dr == DialogResult.OK)
            {
				this.ToolbarProgressBar.Step = 1;
				this.ToolbarProgressBar.Value = 0;
                this.ToolbarProgressBar.Visible = true;
                this.Cursor = Cursors.WaitCursor;
                string[] files = ofd.FileNames;
				this.ToolbarProgressBar.Maximum = files.Length;
				foreach (string path in files)
                {
                    string name = Path.GetFileNameWithoutExtension(path);
                    Image returningImage = new Image(path, new List<Label>(), -1);
                    returningImage.Name = name;
                    library.AddImage(returningImage);
                    importedImages.Add(returningImage);
					this.ToolbarProgressBar.PerformStep();
				}
                this.ToolbarProgressBar.Visible = false;
                this.ToolbarProgressBar.Value = 0;
                ReLoadPanelImage(sender, e);
                SmartList_Click(this, EventArgs.Empty);
                Saved = false;
                this.Cursor = Cursors.Arrow;
            }
            Bitmap baseimage = new Bitmap(MultipleImagesPictureBox.Width, MultipleImagesPictureBox.Height);
            if (importedImages.Count == 1) baseimage = (Bitmap)importedImages[0].BitmapImage.Clone();
            else baseimage = (Bitmap)Bitmap.FromFile(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + @"\logos\previewnotavailable2.jpg");
            AccountPanel.Visible = true;
            panelImages.Visible = false;
            AddLabelPanel.Visible = true;
            MultipleAddLabelPanel.Visible = true;
            menuStrip1.Enabled = false;
            MultipleImagesPictureBox.Image = baseimage;
            MultipleImagesPictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            AddLabelController(importedImages, baseimage);
            this.imagestoaddlabel = importedImages;
        }

        private void AddLabelController(List<Image> importedimages, Bitmap baseImage)
        {
            this.comboBox2.DataSource = Enum.GetValues(typeof(ENationality));
            this.comboBox3.DataSource = Enum.GetValues(typeof(EColor));
            this.comboBox4.DataSource = Enum.GetValues(typeof(EColor));
            this.comboBox5.DataSource = Enum.GetValues(typeof(ESex));
            this.checkBox2.Checked = true;
            this.comboBox1.SelectedIndex = 0;
            this.importToolStripMenuItem1.Enabled = false;
            this.exportToolStripMenuItem.Enabled = false;
            this.saveToolStripMenuItem.Enabled = false;
            this.myAccountToolStripMenuItem.Enabled = false;
            this.exitToolStripMenuItem.Enabled = false;
            this.cleanLibraryToolStripMenuItem.Enabled = false;
        }

        private void MultipleImagesLabelComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox combo = (ComboBox)sender;
            switch (combo.SelectedIndex)
            {
                case 0:
                    panel2.Visible = true;
                    panel1.Visible = false;
                    panel3.Visible = false;
                    break;
                case 1:
                    panel3.Visible = true;
                    panel1.Visible = false;
                    panel2.Visible = false;
                    break;
                case 2:
                    panel1.Visible = true;
                    panel2.Visible = false;
                    panel3.Visible = false;
                    break;
            }
        }

        private void Button17_Click(object sender, EventArgs e)
        {
            switch (this.MultipleImagesLabelComboBox.SelectedIndex)
            {
                case 0:
                    if (this.checkBox2.Checked)
                    {
                        if (this.textBox6.Text == "")
                        {
                            MessageBox.Show("You can't add an empty tag", "Add label error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {

                            this.createdLabel = new SimpleLabel(this.textBox6.Text);
                            foreach (Image image in imagestoaddlabel)
                            {
                                image.AddLabel(createdLabel);
                            }
                        }
                    }
                    else
                    {
                        if (Convert.ToString(this.comboBox6.SelectedItem) == "")
                        {
                            MessageBox.Show("You cant add an empty tag", "Add label error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            this.createdLabel = new SimpleLabel(Convert.ToString(this.comboBox6.SelectedItem));
                            foreach (Image image in imagestoaddlabel)
                            {
                                image.AddLabel(createdLabel);
                            }
                        }
                    }
                    break;
                case 1:
                    this.createdLabel = new PersonLabel(this.textBox1.Text == "" ? null : this.textBox1.Text,
                    (label9.Text == "0" && label10.Text == "0" && label8.Text == "0" && label7.Text == "0") ? null : new double[] { Convert.ToInt32(label9.Text), Convert.ToInt32(label10.Text), Convert.ToInt32(label8.Text), Convert.ToInt32(label7.Text) },
                    this.textBox2.Text == "" ? null : this.textBox2.Text, (ENationality)this.comboBox2.SelectedItem,
                    (EColor)this.comboBox4.SelectedItem, (EColor)this.comboBox3.SelectedItem, (ESex)this.comboBox5.SelectedItem,
                    this.dateTimePicker1.Value.Date.ToString() == "01-01-1930 0:00:00" ? "" : this.dateTimePicker1.Value.Date.ToString());
                    foreach (Image image in imagestoaddlabel)
                    {
                        image.AddLabel(createdLabel);
                    }
                    break;
                case 2:
                    this.createdLabel = new SpecialLabel((this.numericUpDown2.Value == 0 && this.numericUpDown1.Value == 0) ? null : new double[] { Convert.ToDouble(this.numericUpDown2.Value), Convert.ToDouble(this.numericUpDown1.Value) },
                        (this.textBox5.Text == "") ? null : this.textBox5.Text, (this.textBox4.Text == "") ? null : this.textBox4.Text,
                        (this.textBox3.Text == "") ? null : this.textBox3.Text, (this.comboBox1.SelectedItem.ToString() == "Si") ? true : false);
                    foreach (Image image in imagestoaddlabel)
                    {
                        image.AddLabel(createdLabel);
                    }
                    break;
            }
        }

        private void Button16_Click(object sender, EventArgs e)
        {
            if (createdLabel == null)
            {
                if (MessageBox.Show("You didn't create any new Label. Do you want to exit?", "Warning!",
                       MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    MultipleAddLabelPanel.Visible = false;
                }
            }
            AccountPanel.Visible = false;
            panelImages.Visible = true;
            AddLabelPanel.Visible = false;
            MultipleAddLabelPanel.Visible = false;
            this.importToolStripMenuItem1.Enabled = true;
            this.exportToolStripMenuItem.Enabled = true;
            this.saveToolStripMenuItem.Enabled = true;
            this.cleanLibraryToolStripMenuItem.Enabled = true;
            this.myAccountToolStripMenuItem.Enabled = true;
            this.exitToolStripMenuItem.Enabled = true;
            this.SearchTextBox.Enabled = true;
            menuStrip1.Enabled = true;
            SmartList_Click(this, EventArgs.Empty);
            ResetImportWithLabelsEntries();
        }

        private void ResetImportWithLabelsEntries()
        {
            numericUpDown1.Value = 0;
            numericUpDown2.Value = 0;
            comboBox1.SelectedItem = null;
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            textBox6.Text = "";
            textBox1.Text = "";
            textBox2.Text = "";
            comboBox5.SelectedItem = null;
            comboBox3.SelectedItem = null;
            comboBox2.SelectedItem = null;
            comboBox4.SelectedItem = null;
        }

        private void ComboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void CheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            checkBox2.Checked = true;
        }

        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void ContrastBar_MouseUp(object sender, MouseEventArgs e)
        {
            if (chosenEditingImage != null)
            {
                Image image = (Image)chosenEditingImage.Tag;
                image.BitmapImage = producer.Contrast(image.BitmapImage, Convert.ToDouble(ContrastBar.Value));
                SaveFilterApplyed(EFilter.Contrast, image);
                chosenEditingImage.Image = image.BitmapImage;
                pictureChosen.Image = chosenEditingImage.Image;
            }
        }

        private void BrightnessBar_MouseUp(object sender, MouseEventArgs e)
        {
            if (chosenEditingImage != null)
            {
                Image image = (Image)chosenEditingImage.Tag;
                image.BitmapImage = producer.ApplyFilter((Image)chosenEditingImage.Tag, EFilter.Brightness, Color.Empty, brightnessBar.Value);
                SaveFilterApplyed(EFilter.Brightness, image);
                chosenEditingImage.Image = image.BitmapImage;
                pictureChosen.Image = chosenEditingImage.Image;
            }
        }

        private void GoBackButton_MouseEnter(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            btn.Font = new Font(btn.Font, FontStyle.Bold);
        }

        private void GoBackButton_MouseLeave(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            btn.Font = new Font(btn.Font, FontStyle.Regular);
        }

		private void CustomFilter_Click(object sender, EventArgs e)
		{
			if (chosenEditingImage != null)
			{
				if (colorDialogFilter.ShowDialog() == DialogResult.OK)
				{
					Image image = (Image)chosenEditingImage.Tag;
					Bitmap bmap = (Bitmap)image.BitmapImage.Clone();
					BrightnessFilter BF = new BrightnessFilter();
					BlackNWhiteFilter BW = new BlackNWhiteFilter();
					SepiaFilter SF = new SepiaFilter();
					AutomaticAdjustmentFilter auto = new AutomaticAdjustmentFilter();
					ColorFilter CF = new ColorFilter();
					bmap = auto.SetContrast(100, bmap);
					bmap = BF.ApplyFilter(bmap, 15);
					bmap = auto.SetContrast(100, bmap);
					bmap = auto.SetContrast(100, bmap);
					bmap = BF.ApplyFilter(bmap, 30);
					bmap = SF.ApplyFilter(bmap);
					bmap = BW.ApplyFilter(bmap);
					bmap = auto.SetContrast(100, bmap);
					bmap = auto.SetContrast(100, bmap);
					bmap = CF.ApplyFilter(bmap, colorDialogFilter.Color);
					image.BitmapImage = bmap;
                    GC.Collect();
					SaveFilterApplyed(EFilter.Burned, image); 
					chosenEditingImage.Image = image.BitmapImage;
					pictureChosen.Image = chosenEditingImage.Image;
				}
			}
		}

        private void Topauxlabel_Click(object sender, EventArgs e)
        {

        }

		private void CropButton_Click(object sender, EventArgs e)
		{
			if (chosenEditingImage != null)
			{
				Scissors scissors = new Scissors();
				Image image = (Image)chosenEditingImage.Tag;
				SelectFaceLocationForm newForm = new SelectFaceLocationForm();
				newForm.ActualImage = image.BitmapImage;
                newForm.Text = "Crop";
				newForm.ShowDialog();
				int newLeft = newForm.ReturningLeft;
				int newTop = newForm.ReturningTop;
				int newHeight = newForm.ReturningHeight;
				int newWidth = newForm.ReturningWidth;
				// x, y, width, height
				if (newForm.Exit)
				{
					double[] coordinates = { Convert.ToDouble(newLeft), Convert.ToDouble(newTop), Convert.ToDouble(newWidth), Convert.ToDouble(newHeight) };
					image.BitmapImage = scissors.Crop(image.BitmapImage, coordinates);
                    image.ResetFaceLocation();
					chosenEditingImage.Image = image.BitmapImage;
					pictureChosen.Image = chosenEditingImage.Image;
				}
			}
		}

		private void PaintButton_Click(object sender, EventArgs e)
		{
            if (chosenEditingImage != null)
            {
                Image image = (Image)chosenEditingImage.Tag;
                Paint form = new Paint();
                form.ActualImage = image.BitmapImage;
                form.ShowDialog();
                int x = form.X;
                int y = form.Y;
                Color MainColor = form.MainColor;
                if (form.Exit)
                {
                    image.BitmapImage = form.ActualImage;
                    chosenEditingImage.Image = form.ActualImage;
                    pictureChosen.Image = chosenEditingImage.Image;
                }
            }
            else
            {
                Paint form = new Paint();
                Bitmap bmap = new Bitmap(1920, 1080);
                using (Graphics graph = Graphics.FromImage(bmap))
                {
                    Rectangle ImageSize = new Rectangle(0, 0, 1920, 1080);
                    graph.FillRectangle(Brushes.White, ImageSize);
                }
                form.ActualImage = bmap;
                form.ShowDialog();
                int x = form.X;
                int y = form.Y;
                Color MainColor = form.MainColor;
                if (form.Exit)
                {
                    bmap = form.ActualImage;
                    Image newImage = new Image(bmap, new List<Label>(), -1);
                    producer.LoadImagesToWorkingArea(new List<Image>() { newImage });
                    EditingPanel_Paint(sender, e);
                }
            }
        }

        private void Button9_MouseEnter(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            btn.ForeColor = Color.White;
        }

        private void Button9_MouseLeave(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            btn.ForeColor = Color.Black;
        }

        private void ShowFeatureListButton_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            if (FeatureListPanel.Visible == false)
            {
                FeatureListPanel.Visible = true;
                Refresh_FeatureListPanel();
                btn.Text = "Editing area";
                label34.Text = "Feature list images";
            }
            else
            {
                FeatureListPanel.Visible = false;
                btn.Text = "Feature list";
                label34.Text = "Editing area images";
            }
        }

        private void Refresh_FeatureListPanel()
        {
            FeatureListPanel.Controls.Clear();
            Paint_FeatureList(this, EventArgs.Empty);
        }

        private void Paint_FeatureList(object sender, EventArgs e)
        {
            int x = 20;
            int y = 20;
            int maxHeight = -1;
            foreach (Image image in featuresImage)
            {
                PictureBox pic = new PictureBox();
                pic.Image = NewThumbnailImage(image.BitmapImage);
                pic.Location = new Point(x, y);
                pic.SizeMode = PictureBoxSizeMode.Zoom;
                pic.Click += ImageBorderClick;
                pic.Name = image.Name;
                pic.ContextMenuStrip = contextMenuStripFeatures;
                pic.Cursor = Cursors.Hand;
                pic.Tag = image;
                x += pic.Width + 10;
                maxHeight = Math.Max(pic.Height, maxHeight);
                if (x > this.FeatureListPanel.Width - 100)
                {
                    x = 20;
                    y += maxHeight + 10;
                }
                this.FeatureListPanel.Controls.Add(pic);
            }
        }

        private void ClearEditingAreaButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete all the images from the editing area?", "Warning!",
                                   MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                foreach (Control p in topauxlabel.Controls)
                {
                    if (p is PictureBox)
                    {
                        Image im = (Image)p.Tag;
                        PM.producer.RemoveImage(im);
                        //Saved = false;
                        if (p == chosenEditingImage)
                        {
                            pictureChosen.SizeMode = PictureBoxSizeMode.CenterImage;
                            pictureChosen.Image = pictureChosen.ErrorImage;
                            chosenEditingImage = null;
                        }
                    }
                }
                ReLoadEditingPanelImage(this, EventArgs.Empty);
            }
        }

        private void Button19_Click(object sender, EventArgs e)
        {
            featuresImage.Clear();
            Refresh_FeatureListPanel();
        }

        private void ExportAllToFeature_Click(object sender, EventArgs e)
        {
            if (library.Images.Count != 0)
            {
                foreach (Image im in PM.producer.imagesInTheWorkingArea())
                {
                    featuresImage.Add(im);
                    Refresh_FeatureListPanel();
                }
            }
        }

        private void SmartList_Click(object sender, EventArgs e) 
        {
            string imageLocation = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + @"\icons\listicon.png";
            ImageSmartPanel.Controls.Clear();
            library.UpdateSmartList(library.Images);
            foreach (KeyValuePair<string, List<Image>> val in library.SmartList)
            {
                ListSmartDelete.Items.Add(val.Key);
            }
            if (ListSmartDelete.Items.Count > 0)
            {
                Dictionary<string, List<Image>> dicc = library.SmartList;
                int altura = 40;
                MuestraSmartPanel.Controls.Clear();
                ListSmartDelete.Items.Clear();
                foreach (KeyValuePair<string, List<Image>> nuevo in dicc)
                {
                    Button button = new Button();
                    button.Name = nuevo.Key;
                    button.Text = nuevo.Key;
                    button.ForeColor = Color.White;
                    button.FlatStyle = FlatStyle.Flat;
                    button.Dock = DockStyle.Top;
                    button.Click += OnSmartButtonClick;
                    button.Cursor = Cursors.Hand;
                    button.Click += new EventHandler((sender1, e1) => OnImagenes(sender1, e1, nuevo.Value));
                    button.Location = new System.Drawing.Point(12, altura);
                    MuestraSmartPanel.Controls.Add(button);
                    ListSmartDelete.Items.Add(nuevo.Key);
                    altura += 40;
                }
            }
            else
            {
                MuestraSmartPanel.Controls.Clear();
                System.Windows.Forms.Label lb = new System.Windows.Forms.Label();
                lb.Text = "\n\n\nYou don't have any Smart Lists yet";
                lb.Font = new Font("Consolas", 15F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
                lb.ForeColor = Color.White;
                lb.BackColor = Color.Transparent;
                lb.Dock = DockStyle.Fill;
                MuestraSmartPanel.Controls.Add(lb);
            }
        }

        private void OnSmartButtonClick(object sender, EventArgs e)
        {
            foreach (Control ctrl in MuestraSmartPanel.Controls)
            {
                if (ctrl is Button)
                {
                    Button btn1 = (Button)sender;
                    ctrl.BackColor = Color.FromArgb(35, 32, 39);
                    ctrl.ForeColor = Color.White;
                    btn1.FlatAppearance.BorderSize = 1;
                }
            }
            Button btn = (Button)sender;
            btn.BackColor = Color.LightGray;
            btn.ForeColor = Color.Black;
            btn.FlatAppearance.BorderSize = 0;
        }

        private void OnImagenes(object sender1, EventArgs e1, List<Image> seccion)  // Crea las funciones de cada smart list
        {
            ImageSmartPanel.BackColor = Color.LightGray;
            int lado = 20;
            Button button1 = (Button)sender1;
            ImageSmartPanel.Controls.Clear();
            foreach (Image pala in seccion)
            {

                System.Windows.Forms.Label label = new System.Windows.Forms.Label();
                PictureBox pictureBox = new PictureBox();
                pictureBox.Name = pala.Name;
                pictureBox.Tag = pala;
                pictureBox.Cursor = Cursors.Hand;
                pictureBox.Image = NewThumbnailImage((System.Drawing.Image)pala.BitmapImage);
                pictureBox.Size = new Size(150, 100);
                pictureBox.Location = new Point(lado, 12);
                pictureBox.Click += ImageBorderClick;
                pictureBox.Click += OnSmartListClick;
                if (pala.Name.Length < 5) label.Text = pala.Name;
                else
                {
                    string text = "";
                    int count = 0;
                    while (count < 5)
                    {
                        text += pala.Name[count];
                        count++;
                    }
                    text += "...";
                    label.Text = text;
                }
                label.Location = new Point(lado + 50, 120);
                label.ForeColor = Color.Black;
                label.Font = new Font("Consolas", 12F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
                pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                ImageSmartPanel.Controls.Add(pictureBox);
                ImageSmartPanel.Controls.Add(label);
                lado += 160;
            }
        }

        private void OnSmartListClick(object sender, EventArgs e)
        {
            this.selectedSmartBox = (PictureBox)sender;
            Image selectedImg = (Image)selectedSmartBox.Tag;
            PanelImages_Click(this, EventArgs.Empty);
            foreach (Control ctrl in panelImages.Controls)
            {
                if (ctrl is PictureBox)
                {
                    PictureBox PIC = (PictureBox)ctrl;
                    PIC.BackColor = Color.Transparent;
                    PIC.BorderStyle = BorderStyle.None;
                }
            }
            foreach (Control ctrl in panelImages.Controls)
            {
                if (ctrl is PictureBox)
                {
                    if (ctrl.Tag == selectedImg)
                    {
                        ImageBorderClick(ctrl, EventArgs.Empty);
                        ImageDetailClick(ctrl, EventArgs.Empty);
                    }
                }
            }
        }

        private void AddSmarListButton_Click(object sender, EventArgs e)  //Abrir el panel para crear  smart list
        {
            MuestraSmartPanel.Controls.Clear();
            ImageSmartPanel.Controls.Clear();
            SeacherPattern.Text = "";
            busqueda.Text = "Busqueda";
            OpcionesPanel.Visible = false;
            addSmart.Visible = true;
            ImageSmartPanel.Controls.Add(addSmart);
        }

        private void DeleteSmartButton_Click(object sender, EventArgs e)  //muestra el panel de las smart list para eliminar
        {
            SmartList_Click(this, EventArgs.Empty);
            MuestraSmartPanel.Controls.Clear();
            ImageSmartPanel.Controls.Clear();
            DeletePanel.Visible = true;
            ImageSmartPanel.Controls.Add(DeletePanel);
        }

        private void SeacherPattern_TextChanged(object sender, EventArgs e)  //srive cuando se elimina algo
        {
            pattern = new StringBuilder(SeacherPattern.Text);
        }

        private void DeleteButtom_Click(object sender, EventArgs e)  //elimina la smart list
        {
            try
            {
                string palabra = ListSmartDelete.SelectedItem.ToString();
                library.RemoveSmartList(palabra);
                DeletePanel.Visible = false;
            }
            catch
            {
                return;
            }
        }

        private void Atras1_Click(object sender, EventArgs e)  //cierra la ventana
        {
            DeletePanel.Visible = false;
        }

        private void Busqueda_SelectedIndexChanged(object sender, EventArgs e)
        {
            OpcionesPanel.Visible = true;
            if (busqueda.SelectedItem.ToString() == "Sentence:" || busqueda.SelectedItem.ToString() == "Name:" || busqueda.SelectedItem.ToString() == "Surname:" || busqueda.SelectedItem.ToString() == "Address:" || busqueda.SelectedItem.ToString() == "Photographer:" || busqueda.SelectedItem.ToString() == "Photomotive:")
            {
                OpcionesPanel.Controls.Clear();
                SentenceBox.Text = "Sentence";
                SentenceBox.Visible = true;
                UnionComboBox.Visible = true;
                AddButton.Visible = true;
                OpcionesPanel.Controls.Add(SentenceBox);
            }
            else if (busqueda.SelectedItem.ToString() == "GeographicLocation:")
            {
                OpcionesPanel.Controls.Clear();
                Longitud.ResetText();
                Latitud.ResetText();
                Longitud.Visible = true;
                Latitud.Visible = true;
                UnionComboBox.Visible = true;
                AddButton.Visible = true;
                OpcionesPanel.Controls.Add(Longitud);
                OpcionesPanel.Controls.Add(Latitud);
            }
            else if (busqueda.SelectedItem.ToString() == "Selfie:")
            {
                OpcionesPanel.Controls.Clear();
                YesNo.ResetText();
                YesNo.Visible = true;
                UnionComboBox.Visible = true;
                AddButton.Visible = true;
                OpcionesPanel.Controls.Add(YesNo);
            }
            else if (busqueda.SelectedItem.ToString() == "Nationality:")
            {
                OpcionesPanel.Controls.Clear();
                NationalityComboBox.ResetText();
                NationalityComboBox.Visible = true;
                UnionComboBox.Visible = true;
                AddButton.Visible = true;
                OpcionesPanel.Controls.Add(NationalityComboBox);
            }
            else if (busqueda.SelectedItem.ToString() == "HairColor:" || busqueda.SelectedItem.ToString() == "EyesColor:")
            {
                OpcionesPanel.Controls.Clear();
                ColorComboBox.ResetText();
                ColorComboBox.Visible = true;
                UnionComboBox.Visible = true;
                AddButton.Visible = true;
                OpcionesPanel.Controls.Add(ColorComboBox);
            }
            else if (busqueda.SelectedItem.ToString() == "Sex:")
            {
                OpcionesPanel.Controls.Clear();
                SexComboBox.ResetText();
                SexComboBox.Visible = true;
                UnionComboBox.Visible = true;
                AddButton.Visible = true;
                OpcionesPanel.Controls.Add(SexComboBox);
            }
            else if (busqueda.SelectedItem.ToString() == "Birthdate:")
            {
                OpcionesPanel.Controls.Clear();
                BirthDate.ResetText();
                BirthDate.Visible = true;
                UnionComboBox.Visible = true;
                AddButton.Visible = true;
                OpcionesPanel.Controls.Add(BirthDate);
            }
            else if (busqueda.SelectedItem.ToString() == "Filter:")
            {
                OpcionesPanel.Controls.Clear();
                FiltroComboBox.ResetText();
                UnionComboBox.Visible = true;
                AddButton.Visible = true;
                FiltroComboBox.Visible = true;
                OpcionesPanel.Controls.Add(FiltroComboBox);
            }
            else if (busqueda.SelectedItem.ToString() == "Calification:")
            {
                OpcionesPanel.Controls.Clear();
                CalificationUp.ResetText();
                CalificationUp.Visible = true;
                AddButton.Visible = true;
                UnionComboBox.Visible = true;
                OpcionesPanel.Controls.Add(CalificationUp);
            }
            OpcionesPanel.Controls.Add(UnionComboBox);
            OpcionesPanel.Controls.Add(AddButton);
        }

        private void Agregar_Click(object sender, EventArgs e)  // agrega el patron de busqueda a las smart list
        {
            library.AddSmartList(pattern.ToString(), library.Images);
            addSmart.Visible = false;
            SeacherPattern.Text = "Seacher Pattern";
            pattern.Clear();
            busqueda.Text = "Busqueda";
        }

        private void Atras_Click(object sender, EventArgs e)  //vuelve  atras
        {
            addSmart.Visible = false;
            SeacherPattern.Text = "Seacher Pattern";
            pattern.Clear();
            busqueda.Text = "Busqueda";
        }

        private void AddSmartListButton_Click(object sender, EventArgs e)  //Agrega a un string el patron que se quiere buscar
        {
            StringBuilder patron = new System.Text.StringBuilder();
            while (true)
            {
                string parametro = busqueda.SelectedItem.ToString();
                switch (parametro)
                {
                    case "Sentence:":
                        patron.Append(busqueda.SelectedItem + SentenceBox.Text);
                        if (UnionComboBox.SelectedItem.ToString() == "and")
                        {
                            patron.Append(" and ");
                        }
                        else if (UnionComboBox.SelectedItem.ToString() == "or")
                        {
                            patron.Append(" or ");
                        }
                        break;
                    case "Name:":
                        patron.Append(busqueda.SelectedItem + SentenceBox.Text);
                        if (UnionComboBox.SelectedItem.ToString() == "and")
                        {
                            patron.Append(" and ");
                        }
                        else if (UnionComboBox.SelectedItem.ToString() == "or")
                        {
                            patron.Append(" or ");
                        }
                        break;
                    case "Surname:":
                        patron.Append(busqueda.SelectedItem + SentenceBox.Text);
                        if (UnionComboBox.SelectedItem.ToString() == "and")
                        {
                            patron.Append(" and ");
                        }
                        else if (UnionComboBox.SelectedItem.ToString() == "or")
                        {
                            patron.Append(" or ");
                        }
                        break;
                    case "Filter:":
                        patron.Append(FiltroComboBox.SelectedItem.ToString()+":True");
                        if (UnionComboBox.SelectedItem.ToString() == "and")
                        {
                            patron.Append(" and ");
                        }
                        else if (UnionComboBox.SelectedItem.ToString() == "or")
                        {
                            patron.Append(" or ");
                        }
                        break;
                    case "Birthdate:":
                        patron.Append(busqueda.SelectedItem + BirthDate.Value.ToString());
                        if (UnionComboBox.SelectedItem.ToString() == "and")
                        {
                            patron.Append(" and ");
                        }
                        else if (UnionComboBox.SelectedItem.ToString() == "or")
                        {
                            patron.Append(" or ");
                        }
                        break;
                    case "Sex:":
                        patron.Append(busqueda.SelectedItem + SexComboBox.SelectedItem.ToString());
                        if (UnionComboBox.SelectedItem.ToString() == "and")
                        {
                            patron.Append(" and ");
                        }
                        else if (UnionComboBox.SelectedItem.ToString() == "or")
                        {
                            patron.Append(" or ");
                        }
                        break;
                    case "EyesColor:":
                        patron.Append(busqueda.SelectedItem + ColorComboBox.SelectedItem.ToString());
                        if (UnionComboBox.SelectedItem.ToString() == "and")
                        {
                            patron.Append(" and ");
                        }
                        else if (UnionComboBox.SelectedItem.ToString() == "or")
                        {
                            patron.Append(" or ");
                        }
                        break;
                    case "HairColor:":
                        patron.Append(busqueda.SelectedItem + ColorComboBox.SelectedItem.ToString());
                        if (UnionComboBox.SelectedItem.ToString() == "and")
                        {
                            patron.Append(" and ");
                        }
                        else if (UnionComboBox.SelectedItem.ToString() == "or")
                        {
                            patron.Append(" or ");
                        }
                        break;
                    case "Nationality:":
                        patron.Append(busqueda.SelectedItem + NationalityComboBox.SelectedItem.ToString());
                        if (UnionComboBox.SelectedItem.ToString() == "and")
                        {
                            patron.Append(" and ");
                        }
                        else if (UnionComboBox.SelectedItem.ToString() == "or")
                        {
                            patron.Append(" or ");
                        }
                        break;
                    case "GeographicLocation:":
                        patron.Append(busqueda.SelectedItem + Latitud.Value.ToString() + "," + Longitud.Value.ToString());
                        if (UnionComboBox.SelectedItem.ToString() == "and")
                        {
                            patron.Append(" and ");
                        }
                        else if (UnionComboBox.SelectedItem.ToString() == "or")
                        {
                            patron.Append(" or ");
                        }
                        break;
                    case "Address:":
                        patron.Append(busqueda.SelectedItem + SentenceBox.Text);
                        if (UnionComboBox.SelectedItem.ToString() == "and")
                        {
                            patron.Append(" and ");
                        }
                        else if (UnionComboBox.SelectedItem.ToString() == "or")
                        {
                            patron.Append(" or ");
                        }
                        break;
                    case "Photographer:":
                        patron.Append(busqueda.SelectedItem + SentenceBox.Text);
                        if (UnionComboBox.SelectedItem.ToString() == "and")
                        {
                            patron.Append(" and ");
                        }
                        else if (UnionComboBox.SelectedItem.ToString() == "or")
                        {
                            patron.Append(" or ");
                        }
                        break;
                    case "Photomotive:":
                        patron.Append(busqueda.SelectedItem + SentenceBox.Text);
                        if (UnionComboBox.SelectedItem.ToString() == "and")
                        {
                            patron.Append(" and ");
                        }
                        else if (UnionComboBox.SelectedItem.ToString() == "or")
                        {
                            patron.Append(" or ");
                        }
                        break;
                    case "Selfie:":
                        patron.Append(busqueda.SelectedItem + YesNo.SelectedItem.ToString());
                        if (UnionComboBox.SelectedItem.ToString() == "and")
                        {
                            patron.Append(" and ");
                        }
                        else if (UnionComboBox.SelectedItem.ToString() == "or")
                        {
                            patron.Append(" or ");
                        }
                        break;
                    case "Calification:":
                        patron.Append(busqueda.SelectedItem + CalificationUp.Value.ToString());
                        if (UnionComboBox.SelectedItem.ToString() == "and")
                        {
                            patron.Append(" and ");
                        }
                        else if (UnionComboBox.SelectedItem.ToString() == "or")
                        {
                            patron.Append(" or ");
                        }
                        break;
                }
                pattern.Append(patron.ToString());
                SeacherPattern.Text = pattern.ToString();
                break;
            }
        }

        private void ImageSmartPanel_Click(object sender, EventArgs e)
        {
            if (this.selectedSmartBox != null)
            {
                selectedSmartBox.BorderStyle = BorderStyle.None;
                this.selectedSmartBox = null;
            }
            PanelImages_Click(this, EventArgs.Empty);
            foreach (Control ctrl in panelImages.Controls)
            {
                if (ctrl is PictureBox)
                {
                    PictureBox PIC = (PictureBox)ctrl;
                    PIC.BackColor = Color.Transparent;
                    PIC.BorderStyle = BorderStyle.None;
                }
            }
        }

        private void MuestraSmartPanel_Click(object sender, EventArgs e)
        {
            ImageSmartPanel.BackColor = Color.Transparent;
            ImageSmartPanel.Controls.Clear();
            foreach (Control ctrl in MuestraSmartPanel.Controls)
            {
                if (ctrl is Button)
                {
                    ctrl.BackColor = Color.FromArgb(35, 32, 39);
                    ctrl.ForeColor = Color.White;
                }
            }

            PanelImages_Click(this, EventArgs.Empty);
            foreach (Control ctrl in panelImages.Controls)
            {
                if (ctrl is PictureBox)
                {
                    PictureBox PIC = (PictureBox)ctrl;
                    PIC.BackColor = Color.Transparent;
                    PIC.BorderStyle = BorderStyle.None;
                }
            }
        }


        private void toolStripMenuIFeaturesRemove_Click(object sender, EventArgs e)
        {
            ToolStripItem menuItem = sender as ToolStripItem;
            if (menuItem != null)
            {
                ContextMenuStrip owner = menuItem.Owner as ContextMenuStrip;
                if (owner != null)
                {
                    Control sourceControl = owner.SourceControl;
                    PictureBox PIC = (PictureBox)sourceControl;
                    Image im = (Image)PIC.Tag;
                    featuresImage.Remove(im);
                    Refresh_FeatureListPanel();
                }
            }
        }
    }



    public class MyRenderer : ToolStripProfessionalRenderer
    {
        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            Rectangle rc = new Rectangle(Point.Empty, e.Item.Size);
            //Color c = e.Item.Selected ? Color.Crimson : Color.FromArgb(11, 7, 17);
            Color c = Color.FromArgb(11, 7, 17);
            if (e.Item.Selected)
            {
                if (e.Item.Text == "My Account") c = Color.FromArgb(6, 57, 76);
                else if (e.Item.Text == "Import" || e.Item.Text == "Import with labels") c = Color.FromArgb(12, 67, 131);
                else if (e.Item.Text == "Export" || e.Item.Text == "Export as") c = Color.FromArgb(34, 160, 182);
                else if (e.Item.Text == "Save Library") c = Color.FromArgb(123, 19, 70);
                else if (e.Item.Text == "Clean Library") c = Color.FromArgb(203, 12, 89);
                else if (e.Item.Text == "Exit") c = Color.FromArgb(235, 100, 158);
            }
            using (SolidBrush brush = new SolidBrush(c)) e.Graphics.FillRectangle(brush, rc);
        }
    }

}

