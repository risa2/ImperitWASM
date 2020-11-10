using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ImperitWASM.Shared.Cfg;

namespace ImperitWASM.Shared.Entities
{
	public class Land : Province
	{
		public int Earnings { get; }
		public bool IsStart { get; }
		public bool IsFinal { get; }
		public bool HasPort { get; }
		public IReadOnlyCollection<SoldierType> ExtraTypes { get; }
		public Land(string name, Shape shape, Player player, Soldiers soldiers, Soldiers defaultSoldiers, ImmutableList<ProvinceAction> actions, int earnings, bool isStart, bool isFinal, bool hasPort, IReadOnlyCollection<SoldierType> extraTypes) : base(name, shape, player, soldiers, defaultSoldiers, actions)
		{
			Earnings = earnings;
			IsStart = isStart;
			IsFinal = isFinal;
			HasPort = hasPort;
			ExtraTypes = extraTypes;
		}
		public override Description Description => new Description(Name, Name + "<br/>" + (HasPort ? "\u2693" : "") + Soldiers);
		public override Province GiveUpTo(Player p, Soldiers a) => new Land(Name, Shape, p, a, DefaultSoldiers, Actions, Earnings, IsStart, IsFinal, HasPort, ExtraTypes);
		protected override Province WithActions(ImmutableList<ProvinceAction> new_actions) => new Land(Name, Shape, Player, Soldiers, DefaultSoldiers, new_actions, Earnings, IsStart, IsFinal, HasPort, ExtraTypes);

		public int Price => (Earnings * 2) + Soldiers.Price;
		public override Color Fill(Settings s) => Player.Color.Over(s.LandColor);
		public Ratio Instability(Settings s) => s.Instability(Soldiers, DefaultSoldiers);
		public bool HasExtra(SoldierType t) => ExtraTypes.Contains(t);
	}
}