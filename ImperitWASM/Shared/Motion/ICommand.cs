using ImperitWASM.Shared.State;
using System.Collections.Generic;

namespace ImperitWASM.Shared.Motion
{
	public interface ICommand
	{
		(IAction?, Province) Perform(Province province) => (null, province);
		(IAction?, Player) Perform(Player player, IProvinces provinces) => (null, player);
		bool Allowed(IReadOnlyList<Player> players, IProvinces provinces);
	}
}
