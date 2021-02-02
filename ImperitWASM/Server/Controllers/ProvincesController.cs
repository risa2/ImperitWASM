using System.Collections.Generic;
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
	public class ProvincesController : ControllerBase
	{
		readonly IProvinceLoader province_load;
		readonly IPlayerLoader player_load;
		readonly Settings settings;
		public ProvincesController(IProvinceLoader province_load, Settings settings, IPlayerLoader player_load)
		{
			this.province_load = province_load;
			this.settings = settings;
			this.player_load = player_load;
		}
		[HttpPost("Shapes")] public IEnumerable<ProvinceAppearance> Shapes([FromBody] int gameId)
		{
			return province_load[gameId].Select(p => new ProvinceAppearance(p.Border, p.Center, p.Fill, p.Stroke, p.StrokeWidth, p.Inhabitable, p.Text));
		}
		[HttpPost("Current")] public IEnumerable<ProvinceUpdate> Current([FromBody] int gameId)
		{
			return province_load[gameId].Select(p => new ProvinceUpdate(p.Text, p.Fill));
		}
		[HttpPost("Preview")] public IEnumerable<ProvinceUpdate> Preview([FromBody] PlayerId id)
		{
			return player_load[id.G, id.P].Act(province_load[id.G], settings).Item2.Select(p => new ProvinceUpdate(p.Text, p.Fill));
		}
	}
}
