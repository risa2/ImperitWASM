using ImperitWASM.Shared.State;

namespace ImperitWASM.Client.Server
{
	public class ProvinceVariables
	{
		public ProvinceVariables(string text, Color fill)
		{
			T = text;
			F = fill;
		}
		public ProvinceVariables() { }
		public string T { get; set; } = "";
		public Color F { get; set; }
	}
}
