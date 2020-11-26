using System.Collections.Immutable;
using ImperitWASM.Shared.Motion;

namespace ImperitWASM.Shared.State
{
	public record Sea(string Name, Shape Shape, Player Player, Soldiers Soldiers, Soldiers DefaultSoldiers, ImmutableList<IProvinceAction> Actions, Settings Settings)
		: Province(new Description(Name, ImmutableArray.Create(Name, Soldiers.ToString())), Shape, Player, Soldiers, DefaultSoldiers, Actions)
	{
		public virtual bool Equals(Sea? other) => other is not null && other.Name == Name;
		public override int GetHashCode() => base.GetHashCode();
		public override Color Fill => Player.Color.Mix(Settings.SeaColor);
	}
}