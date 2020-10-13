using ImperitWASM.Shared.State;

namespace ImperitWASM.Shared.Motion
{
	public interface IAction
	{
		(Province, IAction?) Perform(Province province, Player active, PlayersAndProvinces pap) => (province, this);
		(Player, IAction?) Perform(Player player, Player active, PlayersAndProvinces pap) => (player, this);
	}
}