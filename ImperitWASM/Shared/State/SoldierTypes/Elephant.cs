using System;
using System.Collections.Immutable;

namespace ImperitWASM.Shared.State.SoldierTypes
{
	public class Elephant : Pedestrian
	{
		public int Capacity { get; }
		public int Speed { get; }
		public ImmutableArray<int> RecruitPlaces { get; }
		public Elephant(int id, Description description, int attackPower, int defensePower, int weight, int price, int capacity, int speed, ImmutableArray<int> recruitPlaces)
			: base(id, description, attackPower, defensePower, weight, price)
		{
			Capacity = capacity;
			Speed = speed;
			RecruitPlaces = recruitPlaces;
		}
		protected override IComparable Identity => (base.Identity, Capacity, Speed, RecruitPlaces);
		int Difficulty(Province to) => to is Land || to is Mountains ? 1 : Speed + 1;
		public override int CanMove(PlayersAndProvinces pap, int from, int to)
		{
			return pap.Province(from) is Land && pap.Province(to) is Land && pap.Passable(from, to, Speed, (_, dest) => Difficulty(dest)) ? Weight + Capacity : 0;
		}
		public override int CanSustain(Province province) => province is Land ? Capacity + Weight : 0;
		public override bool IsRecruitable(Province province) => RecruitPlaces.Contains(province.Id);
	}
}
