using System;
using System.Collections.Generic;

namespace ImperitWASM.Server
{
	public static class RngExtensions
	{
		public static string NextId(this Random rand, int length)
		{
			var buf = new byte[length];
			rand.NextBytes(buf);
			return Convert.ToBase64String(buf).TrimEnd('=').Replace('+', '-').Replace('/', '_');
		}
		public static void Shuffle<T>(this Random rand, IList<T> list)
		{
			int n = list.Count;
			while (n > 1)
			{
				n--;
				int k = rand.Next(n + 1);
				var value = list[k];
				list[k] = list[n];
				list[n] = value;
			}
		}
	}
}