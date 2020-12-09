namespace ImperitWASM.Shared.State
{
	public record PlayerPower(bool Alive, int Final, int Income, int Lands, int Money, int Soldiers)
	{
		public int Total => Alive ? Soldiers + Money + (Income * 5) + (Final * 50) : 0;
	}
}