using System.Text.Json;
using ImperitWASM.Shared;

namespace ImperitWASM.Server.Load
{
	public interface IFile
	{
		T Read<T>() where T : class;
	}
	public class File : IFile
	{
		readonly string path;
		public File(params string[] parts) => path = System.IO.Path.Combine(parts);
		public T Read<T>() where T : class => JsonSerializer.Deserialize<T>(System.IO.File.ReadAllText(path)).Must();
	}
}