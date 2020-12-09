using ImperitWASM.Shared.State;

namespace ImperitWASM.Shared.Actions
{
	public record EndTurn : IPlayerAction
	{
		public (Player, IPlayerAction) Perform(Player player, PlayersAndProvinces pap) => (player.ChangeMoney(pap.IncomeOf(player)), this);
		public (Province, IPlayerAction?) Perform(Province province, Player active, PlayersAndProvinces pap) => (province.ShouldRevolt(active) ? province.Revolt() : province, this);
	}
}