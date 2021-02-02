using System;

namespace ImperitWASM.Shared.Data
{
	public sealed record Game(Game.State Current, DateTimeOffset StartTime, DateTimeOffset FinishTime)
	{
		public enum State { Created, Countdown, Started, Finished, Invalid = -1 }
		public static Game Create => new Game(State.Created, DateTimeOffset.MaxValue, DateTimeOffset.MaxValue);
		public static Game CountDown(TimeSpan delay) => new Game(State.Countdown, DateTimeOffset.UtcNow.Add(delay), DateTimeOffset.MaxValue);
		public static Game Start => new Game(State.Countdown, DateTimeOffset.UtcNow, DateTimeOffset.MaxValue);
		public Game Finish() => this with { Current = State.Finished, FinishTime = DateTimeOffset.UtcNow };

		public bool ShouldStart => Current == State.Countdown && DateTimeOffset.UtcNow >= StartTime;
		public bool Started => Current == State.Started;
	}
}
