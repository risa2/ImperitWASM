using System.Threading.Tasks;
using Blazored.SessionStorage;
using System.Collections.Generic;
using System.Linq;

namespace ImperitWASM.Client
{
	public static class ClientExtensions
	{
		public static IEnumerable<T> Reverse<T>(this IEnumerable<T> e, bool reverse) => reverse ? e.Reverse() : e;
		public static IEnumerable<(int i, T v)> Index<T>(this IEnumerable<T> e) => e.Select((v, i) => (i, v));
		public static IEnumerable<T> Try<T>(this IEnumerable<T>? e) => e ?? Enumerable.Empty<T>();
	}
}
