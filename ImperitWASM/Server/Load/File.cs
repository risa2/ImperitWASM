using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;

namespace ImperitWASM.Server.Load
{
	public interface IFile
	{
		string Read();
		Task Write(string str);
		void Append(string str);
		TR ReadJson<TR, TV>(Func<TV, TR> convert);
		IEnumerable<TR> ReadJsons<TR, TV>(Func<TV, int, TR> convert);
		Task WriteJson<TR, TV>(TR value, Func<TR, TV> convert);
		Task WriteJsons<TR, TV>(IEnumerable<TR> value, Func<TR, TV> convert);
		void AppendJson<TR, TV>(TR value, Func<TR, TV> convert);
	}
	public class File : IFile
	{
		readonly string path;
		public File(string path) => this.path = path;
		public static File Path(params string[] parts) => new File(System.IO.Path.Combine(parts));
		public string Read() => System.IO.File.ReadAllText(path);
		public Task Write(string str) => System.IO.File.WriteAllTextAsync(path, str);
		public void Append(string str) => System.IO.File.AppendAllText(path, str);
		public TResult ReadJson<TResult, T>(Func<T, TResult> convert)
		{
			return convert(JsonSerializer.Deserialize<T>(Read()));
		}
		public IEnumerable<TR> ReadJsons<TR, TV>(Func<TV, int, TR> convert)
		{
			return Read().Split('\n', StringSplitOptions.RemoveEmptyEntries).Select((ln, i) => convert(JsonSerializer.Deserialize<TV>(ln), i));
		}
		public Task WriteJson<TR, TV>(TR value, Func<TR, TV> convert)
		{
			return Write(JsonSerializer.Serialize(convert(value)));
		}
		public Task WriteJsons<TR, TV>(IEnumerable<TR> values, Func<TR, TV> convert)
		{
			return Write(string.Join('\n', values.Select(value => JsonSerializer.Serialize(convert(value)))));
		}
		public void AppendJson<TR, TV>(TR value, Func<TR, TV> convert)
		{
			Append("\n" + JsonSerializer.Serialize(convert(value)));
		}
	}
}