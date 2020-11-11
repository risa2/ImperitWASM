using ImperitWASM.Shared.State;

namespace ImperitWASM.Client.Data
{
	public class Recruit
	{
		public SoldierType[] SoldierTypes = System.Array.Empty<SoldierType>();
		public Int[] Soldiers = System.Array.Empty<Int>();
		public bool Borrow { get; set; }
	}
}
