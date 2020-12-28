namespace ImperitWASM.Shared.Data
{
	public record EndTurn : IPlayerAction
	{
		public (Player, IPlayerAction) Perform(Player player, PlayersAndProvinces pap) => (player.Earn(pap), this);
		public (Province, IPlayerAction?) Perform(Province province, Player active, PlayersAndProvinces pap) => (province.RevoltIfNecessary(active), this);
	}
}