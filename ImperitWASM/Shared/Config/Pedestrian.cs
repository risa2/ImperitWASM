using ImperitWASM.Shared.Data;

namespace ImperitWASM.Shared.Config
{
	public record Pedestrian(Description Description, int AttackPower, int DefensePower, int Weight, int Price)
		: SoldierType(Description, AttackPower, DefensePower, Weight, Price)
	{
		public override int CanMove(PlayersAndProvinces pap, Province from, Province to)
		{
			return from is Land && to is Land && pap.Passable(from, to, 1, (a, b) => a is Land && b is Land ? 1 : 2) ? Weight : 0;
		}
		public override bool IsRecruitable(Province province) => province is Land;
		public override int CanSustain(Province province) => province is Land ? Weight : 0;
	}
}
