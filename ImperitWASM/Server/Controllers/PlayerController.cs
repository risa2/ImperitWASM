using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImperitWASM.Client.Data;
using ImperitWASM.Server.Load;
using ImperitWASM.Server.Services;
using ImperitWASM.Shared.Config;
using ImperitWASM.Shared.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ImperitWASM.Server.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class PlayerController : ControllerBase
	{
		readonly ISessionService session;
		readonly IPlayersProvinces pap;
		readonly IContextService ctx;
		readonly IGameCreator gameCreator;
		readonly IGameService gs;
		readonly IActive active;
		public PlayerController(IPlayersProvinces pap, ISessionService session, IContextService ctx, IGameCreator gameCreator, IGameService gs, IActive active)
		{
			this.pap = pap;
			this.session = session;
			this.ctx = ctx;
			this.gameCreator = gameCreator;
			this.gs = gs;
			this.active = active;
		}
		[HttpPost("Colored")]
		public IEnumerable<ColoredPlayer> Colored([FromBody] int gameId) => ctx.Players.Where(p => p.Type != EntityPlayer.Kind.Savage && p.GameId == gameId).OrderBy(p => p.Index).Select(p => new ColoredPlayer(p.Name, Color.Parse(p.Color)));
		[HttpPost("Money")]
		public int Money([FromBody] Session ses) => ctx.Players.SingleOrDefault(p => p.GameId == ses.G && p.Index == ses.P)?.Money ?? 0;
		[HttpPost("Infos")]
		public IEnumerable<PlayerInfo> Infos([FromBody] Session ses)
		{
			if (!session.IsValid(ses.P, ses.G, ses.Key))
			{
				return Enumerable.Empty<PlayerInfo>();
			}
			var p_p = pap[ses.G];
			var next = p_p.Act(active[ses.G]);
			return p_p.Players.Select((p, i) => new PlayerInfo(i, p is not Savage, p.Name, p.Color, p.Alive, p.Money, p.Debt, next.IncomeOf(next.Player(i))));
		}
		[HttpPost("Correct")]
		public GameState Correct([FromBody] Session user) => session.IsValid(user.P, user.G, user.Key) ? gs.FindNoTracking(user.G)?.GetState() ?? GameState.Invalid : GameState.Invalid;
		[HttpPost("Login")]
		public async Task<Session> Login([FromBody] Login trial)
		{
			await gameCreator.StartAllAsync();
			return ctx.Players.SingleOrDefault(p => p.Type == EntityPlayer.Kind.Human && p.Name == trial.N) is EntityPlayer p && Password.Parse(p.Password).IsCorrect(trial.P) ? new Session(p.Index, p.GameId, await session.AddAsync(p.Index, p.GameId)) : new Session();
		}
		[HttpPost("Color")]
		public Color GetColor([FromBody] PlayerId id) => Color.Parse(ctx.Players.AsNoTracking().SingleOrDefault(p => p.GameId == id.G && p.Index == id.P)?.Color ?? "#00000000");
		[HttpPost("Logout")]
		public Task Logout([FromBody] Session user) => session.RemoveAsync(user.P, user.G, user.Key);
	}
}
