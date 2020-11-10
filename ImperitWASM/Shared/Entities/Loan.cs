using System;
using ImperitWASM.Shared.Cfg;
using ImperitWASM.Shared.Func;

namespace ImperitWASM.Shared.Entities
{
	public class Loan : PlayerAction
	{
		public int Debt { get; }
		public Loan(int debt) => Debt = debt;
		public override (Player, PlayerAction?) Perform(Settings settings, Player player, PlayersAndProvinces pap)
		{
			int next_debt = Debt + (int)Math.Ceiling(Debt * settings.Interest);
			return next_debt <= player.Money
				? (player.ChangeMoney(-next_debt), null)
				: (player.ChangeMoney(-player.Money), new Loan(next_debt - player.Money));
		}
		public override (Province, PlayerAction?) Perform(Settings settings, Province province, Player active, PlayersAndProvinces pap)
		{
			return province is Land land && land.IsAllyOf(active) && Debt > settings.DebtLimit + active.Money
				? (land.Revolt(), land.Price > Debt ? null : new Loan(Debt - land.Price))
				: (province, this);
		}
	}
}