using ImperitWASM.Shared.Cfg;

namespace ImperitWASM.Client.Server
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
