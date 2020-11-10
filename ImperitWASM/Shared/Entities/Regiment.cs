namespace ImperitWASM.Shared.Entities
{
	public class Regiment
	{
		private int _id;
		public SoldierType Type { get; }
		public int Count { get; }
		public Regiment(SoldierType type, int count)
		{
			Type = type;
			Count = count;
		}
	}
}
