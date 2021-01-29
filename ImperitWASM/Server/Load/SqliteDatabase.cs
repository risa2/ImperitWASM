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
		public void Transaction(Action<IDatabase> action)
		{
			using var transaction = con.BeginTransaction();
			action(this);
		}
		SqliteCommand CreateCommand(string sql, (string, object)[] args)
		{
			var command = con.CreateCommand();
			command.CommandText = sql;
			for (int i = 0; i < args.Length; ++i)
			{
				_ = command.Parameters.AddWithValue(args[i].Item1, args[i].Item2);
			}
			return command;
		}
		public void Command(string sql, params (string, object)[] args)
		{
			_ = CreateCommand(sql, args).ExecuteNonQuery();
		}
		public IEnumerable<object?[]> Query(string sql, params (string, object)[] args)
		{
			var command = CreateCommand(sql, args);
			var reader = command.ExecuteReader();
			while (reader.Read())
			{
				var values = new object?[reader.FieldCount];
				_ = reader.GetValues(values);
				yield return values;
			}
		}
	}
}
