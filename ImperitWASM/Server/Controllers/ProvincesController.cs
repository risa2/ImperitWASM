using System;
using System.Collections.Generic;
using System.Linq;
using ImperitWASM.Server.Services;
using ImperitWASM.Shared.Motion;
using ImperitWASM.Shared.Motion.Actions;
using ImperitWASM.Shared.State;
using Microsoft.AspNetCore.Mvc;

namespace ImperitWASM.Server.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class ProvincesController : ControllerBase
	{
		readonly IActionLoader actions;
		readonly IActivePlayer active;
		readonly IProvincesLoader provinces;
		readonly IPlayersLoader players;
		public ProvincesController(IActionLoader actions, IActivePlayer active, IProvincesLoader provinces, IPlayersLoader players)
		{
			this.actions = actions;
			this.active = active;
			this.provinces = provinces;
			this.players = players;
		}
		[HttpGet("Shapes")]
		public IEnumerable<Shared.Data.DisplayableShape> Shapes()
		{
			return provinces.Select(p => new Shared.Data.DisplayableShape(p.ToArray(), p.Center, p.Fill, p.Stroke, p.StrokeWidth, p is Land land && !land.Occupied && land.IsStart));
		}
		[HttpGet("Current")]
		public IEnumerable<Shared.Data.ProvinceVariables> Current()
		{
			return provinces.Select(p => new Shared.Data.ProvinceVariables(p.Text, p.Fill));
		}
		[HttpGet("Preview")]
		public IEnumerable<Shared.Data.ProvinceVariables> Preview()
		{
			var preview = new ActionQueue(actions.OfType<Movement>()).EndOfTurn(players, provinces, active.Id).Item3;
			return preview.Select(p => new Shared.Data.ProvinceVariables(p.Text, p.Fill));
		}
		[HttpGet("Instabilities")]
		public IEnumerable<Shared.Data.ProvinceInstability> Instabilities()
		{
			return provinces.OfType<Land>().Where(l => l.Occupied && l.Instability >= 0.0).Select(l => new Shared.Data.ProvinceInstability(l.Name, l.Fill, l.Instability));
		}
	}
}
