using ImperitWASM.Shared.Cfg;
using ImperitWASM.Shared.Entities;
using ImperitWASM.Shared.Func;

namespace ImperitWASM.Shared.Cmd
{
	public class Borrow : ICommand
	{
		public readonly Player Player;
		public readonly int Amount;
		public Borrow(Player player, int amount)
		{
			Player = player;
			Amount = amount;
		}
		public bool Allowed(Settings settings, PlayersAndProvinces pap)
		{
			return Amount <= settings.DebtLimit && Amount > 0;
		}
		public Player Perform(Settings settings, Player player, PlayersAndProvinces pap)
		{
			return player == Player ? player.ChangeMoney(Amount).Replace(a => true, new Loan(Amount), (x, y) => new Loan(x.Debt + y.Debt)) : player;
		}
	}
}