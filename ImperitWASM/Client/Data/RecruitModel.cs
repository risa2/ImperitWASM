using System.Collections.Generic;
using ImperitWASM.Shared.State;

namespace ImperitWASM.Client.Data
{
	public class RecruitModel
	{
		public List<SoldierType> SoldierTypes = new List<SoldierType>();
		public List<Int> Soldiers = new List<Int>();
		public bool Borrow { get; set; }
	}
}
