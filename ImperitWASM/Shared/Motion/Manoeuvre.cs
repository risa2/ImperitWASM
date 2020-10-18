using ImperitWASM.Shared.State;

namespace ImperitWASM.Shared.Motion
{
	public class Manoeuvre : IProvinceAction
	{
		public readonly Army Army;
		public Manoeuvre(Army army) => Army = army;
		public (Province, IProvinceAction?) Perform(Province province, Player active, PlayersAndProvinces pap)
		{
			return (province.IsAllyOf(Army) ? province.ReinforcedBy(Army.Soldiers) : province.AttackedBy(Army), null);
		}
	}
}