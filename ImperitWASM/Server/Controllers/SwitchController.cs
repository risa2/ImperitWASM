using ImperitWASM.Server.Services;
using ImperitWASM.Shared.State;
using Microsoft.AspNetCore.Mvc;
using Switch = ImperitWASM.Shared.Data.Switch;
using Mode = ImperitWASM.Shared.Data.Switch.Mode;
using System.Linq;
using ImperitWASM.Shared.Motion.Commands;

namespace ImperitWASM.Server.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class SwitchController : ControllerBase
	{
		readonly IPlayersProvinces pap;
		readonly ISettingsLoader sl;
		public SwitchController(IPlayersProvinces pap, ISettingsLoader sl)
		{
			this.pap = pap;
			this.sl = sl;
		}
		bool IsPossible(Switch s) => s.F is int from && s.T is int to && s.M switch
		{
			Mode.Recruit => sl.Settings.RecruitableTypes(pap.Province(to)).Any(),
			Mode.Move => pap.Province(from).SoldierTypes.Any(t => t.CanMoveAlone(pap.PaP, from, to)),
			Mode.Purchase => new Buy(pap.Active, pap.Province(to), 0).Allowed(pap.PaP),
			_ => false
		};
		Switch IfPossible(Switch s) => IsPossible(s) ? s : new Switch(s.S, Mode.Map, null, null);
		Switch ClickedResult(int user, int? from, int clicked) => from switch
		{
			int start => new Switch(null, start == clicked ? Mode.Recruit : Mode.Move, start, clicked),
			null when pap.Province(clicked).IsAllyOf(pap.Player(user)) => new Switch(clicked, Mode.Map, null, null),
			null when pap.Province(clicked) is Land L1 && !L1.Occupied => new Switch(null, Mode.Purchase, clicked, clicked),
			_ => new Switch(null, Mode.Map, null, null)
		};
		[HttpPost("Clicked")]
		public Switch Clicked([FromBody] Shared.Data.Click c)
		{
			return IfPossible(c.U == pap.Active.Id ? ClickedResult(c.U, c.F, c.C) : new Switch(null, Mode.Map, null, null));
		}
	}
}
