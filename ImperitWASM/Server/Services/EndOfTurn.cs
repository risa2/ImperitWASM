using System.Threading.Tasks;
using ImperitWASM.Server.Load;
using ImperitWASM.Shared.State;

namespace ImperitWASM.Server.Services
{
	public interface IEndOfTurn
	{
		Task<bool> NextTurnAsync(int gameId);
	}
	public class EndOfTurn : IEndOfTurn
	{
		readonly IPlayersProvinces pap;
		readonly IGameCreator newGame;
		readonly IContextService ctx;
		readonly IGameService game;
		readonly IPowers powers;
		readonly IConfig cfg;
		public EndOfTurn(IPlayersProvinces pap, IPowers powers, IGameCreator newGame, IConfig cfg, IContextService ctx, IGameService game)
		{
			this.pap = pap;
			this.powers = powers;
			this.newGame = newGame;
			this.cfg = cfg;
			this.ctx = ctx;
			this.game = game;
		}
		static PlayersAndProvinces Actions(PlayersAndProvinces p_p, Game g)
		{
			p_p = p_p.Act(g.Active);
			g.Active = p_p.Next(g.Active);
			return p_p;
		}
		static PlayersAndProvinces AllActions(PlayersAndProvinces p_p, Game g)
		{
			p_p = Actions(p_p, g);
			while (p_p.Player(g.Active) is Robot robot && p_p.LivingHumans > 0)
			{
				p_p = Actions(robot.Think(p_p), g);
			}
			return p_p;
		}
		public async Task<bool> NextTurnAsync(int gameId)
		{
			var p_p = pap[gameId] = AllActions(pap[gameId], game.Find(gameId));
			powers.Add(gameId, p_p);
			if (p_p.LivingHumans <= 0 || p_p.Winner(cfg.Settings.FinalLandsCount) is Human)
			{
				await newGame.FinishAsync(gameId);
				return true;
			}
			await ctx.SaveAsync();
			return false;
		}
	}
}
