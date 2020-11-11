using System.ComponentModel.DataAnnotations;

namespace ImperitWASM.Server.Load
{
	public class EntitySession : IEntity
	{
		[Key] public int Id { get; set; }
		public Game Game { get; set; } = new Game();
		public int GameId { get; set; }
		public int PlayerIndex { get; set; }
		public string SessionKey { get; set; } = "";
	}
}
