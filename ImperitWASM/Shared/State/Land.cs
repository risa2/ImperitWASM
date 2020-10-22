using ImperitWASM.Shared.Motion;
using System.Collections.Immutable;

namespace ImperitWASM.Shared.State
{
	public class Land : Province
	{
		public readonly bool IsStart;
		public Land(int id, string name, Shape shape, Army army, Army defaultArmy, bool isStart, int earnings, ImmutableList<IProvinceAction> actions, Settings settings)
			: base(id, name, shape, army, defaultArmy, earnings, actions, settings) => IsStart = isStart;
		public override Province GiveUpTo(Army army) => new Land(Id, Name, Shape, army, DefaultArmy, IsStart, Earnings, Actions, settings);
		protected override Province WithActions(ImmutableList<IProvinceAction> new_actions) => new Land(Id, Name, Shape, Army, DefaultArmy, IsStart, Earnings, new_actions, settings);

		public int Price => (Earnings * 2) + Soldiers.Price;
		public override Color Fill => Army.Color.Over(settings.LandColor);
		public override string[] Text => new[] { Name, Earnings + "\uD83D\uDCB0", Army.ToString() };
		public Probability Instability => settings.Instability(Soldiers, DefaultArmy.Soldiers);
	}
}