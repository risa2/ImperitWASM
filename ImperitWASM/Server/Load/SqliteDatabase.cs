using System;
using System.Collections.Generic;
using ImperitWASM.Server.Services;
using Microsoft.Data.Sqlite;

namespace ImperitWASM.Server.Load
{
	public class SqliteDatabase : IDatabase
	{
		readonly SqliteConnection con;
		public SqliteDatabase(string dataSource)
		{
			con = new SqliteConnection($"Data Source={dataSource};Cache=Shared");
			con.Open();
		}
		public T Transaction<T>(bool isTransaction, Func<T> action)
		{
			if (isTransaction)
			{
				using var transaction = con.BeginTransaction();
				return action();
			}
			else
			{
				return action();
			}
		}
		public void Transaction(bool isTransaction, Action action)
		{
			_ = Transaction(isTransaction, () =>
			{
				action();
				return 0;
			});
		}
		SqliteCommand CreateCommand(string sql, object?[] args)
		{
			var command = con.CreateCommand();
			command.CommandText = sql;
			for (int i = 0; i < args.Length; ++i)
			{
				_ = command.Parameters.AddWithValue("@x" + i, args[i]);
			}
			return command;
		}
		public void Command(string sql, params object?[] args)
		{
			_ = CreateCommand(sql, args).ExecuteNonQuery();
		}
		IEnumerable<T> Query<T>(string sql, object?[] args, Func<SqliteDataReader, T> convert)
		{
			var command = CreateCommand(sql, args);
			var reader = command.ExecuteReader();
			while (reader.Read())
			{
				object?[] values = new object?[reader.FieldCount];
				_ = reader.GetValues(values);
				yield return convert(reader);
			}
		}
		public IEnumerable<T0> Query<T0>(string sql, params object?[] args) => Query(sql, args, r => r.GetFieldValue<T0>(0));
		public IEnumerable<(T0, T1)> Query<T0, T1>(string sql, params object?[] args) => Query(sql, args, r => (r.GetFieldValue<T0>(0), r.GetFieldValue<T1>(1)));
		public IEnumerable<(T0, T1, T2)> Query<T0, T1, T2>(string sql, params object?[] args) => Query(sql, args, r => (r.GetFieldValue<T0>(0), r.GetFieldValue<T1>(1), r.GetFieldValue<T2>(2)));
		public IEnumerable<(T0, T1, T2, T3)> Query<T0, T1, T2, T3>(string sql, params object?[] args) => Query(sql, args, r => (r.GetFieldValue<T0>(0), r.GetFieldValue<T1>(1), r.GetFieldValue<T2>(2), r.GetFieldValue<T3>(3)));
		public IEnumerable<(T0, T1, T2, T3, T4)> Query<T0, T1, T2, T3, T4>(string sql, params object?[] args) => Query(sql, args, r => (r.GetFieldValue<T0>(0), r.GetFieldValue<T1>(1), r.GetFieldValue<T2>(2), r.GetFieldValue<T3>(3), r.GetFieldValue<T4>(4)));
		public IEnumerable<(T0, T1, T2, T3, T4, T5)> Query<T0, T1, T2, T3, T4, T5>(string sql, params object?[] args) => Query(sql, args, r => (r.GetFieldValue<T0>(0), r.GetFieldValue<T1>(1), r.GetFieldValue<T2>(2), r.GetFieldValue<T3>(3), r.GetFieldValue<T4>(4), r.GetFieldValue<T5>(5)));
		public IEnumerable<(T0, T1, T2, T3, T4, T5, T6)> Query<T0, T1, T2, T3, T4, T5, T6>(string sql, params object?[] args) => Query(sql, args, r => (r.GetFieldValue<T0>(0), r.GetFieldValue<T1>(1), r.GetFieldValue<T2>(2), r.GetFieldValue<T3>(3), r.GetFieldValue<T4>(4), r.GetFieldValue<T5>(5), r.GetFieldValue<T6>(6)));
		public IEnumerable<(T0, T1, T2, T3, T4, T5, T6, T7)> Query<T0, T1, T2, T3, T4, T5, T6, T7>(string sql, params object?[] args) => Query(sql, args, r => (r.GetFieldValue<T0>(0), r.GetFieldValue<T1>(1), r.GetFieldValue<T2>(2), r.GetFieldValue<T3>(3), r.GetFieldValue<T4>(4), r.GetFieldValue<T5>(5), r.GetFieldValue<T6>(6), r.GetFieldValue<T7>(7)));
		public IEnumerable<(T0, T1, T2, T3, T4, T5, T6, T7, T8)> Query<T0, T1, T2, T3, T4, T5, T6, T7, T8>(string sql, params object?[] args) => Query(sql, args, r => (r.GetFieldValue<T0>(0), r.GetFieldValue<T1>(1), r.GetFieldValue<T2>(2), r.GetFieldValue<T3>(3), r.GetFieldValue<T4>(4), r.GetFieldValue<T5>(5), r.GetFieldValue<T6>(6), r.GetFieldValue<T7>(7), r.GetFieldValue<T8>(8)));
		public IEnumerable<(T0, T1, T2, T3, T4, T5, T6, T7, T8, T9)> Query<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(string sql, params object?[] args) => Query(sql, args, r => (r.GetFieldValue<T0>(0), r.GetFieldValue<T1>(1), r.GetFieldValue<T2>(2), r.GetFieldValue<T3>(3), r.GetFieldValue<T4>(4), r.GetFieldValue<T5>(5), r.GetFieldValue<T6>(6), r.GetFieldValue<T7>(7), r.GetFieldValue<T8>(8), r.GetFieldValue<T9>(9)));
		public IEnumerable<(T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10)> Query<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(string sql, params object?[] args) => Query(sql, args, r => (r.GetFieldValue<T0>(0), r.GetFieldValue<T1>(1), r.GetFieldValue<T2>(2), r.GetFieldValue<T3>(3), r.GetFieldValue<T4>(4), r.GetFieldValue<T5>(5), r.GetFieldValue<T6>(6), r.GetFieldValue<T7>(7), r.GetFieldValue<T8>(8), r.GetFieldValue<T9>(9), r.GetFieldValue<T10>(10)));
		public IEnumerable<(T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11)> Query<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(string sql, params object?[] args) => Query(sql, args, r => (r.GetFieldValue<T0>(0), r.GetFieldValue<T1>(1), r.GetFieldValue<T2>(2), r.GetFieldValue<T3>(3), r.GetFieldValue<T4>(4), r.GetFieldValue<T5>(5), r.GetFieldValue<T6>(6), r.GetFieldValue<T7>(7), r.GetFieldValue<T8>(8), r.GetFieldValue<T9>(9), r.GetFieldValue<T10>(10), r.GetFieldValue<T11>(11)));
		public IEnumerable<(T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12)> Query<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(string sql, params object?[] args) => Query(sql, args, r => (r.GetFieldValue<T0>(0), r.GetFieldValue<T1>(1), r.GetFieldValue<T2>(2), r.GetFieldValue<T3>(3), r.GetFieldValue<T4>(4), r.GetFieldValue<T5>(5), r.GetFieldValue<T6>(6), r.GetFieldValue<T7>(7), r.GetFieldValue<T8>(8), r.GetFieldValue<T9>(9), r.GetFieldValue<T10>(10), r.GetFieldValue<T11>(11), r.GetFieldValue<T12>(12)));
		public IEnumerable<(T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13)> Query<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(string sql, params object?[] args) => Query(sql, args, r => (r.GetFieldValue<T0>(0), r.GetFieldValue<T1>(1), r.GetFieldValue<T2>(2), r.GetFieldValue<T3>(3), r.GetFieldValue<T4>(4), r.GetFieldValue<T5>(5), r.GetFieldValue<T6>(6), r.GetFieldValue<T7>(7), r.GetFieldValue<T8>(8), r.GetFieldValue<T9>(9), r.GetFieldValue<T10>(10), r.GetFieldValue<T11>(11), r.GetFieldValue<T12>(12), r.GetFieldValue<T13>(13)));
		public IEnumerable<(T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14)> Query<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(string sql, params object?[] args) => Query(sql, args, r => (r.GetFieldValue<T0>(0), r.GetFieldValue<T1>(1), r.GetFieldValue<T2>(2), r.GetFieldValue<T3>(3), r.GetFieldValue<T4>(4), r.GetFieldValue<T5>(5), r.GetFieldValue<T6>(6), r.GetFieldValue<T7>(7), r.GetFieldValue<T8>(8), r.GetFieldValue<T9>(9), r.GetFieldValue<T10>(10), r.GetFieldValue<T11>(11), r.GetFieldValue<T12>(12), r.GetFieldValue<T13>(13), r.GetFieldValue<T14>(14)));
		public IEnumerable<(T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15)> Query<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(string sql, params object?[] args) => Query(sql, args, r => (r.GetFieldValue<T0>(0), r.GetFieldValue<T1>(1), r.GetFieldValue<T2>(2), r.GetFieldValue<T3>(3), r.GetFieldValue<T4>(4), r.GetFieldValue<T5>(5), r.GetFieldValue<T6>(6), r.GetFieldValue<T7>(7), r.GetFieldValue<T8>(8), r.GetFieldValue<T9>(9), r.GetFieldValue<T10>(10), r.GetFieldValue<T11>(11), r.GetFieldValue<T12>(12), r.GetFieldValue<T13>(13), r.GetFieldValue<T14>(14), r.GetFieldValue<T15>(15)));
		public IEnumerable<(T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16)> Query<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(string sql, params object?[] args) => Query(sql, args, r => (r.GetFieldValue<T0>(0), r.GetFieldValue<T1>(1), r.GetFieldValue<T2>(2), r.GetFieldValue<T3>(3), r.GetFieldValue<T4>(4), r.GetFieldValue<T5>(5), r.GetFieldValue<T6>(6), r.GetFieldValue<T7>(7), r.GetFieldValue<T8>(8), r.GetFieldValue<T9>(9), r.GetFieldValue<T10>(10), r.GetFieldValue<T11>(11), r.GetFieldValue<T12>(12), r.GetFieldValue<T13>(13), r.GetFieldValue<T14>(14), r.GetFieldValue<T15>(15), r.GetFieldValue<T16>(16)));
		public IEnumerable<(T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17)> Query<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17>(string sql, params object?[] args) => Query(sql, args, r => (r.GetFieldValue<T0>(0), r.GetFieldValue<T1>(1), r.GetFieldValue<T2>(2), r.GetFieldValue<T3>(3), r.GetFieldValue<T4>(4), r.GetFieldValue<T5>(5), r.GetFieldValue<T6>(6), r.GetFieldValue<T7>(7), r.GetFieldValue<T8>(8), r.GetFieldValue<T9>(9), r.GetFieldValue<T10>(10), r.GetFieldValue<T11>(11), r.GetFieldValue<T12>(12), r.GetFieldValue<T13>(13), r.GetFieldValue<T14>(14), r.GetFieldValue<T15>(15), r.GetFieldValue<T16>(16), r.GetFieldValue<T17>(17)));
		public IEnumerable<(T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18)> Query<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18>(string sql, params object?[] args) => Query(sql, args, r => (r.GetFieldValue<T0>(0), r.GetFieldValue<T1>(1), r.GetFieldValue<T2>(2), r.GetFieldValue<T3>(3), r.GetFieldValue<T4>(4), r.GetFieldValue<T5>(5), r.GetFieldValue<T6>(6), r.GetFieldValue<T7>(7), r.GetFieldValue<T8>(8), r.GetFieldValue<T9>(9), r.GetFieldValue<T10>(10), r.GetFieldValue<T11>(11), r.GetFieldValue<T12>(12), r.GetFieldValue<T13>(13), r.GetFieldValue<T14>(14), r.GetFieldValue<T15>(15), r.GetFieldValue<T16>(16), r.GetFieldValue<T17>(17), r.GetFieldValue<T18>(18)));
		public IEnumerable<(T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19)> Query<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19>(string sql, params object?[] args) => Query(sql, args, r => (r.GetFieldValue<T0>(0), r.GetFieldValue<T1>(1), r.GetFieldValue<T2>(2), r.GetFieldValue<T3>(3), r.GetFieldValue<T4>(4), r.GetFieldValue<T5>(5), r.GetFieldValue<T6>(6), r.GetFieldValue<T7>(7), r.GetFieldValue<T8>(8), r.GetFieldValue<T9>(9), r.GetFieldValue<T10>(10), r.GetFieldValue<T11>(11), r.GetFieldValue<T12>(12), r.GetFieldValue<T13>(13), r.GetFieldValue<T14>(14), r.GetFieldValue<T15>(15), r.GetFieldValue<T16>(16), r.GetFieldValue<T17>(17), r.GetFieldValue<T18>(18), r.GetFieldValue<T19>(19)));
	}
}