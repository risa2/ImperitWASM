using System;

namespace ImperitWASM.Shared.State.Army
{
	public class Ship : SoldierType
	{
		public override Description Description { get; }
		public override int AttackPower { get; }
		public override int DefensePower { get; }
		public override int Weight { get; }
		public override int Price { get; }
		public int Capacity { get; }
		public Ship(Description description, int attackPower, int defensePower, int weight, int price, int capacity)
		{
			Description = description;
			AttackPower = attackPower;
			DefensePower = defensePower;
			Weight = weight;
			Price = price;
			Capacity = capacity;
		}
		protected static bool IsPassable(Province p) => (p is Land l && l.HasPort) || p is Sea;
		public override int CanMove(PlayersAndProvinces pap, Province from, Province dest)
		{
			return pap.Passable(from, dest, 1, (a, b) => IsPassable(a) && IsPassable(b) ? 1 : 2) ? Capacity + Weight : 0;
		}
		public override bool IsRecruitable(Province p) => p is Land l && l.HasPort;
		public override int CanSustain(Province province) => province is Sea ? Capacity + Weight : (province is Land l && l.HasPort) ? Weight : 0;
	}
}
