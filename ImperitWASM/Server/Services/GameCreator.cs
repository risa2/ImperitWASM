using System;
using System.Threading.Tasks;
using ImperitWASM.Server.Load;
using ImperitWASM.Shared.Data;
using ImperitWASM.Shared.Config;
using ImperitWASM.Shared;

namespace ImperitWASM.Server.Services
{
	public interface IGameCreator
	{
		Color NextColor(int gameId);
		Task<int> CreateAsync();
		Task StartAllAsync();
		Task RegisterAsync(Game game, string name, Password password, int land);
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
		public async Task<int> CreateAsync()
		{
			int gameId = 0;
			db.Transaction(true, () =>
			{
				gameId = game_load.Add(Game.Create);
				game_load.RemoveOld(DateTimeOffset.UtcNow.AddDays(-1.0));
				province_load.Set(gameId, settings.Provinces, fromTransaction: true);
			});
			return gameId;
		}
		public Color NextColor(int gameId) => Settings.ColorOf(province_load.PlayersCount(gameId) - 1);
		void Start(Game g)
		{
			var p_p = province_load[g.Id] = province_load[g.Id].AddRobots(settings, settings.GetNames(province_load.ObsfuscateName));
			ctx.Add(g.Start().SetActive(p_p.Next(0)).Id, p_p.PlayersPower);
		}
		public Task StartAllAsync()
		{
			game_load.ShouldStart.Each(Start);
			return ctx.SaveAsync();
		}
		public Task RegisterAsync(Game game, string name, Password password, int land)
		{
			int count = province_load.PlayersCount(game.Id);
			province_load.Add(game.Id, settings.CreateHuman(count, name, land, password), count, land);
			_ = count == 2 ? game.StartCountdown() : game;
			if (count + 1 >= settings.PlayerCount)
			{
				Start(game);
			}
			return ctx.SaveAsync();
		}
	}
}