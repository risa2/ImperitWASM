using System.Collections.Generic;

namespace ImperitWASM.Server.Load
{
	public interface IFile
	{
		string[] Read();
		void Write(IEnumerable<string> str);
		void Clear();
		void Append(string str);
	}
	public class File : IFile
	{
		readonly string path;
		public File(string path) => this.path = path;
		public static File Path(params string[] parts) => new File(System.IO.Path.Combine(parts));
		public string[] Read() => System.IO.File.ReadAllLines(path);
		public void Write(IEnumerable<string> str) => System.IO.File.WriteAllLines(path, str);
		public void Append(string str) => System.IO.File.AppendAllText(path, str);
		public void Clear() => System.IO.File.WriteAllText(path, string.Empty);
	}
}