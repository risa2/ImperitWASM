using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImperitWASM.Client.Data;
using ImperitWASM.Server.Load;
using ImperitWASM.Server.Services;
using ImperitWASM.Shared.Motion;
using ImperitWASM.Shared.State;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ImperitWASM.Server.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class PlayerController : ControllerBase
	{
		readonly ISessionService login;
		readonly IPlayersProvinces pap;
		readonly IContextService ctx;
		readonly IGameCreator newGame;
		public PlayerController(IPlayersProvinces pap, ISessionService login, IContextService ctx, IGameCreator newGame)
		{
			this.pap = pap;
			this.login = login;
			this.ctx = ctx;
			this.newGame = newGame;
		}
		[HttpPost("Colored")]
		public IEnumerable<ColoredHuman> Colored([FromBody] int gameId) => ctx.Players.Where(p => p.Type == EntityPlayer.Kind.Human && p.GameId == gameId).OrderBy(p => p.Index).Select(p => new Client.Data.ColoredHuman(p.Name, Color.Parse(p.Color)));
		[HttpGet("Logins")]
		public async Task<IEnumerable<PlayerId>> Login()
		{
			await newGame.StartAllAsync();
			return ctx.Players.Include(p => p.Game).Where(p => p.Type == EntityPlayer.Kind.Human && p.Game!.Current == Game.State.Started).OrderBy(p => p.Index).Select(p => new Client.Data.PlayerId(p.Index, p.GameId, p.Name));
		}
		[HttpPost("Money")]
		public int Money([FromBody] Session ses) => ctx.Players.SingleOrDefault(p => p.GameId == ses.G && p.Index == ses.U)?.Money ?? 0;
		[HttpPost("Infos")]
		public IEnumerable<PlayerInfo> Infos([FromBody] Session ses)
		{
			if (!login.IsValid(ses.U, ses.G, ses.I))
			{
				return Enumerable.Empty<PlayerInfo>();
			}
			var p_p = pap[ses.G];
			return p_p.Players.Select((p, i) => new PlayerInfo(i, p is Human, p.Name, p.Color.ToString(), p.Alive, p.Money, p_p.IncomeOf(p), p.Actions.OfType<Loan>().Sum(l => l.Debt)));
		}
		[HttpPost("Correct")]
		public bool Correct([FromBody] Session user) => login.IsValid(user.U, user.G, user.I);
		[HttpPost("Login")]
		public async Task<StringValue> Login([FromBody] Login trial) => new StringValue(pap.Player(trial.G, trial.I) is Human h && h.Password.IsCorrect(trial.P) ? await login.AddAsync(trial.I, trial.G) : null);
		[HttpPost("Logout")]
		public Task Logout([FromBody] Session user) => login.RemoveAsync(user.U, user.G, user.I);
	}
}
