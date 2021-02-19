using System;
using ImperitWASM.Shared.Data;
using ImperitWASM.Shared.Config;
using ImperitWASM.Shared;
using System.Linq;
using System.Collections.Immutable;

namespace ImperitWASM.Server.Services
{
	public interface IGameCreator
	{
		Color NextColor(int gameId);
		int Create();
		void StartAll();
		void Register(int gameId, string name, Password password, int land);
	}
	public class GameCreator : IGameCreator
	{
		readonly IProvinceLoader province_load;
		readonly IPlayerLoader player_load;
		readonly IGameLoader game_load;
		readonly Settings settings;
		readonly IDatabase db;
		public GameCreator(IProvinceLoader province_load, IGameLoader game_load, IPlayerLoader player_load, Settings settings, IDatabase db)
		{
			this.province_load = province_load;
			this.game_load = game_load;
			this.player_load = player_load;
			this.settings = settings;
			this.db = db;
		}
		public int Create() => db.Transaction(true, () =>
		{
			int gameId = game_load.Add(Game.Create);
			game_load.RemoveOld(DateTimeOffset.UtcNow.AddDays(-1.0));
			province_load.Set(gameId, settings.Provinces, fromTransaction: true);
			return gameId;
		});
		public Color NextColor(int gameId) => PlayerIdentity.ColorOf(player_load.Count(gameId));
		void Start(int gameId, bool fromTransaction) => db.Transaction(!fromTransaction, () =>
		{
			var players = player_load[gameId];
			var provinces = province_load[gameId];
			var robots = settings.CreateRobots(gameId, players.Length, provinces.Inhabitable, player_load.ObsfuscateName).ToImmutableArray();

			player_load.Set(gameId, players.AddRange(robots.Select(r => r.Item2)), true);
			province_load.Set(gameId, provinces.Alter(robots.Select(r => (r.Item1, provinces[r.Item1].RuledBy(r.Item2.Id)))), true);
			game_load[gameId] = Game.Start;
		});
		public void StartAll() => game_load.ShouldStart.Each(x => Start(x, false));
		public void Register(int gameId, string name, Password password, int land) => db.Transaction(true, () =>
		{
			var players = player_load[gameId];
			var player = settings.CreatePlayer(gameId, players.Length, name, land, password, true);
			player_load.Set(gameId, players.Add(player), true);
			province_load.Set(gameId, land, province_load[gameId, land].RuledBy(player.Id), true);

			if (players.Length + 1 == 2)
			{
				game_load[gameId] = Game.CountDown(settings.CountdownTime);
			}
			if (players.Length + 1 >= settings.PlayerCount)
			{
				Start(gameId, true);
			}
		});
	}
}