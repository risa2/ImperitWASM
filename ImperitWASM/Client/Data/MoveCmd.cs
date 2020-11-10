using System;

namespace ImperitWASM.Client.Data
{
	public class MoveCmd
	{
		public MoveCmd(int loggedIn, string loginId, int from, int to, int[] counts, int game)
		{
			LoggedIn = loggedIn;
			LoginId = loginId;
			From = from;
			To = to;
			Counts = counts;
			Game = game;
		}
		public MoveCmd() { }
		public int LoggedIn { get; set; }
		public int Game { get; set; }
		public string LoginId { get; set; } = "";
		public int From { get; set; }
		public int To { get; set; }
		public int[] Counts { get; set; } = Array.Empty<int>();
	}
}
