using System.Collections.Immutable;
using ImperitWASM.Shared.Motion;

namespace ImperitWASM.Shared.State
{
	public record Savage() : Player(new Color(), "", 0, true, ImmutableList<IPlayerAction>.Empty)
	{
		public override Player ChangeMoney(int _) => this;
		public override Player Die() => this;
		protected override Player WithActions(ImmutableList<IPlayerAction> _) => this;
	}
}
