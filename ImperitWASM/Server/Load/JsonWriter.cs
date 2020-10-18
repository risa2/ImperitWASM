using ImperitWASM.Shared.Conversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ImperitWASM.Server.Load
{
	public class JsonWriter<T, TK, TA> : JsonLoader<T, TK, TA> where T : IEntity<TK, TA>
	{
		protected Func<TK, T> cvt;
		public JsonWriter(IFile input, TA arg, Func<TK, T> convertor) : base(input, arg) => cvt = convertor;
		string Serialize(TK item) => JsonSerializer.Serialize(cvt(item), new JsonSerializerOptions() { IgnoreNullValues = true });
		string ToWrite(IEnumerable<TK> e) => string.Join('\n', e.Select(Serialize));
		public Task Save(IEnumerable<TK> saved) => io.Write(ToWrite(saved));
		public Task Save(TK saved) => io.Write(Serialize(saved));
		public Task Clear() => io.Write(string.Empty);
		public void Add(IEnumerable<TK> saved) => io.Append("\n" + ToWrite(saved));
		public void Add(TK saved) => Add(new[] { saved });
	}
}
