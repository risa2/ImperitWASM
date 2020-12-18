using System.Collections.Generic;
using ImperitWASM.Server.Services;
using ImperitWASM.Shared.Data;
using Microsoft.AspNetCore.Mvc;

namespace ImperitWASM.Server.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class PowersController : ControllerBase
	{
		readonly IPowers powers;
		public PowersController(IPowers powers) => this.powers = powers;
		[HttpPost("List")]
		public IEnumerable<PlayersPower> List([FromBody] int gameId) => powers.Get(gameId);
	}
}
