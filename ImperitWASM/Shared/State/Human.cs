using System.Collections.Immutable;
using ImperitWASM.Shared.Motion;

namespace ImperitWASM.Shared.State
{
	public record Human(Color Color, string Name, int Money, bool Alive, ImmutableList<IPlayerAction> Actions, Password Password)
		: Player(Color, Name, Money, Alive, Actions)
	{
		public override Player Die() => new Human(Color, Name, 0, false, ImmutableList<IPlayerAction>.Empty, Password);
		public virtual bool Equals(Human? obj) => obj is not null && Name == obj.Name;
		public override int GetHashCode() => Name.GetHashCode();
	}
}
