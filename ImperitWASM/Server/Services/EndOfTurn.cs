using System.Threading.Tasks;
using ImperitWASM.Shared.State;

namespace ImperitWASM.Server.Services
{
	public interface IEndOfTurn
	{
		Task<bool> NextTurn();
	}
	public class EndOfTurn : IEndOfTurn
	{
		readonly IPlayersProvinces pap;
		readonly IPowersLoader powers;
		readonly INewGame newGame;
		readonly ISettingsLoader sl;
		public EndOfTurn(IPlayersProvinces pap, IPowersLoader powers, INewGame newGame, ISettingsLoader sl)
		{
			this.pap = pap;
			this.powers = powers;
			this.newGame = newGame;
			this.sl = sl;
		}
		public async Task<bool> NextTurn()
		{
			pap.Next();
			while (pap.Active is Robot robot && pap.LivingHumans > 0 && pap.Active.Alive)
			{
				pap.PaP = robot.Think(pap.PaP);
				pap.Next();
			}
			powers.Compute();
			if (pap.LivingHumans <= 0 || pap.PaP.Victory(sl.Settings.FinalLandsCount) is int)
			{
				await newGame.Finish();
				return true;
			}
			await pap.Save();
			return false;
		}
	}
}
