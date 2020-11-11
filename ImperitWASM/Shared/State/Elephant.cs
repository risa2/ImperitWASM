namespace ImperitWASM.Shared.State
{
	public class Elephant : Pedestrian
	{
		public int Capacity { get; }
		public int Speed { get; }
		public Elephant(Description description, int attackPower, int defensePower, int weight, int price, int capacity, int speed)
			: base(description, attackPower, defensePower, weight, price)
		{
			Capacity = capacity;
			Speed = speed;
		}
		int Difficulty(Province to) => to is Land || to is Mountains ? 1 : Speed + 1;
		public override int CanMove(PlayersAndProvinces pap, Province from, Province to)
		{
			return from is Land && to is Land && pap.Passable(from, to, Speed, (_, dest) => Difficulty(dest)) ? Weight + Capacity : 0;
		}
		public override int CanSustain(Province province) => province is Land ? Capacity + Weight : 0;
		public override bool IsRecruitable(Province province) => province is Outland o && o.CanRecruit(this);
	}
}
