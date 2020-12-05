using System.Collections.Immutable;
using ImperitWASM.Shared.Motion;

namespace ImperitWASM.Shared.State
{
	public record Human(Color Color, string Name, int Money, bool Alive, ImmutableList<IPlayerAction> Actions, Settings Settings, Password Password)
		: Player(Color, Name, Money, Alive, Actions, Settings)
	{
		public virtual bool Equals(Human? obj) => obj is not null && Name == obj.Name;
		public override int GetHashCode() => Name.GetHashCode();
	}
}
