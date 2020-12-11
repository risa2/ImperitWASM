using ImperitWASM.Shared.State;

namespace ImperitWASM.Shared.Commands
{
	public record Donate(Player Player, Player Recipient, int Amount) : ICommand
	{
		public bool Allowed(PlayersAndProvinces pap) => Player.Money >= Amount && Amount > 0;
		public Player Perform(Player player, PlayersAndProvinces pap)
		{
			return player == Player ? player.ChangeMoney(-Amount) : player == Recipient ? player.ChangeMoney(Amount) : player;
		}
	}
}