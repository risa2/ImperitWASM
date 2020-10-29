using System.Text.Json;
using System.Threading.Tasks;

namespace ImperitWASM.Server.Load
{
	public interface IFile
	{
		string Read();
		Task Write(string str);
		void Append(string str);
		T ReadJson<T>();
		Task WriteJson<T>(T value);
		void AppendJson<T>(T value);
	}
	public class File : IFile
	{
		readonly string path;
		public File(string path) => this.path = path;
		public static File Path(params string[] parts) => new File(System.IO.Path.Combine(parts));
		public string Read() => System.IO.File.ReadAllText(path);
		public Task Write(string str) => System.IO.File.WriteAllTextAsync(path, str);
		public void Append(string str) => System.IO.File.AppendAllText(path, str);
		public T ReadJson<T>() => JsonSerializer.Deserialize<T>(Read());
		public Task WriteJson<T>(T value) => Write(JsonSerializer.Serialize(value));
		public void AppendJson<T>(T value) => Append("\n" + JsonSerializer.Serialize(value));
	}
}