namespace ImperitWASM.Client.Data
{
	public class Session
	{
		public int U { get; set; }
		public int G { get; set; }
		public string I { get; set; } = "";
		public Session() { }
		public Session(int id, int game, string loginId)
		{
			U = id;
			G = game;
			I = loginId;
		}
	}
}
