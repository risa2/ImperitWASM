using System;

namespace ImperitWASM.Shared.State
{
	public class Game
	{
		public readonly bool IsActive;
		public readonly DateTime CountdownStart;
		public Game(bool isActive, DateTime countdownStart)
		{
			IsActive = isActive;
			CountdownStart = countdownStart;
		}
		static DateTime Min(DateTime a, DateTime b) => a < b ? a : b;
		public static Game Start() => new Game(true, DateTime.MinValue);
		public static Game Finish() => new Game(false, DateTime.MaxValue);
		public Game StartCountdown() => new Game(false, Min(DateTime.UtcNow, CountdownStart));
		public TimeSpan TimeSinceCountdownStart => DateTime.UtcNow.Subtract(CountdownStart);
	}
}
