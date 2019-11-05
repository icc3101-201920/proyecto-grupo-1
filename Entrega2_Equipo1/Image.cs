using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using MetadataExtractor;

namespace Entrega2_Equipo1
{
    [Serializable]
    public class Image
    {
        private List<Label> labels;
        private string name;
        private Bitmap bitmapImage;
        private int calification;
        private int[] resolution;
        private int[] aspectRatio;
        private bool darkClear;
        private double saturation;
        private Dictionary<int, Dictionary<string, string>> exif;
        private const int DEFAULT_CALIFICATION = -1;
        private Dictionary<EFilter,bool> applyedFilters;
        private Dictionary<string, Dictionary<string, string>> metadata;

        public List<Label> Labels { get => this.labels; set => this.labels = value; }
        public string Name { get => this.name; set => this.name = value; }
        public Bitmap BitmapImage
        { get => this.bitmapImage;
            set
            {
                this.bitmapImage = value;
                this.resolution = LoadResolution();
                this.aspectRatio = LoadAspectRatio();
                this.darkClear = LoadDarkClear();
                 this.saturation = LoadSaturation();
            }
        }
        public int Calification { get => this.calification; set => this.calification = value; }
        public int[] Resolution { get => this.resolution; set => this.resolution = value; }
        public int[] AspectRatio { get => this.aspectRatio; set => this.aspectRatio = value; }
        public bool DarkClear { get => this.darkClear; set => this.darkClear = value; }
        public Dictionary<int, Dictionary<string, string>> Exif { get => this.exif; set => this.exif = value; }
        public Dictionary<EFilter, bool> ApplyedFilters { get => this.applyedFilters; set => this.applyedFilters = value; }
        public double Saturation { get => this.saturation; set => this.saturation = value; }
        public Dictionary<string, Dictionary<string, string>> Metadata { get => this.metadata; set => this.metadata = value; }

        public Image(Bitmap bitmap, List<Label> labels, int calification)
		{
			this.Name = name;
			this.Labels = labels;
			this.Calification = calification;
			this.bitmapImage = bitmap;
			this.Resolution = LoadResolution();
			this.AspectRatio = LoadAspectRatio();
			this.DarkClear = LoadDarkClear();
			this.exif = LoadExif();
            this.ApplyedFilters = new Dictionary<EFilter, bool>() { { EFilter.AutomaticAdjustment, false}, {EFilter.Grayscale, false },
                { EFilter.Brightness, false}, { EFilter.Color, false}, { EFilter.Invert, false}, { EFilter.Mirror, false},
                { EFilter.OldFilm, false}, { EFilter.RotateFlip, false}, { EFilter.Sepia, false}, {EFilter.Windows, false }
				,{EFilter.Contrast,false },{EFilter.Burned, false } };
            this.saturation = LoadSaturation();
            this.metadata = null;
        }

		public Image(string path, List<Label> labels, int calification)
        {
            this.Name = name;
            this.Labels = labels;
            this.Calification = calification;
            this.bitmapImage = LoadBitmapImage(path);
            this.Resolution = LoadResolution();
            this.AspectRatio = LoadAspectRatio();
            this.DarkClear = LoadDarkClear();
            this.exif = LoadExif();
            this.ApplyedFilters = new Dictionary<EFilter, bool>() { { EFilter.AutomaticAdjustment, false}, {EFilter.Grayscale, false },
                { EFilter.Brightness, false}, { EFilter.Color, false}, { EFilter.Invert, false}, { EFilter.Mirror, false},
                { EFilter.OldFilm, false}, { EFilter.RotateFlip, false}, { EFilter.Sepia, false}, {EFilter.Windows, false }
				,{EFilter.Contrast,false } ,{EFilter.Burned, false }};
            this.saturation = LoadSaturation();
            this.metadata = LoadMetadata(path);
        }

        // Other constructor, used to make copies of other images
        public Image(string name, List<Label> labels, int calification, Bitmap bitmap, int[] resolution, int[] aspectratio, bool darkclear, Dictionary<int, Dictionary<string, string>> exif)
        {
            this.Name = name;
            this.Labels = labels;
            this.Calification = calification;
            this.bitmapImage = bitmap;
            this.Resolution = resolution;
            this.aspectRatio = aspectratio;
            this.darkClear = darkclear;
            this.exif = exif;
            this.ApplyedFilters = new Dictionary<EFilter, bool>() { { EFilter.AutomaticAdjustment, false}, {EFilter.Grayscale, false },
                { EFilter.Brightness, false}, { EFilter.Color, false}, { EFilter.Invert, false}, { EFilter.Mirror, false},
                { EFilter.OldFilm, false}, { EFilter.RotateFlip, false}, { EFilter.Sepia, false}, {EFilter.Windows, false } 
				,{EFilter.Contrast,false },{EFilter.Burned, false }};
            this.saturation = LoadSaturation();
            this.metadata = null;
        }

