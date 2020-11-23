using System;
using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace ImperitWASM.Shared.State
{
	[JsonConverter(typeof(Cvt.DescriptionConverter))]
	public class Description : IEquatable<Description>
	{
		public readonly string Name, Symbol;
		public readonly ImmutableArray<string> Text;
		public Description(string? name = null, ImmutableArray<string> text = default, string? symbol = null)
		{
			Name = name ?? "";
			Text = text.IsDefault ? ImmutableArray<string>.Empty : text;
			Symbol = symbol ?? "";
		}
		public Description(string? name, params string[] args) : this(name, args.ToImmutableArray(), "") { }
		public bool Equals(Description? obj) => Name == obj?.Name;
		public override bool Equals(object? obj) => Equals(obj as Description);
		public override int GetHashCode() => Name.GetHashCode();
		public override string ToString() => Name;
		public static bool operator ==(Description? a, Description? b) => a?.Name == b?.Name;
		public static bool operator !=(Description? a, Description? b) => a?.Name != b?.Name;
	}
}
