using ImperitWASM.Shared.State;

namespace ImperitWASM.Shared.Data
{
	public class ProvinceInstability
	{
		public string Name { get; set; } = "";
		public Color Color { get; set; }
		public double Instability { get; set; }
		public ProvinceInstability() { }
		public ProvinceInstability(string name, Color color, double instability)
		{
			Name = name;
			Color = color;
			Instability = instability;
		}
	}
}
