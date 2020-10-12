using System;

namespace ImperitWASM.Shared.Data
{
	public class RecruitInfo
	{
		public string Name { get; set; } = "";
		public string Soldiers { get; set; } = "";
		public SoldiersItem[] Recruitable { get; set; } = Array.Empty<SoldiersItem>();
		public int Money { get; set; }
		public RecruitInfo() { }
		public RecruitInfo(string name, string soldiers, SoldiersItem[] recruitable, int money)
		{
			Name = name;
			Soldiers = soldiers;
			Recruitable = recruitable;
			Money = money;
		}
	}
}
