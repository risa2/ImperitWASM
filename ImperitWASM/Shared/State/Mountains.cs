using System.Collections.Immutable;
using ImperitWASM.Shared.Motion;

namespace ImperitWASM.Shared.State
{
	public record Mountains(string Name, Shape Shape, Settings Settings)
		: Province(new Description(Name), Shape, new Savage(), new Soldiers(), new Soldiers(), ImmutableList<IProvinceAction>.Empty)
	{
		public override Color Stroke => Settings.MountainsColor;
		public override int StrokeWidth => Settings.MountainsWidth;
		public override Province GiveUpTo(Player p, Soldiers s) => this;
		protected override Province WithActions(ImmutableList<IProvinceAction> new_actions) => this;
	}
}