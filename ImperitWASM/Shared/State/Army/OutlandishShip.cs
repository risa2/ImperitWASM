namespace ImperitWASM.Shared.State.Army
{
	public class OutlandishShip : Ship
	{
		public int Speed { get; }
		public OutlandishShip(Description description, int attackPower, int defensePower, int weight, int price, int capacity, int speed)
			: base(description, attackPower, defensePower, weight, price, capacity) => Speed = speed;
		int Difficulty(Province to) => IsPassable(to) ? 1 : Speed + 1;
		public override int CanMove(PlayersAndProvinces pap, Province from, Province dest)
		{
			return IsPassable(from) && IsPassable(dest) && pap.Passable(from, dest, Speed, (_, to) => Difficulty(to)) ? Capacity + Weight : 0;
		}
		public override int CanSustain(Province province) => province is Sea ? Capacity + Weight : province is Land l && l.HasPort ? Weight : 0;
		public override bool IsRecruitable(Province province) => province is Outland o && o.CanRecruit(this);
	}
}
