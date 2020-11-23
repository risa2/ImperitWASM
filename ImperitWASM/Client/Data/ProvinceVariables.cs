using System;
using System.Collections.Generic;
using ImperitWASM.Shared.State;

namespace ImperitWASM.Client.Data
{
	public class ProvinceVariables
	{
		public ProvinceVariables(IEnumerable<string> text, Color fill)
		{
			T = text;
			F = fill;
		}
		public ProvinceVariables() { }
		public IEnumerable<string> T { get; set; } = Array.Empty<string>();
		public Color F { get; set; }
	}
}
