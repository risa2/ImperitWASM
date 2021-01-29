using ImperitWASM.Shared.Data;

namespace ImperitWASM.Shared.Config
{
	public record Ship(Description Description, int AttackPower, int DefensePower, int Weight, int Price, int Capacity)
		: SoldierType(Description, AttackPower, DefensePower, Weight, Price)
	{
		public override int CanMove(Provinces provinces, Province from, Province dest)
		{
			return provinces.Passable(from, dest, 1, (a, b) => a.Sailable && b.Sailable ? 1 : 2) ? Capacity + Weight : 0;
		}
		public override bool IsRecruitable(Region province) => province.Port;
		public override int CanSustain(Region province) => province.Sailable ? Capacity + Weight : 0;
	}
}
