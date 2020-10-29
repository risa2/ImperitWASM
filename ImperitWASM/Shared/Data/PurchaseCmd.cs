namespace ImperitWASM.Shared.Data
{
	public class PurchaseCmd
	{
		public PurchaseCmd(int loggedIn, string loginId, int land)
		{
			LoggedIn = loggedIn;
			LoginId = loginId;
			Land = land;
		}
		public PurchaseCmd() { }
		public int LoggedIn { get; set; }
		public string LoginId { get; set; } = "";
		public int Land { get; set; }
	}
}
