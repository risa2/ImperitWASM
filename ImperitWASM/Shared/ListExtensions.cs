using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ImperitWASM.Shared
{
	public static class ListExtensions
	{
		public static T FirstOr<T>(this IEnumerable<T> e, T x) => e.DefaultIfEmpty(x).First();
		public static T? MinBy<T, TC>(this IEnumerable<T> e, Func<T, TC> selector, T? v = default) where T : class => e.OrderBy(selector).FirstOr(v);
		public static (ImmutableList<A>, P) Fold<A, P>(this IEnumerable<A> e, P init, Func<P, A, (P, A?)> fn) where A : class
		{
			var result = ImmutableList.CreateBuilder<A>();
			foreach (var item in e)
			{
				var (p, a) = fn(init, item);
				init = p;
				if (a is A)
				{
					result.Add(a);
				}
			}
			return (result.ToImmutable(), init);
		}
		static int Mod(int x, int y) => ((x % y) + y) % y;
		public static int FirstRotated<T>(this IReadOnlyList<T> arr, int shift, Func<T, bool> cond)
		{
			return Enumerable.Range(shift, arr.Count).Select(i => Mod(i, arr.Count)).Where(i => cond(arr[i])).FirstOr(-1);
		}
		public static int Find<T>(this IList<T> ts, Func<T, bool> cond)
		{
			int i = 0;
			while (i < ts.Count && !cond(ts[i]))
			{
				++i;
			}
			return i;
		}
		public static void InsertMatch<T>(this IList<T> s1, IEnumerable<T> s2, Func<T, T, bool> eq, Func<T, T, T> match)
		{
			foreach (var t2 in s2)
			{
				var i = s1.Find(t1 => eq(t1, t2));
				if (i < s1.Count)
				{
					s1[i] = match(s1[i], t2);
				}
				else
				{
					s1.Add(t2);
				}
			}
		}
		public static void Shuffle<T>(this Random rand, IList<T> list)
		{
			int n = list.Count;
			while (n > 1)
			{
				n--;
				int k = rand.Next(n + 1);
				var value = list[k];
				list[k] = list[n];
				list[n] = value;
			}
		}
	}
}