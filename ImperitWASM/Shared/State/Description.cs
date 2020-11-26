using System.Collections.Immutable;

namespace ImperitWASM.Shared.State
{
	public record Description(string Name, ImmutableArray<string> Text, string Symbol = "");
}
