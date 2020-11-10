using System.Collections.Generic;
using System.Linq;
using ImperitWASM.Server.Services;
using ImperitWASM.Shared.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ImperitWASM.Server.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class ProvincesController : ControllerBase
	{
		readonly IPlayersProvinces pap;
		readonly IActive active;
		readonly IConfig cfg;
		public ProvincesController(IPlayersProvinces pap, IActive active, IConfig cfg)
		{
			this.pap = pap;
			this.active = active;
			this.cfg = cfg;
		}
		[HttpGet("Shapes")]
		public IEnumerable<Client.Server.DisplayableShape> Shapes([FromBody] int gameId)
		{
			return pap[gameId].Provinces.Select(p => new Client.Server.DisplayableShape(p.ToArray(), p.Center, p.Fill(cfg.Settings), p.Stroke(cfg.Settings), p.StrokeWidth(cfg.Settings), p is Land land && !land.Occupied && land.IsStart, p.Text));
		}
		[HttpGet("Current")]
		public IEnumerable<Client.Server.ProvinceVariables> Current([FromBody] int gameId)
		{
			return pap[gameId].Provinces.Select(p => new Client.Server.ProvinceVariables(p.Text, p.Fill(cfg.Settings)));
		}
		[HttpPost("Preview")]
		public IEnumerable<Client.Server.ProvinceVariables> Preview([FromBody] int gameId)
		{
			var preview = pap[gameId].Act(active[gameId], false).Provinces;
			return preview.Select(p => new Client.Server.ProvinceVariables(p.Text, p.Fill(cfg.Settings)));
		}
		[HttpGet("Instabilities")]
		public IEnumerable<Client.Server.ProvinceInstability> Instabilities([FromBody] int gameId)
		{
			return pap[gameId].Provinces.OfType<Land>().Where(l => l.Occupied && l.Instability(cfg.Settings).IsZero).Select(l => new Client.Server.ProvinceInstability(l.Name, l.Fill(cfg.Settings), l.Instability(cfg.Settings)));
		}
	}
}
