namespace ImperitWASM.Server.Load
{
	public interface IFile
	{
		string Read();
		void Write(string str);
		void Append(string str);
	}
	public class File : IFile
	{
		readonly string path;
		public File(string path) => this.path = path;
		public static File Path(params string[] parts) => new File(System.IO.Path.Combine(parts));
		public string Read() => System.IO.File.ReadAllText(path);
		public void Write(string str) => System.IO.File.WriteAllText(path, str);
		public void Append(string str) => System.IO.File.AppendAllText(path, str);
	}
}