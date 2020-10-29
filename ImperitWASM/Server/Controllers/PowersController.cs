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
		readonly IPowersLoader powers;
		public PowersController(IPowersLoader powers) => this.powers = powers;
		[HttpGet("List")]
		public IEnumerable<PlayersPower> List() => powers;
		[HttpGet("Count")]
		public int Count() => powers.Count;
	}
}
