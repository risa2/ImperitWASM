using System;

namespace ImperitWASM.Shared.Data
{
	public sealed record Game(Game.State Current, DateTimeOffset StartTime, DateTimeOffset FinishTime)
	{
		public enum State { Created, Countdown, Started, Finished }
		public static Game Create => new Game(State.Created, DateTimeOffset.MaxValue, DateTimeOffset.MaxValue);
		public Game CountDown(TimeSpan delay) => this with { Current = State.Countdown, StartTime = DateTimeOffset.UtcNow.Add(delay) };
		public Game Start() => this with { Current = State.Started };
		public Game Finish() => this with { Current = State.Finished, FinishTime = DateTimeOffset.UtcNow };

		public bool ShouldStart => Current == State.Countdown && DateTimeOffset.UtcNow >= StartTime;
		public bool Started => Current == State.Started;
	}
}
