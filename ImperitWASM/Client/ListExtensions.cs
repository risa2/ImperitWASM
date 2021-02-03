using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ImperitWASM.Client
{
	public static class ListExtensions
	{
		public static IEnumerable<T> ReverseIf<T>(this IEnumerable<T> e, bool reverse) => reverse ? e.Reverse() : e;
		public static T? Try<T>(this ImmutableArray<T> ar, int i) where T : class => ar.IsDefault || ar.Length <= i ? default : ar[i];
		public static IEnumerable<TR> Pairs<T, TR>(this IEnumerable<T> e, Func<T, T, TR> f) => e.Zip(e.Skip(1), f);
	}
}
