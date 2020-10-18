using System.Threading.Tasks;
using ImperitWASM.Shared.State;

namespace ImperitWASM.Server.Services
{
	public interface IEndOfTurn
	{
		Task NextTurn();
		bool Continue { get; }
	}
	public class EndOfTurn : IEndOfTurn
	{
		readonly IPlayersProvinces pap;
		readonly IPowersLoader powers;
		public EndOfTurn(IPlayersProvinces pap, IPowersLoader powers)
		{
			this.pap = pap;
			this.powers = powers;
		}
		public bool Continue => pap.LivingHumans > 0 && !powers.Last.MajorityReached;
		public Task NextTurn()
		{
			pap.Next();
			while (pap.Active is Robot robot && Continue && pap.Active.Alive)
			{
				pap.PaP = robot.Think(pap.PaP);
				pap.Next();
			}
			powers.Compute();
			return pap.Save();
		}
	}
}
