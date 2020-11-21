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
		private readonly IGameService game;
		private readonly INewGame newGame;
		private readonly IPlayersProvinces pap;
		private readonly ISessionService login;
		private readonly IEndOfTurn end;
		private readonly IActive active;
		private readonly IContextService ctx;
		public GameController(IGameService game, INewGame newGame, IPlayersProvinces pap, ISessionService login, IEndOfTurn end, IActive active, IContextService ctx)
		{
			this.game = game;
			this.newGame = newGame;
			this.pap = pap;
			this.login = login;
			this.end = end;
			this.active = active;
			this.ctx = ctx;
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
			if (!string.IsNullOrWhiteSpace(player.N) && !string.IsNullOrWhiteSpace(player.N) && p_p.Province(player.S) is Land land && land.IsStart && !land.Occupied)
			{
				await newGame.RegisterAsync(player.G, player.N, new Password(player.P), player.S);
				return true;
			}
			return false;
		}
		[HttpGet("RegistrableGame")]
		public async Task<int> RegistrableGame()
		{
			await newGame.StartAllAsync();
			if (game.RegistrableGame is int registrable)
			{
				await ctx.SaveAsync();
				return registrable;
			}
			return await newGame.CreateAsync();
		}
		[HttpPost("NextColor")]
		public Color NextColor([FromBody] int gameId) => newGame.NextColor(gameId);
		[HttpPost("NextTurn")]
		public async Task<bool> NextTurnAsync([FromBody] Client.Data.Session loggedIn)
		{
			return active[loggedIn.G] == loggedIn.U && login.IsValid(loggedIn.U, loggedIn.G, loggedIn.I) && await end.NextTurnAsync(loggedIn.G);
		}
		[HttpGet("Finished")]
		public int[] Finished() => game.FinishedGames;
	}
}
