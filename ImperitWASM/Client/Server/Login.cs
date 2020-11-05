namespace ImperitWASM.Client.Server
{
	public class Login
	{
		public Login(int id, int game, string password)
		{
			I = id;
			G = game;
			P = password;
		}
		public Login() { }
		public int I { get; set; }
		public int G { get; set; }
		public string P { get; set; } = "";
	}
}
