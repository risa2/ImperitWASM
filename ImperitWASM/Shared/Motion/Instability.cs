using ImperitWASM.Shared.State;

namespace ImperitWASM.Shared.Motion
{
	public class Instability : IPlayerAction
	{
		public (Province, IPlayerAction?) Perform(Province province, Player active, PlayersAndProvinces pap)
		{
			return (province.Occupied && province is Land Land && Land.IsAllyOf(active) && Land.Instability.RandomBool ? Land.Revolt() : province, this);
		}
	}
}