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

		IEnumerable<Player> Get(string where, object? arg, object? arg2 = null)
		{
			Player? player = null;
			Manoeuvre? manoeuvre = null;

			foreach (var (name, active, alive, human, money, password, debt, province, action, count, type, game_id, order) in db.Query<string, bool, bool, bool, int, string, int?, int?, string?, int?, int?, int, int>(@"
				SELECT Player.Name, Player.Active, Player.Alive, Player.Human, Player.Money, Player.Password,
					Action.Debt, Action.Province, Action.Type, ActionRegiment.Count, ActionRegiment.Type,
					Player.GameId, Player.""Order"" FROM Player
				LEFT JOIN Action ON Action.GameId=Player.GameId AND Action.Player=Player.""Order""
				LEFT JOIN ActionRegiment ON ActionRegiment.ActionId=Action.Id WHERE " + where + @"
				ORDER BY Player.""Order"" ", arg, arg2))
			{
				if (player is not null && player.Order != order)
				{
					yield return manoeuvre is null ? player : player.Add(manoeuvre);
					player = null;
					manoeuvre = null;
				}
				if (player is null)
				{
					player = new Player(new PlayerIdentity(name!, order, game_id, human), money, alive, ImmutableList<IAction>.Empty, settings, Password.FromHash(password!), active);
				}

				player = action == "Loan" && debt is int Debt ? player.Add(new Loan(Debt)) : player;
					
				if (action == "Manoeuvre" && province is int Province && type is int Type && count is int Count)
				{
					if (manoeuvre is not null && manoeuvre.Province != province)
					{
						player = player.Add(manoeuvre);
						manoeuvre = null;
					}
					if (manoeuvre is null)
					{
						manoeuvre = new Manoeuvre(Province, new Soldiers());
					}
					manoeuvre = new Manoeuvre(Province, manoeuvre.Soldiers.Add(new Soldiers(settings.SoldierTypes[Type], Count)));
				}
			}
			if (player is not null)
			{
				yield return player;
			}
		}

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
		public bool IsNameFree(string name) => !name.Any(char.IsDigit) && db.Query<int>("SELECT COUNT(*) FROM Player WHERE Name=@0", name).Single() == 0;
		public string ObsfuscateName(string name, int repetition) => name + ((db.Query<int>("SELECT COUNT(*) FROM Player WHERE Name LIKE @0", name + "%").First() + repetition) is > 0 and int n ? n.ToString() : "");
		public int Count(int gameId) => db.Query<int>("SELECT COUNT(*) FROM Player WHERE GameId = @0", gameId).First();
	}
}
