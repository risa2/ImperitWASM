using ImperitWASM.Shared.State;
using System.Collections.Generic;

namespace ImperitWASM.Shared.Motion.Commands
{
	public class Borrow : ICommand
	{
		readonly Settings settings;
		public readonly Player Player;
		public readonly int Amount;
		public Borrow(Player player, int amount, Settings set)
		{
			Player = player;
			Amount = amount;
			settings = set;
		}
		public bool Allowed(PlayersAndProvinces pap)
		{
			return Amount <= settings.DebtLimit && Amount > 0;
		}
		public Player Perform(Player player, PlayersAndProvinces pap)
		{
			return player == Player ? player.ChangeMoney(Amount).Add(new Loan(Amount, settings)) : player;
		}
	}
}