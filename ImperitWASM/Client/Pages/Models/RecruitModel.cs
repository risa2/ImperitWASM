using ImperitWASM.Shared.State;

namespace ImperitWASM.Client.Pages
{
	public class RecruitModel
	{
		public SoldierType[] SoldierTypes = System.Array.Empty<SoldierType>();
		public IntModel[] Soldiers = System.Array.Empty<IntModel>();
		public bool Borrow { get; set; }
	}
}
