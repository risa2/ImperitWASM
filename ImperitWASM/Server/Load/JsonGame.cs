using System;
using ImperitWASM.Shared.Conversion;
using ImperitWASM.Shared.State;

namespace ImperitWASM.Server.Load
{
	public class JsonGame : IEntity<Game, Settings>
	{
		public bool IsActive { get; set; }
		public DateTime CountdownStart { get; set; }
		public Game Convert(int i, Settings s) => new Game(IsActive, CountdownStart);
		public static JsonGame From(Game game) => new JsonGame { IsActive = game.IsActive, CountdownStart = game.CountdownStart };
	}
}
