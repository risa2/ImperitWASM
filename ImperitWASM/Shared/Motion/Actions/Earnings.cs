using ImperitWASM.Shared.State;
using System.Linq;

namespace ImperitWASM.Shared.Motion.Actions
{
	public class Earnings : IAction
	{
		public (IAction?, Player) Perform(Player player, Player active, IProvinces provinces)
		{
			return (this, player == active ? player.ChangeMoney(provinces.ControlledBy(player.Id).Sum(p => p.Earnings)) : player);
		}
		public byte Priority => 10;
	}
}