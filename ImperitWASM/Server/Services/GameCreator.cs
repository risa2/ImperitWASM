using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using ImperitWASM.Server.Load;
using ImperitWASM.Shared;
using ImperitWASM.Shared.Motion;
using ImperitWASM.Shared.State;

namespace ImperitWASM.Server.Services
{
	public interface INewGame
	{
		Color NextColor(int gameId);
		Task<int> CreateAsync();
		Task StartAllAsync();
		Task FinishAsync(int gameId);
		Task RegisterAsync(Game game, string name, Password password, int land);
	}
	public class GameCreator : INewGame
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

		static Color ColorAt(int i) => new Color(120.0 + (137.507764050037854 * i), 1.0, 1.0);
		IEnumerable<(Land l, int i)> RemainingStarts(PlayersAndProvinces p_p) => p_p.Provinces.Select((p, i) => (p as Land, i)).Where(it => it.Item1 is Land && it.Item1.IsStart && !it.Item1.Occupied)!;

		Player GetRobot(int count, int i, int earnings)
		{
			return new Robot(ColorAt(count + i - 1), cfg.Settings.DefaultMoney - (earnings * 2), true, Actions, cfg.Settings);
		}

		void AddRobots(Game game, PlayersAndProvinces p_p)
		{
			RemainingStarts(p_p).Each((start, i) => pap.Add(game, GetRobot(p_p.PlayersCount, i, start.l.Earnings), start.i));
		}
		public async Task<int> CreateAsync()
		{
			int gameId = pap.Add(new PlayersAndProvinces(ImmutableArray.Create<Player>(new Savage()), cfg.Settings.Provinces)).Id;
			game.RemoveOld(TimeSpan.FromDays(1));
			await ctx.SaveAsync();
			return gameId;
		}
		public Color NextColor(int gameId) => ColorAt(pap.PlayersCount(gameId) - 1);
		public Task StartAllAsync()
		{
			foreach (var g in game.ShouldStart)
			{
				AddRobots(g, pap[g.Id]);
				_ = g.Start().SetActive(pap.FirstActive(g.Id));
				powers.Add(g.Id);
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
			var player = new Human(ColorAt(count), name, cfg.Settings.DefaultMoney - (cfg.Settings.ProvinceData[land]?.Earnings * 2).GetValueOrDefault(), true, Actions, password);
			pap.Add(game, player, land);
			if (count == 2)
			{
				_ = game.StartCountdown();
			}
			return ctx.SaveAsync();
		}
	}
}