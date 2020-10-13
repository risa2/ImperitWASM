using ImperitWASM.Shared.State;

namespace ImperitWASM.Shared.Motion.Actions
{
	public class Fight : IAction
	{
		public readonly Army Army;
		public Fight(Army army) => Army = army;
		public (Province, IAction?) Perform(Province province, Player active, PlayersAndProvinces pap)
		{
			return (province.AttackedBy(Army), null);
		}
	}
}