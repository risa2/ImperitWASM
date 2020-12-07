using System.Collections.Generic;

namespace ImperitWASM.Client.Data
{
	public class Int
	{
		public int Value { get; set; } = 0;
		public static List<Int> Create(int count)
		{
			var arr = new List<Int>(count);
			for (int i = 0; i < count; ++i)
			{
				arr.Add(new Int());
			}
			return arr;
		}
	}
}
