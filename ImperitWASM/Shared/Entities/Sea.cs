using System.Collections.Immutable;
using ImperitWASM.Shared.Cfg;

namespace ImperitWASM.Shared.Entities
{
	public class Sea : Province
	{
		public Sea(string name, Shape shape, Player player, Soldiers soldiers, Soldiers defaultSoldiers, ImmutableList<ProvinceAction> actions)
			: base(name, shape, player, soldiers, defaultSoldiers, actions) { }
		public override Description Description => new Description(Name, string.Format("{0}<br/>{1}", Name, Soldiers));
		public override Province GiveUpTo(Player p, Soldiers a) => new Sea(Name, Shape, p, a, DefaultSoldiers, Actions);
		protected override Province WithActions(ImmutableList<ProvinceAction> new_actions) => new Sea(Name, Shape, Player, Soldiers, DefaultSoldiers, new_actions);
		public override Color Fill(Settings s) => Player.Color.Mix(s.SeaColor);
	}
}