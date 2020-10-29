using System;
using System.Threading.Tasks;
using ImperitWASM.Server.Services;
using ImperitWASM.Shared.State;
using Microsoft.AspNetCore.Mvc;

namespace ImperitWASM.Server.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class GameController : ControllerBase
	{
		readonly IGameLoader game;
		readonly INewGame newGame;
		readonly IPlayersProvinces pap;
		readonly ILoginService login;
		readonly IEndOfTurn end;
		public GameController(IGameLoader game, INewGame newGame, IPlayersProvinces pap, ILoginService login, IEndOfTurn end)
		{
			this.game = game;
			this.newGame = newGame;
			this.pap = pap;
			this.login = login;
			this.end = end;
		}

		[HttpGet("IsActive")]
		public async Task<bool> IsActive()
		{
			if (!game.IsActive && game.TimeToStart <= TimeSpan.Zero)
			{
				await newGame.Start();
				return true;
			}
			return game.IsActive;
		}
		[HttpPost("Register")]
		public async Task<bool> Register([FromBody] Shared.Data.RegisteredPlayer player)
		{
			if (!string.IsNullOrWhiteSpace(player.Name) && !string.IsNullOrWhiteSpace(player.Name) && pap.Province(player.Start) is Land land && land.IsStart && !land.Occupied)
			{
				await newGame.Registration(player.Name, new Password(player.Password), land);
				return true;
			}
			return false;
		}
		[HttpGet("NextColor")]
		public Color NextColor() => newGame.NextColor;
		static int TimeSpanToSec(TimeSpan time) => time > TimeSpan.FromDays(1) ? 86400 : time < TimeSpan.FromDays(-1) ? -86400 : (int)time.TotalSeconds;
		[HttpGet("TimeToStart")]
		public int TimeToStart() => TimeSpanToSec(game.TimeToStart);
		[HttpPost("NextTurn")]
		public async Task<bool> NextTurn([FromBody] Shared.Data.User loggedIn)
		{
			return pap.Active.Id == loggedIn.U && login.Get(loggedIn.U) == loggedIn.I && await end.NextTurn();
		}
	}
}
