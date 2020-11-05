using System;

namespace ImperitWASM.Client.Server
{
	public class RecruitInfo
	{
		public string N { get; set; } = "";
		public string S { get; set; } = "";
		public SoldiersItem[] R { get; set; } = Array.Empty<SoldiersItem>();
		public int M { get; set; }
		public RecruitInfo() { }
		public RecruitInfo(string name, string soldiers, SoldiersItem[] recruitable, int money)
		{
			N = name;
			S = soldiers;
			R = recruitable;
			M = money;
		}
	}
}
