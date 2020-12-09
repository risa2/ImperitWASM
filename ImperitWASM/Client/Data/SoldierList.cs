using System.Collections.Generic;

namespace ImperitWASM.Client.Data
{
	public class SoldierList
	{
		public List<Int> Soldiers { get; set; }
		public SoldierList(int count = 0)
		{
			Soldiers = new List<Int>(count);
			for (int i = 0; i < count; ++i)
			{
				Soldiers.Add(new Int());
			}
		}
	}
}
