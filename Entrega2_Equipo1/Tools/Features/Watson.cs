using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Entrega2_Equipo1
{
	[Serializable]
	public class Watson: Tool
	{
		private string wKey;
		private readonly string DEFAULT_WATSON_PATH = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + @"\Files\Watson.bin";
		public Watson()
		{
			this.wKey = LoadKey().wKey;
		}

		public string WKey { get => wKey; set => wKey = value; }
		
		private Watson LoadKey()
		{
			BinaryFormatter formatter = new BinaryFormatter();
			Stream stream = new FileStream(this.DEFAULT_WATSON_PATH, FileMode.Open, FileAccess.Read, FileShare.None);
			Watson watson = (Watson)formatter.Deserialize(stream);
			stream.Close();
			return watson;
		}
		

	}
}
