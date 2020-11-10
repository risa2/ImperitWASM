using System.Collections.Immutable;
using ImperitWASM.Shared.Cfg;

namespace ImperitWASM.Shared.Entities
{
	public class Human : Player
	{
		public readonly Password Password;
		public Human(Color color, string name, int money, bool alive, ImmutableList<PlayerAction> actions, Password pw)
			: base(color, new Description(name), money, alive, actions) => Password = pw;
		public override Player ChangeMoney(int amount) => new Human(Color, Name, Money + amount, Alive, Actions, Password);
		public override Player Die() => new Human(Color, Name, 0, false, ImmutableList<PlayerAction>.Empty, Password);
		protected override Player WithActions(ImmutableList<PlayerAction> new_actions) => new Human(Color, Name, Money, Alive, new_actions, Password);
	}
}
