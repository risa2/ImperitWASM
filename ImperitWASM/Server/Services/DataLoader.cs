using System;
using System.Collections.Generic;
using ImperitWASM.Shared.Data;

namespace ImperitWASM.Server.Services
{
	public interface IDataLoader
	{
		T Transaction<T>(int gameId, Func<IReadOnlyList<Player>, Provinces, Game, (IEnumerable<Player>, IEnumerable<Province>, T)> update);
	}
	public class DataLoader : IDataLoader
	{
	}
}
