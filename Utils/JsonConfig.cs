using System.IO;
using Terraria;
using Newtonsoft.Json;

namespace Utils.JsonConfig {
	public class JsonConfig<T> {
		public string FileName { get; private set; }
		public T Data { get; private set; }


		public JsonConfig( string filename, T data ) {
			Directory.CreateDirectory(Main.SavePath);

			this.SetFileName(filename);
			this.Data = data;
		}

		public void SetFileName( string filename ) {
			this.FileName = string.Concat(new object[] { Main.SavePath, Path.DirectorySeparatorChar, filename });
		}

		public bool Load() {
			if( !File.Exists(this.FileName) ) {
				return false;
			}
			using( StreamReader r = new StreamReader(this.FileName) ) {
				string json = r.ReadToEnd();
				this.Data = JsonConvert.DeserializeObject<T>(json);
			}
			return true;
		}

		public void Save() {
			string json = JsonConvert.SerializeObject(this.Data, Formatting.Indented);
			File.WriteAllText(this.FileName, json);
		}
	}
}
