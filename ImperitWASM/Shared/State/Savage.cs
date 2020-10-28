using ImperitWASM.Shared.Motion;
using System.Collections.Immutable;

namespace ImperitWASM.Shared.State
{
	public class Savage : Player
	{
		public Savage(int id)
			: base(id, new Color(), 0, true, ImmutableList<IPlayerAction>.Empty) { }
		public override Player ChangeMoney(int amount) => new Savage(Id);
		public override Player Die() => new Savage(Id);
		protected override Player WithActions(ImmutableList<IPlayerAction> new_actions) => new Savage(Id);
	}
}
