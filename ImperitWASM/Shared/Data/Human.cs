using System.Collections.Immutable;
using ImperitWASM.Shared.Config;

namespace ImperitWASM.Shared.Data
{
	public record Human(Color Color, string Name, int Money, bool Alive, ImmutableList<IPlayerAction> Actions, Settings Settings, Password Password)
		: Player(Color, Name, Money, Alive, Actions, Settings)
	{
		public static Human Create(Color Color, string Name, int Money, Settings Settings, Password Password) => new Human(Color, Name, Money, true, DefaultActions, Settings, Password);
		public virtual bool Equals(Human? obj) => obj is not null && Name == obj.Name;
		public override int GetHashCode() => Name.GetHashCode();
	}
}
