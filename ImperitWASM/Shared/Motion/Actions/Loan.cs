using ImperitWASM.Shared.State;
using System;

namespace ImperitWASM.Shared.Motion.Actions
{
	public class Loan : IAction
	{
		readonly Settings settings;
		public readonly int Debt;
		public Loan(int debt, Settings set)
		{
			Debt = debt;
			settings = set;
		}
		public (Player, IAction?) Perform(Player player, Player active, PlayersAndProvinces pap)
		{
			int next_debt = Debt + (int)Math.Ceiling(Debt * settings.Interest);
			if (next_debt <= player.Money)
			{
				return (player.ChangeMoney(-next_debt), null);
			}
			return (player.ChangeMoney(-player.Money), new Loan(next_debt - player.Money, settings));
		}
		public (Province, IAction?) Perform(Province province, Player active, PlayersAndProvinces pap)
		{
			if (province is Land land && land.IsAllyOf(active) && Debt > settings.DebtLimit + active.Money)
			{
				return (land.Revolt(), land.Price > Debt ? null : new Loan(Debt - land.Price, settings));
			}
			return (province, this);
		}
	}
}