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
		readonly IPowerLoader powers;
		public PowersController(IPowerLoader powers) => this.powers = powers;
		[HttpPost("List")] public IEnumerable<Powers> List([FromBody] int gameId) => powers.Get(gameId);
	}
}
