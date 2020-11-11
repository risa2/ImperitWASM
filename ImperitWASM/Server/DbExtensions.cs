using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ImperitWASM.Server.Load;
using ImperitWASM.Shared;
using Microsoft.EntityFrameworkCore;

namespace ImperitWASM.Server
{
	public static class DbExtensions
	{
		public static void UpdateAt<T>(this DbSet<T> set, int id, Action<T> mod) where T : class, IEntity, new() => mod(set.Attach(new T { Id = id }).Entity);
		public static void UpdateAt<T>(this DbSet<T> set, Expression<Func<T, bool>> cond, Action<T> mod) where T : class => set.Where(cond).ToArray().Each(mod);
		public static void RemoveAt<T>(this DbSet<T> set, int id) where T : class, IEntity, new() => set.Remove(set.Attach(new T { Id = id }).Entity);
		public static void RemoveAt<T>(this DbSet<T> set, Expression<Func<T, bool>> cond) where T : class => set.RemoveRange(set.Where(cond).ToArray());
		public static T FirstIfOrFirst<T>(this IEnumerable<T> e, Func<T, bool> cond) => e.Where(cond).DefaultIfEmpty(e.FirstOrDefault()).First();
	}
}