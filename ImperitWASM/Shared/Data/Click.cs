namespace ImperitWASM.Shared.Data
{
	public class Click
	{
		public Click(int loggedIn, int? from, int clicked)
		{
			U = loggedIn;
			F = from;
			C = clicked;
		}
		public Click() { }
		public int U { get; set; }
		public int? F { get; set; }
		public int C { get; set; }
	}
}
