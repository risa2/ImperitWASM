using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ImperitWASM.Shared.Func;
using System.Linq;

namespace ImperitWASM.Server
{
	public static class DbExtensions
	{
		public static void UpdateAt<T>(this DbSet<T> set, Func<T, bool> cond, Action<T> mod) where T : class => set.Where(cond).ToArray().Each(mod);
		public static void RemoveAt<T>(this DbSet<T> set, Func<T, bool> cond) where T : class => set.RemoveRange(set.Where(cond).ToArray());
		public static T FirstIfOrFirst<T>(this IEnumerable<T> e, Func<T, bool> cond) => e.Where(cond).DefaultIfEmpty(e.FirstOrDefault()).First();
	}
}