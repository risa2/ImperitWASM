using System;

namespace ImperitWASM.Shared.State
{
	public class Game
	{
		public readonly bool IsActive;
		public readonly DateTime FirstRegistration;
		public Game(bool isActive, DateTime firstRegistration)
		{
			IsActive = isActive;
			FirstRegistration = firstRegistration;
		}
		static DateTime Min(DateTime a, DateTime b) => a < b ? a : b;
		public static Game Start() => new Game(true, DateTime.MinValue);
		public static Game Finish() => new Game(false, DateTime.MaxValue);
		public Game Register() => new Game(false, Min(DateTime.UtcNow, FirstRegistration));
	}
}
