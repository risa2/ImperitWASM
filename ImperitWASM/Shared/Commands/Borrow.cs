using ImperitWASM.Shared.Config;
using ImperitWASM.Shared.Data;

namespace ImperitWASM.Shared.Commands
{
	public record Borrow(Player Player, int Amount, Settings Settings) : ICommand
	{
		public bool Allowed(PlayersAndProvinces pap)
		{
			return Amount <= Settings.DebtLimit && Amount > 0;
		}
		public Player Perform(Player player, PlayersAndProvinces pap)
		{
			return player == Player ? player.Borrow(Amount) : player;
		}
	}
}