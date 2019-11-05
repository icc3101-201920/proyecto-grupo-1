using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Entrega2_Equipo1
{
	public class ProgramManager
	{

        #region constantsAndAttributesRegion

        public Producer producer;
        private readonly string DEFAULT_LIBRARY_PATH = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + @"\Files\";
        private readonly string DEFAULT_PRODUCER_PATH = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + @"\Files\";

        #endregion

        #region iFruitUsedRegion

        public Library LoadingUsersLibraryManager(string usrname)
        {
            if (this.ExistsUsersLibrary(usrname))
            {
                return LoadUsersLibrary(usrname);
            }
            else
            {
                return new Library();
            }
        }

        public void SavingUsersLibraryManager(string usrname, Library library)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(this.DEFAULT_LIBRARY_PATH + usrname + @"\library.bin", FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, library);
            stream.Close();
        }

        public Library LoadUsersLibrary(string usrname)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(DEFAULT_LIBRARY_PATH + usrname + @"\library.bin", FileMode.Open, FileAccess.Read, FileShare.None);
            Library library = (Library)formatter.Deserialize(stream);
            stream.Close();
            return library;
        }

        public bool ExistsUsersLibrary(string usrname)
        {
            if (File.Exists(DEFAULT_LIBRARY_PATH + usrname + @"\library.bin"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Producer LoadingUsersProducerManager(string usrname)
        {
            if (this.ExistsUsersProducer(usrname))
            {
                return LoadUsersProducer(usrname);
            }
            else
            {
                producer = new Producer();
                producer.LoadWatsonAnalyzer();
                return producer;
            }
        }

        public Producer LoadUsersProducer(string usrname)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(DEFAULT_LIBRARY_PATH + usrname + @"\producer.bin", FileMode.Open, FileAccess.Read, FileShare.None);
            Producer producer = (Producer)formatter.Deserialize(stream);
            stream.Close();
            return producer;
        }

        private bool ExistsUsersProducer(string usrname)
        {
            if (File.Exists(DEFAULT_LIBRARY_PATH + usrname + @"\producer.bin"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<string> LoadWatsonRecommendations(Image image, Producer producer)
        {
            if (image.Resolution[0] > 1920 || image.Resolution[1] > 1080) throw new Exception();
            string temppath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + @"\Files\Temp\";
            string[] currentfiles = Directory.GetFiles(temppath);
            Random randomNumber = new Random();
            string realpath;
            while (true)
            {
                int newRandom = randomNumber.Next(1, 100000);
                string number = Convert.ToString(newRandom);
                realpath = temppath + number + ".jpg";
                try
                {
                    image.BitmapImage.Save(realpath);
                    break;
                }
                catch
                {
                    continue;
                }
            }
            long filelength = new System.IO.FileInfo(realpath).Length;
            filelength /= 1048576;
            if (filelength > 10) throw new Exception();
            Dictionary<int, Dictionary<string, double>> watsonResults = producer.ClassifyImage(realpath);
            List<string> watsonOptions = new List<string>();
            foreach (Dictionary<string, double> dic in watsonResults.Values)
            {
                foreach (KeyValuePair<string, double> pair in dic)
                {
                    watsonOptions.Add(pair.Key);
                }
            }
            return watsonOptions;
        }

        #endregion

    }
}
