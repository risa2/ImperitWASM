using System.Collections.Immutable;
using ImperitWASM.Shared.Motion;

namespace ImperitWASM.Shared.State
{
	public record Sea(string Name, Shape Shape, Player Player, Soldiers Soldiers, Soldiers DefaultSoldiers, ImmutableList<IProvinceAction> Actions, Settings Settings)
		: Province(new Description(Name, Name, Soldiers.ToString()), Shape, Player, Soldiers, DefaultSoldiers, Actions)
	{
		public override Province GiveUpTo(Player p, Soldiers s) => new Sea(Name, Shape, p, s, DefaultSoldiers, Actions, Settings);
		protected override Province WithActions(ImmutableList<IProvinceAction> new_actions) => new Sea(Name, Shape, Player, Soldiers, DefaultSoldiers, new_actions, Settings);
		public override Color Fill => Player.Color.Mix(Settings.SeaColor);
	}
}