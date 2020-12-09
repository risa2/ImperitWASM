using ImperitWASM.Shared.State;

namespace ImperitWASM.Shared.Actions
{
	public record Manoeuvre(Player Player, Soldiers Soldiers) : IProvinceAction
	{
		public (Province, IProvinceAction?) Perform(Province province, PlayersAndProvinces pap)
		{
			return (province.IsAllyOf(Player) ? province.ReinforcedBy(Soldiers) : province.AttackedBy(Player, Soldiers), null);
		}
	}
}