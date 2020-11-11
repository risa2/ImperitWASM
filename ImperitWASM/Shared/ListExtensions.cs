using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace ImperitWASM.Shared
{
	public static class ListExtensions
	{
		public static T Must<T>(this T? value) where T : class => value ?? throw new ArgumentNullException();
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
		public static ImmutableDictionary<T, int> Lookup<T>(this IEnumerable<T> e) where T : notnull => e.Select((a, i) => (a, i)).ToImmutableDictionary(it => it.a, it => it.i);
		public static int FirstRotated<T>(this IReadOnlyList<T> arr, int shift, Func<T, bool> cond, int otherwise)
		{
			return Enumerable.Range(shift, arr.Count).Where(i => cond(arr[i % arr.Count])).FirstOr(otherwise) % arr.Count;
		}
		public static ImmutableList<T> Replace<T, TC>(this ImmutableList<T> lst, Predicate<TC> cond, Func<TC, TC, TC> join, TC init) where T : class where TC : T
		{
			var those = lst.OfType<TC>().Where(x => cond(x));
			var result = lst.RemoveAll(x => x is TC tc && cond(tc));
			return result.Add(those.Aggregate(init, join));
		}
		public static void Each<T>(this IEnumerable<T> e, Action<T> action)
		{
			foreach (var item in e)
			{
				action(item);
			}
		}
		public static void Each<T>(this IEnumerable<T> e, Action<T, int> action)
		{
			int i = 0;
			foreach (var item in e)
			{
				action(item, i);
				++i;
			}
		}
		public static async Task EachAsync<T>(this IEnumerable<T> e, Func<T, Task> action)
		{
			foreach (var item in e)
			{
				await action(item);
			}
		}
		public static async Task EachAsync<T>(this IEnumerable<T> e, Func<T, int, Task> action)
		{
			int i = 0;
			foreach (var item in e)
			{
				await action(item, i);
				++i;
			}
		}
		public static int? Find<T>(this IList<T> ts, Func<T, bool> cond)
		{
			int i = 0;
			while (i < ts.Count && !cond(ts[i]))
			{
				++i;
			}
			return i < ts.Count ? (int?)i : null;
		}
		public static void InsertMatch<T>(this IList<T> s1, IEnumerable<T> s2, Func<T, T, bool> eq, Func<T, T, T> match)
		{
			foreach (var t2 in s2)
			{
				var index = s1.Find(t1 => eq(t1, t2));
				if (index is int i)
				{
					s1[i] = match(s1[i], t2);
				}
				else
				{
					s1.Add(t2);
				}
			}
		}
	}
}