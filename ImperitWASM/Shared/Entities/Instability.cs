using ImperitWASM.Shared.Cfg;
using ImperitWASM.Shared.Func;

namespace ImperitWASM.Shared.Entities
{
	public class Instability : PlayerAction
	{
		public override (Province, PlayerAction?) Perform(Settings settings, Province province, Player active, PlayersAndProvinces pap)
		{
			return (province.Occupied && province is Land Land && Land.IsAllyOf(active) && Land.Instability(settings).RandomBool ? Land.Revolt() : province, this);
		}
	}
}