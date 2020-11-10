namespace ImperitWASM.Client.Data
{
	public class CmdData
	{
		public CmdData(int a, int b, int gameId) => (A, B, G) = (a, b, gameId);
		public CmdData() { }
		public int A { get; set; }
		public int B { get; set; }
		public int G { get; set; }
		public void Deconstruct(out int a, out int b, out int gameId) => (a, b, gameId) = (A, B, G);
	}
}
