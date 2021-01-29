using System;
using System.Collections.Generic;

namespace ImperitWASM.Server.Services
{
	public interface IDatabase
	{
		void Transaction(Action action);
		void Command(string sql, params object[] args);
		IEnumerable<T0> Query<T0>(string sql, params object[] args);
		IEnumerable<(T0, T1)> Query<T0, T1>(string sql, params object[] args);
		IEnumerable<(T0, T1, T2)> Query<T0, T1, T2>(string sql, params object[] args);
		IEnumerable<(T0, T1, T2, T3)> Query<T0, T1, T2, T3>(string sql, params object[] args);
		IEnumerable<(T0, T1, T2, T3, T4)> Query<T0, T1, T2, T3, T4>(string sql, params object[] args);
		IEnumerable<(T0, T1, T2, T3, T4, T5)> Query<T0, T1, T2, T3, T4, T5>(string sql, params object[] args);
		IEnumerable<(T0, T1, T2, T3, T4, T5, T6)> Query<T0, T1, T2, T3, T4, T5, T6>(string sql, params object[] args);
		IEnumerable<(T0, T1, T2, T3, T4, T5, T6, T7)> Query<T0, T1, T2, T3, T4, T5, T6, T7>(string sql, params object[] args);
		IEnumerable<(T0, T1, T2, T3, T4, T5, T6, T7, T8)> Query<T0, T1, T2, T3, T4, T5, T6, T7, T8>(string sql, params object[] args);
		IEnumerable<(T0, T1, T2, T3, T4, T5, T6, T7, T8, T9)> Query<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(string sql, params object[] args);
		IEnumerable<(T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10)> Query<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(string sql, params object[] args);
		IEnumerable<(T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11)> Query<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(string sql, params object[] args);
		IEnumerable<(T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12)> Query<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(string sql, params object[] args);
		IEnumerable<(T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13)> Query<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(string sql, params object[] args);
		IEnumerable<(T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14)> Query<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(string sql, params object[] args);
		IEnumerable<(T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15)> Query<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(string sql, params object[] args);
		IEnumerable<(T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16)> Query<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(string sql, params object[] args);
		IEnumerable<(T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17)> Query<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17>(string sql, params object[] args);
		IEnumerable<(T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18)> Query<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18>(string sql, params object[] args);
		IEnumerable<(T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19)> Query<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19>(string sql, params object[] args);
	}
}
