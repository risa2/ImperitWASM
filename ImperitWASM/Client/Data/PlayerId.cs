namespace ImperitWASM.Client.Data
{
	public class PlayerId
	{
		public PlayerId(int id, int game, string name)
		{
			I = id;
			G = game;
			N = name;
		}
		public PlayerId() { }
		public int I { get; set; }
		public int G { get; set; }
		public string N { get; set; } = "";
	}
}
