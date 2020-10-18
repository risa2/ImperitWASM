using System.Collections.Generic;
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
		public ProvincesController(IPlayersProvinces pp) => pap = pp;
		[HttpGet("Shapes")]
		public IEnumerable<Shared.Data.DisplayableShape> Shapes()
		{
			return pap.Provinces.Select(p => new Shared.Data.DisplayableShape(p.ToArray(), p.Center, p.Fill, p.Stroke, p.StrokeWidth, p is Land land && !land.Occupied && land.IsStart, p.Text));
		}
		[HttpGet("Current")]
		public IEnumerable<Shared.Data.ProvinceVariables> Current()
		{
			return pap.Provinces.Select(p => new Shared.Data.ProvinceVariables(p.Text.LastOrDefault(), p.Fill));
		}
		[HttpGet("Preview")]
		public IEnumerable<Shared.Data.ProvinceVariables> Preview()
		{
			var preview = pap.PaP.Act(false).Provinces;
			return preview.Select(p => new Shared.Data.ProvinceVariables(p.Text.LastOrDefault(), p.Fill));
		}
		[HttpGet("Instabilities")]
		public IEnumerable<Shared.Data.ProvinceInstability> Instabilities()
		{
			return pap.Provinces.OfType<Land>().Where(l => l.Occupied && l.Instability >= 0.0).Select(l => new Shared.Data.ProvinceInstability(l.Name, l.Fill, l.Instability));
		}
	}
}
