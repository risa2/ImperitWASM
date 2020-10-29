namespace ImperitWASM.Shared.State
{
	public class PlayerPower
	{
		public readonly bool Alive;
		public readonly int Soldiers, Lands, Income, Money, Final;
		public PlayerPower(bool alive, int income, int lands, int money, int soldiers, int final)
			=> (Alive, Soldiers, Lands, Income, Money, Final) = alive
				? (true, soldiers, lands, income, money, final)
				: (false, 0, 0, 0, 0, 0);
		public int Total => Alive ? Soldiers + Money + (Income * 5) + (Final * 50) : 0;
	}
}