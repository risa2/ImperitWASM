using System;

namespace ImperitWASM.Client.Server
{
	public class RecruitCmd
	{
		public RecruitCmd(int loggedIn, string loginId, int province, int[] counts, int game)
		{
			LoggedIn = loggedIn;
			LoginId = loginId;
			Province = province;
			Counts = counts;
			Game = game;
		}
		public RecruitCmd() { }
		public int LoggedIn { get; set; }
		public int Game { get; set; }
		public string LoginId { get; set; } = "";
		public int Province { get; set; }
		public int[] Counts { get; set; } = Array.Empty<int>();
	}
}
