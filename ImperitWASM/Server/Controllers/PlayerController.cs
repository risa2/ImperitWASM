using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImperitWASM.Server.Load;
using ImperitWASM.Server.Services;
using ImperitWASM.Shared.Motion;
using ImperitWASM.Shared;
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
		readonly INewGame newGame;
		public PlayerController(IPlayersProvinces pap, ISessionService login, IContextService ctx, INewGame newGame)
		{
			this.pap = pap;
			this.login = login;
			this.ctx = ctx;
			this.newGame = newGame;
		}
		[HttpPost("Colored")]
		public IEnumerable<Client.Data.ColoredHuman> Colored([FromBody] int gameId)
		{
			return ctx.Players.Where(p => p.Type == EntityPlayer.Kind.Human && p.GameId == gameId).Select(p => new Client.Data.ColoredHuman(p.Name, Color.Parse(p.Color)));
		}
		[HttpGet("Login")]
		public async Task<IEnumerable<Client.Data.PlayerId>> Login()
		{
			await newGame.StartAllAsync();
			return ctx.Players.Include(p => p.Game).Where(p => p.Type == EntityPlayer.Kind.Human && p.Game.Started).Select(p => new Client.Data.PlayerId(p.Index, p.GameId, p.Name));
		}
		[HttpPost("Money")]
		public int Money([FromBody] Client.Data.Session ses)
		{
			return ctx.Players.Single(p => p.GameId == ses.G && p.Index == ses.U).Money;
		}
		[HttpPost("Infos")]
		public IEnumerable<Client.Data.PlayerInfo> Infos([FromBody] int gameId)
		{
			var p_p = pap[gameId];
			return p_p.Players.Select((p, i) => new Client.Data.PlayerInfo(i, p is Human, p.Name, p.Color.ToString(), p.Alive, p.Money, p_p.IncomeOf(p), p.Actions.OfType<Loan>().Sum(l => l.Debt)));
		}
		[HttpPost("Correct")]
		public bool Correct([FromBody] Client.Data.Session user) => login.IsValid(user.U, user.G, user.I);
		[HttpPost("Login")]
		public async Task<Client.Data.StringValue> Login([FromBody] Client.Data.Login trial)
		{
			return new Client.Data.StringValue(pap.Player(trial.G, trial.I) is Human h && h.Password.IsCorrect(trial.P) ? await login.Add(trial.I, trial.G) : null);
		}
		[HttpPost("Logout")]
		public Task Logout([FromBody] Client.Data.Session user) => login.Remove(user.U, user.G, user.I);
	}
}
