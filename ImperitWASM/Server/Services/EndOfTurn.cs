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
		public EndOfTurn(IPlayersProvinces pap, IPowersLoader powers)
		{
			this.pap = pap;
			this.powers = powers;
		}
		void End()
		{
			pap.Act();
			powers.Compute();
			pap.Next();
		}
		void AllRobotsActions()
		{
			while (pap.Active is Robot robot && pap.LivingHumans > 0)
			{
				foreach (var cmd in robot.Think(pap.Provinces))
				{
					_ = pap.Do(cmd);
				}
				End();
			}
		}
		public async Task<bool> NextTurn()
		{
			End();
			AllRobotsActions();
			await pap.Save();
			return pap.LivingHumans > 0 && !powers.Last.MajorityReached;
		}
	}
}
