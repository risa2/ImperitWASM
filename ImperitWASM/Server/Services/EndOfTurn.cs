using System.Threading.Tasks;
using ImperitWASM.Server.Load;
using ImperitWASM.Shared.Commands;
using ImperitWASM.Shared.Config;
using ImperitWASM.Shared.Data;

namespace ImperitWASM.Server.Services
{
	public interface IEndOfTurn
	{
		Task<bool> NextTurnAsync(int gameId);
	}
	public class EndOfTurn : IEndOfTurn
	{
		readonly IPlayersProvinces pap;
		readonly IContextService ctx;
		readonly IGameService gs;
		readonly IPowers powers;
		readonly Settings settings;
		public EndOfTurn(IPlayersProvinces pap, IPowers powers, Settings settings, IContextService ctx, IGameService gs)
		{
			this.pap = pap;
			this.powers = powers;
			this.settings = settings;
			this.ctx = ctx;
			this.gs = gs;
		}
		static bool FinishGame(Game g, bool finish, int active)
		{
			(finish ? g.Finish() : g).Active = active;
			return finish;
		}
		public async Task<bool> NextTurnAsync(int gameId)
		{
			var g = gs.Find(gameId);
			var (p_p, active) = pap[gameId].EndOfTurn(g.Active);
			pap[gameId] = p_p;

			bool finish = FinishGame(g, !p_p.AnyHuman || p_p.Winner(settings.FinalLandsCount) is Human, active);
			powers.Add(gameId, p_p);
			await ctx.SaveAsync();
			return finish;
		}
	}
}
