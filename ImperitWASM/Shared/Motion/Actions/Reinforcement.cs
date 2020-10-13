using ImperitWASM.Shared.State;

namespace ImperitWASM.Shared.Motion.Actions
{
	public class Reinforcement : IAction
	{
		public readonly Soldiers Soldiers;
		public Reinforcement(Soldiers soldiers) => Soldiers = soldiers;
		public (Province, IAction?) Perform(Province province, Player active, PlayersAndProvinces pap)
		{
			return (province.ReinforcedBy(Soldiers), null);
		}
	}
}