using ImperitWASM.Shared.State;
using System;

namespace ImperitWASM.Shared.Motion
{
	public class Loan : IPlayerAction
	{
		readonly Settings settings;
		public readonly int Debt;
		public Loan(int debt, Settings set)
		{
			Debt = debt;
			settings = set;
		}
		public (Player, IPlayerAction?) Perform(Player player, PlayersAndProvinces pap)
		{
			int next_debt = Debt + (int)Math.Ceiling(Debt * settings.Interest);
			return next_debt <= player.Money
				? (player.ChangeMoney(-next_debt), null)
				: (player.ChangeMoney(-player.Money), new Loan(next_debt - player.Money, settings));
		}
		public (Province, IPlayerAction?) Perform(Province province, Player active, PlayersAndProvinces pap)
		{
			return province is Land land && land.IsAllyOf(active) && Debt > settings.DebtLimit + active.Money
				? (land.Revolt(), land.Price > Debt ? null : new Loan(Debt - land.Price, settings))
				: (province, this);
		}
	}
}