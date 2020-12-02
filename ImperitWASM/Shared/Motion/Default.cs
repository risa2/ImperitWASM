using ImperitWASM.Shared.State;

namespace ImperitWASM.Shared.Motion
{
	public record Default : IPlayerAction
	{
		public (Player, IPlayerAction) Perform(Player player, PlayersAndProvinces pap)
		{
			return (player.ChangeMoney(pap.IncomeOf(player)), this);
		}
		public (Province, IPlayerAction?) Perform(Province province, Player active, PlayersAndProvinces pap)
		{
			return ((province is Sea Sea && Sea.Occupied && !Sea.HasSoldiers) || !province.CanSoldiersSurvive ? province.Revolt() : province, this);
		}
	}
}