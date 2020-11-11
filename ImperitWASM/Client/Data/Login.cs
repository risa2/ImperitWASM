namespace ImperitWASM.Client.Data
{
	public class Login
	{
		public int I { get; set; }
		public int G { get; set; }
		public string P { get; set; } = "";
		public Login(int id, int game, string password)
		{
			I = id;
			G = game;
			P = password;
		}
		public Login() { }
	}
}
