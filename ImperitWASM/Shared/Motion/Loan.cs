using ImperitWASM.Shared.State;

namespace ImperitWASM.Shared.Motion
{
	public record Loan(int Debt, Settings Settings) : IPlayerAction
	{
		public (Player, IPlayerAction?) Perform(Player player, PlayersAndProvinces pap)
		{
			int next_debt = Debt + Debt * Settings.Interest;
			return next_debt <= player.Money
				? (player.ChangeMoney(-next_debt), null)
				: (player.ChangeMoney(-player.Money), new Loan(next_debt - player.Money, Settings));
		}
		public (Province, IPlayerAction?) Perform(Province province, Player active, PlayersAndProvinces pap)
		{
			return province is Land land && land.IsAllyOf(active) && Debt > Settings.DebtLimit + active.Money
				? (land.Revolt(), land.Price > Debt ? null : new Loan(Debt - land.Price, Settings))
				: (province, this);
		}
	}
}