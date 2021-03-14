using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

namespace ImperitWASM.Server.Services
{
	public class SqliteDatabase : IDatabase
	{
		readonly SqliteConnection con;
		public SqliteDatabase(string dataSource)
		{
			con = new SqliteConnection("DataSource=" + dataSource + ";Cache=Shared;");
			con.Open();
		}
		public T Transaction<T>(bool use, Func<T> action)
		{
			if (use)
			{
				using var transaction = con.BeginTransaction();
				var result = action();
				transaction.Commit();
				return result;
			}
			else
			{
				return action();
			}
		}
		public void Transaction(bool use, Action action)
		{
			if (use)
			{
				using var transaction = con.BeginTransaction();
				action();
				transaction.Commit();
			}
			else
			{
				action();
			}
		}
		SqliteCommand CreateCommand(string sql, object?[] args)
		{
			var command = con.CreateCommand();
			command.CommandText = sql;
			for (int i = 0; i < args.Length; ++i)
			{
				_ = command.Parameters.AddWithValue("@" + i, args[i] ?? DBNull.Value);
			}
			return command;
		}
		public void Command(string sql, params object?[] args)
		{
			_ = CreateCommand(sql, args).ExecuteNonQuery();
		}
		List<T> Query<T>(string sql, object?[] args, Func<SqliteDataReader, T> convert)
		{
			var command = CreateCommand(sql, args);
			using var reader = command.ExecuteReader();
			var result = new List<T>();
			while (reader.Read())
			{
				object?[] values = new object?[reader.FieldCount];
				_ = reader.GetValues(values);
				result.Add(convert(reader));
			}
			reader.Close();
			return result;
		}
		public static T? GetVal<T>(SqliteDataReader r, int i) => r.IsDBNull(i) ? default : r.GetFieldValue<T>(i);
		public List<T0?> Query<T0>(string sql, params object?[] args) => Query(sql, args, r => GetVal<T0>(r, 0));
		public List<(T0?, T1?)> Query<T0, T1>(string sql, params object?[] args) => Query(sql, args, r => (GetVal<T0>(r, 0), GetVal<T1>(r, 1)));
		public List<(T0?, T1?, T2?)> Query<T0, T1, T2>(string sql, params object?[] args) => Query(sql, args, r => (GetVal<T0>(r, 0), GetVal<T1>(r, 1), GetVal<T2>(r, 2)));
		public List<(T0?, T1?, T2?, T3?)> Query<T0, T1, T2, T3>(string sql, params object?[] args) => Query(sql, args, r => (GetVal<T0>(r, 0), GetVal<T1>(r, 1), GetVal<T2>(r, 2), GetVal<T3>(r, 3)));
		public List<(T0?, T1?, T2?, T3?, T4?)> Query<T0, T1, T2, T3, T4>(string sql, params object?[] args) => Query(sql, args, r => (GetVal<T0>(r, 0), GetVal<T1>(r, 1), GetVal<T2>(r, 2), GetVal<T3>(r, 3), GetVal<T4>(r, 4)));
		public List<(T0?, T1?, T2?, T3?, T4?, T5?)> Query<T0, T1, T2, T3, T4, T5>(string sql, params object?[] args) => Query(sql, args, r => (GetVal<T0>(r, 0), GetVal<T1>(r, 1), GetVal<T2>(r, 2), GetVal<T3>(r, 3), GetVal<T4>(r, 4), GetVal<T5>(r, 5)));
		public List<(T0?, T1?, T2?, T3?, T4?, T5?, T6?)> Query<T0, T1, T2, T3, T4, T5, T6>(string sql, params object?[] args) => Query(sql, args, r => (GetVal<T0>(r, 0), GetVal<T1>(r, 1), GetVal<T2>(r, 2), GetVal<T3>(r, 3), GetVal<T4>(r, 4), GetVal<T5>(r, 5), GetVal<T6>(r, 6)));
		public List<(T0?, T1?, T2?, T3?, T4?, T5?, T6?, T7?)> Query<T0, T1, T2, T3, T4, T5, T6, T7>(string sql, params object?[] args) => Query(sql, args, r => (GetVal<T0>(r, 0), GetVal<T1>(r, 1), GetVal<T2>(r, 2), GetVal<T3>(r, 3), GetVal<T4>(r, 4), GetVal<T5>(r, 5), GetVal<T6>(r, 6), GetVal<T7>(r, 7)));
		public List<(T0?, T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?)> Query<T0, T1, T2, T3, T4, T5, T6, T7, T8>(string sql, params object?[] args) => Query(sql, args, r => (GetVal<T0>(r, 0), GetVal<T1>(r, 1), GetVal<T2>(r, 2), GetVal<T3>(r, 3), GetVal<T4>(r, 4), GetVal<T5>(r, 5), GetVal<T6>(r, 6), GetVal<T7>(r, 7), GetVal<T8>(r, 8)));
		public List<(T0?, T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?)> Query<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(string sql, params object?[] args) => Query(sql, args, r => (GetVal<T0>(r, 0), GetVal<T1>(r, 1), GetVal<T2>(r, 2), GetVal<T3>(r, 3), GetVal<T4>(r, 4), GetVal<T5>(r, 5), GetVal<T6>(r, 6), GetVal<T7>(r, 7), GetVal<T8>(r, 8), GetVal<T9>(r, 9)));
		public List<(T0?, T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?, T10?)> Query<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(string sql, params object?[] args) => Query(sql, args, r => (GetVal<T0>(r, 0), GetVal<T1>(r, 1), GetVal<T2>(r, 2), GetVal<T3>(r, 3), GetVal<T4>(r, 4), GetVal<T5>(r, 5), GetVal<T6>(r, 6), GetVal<T7>(r, 7), GetVal<T8>(r, 8), GetVal<T9>(r, 9), GetVal<T10>(r, 10)));
		public List<(T0?, T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?, T10?, T11?)> Query<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(string sql, params object?[] args) => Query(sql, args, r => (GetVal<T0>(r, 0), GetVal<T1>(r, 1), GetVal<T2>(r, 2), GetVal<T3>(r, 3), GetVal<T4>(r, 4), GetVal<T5>(r, 5), GetVal<T6>(r, 6), GetVal<T7>(r, 7), GetVal<T8>(r, 8), GetVal<T9>(r, 9), GetVal<T10>(r, 10), GetVal<T11>(r, 11)));
		public List<(T0?, T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?, T10?, T11?, T12?)> Query<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(string sql, params object?[] args) => Query(sql, args, r => (GetVal<T0>(r, 0), GetVal<T1>(r, 1), GetVal<T2>(r, 2), GetVal<T3>(r, 3), GetVal<T4>(r, 4), GetVal<T5>(r, 5), GetVal<T6>(r, 6), GetVal<T7>(r, 7), GetVal<T8>(r, 8), GetVal<T9>(r, 9), GetVal<T10>(r, 10), GetVal<T11>(r, 11), GetVal<T12>(r, 12)));
		public List<(T0?, T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?, T10?, T11?, T12?, T13?)> Query<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(string sql, params object?[] args) => Query(sql, args, r => (GetVal<T0>(r, 0), GetVal<T1>(r, 1), GetVal<T2>(r, 2), GetVal<T3>(r, 3), GetVal<T4>(r, 4), GetVal<T5>(r, 5), GetVal<T6>(r, 6), GetVal<T7>(r, 7), GetVal<T8>(r, 8), GetVal<T9>(r, 9), GetVal<T10>(r, 10), GetVal<T11>(r, 11), GetVal<T12>(r, 12), GetVal<T13>(r, 13)));
		public List<(T0?, T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?, T10?, T11?, T12?, T13?, T14?)> Query<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(string sql, params object?[] args) => Query(sql, args, r => (GetVal<T0>(r, 0), GetVal<T1>(r, 1), GetVal<T2>(r, 2), GetVal<T3>(r, 3), GetVal<T4>(r, 4), GetVal<T5>(r, 5), GetVal<T6>(r, 6), GetVal<T7>(r, 7), GetVal<T8>(r, 8), GetVal<T9>(r, 9), GetVal<T10>(r, 10), GetVal<T11>(r, 11), GetVal<T12>(r, 12), GetVal<T13>(r, 13), GetVal<T14>(r, 14)));
		public List<(T0?, T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?, T10?, T11?, T12?, T13?, T14?, T15?)> Query<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(string sql, params object?[] args) => Query(sql, args, r => (GetVal<T0>(r, 0), GetVal<T1>(r, 1), GetVal<T2>(r, 2), GetVal<T3>(r, 3), GetVal<T4>(r, 4), GetVal<T5>(r, 5), GetVal<T6>(r, 6), GetVal<T7>(r, 7), GetVal<T8>(r, 8), GetVal<T9>(r, 9), GetVal<T10>(r, 10), GetVal<T11>(r, 11), GetVal<T12>(r, 12), GetVal<T13>(r, 13), GetVal<T14>(r, 14), GetVal<T15>(r, 15)));
		public List<(T0?, T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?, T10?, T11?, T12?, T13?, T14?, T15?, T16?)> Query<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(string sql, params object?[] args) => Query(sql, args, r => (GetVal<T0>(r, 0), GetVal<T1>(r, 1), GetVal<T2>(r, 2), GetVal<T3>(r, 3), GetVal<T4>(r, 4), GetVal<T5>(r, 5), GetVal<T6>(r, 6), GetVal<T7>(r, 7), GetVal<T8>(r, 8), GetVal<T9>(r, 9), GetVal<T10>(r, 10), GetVal<T11>(r, 11), GetVal<T12>(r, 12), GetVal<T13>(r, 13), GetVal<T14>(r, 14), GetVal<T15>(r, 15), GetVal<T16>(r, 16)));
		public List<(T0?, T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?, T10?, T11?, T12?, T13?, T14?, T15?, T16?, T17?)> Query<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17>(string sql, params object?[] args) => Query(sql, args, r => (GetVal<T0>(r, 0), GetVal<T1>(r, 1), GetVal<T2>(r, 2), GetVal<T3>(r, 3), GetVal<T4>(r, 4), GetVal<T5>(r, 5), GetVal<T6>(r, 6), GetVal<T7>(r, 7), GetVal<T8>(r, 8), GetVal<T9>(r, 9), GetVal<T10>(r, 10), GetVal<T11>(r, 11), GetVal<T12>(r, 12), GetVal<T13>(r, 13), GetVal<T14>(r, 14), GetVal<T15>(r, 15), GetVal<T16>(r, 16), GetVal<T17>(r, 17)));
		public List<(T0?, T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?, T10?, T11?, T12?, T13?, T14?, T15?, T16?, T17?, T18?)> Query<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18>(string sql, params object?[] args) => Query(sql, args, r => (GetVal<T0>(r, 0), GetVal<T1>(r, 1), GetVal<T2>(r, 2), GetVal<T3>(r, 3), GetVal<T4>(r, 4), GetVal<T5>(r, 5), GetVal<T6>(r, 6), GetVal<T7>(r, 7), GetVal<T8>(r, 8), GetVal<T9>(r, 9), GetVal<T10>(r, 10), GetVal<T11>(r, 11), GetVal<T12>(r, 12), GetVal<T13>(r, 13), GetVal<T14>(r, 14), GetVal<T15>(r, 15), GetVal<T16>(r, 16), GetVal<T17>(r, 17), GetVal<T18>(r, 18)));
		public List<(T0?, T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?, T10?, T11?, T12?, T13?, T14?, T15?, T16?, T17?, T18?, T19?)> Query<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19>(string sql, params object?[] args) => Query(sql, args, r => (GetVal<T0>(r, 0), GetVal<T1>(r, 1), GetVal<T2>(r, 2), GetVal<T3>(r, 3), GetVal<T4>(r, 4), GetVal<T5>(r, 5), GetVal<T6>(r, 6), GetVal<T7>(r, 7), GetVal<T8>(r, 8), GetVal<T9>(r, 9), GetVal<T10>(r, 10), GetVal<T11>(r, 11), GetVal<T12>(r, 12), GetVal<T13>(r, 13), GetVal<T14>(r, 14), GetVal<T15>(r, 15), GetVal<T16>(r, 16), GetVal<T17>(r, 17), GetVal<T18>(r, 18), GetVal<T19>(r, 19)));
		public List<(T0?, T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?, T10?, T11?, T12?, T13?, T14?, T15?, T16?, T17?, T18?, T19?, T20?)> Query<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20>(string sql, params object?[] args) => Query(sql, args, r => (GetVal<T0>(r, 0), GetVal<T1>(r, 1), GetVal<T2>(r, 2), GetVal<T3>(r, 3), GetVal<T4>(r, 4), GetVal<T5>(r, 5), GetVal<T6>(r, 6), GetVal<T7>(r, 7), GetVal<T8>(r, 8), GetVal<T9>(r, 9), GetVal<T10>(r, 10), GetVal<T11>(r, 11), GetVal<T12>(r, 12), GetVal<T13>(r, 13), GetVal<T14>(r, 14), GetVal<T15>(r, 15), GetVal<T16>(r, 16), GetVal<T17>(r, 17), GetVal<T18>(r, 18), GetVal<T19>(r, 19), GetVal<T20>(r, 20)));
		public List<(T0?, T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?, T10?, T11?, T12?, T13?, T14?, T15?, T16?, T17?, T18?, T19?, T20?, T21?)> Query<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21>(string sql, params object?[] args) => Query(sql, args, r => (GetVal<T0>(r, 0), GetVal<T1>(r, 1), GetVal<T2>(r, 2), GetVal<T3>(r, 3), GetVal<T4>(r, 4), GetVal<T5>(r, 5), GetVal<T6>(r, 6), GetVal<T7>(r, 7), GetVal<T8>(r, 8), GetVal<T9>(r, 9), GetVal<T10>(r, 10), GetVal<T11>(r, 11), GetVal<T12>(r, 12), GetVal<T13>(r, 13), GetVal<T14>(r, 14), GetVal<T15>(r, 15), GetVal<T16>(r, 16), GetVal<T17>(r, 17), GetVal<T18>(r, 18), GetVal<T19>(r, 19), GetVal<T20>(r, 20), GetVal<T21>(r, 21)));
		public List<(T0?, T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?, T10?, T11?, T12?, T13?, T14?, T15?, T16?, T17?, T18?, T19?, T20?, T21?, T22?)> Query<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22>(string sql, params object?[] args) => Query(sql, args, r => (GetVal<T0>(r, 0), GetVal<T1>(r, 1), GetVal<T2>(r, 2), GetVal<T3>(r, 3), GetVal<T4>(r, 4), GetVal<T5>(r, 5), GetVal<T6>(r, 6), GetVal<T7>(r, 7), GetVal<T8>(r, 8), GetVal<T9>(r, 9), GetVal<T10>(r, 10), GetVal<T11>(r, 11), GetVal<T12>(r, 12), GetVal<T13>(r, 13), GetVal<T14>(r, 14), GetVal<T15>(r, 15), GetVal<T16>(r, 16), GetVal<T17>(r, 17), GetVal<T18>(r, 18), GetVal<T19>(r, 19), GetVal<T20>(r, 20), GetVal<T21>(r, 21), GetVal<T22>(r, 22)));
		public List<(T0?, T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?, T10?, T11?, T12?, T13?, T14?, T15?, T16?, T17?, T18?, T19?, T20?, T21?, T22?, T23?)> Query<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23>(string sql, params object?[] args) => Query(sql, args, r => (GetVal<T0>(r, 0), GetVal<T1>(r, 1), GetVal<T2>(r, 2), GetVal<T3>(r, 3), GetVal<T4>(r, 4), GetVal<T5>(r, 5), GetVal<T6>(r, 6), GetVal<T7>(r, 7), GetVal<T8>(r, 8), GetVal<T9>(r, 9), GetVal<T10>(r, 10), GetVal<T11>(r, 11), GetVal<T12>(r, 12), GetVal<T13>(r, 13), GetVal<T14>(r, 14), GetVal<T15>(r, 15), GetVal<T16>(r, 16), GetVal<T17>(r, 17), GetVal<T18>(r, 18), GetVal<T19>(r, 19), GetVal<T20>(r, 20), GetVal<T21>(r, 21), GetVal<T22>(r, 22), GetVal<T23>(r, 23)));
	}
}