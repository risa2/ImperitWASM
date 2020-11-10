using ImperitWASM.Shared.Cfg;
using ImperitWASM.Shared.Func;

namespace ImperitWASM.Shared.Entities
{
	public abstract class ProvinceAction
	{
		private int _id;
		public virtual (Province, ProvinceAction?) Perform(Settings settings, Province province, PlayersAndProvinces pap) => (province, this);
		public virtual (Player, ProvinceAction?) Perform(Settings settings, Player player, PlayersAndProvinces pap) => (player, this);
	}
}
