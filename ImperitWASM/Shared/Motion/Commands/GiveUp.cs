using ImperitWASM.Shared.State;
using System.Collections.Generic;

namespace ImperitWASM.Shared.Motion.Commands
{
	public class GiveUp : ICommand
	{
		public readonly Player Player;
		public GiveUp(Player player) => Player = player;
		public bool Allowed(PlayersAndProvinces pap) => true;
		public Province Perform(Province province) => province.IsAllyOf(Player) ? province.Revolt() : province;
		public Player Perform(Player player, PlayersAndProvinces pap) => player == Player ? player.Die() : player;
	}
}
