using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using ImperitWASM.Shared.Entities;
using ImperitWASM.Shared.Func;
using ImperitWASM.Shared.Cfg;
using System.Collections.Generic;
using System;

namespace ImperitWASM.Server.Services
{
	public interface INewGame
	{
		Color NextColor(int gameId);
		Task StartAsync(int gameId);
		Task FinishAsync(int gameId);
		Task RegisterAsync(int gameId, string name, Password password, int land);
	}
	public class GameCreator : INewGame
	{
		readonly IPlayersProvinces pap;
		readonly IPowers powers;
		readonly IGameService game;
		readonly IConfig cfg;
		readonly IContextService ctx;
		readonly static ImmutableList<PlayerAction> Actions = ImmutableList.Create<PlayerAction>(new Default(), new Instability());
		public GameCreator(IPlayersProvinces pap, IPowers powers, IGameService game, IConfig cfg, IContextService ctx)
		{
			this.pap = pap;
			this.powers = powers;
			this.game = game;
			this.cfg = cfg;
			this.ctx = ctx;
		}
		static Color ColorAt(int i) => new Color(120.0 + (137.507764050037854 * i), 1.0, 1.0);
		IEnumerable<(Land l, int i)> RemainingStarts(int gameId, ImmutableArray<Player> players) => ctx.GetProvinces(gameId, players).Select((p, i) => (p as Land, i)).Where(it => it.Item1 is Land && it.Item1.IsStart && !it.Item1.Occupied)!;
		Player GetRobot(int count, int i, int earnings)
		{
			return new Robot(ColorAt(count + i - 1), cfg.Settings.DefaultMoney - (earnings * 2), true, Actions, cfg.Settings);
		}
		void AddRobots(int gameId, ImmutableArray<Player> players)
		{
			RemainingStarts(gameId, players).Each((start, i) => pap.Add(gameId, GetRobot(players.Length, i, start.l.Earnings), start.l.Soldiers, start.i));
		}
		public Color NextColor(int gameId) => ColorAt(pap.PlayersCount(gameId) - 1);
		public Task StartAsync(int gameId)
		{
			AddRobots(gameId, pap.Players(gameId));
			pap.ResetActive(gameId);
			game.Start(gameId);
			powers.Add(gameId);
			game.RemoveOld(TimeSpan.FromHours(6));
			return ctx.SaveAsync();
		}
		public Task FinishAsync(int gameId)
		{
			game.Finish(gameId);
			game.RemoveOld(TimeSpan.FromHours(6));
			pap.AddPaP(game.Create(), pap[gameId].RemovePlayers(new Savage()));
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