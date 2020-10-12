using ImperitWASM.Shared.Conversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace ImperitWASM.Server.Load
{
	public class JsonWriter<T, TK, TA> : JsonLoader<T, TK, TA> where T : IEntity<TK, TA>
	{
		protected Func<TK, T> cvt;
		public JsonWriter(IFile input, TA arg, Func<TK, T> convertor) : base(input, arg) => cvt = convertor;
		string ToWrite(IEnumerable<TK> e) => string.Join('\n', e.Select(item => JsonSerializer.Serialize(cvt(item), new JsonSerializerOptions() { IgnoreNullValues = true })));
		public void Save(IEnumerable<TK> saved) => io.Write(ToWrite(saved));
		public void Save(TK saved) => Save(new[] { saved });
		public void Clear() => io.Write(string.Empty);
		public void Add(IEnumerable<TK> saved) => io.Append("\n" + ToWrite(saved));
		public void Add(TK saved) => Add(new[] { saved });
	}
}
