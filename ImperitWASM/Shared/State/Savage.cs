using System.Collections.Immutable;
using ImperitWASM.Shared.Actions;

namespace ImperitWASM.Shared.State
{
	public record Savage(Settings Settings) : Player(new Color(), "", 0, true, ImmutableList<IPlayerAction>.Empty, Settings)
	{
		public virtual bool Equals(Savage? obj) => obj is not null;
		public override int GetHashCode() => base.GetHashCode();
	}
}
