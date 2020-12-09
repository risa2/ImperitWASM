using System.Collections.Immutable;
using ImperitWASM.Shared.Actions;

namespace ImperitWASM.Shared.State
{
	public record Land(string Name, Shape Shape, Player Player, Soldiers Soldiers, Soldiers DefaultSoldiers, ImmutableList<IProvinceAction> Actions, Settings Settings, int Earnings, bool IsStart, bool IsFinal, bool HasPort, ImmutableArray<SoldierType> ExtraTypes)
		: Province(Name, Shape, Player, Soldiers, DefaultSoldiers, Actions, Settings)
	{
		public override ImmutableArray<string> Text => ImmutableArray.Create(Name + (IsFinal ? "\u2605" : "") + (HasPort ? "\u2693" : ""), Soldiers.ToString(), Earnings + "\uD83D\uDCB0");
		public bool IsInhabitable => IsStart && !Occupied;
		public bool CanRevolt => Occupied && Instability.Any;
		public virtual bool Equals(Land? other) => other is not null && other.Name == Name;
		public override int GetHashCode() => base.GetHashCode();

		public int Price => (Earnings * 2) + Soldiers.Price;
		public override Color Fill => Player.Color.Over(Settings.LandColor);
		public Ratio Instability => Settings.Instability(Soldiers, DefaultSoldiers);
		public bool CanRecruit(SoldierType t) => ExtraTypes.Contains(t);
		public override bool ShouldRevolt(Player active) => base.ShouldRevolt(active) || (IsAllyOf(active) && Instability.RandomBool);
	}
}