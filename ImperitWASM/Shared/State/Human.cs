using System.Collections.Immutable;
using ImperitWASM.Shared.Motion;

namespace ImperitWASM.Shared.State
{
	public class Human : Player
	{
		public readonly string Name;
		public readonly Password Password;
		public Human(int id, Color color, int money, bool alive, ImmutableList<IPlayerAction> actions, string name, Password pw)
			: base(id, color, money, alive, actions)
		{
			Name = name;
			Password = pw;
		}
		public override Player ChangeMoney(int amount) => new Human(Id, Color, Money + amount, Alive, Actions, Name, Password);
		public override Player Die() => new Human(Id, Color, 0, false, ImmutableList<IPlayerAction>.Empty, Name, Password);
		protected override Player WithActions(ImmutableList<IPlayerAction> new_actions) => new Human(Id, Color, Money, Alive, new_actions, Name, Password);
	}
}
