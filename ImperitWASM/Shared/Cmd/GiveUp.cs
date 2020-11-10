using ImperitWASM.Shared.Cfg;
using ImperitWASM.Shared.Entities;
using ImperitWASM.Shared.Func;

namespace ImperitWASM.Shared.Cmd
{
	public class GiveUp : ICommand
	{
		public readonly Player Player;
		public GiveUp(Player player) => Player = player;
		public Province Perform(Settings settings, Province province) => province.IsAllyOf(Player) ? province.Revolt() : province;
		public Player Perform(Settings settings, Player player, PlayersAndProvinces pap) => player == Player ? player.Die() : player;
	}
}
