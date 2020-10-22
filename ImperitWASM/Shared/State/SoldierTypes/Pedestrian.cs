namespace ImperitWASM.Shared.State.SoldierTypes
{
	public class Pedestrian : SoldierType
	{
		public override Description Description { get; }
		public override int AttackPower { get; }
		public override int DefensePower { get; }
		public override int Weight { get; }
		public override int Price { get; }
		public Pedestrian(int id, Description description, int attackPower, int defensePower, int weight, int price) : base(id)
		{
			Description = description;
			AttackPower = attackPower;
			DefensePower = defensePower;
			Weight = weight;
			Price = price;
		}
		public override int CanMove(PlayersAndProvinces pap, int from, int to)
		{
			return pap.Province(from) is Land && pap.Province(to) is Land && pap.Passable(from, to, 1, (a, b) => a is Land && b is Land ? 1 : 2) ? Weight : 0;
		}
		public override bool IsRecruitable(Province province) => province is Land;
		public override int CanSustain(Province province) => province is Land ? Weight : 0;
	}
}
