using ImperitWASM.Server.Services;
using ImperitWASM.Shared.State;
using Microsoft.AspNetCore.Mvc;
using System;

namespace ImperitWASM.Server.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class GameController : ControllerBase
	{
		readonly IGameLoader game;
		readonly INewGame newGame;
		readonly IProvincesLoader provinces;
		readonly ILoginService login;
		readonly IEndOfTurn end;
		readonly IActivePlayer active;
		public GameController(IGameLoader game, INewGame newGame, IProvincesLoader provinces, ILoginService login, IEndOfTurn end, IActivePlayer active)
		{
			this.game = game;
			this.newGame = newGame;
			this.provinces = provinces;
			this.login = login;
			this.end = end;
			this.active = active;
		}

		[HttpGet("IsActive")]
		public bool IsActive()
		{
			if (!game.IsActive && TimeToStart() < 0)
			{
				newGame.Start();
				return true;
			}
			return game.IsActive;
		}
		[HttpPost("Register")]
		public bool Register([FromBody] Shared.Data.RegisteredPlayer player)
		{
			if (!string.IsNullOrWhiteSpace(player.Name) && !string.IsNullOrWhiteSpace(player.Name) && provinces[player.Start] is Land land && land.IsStart && !land.Occupied)
			{
				newGame.Registration(player.Name, new Password(player.Password), land);
				return true;
			}
			return false;
		}
		[HttpGet("NextColor")]
		public Color NextColor() => newGame.NextColor;
		static int TimeSpanToSec(TimeSpan time) => time > TimeSpan.FromDays(1) ? int.MaxValue : time < TimeSpan.FromDays(-1) ? int.MinValue : (int)time.TotalSeconds;
		[HttpGet("TimeToStart")]
		public int TimeToStart() => TimeSpanToSec(TimeSpan.FromMinutes(4) - game.TimeSinceFirstRegistration);
		[HttpPost("NextTurn")]
		public bool NextTurn([FromBody] Shared.Data.User loggedIn)
		{
			if (active.Id == loggedIn.Id && login.Get(loggedIn.LoginId) == loggedIn.Id)
			{
				if (!end.NextTurn())
				{
					newGame.Finish();
					return true;
				}
			}
			return false;
		}
	}
}
