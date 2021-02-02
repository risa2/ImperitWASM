using System.Collections.Generic;
using System.Linq;
using ImperitWASM.Client.Data;
using ImperitWASM.Server.Services;
using ImperitWASM.Shared.Config;
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
		[HttpGet] public IEnumerable<ProvinceDisplay> GetProvinceList()
		{
			return settings.Regions.Select(region => new ProvinceDisplay(region.Border, region.Center, region.Fill(settings), region.Stroke(settings), region.StrokeWidth(settings), region.Text(region.Soldiers)));
		}
		[HttpPost("Free")] public IEnumerable<bool> Free([FromBody] int gameId) => province_load[gameId].Select(p => p.Inhabitable);
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
