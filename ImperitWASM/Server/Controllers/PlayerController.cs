using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImperitWASM.Server.Load;
using ImperitWASM.Server.Services;
using ImperitWASM.Shared.Entities;
using ImperitWASM.Shared.Cfg;
using Microsoft.AspNetCore.Mvc;

namespace ImperitWASM.Server.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class PlayerController : ControllerBase
	{
		readonly IContextService ctx;
		readonly ISessionService login;
		readonly IPlayersProvinces pap;
		public PlayerController(IPlayersProvinces pap, ISessionService login, IContextService ctx)
		{
			this.pap = pap;
			this.login = login;
			this.ctx = ctx;
		}
		[HttpGet("Login")]
		public IReadOnlyDictionary<int, IEnumerable<Client.Server.PlayerId>> Players()
		{
			return ctx.Players.Where(p => p is Human).GroupBy(p => p.GameId).ToDictionary(g => g.Key, g => g.Select(p => new Client.Server.PlayerId(p.Index, p.Name)));
		}
		[HttpPost("Display")]
		public IEnumerable<Client.Server.DisplayablePlayer> Display([FromBody] int gameId)
		{
			return ctx.Players.Where(p => p is Human && p.GameId == gameId).Select(p => new Client.Server.DisplayablePlayer(p.Name, Color.Parse(p.Color)));
		}
		[HttpPost("Money")]
		public int Money([FromBody] Client.Server.PlayerKey key)
		{
			return ctx.Players.Single(p => p.Index == key.I && p.GameId == key.G).Money;
		}
		[HttpPost("Infos")]
		public IEnumerable<Client.Server.PlayerFullInfo> Infos([FromBody] int gameId)
		{
			var p_p = pap[gameId];
			return p_p.Players.Select((p, i) => new Client.Server.PlayerFullInfo(i, p is Human, p.Name, p.Color.ToString(), p.Alive, p.Money, p_p.IncomeOf(p), p.Actions.OfType<Loan>().Sum(l => l.Debt)));
		}
		[HttpPost("Correct")]
		public bool Correct([FromBody] Client.Server.Session user) => login.IsValid(user.U, user.G, user.I);
		[HttpPost("Login")]
		public async Task<Client.Server.StringValue> Login([FromBody] Client.Server.Login trial)
		{
			return new Client.Server.StringValue(pap.Player(trial.G, trial.I) is Human h && h.Password.IsCorrect(trial.P) ? await login.Add(trial.I, trial.G) : null);
		}
		[HttpPost("Logout")]
		public Task Logout([FromBody] Client.Server.Session user)
		{
			return login.Remove(user.U, user.G, user.I);
		}
	}
}
