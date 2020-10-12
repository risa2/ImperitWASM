namespace ImperitWASM.Shared.Data
{
	public class Click
	{
		public Click(int loggedIn, string loginId, int? from, int clicked)
		{
			LoggedIn = loggedIn;
			LoginId = loginId;
			From = from;
			Clicked = clicked;
		}
		public Click() { }
		public int LoggedIn { get; set; }
		public string LoginId { get; set; } = "";
		public int? From { get; set; }
		public int Clicked { get; set; }
	}
}
