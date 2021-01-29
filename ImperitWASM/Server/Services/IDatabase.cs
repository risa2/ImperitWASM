using System;
using System.Collections.Generic;

namespace ImperitWASM.Server.Services
{
	public interface IDatabase
	{
		void Transaction(Action<IDatabase> action);
		void Command(string sql, params (string, object)[] args);
		IEnumerable<object?[]> Query(string sql, params (string, object)[] args);
	}
}
