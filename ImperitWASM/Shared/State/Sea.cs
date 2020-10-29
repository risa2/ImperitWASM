using System.Collections.Immutable;
using ImperitWASM.Shared.Motion;

namespace ImperitWASM.Shared.State
{
	public class Sea : Province
	{
		public Sea(int id, string name, Shape shape, Army army, Army defaultArmy, ImmutableList<IProvinceAction> actions, Settings settings)
			: base(id, name, shape, army, defaultArmy, 0, actions, settings) { }
		public override Province GiveUpTo(Army army) => new Sea(Id, Name, Shape, army, DefaultArmy, Actions, settings);
		protected override Province WithActions(ImmutableList<IProvinceAction> new_actions) => new Sea(Id, Name, Shape, Army, DefaultArmy, new_actions, settings);

		public override Color Fill => Army.Color.Mix(settings.SeaColor);
		public override string[] Text => new[] { Name, Army.ToString() };
	}
}