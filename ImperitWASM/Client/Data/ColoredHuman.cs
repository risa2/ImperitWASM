using ImperitWASM.Shared.State;

namespace ImperitWASM.Client.Data
{
	public class ColoredHuman
	{
		public string Name { get; set; } = "";
		public Color Color { get; set; }
		public ColoredHuman() { }
		public ColoredHuman(string name, Color color)
		{
			Name = name;
			Color = color;
		}
	}
}
