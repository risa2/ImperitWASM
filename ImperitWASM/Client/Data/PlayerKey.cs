namespace ImperitWASM.Client.Data
{
	public class PlayerKey
	{
		public PlayerKey(int id, int game)
		{
			I = id;
			G = game;
		}
		public PlayerKey() { }
		public int I { get; set; }
		public int G { get; set; }
	}
}
