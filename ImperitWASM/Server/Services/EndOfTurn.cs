using System.Threading.Tasks;
using ImperitWASM.Shared.State;

namespace ImperitWASM.Server.Services
{
	public interface IEndOfTurn
	{
		Task<bool> NextTurnAsync(int gameId);
	}
	public class EndOfTurn : IEndOfTurn
	{
		readonly IContextService ctx;
		readonly IPlayersProvinces pap;
		readonly IPowers powers;
		readonly IGameCreator newGame;
		readonly IGameService game;
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
		public async Task<bool> NextTurnAsync(int gameId)
		{
			var p_p = pap[gameId];
			var g = game.Find(gameId);
			while (p_p.Player(g.Active) is Robot robot && p_p.LivingHumans > 0)
			{
				p_p = robot.Think(p_p).Act(g.Active);
				g.Active = p_p.Next(g.Active);
			}
			pap[gameId] = p_p;
			powers.Add(gameId);
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
