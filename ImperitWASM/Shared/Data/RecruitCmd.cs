using System;

namespace ImperitWASM.Shared.Data
{
	public class RecruitCmd
	{
		public RecruitCmd(int loggedIn, string loginId, int province, int[] counts)
		{
			LoggedIn = loggedIn;
			LoginId = loginId;
			Province = province;
			Counts = counts;
		}
		public RecruitCmd() { }
		public int LoggedIn { get; set; }
		public string LoginId { get; set; } = "";
		public int Province { get; set; }
		public int[] Counts { get; set; } = Array.Empty<int>();
	}
}
