using System.Collections.Immutable;
using ImperitWASM.Shared.Motion;

namespace ImperitWASM.Shared.State
{
	public class Land : Province
	{
		public readonly bool IsStart, IsFinal;
		public Land(int id, string name, Shape shape, Army army, Army defaultArmy, bool isStart, int earnings, ImmutableList<IProvinceAction> actions, Settings settings, bool isFinal)
			: base(id, name, shape, army, defaultArmy, earnings, actions, settings) => (IsStart, IsFinal) = (isStart, isFinal);
		public override Province GiveUpTo(Army army) => new Land(Id, Name, Shape, army, DefaultArmy, IsStart, Earnings, Actions, settings, IsFinal);
		protected override Province WithActions(ImmutableList<IProvinceAction> new_actions) => new Land(Id, Name, Shape, Army, DefaultArmy, IsStart, Earnings, new_actions, settings, IsFinal);

		public int Price => (Earnings * 2) + Soldiers.Price;
		public override Color Fill => Army.Color.Over(settings.LandColor);
		public override string[] Text => new[] { Name, Earnings + "\uD83D\uDCB0", Army.ToString() };
		public Probability Instability => settings.Instability(Soldiers, DefaultArmy.Soldiers);
	}
}