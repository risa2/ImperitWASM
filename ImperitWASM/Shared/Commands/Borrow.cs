using ImperitWASM.Shared.Data;

namespace ImperitWASM.Shared.Commands
{
	public record Borrow(Player Player, int Amount) : ICommand
	{
		public bool Allowed(PlayersAndProvinces pap) => Amount <= Player.MaxBorrowable && Amount > 0;
		public Player Perform(Player player, PlayersAndProvinces pap) => player == Player ? player.Borrow(Amount) : player;
	}
}