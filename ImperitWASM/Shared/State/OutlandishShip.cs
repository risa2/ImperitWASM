namespace ImperitWASM.Shared.State
{
	public record OutlandishShip(Description Description, int AttackPower, int DefensePower, int Weight, int Price, int Capacity, int Speed)
		: Ship(Description, AttackPower, DefensePower, Weight, Price, Capacity)
	{
		int Difficulty(Province to) => IsPassable(to) ? 1 : Speed + 1;
		public override int CanMove(PlayersAndProvinces pap, Province from, Province dest)
		{
			return IsPassable(from) && IsPassable(dest) && pap.Passable(from, dest, Speed, (_, to) => Difficulty(to)) ? Capacity + Weight : 0;
		}
		public override bool IsRecruitable(Province province) => province is Land o && o.CanRecruit(this);
	}
}
