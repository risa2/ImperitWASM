using System.ComponentModel.DataAnnotations;

namespace ImperitWASM.Server.Load
{
	public class EntitySession : IEntity
	{
		[Key] public int Id { get; set; }
		public EntityGame EntityGame { get; set; } = new EntityGame();
		public int EntityGameId { get; set; }
		public int PlayerIndex { get; set; }
		public string SessionKey { get; set; } = "";
	}
}
