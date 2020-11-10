using System.Collections.Immutable;
using ImperitWASM.Shared.Cfg;

namespace ImperitWASM.Shared.Entities
{
	public class Savage : Player
	{
		public Savage()
			: base(new Color(), new Description(), 0, true, ImmutableList<PlayerAction>.Empty) { }
		public override Player ChangeMoney(int _) => this;
		public override Player Die() => this;
		protected override Player WithActions(ImmutableList<PlayerAction> _) => this;
	}
}
