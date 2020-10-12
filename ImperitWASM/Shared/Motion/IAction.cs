using ImperitWASM.Shared.State;
using System.Collections.Generic;

namespace ImperitWASM.Shared.Motion
{
	public interface IAction
	{
		(IAction?, Province) Perform(Province province, Player active) => (this, province);
		(IAction?, Player) Perform(Player player, Player active, IProvinces provinces) => (this, player);
		(IAction?, bool) Interact(ICommand another) => (this, true);
		bool Allows(ICommand another, IReadOnlyList<Player> players, IProvinces provinces) => true;
		byte Priority { get; }
	}
}