using System.ComponentModel.DataAnnotations;
using ImperitWASM.Server.Controllers;

namespace ImperitWASM.Server.Load
{
	public class EntitySession : IEntity<Session, bool>
	{
		[Key] public int Id { get; set; }
		[Required] public int GameId { get; set; }
		[Required] public int PlayerIndex { get; set; }
		[Required] public string SessionKey { get; set; } = "";
		public Session Convert(bool _) => new Session(PlayerIndex, GameId);
	}
}
