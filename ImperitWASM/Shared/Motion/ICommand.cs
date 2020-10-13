using ImperitWASM.Shared.State;
using System.Collections.Generic;

namespace ImperitWASM.Shared.Motion
{
	public interface ICommand
	{
		Province Perform(Province province) => province;
		Player Perform(Player player, PlayersAndProvinces pap) => player;
		bool Allowed(PlayersAndProvinces pap);
	}
}
