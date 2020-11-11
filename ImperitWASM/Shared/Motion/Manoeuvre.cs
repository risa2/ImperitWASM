using ImperitWASM.Shared.State;

namespace ImperitWASM.Shared.Motion
{
	public class Manoeuvre : IProvinceAction
	{
		public readonly Player Player;
		public readonly Soldiers Soldiers;
		public Manoeuvre(Player player, Soldiers soldiers)
		{
			Player = player;
			Soldiers = soldiers;
		}
		public (Province, IProvinceAction?) Perform(Province province, PlayersAndProvinces pap)
		{
			return (province.IsAllyOf(Player) ? province.ReinforcedBy(Soldiers) : province.AttackedBy(Player, Soldiers), null);
		}
	}
}