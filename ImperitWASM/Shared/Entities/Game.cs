using System;

namespace ImperitWASM.Shared.Entities
{
	public class Game
	{
		public enum State { Created, Countdown, Started, Finished }
		private int _id;
		public Player? Active { get; }
		public State Current { get; }
		public DateTime LastChange { get; }
		public Game(Player? active, State current, DateTime lastChange)
		{
			Active = active;
			Current = current;
			LastChange = lastChange;
		}
		public bool Created => Current == State.Created;
		public bool Countdown => Current == State.Countdown;
		public bool Started => Current == State.Started;
		public bool Finished => Current == State.Finished;
		public static Game Create => new Game(null, State.Created, DateTime.UtcNow);
		public static Game StartCountdown(Player first) => new Game(first, State.Countdown, DateTime.UtcNow);
		public Game Start() => new Game(Active, State.Started, DateTime.UtcNow);
		public Game Finish() => new Game(Active, State.Finished, DateTime.UtcNow);
		public Game SetActive(Player next) => new Game(next, Current, LastChange);
	}
}
