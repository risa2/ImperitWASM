using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ImperitWASM.Server.Services;
using ImperitWASM.Shared.State;
using Microsoft.AspNetCore.Mvc;

namespace ImperitWASM.Server.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class ProvincesController : ControllerBase
	{
		readonly IPlayersProvinces pap;
		readonly IActive active;
		public ProvincesController(IPlayersProvinces pap, IActive active)
		{
			this.pap = pap;
			this.active = active;
		}
		[HttpPost("Shapes")]
		public IEnumerable<Client.Data.DisplayableShape> Shapes([FromBody] int gameId)
		{
			return pap[gameId].Provinces.Select(p => new Client.Data.DisplayableShape(p.Border, p.Center, p.Fill, p.Stroke, p.StrokeWidth, p is Land land && land.IsInhabitable, p.Text));
		}
		[HttpPost("Current")]
		public IEnumerable<Client.Data.ProvinceVariables> Current([FromBody] int gameId)
		{
			return pap[gameId].Provinces.Select(p => new Client.Data.ProvinceVariables(p.Text, p.Fill));
		}
		[HttpPost("Preview")]
		public IEnumerable<Client.Data.ProvinceVariables> Preview([FromBody] int gameId)
		{
			var preview = pap[gameId].Act(active[gameId], false).Provinces;
			return preview.Select(p => new Client.Data.ProvinceVariables(p.Text, p.Fill));
		}
		[HttpPost("Instabilities")]
		public IEnumerable<Client.Data.ProvinceInstability> Instabilities([FromBody] int gameId)
		{
			return pap[gameId].Provinces.OfType<Land>().Where(l => l.Occupied && !l.Instability.IsZero).Select(l => new Client.Data.ProvinceInstability(l.Name, l.Fill, l.Instability));
		}
	}
}
