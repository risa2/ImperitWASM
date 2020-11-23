using System.Collections.Immutable;
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
		readonly IGameService game;
		readonly IGameCreator newGame;
		readonly IPlayersProvinces pap;
		readonly ISessionService login;
		readonly IEndOfTurn end;
		readonly IActive active;
		public GameController(IGameService game, IGameCreator newGame, IPlayersProvinces pap, ISessionService login, IEndOfTurn end, IActive active)
		{
			this.game = game;
			this.newGame = newGame;
			this.pap = pap;
			this.login = login;
			this.end = end;
			this.active = active;
		}
		[HttpPost("Info")]
		public Client.Data.BasicInfo Info([FromBody] Client.Data.Session p)
		{
			return new Client.Data.BasicInfo(p.U, pap.Player(p.G, p.U).Color, game.Started(p.G), active[p.G]);
		}
		[HttpPost("Register")]
		public async Task<bool> RegisterAsync([FromBody] Client.Data.RegisteredPlayer player)
		{
			var p_p = pap[player.G];
			if (!string.IsNullOrWhiteSpace(player.N) && !string.IsNullOrWhiteSpace(player.N) && !player.N.StartsWith("AI ") && p_p.Province(player.S) is Land land && land.IsStart && !land.Occupied)
			{
				await newGame.RegisterAsync(game.Find(player.G), player.N, new Password(player.P), player.S);
				return true;
			}
			return false;
		}
		[HttpGet("RegistrableGame")]
		public async Task<int> RegistrableGameAsync()
		{
			await newGame.StartAllAsync();
			return game.RegistrableGame is int registrable ? registrable : await newGame.CreateAsync();
		}
		[HttpPost("NextColor")]
		public Color NextColor([FromBody] int gameId) => newGame.NextColor(gameId);
		[HttpPost("NextTurn")]
		public async Task<bool> NextTurnAsync([FromBody] Client.Data.Session loggedIn)
		{
			return active[loggedIn.G] == loggedIn.U && login.IsValid(loggedIn.U, loggedIn.G, loggedIn.I) && await end.NextTurnAsync(loggedIn.G);
		}
		[HttpGet("Finished")]
		public ImmutableArray<int> Finished() => game.FinishedGames;
	}
}
