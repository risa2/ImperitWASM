namespace ImperitWASM.Server.Services
{
	public interface IActive
	{
		int this[int gameId] { get; set; }
	}
	public class Active : IActive
	{
		private readonly IContextService ctx;
		public Active(IContextService ctx) => this.ctx = ctx;
		public int this[int gameId]
		{
			get => ctx.Games.Find(gameId).Active;
			set => ctx.Games.UpdateAt(gameId, g => g.SetActive(value));
		}
	}
}
