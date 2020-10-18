using ImperitWASM.Shared.State;

namespace ImperitWASM.Shared.Motion
{
	public class Instability : IPlayerAction
	{
		static readonly System.Random rand = new System.Random();
		public (Province, IPlayerAction?) Perform(Province province, Player active, PlayersAndProvinces pap)
		{
			return (province.Occupied && province is Land Land && Land.IsAllyOf(active) && rand.NextDouble() < Land.Instability ? Land.Revolt() : province, this);
		}
	}
}