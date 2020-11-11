using System;
using System.Text.Json.Serialization;

namespace ImperitWASM.Shared.State
{
	[JsonConverter(typeof(Cvt.DescriptionConverter))]
	public class Description : IEquatable<Description>
	{
		public readonly string Name, Text, Symbol;
		public Description(string? name = null, string? text = null, string? symbol = null)
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
