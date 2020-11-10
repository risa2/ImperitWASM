namespace ImperitWASM.Client.Data
{
	public class PurchaseCmd
	{
		public PurchaseCmd(int loggedIn, string loginId, int land, int game)
		{
			LoggedIn = loggedIn;
			LoginId = loginId;
			Land = land;
			Game = game;
		}
		public PurchaseCmd() { }
		public int LoggedIn { get; set; }
		public int Game { get; set; }
		public string LoginId { get; set; } = "";
		public int Land { get; set; }
	}
}
