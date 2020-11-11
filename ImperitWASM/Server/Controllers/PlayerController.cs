using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImperitWASM.Server.Services;
using ImperitWASM.Shared.Motion;
using ImperitWASM.Shared.State;
using Microsoft.AspNetCore.Mvc;

namespace ImperitWASM.Server.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class PlayerController : ControllerBase
	{
		readonly ISessionService login;
		readonly IPlayersProvinces pap;
		readonly IContextService ctx;
		public PlayerController(IPlayersProvinces pap, ISessionService login, IContextService ctx)
		{
			this.pap = pap;
			this.login = login;
			this.ctx = ctx;
		}
		[HttpPost("Display")]
		public IEnumerable<Client.Data.DisplayablePlayer> Display([FromBody] int gameId)
		{
			return pap.Players(gameId).OfType<Human>().Select(p => new Client.Data.DisplayablePlayer(p.Name, p.Color));
		}
		[HttpGet("Players")]
		public IEnumerable<Client.Data.PlayerId> Players()
		{
			return ctx.Players.Where(p => p.Type == "H").Select(p => new Client.Data.PlayerId(p.Index, p.EntityGameId, p.Name));
		}
		[HttpPost("Money")]
		public int Money([FromBody] Client.Data.PlayerKey k)
		{
			return ctx.Players.Single(p => p.EntityGameId == k.G && p.Index == k.I).Money;
		}
		[HttpPost("Infos")]
		public IEnumerable<Client.Data.PlayerFullInfo> Infos([FromBody] int gameId)
		{
			var p_p = pap[gameId];
			return p_p.Players.Select((p, i) => new Client.Data.PlayerFullInfo(i, p is Human, p.Name, p.Color.ToString(), p.Alive, p.Money, p_p.IncomeOf(p), p.Actions.OfType<Loan>().Sum(l => l.Debt)));
		}
		[HttpPost("Correct")]
		public bool Correct([FromBody] Client.Data.Session user) => login.IsValid(user.U, user.G, user.I);
		[HttpPost("Login")]
		public async Task<Client.Data.StringValue> Login([FromBody] Client.Data.Login trial)
		{
			return new Client.Data.StringValue(pap.Player(trial.G, trial.I) is Human h && h.Password.IsCorrect(trial.P) ? await login.Add(trial.I, trial.G) : null);
		}
		[HttpPost("Logout")]
		public Task Logout([FromBody] Client.Data.Session user)
		{
			return login.Remove(user.U, user.G, user.I);
		}
	}
}
