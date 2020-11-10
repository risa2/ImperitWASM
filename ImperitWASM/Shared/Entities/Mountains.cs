using System.Collections.Immutable;
using ImperitWASM.Shared.Cfg;

namespace ImperitWASM.Shared.Entities
{
	public class Mountains : Province
	{
		public Mountains(string name, Shape shape)
			: base(name, shape, new Savage(), Soldiers.Empty, Soldiers.Empty, ImmutableList<ProvinceAction>.Empty) { }
		public override Description Description => new Description();
		public override Color Stroke(Settings s) => s.MountainsColor;
		public override int StrokeWidth(Settings s) => s.MountainsWidth;
		public override Province GiveUpTo(Player p, Soldiers a) => this;
		protected override Province WithActions(ImmutableList<ProvinceAction> new_actions) => this;
	}
}