using ImperitWASM.Server.Services;
using ImperitWASM.Shared.State;
using Microsoft.AspNetCore.Mvc;
using System;
using Switch = ImperitWASM.Shared.Data.Switch;
using Mode = ImperitWASM.Shared.Data.Switch.Mode;

namespace ImperitWASM.Server.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class SwitchController : ControllerBase
	{
		readonly IProvincesLoader provinces;
		readonly ILoginService login;
		readonly IActivePlayer active;
		public SwitchController(IProvincesLoader provinces, ILoginService login, IActivePlayer active)
		{
			this.provinces = provinces;
			this.login = login;
			this.active = active;
		}
		[HttpPost("Clicked")]
		public Switch Clicked([FromBody] Shared.Data.Click c)
		{
			if (login.Get(c.LoginId) != c.LoggedIn || c.LoggedIn != active.Id)
			{
				return new Switch(null, Mode.Map, null, null);
			}
			return c.From switch
			{
				int start => new Switch(null, start == c.Clicked ? Mode.Recruit : Mode.Move, start, c.Clicked),
				null when provinces[c.Clicked].IsAllyOf(c.LoggedIn) => new Switch(c.Clicked, Mode.Map, null, null),
				null when provinces[c.Clicked] is Land L1 && !L1.Occupied => new Switch(null, Mode.Purchase, c.Clicked, c.Clicked),
				_ => new Switch(null, Mode.Map, null, null)
			};
		}
	}
}
