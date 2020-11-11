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
		readonly INewGame newGame;
		readonly IConfig cfg;
		public EndOfTurn(IPlayersProvinces pap, IPowers powers, INewGame newGame, IConfig cfg, IContextService ctx)
		{
			this.pap = pap;
			this.powers = powers;
			this.newGame = newGame;
			this.cfg = cfg;
			this.ctx = ctx;
		}
		public async Task<bool> NextTurnAsync(int gameId)
		{
			var p = pap.Next(gameId);
			while (pap.Active(gameId) is Robot robot && p.LivingHumans > 0)
			{
				pap[gameId] = robot.Think(p);
				p = pap.Next(gameId);
			}
			powers.Add(gameId);
			if (p.LivingHumans <= 0 || p.Winner(cfg.Settings.FinalLandsCount) is Human)
			{
				await newGame.FinishAsync(gameId);
				return true;
			}
			await ctx.SaveAsync();
			return false;
		}
	}
}
