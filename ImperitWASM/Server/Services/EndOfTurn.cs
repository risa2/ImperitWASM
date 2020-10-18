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
		public EndOfTurn(IPlayersProvinces pap, IPowersLoader powers, INewGame newGame)
		{
			this.pap = pap;
			this.powers = powers;
			this.newGame = newGame;
		}
		public bool Continue => pap.LivingHumans > 0 && !powers.Last.MajorityReached;
		public async Task<bool> NextTurn()
		{
			pap.Next();
			while (pap.Active is Robot robot && Continue && pap.Active.Alive)
			{
				pap.PaP = robot.Think(pap.PaP);
				pap.Next();
			}
			powers.Compute();
			if (pap.LivingHumans <= 0 || powers.Last.MajorityReached)
			{
				await newGame.Finish();
				return true;
			}
			await pap.Save();
			return false;
		}
	}
}
