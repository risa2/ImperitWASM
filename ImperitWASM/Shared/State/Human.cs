using System.Collections.Immutable;
using ImperitWASM.Shared.Motion;

namespace ImperitWASM.Shared.State
{
	public record Human(Color Color, string Name, int Money, bool Alive, ImmutableList<IPlayerAction> Actions, Password Password)
		: Player(Color, Name, Money, Alive, Actions)
	{
		public override Player ChangeMoney(int amount) => new Human(Color, Name, Money + amount, Alive, Actions, Password);
		public override Player Die() => new Human(Color, Name, 0, false, ImmutableList<IPlayerAction>.Empty, Password);
		protected override Player WithActions(ImmutableList<IPlayerAction> new_actions) => new Human(Color, Name, Money, Alive, new_actions, Password);
	}
}
