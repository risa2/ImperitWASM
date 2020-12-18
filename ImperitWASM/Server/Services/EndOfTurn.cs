using System.Threading.Tasks;
using ImperitWASM.Server.Load;
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
			var g = gs.Find(gameId);
			var p_p = pap[gameId] = AllActions(pap[gameId], g);
			bool finish = p_p.LivingHumans <= 0 || p_p.Winner(settings.FinalLandsCount) is Human;
			powers.Add(gameId, p_p);
			_ = finish ? g.Finish() : g;
			await ctx.SaveAsync();
			return finish;
		}
	}
}
