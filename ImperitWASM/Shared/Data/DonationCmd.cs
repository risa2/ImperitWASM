namespace ImperitWASM.Shared.Data
{
	public class DonationCmd
	{
		public DonationCmd(int loggedIn, string loginId, int recipient, int amount)
		{
			LoggedIn = loggedIn;
			LoginId = loginId;
			Recipient = recipient;
			Amount = amount;
		}
		public DonationCmd() { }
		public int LoggedIn { get; set; }
		public string LoginId { get; set; } = "";
		public int Recipient { get; set; }
		public int Amount { get; set; }
	}
}
