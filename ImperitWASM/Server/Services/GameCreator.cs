using System;
using System.Collections.Immutable;
using System.Threading.Tasks;
using ImperitWASM.Server.Load;
using ImperitWASM.Shared.Data;
using ImperitWASM.Shared.Config;

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
		readonly IPlayersProvinces pap;
		readonly IPowers powers;
		readonly IGameService game;
		readonly Settings settings;
		readonly IContextService ctx;
		public GameCreator(IPlayersProvinces pap, IPowers powers, IGameService game, Settings settings, IContextService ctx)
		{
			this.pap = pap;
			this.powers = powers;
			this.game = game;
			this.settings = settings;
			this.ctx = ctx;
		}
		public async Task<int> CreateAsync()
		{
			var g = pap.Add(settings.GetPlayersAndProvinces());
			game.RemoveOld(TimeSpan.FromDays(1));
			await ctx.SaveAsync();
			return g.Id;
		}
		public Color NextColor(int gameId) => Settings.ColorOf(pap.PlayersCount(gameId) - 1);
		public Task StartAllAsync()
		{
			foreach (var g in game.ShouldStart)
			{
				var p_p = pap[g.Id] = pap[g.Id].AddRobots(settings, settings.GetNames(pap.ObsfuscateName));
				powers.Add(g.Start().SetActive(p_p.Next(0)).Id, p_p);
			}
			return ctx.SaveAsync();
		}
		public Task RegisterAsync(Game game, string name, Password password, int land)
		{
			int count = pap.PlayersCount(game.Id);
			pap.Add(game.Id, settings.CreateHuman(count, name, land, password), count, land);
			_ = count == 2 ? game.StartCountdown() : game;
			return ctx.SaveAsync();
		}
	}
}