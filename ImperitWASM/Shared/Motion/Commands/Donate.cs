using ImperitWASM.Shared.State;

namespace ImperitWASM.Shared.Motion.Commands
{
	public class Donate : ICommand
	{
		public readonly Player Player, Recipient;
		public readonly int Amount;
		public Donate(Player player, Player recipient, int amount)
		{
			Player = player;
			Recipient = recipient;
			Amount = amount;
		}
		public bool Allowed(PlayersAndProvinces pap) => Player.Money >= Amount && Amount > 0;
		public Player Perform(Player player, PlayersAndProvinces pap)
		{
			return player == Player ? player.ChangeMoney(-Amount) : player == Recipient ? player.ChangeMoney(Amount) : player;
		}
	}
}