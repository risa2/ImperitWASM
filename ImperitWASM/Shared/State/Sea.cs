using System.Collections.Immutable;
using ImperitWASM.Shared.Actions;

namespace ImperitWASM.Shared.State
{
	public record Sea(string Name, Shape Shape, Player Player, Soldiers Soldiers, Soldiers DefaultSoldiers, ImmutableList<IProvinceAction> Actions, Settings Settings)
		: Province(Name, Shape, Player, Soldiers, DefaultSoldiers, Actions, Settings)
	{
		public override ImmutableArray<string> Text => ImmutableArray.Create(Name, Soldiers.ToString());
		public virtual bool Equals(Sea? other) => other is not null && other.Name == Name;
		public override int GetHashCode() => base.GetHashCode();
		public override Color Fill => Player.Color.Mix(Settings.SeaColor);
	}
}