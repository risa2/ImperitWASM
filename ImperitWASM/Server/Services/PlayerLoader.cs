using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ImperitWASM.Shared.Config;
using ImperitWASM.Shared.Data;

namespace ImperitWASM.Server.Services
{
	public interface IPlayerLoader
	{
		ImmutableArray<Player> this[int gameId] { get; }
		void Set(int gameId, IEnumerable<Player> players);
		Player this[string name] { get; }
	}
	public class PlayerLoader : IPlayerLoader
	{
		readonly IDatabase db;
		readonly Settings settings;
		public PlayerLoader(IDatabase db, Settings settings)
		{
			this.db = db;
			this.settings = settings;
		}

		IEnumerable<Player> Get(string where, object arg) => db.Query<string, bool, bool, bool, int, string, int?, int?, int?, string?, int?, int?>(@"
			SELECT Player.Name, Player.Active, Player.Alive, Player.Human, Player.Money, Player.Password,
					Action.Id, Action.Debt, Action.Province, Action.Type, ActionRegiment.Count, ActionRegiment.Type FROM Players
			LEFT JOIN Action ON Action.PlayerName=Player.Name
			LEFT JOIN Regiment ON Regiment.ActionId=Action.Id ORDER BY Player.""Order""
			WHERE " + where, arg).GroupBy(p => p.Item1).Select((group, i) =>
			{
				var player = group.First();
				var actions = group.Where(g => g.Item9 is not null).GroupBy(g => g.Item9!.Value).Select(reg =>
				{
					var action = reg.First();
					return action.Item10 switch
					{
						"Loan" => new Loan(action.Item8!.Value) as IAction,
						"Manoeuvre" => new Manoeuvre(action.Item9!.Value, new Soldiers(reg.Select(r => new Regiment(settings.SoldierTypes[r.Item12!.Value], r.Item11!.Value)))),
						var x => throw new InvalidOperationException(x + " is not a valid Action type")
					};
				}).ToImmutableList();
				return new Player(Settings.ColorOf(i), player.Item1, player.Item5, player.Item3, actions, settings, player.Item4, new Password(player.Item6), player.Item2);
			});
		public ImmutableArray<Player> this[int gameId] => Get("GameId=@x0", gameId).ToImmutableArray();
		public Player this[string name] => Get("Name=@x0", name).First();

		public void Set(int gameId, IEnumerable<Player> players)
		{
			var st_indices = settings.GetSoldierTypeIndices();
			db.Command("DELETE FROM Players WHERE GameId=@x0", gameId);
			foreach (var (i, player) in players.Select((p, i) => (i, p)))
			{
				db.Command("INSERT INTO Players (Name,Active,Alive,GameId,Human,Money,Order,Password) VALUES (@x0,@x1,@x2,@x3,@x4,@x5,@x6,@x7)",
				player.Name, player.Active, player.Alive, gameId, player.Human, player.Money, i, player.Password.ToString());

				foreach (var action in player.Actions)
				{
					if (action is Manoeuvre manoeuvre)
					{
						db.Command("INSERT INTO Actions (PlayerName, Province, Type) VALUES (@x0,@x1,@x2)", player.Name, manoeuvre.Province, "Manoeuvre");
						var id = db.Query<long>("SELECT last_insert_rowid()");
						foreach (var regiment in manoeuvre.Soldiers)
						{
							db.Command("INSERT INTO ActionRegiment (Count, ActionId, Type) VALUES (@x0, @x1, @x2)", regiment.Count, id, st_indices[regiment.Type]);
						}
					}
				}
			}
		}
	}
}
