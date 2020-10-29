using System;
using System.ComponentModel.DataAnnotations;
using ImperitWASM.Shared.Conversion;
using ImperitWASM.Shared.State;

namespace ImperitWASM.Server.Load
{
	public class EntityGame : IEntity<Game, bool>
	{
		[Key] public int Id { get; set; }
		public bool IsActive { get; set; }
		public DateTime CountdownStart { get; set; }
		public Game Convert(bool i) => new Game(IsActive, CountdownStart);
		public static EntityGame From(Game game) => new EntityGame { IsActive = game.IsActive, CountdownStart = game.CountdownStart };
	}
}
