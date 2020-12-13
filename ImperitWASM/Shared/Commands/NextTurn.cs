using ImperitWASM.Shared.Data;

namespace ImperitWASM.Shared.Commands
{
	public record NextTurn : ICommand
	{
		public Player Perform(Player player, PlayersAndProvinces pap) => pap.HasAny(player) ? player : player.Die();
	}
}
