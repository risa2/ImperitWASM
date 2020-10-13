using ImperitWASM.Shared.State;

namespace ImperitWASM.Shared.Motion.Actions
{
	public class Earnings : IAction
	{
		public (Player, IAction) Perform(Player player, Player active, PlayersAndProvinces pap)
		{
			return (player.ChangeMoney(pap.IncomeOf(player)), this);
		}
	}
}