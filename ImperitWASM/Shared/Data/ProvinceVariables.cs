using ImperitWASM.Shared.State;
using System;

namespace ImperitWASM.Shared.Data
{
	public class ProvinceVariables
	{
		public ProvinceVariables(string[] text, Color fill)
		{
			Text = text;
			Fill = fill;
		}
		public ProvinceVariables() { }
		public string[] Text { get; set; } = Array.Empty<string>();
		public Color Fill { get; set; }
	}
}
