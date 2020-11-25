using System.Collections.Immutable;
using ImperitWASM.Shared.Motion;

namespace ImperitWASM.Shared.State
{
	public record Land(string Name, Shape Shape, Player Player, Soldiers Soldiers, Soldiers DefaultSoldiers, ImmutableList<IProvinceAction> Actions, Settings Settings, int Earnings, bool IsStart, bool IsFinal, bool HasPort, ImmutableArray<SoldierType> ExtraTypes)
		: Province(new Description(Name, Name + (HasPort ? "\u2693" : ""), Soldiers.ToString()), Shape, Player, Soldiers, DefaultSoldiers, Actions)
	{
		public bool IsInhabitable => IsStart && !Occupied;
		public override Province GiveUpTo(Player p, Soldiers s) => new Land(Name, Shape, p, s, DefaultSoldiers, Actions, Settings, Earnings, IsStart, IsFinal, HasPort, ExtraTypes);
		protected override Province WithActions(ImmutableList<IProvinceAction> new_actions) => new Land(Name, Shape, Player, Soldiers, DefaultSoldiers, new_actions, Settings, Earnings, IsStart, IsFinal, HasPort, ExtraTypes);

		public int Price => (Earnings * 2) + Soldiers.Price;
		public override Color Fill => Player.Color.Over(Settings.LandColor);
		public Ratio Instability => Settings.Instability(Soldiers, DefaultSoldiers);
		public bool CanRecruit(SoldierType t) => ExtraTypes.Contains(t);
	}
}