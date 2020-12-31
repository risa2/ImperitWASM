using System;
using System.Collections.Immutable;
using System.Threading.Tasks;
using ImperitWASM.Client.Data;
using ImperitWASM.Server.Load;
using ImperitWASM.Server.Services;
using ImperitWASM.Shared.Config;
using ImperitWASM.Shared.Data;
using Microsoft.AspNetCore.Mvc;

namespace ImperitWASM.Server.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class GameController : ControllerBase
	{
		readonly IGameService gs;
		readonly IGameCreator gameCreator;
		readonly IPlayersProvinces pap;
		readonly ISessionService session;
		readonly IEndOfTurn eot;
		readonly IActive active;
		readonly Settings settings;
		public GameController(IGameService gs, IGameCreator gameCreator, IPlayersProvinces pap, ISessionService session, IEndOfTurn eot, IActive active, Settings settings)
		{
			this.gs = gs;
			this.gameCreator = gameCreator;
			this.pap = pap;
			this.session = session;
			this.eot = eot;
			this.active = active;
			this.settings = settings;
		}
		[HttpPost("Active")]
		public int Active([FromBody] int gameId) => gs.FindNoTracking(gameId)?.Active ?? 0;
		[HttpPost("Info")]
		public GameInfo Info([FromBody] int gameId) => gs.FindNoTracking(gameId) is Game g ? new GameInfo(g.GetState(), g.Active) : new GameInfo();
		[HttpPost("StartTime")]
		public DateTime? StartTime([FromBody] int gameId) => gs.FindNoTracking(gameId)?.GetStartTime(settings.CountdownTime);
		async Task<RegistrationErrors> DoRegistrationAsync(RegisteredPlayer player)
		{
			await gameCreator.RegisterAsync(gs.Find(player.G), player.N.Trim(), new Password(player.P.Trim()), player.S);
			return RegistrationErrors.Ok;
		}
		[HttpPost("Register")]
		public async Task<RegistrationErrors> RegisterAsync([FromBody] RegisteredPlayer player) => player.N?.Trim() switch
		{
			null or { Length: 0 } => RegistrationErrors.NoName,
			string name when !pap.IsNameFree(name) => RegistrationErrors.UsedName,
			_ when string.IsNullOrWhiteSpace(player.P) => RegistrationErrors.NoPassword,
			_ when pap.Province(player.G, player.S)?.Inhabitable is true => await DoRegistrationAsync(player),
			_ => RegistrationErrors.InvalidStart
		};
		[HttpGet("RegistrableGame")]
		public async Task<int> RegistrableGameAsync()
		{
			await gameCreator.StartAllAsync();
			return gs.RegistrableGame ?? await gameCreator.CreateAsync();
		}
		[HttpPost("NextColor")]
		public Color NextColor([FromBody] int gameId) => gameCreator.NextColor(gameId);
		[HttpPost("NextTurn")]
		public async Task<bool> NextTurnAsync([FromBody] Session loggedIn)
		{
			return active[loggedIn.G] == loggedIn.P && session.IsValid(loggedIn.P, loggedIn.G, loggedIn.Key) && await eot.NextTurnAsync(loggedIn.G);
		}
		[HttpGet("Finished")]
		public ImmutableArray<int> Finished() => gs.FinishedGames;
	}
}
