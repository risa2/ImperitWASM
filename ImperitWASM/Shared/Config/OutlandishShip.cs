using ImperitWASM.Shared.Data;

namespace ImperitWASM.Shared.Config
{
	public record OutlandishShip(Description Description, int AttackPower, int DefensePower, int Weight, int Price, int Capacity, int Speed)
		: Ship(Description, AttackPower, DefensePower, Weight, Price, Capacity)
	{
		int Difficulty(Province to) => to.Sailable ? 1 : Speed + 1;
		public override int CanMove(Provinces pap, Province from, Province dest)
		{
			return from.Sailable && dest.Sailable && pap.Passable(from, dest, Speed, (_, to) => Difficulty(to)) ? Capacity + Weight : 0;
		}
		public override bool IsRecruitable(Region province) => province.Recruitable(this);
	}
}
