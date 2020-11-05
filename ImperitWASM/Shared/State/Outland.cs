using System.Collections.Immutable;
using ImperitWASM.Shared.Motion;

namespace ImperitWASM.Shared.State
{
	public class Outland : Land
	{
		public readonly ImmutableArray<SoldierType> ExtraTypes;
		public Outland(string name, Shape shape, Player player, Soldiers soldiers, Soldiers defaultSoldiers, ImmutableList<IProvinceAction> actions, Settings settings, int earnings, bool isFinal, ImmutableArray<SoldierType> extraTypes)
			: base(name, shape, player, soldiers, defaultSoldiers, actions, settings, earnings, false, isFinal, true) => ExtraTypes = extraTypes;
		public override Province GiveUpTo(Player p, Soldiers s) => new Outland(Name, Shape, p, s, DefaultSoldiers, Actions, settings, Earnings, IsFinal, ExtraTypes);
		protected override Province WithActions(ImmutableList<IProvinceAction> new_actions) => new Outland(Name, Shape, Player, Soldiers, DefaultSoldiers, new_actions, settings, Earnings, IsFinal, ExtraTypes);
		public bool CanRecruit(SoldierType type) => ExtraTypes.Contains(type);
	}
}
