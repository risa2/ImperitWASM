using System.Collections.Immutable;
using ImperitWASM.Shared.Motion;

namespace ImperitWASM.Shared.State
{
	public class Land : Province
	{
		protected readonly Settings settings;
		public readonly int Earnings;
		public readonly bool IsStart, IsFinal, HasPort;
		public readonly ImmutableArray<SoldierType> ExtraTypes;
		public Land(string name, Shape shape, Player player, Soldiers soldiers, Soldiers defaultSoldiers, ImmutableList<IProvinceAction> actions, Settings settings, int earnings, bool isStart, bool isFinal, bool hasPort, ImmutableArray<SoldierType> extraTypes)
			: base(new Description(name, string.Format("{0}<br/>{1}", name + (hasPort ? "\u2693" : ""), soldiers)), shape, player, soldiers, defaultSoldiers, actions)
		{
			this.settings = settings;
			Earnings = earnings;
			IsStart = isStart;
			IsFinal = isFinal;
			HasPort = hasPort;
			ExtraTypes = extraTypes;
		}
		public override Province GiveUpTo(Player p, Soldiers s) => new Land(Name, Shape, p, s, DefaultSoldiers, Actions, settings, Earnings, IsStart, IsFinal, HasPort, ExtraTypes);
		protected override Province WithActions(ImmutableList<IProvinceAction> new_actions) => new Land(Name, Shape, Player, Soldiers, DefaultSoldiers, new_actions, settings, Earnings, IsStart, IsFinal, HasPort, ExtraTypes);

		public int Price => (Earnings * 2) + Soldiers.Price;
		public override Color Fill => Player.Color.Over(settings.LandColor);
		public Ratio Instability => settings.Instability(Soldiers, DefaultSoldiers);
		public bool CanRecruit(SoldierType t) => ExtraTypes.Contains(t);
	}
}