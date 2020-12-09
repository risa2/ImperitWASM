using ImperitWASM.Shared.State;

namespace ImperitWASM.Shared.Motion
{
	public record EndTurn : IPlayerAction
	{
		public (Player, IPlayerAction) Perform(Player player, PlayersAndProvinces pap)
		{
			return (player.ChangeMoney(pap.IncomeOf(player)), this);
		}
		public (Province, IPlayerAction?) Perform(Province province, Player active, PlayersAndProvinces pap)
		{
			return (province.IsAllyOf(active) && province is { CanSoldiersSurvive: false } or Sea { Occupied: true, HasSoldiers: false } or Land { WillRevolt: true } ? province.Revolt() : province, this);
		}
	}
}