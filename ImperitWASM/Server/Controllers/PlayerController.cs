using ImperitWASM.Server.Services;
using ImperitWASM.Shared.State;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace ImperitWASM.Server.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class PlayerController : ControllerBase
	{
		readonly ILoginService login;
		readonly IPlayersLoader players;
		readonly IFormerPlayersLoader former;
		readonly IActivePlayer active;
		readonly IProvincesLoader provinces;
		public PlayerController(IPlayersLoader players, IFormerPlayersLoader former, ILoginService login, IActivePlayer active, IProvincesLoader provinces)
		{
			this.players = players;
			this.former = former;
			this.login = login;
			this.active = active;
			this.provinces = provinces;
		}
		[HttpGet("Display")]
		public IEnumerable<Shared.Data.DisplayablePlayer> Display()
		{
			return players.Select(p => new Shared.Data.DisplayablePlayer(p.Name, p.Color.ToString(), p is Savage));
		}
		[HttpGet("Players")]
		public IEnumerable<Shared.Data.PlayerId> Players()
		{
			return players.Where(p => !(p is Robot) && !(p is Savage)).Select(p => new Shared.Data.PlayerId(p.Id, p.Name));
		}
		[HttpGet("Former")]
		public IEnumerable<Shared.Data.DisplayablePlayer> Former()
		{
			return former.Select(p => new Shared.Data.DisplayablePlayer(p.Name, p.Color.ToString(), p is Savage));
		}
		[HttpPost("Info")]
		public Shared.Data.PlayerInfo Info([FromBody] int player)
		{
			return new Shared.Data.PlayerInfo(player == active.Id, players[player].Color);
		}
		[HttpPost("Money")]
		public int Money([FromBody] int player)
		{
			return players[player].Money;
		}
		[HttpGet("Infos")]
		public IEnumerable<Shared.Data.PlayerFullInfo> Infos()
		{
			return players.Select(p => new Shared.Data.PlayerFullInfo(p.Id, !(p is Savage), p.Name, p.Color.ToString(), p.Alive, p.Money, provinces.ControlledBy(p.Id).Sum(p => p.Earnings)));
		}
		[HttpPost("FromId")]
		public int? FromId([FromBody] string loginId) => login.Get(loginId);
		[HttpPost("Login")]
		public Shared.Data.StringValue Login([FromBody] Shared.Data.Login trial)
		{
			return new Shared.Data.StringValue(players[trial.Id].Password.IsCorrect(trial.Password) ? login.Add(trial.Id) : null);
		}
		[HttpPost("Logout")]
		public void Logout([FromBody] string id) => login.Remove(id);
	}
}
