using ImperitWASM.Shared.State;

namespace ImperitWASM.Shared.Commands
{
	public record NextTurn : ICommand
	{
		public Player Perform(Player player, PlayersAndProvinces pap) => pap.HasAny(player) ? player : player.Die();
	}
}
