using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
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
		Task RegisterAsync(int gameId, string name, Password password, int land);
	}
	public class GameCreator : INewGame
	{
		private readonly IPlayersProvinces pap;
		private readonly IPowers powers;
		private readonly IGameService game;
		private readonly IConfig cfg;
		private readonly IContextService ctx;
		private static readonly ImmutableList<IPlayerAction> Actions = ImmutableList.Create<IPlayerAction>(new Default(), new Instability());
		public GameCreator(IPlayersProvinces pap, IPowers powers, IGameService game, IConfig cfg, IContextService ctx)
		{
			this.pap = pap;
			this.powers = powers;
			this.game = game;
			this.cfg = cfg;
			this.ctx = ctx;
		}

		private static Color ColorAt(int i) => new Color(120.0 + (137.507764050037854 * i), 1.0, 1.0);
		private IEnumerable<(Land l, int i)> RemainingStarts(int gameId, ImmutableArray<Player> players) => ctx.GetProvinces(gameId, players).Select((p, i) => (p as Land, i)).Where(it => it.Item1 is Land && it.Item1.IsStart && !it.Item1.Occupied)!;

		private Player GetRobot(int count, int i, int earnings)
		{
			return new Robot(ColorAt(count + i - 1), cfg.Settings.DefaultMoney - (earnings * 2), true, Actions, cfg.Settings);
		}

		private void AddRobots(int gameId, ImmutableArray<Player> players)
		{
			RemainingStarts(gameId, players).Each((start, i) => pap.Add(gameId, GetRobot(players.Length, i, start.l.Earnings), start.l.Soldiers, start.i));
		}
		public async Task<int> CreateAsync()
		{
			int gameId = game.Create();
			pap.AddPaP(gameId, new PlayersAndProvinces(ImmutableArray.Create<Player>(new Savage()), cfg.Settings.Provinces));
			game.RemoveOld(TimeSpan.FromDays(1));
			await ctx.SaveAsync();
			return gameId;
		}
		public Color NextColor(int gameId) => ColorAt(pap.PlayersCount(gameId) - 1);
		public Task StartAllAsync()
		{
			foreach (int gameId in game.ShouldStart)
			{
				AddRobots(gameId, pap.Players(gameId));
				pap.ResetActive(gameId);
				game.Start(gameId);
				powers.Add(gameId);
			}
			return ctx.SaveAsync();
		}
		public Task FinishAsync(int gameId)
		{
			game.Finish(gameId);
			return ctx.SaveAsync();
		}
		public Task RegisterAsync(int gameId, string name, Password password, int land)
		{
			var p_p = pap[gameId];
			var player = new Human(ColorAt(p_p.PlayersCount), name, cfg.Settings.DefaultMoney - ((p_p.Province(land) as Land)?.Earnings * 2 ?? 0), true, Actions, password);
			pap.Add(gameId, player, p_p.Province(land).DefaultSoldiers, land);
			if (p_p.PlayersCount == 2)
			{
				game.StartCountdown(gameId);
			}
			return ctx.SaveAsync();
		}
	}
}