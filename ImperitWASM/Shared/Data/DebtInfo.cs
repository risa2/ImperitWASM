namespace ImperitWASM.Shared.Data
{
	public class DebtInfo
	{
		public int Debt { get; set; }
		public int Debtor { get; set; }
		public DebtInfo() { }
		public DebtInfo(int debtor, int debt)
		{
			Debt = debt;
			Debtor = debtor;
		}
	}
}
