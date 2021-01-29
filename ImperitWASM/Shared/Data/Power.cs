namespace ImperitWASM.Shared.Data
{
	public record Power(bool Alive, int Final, int Income, int Money, int Soldiers)
	{
		public int Total => Alive ? Soldiers + Money + (Income * 5) + (Final * 100) : 0;
	}
}