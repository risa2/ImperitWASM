using ImperitWASM.Shared.State;

namespace ImperitWASM.Shared.Actions
{
	public interface IProvinceAction
	{
		(Province, IProvinceAction?) Perform(Province province, PlayersAndProvinces pap) => (province, this);
		(Player, IProvinceAction?) Perform(Player player, PlayersAndProvinces pap) => (player, this);
	}
}
