namespace ImperitWASM.Server.Load
{
	public class EntitySession
	{
		public int Id { get; set; }
		public int GameId { get; set; }
		public int PlayerIndex { get; set; }
		public string SessionKey { get; set; } = "";
	}
}
