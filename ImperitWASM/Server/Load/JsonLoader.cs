using ImperitWASM.Shared.Conversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

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
		public IEnumerable<TK> Load() => io.Read().Split('\n', StringSplitOptions.RemoveEmptyEntries).Select((line, i) => JsonSerializer.Deserialize<T>(line).Convert(i, arg));
		public TK LoadOne() => JsonSerializer.Deserialize<T>(io.Read()).Convert(0, arg);
	}
}