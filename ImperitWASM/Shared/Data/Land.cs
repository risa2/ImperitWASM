using System.Collections.Immutable;
using ImperitWASM.Shared.Config;

namespace ImperitWASM.Shared.Data
{
	public record Land(string Name, Shape Shape, Player Player, Soldiers Soldiers, Soldiers DefaultSoldiers, ImmutableList<IProvinceAction> Actions, Settings Settings, int Earnings, bool IsStart, bool IsFinal, bool HasPort, ImmutableArray<SoldierType> ExtraTypes)
		: Province(Name, Shape, Player, Soldiers, DefaultSoldiers, Actions, Settings)
	{
		public override ImmutableArray<string> Text => ImmutableArray.Create(Name + (IsFinal ? "\u2605" : "") + (HasPort ? "\u2693" : ""), Soldiers.ToString(), Earnings + "\uD83D\uDCB0");
		public override bool Inhabitable => IsStart && !Inhabited;
		public bool Unstable => Inhabited && Instability.Any;
		public virtual bool Equals(Land? other) => other is not null && other.Name == Name;
		public override int GetHashCode() => base.GetHashCode();

		public int Price => Settings.LandPrice(Soldiers, Earnings);
		public override Color Fill => Player.Color.Over(Settings.LandColor);
		public Ratio Instability => Settings.Instability(Soldiers, DefaultSoldiers);
		public bool CanRecruit(SoldierType t) => ExtraTypes.Contains(t);
		public override bool WillRevolt(Player active) => base.WillRevolt(active) || (IsAllyOf(active) && Instability.RandomBool);
	}
}