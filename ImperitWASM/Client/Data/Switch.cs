namespace ImperitWASM.Client.Data
{
	public class Switch
	{
		public Switch(int? selected, Mode nextMode, int? from, int? to)
		{
			S = selected;
			M = nextMode;
			F = from;
			T = to;
		}
		public Switch() { }
		public enum Mode { Map, Move, Purchase, Recruit, Donation, Players, Powers, Preview }
		public int? S { get; set; }
		public Mode M { get; set; }
		public int? F { get; set; }
		public int? T { get; set; }
	}
}
