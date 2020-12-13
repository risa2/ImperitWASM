using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ImperitWASM.Client
{
	public static class ListExtensions
	{
		public static IEnumerable<T> ReverseIf<T>(this IEnumerable<T> e, bool reverse) => reverse ? e.Reverse() : e;
		public static IEnumerable<(int i, T v)> Index<T>(this IEnumerable<T> e) => e.Select((v, i) => (i, v));
		public static ImmutableArray<T> Try<T>(this ImmutableArray<T> ar) => ar.IsDefault ? ImmutableArray<T>.Empty : ar;
		public static T? Try<T>(this ImmutableArray<T> ar, int i) where T : class => ar.IsDefault || ar.Length <= i ? default : ar[i];
	}
}
