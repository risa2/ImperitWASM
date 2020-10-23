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
		public Task Save(IEnumerable<TK> saved) => io.WriteJsons(saved, cvt);
		public Task Save(TK saved) => io.WriteJson(saved, cvt);
		public Task Clear() => io.Write(string.Empty);
		public void Add(TK saved) => io.AppendJson(saved, cvt);
	}
}
