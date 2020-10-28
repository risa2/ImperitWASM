namespace ImperitWASM.Shared.State
{
	public class PlayerPower
	{
		public readonly bool Alive;
		public readonly int Soldiers, Lands, Income, Money, Final;
		public PlayerPower(bool alive, int income, int lands, int money, int soldiers, int final)
		{
			Alive = alive;
			Soldiers = soldiers;
			Lands = lands;
			Income = income;
			Money = money;
			Final = final;
		}
		public int Total => Alive ? Soldiers + Money + (Income * 5) + (Final * 25) : 0;
	}
}