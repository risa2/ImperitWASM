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
		readonly IContextService ctx;
		public PowersController(IContextService ctx) => this.ctx = ctx;
		[HttpPost("List")] public IEnumerable<Powers> List([FromBody] int gameId) => ctx.GetPlayersPowers(gameId);
	}
}
