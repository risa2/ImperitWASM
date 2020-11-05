using System.Text.Json;

namespace ImperitWASM.Server.Load
{
	public interface IFile
	{
		T Read<T>();
	}
	public class File : IFile
	{
		readonly string path;
		public File(string path) => this.path = path;
		public static File Path(params string[] parts) => new File(System.IO.Path.Combine(parts));
		public T Read<T>() => JsonSerializer.Deserialize<T>(System.IO.File.ReadAllText(path));
	}
}