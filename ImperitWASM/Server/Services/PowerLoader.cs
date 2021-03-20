using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ImperitWASM.Shared.Data;

namespace ImperitWASM.Server.Services
{
	public interface IPowerLoader
	{
		void Add(int gameId, IEnumerable<Power> powers, bool fromTransaction);
		ImmutableArray<Powers> Get(int gameId);
	}
	public class PowerLoader : IPowerLoader
	{
		readonly IDatabase db;
		readonly IPlayerLoader players;
		public PowerLoader(IDatabase db, IPlayerLoader players)
		{
			this.db = db;
			this.players = players;
		}

		public void Add(int gameId, IEnumerable<Power> powers, bool fromTransaction) => db.Transaction(!fromTransaction, () =>
		{
			int order = db.Query<int>("SELECT COUNT(*) FROM Power WHERE GameId = @0", gameId).First();
			foreach (var power in powers)
			{
				db.Command(@"INSERT INTO Power (GameId, ""Order"", Alive, Final, Income, Money, Soldiers) VALUES (@0,@1,@2,@3,@4,@5,@6)", gameId, order, power.Alive, power.Final, power.Income, power.Money, power.Soldiers);
				++order;
			}
		});
		public ImmutableArray<Powers> Get(int gameId)
		{
			int player_count = players.Count(gameId);
			return db.Query<int, bool, int, int, int, int>(@"SELECT ""Order"", Alive, Final, Income, Money, Soldiers FROM Power WHERE GameId=@0 ORDER BY ""Order"" ", gameId).GroupBy(p => p.Item1 / player_count).Select(pg => new Powers(pg.Select(p => new Power(p.Item2, p.Item3, p.Item4, p.Item5, p.Item6)).ToImmutableArray())).ToImmutableArray();
		}
	}
}
