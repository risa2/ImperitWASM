using System;
using System.Collections.Immutable;
using System.Threading.Tasks;
using ImperitWASM.Server.Load;
using ImperitWASM.Shared.State;

namespace ImperitWASM.Server.Services
{
	public interface IGameCreator
	{
		Color NextColor(int gameId);
		Task<int> CreateAsync();
		Task StartAllAsync();
		Task FinishAsync(int gameId);
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
			var g = pap.Add(new PlayersAndProvinces(ImmutableArray.Create<Player>(settings.Savage), settings.GetProvinces()));
			game.RemoveOld(TimeSpan.FromDays(1));
			await ctx.SaveAsync();
			return g.Id;
		}
		public Color NextColor(int gameId) => Settings.ColorOf(pap.PlayersCount(gameId) - 1);
		public Task StartAllAsync()
		{
			foreach (var g in game.ShouldStart)
			{
				var p_p = pap[g.Id] = pap[g.Id].AddRobots(settings, pap.ObsfuscateName);
				powers.Add(g.Start().SetActive(p_p.Next(0)).Id, p_p);
			}
			return ctx.SaveAsync();
		}
		public Task FinishAsync(int gameId)
		{
			game.Finish(gameId);
			return ctx.SaveAsync();
		}
		public Task RegisterAsync(Game game, string name, Password password, int land)
		{
			int count = pap.PlayersCount(game.Id);
			var player = Human.Create(Settings.ColorOf(count - 1), name, settings.StartMoney(land), settings, password);
			pap.Add(game.Id, player, count, land);
			_ = count == 2 ? game.StartCountdown() : game;
			return ctx.SaveAsync();
		}
	}
}