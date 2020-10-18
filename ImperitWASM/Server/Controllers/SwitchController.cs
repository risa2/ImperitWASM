using ImperitWASM.Server.Services;
using ImperitWASM.Shared.State;
using Microsoft.AspNetCore.Mvc;
using Switch = ImperitWASM.Shared.Data.Switch;
using Mode = ImperitWASM.Shared.Data.Switch.Mode;

namespace ImperitWASM.Server.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class SwitchController : ControllerBase
	{
		readonly IPlayersProvinces pap;
		public SwitchController(IPlayersProvinces pap) => this.pap = pap;
		Switch ClickedResult(Shared.Data.Click c) => c.F switch
		{
			int start => new Switch(null, start == c.C ? Mode.Recruit : Mode.Move, start, c.C),
			null when pap.Province(c.C).IsAllyOf(pap.Player(c.U)) => new Switch(c.C, Mode.Map, null, null),
			null when pap.Province(c.C) is Land L1 && !L1.Occupied => new Switch(null, Mode.Purchase, c.C, c.C),
			_ => new Switch(null, Mode.Map, null, null)
		};
		[HttpPost("Clicked")]
		public Switch Clicked([FromBody] Shared.Data.Click c)
		{
			return c.U == pap.Active.Id ? ClickedResult(c) : new Switch(null, Mode.Map, null, null);
		}
	}
}
