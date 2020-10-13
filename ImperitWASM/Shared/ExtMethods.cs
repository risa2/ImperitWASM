using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;

namespace ImperitWASM.Shared
{
	public static class ExtMethods
	{
		public static CultureInfo Culture { get; } = CultureInfo.InvariantCulture;
		public static T FirstOr<T>(this IEnumerable<T> e, T x) => e.DefaultIfEmpty(x).First();
		public static IEnumerable<T> Reverse<T>(this IEnumerable<T> e, bool reverse) => reverse ? e.Reverse() : e;
		public static IEnumerable<(int i, T v)> Index<T>(this IEnumerable<T> e) => e.Select((v, i) => (i, v));
		public static T? MinBy<T, TC>(this IEnumerable<T> e, Func<T, TC> selector, T? v = default) where T : class => e.OrderBy(selector).FirstOr(v);
		public static T Must<T>(this T? value) where T : struct => value ?? throw new ArgumentNullException();
		public static IEnumerable<T> Try<T>(this IEnumerable<T>? e) => e is null ? Enumerable.Empty<T>() : e;
		public static int DivUp(this int a, int b) => (a / b) + (a % b > 0 ? 1 : 0);
		public static TA Aggregate<T, TA>(this IEnumerable<T> e, TA init, Func<TA, T, int, TA> fn)
		{
			foreach (var (i, item) in e.Index())
			{
				init = fn(init, item, i);
			}
			return init;
		}
		public static (ImmutableList<A>, P) Fold<A, P>(this IEnumerable<A> e, P init, Func<P, A, (P, A?)> fn) where A : class
		{
			var result = ImmutableList.CreateBuilder<A>();
			foreach (A item in e)
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
		public static int Mod(this int x, int y) => ((x % y) + y) % y;
		public static int FirstRotated<T>(this IReadOnlyList<T> arr, int shift, Func<T, bool> cond)
		{
			return Enumerable.Range(shift, arr.Count).Select(i => i.Mod(arr.Count)).Where(i => cond(arr[i])).FirstOr(-1);
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
		public static string ProbabilityToString(this double prob, int prec = 0)
		{
			int percents = (int)(prob * 100);
			int frac = percents >= 100 ? 0 : (int)(prob * 100 * Math.Pow(10, prec)) - (int)(percents * Math.Pow(10, prec));
			return Math.Max(0, Math.Min(100, percents)).ToString(Culture) + "." + frac.ToString(Culture).PadRight(prec, '0') + " %";
		}
		public static string ToHexString(this byte num) => num.ToString("x2", Culture);
		public static string NextId(this Random rand, int length)
		{
			var buf = new byte[length];
			rand.NextBytes(buf);
			return Convert.ToBase64String(buf).TrimEnd('=').Replace('+', '-').Replace('/', '_');
		}
		public static void Shuffle<T>(this Random rand, IList<T> list)
		{
			int n = list.Count;
			while (n > 1)
			{
				n--;
				int k = rand.Next(n + 1);
				T value = list[k];
				list[k] = list[n];
				list[n] = value;
			}
		}
	}
}