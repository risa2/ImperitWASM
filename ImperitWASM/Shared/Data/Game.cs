using System;

namespace ImperitWASM.Shared.Data
{
	public sealed record Game(Game.State Current, DateTime StartTime, DateTime FinishTime)
	{
		public enum State { Created, Countdown, Started, Finished }
		public static Game Create => new Game(State.Created, DateTime.MaxValue, DateTime.MaxValue);
		public Game CountDown(TimeSpan delay) => this with { Current = State.Countdown, StartTime = DateTime.UtcNow.Add(delay) };
		public Game Start() => this with { Current = State.Started };
		public Game Finish() => this with { Current = State.Finished, FinishTime = DateTime.UtcNow };

		public bool ShouldStart => Current == State.Countdown && DateTime.UtcNow >= StartTime;
		public bool Started => Current == State.Started;
	}
}
