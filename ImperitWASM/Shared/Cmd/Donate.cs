using ImperitWASM.Shared.Cfg;
using ImperitWASM.Shared.Entities;
using ImperitWASM.Shared.Func;

namespace ImperitWASM.Shared.Cmd
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
		public bool Allowed(Settings settings, PlayersAndProvinces pap) => Player.Money >= Amount && Amount > 0;
		public Player Perform(Settings settings, Player player, PlayersAndProvinces pap)
		{
			return player == Player ? player.ChangeMoney(-Amount) : player == Recipient ? player.ChangeMoney(Amount) : player;
		}
	}
}