using System.Collections.Generic;
using System.Linq;
using ImperitWASM.Client.Data;
using ImperitWASM.Shared.Config;
using Microsoft.AspNetCore.Mvc;

namespace ImperitWASM.Server.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class SoldierTypesController : ControllerBase
	{
		readonly Settings settings;
		public SoldierTypesController(Settings settings) => this.settings = settings;
		[HttpGet] public IEnumerable<SoldiersItem> Types() => settings.SoldierTypes.Select(type => new SoldiersItem(type.Description, type.Price));
	}
}
