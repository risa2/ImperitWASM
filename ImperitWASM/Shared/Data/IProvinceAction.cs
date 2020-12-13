namespace ImperitWASM.Shared.Data
{
	public interface IProvinceAction
	{
		(Province, IProvinceAction?) Perform(Province province, PlayersAndProvinces pap) => (province, this);
		(Player, IProvinceAction?) Perform(Player player, PlayersAndProvinces pap) => (player, this);
	}
}
