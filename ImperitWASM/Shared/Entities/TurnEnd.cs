using ImperitWASM.Shared.Cfg;
using ImperitWASM.Shared.Func;

namespace ImperitWASM.Shared.Entities
{
	public class TurnEnd : PlayerAction
	{
		public override (Player, PlayerAction) Perform(Settings settings, Player player, PlayersAndProvinces pap)
		{
			return (player.ChangeMoney(pap.IncomeOf(player)), this);
		}
		public override (Province, PlayerAction?) Perform(Settings settings, Province province, Player active, PlayersAndProvinces pap)
		{
			return ((province is Sea Sea && Sea.Occupied && !Sea.Soldiers.Any) || !province.CanSoldiersSurvive ? province.Revolt() : province, this);
		}
	}
}