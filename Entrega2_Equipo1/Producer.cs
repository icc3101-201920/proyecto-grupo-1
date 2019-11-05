using Entrega2_Equipo1.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Entrega2_Equipo1
{
    [Serializable]
    public class Producer
    {
        private WorkingArea WorkingArea;
        private List<Tool> tools;

        public Producer()
        {
            this.WorkingArea = new WorkingArea();
            this.tools = new List<Tool>() { new Brush(), new Merger(), new Resizer(),
                new Scissors(), new AddCensorship(), new AddImage(),
                new AddShape(), new AddText(), new AutomaticAdjustmentFilter(),
                new BlackNWhiteFilter(), new BrightnessFilter(), new ColorFilter(), new InvertFilter(),
                new MirrorFilter(), new OldFilmFilter(), new RotateFlipFilter(), new SepiaFilter(), new WindowsFilter()};
        }


        public Bitmap AddText(Bitmap bitmap, string text, int xAxis, int yAxis, float fontSize,
            Color colorName1, string fontStyle, string fontName
            , Color colorName2)
        {
            AddText addtext = (AddText)this.tools[7];
            return addtext.InsertText(bitmap, text, xAxis, yAxis, fontSize, colorName1, fontStyle, fontName, colorName2);
        }


        public void DeleteImageInTheWorkingArea(int position)
        {
            WorkingArea.WorkingAreaImages.RemoveAt(position);
            return;
        }

		public bool RemoveImage(Image image)
		{
			foreach (Image imag in WorkingArea.WorkingAreaImages)
			{
				if (imag == image)
				{
					WorkingArea.WorkingAreaImages.Remove(imag);
					return true;
				}
			}
			return false;
		}

		public List<Image> imagesInTheWorkingArea()
        {
            return this.WorkingArea.WorkingAreaImages;
        }

        public void LoadWatsonAnalyzer()
        {
            this.tools.Add(new WatsonAnalizer());
        }

        public void DeleteWatsonAnalyzer()
        {
            this.tools.RemoveAt(18);
        }

        public void LoadImagesToWorkingArea(List<Image> images)
        {
            List<Image> workingAreaImages = new List<Image>();
            foreach (Image image in images)
            {
                List<Label> listLabelCopy = new List<Label>();
                foreach (Label label in image.Labels)
                {
                    if (label.labelType == "SimpleLabel")
                    {
                        SimpleLabel label2 = (SimpleLabel)label;
                        listLabelCopy.Add(new SimpleLabel(label2.Sentence));
                    }
                    else if (label.labelType == "PersonLabel")
                    {
                        PersonLabel label2 = (PersonLabel)label;
                        listLabelCopy.Add(new PersonLabel(label2.Name, label2.FaceLocation, label2.Surname, label2.Nationality, label2.EyesColor, label2.HairColor, label2.Sex, label2.BirthDate, label2.SerialNumber));
                    }
                    else if (label.labelType == "SpecialLabel")
                    {
                        SpecialLabel label2 = (SpecialLabel)label;
                        listLabelCopy.Add(new SpecialLabel(label2.GeographicLocation, label2.Address, label2.Photographer, label2.PhotoMotive, label2.Selfie, label2.SerialNumber));
                    }
                }

                Image newImage = new Image(image.Name, listLabelCopy, image.Calification, image.BitmapImage, image.Resolution, image.AspectRatio, image.DarkClear, image.Exif);
                workingAreaImages.Add(newImage);
            }
            foreach (Image image in workingAreaImages)
            {
                this.WorkingArea.WorkingAreaImages.Add(image);
            }

            return;
        }


        public Dictionary<int, Dictionary<string, double>> ClassifyImage(string path)
        {
            Bitmap bitmapImage = new Bitmap(path);
            WatsonAnalizer myFilter = (WatsonAnalizer)this.tools[18];
            Dictionary<int, Dictionary<string, double>> resultadoClasificacion = myFilter.FindClassifiers(bitmapImage);
            return resultadoClasificacion;
        }

		public Bitmap Contrast(Bitmap bitmap, double contrast)
		{
			AutomaticAdjustmentFilter auto = new AutomaticAdjustmentFilter();
			return auto.ApplyContrast(bitmap, contrast);
		}

        public bool Presentation(List<Image> images)
        {
            throw new NotImplementedException();
        }


        public bool Slideshow(List<Image> images)
        {
            throw new NotImplementedException();
        }


        public System.Drawing.Bitmap Merge(List<Image> images)
        {
			Resizer resizer = new Resizer();
            List<Bitmap> Final = new List<Bitmap>();
            int x = 0;
            int y = 0;
            for (int i = 0; i < images.Count; i++)
            {
                if ((x >= images[i].Resolution[0] && y >= images[i].Resolution[1]) || (x == 0 && y == 0))
                    {
                        x = images[i].Resolution[0];
                        y = images[i].Resolution[1];
                        
                    }
                }
			foreach (Image image in images)
			{
				Bitmap bitmap = resizer.ResizeImage(image.BitmapImage,x,y);
				Final.Add(bitmap);
			}

            return MergeRecursive(Final, Final[0], 2);
        }

        public System.Drawing.Bitmap MergeRecursive(List<Bitmap> images, Bitmap merged = null, int cont = 2)
        {
            Merger merger = new Merger();
            if (images.Count == 2 && cont == 2)
            {
                cont++;
                return MergeRecursive(images, merger.Merge(images[0], images[1]), cont);
            }
            else if (cont < images.Count)
            {
                cont++;
                return MergeRecursive(images, merger.Merge(merged, images[cont-1]), cont);

            }
            return merged;

        }

        public System.Drawing.Bitmap Mosaic (Image imagenBase, List<Image> Imagenes, int width, int height, int Bwidth, int Bheight ,ProgressBar progressBar)
        {
			AddImage AI = new AddImage();
			return AI.Mosaic(imagenBase, Imagenes, width, height, Bwidth, Bheight, progressBar);
        }

        public System.Drawing.Bitmap Collage(List<Image> images, int baseWidth, int baseHeight, int insertWidth, int insertHeight, System.Drawing.Bitmap backgroundImage = null, int R=0, int G=0, int B=0)
        {
            AddImage AI = new AddImage();
            return AI.ImageCollage(images, baseWidth, baseHeight, insertWidth, insertHeight, backgroundImage, R,G,B);
        }

        public System.Drawing.Bitmap Album(List<string> nombresImagenes, int cantFotosXPagina)
        {
            throw new NotImplementedException();
        }

        public System.Drawing.Bitmap Calendar(string[] nombresImágenes, int anio)
        {
            throw new NotImplementedException();
        }

        public System.Drawing.Bitmap ApplyFilter(Image image ,EFilter filtro, Color color = default(Color), int brightness = 0, int noise = 60, 
            RotateFlipType RFT = RotateFlipType.RotateNoneFlipNone)
        {   
            switch (filtro)
            {
                case EFilter.AutomaticAdjustment:
                    AutomaticAdjustmentFilter AAF = new AutomaticAdjustmentFilter();
                    Bitmap ret = AAF.ApplyFilter(image.BitmapImage);
                    GC.Collect();
                    return ret;

                case EFilter.Grayscale:
                    BlackNWhiteFilter BNWF = new BlackNWhiteFilter();
                    Bitmap ret2 = BNWF.ApplyFilter(image.BitmapImage);
                    GC.Collect();
                    return ret2;

                case EFilter.Brightness:
                    BrightnessFilter BF = new BrightnessFilter();
                    Bitmap ret3 = BF.ApplyFilter(image.BitmapImage, brightness);
                    GC.Collect();
                    return ret3;
                
                case EFilter.Color:
                    ColorFilter CF = new ColorFilter();
                    Bitmap ret4 = CF.ApplyFilter(image.BitmapImage, color);
                    GC.Collect();
                    return ret4;
                
                case EFilter.Invert:
                    InvertFilter IF = new InvertFilter();
                    Bitmap ret5 = IF.ApplyFilter(image.BitmapImage);
                    GC.Collect();
                    return ret5;

                case EFilter.Mirror:
                    MirrorFilter MF = new MirrorFilter();
                    Bitmap ret6 = MF.ApplyFilter(image.BitmapImage);
                    GC.Collect();
                    return ret6;

                case EFilter.OldFilm:
                    OldFilmFilter OFF = new OldFilmFilter();
                    Bitmap ret7 = OFF.ApplyFilter(image.BitmapImage, noise);
                    GC.Collect();
                    return ret7;

                case EFilter.RotateFlip:
                    RotateFlipFilter RFF = new RotateFlipFilter();
                    Bitmap ret8 = RFF.RotateFlip(image.BitmapImage, RFT);
                    GC.Collect();
                    return ret8;

                case EFilter.Sepia:
                    SepiaFilter SF = new SepiaFilter();
                    Bitmap ret9 = SF.ApplyFilter(image.BitmapImage);
                    GC.Collect();
                    return ret9;

                case EFilter.Windows:
                    WindowsFilter WF = new WindowsFilter();
                    Bitmap ret10 = WF.ApplyFilter(image.BitmapImage);
                    GC.Collect();
                    return ret10;
            }
            return image.BitmapImage;
        }


        public Dictionary<string, int> SexAndAgeRecognition(Image image)
        {
            Dictionary<string, int> result = new Dictionary<string, int>() { };
            WatsonAnalizer WA = new WatsonAnalizer();
            Dictionary<int, Dictionary<string, object>> facesDict = WA.FindFaces(image.BitmapImage);
            double ageScore = 0;
            int maleScore = 0;
            int femaleScore = 0;
            int cont = 0;

            foreach (KeyValuePair<int, Dictionary<string,object>> pair in facesDict)
            {
                Dictionary<string, object> value = pair.Value;
                foreach(KeyValuePair<string,object> Scores in value)
                {
                    string String = Scores.Key;
                    object Object = Scores.Key;

                    switch (String)
                    {
                        case "age":
                            Age ageObj = (Age)Object;
                            ageScore += (ageObj.MaxAge-ageObj.MinAge)/ 2;
                            break;

                        case "gender":
                            Gender genderObj = (Gender)Object;
                            if (genderObj.GenderLabel == "male")
                            {
                                maleScore++;
                            }
                            else
                            {
                                femaleScore++;
                            }
                            break;
                    }
                }
                cont++;
            }
            if (maleScore > femaleScore)
            {
                result.Add("Male", Convert.ToInt32(Math.Floor(ageScore/cont)));
            }
            else if (maleScore < femaleScore)
            {
                result.Add("Female", Convert.ToInt32(Math.Floor(ageScore / cont)));
            }
            else
            {
                Random rnd = new Random();
                string[] genders = new string[2];
                genders[0] = "Male";
                genders[1] = "Female";
                result.Add(genders[rnd.Next(0,1)], Convert.ToInt32(Math.Floor(ageScore / cont)));
            }

            return result;
        } // => ESTE METODO SE DEBE VOLVER A HACER


        public System.Drawing.Bitmap PixelCensorship(Image image, int[] coordinates)
        {
            AddCensorship AC = new AddCensorship();
            return AC.pixelCensorship(image, coordinates);
        }

        public System.Drawing.Bitmap BlackCensorship(Image image, int[] coordinates)
        {
            AddCensorship AC = new AddCensorship();
            return AC.blackCensorship(image.BitmapImage,coordinates);
        }

    }
}
