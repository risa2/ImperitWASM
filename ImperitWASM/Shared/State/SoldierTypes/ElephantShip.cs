using System;
using System.Collections.Immutable;

namespace ImperitWASM.Shared.State.SoldierTypes
{
	public class ElephantShip : Ship
	{
		public int Speed { get; }
		public ImmutableArray<int> RecruitPlaces { get; }
		public ElephantShip(int id, Description description, int attackPower, int defensePower, int weight, int price, int capacity, int speed, ImmutableArray<int> recruitPlaces)
			: base(id, description, attackPower, defensePower, weight, price, capacity)
		{
			Speed = speed;
			RecruitPlaces = recruitPlaces;
		}
		protected override IComparable Identity => (base.Identity, Speed, RecruitPlaces);
		int Difficulty(Province to) => IsPassable(to) ? 1 : Speed + 1;
		public override int CanMove(PlayersAndProvinces pap, int from, int dest)
		{
			return IsPassable(pap.Province(from)) && IsPassable(pap.Province(dest))
				&& pap.Passable(from, dest, Speed, (_, to) => Difficulty(to)) ? Capacity + Weight : 0;
		}
		public override int CanSustain(Province province) => province is Sea ? Capacity + Weight : province is Port ? Weight : 0;
		public override bool IsRecruitable(Province province) => RecruitPlaces.Contains(province.Id);
	}
}
