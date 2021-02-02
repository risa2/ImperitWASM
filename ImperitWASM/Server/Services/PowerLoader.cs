using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ImperitWASM.Shared.Data;

namespace ImperitWASM.Server.Services
{
	public interface IPowersLoader
	{
		void Add(int gameId, IEnumerable<Power> powers, bool fromTransaction);
		ImmutableArray<Powers> Get(int gameId);
	}
	public class PowerLoader : IPowersLoader
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
			int order = db.Query<int>("SELECT Count(Id) FROM Power WHERE GameId = @x0").First();
			foreach (var (i, power) in powers.Select((p, i) => (i, p)))
			{
				db.Command("INSERT INTO Power (GameId, Order, Alive, Final, Income, Money, Soldiers) VALUES (@x0,@x1,@x2,@x3,@x4,@x5,@x6)", gameId, order + i, power.Alive, power.Final, power.Income, power.Money, power.Soldiers);
			}
		});
		public ImmutableArray<Powers> Get(int gameId)
		{
			int player_count = players.Count(gameId);
			return db.Query<int, bool, int, int, int, int>(@"SELECT ""Order"", Alive, Final, Income, Money, Soldiers FROM Power WHERE GameId=@x0 ORDER BY ""Order"" ").GroupBy(p => p.Item1 / player_count).Select(pg => new Powers(pg.Select(p => new Power(p.Item2, p.Item3, p.Item4, p.Item5, p.Item6)).ToImmutableArray())).ToImmutableArray();
		}
	}
}
