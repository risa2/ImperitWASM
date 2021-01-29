using ImperitWASM.Shared.Data;

namespace ImperitWASM.Shared.Config
{
	public record Pedestrian(Description Description, int AttackPower, int DefensePower, int Weight, int Price)
		: SoldierType(Description, AttackPower, DefensePower, Weight, Price)
	{
		public override int CanMove(Provinces provinces, Province from, Province to)
		{
			return from.Mainland && to.Mainland && provinces.Passable(from, to, 1, (a, b) => a.Mainland && b.Mainland ? 1 : 2) ? Weight : 0;
		}
		public override bool IsRecruitable(Region province) => province.Mainland;
		public override int CanSustain(Region province) => province.Mainland ? Weight : 0;
	}
}
