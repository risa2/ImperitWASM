using System.Collections.Generic;
using ImperitWASM.Server.Services;
using ImperitWASM.Shared.State;
using Microsoft.AspNetCore.Mvc;

namespace ImperitWASM.Server.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class PowersController : ControllerBase
	{
		private readonly IPowers powers;
		public PowersController(IPowers powers) => this.powers = powers;
		[HttpPost("List")]
		public IEnumerable<PlayersPower> List([FromBody] int gameId) => powers.Get(gameId);
		[HttpPost("Count")]
		public int Count([FromBody] int gameId) => powers.Count(gameId);
	}
}
