namespace ImperitWASM.Shared.State
{
	public record Ship(Description Description, int AttackPower, int DefensePower, int Weight, int Price, int Capacity)
		: SoldierType(Description, AttackPower, DefensePower, Weight, Price)
	{
		protected static bool IsPassable(Province p) => (p is Land l && l.HasPort) || p is Sea;
		public override int CanMove(PlayersAndProvinces pap, Province from, Province dest)
		{
			return pap.Passable(from, dest, 1, (a, b) => IsPassable(a) && IsPassable(b) ? 1 : 2) ? Capacity + Weight : 0;
		}
		public override bool IsRecruitable(Province p) => p is Land l && l.HasPort;
		public override int CanSustain(Province province) => province is Sea ? Capacity + Weight : (province is Land l && l.HasPort) ? Weight : 0;
	}
}
