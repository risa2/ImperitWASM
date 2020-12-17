using System;
using System.Linq;
using System.Threading.Tasks;
using ImperitWASM.Server.Load;

namespace ImperitWASM.Server.Services
{
	public interface ISessionService
	{
		Task<string> AddAsync(int player, int gameId);
		bool IsValid(int player, int gameId, string key);
		Task RemoveAsync(int player, int gameId, string key);
	}
	public class SessionService : ISessionService
	{
		static readonly Random rng = new Random();
		readonly IContextService ctx;
		public SessionService(IContextService ctx) => this.ctx = ctx;
		static string GenerateToken(int len)
		{
			byte[]? buf = new byte[len];
			rng.NextBytes(buf);
			return Convert.ToBase64String(buf).TrimEnd('=').Replace('+', '-').Replace('/', '_');
		}
		public async Task<string> AddAsync(int player, int gameId)
		{
			var s = ctx.Sessions.Add(new EntitySession { GameId = gameId, PlayerIndex = player, SessionKey = GenerateToken(64) });
			await ctx.SaveAsync();
			return s.Entity.SessionKey;
		}
		public bool IsValid(int player, int gameId, string key) => ctx.Sessions.Any(s => s.GameId == gameId && s.PlayerIndex == player && s.SessionKey == key);
		public Task RemoveAsync(int player, int gameId, string key) => ctx.Sessions.Remove(ctx.Sessions.First(s => s.GameId == gameId && s.PlayerIndex == player && s.SessionKey == key)).Context.SaveChangesAsync();
	}
}
