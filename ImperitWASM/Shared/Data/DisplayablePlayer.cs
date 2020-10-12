namespace ImperitWASM.Shared.Data
{
	public class DisplayablePlayer
	{
		public string Name { get; set; } = "";
		public string Color { get; set; } = "";
		public bool Savage { get; set; }
		public DisplayablePlayer() { }
		public DisplayablePlayer(string name, string color, bool savage)
		{
			Name = name;
			Color = color;
			Savage = savage;
		}
	}
}
