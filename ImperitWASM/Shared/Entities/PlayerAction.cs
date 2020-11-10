using ImperitWASM.Shared.Cfg;
using ImperitWASM.Shared.Func;

namespace ImperitWASM.Shared.Entities
{
	public abstract class PlayerAction
	{
		private int _id;
		public virtual (Province, PlayerAction?) Perform(Settings settings, Province province, Player active, PlayersAndProvinces pap) => (province, this);
		public virtual (Player, PlayerAction?) Perform(Settings settings, Player player, PlayersAndProvinces pap) => (player, this);
	}
}