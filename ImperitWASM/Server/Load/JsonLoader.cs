using ImperitWASM.Shared.Conversion;
using System.Collections.Generic;

namespace ImperitWASM.Server.Load
{
	public class JsonLoader<T, TK, TA> where T : IEntity<TK, TA>
	{
		protected IFile io;
		protected TA arg;
		public JsonLoader(IFile io, TA arg)
		{
			this.io = io;
			this.arg = arg;
		}
		public IEnumerable<TK> Load() => io.ReadJsons<TK, T>((value, i) => value.Convert(i, arg));
		public TK LoadOne() => io.ReadJson<TK, T>(value => value.Convert(0, arg));
	}
}