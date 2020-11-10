namespace ImperitWASM.Shared.Entities
{
	public class Session
	{
		private int _id;
		public Game Game { get; }
		public Player Player { get; }
		public string SessionKey { get; } = "";
		public Session(Game game, Player player, string sessionKey)
		{
			Game = game;
			Player = player;
			SessionKey = sessionKey;
		}
	}
}
