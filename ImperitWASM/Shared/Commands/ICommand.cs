using System.Collections.Generic;
using ImperitWASM.Shared.Config;
using ImperitWASM.Shared.Data;

namespace ImperitWASM.Shared.Commands
{
	public interface ICommand
	{
		bool Allowed(Player actor, IReadOnlyList<Player> players, Provinces provinces, Settings settings);
		(IEnumerable<Player>, IEnumerable<Province>, Game) Perform(Player actor, IReadOnlyList<Player> players, Provinces provinces, Settings settings, Game game);
	}
}
