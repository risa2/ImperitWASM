namespace ImperitWASM.Client.Data
{
	public class DonationCmd
	{
		public DonationCmd(int loggedIn, string loginId, int recipient, int amount, int game)
		{
			LoggedIn = loggedIn;
			LoginId = loginId;
			Recipient = recipient;
			Amount = amount;
			Game = game;
		}
		public DonationCmd() { }
		public int LoggedIn { get; set; }
		public int Game { get; set; }
		public string LoginId { get; set; } = "";
		public int Recipient { get; set; }
		public int Amount { get; set; }
	}
}
