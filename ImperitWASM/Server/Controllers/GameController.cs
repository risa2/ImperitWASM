using System.Collections.Immutable;
using System.Threading.Tasks;
using ImperitWASM.Client.Data;
using ImperitWASM.Server.Load;
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
		public GameInfo Info([FromBody] int gameId) => game.FindNoTracking(gameId) is Game g ? new GameInfo(g.Started, g.Active) : new GameInfo();
		async Task<RegistrationErrors> DoRegistrationAsync(RegisteredPlayer player)
		{
			await newGame.RegisterAsync(game.Find(player.G), player.N.Trim(), new Password(player.P.Trim()), player.S);
			return RegistrationErrors.Ok;
		}
		[HttpPost("Register")]
		public async Task<RegistrationErrors> RegisterAsync([FromBody] RegisteredPlayer player) => player.N?.Trim() switch
		{
			null or { Length: 0 } => RegistrationErrors.NoName,
			string name when !pap.IsNameFree(name) => RegistrationErrors.UsedName,
			_ when string.IsNullOrWhiteSpace(player.P) => RegistrationErrors.NoPassword,
			_ when pap.Province(player.G, player.S) is Land { IsInhabitable: true } => await DoRegistrationAsync(player),
			_ => RegistrationErrors.InvalidStart
		};
		[HttpGet("RegistrableGame")]
		public async Task<int> RegistrableGameAsync()
		{
			await newGame.StartAllAsync();
			return game.RegistrableGame is int registrable ? registrable : await newGame.CreateAsync();
		}
		[HttpPost("NextColor")]
		public Color NextColor([FromBody] int gameId) => newGame.NextColor(gameId);
		[HttpPost("NextTurn")]
		public async Task<bool> NextTurnAsync([FromBody] Session loggedIn)
		{
			return active[loggedIn.G] == loggedIn.P && login.IsValid(loggedIn.P, loggedIn.G, loggedIn.Key) && await end.NextTurnAsync(loggedIn.G);
		}
		[HttpGet("Finished")]
		public ImmutableArray<int> Finished() => game.FinishedGames;
	}
}
