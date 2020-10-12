namespace ImperitWASM.Shared.Data
{
	public class Switch
	{
		public Switch(int? selected, Mode nextMode, int? from, int? to)
		{
			Selected = selected;
			NextMode = nextMode;
			From = from;
			To = to;
		}
		public Switch() { }
		public enum Mode { Map, Move, Purchase, Recruit, Donation, Players, Powers, Preview }
		public int? Selected { get; set; }
		public Mode NextMode { get; set; }
		public int? From { get; set; }
		public int? To { get; set; }
	}
}
