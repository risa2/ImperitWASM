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
		readonly ILoginService login;
		public SwitchController(IPlayersProvinces pap, ILoginService login)
		{
			this.pap = pap;
			this.login = login;
		}
		[HttpPost("Clicked")]
		public Switch Clicked([FromBody] Shared.Data.Click c)
		{
			if (login.Get(c.LoginId) != c.LoggedIn || c.LoggedIn != pap.Active.Id)
			{
				return new Switch(null, Mode.Map, null, null);
			}
			return c.From switch
			{
				int start => new Switch(null, start == c.Clicked ? Mode.Recruit : Mode.Move, start, c.Clicked),
				null when pap.Province(c.Clicked).IsAllyOf(pap.Player(c.LoggedIn)) => new Switch(c.Clicked, Mode.Map, null, null),
				null when pap.Province(c.Clicked) is Land L1 && !L1.Occupied => new Switch(null, Mode.Purchase, c.Clicked, c.Clicked),
				_ => new Switch(null, Mode.Map, null, null)
			};
		}
	}
}
