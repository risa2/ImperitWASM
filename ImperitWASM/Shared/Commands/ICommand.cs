using ImperitWASM.Shared.Data;

namespace ImperitWASM.Shared.Commands
{
	public interface ICommand
	{
		Province Perform(Province province) => province;
		Player Perform(Player player, PlayersAndProvinces pap) => player;
		bool Allowed(PlayersAndProvinces pap) => true;
	}
}
