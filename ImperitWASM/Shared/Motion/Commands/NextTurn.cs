using ImperitWASM.Shared.State;

namespace ImperitWASM.Shared.Motion.Commands
{
	public class NextTurn : ICommand
	{
		public Player Perform(Player player, PlayersAndProvinces pap) => pap.HasAny(player) ? player : player.Die();
	}
}
