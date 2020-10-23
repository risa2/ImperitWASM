using ImperitWASM.Server.Services;
using ImperitWASM.Shared.Motion;
using ImperitWASM.Shared.State;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImperitWASM.Server.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class PlayerController : ControllerBase
	{
		readonly ILoginService login;
		readonly IPlayersProvinces pap;
		readonly IFormerPlayers former;
		public PlayerController(IPlayersProvinces pap, IFormerPlayers former, ILoginService login)
		{
			this.pap = pap;
			this.former = former;
			this.login = login;
		}
		[HttpGet("Display")]
		public IEnumerable<Shared.Data.DisplayablePlayer> Display()
		{
			return pap.Players.Select(p => new Shared.Data.DisplayablePlayer(p.Name, p.Color.ToString(), p is Savage));
		}
		[HttpGet("Players")]
		public IEnumerable<Shared.Data.PlayerId> Players()
		{
			return pap.Players.Where(p => !(p is Robot) && !(p is Savage)).Select(p => new Shared.Data.PlayerId(p.Id, p.Name));
		}
		[HttpGet("Former")]
		public IEnumerable<Shared.Data.DisplayablePlayer> Former()
		{
			return former.Select(p => new Shared.Data.DisplayablePlayer(p.Name, p.Color.ToString(), p is Savage));
		}
		[HttpPost("Info")]
		public Shared.Data.PlayerInfo Info([FromBody] int player)
		{
			return new Shared.Data.PlayerInfo(player == pap.Active.Id, player >= 0 && player < pap.PlayersCount ? pap.Player(player).Color : new Color());
		}
		[HttpPost("Money")]
		public int Money([FromBody] int player)
		{
			return pap.Player(player).Money;
		}
		[HttpGet("Infos")]
		public IEnumerable<Shared.Data.PlayerFullInfo> Infos()
		{
			return pap.Players.Select(p => new Shared.Data.PlayerFullInfo(p.Id, !(p is Savage), p.Name, p.Color.ToString(), p.Alive, p.Money, pap.PaP.IncomeOf(p), p.Actions.OfType<Loan>().Sum(l => l.Debt)));
		}
		[HttpPost("Correct")]
		public bool Correct([FromBody] Shared.Data.User user) => login.Get(user.U) == user.I;
		[HttpPost("Login")]
		public Shared.Data.StringValue Login([FromBody] Shared.Data.Login trial)
		{
			return new Shared.Data.StringValue(pap.Player(trial.Id).Password.IsCorrect(trial.Password) ? login.Get(trial.Id) : null);
		}
		[HttpPost("Logout")]
		public Task Logout([FromBody] Shared.Data.User user)
		{
			return login.Get(user.U) == user.I ? login.Remove(user.U) : new Task(() => { });
		}
	}
}
