using System;
using System.Text.Json.Serialization;

namespace ImperitWASM.Shared.Cfg
{
	[JsonConverter(typeof(Cvt.DescriptionConverter))]
	public class Description : IEquatable<Description>
	{
		public string Name { get; }
		public string Text { get; }
		public string Symbol { get; }
		public Description(string name = "", string text = "", string symbol = "")
		{
			Name = name ?? "";
			Text = text ?? "";
			Symbol = symbol ?? "";
		}
		public bool Equals(Description? obj) => Name == obj?.Name;
		public override bool Equals(object? obj) => Equals(obj as Description);
		public override int GetHashCode() => Name.GetHashCode();
		public override string ToString() => Name;
		public static bool operator ==(Description? a, Description? b) => a?.Name == b?.Name;
		public static bool operator !=(Description? a, Description? b) => a?.Name != b?.Name;
	}
}
