namespace ImperitWASM.Shared.State
{
	public record Elephant(Description Description, int AttackPower, int DefensePower, int Weight, int Price, int Capacity, int Speed)
		: Pedestrian(Description, AttackPower, DefensePower, Weight, Price)
	{
		int Difficulty(Province to) => to is Land or Mountains ? 1 : Speed + 1;
		public override int CanMove(PlayersAndProvinces pap, Province from, Province to)
		{
			return from is Land && to is Land && pap.Passable(from, to, Speed, (_, dest) => Difficulty(dest)) ? Weight + Capacity : 0;
		}
		public override int CanSustain(Province province) => province is Land ? Capacity + Weight : 0;
		public override bool IsRecruitable(Province province) => province is Land o && o.CanRecruit(this);
	}
}
