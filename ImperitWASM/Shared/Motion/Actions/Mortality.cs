using ImperitWASM.Shared.State;

namespace ImperitWASM.Shared.Motion.Actions
{
	public class Mortality : IAction
	{
		public (Player, IAction?) Perform(Player player, Player active, PlayersAndProvinces pap)
		{
			return (pap.HasAny(player) ? player : player.Die(), null);
		}
		public (Province, IAction?) Perform(Province province, Player active, PlayersAndProvinces pap)
		{
			return ((province is Sea Sea && Sea.Occupied && !Sea.Soldiers.Any) || !province.CanSoldiersSurvive ? province.Revolt() : province, this);
		}
		public byte Priority => 200;
	}
}