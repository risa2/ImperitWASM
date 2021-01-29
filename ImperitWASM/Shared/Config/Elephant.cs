using ImperitWASM.Shared.Data;

namespace ImperitWASM.Shared.Config
{
	public record Elephant(Description Description, int AttackPower, int DefensePower, int Weight, int Price, int Capacity, int Speed)
		: Pedestrian(Description, AttackPower, DefensePower, Weight, Price)
	{
		int Difficulty(Province to) => to.Dry ? 1 : Speed + 1;
		public override int CanMove(Provinces provinces, Province from, Province to)
		{
			return from.Mainland && to.Mainland && provinces.Passable(from, to, Speed, (_, dest) => Difficulty(dest)) ? Weight + Capacity : 0;
		}
		public override int CanSustain(Region province) => province.Mainland ? Capacity + Weight : 0;
		public override bool IsRecruitable(Region province) => province.Recruitable(this);
	}
}
