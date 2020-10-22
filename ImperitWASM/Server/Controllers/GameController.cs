using ImperitWASM.Server.Services;
using ImperitWASM.Shared.State;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

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
			if (!game.IsActive && TimeToStart() < 0)
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
		static int TimeSpanToSec(TimeSpan time) => time > TimeSpan.FromDays(1) ? int.MaxValue : time < TimeSpan.FromDays(-1) ? int.MinValue : (int)time.TotalSeconds;
		[HttpGet("TimeToStart")]
		public int TimeToStart() => TimeSpanToSec(TimeSpan.FromMinutes(0.4) - game.TimeSinceFirstRegistration);
		[HttpPost("NextTurn")]
		public async Task<bool> NextTurn([FromBody] Shared.Data.User loggedIn)
		{
			return pap.Active.Id == loggedIn.U && login.Get(loggedIn.I) == loggedIn.U && await end.NextTurn();
		}
	}
}
