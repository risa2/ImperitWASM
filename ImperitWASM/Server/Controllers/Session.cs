using System;

namespace ImperitWASM.Server.Controllers
{
	public readonly struct Session : IEquatable<Session>
	{
		public readonly int Player, Game;
		public Session(int player, int game)
		{
			Player = player;
			Game = game;
		}
		public bool Equals(Session other) => (Player, Game) == (other.Player, other.Game);
		public override bool Equals(object? obj) => obj is Session s && Equals(s);
		public override int GetHashCode() => (Player, Game).GetHashCode();
	}
}
