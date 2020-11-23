using System.Collections.Immutable;

namespace ImperitWASM.Client.Data
{
	public class RecruitInfo
	{
		public string N { get; set; } = "";
		public string S { get; set; } = "";
		public ImmutableArray<SoldiersItem> R { get; set; }
		public int M { get; set; }
		public RecruitInfo() { }
		public RecruitInfo(string name, string soldiers, ImmutableArray<SoldiersItem> recruitable, int money)
		{
			N = name;
			S = soldiers;
			R = recruitable;
			M = money;
		}
	}
}
