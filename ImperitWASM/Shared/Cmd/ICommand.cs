using ImperitWASM.Shared.Cfg;
using ImperitWASM.Shared.Entities;
using ImperitWASM.Shared.Func;

namespace ImperitWASM.Shared.Cmd
{
	public interface ICommand
	{
		Province Perform(Settings settings, Province province) => province;
		Player Perform(Settings settings, Player player, PlayersAndProvinces pap) => player;
		bool Allowed(Settings settings, PlayersAndProvinces pap) => true;
	}
}
