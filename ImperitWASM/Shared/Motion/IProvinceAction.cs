using ImperitWASM.Shared.State;

namespace ImperitWASM.Shared.Motion
{
	public interface IProvinceAction
	{
		(Province, IProvinceAction?) Perform(Province province, Player active, PlayersAndProvinces pap) => (province, this);
		(Player, IProvinceAction?) Perform(Player player, Player active, PlayersAndProvinces pap) => (player, this);
	}
}
