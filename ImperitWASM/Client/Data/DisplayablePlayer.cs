using ImperitWASM.Shared.State;

namespace ImperitWASM.Client.Data
{
	public class DisplayablePlayer
	{
		public string Name { get; set; } = "";
		public Color Color { get; set; }
		public DisplayablePlayer() { }
		public DisplayablePlayer(string name, Color color)
		{
			Name = name;
			Color = color;
		}
	}
}
