namespace ImperitWASM.Client.Data
{
	public class RegisteredPlayer
	{
		public string N { get; set; } = "";
		public string P { get; set; } = "";
		public int S { get; set; }
		public int G { get; set; }
		public RegisteredPlayer() { }
		public RegisteredPlayer(string name, string password, int start, int gameId)
		{
			N = name;
			P = password;
			S = start;
			G = gameId;
		}
	}
}
