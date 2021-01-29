using System.Collections.Generic;
using System.Linq;
using ImperitWASM.Client.Data;
using ImperitWASM.Server.Services;
using ImperitWASM.Shared.Data;
using Microsoft.AspNetCore.Mvc;

namespace ImperitWASM.Server.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class ProvincesController : ControllerBase
	{
		readonly IProvinceLoader pap;
		readonly IActive active;
		public ProvincesController(IProvinceLoader pap, IActive active)
		{
			this.pap = pap;
			this.active = active;
		}
		[HttpPost("Shapes")]
		public IEnumerable<ProvinceAppearance> Shapes([FromBody] int gameId)
		{
			return pap[gameId].Provinces.Select(p => new ProvinceAppearance(p.Border, p.Center, p.Fill, p.Stroke, p.StrokeWidth, p.Inhabitable, p.Text));
		}
		[HttpPost("Current")]
		public IEnumerable<ProvinceUpdate> Current([FromBody] int gameId)
		{
			return pap[gameId].Provinces.Select(p => new ProvinceUpdate(p.Text, p.Fill));
		}
		[HttpPost("Preview")]
		public IEnumerable<ProvinceUpdate> Preview([FromBody] int gameId)
		{
			return pap[gameId].Act(active[gameId], false).Provinces.Select(p => new ProvinceUpdate(p.Text, p.Fill));
		}
	}
}
