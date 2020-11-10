using ImperitWASM.Shared.Cfg;
using ImperitWASM.Shared.Func;

namespace ImperitWASM.Shared.Entities
{
	public class Manoeuvre : ProvinceAction
	{
		public Player Player { get; }
		public Soldiers Soldiers { get; }
		public Manoeuvre(Player player, Soldiers soldiers)
		{
			Player = player;
			Soldiers = soldiers;
		}
		public override (Province, ProvinceAction?) Perform(Settings settings, Province province, PlayersAndProvinces pap)
		{
			return (province.IsAllyOf(Player) ? province.ReinforcedBy(Soldiers) : province.AttackedBy(Player, Soldiers), null);
		}
	}
}