        // Other constructor with DEFAULT_CALIFICATION
        public Image(string name, List<Label> labels) : this(name, labels, DEFAULT_CALIFICATION)
        {
            this.ApplyedFilters = new Dictionary<EFilter, bool>() { { EFilter.AutomaticAdjustment, false}, {EFilter.Grayscale, false },
                { EFilter.Brightness, false}, { EFilter.Color, false}, { EFilter.Invert, false}, { EFilter.Mirror, false},
                { EFilter.OldFilm, false}, { EFilter.RotateFlip, false}, { EFilter.Sepia, false}, {EFilter.Windows, false } 
				,{EFilter.Contrast,false },{EFilter.Burned, false }};
        }


        private Bitmap LoadBitmapImage(string path)
        {
            Bitmap returningbitmapImage = ConvertToBitmap(path);
            return returningbitmapImage;
        }

        public void AddLabel(Label label)
        {
            this.labels.Add(label);
        }

        // Convert a file into Bitmap object
        private Bitmap ConvertToBitmap(string fileName)
        {
            Bitmap bitmap;
            using (Stream bmpStream = System.IO.File.Open(fileName, System.IO.FileMode.Open))
            {
                System.Drawing.Image image = System.Drawing.Image.FromStream(bmpStream);
                bitmap = new Bitmap(image);
            }
            //System.IO.File.Delete(fileName);
            return bitmap;
        }


        private int[] LoadResolution()
        {
            int v1 = this.bitmapImage.Width;
            int v2 = this.bitmapImage.Height;
            int[] returningArray = new int[] { v1, v2 };
            return returningArray;
        }

		public void ReLoadResolution()
		{
			resolution = LoadResolution();
		}
		

        private int MCD(int a, int b)
        {
            while (b > 0)
            {
                int rem = a % b;
                a = b;
                b = rem;
            }
            return a;
        }


        private void Simplify(int[] numbers)
        {
            int mcd = this.MCD(numbers[0],numbers[1]);
            for (int i = 0; i < numbers.Length; i++) numbers[i] /= mcd;
            return;
        }


        private int[] LoadAspectRatio()
        {
            int[] returningAspect = new int[] { this.bitmapImage.Width, this.bitmapImage.Height };
            Simplify(returningAspect);
            return returningAspect;
        }

		public void ReLoadAspectRatio()
		{
			aspectRatio = LoadAspectRatio();
		}



