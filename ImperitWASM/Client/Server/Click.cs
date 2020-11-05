namespace ImperitWASM.Client.Server
{
	public class Click
	{
		public Click(int loggedIn, int? from, int clicked, int g)
		{
			U = loggedIn;
			F = from;
			C = clicked;
			G = g;
		}
		public Click() { }
		public int U { get; set; }
		public int? F { get; set; }
		public int C { get; set; }
		public int G { get; set; }
	}
}
