using System.Collections.Immutable;
using ImperitWASM.Shared.Motion;

namespace ImperitWASM.Shared.State
{
	public class Mountains : Province
	{
		readonly Settings settings;
		public Mountains(Shape shape, Settings set)
			: base(new Description(), shape, new Savage(), new Soldiers(), new Soldiers(), ImmutableList<IProvinceAction>.Empty) => settings = set;
		public override Color Stroke => settings.MountainsColor;
		public override int StrokeWidth => settings.MountainsWidth;
		public override Province GiveUpTo(Player p, Soldiers s) => this;
		protected override Province WithActions(ImmutableList<IProvinceAction> new_actions) => this;
	}
}