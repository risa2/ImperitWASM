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
		readonly IPowersLoader powers;
		public PowersController(IPowersLoader powers) => this.powers = powers;
		[HttpPost("List")] public IEnumerable<Powers> List([FromBody] int gameId) => powers.Get(gameId);
	}
}
