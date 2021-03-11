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
		Player? this[string name] { get; }
		Player this[int gameId, int order] { get; }
		void Set(int gameId, IEnumerable<Player> players, bool fromTransaction);
		bool IsNameFree(string name);
		string ObsfuscateName(string name, int repetition);
		int Count(int gameId);
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

		IEnumerable<Player> Get(string where, object? arg, object? arg2 = null) => db.Query<string, bool, bool, bool, int, string, int?, int?, int?, string?, int?, int?, int, int>(@"
			SELECT Player.Name, Player.Active, Player.Alive, Player.Human, Player.Money, Player.Password,
					Action.Id, Action.Debt, Action.Province, Action.Type, ActionRegiment.Count, ActionRegiment.Type,
					Player.GameId, Player.""Order"" FROM Player
			LEFT JOIN Action ON Action.GameId=Player.GameId AND Action.Player=Player.""Order""
			LEFT JOIN ActionRegiment ON ActionRegiment.ActionId=Action.Id WHERE " + where + @"
			ORDER BY Player.""Order""", arg, arg2).GroupBy(p => p.Item1).Select((group, i) =>
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
				return new Player(new PlayerIdentity(player.Item1, player.Item14, player.Item13, player.Item4), player.Item5, player.Item3, actions, settings, Password.FromHash(player.Item6), player.Item2);
			});
		public ImmutableArray<Player> this[int gameId] => Get("Player.GameId=@0", gameId).ToImmutableArray();
		public Player this[int gameId, int order] => Get(@"Player.GameId=@0 AND Player.""Order""=@1", gameId, order).First();
		public Player? this[string name] => Get("Player.Name=@0", name).FirstOrDefault();

		void Add(int gameId, int order, Player player)
		{
			db.Command(@"INSERT INTO Player (Name,Active,Alive,GameId,Human,Money,""Order"",Password) VALUES (@0,@1,@2,@3,@4,@5,@6,@7)",
				player.Name, player.Active, player.Alive, gameId, player.Human, player.Money, order, player.Password.ToString());

			foreach (var action in player.Actions)
			{
				if (action is Manoeuvre manoeuvre)
				{
					db.Command("INSERT INTO Action (GameId,Player,Province,Type) VALUES (@0,@1,@2,@3)", gameId, player.Order, manoeuvre.Province, "Manoeuvre");
					long id = db.Query<long>("SELECT last_insert_rowid()").First();
					foreach (var regiment in manoeuvre.Soldiers)
					{
						db.Command(@"INSERT INTO ActionRegiment (""Count"", ActionId, Type) VALUES (@0, @1, @2)", regiment.Count, id, type_indices[regiment.Type]);
					}
				}
				else if (action is Loan loan)
				{
					db.Command("INSERT INTO Action (GameId,Player,Debt,Type) VALUES (@0,@1,@2,@3)", gameId, player.Order, loan.Debt, "Loan");
				}
			}
		}

		public void Set(int gameId, IEnumerable<Player> players, bool fromTransaction) => db.Transaction(!fromTransaction, () =>
		{
			db.Command("DELETE FROM Player WHERE GameId=@0", gameId);
			foreach (var (i, player) in players.Select((p, i) => (i, p)))
			{
				Add(gameId, i, player);
			}
		});
		public bool IsNameFree(string name) => !name.Any(char.IsDigit) && db.Query<int>("SELECT Count(Name) FROM Player WHERE Name=@0", name).Single() == 0;
		public string ObsfuscateName(string name, int repetition) => name + ((db.Query<int>("SELECT Count(Name) FROM Player WHERE Name LIKE @0", name + "%").First() + repetition) is > 0 and int n ? n.ToString() : "");
		public int Count(int gameId) => db.Query<int>("SELECT Count(Name) FROM Player WHERE GameId = @0", gameId).First();
	}
}
