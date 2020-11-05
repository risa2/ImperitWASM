using ImperitWASM.Shared.State;

namespace ImperitWASM.Client.Server
{
	public class BasicInfo
	{
		public BasicInfo(int player, Color color, bool isGameActive, int activePlayer)
		{
			A = player == activePlayer;
			B = color;
			C = isGameActive;
			D = activePlayer;
		}
		public BasicInfo() { }
		public bool A { get; set; }
		public Color B { get; set; }
		public bool C { get; set; }
		public int D { get; set; }
		public bool IsActive => A;
		public Color Color => B;
		public bool IsGameActive => C;
		public int ActivePlayer => D;

	}
}
