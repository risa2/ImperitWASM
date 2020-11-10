namespace ImperitWASM.Shared.Entities
{
	public class PlayerPower
	{
		private int _id;
		public bool Alive { get; }
		public int Soldiers { get; }
		public int Lands { get; }
		public int Income { get; }
		public int Money { get; }
		public int Final { get; }
		public PlayerPower(bool alive, int income, int lands, int money, int soldiers, int final)
			=> (Alive, Soldiers, Lands, Income, Money, Final) = alive
				? (true, soldiers, lands, income, money, final)
				: (false, 0, 0, 0, 0, 0);
		public int Total => Alive ? Soldiers + Money + (Income * 5) + (Final * 50) : 0;
	}
}