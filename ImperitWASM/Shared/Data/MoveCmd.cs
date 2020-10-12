using System;

namespace ImperitWASM.Shared.Data
{
	public class MoveCmd
	{
		public MoveCmd(int loggedIn, string loginId, bool isAttack, int from, int to, int[] counts)
		{
			LoggedIn = loggedIn;
			LoginId = loginId;
			IsAttack = isAttack;
			From = from;
			To = to;
			Counts = counts;
		}
		public MoveCmd() { }
		public int LoggedIn { get; set; }
		public string LoginId { get; set; } = "";
		public bool IsAttack { get; set; }
		public int From { get; set; }
		public int To { get; set; }
		public int[] Counts { get; set; } = Array.Empty<int>();
	}
}
