using System;
using System.Collections.Immutable;
using System.Linq;
using ImperitWASM.Client.Data;
using ImperitWASM.Server.Services;
using ImperitWASM.Shared;
using ImperitWASM.Shared.Config;
using ImperitWASM.Shared.Data;
using Microsoft.AspNetCore.Mvc;

namespace ImperitWASM.Server.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class GameController : ControllerBase
	{
		readonly IGameLoader game_load;
		readonly IGameCreator game_creator;
		readonly IProvinceLoader province_load;
		readonly IPlayerLoader player_load;
		readonly Settings settings;
		readonly IPowerLoader power_load;
		public GameController(IGameLoader game_load, IGameCreator game_creator, IProvinceLoader province_load, IPlayerLoader player_load, Settings settings, IPowerLoader power_load)
		{
			this.game_load = game_load;
			this.game_creator = game_creator;
			this.province_load = province_load;
			this.player_load = player_load;
			this.settings = settings;
			this.power_load = power_load;
		}
		[HttpPost("Active")] public int Active([FromBody] int gameId) => player_load[gameId].First(p => p.Active).Order;
		[HttpPost("Info")] public GameInfo Info([FromBody] PlayerId id)
		{
			return game_load[id.G] is Game g ? new GameInfo(g.Current, player_load[id.G, id.P].Active) : new GameInfo();
		}
		[HttpPost("StartInfo")] public StartInfo StartInfo([FromBody] int gameId)
		{
			game_creator.StartAll();
			return game_load[gameId] is Game g ? new(g.Current, g.StartTime) : new(Game.State.Invalid, DateTime.MinValue);
		}
		[HttpPost("StartTime")] public DateTimeOffset? StartTime([FromBody] int gameId) => game_load[gameId]?.StartTime;
		[HttpPost("Winner")] public Winner? Winner([FromBody] int gameId)
		{
			return province_load[gameId].Winner is ({ Human: true } H, int final) && final >= settings.FinalLandsCount ? new Winner(H.Name, H.Color) : null;
		}
		RegistrationErrors DoRegistration(RegisteredPlayer player)
		{
			game_creator.Register(player.G, player.N.Trim(), new Password(player.P.Trim()), player.S);
			return RegistrationErrors.Ok;
		}
		[HttpPost("Register")] public RegistrationErrors Register([FromBody] RegisteredPlayer player) => player.N?.Trim() switch
		{
			null or { Length: 0 } => RegistrationErrors.NoName,
			string name when !player_load.IsNameFree(name) => RegistrationErrors.UsedName,
			_ when string.IsNullOrWhiteSpace(player.P) => RegistrationErrors.NoPassword,
			_ when province_load[player.G, player.S].Inhabitable => DoRegistration(player),
			_ => RegistrationErrors.InvalidStart
		};
		[HttpGet("RegistrableGame")] public int RegistrableGame()
		{
			game_creator.StartAll();
			return game_load.RegistrableGame ?? game_creator.Create();
		}
		[HttpPost("NextColor")] public Color NextColor([FromBody] int gameId) => game_creator.NextColor(gameId);

		[HttpPost("History")] public HistoryRecord? History([FromBody] string name)
		{
			if (player_load[name] is Player player)
			{
				var (player_count, provinces) = (player_load.Count(player.GameId), province_load[player.GameId]);
				var who = provinces.Winner.Item1 ?? new PlayerIdentity("", 0, 0, false);
				return new HistoryRecord(power_load.Get(player.GameId), player_count.Range(PlayerIdentity.ColorOf).ToImmutableArray(), who.Name, who.Color);
			}
			return null;
		}
	}
}
