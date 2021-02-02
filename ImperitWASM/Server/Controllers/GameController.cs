﻿using System;
using System.Linq;
using ImperitWASM.Client.Data;
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
		readonly IGameLoader game_load;
		readonly IGameCreator game_creator;
		readonly IProvinceLoader province_load;
		readonly ISessionLoader session;
		readonly IEndOfTurn eot;
		readonly IPlayerLoader player_load;
		readonly Settings settings;
		public GameController(IGameLoader game_load, IGameCreator game_creator, IProvinceLoader province_load, ISessionLoader session, IEndOfTurn eot, IPlayerLoader player_load, Settings settings)
		{
			this.game_load = game_load;
			this.game_creator = game_creator;
			this.province_load = province_load;
			this.session = session;
			this.eot = eot;
			this.player_load = player_load;
			this.settings = settings;
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
		[HttpPost("Winner")]
		public Winner? Winner([FromBody] int gameId)
		{
			return province_load[gameId].Winner is ({ Human: true } H, int final) && final >= settings.FinalLandsCount ? new Winner(H.Name, H.Color) : null;
		}
		RegistrationErrors DoRegistration(RegisteredPlayer player)
		{
			game_creator.Register(player.G, player.N.Trim(), new Password(player.P.Trim()), player.S);
			return RegistrationErrors.Ok;
		}
		[HttpPost("Register")]
		public RegistrationErrors Register([FromBody] RegisteredPlayer player) => player.N?.Trim() switch
		{
			null or { Length: 0 } => RegistrationErrors.NoName,
			string name when !player_load.IsNameFree(name) => RegistrationErrors.UsedName,
			_ when string.IsNullOrWhiteSpace(player.P) => RegistrationErrors.NoPassword,
			_ when province_load[player.G, player.S].Inhabitable => DoRegistration(player),
			_ => RegistrationErrors.InvalidStart
		};
		[HttpGet("RegistrableGame")]
		public int RegistrableGame()
		{
			game_creator.StartAll();
			return game_load.RegistrableGame ?? game_creator.Create();
		}
		[HttpPost("NextColor")]
		public Color NextColor([FromBody] int gameId) => game_creator.NextColor(gameId);
		[HttpPost("NextTurn")]
		public bool NextTurn([FromBody] Session ses)
		{
			return session.IsValid(ses.P, ses.G, ses.Key) && eot.NextTurn(ses.G, ses.P);
		}
	}
}
