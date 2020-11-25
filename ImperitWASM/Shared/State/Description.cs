using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace ImperitWASM.Shared.State
{
	[JsonConverter(typeof(Cvt.DescriptionConverter))]
	public sealed record Description(string Name, ImmutableArray<string> Text, string Symbol = "")
	{
		public Description(string name = "", params string[] args) : this(name, args.ToImmutableArray(), "") { }
		public bool Equals(Description? obj) => Name == obj?.Name;
		public override int GetHashCode() => Name.GetHashCode();
		public override string ToString() => Name;
	}
}
