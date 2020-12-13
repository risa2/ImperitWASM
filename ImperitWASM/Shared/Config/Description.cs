using System.Collections.Immutable;

namespace ImperitWASM.Shared.Config
{
	public record Description(string Name, ImmutableArray<string> Text, string Symbol = "");
}
