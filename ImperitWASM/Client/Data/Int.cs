namespace ImperitWASM.Client.Data
{
	public class Int
	{
		public int Value { get; set; } = 0;
		public static Int[] Array(int count)
		{
			var arr = new Int[count];
			for (int i = 0; i < count; ++i)
			{
				arr[i] = new Int();
			}
			return arr;
		}
	}
}
