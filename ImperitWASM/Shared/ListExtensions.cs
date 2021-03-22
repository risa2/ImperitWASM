using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ImperitWASM.Shared
{
	public static class ListExtensions
	{
		static readonly Random rand = new Random();
		public static IEnumerable<int> Indices<T>(this IEnumerable<T> en, Func<T, bool> pred) => en.Select((v, i) => (v, i)).Where(x => pred(x.v)).Select(x => x.i);
		public static T Must<T>(this T? value) where T : class => value ?? throw new ArgumentNullException(typeof(T).FullName);
		public static T FirstOr<T>(this IEnumerable<T> e, T x) => e.DefaultIfEmpty(x).First();
		public static T? MaxBy<T, TC>(this IEnumerable<T> e, Func<T, TC> selector, T? v = default) => e.OrderByDescending(selector).FirstOr(v);
		public static ImmutableDictionary<T, int> Lookup<T>(this IEnumerable<T> e) where T : notnull => e.Select((a, i) => (a, i)).ToImmutableDictionary(it => it.a, it => it.i);
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
				int? index = s1.Find(t1 => eq(t1, t2));
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
		public static void Shuffle<T>(this IList<T> list)
		{
			for (int i = 0, len = list.Count; i < len - 1; ++i)
			{
				int r = i + rand.Next(len - i);
				var tmp = list[r];
				list[r] = list[i];
				list[i] = tmp;
			}
		}
		public static List<T> Shuffled<T>(this IEnumerable<T> e)
		{
			var list = e.ToList();
			list.Shuffle();
			return list;
		}
		public static IEnumerable<TR> MapFold<T, TA, TR>(this IEnumerable<T> e, TA accumulator, Func<T, TA, (TR, TA)> selector)
		{
			foreach (var item in e)
			{
				var (new_item, new_accumulator) = selector(item, accumulator);
				accumulator = new_accumulator;
				yield return new_item;
			}
		}
		public static IEnumerable<(int i, T v)> Index<T>(this IEnumerable<T> e) => e.Select((v, i) => (i, v));
		public static IEnumerable<int> Indices<T>(this IEnumerable<T> e) => e.Select((v, i) => i);
		public static ImmutableArray<T> Alter<T>(this IReadOnlyCollection<T> e, IEnumerable<(int, T)> changes)
		{
			var result = ImmutableArray.CreateBuilder<T>(e.Count);
			foreach (var item in e)
			{
				result.Add(item);
			}
			foreach (var (i, value) in changes)
			{
				result[i] = value;
			}
			return result.MoveToImmutable();
		}
	}
}