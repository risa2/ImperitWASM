using ImperitWASM.Shared.State;

namespace ImperitWASM.Shared.Motion.Actions
{
	public class Instability : IAction
	{
		static readonly System.Random rand = new System.Random();
		public (Province, IAction?) Perform(Province province, Player active, PlayersAndProvinces pap)
		{
			return (province.Occupied && province is Land Land && Land.IsAllyOf(active) && rand.NextDouble() < Land.Instability ? Land.Revolt() : province, this);
		}
	}
}