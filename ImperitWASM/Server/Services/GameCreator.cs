using System;
using System.Collections.Immutable;
using System.Threading.Tasks;
using ImperitWASM.Server.Load;
using ImperitWASM.Shared;
using ImperitWASM.Shared.Motion;
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
		readonly IConfig cfg;
		readonly IContextService ctx;
		static readonly ImmutableList<IPlayerAction> Actions = ImmutableList.Create<IPlayerAction>(new Default(), new Instability());
		public GameCreator(IPlayersProvinces pap, IPowers powers, IGameService game, IConfig cfg, IContextService ctx)
		{
			this.pap = pap;
			this.powers = powers;
			this.game = game;
			this.cfg = cfg;
			this.ctx = ctx;
		}

		static Color ColorAt(int i) => Color.Generate(i, 120.0, 1.0, 1.0);

		Player GetRobot(int i, int earnings) => new Robot(ColorAt(i), "AI " + i, cfg.Settings.StartMoney(earnings), true, Actions, cfg.Settings);
		void AddRobot(int gameId, Land start, int land, int i) => pap.Add(gameId, GetRobot(i, start.Earnings), i, land);
		void AddRobots(int gameId, PlayersAndProvinces p_p) => p_p.Inhabitable.Each((start, i) => AddRobot(gameId, start.Value, start.Key, p_p.PlayersCount + i - 1));
		public async Task<int> CreateAsync()
		{
			var g = pap.Add(new PlayersAndProvinces(ImmutableArray.Create<Player>(new Savage()), cfg.Settings.GetProvinces()));
			game.RemoveOld(TimeSpan.FromDays(1));
			await ctx.SaveAsync();
			return g.Id;
		}
		public Color NextColor(int gameId) => ColorAt(pap.PlayersCount(gameId) - 1);
		public Task StartAllAsync()
		{
			foreach (var g in game.ShouldStart)
			{
				var p_p = pap[g.Id];
				AddRobots(g.Id, p_p);
				_ = g.Start().SetActive(p_p.Next(0));
				powers.Add(g.Id, p_p);
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
			var player = new Human(ColorAt(count - 1), name, cfg.Settings.StartMoney(cfg.Settings.Provinces[land].Earnings!.Value), true, Actions, password);
			pap.Add(game.Id, player, count, land);
			_ = count == 2 ? game.StartCountdown() : game;
			return ctx.SaveAsync();
		}
	}
}