namespace ImperitWASM.Shared.Data
{
	public interface IPlayerAction
	{
		(Province, IPlayerAction?) Perform(Province province, Player active, PlayersAndProvinces pap) => (province, this);
		(Player, IPlayerAction?) Perform(Player player, PlayersAndProvinces pap) => (player, this);
	}
}