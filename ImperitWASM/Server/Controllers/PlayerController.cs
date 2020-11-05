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
		public PlayerController(IPlayersProvinces pap, ISessionService login)
		{
			this.pap = pap;
			this.login = login;
		}
		[HttpPost("Display")]
		public IEnumerable<Client.Server.DisplayablePlayer> Display([FromBody] int gameId)
		{
			return pap.Players(gameId).OfType<Human>().Select(p => new Client.Server.DisplayablePlayer(p.Name, p.Color));
		}
		[HttpPost("Players")]
		public IEnumerable<Client.Server.PlayerId> Players([FromBody] int gameId)
		{
			return pap.Players(gameId).Select((p, i) => (p is Human, new Client.Server.PlayerId(i, p.Name))).Where(p => p.Item1).Select(p => p.Item2);
		}
		[HttpPost("Money")]
		public int Money([FromBody] Client.Server.PlayerKey p)
		{
			return pap.Player(p.G, p.I).Money;
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
