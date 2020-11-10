using System.Threading.Tasks;
using ImperitWASM.Server.Services;
using ImperitWASM.Shared.Func;
using ImperitWASM.Shared.Entities;
using Microsoft.AspNetCore.Mvc;
using ImperitWASM.Shared.Cfg;

namespace ImperitWASM.Server.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class GameController : ControllerBase
	{
		readonly IGameService game;
		readonly INewGame newGame;
		readonly IPlayersProvinces pap;
		readonly ISessionService login;
		readonly IEndOfTurn end;
		readonly IActive active;
		public GameController(IGameService game, INewGame newGame, IPlayersProvinces pap, ISessionService login, IEndOfTurn end, IActive active)
		{
			this.game = game;
			this.newGame = newGame;
			this.pap = pap;
			this.login = login;
			this.end = end;
			this.active = active;
		}
		[HttpGet("Login")]
		public async Task<int[]> LoginAsync()
		{
			await game.ShouldStart.EachAsync(g => newGame.StartAsync(g));
			return game.StartedGames;
		}
		[HttpPost("Info")]
		public Client.Server.BasicInfo Info([FromBody] Client.Server.PlayerKey p)
		{
			return new Client.Server.BasicInfo(p.I, pap.Player(p.G, p.I).Color, game.Started(p.G), active[p.G]);
		}
		[HttpPost("Register")]
		public async Task<bool> RegisterAsync([FromBody] Client.Server.RegisteredPlayer player)
		{
			var gameId = game.RegistrableGame;
			var p_p = pap[gameId];
			if (!string.IsNullOrWhiteSpace(player.Name) && !string.IsNullOrWhiteSpace(player.Name) && p_p.Province(player.Start) is Land land && land.IsStart && !land.Occupied)
			{
				await newGame.RegisterAsync(gameId, player.Name, new Password(player.Password), player.Start);
				return true;
			}
			return false;
		}
		[HttpGet("NextColor")]
		public Color NextColor() => newGame.NextColor(game.RegistrableGame);
		[HttpPost("NextTurn")]
		public async Task<bool> NextTurnAsync([FromBody] Client.Server.Session loggedIn)
		{
			return active[loggedIn.G] == loggedIn.U && login.IsValid(loggedIn.U, loggedIn.G, loggedIn.I) && await end.NextTurnAsync(loggedIn.G);
		}
	}
}
