using System;
using System.Linq;
using System.Threading.Tasks;
using ImperitWASM.Server.Controllers;
using ImperitWASM.Server.Load;

namespace ImperitWASM.Server.Services
{
	public interface ISessionService
	{
		Task<string> Add(int player, int gameId);
		bool IsValid(int player, int gameId, string key);
		Task Remove(int player, int gameId, string key);
	}
	public class SessionService : ISessionService
	{
		static readonly Random rng = new Random();
		readonly IContextService ctx;
		public SessionService(IContextService ctx) => this.ctx = ctx;
		public async Task<string> Add(int player, int gameId)
		{
			var buf = new byte[64];
			rng.NextBytes(buf);
			var key = Convert.ToBase64String(buf).TrimEnd('=').Replace('+', '-').Replace('/', '_');
			var s = ctx.Sessions.Add(new EntitySession { GameId = gameId, PlayerIndex = player, SessionKey = key });
			await ctx.SaveAsync();
			return s.Entity.SessionKey;
		}
		public bool IsValid(int player, int gameId, string key) => ctx.Sessions.Any(s => s.GameId == gameId && s.PlayerIndex == player && s.SessionKey == key);
		public Task Remove(int player, int gameId, string key)
		{
			return ctx.Sessions.Remove(ctx.Sessions.FirstOrDefault(s => s.GameId == gameId && s.PlayerIndex == player && s.SessionKey == key)).Context.SaveChangesAsync();
		}
	}
}
