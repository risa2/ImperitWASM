using System.Collections.Immutable;
using ImperitWASM.Shared.Motion;

namespace ImperitWASM.Shared.State
{
	public class Sea : Province
	{
		private readonly Settings settings;
		public Sea(string name, Shape shape, Player player, Soldiers soldiers, Soldiers defaultSoldiers, ImmutableList<IProvinceAction> actions, Settings settings)
			: base(new Description(name, string.Format("{0}<br/>{1}", name, soldiers)), shape, player, soldiers, defaultSoldiers, actions) => this.settings = settings;
		public override Province GiveUpTo(Player p, Soldiers s) => new Sea(Name, Shape, p, s, DefaultSoldiers, Actions, settings);
		protected override Province WithActions(ImmutableList<IProvinceAction> new_actions) => new Sea(Name, Shape, Player, Soldiers, DefaultSoldiers, new_actions, settings);
		public override Color Fill => Player.Color.Mix(settings.SeaColor);
	}
}