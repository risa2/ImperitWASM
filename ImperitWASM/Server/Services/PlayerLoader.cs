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
		void Set(int gameId, IEnumerable<Player> players, bool fromTransaction);
		Player this[string name] { get; }
		bool IsNameFree(string name);
		string ObsfuscateName(string name, int repetition);
	}
	public class PlayerLoader : IPlayerLoader
	{
		readonly IDatabase db;
		readonly Settings settings;
		readonly ImmutableDictionary<SoldierType, int> type_indices;
		public PlayerLoader(IDatabase db, Settings settings)
		{
			this.db = db;
			this.settings = settings;
			type_indices = settings.GetSoldierTypeIndices();
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

		public void Set(int gameId, IEnumerable<Player> players, bool fromTransaction)
		{
			db.Transaction(!fromTransaction, () =>
			{
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
							long id = db.Query<long>("SELECT last_insert_rowid()").First();
							foreach (var regiment in manoeuvre.Soldiers)
							{
								db.Command("INSERT INTO ActionRegiment (Count, ActionId, Type) VALUES (@x0, @x1, @x2)", regiment.Count, id, type_indices[regiment.Type]);
							}
						}
					}
				}
			});
		}
		public bool IsNameFree(string name) => !name.All(char.IsDigit) && db.Query<int>("SELECT Count(Id) FROM Player WHERE Name=@x0", name).Single() == 0;
		public string ObsfuscateName(string name, int repetition) => name + (db.Query<int>("SELECT MAX(CAST(SUBSTR(Name, LENGTH(@0)) AS INTEGER)) AS NumVal FROM Player WHERE Name LIKE @0 + '%' AND NumVal > 0", name.Trim()).DefaultIfEmpty(0).Max() + 1);
	}
}
