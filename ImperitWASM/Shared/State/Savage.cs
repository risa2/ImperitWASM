using System.Collections.Immutable;
using ImperitWASM.Shared.Motion;

namespace ImperitWASM.Shared.State
{
	public record Savage() : Player(new Color(), "", 0, true, ImmutableList<IPlayerAction>.Empty)
	{
		public override Player Die() => this;
		public virtual bool Equals(Savage? obj) => obj is not null;
		public override int GetHashCode() => base.GetHashCode();
	}
}