        private bool LoadDarkClear()
        {
            Color color;
            double[] brightnessArray = new double[this.bitmapImage.Width * this.bitmapImage.Height];
            int count = 0;
            for (int i = 0; i < this.bitmapImage.Height; i++)
            {
                for (int x = 0; x < this.bitmapImage.Width; x++)
                {
                    color = this.bitmapImage.GetPixel(x, i);
                    double brightness = color.GetBrightness();
                    brightnessArray[count] = brightness;
                    count++;
                }
            }
            double result = brightnessArray.Average();
            if (result < 0.5)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        // Se carga siempre
        private double LoadSaturation()
        {
            int count = 0;
            double sat = 0;
            Color color;
            for (int i = 0; i < this.bitmapImage.Height; i++)
            {
                for (int x = 0; x < this.bitmapImage.Width; x++)
                {
                    color = this.bitmapImage.GetPixel(x, i);
                    sat += color.GetSaturation();
                    count++;
                }
            }
            sat /= count;
            sat = Math.Truncate(sat * 1000) / 1000;
            return sat;
        }

		private double LoadSaturationTEST()
		{
			int count = 0;
			double sat = 0;
			Color color;
	
			Bitmap bmap = bitmapImage;

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

						//currentLine[x] = (byte)((oldBlue * usrColor.B) / 255);
						//currentLine[x + 1] = (byte)((oldGreen * usrColor.G) / 255);
						//currentLine[x + 2] = (byte)((oldRed * usrColor.R) / 255);
						color = Color.FromArgb(oldRed, oldGreen, oldBlue);
						sat += color.GetSaturation();
						count++;
					}
				});
				bmap.UnlockBits(bitmapData);


			}
			sat /= count;
			sat = Math.Truncate(sat * 1000) / 1000;
			return sat;
		}

		private Dictionary<string, Dictionary<string, string>> LoadMetadata(string path)
        {
            Dictionary<string, Dictionary<string, string>> returningDict = new Dictionary<string, Dictionary<string, string>>();
            IEnumerable <MetadataExtractor.Directory> directories = ImageMetadataReader.ReadMetadata(@path);

            if (directories.Count() != 0)
            {
                foreach (var directory in directories)
                {
                    returningDict[directory.Name] = new Dictionary<string, string>();
                    foreach (var tag in directory.Tags)
                    {
                        returningDict[directory.Name][tag.Name] = tag.Description;
                    }
                }
                return returningDict;
            }
            else return null;
        }

        private Dictionary<int, Dictionary<string, string>> LoadExif()
        {

            PropertyItem[] items = this.bitmapImage.PropertyItems;
            string id;
            string type;
            string len;
            string value;
            Dictionary<int, Dictionary<string, string>> returningDic = new Dictionary<int, Dictionary<string, string>>();
            int count = 1;
            foreach (PropertyItem item in items)
            {
                ASCIIEncoding encoding = new ASCIIEncoding();
                id = item.Id.ToString();
                type = item.Id.ToString();
                len = item.Len.ToString();
                if (item.Value != null) value = encoding.GetString(item.Value);
                else value = "Not found";
                returningDic.Add(count, new Dictionary<string, string>());
                returningDic[count].Add("id", id);
                returningDic[count].Add("type", type);
                returningDic[count].Add("len", len);
                returningDic[count].Add("value", value);
                count++;
            }

            return returningDic;
        }

        public void RemoveLabel(string labeltype, int pos)
        {
            int count = 0;
            foreach (Label label in Labels)
            {
                if (label.labelType == labeltype)
                {
                    if (count == pos)
                    {
                        break;
                    }
                    count++;
                }
            }
            labels.RemoveAt(count);
        }

        public List<PersonLabel> SelectPersonLabels()
        {
            List <PersonLabel> returningList = new List<PersonLabel>();
            foreach (Label label in this.labels)
            {
                if (label.labelType == "PersonLabel")
                {
                    returningList.Add((PersonLabel)label);
                }
            }
            return returningList;
        }


        public List<SpecialLabel> SelectSpecialLabel()
        {
            List<SpecialLabel> returningList = new List<SpecialLabel>();
            foreach (Label label in this.labels)
            {
                if (label.labelType == "SpecialLabel")
                {
                    returningList.Add((SpecialLabel)label);
                }
            }
            return returningList;
        }


        public List<SimpleLabel> SelectSimpleLabels()
        {
            List<SimpleLabel> returningList = new List<SimpleLabel>();
            foreach (Label label in this.labels)
            {
                if (label.labelType == "SimpleLabel")
                {
                    returningList.Add((SimpleLabel)label);
                }
            }
            return returningList;
        }


        public bool SomePersonLabelContains(string attribute, string s = null, ENationality En = ENationality.None, EColor Ec = EColor.None, ESex Es = ESex.None, double[] Bd = null)
        {
            List<PersonLabel> internalList = SelectPersonLabels();
            if (s != null)
            {
                switch (attribute)
                {
                    case "Name":
                        foreach (PersonLabel label in internalList)
                        {
                            if (label.Name == s) return true;
                        }
                        return false;
                    case "Surname":
                        foreach (PersonLabel label in internalList)
                        {
                            if (label.Surname == s) return true;
                        }
                        return false;
                    case "Birthdate":
                        foreach (PersonLabel label in internalList)
                        {
                            if (label.BirthDate == s) return true;
                        }
                        return false;
                }
            }
            else if (En != ENationality.None)
            {
                foreach (PersonLabel label in internalList)
                {
                    if (label.Nationality == En) return true;
                }
                return false;
            }
            else if (Ec != EColor.None)
            {
                switch (attribute)
                {
                    case "EyesColor":
                        foreach (PersonLabel label in internalList)
                        {
                            if (label.EyesColor == Ec) return true;
                        }
                        return false;
                    case "HairColor":
                        foreach (PersonLabel label in internalList)
                        {
                            if (label.HairColor == Ec) return true;
                        }
                        return false;
                }
            }
            else if (Es != ESex.None)
            {
                foreach (PersonLabel label in internalList)
                {
                    if (label.Sex == Es) return true;
                }
                return false;
            }
            else if (Bd != null)
            {
                foreach (PersonLabel label in internalList)
                {
                    if (label.FaceLocation == Bd) return true;
                }
                return false;
            }
            throw new Exception("Wrong search parameters");
        }


        public bool SomeSpecialLabelContains(string attribute, double[] geographicLocation = null, string s = null, bool selfie = false)
        {
            List<SpecialLabel> internalList = SelectSpecialLabel();
            if (geographicLocation != null)
            {
                foreach (SpecialLabel label in internalList)
                {
                    if (label.GeographicLocation != geographicLocation) return true;
                }
                return false;
                
            }
            else if (s != null)
            {
                switch (attribute)
                {
                    case "Address":
                        foreach (SpecialLabel label in internalList)
                        {
                            if (label.Address == s) return true;
                        }
                        return false;
                    case "Photographer":
                        foreach (SpecialLabel label in internalList)
                        {
                            if (label.Photographer == s) return true;
                        }
                        return false;
                    case "Photomotive":
                        foreach (SpecialLabel label in internalList)
                        {
                            if (label.PhotoMotive == s) return true;
                        }
                        return false;
                }
            }
            else 
            {
                foreach (SpecialLabel label in internalList)
                {
                    if (label.Selfie == selfie) return true;
                }
                return false;
            }
            throw new Exception("Wrong search parameters");
        }


        public bool SomeSimpleLabelContains(string attribute, string sentence)
        {
            List<SimpleLabel> internalList = SelectSimpleLabels();
            foreach (SimpleLabel label in internalList)
            {
                if (label.Sentence == sentence) return true;
            }
            return false;
        }

        public void ResetFaceLocation()
        {
            foreach (Label labels in Labels)
            {
                if (labels.labelType == "PersonLabel")
                {
                    PersonLabel PL = (PersonLabel)labels;
                    PL.FaceLocation = null;
                }
            }
        }
    }
}
