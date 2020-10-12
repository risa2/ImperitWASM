using ImperitWASM.Shared.Conversion;
using ImperitWASM.Shared.State;
using System;

namespace ImperitWASM.Server.Load
{
	public class JsonGame : IEntity<Game, Settings>
	{
		public bool IsActive { get; set; }
		public DateTime FirstRegistration { get; set; }
		public Game Convert(int i, Settings s) => new Game(IsActive, FirstRegistration);
		public static JsonGame From(Game game) => new JsonGame { IsActive = game.IsActive, FirstRegistration = game.FirstRegistration };
	}
}
